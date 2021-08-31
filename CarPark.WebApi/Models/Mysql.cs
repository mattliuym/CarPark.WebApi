using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;


namespace CarPark.WebApi.Models
{
    public class Mysql
    {
    	//mysql connection configurations
    	private static string constr = "server=127.0.0.1;port=3306;database=parkinglot;user=root;password=mm970708";
		
		// void type： update or insert or delete
		public string ExecuteNonQuery(string str)
        {
            var con = new MySqlConnection(constr);
            try
            {
            	//Connect to mysql
                con.Open();
                //set the SQL query
                string sql=str; 
                // string sql= "delete from ustudent where gid=@classId"; 
                // string sql= "insert into ustudent(sid, sname, ssexy) VALUE (@sid,@sname,@ssexy);
                MySqlCommand command = new MySqlCommand(sql, con);
                command.ExecuteNonQuery();
                return "Success";
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
            	// Shut down the connection
                con.Close();
            }
        }
        
        //Test the execution of MySQL connections.
        public List<TestWeb> ExecuteTestWebs(string str)
        {
            MySqlConnection con = new MySqlConnection(constr);
            MySqlDataReader dataReader = null;
            List<TestWeb> stuInfoList = null;//return value
            //string stuInfoList = null;
            try
            {
                // connect...
                con.Open();
                // sql statement     here
                string sql = str;
                MySqlCommand command = new MySqlCommand(sql, con);
                // 
                //command.Parameters.Add(new MySqlParameter("@name", name));
                // execute sql command
                dataReader = command.ExecuteReader();

                //get the data and then...
                // if there is data
                if (dataReader != null && dataReader.HasRows)
                {
                    stuInfoList = new List<TestWeb>();
                    //stuInfoList = null;
                    //iterate...
                    while (dataReader.Read())
                    {
                        stuInfoList.Add(new TestWeb(
                           dataReader.GetInt32("id"),
                           dataReader.GetString("name"),
                           dataReader.GetInt32("age")
                            ));
                        //stuInfoList = dataReader.GetString("name");
                    }
                }
                dataReader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.Close();
            }
            // return stuInfoList
            
            return stuInfoList;
        }
        
        //Search plate from db
        public List<SearchPlate> ExecuteSearchPlate(string str)
        { 
            MySqlConnection con = new MySqlConnection(constr);
            MySqlDataReader dataReader = null;
            List<SearchPlate> plateInfo = null;//return value
            try
            {
                con.Open();
                string sql = str;
                MySqlCommand command = new MySqlCommand(sql, con);
                dataReader = command.ExecuteReader();
                if (dataReader != null && dataReader.HasRows)
                {
                    plateInfo = new List<SearchPlate>();
                    while (dataReader.Read())
                    {
                        plateInfo.Add(new SearchPlate()
                        {
                            EnterId = dataReader.GetInt32("enter_id"),
                            Plate = dataReader.GetString("plate"),
                            InTime = dataReader.GetDateTime("in_time"),
                            IsEarlyBird = dataReader.GetBoolean("is_earlybird"),
                            IsPaid = dataReader.GetBoolean("is_paid"),
                            Fees = dataReader.GetDecimal("fees"),
                            isLeft = dataReader.GetBoolean("is_left")
                        });
                        //stuInfoList = dataReader.GetString("name");
                    }
                }
                dataReader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.Close();
            }
            // return stuInfoList
            
            return plateInfo;
            
        }
        //Get pricing info
        public List<Pricing> ExecuteGetPricing()
        { 
            MySqlConnection con = new MySqlConnection(constr);
            MySqlDataReader dataReader = null;
            List<Pricing> pricingInfo = null;//return value
            try
            {
                con.Open();
                string sql = "SELECT * FROM parkinglot.pricing_plans where in_use=1";
                MySqlCommand command = new MySqlCommand(sql, con);
                dataReader = command.ExecuteReader();
                if (dataReader != null && dataReader.HasRows)
                {
                    pricingInfo = new List<Pricing>();
                    while (dataReader.Read())
                    {
                        pricingInfo.Add(new Pricing()
                        {
                            PricingId = dataReader.GetInt32("pricing_id"),
                            PricingName = dataReader.GetString("pricing_name"),
                            TotalPark = dataReader.GetInt32("total_park"),
                            //OpenTime = dataReader.GetString("open_time"),
                            OpenTime = dataReader.IsDBNull("open_time") ? null : dataReader.GetString("open_time"),
                            CloseTime = dataReader.IsDBNull("close_time") ? null : dataReader.GetString("close_time"),
                            IsTwentyFour = dataReader.GetBoolean("is_twentyfour"),
                            IsFlatRate = dataReader.GetBoolean("is_flatrate"),
                            PricePh = dataReader.GetDecimal("price_ph"),
                            HaveEarlyBird = dataReader.GetBoolean("have_earlybird"),
                            EarlyBirdPrice = dataReader.IsDBNull("earlybird_price") ? 0 : dataReader.GetDecimal("earlybird_price"),
                            HaveMax = dataReader.GetBoolean("have_max"),
                            MaxPrice = dataReader.IsDBNull("max_price") ? 0 : dataReader.GetDecimal("max_price"),
                            FreeBefore = dataReader.GetInt32("free_before"),
                            InUse = dataReader.GetBoolean("in_use"),
                            IsMonthly = dataReader.GetBoolean("is_paymonthly"),
                            MonthlyFees = dataReader.IsDBNull("monthly_fee")? 0 : dataReader.GetDecimal("monthly_fee")
                        });
                    }
                }
                dataReader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.Close();
            }
            return pricingInfo;
            
        }

