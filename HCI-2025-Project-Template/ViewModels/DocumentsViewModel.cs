using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Ui;
using HCI_2025_Project_Template.Core.Services;
using Microsoft.Xaml.Behaviors.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace HCI_2025_Project_Template.ViewModels
{
    public class DocumentsViewModel : INotifyPropertyChanged
    {
        private readonly DocumentLoaderService _loader;

        private bool _isLoading;
        private int _currentPage = 1;
        private DispatcherTimer _searchTimer;
        private string _searchTitle = "";
        private string _currentSearchTitle = "";
        private bool _suppressLoad = false;
        private bool _suppressLoading = false;
        public int PageSize { get; set; } = 50;
        public List<Document> DocumentsAll { get; set; } = new();
        public ObservableCollection<Document> Documents { get; set; } = new();

        #region ObservableCollections - Binding ComboBoxes  
        public ObservableCollection<TagInfo> TagsList { get; set; } = new();
        public ObservableCollection<DocTypeInfo> TypesList { get; set; } = new();
        public ObservableCollection<CorrespondentsInfo> CorrespondentsList { get; set; } = new();
        #endregion

        #region ObservableCollections - Selected Filters 
        public ObservableCollection<TagInfo> SelectedTags { get; set; } = new();
        public ObservableCollection<DocTypeInfo> SelectedTypes { get; set; } = new();
        public ObservableCollection<CorrespondentsInfo> SelectedCorrespondents { get; set; } = new();
        public ObservableCollection<object> ActiveFilters { get; } = new ObservableCollection<object>();
        #endregion
        
        #region Properties
        public bool HasActiveFilters => ActiveFilters.Any() || !string.IsNullOrEmpty(SearchTitle);

        private int _totalDocuments;
        public int TotalDocuments
        {
            get => _totalDocuments;
            set
            {
                if (_totalDocuments != value)
                { 
                    _totalDocuments = value;
                    OnPropertyChanged(nameof(TotalDocuments));
                    OnPropertyChanged(nameof(DocumentCountDisplay));
                }
                
            }
        }
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
        public string SearchTitle
        {
            get => _searchTitle;
            set
            {
                _searchTitle = value;
                OnPropertyChanged(nameof(SearchTitle));

                if (_suppressLoad)
                {
                    _searchTimer?.Stop();
                    return;
                }


                if (_searchTimer == null)
                {
                    _searchTimer = new DispatcherTimer();

                    _searchTimer.Interval = TimeSpan.FromMilliseconds(500);
                    _searchTimer.Tick += async (s, e) =>
                    {
                        _searchTimer.Stop();
                        await LoadPageAsync(1, title: _searchTitle);

                        OnPropertyChanged(nameof(HasActiveFilters));
                    };
                }
                _searchTimer.Stop();
                _searchTimer.Start();
            }
        }
        #endregion

        public DocumentsViewModel(DocumentLoaderService loader)
        {
            _loader = loader;

            SelectedTags.CollectionChanged += async (s, e) =>
            {
                if (!_suppressLoad)
                    await LoadPageAsync(1, isTagChange: true);
            };
            SelectedTypes.CollectionChanged += async (s, e) =>
            {
                if (!_suppressLoad)
                    await LoadPageAsync(1, isTypeChange: true);
            };
            SelectedCorrespondents.CollectionChanged += async (s, e) =>
            {
                if (!_suppressLoad)
                    await LoadPageAsync(1, isCorrChange: true);
            };

            ActiveFilters.CollectionChanged += (s, e) =>
            { 
                OnPropertyChanged(nameof(HasActiveFilters));
                OnPropertyChanged(nameof(DocumentCountDisplay));
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Public Methods
        public async Task LoadInitialAsync()
        {
            IsLoading = true;
            await _loader.InitializeAsync();

            TagsList.Clear();
            foreach (var tag in _loader.TagsDict.Values)
                TagsList.Add(tag);

            TypesList.Clear();
            foreach (var type in _loader.TypesDict.Values)
                TypesList.Add(type);

            CorrespondentsList.Clear();
            foreach (var correspondent in _loader.CorrespondentsDict.Values)
                CorrespondentsList.Add(correspondent);

            TotalDocuments = await _loader.GetTotalDocumentsAsync();
            await LoadPageAsync(1);
            IsLoading = false;
        }
        public async Task LoadPageAsync(
            int page, 
            bool isTagChange = false, 
            bool isTypeChange = false, 
            bool isCorrChange = false, 
            bool isPagination = false, 
            string? title = null)

        {
            if (page < 1)
                page = 1;

            if (!_suppressLoading)
                IsLoading = true;

            try
            {
                var tagIds = SelectedTags.Select(t => t.Id).ToList();
                var typeIds = SelectedTypes.Select(t => t.Id).ToList();
                var corrIds = SelectedCorrespondents.Select(t => t.Id).ToList();


                string? effectiveTitle = title ?? _currentSearchTitle;


                bool isTitleChange = title != null && title != _currentSearchTitle;

                Debug.WriteLine($"isTagChange={isTagChange}, isTypeChange={isTypeChange}, isCorrChange={isCorrChange}, isTitleChange={isTitleChange}");


                if (isTagChange || isTypeChange || isCorrChange || isTitleChange)
                {

                    Debug.WriteLine("Ušao u if za računanje TotalDocuments");
                    TotalDocuments = await CalculateTotalDocumentsAsync(
                        tagIds.Count > 0 ? tagIds : null,
                        typeIds.Count > 0 ? typeIds : null,
                        corrIds.Count > 0 ? corrIds : null,
                        string.IsNullOrEmpty(effectiveTitle) ? null : effectiveTitle
                    );
                }

                DocumentsAll = await _loader.LoadPageAsync(page, PageSize, tagIds, typeIds, corrIds, effectiveTitle);

                if (DocumentsAll != null && DocumentsAll.Count > 0)
                {
                    CurrentPage = page;
                    UpdateDocumentsView();

                    if (title != null)
                        _currentSearchTitle = title;
                }

                else
                {
                    if (isTagChange || isTypeChange || isCorrChange || (isTitleChange && !isPagination))
                    {
                        Documents.Clear();
                        DocumentsAll = new List<Document>();
                        TotalDocuments = 0;
                    }
                }
            }
            finally
            {
                if (!_suppressLoading)
                    IsLoading = false;
            }
        }
        public async Task ClearFiltersAsync(bool clearAll)
        {
            IsLoading = true;
            _suppressLoad = true;
            _suppressLoading = true;

            try
            {
                _searchTimer?.Stop();
                SearchTitle = "";
                _currentSearchTitle = "";
                
                if (clearAll)
                {
                    SelectedTags.Clear();
                    SelectedTypes.Clear();
                    SelectedCorrespondents.Clear();
                    ActiveFilters.Clear();
                }

                OnPropertyChanged(nameof(HasActiveFilters));

                _suppressLoad = false;

                await LoadPageAsync(1, isTagChange: true, isTypeChange: true, isCorrChange: true);
            }
            finally 
            { 
                IsLoading = false; 
                _suppressLoading = false; 
            }
        }
        public string DocumentCountDisplay
        {
            get
            {
                if (HasActiveFilters)
                    return $"Documents: {TotalDocuments} (filtered)";
                else
                    return $"Documents: {TotalDocuments}";
            }
        }
        private void UpdateDocumentsView()
         {
             Documents.Clear();
             foreach (var doc in DocumentsAll)
                 Documents.Add(doc);

             OnPropertyChanged(nameof(Documents));
         }
        public async Task NextPageAsync()
        {
            IsLoading = true;
            
            await LoadPageAsync(CurrentPage + 1, isPagination: true,title: _currentSearchTitle);
            IsLoading = false;
        }
        public async Task PreviousPageAsync()
        {
            IsLoading = true;
            if (CurrentPage > 1)
                await LoadPageAsync(CurrentPage - 1, isPagination: true, title: _currentSearchTitle);
            IsLoading = false;
        }
        #endregion
        private async Task<int> CalculateTotalDocumentsAsync(List<int>? tagIds = null, List<int>? typeIds = null, List<int>? corrIds = null, string? title = null)
        {
            int total = 0;
            int page = 1;
            List<Document> docs;

            do
            {
                docs = await _loader.LoadPageAsync(page, PageSize, tagIds, typeIds,corrIds,title);
                total += docs.Count;
                page++;
            }
            while (docs != null && docs.Count > 0);

            return total;
        }
    
    }
}
