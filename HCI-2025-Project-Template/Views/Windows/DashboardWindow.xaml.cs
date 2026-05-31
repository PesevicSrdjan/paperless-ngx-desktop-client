using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Services;
using HCI_2025_Project_Template.Helpers;
using HCI_2025_Project_Template.Views.UserControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HCI_2025_Project_Template.Views
{
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {
        private Button _selectedSidebarButton;
        private readonly ISpeechService _speechService;
        public DashboardWindow()
        {
            InitializeComponent();

            MainContentControl.Content = new DashboardView();
            _selectedSidebarButton = button_dashboard;

            _speechService = new SpeechService();

            // Kad god se event desi, poziva se OnVoceCommand.
            _speechService.CommandRecognized += OnVoiceCommand;

            Loaded += dashboardWindow_Loaded;
        }

        private void dashboardWindow_Loaded(object sender, RoutedEventArgs e)
        {
            setNewSelectedButton(_selectedSidebarButton);
        }
        private void button_dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new DashboardView();
        }

        private void button_documents_Click(object sender, RoutedEventArgs e)
        {
            NavigateWithInternetCheck(
                button_documents,
                () => new DocumentsView(),
                async view => await ((DocumentsView)view).TriggerRefresh()
            );
        }

        private void Button_Correspondents_Click(object sender, RoutedEventArgs e)
        {
            NavigateWithInternetCheck(
                button_documents,
                () => new CorrespondentsView(),
                async view => await ((CorrespondentsView)view).TriggerRefresh()
            );
        }
        private void Button_DocumentTypes_Click(object sender, RoutedEventArgs e)
        {
            NavigateWithInternetCheck(
                button_documents,
                () => new DocumentTypeView(),
                async view => await ((DocumentTypeView)view).TriggerRefresh()
            );
        }

        private void Documentation_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://docs.paperless-ngx.com/",
                UseShellExecute = true
            });
        }
        private void resetPrevSelectedButton()
        {
            if (_selectedSidebarButton != null)
            {
                _selectedSidebarButton.FontWeight = FontWeights.Normal;
                _selectedSidebarButton.ClearValue(Button.ForegroundProperty);
            }
        }

        private void setNewSelectedButton(Button? newButton)
        {
            if (newButton == null) return;

            _selectedSidebarButton = newButton;

            _selectedSidebarButton.FontWeight = FontWeights.Bold;

            _selectedSidebarButton.SetResourceReference(
                Button.ForegroundProperty,
                "AccentColor");
        }
        private void SidebarButton_Click(object sender, RoutedEventArgs e)
        {
            resetPrevSelectedButton();
            setNewSelectedButton(sender as Button);
        }
        private void button_tags_Click(object sender, RoutedEventArgs e)
        {
            NavigateWithInternetCheck(
                button_documents,
                () => new TagsView(),
                async view => await ((TagsView)view).TriggerRefresh()
            );
        }
        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        { 
            MainContentControl.Content = new SettingsView();
            resetPrevSelectedButton();
            setNewSelectedButton(Button_Settings);
        }
        public void RefreshSidebarButtonColors()
        {
            if (_selectedSidebarButton == null)
                return;

            _selectedSidebarButton.ClearValue(Button.ForegroundProperty);

            _selectedSidebarButton.Foreground =
                (Brush)Application.Current.Resources["AccentColor"];

            _selectedSidebarButton.FontWeight = FontWeights.Bold;
        }
        private void Button_Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        public static RoutedUICommand NavigateCommand = new RoutedUICommand("Navigate", "Navigate", typeof(DashboardWindow));

        private void NavigateCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string target = e.Parameter as string;

            switch (target)
            {
                case "documents":
                    Navigate(button_documents, button_documents_Click);
                    break;

                case "dashboard":
                    Navigate(button_dashboard, button_dashboard_Click);
                    break;

                case "tags":
                    Navigate(button_tags, button_tags_Click);
                    break;

                case "correspondents":
                    Navigate(Button_Correspondents, Button_Correspondents_Click);
                    break;

                case "types":
                    Navigate(Button_DocumentTypes, Button_DocumentTypes_Click);
                    break;

                case "settings":
                    Navigate(Button_Settings, Button_Settings_Click);
                    break;
            }
        }

        private void DocumentsShortcut(object sender, ExecutedRoutedEventArgs e)
        {
            Navigate(Button_DocumentTypes, Button_DocumentTypes_Click);
        }

        private void DashboardShortcut(object sender, ExecutedRoutedEventArgs e)
        {
            Navigate(button_dashboard, button_dashboard_Click);
        }

        private void TagsViewShortcut(object sender, ExecutedRoutedEventArgs e)
        {
            Navigate(button_tags, button_tags_Click);
        }

        private void CorrespondentsViewShortcut(object sender, ExecutedRoutedEventArgs e)
        {
            Navigate(Button_Correspondents, Button_Correspondents_Click);
        }

        private void SettingsShortcut(object sender, ExecutedRoutedEventArgs e)
        {
            Navigate(Button_Settings, Button_Settings_Click);
        }

        private void DocumentTypesShortcut(object sender, ExecutedRoutedEventArgs e)
        {
            Navigate(Button_DocumentTypes, Button_DocumentTypes_Click);
        }



        private void MicButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_speechService.IsListening)
            {
                _speechService.Start();
                MicIcon.Foreground = Brushes.Red;
            }
            else
            {
                _speechService.Stop();
                MicIcon.Foreground = Brushes.White;
            }
        }

        /// <summary>
        /// Dispatcher.Invoke - izvršavanje se prebacuje na UI thread, jer se UI elementi ne mogu mijenjati iz pozadinskih Thread -ova.
        /// Na osnovu string - a koji se prosljedjuje kao parametar 'navigira' se na različite sekcije aplikacije, ili se pokreće pretraga.
        /// </summary>
        /// <param name="speech"></param>
        private void OnVoiceCommand(string speech)
        {
            Dispatcher.Invoke(() =>
            {
                if (speech.StartsWith("search"))
                {
                    string query = speech.Replace("search", "").Trim();
                    SearchFromVoice(query);
                    return;
                }

                switch (speech)
                {
                    case "dashboard":
                        Navigate(button_dashboard, button_dashboard_Click);
                        break;

                    case "documents":
                        Navigate(button_documents, button_documents_Click);
                        break;

                    case "upload":
                        UploadFromVoice();
                        break;

                    case "tags":
                        Navigate(button_tags, button_tags_Click);
                        break;

                    case "settings":
                        Navigate(Button_Settings, Button_Settings_Click);
                        break;

                    case "correspondents":
                        Navigate(Button_Correspondents, Button_Correspondents_Click);
                        break;

                    case "types":
                        Navigate(Button_DocumentTypes, Button_DocumentTypes_Click);
                        break;

                    case "refresh":
                        RefreshFromVoice();
                        break;

                    case "logout":
                        Button_Logout_Click(null, null);
                        break;
                }
            });
        }

        private void SearchFromVoice(string text)
        {
            if (MainContentControl.Content is DocumentsView documentsView)
            {
                documentsView.TriggerSearch(text);
            }
        }

        private void Navigate(Button button, RoutedEventHandler action)
        {
            action(null, null);
            resetPrevSelectedButton();
            setNewSelectedButton(button);
        }
        private void UploadFromVoice()
        {
            if (MainContentControl.Content is DashboardView dashboardView)
            {
                dashboardView.TriggerUpload();
            }
        }

        private async void RefreshFromVoice()
        {
            if (MainContentControl.Content is DocumentsView documentsView)
            {
                await documentsView.TriggerRefresh();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _speechService.Stop();
            base.OnClosed(e);
        }


        private void NavigateWithInternetCheck<T>(Button button, Func<T> createView, Func<T, Task>? loadAsync = null) where T : UserControl, INoInternetAware
        {
            var view = createView();

            view.NoInternetDetectedExternally += () =>
            {
                var noInternetView = new NoInternetView();
                noInternetView.OnRetry += () => NavigateWithInternetCheck(button, createView, loadAsync);

                MainContentControl.Dispatcher.Invoke(() =>
                {
                    MainContentControl.Content = noInternetView;
                });
            };

            MainContentControl.Content = view;

            if (loadAsync != null)
                _ = loadAsync(view);
        }

    }
}
