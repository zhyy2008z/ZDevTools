using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ZDevTools.Collections
{
    /// <summary>
    /// 移动缓存（支持固定缓存大小模式，推入多余数据后旧数据自动出队，为超高性能而生，不支持foreach集合版本变更检测机制，非线程安全类型）
    /// </summary>     
    /// <remarks>
    /// 使用须知
    /// <para>本类型为了更高的性能不会自动断开对已经被移除的元素的引用。也就是说，您如果向本队列存放引用类型的对象，他们可能有长期无法被释放的风险，您可以考虑在必要时手动调用<see cref="MovingCache{T}.EraseExcess()"/>方法来释放这些引用。</para>
    /// </remarks>
    public class MovingCache<T> : IEnumerable<T>
    {
        /// <summary>
        /// 缓存
        /// </summary>
        readonly T[] Buffer;

        /// <summary>
        /// 当前位置
        /// </summary>
        int _position;

        /// <summary>
        /// 通过指定容量初始化一个移动缓存实例
        /// </summary>
        /// <param name="capacity">容量</param>
        public MovingCache(int capacity)
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException(nameof(capacity), "容量不能小于1。");

            Buffer = new T[capacity];
        }

        /// <summary>
        /// 移动缓存的容量
        /// </summary>
        public int Capacity => Buffer.Length;

        /// <summary>
        /// 当前缓存已存储数量
        /// </summary>
        public int Count => _isFull ? Buffer.Length : _position;

        bool _isFull;
        /// <summary>
        /// 当前缓存是否已存满
        /// </summary>
        public bool IsFull => _isFull;

        /// <summary>
        /// 获取或设置指定索引处元素
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get => getElement(index);
            set => getElement(index) = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref T getElement(int index)
        {
            if ((uint)index >= (uint)Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (_isFull)
            {
                var rightLength = Buffer.Length - _position;
                if (index < rightLength)
                    return ref Buffer[_position + index];
                else
                    return ref Buffer[index - rightLength];
            }
            else
                return ref Buffer[index];
        }

        /// <summary>
        /// 向移动缓存泵入一个元素
        /// </summary>
        /// <param name="value">元素</param>
        public void Enqueue(T value)
        {
            Buffer[_position] = value;
            _position = (_position + 1) % Buffer.Length;
            if (_position == 0)
                _isFull = true;
        }

        /// <summary>
        /// 拷贝到数组
        /// </summary>
        public T[] ToArray()
        {
            T[] result = new T[Count];
            if (_isFull)
            {
                var rightLength = getRightLength();
                Array.Copy(Buffer, _position, result, 0, rightLength);
                Array.Copy(Buffer, 0, result, rightLength, _position);
            }
            else
                Array.Copy(Buffer, result, Count);
            return result;
        }

        /// <summary>
        /// 重置本类型到无数据状态（仅重置内部指针，不实际清除内部缓存内容，如需清除请在调用此方法后调用<see cref="EraseExcess()"/>方法）
        /// </summary>
        public void Clear()
        {
            _isFull = default;
            _position = default;
        }

        /// <summary>
        /// 擦除缓冲区中未存储实际元素的空间。由于本类型为了更高的性能不会自动断开对已经被移除的元素的引用，因此特别提供了本方法，方便您手动清除元素引用。一般来说，这个方法不需要调用，除非您的队列里保存了大量的引用类型的大对象或者管理了非托管资源。
        /// </summary>
        public void EraseExcess()
        {
            if (!_isFull)
                Array.Clear(Buffer, _position, Buffer.Length - _position);
        }

        /// <summary>
        /// 获取从头部算起的右侧有值空间（该值可能会大于_length）
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int getRightLength() => Buffer.Length - _position;

        #region Enumerator
        /// <summary>
        /// 获取迭代器
        /// </summary>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// 迭代器
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            readonly MovingCache<T> Cache;
            readonly int RightLength;
            int _index;
            T _current;

            /// <summary>
            /// 初始化一个新的迭代器
            /// </summary>
            public Enumerator(MovingCache<T> cache)
            {
                this.Cache = cache;
                this.RightLength = cache.getRightLength();
                this._index = -1;
                this._current = default;
            }

            /// <inheritdoc/>
            public T Current
            {
                get
                {
                    if (_index < 0)
                    {
                        if (_index == -1)
                            throw new InvalidOperationException("枚举尚未开始。");
                        else
                            throw new InvalidOperationException("枚举已结束。");
                    }
                    else
                        return _current;
                }
            }

            /// <inheritdoc/>
            object IEnumerator.Current
            {
                get
                {
                    if (_index < 0)
                    {
                        if (_index == -1)
                            throw new InvalidOperationException("枚举尚未开始。");
                        else
                            throw new InvalidOperationException("枚举已结束。");
                    }
                    else
                        return _current;
                }
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                _index = -2;
                _current = default;
            }

            /// <inheritdoc/>
            public bool MoveNext()
            {
                if (_index == -2) return false;

                _index++;

                if (_index == Cache.Count)
                {
                    _index = -2;
                    _current = default;
                    return false;
                }

                if (Cache._isFull)
                    if (_index < RightLength)
                        _current = Cache.Buffer[Cache._position + _index];
                    else
                        _current = Cache.Buffer[_index - RightLength];
                else
                    _current = Cache.Buffer[_index];

                return true;
            }

            /// <inheritdoc/>
            public void Reset()
            {
                this._index = -1;
                this._current = default;
            }
        }
        #endregion
    }
}
