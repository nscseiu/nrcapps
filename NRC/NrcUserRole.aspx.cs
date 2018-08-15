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
    public partial class NrcUserRole : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
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

                if (IS_PAGE_ACTIVE == "Enable")
                {
                     
                    if (!IsPostBack)
                    {

                        Display();
                        alert_box.Visible = false;

                    } 
                    IsLoad = false;
                }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
                   
                 
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        public void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
               if (IS_ADD_ACTIVE == "Enable")
                {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                int userID = Convert.ToInt32(Session["USER_ID"]); 

                string get_user_id = "select NRC_USER_ROLE_SEQ.nextval from dual";
                cmdu = new OracleCommand(get_user_id, conn);
                int newUserID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string insert_user = "insert into NRC_USER_ROLE (USER_ROLE_ID, USER_ROLE_NAME, USER_ROLE_SHORT_NAME, UR_BG_COLOR, CREATE_DATE, C_USER_ID, IS_ACTIVE) values ( :NoUserID, :TextUserRoleName, :TextUserRoleSname, :TextRoleBgColor, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive)";
                cmdi = new OracleCommand(insert_user, conn);
                 
                OracleParameter[] objPrm = new OracleParameter[7];
                objPrm[0] = cmdi.Parameters.Add("NoUserID", newUserID);
                objPrm[1] = cmdi.Parameters.Add("TextUserRoleName", TextUserRoleName.Text);
                objPrm[2] = cmdi.Parameters.Add("TextUserRoleSname", TextUserRoleSname.Text);
                objPrm[3] = cmdi.Parameters.Add("TextRoleBgColor", TextRoleBgColor.Text); 
                objPrm[4] = cmdi.Parameters.Add("u_date", u_date);  
                objPrm[5] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[6] = cmdi.Parameters.Add("TextIsActive", ISActive);

                cmdi.ExecuteNonQuery();
 
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Insert New User Role successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                Display();
               }
               else
               {
                   Response.Redirect("~/PagePermissionError.aspx");
               }
            }
            catch
            {
                Response.Redirect("~/ParameterError.aspx");
            }
        }

        protected void linkSelectClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             LinkButton btn = (LinkButton)sender;
             Session["user_data_id"] = btn.CommandArgument;
             int USER_DATA_ID = Convert.ToInt32(Session["user_data_id"]); 
             

             DataTable dtUserTypeID = new DataTable();
             DataSet ds = new DataSet();
             string makeSQL = " select *  from NRC_USER_ROLE where USER_ROLE_ID  = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextUserRoleID.Text     = dt.Rows[i]["USER_ROLE_ID"].ToString();
                 TextUserRoleName.Text   = dt.Rows[i]["USER_ROLE_NAME"].ToString();
                 TextUserRoleSname.Text  = dt.Rows[i]["USER_ROLE_SHORT_NAME"].ToString();
                 TextRoleBgColor.Text    = dt.Rows[i]["UR_BG_COLOR"].ToString(); 
                 CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);

                      
             } 
             
             conn.Close();
             Display();
             CheckUserRoleSname.Text = "";
             alert_box.Visible = false;
             BtnAdd.Attributes.Add("aria-disabled", "false");
             BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 

        }

        public void Display()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                DataTable dtUserTypeID = new DataTable();
                DataSet ds = new DataSet();

                string makeSQL = "";
                if (txtSearchUserRole.Text == "")
                {
                    makeSQL = " select  * from NRC_USER_ROLE ORDER BY UPDATE_DATE desc, CREATE_DATE desc";
                }
                else
                {
                    makeSQL = " select  * from NRC_USER_ROLE where USER_ROLE_NAME like '" + txtSearchUserRole.Text + "%' or USER_ROLE_SHORT_NAME like '" + txtSearchUserRole.Text + "%' or IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY UPDATE_DATE desc, CREATE_DATE desc";

                    alert_box.Visible = false;
                }


                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "USER_ROLE_ID" };

                GridView1.DataBind();
                conn.Close();
                //alert_box.Visible = false;
            }
        }

        protected void GridViewSearchUser(object sender, EventArgs e)
        {
            this.Display();
        }
 
         protected void GridViewUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            Display();
            alert_box.Visible = false;
        }
 

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {  
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int USER_DATA_ID = Convert.ToInt32(TextUserRoleID.Text);   
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");



                string update_user = "update  NRC_USER_ROLE  set USER_ROLE_NAME = :TextUserRoleName, USER_ROLE_SHORT_NAME = :TextUsRoleName, UR_BG_COLOR = :TextRoleBgColor, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where USER_ROLE_ID = :NoUserID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[7];
                objPrm[0] = cmdi.Parameters.Add("TextUserRoleName", TextUserRoleName.Text);
                objPrm[1] = cmdi.Parameters.Add("TextUsRoleName", TextUserRoleSname.Text);
                objPrm[2] = cmdi.Parameters.Add("TextRoleBgColor", TextRoleBgColor.Text); 
                objPrm[3] = cmdi.Parameters.Add("u_date", u_date); 
                objPrm[4] = cmdi.Parameters.Add("NoUserID", USER_DATA_ID);
                objPrm[5] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[6] = cmdi.Parameters.Add("TextIsActive", ISActive);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();  

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("User Role Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                Display();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        


        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (IS_DELETE_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int USER_DATA_ID = Convert.ToInt32(TextUserRoleID.Text);
                    string delete_user = " delete from NRC_USER_ROLE where USER_ROLE_ID  = '" + USER_DATA_ID + "'";

                    cmdi = new OracleCommand(delete_user, conn);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("User Role Data Delete successfully"));
                    alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                    clearText();
                    Display();
                }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
            }
            catch
            {
                Response.Redirect("~/ParameterError.aspx");
            }
        }

        public void clearTextField(object sender, EventArgs e)
        {
            TextUserRoleID.Text         = "";
            TextUserRoleName.Text       = "";
            TextUserRoleSname.Text      = "";
            TextRoleBgColor.Text        = "";  
            CheckUserRoleSname.Text     = ""; 
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            
        }

        public void clearText()
        {
            TextUserRoleID.Text    = "";
            TextUserRoleName.Text  = "";
            TextUserRoleSname.Text    = "";
            TextRoleBgColor.Text   = "";
            CheckUserRoleSname.Text = "";  
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 

        }

        public DataSet ExecuteBySqlStringUserType(string sqlString)
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            DataSet ds = new DataSet();
            OracleConnection conn = new OracleConnection(connStr);
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(sqlString, conn);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlString;
                bool mustCloseConnection = false;
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    da.Fill(ds);
                    cmd.Parameters.Clear();
                    if (mustCloseConnection)
                    {
                        conn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }

        public void TextUserRoleSName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextUserRoleSname.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from NRC_USER_ROLE where USER_ROLE_SHORT_NAME = '" + TextUserRoleSname.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckUserRoleSname.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> User Role Short Name is not available</label>";
                    CheckUserRoleSname.ForeColor = System.Drawing.Color.Red;
                    TextUserRoleName.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckUserRoleSname.Text = "<label class='control-label'><i class='fa fa fa-check'></i> User Role Short Name is available</label>";
                    CheckUserRoleSname.ForeColor = System.Drawing.Color.Green;
                    TextUserRoleSname.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else {
                    CheckUserRoleSname.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> User Role Short Name is not blank</label>";
                    CheckUserRoleSname.ForeColor = System.Drawing.Color.Red;
                    TextUserRoleName.Focus();
            }
            
        } 
   }
}