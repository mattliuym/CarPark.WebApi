using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using CarPark.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarPark.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LeaseParkController:ControllerBase
    {
        [HttpGet]
        public LeaseStatus GetAllLease()
        {
            string token = Request.Cookies["token"];
            var sqlcontent = new Mysql();
            var tk = sqlcontent.VerifyToken(token);
            if (!tk.Status)
            {
                return new LeaseStatus()
                {
                    Status = false,
                    Error = "Error, please login again!"
                };
            }

            var res = sqlcontent.ExecuteAllLease();

            return new LeaseStatus()
            {
                Status = true,
                LeaseLists = Enumerable.Range(1, res.Count).Select(index => new LeaseInfo()
                {
                    LeaseId = res[index - 1].LeaseId,
                    Plate = res[index - 1].Plate,
                    Expiry = res[index - 1].Expiry,
                    Valid = res[index - 1].Valid
                })
            };
        }

        [HttpPost]
        public AppendLease AddLease([FromBody] LeaseInfo p)
        {
            var sqlcontent = new Mysql();
            string res;
            string res1;
            if (p.LeaseId == 0)
            {
                res = sqlcontent.ExecuteNonQuery(
                    $"INSERT INTO `parkinglot`.`parking_lease` (`plate`, `expired_date`, `is_valid`) VALUES ('{p.Plate}', '{p.Expiry.ToString("yyyy-MM-dd")}', {p.Valid});");
                res1 = "Success";
            }
            else
            {
                res = sqlcontent.ExecuteNonQuery(
                    $"UPDATE `parkinglot`.`parking_lease` SET `expired_date` = '{p.Expiry.ToString("yyyy-MM-dd")}' WHERE (`lease_id` = {p.LeaseId});");
                res1 = sqlcontent.ExecuteNonQuery(
                    $"UPDATE `parkinglot`.`parking_lease` SET `expired_date` = '{p.Expiry.ToString("yyyy-MM-dd")}', `is_valid` = {p.Valid} WHERE (`lease_id` = {p.LeaseId});");
            }

            if (res == "Success" && res1 =="Success")
            {
                return new AppendLease()
                {
                    Status = true,
                    Public = true
                };
            }
            return new AppendLease()
                {
                    Status = true,
                    Public = false,
                    Error = "Action failed, please retry!"
                };
        }

        [HttpGet]
        public LeaseInfo SearchLease(string p)
        {
            var sqlcontent = new Mysql();
            var res = sqlcontent.ExecuteOneLease($"SELECT * FROM parkinglot.parking_lease WHERE plate='{p}';");
            if (res != null)
            {
                return res[0];
            }
            return new LeaseInfo()
            {
                LeaseId = 0
            };

        }
    }
}