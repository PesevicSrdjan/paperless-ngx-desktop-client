using HCI_2025_Project_Template.Core.Models.Api;
using HCI_2025_Project_Template.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Interfaces
{
    public interface ICorrespondentsService
    {
        Task<List<Correspondents>> getCorrespondentsAsync();
    }
}
