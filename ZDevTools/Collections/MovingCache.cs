using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ZDevTools.Collections
{
    /// <summary>
    /// 移动缓存（支持固定缓存大小模式，推入多余数据后旧数据自动出队，为超高性能而生，不支持foreach集合版本变更检测机制）
    /// </summary>
    public class MovingCache<T> : IEnumerable<T>
    {
        /// <summary>
        /// 缓存
        /// </summary>
        T[] _buffer;
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

            _buffer = new T[capacity];
        }

        /// <summary>
        /// 移动缓存的容量
        /// </summary>
        public int Capacity => _buffer.Length;

        /// <summary>
        /// 缓存是否可读（缓存已存满）
        /// </summary>
        public int Length => _isFull ? _buffer.Length : _position;

        bool _isFull;
        /// <summary>
        /// 当前缓存是否已存满
        /// </summary>
        public bool IsFull => _isFull;

        /// <summary>
        /// 获取指定索引处元素
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (_isFull)
                {
                    var rightLength = _buffer.Length - _position;
                    if (index < rightLength)
                        return _buffer[_position + index];
                    else
                        return _buffer[index - rightLength];
                }
                else
                    return _buffer[index];
            }
        }

        /// <summary>
        /// 向移动缓存泵入一个元素
        /// </summary>
        /// <param name="value">元素</param>
        public void Enqueue(T value)
        {
            _buffer[_position] = value;
            _position = (_position + 1) % _buffer.Length;
            if (_position == 0)
                _isFull = true;
        }

        /// <summary>
        /// 拷贝到数组
        /// </summary>
        public T[] ToArray()
        {
            T[] result = new T[Length];
            if (_isFull)
            {
                var rightLength = getRightLength();
                Array.Copy(_buffer, _position, result, 0, rightLength);
                Array.Copy(_buffer, 0, result, rightLength, _position);
            }
            else
                Array.Copy(_buffer, result, Length);
            return result;
        }

        /// <summary>
        /// 获取从头部算起的右侧有值空间（该值可能会大于_length）
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int getRightLength() => _buffer.Length - _position;

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

                if (_index == Cache.Length)
                {
                    _index = -2;
                    _current = default;
                    return false;
                }

                if (Cache._isFull)
                    if (_index < RightLength)
                        _current = Cache._buffer[Cache._position + _index];
                    else
                        _current = Cache._buffer[_index - RightLength];
                else
                    _current = Cache._buffer[_index];

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
