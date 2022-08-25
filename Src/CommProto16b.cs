using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

public class CommProto16b
{
    /* Delegate for received broadcast data */
    public delegate void RecBroadcast(ref byte[] data, int length);
    public event RecBroadcast RecBroadcastEvent;

    /* Delegate for received address data */
    public delegate void RecAddress(int addr, ref byte[] data, int length);
    public event RecAddress RecFromMasterEvent;
    public event RecAddress RecFromSlaveEvent;

    /* Delegate for valid received data */
    public delegate void RecValidData();
    public event RecValidData RecValidDataEvent;

    /* Internal constants --------------------------------------------------------*/
    /* Communication protocol fields constants */
    /* Marker of data package beginning */
    private const Byte COMM_16B_MARKER = 0x55;

    private const int COMM_16B_MARKER_SIZE = 1;
    private const int COMM_16B_ADDRESS_SIZE = 1;
    private const int COMM_16B_DATA_LENG_SIZE = 2;
    private const int COMM_16B_CRC_SIZE = 1;

    /* Additional non-field constants */
    private const int COMM_16B_HEADER_SIZE = COMM_16B_DATA_POS;

    /* Communication protocol fields positions */
    private const int COMM_16B_MARKER_POS = 0;
    private const int COMM_16B_ADDR_POS =
        COMM_16B_MARKER_POS + COMM_16B_MARKER_SIZE;
    private const int COMM_16B_LENGTH_POS =
        COMM_16B_ADDR_POS + COMM_16B_ADDRESS_SIZE;
    private const int COMM_16B_DATA_POS =
        COMM_16B_LENGTH_POS + COMM_16B_DATA_LENG_SIZE;

    private const int COMM_16B_MAX_REC_DATA_SIZE = 0x400;
    private const int COMM_16B_MAX_REC_PACKAGE_SIZE =
        (COMM_16B_HEADER_SIZE + COMM_16B_MAX_REC_DATA_SIZE + COMM_16B_CRC_SIZE);

    private const int COMM_16B_MAX_TRANS_DATA_SIZE = 0x400;
    private const int COMM_16B_MAX_TRANS_PACKAGE_SIZE =
        (COMM_16B_HEADER_SIZE + COMM_16B_MAX_TRANS_DATA_SIZE + COMM_16B_CRC_SIZE);

    /* Bit masks definitions for address byte */
    private const byte COMM_16B_MASTER_FLAG_MASK = (1 << 7);
    private const byte COMM_16B_BROADCAST_ADDRESS = 0xFF;

    private byte[] CRC_Table =
    {
          0, 94,188,226, 97, 63,221,131,194,156,126, 32,163,253, 31, 65,
        157,195, 33,127,252,162, 64, 30, 95,  1,227,189, 62, 96,130,220,
         35,125,159,193, 66, 28,254,160,225,191, 93,  3,128,222, 60, 98,
        190,224,  2, 92,223,129, 99, 61,124, 34,192,158, 29, 67,161,255,
         70, 24,250,164, 39,121,155,197,132,218, 56,102,229,187, 89,  7,
        219,133,103, 57,186,228,  6, 88, 25, 71,165,251,120, 38,196,154,
        101, 59,217,135,  4, 90,184,230,167,249, 27, 69,198,152,122, 36,
        248,166, 68, 26,153,199, 37,123, 58,100,134,216, 91,  5,231,185,
        140,210, 48,110,237,179, 81, 15, 78, 16,242,172, 47,113,147,205,
         17, 79,173,243,112, 46,204,146,211,141,111, 49,178,236, 14, 80,
        175,241, 19, 77,206,144,114, 44,109, 51,209,143, 12, 82,176,238,
         50,108,142,208, 83, 13,239,177,240,174, 76, 18,145,207, 45,115,
        202,148,118, 40,171,245, 23, 73,  8, 86,180,234,105, 55,213,139,
         87,  9,235,181, 54,104,138,212,149,203, 41,119,244,170, 72, 22,
        233,183, 85, 11,136,214, 52,106, 43,117,151,201, 74, 20,246,168,
        116, 42,200,150, 21, 75,169,247,182,232, 10, 84,215,137,107, 53,
    };

    /* Variables -----------------------------------------------------------------*/
    /* For transmitting answer's bytes */
    private byte[] transBuff = new byte[COMM_16B_MAX_TRANS_PACKAGE_SIZE];
    private int transCounter;

    /* Public functions ----------------------------------------------------------*/
    public CommProto16b()
    {

    }

    /* Functions for sending data */
    public bool PrepareToSend(byte addr, bool master)
    {
        /* Form header and set data counter */
        transBuff[COMM_16B_MARKER_POS] = COMM_16B_MARKER;
        transBuff[COMM_16B_ADDR_POS] = addr;
        if (master) transBuff[COMM_16B_ADDR_POS] |= COMM_16B_MASTER_FLAG_MASK;
        transCounter = COMM_16B_HEADER_SIZE;
        return true;
    }

