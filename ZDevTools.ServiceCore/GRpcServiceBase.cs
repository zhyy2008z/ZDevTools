using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceStack.Redis;
using Grpc.Core;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// 专为承载WCF服务创建的服务基类
    /// </summary>
    public abstract class GRpcServiceBase : ServiceBase, IHostedService
    {
        readonly GRpcServiceDefinitionFactory GRpcServiceDefinitionProvider;
        readonly int ServicePort;

        protected GRpcServiceBase(GRpcServiceDefinitionFactory gRpcServiceDefinitionProvider, int servicePort, ILogger logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            this.GRpcServiceDefinitionProvider = gRpcServiceDefinitionProvider;
            this.ServicePort = servicePort;
        }

        void logError(string message) => Logger.LogError($"【{DisplayName}】{message}");


        /// <summary>
        /// 当服务执行失败时发生
        /// </summary>
        public event EventHandler<ErrorEventArgs> Faulted;

        Server _server;
        /// <summary>
        /// 开始服务
        /// </summary>
        public Task StartAsync()
        {
            _server = new Server
            {
                Services = { GRpcServiceDefinitionProvider() },
                Ports = { new ServerPort("0.0.0.0", ServicePort, ServerCredentials.Insecure) }
            };
            _server.Start();
            ReportStatus("状态：正在运行");
            return Task.CompletedTask;
        }

        private void serviceHost_Faulted(object sender, EventArgs e)
        {
            _server.ShutdownAsync();
            logError("GRpc服务失败");
            ReportError("状态：已停止，GRpc服务失败");
            Faulted?.Invoke(this, new ErrorEventArgs("GRpc服务失败"));
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public async Task StopAsync()
        {
            await _server.ShutdownAsync().ConfigureAwait(false);
            ReportStatus("状态：停止运行");
        }
    }
}
