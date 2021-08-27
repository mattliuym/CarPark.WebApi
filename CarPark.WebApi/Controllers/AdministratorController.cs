using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CarPark.WebApi.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CarPark.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdministratorController : Controller
    {
        //sign up a new user 
        //para: username,email address, password
        //return:status, result
        [HttpPost]
        public SignUp SignUpAccount([FromBody]SignUpAdmin account)
        {

            Mysql dbcontent = new Mysql();
            SignUp res = new SignUp();
            if (!new EmailAddressAttribute().IsValid(account.Email))
            {
                res.Status = false;
                res.Result = "Please enter correct email address";
                return res;
            }
            var result = dbcontent.ExecuteNonQuery($"INSERT INTO `parkinglot`.`admin_table` (`user_name`, `pwd`, `email`) VALUES ('{account.UserName}', '{account.Pwd}', '{account.Email}');");
            if (result == "Success")
            {
                res.Status = true;
                res.Result = result;
            }
            else
            {
                res.Status = false;
                res.Result = result;
            }
            return res;
        }
        //login
        //
        //
        [HttpPost]
        public LoginStatus LoginAccount([FromBody] Login account)
        {
            
            return null;
        }
    }
}