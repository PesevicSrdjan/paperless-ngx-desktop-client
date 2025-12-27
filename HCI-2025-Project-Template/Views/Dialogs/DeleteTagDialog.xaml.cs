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
    public partial class DeleteTagDialog : UserControl
    {
        private readonly TagsViewModel _viewModel;
        public DeleteTagDialog(TagsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.DeleteTagAsync();
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
