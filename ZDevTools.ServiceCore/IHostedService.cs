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
        Task StartAsync();

        Task StopAsync();
    }
}
