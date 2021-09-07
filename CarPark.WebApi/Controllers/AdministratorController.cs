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
    public class AdministratorController : ControllerBase
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
            var result = dbcontent.ExecuteNonQuery($"INSERT INTO `parkinglot`.`admin_table` (`user_name`, `pwd`, `email`,`phone`) VALUES ('{account.UserName}', '{MD5Cryption.MD5Hash(account.Pwd)}', '{account.Email}','{account.Phone}');");
            if (result == "Success")
            {
                res.Status = true;
                res.Result = result;
            }
            else
            {
                res.Status = false;
                if (result == $"Duplicate entry '{account.UserName}' for key 'admin_table.user_name_UNIQUE'")
                {
                    res.Result = "Your username has been registered, please change your username!";
                }
                else if (result ==$"Duplicate entry '{account.Email}' for key 'admin_table.email_UNIQUE'" )
                {
                    res.Result = "Your email has been registered, please use another email address";
                }
                else
                {
                    res.Result = result;
                }

            }
            return res;
        }
        //login
        //
        //
        [HttpPost]
        public LoginStatus LoginAccount([FromBody] Login account)
        {
            Mysql sqlContent = new Mysql();
            var res = sqlContent.VerifyLogin(account.UserName, MD5Cryption.MD5Hash(account.Pwd));//verify user's account and get token
            if (res.Status)
            {
                //save token 
                var tokenStatus=sqlContent.ExecuteNonQuery(
                    $"UPDATE `parkinglot`.`admin_table` SET `token` = '{res.Result.Token}' WHERE (`user_id` = '{res.Info[0].UserId}');"); 
                if (tokenStatus == "Success")
                {
                    //if get token successfully, then return true result
                    return res;
                }
                //err message
                return new LoginStatus()
                        {
                            Status = false,
                            Info = null,
                            Result = new DataResult()
                            {
                                Token = null,
                                Success = false,
                                Message = "Unsuccessful to set token, please login again"
                            }
                        };
            }
           
            return res;
        }
    }
}