using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using log4net.Core;

namespace ZDevTools.ServiceConsole.Converters
{
    public class LogLevelBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var level = value as Level;
            Brush brush = null;
            if (level == Level.Info)
                brush = Brushes.Black;
            else if (level == Level.Warn)
                brush = Brushes.DarkOrange;
            else if (level == Level.Error)
                brush = Brushes.Red;
            else if (level == Level.Debug)
                brush = Brushes.Blue;
            else if (level == Level.Fatal)
                brush = Brushes.DarkRed;
            else
                brush = Brushes.Black;

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
