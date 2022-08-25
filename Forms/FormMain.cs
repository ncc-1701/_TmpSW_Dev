using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARS_MsgTesterForm
{
    public partial class FormMain : Form
    {
        #region Classes and structs members
        private Comm comm;
        #endregion

        #region Variables
        private string _COM_Port = "";
        private string COM_Port
        {
            get
            {
                if ((_COM_Port == "") || (_COM_Port == null)) return "COM1";
                else return _COM_Port;
            }
            set
            {
                if ((value == "") || (value == null)) value = "COM1";
                _COM_Port = value;
            }
        }
        #endregion

        public FormMain()
        {
            InitializeComponent();

            /* Create objects */
            comm = new Comm();
            comm.baudRate = 19200;

            /* Init combobox for selecting COM-port */
            string[] COM_Ports = comm.COM_PortNames;
            ComboBox COM_PortsList = this.comboBoxCOM_Ports;
            COM_PortsList.Items.Clear();
            COM_PortsList.Items.AddRange(COM_Ports);

            /* Init classes objects */
            comm.CommStatusHasChangedEvent +=
                new Comm.CommStatusHasChanged(ShowCommStatus);
            comm.SendMessageToParentEvent +=
                new Comm.SendMessageToParent(GetMessageFromComm);
            comm.ReceiveTimeoutEvent +=
                new Comm.ReceiveTimeout(ClearAllMessages);
        }

        #region Control functions
        private void FormMain_Load(object sender, EventArgs e)
        {
            /* Show initial connection state */
            ShowCommStatus();
            this.comboBoxCOM_Ports.Text = this.COM_Port;
        }
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            comm.DeactivateConnection();
        }
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            comm.currentCOM_Port = COM_Port;
            comm.ReActivateConnection();
        }

        private void comboBoxCOM_Ports_Click(object sender, EventArgs e)
        {
            ComboBox COM_PortsList = (ComboBox)(sender);
            string[] COM_Ports = comm.COM_PortNames;
            COM_PortsList.Items.Clear();
            COM_PortsList.Items.AddRange(COM_Ports);
        }

        private void comboBoxCOM_Ports_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox COM_PortsList = (ComboBox)(sender);
            COM_Port = COM_PortsList.Text;
            if (comm.connectionState != Comm.CommStatus.connectionOff)
                comm.TryToReconnect(COM_Port);
        }
        #endregion

        #region Private functions
        private bool showedMessage = false;
        private void ShowMessage(string message)
        {
            if (message == "") return;
            if (showedMessage == false)
            {
                DialogResult result;
                showedMessage = true;
                result = MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (result == DialogResult.Cancel) showedMessage = false;
                showedMessage = false;
            }
        }
        private void ShowCommStatus()
        {
            if (comm.connectionState != Comm.CommStatus.connectionActive)
                this.buttonConnect.Text = "Connect";
            else this.buttonConnect.Text = "Disconnect";

            switch (comm.connectionState)
            {
                case Comm.CommStatus.connectionOff:
                    this.buttonConnect.BackColor = Color.FromArgb(255, 0, 0);
                    break;

                case Comm.CommStatus.connectionActive:
                    this.buttonConnect.BackColor = Color.FromArgb(0, 255, 0);
                    break;
            }
        }

        private int[] lastMsgID = new int[(int)(Comm.MsgTransID.MSG_TRANS_LAST_MSG)];
        private void GetMessageFromComm(int msgID, Comm.MsgTransID msgTypeID, string text)
        {
            TextBox textBox = null;

            /* Compare last message ID with current */
            if (lastMsgID[(int)(msgTypeID)] == msgID) return;

            /* Parce by message type ID */
            switch (msgTypeID)
            {
                case Comm.MsgTransID.MSG_TRANS_SEND_TIME:
                    textBox = textBoxDateTime;
                    break;

                case Comm.MsgTransID.MSG_TRANS_SEND_GPS_FIX:
                    textBox = textBoxGPS_Coordinate;
                    break;

                case Comm.MsgTransID.MSG_TRANS_SEND_ROUTE_NUM:
                    textBox = textBoxRouteNum;
                    break;

                case Comm.MsgTransID.MSG_TRANS_SEND_ROUTE_NAME:
                    textBox = textBoxRouteName;
                    break;

                case Comm.MsgTransID.MSG_TRANS_SEND_CURR_STOP_NAME:
                    textBox = textBoxCurrStopName;
                    break;

                case Comm.MsgTransID.MSG_TRANS_SEND_CURR_STOP_TEXT:
                    textBox = textBoxCurrStopMsg;
                    break;

                case Comm.MsgTransID.MSG_TRANS_SEND_NXT_STOP_NAME:
                    textBox = textBoxNxtStopName;
                    break;

                case Comm.MsgTransID.MSG_TRANS_SEND_NXT_STOP_TEXT:
                    textBox = textBoxNxtStopMsg;
                    break;
            }

            /* Store last message ID */
            lastMsgID[(int)(msgTypeID)] = msgID;

            /* Show the message with selected textBox control */
            if (textBox != null) SafeSetTxtToTextBox(textBox, text);
        }

        private void ClearAllMessages()
        {
            /* Clear last message IDs */
            for (int i = 0; i < lastMsgID.Length; i++)
            {
                lastMsgID[i] = 0;
            }

            SafeSetTxtToTextBox(textBoxDateTime, "");
            SafeSetTxtToTextBox(textBoxGPS_Coordinate, "");
            SafeSetTxtToTextBox(textBoxRouteNum, "");
            SafeSetTxtToTextBox(textBoxRouteName, "");
            SafeSetTxtToTextBox(textBoxCurrStopName, "");
            SafeSetTxtToTextBox(textBoxCurrStopMsg, "");
            SafeSetTxtToTextBox(textBoxNxtStopName, "");
            SafeSetTxtToTextBox(textBoxNxtStopMsg, "");
        }

        private delegate void SafeCallDelegate(TextBox textBox, string text);
        private void SafeSetTxtToTextBox(TextBox textBox, string text)
        {
            if (textBox.InvokeRequired)
            {
                var d = new SafeCallDelegate(SafeSetTxtToTextBox);
                textBox.Invoke(d, new object[] { textBox, text });
            }
            else
            {
                textBox.Text = text;
            }
        }

        #endregion
    }
}

