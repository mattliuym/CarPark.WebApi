using System;
using System.Collections.Generic;

namespace CarPark.WebApi.Models
{
    public class SearchPlate
    {
        public int EnterId { get; set; }
        public string Plate { get; set; }
        public DateTime InTime { get; set; }
        public Boolean IsEarlyBird { get; set; }
        public Boolean IsPaid { get; set; }
        public Boolean IsMonthly { get; set; }
        public decimal Fees { get; set; }
        public Boolean isLeft { get; set; }
        public double timeLength { get; set; }

        /* Calculate current parking fees(only for non-monthly pay cars)*/
        public decimal ObtainFees(double tm)
        {
            decimal fees;
            Mysql sqlData = new Mysql();
            List<Pricing> arrPricing = sqlData.ExecuteGetPricing();
            Pricing priceInfo = arrPricing[0];

            //verify if the car needs to pay
            if (tm <= priceInfo.FreeBefore)
            {
                return 0;
            }
            //check if have a flat rate
            if (priceInfo.IsFlatRate)
            {
                fees = priceInfo.PricePh;
                return fees;
            }
            //calculate the fee by hour
            decimal fee = (decimal)tm / 60 * priceInfo.PricePh;
            //if the car belongs to early bird policy
            if (IsEarlyBird)
            {
                if (fee < priceInfo.EarlyBirdPrice)
                {
                    fees=Math.Round(fee,2);
                }
                else
                {
                    fees = priceInfo.EarlyBirdPrice;
                }
                return fees;
            }
            //if reach the max charge.
            if (priceInfo.HaveMax)
            {
                if (fee > priceInfo.MaxPrice)
                {
                    fees = priceInfo.MaxPrice;
                }
                else
                {
                    fees=Math.Round(fee,2);
                }

                return fees;
            }
            //else 
            fees=Math.Round(fee,2);
            return fees;
        }
    }

    public class CurrentPlate
    {
        public bool Status { get; set; }
        public IEnumerable<SearchPlate> AllPlate{get;set;}
        public string Error { get; set; }
    }

    public class AddPlate
    {
        public bool Status { get; set; }
        public bool AddSuccess { get; set; }
        public string Error { get; set; }
    }

    public class Plate
    {
        public string PlateNum { get; set; }
    }

    public class ReleasePlate
    {
        //public int enterId { get; set; }
        public bool Status { get; set; }
        public bool ReleaseSuccess { get; set; }
        public string Error { get; set; }
    }

    public class HistoryParking
    {
        public int EnterId{ get; set; }
        public string Plate { get; set; }
        public DateTime InTime { get; set; }
        public DateTime OutTime { get; set; }
        public bool IsMonthly{ get; set; } 
        public decimal Fees { get; set; }
    }
    public class HistoryPlate
    {
        public bool Status { get; set; }
        public IEnumerable<HistoryParking> History{get;set;}
        public string Error { get; set; }
    }
}