using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Text;
using System.Threading;

public class Comm
{
    #region Public constants
    /* Connection statuses enumeration */
    public enum CommStatus
    {
        connectionOff = 0,
        connectionActive
    };

    public enum MsgTransID
    {
        /* Messages IDs */
        MSG_TRANS_SEND_TIME = 0,
        MSG_TRANS_SEND_GPS_FIX,
        MSG_TRANS_SEND_ROUTE_NUM,
        MSG_TRANS_SEND_ROUTE_NAME,
        MSG_TRANS_SEND_CURR_STOP_NAME,
        MSG_TRANS_SEND_CURR_STOP_TEXT,
        MSG_TRANS_SEND_NXT_STOP_NAME,
        MSG_TRANS_SEND_NXT_STOP_TEXT,
        MSG_TRANS_LAST_MSG,
        MSG_TRANS_EOM_SRVC_CMD = 0x80
    };
    #endregion

    #region Delegates and events
    /* Delegate and event for changed connection state */
    public delegate void CommStatusHasChanged();
    public event CommStatusHasChanged CommStatusHasChangedEvent;

    /* Delegate and event for failed connection state */
    public delegate void CommConnFailed(string msg);
    public event CommConnFailed CommConnFailedEvent;

    /* Delegate and event for receive timeout */
    public delegate void ReceiveTimeout();
    public event ReceiveTimeout ReceiveTimeoutEvent;

    /* Delegate and event to send message to parent */
    public delegate void SendMessageToParent(int msgID, Comm.MsgTransID msgTypeID, string msg);
    public event SendMessageToParent SendMessageToParentEvent;
    #endregion

    #region Public variables
    public int baudRate
    {
        get { return serialPort.BaudRate; }
        set { serialPort.BaudRate = value; }
    }
    public UInt16 noReceiveTimeout
    {
        get { return noRecTimeout; }
        set
        {
            if (value == watchTimer.Interval)
            {
                /* Nothing changes */
                return;
            }
            if (value == 0)
            {
                /* Stop watchdog receive timer */
                if (watchTimer.Enabled)
                    watchTimer.Stop();
                return;
            }
            /* Reinit watchdog receive timer */
            bool enabled = watchTimer.Enabled;
            if (enabled) watchTimer.Stop();
            watchTimer.Interval = value;
            if (enabled) watchTimer.Start();
        }
    }
    public CommStatus connectionState
    {
        get
        {
            if (serialPort == null) return CommStatus.connectionOff;
            if (serialPort.IsOpen) return CommStatus.connectionActive;
            return CommStatus.connectionOff;
        }
    }
    public string currentCOM_Port
    {
        get { return currCOM_Port; }
        set
        {
            if (value == null) return;
            if (value == "") return;
            currCOM_Port = value;
        }
    }
    public string[] COM_PortNames
    {
        get { return SerialPort.GetPortNames(); }
    }
    #endregion

    #region Private variables
    private string currCOM_Port = "";
    private UInt16 noRecTimeout = 1000;
    #endregion

    #region Private stricture and class objects
    private SerialPort serialPort;
    private BackgroundWorker backgroundWorkerCommRec;
    private System.Timers.Timer watchTimer;
    private CommProto16b proto;
    #endregion

    public Comm()
    {
        /* Init connection */
        serialPort = new SerialPort();

        serialPort.BaudRate = 115200;
        serialPort.Parity = System.IO.Ports.Parity.None;
        serialPort.DataBits = 8;
        serialPort.StopBits = System.IO.Ports.StopBits.One;
        serialPort.Handshake = System.IO.Ports.Handshake.None;

        /* Init the background worker before the receive thread */
        backgroundWorkerCommRec = new BackgroundWorker();
        backgroundWorkerCommRec.DoWork +=
            new DoWorkEventHandler(backgroundWorkerCommRec_DoWork);
        backgroundWorkerCommRec.RunWorkerCompleted +=
            new RunWorkerCompletedEventHandler(backgroundWorkerCommRec_RunWorkerCompleted);

        /* Set the read/write timeouts */
        serialPort.ReadTimeout = SerialPort.InfiniteTimeout;
        serialPort.WriteTimeout = 1000;

        /* Init watchdog timer for receive */
        watchTimer = new System.Timers.Timer();
        watchTimer.Elapsed += new System.Timers.ElapsedEventHandler(WatchConnect);
        /* Set default receive timeout */
        noReceiveTimeout = 5000;

        /* Init protocol parser */
        proto = new CommProto16b();
        proto.RecBroadcastEvent += new CommProto16b.RecBroadcast(ParseBroadcastMsg);
        proto.RecValidDataEvent += new CommProto16b.RecValidData(ResetCommTimeout);

        /* Show initial connection state */
        if (CommStatusHasChangedEvent != null)
            CommStatusHasChangedEvent();
    }

