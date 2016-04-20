using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using ZDevTools.ServiceCore;
using System.ServiceProcess;
using System.IO;

namespace ZDevTools.ServiceConsole
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {

        public ProjectInstaller()
        {
            InitializeComponent();

            DirectoryCatalog directoryCatalog = new DirectoryCatalog(Path.Combine(Path.GetDirectoryName(typeof(MainForm).Assembly.Location), "services"));
            CompositionContainer container = new CompositionContainer(directoryCatalog);

            //定义载入的服务
            IServiceBase[] services = (from l in container.GetExports<IServiceBase, IServiceMetadata>()
                                       orderby l.Metadata.DisplayOrder
                                       select l.Value).ToArray();

            foreach (var service in services)
            {
                var windowsService = service as WindowsServiceBase;

                if (windowsService != null)
                {
                    ServiceInstaller serviceInstaller = new ServiceInstaller();
                    serviceInstaller.ServiceName = windowsService.ServiceName;
                    serviceInstaller.DisplayName = windowsService.DisplayName;
                    this.Installers.Add(serviceInstaller);
                }
            }
        }
    }
}
