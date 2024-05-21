using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyProject.Tests.Models
{
    public class ConnectionModel
    {
        public string DBTEST3con()
        {
            //System.Configuration.ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString
            return "Data Source =dbtest3; User ID =ITF0063 ; Password = DJ4KYWYJONL2;";
        }
    }
}