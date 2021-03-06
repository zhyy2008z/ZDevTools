﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace ZDevTools.Collections
{
    /// <summary>
    /// 高效的块入块出队列类型（非线程安全类型）
    /// </summary>
    /// <remarks>
    /// 使用须知
    /// <para>本类型更侧重于Queue，对List的插入与移除操作性能支持不如List，因此在要求高性能的环境下不能代替List。</para>
    /// <para>本类型非线程安全类型，请君自己做好线程同步工作。</para>
    /// <para>本类型杂糅了Queue、List、Stack等类型的功能，一个类型可做多种用途。</para>
    /// <para>本类型为了更高的性能不会自动断开对已经被移除的元素的引用。也就是说，您如果向本队列存放引用类型的对象，他们可能有长期无法被释放的风险，您可以考虑在必要时手动调用<see cref="BufferQueue{T}.EraseExcess()"/>方法来释放这些引用。</para>
    /// </remarks>
    public class BufferQueue<T> : IList<T>, IReadOnlyList<T>, IList
    {
        #region Constructor & Fields & Properties
        /// <summary>
        /// 头部指针（或者左侧剩余空间【可能大于实际剩余空间】）
        /// </summary>
        private int _head;
        /// <summary>
        /// 尾部指针（或者左侧有值空间【可能大于实际有值空间】）
        /// </summary>
        private int _tail;
        /// <summary>
        /// 有值长度
        /// </summary>
        private int _length;
        /// <summary>
        /// 内部数据实际缓存
        /// </summary>
        private T[] _internalBuffer;
        /// <summary>
        /// 队列版本
        /// </summary>
        private int _version;

        /// <summary>
        /// 获取队列长度
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// 获取元素数量
        /// </summary>
        public int Count => _length;

        /// <summary>
        /// 获取或设置当前容量
        /// </summary>
        public int Capacity
        {
            get => _internalBuffer.Length;
            set
            {
                if (value < _length)
                    throw new ArgumentOutOfRangeException(nameof(value), "容量设置过小，无法保留队列中所有的元素。");

                if (value == _internalBuffer.Length) return;

                if (value > 0)
                {
                    var array = new T[value];
                    if (_length > 0)
                    {
                        int rightLength = getRightFilledLength();
                        if (rightLength >= _length) //这里之所以不用_head<_tail，是为了兼容列队满员时且_head==_tail==0这一情况，这种情况下也是可以一次性提取所有数据的
                        {
                            Array.Copy(_internalBuffer, _head, array, 0, _length);
                        }
                        else
                        {
                            Array.Copy(_internalBuffer, _head, array, 0, rightLength);
                            Array.Copy(_internalBuffer, 0, array, rightLength, _length - rightLength);
                        }
                    }
                    _internalBuffer = array;
                    _head = 0;
                    _tail = _length % _internalBuffer.Length;
                }
                else
                {
                    _internalBuffer = Array.Empty<T>();
                    _head = 0;
                    _tail = 0;
                }
                _version++;
            }
        }

        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => false;

        object _syncRoot;
        /// <inheritdoc/>
        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        /// <summary>
        /// 获取索引所在位置元素
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                throwIfOutOfRangeForAccess(index);
                var rightLength = getRightFilledLength();
                if (index < rightLength)
                    return _internalBuffer[_head + index];
                else
                    return _internalBuffer[index - rightLength];
            }
            set
            {
                throwIfOutOfRangeForAccess(index);
                var rightLength = getRightFilledLength();
                if (index < rightLength)
                    _internalBuffer[_head + index] = value;
                else
                    _internalBuffer[index - rightLength] = value;
                _version++;
            }
        }

        /// <summary>
        /// 构造一个新实例
        /// </summary>
        public BufferQueue() : this(2048) { }

        /// <summary>
        /// 指定默认缓存大小初始化一个T队列
        /// </summary>
        public BufferQueue(int capacity)
        {
            _internalBuffer = new T[capacity];
        }

        /// <summary>
        /// 使用已有序列构造一个新实例
        /// </summary>
        public BufferQueue(IEnumerable<T> items)
        {
            _internalBuffer = items.ToArray();
            _length = _internalBuffer.Length;
        }

        /// <summary>
        /// 使用一个已存在的元素数组作为内部缓存（注意：此数组本身及其所有元素将被本类接管，传入后您不再被允许修改此传入数组）
        /// </summary>
        public BufferQueue(T[] buffer)
        {
            _internalBuffer = buffer;
            _length = _internalBuffer.Length;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="BufferQueue{T}" />, if that number is less than a threshold value.
        /// </summary>
        public void TrimExcess()
        {
            int num = (int)(_internalBuffer.Length * 0.9);
            if (_length < num)
            {
                Capacity = _length;
            }
        }

        /// <summary>
        /// 擦除缓冲区中未存储实际元素的空间。由于本类型为了更高的性能不会自动断开对已经被移除的元素的引用，因此特别提供了本方法，方便您手动清除元素引用。一般来说，这个方法不需要调用，除非您的队列里保存了大量的引用类型的大对象（或者管理了非托管资源）。
        /// </summary>
        public void EraseExcess()
        {
            if (_head < _tail)
            {
                Array.Clear(_internalBuffer, 0, _head);
                Array.Clear(_internalBuffer, _tail, getRightFreeLength());
            }
            else if (_head > _tail)
            {
                Array.Clear(_internalBuffer, _tail, _head - _tail);
            }
            else if (_length == 0) //这里_head==_tail对应队列为空和队列满员两种情况，我们只需要处理队列为空就可以了。
            {
                Array.Clear(_internalBuffer, 0, _internalBuffer.Length);
            }
        }
        #endregion

        #region Enumerator
        /// <summary>
        /// 迭代器
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            readonly BufferQueue<T> Queue;
            readonly int Version;
            readonly int RightLength;
            int _index;
            T _current;

            /// <summary>
            /// 初始化一个新的迭代器
            /// </summary>
            public Enumerator(BufferQueue<T> queue)
            {
                this.Queue = queue;
                this.Version = queue._version;
                this.RightLength = queue.getRightFilledLength();
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
                if (this.Version != Queue._version)
                    throw new InvalidOperationException("集合已被修改！");

                if (_index == -2) return false;

                _index++;

                if (_index == Queue._length)
                {
                    _index = -2;
                    _current = default;
                    return false;
                }

                if (_index < RightLength)
                    _current = Queue._internalBuffer[Queue._head + _index];
                else
                    _current = Queue._internalBuffer[_index - RightLength];

                return true;
            }

            /// <inheritdoc/>
            public void Reset()
            {
                if (this.Version != Queue._version)
                    throw new InvalidOperationException("集合已被修改！");

                this._index = -1;
                this._current = default;
            }
        }

        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        #endregion

        #region Clear
        /// <summary>
        /// 清空 T 队列
        /// </summary>
        public void Clear()
        {
            _head = 0;
            _tail = 0;
            _length = 0;
            _version++;
        }

        /// <summary>
        /// 从出队位置清除length大小的T数据
        /// </summary>
        public void Clear(int length)
        {
            if (length > _length)
                length = _length;

            if (length == 0)
                return;

            _head = (_head + length) % _internalBuffer.Length;
            _length -= length;

            if (_length == 0)
            {
                _head = 0;
                _tail = 0;
            }

            _version++;

            return;
        }
        #endregion

        #region Enqueue
        /// <summary>
        /// 入队一个T
        /// </summary>
        /// <param name="content"></param>
        public void Enqueue(T content)
        {
            int newLength = _length + 1;
            if (newLength > _internalBuffer.Length)
                gainCapacity(newLength);

            _internalBuffer[_tail] = content;

            _tail = (_tail + 1) % _internalBuffer.Length;
            _length++;
            _version++;
        }

        //#if NETCOREAPP
        /// <summary>
        /// 入队一组T
        /// </summary>
        public void Enqueue(ReadOnlySpan<T> span)
        {
            if (span.Length == 0)
                return;

            int newLength = _length + span.Length;
            if (newLength > _internalBuffer.Length)
                gainCapacity(newLength);

            int rightFreeLength = getRightFreeLength();
            var bufferSpan = _internalBuffer.AsSpan();

            if (rightFreeLength >= span.Length)
            {
                span.CopyTo(bufferSpan.Slice(_tail));
            }
            else
            {
                span[..rightFreeLength].CopyTo(bufferSpan.Slice(_tail));
                span[rightFreeLength..].CopyTo(bufferSpan);
            }

            _tail = (_tail + span.Length) % _internalBuffer.Length;
            _length += span.Length;
            _version++;

            this.AddRange(new T[2]);
        }
        //#else
        //        /// <summary>
        //        /// 入队一组T
        //        /// </summary>
        //        public void Enqueue(ArraySegment<T> segment)
        //        {
        //            if (segment.Count == 0)
        //                return;

        //            int newLength = _length + segment.Count;
        //            if (newLength > _internalBuffer.Length)
        //                gainCapacity(newLength);

        //            int rightFreeLength = getRightFreeLength();

        //            if (rightFreeLength >= segment.Count)
        //            {
        //                Array.Copy(segment.Array, segment.Offset, _internalBuffer, _tail, segment.Count);
        //            }
        //            else
        //            {
        //                Array.Copy(segment.Array, segment.Offset, _internalBuffer, _tail, rightFreeLength);
        //                Array.Copy(segment.Array, segment.Offset + rightFreeLength, _internalBuffer, 0, segment.Count - rightFreeLength);
        //            }

        //            _tail = (_tail + segment.Count) % _internalBuffer.Length;
        //            _length += segment.Count;
        //            _version++;
        //        }
        //#endif
        #endregion

        #region Dequeue

        #region Out
        /// <summary>
        /// 当数据足够时，从队列中提取一个字节
        /// </summary>
        public bool Dequeue(out T content)
        {
            if (_length <= 0)
            {
                content = default;
                return false;
            }

            content = _internalBuffer[_head];

            _length--;

            if (_length == 0)
            {
                _head = 0;
                _tail = 0;
            }
            else
            {
                _head = (_head + 1) % _internalBuffer.Length;
            }

            _version++;

            return true;
        }

        /// <summary>
        /// 当数据足够时，从队列中提取指定数量的数据
        /// </summary>
        public bool Dequeue(int count, out T[] buffer)
        {
            if (count <= 0 || count > _length)
            {
                buffer = Array.Empty<T>();
                return count == 0;
            }

            int rightLength = getRightFilledLength();

            var array = new T[count];

            if (rightLength >= count)
            {
                Array.Copy(_internalBuffer, _head, array, 0, count);
            }
            else
            {
                Array.Copy(_internalBuffer, _head, array, 0, rightLength);
                Array.Copy(_internalBuffer, 0, array, rightLength, count - rightLength);
            }

            buffer = array;

            _length -= count;

            if (_length == 0)
            {
                _head = 0;
                _tail = 0;
            }
            else
            {
                _head = (_head + count) % _internalBuffer.Length;
            }

            _version++;

            return true;
        }

        /// <summary>
        /// 当数据足够时，从队列中提取指定数量的数据
        /// </summary>
        public bool Dequeue(int count, out ArraySegment<T> segment)
        {
            if (Dequeue(count, out T[] buffer))
            {
                segment = new ArraySegment<T>(buffer);
                return true;
            }
            else
            {
                segment = default;
                return false;
            }
        }

