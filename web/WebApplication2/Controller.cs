using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient; // must add this...
using System.Data; // must add this...

namespace WebApplication2
{
    public class Controller
    {
        public static void emptyPlate(int tableID)
        {
            //string connection = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            string connection = "Server=tcp:y71sqpp01v.database.windows.net,1433;Database=ubiquidine;User ID=superuser@y71sqpp01v;Password=password!1234;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

            string query = "UPDATE tables SET emptyplate=1 WHERE tableID=" + tableID;

            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

            DataSet data = new DataSet();

            adapter.Fill(data);
        }

        public static void emptyDrink(int tableID)
        {
            //string connection = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            string connection = "Server=tcp:y71sqpp01v.database.windows.net,1433;Database=ubiquidine;User ID=superuser@y71sqpp01v;Password=password!1234;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

            string query = "UPDATE tables SET needsrefill=1 WHERE tableID=" + tableID;

            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

            DataSet data = new DataSet();

            adapter.Fill(data);
        }



        public static void waiterCalled(int tableID)
        {
            //string connection = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

            //string connection = "Data Source=JON-DESKTOP;Initial Catalog=ubiquidine;Integrated Security=True";

            string connection = "Server=tcp:y71sqpp01v.database.windows.net,1433;Database=ubiquidine;User ID=superuser@y71sqpp01v;Password=password!1234;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;"; 

            string query = "UPDATE tables SET needswaiter=1 WHERE tableID=" + tableID;

            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

            DataSet data = new DataSet();

            adapter.Fill(data);
        }
    }
}