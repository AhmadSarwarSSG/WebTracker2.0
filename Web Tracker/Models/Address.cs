using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebTracker.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string CountryCode { get; set; } = "";
        public string CountryName { get; set; } = "";
        public string City { get; set; } = "";
        public string Postal { get; set; } = "";
        public string Latitude { get; set; } = "";
        public string Longitude { get; set; } = "";
        public string IPv4 { get; set; } = "";
        public string State { get; set; } = "";
    }
}