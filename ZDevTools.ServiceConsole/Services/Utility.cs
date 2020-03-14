using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace ZDevTools.ServiceConsole.Services
{
    class Utility : IUtility
    {
        public Utility() { }


        string _exePath;
        public string ExePath
        {
            get
            {
                if (_exePath == null)
                    _exePath = typeof(Utility).Assembly.Location;
                return _exePath;
            }
        }




    }
}
