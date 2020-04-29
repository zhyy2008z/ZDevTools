using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// 获取GRPC服务定义工厂
    /// </summary>
    public delegate ServerServiceDefinition GRpcServiceDefinitionFactory();
}
