using System;

namespace CarPark.WebApi
{
    //接口名，以及接口返回的数据key->value
    public class TestWeb
    {
        public TestWeb(int a,string b,int c)
        {
            this.Sid = a;
            this.Name = b;
            this.Age = c;
        }

        public TestWeb()
        {
            
        }
        // public DateTime Date { get; set; }
        //
        // public int TemperatureC { get; set; }
        //
        // public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        //
        // public string Summary { get; set; }
        //
        // public string Test { get; set; }
        // public string Test => "hello world";
        public int Sid { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}