using System;

namespace CarPark.WebApi.Models
{
    public class Lease
    {
        public string Plate { get; set; }
        public bool Status { get; set; }
        public DateTime Expiry { get; set; }
    }
}