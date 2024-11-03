using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DTOs;
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
    /// Interaction logic for ManageVarietyWindow.xaml
    /// </summary>
    public partial class ManageVarietyWindow : Window
    {
        private readonly IVarietyService varietyService;
        public ManageVarietyWindow()
        {
            varietyService = VarietyService.Instance;
            InitializeComponent();
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgData.ItemsSource = await varietyService.GetAll();
        }

        private async void Add_Button(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                MessageBox.Show("Please enter a variety name.");
                return;
            }

            var newVariety = new VarietyDTO
            {
                Name = VarietyNameTextBox.Text,
                Status = TrueRadioButton.IsChecked == true
            };

            await varietyService.AddVariety(newVariety);
            MessageBox.Show("Variety added successfully.");


            dgData.ItemsSource = await varietyService.GetAll();
            RefreshText();
        }


        private async void Delete_Button(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is VarietyDTO selectedVariety)
            {
                var confirmResult = MessageBox.Show("Are you sure to delete this variety?", "Confirm Delete", MessageBoxButton.YesNo);
                if (confirmResult == MessageBoxResult.Yes)
                {
                    await varietyService.DeleteVariety(selectedVariety.Id);
                    MessageBox.Show("Variety deleted successfully.");

                    dgData.ItemsSource = await varietyService.GetAll();
                    RefreshText();
                }
            }
            else
            {
                MessageBox.Show("Please select a variety to delete.");
            }
        }


        private async void BtnSearch(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                MessageBox.Show("Please enter a variety name to search.");
                return;
            }

            var result = await varietyService.SearchVarietyByName(txtSearch.Text);
            dgData.ItemsSource = null; // Xóa dữ liệu cũ nếu có
            if (result != null && result.Any())
            {
                dgData.ItemsSource = result;
                RefreshText();
            }
            else
            {
                MessageBox.Show("No Variety found with the given name.");
            }
        }


        private void Return_HomePage(object sender, RoutedEventArgs e)
        {

        }

        private async void Update_Button(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem is VarietyDTO selectedVariety)
            {
                selectedVariety.Name = VarietyNameTextBox.Text;
                selectedVariety.Status = TrueRadioButton.IsChecked == true;

                await varietyService.UpdateVariety(selectedVariety);
                MessageBox.Show("Variety updated successfully.");

                // Cập nhật lại DataGrid sau khi cập nhật
                dgData.ItemsSource = await varietyService.GetAll();
                RefreshText();
            }
            else
            {
                MessageBox.Show("Please select a variety to update.");
            }
        }
        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgData.SelectedItem is VarietyDTO selectReferee)
            {
                FillElement(selectReferee);
            }
        }
        private void FillElement(VarietyDTO variety)
        {
            

                VarietyNameTextBox.Text = variety.Name;
                TrueRadioButton.IsChecked = variety.Status;
                FalseRadioButton_Copy.IsChecked = !variety.Status;
                IdTextBox.Text = variety.Id.ToString(); 
            
        }

        private async void GetAll_Button(object sender, RoutedEventArgs e)
        {
            dgData.ItemsSource = await varietyService.GetAll();
        }

        private void RefreshText()
        {
            VarietyNameTextBox.Text = string.Empty; 
            TrueRadioButton.IsChecked = false;
            FalseRadioButton_Copy.IsChecked = false;
            IdTextBox.Text = string.Empty;
        }

    }
}
