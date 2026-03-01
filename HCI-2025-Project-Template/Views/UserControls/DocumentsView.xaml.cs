using HCI_2025_Project_Template.Core.Models.Ui;
using HCI_2025_Project_Template.Core.Services;
using HCI_2025_Project_Template.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            _viewModel = new DocumentsViewModel(new DocumentLoaderService());

            _ = loadDataAsyncHelper();

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

        private void CheckBox_Checked_Tag(object sender, RoutedEventArgs e)
        {
           var vm = DataContext as DocumentsViewModel;
            OnFilterToggle((CheckBox)sender, vm.SelectedTags, vm.ActiveFilters);
        }
        private void CheckBox_Unchecked_Tag(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DocumentsViewModel;
            OnFilterToggle((CheckBox)sender, vm.SelectedTags, vm.ActiveFilters);
        }
        private void CheckBox_Checked_Type(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DocumentsViewModel;
            OnFilterToggle((CheckBox)sender, vm.SelectedTypes, vm.ActiveFilters);
        }
        private void CheckBox_Unchecked_Type(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DocumentsViewModel;
            OnFilterToggle((CheckBox)sender, vm.SelectedTypes, vm.ActiveFilters);
        }
        private void CheckBox_Checked_Corr(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DocumentsViewModel;
            OnFilterToggle((CheckBox)sender, vm.SelectedCorrespondents, vm.ActiveFilters);
        }

        private void CheckBox_Unchecked_Corr(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DocumentsViewModel;
            OnFilterToggle((CheckBox)sender, vm.SelectedCorrespondents, vm.ActiveFilters);
        }
        private void OnFilterToggle<T>(CheckBox cb, ObservableCollection<T> selectedList, ObservableCollection<object> activeFilters)
        {
            bool isChecked = cb.IsChecked ?? false;
            T item = (T)cb.DataContext;

            if (isChecked)
            {
                if (!selectedList.Contains(item))
                    selectedList.Add(item);

                if (!activeFilters.Contains(item))
                    activeFilters.Add(item);
            }
            else
            {
                selectedList.Remove(item);
                activeFilters.Remove(item);
            }
        }
        private void Item_Click(object sender, MouseButtonEventArgs e)
        {

            var cb = FindVisualChild<CheckBox>((Border)sender); 
            cb.IsChecked = !cb.IsChecked; 
            e.Handled = true;
        }
        private async void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DocumentsViewModel;
            if (vm == null) return;

            ComboBoxTags.SelectedItem = null;
            ComboBoxTypes.SelectedItem = null;
            ComboBoxCorrespondents.SelectedItem = null;

            ResetComboBoxCheckBoxes(ComboBoxTags);
            ResetComboBoxCheckBoxes(ComboBoxTypes);
            ResetComboBoxCheckBoxes(ComboBoxCorrespondents);

            await vm.ClearFiltersAsync(clearAll: true);
        }
        private void ResetComboBoxCheckBoxes(ComboBox comboBox)
        {
            comboBox.SelectedItem = null;

            foreach (var item in comboBox.Items)
            {
                if (comboBox.ItemContainerGenerator.ContainerFromItem(item) is ComboBoxItem cbi)
                {
                    if (FindVisualChild<CheckBox>(cbi) is CheckBox cb)
                        cb.IsChecked = false;
                }
            }
        }
        private void ComboBoxTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxTags.SelectedItem = null;
        }
        private void ComboBoxTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxTypes.SelectedItem = null;
        }
        private void ComboBoxCorrespondents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxCorrespondents.SelectedItem = null;
        }
        private void textBox_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && string.IsNullOrEmpty(tb.Text))
            {
                _viewModel?.ClearFiltersAsync(clearAll: false);
            }
        }
        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T t)
                    return t;
                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }
    }
}
