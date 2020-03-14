using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace ZDevTools.ServiceMonitor
{
    public class EventSink : ILogEventSink, IDisposable
    {
        readonly ITextFormatter TextFormatter = new MessageTemplateTextFormatter("[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}", CultureInfo.CurrentUICulture);


        public void Emit(LogEvent logEvent)
        {
            var logHandler = Log;
            if (logHandler != null)
            {
                string message;
                using (var renderSpace = new StringWriter())
                {
                    TextFormatter.Format(logEvent, renderSpace);
                    message = renderSpace.ToString();
                }
                logHandler(logEvent.Level, message.Trim());
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Log = null;
        }

        ~EventSink()
        {
            Dispose(false);
        }

        public event Action<LogEventLevel, string> Log;

    }
}
