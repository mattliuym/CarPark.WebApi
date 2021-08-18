using System;

namespace CarPark.WebApi
{
    public class Pricing
    {
        public int PricingId { get; set; }
        public string PricingName { get; set; }
        public int TotalPark { get; set; }
        public String OpenTime { get; set; }
        public String CloseTime { get; set; }
        public Boolean IsTwentyFour { get; set; }
        public Boolean IsFlatRate { get; set; }
        public decimal PricePh { get; set; }
        public Boolean HaveEarlyBird { get; set; }
        public decimal EarlyBirdPrice { get; set; }
        public Boolean HaveMax { get; set; }
        public decimal MaxPrice { get; set; }
        public int FreeBefore { get; set; }
        public Boolean IsMonthly { get; set; }
        public decimal MonthlyFees { get; set; }
        public Boolean InUse { get; set; }
    }
}