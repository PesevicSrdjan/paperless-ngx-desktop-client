using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCI_2025.Core.Models
{
    public class DocumentJson
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("created")]
        public DateTime Date { get; set; }

        [JsonPropertyName("tags")]
        public List<int> Tags { get; set; } = new();

        [JsonPropertyName("document_type")]
        public int? Type { get; set; }
    }
}
