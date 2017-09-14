using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace ZDevTools.ServiceConsole.Models
{
    public class ServiceItemModel : BindableBase
    {
        string _serviceKey;
        public string ServiceKey { get { return _serviceKey; } set { SetProperty(ref _serviceKey, value); } }

        string _serviceName;
        public string ServiceName { get { return _serviceName; } set { SetProperty(ref _serviceName, value); } }

        bool _oneKeyStart;
        public bool OneKeyStart { get { return _oneKeyStart; } set { SetProperty(ref _oneKeyStart, value); } }
    }
}
