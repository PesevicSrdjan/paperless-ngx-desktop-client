using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025.Core.Models
{
    public class Document
    {
        public string Title {  get; set; }
        public DateTime Date { get; set; }
        public List<string> Tags { get; set; } = new();

        public string TagsString => Tags.Count > 0 ? string.Join(", ", Tags) : "None";
        public string Type {  get; set; }
    }
}
