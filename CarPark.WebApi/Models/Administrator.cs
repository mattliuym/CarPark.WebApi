
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CarPark.WebApi.Models
{
    public class Administrator
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Pwd { get; set; }
    }

    public class SignUpAdmin
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Pwd { get; set; }
    }

    public class SignUp
    {
        public bool Status { get; set; }
        public string Result { get; set; }
        
    }

    public class Login
    {
        public string UserName { get; set; }
        public string Pwd { get; set; }
    }

    public class LoginInfo
    {
        public string UserName { get; set; }
        public int UserId { get; set; }
        
        public string Pwd { get; set; }
        //public string Token { get; set; }
    }
    
    public class LoginStatus
    {
        public bool Status { get; set; }
        public List<LoginInfo> info { get; set;}
        public DataResult result { get; set; } 
        
    }
}