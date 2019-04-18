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
 
namespace NRCAPPS.MF
{
    public partial class MfFurnaces : System.Web.UI.Page
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
                          
                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled");

                       
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
                    string ISActive   = CheckIsActive.Checked ? "Enable" : "Disable";  
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string get_id = "select MF_PRODUCTION_FURNACES_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_id, conn);
                    int newFurnacesID = Int16.Parse(cmdu.ExecuteScalar().ToString());
                     
                    string insert_user = "insert into MF_PRODUCTION_FURNACES (FURNACES_ID, FURNACES_NAME, FURNACES_DESCRIPTION, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES ( :NoFurnacesID, :TextFurnacesName, :TextDescription,  :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[6];
                    objPrm[0] = cmdi.Parameters.Add("NoFurnacesID", newFurnacesID);
                    objPrm[1] = cmdi.Parameters.Add("TextFurnacesName", TextFurnacesName.Text);
                    objPrm[2] = cmdi.Parameters.Add("TextDescription", TextDescription.Text); 
                    objPrm[3] = cmdi.Parameters.Add("TextIsActive", ISActive); 
                    objPrm[4] = cmdi.Parameters.Add("u_date", c_date);
                    objPrm[5] = cmdi.Parameters.Add("NoCuserID", userID);


                    cmdi.ExecuteNonQuery();

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Furnaces Successfully"));
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
              
             string makeSQL = " select *  from MF_PRODUCTION_FURNACES where FURNACES_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextFurnacesID.Text = dt.Rows[i]["FURNACES_ID"].ToString();
                 TextFurnacesName.Text = dt.Rows[i]["FURNACES_NAME"].ToString();
                 TextDescription.Text = dt.Rows[i]["FURNACES_DESCRIPTION"].ToString(); 
                 CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false); 
            } 
             
             conn.Close();
             Display();
             CheckFurnacesName.Text = "";
             alert_box.Visible = false;
             BtnAdd.Attributes.Add("aria-disabled", "false");
             BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
             BtnUpdate.Attributes.Add("aria-disabled", "false");
             BtnUpdate.Attributes.Add("class", "btn btn-success active");
             BtnDelete.Attributes.Add("aria-disabled", "false");
             BtnDelete.Attributes.Add("class", "btn btn-danger active");

        }


        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int USER_DATA_ID = Convert.ToInt32(TextFurnacesID.Text); 
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable"; 
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  MF_PRODUCTION_FURNACES  set FURNACES_NAME = :TextFurnacesName, FURNACES_DESCRIPTION = :TextDescription, IS_ACTIVE = :TextIsActive, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID where FURNACES_ID = :NoFurnacesID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[6];
                objPrm[0] = cmdi.Parameters.Add("TextFurnacesName", TextFurnacesName.Text);
                objPrm[1] = cmdi.Parameters.Add("TextDescription", TextDescription.Text);
                objPrm[2] = cmdi.Parameters.Add("TextIsActive", ISActive); 
                objPrm[3] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[4] = cmdi.Parameters.Add("NoFurnacesID", USER_DATA_ID);
                objPrm[5] = cmdi.Parameters.Add("NoC_USER_ID", userID);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Furnaces Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearText();
                Display();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        public void Display()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                  
                string makeSQL = "";
                if (txtSearchUserRole.Text == "")
                {
                    makeSQL = " select  MI.* from MF_PRODUCTION_FURNACES MI  ORDER BY MI.FURNACES_NAME, MI.UPDATE_DATE desc, MI.CREATE_DATE desc";
                }
                else
                {
                    makeSQL = " select  MI.* from MF_PRODUCTION_FURNACES MI where MI.FURNACES_ID like '" + txtSearchUserRole.Text + "%' or MI.FURNACES_NAME like '" + txtSearchUserRole.Text + "%'  or MI.IS_ACTIVE like '" + txtSearchUserRole.Text + "%'  ORDER BY MI.FURNACES_NAME, MI.UPDATE_DATE desc, MI.CREATE_DATE desc ";

                    alert_box.Visible = false;
                }
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "FURNACES_ID" }; 
                GridView1.DataBind();
                
                conn.Close();
                //alert_box.Visible = false;
            }
            else
            {
             //  Response.Redirect("~/PagePermissionError.aspx");
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

     
         
        protected void BtnDelete_Click(object sender, EventArgs e)
        { 
          try
           {
            if (IS_DELETE_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                int USER_DATA_ID = Convert.ToInt32(TextFurnacesID.Text);
                string delete_user_page = " delete from MF_PRODUCTION_FURNACES where FURNACES_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Furnaces Delete Successfully"));
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
            TextFurnacesID.Text = "";
            TextFurnacesName.Text = "";
            TextDescription.Text = ""; 
            CheckFurnacesName.Text = "";  
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            
        }

        public void clearText()
        {
            TextFurnacesID.Text = "";
            TextFurnacesName.Text = "";
            TextDescription.Text = ""; 
            CheckFurnacesName.Text = ""; 
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 

        }
         

        public void TextFurnacesName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextFurnacesName.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from MF_PRODUCTION_FURNACES where FURNACES_NAME = '" + TextFurnacesName.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckFurnacesName.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Furnaces name is already entry</label>";
                    CheckFurnacesName.ForeColor = System.Drawing.Color.Red;
                    TextFurnacesName.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckFurnacesName.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Furnaces name is available</label>";
                    CheckFurnacesName.ForeColor = System.Drawing.Color.Green;
                    TextDescription.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else {
                    CheckFurnacesName.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Furnaces name is not blank</label>";
                    CheckFurnacesName.ForeColor = System.Drawing.Color.Red;
                    TextFurnacesName.Focus();
            }
            
        }

         
        public DataSet ExecuteBySqlString(string sqlString)
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

    }
}