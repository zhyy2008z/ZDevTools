using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ZDevTools.ServiceConsole
{
    public class Synchronizer
    {
        Dispatcher _dispatcher;
        public Synchronizer(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Invoke(Action action)
        {
            _dispatcher.InvokeAsync(action);
        }
    }
}
