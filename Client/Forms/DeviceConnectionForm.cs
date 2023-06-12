using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Data.OleDb;
using Microsoft.Win32;
using RestSharp;
using Newtonsoft.Json;

namespace Client.Forms.PayRoll
{
    public partial class TADeviceIntegratorForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public TADeviceIntegratorForm(DeviceRecord deviceInfo)
        {
            InitializeComponent();

            this.ConnectedDeviceInfo = deviceInfo;

            btnUpload.Visible = true;
            btnUpload.Visible = true;

            Load += delegate
            {
                dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvUsers.MultiSelect = false;

                ContextMenuStrip contextMenuUsers = new ContextMenuStrip();
                var promote = new ToolStripMenuItem("Promote To Admin");
                promote.Click += new EventHandler(promote_Click);
                var demote = new ToolStripMenuItem("Demote From Admin");
                demote.Click += new EventHandler(demote_Click);
                contextMenuUsers.Items.Add(promote);
                contextMenuUsers.Items.Add(demote);

                dgvUsers.ContextMenuStrip = contextMenuUsers;

                // remove default [x] image for data DataGridViewImageColumn columns
                foreach (var column in dgvUsers.Columns)
                {
                    if (column is DataGridViewImageColumn)
                        (column as DataGridViewImageColumn).DefaultCellStyle.NullValue = null;
                }
            };

            Connect();
        }

        private void promote_Click(object sender, EventArgs e)
        {
            SetPrevilege(2);
        }

        private void demote_Click(object sender, EventArgs e)
        {
            SetPrevilege(0);
        }

        private void SetPrevilege(int iNewPrivilege)
        {
            if (dgvUsers.SelectedRows != null)
            {
                Cursor = Cursors.WaitCursor;

                string sUserID = string.Empty, sName = string.Empty, sTmpData = string.Empty, sPassword = string.Empty, sEnabled = string.Empty;
                int iPrivilege = 0;
                bool bEnabled = false;

                var _row = dgvUsers.SelectedRows[0];
                if (axCZKEM1.SSR_GetUserInfo(iMachineNumber, sUserID, out sName, out sPassword, out iPrivilege, out bEnabled))
                    axCZKEM1.SSR_SetUserInfo(iMachineNumber, sUserID, sName, sPassword, iNewPrivilege, bEnabled);

                _row.Cells["Admin"].Value = (iNewPrivilege == 2) ? global::Client.Properties.Resources.user : null;

                Cursor = Cursors.Default;
            }
        }

        public zkemkeeper.CZKEM axCZKEM1 = new zkemkeeper.CZKEM();

        private int idwErrorCode = 0;
        private bool bIsConnected = false;
        private DeviceRecord ConnectedDeviceInfo { get; set; }
        private int iMachineNumber = 1; //the serial number of the device.After connecting the device ,this value will be changed.

        //If your fingerprint(or your card) passes the verification,this event will be triggered
        private void axCZKEM1_OnAttTransactionEx(string sEnrollNumber, int iIsInValid, int iAttState, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond, int iWorkCode)
        {
            //var name_obj = DBHelper.Instance.db_query_scalar_as_object(WhichDatabase.CONFIG, string.Format("SELECT empmaster_name FROM empmaster WHERE id={0}", sEnrollNumber));
            string name_obj = "__name";
            //var username = (name_obj != DBNull.Value && name_obj != null) ? name_obj.ToString() : "*** ERROR ***";
            string username = "sysadmin";

            txtLog.Text += Environment.NewLine + string.Format("User Passes Verification... UserID:{0}, UserName: {1}, Time:{2}", sEnrollNumber, username, string.Format("{0}-{1}-{2} {3}:{4}:{5}", iYear, iMonth, iDay, iHour, iMinute, iSecond));
            if (username == "*** ERROR ***")
                txtLog.Text += Environment.NewLine + string.Format("-- FATAL ERROR: User with UserID {0} NOT FOUND! CONTACT YOUR ADMIN.", sEnrollNumber);
        }

        //When the dismantling machine or duress alarm occurs, trigger this event.
        private void axCZKEM1_OnAlarm(int iAlarmType, int iEnrollNumber, int iVerified)
        {
            txtLog.Text += Environment.NewLine + string.Format("Device Alarm Triggered... AlarmType:{0}, EnrollNumber:{1}, Verified:{2}", iAlarmType, iEnrollNumber, iVerified);
        }

        //After function GetRTLog() is called ,RealTime Events will be triggered. 
        //When you are using these two functions, it will request data from the device forwardly.
        private void rtTimer_Tick(object sender, EventArgs e)
        {
            if (axCZKEM1.ReadRTLog(iMachineNumber))
            {
                while (axCZKEM1.GetRTLog(iMachineNumber))
                {
                    ;
                }
            }
        }

