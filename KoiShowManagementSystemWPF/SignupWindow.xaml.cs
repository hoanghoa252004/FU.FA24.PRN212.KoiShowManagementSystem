using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KoiShowManagementSystemWPF
{
    /// <summary>
    /// Interaction logic for SignupWindow.xaml
    /// </summary>
    public partial class SignupWindow : Window
    {
        private readonly IUserService _userService;
        public SignupWindow()
        {
            _userService = UserService.Instance;
            InitializeComponent();
        }

        private async void Create_Button(object sender, RoutedEventArgs e)
        {
            ErrorMessageTextBlock.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ErrorMessageTextBlock.Text = "Please fill in all required fields.";
                return;
            }

            if (!IsPhoneNumberValid(PhoneTextBox.Text))
            {
                ErrorMessageTextBlock.Text = "Phone numbers must contain only digits and have to contain 10 digits";
                return;
            }

            if (!IsEmailValid(EmailTextBox.Text))
            {
                ErrorMessageTextBlock.Text = "Please enter a valid email address.";
                return;
            }

            if (!IsNameValid(NameTextBox.Text))
            {
                ErrorMessageTextBlock.Text = "Name must contain only letters and spaces.";
                return;
            }

            var user = new UserDTO
            {
                Phone = PhoneTextBox.Text,
                Name = NameTextBox.Text,
                Password = PasswordTextBox.Text,
                Email = EmailTextBox.Text,
            };

            try
            {
                bool result = await _userService.CreateUser(user, 3);

                if (result)
                {
                    MessageBox.Show("Created successfully!");
                    LoginWindow window = new LoginWindow();
                    window.Show();
                    this.Close();
                }
                else
                {
                    ErrorMessageTextBlock.Text = "Error: Unable to create user.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessageTextBlock.Text = $"Error to signup: {ex.Message}";
            }
        }


        private bool IsPhoneNumberValid(string phone)
        {
            return phone.Length == 10 && phone.All(char.IsDigit);
        }


        private bool IsEmailValid(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private bool IsNameValid(string name)
        {
            return name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }



        private void ResetFields()
        {
            PhoneTextBox.Text = string.Empty;
            NameTextBox.Text = string.Empty;
            PasswordTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
        }

        private void Cancel_Button(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow(); 
            window.Show();
            this.Close();
        }

        private void Reset_Button(object sender, RoutedEventArgs e)
        {
            ResetFields();
        }
    }
}
