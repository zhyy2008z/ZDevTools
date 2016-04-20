using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ZDevTools.ServiceConsole
{
    public static class ServiceHelper
    {
        #region Windows API
        [DllImport("advapi32.dll", SetLastError = true)]
        static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern IntPtr OpenSCManager(
            string machineName, string databaseName, uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern int CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool QueryServiceConfig(IntPtr service, IntPtr queryServiceConfig, uint bufferSize, out uint bytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool QueryServiceConfig2(IntPtr hService, uint dwInfoLevel, IntPtr buffer, uint cbBufSize, out uint pcbBytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool ChangeServiceConfig(IntPtr hService, uint nServiceType, uint nStartType, uint nErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, IntPtr lpdwTagId, [In] char[] lpDependencies, string lpServiceStartName, string lpPassword, string lpDisplayName);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool ChangeServiceConfig2(IntPtr hService, uint dwInfoLevel, IntPtr lpInfo);

        #endregion

        #region Consts & Structs
        const uint SERVICE_NO_CHANGE = 0xFFFFFFFF;
        const uint SERVICE_QUERY_CONFIG = 0x00000001;
        const uint SERVICE_CHANGE_CONFIG = 0x00000002;
        const uint SC_MANAGER_ALL_ACCESS = 0x000F003F;
        const uint SC_MANAGER_CONNECT = 0x0001;
        const uint SERVICE_CONFIG_DELAYED_AUTO_START_INFO = 3;

        struct SERVICE_DELAYED_AUTO_START_INFO
        {
            public bool fDelayedAutostart;
        }

#pragma warning disable CS0649
        struct QueryServiceConfigStruct
        {
            public int serviceType;
            public int startType;
            public int errorControl;
            public IntPtr binaryPathName;
            public IntPtr loadOrderGroup;
            public int tagID;
            public IntPtr dependencies;
            public IntPtr startName;
            public IntPtr displayName;
        }
#pragma warning restore

        #endregion

        public static void ChangeStartMode(string serviceName, ServiceStartMode mode, bool delayedAutoStart)
        {
            var scManagerHandle = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
            if (scManagerHandle == IntPtr.Zero)
                throw new Win32Exception();
            try
            {
                var serviceHandle = OpenService(scManagerHandle, serviceName, SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);

                if (serviceHandle == IntPtr.Zero)
                    throw new Win32Exception();
                try
                {
                    var retFlag = ChangeServiceConfig(serviceHandle, SERVICE_NO_CHANGE, (uint)mode, SERVICE_NO_CHANGE, null, null, IntPtr.Zero, null, null, null, null);

                    if (!retFlag)
                    {
                        int nError = Marshal.GetLastWin32Error();
                        var win32Exception = new Win32Exception(nError);
                        throw new ExternalException("Could not change service start type: " + win32Exception.Message);
                    }

                    if (mode == ServiceStartMode.Automatic) //自动模式，需要设置延迟启动
                    {
                        SERVICE_DELAYED_AUTO_START_INFO info = new SERVICE_DELAYED_AUTO_START_INFO();
                        info.fDelayedAutostart = delayedAutoStart;
                        var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(info));
                        try
                        {
                            Marshal.StructureToPtr(info, ptr, true);
                            retFlag = ChangeServiceConfig2(serviceHandle, SERVICE_CONFIG_DELAYED_AUTO_START_INFO, ptr);
                            if (!retFlag)
                                throw new Win32Exception();
                        }
                        finally { Marshal.FreeCoTaskMem(ptr); }
                    }
                }
                finally { CloseServiceHandle(serviceHandle); }
            }
            finally { CloseServiceHandle(scManagerHandle); }
        }

        public static void ChangeExePath(string serviceName, string exePath)
        {
            var scManagerHandle = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);

            if (scManagerHandle == IntPtr.Zero)
                throw new Win32Exception();
            try
            {
                var serviceHandle = OpenService(scManagerHandle, serviceName, SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);
                if (serviceHandle == IntPtr.Zero)
                    throw new Win32Exception();
                try
                {
                    var retFlag = ChangeServiceConfig(serviceHandle, SERVICE_NO_CHANGE, SERVICE_NO_CHANGE, SERVICE_NO_CHANGE, exePath, null, IntPtr.Zero, null, null, null, null);

                    if (!retFlag)
                        throw new Win32Exception();
                }
                finally { CloseServiceHandle(serviceHandle); }
            }
            finally { CloseServiceHandle(scManagerHandle); }

        }

        public static ServiceInfo QueryServiceConfig(string serviceName, out bool delayedAutoStart)
        {
            IntPtr scManagerHandle = OpenSCManager(".", null, SC_MANAGER_CONNECT);
            if (scManagerHandle == IntPtr.Zero)
                throw new Win32Exception();

            ServiceInfo serviceInfo = new ServiceInfo();
            try
            {
                IntPtr serviceHandle = OpenService(scManagerHandle,
                    serviceName, SERVICE_QUERY_CONFIG);

                if (serviceHandle == IntPtr.Zero)
                    throw new NullReferenceException();

                try
                {
                    uint bytesNeeded;

                    bool retFlag = QueryServiceConfig(serviceHandle, IntPtr.Zero, 0, out bytesNeeded);

                    if (!retFlag && bytesNeeded == 0)
                        throw new Win32Exception();

                    var ptr = Marshal.AllocCoTaskMem((int)bytesNeeded);
                    try
                    {
                        retFlag = QueryServiceConfig(serviceHandle, ptr, bytesNeeded, out bytesNeeded);

                        if (!retFlag)
                            throw new Win32Exception();

                        var qscs = (QueryServiceConfigStruct)Marshal.PtrToStructure(ptr, typeof(QueryServiceConfigStruct));

                        serviceInfo.binaryPathName = Marshal.PtrToStringAuto(qscs.binaryPathName);
                        serviceInfo.dependencies = Marshal.PtrToStringAuto(qscs.dependencies);
                        serviceInfo.displayName = Marshal.PtrToStringAuto(qscs.displayName);
                        serviceInfo.loadOrderGroup = Marshal.PtrToStringAuto(qscs.loadOrderGroup);
                        serviceInfo.startName = Marshal.PtrToStringAuto(qscs.startName);

                        serviceInfo.errorControl = qscs.errorControl;
                        serviceInfo.serviceType = qscs.serviceType;
                        serviceInfo.startType = qscs.startType;
                        serviceInfo.tagID = qscs.tagID;
                    }
                    finally { Marshal.FreeCoTaskMem(ptr); }

                    if (serviceInfo.startType == (int)ServiceStartMode.Automatic)  //自动启动类型需要检测是否延迟启动
                    {
                        retFlag = QueryServiceConfig2(serviceHandle, SERVICE_CONFIG_DELAYED_AUTO_START_INFO, IntPtr.Zero, 0, out bytesNeeded);

                        if (!retFlag && bytesNeeded == 0)
                            throw new Win32Exception();

                        ptr = Marshal.AllocCoTaskMem((int)bytesNeeded);

                        try
                        {
                            retFlag = QueryServiceConfig2(serviceHandle, SERVICE_CONFIG_DELAYED_AUTO_START_INFO, ptr, bytesNeeded, out bytesNeeded);

                            if (!retFlag)
                                throw new Win32Exception();

                            var sdasi = (SERVICE_DELAYED_AUTO_START_INFO)Marshal.PtrToStructure(ptr, typeof(SERVICE_DELAYED_AUTO_START_INFO));

                            delayedAutoStart = sdasi.fDelayedAutostart;
                        }
                        finally { Marshal.FreeCoTaskMem(ptr); }
                    }
                    else
                        delayedAutoStart = false;

                }
                finally { CloseServiceHandle(serviceHandle); }
            }
            finally { CloseServiceHandle(scManagerHandle); }
            return serviceInfo;
        }
    }

    public struct ServiceInfo
    {
        public int serviceType;
        public int startType;
        public int errorControl;
        public string binaryPathName;
        public string loadOrderGroup;
        public int tagID;
        public string dependencies;
        public string startName;
        public string displayName;
    }
}