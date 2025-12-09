using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace HCI_2025_Project_Template.Core.Models.Ui
{
    public class Document
    {
        public int Id { get; set; }
        public string? Title {  get; set; }
        public DateTime Date { get; set; }
        public List<TagInfo> Tags { get; set; } = new();
        public required string Type {  get; set; }
        public BitmapImage? Thumbnail { get; set; }

        public string TagsString => Tags.Count > 0 ? string.Join(", ", Tags.Select(t => t.Name)) : "None";
        public string DateOnly => Date.ToShortDateString();

    }
}
