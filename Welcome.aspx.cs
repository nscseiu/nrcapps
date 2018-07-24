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

namespace NRCAPPS
{
    public partial class Welcome : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_NAME"] != null)
            {
                lb1.Text = "<b><font color=Brown>" + "WELLCOME :: " + "</font>" + "<b><font color=red>" + Session["USER_NAME"] + " User ID:" + Session["USER_ID"] + "</font>";
            }
            else {
                Response.Redirect("~/Default.aspx");
            }
           
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Session.Remove("USER_NAME");
            Response.Redirect("~/Default.aspx");
        }
    }
}
