﻿using ProjectZavod.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectZavod.Views.doorViewRedactor
{
    /// <summary>
    /// Логика взаимодействия для ViewRedactor.xaml
    /// </summary>
    public partial class ComboBoxInit : Window
    {
        public ComboBoxInit()
        {
            InitializeComponent();
            DataContext = new ComboBoxInitVM();
            Loaded += ComboBoxInit_Loaded;
        }

        private void ComboBoxInit_Loaded(object sender, RoutedEventArgs e)
        {
            if(DataContext is ICloseWindow vm)
            {
                vm.Close += () =>
                {
                    this.Close();
                };
            }
        }
    }
}
