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
    public class DocTypeService : IDocTypeService
    {
        public async Task<DocTypeResponse> getDocTypeAsync(int page = 1, int pageSize = 25)
        {
            try
            {
                var client = ApiClient.Instance;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = $"api/document_types/?page={page}&page_size={pageSize}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<DocTypeResponse>();

            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<List<DocType>> GetAllDocTypesAsync()
        {
            var allDocTypes = new List<DocType>();
            int page = 1;
            int pageSize = 25;

            DocTypeResponse? response;

            do
            {
                response = await getDocTypeAsync(page, pageSize);

                if (response == null || response.Results.Count == 0)
                    break;

                allDocTypes.AddRange(response.Results);

                page++;

            } while (allDocTypes.Count < response.Count);

            return allDocTypes;
        }

        public async Task<bool> DeleteDocumentTypeAsync(int documentTypeId)
        {
            try
            {
                var client = ApiClient.Instance;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = $"api/document_types/{documentTypeId}/";

                var response = await client.DeleteAsync(url);

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateDocumentTypeAsync(DocTypeInfo documentType)
        {
            if (documentType == null) return false;

            try
            {
                var client = ApiClient.Instance;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = $"api/document_types/{documentType.Id}/";

                var json = System.Text.Json.JsonSerializer.Serialize(new
                {
                    name = documentType.Name,
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

        public async Task<DocType?> CreateDocumentTypeAsync(DocTypeInfo documentType)
        {
            if (documentType == null) return null;

            try
            {
                var client = ApiClient.Instance;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = "api/document_types/";

                var json = System.Text.Json.JsonSerializer.Serialize(new
                {
                    name = documentType.Name,
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var createdDocumentType = await response.Content.ReadFromJsonAsync<DocType>();

                return createdDocumentType;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
