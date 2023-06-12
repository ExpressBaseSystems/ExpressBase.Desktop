using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Client.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Client.Forms.PayRoll;
using System.Data.Common;
using System.Diagnostics;

namespace Client
{
    public partial class MDIContainerForm : Form
    {
        public DockPanel dockPanel;
        private Login _loginform;
        private DataSet ds;
        private System.Timers.Timer timer;

        private DateTime _currentdt = DateTime.MinValue;
        public DateTime CurrentDBDateTime
        {
            get { return _currentdt; }
            set { _currentdt = value; }
        }

        public System.Windows.Forms.ComboBox CmbLocations
        {
            get { return null; }
        }

        protected override void OnLoad(EventArgs e)
        {
            timer.Start();
            base.OnLoad(e);
        }

        public string SolutionURL { get; set; }
        public string BToken { get; set; }
        public string RToken { get; set; }

        private List<DeviceRecord> Devices { get; set; }

        public MDIContainerForm(List<DeviceRecord> deviceRecords, Login loginform, string solutionURL, string bToken, string rToken)
        {
            InitializeComponent();

            this.SolutionURL = solutionURL;
            this.BToken = bToken;
            this.RToken = rToken;

            Devices = deviceRecords;
            
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true;
            this.KeyUp += new KeyEventHandler(MDIContainerForm_KeyDown);

            dockPanel = new DockPanel();
            dockPanel.ActiveAutoHideContent = null;
            dockPanel.Dock = DockStyle.Fill;
            dockPanel.Font = new Font("Tahoma", 14F, FontStyle.Regular, GraphicsUnit.World);
            dockPanel.Theme = new VS2005Theme();
            this.Controls.Add(dockPanel);
            dockPanel.BringToFront();
            _loginform = loginform;
            //this.tsStatusUserName.Text = loggedInUser.UserName;
            //tssl_DBServer.Text = !string.IsNullOrEmpty(DBHelper.Instance.DBServerMask) ? DBHelper.Instance.DBServerMask : DBHelper.Instance.DBServerName;
            //lblDatabase.Text = DBHelper.Instance.DBName;
            tsslblVersion.Text = Application.ProductVersion;
            lblBusyMessage.Width = statusStrip1.Width - tsStatusUserName.Width - tsslblVersion.Width - tssl_DBServer.Width - lblKeyboard.Width - lblNumLock.Width - lblCapsLock.Width - lblInsert.Width;
            lblNumLock.Text = Control.IsKeyLocked(Keys.NumLock) ? "NUM" : "   ";
            lblCapsLock.Text = Control.IsKeyLocked(Keys.CapsLock) ? "CAPS" : "   ";
            lblInsert.Text = Control.IsKeyLocked(Keys.Insert) ? "CAPS" : "   ";
            var start1 = DateTime.Now;
        }

