using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
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

namespace KoiShowManagementSystemWPF.Manager
{
    /// <summary>
    /// Interaction logic for FinalResultWindow.xaml
    /// </summary>
    public partial class FinalResultWindow : Window
    {
        private readonly IEnumerable<RegistrationDTO> _result = null!;
        private readonly ShowDTO _show = null!;
        private readonly IShowService _service;
        public FinalResultWindow(ShowDTO show, IEnumerable<RegistrationDTO> result)
        {
            _result = result;
            _show = show;
            InitializeComponent();
            RegistrationGrid.ItemsSource = _result;
            _service = ShowService.Instance;
            if(_show.Status.Equals("Finished", StringComparison.OrdinalIgnoreCase) == true)
            {
                btnAnnouceScore.Visibility = Visibility.Collapsed;
            }
            
        }

        private void BtnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnAnnouceScore(object sender, RoutedEventArgs e)
        {
            try
            {
                bool result = await _service.AnnouceResult(_show.Id);
                if(result == true)
                {
                    MessageBox.Show("Annouce Score Successfully .", "Successfully:", MessageBoxButton.OK);
                    btnAnnouceScore.Visibility = Visibility.Collapsed;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Annouce Score Failed .", "Failed:", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
