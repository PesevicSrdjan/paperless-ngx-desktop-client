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
    /// Interaction logic for NoInternetView.xaml
    /// </summary>
    public partial class NoInternetView : UserControl
    {
        // Event koji se pokreće kada korisnik klikne na dugme "Retry", kako bi se ponovo pokušala uspostaviti internet konekcija.
        public event Action? OnRetry;

        public NoInternetView()
        {
            InitializeComponent();
        }

        private void Retry_Click(object sender, RoutedEventArgs e)
        {
            OnRetry?.Invoke();
        }
    }
}
