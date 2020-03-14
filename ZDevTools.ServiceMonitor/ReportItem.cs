
using System;
using System.Collections.Generic;
using System.Text;

namespace ZDevTools.ServiceMonitor
{
    class ReportItem
    {
        /// <summary>
        /// 刷新服务是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 若不成功，此消息不为null
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 是否获取到全部报告
        /// </summary>
        public bool IsAll { get; set; }

        /// <summary>
        /// 单个报告对象<code><see cref="IsAll"/>==true</code>
        /// </summary>
        public ServiceReport ServiceReport { get; set; }

        /// <summary>
        /// 全部报告字段<code><see cref="IsAll"/>==false</code>
        /// </summary>
        public List<ServiceReport> ServiceReports { get; set; }
    }
}
