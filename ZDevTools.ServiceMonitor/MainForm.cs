using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceStack.Redis;
using Newtonsoft.Json;


namespace ZDevTools.ServiceMonitor
{
    public partial class MainForm : Form
    {
        static RedisManagerPool redisManagerPool { get; } = new RedisManagerPool(Properties.Settings.Default.RedisServer);

        public MainForm()
        {
            InitializeComponent();

            this.Text = Properties.Settings.Default.ServiceMonitorTitle;

            niMain.Text = this.Text;

            refreshAllReports();

            redisManagerPool.CreatePubSubServer(RedisKeys.ServiceReports, (channel, message) =>
            {
                using (var client = redisManagerPool.GetClient())
                {
                    var newReport = JsonConvert.DeserializeObject<ServiceReport>(client.GetValueFromHash(RedisKeys.ServiceReports, message));
                    var reports = dgvMain.DataSource as List<ServiceReport>;
                    int index = reports.FindIndex(sr => sr.ServiceName == newReport.ServiceName);

                    if (this.IsHandleCreated)
                        Invoke(new Action(() =>
                        {
                            if (index > -1) //找到了这个服务，从reports中删除，然后再添加为第一条
                            {
                                reports.RemoveAt(index);
                                reports.Insert(0, newReport);
                                dgvMain.Refresh(); //强制刷新显示状态
                                reportIfHasError(newReport);
                            }
                            else //没找到这个服务，那么说明某些服务名称已更换，或者新增了服务，那么刷新所有的服务状态
                            {
                                refreshAllReports();
                            }
                        }));
                }
            }).Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
#if !DEBUG
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Visible = false;
            }
#endif
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void niMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = !this.Visible;
            if (Visible)
            {
                this.WindowState = FormWindowState.Normal;
                this.Focus();
            }
        }

        private void dgvMain_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;

            if (dgvMain.Columns[e.ColumnIndex].Name == "ServiceStatusColumn")
            {
                e.PaintBackground(e.CellBounds, true);

                if (!(bool)e.Value) //服务正常
                {
                    if ((DateTime.Now - ((DateTime)dgvMain.Rows[e.RowIndex].Cells["ServiceLastedExecuteTime"].Value)).Days > 1)
                        e.Graphics.DrawString("长期未更新", e.CellStyle.Font, Brushes.Orange, e.CellBounds, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
                    else
                        e.Graphics.DrawString("正常", e.CellStyle.Font, Brushes.Green, e.CellBounds, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });
                }
                else
                    e.Graphics.DrawString("异常", e.CellStyle.Font, Brushes.Red, e.CellBounds, new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center });

                e.Handled = true;
            }
        }

        void reportIfHasError(ServiceReport report)
        {
            if (report.HasError && (DateTime.Now - report.UpdateTime).TotalDays < 3)
                niMain.ShowBalloonTip(120, report.ServiceName + "异常", report.Message, ToolTipIcon.Error);
        }

        void refreshAllReports()
        {
            try
            {
                using (var client = redisManagerPool.GetClient())
                {
                    var dic = client.GetAllEntriesFromHash(RedisKeys.ServiceReports);
                    List<ServiceReport> reports = new List<ServiceReport>();

                    foreach (var json in dic.Values)
                    {
                        var report = JsonConvert.DeserializeObject<ServiceReport>(json);

                        reports.Add(report);

                        reportIfHasError(report);
                    }

                    dgvMain.DataSource = reports.OrderByDescending(sr => sr.UpdateTime).ToList();
                }
            }
            catch (Exception ex)
            {
                niMain.ShowBalloonTip(120, "刷新服务状态时出错", ex.Message, ToolTipIcon.Error);
            }
        }


        private void dgvMain_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            var list = dgvMain.DataSource as List<ServiceReport>;
            if (list == null)
                return;


            if (e.ColumnIndex != -1 && e.RowIndex != -1 && dgvMain.Columns[e.ColumnIndex].Name == ServiceMessageColumn.Name && list[e.RowIndex].MessageArray != null)
            {
                dgvMain.Cursor = Cursors.Hand;
            }
            else
                dgvMain.Cursor = Cursors.Default;
        }

        private void dgvMain_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgvMain.Cursor = Cursors.Default;
        }

        private void dgvMain_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;

            var list = dgvMain.DataSource as List<ServiceReport>;
            if (list == null)
                return;


            if (dgvMain.Columns[e.ColumnIndex].Name == ServiceMessageColumn.Name && list[e.RowIndex].MessageArray != null)
            {
                var lb = new ListBox();
                lb.Width = 150;
                lb.Height = 180;
                lb.Items.AddRange(list[e.RowIndex].MessageArray);



                ToolStripDropDown dropDown = new ToolStripDropDown();
                dropDown.Padding = Padding.Empty;
                dropDown.Margin = Padding.Empty;

                var controlHost = new ToolStripControlHost(lb);
                controlHost.Padding = Padding.Empty;
                controlHost.Margin = Padding.Empty;
                controlHost.AutoSize = false;

                dropDown.Items.Add(controlHost);

                dropDown.Show(MousePosition);
            }
        }
    }
}
