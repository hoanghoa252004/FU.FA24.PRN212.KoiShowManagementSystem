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
        private readonly IVarietyService _varietyService;
        private readonly IUserService _userService;
        private  ShowDTO _selectedShow = null!;
        private readonly UserDTO _user = null!;
        private int? _lastBehavior = null!;
        // 1 là search.
        // default null || 2 là get all
        public ShowManagementWindow(UserDTO user)
        {
            InitializeComponent();
            _showService = ShowService.Instance;
            _koiService = KoiService.Instance;
            _varietyService = VarietyService.Instance;
            _userService = UserService.Instance;
            _user = user;
            LoadData();
        }

        private async void LoadData()
        {
            btnViewAllKoiParticipants.Visibility = Visibility.Collapsed;
            var showList = await _showService.GetAll(_user.Id);
            ShowGrid.ItemsSource = showList;
            btnPublish.Visibility = Visibility.Collapsed;
            //btnAnnouceScore.Visibility = Visibility.Collapsed;
            btnScoring.Visibility = Visibility.Collapsed;
            btnReviewScore.Visibility = Visibility.Collapsed;
            if (_user.Role!.Equals("Member", StringComparison.OrdinalIgnoreCase) == true)
            {
                btnUpdate.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
                btnCreate.Visibility = Visibility.Collapsed;
            }
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
                        _lastBehavior = 1;
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

                if (dto.Status.Equals("UpComing", StringComparison.OrdinalIgnoreCase) == true)
                {
                    btnPublish.Visibility = Visibility.Visible;
                }
                else
                {
                    btnPublish.Visibility = Visibility.Collapsed;
                }
                // ONGOING:
                if (dto.Status.Equals("OnGoing", StringComparison.OrdinalIgnoreCase) == true)
                {
                    btnScoring.Visibility = Visibility.Visible;
                    btnViewAllKoiParticipants.Visibility = Visibility.Visible;
                }
                else
                {
                    btnViewAllKoiParticipants.Visibility = Visibility.Collapsed;
                    btnScoring.Visibility = Visibility.Collapsed;
                }

                // SCORING:
                if (dto.Status.Equals("Scoring", StringComparison.OrdinalIgnoreCase) == true
                    || dto.Status.Equals("Finished", StringComparison.OrdinalIgnoreCase) == true)
                {
                    btnViewAllKoiParticipants.Visibility = Visibility.Visible;
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
                IEnumerable<ShowDTO> result = null!;
                if (_lastBehavior == 1)
                {
                    result = await _showService.Search(_user.Id, txtSearch.Text);
                    ShowGrid.ItemsSource = result;
                }
                else
                {
                    result = await _showService.GetAll(_user.Id);
                    ShowGrid.ItemsSource = result;
                }
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
            VarietyListBox.ItemsSource = null;
            CriteriaListBox.ItemsSource = null;
            RefereeListBox.ItemsSource = null;
            btnRegister.Visibility = Visibility.Collapsed;
            btnReviewScore.Visibility = Visibility.Collapsed;
            btnPublish.Visibility = Visibility.Collapsed;
            btnScoring.Visibility = Visibility.Collapsed;
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
                    RefreshWindow();
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

        private async void BtnPublish(object sender, RoutedEventArgs e)
        {
            if(_selectedShow != null)
            {
                try
                {
                    bool result = await _showService.Update(new ShowDTO()
                    {
                        Id = _selectedShow.Id,
                        Status = "OnGoing",
                    });
                    if (result == true)
                    {
                        RefreshWindow();
                    }
                    else
                    {
                        throw new Exception("Publish Show Failed !");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void BtnScoring(object sender, RoutedEventArgs e)
        {
            if (_selectedShow != null)
            {
                try
                {
                    bool result = await _showService.Update(new ShowDTO()
                    {
                        Id = _selectedShow.Id,
                        Status = "Scoring",
                    });
                    if (result == true)
                    {
                        RefreshWindow();
                    }
                    else
                    {
                        throw new Exception("Update Show to Scoring Failed !");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void BtnUpdate(object sender, RoutedEventArgs e)
        {
            if(_selectedShow != null)
            {
                if(_selectedShow.Status.Equals("UpComing", StringComparison.OrdinalIgnoreCase) == true)
                {
                    var varieties = await _varietyService.GetAll();
                    var referees = await _userService.GetAllReferee();
                    UpdateShowDialog dialog = new UpdateShowDialog(varieties.ToList(), referees.ToList(), _selectedShow);
                    dialog.ShowDialog();
                    RefreshWindow();
                }
                else
                {
                    MessageBox.Show("This show can not be updated as it is " + _selectedShow.Status, "Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void BtnGetAll(object sender, RoutedEventArgs e)
        {
            var showList = await _showService.GetAll(_user.Id);
            ShowGrid.ItemsSource = showList;
            _lastBehavior = 2;
        }

        private async void BtnViewAllKoiParticipants(object sender, RoutedEventArgs e)
        {
            var result = await _showService.GetAllKoiParticipants(_selectedShow.Id);
            if (result != null && result.Any() == true)
            {
                KoiParticipantsWindow w = new KoiParticipantsWindow(result);
                w.ShowDialog();
            }
            else
            {
                MessageBox.Show("No Koi Participant has register !", "Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
