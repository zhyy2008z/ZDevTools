using System;
using System.Collections.Generic;
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
        /// <summary>
        /// 服务显示名称
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// 服务內部名称（其实就是类名，名称唯一）
        /// </summary>
        string ServiceName { get; }
    }
}
