using Microsoft.AspNetCore.Mvc;
using CarPark.WebApi.Models;
using System.Linq;
using MySql.Data.MySqlClient;

namespace CarPark.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TransactionController:ControllerBase
    {
        [HttpPost]
        public TransactionStatus ChangePayStatus([FromBody] SearchPlate plate)
        {
            var sqlcontent = new Mysql();
            var res = sqlcontent.ExecuteNonQuery(
                $"UPDATE `parkinglot`.`plate` SET `is_paid` = '1' WHERE (`enter_id` = '{plate.EnterId}');");
            if (res == "Success")
            {
                return new TransactionStatus()
                {
                    Status=true
                };
            }

            return new TransactionStatus()
            {
                Status = false,
                Error = res
            };
        }
    }

    public class TransactionStatus
    {
        public bool Status { get; set; }
        public string Error { get; set; }
    }
}