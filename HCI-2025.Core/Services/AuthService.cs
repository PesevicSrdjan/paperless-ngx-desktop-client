using HCI_2025.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HCI_2025.Core.Services
{
    public class AuthService : IAuthService
    {
        private static AuthService _instance = null;
        private readonly HttpClient _httpClient;
        private AuthService()
        { 
            _httpClient = new HttpClient(); 
        }
        public static AuthService getInstance()
        {
            if (_instance == null)
            { 
                _instance = new AuthService();
            }
            return _instance;
        }
        public async Task<LoginResponse> loginAsync(string username, string password)
        {
            var request = new LoginRequest { Username = username, Password = password};
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://paperless-etfbl.duckdns.org:8000/api/token/", content);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent);
            return loginResponse;
        }

        public void metoda2()
        { 
        }

    }
}
