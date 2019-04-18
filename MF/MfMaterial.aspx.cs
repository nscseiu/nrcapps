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
    public partial class MfMaterial : System.Web.UI.Page
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
                        TextMaterialName.Focus();
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
                    string get_customer_id = "select MF_MATERIAL_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_customer_id, conn);
                    int newMaterialID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive   = CheckIsActive.Checked ? "Enable" : "Disable";
                    string IsRmActive = CheckIsRmActive.Checked ? "Enable" : "Disable";
                    string IsFgActive = CheckIsFgActive.Checked ? "Enable" : "Disable";
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string insert_user = "insert into MF_MATERIAL (MATERIAL_ID, MATERIAL_NAME, MATERIAL_CODE,  IS_ACTIVE, CREATE_DATE, C_USER_ID, IS_RM_ACTIVE, IS_FG_ACTIVE) VALUES ( :NoMaterialID, :TextMaterialName, :TextMaterialCode,  :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsRmActive, :TextIsFgActive)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[8];
                    objPrm[0] = cmdi.Parameters.Add("NoMaterialID", newMaterialID);
                    objPrm[1] = cmdi.Parameters.Add("TextMaterialName", TextMaterialName.Text);
                    objPrm[2] = cmdi.Parameters.Add("TextMaterialCode", TextMaterialCode.Text); 
                    objPrm[3] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[4] = cmdi.Parameters.Add("TextIsRmActive", IsRmActive);
                    objPrm[5] = cmdi.Parameters.Add("TextIsFgActive", IsFgActive);
                    objPrm[6] = cmdi.Parameters.Add("u_date", c_date);
                    objPrm[7] = cmdi.Parameters.Add("NoCuserID", userID);


                    cmdi.ExecuteNonQuery();

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Material successfully"));
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
              
             string makeSQL = " select *  from MF_MATERIAL where MATERIAL_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextMaterialID.Text = dt.Rows[i]["MATERIAL_ID"].ToString();
                 TextMaterialName.Text = dt.Rows[i]["MATERIAL_NAME"].ToString();
                 TextMaterialCode.Text = dt.Rows[i]["MATERIAL_CODE"].ToString(); 
                 CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                 CheckIsRmActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_RM_ACTIVE"].ToString() == "Enable" ? true : false);
                 CheckIsFgActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_FG_ACTIVE"].ToString() == "Enable" ? true : false);       
             } 
             
             conn.Close();
             Display();
             CheckMaterialName.Text = "";
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
                  
                string makeSQL = "";
                if (txtSearchUserRole.Text == "")
                {
                    makeSQL = " select  * from MF_MATERIAL ORDER BY UPDATE_DATE desc, CREATE_DATE desc";
                }
                else
                {
                    makeSQL = " select  * from MF_MATERIAL where MATERIAL_ID like '" + txtSearchUserRole.Text + "%' or MATERIAL_NAME like '" + txtSearchUserRole.Text + "%'  or IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY UPDATE_DATE desc, CREATE_DATE desc";

                    alert_box.Visible = false;
                }
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "MATERIAL_ID" };

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
 

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int USER_DATA_ID = Convert.ToInt32(TextMaterialID.Text);   
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string IsRmActive = CheckIsRmActive.Checked ? "Enable" : "Disable";
                string IsFgActive = CheckIsFgActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  MF_MATERIAL  set MATERIAL_NAME = :TextMaterialName, MATERIAL_CODE = :TextMaterialCode,  UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive, IS_RM_ACTIVE = :TextIsRmActive, IS_FG_ACTIVE = :TextIsFgActive where MATERIAL_ID = :NoMaterialID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[8];
                objPrm[0] = cmdi.Parameters.Add("TextMaterialName", TextMaterialName.Text);
                objPrm[1] = cmdi.Parameters.Add("TextMaterialCode", TextMaterialCode.Text);  
                objPrm[2] = cmdi.Parameters.Add("TextIsActive", ISActive); 
                objPrm[3] = cmdi.Parameters.Add("TextIsRmActive", IsRmActive);
                objPrm[4] = cmdi.Parameters.Add("TextIsFgActive", IsFgActive);
                objPrm[5] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[6] = cmdi.Parameters.Add("NoMaterialID", USER_DATA_ID);
                objPrm[7] = cmdi.Parameters.Add("NoC_USER_ID", userID);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();  

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Material Update successfully"));
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

                int USER_DATA_ID = Convert.ToInt32(TextMaterialID.Text);
                string delete_user_page = " delete from MF_MATERIAL where MATERIAL_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Material Delete successfully"));
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
            TextMaterialID.Text = "";
            TextMaterialName.Text = "";
            TextMaterialCode.Text = ""; 
            CheckMaterialName.Text = ""; 
            CheckMaterialCode.Text = ""; 
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            
        }

        public void clearText()
        {
            TextMaterialID.Text = "";
            TextMaterialName.Text = "";
            TextMaterialCode.Text = ""; 
            CheckMaterialName.Text = "";
            CheckMaterialCode.Text = ""; 
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 

        }
         

        public void TextMaterialName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextMaterialName.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from MF_MATERIAL where MATERIAL_NAME = '" + TextMaterialName.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckMaterialName.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Material name is already entry</label>";
                    CheckMaterialName.ForeColor = System.Drawing.Color.Red;
                    TextMaterialName.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckMaterialName.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Material name is available</label>";
                    CheckMaterialName.ForeColor = System.Drawing.Color.Green;
                    TextMaterialCode.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else {
                    CheckMaterialName.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Material name is not blank</label>";
                    CheckMaterialName.ForeColor = System.Drawing.Color.Red;
                    TextMaterialName.Focus();
            }
            
        }


        public void TextMaterialCode_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextMaterialCode.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from MF_MATERIAL where MATERIAL_CODE = '" + TextMaterialCode.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckMaterialCode.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Material Code is already entry</label>";
                    CheckMaterialCode.ForeColor = System.Drawing.Color.Red;
                    TextMaterialCode.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckMaterialCode.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Material Code is available</label>";
                    CheckMaterialCode.ForeColor = System.Drawing.Color.Green;
                    CheckIsRmActive.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
                }
            }
            else {
                    CheckMaterialCode.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Material Code is not blank</label>";
                    CheckMaterialCode.ForeColor = System.Drawing.Color.Red;
                    TextMaterialCode.Focus();
            }
            
        } 



   }
}