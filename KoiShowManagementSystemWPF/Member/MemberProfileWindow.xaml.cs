using DTOs;
using KoiShowManagementSystemWPF.Manager;
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
        public MemberProfileWindow(UserDTO user)
        {
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
                btnKois.Visibility = Visibility.Visible;
                if (_user.Role!.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
                {
                    btnReferees.Visibility = Visibility.Visible;
                    btnVarieties.Visibility = Visibility.Visible;
                }
                if (_user.Role!.Equals("Referee", StringComparison.OrdinalIgnoreCase) == true)
                {
                    btnScoring.Visibility = Visibility.Visible;
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
    }
}
