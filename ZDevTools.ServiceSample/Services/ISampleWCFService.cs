using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceSample.Services
{
    [ServiceContract]
    public interface ISampleWCFService
    {
        [OperationContract]
        string TestApi();
    }
}
