using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExpressBase.Desktop
{
    public partial class frmDeviceList : Form
    {
        public List<DeviceRecord> _devices { get; set; }

        public frmDeviceList(List<DeviceRecord> devices)
        {
            InitializeComponent();
            _devices = devices;

            dataGridView1.Columns.Add("DeviceName", "Device Name");
            dataGridView1.Columns.Add("Ip", "IP Address");
            dataGridView1.DataSource = _devices;
        }
    }

    public class DeviceRecord
    {
        public int Id {  get; set;}

        public string? DeviceName { get; set;}

        public string? Ip { get; set; }

        public int Port { get; set; }

        public int LocationId { get; set; }

        public string? LocationShortName { get; set; }

        public string LastSyncTs { get; set; }
    }
}
