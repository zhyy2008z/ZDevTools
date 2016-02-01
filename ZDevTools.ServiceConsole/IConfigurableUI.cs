using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceConsole
{
    /// <summary>
    /// 这个UI可以被配置，也就需要提供加载和保存配置接口
    /// </summary>
    public interface IConfigurableUI
    {
        string SaveConfig();

        void LoadConfig(string jsonString);
    }
}
