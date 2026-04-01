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
    /// Interaction logic for EditDialog.xaml
    /// </summary>
    public partial class NameDialog : UserControl
    {
        private readonly INameDialogViewModel _viewModel;
        public NameDialog(INameDialogViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
        private async void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel is INameDialogViewModel vm)
            {
                if (!vm.Validate(out string error))
                {
                    MessageBox.Show(error, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            await _viewModel.SaveAsync();
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
