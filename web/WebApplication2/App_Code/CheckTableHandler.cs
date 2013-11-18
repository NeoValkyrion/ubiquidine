using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient; // must add this...
using System.Data; // must add this...
using WebApplication2; 

public class CheckTableHandler : IHttpHandler
{
    public CheckTableHandler()
    {
    }
    public void ProcessRequest(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;
        // This handler is called whenever a file ending 
        // in .sample is requested. A file with that extension

        DataSet d = WebApplication2.Controller.checkTable(1);

        string drink = d.Tables[0].Rows[0]["needsRefill"].ToString();
        string waiter = d.Tables[0].Rows[0]["needsWaiter"].ToString();
        string plate = d.Tables[0].Rows[0]["emptyPlate"].ToString();

        Response.Write("needs refill: " + drink + " needs waiter: " + waiter + " needs plate: " + plate); 
        
    }
    public bool IsReusable
    {
        // To enable pooling, return true here.
        // This keeps the handler in memory.
        get { return false; }
    }
}