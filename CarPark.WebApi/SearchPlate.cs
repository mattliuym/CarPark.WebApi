using System;
using System.Collections.Generic;
using CarPark.WebApi.Models;

namespace CarPark.WebApi
{
    public class SearchPlate
    {
        public int EnterId { get; set; }
        public string Plate { get; set; }
        public DateTime InTime { get; set; }
        public Boolean IsEarlyBird { get; set; }
        public Boolean IsPaid { get; set; }
        public decimal Fees { get; set; }
        
        public Boolean isLeft { get; set; }
        
        /* Calculate current parking fees*/
        public float ObtainFees(DateTime tm)
        {
            Mysql sqlData = new Mysql();
            List<Pricing> arrPricing = sqlData.ExecuteGetPricing();
            //current time
            DateTime currentTime = DateTime.Now;
            //parking time
            TimeSpan diff = currentTime - tm;
            //parking minutes.
            var mindiff = Math.Truncate(diff.TotalMinutes); 

            return 0;
        }
    }
    
    
}