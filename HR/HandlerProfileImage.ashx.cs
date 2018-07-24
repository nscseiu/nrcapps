using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.OracleClient;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient; 

namespace NRCAPPS.HR
{
    /// <summary>
    /// Summary description for HandlerProfileImage
    /// </summary>
    public class HandlerProfileImage : IHttpHandler
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; 

        public void ProcessRequest(HttpContext context)
        {
          // context.Response.ContentType = "text/plain";
          // context.Response.Write("Hello World");
           OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            HttpRequest request = context.Request;
            int userID = Convert.ToInt32(context.Request.QueryString["id"]);

             OracleCommand com = conn.CreateCommand();
             com.CommandText = "SELECT IMAGE_DATA FROM NRC_USER WHERE USER_ID = '" + userID + "'";
             OracleDataReader img = com.ExecuteReader();  
            img.Read();
            context.Response.ContentType = "image/jpeg";
            var myImage = (byte[])img[img.GetOrdinal("IMAGE_DATA")]; 
            context.Response.BinaryWrite(myImage); 
            img.Close();
            conn.Close();    
        }

    
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}