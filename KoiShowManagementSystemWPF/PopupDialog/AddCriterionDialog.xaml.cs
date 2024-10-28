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
    /// Interaction logic for AddCriterionDialog.xaml
    /// </summary>
    public partial class AddCriterionDialog : Window
    {
        public CriterionDTO NewCriterion { get; set; } = null!;
        private readonly decimal _remainingPercentage;
        public AddCriterionDialog(decimal remainingPercentage)
        {
            _remainingPercentage = remainingPercentage;
            InitializeComponent();
        }

        private void BtnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnReset(object sender, RoutedEventArgs e)
        {
            txtDescription.Clear();
            txtName.Clear();
            txtPercentage.Clear();
        }

        private void BtnSubmit(object sender, RoutedEventArgs e)
        {
            try
            {
                // VALIDATE:
                string message = "";
                if (string.IsNullOrWhiteSpace(txtName.Text) == true)
                {
                    message +="Name of criterion is invalid !\n";
                }
                if (string.IsNullOrWhiteSpace(txtDescription.Text) == true)
                {
                    message += "Description of criterion is invalid !\n";
                }
                if (decimal.TryParse(txtPercentage.Text, out _) == false)
                {
                    message += "Percentage of criterion is invalid !\n";
                }
                else
                {
                    if(decimal.Parse(txtPercentage.Text) <= 0)
                    {
                        message += $"Percentage must > 0 {_remainingPercentage} !\n";
                    }
                    if (decimal.Parse(txtPercentage.Text) > _remainingPercentage)
                    {
                        message += $"Percentage now only can be less or equal {_remainingPercentage} !\n";
                    }    
                }

                if (message.Length > 0)
                {
                    throw new Exception(message);
                }
                // CREATE NEW CRITERION:
                NewCriterion = new CriterionDTO()
                {
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                    Percentage = decimal.Parse(txtPercentage.Text),
                };
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
