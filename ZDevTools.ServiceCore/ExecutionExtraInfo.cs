using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    public class ExecutionExtraInfo
    {
        public string WarnMessage { get; }
        public List<string> WarnMessageArray { get; }


        public ExecutionExtraInfo(string warnMessage)
        {
            WarnMessage = warnMessage;
        }

        public ExecutionExtraInfo(string warnMessage, List<string> warnMassageArray)
        {
            WarnMessage = warnMessage;
            WarnMessageArray = warnMassageArray;
        }
    }
}
