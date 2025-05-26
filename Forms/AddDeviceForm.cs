using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.PayRoll
{
    public partial class AddDeviceForm : Form
    {
        private int EditID { get; set; }
        private TADeviceIntegratorForm EnrollUsers { get; set; }

        public AddDeviceForm(TADeviceIntegratorForm enrollusers_form)
        {
            InitializeComponent();
            this.EnrollUsers = enrollusers_form;
        }

        public AddDeviceForm(TADeviceIntegratorForm enrollusers_form, int id)
            : this(enrollusers_form)
        {
            this.EditID = id;

            //var dt = DBHelper.Instance.db_query(WhichDatabase.CONFIG, string.Format("SELECT * FROM app_att_devices WHERE id={0}", this.EditID));
            //txtDeviceName.Text = dt.Rows[0]["devicename"].ToString();
            //txtIPAddress.Text = dt.Rows[0]["ipaddress"].ToString();
            //numPort.Value = Convert.ToInt32(dt.Rows[0]["port"]);
            //numCommKey.Value = Convert.ToInt32(dt.Rows[0]["commkey"]);
            //chkUserIDisInteger.Checked = (dt.Rows[0]["useridint"] != DBNull.Value) ? Convert.ToBoolean(dt.Rows[0]["useridint"]) : false;

            btnAdd.Text = "Modify Device";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDeviceName.Text))
            {
                toolStripStatusLabel1.Text = "Device Name is mandatory!";
                return;
            }

            if (string.IsNullOrEmpty(txtIPAddress.Text))
            {
                toolStripStatusLabel1.Text = "IP Address is mandatory!";
                return;
            }

            string _sql = null;
            if (btnAdd.Text.Contains("Add"))
            {
                _sql = string.Format("INSERT INTO app_att_devices (devicename, ipaddress, port, commkey, useridint) VALUES ('{0}', '{1}', {2}, '{3}', {4});",
                    txtDeviceName.Text.Trim(), txtIPAddress.Text.Trim(), numPort.Value, numCommKey.Value, chkUserIDisInteger.Checked);
            }
            else if (btnAdd.Text.Contains("Modify"))
            {
                _sql = string.Format("UPDATE app_att_devices SET devicename='{0}', ipaddress='{1}', port={2}, commkey='{3}', useridint={4} WHERE id={5};",
                    txtDeviceName.Text.Trim(), txtIPAddress.Text.Trim(), numPort.Value, numCommKey.Value, chkUserIDisInteger.Checked, this.EditID);
            }

            //DBHelper.Instance.ExecuteNonQuery(WhichDatabase.CONFIG, _sql);
            //EnrollUsers.UpdateDeviceMenu();
            this.Close();
        }
    }
}
