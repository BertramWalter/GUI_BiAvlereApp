﻿using System;
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

namespace GUI_Eksamen_BiAvlereApp.Views
{
    /// <summary>
    /// Interaction logic for VarroaCountView.xaml
    /// </summary>
    public partial class VarroaCountView : Window
    {
        public VarroaCountView()
        {
            InitializeComponent();
          
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
