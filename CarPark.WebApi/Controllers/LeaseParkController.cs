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
            var res = sqlcontent.ExecuteNonQuery(
                $"INSERT INTO `parkinglot`.`parking_lease` (`plate`, `expired_date`, `is_valid`) VALUES ('{p.Plate}', '{p.Expiry.ToString("yyyy-MM-dd")}', {p.Valid});");
            if (res == "Success")
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
                    Error = "Fail to action, please retry!"
                };
        }
    }
}