﻿using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems.Utility.Database_handling;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projekt_HjemIS.Views
{
    /// <summary>
    /// Interaction logic for ProductsView.xaml
    /// </summary>
    public partial class ProductsView : UserControl
    {
        DatabaseHandler dh = new DatabaseHandler();

        public ObservableCollection<Product> InternalProducts { get; set; }

        public ProductsView()
        {
            InitializeComponent();

            //Setup collections
            InternalProducts = new ObservableCollection<Product>(dh.GetTable<Product>("SELECT * FROM Products"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
