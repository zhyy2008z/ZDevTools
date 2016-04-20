using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceConsole
{
    /// <summary>
    /// 这个UI可以被控制，提供控制相关操作
    /// </summary>
    public interface IControllableUI
    {
        void Stop();

        bool IsStopped { get; }

        void Start();

        void RefreshStatus();
    }
}
