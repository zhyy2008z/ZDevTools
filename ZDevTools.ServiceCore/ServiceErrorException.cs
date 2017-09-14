using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// 服务错误异常
    /// </summary>
    [Serializable]
    public class ServiceErrorException : Exception
    {
        /// <summary>
        /// 初始化一个异常
        /// </summary>
        public ServiceErrorException(string message) : base(message) { }

        /// <summary>
        /// 初始化一个异常
        /// </summary>
        public ServiceErrorException(string message, Exception inner) : base(message, inner) { }
    }
}
