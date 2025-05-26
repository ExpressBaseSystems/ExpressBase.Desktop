using System;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using RestSharp;
using Client.Extensions;
using Newtonsoft.Json;
using DeviceId;
using System.Collections.Generic;

namespace Client
{
    public partial class Login : Form
    {
        private MDIContainerForm mdiform = null;

        public Login()
        {
            InitializeComponent();

#if DEBUG
            txtSolution.Text = "hairocraft.eb-test.shop";
            txtUserName.Text = "febin@gmail.com";
            txtPassword.Text = "123123123";
#endif
        }

        public Login(MDIContainerForm _mdiform, bool bLogOff) // LOCKED
        {
            mdiform = _mdiform;

            InitializeComponent();

            if (!bLogOff)
            {
                btnCancel.Enabled = false;
                this.ControlBox = false;
                this.ShowInTaskbar = false;

                txtUserName.Enabled = false;
            }
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            if (_backgroundLoginWorker != null && _backgroundLoginWorker.IsBusy)
                _backgroundLoginWorker.CancelAsync();

            this.Close();
        }

        private string confname = null;
        private string username = null;
        private string password = null;
        private string servername = null;
        private BackgroundWorker _backgroundLoginWorker = null;

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            picProcessing.Visible = true;
            btnLogin.Enabled = false;
            txtPassword.ReadOnly = true;
            txtUserName.ReadOnly = true;
            txtSolution.ReadOnly = true;

            username = this.txtUserName.Text.Trim();
            password = this.txtPassword.Text.Trim();
            servername = txtSolution.Text.Trim();

            _backgroundLoginWorker = new BackgroundWorker();
            _backgroundLoginWorker.WorkerSupportsCancellation = true;
            _backgroundLoginWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_backgroundLoginWorker_RunWorkerCompleted);
            _backgroundLoginWorker.DoWork += new DoWorkEventHandler(_backgroundLoginWorker_DoWork);
            _backgroundLoginWorker.RunWorkerAsync();
        }

        private void _backgroundLoginWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            picProcessing.Visible = false;
            btnLogin.Enabled = true;
            txtPassword.ReadOnly = false;
            txtUserName.ReadOnly = false;
            txtSolution.ReadOnly = false;
        }

        public string SolutionURL
        {
            get
            {
                string sSolutionURL = txtSolution.Text;

                if (!string.IsNullOrEmpty(sSolutionURL))
                {
                    if (!sSolutionURL.Contains("eb-test.shop"))
                        sSolutionURL = $"{sSolutionURL}.expressbase.com";

                    if (!sSolutionURL.StartsWith("https://"))
                        sSolutionURL = $"https://{sSolutionURL}";
                }

                return sSolutionURL;
            }
        }

        private void _backgroundLoginWorker_DoWork(object sender, DoWorkEventArgs args)
        {
            
            var deviceIdBuilder = new DeviceId.DeviceIdBuilder();
            deviceIdBuilder.AddMacAddress(false);
            deviceIdBuilder.AddMachineName();

            //var options = new RestClientOptions($"{sSolutionURL}/api/auth");
            var client = new RestClient($"{SolutionURL}");

            RestSharp.RestRequest request = new RestSharp.RestRequest($"api/auth", Method.Get);
            request.AddQueryParameter("username", txtUserName.Text);
            request.AddQueryParameter("password", string.Concat(txtPassword.Text, txtUserName.Text).ToMD5Hash());
            request.AddQueryParameter("deviceid", deviceIdBuilder.ToString());

            var response = client.Get(request);

            dynamic jsonO = JsonConvert.DeserializeObject(response.Content);
            string rToken = jsonO.rToken;
            string bToken = jsonO.bToken;

            if (!string.IsNullOrEmpty(rToken) && !string.IsNullOrEmpty(bToken))
            {
                //this.Hide();
                client = new RestClient($"{SolutionURL}");

                RestSharp.RestRequest request2 = new RestSharp.RestRequest($"api/get_attendance_device_list", Method.Get);
                request2.AddHeader("bToken", bToken);
                request2.AddHeader("rToken", rToken);

                var response2 = client.Get(request2);

                dynamic json1 = JsonConvert.DeserializeObject(response2.Content);
                List<DeviceRecord> deviceList = JsonConvert.DeserializeObject<List<DeviceRecord>>(json1.deviceList.ToString());

                if (deviceList.Count > 0)
                {
                    LoginToOpenMDI(deviceList, this.SolutionURL, bToken, rToken);
                }
            }
        }

        delegate void LoginToOpenMDICallback(List<DeviceRecord> deviceList, string solutionURL, string bToken, string rToken);
        public void LoginToOpenMDI(List<DeviceRecord> deviceList, string solutionURL, string bToken, string rToken)
        {
            if (this.InvokeRequired)
            {
                LoginToOpenMDICallback d = new LoginToOpenMDICallback(LoginToOpenMDI);
                this.Invoke(d, new object[] { deviceList, this.SolutionURL, bToken, rToken });
            }
            else
            {
                try
                {
                    picProcessing.Visible = false;
                    this.Hide();

                    if (mdiform == null)
                        mdiform = new MDIContainerForm(deviceList, this, this.SolutionURL, bToken, rToken);
                    mdiform.UnMask();
                }
                catch (Exception e)
                {
                    this.Visible = true;
                    PostErrorMessage(e.Message);
                }
            }
        }

        delegate void PostErrorMessageCallback(string message);
        public void PostErrorMessage(string message)
        {
            if (picProcessing.InvokeRequired)
            {
                PostErrorMessageCallback d = new PostErrorMessageCallback(PostErrorMessage);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                picProcessing.Visible = false;
                System.Media.SystemSounds.Beep.Play();
                lblStatusLogin.ForeColor = System.Drawing.Color.Red;
                lblStatusLogin.Text = message.ToUpper();
            }
        }

        private void txtUsername_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                txtPassword.Focus();
        }

        private void txtPassword_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin.Focus();
        }

        private string ReadFileName(string path)
        {
            string result = string.Empty;

            string localsavefile = string.Format(@"{0}\default.txt", path);
            if (File.Exists(localsavefile))
            {
                using (var reader = new System.IO.StreamReader(localsavefile))
                {
                    result = reader.ReadLine();
                }
            }

            return result;
        }

        [DllImport("User32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);
    }

    public class DeviceRecord
    {
        public int Id { get; set; }

        public string DeviceName { get; set; }

        public string Ip { get; set; }

        public int Port { get; set; }

        public int LocationId { get; set; }

        public string LocationShortName { get; set; }

        public string LastSyncTs { get; set; }

        public string CommKey { get; set; }

        public string CommKeyType { get; set; }

        public string MacAddress { get; set; }
    }

}
