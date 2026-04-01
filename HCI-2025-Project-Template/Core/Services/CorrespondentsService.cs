using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Api;
using HCI_2025_Project_Template.Core.Models.Responses;
using HCI_2025_Project_Template.Core.Models.Ui;
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
    public class CorrespondentsService : ICorrespondentsService
    {
        public async Task<CorrespondentsResponse> getCorrespondentsAsync(int page = 1, int pageSize = 25)
        {
            try
            {
                var client = ApiClient.Instance;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = $"api/correspondents/?page={page}&page_size={pageSize}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<CorrespondentsResponse>();

            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<List<Correspondents>> GetAllCorrespondentsAsync()
        {
            var allCorrespondents = new List<Correspondents>();
            int page = 1;
            int pageSize = 25;

            CorrespondentsResponse? response;

            do
            {
                response = await getCorrespondentsAsync(page, pageSize);

                if (response == null || response.Results.Count == 0)
                    break;

                allCorrespondents.AddRange(response.Results);

                page++;

            } while (allCorrespondents.Count < response.Count);

            return allCorrespondents;
        }


        public async Task<bool> DeleteCorrespondentAsync(int correspondentId)
        {
            try
            {
                var client = ApiClient.Instance;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = $"api/correspondents/{correspondentId}/";

                var response = await client.DeleteAsync(url);

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCorrespondentAsync(CorrespondentsInfo correspondent)
        {
            if (correspondent == null) return false;

            try
            {
                var client = ApiClient.Instance;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = $"api/correspondents/{correspondent.Id}/";

                var json = System.Text.Json.JsonSerializer.Serialize(new
                {
                    name = correspondent.Name,
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

        public async Task<Correspondents?> CreateCorrespondentAsync(CorrespondentsInfo correspondent)
        {
            if (correspondent == null) return null;

            try
            {
                var client = ApiClient.Instance;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = "api/correspondents/";

                var json = System.Text.Json.JsonSerializer.Serialize(new
                {
                    name = correspondent.Name,
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var createdCorrespondent = await response.Content.ReadFromJsonAsync<Correspondents>();

                return createdCorrespondent;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
