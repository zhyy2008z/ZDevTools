using System;
using System.Runtime.InteropServices;

namespace ZDevTools
{
    static class NativeMethods
    {
        /// <summary>
        /// 设置UTC时间
        /// </summary>
        [DllImport("Kernel32.dll")]
        public static extern bool SetSystemTime(ref SYSTEMTIME time);

        /// <summary>
        /// 设置本地时间
        /// </summary>
        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SYSTEMTIME time);

        /// <summary>
        /// Enables an application to inform the system that it is in use, thereby preventing the system from entering sleep or turning off the display while the application is running.
        /// </summary>
        /// <param name="esFlags">The thread's execution requirements. </param>
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SetThreadExecutionState(ES_Flags esFlags);

        //BOOL
        //WINAPI
        //MiniDumpWriteDump(
        //    __in HANDLE hProcess,
        //    __in DWORD ProcessId,
        //    __in HANDLE hFile,
        //    __in MINIDUMP_TYPE DumpType,
        //    __in_opt PMINIDUMP_EXCEPTION_INFORMATION ExceptionParam,
        //    __in_opt PMINIDUMP_USER_STREAM_INFORMATION UserStreamParam,
        //    __in_opt PMINIDUMP_CALLBACK_INFORMATION CallbackParam
        //    );
        [DllImport("dbghelp.dll",
          EntryPoint = "MiniDumpWriteDump",
          CallingConvention = CallingConvention.StdCall,
          CharSet = CharSet.Unicode,
          ExactSpelling = true, SetLastError = true)]
        public static extern bool MiniDumpWriteDump(
          IntPtr hProcess,
          uint processId,
          IntPtr hFile,
          uint dumpType,
          ref MiniDumpExceptionInformation expParam,
          IntPtr userStreamParam,
          IntPtr callbackParam);

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        public static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcess", ExactSpelling = true)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcessId", ExactSpelling = true)]
        public static extern uint GetCurrentProcessId();
    }

    [StructLayout(LayoutKind.Sequential)]
    struct SYSTEMTIME
    {
        public ushort WYear;
        public ushort WMonth;
        public ushort WDayOfWeek;
        public ushort WDay;
        public ushort WHour;
        public ushort WMinute;
        public ushort WSecond;
        public ushort WMilliseconds;

        public void FromDateTime(DateTime dateTime)
        {
            WYear = (ushort)dateTime.Year;
            WMonth = (ushort)dateTime.Month;
            WDayOfWeek = (ushort)dateTime.DayOfWeek;
            WDay = (ushort)dateTime.Day;
            WHour = (ushort)dateTime.Hour;
            WMinute = (ushort)dateTime.Minute;
            WSecond = (ushort)dateTime.Second;
            WMilliseconds = (ushort)dateTime.Millisecond;
        }

        public DateTime ToDateTime()
        {
            return new DateTime(WYear, WMonth, WDay, WHour, WMinute, WSecond);
        }
    }

    /// <summary>
    /// The thread's execution requirements. 
    /// </summary>
    /// <remarks>This parameter can be one or more of the following values.</remarks>
    enum ES_Flags : uint
    {
        /// <summary>
        /// Enables away mode. This value must be specified with ES_CONTINUOUS. 
        /// Away mode should be used only by media-recording and media-distribution applications that must perform critical background processing on desktop computers while the computer appears to be sleeping.
        /// </summary>
        /// <remarks>Windows Server 2003 and Windows XP/2000:  ES_AWAYMODE_REQUIRED is not supported.</remarks>
        ES_AWAYMODE_REQUIRED = 0x40,
        /// <summary>
        /// Informs the system that the state being set should remain in effect until the next call that uses ES_CONTINUOUS and one of the other state flags is cleared.
        /// </summary>
        ES_CONTINUOUS = 0x80000000,
        /// <summary>
        /// Forces the display to be on by resetting the display idle timer.
        /// </summary>
        ES_DISPLAY_REQUIRED = 0x2,
        /// <summary>
        /// Forces the system to be in the working state by resetting the system idle timer.
        /// </summary>
        ES_SYSTEM_REQUIRED = 0x1,
        /// <summary>
        /// This value is not supported. If ES_USER_PRESENT is combined with other esFlags values, the call will fail and none of the specified states will be set. 
        /// </summary>
        /// <remarks>Windows Server 2003 and Windows XP/2000:  Informs the system that a user is present and resets the display and system idle timers. ES_USER_PRESENT must be called with ES_CONTINUOUS.</remarks>
        ES_USER_PRESENT = 0x4
    }

    [Flags]
    enum MiniDumpTypes : uint
    {
        // From dbghelp.h:
        MiniDumpNormal = 0x00000000,
        MiniDumpWithDataSegs = 0x00000001,
        MiniDumpWithFullMemory = 0x00000002,
        MiniDumpWithHandleData = 0x00000004,
        MiniDumpFilterMemory = 0x00000008,
        MiniDumpScanMemory = 0x00000010,
        MiniDumpWithUnloadedModules = 0x00000020,
        MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
        MiniDumpFilterModulePaths = 0x00000080,
        MiniDumpWithProcessThreadData = 0x00000100,
        MiniDumpWithPrivateReadWriteMemory = 0x00000200,
        MiniDumpWithoutOptionalData = 0x00000400,
        MiniDumpWithFullMemoryInfo = 0x00000800,
        MiniDumpWithThreadInfo = 0x00001000,
        MiniDumpWithCodeSegs = 0x00002000,
        MiniDumpWithoutAuxiliaryState = 0x00004000,
        MiniDumpWithFullAuxiliaryState = 0x00008000,
        MiniDumpWithPrivateWriteCopyMemory = 0x00010000,
        MiniDumpIgnoreInaccessibleMemory = 0x00020000,
        MiniDumpValidTypeFlags = 0x0003ffff,
    };

    //typedef struct _MINIDUMP_EXCEPTION_INFORMATION {
    //    DWORD ThreadId;
    //    PEXCEPTION_POINTERS ExceptionPointers;
    //    BOOL ClientPointers;
    //} MINIDUMP_EXCEPTION_INFORMATION, *PMINIDUMP_EXCEPTION_INFORMATION;
    [StructLayout(LayoutKind.Sequential, Pack = 4)]  // Pack=4 is important! So it works also for x64!
    struct MiniDumpExceptionInformation
    {
        public uint ThreadId;
        public IntPtr ExceptioonPointers;
        [MarshalAs(UnmanagedType.Bool)]
        public bool ClientPointers;
    }
}
