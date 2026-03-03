using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hearth.Prism.Toolkit;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using Workman.Core.Entities;
using Workman.Core.Repositories;

namespace Workman.Apps.ViewModels
{
    [RegisterDialog("ExportWorkLogView")]
    internal partial class ExportWorkLogViewModel : ObservableObject, IDialogAware
    {
        private readonly IRepository<WorkLog> _workLogRepository;
        private readonly IRepository<Project> _projectRepository;

        public ExportWorkLogViewModel(IRepository<WorkLog> workLogRepository,
                                      IRepository<Project> projectRepository)
        {
            _workLogRepository = workLogRepository;
            _projectRepository = projectRepository;
        }

        [ObservableProperty]
        private string _output = string.Empty;

        [ObservableProperty]
        private DateTime _startDate;

        [ObservableProperty]
        private DateTime _endDate;

        public DialogCloseListener RequestClose { get; }

        [RelayCommand]
        private async Task Confirm()
        {
            if (string.IsNullOrWhiteSpace(Output))
            {
                MessageBox.Show("输出目录不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DateTime start = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day);
            DateTime end = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day);
            if(start == end)
            {
                end = end.AddDays(1);
            }
            IEnumerable<WorkLog> workLogs = await _workLogRepository.QueryRange(q => q.Where(l => l.Date >= start && l.Date < end));
            if (!workLogs.Any())
            {
                MessageBox.Show("所选时间段未查询到日志！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IEnumerable<Project> projects = await _projectRepository.QueryRange();

            // 使用 using 确保资源释放
            using StreamWriter writer = new StreamWriter(Output, append: false, encoding: Encoding.UTF8);
            writer.WriteLine("日期,项目,内容,耗时");
            foreach (WorkLog log in workLogs)
            {
                string line = $"{log.Date:yyyy-MM-dd},";
                Project? project = await _projectRepository.Query(log.ProjectId);
                if(project?.Name?.Contains(',') == true)
                {
                    line += $"\"{project.Name}\",";
                }
                else
                {
                    line += $"{project?.Name},";
                }
                if(log.Content.Contains(','))
                {
                    line += $"\"{log.Content}\",";
                }
                else
                {
                    line += $"{log.Content},";
                }
                line += $"{log.ElapsedTime}";
                writer.WriteLine(line);
            }
            MessageBox.Show("导出成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
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
            if(dialog.ShowDialog() != true)
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
            Output = Path.Combine(Path.GetDirectoryName(Output), $"{StartDate:yyyyMMdd}-{EndDate:yyyyMMdd}.csv");
        }

        partial void OnStartDateChanged(DateTime value)
        {
            Output = Path.Combine(Path.GetDirectoryName(Output), $"{StartDate:yyyyMMdd}-{EndDate:yyyyMMdd}.csv");
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
        }
    }
}
