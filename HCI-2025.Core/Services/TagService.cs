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
    public class TagService : ITagService
    {
        public async Task<List<Tag>> getTagsAsync()
        { 
            var client = ApiClient.Instance;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Session.CurrentUser.getBasicAuthHeader());

            string url = "api/tags/";
            var response = await client.GetFromJsonAsync<TagResponse>(url);

            if (response != null)
            {
                return response.Results;
            }
            else
            {
                return new List<Tag>();
            }
        }
    }
}
