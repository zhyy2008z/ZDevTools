using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// 请求的资源特性，用来告知一个服务所需要的所有资源有哪些
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class RequestResourceAttribute : Attribute
    {
        /// <summary>
        /// 初始化请求资源特性
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        public RequestResourceAttribute(string resourceName)
        {
            this.ResourceName = resourceName;
        }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string ResourceName { get; }

        /// <summary>
        /// 等待资源超时时间（不设置【0】代表使用默认超时时间，单位：秒）
        /// </summary>
        public int WaitTimeout { get; set; }

        /// <summary>
        /// 请求动作
        /// </summary>
        public RequestAction RequestAction { get; set; }
    }

}
