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
    /// Interaction logic for DocumentContextMenu.xaml
    /// </summary>
    public partial class DocumentContextMenu : UserControl
    {
        public DocumentContextMenu()
        {
            InitializeComponent();
        }
        public Document SelectedDocument { get; set; }
        public Snackbar MainSnackbar { get; set; }
        public DialogHost RootDialog { get; set; }

        public event Action<Document> OnDocumentOpen;
        private void OpenDocument_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDocument != null)
            {
                OnDocumentOpen?.Invoke(SelectedDocument);
            }
        }
        private async void DownloadDocument_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDocument != null)
            {
                if (DataContext is DocumentsViewModel vm)
                {
                    var result = await vm.DownloadDocumentAsync(SelectedDocument);
                    if (result == true)
                        MainSnackbar?.MessageQueue?.Enqueue($"Document '{SelectedDocument.Title}' saved.");
                    else
                        MainSnackbar?.MessageQueue?.Enqueue($"Download failed for '{SelectedDocument.Title}'.");
                }
            }
        }
        private async void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDocument != null)
            {
                var detailsView = new DocumentsDetailView
                {
                    DataContext = SelectedDocument
                };

                if (RootDialog != null)
                    await DialogHost.Show(detailsView, RootDialog.Identifier);
            }
        }
    }
}
