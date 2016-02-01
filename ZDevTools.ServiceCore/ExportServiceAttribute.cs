using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceCore
{
    /// <summary>
    /// 显示顺序默认999，一般来说就是最后了
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportServiceAttribute : ExportAttribute
    {
        public ExportServiceAttribute() : base(typeof(IServiceBase)) { DisplayOrder = 999; }

        public ExportServiceAttribute(int displayOrder) : base(typeof(IServiceBase)) { DisplayOrder = displayOrder; }

        public int DisplayOrder { get; private set; }
    }
}
