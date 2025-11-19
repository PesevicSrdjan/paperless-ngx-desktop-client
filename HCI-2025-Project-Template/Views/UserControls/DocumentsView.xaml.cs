using HCI_2025.ViewModel;
using HCI_2025_Project_Template.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
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
    /// Interaction logic for DocumentsView.xaml
    /// </summary>
    public partial class DocumentsView : UserControl
    {
        private static DocumentsViewModel _viewModel;

        private UserControl _tableView = new DocumentsTableView();
        private UserControl _cardView = new DocumentsCardView();
        public DocumentsView()
        {
            InitializeComponent();

            if (_viewModel == null)
            {
                _viewModel = new DocumentsViewModel(new TagService(),new DocTypeService(), new DocumentService());

                _ = loadDataAsyncHelper();
            }

            this.DataContext = _viewModel;
            DocumentContentControl.Content = _tableView;
        }

        private async Task loadDataAsyncHelper()
        {
            await _viewModel.LoadInitialAsync();
        }

        private void tableView_Click(object sender, RoutedEventArgs e)
        {
            DocumentContentControl.Content = _tableView;
        }
        private void cardView_Click(object sender, RoutedEventArgs e)
        { 
            DocumentContentControl.Content = _cardView;
        }

        private async void NextPage_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.NextPageAsync();
        }

        private async void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.PreviousPageAsync();
        }
    }
}
