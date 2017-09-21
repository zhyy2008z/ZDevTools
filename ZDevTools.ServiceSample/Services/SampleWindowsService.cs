using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net;
using ZDevTools.ServiceCore;

namespace ZDevTools.ServiceSample.Services
{
    [ExportService(1)]
    public class SampleWindowsService : WindowsServiceBase
    {
        public override string DisplayName => "Windows服务样例服务";

        System.Timers.Timer timer = new System.Timers.Timer();
        protected override void DoWork(string[] args)
        {
            timer.Interval = 10000;

            timer.Elapsed += Timer_Elapsed;

            timer.Start();

            // TODO: Add code here to start your service.
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ReportErrorAndStop("这是个严重错误，会导致服务停止", null);
        }

        protected override void CleanUp()
        {

        }

    }
}
