using HCI_2025.Core.Models;
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
                var prevTextBlock = _selectedSidebarButton.Template.FindName("ButtonText", _selectedSidebarButton) as TextBlock;
                if (prevTextBlock != null)
                {
                    prevTextBlock.FontWeight = FontWeights.Normal;
                    prevTextBlock.Foreground = Brushes.Black;
                }
            }
        }
        private void setNewSelectedButton(Button? newButton)
        {
            if (newButton == null) return;

            _selectedSidebarButton = newButton;
            var currTextBlock = _selectedSidebarButton.Template.FindName("ButtonText", _selectedSidebarButton) as TextBlock;
            if (currTextBlock != null)
            {
                currTextBlock.FontWeight = FontWeights.Bold;

                var brush = new BrushConverter().ConvertFromString("#17541F") as Brush;

                if (brush != null)
                {
                    currTextBlock.Foreground = brush;

                }
            }
        }

        private void SidebarButton_Click(object sender, RoutedEventArgs e)
        {
            resetPrevSelectedButton();
            setNewSelectedButton(sender as Button);
        }
    }
}