        private void PullFaceTemplates()
        {
            string sUserID = string.Empty, sName = string.Empty, sPassword = string.Empty, sTmpData = string.Empty;
            bool bEnabled = false;
            int iFaceIndex = 50, iLength = 0, iTotalCount = 0, iGLCount = 0, iPrivilege = 0;

            iTotalCount = GetDeviceStatusHelper(2);
            txtLog.Text += Environment.NewLine + "----------- START Pull Face Templates -----------";

            var bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;

            bw.DoWork += delegate
            {
                axCZKEM1.ReadAllUserID(iMachineNumber);//read all the user information to the memory
                while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out sUserID, out sName, out sPassword, out iPrivilege, out bEnabled))//get all the users' information from the memory
                {
                    if (axCZKEM1.GetUserFaceStr(iMachineNumber, sUserID, iFaceIndex, ref sTmpData, ref iLength))//get the face templates from the memory
                    {
                        iGLCount++;
                        bw.ReportProgress(iGLCount);
                        UpdateListViewlvFace(sUserID, sName, iPrivilege, "Face", iFaceIndex, iLength, bEnabled.ToString());

                        string _update_qry = string.Format("UPDATE app_att_facetemplates SET tmpdata='{0}', length={1} WHERE userid={2}",
                            sTmpData, iLength, Convert.ToInt32(sUserID));

                        string _insert_qry = string.Format("INSERT INTO app_att_facetemplates (userid, faceindex, tmpdata, length) SELECT {0}, {1}, '{2}', {3}",
                            Convert.ToInt32(sUserID), iFaceIndex, sTmpData, iLength);

                        //DBHelper.Instance.db_query(WhichDatabase.CONFIG, string.Format(StringConstants.UPSERT_QRY, _update_qry, _insert_qry));
                    }
                }
            };

            bw.ProgressChanged += delegate(object senderp, ProgressChangedEventArgs ep)
            {
                toolStripProgressBar1.Value = ep.ProgressPercentage;
            };

            bw.RunWorkerCompleted += delegate(object senderp, RunWorkerCompletedEventArgs er)
            {
                if (er.Error != null)
                    txtLog.Text += Environment.NewLine + "----------- ERROR: " + er.Error.Message + " -----------";
                else
                    txtLog.Text += Environment.NewLine + "----------- END Pull Face Templates -----------";

                toolStripProgressBar1.Visible = false;
                axCZKEM1.EnableDevice(iMachineNumber, true);
            };

            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Maximum = iTotalCount;
            axCZKEM1.EnableDevice(iMachineNumber, false);
            bw.RunWorkerAsync();
        }

        private BackgroundWorker bw;

        private void PullFPTemplates()
        {
            string sUserID = string.Empty, sName = string.Empty, sPassword = string.Empty, sTmpData = string.Empty;
            int iUserID = 0;
            bool bEnabled = false;
            int iLength = 0, iTotalCount = 0, iGLCount = 0, iPrivilege = 0, iFlag = 0;

            iTotalCount = GetDeviceStatusHelper(3);
            txtLog.Text += Environment.NewLine + "----------- START Pull FP Templates -----------";

            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;

            bw.DoWork += delegate
            {
                axCZKEM1.ReadAllUserID(iMachineNumber);
                axCZKEM1.ReadAllTemplate(iMachineNumber);

                //if (ConnectedDeviceInfo.IsUserIDInt)
                {
                    while (axCZKEM1.GetAllUserInfo(iMachineNumber, ref iUserID, ref sName, ref sPassword, ref iPrivilege, ref bEnabled))
                    {
                        //PullFPTemplateHelper(axCZKEM1, iMachineNumber, iUserID, ref sTmpData, ref iLength, ref iGLCount, sName, sPassword, iPrivilege, bEnabled);

                        for (int idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
                        {
                            if (axCZKEM1.GetUserTmpStr(iMachineNumber, iUserID, idwFingerIndex, ref sTmpData, ref iLength))
                            {
                                iGLCount++;
                                bw.ReportProgress(iGLCount);
                                UpdateListViewlvFace(iUserID.ToString(), sName, iPrivilege, "FP", idwFingerIndex, iLength, bEnabled.ToString());

                                string _update_qry = string.Format("UPDATE app_att_facetemplates SET tmpdata='{0}', length={1} WHERE userid={2}",
                                    sTmpData, iLength, iUserID);

                                string _insert_qry = string.Format("INSERT INTO app_att_facetemplates (userid, faceindex, tmpdata, length) SELECT {0}, {1}, '{2}', {3}",
                                    iUserID, idwFingerIndex, sTmpData, iLength);

                                //DBHelper.Instance.db_query(WhichDatabase.CONFIG, string.Format(StringConstants.UPSERT_QRY, _update_qry, _insert_qry));
                            }
                        }
                    }
                }
                //else
                //{
                //    while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out sUserID, out sName, out sPassword, out iPrivilege, out bEnabled))
                //        PullFPTemplateHelper(axCZKEM1, iMachineNumber, sUserID, iFlag, ref sTmpData, ref iLength, ref iGLCount, sName, sPassword, iPrivilege, bEnabled);
                //}
            };

            bw.ProgressChanged += delegate(object senderp, ProgressChangedEventArgs ep)
            {
                toolStripProgressBar1.Value = ep.ProgressPercentage;
            };

            bw.RunWorkerCompleted += delegate(object senderp, RunWorkerCompletedEventArgs er)
            {
                if (er.Error != null)
                    txtLog.Text += Environment.NewLine + "----------- ERROR: " + er.Error.Message + " -----------";
                else
                    txtLog.Text += Environment.NewLine + "----------- END Pull FP Templates -----------";

                toolStripProgressBar1.Visible = false;
                axCZKEM1.EnableDevice(iMachineNumber, true);
            };

            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Maximum = iTotalCount;
            axCZKEM1.EnableDevice(iMachineNumber, false);
            bw.RunWorkerAsync();
        }

        private void PullFPTemplateHelper(zkemkeeper.CZKEM axCZKEM1, int iMachineNumber, int iUserID, ref string sTmpData, ref int iLength, ref int iGLCount, string sName, string sPassword, int iPrivilege, bool bEnabled)
        {
            for (int idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
            {
                if (axCZKEM1.GetUserTmpStr(iMachineNumber, iUserID, idwFingerIndex, ref sTmpData, ref iLength))
                {
                    iGLCount++;
                    bw.ReportProgress(iGLCount);
                    UpdateListViewlvFace(iUserID.ToString(), sName, iPrivilege, "FP", idwFingerIndex, iLength, bEnabled.ToString());

                    string _update_qry = string.Format("UPDATE app_att_facetemplates SET tmpdata='{0}', length={1} WHERE userid={2}",
                        sTmpData, iLength, iUserID);

                    string _insert_qry = string.Format("INSERT INTO app_att_facetemplates (userid, faceindex, tmpdata, length) SELECT {0}, {1}, '{2}', {3}",
                        iUserID, idwFingerIndex, sTmpData, iLength);

                    //DBHelper.Instance.db_query(WhichDatabase.CONFIG, string.Format(StringConstants.UPSERT_QRY, _update_qry, _insert_qry));
                }
            }
        }

        private void PullFPTemplateHelper(zkemkeeper.CZKEM axCZKEM1, int iMachineNumber, string sUserID, int iFlag, ref string sTmpData, ref int iLength, ref int iGLCount, string sName, string sPassword, int iPrivilege, bool bEnabled)
        {
            for (int idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
            {
                if (axCZKEM1.GetUserTmpExStr(iMachineNumber, sUserID, idwFingerIndex, out iFlag, out sTmpData, out iLength))
                {
                    iGLCount++;
                    //bw.ReportProgress(iGLCount);
                    UpdateListViewlvFace(sUserID, sName, iPrivilege, "FP", idwFingerIndex, iLength, bEnabled.ToString());

                    string _update_qry = string.Format("UPDATE app_att_facetemplates SET tmpdata='{0}', length={1} WHERE userid={2}",
                        sTmpData, iLength, sUserID);

                    string _insert_qry = string.Format("INSERT INTO app_att_facetemplates (userid, faceindex, tmpdata, length) SELECT {0}, {1}, '{2}', {3}",
                        sUserID, idwFingerIndex, sTmpData, iLength);

                    //DBHelper.Instance.db_query(WhichDatabase.CONFIG, string.Format(StringConstants.UPSERT_QRY, _update_qry, _insert_qry));
                }
            }
        }

        //Upload the 9.0 or 10.0 fingerprint arithmetic templates to the device(in strings) in batches.
        //Only TFT screen devices with firmware version Ver 6.60 version later support function "SetUserTmpExStr" and "SetUserTmpEx".
        //While you are using 9.0 fingerprint arithmetic and your device's firmware version is under ver6.60,you should use the functions "SSR_SetUserTmp" or 
        //"SSR_SetUserTmpStr" instead of "SetUserTmpExStr" or "SetUserTmpEx" in order to upload the fingerprint templates.
        //private void FPBatchUpdate()
        //{
        //    string sdwEnrollNumber = "";
        //    string sName = "";
        //    int idwFingerIndex = 0;
        //    string sTmpData = "";
        //    int iPrivilege = 0;
        //    string sPassword = "";
        //    string sEnabled = "";
        //    bool bEnabled = false;
        //    int iFlag = 1;

        //    int iUpdateFlag = 1;

        //    Cursor = Cursors.WaitCursor;
        //    axCZKEM1.EnableDevice(iMachineNumber, false);
        //    if (axCZKEM1.BeginBatchUpdate(iMachineNumber, iUpdateFlag))//create memory space for batching data
        //    {
        //        string sLastEnrollNumber = "";//the former enrollnumber you have upload(define original value as 0)
        //        for (int i = 0; i < lvDownload.Items.Count; i++)
        //        {
        //            sdwEnrollNumber = lvDownload.Items[i].SubItems[0].Text;
        //            sName = lvDownload.Items[i].SubItems[1].Text;
        //            idwFingerIndex = Convert.ToInt32(lvDownload.Items[i].SubItems[2].Text);
        //            sTmpData = lvDownload.Items[i].SubItems[3].Text;
        //            iPrivilege = Convert.ToInt32(lvDownload.Items[i].SubItems[4].Text);
        //            sPassword = lvDownload.Items[i].SubItems[5].Text;
        //            sEnabled = lvDownload.Items[i].SubItems[6].Text;
        //            iFlag = Convert.ToInt32(lvDownload.Items[i].SubItems[7].Text);
        //            bEnabled = (sEnabled == "true") ? true : false;

        //            if (sdwEnrollNumber != sLastEnrollNumber)//identify whether the user information(except fingerprint templates) has been uploaded
        //            {
        //                if (axCZKEM1.SSR_GetUserInfo(iMachineNumber, sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))//upload user information to the memory
        //                    axCZKEM1.SetUserTmpExStr(iMachineNumber, sdwEnrollNumber, idwFingerIndex, iFlag, sTmpData);//upload templates information to the memory
        //                else
        //                {
        //                    axCZKEM1.GetLastError(ref idwErrorCode);
        //                    MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
        //                    Cursor = Cursors.Default;
        //                    axCZKEM1.EnableDevice(iMachineNumber, true);
        //                    return;
        //                }
        //            }
        //            else//the current fingerprint and the former one belongs the same user,that is ,one user has more than one template
        //                axCZKEM1.SetUserTmpExStr(iMachineNumber, sdwEnrollNumber, idwFingerIndex, iFlag, sTmpData);

        //            sLastEnrollNumber = sdwEnrollNumber;//change the value of iLastEnrollNumber dynamicly
        //        }
        //    }
        //    axCZKEM1.BatchUpdate(iMachineNumber);//upload all the information in the memory
        //    axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
        //    Cursor = Cursors.Default;
        //    axCZKEM1.EnableDevice(iMachineNumber, true);
        //    MessageBox.Show("Successfully upload fingerprint templates in batches , " + "total:" + lvDownload.Items.Count.ToString(), "Success");
        //}

        //Upload users' face template(in strings)(For TFT screen IFace series devices only)
        //Uploading the face templates in batches is not supported temporarily.
        private void PushFaceTemplates()
        {
            string sUserID = string.Empty, sName = string.Empty, sTmpData = string.Empty, sPassword = string.Empty, sEnabled = string.Empty;
            int iFaceIndex = 0, iLength = 0, iPrivilege = 0, iCount = 0, iTotalCount = 0;
            bool bEnabled = false;

            iTotalCount = GetDeviceStatusHelper(2);
            txtLog.Text += Environment.NewLine + "----------- START Push Face Templates -----------";

            var bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;

            bw.DoWork += delegate
            {
                //using (var con = DBHelper.Instance.GetConnection(WhichDatabase.CONFIG))
                {
                    //con.Open();
                    //var cmd = DBHelper.Instance.GetCommand(con, "SELECT A.* FROM app_att_facetemplates A, empmaster B WHERE A.userid=B.id AND B.sys_cancelled<>TRUE AND B.active=TRUE ORDER BY A.userid;");

                    //var reader = cmd.ExecuteReader();
                    //if (reader.HasRows)
                    //{
                    //    while (reader.Read())
                    //    {
                    //        sUserID = reader.GetInt32(1).ToString();
                    //        iFaceIndex = reader.GetInt32(2);
                    //        sTmpData = reader.GetString(3);
                    //        iLength = reader.GetInt32(4);
                    //        bEnabled = true;

                    //        if (axCZKEM1.SSR_GetUserInfo(iMachineNumber, sUserID, out sName, out sPassword, out iPrivilege, out bEnabled))//face templates are part of users' information
                    //        {
                    //            iCount++;
                    //            bw.ReportProgress(iCount);
                    //            UpdateListViewlvFace(sUserID, sName, iPrivilege, "Face", iFaceIndex, iLength, bEnabled.ToString());
                    //            axCZKEM1.SetUserFaceStr(iMachineNumber, sUserID, iFaceIndex, sTmpData, iLength); //upload face templates information to the device
                    //        }
                    //        else
                    //        {
                    //            axCZKEM1.GetLastError(ref idwErrorCode);
                    //            MessageBox.Show(string.Format("Operation failed for UserID {0}, ErrorCode={1}", sUserID, idwErrorCode.ToString()), "PHX+ Error");
                    //            continue;
                    //        }
                    //    }
                    //}
                    //else
                    //    MessageBox.Show("No Face Templates found.", "PHX+ Message");

                    //reader.Close();
                }
            };

            bw.ProgressChanged += bw_ProgressChanged;

            bw.RunWorkerCompleted += delegate (object senderp, RunWorkerCompletedEventArgs er)
            {
                if (er.Error != null)
                    txtLog.Text += Environment.NewLine + "----------- ERROR: " + er.Error.Message + " -----------";
                else
                    txtLog.Text += Environment.NewLine + "----------- END Push Face Templates -----------";

                if (iCount == iTotalCount)
                    toolStripProgressBar1.Visible = false;

                axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                axCZKEM1.EnableDevice(iMachineNumber, true);
            };

            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Maximum = iTotalCount;
            axCZKEM1.EnableDevice(iMachineNumber, false);
            bw.RunWorkerAsync();
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs ep)
        {
            if (ep.ProgressPercentage <= toolStripProgressBar1.Maximum)
                toolStripProgressBar1.Value = ep.ProgressPercentage;
        }

        //Connect
        private void Connect()
        {
            Cursor = Cursors.WaitCursor;

            var deviceInfo = this.ConnectedDeviceInfo;

            //axCZKEM1.SetCommPassword(Convert.ToInt32(deviceInfo.CommKey));
            axCZKEM1.SetCommPassword(0);
            bIsConnected = axCZKEM1.Connect_Net(deviceInfo.Ip, deviceInfo.Port);

            if (bIsConnected == true)
            {
                iMachineNumber = 1; //In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                ConnectedDeviceInfo = deviceInfo;
                btnColor.Image = global::Client.Properties.Resources.circle_green;
                btnColor.ToolTipText = "Connected";
                lblDeviceName.Text = string.Format("{0} ({1})", ConnectedDeviceInfo.DeviceName, ConnectedDeviceInfo.Ip);
                tabControl2.Enabled = true;
                btnUpload.Enabled = true;
                btnDownload.Enabled = true;

                this.RefreshDashboardInfo();
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                MessageBox.Show("Unable to connect the device, ErrorCode=" + idwErrorCode.ToString(), "Error");
            }

            Cursor = Cursors.Default;
        }

        private void RefreshDashboardInfo()
        {
            lblUserCount.Text = GetDeviceStatusHelper(2).ToString();
            lblAttLogs.Text = GetDeviceStatusHelper(6).ToString();
            lblIPAddress.Text = ConnectedDeviceInfo.Ip;
            lblMacAddress.Text = GetMacAddress();
        }

        // Disconnect
        //private void item2_Click(object sender, EventArgs e)
        //{
        //    var deviceInfo = (sender as ToolStripMenuItem).Tag as DeviceInfo;
        //    if (deviceInfo.ID == ConnectedDeviceInfo.ID)
        //    {
        //        Cursor = Cursors.WaitCursor;
        //        axCZKEM1.Disconnect();
        //        btnConnectStatus.Image = global::Client.Properties.Resources.not_connected_icon;
        //        btnConnectStatus.ToolTipText = "Disconnected";
        //        btnColor.Image = global::Client.Properties.Resources.circle_red;
        //        btnColor.ToolTipText = "Disconnected";
        //        lblDeviceName.Text = "Device Disconnected";
        //        tabControl2.Enabled = false;
        //        btnUpload.Enabled = false;
        //        btnDownload.Enabled = false;
        //        btnClear.Enabled = true;
        //        ConnectedDeviceInfo = null;

        //        lblUserCount.Text = "0";
        //        lblAttLogs.Text = "0";
        //        lblIPAddress.Text = "0.0.0.0";

        //        bIsConnected = false;
        //        Cursor = Cursors.Default;
        //    }
        //}

        protected override void OnClosing(CancelEventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            axCZKEM1.Disconnect();
            bIsConnected = false;
            ConnectedDeviceInfo = null;
            Cursor = Cursors.Default;
            base.OnClosing(e);
        }

        private string GetMacAddress()
        {
            string _mac = null;

            axCZKEM1.GetDeviceMAC(iMachineNumber, ref _mac);
            ConnectedDeviceInfo.MacAddress = _mac;

            return _mac;
        }

        private int GetDeviceStatusHelper(int status)
        {
            //1 Administrator Count
            //2 Register users Count
            //3 Fingerprint template Count
            //4 Password Count
            //5 The record number of times which administrator perform
            //management.
            //6 Attendance records number of times.
            //7 Fingerprint capacity.
            //8 User’s capacity
            //9 Recording capacity
            int iCnt = 0;

            axCZKEM1.EnableDevice(iMachineNumber, false);

            if (!axCZKEM1.GetDeviceStatus(iMachineNumber, status, ref iCnt)) //Here we use the function "GetDeviceStatus" to get the record's count.The parameter "Status" is 6.
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                axCZKEM1.EnableDevice(iMachineNumber, true);
                MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "PHX+ Error");
            }

            axCZKEM1.EnableDevice(iMachineNumber, true);

            return iCnt;
        }

        private void downloadAttendanceLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedTab = tabpAttLogs;

            int idwYear = 0, idwMonth = 0, idwDay = 0, idwHour = 0, idwMinute = 0, idwSecond = 0, iGLCount = 0, iTotalCount = 0, idwVerifyMode = 0, idwInOutMode = 0, idwWorkcode = 0;
            StringBuilder _sb = new StringBuilder();
            _sb.Append("INSERT INTO app_att_deviceattlogs (machineno, userid, verifymode, inoutmode, punched_at, workcode, sys_submitter_uid, sys_submitted_ts) VALUES ");
            int uid = 999999999;
            string _sql_tmpl = "('{0}',{1},{2},{3},'{4}',{5},{6},now())";

            iTotalCount = GetDeviceStatusHelper(6);

            var bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;

            bw.DoWork += delegate
            {
                if (axCZKEM1.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
                {
                    //if (!this.ConnectedDeviceInfo.IsUserIDInt)
                    {
                        string sdwEnrollNumber = string.Empty;
                        while (axCZKEM1.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode,
                                    out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                        {
                            bw.ReportProgress(iGLCount);
                            UpdateListViewlvLogs(iGLCount, sdwEnrollNumber, idwVerifyMode, idwInOutMode, idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond, idwWorkcode);

                            if (iGLCount == 0)
                                _sb.Append(string.Format(_sql_tmpl, ConnectedDeviceInfo.MacAddress, sdwEnrollNumber, idwVerifyMode, idwInOutMode, string.Format("{0}-{1}-{2} {3}:{4}:{5}", idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond), idwWorkcode, uid));
                            else
                                _sb.Append(',').Append(string.Format(_sql_tmpl, ConnectedDeviceInfo.MacAddress, sdwEnrollNumber, idwVerifyMode, idwInOutMode, string.Format("{0}-{1}-{2} {3}:{4}:{5}", idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond), idwWorkcode, uid));

                            iGLCount++;
                        }
                    }
                    //else
                    //{
                    //    int sdwEnrollNumber = 0;
                    //    int _machineNumber = 0;
                    //    int _enrollMachineNumber = 0;
                    //    while (axCZKEM1.GetGeneralLogData(iMachineNumber, ref _machineNumber, ref sdwEnrollNumber, ref _enrollMachineNumber, ref idwVerifyMode,
                    //                ref idwInOutMode, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute))//get records from the memory
                    //    {
                    //        bw.ReportProgress(iGLCount);
                    //        UpdateListViewlvLogs(iGLCount, sdwEnrollNumber.ToString(), idwVerifyMode, idwInOutMode, idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond, idwWorkcode);

                    //        if (iGLCount == 0)
                    //            _sb.Append(string.Format(_sql_tmpl, ConnectedDeviceInfo.MacAddress, sdwEnrollNumber, idwVerifyMode, idwInOutMode, string.Format("{0}-{1}-{2} {3}:{4}:{5}", idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond), idwWorkcode, uid));
                    //        else
                    //            _sb.Append(',').Append(string.Format(_sql_tmpl, ConnectedDeviceInfo.MacAddress, sdwEnrollNumber, idwVerifyMode, idwInOutMode, string.Format("{0}-{1}-{2} {3}:{4}:{5}", idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond), idwWorkcode, uid));

                    //        iGLCount++;
                    //    }
                    //}
                }
                else
                {
                    axCZKEM1.GetLastError(ref idwErrorCode);

                    if (idwErrorCode != 0)
                        MessageBox.Show("Reading data from the terminal failed,ErrorCode: " + idwErrorCode.ToString(), "PHX+ Error");
                    else
                        MessageBox.Show("No Attendence data found to download!", "PHX+ Message");
                }
            };

            bw.ProgressChanged += delegate(object senderp, ProgressChangedEventArgs ep) 
            {
                toolStripProgressBar1.Value = ep.ProgressPercentage;
            };

            bw.RunWorkerCompleted += delegate
            {
                if (iTotalCount == iGLCount && iTotalCount > 0)
                {
                    //DBHelper.Instance.ExecuteNonQuery(WhichDatabase.CONFIG, _sb.ToString());

                    if (axCZKEM1.clearClearGLog(iMachineNumber))
                        axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                    else
                    {
                        axCZKEM1.GetLastError(ref idwErrorCode);
                        MessageBox.Show("Operation failed, ErrorCode=" + idwErrorCode.ToString(), "PHX+ Error");
                    }
                }

                toolStripProgressBar1.Visible = false;
                axCZKEM1.EnableDevice(iMachineNumber, true);
            };

            lvLogs.Items.Clear();
            axCZKEM1.EnableDevice(iMachineNumber, false);
            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Maximum = iTotalCount;
            bw.RunWorkerAsync();
        }

        delegate void UpdateListViewlvLogsCallback(int idx, string sUserID, int idwVerifyMode, int idwInOutMode, int idwYear, int idwMonth, int idwDay, int idwHour, int idwMinute, int idwSecond, int idwWorkcode);
        private void UpdateListViewlvLogs(int idx, string sUserID, int idwVerifyMode, int idwInOutMode, int idwYear, int idwMonth, int idwDay, int idwHour, int idwMinute, int idwSecond, int idwWorkcode)
        {
            if (lvLogs.InvokeRequired)
            {
                UpdateListViewlvLogsCallback d = new UpdateListViewlvLogsCallback(UpdateListViewlvLogs);
                this.Invoke(d, new object[] { idx, sUserID, idwVerifyMode, idwInOutMode, idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond, idwWorkcode });
            }
            else
            {
                lvLogs.Items.Add(idx.ToString());
                lvLogs.Items[idx].SubItems.Add(sUserID);
                lvLogs.Items[idx].SubItems.Add(idwVerifyMode.ToString());
                lvLogs.Items[idx].SubItems.Add(idwInOutMode.ToString());
                lvLogs.Items[idx].SubItems.Add(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                lvLogs.Items[idx].SubItems.Add(idwWorkcode.ToString());
            }
        }

        delegate void UpdateListViewlvFaceCallback(string sUserID, string sName, int iPrivilege, string faceOrFP, int iFaceFPIdx, int iLength, string sEnabled);
        private void UpdateListViewlvFace(string sUserID, string sName, int iPrivilege, string faceOrFP, int iFaceFPIdx, int iLength, string sEnabled)
        {
            if (txtLog.InvokeRequired)
            {
                UpdateListViewlvFaceCallback d = new UpdateListViewlvFaceCallback(UpdateListViewlvFace);
                this.Invoke(d, new object[] { sUserID, sName, iPrivilege, faceOrFP, iFaceFPIdx, iLength, sEnabled });
            }
            else
            {
                txtLog.Text += Environment.NewLine + string.Format("{0} {1}", sUserID, sName);
            }
        }

        private void pushUserInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            iMachineNumber = 1;
            Cursor = Cursors.WaitCursor;

            txtLog.Text += Environment.NewLine + "----------- START Push User Info -----------";
            
            int sys_location_id = ConnectedDeviceInfo.LocationId;

            MDIContainerForm __mdiContainerForm = this.DockPanel.Parent as MDIContainerForm;
            var client = new RestClient($"{__mdiContainerForm.SolutionURL}");

            RestSharp.RestRequest request2 = new RestSharp.RestRequest($"api/get_employees_list?eb_loc_id={ConnectedDeviceInfo.LocationId}", Method.GET);
            request2.AddHeader("bToken", __mdiContainerForm.BToken);
            request2.AddHeader("rToken", __mdiContainerForm.RToken);

            var response2 = client.Get(request2);

            dynamic json1 = JsonConvert.DeserializeObject(response2.Content);
            List<UserInfo> userList = JsonConvert.DeserializeObject<List<UserInfo>>(json1.employees.ToString());

            if (userList.Count > 0)
            {
                this.dataGridView1.DataSource = userList;
            }
            
            axCZKEM1.EnableDevice(iMachineNumber, false);
            if (axCZKEM1.BeginBatchUpdate(iMachineNumber, 1))
            {
                foreach (UserInfo _row in userList)
                {
                    bool result = false;
                    result = axCZKEM1.SSR_SetUserInfo(iMachineNumber, _row.PunchId1.Trim(), _row.Name.Trim(), _row.PunchId2.Trim(), 0, true);

                    if (result)
                        txtLog.Text += Environment.NewLine + _row.Name + " pushed to Device.";
                    else
                    {
                        axCZKEM1.GetLastError(ref idwErrorCode);
                        MessageBox.Show("Operation failed, ErrorCode=" + idwErrorCode.ToString(), "Error");
                        Cursor = Cursors.Default;
                        axCZKEM1.EnableDevice(iMachineNumber, true);
                        return;
                    }
                }
            }

            axCZKEM1.BatchUpdate(iMachineNumber);
            axCZKEM1.RefreshData(iMachineNumber);

            txtLog.Text += Environment.NewLine + "----------- END Push User Info -----------";
            axCZKEM1.EnableDevice(iMachineNumber, true);
            Cursor = Cursors.Default;
        }

        private void pushFingerPrintTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //FPBatchUpdate();
        }

        private void pushFaceTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PushFaceTemplates();
        }

        private void downloadFingerPrintTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            PullFPTemplates();
            Cursor = Cursors.Default;
        }

        private void downloadFaceTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PullFaceTemplates();
        }

        private void showUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedTab = tabpUsers;
            dgvUsers.Rows.Clear();

            int iTotalCount = 0, iCount = 0;

            iTotalCount = GetDeviceStatusHelper(2);

            var bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;

            int iUserID = 0;

            bw.DoWork += delegate
            {
                string sUserID = string.Empty, sName = string.Empty, sPassword = string.Empty, sTmpData = string.Empty, sCardNumber = string.Empty;
                bool bEnabled = true;
                int iPrivilege = 0, iFaceIndex = 0, iLength = 0, idwFingerIndex = 0, iFlag = 0;

                axCZKEM1.EnableDevice(iMachineNumber, false);
                axCZKEM1.ReadAllUserID(iMachineNumber);

                //if (this.ConnectedDeviceInfo.IsUserIDInt)
                {
                    while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out sUserID, out sName, out sPassword, out iPrivilege, out bEnabled))
                    {
                        iCount++;
                        bw.ReportProgress(iCount);
                        Image imgFP = (axCZKEM1.SSR_GetUserTmpStr(iMachineNumber, sUserID, idwFingerIndex, out sTmpData, out iLength)) ? global::Client.Properties.Resources.finger : null;
                        Image imgFace = (axCZKEM1.GetUserFaceStr(iMachineNumber, sUserID, iFaceIndex, ref sTmpData, ref iLength)) ? global::Client.Properties.Resources.face_plain_24 : null;
                        axCZKEM1.GetStrCardNumber(out sCardNumber);

                        object[] _objarr = new object[] 
                                            { 
                                                sUserID, 
                                                sName, 
                                                imgFP,
                                                imgFace,
                                                (sCardNumber != "0") ? global::Client.Properties.Resources.card : null,
                                                !string.IsNullOrEmpty(sPassword) ? global::Client.Properties.Resources.key : null,
                                                (iPrivilege == 2) ? global::Client.Properties.Resources.user : null
                                            };

                        //axCZKEM1.GetLastError(ref idwErrorCode);
                        //MessageBox.Show("Operation failed, ErrorCode=" + idwErrorCode.ToString(), "Error");

                        UpdateDataGridViewUsers(_objarr);
                    }
                }
                //else
                //{
                //    while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out sUserID, out sName, out sPassword, out iPrivilege, out bEnabled))
                //    {
                //        iCount++;
                //        bw.ReportProgress(iCount);
                //        Image imgFP = (axCZKEM1.GetUserTmpExStr(iMachineNumber, sUserID, idwFingerIndex, out iFlag, out sTmpData, out iLength)) ? global::Client.Properties.Resources.finger : null;
                //        Image imgFace = (axCZKEM1.GetUserFaceStr(iMachineNumber, sUserID, iFaceIndex, ref sTmpData, ref iLength)) ? global::Client.Properties.Resources.face_plain_24 : null;
                //        axCZKEM1.GetStrCardNumber(out sCardNumber);

                //        object[] _objarr = new object[] 
                //                            { 
                //                                iUserID, 
                //                                sName, 
                //                                imgFP,
                //                                imgFace,
                //                                (sCardNumber != "0") ? global::Client.Properties.Resources.card : null,
                //                                !string.IsNullOrEmpty(sPassword) ? global::Client.Properties.Resources.key : null,
                //                                (iPrivilege == 2) ? global::Client.Properties.Resources.user : null
                //                            };

                //        UpdateDataGridViewUsers(_objarr);
                //    }
                //}
            };

            bw.ProgressChanged += bw_ProgressChanged;

            bw.RunWorkerCompleted += delegate(object senderp, RunWorkerCompletedEventArgs er)
            {
                if (iCount == iTotalCount)
                    toolStripProgressBar1.Visible = false;

                axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                axCZKEM1.EnableDevice(iMachineNumber, true);
            };

            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Maximum = iTotalCount;
            axCZKEM1.EnableDevice(iMachineNumber, false);
            bw.RunWorkerAsync();
        }

        delegate void UpdateDataGridViewUsersCallback(object[] _objarr);
        private void UpdateDataGridViewUsers(object[] _objarr)
        {
            if (lvLogs.InvokeRequired)
            {
                UpdateDataGridViewUsersCallback d = new UpdateDataGridViewUsersCallback(UpdateDataGridViewUsers);
                this.Invoke(d, new object[] { _objarr });
            }
            else
            {
                dgvUsers.Rows.Add(_objarr);
            }
        }

        private void btnDevice_Click(object sender, EventArgs e)
        {

        }

        private void lblIPAddress_Click(object sender, EventArgs e)
        {

        }

        private void lblUserCount_Click(object sender, EventArgs e)
        {

        }

        private void lblMacAddress_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.RefreshDashboardInfo();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {

        }
    }

    internal class DeviceInfo
    {
        public int ID { get; set; }
        public string DeviceName { get; set; }
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }
        public int Port { get; set; }
        public int CommKey { get; set; }
        public bool IsUserIDInt { get; set; }

        public DeviceInfo(int id, string deviceName, string iPAddress, int port, int commKey, bool useridint)
        {
            this.ID = id;
            this.DeviceName = deviceName;
            this.IPAddress = iPAddress;
            this.Port = port;
            this.CommKey = commKey;
            this.IsUserIDInt = useridint;
        }
    }

    internal class UserInfo
    {
        public int Id { get; set; }
        public string Xid { get; set; }

        public string Name { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string PunchId1 { get; set; }

        public string PunchId2 { get; set; }

        public string ShiftStart { get; set; }

        public string ShiftEnd { get; set; }

        public UserInfo (int id, string xid, string name, string designation, string department, string punchId1, string punchId2, string shiftStart, string shiftEnd)
        {
            this.Id = id;
            this.Xid = xid;
            this.Name = name;
            this.Designation = designation;
            this.Department = department;
            this.PunchId1 = punchId1;
            this.PunchId2 = punchId2;
            this.ShiftStart = shiftStart;
            this.ShiftEnd = shiftEnd;
        }
    }
} 
