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
using HCI_2025_Project_Template.Views.Dialogs;
using HCI_2025_Project_Template.Core.Models.Ui;
using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Services;
using HCI_2025_Project_Template.ViewModels;
using System.Diagnostics;

namespace HCI_2025_Project_Template.Views.UserControls
{
    /// <summary>
    /// Interaction logic for TagsTableView.xaml
    /// </summary>
    public partial class TagsTableView : UserControl
    {

        private readonly ITagService _tagService;

        private TagsViewModel? ViewModel => DataContext as TagsViewModel;
        public TagsTableView()
        {
            InitializeComponent();
            _tagService = new TagService();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is TagInfo tag)
            {
                ViewModel!.SelectedTag = tag;
                var dialog = new DeleteDialog(ViewModel);
                await DialogHost.Show(dialog, "MainDialog");
            }
        }
        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is TagInfo tag)
            {
                ViewModel!.SelectedTag = tag;
                ViewModel.Mode = TagsViewModel.TagDialogMode.Edit;


                var dialog = new TagDialog(ViewModel);
                await DialogHost.Show(dialog, "MainDialog");
            }
        }
    }
}
