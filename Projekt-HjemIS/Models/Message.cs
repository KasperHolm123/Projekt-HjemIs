﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    public class Message
    {

        public List<Customer> Recipients { get; set; }
        public int ID { get; set; }
        public string MessageBody { get; set; }
        public List<Product> Offers { get; set; }

        public Message()
        {

        }
    }
}