using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using CarPark.WebApi.Models;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace CarPark.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HeaderController : ControllerBase
    {
        [HttpGet]
        public AdministratorStatus GetToken()
        {
            //get token cookie from request's header
            string token = Request.Cookies["token"];
            Mysql sqlContent = new Mysql();
            var res = sqlContent.VerifyToken(token);
            return res;
        }
    }
}