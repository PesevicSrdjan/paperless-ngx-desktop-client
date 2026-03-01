using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HCI_2025_Project_Template.Core.Models.Ui
{
    public class FileMetadata : INotifyPropertyChanged
    {
        public string FilePath { get; set; }
        public string DocumentName { get; set; }
        public CorrespondentsInfo Correspondent { get; set; }
        public DocTypeInfo DocumentType { get; set; }
        public DateTime? DocumentDate { get; set; }
        public ObservableCollection<TagInfo> SelectedTags { get; } = new ObservableCollection<TagInfo>();

        private SolidColorBrush _progressColor;
        public SolidColorBrush ProgressColor
        {
            get => _progressColor;
            set
            {
                if (_progressColor != value)
                {
                    _progressColor = value;
                    OnPropertyChanged(nameof(ProgressColor));
                }
            }
        }
        public enum FileStage
        {
            Waiting,
            ReadyToUpload,
            Uploading,
            Uploaded,
            Failed, 
            FailedInPreparing
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }
       
        private double _uploadProgress;
        public double UploadProgress
        {
            get => _uploadProgress;
            set
            {
                if (_uploadProgress != value)
                {
                    _uploadProgress = value;
                    OnPropertyChanged(nameof(UploadProgress));
                    UpdateUploadStatus();
                }
            }
        }
        private string _uploadStatus = "";

        private FileStage _stage = FileStage.Waiting;
        public FileStage Stage
        {
            get => _stage;
            set
            {
                _stage = value;
                OnPropertyChanged(nameof(Stage));
                UpdateUploadStatus();
            }
        }

        private SolidColorBrush _progressBackgroundColor = Brushes.LightGray;
        public SolidColorBrush ProgressBackgroundColor
        {
            get => _progressBackgroundColor;
            set
            {
                if (_progressBackgroundColor != value)
                {
                    _progressBackgroundColor = value;
                    OnPropertyChanged(nameof(ProgressBackgroundColor));
                }
            }
        }
        public string UploadStatus
        {
            get => _uploadStatus;
            set
            {
                if (_uploadStatus != value)
                {
                    _uploadStatus = value;
                    OnPropertyChanged(nameof(UploadStatus));
                }
            }
        }
        public string FileIconPath
        {
            get
            {
                var ext = System.IO.Path.GetExtension(FilePath)?.ToLower();
                return ext switch
                {
                    ".pdf" => "/Assets/FileIcons/pdf.png",
                    ".doc" => "/Assets/FileIcons/doc.png",
                    ".docx" => "/Assets/FileIcons/doc.png",
                    ".xls" => "/Assets/FileIcons/xls.png",
                    ".xlsx" => "/Assets/FileIcons/xls.png",
                    ".png" => "/Assets/FileIcons/image.png",
                    ".jpg" => "/Assets/FileIcons/image.png",
                    ".jpeg" => "/Assets/FileIcons/image.png",
                    _ => "/Assets/FileIcons/file.png" // default ikona
                };
            }
        }

        private void UpdateUploadStatus()
        {

            Debug.WriteLine($"UpdateUploadStatus called for {DocumentName}, Stage: {Stage}");
            switch (Stage)
            {

                case FileStage.Waiting:
                    UploadStatus = "Waiting...";
                    ProgressColor = Brushes.LightGreen;
                    break;

                case FileStage.ReadyToUpload:
                    UploadStatus = "Ready to upload";
                    ProgressColor = Brushes.LightGreen;
                    break;

                case FileStage.Uploading:
                    ProgressColor = Brushes.Blue;
                    UploadStatus = $"Uploading... {(int)UploadProgress}%";
                    break;

                case FileStage.Uploaded:
                    UploadStatus = "Upload successful";
                    ProgressColor = Brushes.LightBlue;
                    break;

                case FileStage.FailedInPreparing:
                    UploadStatus = "Failed";
                    ProgressColor = Brushes.Red;
                    ProgressBackgroundColor = Brushes.LightCoral;
                    break;

                case FileStage.Failed:
                    UploadStatus = "Upload failed";
                    ProgressColor = Brushes.Red;
                    ProgressBackgroundColor = Brushes.LightCoral;
                    break;
                default:
                    ProgressBackgroundColor = Brushes.LightGray;
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
