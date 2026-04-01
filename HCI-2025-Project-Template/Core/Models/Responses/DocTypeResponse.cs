using HCI_2025_Project_Template.Core.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Models.Responses
{
    public class DocTypeResponse
    {
        public int Count { get; set; }
        public List<DocType>? Results { get; set; }
    }
}
