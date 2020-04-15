using System;
using System.Threading;
using System.Windows.Forms;

namespace ZDevTools.WindowsForms
{
    /// <summary>
    /// 界面任务管理器
    /// </summary>
    public interface IUITaskManager
    {
        /// <summary>
        /// 获取取消UI任务的Token令牌，外部代码调用Token的ThrowIfCancellationRequested()方法抛出异常立即终止其余代码执行，或者获取IsCancellationRequested来确定是否继续执行剩余代码。
        /// </summary>
        CancellationToken Token { get; }

        /// <summary>
        /// UI操作的目标窗体。
        /// </summary>
        Form UITarget { get; set; }

        /// <summary>
        /// 向任务管理器添加任务
        /// </summary>
        /// <param name="action"></param>
        void AddTask(Action action);

        /// <summary>
        /// 在当前操作线程需要Invoke时，调用Invoke执行控件操作。
        /// </summary>
        /// <param name="uiAction">UI操作委托</param>
        void DoSafeUIWork(Action uiAction);

        /// <summary>
        /// 发出终止所有线程执行的信号，并等待所有线程完成其任务，一般在窗体的Closing事件中间接调用，以等待其他线程完成其任务后再关闭窗体。
        /// </summary>
        void SignalAllStopAndWait();

        /// <summary>
        /// 发送终止信号并等待所有任务完成后向窗体发送关闭信号，返回值为窗体的Closing事件参数的Cancel应该发送的值，以确保在所有UI线程完成后再关闭本窗体。
        /// </summary>
        /// <returns>窗体的Closing事件参数的Cancel应该发送的值</returns>
        bool SignalAllStopAndWaitForClose();
    }
}