using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Ui;
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

namespace HCI_2025_Project_Template.ViewModels
{
    public class StatisticsViewModel : INotifyPropertyChanged
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsViewModel(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private bool _noInternet;
        public bool NoInternet
        {
            get => _noInternet;
            set
            {
                _noInternet = value;
                OnPropertyChanged();
            }
        }

        public async void StartInternetCheck()
        {
            while (true)
            {
                NoInternet = !await NetworkHelper.HasInternetCachedAsync();
                await Task.Delay(5000);
            }
        }

        private int _documentsTotal;
        public int DocumentsTotal
        {
            get => _documentsTotal;
            set { _documentsTotal = value; OnPropertyChanged(); }
        }

        private int _documentsInbox;
        public int DocumentsInbox
        {
            get => _documentsInbox;
            set { _documentsInbox = value; OnPropertyChanged(); }
        }

        private long _characterCount;
        public long CharacterCount
        {
            get => _characterCount;
            set { _characterCount = value; OnPropertyChanged(); }
        }

        private int _currentAsn;
        public int CurrentAsn
        {
            get => _currentAsn;
            set { _currentAsn = value; OnPropertyChanged(); }
        }

        private int _tagCount;
        public int TagCount
        {
            get => _tagCount;
            set { _tagCount = value; OnPropertyChanged(); }
        }

        private int _correspondentCount;
        public int CorrespondentCount
        {
            get => _correspondentCount;
            set { _correspondentCount = value; OnPropertyChanged(); }
        }

        private int _documentTypeCount;
        public int DocumentTypeCount
        {
            get => _documentTypeCount;
            set { _documentTypeCount = value; OnPropertyChanged(); }
        }

        public ObservableCollection<FileTypeItem> FileTypes { get; set; } = new();


        public async Task LoadData()
        {
            var data = await _statisticsService.GetStatisticsAsync();

            if (data == null)
            {
                Debug.WriteLine("Statistics data is null!");
                return;
            }

            DocumentsTotal = data.DocumentsTotal;
            DocumentsInbox = data.DocumentsInbox;
            CharacterCount = data.CharacterCount;
            CurrentAsn = data.CurrentAsn;

            TagCount = data.TagCount;
            CorrespondentCount = data.CorrespondentCount;
            DocumentTypeCount = data.DocumentTypeCount;

            FileTypes.Clear();

            var grouped = data.DocumentFileTypeCounts
                .GroupBy(f => MapMime(f.MimeType))
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Sum(x => x.MimeTypeCount)
                });

            foreach (var g in grouped)
            {
                FileTypes.Add(new FileTypeItem
                {
                    Name = g.Name,
                    Count = g.Count,
                    Percentage = (double)g.Count / data.DocumentsTotal * 100
                });
            }
        }

        private string MapMime(string mime)
        {
            return mime switch
            {
                "application/pdf" => "PDF",
                "text/plain" => "TXT",
                "image/jpeg" => "JPEG",
                "image/png" => "PNG",
                "application/msword" => "DOC",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => "DOCX",
                _ => "Other"
            };
        }
    }
}
