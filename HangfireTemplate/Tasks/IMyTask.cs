using System.Threading.Tasks;

namespace HangfireTemplate.Tasks
{
    public interface IMyTask
    {
        /// <summary>
        /// 获取任务名称
        /// </summary>
        /// <returns></returns>
        string GetTaskName();

        /// <summary>
        /// 启动任务
        /// </summary>
        /// <param name="cron"></param>
        /// <returns></returns>
        void Start(string cron);

        /// <summary>
        /// 工作方法
        /// </summary>
        /// <returns></returns>
        Task WorkAsync();
    }
}