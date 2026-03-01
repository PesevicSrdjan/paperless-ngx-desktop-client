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
    /// Interaction logic for TagDialog.xaml
    /// </summary>
    public partial class TagDialog : UserControl
    {
        public TagDialog(TagsViewModel _viewModel)
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
        private async void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not TagsViewModel vm) return;

            if (vm.Mode == TagsViewModel.TagDialogMode.Edit)
                await vm.EditTagAsync();
            else
                await vm.CreateTagAsync();

            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
