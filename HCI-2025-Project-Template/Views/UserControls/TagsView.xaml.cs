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
    /// Interaction logic for TagsView.xaml
    /// </summary>
    public partial class TagsView : UserControl
    {
        private static TagsViewModel? _viewModel;
        private UserControl _tableView = new TagsTableView();
        public TagsView()
        {
            InitializeComponent();

            if(_viewModel == null)
            {
                _viewModel = new TagsViewModel(new DocumentLoaderService());
                _ = loadDataAsyncHelper();
            }

            this.DataContext = _viewModel;
            TagsContentControl.Content = _tableView;
        }
        private async Task loadDataAsyncHelper()
        {
            await _viewModel!.LoadPageAsync(1);
        }

        private async void CreateTag_Click(object sender, RoutedEventArgs e)
        {
            _viewModel!.Mode = TagsViewModel.TagDialogMode.Create;
            
            _viewModel.SelectedTag = null;

            _viewModel.Name = string.Empty;

            _viewModel.SelectedColor = Colors.Red;

            var dialog = new TagDialog(_viewModel);
            await DialogHost.Show(dialog, "MainDialog");
        }
    }
}
