using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using ZDevTools.ServiceConsole.Schedules;
using static ZDevTools.ServiceConsole.CommonFunctions;

namespace ZDevTools.ServiceConsole
{
    public partial class ScheduleManageForm : Form
    {
        public ScheduleManageForm()
        {
            InitializeComponent();
        }

        public bool CanManage { get; set; }

        public List<BasicSchedule> Schedules { get; set; }

        void refreshItems()
        {
            lvScheduleManage.Items.Clear();

            foreach (var item in Schedules)
            {
                lvScheduleManage.Items.Add(item.Title).SubItems.AddRange(new string[] { item.ToString(), item.Enabled ? "已启用" : "已禁用" });
            }

            updateButtonStates();

        }

        private void lvScheduleManage_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateButtonStates();
        }

        private void updateButtonStates()
        {
            if (CanManage)
            {
                if (lvScheduleManage.SelectedItems.Count == 0)
                {
                    bDel.Enabled = false;
                    bEdit.Enabled = false;
                }
                else if (lvScheduleManage.SelectedItems.Count == 1)
                {
                    bDel.Enabled = true;
                    bEdit.Enabled = true;
                }
                else
                {
                    bDel.Enabled = true;
                    bEdit.Enabled = false;
                }
            }
            else
            {
                bDel.Enabled = false;
                bEdit.Enabled = false;
                bAdd.Enabled = false;
                bOK.Visible = false;
            }

        }

        private void lvScheduleManage_DoubleClick(object sender, EventArgs e)
        {
            bEdit.PerformClick();
        }

        private void bEdit_Click(object sender, EventArgs e)
        {
            var item = lvScheduleManage.FocusedItem;

            using (var form = new ScheduleForm())
            {
                form.LoadModel(Schedules[item.Index]);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Schedules[item.Index] = form.SaveSchedule();
                    refreshItems();
                }
            }
        }

        private void bDel_Click(object sender, EventArgs e)
        {
            if (ShowConfirm("确定要删除选中的计划？"))
            {
                foreach (var item in lvScheduleManage.SelectedIndices.Cast<int>().Reverse())
                {
                    Schedules.RemoveAt(item);
                }
                refreshItems();
            }
        }

        private void bAdd_Click(object sender, EventArgs e)
        {
            using (var form = new ScheduleForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Schedules.Add(form.SaveSchedule());
                    refreshItems();
                }
            }
        }

        private void ScheduleManageForm_Load(object sender, EventArgs e)
        {
            refreshItems();
        }
    }
}
