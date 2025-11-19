using HCI_2025.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025.Core.Interfaces
{
    public interface IDocTypeService
    {
        Task<List<DocType>> getDocTypeAsync();
    }
}
