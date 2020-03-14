using System;
using System.Collections.Generic;

namespace ZDevTools.NetCore
{
    /// <summary>
    /// 消息总线接口（要求仅使用UI线程调用订阅与取消订阅接口），用来传递消息
    /// </summary>
    public interface IMessageBus
    {
        void Send<TMessage>(object messageType, TMessage message);

        IEnumerable<TResult> Send<TMessage, TResult>(object messageType, TMessage message);

        void Send(object messageType);

        IEnumerable<TResult> Send<TResult>(object messageType);

        void Subscribe<TMessage>(object messageType, Action<TMessage> listener);
        void Subscribe<TMessage, TResult>(object messageType, Func<TMessage, TResult> listener);
        void Subscribe(object messageType, Action listener);
        void Subscribe<TResult>(object messageType, Func<TResult> listener);

        void Unsubscribe<TMessage>(object messageType, Action<TMessage> listener);
        void Unsubscribe<TMessage, TResult>(object messageType, Func<TMessage, TResult> listener);
        void Unsubscribe(object messageType, Action listener);
        void Unsubscribe<TResult>(object messageType, Func<TResult> listener);
    }
}
