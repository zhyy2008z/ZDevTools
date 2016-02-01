using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// 代表一个服务可以被取消，目前仅ScheduledService服务类型支持该接口
    /// </summary>
    public interface IServiceRevokable
    {
        void Cancel();
    }
}
