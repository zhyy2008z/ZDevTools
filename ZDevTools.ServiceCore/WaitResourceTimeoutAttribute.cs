using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class WaitResourceTimeoutAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly int waitTimeout;

        /// <summary>
        /// 初始化一个WaitResourceTimeout特性
        /// </summary>
        /// <param name="waitTimeout">等待资源超时时间（不设置【0】代表使用默认超时时间，单位：秒）</param>
        public WaitResourceTimeoutAttribute(int waitTimeout)
        {
            this.waitTimeout = waitTimeout;

        }

        /// <summary>
        /// 等待资源超时时间（不设置【0】代表使用默认超时时间，单位：秒）
        /// </summary>
        public int WaitTimeout
        {
            get { return waitTimeout; }
        }

    }

}
