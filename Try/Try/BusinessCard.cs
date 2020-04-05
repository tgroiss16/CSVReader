using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Try
{
    public class BusinessCard
    {
        public string CompanyName { get; set; }
        public string Telefon { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
        public string Notes { get; set; }
        public bool? IsReviewed { get; set; }
        public long Id { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
    }
}

