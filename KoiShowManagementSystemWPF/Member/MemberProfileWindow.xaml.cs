﻿using DTOs;
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

namespace KoiShowManagementSystemWPF.Member
{
    /// <summary>
    /// Interaction logic for MemberProfileWindow.xaml
    /// </summary>
    public partial class MemberProfileWindow : Window
    {
        private readonly UserDTO _user = null!;
        public MemberProfileWindow(UserDTO user)
        {
            _user = user;
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            txtName.Text = _user.Name; 
        }
    }
}
