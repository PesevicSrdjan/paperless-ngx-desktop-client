using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Api;
using HCI_2025_Project_Template.Core.Models.Responses;
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
        // TODO
        // - Popravak dohvatanja Korespondenata
        // - Omogućavanje paginacije kao kod 'TagService'
        public async Task<List<Correspondents>> getCorrespondentsAsync()
        {
            try
            {
                var client = ApiClient.Instance;

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", AuthSession.Token);

                string url = "api/correspondents/";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return new List<Correspondents>();

                var data = await response.Content.ReadFromJsonAsync<CorrespondentsResponse>();

                if (data != null && data.Results != null) return data.Results;

                return new List<Correspondents>();
            }
            catch (HttpRequestException)
            {
                return new List<Correspondents>();
            }
        }
    }
}
