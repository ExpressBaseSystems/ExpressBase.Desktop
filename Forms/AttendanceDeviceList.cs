using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Client.Forms.PayRoll
{
    public partial class AttendanceDeviceList : DockContent
    {
        private List<DeviceRecord> __devices = null;
        public List<DeviceRecord> Devices 
        {
            get
            {
                return __devices;
            }

            set
            {
                __devices = value;
                dataGridView1.DataSource = __devices;
            }
        }

        public AttendanceDeviceList()
        {
            InitializeComponent();

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                btnConnect.Tag = dataGridView1.SelectedRows[0].DataBoundItem;
                btnConnect.Text = $"Connect to {dataGridView1.SelectedRows[0].Cells["Ip"].Value.ToString()}";
            }
        }

        private int idwErrorCode = 0;
        private bool bIsConnected = false;
        private DeviceRecord ConnectedDeviceInfo { get; set; }

        public zkemkeeper.CZKEM axCZKEM1 = new zkemkeeper.CZKEM();

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            var deviceInfo = (sender as Button).Tag as DeviceRecord;

            TADeviceIntegratorForm _enroll = new TADeviceIntegratorForm(deviceInfo);
            _enroll.Show(DockPanel);

            Cursor = Cursors.Default;
        }
    }
}
