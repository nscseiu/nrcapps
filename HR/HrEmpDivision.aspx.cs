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
    public partial class HrEmpDivision : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt;
        int RowCount;

        string IS_PAGE_ACTIVE   = "";
        string IS_ADD_ACTIVE    = "";
        string IS_EDIT_ACTIVE   = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE   = "";  
 
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
                    IS_PAGE_ACTIVE   = dt.Rows[i]["IS_PAGE_ACTIVE"].ToString();
                    IS_ADD_ACTIVE    = dt.Rows[i]["IS_ADD_ACTIVE"].ToString();
                    IS_EDIT_ACTIVE   = dt.Rows[i]["IS_EDIT_ACTIVE"].ToString();
                    IS_DELETE_ACTIVE = dt.Rows[i]["IS_DELETE_ACTIVE"].ToString();
                    IS_VIEW_ACTIVE   = dt.Rows[i]["IS_VIEW_ACTIVE"].ToString();  
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
                else {
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

                    string get_user_id = "select HR_EMP_DIVISIONID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_user_id, conn);
                    int newDivisionID = Int32.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string insert_user = "insert into HR_EMP_DIVISIONS (DIVISION_ID, DIVISION_NAME, DIV_SHORT_NAME, DIVISION_ADD, DIV_BG_COLOR, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES ( :NoDivisionID, :TextDivisionName, :TextDivShortName, :TextDivisionAdd, :TextDivBgColor, :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[8];
                    objPrm[0] = cmdi.Parameters.Add("NoDivisionID", newDivisionID);
                    objPrm[1] = cmdi.Parameters.Add("TextDivisionName", TextDivisionName.Text);
                    objPrm[2] = cmdi.Parameters.Add("TextDivShortName", TextDivShortName.Text);
                    objPrm[3] = cmdi.Parameters.Add("TextDivisionAdd", TextDivisionAdd.Text);
                    objPrm[4] = cmdi.Parameters.Add("TextDivBgColor", TextDivBgColor.Text);
                    objPrm[5] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[6] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[7] = cmdi.Parameters.Add("NoCuserID", userID);
                     
                    cmdi.ExecuteNonQuery(); 
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Division Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                    clearText();
                    Display();
                }
                else { 
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
             Session["user_page_data_id"] = btn.CommandArgument;
             int USER_DATA_ID = Convert.ToInt32(Session["user_page_data_id"]); 
             

             DataTable dtUserTypeID = new DataTable();
             DataSet ds = new DataSet();
             string makeSQL = " select *  from HR_EMP_DIVISIONS where DIVISION_ID  = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextDivisionID.Text   = dt.Rows[i]["DIVISION_ID"].ToString();
                 TextDivisionName.Text = dt.Rows[i]["DIVISION_NAME"].ToString();
                 TextDivShortName.Text = dt.Rows[i]["DIV_SHORT_NAME"].ToString();
                 TextDivisionAdd.Text  = dt.Rows[i]["DIVISION_ADD"].ToString();
                 TextDivBgColor.Text   = dt.Rows[i]["DIV_BG_COLOR"].ToString(); 
                 CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                         
             } 
             
             conn.Close();
             Display();
             CheckUserDivisionName.Text = "";
             CheckDivisionShortName.Text = "";
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
                    makeSQL = " select  * from HR_EMP_DIVISIONS ORDER BY UPDATE_DATE desc, CREATE_DATE desc";
                }
                else
                {
                    makeSQL = " select  * from HR_EMP_DIVISIONS where DIVISION_ID like '" + txtSearchUserRole.Text + "%' or DIVISION_NAME like '" + txtSearchUserRole.Text + "%' or IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY UPDATE_DATE desc, CREATE_DATE desc";

                    alert_box.Visible = false;
                }


                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "DIVISION_ID" };

                GridView1.DataBind();
                conn.Close();
                //alert_box.Visible = false;
            }
            else
            {
             //   Response.Redirect("~/PagePermissionError.aspx");
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
                int USER_DATA_ID = Convert.ToInt32(TextDivisionID.Text);   
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                 
                string update_user = "update  HR_EMP_DIVISIONS  set DIVISION_NAME = :TextDivisionName, DIV_SHORT_NAME = :TextDivShortName,  DIVISION_ADD = :TextDivisionAdd, DIV_BG_COLOR = :TextDivBgColor, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where DIVISION_ID = :NoDivisionID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[8];
                objPrm[0] = cmdi.Parameters.Add("TextDivisionName", TextDivisionName.Text);
                objPrm[1] = cmdi.Parameters.Add("TextDivShortName", TextDivShortName.Text);
                objPrm[2] = cmdi.Parameters.Add("TextDivisionAdd", TextDivisionAdd.Text);
                objPrm[3] = cmdi.Parameters.Add("TextDivBgColor", TextDivBgColor.Text); 
                objPrm[4] = cmdi.Parameters.Add("u_date", u_date); 
                objPrm[5] = cmdi.Parameters.Add("NoDivisionID", USER_DATA_ID);
                objPrm[6] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[7] = cmdi.Parameters.Add("TextIsActive", ISActive);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();  

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Division Data Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                Display();
            }
                else { 
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

                int USER_DATA_ID = Convert.ToInt32(TextDivisionID.Text);
                string delete_user_page = " delete from HR_EMP_DIVISIONS where DIVISION_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user_page, conn); 
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Division Data Delete Successfully"));
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
            TextDivisionID.Text = "";
            TextDivisionName.Text = "";
            TextDivShortName.Text = "";
            TextDivisionAdd.Text = "";
            TextDivBgColor.Text = "";
            CheckUserDivisionName.Text = "";
            CheckDivisionShortName.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            
        }

        public void clearText()
        {
            TextDivisionID.Text = "";
            TextDivisionName.Text = "";
            TextDivShortName.Text = "";
            TextDivisionAdd.Text = "";
            TextDivBgColor.Text = "";
            CheckUserDivisionName.Text = "";
            CheckDivisionShortName.Text = "";
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

        public void TextDivisionName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextDivisionName.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from HR_EMP_DIVISIONS where DIVISION_NAME = '" + TextDivisionName.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckUserDivisionName.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Division name is already entry</label>";
                    CheckUserDivisionName.ForeColor = System.Drawing.Color.Red;
                    TextDivisionName.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckUserDivisionName.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Division name is available</label>";
                    CheckUserDivisionName.ForeColor = System.Drawing.Color.Green;
                    CheckIsActive.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else {
                    CheckUserDivisionName.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Division name is not blank</label>";
                    CheckUserDivisionName.ForeColor = System.Drawing.Color.Red;
                    TextDivisionName.Focus();
            }
            
        } 

        public void TextDivisionShortName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextDivShortName.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from HR_EMP_DIVISIONS where DIV_SHORT_NAME = '" + TextDivShortName.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckDivisionShortName.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Division Short name is already entry</label>";
                    CheckDivisionShortName.ForeColor = System.Drawing.Color.Red;
                    TextDivShortName.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckDivisionShortName.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Division Short name is available</label>";
                    CheckDivisionShortName.ForeColor = System.Drawing.Color.Green;
                    CheckIsActive.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else {
                    CheckDivisionShortName.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Division Short name is not blank</label>";
                    CheckDivisionShortName.ForeColor = System.Drawing.Color.Red;
                    TextDivisionName.Focus();
            }
            
        } 

   }
}