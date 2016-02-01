using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceConsole
{
    public interface IServiceMetadata
    {
        int DisplayOrder { get; }
    }
}
