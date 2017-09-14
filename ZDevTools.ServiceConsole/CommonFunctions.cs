using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZDevTools.ServiceConsole
{
    public static class CommonFunctions
    {
        public static void ShowMessage(string message)
        {
            MessageBox.Show(message, Properties.Settings.Default.ServiceConsoleTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(message, Properties.Settings.Default.ServiceConsoleTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool ShowConfirm(string message)
        {
            return MessageBox.Show(message, Properties.Settings.Default.ServiceConsoleTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }


        public static Window GetActiveWindow()
        {
            return App.Current.Windows.Cast<Window>().FirstOrDefault(window => window.IsActive);
        }
    }
}
