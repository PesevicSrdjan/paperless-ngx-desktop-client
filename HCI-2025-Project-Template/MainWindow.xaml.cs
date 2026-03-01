using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HCI_2025_Project_Template.Views;
using HCI_2025_Project_Template.ViewModels;
using HCI_2025_Project_Template.Core.Services;

namespace HCI_2025_Project_Template
{
    /// <summary>
    /// Klasa koja predstavlja UI - ono što korisnik vidi.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Komunicira sa LoginViewModel, ali ne zna ništa o servisima.
        /// </summary>
        private readonly LoginViewModel _viewModel;

        /// <summary>
        /// Konstruktor klase.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Instanciranje LoginViewModel - a, ubrizgavamo instancu AuthService.
            _viewModel = new LoginViewModel(new AuthService());

            // Učitaj prethodne korisnike iz Settings
            var savedUsers = Settings.Default.SavedUser?.Split(';');
            _viewModel.loadPreviousUsers(savedUsers);

            // Postavi početni izvor za popup (ako već ima prethodnih korisnika)
            if (_viewModel.previousUsers.Count > 0)
            {
                listSuggestions.ItemsSource = _viewModel.previousUsers;
            }

            bool isDarkTheme = Settings.Default.AppTheme == "Dark";

            SetLogoForTheme(isDarkTheme);
        }

        /// <summary>
        /// Postavljanje logo - a u zavisnosti od teme primjenjene na app.
        /// </summary>
        /// <param name="isDarkTheme"></param>
        private void SetLogoForTheme(bool isDarkTheme)
        {
            string logoUri = isDarkTheme
                    ? "pack://application:,,,/Assets/Paperless-Ngx_White_Logo.png"
                    : "pack://application:,,,/Assets/Paperless-Ngx_Black_Logo.png";

            Application.Current.Resources["Logo"] = new BitmapImage(new Uri(logoUri));
        }

        /// <summary>
        /// Update popup dok korisnik tipka.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_username_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.Username = txtBox_username.Text;
            _viewModel.updateSuggestions();

            listSuggestions.ItemsSource = _viewModel.suggestions;
            popupSuggestions.IsOpen = _viewModel.suggestions.Count > 0;
        }

        /// <summary>
        /// Kada korisnik fokusira textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_username_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_viewModel.previousUsers.Count > 0)
            {
                listSuggestions.ItemsSource = _viewModel.previousUsers;
                popupSuggestions.IsOpen = true;
            }
        }

        /// <summary>
        /// Klikom na prijedlog iz popup-a
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listSuggestions.SelectedItem != null)
            {
                txtBox_username.Text = listSuggestions.SelectedItem.ToString();
                popupSuggestions.IsOpen = false;
            }
        }

        /// <summary>
        /// Kad textbox izgubi fokus, odnosno kad se pozicioniramo na nešto drugo na WPF-u.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_username_LostFocus(object sender, RoutedEventArgs e)
        {
            popupSuggestions.IsOpen = false;
        }

        /// <summary>
        /// Klikom na login button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = passwordBox_password.Password;

            var result = await _viewModel.loginAsync();

            if (result.Success)
            {

                // Spremi username u Settings
                List<string> users = new List<string>();

                if (Settings.Default.SavedUser != null && Settings.Default.SavedUser != "")
                {
                    string[] savedUsers = Settings.Default.SavedUser.Split(';');
                    for (int i = 0; i < savedUsers.Length; i++)
                    {
                        users.Add(savedUsers[i]);
                    }
                }
                // Ako username već nije u listi, dodaj ga
                bool exists = false;
                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i] == _viewModel.Username)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    users.Add(_viewModel.Username);
                }

                // Spremi nazad u Settings
                string savedString = "";
                for (int i = 0; i < users.Count; i++)
                {
                    if (i > 0) savedString += ";";
                    savedString += users[i];
                }

                Settings.Default.SavedUser = savedString;
                Settings.Default.Save();

                // Otvori Dashboard
                DashboardWindow dbw = new DashboardWindow();
                dbw.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Login nije uspio!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
