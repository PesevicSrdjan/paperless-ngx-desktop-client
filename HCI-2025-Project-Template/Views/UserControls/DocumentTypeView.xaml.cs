using HCI_2025_Project_Template.Core.Interfaces;
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
    /// Interaction logic for DocumentTypeView.xaml
    /// </summary>
    public partial class DocumentTypeView : UserControl, INoInternetAware
    {
        private static DocumentTypeViewModel? _viewModel;
        private UserControl _tableView = new DocumentTypeTableView();

        // Event koji se prosljeđuje 'DashboardWindow' ukoliko je došlo do problema sa konekcijom.
        public event Action? NoInternetDetectedExternally;

        public DocumentTypeView()
        {
            InitializeComponent();
            _viewModel = new DocumentTypeViewModel(new DocumentLoaderService());
            DataContext = _viewModel;

            _viewModel.NoInternetDetected += () =>
            {
                NoInternetDetectedExternally?.Invoke();
            };

            _ = loadDataAsyncHelper();

            DocumentTypeContentControl.Content = _tableView;
        }

        private async Task loadDataAsyncHelper()
        {
            await _viewModel!.LoadPageAsync(1);
        }

        private async void CreateDocumentType_Click(object sender, RoutedEventArgs e)
        {
            _viewModel!.Mode = DocumentTypeViewModel.DocumentTypeDialogMode.Create;

            _viewModel.SelectedDocumentType = null;

            _viewModel.Name = string.Empty;


            var dialog = new NameDialog(_viewModel);
            await DialogHost.Show(dialog, "MainDialog");
        }

        public async Task TriggerRefresh()
        {
            _viewModel!.DocumentTypesList.Clear();
            await _viewModel.LoadPageAsync(1);
        }
    }
}
