using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZDevTools.ServiceConsole
{
    public partial class OneKeyStartConfigForm : Form
    {
        public OneKeyStartConfigForm()
        {
            InitializeComponent();
        }

        public Dictionary<string, ServiceItemConfig> Configs { get; set; }

        private void bOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbMain.Items.Count; i++)
            {
                ((ServiceItemConfig)clbMain.Items[i]).OneKeyStart = clbMain.GetItemChecked(i);
            }
        }

        private void OneKeyStartConfigForm_Load(object sender, EventArgs e)
        {
            foreach (var keyValue in Configs)
            {
                clbMain.Items.Add(keyValue.Value, keyValue.Value.OneKeyStart);
            }
        }
    }
}
