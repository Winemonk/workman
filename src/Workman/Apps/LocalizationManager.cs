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
        public string Log => this["str_log"];
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
        public string Hint => this["str_hint"];
        public string Error => this["str_error"];
        public string Warn => this["str_warn"];
        public string Info => this["str_info"];
        public string ExitAppHint => this["str_exit_app_hint"];
        public string DeleteIterationHint => this["str_delete_iteration_hint"];

        /// <summary>
        /// {0}失败！
        /// {0} failed!
        /// </summary>
        public string FailedMessage => this["str_failed_message"];
        /// <summary>
        /// {1}，第{0}项失败！
        /// {1}, item {0} failed!
        /// </summary>
        public string ItemFailedMessage => this["str_item_failed_message"];
        /// <summary>
        /// {0}成功！
        /// {0} successfully!
        /// </summary>
        public string SuccessMessage => this["str_success_message"];
        /// <summary>
        /// {0}不能为空！
        /// The "{0}" cannot be left blank.
        /// </summary>
        public string NotBeNullMessage => this["str_not_be_null_message"];
        /// <summary>
        /// 第{0}项：{1}不能为空！
        /// The "{1}" of item number {0} cannot be left blank.
        /// </summary>
        public string ItemNotBeNullMessage => this["str_item_not_be_null_message"];
        /// <summary>
        /// {0}必须大于0！
        /// The "{0}" must be greater than 0!
        /// </summary>
        public string MustBeGraterThenZeroMessage => this["str_must_be_grater_then_zero_message"];
        /// <summary>
        /// 第{0}项：{1}必须大于0！
        /// The "{1}" of item number {0} must be greater than 0!
        /// </summary>
        public string ItemMustBeGraterThenZeroMessage => this["str_item_must_be_grater_then_zero_message"];
        /// <summary>
        /// {0}未查询到！
        /// {0} not found!
        /// </summary>
        public string NotFoundMessage => this["str_not_found_message"];
        /// <summary>
        /// Date
        /// </summary>
        public string Date => this["str_date"];

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
