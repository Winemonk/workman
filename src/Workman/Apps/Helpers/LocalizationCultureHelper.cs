using System.Globalization;

namespace Workman.Apps.Helpers
{
    internal class LocalizationCultureHelper
    {
        public static void SetCulture(string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
