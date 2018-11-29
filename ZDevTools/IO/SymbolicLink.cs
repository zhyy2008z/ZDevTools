using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace ZDevTools.IO
{
    /// <summary>
    /// 符号链接操作实用类
    /// 引用自：http://troyparsons.com/blog/2012/03/symbolic-links-in-c-sharp/
    /// </summary>
    public static class SymbolicLink
    {
        const uint GenericReadAccess = 0x80000000;

        const uint FileFlagsForOpenReparsePointAndBackupSemantics = 0x02200000;

        const int IoctlCommandGetReparsePoint = 0x000900A8;

        const uint OpenExisting = 0x3;

        const uint PathNotAReparsePointError = 0x80071126;

        const uint ShareModeAll = 0x7; // Read, Write, Delete

        const uint SymLinkTag = 0xA000000C;

        /// <summary>
        /// Reparse point tag used to identify mount points and junction points.
        /// </summary>
        private const uint IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "CreateFile")]
        private static extern SafeFileHandle createFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, EntryPoint = "DeviceIoControl")]
        private static extern bool deviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            int nInBufferSize,
            IntPtr lpOutBuffer,
            int nOutBufferSize,
            out int lpBytesReturned,
            IntPtr lpOverlapped);


        private static SafeFileHandle getFileHandle(string path)
        {
            return createFile(path, GenericReadAccess, ShareModeAll, IntPtr.Zero, OpenExisting,
                FileFlagsForOpenReparsePointAndBackupSemantics, IntPtr.Zero);
        }

        /// <summary>
        /// 获取目标符号链接（同时支持软链接与Junction）路径的真实路径
        /// </summary>
        /// <param name="targetPath"></param>
        /// <returns>如果目标路径不是符号链接，那么将返回 null，否则返回目标路径的真实路径</returns>
        public static string GetRealPath(string targetPath)
        {
            SymbolicLinkReparseData reparseDataBuffer;

            using (SafeFileHandle fileHandle = getFileHandle(targetPath))
            {
                if (fileHandle.IsInvalid)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                int outBufferSize = Marshal.SizeOf(typeof(SymbolicLinkReparseData));
                IntPtr outBuffer = IntPtr.Zero;
                try
                {
                    outBuffer = Marshal.AllocHGlobal(outBufferSize);
                    int bytesReturned;
                    bool success = deviceIoControl(
                        fileHandle.DangerousGetHandle(), IoctlCommandGetReparsePoint, IntPtr.Zero, 0,
                        outBuffer, outBufferSize, out bytesReturned, IntPtr.Zero);

                    fileHandle.Close();

                    if (!success)
                    {
                        if (((uint)Marshal.GetHRForLastWin32Error()) == PathNotAReparsePointError)
                        {
                            return null;
                        }
                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                    }

                    reparseDataBuffer = (SymbolicLinkReparseData)Marshal.PtrToStructure(
                        outBuffer, typeof(SymbolicLinkReparseData));
                }
                finally
                {
                    Marshal.FreeHGlobal(outBuffer);
                }
            }
            if (reparseDataBuffer.ReparseTag != SymLinkTag && reparseDataBuffer.ReparseTag != IO_REPARSE_TAG_MOUNT_POINT)
            {
                return null;
            }

            string target = Encoding.Unicode.GetString(reparseDataBuffer.PathBuffer,
                reparseDataBuffer.PrintNameOffset, reparseDataBuffer.PrintNameLength);

            return target;
        }
    }
}