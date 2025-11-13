using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025.Core.Models
{
    public class DocumentsResponse
    {
        public int count { get; set; }
        public List<DocumentJson> results { get; set; } = new();
    }
}
