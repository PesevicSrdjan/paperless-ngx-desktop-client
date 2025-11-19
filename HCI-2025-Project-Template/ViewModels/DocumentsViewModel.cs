using HCI_2025.Core.Interfaces;
using HCI_2025.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace HCI_2025.ViewModel
{
    public class DocumentsViewModel : INotifyPropertyChanged
    {
        private readonly IDocumentService _documentService;
        private readonly ITagService _tagService;
        private readonly IDocTypeService _docTypeService;
        private bool _isLoading;
        private int _currentPage = 1;
        public int TotalDocuments { get; private set; }
        public int PageSize { get; set; } = 50;
        public Dictionary<int, string> TagsDict { get; set; } = new();
        public Dictionary<int, string> TypesDict { get; set; } = new();

        private readonly Dictionary<int, BitmapImage?> _thumbnailCache = new();
        public ObservableCollection<Document> Documents { get; set; } = new();

        public bool IsLoading
        {
            get => _isLoading;
            set 
            { 
                _isLoading = value; 
                OnPropertyChanged(nameof(IsLoading)); 
            }
        }
        
        public int CurrentPage
        {
            get => _currentPage;
            private set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                }
            }
        }
        
        public DocumentsViewModel(ITagService tagService, IDocTypeService docTypeService, IDocumentService documentService)
        {
            _documentService = documentService;
            _tagService = tagService;
            _docTypeService = docTypeService;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public async Task LoadInitialAsync()
        {
            IsLoading = true; 
            try
            {
                TotalDocuments = await _documentService.getOneDocAsync();
                OnPropertyChanged(nameof(TotalDocuments));

                var tags = await _tagService.getTagsAsync();
                TagsDict = tags.ToDictionary(t => t.Id, t => t.Name);

                var types = await _docTypeService.getDocTypeAsync();
                TypesDict = types.ToDictionary(t => t.Id, t => t.Name);

                await LoadPageAsync(1);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadPageAsync(int page)
        {
            if (page < 1)
                page = 1;

            IsLoading = true;

            try
            {
                var rawDocs = await _documentService.getAllDocumentsAsync(page, PageSize);

                if (rawDocs == null || rawDocs.Count == 0)
                    return;

                CurrentPage = page;

                var loadedDocs = new List<Document>();

                foreach (var d in rawDocs)
                {
                    BitmapImage? thumbBitmap;

                    if (!_thumbnailCache.TryGetValue(d.Id, out thumbBitmap))
                    {
                        thumbBitmap = await _documentService.getDocumentThumbAsync(d.Id);
                        _thumbnailCache[d.Id] = thumbBitmap;
                    }

                    loadedDocs.Add(new Document
                    {
                        Id = d.Id,
                        Title = d.Title,
                        Type = d.Type.HasValue && TypesDict.ContainsKey(d.Type.Value)
                                ? TypesDict[d.Type.Value]
                                : "Nepoznato",
                        Tags = d.Tags.Select(id => TagsDict.ContainsKey(id) ? TagsDict[id] : "Nepoznato").ToList(),
                        Date = d.Date,
                        Thumbnail = thumbBitmap
                    });
                }

                Documents.Clear();
                foreach (var doc in loadedDocs)
                    Documents.Add(doc);

                OnPropertyChanged(nameof(Documents));
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task NextPageAsync()
        {
            await LoadPageAsync(CurrentPage + 1);
        }

        public async Task PreviousPageAsync()
        {
            if (CurrentPage > 1)
                await LoadPageAsync(CurrentPage - 1);
        }
    }
}
