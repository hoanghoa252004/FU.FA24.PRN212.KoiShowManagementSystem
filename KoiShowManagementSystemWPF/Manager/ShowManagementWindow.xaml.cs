using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DTOs;
using Entities;
using KoiShowManagementSystemWPF.Member;
using KoiShowManagementSystemWPF.PopupDialog;
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
    /// Interaction logic for ShowManagementWindow.xaml
    /// </summary>
    public partial class ShowManagementWindow : Window
    {
        private readonly IShowService _showService;
        private readonly IKoiService _koiService;
        private  ShowDTO _selectedShow = null!;
        private readonly UserDTO _user = null!;
        public ShowManagementWindow(UserDTO user)
        {
            InitializeComponent();
            _showService = ShowService.Instance;
            _koiService = KoiService.Instance;
            _user = user;
            LoadData();
        }

        private async void LoadData()
        {
            var showList = await _showService.GetAll(_user.Id);
            ShowGrid.ItemsSource = showList;
            btnCreate.Visibility = Visibility.Visible;
            btnPublish.Visibility = Visibility.Collapsed;
            //btnAnnouceScore.Visibility = Visibility.Collapsed;
            btnScoring.Visibility = Visibility.Collapsed;
            btnReviewScore.Visibility = Visibility.Collapsed;
        }

        private void BtnCreate(object sender, RoutedEventArgs e)
        {
            AddShowDialog dialog = new AddShowDialog(RefreshWindow);
            dialog.ShowDialog();
        }

        private async void BtnDelete(object sender, RoutedEventArgs e)
        {
            if (_selectedShow != null)
            {
                try
                {
                    bool confirm = Confirm();
                    if (confirm == true)
                    {
                        bool result = await _showService.Delete(_selectedShow.Id);
                        if(result == true)
                        {
                            RefreshWindow();
                        }
                        else
                        {
                            throw new Exception("Delete Failed !");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void BtnSearch(object sender, RoutedEventArgs e)
        {
            try
            {
                string key = txtSearch.Text;
                if (string.IsNullOrWhiteSpace(key) == true)
                {
                    throw new Exception("Please enter search value !");
                }
                else
                {
                    var result = await _showService.Search(_user.Id, key);
                    if(result != null && result.Any() == true)
                    {
                        ShowGrid.ItemsSource = result;
                        ResetTextBox();
                    }
                    else
                    {
                        throw new Exception("No show matches !");
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool Confirm()
        {
            MessageBoxResult result = MessageBox.Show("You're deleting a room information. Are you sure ?",
                                                      "Confirmation",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                return true;
            else
                return false;
        }

        private void SelectShow(object sender, SelectionChangedEventArgs e)
        {
            if(ShowGrid.SelectedItem is ShowDTO show)
            {
                _selectedShow = show;
                DisplayShowInformation(show);
            }
        }

        private void DisplayShowInformation(ShowDTO dto)
        {
            txtTitle.Text = dto.Title;
            txtDescription.Text = dto.Description;
            txtStatus.Text = dto.Status;
            txtEntranceFee.Text = dto.EntranceFee.ToString();
            txtRegisterDate.Text = $"from {dto.RegisterStartDate} to {dto.RegisterEndDate}";
            VarietyListBox.ItemsSource = dto.Varieties;
            CriteriaListBox.ItemsSource = dto.Criteria;
            RefereeListBox.ItemsSource = dto.Referees;
            if (_user.Role!.Equals("Member", StringComparison.OrdinalIgnoreCase) == true)
            {
                if (dto.Status.Equals("OnGoing", StringComparison.OrdinalIgnoreCase) == true)
                {
                    btnRegister.Visibility = Visibility.Visible;
                }
                else
                {
                    btnRegister.Visibility = Visibility.Collapsed;
                }
            }
            else if (_user.Role!.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
            {
                // UPCOMING
                if (dto.Status.Equals("UpComing", StringComparison.OrdinalIgnoreCase) == true)
                {
                    btnUpdate.Visibility = Visibility.Visible;
                    btnDelete.Visibility = Visibility.Visible;
                    btnPublish.Visibility = Visibility.Visible;
                }
                else
                {
                    btnUpdate.Visibility = Visibility.Collapsed;
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnPublish.Visibility = Visibility.Collapsed;
                }

                // ONGOING:
                if (dto.Status.Equals("OnGoing", StringComparison.OrdinalIgnoreCase) == true)
                {
                    btnScoring.Visibility = Visibility.Visible;
                }
                else
                {
                    btnScoring.Visibility = Visibility.Collapsed;
                }

                // SCORING:
                if (dto.Status.Equals("Scoring", StringComparison.OrdinalIgnoreCase) == true
                    || dto.Status.Equals("Finished", StringComparison.OrdinalIgnoreCase) == true)
                {
                    btnReviewScore.Visibility = Visibility.Visible;
                    if (dto.Status.Equals("Scoring", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        //btnAnnouceScore.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        //btnAnnouceScore.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    btnReviewScore.Visibility = Visibility.Collapsed;
                }
            }
        }
        private async void RefreshWindow()
        {
            if (_selectedShow != null)
            {
                var result = await _showService.Search(_user.Id, txtSearch.Text);
                ShowGrid.ItemsSource = result;
                var showToSelect = result
                    .FirstOrDefault(s => s.Id == _selectedShow.Id);
                if (showToSelect != null)
                {
                    _selectedShow = showToSelect;
                    ShowGrid.SelectedItem = showToSelect;
                    ShowGrid.ScrollIntoView(showToSelect);
                    DisplayShowInformation(showToSelect);
                }
                else
                    ResetTextBox();
            }
        }

        private void ResetTextBox()
        {
            _selectedShow = null!;
            txtDescription.Clear();
            txtStatus.Clear();
            txtRegisterDate.Clear();
            txtTitle.Clear();
            txtEntranceFee.Clear();
            VarietyListBox.ItemsSource = null;
            CriteriaListBox.ItemsSource = null;
            RefereeListBox.ItemsSource = null;
            btnRegister.Visibility = Visibility.Collapsed;
            btnCreate.Visibility = Visibility.Collapsed;
            btnUpdate.Visibility = Visibility.Collapsed;
            btnDelete.Visibility = Visibility.Collapsed;
        }

        private async void BtnReviewScore(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = await _showService.ReviewScore(_selectedShow.Id);
                if (result != null && result.Any() == true)
                {
                    FinalResultWindow dialog = new FinalResultWindow(_selectedShow,result);
                    dialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void BtnRegister(object sender, RoutedEventArgs e)
        {
            try
            {
                var koiOfUser = await _koiService.GetKoiToRegiterShow(_user.Id);
                if (koiOfUser != null && koiOfUser.Any() == true)
                {
                    AddRegistrationDialog dialog = new AddRegistrationDialog(koiOfUser, _selectedShow);
                    dialog.ShowDialog();
                }
                else
                {
                    throw new Exception("You do not have any Koi Fish. Please add a Koi fish first !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnHomePage(object sender, RoutedEventArgs e)
        {
            MemberProfileWindow window = new MemberProfileWindow(_user);
            window.Show();
            this.Close();
        }

        private void BtnPublish(object sender, RoutedEventArgs e)
        {

        }

        private void BtnScoring(object sender, RoutedEventArgs e)
        {

        }

    }
}