//#if NETCOREAPP
        /// <summary>
        /// 当数据足够时，从队列中提取指定数量的数据
        /// </summary>
        public bool Dequeue(int count, out Span<T> span)
        {
            if (Dequeue(count, out T[] buffer))
            {
                span = buffer;
                return true;
            }
            else
            {
                span = default;
                return false;
            }
        }

        /// <summary>
        /// 当数据足够时，从队列中提取指定数量的数据
        /// </summary>
        public bool Dequeue(int count, out Memory<T> memory)
        {
            if (Dequeue(count, out T[] buffer))
            {
                memory = buffer;
                return true;
            }
            else
            {
                memory = default;
                return false;
            }
        }
//#endif
        #endregion

        #region In
//#if NETCOREAPP
        /// <summary>
        /// 当数据足够时，从队列中提取数据
        /// </summary>
        public bool Dequeue(Span<T> span)
        {
            if (span.Length > _length)
                return false;

            if (span.Length == 0)
                return true;

            int rightLength = getRightFilledLength();
            var bufferSpan = _internalBuffer.AsSpan();

            if (rightLength >= span.Length)
            {
                bufferSpan.Slice(_head, span.Length).CopyTo(span);
            }
            else
            {
                bufferSpan.Slice(_head, rightLength).CopyTo(span);
                bufferSpan[..(span.Length - rightLength)].CopyTo(span.Slice(rightLength));
            }

            _length -= span.Length;

            if (_length == 0)
            {
                _head = 0;
                _tail = 0;
            }
            else
            {
                _head = (_head + span.Length) % _internalBuffer.Length;
            }

            _version++;

            return true;
        }
