using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Unity;
using Microsoft.Practices.Unity;
using Prism.Logging;

namespace ZDevTools.ServiceConsole
{
    using Views;
    using DIServices;

    class Bootstrapper : UnityBootstrapper
    {
        //注释不注释，效果没差

        //protected override DependencyObject CreateShell() { return null; }

        //protected override void InitializeShell() { }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IDialogs, Dialogs>(new ContainerControlledLifetimeManager());
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new MyLoggerFacade();
        }

    }
}