    ~Comm()
    {
        /* Close serial port */
        DestroyConnection();

        /* Dispose serial port class member */
        serialPort.Dispose();
    }

    #region Serial connection public functions
    public void TryToReconnect(string COM_Port)
    {
        currCOM_Port = COM_Port;
        TryToConnect();

        /* Watch for connection state */
        WatchForConnChangedState();
    }
    public void ReActivateConnection()
    {
        if (serialPort.IsOpen == false)
        {
            TryToConnect();
        }
        else
        {
            DeactivateConnection();
        }

        /* Watch for connection state */
        WatchForConnChangedState();
    }
    public void DeactivateConnection()
    {
        DestroyConnection();

        /* Watch for connection state */
        WatchForConnChangedState();
    }
    #endregion

    #region Data exchange functions
    public void RecData(byte recByte)
    {
        /* Send received byte to protocol parser */
        proto.RecByte(recByte);
    }
    public bool SendData(byte[] data)
    {
        /* Check for connection state */
        if (serialPort.IsOpen == false)
        {
            /* Send connection failed message */
            if (CommConnFailedEvent != null)
                CommConnFailedEvent("No active connection");
            return false;
        }

        /*Connection is active, send data */
        try
        {
            serialPort.Write(data, 0, data.Length);
            return true;
        }
        catch (Exception exp)
        {
            /* Something wrong with serial connection */
            DestroyConnection();

            /* Send connection failed message */
            if (CommConnFailedEvent != null)
                CommConnFailedEvent(exp.Message);

            /* Watch for connection state */
            WatchForConnChangedState();
            return false;
        }
    }
    #endregion

    #region Serial connection private functions
    private void TryToConnect()
    {
        DestroyConnection();

        /* Try to find needed port */
        string[] serialPorts = SerialPort.GetPortNames();
        foreach (string port in serialPorts)
        {
            if (port == currCOM_Port)
            {
                // try to connect
                try
                {
                    serialPort.PortName = currCOM_Port;
                    serialPort.Open();

                    /* Start receive thread */
                    StartRecThread();

                    /* Restart watchdog timer */
                    ResetCommTimeout();
                }
                catch (Exception exp)
                {
                    DestroyConnection();

                    if (currCOM_Port == "")
                    {
                        /* Send connection failed message */
                        if (CommConnFailedEvent != null)
                            CommConnFailedEvent("No active connection");
                    }
                    else
                    {
                        /* Send connection failed message */
                        if (CommConnFailedEvent != null)
                            CommConnFailedEvent(exp.Message);
                    }
                }
                break;
            }
        }
    }

