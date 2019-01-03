using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EthnoBot.Models
{
    public class Helpers
    {
        public static string GetRDSConnectionString()
        {
            var appConfig = ConfigurationManager.AppSettings;

            string dbname = "ethnobotanicaldatabase";

            if (string.IsNullOrEmpty(dbname)) return null;

            string username = "root";
            string password = "ahcaameE1!!!!";
            string hostname = "mssqlinstance.czix1f7pd6zi.eu-west-1.rds.amazonaws.com,1433";
            string port = appConfig["RDS_PORT"];

            return "Data Source=" + hostname + ";Initial Catalog=" + dbname + ";User ID=" + username + ";Password=" + password + ";";
        }
    }
}