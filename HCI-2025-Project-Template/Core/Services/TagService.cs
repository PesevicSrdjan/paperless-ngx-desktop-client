using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Api;
using HCI_2025_Project_Template.Core.Models.Responses;
using HCI_2025_Project_Template.Core.Models.Ui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public async Task<TagResponse> getTagsAsync(int page = 1, int pageSize = 25)
        {
            try
            {
                var client = ApiClient.Instance;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = $"api/tags/?page={page}&page_size={pageSize}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<TagResponse>();

            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        // Metoda koja je potrebna za postavljanje tagova (oznaka) u ComboBox (DocumentsView).
        public async Task<List<Tag>> GetAllTagsAsync()
        {
            var allTags = new List<Tag>();
            int page = 1;
            int pageSize = 25;

            TagResponse? response;

            do
            {
                response = await getTagsAsync(page, pageSize);

                if (response == null || response.Results.Count == 0)
                    break;

                allTags.AddRange(response.Results);

                page++;

            }while (allTags.Count < response.Count);

            return allTags;
        }

        public async Task<bool> DeleteTagAsync(int tagId)
        {
            try
            {
                var client = ApiClient.Instance;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = $"api/tags/{tagId}/";

                var response = await client.DeleteAsync(url);

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
        public async Task<bool> UpdateTagAsync(TagInfo tag)
        {
            if (tag == null) return false;

            try
            {
                var client = ApiClient.Instance;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = $"api/tags/{tag.Id}/";

                var json = System.Text.Json.JsonSerializer.Serialize(new
                {
                    name = tag.Name,
                    color = tag.Color
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(url, content);

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
        public async Task<Tag?> CreateTagAsync(TagInfo tag)
        {
            if (tag == null) return null;

            try
            {
                var client = ApiClient.Instance;
                
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = "api/tags/";

                var json = System.Text.Json.JsonSerializer.Serialize(new
                {
                    name = tag.Name,
                    color = tag.Color,
                    matching_algorithm = 6
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var createdTag = await response.Content.ReadFromJsonAsync<Tag>();

                return createdTag;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
