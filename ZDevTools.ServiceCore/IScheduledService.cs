using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// 代表最常见的服务
    /// </summary>
    public interface IScheduledService : IServiceBase
    {
        /// <summary>
        /// 运行服务
        /// </summary>
        /// <returns>本次执行是否成功</returns>
        bool Run();

    }
}
