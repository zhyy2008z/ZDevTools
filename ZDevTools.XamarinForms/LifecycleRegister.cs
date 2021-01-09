using System;
using System.Collections.Generic;

using Microsoft.Extensions.Hosting;

namespace ZDevTools.XamarinForms
{
    public class LifecycleRegister : ILifecycleRegister
    {
        readonly HashSet<Action> Callbacks = new HashSet<Action>();

        public void Register(Action callback)
        {
            Callbacks.Add(callback);
        }

        public void Notify()
        {
            foreach (var callback in Callbacks)
            {
                callback.Invoke();
            }
        }
    }
}
