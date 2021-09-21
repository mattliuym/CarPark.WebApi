using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CarPark.WebApi.Models
{
    public class Lease
    {
        public string Plate { get; set; }
        public bool Status { get; set; }
        public DateTime Expiry { get; set; }
    }

    public class LeaseInfo
    {
        public int LeaseId { get; set; }
        public string Plate { get; set; }
        public DateTime Expiry { get; set; }
        public bool Valid { get; set; }
        
    }
    public class LeaseStatus
    {
        public bool Status { get; set; }
        public IEnumerable<LeaseInfo> LeaseLists{ get; set; }
        public string Error { get; set; }
    }

    public class AppendLease
    {
        public bool Status { get; set; }
        public bool Public { get; set; }
        public string Error { get; set; }
    }
}