//#else
        ///// <summary>
        ///// 当数据足够时，从队列中提取数据
        ///// </summary>
        //public bool Dequeue(ArraySegment<T> segment)
        //{
        //    if (segment.Count > _length)
        //        return false;

        //    if (segment.Count == 0)
        //        return true;

        //    int rightLength = getRightFilledLength();

        //    if (rightLength >= segment.Count)
        //    {
        //        Array.Copy(_internalBuffer, _head, segment.Array, segment.Offset, segment.Count);
        //    }
        //    else
        //    {
        //        Array.Copy(_internalBuffer, _head, segment.Array, segment.Offset, rightLength);
        //        Array.Copy(_internalBuffer, 0, segment.Array, segment.Offset + rightLength, segment.Count - rightLength);
        //    }

        //    _length -= segment.Count;

        //    if (_length == 0)
        //    {
        //        _head = 0;
        //        _tail = 0;
        //    }
        //    else
        //    {
        //        _head = (_head + segment.Count) % _internalBuffer.Length;
        //    }

        //    _version++;

        //    return true;
        //}
//#endif
        #endregion

        #endregion

        #region Peek

        #region Out
        /// <summary>
        /// 查看队首的T
        /// </summary>
        public bool Peek(out T content)
        {
            if (_length < 1)
            {
                content = default;
                return false;
            }

            content = _internalBuffer[_head];
            return true;
        }

        /// <summary>
        /// 查看指定位置T
        /// </summary>
        public bool Peek(int index, out T content)
        {
            if ((uint)index >= (uint)_length)
            {
                content = default;
                return false;
            }

            var rightLength = getRightFilledLength();
            if (index < rightLength)
            {
                content = _internalBuffer[index];
            }
            else
            {
                content = _internalBuffer[index - rightLength];
            }

            return true;
        }

        /// <summary>
        /// 从队头读取数据
        /// </summary>
        public bool Peek(int count, out T[] buffer)
        {
            if (count <= 0 || count > _length)
            {
                buffer = Array.Empty<T>();
                return false;
            }

            int rightLength = getRightFilledLength();

            var array = new T[count];

            if (rightLength >= count)
            {
                Array.Copy(_internalBuffer, _head, array, 0, count);
            }
            else
            {
                Array.Copy(_internalBuffer, _head, array, 0, rightLength);
                Array.Copy(_internalBuffer, 0, array, rightLength, count - rightLength);
            }

            buffer = array;

            return true;
        }

        /// <summary>
        /// 从队列指定位置读取指定数量的数据
        /// </summary>
        public bool Peek(int index, int count, out T[] buffer)
        {
            if (count <= 0 || index < 0 || index + count >= _length)
            {
                buffer = Array.Empty<T>();
                return false;
            }

            int rightLength = getRightFilledLength();

            var array = new T[count];

            if (rightLength - index >= count)
            {
                Array.Copy(_internalBuffer, _head + index, array, 0, count);
            }
            else if (index < rightLength)
            {
                Array.Copy(_internalBuffer, _head + index, array, 0, rightLength - index);
                Array.Copy(_internalBuffer, 0, array, rightLength - index, count - (rightLength - index));
            }
            else
            {
                Array.Copy(_internalBuffer, index - rightLength, array, 0, count);
            }

            buffer = array;

            return true;
        }

        /// <summary>
        /// 从队头读取数据
        /// </summary>
        public bool Peek(int count, out ArraySegment<T> segment)
        {
            if (Peek(count, out T[] buffer))
            {
                segment = new ArraySegment<T>(buffer);
                return true;
            }
            else
            {
                segment = default;
                return false;
            }
        }

        /// <summary>
        /// 从队列指定位置读取指定数量的数据
        /// </summary>
        public bool Peek(int index, int count, out ArraySegment<T> segment)
        {
            if (Peek(index, count, out T[] buffer))
            {
                segment = new ArraySegment<T>(buffer);
                return true;
            }
            else
            {
                segment = default;
                return false;
            }
        }

