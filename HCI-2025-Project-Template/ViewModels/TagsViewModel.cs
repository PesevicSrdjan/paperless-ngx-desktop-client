using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Ui;
using HCI_2025_Project_Template.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HCI_2025_Project_Template.ViewModels
{
    public class TagsViewModel : INotifyPropertyChanged
    {
        private readonly DocumentLoaderService _loader;
        private readonly ITagService _tagService;
        private bool _isLoading;
        private string _name;
        private TagInfo _selectedTag;
        private Color _selectedColor;
        private string _colorHex;
        private TagDialogMode _mode;
        public enum TagDialogMode
        {
            Create,
            Edit
        }
        public ObservableCollection<TagInfo> TagsList { get; set; } = new();
        public TagsViewModel(DocumentLoaderService loader)
        {
            _loader = loader;
            _tagService = new TagService();
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

        public string Title => Mode == TagDialogMode.Create ? "Create tag" : "Edit tag";

        public async Task LoadInitialAsync()
        {
            IsLoading = true;
            try
            {
                await _loader.InitializeAsync();

                TagsList.Clear();
                foreach (var tag in _loader.TagsDict.Values)
                    TagsList.Add(tag);
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
            }
        }

        public async Task DeleteTagAsync()
        {
            if (SelectedTag == null) return;

            bool deleted = await _tagService.DeleteTagAsync(SelectedTag.Id);
            if (deleted)
            {
                TagsList.Remove(SelectedTag);
                SelectedTag = null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
