using HCI_2025_Project_Template.Views.Windows;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using HCI_2025_Project_Template.Resources;
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
using System.Diagnostics;
using HCI_2025_Project_Template.ViewModels;
using HCI_2025_Project_Template.Core.Interfaces;
using HCI_2025_Project_Template.Core.Services;

namespace HCI_2025_Project_Template.Views.UserControls
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        private readonly StatisticsViewModel _vm;
        private readonly IStatisticsService statisticsService = new StatisticsService();
        public DashboardView()
        {
            InitializeComponent();
            _vm = new StatisticsViewModel(statisticsService);

            this.DataContext = _vm;

            this.Loaded += async (s, e) => await _vm.LoadData();

            _vm.StartInternetCheck();

            SetupPaperlessDescription();
            this.Loaded += (s, e) => MainGrid.Focus();
        }
        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true; 

            openFileDialog.Filter = "All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                HandleUploadedFiles(openFileDialog.FileNames);
            }
        }
        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                HandleUploadedFiles(files);
            }
        }
        private void HandleUploadedFiles(string[] files)
        {
            if (files == null || files.Length == 0)
                return;

            MetaDataWindow metadataWindow = new MetaDataWindow(files);
            metadataWindow.Show();
        }
        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
        }

        private void UploadShortcut(object sender, ExecutedRoutedEventArgs e)
        {
            UploadButton_Click(null, null);
        }
        public void TriggerUpload()
        {
            UploadButton_Click(null, null);
        }
        private void SetupPaperlessDescription()
        {
            string fullText = Strings.ResourceManager.GetString("PaperlessDescription")!;
            string linkWord = Strings.ResourceManager.GetString("PaperlessLinkWord")!;

            PaperlessTextBlock.Inlines.Clear();

            int index = fullText.IndexOf(linkWord);
            if (index >= 0)
            {
                PaperlessTextBlock.Inlines.Add(new Run(fullText.Substring(0, index)));


                Hyperlink link = new Hyperlink(new Run(fullText.Substring(index, linkWord.Length)))
                {
                    NavigateUri = new Uri("https://docs.paperless-ngx.com/")
                };
                link.RequestNavigate += Hyperlink_RequestNavigate;
                PaperlessTextBlock.Inlines.Add(link);

                PaperlessTextBlock.Inlines.Add(new Run(fullText.Substring(index + linkWord.Length)));
            }
            else
            {
                PaperlessTextBlock.Inlines.Add(new Run(fullText));
            }
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
