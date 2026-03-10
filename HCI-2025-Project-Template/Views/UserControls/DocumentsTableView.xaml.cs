using HCI_2025_Project_Template.ViewModels;
using HCI_2025_Project_Template.Core.Models.Ui;
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
    /// Interaction logic for DocumentsTableView.xaml
    /// </summary>
    public partial class DocumentsTableView : UserControl
    {
        public DocumentsTableView()
        {
            InitializeComponent();
        }

        public event Action<Document> OnDocumentDoubleClicked;

        private void DocumentsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (documentsGrid.SelectedItem is Document selectedDocument)
            {
                OnDocumentDoubleClicked?.Invoke(selectedDocument);
            }
        }
    }
}
