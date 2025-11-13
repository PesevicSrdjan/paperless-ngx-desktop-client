using HCI_2025.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025.Core.Interfaces
{
    public interface IDocumentService
    {
        Task<List<DocumentJson>> getAllDocumentsAsync(int page = 1, int pageSize = 20);
    }
}
