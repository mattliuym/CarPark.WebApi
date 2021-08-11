using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CarPark.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //test the connection of mysql
            //Mysql sqltest = new Mysql();
            //string l = "INSERT INTO `school`.`student` (`id`, `name`,`age`) VALUES ('119', 'nat','32');";
            //sqltest.ExecuteNonQuery(l);
            // string result = sqltest.Execute();
            // Console.WriteLine(result);
            //
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}