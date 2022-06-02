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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems.Utility.Database_handling;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace Projekt_HjemIS.Views
{
    /// <summary>
    /// Interaction logic for OfferViews.xaml
    /// </summary>
    public partial class EditCustomerView : UserControl
    {
        DatabaseHandler dh = new DatabaseHandler();

        public ObservableCollection<Customer> _customers { get; set; }

        public EditCustomerView()
        {
            InitializeComponent();

            UpdateGrid();
        }

        private void UpdateGrid()
        {
            try
            {
                _customers = new ObservableCollection<Customer>(dh.GetTable<Customer>("SELECT * FROM Customers"));
                customerInfoGrid.ItemsSource = _customers;
            }
            catch (Exception ex) { MessageBox.Show($"Oops, something went wrong. Error code: {ex}"); }
        }

        private void customerInfoGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Customer selectedCustomer = customerInfoGrid.SelectedItem as Customer;
                if (selectedCustomer != null)
                {
                    firstName.Text = selectedCustomer.FirstName;
                    lastName.Text = selectedCustomer.LastName;
                    phoneNum.Text = selectedCustomer.PhoneNumber.ToString();
                    streetCode.Text = selectedCustomer.StreetCode;
                    countyCode.Text = selectedCustomer.CountyCode;

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private static SqlParameter CreateParameter(string paramName, object value, SqlDbType type)
        {
            SqlParameter param = new SqlParameter
            {
                ParameterName = paramName,
                Value = value,
                SqlDbType = type
            };
            return param;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "UPDATE Customers " +
                    "SET FirstName = @firstName, LastName = @lastName, PhoneNumber = @phoneNum, StreetCode = @streetCode, CountyCode = @countyCode " +
                    "WHERE PhoneNumber = @phoneNum AND CountyCode = @countyCode AND StreetCode = @streetCode";
                SqlParameter[] sp = new SqlParameter[]
                {
                    CreateParameter("@firstName", firstName.Text, SqlDbType.NVarChar),
                    CreateParameter("@lastName", lastName.Text, SqlDbType.NVarChar),
                    CreateParameter("@phoneNum", int.Parse(phoneNum.Text), SqlDbType.Int),
                    CreateParameter("@streetCode", streetCode.Text, SqlDbType.NVarChar),
                    CreateParameter("@countyCode", countyCode.Text, SqlDbType.NVarChar)
                };
                PromptProductHandling(dh.AddData(query, sp));
                UpdateGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PromptProductHandling(int notFound)
        {
            try
            {


                if (notFound == 0)
                {
                    MessageBoxResult msgResult = MessageBox.Show("Kunden er ikke fundet. Ønsker du at oprette en ny?", "Error", MessageBoxButton.YesNo);
                    switch (msgResult)
                    {
                        case MessageBoxResult.Yes:
                            string insertQuery = "INSERT INTO Customers (FirstName, LastName, PhoneNumber, StreetCode, CountyCode) " +
                                "VALUES (@firstName, @lastName, @phoneNum, @streetCode, @countyCode);";
                            SqlParameter[] insertSp = new SqlParameter[]
                            {
                            CreateParameter("@firstName", firstName.Text, SqlDbType.NVarChar),
                            CreateParameter("@lastName", lastName.Text, SqlDbType.NVarChar),
                            CreateParameter("@phoneNum", int.Parse(phoneNum.Text), SqlDbType.Int),
                            CreateParameter("@streetCode", streetCode.Text, SqlDbType.NVarChar),
                            CreateParameter("@countyCode", countyCode.Text, SqlDbType.NVarChar)
                            };
                            dh.AddData(insertQuery, insertSp);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    MessageBoxResult confirmBox = MessageBox.Show("Kunden er opdateret.", "Success", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
    }
}