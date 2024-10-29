using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DTOs;
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
    /// Interaction logic for AddRegistrationDialog.xaml
    /// </summary>
    public partial class AddRegistrationDialog : Window
    {
        private readonly IEnumerable<KoiDTO> _koiOfUser = null!;
        private readonly IRegistrationService _registrationService;
        private readonly ShowDTO _show = null!;
        private readonly List<byte[]> _image = new List<byte[]>();
        public AddRegistrationDialog(IEnumerable<KoiDTO> koiOfUser, ShowDTO show)
        {
            InitializeComponent();
            _koiOfUser = koiOfUser;
            _registrationService = RegistrationService.Instance;
            KoiComboBox.ItemsSource = _koiOfUser;
            _show = show;
        }

        private void BtnSelectImage(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = _image.Count;
                if (count < 3)
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

                        // Lưu byte:
                        _image.Add(File.ReadAllBytes(selectedFilePath));

                        // HIện lên cho nó xem:
                        count = _image.Count;
                        switch (count)
                        {
                            case 1:
                                {
                                    SelectedImage1.Source = bitmap;
                                    break;
                                }
                            case 2:
                                {
                                    SelectedImage2.Source = bitmap;
                                    break;
                                }
                            case 3:
                                {
                                    SelectedImage3.Source = bitmap;
                                    break;
                                }
                        }
                    }
                }
                else
                {
                    throw new Exception("You're only required to select just 3 image !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                if(KoiComboBox.SelectedItem == null)
                {
                    message += "Please select a Koi ! \n";
                }
                if(_image.Count != 3)
                {
                    message += "Please select 3 image ! \n";
                }
                if(message.Length > 0)
                {
                    throw new Exception(message);
                }
                else
                {
                    RegistrationDTO registrationDTO = new RegistrationDTO()
                    {
                        KoiId = (int)KoiComboBox.SelectedValue!,
                        Image01 = _image[0],
                        Image02 = _image[1],
                        Image03 = _image[2],
                        ShowId = _show.Id,
                    };
                    bool result = await _registrationService.Add(registrationDTO);
                    if(result == true)
                    {
                        MessageBox.Show("Register Koi For Show Successfully ." +
                            "\n Please tracking your registration !", "Successful:", MessageBoxButton.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }

        private void BtnReset(object sender, RoutedEventArgs e)
        {
            SelectedImage1.Source = null;
            SelectedImage2.Source = null;
            SelectedImage3.Source = null;
            KoiComboBox.SelectedIndex = -1;
            _image.Clear();
        }
    }
}
