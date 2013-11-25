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

public class SpoofDrinkHandler : IHttpHandler
{
    public SpoofDrinkHandler()
    {
    }
    public void ProcessRequest(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;
        // This handler is called whenever a file ending 
        // in .sample is requested. A file with that extension

        WebApplication2.Controller.setTable(1, 1, "needsRefill");
 

        //string res = d.Tables[0].Rows[0]["needsWaiter"].ToString();

        Response.Write("drink spoofed");
        
    }
    public bool IsReusable
    {
        // To enable pooling, return true here.
        // This keeps the handler in memory.
        get { return false; }
    }
}