using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZDevTools.ServiceCore;

namespace ZDevTools.ServiceConsole
{
    /// <summary>
    /// 自制读写锁（专为需要同时获取多个读写锁而设计）
    /// </summary>
    public class MyReaderWriterLock
    {
        /// <summary>
        /// 內部同步锁
        /// </summary>
        static readonly object InnerLock = new object();
        /// <summary>
        /// 所有的等待锁
        /// </summary>
        static readonly List<AutoResetEvent> waitAllLocks = new List<AutoResetEvent>();

        /// <summary>
        /// 获得读取锁个数
        /// </summary>
        public int ReadingCount { get; private set; }

        /// <summary>
        /// 是否已经获取写入锁
        /// </summary>
        public bool IsWritting { get; private set; }

        /// <summary>
        /// 同时进入多个锁
        /// </summary>
        /// <param name="locks">锁们</param>
        /// <param name="actions">每个锁对应的动作</param>
        /// <param name="timeOut">超时（毫秒）</param>
        /// <returns></returns>
        public static bool EnterLocks(MyReaderWriterLock[] locks, RequestAction[] actions, int timeOut)
        {
            int tick = Environment.TickCount;
            AutoResetEvent are = null;
            try
            {
                lock (InnerLock)
                {
                    are = new AutoResetEvent(false);
                    waitAllLocks.Add(are);
                }
                while (true)
                {
                    lock (InnerLock)
                    {
                        //检查是否可以获取所有锁，如果不能，就等待下次检查
                        int i;
                        for (i = 0; i < locks.Length; i++)
                        {
                            var lo = locks[i];
                            var action = actions[i];
                            if (action == RequestAction.Read) //读
                            {
                                if (lo.IsWritting)
                                    break;
                            }
                            else //写
                            {
                                if (lo.ReadingCount > 0 || lo.IsWritting)  //在读或者写都是不行的
                                    break;
                            }
                        }

                        if (i == locks.Length) //检测通过，加锁
                        {
                            for (i = 0; i < locks.Length; i++)
                            {
                                var lo = locks[i];
                                var action = actions[i];
                                if (action == RequestAction.Read)
                                    lo.ReadingCount++;
                                else
                                    lo.IsWritting = true;
                            }
                            return true;
                        }
                    }
                    if (!are.WaitOne(timeOut) || Environment.TickCount - tick >= timeOut)
                        return false;
                }
            }
            finally
            {
                lock (InnerLock)
                {
                    waitAllLocks.Remove(are);
                    are.Dispose();
                }
            }
        }

        /// <summary>
        /// 同时释放多个锁
        /// </summary>
        /// <param name="locks">锁们</param>
        /// <param name="actions">要释放的动作</param>
        public static void LeaveLocks(MyReaderWriterLock[] locks, RequestAction[] actions)
        {
            lock (InnerLock)
            {
                for (int i = 0; i < locks.Length; i++)
                {
                    var lo = locks[i];
                    var action = actions[i];
                    if (action == RequestAction.Read)
                        lo.ReadingCount--;
                    else
                        lo.IsWritting = false;
                }
                foreach (var waitAllLocker in waitAllLocks)
                    waitAllLocker.Set();
            }
        }
    }

}
