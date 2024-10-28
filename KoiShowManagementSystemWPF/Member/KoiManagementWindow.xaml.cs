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
        public KoiManagementWindow()
        {
            _koiService = KoiService.Instance;
            _varietyService = VarietyService.Instance;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
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
            dgData.ItemsSource = await _koiService.GetAllKoisByUser(13);
        }

        private void FillElements(KoiDTO koi)
        {

            KoiIdTextBox.Text = koi.Id.ToString();
            KoiNameTextBox.Text = koi.Name;
            KoiVarietyTextBox.Text = koi.VarietyName;
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

            var result = await _koiService.SearchKoiName(KoiNameSearchTextBox.Text, 13);
            dgData.ItemsSource = null; // Clear the grid

            if (result != null)
            {
                dgData.ItemsSource = result ;
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
            }
        }



        private void Add_Click(object sender, RoutedEventArgs e)
        {
            CreateKoiWindow createKoiWindow = new CreateKoiWindow();
            createKoiWindow.ShowDialog();
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is KoiDTO selectedKoi)
            {
                // Validate the input
                if (string.IsNullOrWhiteSpace(KoiNameTextBox.Text) || !decimal.TryParse(KoiSizeTextBox.Text, out decimal size))
                {
                    MessageBox.Show("Please enter valid values for Name and Size.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update the KoiDTO
                selectedKoi.Name = KoiNameTextBox.Text;
                if (selectedKoi.Size > size)
                {
                    MessageBox.Show("Please enter a valid value for Size.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    selectedKoi.Size = size;
                }

                try
                {
                    await _koiService.UpdateKoi(selectedKoi); // Await the update
                    MessageBox.Show("Koi updated successfully!");
                    FillDataGrid(); // Refresh the data grid
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating Koi: {ex.Message}");
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
        }
    }
}
