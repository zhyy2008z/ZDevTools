using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

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

        static string _exePath;
        public static string GetExePath()
        {
            if (_exePath == null)
                _exePath = typeof(CommonFunctions).Assembly.Location;
            return _exePath;
        }

        public static string GetAbsolutePath(string relativeExePath)
        {
            return Path.Combine(Path.GetDirectoryName(GetExePath()), relativeExePath);
        }
    }
}
