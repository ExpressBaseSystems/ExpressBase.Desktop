using DeviceId;
using ExpressBase.Desktop.Extensions;
using RestSharp;
using RestSharp.Authenticators;

namespace ExpressBase.Desktop
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

                if (!sSolutionURL.EndsWith("/api/auth"))
                    sSolutionURL = $"{sSolutionURL}/api/auth";
            }

            var deviceIdBuilder = new DeviceId.DeviceIdBuilder();
            deviceIdBuilder.AddMacAddress(false);
            deviceIdBuilder.AddMachineName();

            var options = new RestClientOptions(sSolutionURL);
            var client = new RestClient(options);

            RestSharp.RestRequest request = new RestSharp.RestRequest(sSolutionURL, RestSharp.Method.Get);
            request.AddQueryParameter("username", txtUserName.Text);
            request.AddQueryParameter("password", string.Concat(txtUserName.Text, txtPassword.Text).ToMD5Hash());
            request.AddQueryParameter("deviceid", deviceIdBuilder.ToString());

            var response = client.Get(request);

            //if (response.IsCompletedSuccessfully)
            {

            }
        }
    }
}