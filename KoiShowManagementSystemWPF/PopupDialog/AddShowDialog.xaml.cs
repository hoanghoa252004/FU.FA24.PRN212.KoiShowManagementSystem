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

namespace KoiShowManagementSystemWPF.PopupDialog
{
    /// <summary>
    /// Interaction logic for AddShowDialog.xaml
    /// </summary>
    public partial class AddShowDialog : Window
    {
        private readonly IVarietyService _varietyService;
        private readonly IShowService _showService;
        private readonly IUserService _userService;
        private Action RefreshWindow;
        private List<CriterionDTO> _criteria = new List<CriterionDTO>()
                {
                    new CriterionDTO()
                    {
                        Name = "Pattern",
                        Description = "XYXYXYXfffffffffffffffffffffffffffffffffffffYssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss",
                        Percentage = 30,
                    },
                    new CriterionDTO()
                    {
                        Name = "Color",
                        Description = "XYXYXYXY",
                        Percentage = 30.4m,
                    },
                    new CriterionDTO()
                    {
                        Name = "Pattern",
                        Description = "XYXYXYXfffffffffffffffffffffffffffffffffffffYssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss",
                        Percentage = 12.4m,
                    },
                };
        public AddShowDialog(Action RefreshWindow)
        {
            this.RefreshWindow += RefreshWindow;
            _varietyService = VarietyService.Instance;
            _showService = ShowService.Instance;
            _userService = UserService.Instance;
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            VarietyListBox.ItemsSource = await _varietyService.GetAll();
            CriteriaListBox.ItemsSource = _criteria;
            RefereeListBox.ItemsSource = await _userService.GetAllReferee();
        }
        private void BtnAddCriteria(object sender, RoutedEventArgs e)
        {
            // Tính Percentage còn lại:
            decimal remainingPercentage = CalculateRemainingPercentage();
            AddCriterionDialog criterionDialog = new AddCriterionDialog(remainingPercentage);
            if (criterionDialog.ShowDialog() == true)
            {
                _criteria.Add(criterionDialog.NewCriterion);
                CriteriaListBox.Items.Refresh(); // Refresh ListView to display newly added Criterion
            }
        }
        private T FindVisualChild<T>(DependencyObject parent, string name = null!) where T : FrameworkElement
        {
            if (parent == null) return null!;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T element && (string.IsNullOrEmpty(name) || element.Name == name))
                {
                    return element;
                }

                var result = FindVisualChild<T>(child, name);
                if (result != null)
                    return result;
            }
            return null!;
        }

        private void BtnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnReset(object sender, RoutedEventArgs e)
        {

        }

        private async void BtnSubmit(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. VALIDATE:
                string message = "";
                if (string.IsNullOrWhiteSpace(txtTitle.Text) == true)
                    message += "Title is invalid !\n";
                if (string.IsNullOrWhiteSpace(txtDescription.Text) == true)
                    message += "Description is invalid !\n";
                if (decimal.TryParse(txtTEntranceFee.Text, out _) == false)
                    message += "Entrance Fee is invalid !\n";
                else
                {
                    if(decimal.Parse(txtTEntranceFee.Text) >= 0)
                    {
                        message += "Entrance Fee must >= 0 !\n";
                    }
                }
                if (!txtStartDate.SelectedDate.HasValue || !txtEndDate.SelectedDate.HasValue)
                {
                    message += "Start or End Date is invalid !\n";
                }
                else
                {
                    if(DateOnly.Parse(txtStartDate.Text) > DateOnly.Parse(txtEndDate.Text))
                    {
                        message += "Start Date can not be less than End Date !\n";
                    }
                }
                // Tính Percentage còn lại:
                decimal remainingPercentage = CalculateRemainingPercentage();
                if (remainingPercentage == 100)
                {
                    message += "Please add at least 1 criterion ! \n";
                }
                else
                {
                    if (remainingPercentage != 0)
                    {
                        message += "Total percentage of all criteria of a show must be 100% ! \n";
                    }
                }

                // 2. Lấy Variety:
                List<VarietyDTO> selectedVarieties = new List<VarietyDTO>();
                foreach (var item in VarietyListBox.Items)
                {
                    var variety = item as VarietyDTO;
                    // Whether có chọn ko:
                    var container = VarietyListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                    var checkBox = FindVisualChild<CheckBox>(container!);

                    if (checkBox != null && checkBox.IsChecked == true)
                    {
                        // Gom data:
                        selectedVarieties.Add(variety!);
                    }
                }
                if (selectedVarieties.Count < 3)
                {
                    message += "Please choose at least 3 varieties ! \n";
                }
                // 3. Lấy Referee:
                List<UserDTO> selectedReferees = new List<UserDTO>();
                foreach (var item in RefereeListBox.Items)
                {
                    var referee = item as UserDTO;
                    // Whether có chọn ko:
                    var container = VarietyListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                    var checkBox = FindVisualChild<CheckBox>(container!);

                    if (checkBox != null && checkBox.IsChecked == true)
                    {
                        // Gom data:
                        selectedReferees.Add(referee!);
                    }
                }
                if (selectedReferees.Count <= 0)
                {
                    message += "Please choose at least 1 referee ! \n";
                }

                if (message.Length > 0)
                    throw new Exception(message);
                else
                {
                    ShowDTO newShow = new ShowDTO()
                    {
                        Title = txtTitle.Text,
                        Description = txtDescription.Text,  
                        RegisterStartDate = DateOnly.Parse(txtStartDate.Text),
                        RegisterEndDate = DateOnly.Parse(txtEndDate.Text),
                        EntranceFee = decimal.Parse(txtTEntranceFee.Text),
                        Criteria = _criteria,
                        Varieties = selectedVarieties, 
                        Referees = selectedReferees,
                    };
                    await _showService.Add(newShow);
                    RefreshWindow();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private decimal CalculateRemainingPercentage()
        {
            // Tính Percentage còn lại:
            decimal remainingPercentage = 100;
            foreach (var criterion in _criteria)
            {
                remainingPercentage -= criterion.Percentage;
            }
            return remainingPercentage;
        }
    }
}
