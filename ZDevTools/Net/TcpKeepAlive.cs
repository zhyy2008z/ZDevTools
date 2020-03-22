using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace ZDevTools.Net
{
    public struct TcpKeepAlive
    {

        public TcpKeepAlive(bool isOn, uint idleTime, uint retryInterval)
        {
            this.IsOn = Convert.ToUInt32(isOn);
            this.KeepAliveTime = idleTime;
            this.KeepAliveInterval = retryInterval;
        }

        //[FieldOffset(0)]
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        //public unsafe fixed byte Bytes[12];

        /// <summary>
        /// 是否开启存活性探测机制（默认无需开启）
        /// </summary>
        //[FieldOffset(0)]
        public uint IsOn;

        /// <summary>
        /// 空闲多少毫秒后开始进行探测
        /// </summary>
        //[FieldOffset(4)]
        public uint KeepAliveTime;

        /// <summary>
        /// 探测间隔时间，单位：毫秒
        /// </summary>
        //[FieldOffset(8)]
        public uint KeepAliveInterval;


        public byte[] GetBytes()
        {
            return BitConverter.GetBytes(IsOn)
            .Concat(BitConverter.GetBytes(KeepAliveTime))
            .Concat(BitConverter.GetBytes(KeepAliveInterval))
            .ToArray();
        }
    }
}
