using HCI_2025_Project_Template.Core.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> loginAsync(string username, string password);
    }
}
