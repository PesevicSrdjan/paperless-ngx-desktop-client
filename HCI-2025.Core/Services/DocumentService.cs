using HCI_2025.Core.Interfaces;
using HCI_2025.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HCI_2025.Core.Services
{
    public class DocumentService : IDocumentService
    {
        public async Task<List<DocumentJson>> getAllDocumentsAsync(int page = 1, int pageSize = 20)
        {
            var client = ApiClient.Instance;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Session.CurrentUser.getBasicAuthHeader());

            string url = $"api/documents/?page={page}&page_size={pageSize}";

            var response = await client.GetFromJsonAsync<DocumentsResponse>(url);

            if (response != null && response.results != null)
            {
                return response.results;
            }
            else
            {
                return new List<DocumentJson>();
            }
        }
    }
}