//#if NETCOREAPP
        /// <summary>
        /// 从队头读取数据
        /// </summary>
        public bool Peek(int count, out Span<T> span)
        {
            if (Peek(count, out T[] buffer))
            {
                span = buffer;
                return true;
            }
            else
            {
                span = default;
                return false;
            }
        }

        /// <summary>
        /// 从队头读取数据
        /// </summary>
        public bool Peek(int count, out Memory<T> memory)
        {
            if (Peek(count, out T[] buffer))
            {
                memory = buffer;
                return true;
            }
            else
            {
                memory = default;
                return false;
            }
        }

        /// <summary>
        /// 从队列指定位置读取指定数量的数据
        /// </summary>
        public bool Peek(int index, int count, out Span<T> span)
        {
            if (Peek(index, count, out T[] buffer))
            {
                span = buffer;
                return true;
            }
            else
            {
                span = default;
                return false;
            }
        }

        /// <summary>
        /// 从队列指定位置读取指定数量的数据
        /// </summary>
        public bool Peek(int index, int count, out Memory<T> memory)
        {
            if (Peek(index, count, out T[] buffer))
            {
                memory = buffer;
                return true;
            }
            else
            {
                memory = default;
                return false;
            }
        }
//#endif

        #endregion

        #region In
//#if NETCOREAPP
        /// <summary>
        /// 从队头读取数据
        /// </summary>
        public bool Peek(Span<T> span)
        {
            if (span.Length == 0 || span.Length > _length)
                return false;

            int rightLength = getRightFilledLength();
            var bufferSpan = _internalBuffer.AsSpan();
            if (rightLength >= span.Length)
            {
                bufferSpan.Slice(_head, span.Length).CopyTo(span);
            }
            else
            {
                bufferSpan.Slice(_head, rightLength).CopyTo(span);
                bufferSpan[..(span.Length - rightLength)].CopyTo(span.Slice(rightLength));
            }
            return true;
        }

        /// <summary>
        /// 从指定位置读取数据
        /// </summary>
        public bool Peek(int index, Span<T> span)
        {
            if (span.Length == 0 || index < 0 || index + span.Length >= _length)
                return false;

            int rightLength = getRightFilledLength();
            var bufferSpan = _internalBuffer.AsSpan();
            if (rightLength - index >= span.Length)
                bufferSpan.Slice(_head + index, span.Length).CopyTo(span);
            else if (index < rightLength)
            {
                bufferSpan.Slice(_head + index, rightLength - index).CopyTo(span);
                bufferSpan[..(span.Length - (rightLength - index))].CopyTo(span.Slice(rightLength - index));
            }
            else
            {
                bufferSpan.Slice(index - rightLength, span.Length).CopyTo(span);
            }
            return true;
        }
//#else
        ///// <summary>
        ///// 从队头读取数据
        ///// </summary>
        //public bool Peek(ArraySegment<T> segment)
        //{
        //    if (segment.Count == 0 || segment.Count > _length)
        //        return false;

        //    int rightLength = getRightFilledLength();

        //    if (rightLength >= segment.Count)
        //    {
        //        Array.Copy(_internalBuffer, _head, segment.Array, segment.Offset, segment.Count);
        //    }
        //    else
        //    {
        //        Array.Copy(_internalBuffer, _head, segment.Array, segment.Offset, rightLength);
        //        Array.Copy(_internalBuffer, 0, segment.Array, segment.Offset + rightLength, segment.Count - rightLength);
        //    }
        //    return true;
        //}

        ///// <summary>
        ///// 从指定位置读取数据
        ///// </summary>
        //public bool Peek(int index, ArraySegment<T> segment)
        //{
        //    if (segment.Count == 0 || index < 0 || index + segment.Count > _length)
        //        return false;

        //    int rightLength = getRightFilledLength();
        //    if (rightLength - index >= segment.Count)
        //    {
        //        Array.Copy(_internalBuffer, _head + index, segment.Array, segment.Offset, segment.Count);
        //    }
        //    else if (index < rightLength)
        //    {
        //        Array.Copy(_internalBuffer, _head + index, segment.Array, segment.Offset, rightLength - index);
        //        Array.Copy(_internalBuffer, 0, segment.Array, segment.Offset + rightLength - index, segment.Count - (rightLength - index));
        //    }
        //    else
        //    {
        //        Array.Copy(_internalBuffer, index - rightLength, segment.Array, segment.Offset, segment.Count);
        //    }
        //    return true;
        //}