        public LoginStatus VerifyLogin(string userName,string pwd)
        {
            MySqlConnection con = new MySqlConnection(constr);
            MySqlDataReader dataReader = null;
            List<LoginInfo> loginInfo = null;//return value
            DataResult result = new DataResult();
            bool status = false;
            try
            {
                con.Open();
                string sql =$"Select * From parkinglot.admin_table where user_name='{userName}' && pwd='{pwd}' ";
                MySqlCommand command = new MySqlCommand(sql, con);
                dataReader = command.ExecuteReader();
                if (dataReader != null && dataReader.HasRows)
                {
                    status = true;
                    loginInfo = new List<LoginInfo>();
                    while (dataReader.Read())
                    {
                        loginInfo.Add(new LoginInfo()
                        {
                            UserName = dataReader.GetString("user_name"),
                            UserId = dataReader.GetInt32("user_id"),
                            Pwd = dataReader.GetString("pwd")
                        });
                    }
                    var payload = new Dictionary<string, object>
                    {
                        { "username",loginInfo[0].UserName+DateTime.Now},
                        { "pwd", loginInfo[0].Pwd}
                    };
                    result.Token = JwtHelp.SetJwtEncode(payload);
                    result.Success = true;
                    result.Message = "成功";
                }
                else
                {
                    status = false;
                    loginInfo = null;
                    result.Message = "Please Confirm your username or password!";
                    result.Success = false;
                    result.Token = null;
                }
                dataReader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.Close();
            }
            return new LoginStatus()
            {
                Status = status,
                Info=loginInfo,
                Result = result
            };
        }

        public AdministratorStatus VerifyToken(string token)
        {
            MySqlConnection con = new MySqlConnection(constr);
            MySqlDataReader dataReader = null;
            AdministratorStatus verifyStatus = new AdministratorStatus();
            Administrator headerInfo = null;//return value
            try
            {
                con.Open();
                string sql =$"Select * From parkinglot.admin_table where token='{token}' ";
                MySqlCommand command = new MySqlCommand(sql, con);
                dataReader = command.ExecuteReader();
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        headerInfo=(new Administrator()
                        {
                            UserId = dataReader.GetInt32("user_id"),
                            Email = dataReader.GetString("email"),
                            Phone = dataReader.GetString("phone"),
                            Pwd = dataReader.GetString("pwd"),
                            UserName = dataReader.GetString("user_name")
                        });
                    }
                    verifyStatus.Status = true;
                    verifyStatus.User = headerInfo;
                }
                else
                {
                    verifyStatus.Status = false;
                    verifyStatus.Error = "Please check your login status";
                }
                
               
                dataReader.Close();
            }
            catch (Exception e)
            {
                verifyStatus.Status = false;
                verifyStatus.Error = e.Message;
                throw;
            }
            finally
            {
                con.Close();
            }
            return verifyStatus;
        }

    }
}
