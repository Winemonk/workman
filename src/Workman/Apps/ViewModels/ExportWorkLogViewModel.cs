using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Windows;
using Workman.Apps.Helpers;
using Workman.Core.Entities;
using Workman.Core.Services;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("ExportWorkLogView")]
    internal partial class ExportWorkLogViewModel : ObservableObject, IDialogAware
    {
        private readonly IWorkmanService _workmanService;

        public ExportWorkLogViewModel(IWorkmanService workmanService)
        {
            _workmanService = workmanService;
        }

        [ObservableProperty]
        private string _output = string.Empty;

        [ObservableProperty]
        private DateTime _startDate;

        [ObservableProperty]
        private DateTime _endDate;

        [ObservableProperty]
        private List<CheckWorkProjectVO> _projects;

        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private async Task Confirm()
        {
            if (string.IsNullOrWhiteSpace(Output))
            {
                MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.NotBeNullMessage, LocalizationManager.Instance.Output));
                return;
            }
            DateTime start = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day);
            DateTime end = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day);
            if (start == end)
            {
                end = end.AddDays(1);
            }
            IEnumerable<WorkLog> workLogs = await _workmanService.GetLogs(start, end);
            if (!workLogs.Any())
            {
                MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.NotFoundMessage, LocalizationManager.Instance.Log));
                return;
            }

            List<WorkTask> tasks = await _workmanService.GetTasks();

            using StreamWriter writer = new StreamWriter(Output, append: false, encoding: Encoding.UTF8);
            writer.WriteLine($"{LocalizationManager.Instance.Date},{LocalizationManager.Instance.Iteration},{LocalizationManager.Instance.Task},{LocalizationManager.Instance.Content},{LocalizationManager.Instance.ElapsedTime}");
            foreach (WorkLog log in workLogs)
            {
                WorkTask? task = tasks.FirstOrDefault(t => t.Id == log.TaskId);
                if (task == null)
                {
                    continue;
                }
                CheckWorkProjectVO? project = Projects.FirstOrDefault(p => p.Id == task.ProjectId && p.IsChecked);
                if (project == null)
                {
                    continue;
                }
                StringBuilder line = new StringBuilder(log.Date.ToString("yyyy-MM-dd"));
                line.Append(',');
                line.Append(GetCsvValue(project.Name));
                line.Append(',');
                line.Append(GetCsvValue(task.Name));
                line.Append(',');
                line.Append(GetCsvValue(log.Content));
                line.Append(',');
                line.Append(log.ElapsedTime);
                writer.WriteLine(line);
            }
            MessageHelper.ShowInfo(string.Format(LocalizationManager.Instance.SuccessMessage, LocalizationManager.Instance.ExportLog));
            RequestClose.Invoke();
        }

        [RelayCommand]
        private async Task SelectOutput()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "CSV|*.csv",
                FileName = $"{StartDate:yyyyMMdd}-{EndDate:yyyyMMdd}",
                DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }
            Output = dialog.FileName;
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

        partial void OnEndDateChanged(DateTime value)
        {
            Output = Path.Combine(Path.GetDirectoryName(Output)!, $"{StartDate:yyyyMMdd}-{EndDate:yyyyMMdd}.csv");
        }

        partial void OnStartDateChanged(DateTime value)
        {
            Output = Path.Combine(Path.GetDirectoryName(Output)!, $"{StartDate:yyyyMMdd}-{EndDate:yyyyMMdd}.csv");
        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            bool success = parameters.TryGetValue("date", out DateTime dateTime);
            if (!success)
            {
                RequestClose.Invoke(ButtonResult.Cancel);
            }
            Output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{StartDate:yyyyMMdd}-{EndDate:yyyyMMdd}.csv");
            StartDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            EndDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

            List<WorkProject> projects = await _workmanService.GetProjects();
            IEnumerable<CheckWorkProjectVO> checkProjects = projects.Select(project => new CheckWorkProjectVO
            {
                Id = project.Id,
                IsChecked = true,
                Name = project.Name,
                IsArchived = project.IsArchived,
                ArchivedTime = project.ArchivedTime,
                CreatedTime = project.CreatedTime,
            });
            Projects = checkProjects.ToList();
        }

        private string GetCsvValue(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            if (value.Contains(','))
            {
                return $"\"{value}\"";
            }
            return value;
        }
    }
}
