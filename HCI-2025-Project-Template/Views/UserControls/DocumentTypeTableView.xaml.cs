using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Models.Ui;
using HCI_2025_Project_Template.Core.Services;
using HCI_2025_Project_Template.ViewModels;
using HCI_2025_Project_Template.Views.Dialogs;
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
    /// Interaction logic for DocumentTypeTableView.xaml
    /// </summary>
    public partial class DocumentTypeTableView : UserControl
    {
        private readonly IDocTypeService _documentTypeService;

        private DocumentTypeViewModel? ViewModel => DataContext as DocumentTypeViewModel;

        public DocumentTypeTableView()
        {
            InitializeComponent();
            _documentTypeService = new DocTypeService();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is DocTypeInfo documentType)
            {
                ViewModel!.SelectedDocumentType = documentType;
                var dialog = new DeleteDialog(ViewModel);
                await DialogHost.Show(dialog, "MainDialog");
            }
        }
        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is DocTypeInfo documentType)
            {
                ViewModel!.SelectedDocumentType = documentType;
                ViewModel.Mode = DocumentTypeViewModel.DocumentTypeDialogMode.Edit;

                var dialog = new NameDialog(ViewModel);
                await DialogHost.Show(dialog, "MainDialog");
            }

        }
    }
}
