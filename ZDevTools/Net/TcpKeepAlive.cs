using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace ZDevTools.Net
{
    /// <summary>
    /// TCP心跳设置
    /// </summary>
    public struct TcpKeepAlive
    {
        /// <summary>
        /// 初始化一个心跳设置
        /// </summary>
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
        /// 连接空闲多少毫秒（从最后一次数据交互之后算起至当前）后开始进行探测
        /// </summary>
        //[FieldOffset(4)]
        public uint KeepAliveTime;

        /// <summary>
        /// 探测间隔时间，单位：毫秒，探测次数由操作系统决定，这些探测均失败后断开连接
        /// </summary>
        //[FieldOffset(8)]
        public uint KeepAliveInterval;


        /// <summary>
        /// 将设置转化为Byte数组
        /// </summary>
        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(IsOn)
            .Concat(BitConverter.GetBytes(KeepAliveTime))
            .Concat(BitConverter.GetBytes(KeepAliveInterval))
            .ToArray();
        }
    }
}
