using HCI_2025_Project_Template.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HCI_2025_Project_Template.Views.UserControls
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {

        private readonly Color DefaultAccentColor = (Color)ColorConverter.ConvertFromString("#17541F");
        public SettingsView()
        {
            InitializeComponent();
            SyncRadioButtonsWithTheme();

        }
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                string theme = rb.Content.ToString(); // "Light" ili "Dark"
                ChangeTheme(theme);
            }
        }
        private async void ChooseColor_Click(object sender, RoutedEventArgs e)
        {

            Color currentAccent = Colors.Green;
            if (Application.Current.Resources["AccentColor"] is SolidColorBrush brush)
                currentAccent = brush.Color;

            var dialog = new AccentColorDialog
            {
                SelectedColor = currentAccent
            };

            var result = await DialogHost.Show(dialog, "AccentColorDialog");

            if (result is Color selectedColor)
            {
                ApplyAccentColor(selectedColor);
            }

        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            bool isDark = theme.GetBaseTheme() == BaseTheme.Dark;

            Color defaultColor = isDark
                ? (Color)Application.Current.Resources["AccentColorDark"]
                : (Color)Application.Current.Resources["AccentColorLight"];

            ApplyAccentColor(defaultColor);
        }

        private void ApplyAccentColor(Color color)
        {
            Application.Current.Resources["AccentColor"] = new SolidColorBrush(color);

            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            theme.SetPrimaryColor(color);
            paletteHelper.SetTheme(theme);

            Settings.Default.AccentColor = color.ToString(); 
            Settings.Default.Save();

            if (Application.Current.MainWindow is DashboardWindow dashboard)
                dashboard.RefreshSidebarButtonColors();
        }

        private void SyncRadioButtonsWithTheme()
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            if (theme.GetBaseTheme() == BaseTheme.Dark)
                DarkRadio.IsChecked = true;
            else
                LightRadio.IsChecked = true;
        }

        private void ChangeTheme(string themeName)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            bool wasDark = theme.GetBaseTheme() == BaseTheme.Dark;

            Color defaultLight = (Color)Application.Current.Resources["AccentColorLight"];
            Color defaultDark = (Color)Application.Current.Resources["AccentColorDark"];

            Color currentAccent = Colors.Green;
            if (Application.Current.Resources["AccentColor"] is SolidColorBrush brush)
                currentAccent = brush.Color;

            if (themeName == "Dark")
                theme.SetBaseTheme(BaseTheme.Dark);
            else
                theme.SetBaseTheme(BaseTheme.Light);

            paletteHelper.SetTheme(theme);

            bool wasDefault = (!wasDark && currentAccent == defaultLight) || (wasDark && currentAccent == defaultDark);

            bool isDark = themeName == "Dark";
            Color newAccent = wasDefault
                ? (isDark ? defaultDark : defaultLight)
                : currentAccent;

            Application.Current.Resources["AccentColor"] = new SolidColorBrush(newAccent);

            Settings.Default.AccentColor = newAccent.ToString();
            Settings.Default.AppTheme = themeName;
            Settings.Default.Save();

            Application.Current.Resources["Logo"] = new BitmapImage(new Uri(
                isDark
                    ? "pack://application:,,,/Assets/Paperless-Ngx_White_Logo.png"
                    : "pack://application:,,,/Assets/Paperless-Ngx_Black_Logo.png"));

            if (Application.Current.MainWindow is DashboardWindow dashboard)
                dashboard.RefreshSidebarButtonColors();
        }
    }
}