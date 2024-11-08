using DTOs;
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

namespace KoiShowManagementSystemWPF.PopupDialog
{
    /// <summary>
    /// Interaction logic for KoiParticipantsWindow.xaml
    /// </summary>
    public partial class KoiParticipantsWindow : Window
    {
        public KoiParticipantsWindow(IEnumerable<RegistrationDTO> _list)
        {
            InitializeComponent();
            RegistrationGrid.ItemsSource = _list;
        }

        private void BtnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
