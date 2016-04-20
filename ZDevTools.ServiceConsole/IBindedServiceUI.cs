using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDevTools.ServiceCore;

namespace ZDevTools.ServiceConsole
{
    /// <summary>
    /// 这个UI被服务绑定
    /// </summary>
    public interface IBindedServiceUI
    {
        IServiceBase BindedService { get; set; }

        string DisplayName { get; }
    }
}
