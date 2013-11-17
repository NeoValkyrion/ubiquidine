using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;

namespace WebApplication2
{
    public partial class _Default : System.Web.UI.Page
    {

        private SqlConnection _connection;
        private SqlCommand _command;
        private SqlDataReader _reader;

        protected void Page_Load(object sender, EventArgs e) 
        { 
            if (!IsPostBack)
                {
                    // Hook PreRenderComplete event for data binding
                    this.PreRenderComplete += new EventHandler(Page_PreRenderComplete);

                    // Register async methods
                    AddOnPreRenderCompleteAsync(
                        new BeginEventHandler(BeginAsyncOperation),
                        new EndEventHandler(EndAsyncOperation)
                    );
                }
        }

        IAsyncResult BeginAsyncOperation(object sender, EventArgs e, AsyncCallback cb, object state)
        {
            string connect = WebConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            _connection = new SqlConnection(connect);
            _connection.Open();
            _command = new SqlCommand("SELECT * from tables", _connection);
            return _command.BeginExecuteReader(cb, state);
        }
        
        void EndAsyncOperation(IAsyncResult ar)
        {
            _reader = _command.EndExecuteReader(ar);
        }

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            GridView2.DataSource = _reader;
            GridView2.DataBind();
        }
    }

    /*
        protected void Page_Load(object sender, EventArgs e)
        {
             //string connection = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

             //SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM tables", connection);

             ///DataSet myDS = new DataSet();
             //adapter.Fill(myDS);

            DataSet myDS = new DataSet();

            runSelectQuery("SELECT * FROM tables", myDS); 

             TextBox1.Text = myDS.Tables[0].Rows[0]["tableID"].ToString();
             //TextBox2.Text = myDS.Tables[0].Rows[0]["calledWaiter"].ToString();
             //TextBox3.Text = myDS.Tables[0].Rows[0]["emptyPlate"].ToString();
             //TextBox4.Text = myDS.Tables[0].Rows[0]["emptyDrink"].ToString();

             if (myDS.Tables[0].Rows[0]["needsWaiter"].ToString().Equals("1"))
             {
                 theexclam.Visible = true;
             }

             if (myDS.Tables[0].Rows[0]["emptyPlate"].ToString().Equals("1"))
             {
                 theplate.Visible = true;
             }

             if (myDS.Tables[0].Rows[0]["needsRefill"].ToString().Equals("1"))
             {
                 thecup.Visible = true;
             }

        }

        


        // pass a SQL query + a dataset to fill
        private void runSelectQuery(string query, DataSet myDS)
        {
            try
            {
                //string connection = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

                string connection = "Server=tcp:y71sqpp01v.database.windows.net,1433;Database=ubiquidine;User ID=superuser@y71sqpp01v;Password=password!1234;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                //DataSet myDS = new DataSet();

                adapter.Fill(myDS);

                //return adapter; 
            }
            catch (Exception e)
            {
                Console.Write(e.Message); 
            }
        }
     */
}