    public bool PrepareToBroadcastSend()
    {
        /* Form header and set data counter */
        transBuff[COMM_16B_MARKER_POS] = COMM_16B_MARKER;
        transBuff[COMM_16B_ADDR_POS] = COMM_16B_BROADCAST_ADDRESS;
        transCounter = COMM_16B_HEADER_SIZE;
        return true;
    }

    public bool AppendData(byte data)
    {
        /* Check for the data overflow (reserve space for CRC) */
        if (transCounter >= (transBuff.Length - 2)) return false;

        /* Store data */
        transBuff[transCounter] = data;
        transCounter++;
        return true;
    }

    public bool SendData()
    {
        /* Validate current transmit data counter */
        if (transCounter < COMM_16B_HEADER_SIZE) return false;

        /* Store data length */
        int length = transCounter - COMM_16B_HEADER_SIZE;
        transBuff[COMM_16B_LENGTH_POS] = (byte)(0xFF & length);
        transBuff[COMM_16B_LENGTH_POS + 1] = (byte)(0xFF & (length >> 8));

        /* Calculate and store CRC */
        byte CRC = CheckCRC(ref transBuff, transCounter);
        transBuff[transCounter] = CRC;
        transCounter++;
        return /*DriverSendData(transBuff, transCounter);*/true;
    }

    /* Functions for receiving data */
    /* Received data bytes */
    private byte[] recBuff = new byte[COMM_16B_MAX_REC_PACKAGE_SIZE];
    private byte[] recData = new byte[COMM_16B_MAX_REC_DATA_SIZE];
    private int recPointer = 0;
    private int recDataLength = 0;
    public void RecByte(byte recByte)
    {
        /* If in receiving buffer number of bytes more than max package data size -
		restart receive */
        if (recPointer >= COMM_16B_MAX_REC_PACKAGE_SIZE) recPointer = 0;

        /* Store receive byte */
        recBuff[recPointer] = recByte;
        if (recPointer == COMM_16B_MARKER_POS)
        {
            /* Validate marker */
            if (recByte != COMM_16B_MARKER)
            {
                /* Wrong marker */
                recPointer = 0;
                return;
            }
            recPointer++;
            return;
        }
        if (recPointer == COMM_16B_ADDR_POS)
        {
            /* Analyze address after receive the whole data package */
            recPointer++;
            return;
        }
        if (recPointer == COMM_16B_LENGTH_POS)
        {
            /* Store the first part of data length */
            recDataLength = recByte;
            recPointer++;
            return;
        }
        if (recPointer == COMM_16B_LENGTH_POS + 1)
        {
            /* Store the second part of data length */
            recDataLength |= (recByte << 8);

            /* Validate length */
            if (recDataLength == 0)
            {
                /* Invalid package */
                recPointer = 0;
                return;
            }
            if (recDataLength > COMM_16B_MAX_REC_DATA_SIZE)
            {
                /* Too long package */
                recPointer = 0;
                return;
            }
            recPointer++;
            return;
        }

        if (recPointer < (COMM_16B_HEADER_SIZE + recDataLength))
        {
            /* Wait for receiving the whole data package */
            recPointer++;
            return;
        }

        /* All data has been received */
        recPointer++;

        /* The whole data package has been received, calculate and compare CRC */
        if (CheckCRC(ref recBuff, recPointer) != recByte)
        {
            /* Package is not valid */
            recPointer = 0;
            return;
        }

        /* Package is valid */
        recPointer = 0;

        if (RecValidDataEvent != null)
            RecValidDataEvent();

        /* Store and validate the address */
        int recAddr = recBuff[COMM_16B_ADDR_POS];
        if ((recAddr & (~COMM_16B_MASTER_FLAG_MASK)) == 0)
        {
            /* Invalid address */
            return;
        }

        /* Copy data */
        for (int i = 0; i < recDataLength; i++)
        {
            recData[i] = recBuff[i + COMM_16B_DATA_POS];
        }

        if (recAddr == COMM_16B_BROADCAST_ADDRESS)
        {
            /* Broadcast package */
            if (RecBroadcastEvent != null)
                RecBroadcastEvent(ref recData, recDataLength);
            return;
        }
        if ((recAddr & COMM_16B_MASTER_FLAG_MASK) != 0)
        {
            /* Package from master device, correct the address */
            recAddr &= ~COMM_16B_MASTER_FLAG_MASK;
            if (RecFromMasterEvent != null)
                RecFromMasterEvent(recAddr, ref recData, recDataLength);
            return;
        }

        /* Package from slave device */
        if (RecFromSlaveEvent != null)
            RecFromSlaveEvent(recAddr, ref recData, recDataLength);
    }

    /* Private functions ---------------------------------------------------------*/
    private byte CheckCRC(ref byte[] buff, int size)
    {
        byte CRC = 0;
        for (int i = 1; i < size - 1; i++)
        {
            CRC ^= buff[i];
            CRC = CRC_Table[CRC];
        }
        return CRC;
    }
}

