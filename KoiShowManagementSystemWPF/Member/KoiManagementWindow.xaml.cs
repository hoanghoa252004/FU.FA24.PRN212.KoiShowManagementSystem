using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DTOs;
using KoiShowManagementSystemWPF.PopupDialog;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for KoiManagementWindow.xaml
    /// </summary>
    public partial class KoiManagementWindow : Window
    {
        private readonly IKoiService _koiService;
        private readonly IVarietyService _varietyService;
        private readonly UserDTO _user;
        public KoiManagementWindow(UserDTO user)
        {
            _koiService = KoiService.Instance;
            _varietyService = VarietyService.Instance;
            InitializeComponent();
            _user = user;
        }
        private async void FillCombox()
        {
            // Display name nhưng lấy Id
            var varieties = await _varietyService.GetAll();
            VarietyIdComboBox.ItemsSource = varieties;
            //VarietyIdComboBox.DisplayMemberPath = "Name";
            //VarietyIdComboBox.SelectedValuePath = "Id";
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
            FillCombox();
        }
        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Nếu cái được chọn là một DTO thì thay đổi theo
            if (dgData.SelectedItem is KoiDTO selectedKoi)
            {
                FillElements(selectedKoi);
            }
        }

        private async void FillDataGrid()
        {
            dgData.ItemsSource = null; //xóa grid
            dgData.ItemsSource = await _koiService.GetAllKoisByUser(_user.Id);
        }

        private async void FillElements(KoiDTO koi)
        {

            KoiIdTextBox.Text = koi.Id.ToString();
            KoiNameTextBox.Text = koi.Name;
            VarietyIdComboBox.SelectedValue = koi.VarietyId;
            KoiSizeTextBox.Text = koi.Size.ToString();
            KoiImagePath.Source = ByteArrayToImage(koi.Image);
            if (koi.Status)
            {
                ActiveRadioButton.IsChecked = true;
            }
            else
            {
                InactiveRadioButton.IsChecked = true;
            }
            // Lấy achivements:
            var achivements = await _koiService.GetKoiAchivements(koi.Id);
            if(achivements != null && achivements.Any() == true)
            {
                AchivementsListBox.ItemsSource = achivements;
            }
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

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(KoiNameSearchTextBox.Text))
            {
                MessageBox.Show("Incorrect name of koi. Please type an input is string", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = await _koiService.SearchKoiName(KoiNameSearchTextBox.Text, _user.Id);
            dgData.ItemsSource = null; // Clear the grid

            if (result != null)
            {
                dgData.ItemsSource = result ;
                ResetFields();
            }
            else
            {
                MessageBox.Show("No Koi found with the given name.");
            }
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            KoiNameSearchTextBox.Clear();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgData.SelectedItem is KoiDTO selectedKoi)
                {
                    var result = MessageBox.Show("Are you sure you want to delete this Koi?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        await _koiService.DeleteKoi(selectedKoi.Id); // phải xóa xong rồi mới load lại data grid
                        MessageBox.Show("Koi deleted successfully.");
                    }
                }
                else
                {
                    MessageBox.Show("Please select a Koi to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting Koi: {ex.Message}");
            }
            finally
            {
                FillDataGrid();
                ResetFields();
            }
        }



        private void Add_Click(object sender, RoutedEventArgs e)
        {
            CreateKoiWindow createKoiWindow = new CreateKoiWindow(_user);
            createKoiWindow.ShowDialog();
            FillDataGrid();
            FillCombox();
            ResetFields();
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is KoiDTO selectedKoi)
            {
                // Validate the input
                if (string.IsNullOrWhiteSpace(KoiNameTextBox.Text) ||
                    !decimal.TryParse(KoiSizeTextBox.Text, out decimal size))
                {
                    MessageBox.Show("Please enter valid values for Name and Size.", "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                selectedKoi.Name = KoiNameTextBox.Text;
                selectedKoi.Size = size;

                if (VarietyIdComboBox.SelectedValue != null)
                {
                    selectedKoi.VarietyId = (int)VarietyIdComboBox.SelectedValue;
                }
                else
                {
                    MessageBox.Show("Please select a variety.", "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Set status based on radio button selection
                selectedKoi.Status = ActiveRadioButton.IsChecked == true;

                try
                {
                    var result =
                    await _koiService.UpdateKoi(selectedKoi);
                    if(result == false)
                    {
                        MessageBox.Show("Update vartiety failed ! \nThis Koi fish has already participanted in a show.", "Error",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                        ResetFields();
                        return;
                    }else
                    MessageBox.Show("Koi updated successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating Koi: {ex.Message}", "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    FillDataGrid();
                    ResetFields();
                }
            }
            else
            {
                MessageBox.Show("Please select a Koi to update.");
            }
        }


        private void All_Click(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
            ResetFields();
        }

        private void BtnHomePage(object sender, RoutedEventArgs e)
        {
            MemberProfileWindow window = new MemberProfileWindow(_user);
            window.Show();
            this.Close();
        }
        private void ResetFields()
        {
            KoiIdTextBox.Text = string.Empty;
            KoiNameTextBox.Text = string.Empty;
            VarietyIdComboBox.SelectedIndex = -1; 
            KoiSizeTextBox.Text = string.Empty;
            KoiImagePath.Source = null; 
            ActiveRadioButton.IsChecked = false;
            InactiveRadioButton.IsChecked = false;
        }

        private void BtnAchivements(object sender, RoutedEventArgs e)
        {

        }
    }
}
