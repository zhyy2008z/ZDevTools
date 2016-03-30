using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using System.Windows.Forms;
using System.IO;

namespace ZDevTools.ServiceConsole
{
    public class FormAppender : log4net.Appender.AppenderSkeleton
    {
        //注意：PatternLayout中的Footer与Header属性的输出工作【不在AppenderSkeleton中】是交给AppenderSkeleton的实现者的，因为并不是所有的Appender都需要输出他们。在Log4net中也只有TextWriteAppender、SmtpAppender和SmtpPickupAppender这几个类输出了他们。
        protected override void Append(LoggingEvent loggingEvent)
        {
            string message = this.RenderLoggingEvent(loggingEvent);
            MainForm.Instance?.OutputLog(new MessageItem(message, loggingEvent.Level));
        }
    }
}
