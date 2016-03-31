using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace ZDevTools.ServiceConsole
{
    static class Program
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if !DEBUG
            //设置UI线程异常不要被界面捕获，直接抛出
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            //处理所有异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            handleException(e.ExceptionObject); //未预料到的任何错误，记录日志即可，程序会因异常未处理而退出
        }

        private static void handleException(object exception)
        {
            if (exception == null)
                log.Fatal("！v！<发生异常，但异常对象为空>！v！");
            else
            {
                var except = exception as Exception;
                if (except == null)
                    log.Fatal($"！v！<未知类型的异常:{exception}>！v！");
                else
                    log.Fatal("！v！<未捕获的异常>！v！", except);
            }
        }
    }
}