//#endif
        #endregion

        #endregion

        #region To
        /// <summary>
        /// 将队列中的所有元素作为数组输出
        /// </summary>
        public T[] ToArray()
        {
            if (_length > 0)
            {
                var array = new T[_length];

                int rightLength = getRightFilledLength();
                if (rightLength >= _length)
                {
                    Array.Copy(_internalBuffer, _head, array, 0, _length);
                }
                else
                {
                    Array.Copy(_internalBuffer, _head, array, 0, rightLength);
                    Array.Copy(_internalBuffer, 0, array, rightLength, _length - rightLength);
                }

                return array;
            }
            else
            {
                return Array.Empty<T>();
            }
        }
        #endregion

        #region List

        #region Miscellaneous
        void ICollection<T>.Add(T item) => Enqueue(item);

        /// <summary>
        /// Returns a read-only System.Collections.ObjectModel.ReadOnlyCollection`1 wrapper for the current collection.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current System.Collections.Generic.List`1.</returns>
        public ReadOnlyCollection<T> AsReadOnly()
        {
            return new ReadOnlyCollection<T>(this);
        }

        /// <summary>
        /// 获取某个元素所在位置
        /// </summary>
        public int IndexOf(T item)
        {
            if (_length > 0)
            {
                var rightLength = getRightFilledLength();
                int index;
                if (rightLength >= _length)
                {
                    index = Array.IndexOf(_internalBuffer, item, _head, _length);
                    if (index > -1)
                        return index - _head;
                }
                else
                {
                    index = Array.IndexOf(_internalBuffer, item, _head);
                    if (index > -1) return index - _head;
                    index = Array.IndexOf(_internalBuffer, item, 0, _tail);
                    if (index > -1) return index + rightLength;
                }
            }
            return -1;
        }

        /// <summary>
        /// 获取某个元素从尾部开始搜索所在位置
        /// </summary>
        public int LastIndexOf(T item)
        {
            if (_length > 0)
            {
                var rightLength = getRightFilledLength();
                int index;
                if (rightLength >= _length)
                {
                    index = Array.LastIndexOf(_internalBuffer, item, _head + _length - 1, _length);
                    if (index > -1) return index - _head;
                }
                else
                {
                    index = Array.LastIndexOf(_internalBuffer, item, _tail - 1);
                    if (index > -1) return index + rightLength;
                    index = Array.LastIndexOf(_internalBuffer, item, _internalBuffer.Length - 1, rightLength);
                    if (index > -1) return index - _head;
                }
            }
            return -1;
        }

        /// <summary>
        /// 队列中是否存在指定元素
        /// </summary>
        public bool Contains(T item) => IndexOf(item) > -1;
        #endregion

        #region Insert
        /// <summary>
        /// 该方法性能开销较大
        /// </summary>
        public void Insert(int index, T item)
        {
            throwIfOutOfRangeForInsert(index);

            int newLength = _length + 1;
            if (newLength > _internalBuffer.Length)
                gainCapacity(newLength);

            if (index == _length) //尾部直接赋值
            {
                //先赋值再移动指针
                _internalBuffer[_tail] = item;
                _tail = (_tail + 1) % _internalBuffer.Length;
            }
            else if (index == 0) //头部直接赋值
            {
                //先移动指针再赋值
                _head = (_head - 1 + _internalBuffer.Length) % _internalBuffer.Length;
                _internalBuffer[_head] = item;
            }
            else
            {
                //执行到此处_head!=_tail，列队肯定不为空的。
                if (_head < _tail)
                {
                    var rightFreeLength = getRightFreeLength();
                    var allAvailable = rightFreeLength > 0 && _head > 0;
                    if (allAvailable && index >= _length / 2 || !allAvailable && rightFreeLength > 0) //都有足够空间，且更靠近尾部 或者 不都有足够的空间但尾部有空间
                    {
                        Array.Copy(_internalBuffer, _head + index, _internalBuffer, _head + index + 1, _length - index);
                        _internalBuffer[_head + index] = item;
                        _tail++;
                    }
                    else //头部有空间
                    {
                        Array.Copy(_internalBuffer, _head, _internalBuffer, _head - 1, index);
                        _internalBuffer[_head + index - 1] = item;
                        _head--;
                    }
                }
                else
                {
                    var rightLength = getRightFilledLength();
                    if (index < rightLength)//在头部侧插入
                    {
                        Array.Copy(_internalBuffer, _head, _internalBuffer, _head - 1, index);
                        _internalBuffer[_head + index - 1] = item;
                        _head--;
                    }
                    else //在尾部侧插入
                    {
                        Array.Copy(_internalBuffer, index - rightLength, _internalBuffer, index - rightLength + 1, _length - index);
                        _internalBuffer[index - rightLength] = item;
                        _tail++;
                    }
                }
            }
            _length++;
            _version++;
        }

