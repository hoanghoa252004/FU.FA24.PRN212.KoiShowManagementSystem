using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DataAccessLayer.Implementation;
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
    /// Interaction logic for AdminUpdateRegistrationDialog.xaml
    /// </summary>
    public partial class AdminUpdateRegistrationDialog : Window
    {
        private readonly IRegistrationService _service;
        private readonly RegistrationDTO _registrationDTO;
        private readonly Action RefreshWindow;
        public AdminUpdateRegistrationDialog(RegistrationDTO registrationDTO, Action RefreshWindow)
        {
            _service = RegistrationService.Instance;
            _registrationDTO = registrationDTO;
            this.RefreshWindow = RefreshWindow;
            InitializeComponent();
        }

        private void BtnReset(object sender, RoutedEventArgs e)
        {
            txtNote.Clear();
            txtRejected.IsChecked = true;
        }

        private void BtnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnSubmit(object sender, RoutedEventArgs e)
        {
            if(txtRejected.IsChecked == true)
            {
                _registrationDTO.Note = txtNote.Text;
                _registrationDTO.Status = "Rejected";
                await _service.Update(_registrationDTO);
            }
            else
            {
                _registrationDTO.Status = "Accepted";
                await _service.Update(_registrationDTO);
            }
            RefreshWindow();
            this.Close();
        }
    }
}
