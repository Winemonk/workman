namespace Workman.Apps.Configs
{
    public class AppSettings
    {
        public bool Autostart { get; set; } = true;
        public bool DockOnlyMainScreen { get; set; } = true;
        public bool TurnOnReminder { get; set; } = true;
        public TimeOnly ReminderOfStartTime { get; set; } = new TimeOnly(14, 0);
        public int ReminderInterval { get; set; } = 30;
    }
}
