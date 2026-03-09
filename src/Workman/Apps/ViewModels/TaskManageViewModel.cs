using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using System.Collections.ObjectModel;
using System.Windows;
using Workman.Core.Entities;
using Workman.Core.Services;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("TaskManageView")]
    internal partial class TaskManageViewModel : ObservableObject, IDialogAware
    {
        private readonly IWorkmanService _workmanService;
        private readonly IDialogService _dialogService;

        public TaskManageViewModel(IWorkmanService workmanService, IDialogService dialogService)
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
                                          {"taskId", task.Id }
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
            MessageBoxResult messageBoxResult = MessageBox.Show("任务删除后相关日志也会删除，确认删除项目？",
                                                                "提示",
                                                                MessageBoxButton.OKCancel,
                                                                MessageBoxImage.Warning);
            if (messageBoxResult != MessageBoxResult.OK)
            {
                return;
            }
            bool success = await _workmanService.DeleteTask(task.Id);
            if (!success)
            {
                MessageBox.Show("删除任务失败！",
                                "提示",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
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
            SelectedProject = Projects.LastOrDefault();
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
            int orderId = 1;
            foreach (WorkTask wt in workTasks)
            {
                float taskElapsedTime = await _workmanService.GetTaskElapsedTime(wt.Id);
                WorkTaskVO taskVO = new WorkTaskVO
                {
                    OrderId = orderId++,
                    Id = wt.Id,
                    Name = wt.Name,
                    TotalElapsedTime = taskElapsedTime,
                };
                await _workmanService.GetProject(wt.ProjectId).ContinueWith(projectTask =>
                {
                    if (projectTask.Result != null)
                    {
                        taskVO.Project = new WorkProjectVO
                        {
                            Id = projectTask.Result.Id,
                            Name = projectTask.Result.Name
                        };
                    }
                });
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
