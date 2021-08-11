using System;

namespace CarPark.WebApi
{
    public class SearchPlate
    {
        public int EnterId { get; set; }
        public string Plate { get; set; }
        public DateTime InTime { get; set; }
        public Boolean IsEarlyBird { get; set; }
        public Boolean IsPaid { get; set; }
        public float Fees { get; set; }
    }
}