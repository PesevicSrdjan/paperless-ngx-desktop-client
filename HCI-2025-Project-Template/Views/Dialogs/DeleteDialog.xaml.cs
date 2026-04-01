using HCI_2025_Project_Template.Core.Interfaces;
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

namespace HCI_2025_Project_Template.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for DeleteTagDialog.xaml
    /// </summary>
    public partial class DeleteDialog : UserControl
    {
        //private readonly TagsViewModel _viewModel;

        private readonly IDeleteDialogViewModel _viewModel;
        public DeleteDialog(IDeleteDialogViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.DeleteAsync();
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
