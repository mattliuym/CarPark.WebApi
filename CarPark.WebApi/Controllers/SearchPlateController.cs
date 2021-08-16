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
        
        /*
         Additional:
         1.Add a field contains current stop time.
         2.Add a field contains the current parking fee.
         */
        public IEnumerable<SearchPlate> GetCarInfo(string s)
        {
            Mysql sqlcontent = new Mysql();
            var res = sqlcontent.ExecuteSearchPlate($"SELECT * FROM parkinglot.plate where plate='{s}';");
            //call when the car is still there.
            if (res[0].isLeft == false)
            {
                //get the parking fee.
                res[0].ObtainFees(res[0].InTime);
            }
            return Enumerable.Range(1,res.Count).Select(index=>new SearchPlate()
            {
                Plate = res[index-1].Plate,
                EnterId = res[index-1].EnterId,
                Fees = res[index-1].Fees,
                InTime = res[index-1].InTime,
                IsEarlyBird = res[index-1].IsEarlyBird,
                IsPaid = res[index-1].IsPaid
            }).ToArray();
        }
       

    }
}