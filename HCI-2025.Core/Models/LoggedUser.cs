using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025.Core.Models
{
    public class LoggedUser
    {
        public string Username { get; set; }
        public string Password { get; set; }

        // Metoda za kreiranje Basic Auth headera
        public string getBasicAuthHeader()
        {
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{Password}"));
            return authToken;
        }
    }
}
