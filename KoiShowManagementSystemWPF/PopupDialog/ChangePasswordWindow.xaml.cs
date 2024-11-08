using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DTOs;
using KoiShowManagementSystemWPF.Member;
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
using System.Xml.Linq;

namespace KoiShowManagementSystemWPF.PopupDialog
{
    /// <summary>
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private UserDTO _user = null!;
        private readonly IUserService _userService;
        public ChangePasswordWindow(UserDTO user)
        {
            _userService = UserService.Instance;
            _user = user;
            InitializeComponent();
        }

        private async void Update_Button(object sender, RoutedEventArgs e)
        {
            // Kiểm tra nếu các trường đầu vào không để trống
            if (string.IsNullOrEmpty(txtCurrentPass.Text) || string.IsNullOrEmpty(txtNewPass.Text) || string.IsNullOrEmpty(txtConfirmPass.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kiểm tra nếu mật khẩu hiện tại đúng
            if (txtCurrentPass.Text != _user.Password)
            {
                MessageBox.Show("Current password is incorrect.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kiểm tra nếu mật khẩu mới khác với mật khẩu hiện tại
            if (txtNewPass.Text == _user.Password)
            {
                MessageBox.Show("New password cannot be the same as the current password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kiểm tra nếu mật khẩu mới và mật khẩu xác nhận khớp nhau
            if (txtNewPass.Text != txtConfirmPass.Text)
            {
                MessageBox.Show("New password and confirmation password do not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
         
            var userDTO = new UserDTO
            {
                Id = _user.Id,
                Name = _user.Name,
                Phone = _user.Phone,
                Email = _user.Email,
                Status = true,
                Password = txtNewPass.Text,
            };

            bool updateResult = await _userService.UpdateUser(userDTO);
            if (updateResult)
            {
                MessageBox.Show("Password updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ProfileWindow window = new ProfileWindow(_user);
                window.Show();
                //this.Close();
            }
            else
            {
                MessageBox.Show("Password update failed. User might not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Cancel_Button(object sender, RoutedEventArgs e)
        {
            ProfileWindow window = new ProfileWindow(_user);
            window.Show();
            this.Close();
        }
    }
}
