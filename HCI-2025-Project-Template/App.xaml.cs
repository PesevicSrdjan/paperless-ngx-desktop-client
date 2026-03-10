using HCI_2025_Project_Template.Helpers;
using MaterialDesignThemes.Wpf;
using System.Configuration;
using System.Data;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HCI_2025_Project_Template
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application 
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var paletteHelper = new PaletteHelper();
            Theme theme = paletteHelper.GetTheme();

            if (Settings.Default.AppTheme == "Dark")
                theme.SetBaseTheme(BaseTheme.Dark);
            else
                theme.SetBaseTheme(BaseTheme.Light);


            if (!string.IsNullOrWhiteSpace(Settings.Default.AccentColor))
            {
                Color accent = (Color)ColorConverter.ConvertFromString(Settings.Default.AccentColor);
                theme.SetPrimaryColor(accent);

                Application.Current.Resources["AccentColor"] = new SolidColorBrush(accent);
            }

            paletteHelper.SetTheme(theme);

            string language = Settings.Default.Language;

            if (string.IsNullOrEmpty(language))
                language = "en";

            LocalizationManager.ChangeCulture(language);
        }
    }
}

    
