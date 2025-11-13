using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace HCI_2025.Core.Services
{
    public static class ApiClient
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://paperless-etfbl.duckdns.org:8000/")
        };
        public static HttpClient Instance => _httpClient;
    }
}
