using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Windows.Media;

namespace ZDevTools.ServiceConsole
{
    using ServiceCore;

    public abstract class ServiceViewModelBase : BindableBase
    {
        public ServiceViewModelBase()
        {
            ButtonEnabled = true;
            StatusText = "已停止";
            StatusColor = Brushes.Black;
        }

        /// <summary>
        /// 同步器
        /// </summary>
        public Synchronizer Synchronizer { get; set; }


        /// <summary>
        /// 绑定的服务
        /// </summary>
        public abstract IServiceBase BindedService { get; set; }

        string _displayName;
        /// <summary>
        /// 服务名称
        /// </summary>
        public string DisplayName { get { return _displayName; } set { SetProperty(ref _displayName, value); } }

        string _statusText;
        /// <summary>
        /// 状态文本
        /// </summary>
        public string StatusText { get { return _statusText; } set { SetProperty(ref _statusText, value); } }

        string _buttonText;
        /// <summary>
        /// 按钮文本
        /// </summary>
        public string ButtonText { get { return _buttonText; } set { SetProperty(ref _buttonText, value); } }

        Brush _statusColor;
        /// <summary>
        /// 状态颜色
        /// </summary>
        public Brush StatusColor { get { return _statusColor; } set { SetProperty(ref _statusColor, value); } }

        bool _buttonEnabled;
        /// <summary>
        /// 按钮启用状态
        /// </summary>
        public bool ButtonEnabled { get { return _buttonEnabled; } set { SetProperty(ref _buttonEnabled, value); } }

        string _statusTooltip;
        /// <summary>
        /// 状态提示
        /// </summary>
        public string StatusToolTip { get { return _statusTooltip; } set { SetProperty(ref _statusTooltip, value); } }

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
