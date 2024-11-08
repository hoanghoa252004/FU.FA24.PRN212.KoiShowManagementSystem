using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DTOs;
using KoiShowManagementSystemWPF.Member;
using KoiShowManagementSystemWPF.PopupDialog;
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
    /// Interaction logic for RefereeManagementWindow.xaml
    /// </summary>
    public partial class RefereeManagementWindow : Window
    {
        private readonly IUserService _userService;
        private readonly UserDTO _user;
        public RefereeManagementWindow(UserDTO user)
        {
            _userService = UserService.Instance;
            InitializeComponent();
            _user = user;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
        }
        private async void FillDataGrid()
        {
            dgData.ItemsSource = null; //xóa grid
            dgData.ItemsSource = await _userService.GetAllRefereeToView(); // Hardcode user ID nè
        }
        private void BtnHomePage(object sender, RoutedEventArgs e)
        {
            ProfileWindow window = new ProfileWindow(_user);
            window.Show();
            this.Close();
        }
        private async void Delete_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                if(dgData.SelectedItem is UserDTO selectedUser)
                {
                    var result = MessageBox.Show("Are you sure you want to delete this Koi?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        await _userService.DeleteReferee(selectedUser.Id); // phải xóa xong rồi mới load lại data grid
                        MessageBox.Show("user deleted successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting Referee: {ex.Message}");
            }
            finally
            {
                FillDataGrid();
                ClearFields();
            }
        }

        private void Add_Button(object sender, RoutedEventArgs e)
        {
            CreateRefereeWindow createRefereeWindow = new CreateRefereeWindow();
            createRefereeWindow.ShowDialog();
            FillDataGrid();
        }

        private async void BtnSearch(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                MessageBox.Show("Please enter a name to search.");
                return;
            }

            var result = await _userService.SearchRefereeName(txtSearch.Text);

            dgData.ItemsSource = result;
            ClearFields();
            if (!result.Any())
            {
                MessageBox.Show("No Referee found with the given name.");
                ClearFields();
            }
        }


        private async void Update_Button(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is UserDTO user)
            {

                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Please enter a valid name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || !IsValidEmail(EmailTextBox.Text))
                {
                    MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(PhoneTextBox.Text) || !IsValidPhoneNumber(PhoneTextBox.Text))
                {
                    MessageBox.Show("Please enter a valid phone number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                user.Name = NameTextBox.Text;
                user.Email = EmailTextBox.Text;
                user.Phone = PhoneTextBox.Text;
                user.Status = TrueRadioButton.IsChecked == true;

                try
                {
                    await _userService.UpdateUser(user);
                    MessageBox.Show("Referee updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    FillDataGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating referee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    FillDataGrid();
                    ClearFields();
                }
            }
            else
            {
                MessageBox.Show("Please select a referee to update.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //function hỗ trợ 
        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }
        //function hỗ trợ
        private bool IsValidPhoneNumber(string phone)
        {
            return Regex.IsMatch(phone, @"^\+?[0-9]{10,15}$"); // Adjust pattern based on phone number format requirements
        }


        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is UserDTO selectReferee) {
                FillElements(selectReferee);
            }
        }

        private void FillElements(UserDTO user)
        {
            NameTextBox.Text = user.Name;
            EmailTextBox.Text = user.Email;
            PhoneTextBox.Text = user.Phone;
            TrueRadioButton.IsChecked = user.Status;
            FalseRadioButton.IsChecked = !user.Status;
        }

        private void GetAllReferee_Button(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
            ClearFields();
            txtSearch.Text = string.Empty;
        }

        private void ClearFields()
        {
            // Clear TextBox fields
            NameTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
            PhoneTextBox.Text = string.Empty;


            // Deselect RadioButton selections
            TrueRadioButton.IsChecked = false;
            FalseRadioButton.IsChecked = false;
        }


    }
}
