using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(string message, Exception exception)
        {
            ErrorMessage = message;
            Exception = exception;
        }

        public ErrorEventArgs(string message)
        {
            ErrorMessage = message;
        }

        public ErrorEventArgs(Exception exception)
        {
            ErrorMessage = exception.Message;
            Exception = exception;
        }

        public string ErrorMessage { get; }

        public Exception Exception { get; }
    }
}
