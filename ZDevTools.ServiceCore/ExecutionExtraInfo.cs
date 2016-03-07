using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// 执行额外信息
    /// </summary>
    public class ExecutionExtraInfo
    {
        /// <summary>
        /// 警告信息
        /// </summary>
        public string WarnMessage { get; }

        /// <summary>
        /// 警告信息数组
        /// </summary>
        public List<string> WarnMessageArray { get; }

        /// <summary>
        /// 初始化一个执行额外信息类
        /// </summary>
        public ExecutionExtraInfo(string warnMessage)
        {
            WarnMessage = warnMessage;
        }

        /// <summary>
        /// 初始化一个执行额外信息类
        /// </summary>
        public ExecutionExtraInfo(string warnMessage, List<string> warnMassageArray)
        {
            WarnMessage = warnMessage;
            WarnMessageArray = warnMassageArray;
        }
    }
}
