using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace HCI_2025_Project_Template.Core.Models.Ui
{
    public class Document : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string? Title {  get; set; }
        public DateTime Date { get; set; }
        public List<TagInfo> Tags { get; set; } = new();
        public required string Type {  get; set; }

        private BitmapImage? _thumbnail;
        public BitmapImage? Thumbnail
        {
            get => _thumbnail;
            set
            {
                if (_thumbnail != value)
                {
                    _thumbnail = value;
                    OnPropertyChanged(nameof(Thumbnail));
                }
            }
        }

        private bool _isThumbnailLoading = false;
        public bool IsThumbnailLoading
        {
            get => _isThumbnailLoading;
            set { _isThumbnailLoading = value; OnPropertyChanged(nameof(IsThumbnailLoading)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public string TagsString => Tags.Count > 0 ? string.Join(", ", Tags.Select(t => t.Name)) : "None";
        public string DateOnly => Date.ToShortDateString();

    }
}
