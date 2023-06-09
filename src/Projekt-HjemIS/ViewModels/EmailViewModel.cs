﻿using Projekt_HjemIS.Models;
using Projekt_HjemIS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Projekt_HjemIS.ViewModels
{
    public class EmailViewModel : BaseViewModel
    {

        #region Fields

        #region Search fields

        private string _streetNameFilter;
        public string StreetNameFilter
        {
            get => _streetNameFilter;
            set
            {
                _streetNameFilter = value;
                OnPropertyChanged();
            }
        }

        private string _postalCodeFilter;
        public string PostalCodeFilter
        {
            get => _postalCodeFilter;
            set
            {
                _postalCodeFilter = value;
                OnPropertyChanged();
            }
        }

        private string _countyCodeFilter;
        public string CountyCodeFilter
        {
            get => _countyCodeFilter;
            set
            {
                _countyCodeFilter = value;
                OnPropertyChanged();
            }
        }

        private string _streetCodeFilter;
        public string StreetCodeFilter
        {
            get => _streetCodeFilter;
            set
            {
                _streetCodeFilter = value;
                OnPropertyChanged();
            }
        }

        private string _houseNumberFromFilter;
        public string HouseNumberFromFilter
        {
            get => _houseNumberFromFilter;
            set
            {
                _houseNumberFromFilter = value;
                OnPropertyChanged();
            }
        }

        private string _houseNumberToFilter;
        public string HouseNumberToFilter
        {
            get => _houseNumberToFilter;
            set
            {
                _houseNumberToFilter = value;
                OnPropertyChanged();
            }
        }

        private string _evenOddFilter;
        public string EvenOddFilter
        {
            get => _evenOddFilter;
            set
            {
                _evenOddFilter = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Collections/Collection objects

        private ObservableCollection<RecordTypeLocation> _locations;
        public ObservableCollection<RecordTypeLocation> Locations
        {
            get => _locations;
            set
            {
                _locations = value;
                OnPropertyChanged();
            }
        }

        private CollectionViewSource _filteredLocations;
        public CollectionViewSource FilteredLocations
        {
            get => _filteredLocations;
            set
            {
                _filteredLocations = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<RecordTypeLocation> _recipients;
        public ObservableCollection<RecordTypeLocation> Recipients
        {
            get => _recipients;
            set
            {
                _recipients = value;
                OnPropertyChanged();
            }
        }

        private Message _message;
        public Message Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        public RelayCommand AddRecipientCommand { get; set; }
        public RelayCommand RemoveRecipientCommand { get; set; }
        public RelayCommand CreateMessageCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ClearSearchCommand { get; set; }
        

        #endregion

        public EmailViewModel()
        {
            FilteredLocations = new CollectionViewSource();
            Recipients = new ObservableCollection<RecordTypeLocation>();
            Message = new Message();

            Task.Run(() => LoadCollectionsAsync());


            AddRecipientCommand = new RelayCommand(p => AddRecipient((RecordTypeLocation)p));
            RemoveRecipientCommand = new RelayCommand(p => RemoveRecipient((RecordTypeLocation)p));
            CreateMessageCommand = new RelayCommand(p => CreateMessageAsync());
            SearchCommand = new RelayCommand(p => Search());
            ClearSearchCommand = new RelayCommand(p => ClearSearch());
        }

        private async Task LoadCollectionsAsync()
        {
            IsBusy = true;

            var locations = await dh.GetTable<RecordTypeLocation>("SELECT * FROM [location]");
            Locations = new ObservableCollection<RecordTypeLocation>(locations);

            Application.Current.Dispatcher.Invoke(() =>
            {
                FilteredLocations.Source = Locations;
            });
            
            IsBusy = false;
        }

        private async void CreateMessageAsync()
        {
            var query = "INSERT INTO [message] ([Subject], Body) " +
                        "OUTPUT Inserted.ID " +
                        "VALUES (@Subject, @Body)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Subject", Message.Subject),
                new SqlParameter("@Body", Message.Body)
            };

            var messageId = await dh.AddDataReturn(query, parameters);

            await AddRecipientToMessageAsync(messageId);
        }

        private async Task AddRecipientToMessageAsync(int id)
        {
            foreach (var recipient in Recipients)
            {
                var phoneNumbers = await GetCustomersOnRecipientAsync(recipient);

                if (phoneNumbers.Count() < 1)
                {
                    var query = $"DELETE FROM [message] WHERE ID = {id}";

                    await dh.AddDataAsync(query);
                }
                
                foreach (var phoneNumber in phoneNumbers)
                {
                    var query = "INSERT INTO recipient (MessageID, RecipientPhoneNumber) " +
                                "VALUES (@ID, @PhoneNumber)";

                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@ID", id),
                        new SqlParameter("@PhoneNumber", phoneNumber)
                    };

                    await dh.AddDataAsync(query, parameters.ToArray());
                }
            }
        }

        private async Task<List<int>> GetCustomersOnRecipientAsync(RecordTypeLocation location)
        {
            var query = "SELECT PhoneNumber FROM customer " +
                        "INNER JOIN (" +
                        "SELECT CountyCode, StreetCode " +
                        "FROM [location] " +
                        "WHERE CountyCode = @CountyCode " +
                        "AND StreetCode = @StreetCode" +
                        ") AS l " +
                        "ON l.CountyCode = Customers.CountyCode " +
                        "AND l.StreetCode = Customers.StreetCode";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@CountyCode", location.CountyCode),
                new SqlParameter("@StreetCode", location.StreetCode)
            };

            List<Customer> customers = new List<Customer>();

            try
            {
                customers = await dh.GetTable<Customer>(query, parameters);

                if (customers.Count < 1)
                {
                    throw new Exception("No customers at chosen location");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            var phoneNumbers = customers.Select(x => x.PhoneNumber).ToList();

            return phoneNumbers;
        }

        private void AddRecipient(RecordTypeLocation location)
        {
            if (location != null)
            {
                Recipients.Add(location);
                Locations.Remove(location);
            }
        }

        private void RemoveRecipient(RecordTypeLocation location)
        {
            if (location != null)
            {
                Recipients.Remove(location);
                Locations.Add(location);
            }
        }

        private void Search()
        {
            FilteredLocations.Source = Locations.Where(location =>
                (string.IsNullOrEmpty(StreetNameFilter) || location.StreetName.ToUpper().Contains(StreetNameFilter.ToUpper())) &&
                (string.IsNullOrEmpty(PostalCodeFilter) || location.PostalCode.ToUpper().Contains(PostalCodeFilter.ToUpper())) &&
                (string.IsNullOrEmpty(CountyCodeFilter) || location.CountyCode.ToUpper().Contains(CountyCodeFilter.ToUpper())) &&
                (string.IsNullOrEmpty(StreetCodeFilter) || location.StreetCode.ToUpper().Contains(StreetCodeFilter.ToUpper())) &&
                (string.IsNullOrEmpty(HouseNumberFromFilter) || location.HouseNumberFrom.ToUpper().Contains(HouseNumberFromFilter.ToUpper())) &&
                (string.IsNullOrEmpty(HouseNumberToFilter) || location.HouseNumberTo.ToUpper().Contains(HouseNumberToFilter.ToUpper())) &&
                (string.IsNullOrEmpty(EvenOddFilter) || location.EvenOdd.ToUpper().Contains(EvenOddFilter.ToUpper())));
        }

        private void ClearSearch()
        {
            StreetNameFilter = "";
            PostalCodeFilter = "";
            CountyCodeFilter = "";
            StreetCodeFilter = "";
            HouseNumberFromFilter = "";
            HouseNumberToFilter = "";
            EvenOddFilter = "";
        }
    }
}
