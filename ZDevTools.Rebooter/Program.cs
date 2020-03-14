using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ZDevTools.Rebooter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var targetPath = args[0];
            var processId = int.Parse(args[1]);

            //给予一分钟时间等待重启，如果超时就放弃重启
            for (int i = 0; i < 60 * 2; i++)
            {
                if (!Process.GetProcesses().Any(p => p.Id == processId))
                {
                    Process.Start(targetPath);
                    break;
                }

                Thread.Sleep(500);
            }

        }
    }
}
