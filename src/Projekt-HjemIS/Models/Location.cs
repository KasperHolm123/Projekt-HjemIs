﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    /// <summary>
    /// Hovedforfatter: Jonas 
    /// </summary>
    public class Location
    {
        public string StreetCode { get; set; }
        public string CountyCode { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string PostalDistrict { get; set; }
        private string _street;
        public string Street
        {
            get { return _street; }
            set
            {
                _street = CleanInput(value);
            }
        }

        public override string ToString()
        {
            return $"{Street} {City}-{PostalCode}";
        }

        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-]", "",
                    RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            catch (RegexMatchTimeoutException)
            {
                // If we timeout when replacing invalid characters,
                // we should return Empty.
                return String.Empty;
            }
        }
    }
}
