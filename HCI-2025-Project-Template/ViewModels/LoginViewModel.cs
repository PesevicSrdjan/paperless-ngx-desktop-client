using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Api;
using HCI_2025_Project_Template.Core.Models.Responses;
using System.Collections.ObjectModel;
namespace HCI_2025_Project_Template.ViewModels
{
    /// <summary>
    /// Klasa koja predstavlja 'posrednika' između UI-a i servisa (Core -a)
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// 'LoginViewModel' komunicira sa servisom 'AuthService' preko interfejsa 'IAuthService'.
        /// </summary>
        private readonly IAuthService _authService;

        /// <summary>
        /// Trenutno uneseni podaci u 'TextBox' i 'PasswordBox'.
        /// </summary>
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;

        /// <summary>
        /// Lista prethodnih korisnika koje smo već unijeli i spremili u Settings.
        /// </summary>
        public List<string> previousUsers { get; private set; } = new();

        /// <summary>
        /// Prijedlozi koji se prikazuju u popup - u dok korisnik tipa username.
        /// </summary>
        public ObservableCollection<string> suggestions { get; private set; } = new();
        
        /// <summary>
        /// Konstrutktor koji prima interfejs servisa (dependency injection).
        /// </summary>
        /// <param name="authService"></param>
        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }
        /// <summary>
        /// Metode koja učitava korisnike iz Settings.
        /// </summary>
        /// <param name="users"></param>
        public void loadPreviousUsers(IEnumerable<string> users)
        {
            if (users != null) // Ako lista postoji sprema se u 'previousUsers'
                previousUsers = users.ToList();
            else
                previousUsers = new List<string>(); // Ako ne, pravi se nova lista.
            updateSuggestions(); // Priprema popup prijedloge.
        }
        /// <summary>
        /// Metoda koja ažurira prijedloge.
        /// </summary>
        public void updateSuggestions()
        {
            suggestions.Clear(); // Očisti prethodne prijedloge

            // Ukoliko je 'Username' null ili blank - znači da korisnik nije unio ništa.
            if (string.IsNullOrWhiteSpace(Username))
                return;

            // Prolazimo kroz listu korisnika u Settings, i provjeravamo da li ono što kucamo odgovara username - u  nekom od njih.
            foreach (var user in previousUsers)
            {
                if (user.StartsWith(Username, System.StringComparison.OrdinalIgnoreCase))
                    suggestions.Add(user);
            }
        }

        /// <summary>
        /// Metoda koja poziva servis za login sa trenutno unesenim 'Username' i  'Password'
        /// </summary>
        /// <returns>
        /// (true, response) - OK
        /// (false, null) - NOT OK.
        ///</returns>

        public async Task<LoginResponse> loginAsync()
        {

            var response = await _authService.loginAsync(Username, Password);

            if (response == null || !response.Success)
                return new LoginResponse { Token = null };

            AuthSession.Token = response.Token;

            return response;
        }
    }
}
 