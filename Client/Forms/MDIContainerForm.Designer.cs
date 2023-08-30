namespace Client
{
    partial class MDIContainerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MDIContainerForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lblCompany = new System.Windows.Forms.ToolStripLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attendanceDevicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shutdwnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssl_DBServer = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblDatabase = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsStatusUserName = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusDate = new System.Windows.Forms.ToolStripStatusLabel();
            this.pbBusy = new System.Windows.Forms.ToolStripProgressBar();
            this.tsslblVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblKeyboard = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCapsLock = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblNumLock = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblInsert = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblBusyMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblCompany});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1289, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // lblCompany
            // 
            this.lblCompany.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblCompany.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCompany.ForeColor = System.Drawing.Color.Navy;
            this.lblCompany.Name = "lblCompany";
            this.lblCompany.Size = new System.Drawing.Size(0, 22);
            this.lblCompany.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1289, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // createToolStripMenuItem
            // 
            this.createToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.attendanceDevicesToolStripMenuItem,
            this.shutdwnToolStripMenuItem});
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            this.createToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.createToolStripMenuItem.Text = "File";
            // 
            // attendanceDevicesToolStripMenuItem
            // 
            this.attendanceDevicesToolStripMenuItem.Name = "attendanceDevicesToolStripMenuItem";
            this.attendanceDevicesToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.attendanceDevicesToolStripMenuItem.Text = "Attendance Devices";
            this.attendanceDevicesToolStripMenuItem.Click += new System.EventHandler(this.attendanceDevicesToolStripMenuItem_Click);
            // 
            // shutdwnToolStripMenuItem
            // 
            this.shutdwnToolStripMenuItem.Name = "shutdwnToolStripMenuItem";
            this.shutdwnToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.shutdwnToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.shutdwnToolStripMenuItem.Text = "Exit";
            this.shutdwnToolStripMenuItem.Click += new System.EventHandler(this.logoutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pbBusy,
            this.lblKeyboard,
            this.lblCapsLock,
            this.lblNumLock,
            this.lblInsert,
            this.lblBusyMessage});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 414);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(1289, 25);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssl_DBServer
            // 
            this.tssl_DBServer.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssl_DBServer.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.tssl_DBServer.Image = global::Client.Properties.Resources.server_16x16;
            this.tssl_DBServer.Name = "tssl_DBServer";
            this.tssl_DBServer.Size = new System.Drawing.Size(20, 20);
            // 
            // lblDatabase
            // 
            this.lblDatabase.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lblDatabase.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.lblDatabase.Image = global::Client.Properties.Resources.databases_16x16;
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(20, 20);
            // 
            // tsStatusUserName
            // 
            this.tsStatusUserName.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsStatusUserName.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.tsStatusUserName.Image = global::Client.Properties.Resources.user;
            this.tsStatusUserName.Name = "tsStatusUserName";
            this.tsStatusUserName.Size = new System.Drawing.Size(20, 20);
            // 
            // statusDate
            // 
            this.statusDate.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusDate.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.statusDate.Image = global::Client.Properties.Resources.clock16x16;
            this.statusDate.Name = "statusDate";
            this.statusDate.Size = new System.Drawing.Size(20, 20);
            // 
            // pbBusy
            // 
            this.pbBusy.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.pbBusy.CausesValidation = false;
            this.pbBusy.Name = "pbBusy";
            this.pbBusy.Size = new System.Drawing.Size(200, 19);
            this.pbBusy.Visible = false;
            // 
            // tsslblVersion
            // 
            this.tsslblVersion.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsslblVersion.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.tsslblVersion.Name = "tsslblVersion";
            this.tsslblVersion.Size = new System.Drawing.Size(76, 20);
            this.tsslblVersion.Text = "tsslblVersion";
            // 
            // lblKeyboard
            // 
            this.lblKeyboard.Image = global::Client.Properties.Resources.keyboard_16x16;
            this.lblKeyboard.Name = "lblKeyboard";
            this.lblKeyboard.Size = new System.Drawing.Size(16, 20);
            // 
            // lblCapsLock
            // 
            this.lblCapsLock.AutoSize = false;
            this.lblCapsLock.Name = "lblCapsLock";
            this.lblCapsLock.Size = new System.Drawing.Size(40, 20);
            // 
            // lblNumLock
            // 
            this.lblNumLock.AutoSize = false;
            this.lblNumLock.Name = "lblNumLock";
            this.lblNumLock.Size = new System.Drawing.Size(40, 20);
            // 
            // lblInsert
            // 
            this.lblInsert.AutoSize = false;
            this.lblInsert.Name = "lblInsert";
            this.lblInsert.Size = new System.Drawing.Size(40, 20);
            // 
            // lblBusyMessage
            // 
            this.lblBusyMessage.AutoSize = false;
            this.lblBusyMessage.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.lblBusyMessage.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.lblBusyMessage.Name = "lblBusyMessage";
            this.lblBusyMessage.Size = new System.Drawing.Size(100, 20);
            this.lblBusyMessage.Spring = true;
            // 
            // MDIContainerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1289, 439);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MDIContainerForm";
            this.Text = "ExpressBase.Desktop";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsStatusUserName;
        private System.Windows.Forms.ToolStripStatusLabel statusDate;
        private System.Windows.Forms.ToolStripStatusLabel tssl_DBServer;
        private System.Windows.Forms.ToolStripProgressBar pbBusy;
        private System.Windows.Forms.ToolStripStatusLabel lblBusyMessage;
        private System.Windows.Forms.ToolStripStatusLabel lblDatabase;
        private System.Windows.Forms.ToolStripStatusLabel lblKeyboard;
        private System.Windows.Forms.ToolStripStatusLabel lblCapsLock;
        private System.Windows.Forms.ToolStripStatusLabel lblNumLock;
        private System.Windows.Forms.ToolStripStatusLabel lblInsert;
        private System.Windows.Forms.ToolStripMenuItem shutdwnToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel lblCompany;
        private System.Windows.Forms.ToolStripStatusLabel tsslblVersion;
        private System.Windows.Forms.ToolStripMenuItem attendanceDevicesToolStripMenuItem;
    }
}

