using CommunityToolkit.Mvvm.Input;
using HCI_2025_Project_Template.Core.Models.Ui;
using HCI_2025_Project_Template.Core.Services;
using HCI_2025_Project_Template.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace HCI_2025_Project_Template.ViewModels
{
    public class DocumentsViewModel : PaginationViewModel
    {
        private readonly DocumentLoaderService _loader;

        private bool _isLoading;
        private DispatcherTimer? _searchTimer;
        private DispatcherTimer? _filterTimer;
        private string _searchTitle = "";
        private string _currentSearchTitle = "";
        private bool _suppressLoad = false;
        private bool _suppressLoading = false;
        private bool _isFilteredAfterLoad = false;
        private bool _isPageLoading;
        private int _totalDocumentsUnfiltered;

        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public IRelayCommand<PageItem> GoToPageCommand { get; }

        public DocumentLoaderService Loader => _loader;

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
        public bool HasSelectedTags => SelectedTags.Count > 0;
        public bool HasSelectedCorrespondent => SelectedCorrespondents.Count > 0;
        public bool HasSelectedDocType => SelectedTypes.Count > 0;
        public bool HasActiveFilters => _isFilteredAfterLoad;

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
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
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

                if (_suppressLoad) return;

                _searchTimer?.Stop();
                _searchTimer?.Start();
            }
        }

        public string DocumentCountDisplay
        {
            get
            {
                var loc = LocalizationManager.Strings;

                if (HasActiveFilters)
                {
                    return string.Format(
                        loc["DocumentsFilteredFormat"],
                        TotalDocuments,
                        _totalDocumentsUnfiltered);
                }

                return string.Format(
                    loc["DocumentsFormat"],
                    TotalDocuments);
            }
        }
        #endregion

        public DocumentsViewModel(DocumentLoaderService loader)
        {
            _loader = loader;

            NextPageCommand = new AsyncRelayCommand(NextPageAsync);
            PreviousPageCommand = new AsyncRelayCommand(PreviousPageAsync);
            GoToPageCommand = new AsyncRelayCommand<PageItem>(GoToPageAsync);

            #region Timer za Search
            _searchTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _searchTimer.Tick += async (s, e) =>
            {
                _searchTimer.Stop();
                _currentSearchTitle = SearchTitle;
                await ReloadWithCurrentFilters();
            };
            #endregion

            #region Timer za Filtere (tag, type, correspondent)
            _filterTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _filterTimer.Tick += async (s, e) =>
            {
                _filterTimer.Stop();
                await ReloadWithCurrentFilters();
            };

            SelectedTags.CollectionChanged += OnFiltersChanged;
            SelectedTypes.CollectionChanged += OnFiltersChanged;
            SelectedCorrespondents.CollectionChanged += OnFiltersChanged;
            #endregion

            ActiveFilters.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(DocumentCountDisplay));
            };
        }

        #region Public Methods

        public async Task LoadInitialAsync()
        {
            IsLoading = true;
            await _loader.InitializeAsync();

            // Popuni liste filtera
            TagsList.Clear();
            foreach (var tag in _loader.TagsDict.Values)
                TagsList.Add(tag);

            TypesList.Clear();
            foreach (var type in _loader.TypesDict.Values)
                TypesList.Add(type);

            CorrespondentsList.Clear();
            foreach (var corr in _loader.CorrespondentsDict.Values)
                CorrespondentsList.Add(corr);

            // Load prve stranice i total count kroz loader
            var documents = await _loader.LoadPageAsync(1, PageSize);
            DocumentsAll = documents;
            TotalDocuments = documents.FirstOrDefault()?.TotalCount ?? 0;
            _totalDocumentsUnfiltered = TotalDocuments;

            TotalPages = (int)Math.Ceiling(TotalDocuments / (double)PageSize);

            Debug.WriteLine($"TotalDocuments: {TotalDocuments}");
            Debug.WriteLine($"TotalPages: {TotalPages}");

            if (DocumentsAll.Count > 0)
            {
                CurrentPage = 1;
                UpdateDocumentsView();
            }
            IsLoading = false;
        }

        public async Task LoadPageAsync(int page, string? title = null, bool forceRecalcTotalDocuments = false, bool isPagination = false)
        {
            if (_isPageLoading)
                return;

            if (page > TotalPages && TotalPages > 0)
                page = TotalPages;

            if (page < 1) page = 1;

            if (!_suppressLoading)
                IsLoading = true;

            try
            {
                _isPageLoading = true;

                var tagIds = SelectedTags.Select(t => t.Id).ToList();
                var typeIds = SelectedTypes.Select(t => t.Id).ToList();
                var corrIds = SelectedCorrespondents.Select(t => t.Id).ToList();

                string? effectiveTitle = title ?? _currentSearchTitle;

                // Load stranice i mapiranje kroz loader
                var documents = await _loader.LoadPageAsync(
                    page,
                    PageSize,
                    tagIds.Count > 0 ? tagIds : null,
                    typeIds.Count > 0 ? typeIds : null,
                    corrIds.Count > 0 ? corrIds : null,
                    string.IsNullOrEmpty(effectiveTitle) ? null : effectiveTitle
                );

                DocumentsAll = documents;
                TotalDocuments = documents.FirstOrDefault()?.TotalCount ?? 0; ;
                TotalPages = (int)Math.Ceiling(TotalDocuments / (double)PageSize);

                if (DocumentsAll != null && DocumentsAll.Count > 0)
                {
                    CurrentPage = page;
                    UpdateDocumentsView();
                }
                else if (!isPagination)
                {
                    // Nema dokumenata nakon filtera/search-a
                    Documents.Clear();
                    DocumentsAll = new List<Document>();
                    TotalDocuments = 0;
                }

                if (title != null)
                    _currentSearchTitle = title;
            }
            finally
            {
                if (!_suppressLoading)
                    IsLoading = false;

                _isPageLoading = false;

                _isFilteredAfterLoad = SelectedTags.Count > 0
                       || SelectedTypes.Count > 0
                       || SelectedCorrespondents.Count > 0
                       || !string.IsNullOrEmpty(_currentSearchTitle);

                OnPropertyChanged(nameof(HasActiveFilters));
                OnPropertyChanged(nameof(DocumentCountDisplay));
            }
        }

        public async Task ClearFiltersAsync(bool clearAll)
        {
            _suppressLoad = true;

            _searchTimer?.Stop();
            SearchTitle = "";
            _currentSearchTitle = "";

            if (clearAll)
            {
                SelectedTags.Clear();
                SelectedTypes.Clear();
                SelectedCorrespondents.Clear();
                ActiveFilters.Clear();

                _isFilteredAfterLoad = false;

            }

            OnPropertyChanged(nameof(HasActiveFilters));
            OnPropertyChanged(nameof(DocumentCountDisplay));

            _suppressLoad = false;

            await ReloadWithCurrentFilters();
        }

        #endregion

        #region Private Methods
        private void OnFiltersChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasSelectedTags));
            OnPropertyChanged(nameof(HasSelectedDocType));
            OnPropertyChanged(nameof(HasSelectedCorrespondent));

            if (_suppressLoad) return;

            _filterTimer?.Stop();
            _filterTimer?.Start();
        }

        private async Task ReloadWithCurrentFilters()
        {
            await LoadPageAsync(1, title: _currentSearchTitle, forceRecalcTotalDocuments: true);

            OnPropertyChanged(nameof(HasActiveFilters));
        }

        private void UpdateDocumentsView()
        {
            Documents.Clear();
            foreach (var doc in DocumentsAll)
                Documents.Add(doc);
        }
        #endregion


        private async Task GoToPageAsync(PageItem page)
        {
            if (page?.Number != null)
                await LoadPageAsync(page.Number.Value);
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