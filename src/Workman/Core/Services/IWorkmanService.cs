using Workman.Core.Entities;

namespace Workman.Core.Services
{
    public interface IWorkmanService
    {
        Task<WorkProject?> CreateProject(string name);

        Task<bool> DeleteProject(int projectId);

        Task<List<WorkProject>> GetProjects();

        Task<WorkProject?> GetProject(int projectId);

        Task<WorkProject?> UpdateProject(int projectId, string name);

        Task<WorkTask?> CreateTask(int projectId, string content);

        Task<bool> DeleteTask(int taskId);

        Task<List<WorkTask>> GetTasks();

        Task<List<WorkTask>> GetTasks(int projectId);

        Task<WorkTask?> GetTask(int taskId);

        Task<float> GetTaskElapsedTime(int taskId);

        Task<WorkTask?> UpdateTask(int taskId, int projectId, string content);

        Task<WorkLog?> CreateLog(DateTime date, int taskId, string content, float elapsedTime);

        Task<bool> DeleteLog(int logId);

        Task<List<WorkLog>> GetLogs(DateTime date);

        Task<List<WorkLog>> GetLogs(DateTime startDate, DateTime endDate);

        Task<WorkLog?> GetLog(int logId);

        Task<WorkLog?> UpdateLog(int logId, int taskId, string content, float elapsedTime);
        Task<int> GetProjectTaskCount(int projectId);
        Task<float> GetProjectElapsedTime(int projectId);
    }
}
