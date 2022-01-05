using System.Collections.Generic;

namespace HangfireTemplate.Tasks
{
    public class MyTaskOptions
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 任务列表
        /// </summary>
        public List<TaskOption> Tasks { get; set; }
    }

    public class TaskOption
    {
        /// <summary>
        /// 任务名
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// Cron表达式
        /// </summary>
        public string Cron { get; set; }
    }
}