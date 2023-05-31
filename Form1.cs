using DeviceId;
using ExpressBase.Desktop.Extensions;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace ExpressBase.Desktop
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();

            txtSolution.Text = "hairocraft.eb-test.shop";
            txtUserName.Text = "febin@gmail.com";
            txtPassword.Text = "123123123";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string sSolutionURL = txtSolution.Text;

            if (!string.IsNullOrEmpty(sSolutionURL))
            {
                if (!sSolutionURL.Contains("eb-test.shop"))
                    sSolutionURL = $"{sSolutionURL}.expressbase.com";

                if (!sSolutionURL.StartsWith("https://"))
                    sSolutionURL = $"https://{sSolutionURL}";
            }

            var deviceIdBuilder = new DeviceId.DeviceIdBuilder();
            deviceIdBuilder.AddMacAddress(false);
            deviceIdBuilder.AddMachineName();

            var options = new RestClientOptions($"{sSolutionURL}/api/auth");
            var client = new RestClient(options);

            RestSharp.RestRequest request = new RestSharp.RestRequest($"{sSolutionURL}/api/auth", RestSharp.Method.Get);
            request.AddQueryParameter("username", txtUserName.Text);
            request.AddQueryParameter("password", string.Concat(txtPassword.Text, txtUserName.Text).ToMD5Hash());
            request.AddQueryParameter("deviceid", deviceIdBuilder.ToString());

            var response = client.Get(request);

            dynamic jsonO = JsonConvert.DeserializeObject(response.Content);
            string rToken = jsonO.rToken;
            string bToken = jsonO.bToken;

            if (!string.IsNullOrEmpty(rToken) && !string.IsNullOrEmpty(bToken))
            {
                this.Hide();
                options = new RestClientOptions($"{sSolutionURL}/api/get_zkteco_attendance_device_list");
                client = new RestClient(options);

                RestSharp.RestRequest request2 = new RestSharp.RestRequest($"{sSolutionURL}/api/get_zkteco_attendance_device_list", RestSharp.Method.Get);
                request2.AddHeader("bToken", bToken);
                request2.AddHeader("rToken", rToken);

                var response2 = client.Get(request2);

                dynamic json1 = JsonConvert.DeserializeObject(response2.Content);
                List<DeviceRecord> deviceList = JsonConvert.DeserializeObject<List<DeviceRecord>>(json1.deviceList.ToString());

                if (deviceList.Count > 0)
                {
                    frmDeviceList _frmDeviceList = new frmDeviceList(deviceList);
                    _frmDeviceList.Show();
                }

            }
        }
    }
}