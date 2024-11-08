using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DataAccessLayer.Implementation;
using DTOs;
using KoiShowManagementSystemWPF.Member;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace KoiShowManagementSystemWPF.Referee
{
    public partial class ScoringWindow : Window
    {
        private readonly ICrierionService _crierionService;
        private readonly IRegistrationService _registrationService;
        private readonly IScoreService _scoreService;
        private readonly UserDTO _user = null!;
        public ScoringWindow(UserDTO user)
        {
            _crierionService = CriterionService.Instance;
            _registrationService = RegistrationService.Instance;
            _scoreService = ScoreService.Instance;
            _user = user;
            InitializeComponent();
        }


        private async Task LoadRegistrations()
        {
            var registrations = await _registrationService.GetAllRegistrationForReferee(_user.Id); 
            dgData.ItemsSource = registrations;
        }

        private  void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is RegistrationDTO selectedRegistration)
            {
                CriteriaDataGrid.ItemsSource = selectedRegistration.Scores;
                FillElements(selectedRegistration);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var registrations = await RegistrationDAO.Instance.GetRegistrationsByReferee(_user.Id);
            dgData.ItemsSource = registrations;
        }


        private void FillElements(RegistrationDTO registration)
        {
            KoiIdTextBox.Text = registration.Id.ToString();
            KoiNameTextBox.Text = registration.KoiName;
            KoiVarietyTextBox.Text = registration.VarietyName;
            KoiSizeTextBox.Text = registration.Size.ToString();
            ImagePath1.Source = ByteArrayToImage(registration.Image01);
            ImagePath2.Source = ByteArrayToImage(registration.Image02);
            ImagePath3.Source = ByteArrayToImage(registration.Image03);
            CriteriaDataGrid.ItemsSource = registration.Scores;

            Console.WriteLine("Scores: " + registration.Scores.Count);
        }

        private BitmapImage ByteArrayToImage(byte[] byteArray)
        {
            using (var stream = new MemoryStream(byteArray))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                return image;
            }
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is RegistrationDTO selectedRegistration &&
                CriteriaDataGrid.ItemsSource is IEnumerable<ScoreDTO> scoresSource)
            {
                var scores = scoresSource.ToList();

                foreach (var score in scores)
                {
                    if (score.Score1 < 0 || score.Score1 > 100)
                    {
                        MessageBox.Show("Please input scores between 0 and 100 for all criteria before submitting.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!decimal.TryParse(score.Score1.ToString(), out _))
                    {
                        MessageBox.Show("Please input valid numeric scores for all criteria before submitting.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                try
                {
                    await _scoreService.InsertScores(_user.Id, selectedRegistration.Id, scores); // Replace 11 with actual UserId
                    await LoadRegistrations();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error submitting scores: {ex.Message}");
                }
                finally
                {
                    ResetFields();
                }
            }
            else
            {
                MessageBox.Show("Please select a registration to submit scores for.");
            }
        }

        private void ResetFields()
        {
            KoiIdTextBox.Text = string.Empty;
            KoiNameTextBox.Text = string.Empty;
            KoiVarietyTextBox.Text = string.Empty;
            KoiSizeTextBox.Text = string.Empty;
            ImagePath1.Source = null;
            ImagePath2.Source = null;
            ImagePath3.Source = null;
            CriteriaDataGrid.ItemsSource = null;
        }


        private void BtnHomePage(object sender, RoutedEventArgs e)
        {
            MemberProfileWindow window = new MemberProfileWindow(_user);
            window.Show();
            this.Close();
        }
    }
}
