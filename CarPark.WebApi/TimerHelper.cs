using System;
using CarPark.WebApi.Models;

namespace CarPark.WebApi
{
    public class TimerHelper
    {
        System.Threading.Timer timer;
        private static int interval = 1000*60 * 60;

        public void THelper()
        {
            timer = new System.Threading.Timer(TimerTask, null,0, interval);
        }

        public int Count = 1;
        //Calculate whether monthly payment is due or not.
        public void TimerTask(object state)
        {
            var sqlcontent = new Mysql();
            var currentTime = DateTime.Now;
            var res = sqlcontent.ExecuteAllLease();
            foreach (var t in res)
            {
                if (currentTime.CompareTo(t.Expiry)>0)
                {
                    //expired
                    var renew = sqlcontent.ExecuteNonQuery(
                        $"UPDATE `parkinglot`.`parking_lease` SET `is_valid` = false WHERE (`lease_id` = '{t.LeaseId}');");
                }
              
            }
        }
        
    }
}