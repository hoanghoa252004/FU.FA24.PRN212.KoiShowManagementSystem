using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DTOs;
using Entities;
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
        private  ShowDTO _selectedShow = null!;
        public ShowManagementWindow()
        {
            _showService = ShowService.Instance;
            InitializeComponent();
        }

        private async void BtnCreate(object sender, RoutedEventArgs e)
        {
            //ShowDTO newShow = new ShowDTO()
            //{
            //    Title = "Koi Show 2004",
            //    Description = "XXXXXXXXXXXX",
            //    EntranceFee = 125,
            //    RegisterEndDate = DateOnly.FromDateTime(DateTime.Now),
            //    RegisterStartDate = DateOnly.FromDateTime(DateTime.Now),
            //    Status = "UpComming",
            //    Criteria = new List<CriterionDTO>()
            //    {
            //        new CriterionDTO()
            //        {
            //            Name = "Pattern",
            //            Description = "XYXYXYXY",
            //            Percentage = 30,
            //        },
            //        new CriterionDTO()
            //        {
            //            Name = "Color",
            //            Description = "XYXYXYXY",
            //            Percentage = 70,
            //        }
            //    },
            //    Varieties = new List<VarietyDTO>()
            //    {
            //        new VarietyDTO()
            //        {
            //            Id = 1
            //        },
            //        new VarietyDTO()
            //        {
            //            Id = 3
            //        },
            //        new VarietyDTO()
            //        {
            //            Id = 5
            //        },
            //        new VarietyDTO()
            //        {
            //            Id = 6
            //        }
            //    },
            //    Referees = new List<UserDTO>()
            //    {
            //        new UserDTO()
            //        {
            //            Id = 11
            //        },
            //        new UserDTO()
            //        {
            //            Id = 12
            //        }
            //    },
            //};
            //bool result = await _showService.Add(newShow);
            //MessageBox.Show(result.ToString());
            AddShowDialog dialog = new AddShowDialog(RefreshWindow);
            dialog.ShowDialog();
        }

        private async void BtnDelete(object sender, RoutedEventArgs e)
        {
            bool result = await _showService.Delete(8);
            MessageBox.Show(result.ToString());
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
                    var result = await _showService.Search(key);
                    ShowGrid.ItemsSource = result;
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
        }
        private async void RefreshWindow()
        {
            if (_selectedShow != null)
            {
                var result = await _showService.Search(txtSearch.Text);
                ShowGrid.ItemsSource = result;
                var showToSelect = result
                    .FirstOrDefault(s => s.Id == _selectedShow.Id);
                if (showToSelect != null)
                {
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
            txtDescription.Clear();
            txtStatus.Clear();
            txtRegisterDate.Clear();
            txtTitle.Clear();
            VarietyListBox.ItemsSource = null;
            CriteriaListBox.ItemsSource = null;
        }
    }
}
