using HCI_2025_Project_Template.Core.Models.Ui;
using HCI_2025_Project_Template.ViewModels;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for DocumentsCardView.xaml
    /// </summary>
    public partial class DocumentsCardView : UserControl
    {
        public DocumentsCardView()
        {
            InitializeComponent();
        }

        public event Action<Document> OnDocumentDoubleClicked;

        private void DocumentsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListBox)?.SelectedItem is Document doc)
            {
                OnDocumentDoubleClicked?.Invoke(doc);
            }
        }
        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (documentsListBox.SelectedItem is Document selectedDocument)
            {
                OnDocumentDoubleClicked?.Invoke(selectedDocument);
            }
        }
        private async void DownloadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (documentsListBox.SelectedItem is Document selectedDocument)
            {
                var vm = DataContext as DocumentsViewModel;
                if (vm == null) return;

                var result = await vm.DownloadDocumentAsync(selectedDocument);

                if (result == true)
                {
                    MainSnackbar.MessageQueue?.Enqueue($"Document '{selectedDocument.Title}' saved.");
                }
                else if (result == false)
                {
                    MainSnackbar.MessageQueue?.Enqueue($"Download failed for '{selectedDocument.Title}'.");

                }
            }
        }
        private async void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (documentsListBox.SelectedItem is Document selectedDocument)
            {
                var detailsView = new DocumentsDetailView
                {
                    DataContext = selectedDocument
                };

                await DialogHost.Show(detailsView, "RootDialog");
            }
        }
    }
}
