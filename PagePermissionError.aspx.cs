using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NRCAPPS
{
    public partial class PagePermissionError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_NAME"] != null)
            {
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }
    }
}