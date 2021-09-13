using System;
using System.Collections;
using System.Collections.Generic;
using CarPark.WebApi.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

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
            //reset lease condition
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
                    IsMonthly = res[index-1].IsMonthly,
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
                IsMonthly = res[index-1].IsMonthly,
                timeLength = mindiff
            }).ToArray();
        }

        [HttpGet]
        //Get all current parking car info
        public CurrentPlate GetCurrentInfo()
        {
            //verify login status
            string token = Request.Cookies["token"];
            var sqlcontent = new Mysql();
            var tk = sqlcontent.VerifyToken(token);
            if (!tk.Status)
            {
                return new CurrentPlate
                {
                    Status = false,
                    Error = "Error, please login again!"

                };
            }
            //get plate info
            var res = sqlcontent.ExecuteSearchPlate("SELECT * FROM parkinglot.plate WHERE is_left = false");
            //current time
            var currentTime = DateTime.Now;
            foreach (var t in res)
            {
                var diff = currentTime - t.InTime;
                var mindiff = Math.Truncate(diff.TotalMinutes);
                var lease = sqlcontent.VerifyLease(t.Plate);
                //check lease status
                if (lease)
                {
                    t.IsPaid = true;
                    t.IsMonthly = true;
                }
                else if(t.IsPaid&&t.IsMonthly)
                {
                    sqlcontent.ExecuteNonQuery($"UPDATE `parkinglot`.`plate` SET `is_leased` = false, `is_paid` = false WHERE (`enter_id` = '{t.EnterId}');");
                    t.IsPaid = false;
                    t.IsMonthly = false;
                }
                if (!t.IsPaid)//if the car have yet paid, then calculate the parking fee
                {
                    sqlcontent.ExecuteNonQuery($"UPDATE `parkinglot`.`plate` SET `fees` = '{t.ObtainFees(mindiff)}' WHERE (`enter_id` = '{t.EnterId}');");
                    t.Fees = t.ObtainFees(mindiff);
                }
                t.timeLength = mindiff;
            }

            return new CurrentPlate()
            {
                AllPlate = Enumerable.Range(1, res.Count).Select(index => new SearchPlate()
                {
                    Plate = res[index - 1].Plate,
                    EnterId = res[index - 1].EnterId,
                    Fees = res[index - 1].Fees,
                    InTime = res[index - 1].InTime,
                    IsEarlyBird = res[index - 1].IsEarlyBird,
                    IsPaid = res[index - 1].IsPaid,
                    IsMonthly = res[index - 1].IsMonthly,
                    timeLength = res[index - 1].timeLength
                }),
                Status = true
            };
        }

        [HttpGet]
        //Get all history parking information
        public HistoryPlate GetHistoryInfo()
        {
            //verify login status
            string token = Request.Cookies["token"];
            var sqlcontent = new Mysql();
            var tk = sqlcontent.VerifyToken(token);
            if (!tk.Status)
            {
                return new HistoryPlate
                {
                    Status = false,
                    Error = "Error, please login again!"
                };
            }         
            //get info
            var res = sqlcontent.ExcuteHistoryPlate();
            
            return new HistoryPlate()
            {
                History = Enumerable.Range(1,res.Count).Select(index=>new HistoryParking()
                {
                    EnterId = res[index - 1].EnterId,
                    Plate = res[index - 1].Plate,
                    Fees = res[index - 1].Fees,
                    InTime = res[index - 1].InTime,
                    OutTime = res[index-1].OutTime,
                    IsMonthly = res[index - 1].IsMonthly
                }),
                Status = true
            };
        }

        [HttpPost]
        public AddPlate AddPlate([FromBody] Plate plate)

        {
            string token = Request.Cookies["token"];
            var sqlcontent = new Mysql();
            var tk = sqlcontent.VerifyToken(token);
            if (!tk.Status)
            {
                return new AddPlate()
                {
                    Status = false,
                    AddSuccess = false,
                    Error = "Error, please login again!"

                };
            }
            var currentTime = DateTime.Now;
            //determine if monthly paid
            var lease = sqlcontent.VerifyLease(plate.PlateNum);
            //Get current pricing
            var pricingInfo = sqlcontent.ExecuteGetPricing(true);
            //Determine earlybird 
            var eb = false;
            if (pricingInfo[0].HaveEarlyBird)
            {
                DateTime ebTime = DateTime.Now.Date.AddHours(10);
                if (currentTime.CompareTo(ebTime) <= 0)
                {
                    eb = true;
                }
            }
            //Determine if already exist
            var exist = sqlcontent.VerifyExist(plate.PlateNum);
            //if the plate is exist, then return false status
            if (exist)
            {
                return new AddPlate
                {
                    AddSuccess = false,
                    Status = true,
                    Error = "The car has already been in the parking lot."
                };
            }
            var timeString = currentTime.ToString("yyyy-MM-dd HH:mm:ss");
            var res = sqlcontent.ExecuteNonQuery($"INSERT INTO `parkinglot`.`plate` (`plate`, `in_time`, `is_earlybird`, `is_leased`, `is_paid`, `pricing_id`) VALUES ('{plate.PlateNum}', '{timeString}', {eb}, {lease}, {lease}, '{pricingInfo[0].PricingId}');");
            if (res == "Success")
            {
                return new AddPlate
                {
                    AddSuccess = true,
                    Status = true
                };
            }

            return new AddPlate
            {
                AddSuccess = false,
                Status = true,
                Error = "Fail to upload plate"
            };
        }

        [HttpPost]
        public ReleasePlate ReleaseCar([FromBody] SearchPlate plateinfo)
        {
            string token = Request.Cookies["token"];
            var sqlcontent = new Mysql();
            var tk = sqlcontent.VerifyToken(token);
            if (!tk.Status)
            {
                return new ReleasePlate
                {
                    Status = false,
                    ReleaseSuccess = false,
                    Error = "Error, please login again!"

                };
            }
            
            var currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var intime = plateinfo.InTime.ToString("yyyy-MM-dd HH:mm:ss");
            var suc=sqlcontent.ExecuteNonQuery(
                $"INSERT INTO `parkinglot`.`history` (`enter_id`,`plate`, `in_time`, `out_time`, `is_leased`,`fees`) VALUES ('{plateinfo.EnterId}','{plateinfo.Plate}', '{intime}', '{currentTime}', {plateinfo.IsMonthly},'{plateinfo.Fees}');");
            if (suc == "Success")
            {
                var suc2 = sqlcontent.ExecuteNonQuery($"UPDATE `parkinglot`.`plate` SET `is_left` = true WHERE (`enter_id` = '{plateinfo.EnterId}');");
                if (suc2 == "Success")
                {
                    return new ReleasePlate
                    {
                        Status = true,
                        ReleaseSuccess = true,
                    };
                }
            }
            return new ReleasePlate
            {
                Status = true,
                ReleaseSuccess = false,
                Error = "Error, please try again!"
            };
        }
        
    }
}