using HCI_2025.Core.Services;
using HCI_2025.ViewModel;
using System;
using System.Collections.Generic;
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
        private readonly DocumentsViewModel _viewModel;

        private UserControl _tableView = new DocumentsTableView();
        private UserControl _cardView = new DocumentsCardView();
        public DocumentsView()
        {
            InitializeComponent();

            _viewModel = new DocumentsViewModel(new TagService(),new DocTypeService(), new DocumentService());
            this.DataContext = _viewModel;


            DocumentContentControl.Content = _tableView;


            Loaded += async (s, e) =>
            {
                await _viewModel.loadDataAsync();
            };
        }
        private void tableView_Click(object sender, RoutedEventArgs e)
        {
            DocumentContentControl.Content = _tableView;
        }
        private void cardView_Click(object sender, RoutedEventArgs e)
        { 
            DocumentContentControl.Content = _cardView;
        }
    }
}
