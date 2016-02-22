using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZDevTools.ServiceConsole
{
    public static class CommonFunctions
    {
        public static void ShowMessage(string message)
        {
            MessageBox.Show(message, Properties.Settings.Default.ServiceConsoleTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool ShowConfirm(string message)
        {
            return MessageBox.Show(message, Properties.Settings.Default.ServiceConsoleTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

    }
}
