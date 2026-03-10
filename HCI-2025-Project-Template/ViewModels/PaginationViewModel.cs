using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_2025_Project_Template.ViewModels
{
    public abstract class PaginationViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<PageItem> Pages { get; set; } = new ObservableCollection<PageItem>();

        private int _totalPages;
        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if (_totalPages != value)
                {
                    _totalPages = value;
                    OnPropertyChanged(nameof(TotalPages));
                    UpdatePages();
                }
            }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                    UpdatePages();
                }
            }
        }
        public class PageItem
        {
            public int? Number { get; set; }   // ako je null, to je "..."
            public bool IsCurrent { get; set; }
            public bool IsEllipsis => Number == null;
            public bool IsEnabled => !IsEllipsis;
        }

        protected void UpdatePages()
        {
            Pages.Clear();

            if (TotalPages <= 5)
            {
                for (int i = 1; i <= TotalPages; i++)
                    Pages.Add(new PageItem { Number = i, IsCurrent = i == CurrentPage });
                return;
            }

            int windowSize = 3;

            if (CurrentPage <= 3)
            {
                for (int i = 1; i <= windowSize; i++)
                    Pages.Add(new PageItem { Number = i, IsCurrent = i == CurrentPage });

                Pages.Add(new PageItem { Number = null }); // "..."
                Pages.Add(new PageItem { Number = TotalPages, IsCurrent = false });
            }
            else if (CurrentPage >= TotalPages - 2)
            {
                Pages.Add(new PageItem { Number = 1, IsCurrent = false });
                Pages.Add(new PageItem { Number = null }); // "..."

                for (int i = TotalPages - windowSize + 1; i <= TotalPages; i++)
                    Pages.Add(new PageItem { Number = i, IsCurrent = i == CurrentPage });
            }
            else
            {
                Pages.Add(new PageItem { Number = 1, IsCurrent = false });
                Pages.Add(new PageItem { Number = null }); // "..."

                for (int i = CurrentPage - 1; i <= CurrentPage + 1; i++)
                    Pages.Add(new PageItem { Number = i, IsCurrent = i == CurrentPage });

                Pages.Add(new PageItem { Number = null }); // "..."
                Pages.Add(new PageItem { Number = TotalPages, IsCurrent = false });
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
