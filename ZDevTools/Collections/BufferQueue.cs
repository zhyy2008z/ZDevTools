using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ZDevTools.Collections
{
    /// <summary>
    /// 高效的块入块出队列类型（非线程安全类型）
    /// </summary>
    public class BufferQueue<T> : IEnumerable<T>
    {
        #region Constructor & Fields
        private int _head;
        private int _tail;
        private int _length;
        private T[] _internalBuffer;

        /// <summary>
        /// 获取队列长度
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// 构造一个新实例
        /// </summary>
        public BufferQueue() : this(2048) { }

        /// <summary>
        /// 指定默认缓存大小初始化一个T队列
        /// </summary>
        /// <param name="capacity"></param>
        public BufferQueue(int capacity)
        {
            _internalBuffer = new T[capacity];
        }
        #endregion

        #region Clear
        /// <summary>
        /// 清空 T 队列
        /// </summary>
        internal void Clear()
        {
            _head = 0;
            _tail = 0;
            _length = 0;
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
            if (_length + 1 > _internalBuffer.Length)
                gainCapacity(_internalBuffer.Length * 2);

            _internalBuffer[_tail] = content;

            _tail = (_tail + 1) % _internalBuffer.Length;
            _length += 1;
        }

#if NETCOREAPP
        /// <summary>
        /// 入队一组T
        /// </summary>
        public void Enqueue(ReadOnlySpan<T> span)
        {
            if (span.Length == 0)
                return;

            if (_length + span.Length > _internalBuffer.Length)
                gainCapacity(_internalBuffer.Length * 2);

            int rightLength = getRightLengthFromTail();

            if (rightLength >= span.Length)
            {
                span.CopyTo(_internalBuffer.AsSpan().Slice(_tail));
            }
            else
            {
                span[..rightLength].CopyTo(_internalBuffer.AsSpan().Slice(_tail));
                span[rightLength..].CopyTo(_internalBuffer);
            }

            _tail = (_tail + span.Length) % _internalBuffer.Length;
            _length += span.Length;
        }
#else
        /// <summary>
        /// 入队一组T
        /// </summary>
        public void Enqueue(ArraySegment<T> segment)
        {
            if (segment.Count == 0)
                return;

            if ((_length + segment.Count) > _internalBuffer.Length)
                gainCapacity(_internalBuffer.Length * 2);

            int rightLength = getRightLengthFromTail();

            if (rightLength >= segment.Count)
            {
                Array.Copy(segment.Array, segment.Offset, _internalBuffer, _tail, segment.Count);
            }
            else
            {
                Array.Copy(segment.Array, segment.Offset, _internalBuffer, _tail, rightLength);
                Array.Copy(segment.Array, segment.Offset + rightLength, _internalBuffer, 0, segment.Count - rightLength);
            }

            _tail = (_tail + segment.Count) % _internalBuffer.Length;
            _length += segment.Count;
        }
#endif
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

            _length -= 1;

            if (_length == 0)
            {
                _head = 0;
                _tail = 0;
            }
            else
            {
                _head = (_head + 1) % _internalBuffer.Length;
            }

            return true;
        }

        /// <summary>
        /// 当数据足够时，从队列中提取指定数量的数据
        /// </summary>
        public bool Dequeue(int count, out T[] buffer)
        {
            if (count <= 0 || count > _length)
            {
                buffer = default;
                return false;
            }

            int rightLength = getRightLengthFromHead();

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

#if NETCOREAPP
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
#endif
        #endregion

        #region In
#if NETCOREAPP
        /// <summary>
        /// 当数据足够时，从队列中提取数据
        /// </summary>
        public bool Dequeue(Span<T> span)
        {
            if (span.Length == 0 || span.Length > _length)
                return false;

            int rightLength = getRightLengthFromHead();

            if (rightLength >= span.Length)
            {
                _internalBuffer.AsSpan().Slice(_head, span.Length).CopyTo(span);
            }
            else
            {
                _internalBuffer.AsSpan().Slice(_head, rightLength).CopyTo(span);
                _internalBuffer.AsSpan()[..(span.Length - rightLength)].CopyTo(span.Slice(rightLength));
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

            return true;
        }
#else
        /// <summary>
        /// 当数据足够时，从队列中提取数据
        /// </summary>
        public bool Dequeue(ArraySegment<T> segment)
        {
            if (segment.Count == 0 || segment.Count > _length)
                return false;

            int rightLength = getRightLengthFromHead();

            if (rightLength >= segment.Count)
            {
                Array.Copy(_internalBuffer, _head, segment.Array, segment.Offset, segment.Count);
            }
            else
            {
                Array.Copy(_internalBuffer, _head, segment.Array, segment.Offset, rightLength);
                Array.Copy(_internalBuffer, 0, segment.Array, segment.Offset + rightLength, segment.Count - rightLength);
            }

            _length -= segment.Count;

            if (_length == 0)
            {
                _head = 0;
                _tail = 0;
            }
            else
            {
                _head = (_head + segment.Count) % _internalBuffer.Length;
            }

            return true;
        }
#endif
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
        /// 从队头读取数据
        /// </summary>
        public bool Peak(int count, out T[] buffer)
        {
            if (count <= 0 || count > _length)
            {
                buffer = default;
                return false;
            }

            int rightLength = getRightLengthFromHead();

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
        /// 从队头读取数据
        /// </summary>
        public bool Peak(int count, out ArraySegment<T> segment)
        {
            if (Peak(count, out T[] buffer))
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

#if NETCOREAPP
        /// <summary>
        /// 从队头读取数据
        /// </summary>
        public bool Peak(int count, out Span<T> span)
        {
            if (Peak(count, out T[] buffer))
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
        public bool Peak(int count, out Memory<T> memory)
        {
            if (Peak(count, out T[] buffer))
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
#endif

        #endregion

        #region In
#if NETCOREAPP
        /// <summary>
        /// 从队头读取数据
        /// </summary>
        public bool Peak(Span<T> span)
        {
            if (span.Length == 0 || span.Length > _length)
                return false;

            int rightLength = getRightLengthFromHead();
            if (rightLength >= span.Length)
            {
                _internalBuffer.AsSpan().Slice(_head, span.Length).CopyTo(span);
            }
            else
            {
                _internalBuffer.AsSpan().Slice(_head, rightLength).CopyTo(span);
                _internalBuffer.AsSpan()[..(span.Length - rightLength)].CopyTo(span.Slice(rightLength));
            }
            return true;
        }
#else
        /// <summary>
        /// 从队头读取数据
        /// </summary>
        public bool Peak(ArraySegment<T> segment)
        {
            if (segment.Count == 0 || segment.Count > _length)
                return false;

            int rightLength = getRightLengthFromHead();

            if (rightLength >= segment.Count)
            {
                Array.Copy(_internalBuffer, _head, segment.Array, segment.Offset, segment.Count);
            }
            else
            {
                Array.Copy(_internalBuffer, _head, segment.Array, segment.Offset, rightLength);
                Array.Copy(_internalBuffer, 0, segment.Array, segment.Offset + rightLength, segment.Count - rightLength);
            }
            return true;
        }
#endif
        #endregion

        #endregion

        #region Enumerable
        /// <summary>
        /// 枚举T队列
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _length; i++)
            {
                var rightLength = getRightLengthFromHead();
                if (i < rightLength)
                {
                    yield return _internalBuffer[_head + i];
                }
                else
                {
                    yield return _internalBuffer[i - rightLength];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// 增加缓存大小
        /// </summary>
        void gainCapacity(int capacity)
        {
            T[] newBuffer = new T[capacity];

            if (_length > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_internalBuffer, _head, newBuffer, 0, _length);
                }
                else
                {
                    Array.Copy(_internalBuffer, _head, newBuffer, 0, _internalBuffer.Length - _head);
                    Array.Copy(_internalBuffer, 0, newBuffer, _internalBuffer.Length - _head, _tail);
                }
            }

            _head = 0;
            _tail = _length;
            _internalBuffer = newBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int getRightLengthFromHead() => _internalBuffer.Length - _head;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int getRightLengthFromTail() => _internalBuffer.Length - _tail;
        #endregion
    }
}
