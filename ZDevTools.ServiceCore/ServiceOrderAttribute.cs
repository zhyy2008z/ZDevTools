using System;
using System.Collections.Generic;
using System.Text;

namespace ZDevTools.ServiceCore
{

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ServiceOrderAttribute : Attribute
    {

        // This is a positional argument
        public ServiceOrderAttribute(int order)
        {
            this.Order = order;
        }

        /// <summary>
        /// 值越小越靠前
        /// </summary>
        public int Order { get; }
    }
}
