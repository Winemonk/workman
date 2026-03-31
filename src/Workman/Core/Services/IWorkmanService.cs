using Workman.Core.Entities;

namespace Workman.Core.Services
{
    public interface IWorkmanService
    {
        Task<WorkLog?> CreateLog(DateTime date, int taskId, string content, float elapsedTime);

        Task<WorkProject?> CreateProject(string name);

        Task<WorkTask?> CreateTask(int projectId, string content);

        Task<bool> DeleteLog(int logId);

        Task<bool> DeleteProject(int projectId);

        Task<bool> DeleteTask(int taskId);

        Task<WorkLog?> GetLog(int logId);

        Task<List<WorkLog>> GetLogs(int taskId);

        Task<List<WorkLog>> GetLogs(DateTime date);

        bool TheDayHasLogs(DateTime date);

        Task<List<WorkLog>> GetLogs(DateTime startDate, DateTime endDate);

        Task<WorkProject?> GetProject(int projectId);

        Task<float> GetProjectElapsedTime(int projectId);

        Task<List<WorkProject>> GetProjects();

        Task<List<WorkProject>> GetNotArchivedProjects();

        Task<int> GetProjectTaskCount(int projectId);

        Task<WorkTask?> GetTask(int taskId);

        Task<float> GetTaskElapsedTime(int taskId);

        Task<List<WorkTask>> GetTasks();

        Task<List<WorkTask>> GetNotArchivedTasks();

        Task<List<WorkTask>> GetTasks(int projectId);

        Task<List<WorkTask>> GetNotArchivedTasks(int projectId);

        Task<WorkLog?> UpdateLog(int logId, int taskId, string content, float elapsedTime, DateTime date);

        Task<WorkProject?> UpdateProject(int projectId, string name, bool isArchived);

        Task<WorkTask?> UpdateTask(int taskId, int projectId, string content, bool isArchived);
    }
}
