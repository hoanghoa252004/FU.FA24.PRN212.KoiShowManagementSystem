﻿using BusinessLogicLayer.Implementation;
using BusinessLogicLayer.Interface;
using DTOs;
using KoiShowManagementSystemWPF.PopupDialog;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace KoiShowManagementSystemWPF.Member
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private readonly IRegistrationService _service;
        private RegistrationDTO _selectedRegistration = null!;
        private readonly UserDTO _user;
        public RegistrationWindow(UserDTO user)
        {
            _service = RegistrationService.Instance;
            _user = user;
            InitializeComponent();
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
                    var result = await _service.Search(key);
                    RegistrationGrid.ItemsSource = result;
                    ResetTextBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed:", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ResetTextBox()
        {
            _selectedRegistration = null!;
            txtDescription.Text = "";
            txtStatus.Text = "";
            txtId.Text = "";
            txtKoiVariety.Text = "";
            txtKoiName.Text = "";
            txtSize.Text = "";
            txtImage1.Visibility = Visibility.Collapsed;
            txtImage2.Visibility = Visibility.Collapsed;
            txtImage3.Visibility = Visibility.Collapsed;
            btnUpdate.Visibility = Visibility.Collapsed;
            btnDelete.Visibility = Visibility.Collapsed;
            PanelResult.Visibility = Visibility.Collapsed;
        }

        private void SelectRegistration(object sender, SelectionChangedEventArgs e)
        {
            if (RegistrationGrid.SelectedItem is RegistrationDTO registration)
            {
                _selectedRegistration = registration;
                DisplayRegistrationInformation();
            }
        }

        private void DisplayRegistrationInformation()
        {
            if(_selectedRegistration != null)
            {
                if (_user.Role!.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (_selectedRegistration.Status!.Equals("Pending", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        btnUpdate.Visibility = Visibility.Visible;
                    }
                }
                else if (_user.Role!.Equals("Member", StringComparison.OrdinalIgnoreCase) == true)
                {
                    btnDelete.Visibility = Visibility.Visible;
                    if (_selectedRegistration.Status!.Equals("Rejected", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        btnUpdate.Visibility = Visibility.Visible;
                    }

                }
                txtDescription.Text = _selectedRegistration!.Description;
                txtStatus.Text = _selectedRegistration.Status;
                txtId.Text = _selectedRegistration.Id.ToString();
                txtKoiVariety.Text = _selectedRegistration.KoiVariety;
                txtKoiName.Text = _selectedRegistration.KoiName;
                txtSize.Text = _selectedRegistration.Size.ToString();
                txtNote.Text = _selectedRegistration.Note;
                if (_selectedRegistration.TotalScore != null
                    && _selectedRegistration.Rank != null)
                {
                    txtTotalScore.Text = _selectedRegistration.TotalScore.ToString();
                    txtRanking.Text = _selectedRegistration.Rank.ToString();
                    if (_selectedRegistration.IsBestVote == true)
                    {
                        txtIsBestVote.Text = "Yes";
                    }
                    else
                    {
                        txtIsBestVote.Text = "No";
                    }
                    PanelResult.Visibility = Visibility.Visible;
                }
                else
                {
                    PanelResult.Visibility = Visibility.Collapsed;
                }
                // Load Image
                BitmapImage image1 = ToImage(_selectedRegistration.Image01!);
                BitmapImage image2 = ToImage(_selectedRegistration.Image02!);
                BitmapImage image3 = ToImage(_selectedRegistration.Image03!);
                txtImage1.Source = image1;
                txtImage2.Source = image2;
                txtImage3.Source = image3;
                txtImage1.Visibility = Visibility.Visible;
                txtImage2.Visibility = Visibility.Visible;
                txtImage3.Visibility = Visibility.Visible;
            }
        }
        public BitmapImage ToImage(byte[] array)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new System.IO.MemoryStream(array);
            image.EndInit();
            return image;
        }


        private void BtnUpdate(object sender, RoutedEventArgs e)
        {
            if(_selectedRegistration != null)
            {
                if (_user.Role!.Equals("Member", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (_selectedRegistration.Status!.Equals("Rejected", StringComparison.OrdinalIgnoreCase) == true)
                    {

                    }
                }
                else if (_user.Role!.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (_selectedRegistration.Status!.Equals("Pending", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        AdminUpdateRegistrationDialog dialog = new AdminUpdateRegistrationDialog(_selectedRegistration, RefreshWindow);
                        dialog.ShowDialog();
                    }
                }
            }
        }

        private void BtnAdd(object sender, RoutedEventArgs e)
        {

        }

        private async void BtnDelete(object sender, RoutedEventArgs e)
        {
            try
            {
                bool result = false;
                if (_selectedRegistration != null)
                {
                    if (_user.Role!.Equals("Member", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        if(Confirm() == true)
                        {
                            result = await _service.Delete(_selectedRegistration.Id);
                        }
                    }
                }
                if (result == true)
                {
                    RefreshWindow();
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message,"Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void RefreshWindow()
        {
            if (_selectedRegistration != null)
            {
                var result = await _service.Search(txtSearch.Text);
                RegistrationGrid.ItemsSource = result;
                var registrationToSelect = result
                    .FirstOrDefault(registration => registration.Id == _selectedRegistration.Id);
                if (_selectedRegistration != null)
                {
                    RegistrationGrid.SelectedItem = _selectedRegistration;
                    RegistrationGrid.ScrollIntoView(_selectedRegistration);
                    DisplayRegistrationInformation();
                }
                else
                    ResetTextBox();
            }
        }

        private async void BtnGetAll(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_user != null)
                {
                    var result = await _service.GetAllRegistration(_user.Id);
                    if (result != null)
                    {
                        if (result.Any() == true)
                        {
                            RegistrationGrid.ItemsSource = result;
                            ResetTextBox();
                        }
                        else
                        {
                            throw new Exception("There do not have any registration !");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool Confirm()
        {
            MessageBoxResult result = MessageBox.Show("You're deleting a room information. Are you sure ?",
                                                      "Confirmation",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                return true;
            else
                return false;
        }
    }
}
