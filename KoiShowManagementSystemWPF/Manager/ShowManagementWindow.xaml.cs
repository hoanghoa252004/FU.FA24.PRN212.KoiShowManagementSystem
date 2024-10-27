using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DTOs;
using Entities;
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
        public ShowManagementWindow()
        {
            _showService = ShowService.Instance;
            InitializeComponent();
        }

        private async void BtnCreate(object sender, RoutedEventArgs e)
        {
            ShowDTO newShow = new ShowDTO()
            {
                Title = "Koi Show 2004",
                Description = "XXXXXXXXXXXX",
                EntranceFee = 125,
                RegisterEndDate = DateOnly.FromDateTime(DateTime.Now),
                RegisterStartDate = DateOnly.FromDateTime(DateTime.Now),
                Status = "UpComming",
                Criteria = new List<CriterionDTO>()
                {
                    new CriterionDTO()
                    {
                        Name = "Pattern",
                        Description = "XYXYXYXY",
                        Percentage = 30,
                    },
                    new CriterionDTO()
                    {
                        Name = "Color",
                        Description = "XYXYXYXY",
                        Percentage = 70,
                    }
                },
                Varieties = new List<VarietyDTO>()
                {
                    new VarietyDTO()
                    {
                        Id = 1
                    },
                    new VarietyDTO()
                    {
                        Id = 3
                    },
                    new VarietyDTO()
                    {
                        Id = 5
                    },
                    new VarietyDTO()
                    {
                        Id = 6
                    }
                },
                Referees = new List<UserDTO>()
                {
                    new UserDTO()
                    {
                        Id = 11
                    },
                    new UserDTO()
                    {
                        Id = 12
                    }
                },
            };
            bool result = await _showService.Add(newShow);
            MessageBox.Show(result.ToString());
        }

        private async void BtnDelete(object sender, RoutedEventArgs e)
        {
            bool result = await _showService.Delete(8);
            MessageBox.Show(result.ToString());
        }
    }
}
