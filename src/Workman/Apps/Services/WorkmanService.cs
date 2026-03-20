using Hearth.Prism.Toolkit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.Logging;
using Workman.Core.Entities;
using Workman.Core.Repositories;
using Workman.Core.Services;

namespace Workman.Apps.Services
{
    [RegisterService(ServiceType = typeof(IWorkmanService), Lifetime = ServiceLifetime.Scoped)]
    internal class WorkmanService : IWorkmanService
    {
        private readonly IRepository<WorkLog> _logRepository;
        private readonly IRepository<WorkProject> _projectRepository;
        private readonly IRepository<WorkTask> _taskRepository;
        public WorkmanService(IRepository<WorkProject> projectRepository,
                              IRepository<WorkTask> taskRepository,
                              IRepository<WorkLog> logRepository)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _logRepository = logRepository;
        }

        public Task<WorkLog?> CreateLog(DateTime date, int taskId, string content, float elapsedTime)
        {
            return _logRepository.Insert(new WorkLog
            {
                Date = date,
                TaskId = taskId,
                ElapsedTime = elapsedTime,
                Content = content,
            });
        }

        public Task<WorkProject?> CreateProject(string name)
        {
            return _projectRepository.Insert(new WorkProject
            {
                Name = name,
                CreatedTime = DateTime.Now,
                IsArchived = false,
            });
        }

        public Task<WorkTask?> CreateTask(int projectId, string content)
        {
            return _taskRepository.Insert(new WorkTask
            {
                ProjectId = projectId,
                Name = content,
                CreatedTime = DateTime.Now,
                IsArchived = false,
            });
        }

        public Task<bool> DeleteLog(int logId)
        {
            return _logRepository.Delete(logId);
        }

        public Task<bool> DeleteProject(int projectId)
        {
            return _projectRepository.Delete(projectId);
        }

        public Task<bool> DeleteTask(int taskId)
        {
            return _taskRepository.Delete(taskId);
        }

        public Task<WorkLog?> GetLog(int logId)
        {
            return _logRepository.Query(logId);
        }

        public async Task<List<WorkLog>> GetLogs(int taskId)
        {
            IEnumerable<WorkLog> workLogs = await _logRepository.QueryRange(q => q.Where(l => l.TaskId == taskId).OrderBy(l => l.Date));
            return workLogs.ToList();
        }

        public async Task<List<WorkLog>> GetLogs(DateTime date)
        {
            IEnumerable<WorkLog> workLogs = await _logRepository.QueryRange(q => q.Where(l => l.Date >= date && l.Date < date.AddDays(1)).OrderBy(l => l.Date));
            return workLogs.ToList();
        }

        public async Task<List<WorkLog>> GetLogs(DateTime startDate, DateTime endDate)
        {
            IEnumerable<WorkLog> workLogs = await _logRepository.QueryRange(q => q.Where(l => l.Date >= startDate && l.Date <= endDate).OrderBy(l => l.Date));
            return workLogs.ToList();
        }

        public Task<WorkProject?> GetProject(int projectId)
        {
            return _projectRepository.Query(projectId);
        }

        public async Task<float> GetProjectElapsedTime(int projectId)
        {
            IEnumerable<WorkTask> tasks = await _taskRepository.QueryRange(q => q.Where(t => t.ProjectId == projectId));
            IEnumerable<int> taskIds = tasks.Select(t => t.Id);
            IEnumerable<WorkLog> workLogs = await _logRepository.QueryRange(q => q.Where(l => taskIds.Contains(l.TaskId)));
            return workLogs.Sum(l => l.ElapsedTime);
        }

        public async Task<List<WorkProject>> GetProjects()
        {
            IEnumerable<WorkProject> workProjects = await _projectRepository.QueryRange();
            return workProjects.ToList();
        }

        public async Task<List<WorkProject>> GetNotArchivedProjects()
        {
            IEnumerable<WorkProject> workProjects = await _projectRepository.QueryRange(q => q.Where(p => !p.IsArchived));
            return workProjects.ToList();
        }

        public async Task<int> GetProjectTaskCount(int projectId)
        {
            IEnumerable<WorkTask> tasks = await _taskRepository.QueryRange(q => q.Where(t => t.ProjectId == projectId));
            return tasks.Count();
        }
        public Task<WorkTask?> GetTask(int taskId)
        {
            return _taskRepository.Query(taskId);
        }

        public async Task<float> GetTaskElapsedTime(int taskId)
        {
            IEnumerable<WorkLog> workLogs = await _logRepository.QueryRange(q => q.Where(l => l.TaskId == taskId));
            return workLogs.Sum(l => l.ElapsedTime);
        }

        public async Task<List<WorkTask>> GetTasks()
        {
            IEnumerable<WorkTask> workTasks = await _taskRepository.QueryRange();
            return workTasks.ToList();
        }

        public async Task<List<WorkTask>> GetNotArchivedTasks()
        {
            IEnumerable<WorkTask> workTasks = await _taskRepository.QueryRange(q => q.Where(t => !t.IsArchived));
            return workTasks.ToList();
        }

        public async Task<List<WorkTask>> GetTasks(int projectId)
        {
            IEnumerable<WorkTask> workTasks = await _taskRepository.QueryRange(q => q.Where(t => t.ProjectId == projectId));
            return workTasks.ToList();
        }

        public async Task<List<WorkTask>> GetNotArchivedTasks(int projectId)
        {
            IEnumerable<WorkTask> workTasks = await _taskRepository.QueryRange(q => q.Where(t => t.ProjectId == projectId && !t.IsArchived));
            return workTasks.ToList();
        }

        public async Task<WorkLog?> UpdateLog(int logId, int taskId, string content, float elapsedTime, DateTime date)
        {
            WorkLog? workLog = await _logRepository.Query(logId);
            if (workLog == null)
            {
                return null;
            }
            workLog.TaskId = taskId;
            workLog.ElapsedTime = elapsedTime;
            workLog.Content = content;
            workLog.Date = date;
            await _logRepository.Update(workLog);
            return workLog;
        }

        public async Task<WorkProject?> UpdateProject(int projectId, string name, bool isArchived)
        {
            WorkProject? project = await _projectRepository.Query(projectId);
            if (project == null)
            {
                return null;
            }
            project.Name = name;
            if(project.IsArchived != isArchived)
            {
                project.IsArchived = isArchived;
                if (isArchived)
                {
                    project.ArchivedTime = DateTime.Now;
                    List<WorkTask> tasks = await GetTasks(projectId);
                    foreach (WorkTask task in tasks)
                    {
                        WorkTask? updatedTask = await UpdateTask(task.Id, task.ProjectId, task.Name, true);
                    }
                }
                else
                {
                    project.ArchivedTime = null;
                }
            }
            WorkProject updatedProject = await _projectRepository.Update(project);
            return updatedProject;
        }

        public async Task<WorkTask?> UpdateTask(int taskId, int projectId, string content, bool isArchived)
        {
            WorkTask? workTask = await _taskRepository.Query(taskId);
            if(workTask == null)
            {
                return null;
            }
            workTask.ProjectId = projectId;
            workTask.Name = content;
            if (workTask.IsArchived != isArchived)
            {
                workTask.IsArchived = isArchived;
                if (isArchived)
                {
                    workTask.ArchivedTime = DateTime.Now;
                }
                else
                {
                    workTask.ArchivedTime = null;
                }
            }
            await _taskRepository.Update(workTask);
            return workTask;
        }
    }
}
