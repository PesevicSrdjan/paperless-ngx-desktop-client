using CommunityToolkit.Mvvm.Input;
using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Api;
using HCI_2025_Project_Template.Core.Models.Ui;
using HCI_2025_Project_Template.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace HCI_2025_Project_Template.ViewModels
{
    public class DocumentPreviewViewModel : INotifyPropertyChanged
    {
        private readonly IDocumentService _documentService;
        private readonly DocumentLoaderService _loader;
        

        private bool _isEdited;
        private string _title = string.Empty;
        private DateTime? _dateCreated;
        private CorrespondentsInfo? _selectedCorrespondent;
        private DocTypeInfo? _selectedDocumentType;

        public IDocumentService DocumentService => _documentService;
        public ICollectionView AvailableTagsView { get; }

        #region Constructor
        public DocumentPreviewViewModel(DocumentLoaderService loader)
        {
            _loader = loader;
            _documentService = loader.DocumentService;

            AvailableTagsView = CollectionViewSource.GetDefaultView(Tags);
            AvailableTagsView.Filter = FilterAvailableTags;
            SelectedTags.CollectionChanged += (s, e) =>
            {
                IsEdited = true;
                AvailableTagsView.Refresh();
            };
        }
        #endregion

        public bool IsEdited
        {
            get => _isEdited;
            set
            {
                if (_isEdited != value)
                {
                    _isEdited = value;
                    OnPropertyChanged(nameof(IsEdited));
                }
            }
        }
        private bool FilterAvailableTags(object obj)
        {
            if (obj is TagInfo tag)
            {
                return !SelectedTags.Any(t => t.Id == tag.Id);
            }
            return false;
        }

        #region Metadata Properties
        
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    IsEdited = true;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }
        public DateTime? DateCreated
        {
            get => _dateCreated;
            set
            {
                if (_dateCreated != value)
                {
                    _dateCreated = value;
                    IsEdited = true;
                    OnPropertyChanged(nameof(DateCreated));
                }
            }
        }
        public ObservableCollection<CorrespondentsInfo> Correspondents { get; } = new();
        
        public CorrespondentsInfo? SelectedCorrespondent
        {
            get => _selectedCorrespondent;
            set
            {
                if (_selectedCorrespondent != value)
                {
                    _selectedCorrespondent = value;
                    IsEdited = true;
                    OnPropertyChanged(nameof(SelectedCorrespondent));
                }
            }
        }
        public async Task InitializeAsync()
        {
            await _loader.InitializeAsync(); 

            Correspondents.Clear();
            foreach (var c in _loader.CorrespondentsDict.Values)
                Correspondents.Add(c);

            DocumentTypes.Clear();
            foreach (var t in _loader.TypesDict.Values)
                DocumentTypes.Add(t);

            Tags.Clear();
            foreach (var tag in _loader.TagsDict.Values)
                Tags.Add(tag);
        }

        public ObservableCollection<DocTypeInfo> DocumentTypes { get; } = new();
        
        public DocTypeInfo? SelectedDocumentType
        {
            get => _selectedDocumentType;
            set
            {
                if (_selectedDocumentType != value)
                {
                    _selectedDocumentType = value;
                    IsEdited = true;
                    OnPropertyChanged(nameof(SelectedDocumentType));
                }
            }
        }
        public ObservableCollection<TagInfo> Tags { get; } = new();
        public ObservableCollection<TagInfo> SelectedTags { get; } = new();
        #endregion

        #region Preview Properties
        private byte[]? _previewBytes;
        public byte[]? PreviewBytes
        {
            get => _previewBytes;
            set
            {
                if (_previewBytes != value)
                {
                    _previewBytes = value;
                    OnPropertyChanged(nameof(PreviewBytes));
                }
            }
        }
        private string _mimeType = string.Empty;
        public string MimeType
        {
            get => _mimeType;
            set
            {
                if (_mimeType != value)
                {
                    _mimeType = value;
                    OnPropertyChanged(nameof(MimeType));
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Učitaj preview dokumenta preko servisa
        /// </summary>
        public async Task LoadPreviewAsync(int documentId)
        {
            IsLoading = true;

            try
            {
                var bytes = await _documentService.GetDocumentPreviewAsync(documentId);
                PreviewBytes = bytes;

            }
            finally
            {
                IsLoading = false;
            }
        }

        
        #endregion
        public void LoadDocumentMetadata(Document doc)
        {
            DocumentId = doc.Id;
            Title = doc.Title;
            DateCreated = doc.Date;
            SelectedDocumentType = DocumentTypes.FirstOrDefault(t => t.Name == doc.Type);

            if (!string.IsNullOrEmpty(doc.Correspondent))
            {
                SelectedCorrespondent = Correspondents.FirstOrDefault(c => c.Name == doc.Correspondent);
            }
            else
            {
                SelectedCorrespondent = null;
            }

            SelectedTags.Clear();
            foreach (var tag in doc.Tags)
            {
                SelectedTags.Add(new TagInfo
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Color = tag.Color
                });
            }
            IsEdited = false;
        }

        public int DocumentId { get; set; }
        public DocumentUpdateRequest CreateUpdateRequest()
        {
            return new DocumentUpdateRequest
            {
                title = Title,
                created = DateCreated?.ToString("yyyy-MM-dd"),
                correspondent = SelectedCorrespondent?.Id,
                document_type = SelectedDocumentType?.Id,
                storage_path = null,
                tags = SelectedTags.Select(t => t.Id).ToList()
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
