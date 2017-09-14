using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// 可承载的服务，一般用来表示长时间不间断运行的服务
    /// </summary>
    public interface IHostedService : IServiceBase
    {
        /// <summary>
        /// 开始执行承载服务
        /// </summary>
        /// <returns></returns>
        Task StartAsync();

        /// <summary>
        /// 停止执行承载服务
        /// </summary>
        /// <returns></returns>
        Task StopAsync();

        /// <summary>
        /// 承载服务因某种原因失败时发生
        /// </summary>
        event EventHandler<ErrorEventArgs> Faulted;
    }
}
