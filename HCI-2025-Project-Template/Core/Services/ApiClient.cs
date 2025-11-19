using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;

namespace HCI_2025_Project_Template.Core.Services
{
    public static class ApiClient
    {
        private static readonly HttpClient _httpClient;
        static ApiClient()
        {
            var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"]!;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }
        public static HttpClient Instance => _httpClient;
    }
}
