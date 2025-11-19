using HCI_2025.Core.Interfaces;
using HCI_2025.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Services
{
    public class DocTypeService : IDocTypeService
    {
        public async Task<List<DocType>> getDocTypeAsync()
        {
            try
            {
                var client = ApiClient.Instance;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = "api/document_types/";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return new List<DocType>();

                var data = await response.Content.ReadFromJsonAsync<DocTypeResponse>();

                if (data != null && data.Results != null) return data.Results;

                return new List<DocType>();
            }
            catch (HttpRequestException)
            {
                return new List<DocType>();
            }
            
        }
    }
}
