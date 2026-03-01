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
    /// Interaction logic for AccentColorDialog.xaml
    /// </summary>
    public partial class AccentColorDialog : UserControl
    {
        public Color SelectedColor { get; set; } = Colors.Blue;
        public AccentColorDialog()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(ColorPickerControl.Color, this);
        }
    }
}
