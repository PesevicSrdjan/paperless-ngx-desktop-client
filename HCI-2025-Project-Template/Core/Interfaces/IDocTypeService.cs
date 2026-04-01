using HCI_2025_Project_Template.Core.Models.Api;
using HCI_2025_Project_Template.Core.Models.Responses;
using HCI_2025_Project_Template.Core.Models.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Interfaces
{
    public interface IDocTypeService
    {
        Task<DocTypeResponse> getDocTypeAsync(int page = 1, int pageSize = 25);
        Task<List<DocType>> GetAllDocTypesAsync();
        Task<bool> DeleteDocumentTypeAsync(int documentTypeId);
        Task<bool> UpdateDocumentTypeAsync(DocTypeInfo documentType);
        Task<DocType?> CreateDocumentTypeAsync(DocTypeInfo documentType);

    }
}
