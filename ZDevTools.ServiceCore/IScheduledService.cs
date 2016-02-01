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
        void Run();

    }
}
