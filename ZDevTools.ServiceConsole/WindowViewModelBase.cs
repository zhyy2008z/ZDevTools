using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Windows;

namespace ZDevTools.ServiceConsole
{
    public abstract class WindowViewModelBase<T> : BindableBase
        where T:Window
    {

        public Synchronizer Synchronizer { get; set; }

        public T Window { get; set; }


    }
}
