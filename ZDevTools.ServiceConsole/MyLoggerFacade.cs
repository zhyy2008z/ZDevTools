using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Logging;

namespace ZDevTools.ServiceConsole
{
    public class MyLoggerFacade : ILoggerFacade
    {

        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MyLoggerFacade));


        public void Log(string message, Category category, Priority priority)
        {
            message = priority + " - " + message;

            switch (category)
            {
                case Category.Debug:
                    log.Debug(message);
                    break;
                case Category.Exception:
                    log.Error(message);
                    break;
                case Category.Info:
                    log.Info(message);
                    break;
                case Category.Warn:
                    log.Warn(message);
                    break;
                default:
                    break;
            }
        }
    }
}
