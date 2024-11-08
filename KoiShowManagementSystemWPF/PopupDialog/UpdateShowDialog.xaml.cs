using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for UpdateShowDialog.xaml
    /// </summary>
    public partial class UpdateShowDialog : Window
    {
        private readonly IShowService _showService;
        private readonly List<VarietyDTO> _varieties = null!;
        private readonly List<UserDTO> _referees = null!;
        private readonly ShowDTO _show;
        private ObservableCollection<CriterionDTO> _criteria = null!;
        public UpdateShowDialog(List<VarietyDTO> varieties, List<UserDTO> referees, ShowDTO show)
        {
            InitializeComponent();
            _showService = ShowService.Instance;
            _show = show;
            _varieties = varieties;
            _referees = referees;
            _criteria = new ObservableCollection<CriterionDTO>(_show.Criteria!);
            LoadData();
        }

        private void LoadData()
        {
            VarietyListBox.ItemsSource = _varieties;
            RefereeListBox.ItemsSource = _referees;
            CriteriaListBox.ItemsSource = _criteria;
            txtTitle.Text = _show.Title;
            txtDescription.Text = _show.Description;
            txtStartDate.Text = _show.RegisterStartDate.ToString();
            txtEndDate.Text = _show.RegisterEndDate.ToString();
            txtTEntranceFee.Text = _show.EntranceFee.ToString();
        }

        private void BtnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                    if (decimal.Parse(txtTEntranceFee.Text) <= 0)
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
                    if (DateOnly.Parse(txtStartDate.Text) > DateOnly.Parse(txtEndDate.Text))
                    {
                        message += "Start Date can not be less than End Date !\n";
                    }
                    else
                    {
                        if (DateOnly.Parse(txtStartDate.Text) < DateOnly.FromDateTime(DateTime.Now))
                        {
                            message += "Start Date can not be the pass !\n";
                        }
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
                    var container = RefereeListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
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
                        Id = _show.Id,
                        Title = txtTitle.Text,
                        Description = txtDescription.Text,
                        RegisterStartDate = DateOnly.Parse(txtStartDate.Text),
                        RegisterEndDate = DateOnly.Parse(txtEndDate.Text),
                        EntranceFee = decimal.Parse(txtTEntranceFee.Text),
                        Criteria = _criteria,
                        Varieties = selectedVarieties,
                        Referees = selectedReferees,
                        Status = _show.Status,
                    };
                    bool result = await _showService.Update(newShow);
                    if(result == true)
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

        private void LoadedHehe(object sender, RoutedEventArgs e)
        {
            // Select lại Varieties:
            IEnumerable<VarietyDTO> varieties = _show.Varieties!;
            foreach (var item in VarietyListBox.ItemsSource)
            {
                var variety = item as VarietyDTO;
                VarietyDTO selectedVariety = varieties.SingleOrDefault(v => v.Id == variety!.Id)!;
                if (selectedVariety != null)
                {
                    var container = VarietyListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                    var checkBox = FindVisualChild<CheckBox>(container!);
                    if (checkBox != null)
                    {
                        checkBox.IsChecked = true;
                    }
                }
            }
            // Select lại Referee:
            IEnumerable<UserDTO> referees = _show.Referees!;
            foreach (var item in RefereeListBox.ItemsSource)
            {
                var referee = item as UserDTO;
                UserDTO selectedReferee = referees.SingleOrDefault(r => r.Id == referee!.Id)!;
                if (selectedReferee != null)
                {
                    var container = RefereeListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                    var checkBox = FindVisualChild<CheckBox>(container!);
                    if (checkBox != null)
                    {
                        checkBox.IsChecked = true;
                    }
                }
            }
        }

        private void BtnRemoveCriteria(object sender, RoutedEventArgs e)
        {
            // Lấy Criteria từ nút Remove
            var button = sender as FrameworkElement;
            var criteria = button?.Tag as CriterionDTO;
            if (criteria != null)
            {
                var c = _criteria.SingleOrDefault(cr => cr.Id == criteria!.Id);
                if(c != null)
                {
                    _criteria.Remove(criteria);
                    //CriteriaListBox.ItemsSource = _criteria;
                }
            }
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

        private void BtnUpdateCriteria(object sender, RoutedEventArgs e)
        {
            // Lấy Criteria từ nút Remove
            var button = sender as FrameworkElement;
            var criteria = button?.Tag as CriterionDTO;
            if (criteria != null)
            {
                var c = _criteria.SingleOrDefault(cr => cr.Id == criteria!.Id);
                if (c != null)
                {
                    UpdateCriterionDialog dialog = new UpdateCriterionDialog(c, CalculateRemainingPercentage()+c.Percentage);
                    if (dialog.ShowDialog() == true)
                    {
                        int index = _criteria.IndexOf(c);
                        _criteria.RemoveAt(index);
                        _criteria.Insert(index, dialog._criterion);
                        CriteriaListBox.Items.Refresh();
                    }
                }
            }
        }
    }
}
