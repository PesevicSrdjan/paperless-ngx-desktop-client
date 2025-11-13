using HCI_2025.Core.Interfaces;
using HCI_2025.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025.Core.Services
{
    public class DocTypeService : IDocTypeService
    {
        public async Task<List<DocType>> getDocTypeAsync()
        {
            var client = ApiClient.Instance;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Session.CurrentUser.getBasicAuthHeader());

            string url = "api/document_types/";
            var response = await client.GetFromJsonAsync<DocTypeResponse>(url);

            if (response != null)
            {
                return response.Results;
            }
            else
            {
                return new List<DocType>();
            }
        }
    }
}
