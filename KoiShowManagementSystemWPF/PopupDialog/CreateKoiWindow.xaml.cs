using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DTOs;
using Entities;
using Microsoft.Win32;
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

namespace KoiShowManagementSystemWPF.PopupDialog
{
    /// <summary>
    /// Interaction logic for CreateKoiWindow.xaml
    /// </summary>
    public partial class CreateKoiWindow : Window
    {
        private readonly IKoiService _koiService;
        private readonly IVarietyService _varietyService;
        private UserDTO _user= new();
        private byte[]? _imageBinaryData;

        public CreateKoiWindow()
        {
            InitializeComponent();
            _koiService =  KoiService.Instance;
            _varietyService =  VarietyService.Instance;
            FillCombox();
        }

        private async void FillCombox()
        {
            //Show name nhưng lấy Id
            var varieties = await _varietyService.GetAllVarieties();
            VarietyIdComboBox.ItemsSource = varieties;
            VarietyIdComboBox.DisplayMemberPath = "Name";
            VarietyIdComboBox.SelectedValuePath = "Id";
        }

        private void Button_Upload_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                //chỉ chấp nhận các file ảnh sau jpg, jpeg, png
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Select an Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePicture.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                string filePath = openFileDialog.FileName;
                _imageBinaryData = File.ReadAllBytes(filePath);
                //MessageBox.Show("Image uploaded successfully!");
            }
        }

        private void Button_Create_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(KoiNameTextBox.Text))
            {
                MessageBox.Show("Please enter a name for the Koi.");
                return;
            }

            if (VarietyIdComboBox.SelectedValue == null)
            {
                MessageBox.Show("Please select a variety for the Koi.");
                return;
            }

            if (!int.TryParse(SizeTextBox.Text, out int size))
            {
                MessageBox.Show("Please enter a valid size for the Koi.");
                return;
            }

            if (_imageBinaryData == null)
            {
                MessageBox.Show("Please upload an image for the Koi.");
                return;
            }

            var newKoi = new KoiDTO
            {
                Name = KoiNameTextBox.Text,
                Size = size,
                Description = KoiDescriptionTextBox.Text,
                Status = true,
                VarietyId = (int)VarietyIdComboBox.SelectedValue,
                Image = _imageBinaryData,
                UserId = 13                                         //HARD CODE IS DEFAULT USER HAS ID 13 ==> THẤT BẠI :))
            };

            try
            {
                _koiService.CreateKoi(newKoi);
                MessageBox.Show("Koi created successfully!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating Koi: {ex.Message}");
            }
        }

    }
}
