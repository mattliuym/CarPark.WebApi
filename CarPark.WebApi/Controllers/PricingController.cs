using System;
using System.Collections;
using System.Collections.Generic;
using CarPark.WebApi.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CarPark.WebApi.Controllers
{
    /// <summary>
    /// Get the pricing plan
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PricingController : ControllerBase
    {
        [HttpGet]
        //para:the plate number of car (in capital letters)
        public IEnumerable<Pricing> GetPricing()
        {
            Mysql dbcontent = new Mysql();
            var res = dbcontent.ExecuteGetPricing(true);
            return Enumerable.Range(1, res.Count).Select(index => new Pricing()
            {
                PricingId = res[0].PricingId,
                PricingName = res[0].PricingName,
                TotalPark = res[0].TotalPark,
                OpenTime = res[0].OpenTime,
                CloseTime = res[0].CloseTime,
                IsTwentyFour = res[0].IsTwentyFour,
                IsFlatRate = res[0].IsFlatRate,
                PricePh = res[0].PricePh,
                HaveEarlyBird = res[0].HaveEarlyBird,
                HaveMax = res[0].HaveMax,
                EarlyBirdPrice = res[0].EarlyBirdPrice,
                MaxPrice = res[0].MaxPrice,
                FreeBefore = res[0].FreeBefore,
                IsMonthly = res[0].IsMonthly,
                MonthlyFees = res[0].MonthlyFees,
                InUse = res[0].InUse
            }).ToArray();
        }

        [HttpGet]
        public PricingStatus GetAllPricing()
        {
            string token = Request.Cookies["token"];
            var sqlcontent = new Mysql();
            var tk = sqlcontent.VerifyToken(token);
            if (!tk.Status)
            {
                return new PricingStatus()
                {
                    Status = false,
                    Error = "Error, please login again!"
                };
            }
            var res = sqlcontent.ExecuteGetPricing(false);
            return new PricingStatus()
            {
                Status = true,
                Pricings = Enumerable.Range(1, res.Count).Select(index => new Pricing()
                {
                    PricingId = res[index-1].PricingId,
                    PricingName = res[index-1].PricingName,
                    TotalPark = res[index-1].TotalPark,
                    OpenTime = res[index-1].OpenTime,
                    CloseTime = res[index-1].CloseTime,
                    IsTwentyFour = res[index-1].IsTwentyFour,
                    IsFlatRate = res[index-1].IsFlatRate,
                    PricePh = res[index-1].PricePh,
                    HaveEarlyBird = res[index-1].HaveEarlyBird,
                    HaveMax = res[index-1].HaveMax,
                    EarlyBirdPrice = res[index-1].EarlyBirdPrice,
                    MaxPrice = res[index-1].MaxPrice,
                    FreeBefore = res[index-1].FreeBefore,
                    IsMonthly = res[index-1].IsMonthly,
                    MonthlyFees = res[index-1].MonthlyFees,
                    InUse = res[index-1].InUse
                }).ToArray(),
            };
        }

        [HttpPost]
        public UploadPricing UpdatePricing([FromBody] Pricing p)
        {
            string token = Request.Cookies["token"];
            var sqlcontent = new Mysql();
            var tk = sqlcontent.VerifyToken(token);
            if (!tk.Status)
            {
                return new UploadPricing()
                {
                    Status = false,
                    Error = "Error, please login again!"
                };
            }
            string res;
            if (p.PricingId==0)
            {
                res = sqlcontent.ExecuteNonQuery($"INSERT INTO `parkinglot`.`pricing_plans` (`pricing_name`, `total_park`, `open_time`, `close_time`, `is_twentyfour`, `is_flatrate`, `price_ph`, `have_earlybird`, `earlybird_price`, `have_max`, `max_price`, `free_before`, `is_paymonthly`,`monthly_fee`) VALUES ('{p.PricingName}', '{p.TotalPark}', '{p.OpenTime}', '{p.CloseTime}', {p.IsTwentyFour}, {p.IsFlatRate}, '{p.PricePh}', {p.HaveEarlyBird}, '{p.EarlyBirdPrice}', {p.HaveMax}, '{p.MaxPrice}', '{p.FreeBefore}', {p.IsMonthly},'{p.MonthlyFees}');");
            }
            else
            {
                res = sqlcontent.ExecuteNonQuery(
                    $"UPDATE `parkinglot`.`pricing_plans` SET `pricing_name` = '{p.PricingName}', `total_park` = '{p.TotalPark}', `open_time` = '{p.OpenTime}', `close_time` = '{p.CloseTime}', `is_twentyfour` = {p.IsTwentyFour}, `is_flatrate` = {p.IsFlatRate}, `price_ph` = '{p.PricePh}', `have_earlybird` = {p.HaveEarlyBird}, `earlybird_price` = '{p.EarlyBirdPrice}', `have_max` = {p.HaveMax}, `max_price` = '{p.MaxPrice}', `free_before` = '{p.FreeBefore}', `is_paymonthly` = {p.IsMonthly},`monthly_fee` = {p.MonthlyFees} WHERE (`pricing_id` = '{p.PricingId}');");
            }
            if (res == "Success")
            {
                //if try to enable the new plan, then change the scheme status.
                if (p.InUse)
                {
                    var change=sqlcontent.ExcutePricingChange(p.PricingId);
                    if (change)
                    {
                        return new UploadPricing()
                        {
                            Status = true,
                            Public = true
                        };
                    }
                    return new UploadPricing()
                        {
                            Status = true,
                            Public = false,
                            Error = "Fail to enable the pricing scheme."
                        };
                }
                return new UploadPricing()
                {
                    Status = true,
                    Public = true
                };
            }

            return new UploadPricing()
            {
                Status = true,
                Public = false,
                Error = res
            };
        }
    }
}