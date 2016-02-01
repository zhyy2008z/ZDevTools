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
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string resourceName;

        // This is a positional argument
        public RequestResourceAttribute(string resourceName)
        {
            this.resourceName = resourceName;
        }

        public string ResourceName
        {
            get { return resourceName; }
        }
    }

}
