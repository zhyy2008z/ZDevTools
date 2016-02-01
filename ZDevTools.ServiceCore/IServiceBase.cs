using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// 服务基本接口
    /// </summary>
    public interface IServiceBase
    {
        string ServiceName { get; }
    }
}
