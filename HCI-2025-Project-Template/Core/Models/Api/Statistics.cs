using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.Core.Models.Api
{
    public class Statistics
    {
        [JsonPropertyName("documents_total")]
        public int DocumentsTotal { get; set; }

        [JsonPropertyName("documents_inbox")]
        public int? DocumentsInbox { get; set; }

        [JsonPropertyName("inbox_tag")]
        public int? InboxTag { get; set; }

        [JsonPropertyName("inbox_tags")]
        public List<int>? InboxTags { get; set; }

        [JsonPropertyName("document_file_type_counts")]
        public List<FileTypeCountDto> DocumentFileTypeCounts { get; set; }

        [JsonPropertyName("character_count")]
        public long CharacterCount { get; set; }

        [JsonPropertyName("tag_count")]
        public int TagCount { get; set; }

        [JsonPropertyName("correspondent_count")]
        public int CorrespondentCount { get; set; }

        [JsonPropertyName("document_type_count")]
        public int DocumentTypeCount { get; set; }

        [JsonPropertyName("storage_path_count")]
        public int StoragePathCount { get; set; }

        [JsonPropertyName("current_asn")]
        public int CurrentAsn { get; set; }
    }
    public class FileTypeCountDto
    {
        [JsonPropertyName("mime_type")]
        public string MimeType { get; set; }

        [JsonPropertyName("mime_type_count")]
        public int MimeTypeCount { get; set; }
    }
}