        private string numLock = "   ", capsLock = "   ", insert = "   ";
        private void MDIContainerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode & Keys.KeyCode) == Keys.CapsLock)
                capsLock = Control.IsKeyLocked(Keys.CapsLock) ? "CAPS" : "   ";

            if ((e.KeyCode & Keys.KeyCode) == Keys.NumLock)
                numLock = Control.IsKeyLocked(Keys.NumLock) ? "NUM" : "   ";

            if ((e.KeyCode & Keys.KeyCode) == Keys.Insert)
                insert = Control.IsKeyLocked(Keys.Insert) ? "INS" : "   ";

            lblNumLock.Text = numLock;
            lblCapsLock.Text = capsLock;
            lblInsert.Text = insert;
        }

        delegate void UpdateTimeCallback();
        public void UpdateTime()
        {
            if (statusStrip1.InvokeRequired)
            {
                try
                {
                    UpdateTimeCallback d = new UpdateTimeCallback(UpdateTime);
                    this.Invoke(d);
                }
                catch (ObjectDisposedException ode) { }
            }
            //else
                //statusDate.Text = CacheHelper.Get<DateTime>(CacheKeys.SYSVARS_NOW_LOCALE).ToString("yyyy-MM-dd HH:mm tt");
        }

        private double ElapsedInterval { get; set; }
        private bool _bJustAfterLogin = true;

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //ElapsedInterval += timer.Interval;

            //CurrentDBDateTime = (CurrentDBDateTime == DateTime.MinValue) ? DBHelper.Instance.GetCurrentDateTimeFromDB() : CurrentDBDateTime.AddMilliseconds(timer.Interval);

            //CacheHelper.Set(CacheKeys.SYSVARS_NOW, CurrentDBDateTime);
            //CacheHelper.Set(CacheKeys.SYSVARS_NOW_LOCALE, MiscHelper.ConvertToLocalTime(CurrentDBDateTime));

            //if (_bJustAfterLogin || ElapsedInterval % (30 * timer.Interval) == 0)
            //    UpdateTime();

            //_bJustAfterLogin = false;
        }

        public int SelectedLocationID
        {
            get { return Convert.ToInt32(CmbLocations.SelectedValue); }
        }

        private int prevExampleIndex = 0;
        private void cbx_Example_Enter(object sender, EventArgs e)
        {
            prevExampleIndex = (sender as System.Windows.Forms.ComboBox).SelectedIndex;
        }

        delegate void BecomeBusyCallback(string message, int completion);
        public void BecomeBusy(string message, int completion)
        {
            if (statusStrip1.InvokeRequired)
            {
                BecomeBusyCallback d = new BecomeBusyCallback(BecomeBusy);
                this.Invoke(d, new object[] { message, completion } );
            }
            else
            {
                if (!pbBusy.Visible)
                    pbBusy.Visible = true;
                lblBusyMessage.Text = message;
                if (completion <= pbBusy.Maximum)
                    pbBusy.Value = completion;
                else
                    pbBusy.Value = pbBusy.Maximum;
                statusStrip1.Refresh();
            }
        }

        delegate void BecomeFreeCallback();
        public void BecomeFree()
        {
            if (statusStrip1.InvokeRequired)
            {
                BecomeFreeCallback d = new BecomeFreeCallback(BecomeFree);
                this.Invoke(d);
            }
            else
            {
                lblBusyMessage.Text = string.Empty;
                pbBusy.Value = pbBusy.Maximum;
                pbBusy.Visible = false;
                statusStrip1.Refresh();
            }
        }

        delegate void PostMessageCallback(string message);
        public void PostMessage(string message)
        {
            if (statusStrip1.InvokeRequired)
            {
                PostMessageCallback d = new PostMessageCallback(PostMessage);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                lblBusyMessage.Text = message;
                statusStrip1.Refresh();
            }
        }

        delegate void SetMaximumCallback(int value);
        public void SetMaximum(int value)
        {
            if (statusStrip1.InvokeRequired)
            {
                SetMaximumCallback d = new SetMaximumCallback(SetMaximum);
                this.Invoke(d, new object[] { value });
            }
            else
            {
                if (!pbBusy.Visible)
                    pbBusy.Visible = true;
                pbBusy.Maximum = value;
                statusStrip1.Refresh();
            }
        }

        [DllImport("User32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);

        private System.Diagnostics.Process _calculatorProcess;
        private void InvokeCalculator()
        {
            if (_calculatorProcess != null && _calculatorProcess.HasExited)
                _calculatorProcess = null;

            if (_calculatorProcess == null)
                _calculatorProcess = System.Diagnostics.Process.Start("calc.exe");

            _calculatorProcess.WaitForInputIdle();
            SetParent(_calculatorProcess.MainWindowHandle, this.Handle);
        }

        private System.Diagnostics.Process _ammyProcess;
        private void InvokeAmmy()
        {
            if (_ammyProcess != null && _ammyProcess.HasExited)
                _ammyProcess = null;

            if (_ammyProcess == null)
                _ammyProcess = System.Diagnostics.Process.Start("AA_v3.5.exe");

            _ammyProcess.WaitForInputIdle();
            SetParent(_ammyProcess.MainWindowHandle, this.Handle);
        }

        private void calculatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvokeCalculator();
        }

        private void btnCalculator_Click(object sender, EventArgs e)
        {
            InvokeCalculator();
        }

        private void myAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MyAccountForm _myAccountForm = new MyAccountForm();
            //_myAccountForm.ShowDialog(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            Application.Exit();            
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!closingViaProgram)
            {
                if (PreLogout() == System.Windows.Forms.DialogResult.Yes)
                    base.OnClosing(e);
                else
                    e.Cancel = true;
            }
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Logout(true);
        }

        private DialogResult PreLogout()
        {
            Mask(false, null);
            if (DialogResult.Yes == MessageBox.Show("Are you sure you want to logout of PHX+?", "PHX+", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _loginform.Close();
                return System.Windows.Forms.DialogResult.Yes;
            }
            else
                UnMask();

            return System.Windows.Forms.DialogResult.No;
        }

        internal bool closingViaProgram = false;
        internal void Logout(bool ask)
        {
            if (ask)
            {
                if (PreLogout() == System.Windows.Forms.DialogResult.Yes)
                    closingViaProgram = true;
            }
            else
                closingViaProgram = true;

            if (closingViaProgram)
                this.Close();
       }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //SettingsForm settings = new SettingsForm();
            //settings.ShowDialog();
        }

        public Form Overlay { get { return _overlay; } }


        delegate void MaskCallback(bool force_logout, string message);
        public void Mask(bool force_logout, string message)
        {
            if (this.InvokeRequired)
            {
                MaskCallback d = new MaskCallback(MaskInner);
                this.Invoke(d, new object[] { force_logout, message });
            }
            else
            {
                MaskInner(force_logout, message);
            }
        }

        private LayerWindow _overlay = null;
        public void MaskInner(bool force_logout, string message)
        {
            if (_overlay == null)
            {
                if (!force_logout)
                    _overlay = new LayerWindow();
                else
                    _overlay = new LayerWindow(this, true, message);
            }
            _overlay.Show(this);
        }

        public void UnMask()
        {
            if (_overlay != null)
                _overlay.Unmask();
            this.Show();
            this.BringToFront();
        }

        public void ShowShutDownLabel()
        {
            if (_overlay == null)
                _overlay = new LayerWindow();
            _overlay.ShowShutDownLabel();
        }

        internal void Lock()
        {
            _loginform = new Login(this, false);
            Mask(false, null);
            _loginform.Show(_overlay);
        }

        internal void LogOff()
        {
            _loginform = new Login(this, true);
            Mask(false, null);
            _loginform.Show(_overlay);
        }

        private void lockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Lock();
        }

        private void shutdownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Logout(true);
        }

        private void logOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LogOff();
        }

        /// <summary>
        /// Gets an assembly path from the GAC given a partial name.
        /// </summary>
        /// <param name="name">An assembly partial name. May not be null.</param>
        /// <returns>
        /// The assembly path if found; otherwise null;
        /// </returns>
        public static string GetAssemblyPath(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            string finalName = name;
            AssemblyInfo aInfo = new AssemblyInfo();
            aInfo.cchBuf = 1024; // should be fine...
            aInfo.currentAssemblyPath = new String('\0', aInfo.cchBuf);

            IAssemblyCache ac;
            int hr = CreateAssemblyCache(out ac, 0);
            if (hr >= 0)
            {
                hr = ac.QueryAssemblyInfo(0, finalName, ref aInfo);
                if (hr < 0)
                    return null;
            }

            return aInfo.currentAssemblyPath;
        }


        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
        private interface IAssemblyCache
        {
            void Reserved0();

            [PreserveSig]
            int QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] string assemblyName, ref AssemblyInfo assemblyInfo);
        }

        private void attendanceDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AttendanceDeviceList __deviceList = new AttendanceDeviceList();
            __deviceList.Devices = this.Devices;
            __deviceList.Show(dockPanel);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AssemblyInfo
        {
            public int cbAssemblyInfo;
            public int assemblyFlags;
            public long assemblySizeInKB;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string currentAssemblyPath;
            public int cchBuf; // size of path buf.
        }

        [DllImport("fusion.dll")]
        private static extern int CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);
    }

    public class LayerWindow : Form
    {
        private bool bForceLogout = false;
        private string Message = string.Empty;
        private MDIContainerForm MDIContainerForm { get; set; }

        public LayerWindow()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            TransparencyKey = Color.Fuchsia;
            base.BackColor = Color.DarkSlateBlue;
            Opacity = 0.8;
            ShowInTaskbar = false;
        }

        public LayerWindow(MDIContainerForm mdiparent, bool forcelogout, string message)
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            TransparencyKey = Color.Fuchsia;
            base.BackColor = Color.DarkSlateBlue;
            Opacity = 0.8;
            ShowInTaskbar = false;

            this.MDIContainerForm = mdiparent;
            this.bForceLogout = forcelogout;
            this.Message = message;
        }

        public void Show(Control parent)
        {
            if (parent == null)
                throw new ApplicationException("No parent provided");

            var container = parent.FindForm();
            if (container == null)
                throw new ApplicationException("No parent Form found. Make sure that the control is contained in a form before showing a popup.");

            Location = PointToScreen(container.Location);
            Bounds = container.Bounds;

            Owner = container;
            Owner.Enabled = false;

            base.Visible = false;
            base.Show(container);

            if (this.bForceLogout)
            {
                if (DialogResult.OK == MessageBox.Show(this, this.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Hand))
                    this.MDIContainerForm.Logout(false);
            }
        }

        public void ShowShutDownLabel()
        {
            Label _sh = new Label();
            _sh.AutoSize = false;
            _sh.TextAlign = ContentAlignment.MiddleCenter;
            _sh.Dock = DockStyle.Fill;
            _sh.Font = new Font("Arial", 30f);
            _sh.ForeColor = Color.Black;
            _sh.Text = "Shutting Down PHX+ Server... \r\nPlease Wait!";
            this.Controls.Add(_sh);
        }

        public void Unmask()
        {
            if (Owner != null)
                Owner.Enabled = true;

            Hide();
        }
    }
}

// http://www.codeproject.com/Articles/43181/A-Serious-Outlook-Style-Navigation-Pane-Control
// http://www.guifreaks.net/index.php?action=feat1
// http://www.codeproject.com/Articles/17200/Outlook-2003-Style-Navigation-Pane