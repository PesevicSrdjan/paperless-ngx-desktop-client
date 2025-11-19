using HCI_2025.Core.Interfaces;
using HCI_2025.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Services
{
    public class AuthService : IAuthService
    {
        public async Task<LoginResponse> loginAsync(string username, string password)
        {
            var request = new LoginRequest 
            { 
                Username = username, 
                Password = password
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiClient.Instance.PostAsync("api/token/", content);

            if (!response.IsSuccessStatusCode)
            {
                return new LoginResponse 
                { 
                    Token = null 
                };
            }
            //response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent);

            return loginResponse != null ? loginResponse : new LoginResponse { Token = null };
        }
    }
}
