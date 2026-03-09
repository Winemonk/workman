using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Workman.Apps
{
    internal class LocalizationManager : INotifyPropertyChanged
    {
        private static LocalizationManager _instance = new LocalizationManager();
        public static LocalizationManager Instance => _instance;

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ResourceManager _resourceManager;

        private LocalizationManager()
        {
            _resourceManager = new ResourceManager("Workman.Properties.Resources.lang", typeof(LocalizationManager).Assembly);
        }
        public string this[string name]
        {
            get
            {
                return name == null ? throw new ArgumentNullException(nameof(name)) : _resourceManager.GetString(name)!;
            }
        }

        // 示例属性
        public string AddLog => this["str_add_log"];
        public string All => this["str_all"];
        public string AppSetting => this["str_app_setting"];
        public string Cancel => this["str_cancel"];
        public string Close => this["str_close"];
        public string Confirm => this["str_confirm"];
        public string Content => this["str_content"];
        public string Delete => this["str_delete"];
        public string DeleteRow => this["str_delete_row"];
        public string Dock => this["str_dock"];
        public string Edit => this["str_edit"];
        public string ElapsedTime => this["str_elapsed_time"];
        public string ExportLog => this["str_export_log"];
        public string IDX => this["str_idx"];
        public string Iteration => this["str_iteration"];
        public string IterationManage => this["str_iteration_manage"];
        public string Name => this["str_name"];
        public string NewIteration => this["str_new_iteration"];
        public string NewRow => this["str_new_row"];
        public string NewTask => this["str_new_task"];
        public string NextDay => this["str_next_day"];
        public string Operation => this["str_operation"];
        public string PrevDay => this["str_prev_day"];
        public string Task => this["str_task"];
        public string TaskCount => this["str_task_count"];
        public string TaskManage => this["str_task_manage"];
        public string Today => this["str_today"];
        public string TotalElapsedTime => this["str_total_elapsed_time"];
        public string StartDate => this["str_start_date"];
        public string EndDate => this["str_end_date"];
        public string Output => this["str_output"];
        public string Exit => this["str_exit"];
        public string OpenMainWindow => this["str_open_main_window"];
        public string AutoStartup => this["str_auto_startup"];
        public string DockMainScreen => this["str_dock_main_screen"];
        public string SaveAndClose => this["str_save_and_close"];

        public void ChangeCulture(string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            OnPropertyChanged();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
