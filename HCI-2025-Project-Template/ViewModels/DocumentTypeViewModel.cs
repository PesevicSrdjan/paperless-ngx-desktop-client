using CommunityToolkit.Mvvm.Input;
using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Ui;
using HCI_2025_Project_Template.Core.Services;
using HCI_2025_Project_Template.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HCI_2025_Project_Template.ViewModels
{
    public class DocumentTypeViewModel : PaginationViewModel, INameDialogViewModel, IDeleteDialogViewModel
    {
        private readonly IDocTypeService _documentTypeService;
        private bool _isLoading;
        private string _name;
        private DocTypeInfo _selectedDocumentType;
        private DocumentTypeDialogMode _mode;
        public string DeleteTitle => LocalizationManager.Strings["DeleteDocumentTypeTitle"];
        public string DeleteSubtitle => LocalizationManager.Strings["DeleteDocumentTypeSubtitle"];
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public IRelayCommand<PageItem> GoToPageCommand { get; }

        public ObservableCollection<DocTypeInfo> DocumentTypesList { get; set; } = new();

        public enum DocumentTypeDialogMode
        {
            Create,
            Edit
        }

        public DocumentTypeDialogMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                OnPropertyChanged(nameof(Mode));
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Title
        {
            get
            {
                var loc = LocalizationManager.Strings;
                return Mode == DocumentTypeDialogMode.Create ? loc["CreateDocumentType"] : loc["EditDocumentType"];
            }
        }

        public DocumentTypeViewModel(DocumentLoaderService loader)
        {
            _documentTypeService = new DocTypeService();

            NextPageCommand = new AsyncRelayCommand(NextPageAsync);
            PreviousPageCommand = new AsyncRelayCommand(PreviousPageAsync);
            GoToPageCommand = new AsyncRelayCommand<PageItem>(GoToPageAsync);
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
        public DocTypeInfo SelectedDocumentType
        {
            get => _selectedDocumentType;
            set
            {
                _selectedDocumentType = value;
                if (_selectedDocumentType != null)
                {
                    Name = _selectedDocumentType.Name;
                }
                OnPropertyChanged(nameof(SelectedDocumentType));

            }
        }

        private int _totalDocumentTypes;
        public int TotalDocumentTypes
        {
            get => _totalDocumentTypes;
            set
            {
                if (_totalDocumentTypes == value) return;
                _totalDocumentTypes = value;
                OnPropertyChanged(nameof(TotalDocumentTypes));
                OnPropertyChanged(nameof(TotalDocumentTypesText));
            }
        }

        public string TotalDocumentTypesText => string.Format(LocalizationManager.Strings["DocumentTypeFormat"], TotalDocumentTypes);

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public int PageSize { get; set; } = 25;

        public event Action? NoInternetDetected;
        public async Task LoadPageAsync(int page)
        {
            IsLoading = true;

            if (!await NetworkHelper.HasInternetCachedAsync())
            {
                NoInternetDetected?.Invoke();
                IsLoading = false;
                return;
            }

            try
            {
                var response = await _documentTypeService.getDocTypeAsync(page, PageSize);

                if (response == null)
                    return;

                CurrentPage = page;

                TotalDocumentTypes = response.Count;

                TotalPages = response.Count;
                TotalPages = (int)Math.Ceiling((double)TotalPages / PageSize);

                UpdatePages();

                DocumentTypesList.Clear();

                foreach (var t in response.Results)
                {
                    DocumentTypesList.Add(new DocTypeInfo
                    {
                        Id = t.Id,
                        Name = t.Name,
                        DocumentCount = t.DocumentCount
                    });
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
        public async Task SaveAsync()
        {
            if (Mode == DocumentTypeDialogMode.Edit)
            {
                if (SelectedDocumentType == null) return;

                SelectedDocumentType.Name = Name;
                await _documentTypeService.UpdateDocumentTypeAsync(SelectedDocumentType);
            }
            else
            {
                var newDocumentType = new DocTypeInfo
                {
                    Name = Name
                };

                var created = await _documentTypeService.CreateDocumentTypeAsync(newDocumentType);
                if (created != null)
                {
                    var documentTypeInfo = new DocTypeInfo
                    {
                        Id = created.Id,
                        Name = created.Name,
                    };

                    DocumentTypesList.Add(documentTypeInfo);

                    TotalDocumentTypes += 1;
                }
            }
        }

        public bool Validate(out string error)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                error = LocalizationManager.Strings["NameRequired"];
                return false;
            }

            error = string.Empty;
            return true;
        }
        public async Task DeleteAsync()
        {
            if (SelectedDocumentType == null) return;

            await _documentTypeService.DeleteDocumentTypeAsync(SelectedDocumentType.Id);

            DocumentTypesList.Remove(SelectedDocumentType);
            SelectedDocumentType = null;

            TotalDocumentTypes -= 1;
        }

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
