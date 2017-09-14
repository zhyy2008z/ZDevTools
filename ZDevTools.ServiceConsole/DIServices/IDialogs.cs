using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZDevTools.ServiceConsole.DIServices
{
    using ServiceCore;
    using Schedules;


    public interface IDialogs
    {

        /// <summary>
        /// 显示计划列表对话框
        /// </summary>
        /// <param name="canManage">是否可以管理计划</param>
        /// <param name="schedules">初始计划列表，计划列表对话框对此列表进行修改</param>
        /// <returns>用户是否确认保存计划列表</returns>
        List<BasicSchedule> ShowSchedulesDialog(bool canManage, List<BasicSchedule> schedules);

        /// <summary>
        /// 显示一键启动对话框
        /// </summary>
        /// <param name="configs">配置字典</param>
        void ShowOneKeyStartConfigDialog(Dictionary<string, ServiceItemConfig> configs);

        /// <summary>
        /// 显示计划设置对话框
        /// </summary>
        /// <param name="basicSchedule">基本计划</param>
        /// <returns></returns>
        BasicSchedule ShowScheduleDialog(BasicSchedule basicSchedule);
    }
}
