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
