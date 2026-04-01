using CommunityToolkit.Mvvm.Input;
using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Ui;
using HCI_2025_Project_Template.Core.Services;
using HCI_2025_Project_Template.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HCI_2025_Project_Template.ViewModels
{
    public class CorrespondentsViewModel : PaginationViewModel, INameDialogViewModel, IDeleteDialogViewModel
    {
        private readonly ICorrespondentsService _correspondentsService;

        private bool _isLoading;
        private string _name;
        private CorrespondentsInfo _selectedCorrespondent;
        private CorrespondentsDialogMode _mode;

        public string DeleteTitle => LocalizationManager.Strings["DeleteCorrespondentTitle"];
        public string DeleteSubtitle => LocalizationManager.Strings["DeleteCorrespondentSubtitle"];
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public IRelayCommand<PageItem> GoToPageCommand { get; }

        public ObservableCollection<CorrespondentsInfo> CorrespondentsList { get; set; } = new();

        public enum CorrespondentsDialogMode
        {
            Create,
            Edit
        }

        public CorrespondentsDialogMode Mode
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
                return Mode == CorrespondentsDialogMode.Create ? loc["CreateCorrespondent"] : loc["EditCorrespondent"];
            }
        }

        public CorrespondentsViewModel(DocumentLoaderService loader)
        {
            _correspondentsService = new CorrespondentsService();

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
        public CorrespondentsInfo SelectedCorrespondent
        {
            get => _selectedCorrespondent;
            set
            {
                _selectedCorrespondent = value;
                if (_selectedCorrespondent != null)
                {
                    Name = _selectedCorrespondent.Name;
                }
                OnPropertyChanged(nameof(SelectedCorrespondent));

            }
        }

        private int _totalCorrespondents;
        public int TotalCorrespondents
        {
            get => _totalCorrespondents;
            set
            {
                if (_totalCorrespondents == value) return;
                _totalCorrespondents = value;
                OnPropertyChanged(nameof(TotalCorrespondents));
                OnPropertyChanged(nameof(TotalCorrespondentsText));
            }
        }

        public string TotalCorrespondentsText => string.Format(LocalizationManager.Strings["CorrespondentsFormat"], TotalCorrespondents);

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
                var response = await _correspondentsService.getCorrespondentsAsync(page, PageSize);

                if (response == null)
                    return;

                CurrentPage = page;

                TotalCorrespondents = response.Count;

                TotalPages = response.Count;
                TotalPages = (int)Math.Ceiling((double)TotalPages / PageSize);

                UpdatePages();

                CorrespondentsList.Clear();

                foreach (var t in response.Results)
                {
                    CorrespondentsList.Add(new CorrespondentsInfo
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
            if (Mode == CorrespondentsDialogMode.Edit)
            {
                if (SelectedCorrespondent == null) return;

                SelectedCorrespondent.Name = Name;
                await _correspondentsService.UpdateCorrespondentAsync(SelectedCorrespondent);
            }
            else
            {
                var newCorrespondent = new CorrespondentsInfo
                {
                    Name = Name
                };

                var created = await _correspondentsService.CreateCorrespondentAsync(newCorrespondent);
                if (created != null)
                {
                    var correspondentInfo = new CorrespondentsInfo
                    {
                        Id = created.Id,
                        Name = created.Name,
                    }; 
                    
                    CorrespondentsList.Add(correspondentInfo);

                    TotalCorrespondents += 1;
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
            if (SelectedCorrespondent == null) return;

            await _correspondentsService.DeleteCorrespondentAsync(SelectedCorrespondent.Id);

            CorrespondentsList.Remove(SelectedCorrespondent);
            SelectedCorrespondent = null;

            TotalCorrespondents -= 1;
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
