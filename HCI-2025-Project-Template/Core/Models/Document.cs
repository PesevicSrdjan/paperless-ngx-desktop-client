using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace HCI_2025.Core.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string? Title {  get; set; }
        public DateTime Date { get; set; }
        public List<string> Tags { get; set; } = new();
        public required string Type {  get; set; }
        public BitmapImage? Thumbnail { get; set; }

        public string TagsString => Tags.Count > 0 ? string.Join(", ", Tags) : "None";
        public string DateOnly => Date.ToShortDateString();

    }
}
