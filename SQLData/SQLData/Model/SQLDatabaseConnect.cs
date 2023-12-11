using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace SQLData.Model
{
    public class SQLDatabaseConnect 
    {

        #region public methods

        public static SqlConnection connect;

        public void ConnectDataBase()
        {
            // connection string
            connect = new SqlConnection("Data Source=DESKTOP-0O3AQVO\\MSSQL19;Initial Catalog=RevitData;Integrated Security=True");
                      
            // Open connection Database
            connect.Open();
        }

        public SqlCommand Query(string sqlQuery)
        {
            SqlCommand Command = new SqlCommand(sqlQuery, connect);
            return Command;
        }

        #endregion
    }
}
