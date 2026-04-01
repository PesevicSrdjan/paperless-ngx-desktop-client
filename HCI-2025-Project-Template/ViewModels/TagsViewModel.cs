using CommunityToolkit.Mvvm.Input;
using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Api;
using HCI_2025_Project_Template.Core.Models.Ui;
using HCI_2025_Project_Template.Core.Services;
using HCI_2025_Project_Template.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace HCI_2025_Project_Template.ViewModels
{
    public class TagsViewModel : PaginationViewModel, IDeleteDialogViewModel
    {
        private readonly ITagService _tagService;

        private bool _isLoading;
        private string _name;
        private TagInfo _selectedTag;
        private Color _selectedColor;
        private string _colorHex;
        private TagDialogMode _mode;

        public string DeleteTitle => LocalizationManager.Strings["DeleteTagTitle"];
        public string DeleteSubtitle => LocalizationManager.Strings["DeleteTagSubtitle"];
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public IRelayCommand<PageItem> GoToPageCommand { get; }

        public enum TagDialogMode
        {
            Create,
            Edit
        }
        public ObservableCollection<TagInfo> TagsList { get; set; } = new();
        public TagsViewModel(DocumentLoaderService loader)
        {
            _tagService = new TagService();

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
        public TagInfo SelectedTag
        {
            get => _selectedTag;
            set
            {
                _selectedTag = value;
                if (_selectedTag != null)
                {
                    Name = _selectedTag.Name;
                    SelectedColor = (Color)ColorConverter.ConvertFromString(_selectedTag.Color);
                }
                OnPropertyChanged(nameof(SelectedTag));
                
            }
        }

        private int _totalTags;
        public int TotalTags
        {
            get => _totalTags;
            set
            {
                if (_totalTags == value) return;
                _totalTags = value;
                OnPropertyChanged(nameof(TotalTags));
                OnPropertyChanged(nameof(TotalTagsText));
            }
        }

        public string TotalTagsText => string.Format(LocalizationManager.Strings["TagsFormat"], TotalTags);
        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                _selectedColor = value;
                ColorHex = $"#{value.R:X2}{value.G:X2}{value.B:X2}";
                OnPropertyChanged(nameof(SelectedColor));
                OnPropertyChanged(nameof(ColorHex));
            }
        }
        public string ColorHex
        {
            get => _colorHex;
            set
            {
                if (_colorHex == value) return;

                _colorHex = value;

                try
                {
                    var converted = (Color)ColorConverter.ConvertFromString(value);
                    _selectedColor = converted;
                }
                catch
                {
                    // Nevalidan unos, ignoriši
                }

                OnPropertyChanged(nameof(SelectedColor));
                OnPropertyChanged(nameof(ColorHex));
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                _name = value; 
                OnPropertyChanged(nameof(Name));
            }
        }

        public TagDialogMode Mode
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
                return Mode == TagDialogMode.Create ? loc["CreateTag"] : loc["EditTag"];
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
                var response = await _tagService.getTagsAsync(page, PageSize);

                if (response == null)
                    return;

                CurrentPage = page;

                TotalTags = response.Count;

                TotalPages = response.Count;
                TotalPages = (int)Math.Ceiling((double)TotalPages/ PageSize);

                UpdatePages(); 

                TagsList.Clear();

                foreach (var t in response.Results)
                {
                    TagsList.Add(new TagInfo
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Color = t.TagColor,
                        DocumentCount = t.DocumentCount
                    });
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task EditTagAsync()
        {
            if (SelectedTag == null) return;

            SelectedTag.Name = Name;
            SelectedTag.Color = ColorHex;

            await _tagService.UpdateTagAsync(SelectedTag);
        }

        public async Task CreateTagAsync()
        {
            var newTag = new TagInfo
            {
                Name = Name,
                Color = ColorHex
            };

            var createdTag = await _tagService.CreateTagAsync(newTag);

            if (createdTag != null)
            {
                var tagInfo = new TagInfo
                {
                    Id = createdTag.Id,
                    Name = createdTag.Name,
                    Color = createdTag.TagColor
                };

                TagsList.Add(tagInfo);

                TotalTags = TotalTags + 1;
            }
        }
        public bool Validate(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                errorMessage = LocalizationManager.Strings["NameRequired"];
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public async Task DeleteAsync()
        {
            if (SelectedTag == null) return;

            bool deleted = await _tagService.DeleteTagAsync(SelectedTag.Id);
            if (deleted)
            {
                TagsList.Remove(SelectedTag);
                SelectedTag = null;
            }
            TotalTags = TotalTags - 1;
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
