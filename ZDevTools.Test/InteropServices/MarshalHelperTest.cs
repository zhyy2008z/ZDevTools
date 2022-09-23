using System.Linq;
using System.Runtime.InteropServices;
using System;

using Xunit;

using ZDevTools.InteropServices;
using System.Data.SqlTypes;
using System.Collections.Generic;

namespace ZDevTools.Test.InteropServices
{
    public class MarshalHelperTest
    {
        [Fact]
        public void IsReferenceOrContainsReferences()
        {
            Assert.False(MarshalHelper.IsReferenceOrContainsReferences<StructA>());
            Assert.True(MarshalHelper.IsReferenceOrContainsReferences<ClassB>());
            Assert.True(MarshalHelper.IsReferenceOrContainsReferences<MonoC>());
        }

        [Fact]
        public void StructureToBytes()
        {
            Assert.Equal(new byte[] { 22, 0, 0, 0 }, MarshalHelper.StructureToBytes(new StructA() { A = 22 }));

            Assert.Equal(new byte[] { 0, 0, 0, 0, 23, 0, 0, 0 }, MarshalHelper.StructureToBytes(new ClassE()
            {
                MeasurementDatas = new[] {
                    new StructF() { SignalValue4 = 23 }
                }
            }));

            Assert.Equal(new byte[] { 32, 0, 0, 0 }, MarshalHelper.StructureToBytes(32));
        }

        [Fact]
        public void StructureFromBytes()
        {
            Assert.Equal(new StructA() { A = 22 }, MarshalHelper.StructureFromBytes<StructA>(new byte[] { 22, 0, 0, 0 }));

            Assert.Equal(new ClassE()
            {
                MeasurementDatas = new[] {
                    new StructF() { SignalValue4 = 23 }
                }
            }, MarshalHelper.StructureFromBytes<ClassE>(new byte[] { 0, 0, 0, 0, 23, 0, 0, 0 }));

            Assert.Equal(32, MarshalHelper.StructureFromBytes<int>(new byte[] { 32, 0, 0, 0 }));
        }

        [Fact]
        public void ArrayToBytes()
        {
            Assert.Equal(new byte[] { 22, 0, 0, 0 }, MarshalHelper.ArrayToBytes(new StructA[] { new StructA() { A = 22 } }));

            Assert.Equal(new byte[] { 0, 0, 0, 0, 23, 0, 0, 0 }, MarshalHelper.ArrayToBytes(new[]{ new ClassE()
            {
                MeasurementDatas = new[] {
                    new StructF() { SignalValue4 = 23 }
                }
            }}));

            Assert.Equal(new byte[] { 32, 0, 0, 0 }, MarshalHelper.ArrayToBytes(new[] { 32 }));
        }

        [Fact]
        public void ArrayFromBytes()
        {
            Assert.Equal(new[] { new StructA() { A = 22 } }.AsEnumerable(), MarshalHelper.ArrayFromBytes<StructA>(new byte[] { 22, 0, 0, 0 }));

            Assert.Equal(new[]{ new ClassE()
            {
                MeasurementDatas = new[] {
                    new StructF() { SignalValue4 = 23 }
                }
            } }.AsEnumerable(), MarshalHelper.ArrayFromBytes<ClassE>(new byte[] { 0, 0, 0, 0, 23, 0, 0, 0 }));

            Assert.Equal(new[] { 32 }.AsEnumerable(), MarshalHelper.ArrayFromBytes<int>(new byte[] { 32, 0, 0, 0 }));
        }
    }

    struct StructA
    {
        public int A { get; set; }
    }


    class ClassB
    {

    }

    struct MonoC
    {
        public int B { get; set; }

        public ClassB ClassB { get; set; }
    }

    enum EnumD
    {
        A,
        B,
        C
    }

    [StructLayout(LayoutKind.Sequential)]
    class ClassE : IEquatable<ClassE>
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
        StructF[] _measurementDatas;

        /// <summary>
        /// 测量数据（数组长度：2）
        /// </summary>
        public StructF[] MeasurementDatas { get => _measurementDatas; set => _measurementDatas = value; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ClassE);
        }

        public bool Equals(ClassE other)
        {
            return other is not null &&
                 this.MeasurementDatas[0].SignalValue4 == other.MeasurementDatas[0].SignalValue4 &&
                 this.MeasurementDatas[0].A == other.MeasurementDatas[0].A;
        }

        public override int GetHashCode()
        {
            return MeasurementDatas[0].SignalValue4.GetHashCode() ^ MeasurementDatas[0].A.GetHashCode();
        }

        public static bool operator ==(ClassE left, ClassE right)
        {
            return EqualityComparer<ClassE>.Default.Equals(left, right);
        }

        public static bool operator !=(ClassE left, ClassE right)
        {
            return !(left == right);
        }
    }

    struct StructF
    {
        public int A { get; set; }

        /// 信号值 4（探测器信号）
        /// </summary>
        public short SignalValue4 { get; set; }

    }
}
