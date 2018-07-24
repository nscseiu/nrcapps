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



namespace NRCAPPS.NRC
{
    public partial class NrcUserChangePassword : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdi, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt;
        int RowCount;

        string IS_PAGE_ACTIVE = "";
        string IS_ADD_ACTIVE = "";
        string IS_EDIT_ACTIVE = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE = "";  
 
        public bool IsLoad { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_NAME"] != null)
            {
                string requestedFile = Path.GetFileName(Request.Path);
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = " SELECT  NUPP.IS_PAGE_ACTIVE, NUPP.IS_ADD_ACTIVE, NUPP.IS_EDIT_ACTIVE, NUPP.IS_DELETE_ACTIVE, NUPP.IS_VIEW_ACTIVE FROM NRC_USER_PAGE_PERMISSION NUPP LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NUPP.USER_PAGE_ID  WHERE NUPP.USER_ID = '" + Session["USER_ID"] + "' AND NUP.IS_ACTIVE = 'Enable' AND NUP.PAGE_URL = '" + requestedFile + "' ";
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                for (int i = 0; i < RowCount; i++)
                {
                    IS_PAGE_ACTIVE = dt.Rows[i]["IS_PAGE_ACTIVE"].ToString();
                    IS_ADD_ACTIVE = dt.Rows[i]["IS_ADD_ACTIVE"].ToString();
                    IS_EDIT_ACTIVE = dt.Rows[i]["IS_EDIT_ACTIVE"].ToString();
                    IS_DELETE_ACTIVE = dt.Rows[i]["IS_DELETE_ACTIVE"].ToString();
                    IS_VIEW_ACTIVE = dt.Rows[i]["IS_VIEW_ACTIVE"].ToString();
                }

            //    if (IS_PAGE_ACTIVE == "Enable")
            //    {
                      if (!IsPostBack)
                    {
                        TextPassword.Focus();  
                        TextPassword.TextMode = TextBoxMode.Password; 
                        TextPasswordConfirm.Text = "";

                        alert_box.Visible = false;

                    } IsLoad = false;
           //     }
             //   else
             //   {
              //      Response.Redirect("~/PagePermissionError.aspx");
            //    }
                   
                 
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }
         
        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
       //  if (IS_EDIT_ACTIVE == "Enable")
       //     {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);  
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string pass_add = "1234567891234560";

                OracleCommand cmdep = new OracleCommand();
                cmdep.Connection = conn;
                cmdep.CommandText = " SELECT get_enc_val ('" + TextPassword.Text + "','" + pass_add + "') FROM dual";
                cmdep.CommandType = CommandType.Text;
                OracleDataReader dr = cmdep.ExecuteReader();

                string EncryptoPassword = "";
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                    {
                        EncryptoPassword = dr.GetString(0);
                    }
                }

                string update_user = "update  NRC_USER  set PASSWORD = :TextPassword, IS_PASSWORD_CHANGE = :NoIsPasswordChange , UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID where USER_ID = :NoUserID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[5];  
                objPrm[0] = cmdi.Parameters.Add("TextPassword", EncryptoPassword);
                objPrm[1] = cmdi.Parameters.Add("NoIsPasswordChange", 1);
                objPrm[2] = cmdi.Parameters.Add("u_date", u_date); 
                objPrm[3] = cmdi.Parameters.Add("NoUserID", userID);
                objPrm[4] = cmdi.Parameters.Add("NoC_USER_ID", userID); 

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();  

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Password Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
              
                Response.Redirect("~/Default.aspx"); 
              //  Response.AddHeader("REFRESH", "5;URL=Default.aspx");
       //     }
      //   else
       //  {
          //   Response.Redirect("~/PagePermissionError.aspx");
      //   }
        }

         
        public void clearTextField(object sender, EventArgs e)
        {
             
            TextPassword.Text       = "";
           
            
        }

        public void clearText()
        {
           
            TextPassword.Text = "";
           

        }
         
       
   }
}