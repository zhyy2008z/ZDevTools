using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// Windows服务日志级别
    /// </summary>
    public enum WindowsServiceLogLevel
    {
        /// <summary>
        /// 所有
        /// </summary>
        All,
        /// <summary>
        /// 普通消息
        /// </summary>
        Info,
        /// <summary>
        /// 警告消息
        /// </summary>
        Warn,
        /// <summary>
        /// 错误消息
        /// </summary>
        Error,
        /// <summary>
        /// 无消息
        /// </summary>
        None
    }
}
