﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
//using Microsoft.WindowsAPICodePack.Dialogs;
//using System.Windows.Shapes;
using netDxf;
using netDxf.Entities;
using netDxf.Header;
using Ninject;
using ProjectZavod.classes;
using ProjectZavod.Interfaces;
using ProjectZavod.ViewModels;

namespace ProjectZavod
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowVM(ConfigureContainer());
        }

        public StandardKernel ConfigureContainer()
        {
            var container = new StandardKernel();
            container.Bind<RootPaths>().ToSelf().InSingletonScope();
            container.Bind<IParamsReader>().To<OrderReader>().InSingletonScope();
            return container;
        }
    }
}