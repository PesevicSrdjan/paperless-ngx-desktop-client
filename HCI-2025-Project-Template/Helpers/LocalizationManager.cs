using HCI_2025_Project_Template.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HCI_2025_Project_Template.Helpers
{
    public static class LocalizationManager
    {
        public static LocalizedStrings Strings => (LocalizedStrings)Application.Current.Resources["LStrings"];

        private static string _currentLanguage = "en";

        public static string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                ChangeCulture(_currentLanguage);
            }
        }
        public static void ChangeCulture(string cultureCode)
        {
            var culture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            Settings.Default.Language = cultureCode;
            Settings.Default.Save();

            Strings.RaiseAllPropertiesChanged();
        }
    }
}
