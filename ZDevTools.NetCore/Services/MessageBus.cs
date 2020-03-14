using System;
using System.Collections.Generic;

namespace ZDevTools.NetCore.Services
{

    class MessageBus : IMessageBus
    {
        public readonly Dictionary<object, Delegate> MessageHandlerDic = new Dictionary<object, Delegate>();

        public void Send<TMessage>(object messageType, TMessage message)
        {
            if (MessageHandlerDic.TryGetValue(messageType, out var dlg) && dlg != null)
                ((Action<TMessage>)dlg).Invoke(message);
        }

        public IEnumerable<TResult> Send<TMessage, TResult>(object messageType, TMessage message)
        {
            if (MessageHandlerDic.TryGetValue(messageType, out var dlg) && dlg != null)
                foreach (Func<TMessage, TResult> func in dlg.GetInvocationList())
                    yield return func(message);
        }

        public void Send(object messageType)
        {
            if (MessageHandlerDic.TryGetValue(messageType, out var dlg) && dlg != null)
                ((Action)dlg).Invoke();
        }

        public IEnumerable<TResult> Send<TResult>(object messageType)
        {
            if (MessageHandlerDic.TryGetValue(messageType, out var dlg) && dlg != null)
                foreach (Func<TResult> func in dlg.GetInvocationList())
                    yield return func();
        }

        public void Subscribe<TMessage>(object messageType, Action<TMessage> listener) => subscribe(messageType, listener);
        public void Subscribe<TMessage, TResult>(object messageType, Func<TMessage, TResult> listener) => subscribe(messageType, listener);
        public void Subscribe(object messageType, Action listener) => subscribe(messageType, listener);
        public void Subscribe<TResult>(object messageType, Func<TResult> listener) => subscribe(messageType, listener);

        public void Unsubscribe<TMessage>(object messageType, Action<TMessage> listener) => unsubscribe(messageType, listener);
        public void Unsubscribe<TMessage, TResult>(object messageType, Func<TMessage, TResult> listener) => unsubscribe(messageType, listener);
        public void Unsubscribe(object messageType, Action listener) => unsubscribe(messageType, listener);
        public void Unsubscribe<TResult>(object messageType, Func<TResult> listener) => unsubscribe(messageType, listener);

        void subscribe(object messageType, Delegate listener)
        {
            if (MessageHandlerDic.TryGetValue(messageType, out var handler))
                MessageHandlerDic[messageType] = Delegate.Combine(handler, listener);
            else
                MessageHandlerDic[messageType] = listener;
        }

        void unsubscribe(object messageType, Delegate listener)
        {
            if (MessageHandlerDic.TryGetValue(messageType, out var handler))
                MessageHandlerDic[messageType] = Delegate.Remove(handler, listener);
        }
    }
}
