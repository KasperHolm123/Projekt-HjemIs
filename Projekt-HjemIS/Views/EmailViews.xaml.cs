﻿using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems;
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
    /// Interaction logic for EmailViews.xaml
    /// </summary>
    public partial class EmailViews : UserControl
    {
        public ObservableCollection<Customer> InternalCustomers { get; set; } = DatabaseHandler.GetCustomers();
        public ObservableCollection<Product> InternalProducts { get; set; } //= DatabaseHandler.GetProducts(); // mangler metode

        public List<Customer> messageRecipients { get; set; }

        public EmailViews()
        {
            InitializeComponent();

            // Setup collections

            // Bind comboboxes
            ComboTo.ItemsSource = InternalCustomers;
            ComboOffers.ItemsSource = InternalProducts;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Message message = new Message(messageBodytxt.Text, messageRecipients);
        }
    }
}
