using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Api;
using HCI_2025_Project_Template.Core.Models.Responses;
using HCI_2025_Project_Template.Core.Models.Ui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace HCI_2025_Project_Template.Core.Services
{
    public class DocumentService : IDocumentService
    {
        public async Task<List<DocumentJson>> getAllDocumentsAsync(
            int page = 1, int pageSize = 50,
            List<int>? tagIds = null, 
            List<int>? typeIds = null, 
            List<int>? corrIds = null,
            string? title = null)
        {
            try
            {
                // 1. Dohvatanje HTTP instance.
                var client = ApiClient.Instance;

                // 2. Postavljanje Tokena u Header HTTP zaglavlja.
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                List<string> query = new();

                // 3. Konstrukcija URL- a.
                if (tagIds != null && tagIds.Count > 0)
                {
                    string tagParam = string.Join(",", tagIds);
                    query.Add($"tags__id__all={tagParam}");
                }

                if (typeIds != null && typeIds.Count > 0)
                {
                    string typeParam = string.Join(",", typeIds);
                    query.Add($"document_type__id__in={typeParam}");
                }

                if (corrIds != null && corrIds.Count > 0)
                {
                    string corrParam = string.Join(",", corrIds);
                    query.Add($"correspondent__id__in={corrParam}");
                }

                if (!string.IsNullOrWhiteSpace(title))
                {
                    string encodedTitle = WebUtility.UrlEncode(title);
                    query.Add($"title_content={encodedTitle}");
                }

                query.Add($"page={page}");
                query.Add($"page_size={pageSize}");

                string finalQuery = string.Join("&", query);
                string url = $"api/documents/?{finalQuery}";


                // 4. Slanje zahtjeva.
                var response = await client.GetAsync(url);

                // 5. Provjera statusnog koda.
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new List<DocumentJson>();
                }

                response.EnsureSuccessStatusCode();

                //6. Čitaj JSON
                var data = await response.Content.ReadFromJsonAsync<DocumentsResponse>();

                if (data != null && data.results != null)
                {
                    return data.results;
                }
                else
                {
                    return new List<DocumentJson>();
                }
            }
            catch (HttpRequestException)
            {
                return new List<DocumentJson>();
            }
        }

        public async Task<int> getTotalDocumentsAsync(
            List<int>? tagIds = null,
            List<int>? typeIds = null,
            List<int>? corrIds = null,
            string? title = null)
        {
            try
            {
                var client = ApiClient.Instance;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                List<string> query = new();

                if (tagIds != null && tagIds.Count > 0)
                {
                    string tagParam = string.Join(",", tagIds);
                    query.Add($"tags__id__all={tagParam}");
                }

                if (typeIds != null && typeIds.Count > 0)
                {
                    string typeParam = string.Join(",", typeIds);
                    query.Add($"document_type__id__in={typeParam}");
                }

                if (corrIds != null && corrIds.Count > 0)
                {
                    string corrParam = string.Join(",", corrIds);
                    query.Add($"correspondent__id__in={corrParam}");
                }

                if (!string.IsNullOrWhiteSpace(title))
                {
                    string encodedTitle = WebUtility.UrlEncode(title);
                    query.Add($"title_content={encodedTitle}");
                }

                query.Add($"page=1");
                query.Add($"page_size=1");

                string finalQuery = string.Join("&", query);
                string url = $"api/documents/?{finalQuery}";

                var response = await client.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return 0;

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<DocumentsResponse>();
                return data?.count ?? 0;
            }
            catch (HttpRequestException)
            {
                return 0;
            }
        }

        public async Task<BitmapImage?> getDocumentThumbAsync(int idDoc)
        {
            try
            {
                var client = ApiClient.Instance;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = $"api/documents/{idDoc}/thumb/";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    // Ako nije uspjelo (404, 500 itd.) vrati null ili prazni array
                    return null;
                }

                var bytes = await response.Content.ReadAsByteArrayAsync();

                return ConvertToBitmapImage(bytes);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        private BitmapImage? ConvertToBitmapImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            using var ms = new MemoryStream(bytes);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        public async Task<string?> UploadDocumentAsync(FileMetadata file, IProgress<double>? progress = null)
        {
            try
            {
                var client = ApiClient.Instance;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                using var content = new MultipartFormDataContent();

                var fileStream = File.OpenRead(file.FilePath);
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                content.Add(fileContent, "document", Path.GetFileName(file.FilePath));

                if (file.DocumentName != null)
                    content.Add(new StringContent(file.DocumentName), "title");

                if (file.DocumentDate.HasValue)
                    content.Add(new StringContent(file.DocumentDate.Value.ToString("yyyy-MM-dd")), "created");

                if (file.DocumentType != null)
                    content.Add(new StringContent(file.DocumentType.Id.ToString()), "document_type");

                if (file.Correspondent != null)
                    content.Add(new StringContent(file.Correspondent.Id.ToString()), "correspondent");

                if (file.SelectedTags.Any())
                    foreach (var tag in file.SelectedTags)
                        content.Add(new StringContent(tag.Id.ToString()), "tags");

                var progressContent = new ProgressableStreamContent(content, 4096, progress);

                var response = await client.PostAsync("api/documents/post_document/", progressContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<(List<DocumentJson> Results, int Count)> GetDocumentsAsync(
            int page = 1,
            int pageSize = 50,
            List<int>? tagIds = null,
            List<int>? typeIds = null,
            List<int>? corrIds = null,
            string? title = null)
        {
            try
            {
                var client = ApiClient.Instance;
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Token", AuthSession.Token);

                List<string> query = new();

                if (tagIds?.Count > 0)
                    query.Add($"tags__id__all={string.Join(",", tagIds)}");

                if (typeIds?.Count > 0)
                    query.Add($"document_type__id__in={string.Join(",", typeIds)}");

                if (corrIds?.Count > 0)
                    query.Add($"correspondent__id__in={string.Join(",", corrIds)}");

                if (!string.IsNullOrWhiteSpace(title))
                    query.Add($"title_content={WebUtility.UrlEncode(title)}");

                query.Add($"page={page}");
                query.Add($"page_size={pageSize}");

                string url = $"api/documents/?{string.Join("&", query)}";

                var response = await client.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return (new List<DocumentJson>(), 0);

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<DocumentsResponse>();

                return (data?.results ?? new List<DocumentJson>(), data?.count ?? 0);
            }
            catch
            {
                return (new List<DocumentJson>(), 0);
            }
        }
    }
}
