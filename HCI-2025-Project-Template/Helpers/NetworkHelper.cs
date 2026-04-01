using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Helpers
{
    public static class NetworkHelper
    {
        public static bool HasInternet()
        {
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
                var response = client.GetAsync("https://www.google.com").Result;
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<bool> HasInternetAsync()
        {
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
                var response = await client.GetAsync("https://www.google.com");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private static bool _lastInternetStatus = true;
        private static DateTime _lastChecked = DateTime.MinValue;

        public static async Task<bool> HasInternetCachedAsync()
        {
            // ako je provjera rađena u posljednjih 5 sekundi, vrati cached rezultat
            if ((DateTime.Now - _lastChecked).TotalSeconds < 5)
                return _lastInternetStatus;

            _lastInternetStatus = await HasInternetAsync();
            _lastChecked = DateTime.Now;
            return _lastInternetStatus;
        }
    }
}

