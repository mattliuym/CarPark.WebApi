using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarPark.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CarPark.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestWebController:ControllerBase
    {

        [HttpGet]
        // public IActionResult Login()
        // {
        //     TestWeb re = new TestWeb();
        //     re.Result = "诶嘿嘿";
        //     TestWeb re2 = new TestWeb();
        //     re.Result = "haha";
        //     ArrayList arr = new ArrayList();
        //     arr.Add(re);
        //     arr.Add(re2);
        //     
        //     return arr;
        // }
        public IEnumerable<TestWeb> Get()
        {
            Mysql sqltest = new Mysql();
            var str = sqltest.ExecuteTestWebs("SELECT * FROM school.student where id=119;");

            return Enumerable.Range(1, str.Count).Select(index => new TestWeb()
            {
                Sid = str[index-1].Sid,
                Age = str[index-1].Age,
                Name = str[index-1].Name
            }).ToArray();
        }
    }
}