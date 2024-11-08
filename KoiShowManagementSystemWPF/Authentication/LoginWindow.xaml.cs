using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using KoiShowManagementSystemWPF.Manager;
using KoiShowManagementSystemWPF.Member;
using Microsoft.IdentityModel.Tokens;
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

namespace KoiShowManagementSystemWPF.Authentication
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IAuthenticationService _service;
        public LoginWindow()
        {
            _service = AuthenticationService.Instance;
            InitializeComponent();
        }

        private async void BtnSubmit(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtEmail == null || txtEmail.Text.IsNullOrEmpty() == true
                    || txtPassword == null || txtPassword.Text.IsNullOrEmpty() == true)
                {
                    throw new Exception("Please enter EMAIL & PASSWORD !");
                }
                else
                {
                    var user = await _service.Login(txtEmail.Text, txtPassword.Text);
                    ProfileWindow window = new ProfileWindow(user);
                    window.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        }

        private void SignUp_Button(object sender, RoutedEventArgs e)
        {
            SignupWindow window = new SignupWindow();
            window.Show();
            this.Close();
        }
    }
}
