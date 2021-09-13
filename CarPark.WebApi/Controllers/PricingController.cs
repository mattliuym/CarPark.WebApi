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
                }).ToArray(),
            };
        }
        

    }
}