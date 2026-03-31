using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Workman.Core.Services;

namespace Workman.Apps.Helpers
{
    internal class CalendarDayButtonBrushConverter : IValueConverter
    {
        private readonly IWorkmanService _workmanService;

        public CalendarDayButtonBrushConverter()
        {
            if(System.Windows.Application.Current is App app)
            {
                _workmanService = app.Container.Resolve<IWorkmanService>();
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter is string type)
            {
                if(type == "Background")
                {
                    if (_workmanService == null || value is not DateTime dateTime)
                    {
                        return Brushes.Transparent;
                    }
                    bool theDayHasLogs = _workmanService.TheDayHasLogs(dateTime);
                    if (theDayHasLogs)
                    {
                        return new SolidColorBrush(Color.FromArgb(0x88, 0x3f, 0xB9, 0x50));
                    }
                    else if (dateTime.DayOfWeek == DayOfWeek.Sunday || dateTime.DayOfWeek == DayOfWeek.Saturday)
                    {
                        return Brushes.AliceBlue;
                    }
                    else
                    {
                        return new SolidColorBrush(Color.FromArgb(0x88, 0xf8, 0xbd, 0x16));
                    }
                }
                else if (type == "Foreground")
                {
                    if (_workmanService == null || value is not DateTime dateTime)
                    {
                        return Brushes.Black;
                    }
                    bool theDayHasLogs = _workmanService.TheDayHasLogs(dateTime);
                    if (theDayHasLogs)
                    {
                        return Brushes.White;
                    }
                    if (dateTime.DayOfWeek == DayOfWeek.Sunday || dateTime.DayOfWeek == DayOfWeek.Saturday)
                    {
                        return Brushes.Black;
                    }
                    else
                    {
                        return Brushes.White;
                    }
                }
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
