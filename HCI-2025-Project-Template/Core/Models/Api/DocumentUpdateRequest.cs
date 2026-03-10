using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Models.Api
{
    public class DocumentUpdateRequest
    {
        public string? title { get; set; }

        public string? created { get; set; }

        public int? document_type { get; set; }

        public int? correspondent { get; set; }

        public int? storage_path { get; set; }

        public List<int>? tags { get; set; }
    }
}
