using HCI_2025_Project_Template.Core.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Models.Responses
{
    public class TagResponse
    {
        public int Count { get; set; }
        public required List<Tag> Results { get; set; }
    }
}
