using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Models.Responses
{
    // Ovo je klasa koja predstavlja odgovor API-ja nakon login pokušaja.
    public class LoginResponse
    {
        // 'Token' se vraća ukoliko je login uspješan.
        [JsonPropertyName("token")]
        public string? Token { get; set; }
        // 'Success' provjerava da li je u UI login prošao.
        public bool Success => !string.IsNullOrEmpty(Token);

        public string ?Error { get; set; }
    }
}