//#if NETCOREAPP
        /// <summary>
        /// 入队一组T
        /// </summary>
        public void Insert(int index, ReadOnlySpan<T> span)
        {
            throwIfOutOfRangeForInsert(index);

            if (span.Length == 0)
                return;

            int newLength = _length + span.Length;
            if (newLength > _internalBuffer.Length)
                gainCapacity(newLength);

            var bufferSpan = _internalBuffer.AsSpan();
            if (index == _length) //尾部直接入队
            {
                int rightFreeLength = getRightFreeLength();

                if (rightFreeLength >= span.Length)
                {
                    span.CopyTo(bufferSpan.Slice(_tail));
                }
                else
                {
                    span[..rightFreeLength].CopyTo(bufferSpan.Slice(_tail));
                    span[rightFreeLength..].CopyTo(bufferSpan);
                }

                _tail = (_tail + span.Length) % _internalBuffer.Length;
            }
            else if (index == 0) //头部直接赋值
            {
                if (_head >= span.Length)
                {
                    span.CopyTo(bufferSpan.Slice(_head - span.Length));
                }
                else
                {
                    span[..^_head].CopyTo(bufferSpan[^(span.Length - _head)..]);
                    span[^_head..].CopyTo(bufferSpan);
                }
                _head = (_head - span.Length + _internalBuffer.Length) % _internalBuffer.Length;
            }
            else
            {
                //执行到此处_head!=_tail，列队肯定不为空的。
                if (_head < _tail)
                {
                    var rightFreeLength = getRightFreeLength();
                    var allAvailable = rightFreeLength >= span.Length && _head >= span.Length;
                    if (allAvailable && index >= _length / 2 || !allAvailable && rightFreeLength >= span.Length) //都有足够空间，且更靠近尾部 或者 不都有足够的空间但尾部有空间
                    {
                        Array.Copy(_internalBuffer, _head + index, _internalBuffer, _head + index + span.Length, _length - index);
                        span.CopyTo(bufferSpan.Slice(_head + index));
                        _tail += span.Length;
                    }
                    else //头部有空间
                    {
                        Array.Copy(_internalBuffer, _head, _internalBuffer, _head - span.Length, index);
                        span.CopyTo(bufferSpan.Slice(_head - span.Length + index));
                        _head -= span.Length;
                    }
                }
                else
                {
                    var rightLength = getRightFilledLength();
                    if (index < rightLength)//在头部侧插入
                    {
                        Array.Copy(_internalBuffer, _head, _internalBuffer, _head - span.Length, index);
                        span.CopyTo(bufferSpan.Slice(_head - span.Length + index));
                        _head -= span.Length;
                    }
                    else //在尾部侧插入
                    {
                        Array.Copy(_internalBuffer, index - rightLength, _internalBuffer, index - rightLength + span.Length, _length - index);
                        span.CopyTo(bufferSpan.Slice(index - rightLength));
                        _tail += span.Length;
                    }
                }
            }
            _length += span.Length;
            _version++;
        }
//#else
//        /// <summary>
//        /// 入队一组T
//        /// </summary>
//        public void Insert(int index, ArraySegment<T> segment)
//        {
//            throwIfOutOfRangeForInsert(index);

//            if (segment.Count == 0)
//                return;

//            int newLength = _length + segment.Count;
//            if (newLength > _internalBuffer.Length)
//                gainCapacity(newLength);

//            if (index == _length) //尾部直接入队
//            {
//                int rightFreeLength = getRightFreeLength();

//                if (rightFreeLength >= segment.Count)
//                {
//                    segment.CopyTo(_internalBuffer, _tail);
//                }
//                else
//                {
//                    segment.Slice(0, rightFreeLength).CopyTo(_internalBuffer, _tail);
//                    segment.Slice(rightFreeLength).CopyTo(_internalBuffer);
//                }

//                _tail = (_tail + segment.Count) % _internalBuffer.Length;
//            }
//            else if (index == 0) //头部直接赋值
//            {
//                if (_head >= segment.Count)
//                {
//                    segment.CopyTo(_internalBuffer, _head - segment.Count);
//                }
//                else
//                {
//                    segment.Slice(0, segment.Count - _head).CopyTo(_internalBuffer, _internalBuffer.Length - (segment.Count - _head));
//                    segment.Slice(segment.Count - _head).CopyTo(_internalBuffer);
//                }
//                _head = (_head - segment.Count + _internalBuffer.Length) % _internalBuffer.Length;
//            }
//            else
//            {
//                //执行到此处_head!=_tail，列队肯定不为空的。
//                if (_head < _tail)
//                {
//                    var rightFreeLength = getRightFreeLength();
//                    var allAvailable = rightFreeLength >= segment.Count && _head >= segment.Count;
//                    if (allAvailable && index >= _length / 2 || !allAvailable && rightFreeLength >= segment.Count) //都有足够空间，且更靠近尾部 或者 不都有足够的空间但尾部有空间
//                    {
//                        Array.Copy(_internalBuffer, _head + index, _internalBuffer, _head + index + segment.Count, _length - index);
//                        segment.CopyTo(_internalBuffer, _head + index);
//                        _tail += segment.Count;
//                    }
//                    else //头部有空间
//                    {
//                        Array.Copy(_internalBuffer, _head, _internalBuffer, _head - segment.Count, index);
//                        segment.CopyTo(_internalBuffer, _head - segment.Count + index);
//                        _head -= segment.Count;
//                    }
//                }
//                else
//                {
//                    var rightLength = getRightFilledLength();
//                    if (index < rightLength)//在头部侧插入
//                    {
//                        Array.Copy(_internalBuffer, _head, _internalBuffer, _head - segment.Count, index);
//                        segment.CopyTo(_internalBuffer, _head - segment.Count + index);
//                        _head -= segment.Count;
//                    }
//                    else //在尾部侧插入
//                    {
//                        Array.Copy(_internalBuffer, index - rightLength, _internalBuffer, index - rightLength + segment.Count, _length - index);
//                        segment.CopyTo(_internalBuffer, index - rightLength);
//                        _tail += segment.Count;
//                    }
//                }
//            }
//            _length += segment.Count;
//            _version++;
//        }
//#endif
        #endregion

        #region SetRange

