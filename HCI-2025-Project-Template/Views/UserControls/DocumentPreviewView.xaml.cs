using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Api;
using HCI_2025_Project_Template.Core.Models.Ui;
using HCI_2025_Project_Template.Core.Services;
using HCI_2025_Project_Template.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HCI_2025_Project_Template.Views.UserControls
{
    /// <summary>
    /// Interaction logic for DocumentPreviewView.xaml
    /// </summary>
    public partial class DocumentPreviewView : UserControl
    {
        public DocumentPreviewViewModel ViewModel { get; }
        private Document _document;
        public DocumentPreviewView(DocumentLoaderService loader)
        {
            InitializeComponent();
            ViewModel = new DocumentPreviewViewModel(loader);
            this.DataContext = ViewModel;
        }

        public event Action OnClose;
        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnClose?.Invoke();
        }
        private void TagComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox &&
                comboBox.SelectedItem is TagInfo tag &&
                DataContext is DocumentPreviewViewModel vm)
            {
                if (!vm.SelectedTags.Any(t => t.Id == tag.Id))
                {
                    vm.SelectedTags.Add(tag);
                }

                comboBox.SelectedItem = null;

            }
        }
        private void RemoveTag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ItemsControl ic && btn.DataContext is TagInfo tag)
            {
                if (ic.DataContext is DocumentPreviewViewModel vm)
                {
                    vm.SelectedTags.Remove(tag);
                }
            }
        }

        public event Action<DocumentUpdateRequest> OnSaved;
        public async Task LoadDocumentAsync(Document document)
        {
            _document = document;

            await ViewModel.InitializeAsync();

            ViewModel.LoadDocumentMetadata(document);

            ViewModel.MimeType = document.MimeType.ToString();

            await ViewModel.LoadPreviewAsync(document.Id);

            var mimeType = document.MimeType.ToString().ToLower();

            if (mimeType.ToLower() == "text/plain")
            {
                PreviewText.Text = System.Text.Encoding.UTF8.GetString(ViewModel.PreviewBytes ?? Array.Empty<byte>());
                PreviewText.Visibility = Visibility.Visible;
                PreviewBrowser.Visibility = Visibility.Collapsed;
            }
            else
            {
                var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
                await System.IO.File.WriteAllBytesAsync(tempPath, ViewModel.PreviewBytes ?? Array.Empty<byte>());

                PreviewBrowser.Source = new Uri(tempPath);
                PreviewBrowser.Visibility = Visibility.Visible;
                PreviewText.Visibility = Visibility.Collapsed;
            }
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var request = ViewModel.CreateUpdateRequest();

            bool success = await ViewModel.DocumentService.UpdateDocMetadataAsync(ViewModel.DocumentId, request);

            if (success)
            {
                OnSaved?.Invoke(request);

                ViewModel.IsEdited = false;

                MessageBox.Show("Document updated successfully.");
            }
            else
            {
                MessageBox.Show("Failed to update document.");
            }
                
        }
        private async void SaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            var request = ViewModel.CreateUpdateRequest();

            bool success = await ViewModel.DocumentService.UpdateDocMetadataAsync(ViewModel.DocumentId, request);

            if (success)
            {
                OnSaved?.Invoke(request);

                ViewModel.IsEdited = false;

                OnClose?.Invoke();
            }
            else
            {
                MessageBox.Show("Failed to update document.");
            }
        }
        private void DiscardButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadDocumentMetadata(_document);

            ViewModel.IsEdited = false;
        }
    }
}
