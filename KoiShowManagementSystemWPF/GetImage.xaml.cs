using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DTOs;
using Entities;
using Microsoft.EntityFrameworkCore.Internal;
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

namespace KoiShowManagementSystemWPF
{
    /// <summary>
    /// Interaction logic for GetImage.xaml
    /// </summary>
    public partial class GetImage : Window
    {
        private readonly IRegistrationService _services;
        public byte[] ImageData { get; set; } = null!;
        public GetImage()
        {
            _services = RegistrationService.Instance;
            InitializeComponent();
        }
        private async void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            // Mở hộp thoại chọn file
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Lấy đường dẫn tệp đã chọn
                string selectedFilePath = openFileDialog.FileName;

                try
                {
                    // Kiểm tra xem tệp có phải là ảnh hợp lệ hay không
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(selectedFilePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    // Hiển thị ảnh để xác nhận
                    imagePreview.Source = bitmap;

                    // Chuyển đổi ảnh thành byte[]
                    ImageData = ConvertImageToByteArray(selectedFilePath);

                    // Thông báo trạng thái thành công
                    statusText.Text = "Image loaded successfully.";
                    statusText.Foreground = new SolidColorBrush(Colors.Green);

                    bool result = await _services.Add(new RegistrationDTO()
                    {
                        KoiId = 3,
                        Description = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                        Size = 35,
                        ShowId = 9,
                        Image01 = ImageData,
                        Image02 = ImageData,
                        Image03 = ImageData,
                    });
                    MessageBox.Show(result.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private byte[] ConvertImageToByteArray(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }
    }
}
