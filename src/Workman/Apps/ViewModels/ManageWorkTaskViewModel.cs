using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Collections.ObjectModel;
using System.Windows;
using Workman.Apps.Helpers;
using Workman.Core.Entities;
using Workman.Core.Services;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("ManageWorkTaskView")]
    internal partial class ManageWorkTaskViewModel : ObservableObject, IDialogAware
    {
        private readonly IWorkmanService _workmanService;
        private readonly IDialogService _dialogService;

        public ManageWorkTaskViewModel(IWorkmanService workmanService, IDialogService dialogService)
        {
            _workmanService = workmanService;
            _dialogService = dialogService;
        }

        [ObservableProperty]
        private List<WorkProject> _projects;

        [ObservableProperty]
        private WorkProject _selectedProject;

        [ObservableProperty]
        private ObservableCollection<WorkTaskVO> _tasks;


        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private void CreateTask()
        {
            _dialogService.ShowDialog("CreateWorkTaskView", async dr =>
            {
                if (dr.Result == ButtonResult.OK)
                {
                    await RefreshTasks(SelectedProject.Id);
                }
            });
        }

        [RelayCommand]
        private void EditTask(WorkTaskVO? task)
        {
            if (task == null)
            {
                return;
            }
            _dialogService.ShowDialog("UpdateWorkTaskView", 
                                      new DialogParameters
                                      {
                                          {"workTaskId", task.Id }
                                      },
                                      async dr =>
                                      {
                                          if (dr.Result == ButtonResult.OK)
                                          {
                                              await RefreshTasks(SelectedProject.Id);
                                          }
                                      });
        }

        [RelayCommand]
        private void ManageLog(WorkTaskVO? task)
        {
            if (task == null)
            {
                return;
            }
            _dialogService.ShowDialog("ManageWorkLogView", 
                                      new DialogParameters
                                      {
                                          {"workTaskId", task.Id }
                                      },
                                      async dr =>
                                      {
                                          if (dr.Result == ButtonResult.OK)
                                          {
                                              await RefreshTasks(SelectedProject.Id);
                                          }
                                      });
        }

        [RelayCommand]
        private async Task DeleteTask(WorkTaskVO? task)
        {
            if (task == null)
            {
                return;
            }
            MessageBoxResult messageBoxResult = MessageHelper.ShowOKCancel(LocalizationManager.Instance.DeleteTaskHint);
            if (messageBoxResult != MessageBoxResult.OK)
            {
                return;
            }
            bool success = await _workmanService.DeleteTask(task.Id);
            if (!success)
            {
                MessageHelper.ShowError(string.Format(LocalizationManager.Instance.FailedMessage, LocalizationManager.Instance.Delete));
                return;
            }
            Tasks.Remove(task);
        }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose.Invoke(ButtonResult.Cancel);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        partial void OnSelectedProjectChanged(WorkProject value)
        {
            RefreshTasks(value.Id);
        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            IEnumerable<WorkProject> projects = await _workmanService.GetProjects();
            Projects = new List<WorkProject>(projects)
            {
                new WorkProject
                {
                    Id = -1,
                    Name = LocalizationManager.Instance.All,
                },
            };
            bool success = parameters.TryGetValue("workProjectId", out int projectId);
            if (!success)
            {
                SelectedProject = Projects.LastOrDefault()!;
            }
            else
            {
                SelectedProject = Projects.FirstOrDefault(p => p.Id == projectId)!;
            }
            if (SelectedProject != null)
            {
                await RefreshTasks(SelectedProject.Id);
            }
        }

        private async Task RefreshTasks(int projectId)
        {
            List<WorkTask> workTasks = projectId == -1
                ? await _workmanService.GetTasks()
                : await _workmanService.GetTasks(projectId);
            List<WorkTaskVO> taskVOs = new List<WorkTaskVO>();
            IEnumerable<WorkProject> projects = await _workmanService.GetProjects();
            List<WorkProjectVO> projectVOs = new List<WorkProjectVO>();
            foreach (WorkProject project in projects)
            {
                projectVOs.Add(new WorkProjectVO
                {
                    Id = project.Id,
                    ArchivedTime = project.ArchivedTime,
                    CreatedTime = project.CreatedTime,
                    Name = project.Name,
                    IsArchived = project.IsArchived,
                });
            }
            int orderId = 1;
            foreach (WorkTask wt in workTasks)
            {
                float taskElapsedTime = await _workmanService.GetTaskElapsedTime(wt.Id);
                WorkTaskVO taskVO = new WorkTaskVO
                {
                    OrderId = orderId++,
                    Id = wt.Id,
                    ArchivedTime = wt.ArchivedTime,
                    CreatedTime = wt.CreatedTime,
                    Name = wt.Name,
                    IsArchived = wt.IsArchived,
                    TotalElapsedTime = taskElapsedTime,
                    Project = projectVOs.FirstOrDefault(p => p.Id == wt.ProjectId)!,
                };
                taskVOs.Add(taskVO);
            }
            Tasks = new ObservableCollection<WorkTaskVO>(taskVOs);
            Tasks.CollectionChanged += (s, e) =>
            {
                for (int i = 0; i < Tasks.Count; i++)
                {
                    Tasks[i].OrderId = i + 1;
                }
            };
        }
    }
}
