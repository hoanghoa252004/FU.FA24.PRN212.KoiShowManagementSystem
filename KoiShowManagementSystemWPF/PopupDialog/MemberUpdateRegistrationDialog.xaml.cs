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
    /// Interaction logic for MemberUpdateRegistrationDialog.xaml
    /// </summary>
    public partial class MemberUpdateRegistrationDialog : Window
    {
        private readonly IEnumerable<KoiDTO> _koiOfUser = null!;
        private readonly IRegistrationService _registrationService;
        private RegistrationDTO _selectedRegistration = null!;
        public MemberUpdateRegistrationDialog(IEnumerable<KoiDTO> koiOfUser, RegistrationDTO selectedRegistration)
        {
            InitializeComponent();
            _koiOfUser = koiOfUser;
            _selectedRegistration = selectedRegistration;
            _registrationService = RegistrationService.Instance;
            LoadData();
        }

        public BitmapImage ToImage(byte[] array)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new System.IO.MemoryStream(array);
            image.EndInit();
            return image;
        }

        private void LoadData()
        {
            // Load Koi:
            KoiComboBox.ItemsSource = _koiOfUser;
            KoiComboBox.SelectedValue = _selectedRegistration.KoiId;
            BitmapImage image1 = ToImage(_selectedRegistration.Image01!);
            BitmapImage image2 = ToImage(_selectedRegistration.Image02!);
            BitmapImage image3 = ToImage(_selectedRegistration.Image03!);
            SelectedImage1.Source = image1;
            SelectedImage2.Source = image2;
            SelectedImage3.Source = image3;
        }

        private void BtnReplaceImage1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                // Kiểm tra hợp lệ:
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFilePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                // Update image1:
                _selectedRegistration.Image01 =  File.ReadAllBytes(selectedFilePath);

                // Update UI:
                SelectedImage1.Source = bitmap;
            }
        }

        private void BtnReplaceImage2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                // Kiểm tra hợp lệ:
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFilePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                // Update image1:
                _selectedRegistration.Image02 = File.ReadAllBytes(selectedFilePath);

                // Update UI:
                SelectedImage2.Source = bitmap;
            }
        }

        private void BtnReplaceImage3(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                // Kiểm tra hợp lệ:
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFilePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                // Update image1:
                _selectedRegistration.Image03 = File.ReadAllBytes(selectedFilePath);

                // Update UI:
                SelectedImage3.Source = bitmap;
            }
        }

        private void BtnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnSubmit(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = "";
                if (KoiComboBox.SelectedItem == null)
                {
                    message += "Please select a Koi ! \n";
                }
                if (_selectedRegistration.Image01 == null 
                    || _selectedRegistration.Image02 == null
                    || _selectedRegistration.Image02 == null)
                {
                    message += "Please select 3 image ! \n";
                }
                if (message.Length > 0)
                {
                    throw new Exception(message);
                }
                else
                {
                    _selectedRegistration.KoiId = (int)KoiComboBox.SelectedValue;
                    bool result = await _registrationService.Update(_selectedRegistration);
                    if (result == true)
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
    }
}
