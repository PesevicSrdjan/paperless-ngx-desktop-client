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
    public class TagService : ITagService
    {
        public async Task<List<Tag>> getTagsAsync()
        {
            try
            {
                var client = ApiClient.Instance;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = "api/tags/";

                var response = await client.GetAsync(url);


                if (!response.IsSuccessStatusCode)
                {
                    return new List<Tag>();
                }

                //response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<TagResponse>();

                if(data != null)
                    return data.Results;

                return new List<Tag>();
            }
            catch (HttpRequestException)
            {
                return new List<Tag>();
            }
        }
    }
}
