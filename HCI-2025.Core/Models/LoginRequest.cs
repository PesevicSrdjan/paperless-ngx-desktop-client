using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCI_2025.Core.Models
{
    // Ovo je klasa koja predstavlja šta šaljemo API-ju kada se korisnik loguje.
    public class LoginRequest
    {
        // 'Username' i 'Password' su string-ovi koje korisnik unosi.
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
