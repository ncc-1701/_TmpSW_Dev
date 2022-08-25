namespace ARS_MsgTesterForm
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxConn = new System.Windows.Forms.GroupBox();
            this.comboBoxCOM_Ports = new System.Windows.Forms.ComboBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.textBoxDateTime = new System.Windows.Forms.TextBox();
            this.labelDateTime = new System.Windows.Forms.Label();
            this.labelGPS_Coordinate = new System.Windows.Forms.Label();
            this.textBoxGPS_Coordinate = new System.Windows.Forms.TextBox();
            this.textBoxRouteName = new System.Windows.Forms.TextBox();
            this.labelRouteNumAndName = new System.Windows.Forms.Label();
            this.textBoxRouteNum = new System.Windows.Forms.TextBox();
            this.textBoxCurrStopMsg = new System.Windows.Forms.TextBox();
            this.labelCurrStopNameAndMsg = new System.Windows.Forms.Label();
            this.textBoxCurrStopName = new System.Windows.Forms.TextBox();
            this.textBoxNxtStopMsg = new System.Windows.Forms.TextBox();
            this.labelNxtStopNameAndMsg = new System.Windows.Forms.Label();
            this.textBoxNxtStopName = new System.Windows.Forms.TextBox();
            this.groupBoxConn.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxConn
            // 
            this.groupBoxConn.Controls.Add(this.comboBoxCOM_Ports);
            this.groupBoxConn.Controls.Add(this.buttonConnect);
            this.groupBoxConn.Location = new System.Drawing.Point(7, 5);
            this.groupBoxConn.Name = "groupBoxConn";
            this.groupBoxConn.Size = new System.Drawing.Size(190, 51);
            this.groupBoxConn.TabIndex = 0;
            this.groupBoxConn.TabStop = false;
            this.groupBoxConn.Text = "Connection";
            // 
            // comboBoxCOM_Ports
            // 
            this.comboBoxCOM_Ports.FormattingEnabled = true;
            this.comboBoxCOM_Ports.Location = new System.Drawing.Point(6, 19);
            this.comboBoxCOM_Ports.Name = "comboBoxCOM_Ports";
            this.comboBoxCOM_Ports.Size = new System.Drawing.Size(95, 21);
            this.comboBoxCOM_Ports.TabIndex = 1;
            this.comboBoxCOM_Ports.SelectedIndexChanged += new System.EventHandler(this.comboBoxCOM_Ports_SelectedIndexChanged);
            this.comboBoxCOM_Ports.Click += new System.EventHandler(this.comboBoxCOM_Ports_Click);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(107, 17);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(75, 23);
            this.buttonConnect.TabIndex = 0;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // textBoxDateTime
            // 
            this.textBoxDateTime.Location = new System.Drawing.Point(203, 24);
            this.textBoxDateTime.Name = "textBoxDateTime";
            this.textBoxDateTime.ReadOnly = true;
            this.textBoxDateTime.Size = new System.Drawing.Size(134, 20);
            this.textBoxDateTime.TabIndex = 1;
            // 
            // labelDateTime
            // 
            this.labelDateTime.AutoSize = true;
            this.labelDateTime.Location = new System.Drawing.Point(200, 5);
            this.labelDateTime.Name = "labelDateTime";
            this.labelDateTime.Size = new System.Drawing.Size(104, 13);
            this.labelDateTime.TabIndex = 2;
            this.labelDateTime.Text = "Date and time (UTC)";
            // 
            // labelGPS_Coordinate
            // 
            this.labelGPS_Coordinate.AutoSize = true;
            this.labelGPS_Coordinate.Location = new System.Drawing.Point(340, 5);
            this.labelGPS_Coordinate.Name = "labelGPS_Coordinate";
            this.labelGPS_Coordinate.Size = new System.Drawing.Size(82, 13);
            this.labelGPS_Coordinate.TabIndex = 4;
            this.labelGPS_Coordinate.Text = "GPS coordinate";
            // 
            // textBoxGPS_Coordinate
            // 
            this.textBoxGPS_Coordinate.Location = new System.Drawing.Point(343, 24);
            this.textBoxGPS_Coordinate.Name = "textBoxGPS_Coordinate";
            this.textBoxGPS_Coordinate.ReadOnly = true;
            this.textBoxGPS_Coordinate.Size = new System.Drawing.Size(160, 20);
            this.textBoxGPS_Coordinate.TabIndex = 3;
            // 
            // textBoxRouteName
            // 
            this.textBoxRouteName.Location = new System.Drawing.Point(235, 61);
            this.textBoxRouteName.Name = "textBoxRouteName";
            this.textBoxRouteName.ReadOnly = true;
            this.textBoxRouteName.Size = new System.Drawing.Size(268, 20);
            this.textBoxRouteName.TabIndex = 7;
            // 
            // labelRouteNumAndName
            // 
            this.labelRouteNumAndName.AutoSize = true;
            this.labelRouteNumAndName.Location = new System.Drawing.Point(4, 64);
            this.labelRouteNumAndName.Name = "labelRouteNumAndName";
            this.labelRouteNumAndName.Size = new System.Drawing.Size(124, 13);
            this.labelRouteNumAndName.TabIndex = 6;
            this.labelRouteNumAndName.Text = "Route number and name";
            // 
            // textBoxRouteNum
            // 
            this.textBoxRouteNum.Location = new System.Drawing.Point(134, 61);
            this.textBoxRouteNum.Name = "textBoxRouteNum";
            this.textBoxRouteNum.ReadOnly = true;
            this.textBoxRouteNum.Size = new System.Drawing.Size(95, 20);
            this.textBoxRouteNum.TabIndex = 5;
            // 
            // textBoxCurrStopMsg
            // 
            this.textBoxCurrStopMsg.Location = new System.Drawing.Point(7, 118);
            this.textBoxCurrStopMsg.Multiline = true;
            this.textBoxCurrStopMsg.Name = "textBoxCurrStopMsg";
            this.textBoxCurrStopMsg.ReadOnly = true;
            this.textBoxCurrStopMsg.Size = new System.Drawing.Size(496, 115);
            this.textBoxCurrStopMsg.TabIndex = 10;
            // 
            // labelCurrStopNameAndMsg
            // 
            this.labelCurrStopNameAndMsg.AutoSize = true;
            this.labelCurrStopNameAndMsg.Location = new System.Drawing.Point(4, 95);
            this.labelCurrStopNameAndMsg.Name = "labelCurrStopNameAndMsg";
            this.labelCurrStopNameAndMsg.Size = new System.Drawing.Size(159, 13);
            this.labelCurrStopNameAndMsg.TabIndex = 9;
            this.labelCurrStopNameAndMsg.Text = "Current stop name and message";
            // 
            // textBoxCurrStopName
            // 
            this.textBoxCurrStopName.Location = new System.Drawing.Point(169, 92);
            this.textBoxCurrStopName.Name = "textBoxCurrStopName";
            this.textBoxCurrStopName.ReadOnly = true;
            this.textBoxCurrStopName.Size = new System.Drawing.Size(334, 20);
            this.textBoxCurrStopName.TabIndex = 8;
            // 
            // textBoxNxtStopMsg
            // 
            this.textBoxNxtStopMsg.Location = new System.Drawing.Point(7, 270);
            this.textBoxNxtStopMsg.Multiline = true;
            this.textBoxNxtStopMsg.Name = "textBoxNxtStopMsg";
            this.textBoxNxtStopMsg.ReadOnly = true;
            this.textBoxNxtStopMsg.Size = new System.Drawing.Size(496, 115);
            this.textBoxNxtStopMsg.TabIndex = 13;
            // 
            // labelNxtStopNameAndMsg
            // 
            this.labelNxtStopNameAndMsg.AutoSize = true;
            this.labelNxtStopNameAndMsg.Location = new System.Drawing.Point(4, 247);
            this.labelNxtStopNameAndMsg.Name = "labelNxtStopNameAndMsg";
            this.labelNxtStopNameAndMsg.Size = new System.Drawing.Size(147, 13);
            this.labelNxtStopNameAndMsg.TabIndex = 12;
            this.labelNxtStopNameAndMsg.Text = "Next stop name and message";
            // 
            // textBoxNxtStopName
            // 
            this.textBoxNxtStopName.Location = new System.Drawing.Point(169, 244);
            this.textBoxNxtStopName.Name = "textBoxNxtStopName";
            this.textBoxNxtStopName.ReadOnly = true;
            this.textBoxNxtStopName.Size = new System.Drawing.Size(334, 20);
            this.textBoxNxtStopName.TabIndex = 11;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 392);
            this.Controls.Add(this.textBoxNxtStopMsg);
            this.Controls.Add(this.labelNxtStopNameAndMsg);
            this.Controls.Add(this.textBoxNxtStopName);
            this.Controls.Add(this.textBoxCurrStopMsg);
            this.Controls.Add(this.labelCurrStopNameAndMsg);
            this.Controls.Add(this.textBoxCurrStopName);
            this.Controls.Add(this.textBoxRouteName);
            this.Controls.Add(this.labelRouteNumAndName);
            this.Controls.Add(this.textBoxRouteNum);
            this.Controls.Add(this.labelGPS_Coordinate);
            this.Controls.Add(this.textBoxGPS_Coordinate);
            this.Controls.Add(this.labelDateTime);
            this.Controls.Add(this.textBoxDateTime);
            this.Controls.Add(this.groupBoxConn);
            this.Name = "FormMain";
            this.Text = "ARS message tester";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBoxConn.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxConn;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.ComboBox comboBoxCOM_Ports;
        private System.Windows.Forms.TextBox textBoxDateTime;
        private System.Windows.Forms.Label labelDateTime;
        private System.Windows.Forms.Label labelGPS_Coordinate;
        private System.Windows.Forms.TextBox textBoxGPS_Coordinate;
        private System.Windows.Forms.TextBox textBoxRouteName;
        private System.Windows.Forms.Label labelRouteNumAndName;
        private System.Windows.Forms.TextBox textBoxRouteNum;
        private System.Windows.Forms.TextBox textBoxCurrStopMsg;
        private System.Windows.Forms.Label labelCurrStopNameAndMsg;
        private System.Windows.Forms.TextBox textBoxCurrStopName;
        private System.Windows.Forms.TextBox textBoxNxtStopMsg;
        private System.Windows.Forms.Label labelNxtStopNameAndMsg;
        private System.Windows.Forms.TextBox textBoxNxtStopName;
    }
}