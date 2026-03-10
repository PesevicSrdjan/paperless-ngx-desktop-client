using HCI_2025_Project_Template.Helpers;
using HCI_2025_Project_Template.Views.UserControls;
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
using System.Windows.Shapes;

namespace HCI_2025_Project_Template.Views
{
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {
        private Button _selectedSidebarButton;
        public DashboardWindow()
        {
            InitializeComponent();

            MainContentControl.Content = new DashboardView();
            _selectedSidebarButton = button_dashboard;

            Loaded += dashboardWindow_Loaded;
        }

        private void dashboardWindow_Loaded(object sender, RoutedEventArgs e)
        {
            setNewSelectedButton(_selectedSidebarButton);
        }
        private void button_dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new DashboardView();
        }
        private void button_documents_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new DocumentsView();
        }

        private void resetPrevSelectedButton()
        {
            if (_selectedSidebarButton != null)
            {
                _selectedSidebarButton.FontWeight = FontWeights.Normal;
                _selectedSidebarButton.ClearValue(Button.ForegroundProperty);
            }
        }

        private void setNewSelectedButton(Button? newButton)
        {
            if (newButton == null) return;

            _selectedSidebarButton = newButton;

            _selectedSidebarButton.FontWeight = FontWeights.Bold;

            _selectedSidebarButton.SetResourceReference(
                Button.ForegroundProperty,
                "AccentColor");
        }
        private void SidebarButton_Click(object sender, RoutedEventArgs e)
        {
            resetPrevSelectedButton();
            setNewSelectedButton(sender as Button);
        }
        private void button_tags_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new TagsView();
        }
        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        { 
             MainContentControl.Content = new SettingsView();
        }
        public void RefreshSidebarButtonColors()
        {
            if (_selectedSidebarButton == null)
                return;

            _selectedSidebarButton.ClearValue(Button.ForegroundProperty);

            _selectedSidebarButton.Foreground =
                (Brush)Application.Current.Resources["AccentColor"];

            _selectedSidebarButton.FontWeight = FontWeights.Bold;
        }
    }
}
