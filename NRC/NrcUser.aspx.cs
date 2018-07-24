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
    public partial class NrcUser : System.Web.UI.Page
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
                    // lb1.Text = "<b><font color=Brown>" + "WELLCOME :: " + "</font>" + "<b><font color=red>" + Session["USER_NAME"] + " User ID:" + Session["USER_ID"] + "</font>";
                    if (!IsPostBack)
                    {
                        DataTable dtEmployeeID = new DataTable();
                        DataSet dse = new DataSet();
                        string makeEmployeeSQL = " SELECT EMP_ID, EMP_FNAME || ' ' ||EMP_LNAME AS EMP_NAME FROM HR_EMPLOYEES WHERE IS_ACTIVE = 'Enable' ORDER BY EMP_ID ASC";
                        dse = ExecuteBySqlStringUserType(makeEmployeeSQL);
                        dtEmployeeID = (DataTable)dse.Tables[0];
                        DropDownEmployeeID.DataSource = dtEmployeeID;
                        DropDownEmployeeID.DataValueField = "EMP_ID";
                        DropDownEmployeeID.DataTextField = "EMP_NAME";
                        DropDownEmployeeID.DataBind();
                        DropDownEmployeeID.Items.Insert(0, new ListItem("Select  Employee", "0"));

                        DataTable dtUserRoleID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeRoleSQL = " SELECT USER_ROLE_ID, USER_ROLE_SHORT_NAME || ' - ' ||USER_ROLE_NAME AS USER_ROLE_SNAME FROM NRC_USER_ROLE WHERE IS_ACTIVE = 'Enable' ";
                        ds = ExecuteBySqlStringUserType(makeRoleSQL);
                        dtUserRoleID = (DataTable)ds.Tables[0];
                        DropDownUserRoleID.DataSource     = dtUserRoleID;
                        DropDownUserRoleID.DataValueField = "USER_ROLE_ID";
                        DropDownUserRoleID.DataTextField  = "USER_ROLE_SNAME";
                        DropDownUserRoleID.DataBind();
                        DropDownUserRoleID.Items.Insert(0, new ListItem("Select User Role", "0"));

                        Display();
                        TextPassword.TextMode = TextBoxMode.Password; 
                        TextPasswordConfirm.Text = "";
                        ChangeOpen.Visible = false; 
                        alert_box.Visible = false;

                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled"); 

                    } IsLoad = false;
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
            
               if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    int UserRoleID = Convert.ToInt32(DropDownUserRoleID.Text);
                    int EmpID = Convert.ToInt32(DropDownEmployeeID.Text); 
                    string get_user_id = "select NRC_USER_USERID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_user_id, conn);
                    int newUserID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
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

                    string insert_user = "insert into  NRC_USER (USER_ID, USER_NAME, PASSWORD, CREATE_DATE, USER_ROLE_ID, C_USER_ID, IS_ACTIVE, EMP_ID) values ( :NoUserID, :TextUserName,  :TextPassword, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoUserRoleID , :NoCuserID, :TextIsActive, :NoEmpID)";
                    cmdi = new OracleCommand(insert_user, conn);
                    
                    OracleParameter[] objPrm = new OracleParameter[8];
                    objPrm[0] = cmdi.Parameters.Add("NoUserID", newUserID);
                    objPrm[1] = cmdi.Parameters.Add("TextUserName", TextUserName.Text);
                    objPrm[2] = cmdi.Parameters.Add("TextPassword", EncryptoPassword); 
                    objPrm[3] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[4] = cmdi.Parameters.Add("NoUserRoleID", UserRoleID);
                    objPrm[5] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[6] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[7] = cmdi.Parameters.Add("NoEmpID", EmpID);

                    cmdi.ExecuteNonQuery(); 
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();    

                 //   string filename = Path.GetFileName(FileUpload1.PostedFile.FileName);
                  //  string contentType = FileUpload1.PostedFile.ContentType;

                    String rootPath = Server.MapPath("~/image/default-profile-image.jpg"); 
                    String filename = "default-profile-image.jpg";  
                    string contentType = "image/jepg"; 
                    
                    using (Stream fs = new FileStream(rootPath, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            byte[] bytes = br.ReadBytes((Int32)fs.Length);
                            using (OracleConnection con = new OracleConnection(strConnString))
                            {
                                string query = "update  NRC_USER set USER_IMAGE_NAME = :Name, IMAGE_CONTENT_TYPE = :ContentType, IMAGE_DATA = :Data where USER_ID = :NoUserID ";
                                using (OracleCommand cmd = new OracleCommand(query))
                                {
                                    cmd.Connection = con;
                                    OracleParameter[] objPrmImg = new OracleParameter[4];
                                    objPrmImg[0] = cmd.Parameters.Add("Name", filename);
                                    objPrmImg[1] = cmd.Parameters.Add("ContentType", contentType);
                                    objPrmImg[2] = cmd.Parameters.Add("Data", bytes);
                                    objPrmImg[3] = cmd.Parameters.Add("NoUserID", newUserID);

                                    con.Open();
                                    cmd.ExecuteNonQuery(); 
                                }
                            }
                        }
                    }

                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert new User successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                    clearText();
                    Display();
                }
               else
               {
                   Response.Redirect("~/PagePermissionError.aspx");
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
             string makeSQL = " select NU.*, HE.EMP_FNAME || ' ' || HE.EMP_LNAME AS EMP_NAME, NUR.USER_ROLE_NAME, NUR.UR_BG_COLOR from NRC_USER NU left join HR_EMPLOYEES HE ON HE.EMP_ID = NU.EMP_ID left join NRC_USER_ROLE  NUR ON NUR.USER_ROLE_ID =  NU.USER_ROLE_ID where NU.USER_ID  = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count;
             string passDB = "";

             for (int i = 0; i < RowCount; i++)
             {
                 TextUserID.Text          = dt.Rows[i]["USER_ID"].ToString();
                 TextUserName.Text        = dt.Rows[i]["USER_NAME"].ToString();
                 DropDownEmployeeID.Text  = dt.Rows[i]["EMP_ID"].ToString();
                 passDB                   = dt.Rows[i]["PASSWORD"].ToString();
                 DropDownUserRoleID.Text  = dt.Rows[i]["USER_ROLE_ID"].ToString();
                 CheckIsActive.Checked    = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);

                 string pass_add = "1234567891234560";

                 OracleCommand cmdep = new OracleCommand();
                 cmdep.Connection = conn;
                 cmdep.CommandText = " SELECT get_dec_val ('" + passDB + "','" + pass_add + "') FROM dual";
                 cmdep.CommandType = CommandType.Text;
                 OracleDataReader dre = cmdep.ExecuteReader();

                  
                 while (dre.Read())
                 {
                     if (!dre.IsDBNull(0))
                     {
                         TextPassword.Text = dre.GetString(0);
                     }
                 }
                   
             }
        //     radPassChange.Attributes.Add("checked", "checked");  
             radPassChange.SelectedIndex = 0; 
             ChangePass.Visible = false;
             ChangeOpen.Visible = true; 
             conn.Close();
             Display();
             CheckUsername.Text = "";
             alert_box.Visible = false;
             DropDownEmployeeID.Attributes.Add("disabled", "disabled"); 
             BtnAdd.Attributes.Add("aria-disabled", "false");
             BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
             
             BtnUpdate.Attributes.Add("aria-disabled", "true");
             BtnUpdate.Attributes.Add("class", "btn btn-success active");
             BtnDelete.Attributes.Add("aria-disabled", "true");
             BtnDelete.Attributes.Add("class", "btn btn-danger active"); 

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
                if (txtSearchUser.Text == "")
                {
                    makeSQL = " select NU.*, HE.EMP_FNAME || ' ' || HE.EMP_LNAME AS EMP_NAME, NUR.USER_ROLE_NAME, NUR.UR_BG_COLOR from NRC_USER NU left join HR_EMPLOYEES HE ON HE.EMP_ID = NU.EMP_ID left join NRC_USER_ROLE  NUR ON NUR.USER_ROLE_ID =  NU.USER_ROLE_ID ORDER BY NU.UPDATE_DATE desc";
                }
                else
                {
                    makeSQL = " select NU.*, NUR.USER_ROLE_NAME, NUR.UR_BG_COLOR from NRC_USER NU left join NRC_USER_ROLE  NUR ON NUR.USER_ROLE_ID =  NU.USER_ROLE_ID where NU.USER_NAME like '" + txtSearchUser.Text + "%' or NU.FNAME like '" + txtSearchUser.Text + "%' or NU.LNAME like '" + txtSearchUser.Text + "%' ORDER BY NU.UPDATE_DATE desc";
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "USER_ID" };
                GridView1.DataBind();
                conn.Close();
                // alert_box.Visible = false;
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
                int USER_DATA_ID = Convert.ToInt32(TextUserID.Text);  
                int UserRoleID = Convert.ToInt32(DropDownUserRoleID.Text);
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");


                if(radPassChange.Text == "Change")
                {
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

                    string update_user = "update  NRC_USER  set USER_NAME = :TextUserName,  PASSWORD = :TextPassword, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , USER_ROLE_ID = :NoUserRoleID, U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where USER_ID = :NoUserID ";
                    cmdi = new OracleCommand(update_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[7];
                    objPrm[0] = cmdi.Parameters.Add("TextUserName", TextUserName.Text);
                    objPrm[1] = cmdi.Parameters.Add("TextPassword", EncryptoPassword);
                    objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[3] = cmdi.Parameters.Add("NoUserRoleID", UserRoleID);
                    objPrm[4] = cmdi.Parameters.Add("NoUserID", USER_DATA_ID);
                    objPrm[5] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                    objPrm[6] = cmdi.Parameters.Add("TextIsActive", ISActive);

                } else {

                    string update_user = "update  NRC_USER  set USER_NAME = :TextUserName, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), USER_ROLE_ID = :NoUserRoleID, U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where USER_ID = :NoUserID ";
                    cmdi = new OracleCommand(update_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[6];
                    objPrm[0] = cmdi.Parameters.Add("TextUserName", TextUserName.Text);
                    objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[2] = cmdi.Parameters.Add("NoUserRoleID", UserRoleID);
                    objPrm[3] = cmdi.Parameters.Add("NoUserID", USER_DATA_ID);
                    objPrm[4] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                    objPrm[5] = cmdi.Parameters.Add("TextIsActive", ISActive);
                                
                }
                     
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();  

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("User Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                Display();
            }
         else
         {
             Response.Redirect("~/PagePermissionError.aspx");
         }
        }

        public void Redio_CheckedChanged(object sender, EventArgs e)
        {
            if (radPassChange.SelectedValue == "Not_Change")
            {
                ChangePass.Visible = false; 
            }
            else
            { 
                ChangePass.Visible = true;
            }
        }


        protected void BtnDelete_Click(object sender, EventArgs e)
        {
          if (IS_DELETE_ACTIVE == "Enable")
            {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            int USER_DATA_ID = Convert.ToInt32(TextUserID.Text); 
            string delete_user = " delete from NRC_USER where USER_ID  = '" + USER_DATA_ID + "'";

            cmdi = new OracleCommand(delete_user, conn);
            
            cmdi.ExecuteNonQuery();
            cmdi.Parameters.Clear();
            cmdi.Dispose();
            conn.Close();
            alert_box.Visible = true;
            alert_box.Controls.Add(new LiteralControl("User Data Delete successfully"));
            alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
            clearText();
            clearText();
            Display();
            }
          else
          {
              Response.Redirect("~/PagePermissionError.aspx");
          }

        }

        public void clearTextField(object sender, EventArgs e)
        {
            TextUserID.Text         = "";
            TextUserName.Text       = ""; 
            DropDownEmployeeID.Text  = "0";
            TextPassword.Text       = "";
            CheckUsername.Text      = "";
            CheckEmpID.Text = "";
            DropDownUserRoleID.Text = "0";
            DropDownEmployeeID.Attributes.Remove("disabled");
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            
        }

        public void clearText()
        {
            TextUserID.Text = "";
            TextUserName.Text = ""; 
            DropDownEmployeeID.Text = "0";
            TextPassword.Text = "";
            CheckUsername.Text = "";
            CheckEmpID.Text = "";
            DropDownUserRoleID.Text = "0";
            DropDownEmployeeID.Attributes.Remove("disabled");
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

       

        public void TextEmpID_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DropDownEmployeeID.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from NRC_USER where EMP_ID = '" + DropDownEmployeeID.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckEmpID.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Employee is already include user list</label>";
                    CheckEmpID.ForeColor = System.Drawing.Color.Red;
                    DropDownEmployeeID.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckEmpID.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Employee has not user list</label>";
                    CheckEmpID.ForeColor = System.Drawing.Color.Green;
                    DropDownEmployeeID.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else
            {
                CheckEmpID.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Select Employee is not blank</label>";
                CheckEmpID.ForeColor = System.Drawing.Color.Red;
                DropDownEmployeeID.Focus();
            }

        } 

        public void TextUserName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextUserName.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from NRC_USER where USER_NAME = '" + TextUserName.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckUsername.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> User name is not available</label>";
                    CheckUsername.ForeColor = System.Drawing.Color.Red;
                    TextUserName.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                { 
                    CheckUsername.Text = "<label class='control-label'><i class='fa fa fa-check'></i> User name is available</label>";
                    CheckUsername.ForeColor = System.Drawing.Color.Green;
                    TextUserName.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else {
                    CheckUsername.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> User name is not blank</label>";
                    CheckUsername.ForeColor = System.Drawing.Color.Red;
                    TextUserName.Focus();
            }
            
        } 
   }
}