    private void DestroyConnection()
    {
        if (serialPort != null)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.Close();
                }
                catch (Exception exp)
                {
                    /* Send connection failed message */
                    if (CommConnFailedEvent != null)
                        CommConnFailedEvent(exp.Message);
                }
            }
        }
    }

    private CommStatus lastConnectionState = CommStatus.connectionOff;
    private void WatchForConnChangedState()
    {
        CommStatus state = connectionState;
        if (lastConnectionState != state)
        {
            lastConnectionState = state;

            /* Show that state has been changed */
            if (CommStatusHasChangedEvent != null) CommStatusHasChangedEvent();
        }
    }

    private void WatchConnect(Object myObject, EventArgs myEventArgs)
    {
        /* Stop timer */
        if (watchTimer.Enabled)
            watchTimer.Stop();

        /* Renerate receive timeout event */
        if (ReceiveTimeoutEvent != null) ReceiveTimeoutEvent();
    }

    private void ResetCommTimeout()
    {
        if (watchTimer.Enabled)
            watchTimer.Stop();
        watchTimer.Start();
    }
    #endregion

    private void StartRecThread()
    {
        /* Run background worker */
        if (backgroundWorkerCommRec.IsBusy == false)
            backgroundWorkerCommRec.RunWorkerAsync();
    }
    private string expMsg;
    private Byte[] receive = new Byte[0x400];
    private void backgroundWorkerCommRec_DoWork(object sender, DoWorkEventArgs e)
    {
        while (true)
        {
            /* Check for connection state */
            if (serialPort.IsOpen == false)
            {
                return;
            }

            bool exeption = false;
            try
            {
                int recLenght = serialPort.Read(receive, 0, receive.Length);
                for (int i = 0; i < recLenght; i++)
                {
                    /* Send received byte to protocol parser */
                    proto.RecByte(receive[i]);
                }
            }
            catch (Exception exp)
            {
                if (exp.Message == "The operation has timed out.")
                {
                    /* Read timeout has occured */
                }
                else
                {
                    /* Store exeption message */
                    expMsg = exp.Message;
                    exeption = true;
                }
            }

            if (exeption)
            {
                /* Something wrong with serial connection */
                DestroyConnection();
                return;
            }
            //else System.Threading.Thread.Sleep(1);
        }
    }

    private void backgroundWorkerCommRec_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        /* Send connection failed message */
        if (expMsg != null)
        {
            if (CommConnFailedEvent != null)
                CommConnFailedEvent(expMsg);
            expMsg = null;
        }

        /* Watch for connection state */
        WatchForConnChangedState();
    }

    private void ParseBroadcastMsg(ref byte[] data, int length)
    {
        /* Set pointer and length for parsing message */
        int dataPos = 0;

        /* Get and validate message type ID */
        if (length == 0) return;
        MsgTransID msgTypeID = (MsgTransID)(data[dataPos++]);
        length--;

        /* Get message ID */
        if (length < 2) return;
        int msgID = data[dataPos++];
        msgID |= ((data[dataPos++]) << 8);
        length -= 2;

        /* Parce by message type ID */
        switch (msgTypeID)
        {
            case MsgTransID.MSG_TRANS_SEND_TIME:
                if (length == 0) return;
                byte second = data[dataPos++];
                length--;

                if (length == 0) return;
                byte minute = data[dataPos++];
                length--;

                if (length == 0) return;
                byte hour = data[dataPos++];
                length--;

                if (length == 0) return;
                byte day = data[dataPos++];
                length--;

                if (length == 0) return;
                byte month = data[dataPos++];
                length--;

                if (length == 0) return;
                byte year = data[dataPos++];
                length--;

                try
                {
                    DateTime dateTime =
                        new DateTime(year + 2000, month, day, hour, minute, second);

                    if (SendMessageToParentEvent != null)
                        SendMessageToParentEvent(msgID, msgTypeID, dateTime.ToString());
                }
                catch { }
                break;

            case MsgTransID.MSG_TRANS_SEND_GPS_FIX:
                bool isValid = false;
                if (length == 0) return;
                if (data[dataPos++] != 0) isValid = true;
                length--;

                if (length < (2 * sizeof(double))) return;
                double[] coordinate = new double[2];
                for (int i = 0; i < coordinate.Length; i++)
                {
                    coordinate[i] = BitConverter.ToDouble(data, dataPos);
                    dataPos += sizeof(double);
                    length -= sizeof(double);
                }

                if (SendMessageToParentEvent != null)
                {
                    if (isValid == false)
                        SendMessageToParentEvent(msgID, msgTypeID, "Invalid");
                    else
                    {
                        string msg = "";
                        for (int i = 0; i < coordinate.Length; i++)
                        {
                            msg += coordinate[i].ToString("00.000000");
                            if (i < coordinate.Length - 1) msg += ", ";
                        }
                        SendMessageToParentEvent(msgID, msgTypeID, msg);
                    }
                }
                break;

            case MsgTransID.MSG_TRANS_EOM_SRVC_CMD:
                /* Ignore this service command */
                break;

            default:
                /* Get string from byte array */
                Encoding encoding = Encoding.GetEncoding("windows-1251");
                string msgEncode = encoding.GetString(data, dataPos, length);

                SendMessageToParentEvent(msgID, msgTypeID, msgEncode);
                break;
        }
    }
}
