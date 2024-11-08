using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DTOs;
using KoiShowManagementSystemWPF.Manager;
using KoiShowManagementSystemWPF.PopupDialog;
using KoiShowManagementSystemWPF.Referee;
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

namespace KoiShowManagementSystemWPF.Member
{
    /// <summary>
    /// Interaction logic for MemberProfileWindow.xaml
    /// </summary>
    public partial class MemberProfileWindow : Window
    {
        private UserDTO _user = null!;
        private readonly IUserService _userService;
        public MemberProfileWindow(UserDTO user)
        {
            _userService = UserService.Instance;
            _user = user;
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            // Information:
            if( _user != null )
            {
                txtEmail.Text = _user.Email;
                txtName.Text = _user.Name;
                txtPhone.Text = _user.Phone;
                // Load Button:
                btnShows.Visibility = Visibility.Visible;
                btnRegistrations.Visibility = Visibility.Visible;
                if (_user.Role!.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
                {
                    btnReferees.Visibility = Visibility.Visible;
                    btnVarieties.Visibility = Visibility.Visible;
                }
                if (_user.Role!.Equals("Referee", StringComparison.OrdinalIgnoreCase) == true)
                {
                    btnScoring.Visibility = Visibility.Visible;
                }
                if (_user.Role!.Equals("Member", StringComparison.OrdinalIgnoreCase) == true)
                {
                    btnKois.Visibility = Visibility.Visible;
                }
            }
        }

        private void BtnShows(object sender, RoutedEventArgs e)
        {
            ShowManagementWindow window = new ShowManagementWindow(_user);
            window.Show();
            this.Close();
        }

        private void BtnRegistrations(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow(_user);
            registrationWindow.Show();
            this.Close();
        }

        private void BtnKois(object sender, RoutedEventArgs e)
        {
            KoiManagementWindow window = new KoiManagementWindow(_user);
            window.Show();
            this.Close();
        }

        private void BtnReferees(object sender, RoutedEventArgs e)
        {
            ManageRefereeWindow window = new ManageRefereeWindow(_user);
            window.Show();
            this.Close();
        }

        private void BtnVarieties(object sender, RoutedEventArgs e)
        {
            ManageVarietyWindow window = new ManageVarietyWindow(_user);
            window.Show();
            this.Close();
        }

        private void BTnScoring(object sender, RoutedEventArgs e)
        {
            ScoringWindow window = new ScoringWindow(_user);
            window.Show();
            this.Close();
        }


        private void BtnLogout(object sender, RoutedEventArgs e)
        {
            _user = null!;
            LoginWindow window = new LoginWindow();
            window.Show();
            this.Close();
        }

        private async void EditInformation_Button(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtPassword.Password) || string.IsNullOrEmpty(txtPhone.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!long.TryParse(txtPhone.Text, out long phoneNumber) || txtPhone.Text.Length != 10 || phoneNumber < 0)
            {
                MessageBox.Show("Please enter a valid 10-digit phone number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (txtName.Text == _user.Name && txtPhone.Text == _user.Phone)
            {
                MessageBox.Show("No changes detected. Please update the information before saving.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            var userDTO = new UserDTO
            {
                Id = _user.Id,
                Name = txtName.Text,
                Phone = txtPhone.Text,
                Password = _user.Password,
                Email = _user.Email,
                Status = true
            };

            bool updateResult = await _userService.UpdateUser(userDTO);
            if (updateResult)
            {
                MessageBox.Show("User updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("User update failed. User might not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ChangePassword_Button(object sender, RoutedEventArgs e)
        {
            ChangePasswordWindow window = new ChangePasswordWindow(_user);
            window.ShowDialog();
            //this.Close(); ko được Close vì đây là show dialog
        }
    }
}
