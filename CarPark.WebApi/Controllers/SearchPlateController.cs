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
    /// search the car information by plate
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SearchPlateController : ControllerBase
    {
        [HttpGet]
        //para:the plate number of car (in capital letters)
        
        public IEnumerable<SearchPlate> GetCarInfo(string s)
        {
            Mysql sqlcontent = new Mysql();
            var res = sqlcontent.ExecuteSearchPlate($"SELECT * FROM parkinglot.plate where plate='{s}'&& is_left=0;");
            //verify lease condition.
            var lease = sqlcontent.VerifyLease(res[0].Plate);
            Console.WriteLine("@ "+lease);
            if (lease)
            {
                sqlcontent.ExecuteNonQuery($"UPDATE `parkinglot`.`plate` SET `is_leased` = '1', `is_paid` = '1' WHERE (`enter_id` = '{res[0].EnterId}');");
                res[0].IsPaid = true;
                res[0].IsMonthly = true;
            }
            else
            {
                sqlcontent.ExecuteNonQuery($"UPDATE `parkinglot`.`plate` SET `is_leased` = '0', `is_paid` = '0' WHERE (`enter_id` = '{res[0].EnterId}');");
                res[0].IsPaid = false;
                res[0].IsMonthly = false;
            }
            //current time
            var currentTime = DateTime.Now;
            //parking time
            var diff = currentTime - res[0].InTime;
            //parking minutes
            var mindiff = Math.Truncate(diff.TotalMinutes);
            if (res[0].IsPaid)//have paid the fees, no needs to calculate again
                return Enumerable.Range(1, res.Count).Select(index => new SearchPlate()
                {
                    Plate = res[index - 1].Plate,
                    EnterId = res[index - 1].EnterId,
                    Fees = res[index - 1].Fees,
                    InTime = res[index - 1].InTime,
                    IsEarlyBird = res[index - 1].IsEarlyBird,
                    IsPaid = res[index - 1].IsPaid,
                    timeLength = mindiff
                }).ToArray();
            //calculate the parking charge and store into database
            sqlcontent.ExecuteNonQuery($"UPDATE `parkinglot`.`plate` SET `fees` = '{res[0].ObtainFees(mindiff)}' WHERE (`enter_id` = '{res[0].EnterId}');");
            return Enumerable.Range(1,res.Count).Select(index=>new SearchPlate()
            {
                Plate = res[index-1].Plate,
                EnterId = res[index-1].EnterId,
                Fees = res[index-1].ObtainFees(mindiff),
                InTime = res[index-1].InTime,
                IsEarlyBird = res[index-1].IsEarlyBird,
                IsPaid = res[index-1].IsPaid,
                timeLength = mindiff
            }).ToArray();
        }

        [HttpGet]
        //Get all current parking car info
        public IEnumerable<SearchPlate> GetCurrentInfo()
        {
            var sqlcontent = new Mysql();
            var res = sqlcontent.ExecuteSearchPlate("SELECT * FROM parkinglot.plate WHERE 'is_left'=0");
            //current time
            var currentTime = DateTime.Now;
            foreach (var t in res)
            {
                if (t.IsPaid==false)//if the car have yet paid, then calculate the parking fee
                {
                    var diff = currentTime - t.InTime;
                    var mindiff = Math.Truncate(diff.TotalMinutes);
                    sqlcontent.ExecuteNonQuery($"UPDATE `parkinglot`.`plate` SET `fees` = '{t.ObtainFees(mindiff)}' WHERE (`enter_id` = '{t.EnterId}');");
                    t.Fees = t.ObtainFees(mindiff);
                    t.timeLength = mindiff;
                }
            }
            return Enumerable.Range(1,res.Count).Select(index=>new SearchPlate()
            {
                Plate = res[index-1].Plate,
                EnterId = res[index-1].EnterId,
                Fees = res[index-1].Fees,
                InTime = res[index-1].InTime,
                IsEarlyBird = res[index-1].IsEarlyBird,
                IsPaid = res[index-1].IsPaid,
                timeLength = res[index-1].timeLength
            });
        }
       

    }
}