//#if NETCOREAPP
        /// <summary>
        /// 设置指定位置一个区域的元素
        /// </summary>
        public void SetRange(int index, ReadOnlySpan<T> span)
        {
            throwIfOutOfRangeForAccess(index, span.Length);

            if (span.Length == 0) return;

            int rightLength = getRightFilledLength();
            var bufferSpan = _internalBuffer.AsSpan();
            if (rightLength - index >= span.Length)
                span.CopyTo(bufferSpan.Slice(_head + index));
            else if (index < rightLength)
            {
                span[..(rightLength - index)].CopyTo(bufferSpan.Slice(_head + index));
                span.Slice(rightLength - index).CopyTo(bufferSpan);
            }
            else
            {
                span.CopyTo(bufferSpan.Slice(index - rightLength));
            }
            _version++;
        }
//#else
//        /// <summary>
//        /// 设置指定位置一个区域的元素
//        /// </summary>
//        public void SetRange(int index, ArraySegment<T> segment)
//        {
//            throwIfOutOfRangeForAccess(index, segment.Count);

//            if (segment.Count == 0) return;

//            int rightLength = getRightFilledLength();
//            if (rightLength - index >= segment.Count)
//            {
//                Array.Copy(segment.Array, segment.Offset, _internalBuffer, _head + index, segment.Count);
//            }
//            else if (index < rightLength)
//            {
//                Array.Copy(segment.Array, segment.Offset, _internalBuffer, _head + index, rightLength - index);
//                Array.Copy(segment.Array, segment.Offset + rightLength - index, _internalBuffer, 0, segment.Count - (rightLength - index));
//            }
//            else
//            {
//                Array.Copy(segment.Array, segment.Offset, _internalBuffer, index - rightLength, segment.Count);
//            }
//            _version++;
//        }
//#endif
        #endregion

        #region Remove
        /// <summary>
        /// 在指定位置移除一个元素（性能开销较大）
        /// </summary>
        public void RemoveAt(int index)
        {
            throwIfOutOfRangeForAccess(index);

            if (index == _length - 1) //尾部直接移除
            {
                _tail = (_tail - 1 + _internalBuffer.Length) % _internalBuffer.Length;
            }
            else if (index == 0) //头部直接移除
            {
                _head = (_head + 1) % _internalBuffer.Length;
            }
            else
            {
                //执行到此处_head!=_tail，列队肯定不为空的。
                if (_head < _tail)
                {
                    if (index >= _length / 2) //更靠近尾部
                    {
                        Array.Copy(_internalBuffer, _head + index + 1, _internalBuffer, _head + index, _length - index - 1);
                        _tail--;
                    }
                    else //更靠近头部
                    {
                        Array.Copy(_internalBuffer, _head, _internalBuffer, _head + 1, index);
                        _head++;
                    }
                }
                else
                {
                    var rightLength = getRightFilledLength();
                    if (index < rightLength)//在头部侧移除
                    {
                        Array.Copy(_internalBuffer, _head, _internalBuffer, _head + 1, index);
                        _head++;
                    }
                    else //在尾部侧移除
                    {
                        Array.Copy(_internalBuffer, index - rightLength + 1, _internalBuffer, index - rightLength, _length - index - 1);
                        _tail--;
                    }
                }
            }
            _length--;
            _version++;
        }

        /// <summary>
        /// 在指定位置移除指定数量的元素（性能开销较大）
        /// </summary>
        public void RemoveRange(int index, int count)
        {
            throwIfOutOfRangeForAccess(index, count);

            if (count == 0) return;

            if (index == _length - count) //尾部直接移除
            {
                _tail = (_tail - count + _internalBuffer.Length) % _internalBuffer.Length;
            }
            else if (index == 0) //头部直接移除
            {
                _head = (_head + count) % _internalBuffer.Length;
            }
            else
            {
                //执行到此处_head!=_tail，列队肯定不为空的。
                if (_head < _tail)
                {
                    if (index >= _length / 2) //更靠近尾部
                    {
                        Array.Copy(_internalBuffer, _head + index + count, _internalBuffer, _head + index, _length - index - count);
                        _tail -= count;
                    }
                    else //更靠近头部
                    {
                        Array.Copy(_internalBuffer, _head, _internalBuffer, _head + count, index);
                        _head += count;
                    }
                }
                else
                {
                    var rightLength = getRightFilledLength();
                    if (index < rightLength)//在头部侧移除
                    {
                        Array.Copy(_internalBuffer, _head, _internalBuffer, _head + count, index);
                        _head += count;
                    }
                    else //在尾部侧移除
                    {
                        Array.Copy(_internalBuffer, index - rightLength + count, _internalBuffer, index - rightLength, _length - index - count);
                        _tail -= count;
                    }
                }
            }
            _length -= count;
            _version++;
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            int num = IndexOf(item);
            if (num >= 0)
            {
                RemoveAt(num);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate（性能不佳）。
        /// </summary>
        public int RemoveAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            int count = default;
            for (int i = _length - 1; i > -1; i--)
            {
                if (match(this[i]))
                {
                    RemoveAt(i);
                    count++;
                }
            }
            return count;
        }

        #endregion

        #region CopyTo
        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => copyTo(array, arrayIndex);

//#if NETCOREAPP
        /// <summary>
        /// 将整个队列复制到目标块
        /// </summary>
        public void CopyTo(Span<T> span)
        {
            if (span.Length < _length)
                throw new ArgumentException("目标块长度不足以储存本队列所有元素。");

            if (_length > 0)
            {
                int rightLength = getRightFilledLength();
                var bufferSpan = _internalBuffer.AsSpan();
                if (rightLength >= _length)
                {
                    bufferSpan.Slice(_head, _length).CopyTo(span);
                }
                else
                {
                    bufferSpan.Slice(_head).CopyTo(span[..rightLength]);
                    bufferSpan[.._tail].CopyTo(span[rightLength..]);
                }
            }
        }
//#else
//        /// <summary>
//        /// 将整个队列复制到目标数组片段
//        /// </summary>
//        public void CopyTo(ArraySegment<T> segment)
//        {
//            if (segment.Count < _length)
//                throw new ArgumentException("目标数组长度不足以储存本队列所有元素。");

//            if (_length > 0)
//            {
//                int rightLength = getRightFilledLength();
//                if (rightLength >= _length)
//                {
//                    Array.Copy(_internalBuffer, _head, segment.Array, segment.Offset, _length);
//                }
//                else
//                {
//                    Array.Copy(_internalBuffer, _head, segment.Array, segment.Offset, rightLength);
//                    Array.Copy(_internalBuffer, 0, segment.Array, segment.Offset + rightLength, _length - rightLength);
//                }
//            }
//        }
//#endif
        #endregion

        #region IList
        /// <inheritdoc/>
        bool ICollection.IsSynchronized => false;

        int ICollection.Count => this.Count;

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        object IList.this[int index] { get => this[index]; set => this[index] = (T)value; }

        int IList.Add(object value)
        {
            this.Add((T)value);
            return Count - 1;
        }

        bool IList.Contains(object value)
        {
            if (value is T || (value == null && default(T) == null))
                return this.Contains((T)value);
            else
                return false;
        }

        int IList.IndexOf(object value)
        {
            if (value is T || (value == null && default(T) == null))
                return this.IndexOf((T)value);
            else
                return -1;
        }

        void IList.Insert(int index, object value) => this.Insert(index, (T)value);

        void IList.Remove(object value)
        {
            if (value is T || (value == null && default(T) == null))
                this.Remove((T)value);
        }

        void ICollection.CopyTo(Array array, int index) => copyTo(array, index);

        #endregion

        #endregion

        #region Stack
        /// <summary>
        /// 从栈中弹出一个元素
        /// </summary>
        public bool Pop(out T item)
        {
            if (_length <= 0)
            {
                item = default;
                return false;
            }

            item = _internalBuffer[_head];

            _length--;

            if (_length == 0)
            {
                _head = 0;
                _tail = 0;
            }
            else
            {
                _head = (_head + 1) % _internalBuffer.Length;
            }

            _version++;

            return true;
        }

        /// <summary>
        /// 向栈中压入一个元素
        /// </summary>
        public void Push(T item)
        {
            int newLength = _length + 1;
            if (newLength > _internalBuffer.Length)
                gainCapacity(newLength);

            //先移动指针再赋值
            _head = (_head - 1 + _internalBuffer.Length) % _internalBuffer.Length;
            _internalBuffer[_head] = item;
            _length++;
            _version++;
        }
        #endregion

        #region Helpers
        /// <summary>
        /// 增加缓存大小
        /// </summary>
        void gainCapacity(int newLength)
        {
            int capacity = _internalBuffer.Length == 0 ? 4 : _internalBuffer.Length;
            do
                capacity *= 2;
            while (capacity < newLength);

            T[] newBuffer = new T[capacity];

            if (_length > 0)
            {
                var rightLength = getRightFilledLength();
                if (rightLength >= _length)
                {
                    Array.Copy(_internalBuffer, _head, newBuffer, 0, _length);
                }
                else
                {
                    Array.Copy(_internalBuffer, _head, newBuffer, 0, rightLength);
                    Array.Copy(_internalBuffer, 0, newBuffer, rightLength, _tail);
                }
            }

            _head = 0;
            _tail = _length; //此时肯定有未占用的空间，因此直接赋值为_length是没有问题的，不可能出现_head==_tail的情况
            _internalBuffer = newBuffer;
        }

        /// <summary>
        /// 获取从头部算起的右侧有值空间（该值可能会大于_length）
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int getRightFilledLength() => _internalBuffer.Length - _head;

        /// <summary>
        /// 获取尾部剩余空白空间
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int getRightFreeLength() => _internalBuffer.Length - _tail;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void throwIfOutOfRangeForAccess(int index)
        {
            if ((uint)index >= (uint)_length)
                throw new ArgumentOutOfRangeException(nameof(index));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void throwIfOutOfRangeForAccess(int index, int count)
        {
            if ((uint)index >= (uint)_length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0 || index + count >= _length)
                throw new ArgumentOutOfRangeException(nameof(count));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void throwIfOutOfRangeForInsert(int index)
        {
            if ((uint)index > (uint)_length)
                throw new ArgumentOutOfRangeException(nameof(index));
        }

        void copyTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (array.Length - arrayIndex < _length)
                throw new ArgumentException("目标数组长度不足以储存本队列所有元素。");

            if (_length > 0)
            {
                int rightLength = getRightFilledLength();
                if (rightLength >= _length)
                {
                    Array.Copy(_internalBuffer, _head, array, arrayIndex, _length);
                }
                else
                {
                    Array.Copy(_internalBuffer, _head, array, arrayIndex, rightLength);
                    Array.Copy(_internalBuffer, 0, array, arrayIndex + rightLength, _length - rightLength);
                }
            }
        }
        #endregion
    }
}
