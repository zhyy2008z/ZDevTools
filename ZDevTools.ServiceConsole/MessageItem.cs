using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceConsole
{
    public class MessageItem
    {

        public MessageItem(string content, log4net.Core.Level level)
        {
            this.Content = content;
            this.Level = level;
        }

        public string Content { get; }

        public log4net.Core.Level Level { get; }

        public override string ToString()
        {
            return Content;
        }
    }
}
