using HCI_2025_Project_Template.Core.Models.Ui;
using HCI_2025_Project_Template.Helpers;
using HCI_2025_Project_Template.ViewModels;
using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static HCI_2025_Project_Template.Core.Models.Ui.FileMetadata;

namespace HCI_2025_Project_Template.Views.Windows
{
    /// <summary>
    /// Interaction logic for MetaDataWindow.xaml
    /// </summary>
    public partial class MetaDataWindow : Window
    {
        public MetaDataViewModel ViewModel { get; set; }
        public MetaDataWindow(string[] filePaths)
        {
            InitializeComponent();

            ViewModel = new MetaDataViewModel(filePaths);
            DataContext = ViewModel;

            Loaded += MetaDataWindow_Loaded;
        }

        private async void MetaDataWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync();
        }

        private void TagComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cb && cb.SelectedItem is TagInfo tag)
            {
                var vm = (FileMetadata)cb.DataContext;

                if (!vm.SelectedTags.Contains(tag))
                    vm.SelectedTags.Add(tag);

                cb.SelectedItem = null;
            }
        }

        private void RemoveTag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ItemsControl ic && btn.DataContext is TagInfo tag)
            {
                if (ic.DataContext is FileMetadata vm)
                {
                    vm.SelectedTags.Remove(tag);
                }
            }
        }

        private void RemoveFile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is FileMetadata file)
            {
                if (file.Stage == FileStage.Uploading || file.Stage == FileStage.Uploaded)
                    return;

                ViewModel.FilesMetadata.Remove(file);
            }

            if (!ViewModel.FilesMetadata.Any())
            {
                SetEditingMode();
            }
        }
        private void SetEditingMode()
        {
            AddMoreFilesButton.IsEnabled = true;
            SubmitButton.IsEnabled = false;
            UploadButton.IsEnabled = false;

            UploadFilesPanel.Visibility = Visibility.Collapsed;
            BorderUploadPanel.Visibility = Visibility.Collapsed;
            foreach (var file in ViewModel.FilesMetadata)
            {
                file.IsEnabled = true;
            }
        }
        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {

            if (!ViewModel.AreAllFilesValid(out var invalidFiles))
            {
                
                MessageBox.Show(
                    LocalizationManager.Strings["MissingFields"],
                    LocalizationManager.Strings["MissingReqFields"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return;
            }

            foreach (var file in ViewModel.FilesMetadata)
            {
                file.IsEnabled = false;
                
            }

            ((Button)sender).IsEnabled = false;
            AddMoreFilesButton.IsEnabled = false;
            UploadButton.IsEnabled = false;

            UploadFilesPanel.ItemsSource = ViewModel.FilesMetadata;
            BorderUploadPanel.Visibility = Visibility.Visible;
            UploadFilesPanel.Visibility = Visibility.Visible;

            await ViewModel.PrepareFilesAsync();


            var readyFiles = ViewModel.FilesMetadata.Where(f => f.Stage == FileStage.ReadyToUpload).ToList();
            var failedFiles = ViewModel.FilesMetadata.Where(f => f.Stage == FileStage.Failed).ToList();
            var missingFiles = ViewModel.FilesMetadata.Where(f => f.Stage == FileStage.FailedInPreparing).ToList();

            bool hasReadyFile = readyFiles.Any();
            bool hasFailedFile = failedFiles.Any();
            bool hasMissingFile = missingFiles.Any();

            if (hasReadyFile || hasFailedFile)
            {
                UploadButton.IsEnabled = true;

                if (hasMissingFile)
                {
                    string missingNames = string.Join(", ", missingFiles.Select(f => f.DocumentName));

                    MessageBox.Show(
                        string.Format(LocalizationManager.Strings["FilesMissingWarning"], missingNames),
                        LocalizationManager.Strings["FilesMissingTitle"],
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
                else if (hasFailedFile && !hasReadyFile)
                {
                    MessageBox.Show(
                        LocalizationManager.Strings["AllFilesFailedMessage"],
                        LocalizationManager.Strings["AllFilesFailedTitle"],
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
            }
            else if (hasMissingFile)
            {
                string missingNames = string.Join(", ", missingFiles.Select(f => f.DocumentName));
                MessageBox.Show(
                        string.Format(LocalizationManager.Strings["AllFilesMissingMessage"], missingNames),
                        LocalizationManager.Strings["AllFilesMissingTitle"],
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );


                foreach (var file in missingFiles.ToList())
                {
                    ViewModel.FilesMetadata.Remove(file);
                }

                if (!ViewModel.FilesMetadata.Any())
                {
                    BorderUploadPanel.Visibility = Visibility.Collapsed;
                    UploadFilesPanel.Visibility = Visibility.Collapsed;
                    AddMoreFilesButton.IsEnabled = true;
                    SubmitButton.IsEnabled = false;
                }
                else
                {
                    UploadButton.IsEnabled = true;
                }
            }
            else
            {
                BorderUploadPanel.Visibility = Visibility.Collapsed;
                UploadFilesPanel.Visibility = Visibility.Collapsed;
                AddMoreFilesButton.IsEnabled = true;
                SubmitButton.IsEnabled = false;
            }
            //UploadButton.IsEnabled = true;
        }
        
        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            UploadButton.IsEnabled = false;

            var filesToUpload = new List<FileMetadata>();
            foreach (var file in ViewModel.FilesMetadata)
            {
                if (file.Stage == FileStage.ReadyToUpload || file.Stage == FileStage.Failed)
                {
                    file.Stage = FileStage.Uploading;
                    file.UploadProgress = 0;
                    filesToUpload.Add(file);
                }
            }

            if (!filesToUpload.Any())
            {
                MessageBox.Show(
                    LocalizationManager.Strings["NoDocToUpload"],
                    LocalizationManager.Strings["NoUpload"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return;
            }

            bool success = await ViewModel.UploadAllAsync(filesToUpload);

            if (success)
            {
                MessageBox.Show
                (
                    LocalizationManager.Strings["UploadSuccess"],
                    LocalizationManager.Strings["UploadCompleted"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            else
            {
                MessageBox.Show
                (
                    LocalizationManager.Strings["UploadUnsuccess"],
                    LocalizationManager.Strings["UploadFailed"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            SetEditingMode();
            ViewModel.FilesMetadata.Clear();
        }

        private void AddMoreFilesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsEnabled)
                return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                var files = openFileDialog.FileNames;

                foreach (var file in files)
                {
                    if (!ViewModel.FilesMetadata.Any(f => f.FilePath == file))
                    {
                        ViewModel.FilesMetadata.Add(new FileMetadata
                        {
                            FilePath = file,
                            DocumentName = System.IO.Path.GetFileNameWithoutExtension(file),
                            DocumentDate = null,
                            UploadProgress = 0
                        });
                    }
                }
            }
            SubmitButton.IsEnabled = true;
        }
    }
}
