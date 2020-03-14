using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ZDevTools.ServiceCore;

namespace ZDevTools.ServiceConsole
{
    public abstract class ServiceViewModelBase : ReactiveObject
    {
        public ServiceViewModelBase()
        {
            ButtonEnabled = true;
            StatusText = "已停止";
            StatusColor = Brushes.Black;
        }

        /// <summary>
        /// 绑定的服务
        /// </summary>
        public abstract IServiceBase BindedService { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        [Reactive]
        public string DisplayName { get; set; }


        /// <summary>
        /// 状态文本
        /// </summary>
        [Reactive]
        public string StatusText { get; set; }

        /// <summary>
        /// 按钮文本
        /// </summary>
        [Reactive]
        public string ButtonText { get; set; }

        /// <summary>
        /// 状态颜色
        /// </summary>
        [Reactive]
        public Brush StatusColor { get; set; }

        /// <summary>
        /// 按钮启用状态
        /// </summary>
        [Reactive]
        public bool ButtonEnabled { get; set; }

        /// <summary>
        /// 状态提示
        /// </summary>
        [Reactive]
        public string StatusToolTip { get; set; }

        /// <summary>
        /// 停止当前服务
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// 当前服务是否为停止状态
        /// </summary>
        public abstract bool IsStopped { get; }

        /// <summary>
        /// 启动当前服务
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// 刷新当前服务状态
        /// </summary>
        public abstract void RefreshStatus();
    }
}
