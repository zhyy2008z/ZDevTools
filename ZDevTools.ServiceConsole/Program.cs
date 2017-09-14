using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZDevTools.ServiceConsole
{
    using ServiceCore;
    using ViewModels;

    class Program
    {
        static log4net.ILog _log;


        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "-Daemon")
            {
                var services = MainWindowViewModel.GetServicesFromMef();

                if ((from s in services
                     where s is WindowsServiceBase && s.ServiceName == args[1]
                     select s).SingleOrDefault() is WindowsServiceBase service)
                    System.ServiceProcess.ServiceBase.Run(service);
            }
            else
            {
                log4net.Config.XmlConfigurator.Configure();
                _log = log4net.LogManager.GetLogger(typeof(Program));

#if !DEBUG
                //处理所有异常
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif

                App.Main();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            handleException(e.ExceptionObject); //未预料到的任何错误，记录日志即可，程序会因异常未处理而退出
        }

        private static void handleException(object exception)
        {
            if (exception == null)
                _log.Fatal("！v！<发生异常，但异常对象为空>！v！");
            else
            {
                var except = exception as Exception;
                if (except == null)
                    _log.Fatal($"！v！<未知类型的异常:{exception}>！v！");
                else
                    _log.Fatal("！v！<未捕获的异常>！v！", except);
            }
        }
    }
}
