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
using System.Text.RegularExpressions;


namespace NRCAPPS.NRC
{
    public partial class NrcDashboardItem : System.Web.UI.Page
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
                        DataTable dtDepartmentID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeDepartmentSQL = " SELECT * FROM HR_EMP_DEPARTMENTS WHERE IS_ACTIVE = 'Enable' ORDER BY DEPARTMENT_ID ASC";
                        ds = ExecuteBySqlStringEmpType(makeDepartmentSQL);
                        dtDepartmentID = (DataTable)ds.Tables[0];
                        DropDownDepartmentID.DataSource = dtDepartmentID;
                        DropDownDepartmentID.DataValueField = "DEPARTMENT_ID";
                        DropDownDepartmentID.DataTextField = "DEPARTMENT_NAME";
                        DropDownDepartmentID.DataBind();
                        DropDownDepartmentID.Items.Insert(0, new ListItem("Select  Department", "0"));

                        DataTable dtDivisionID = new DataTable();
                        DataSet dsd = new DataSet();
                        string makeDivisionSQL = " SELECT * FROM HR_EMP_DIVISIONS WHERE IS_ACTIVE = 'Enable' ORDER BY DIVISION_ID ASC";
                        dsd = ExecuteBySqlStringEmpType(makeDivisionSQL);
                        dtDepartmentID = (DataTable)dsd.Tables[0];
                        DropDownDivisionID.DataSource = dtDepartmentID;
                        DropDownDivisionID.DataValueField = "DIVISION_ID";
                        DropDownDivisionID.DataTextField = "DIVISION_NAME";
                        DropDownDivisionID.DataBind();
                        DropDownDivisionID.Items.Insert(0, new ListItem("Select  Division", "0"));
                          
                        Display();

                        alert_box.Visible = false;

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
            try
            {
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    int DepartmentID = Convert.ToInt32(DropDownDepartmentID.Text);
                    int DivisionID   = Convert.ToInt32(DropDownDivisionID.Text); 
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string get_dashboard_item_id = "select NRC_DASHBOARD_ITEMSID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_dashboard_item_id, conn);
                    int newDashboardItemID = Int16.Parse(cmdu.ExecuteScalar().ToString());


                    string insert_user = "insert into NRC_DASHBOARD_ITEMS (DASH_ITEM_ID, ITEM_NAME, DEPARTMENT_ID, DIVISION_ID, CREATE_DATE, C_USER_ID, IS_ACTIVE) values ( :NoDashboardItemID, :TextDashboardItemName, :NoDepartmentID, :NoDivisionID, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive)";
                    cmdi = new OracleCommand(insert_user, conn);
                      
                    OracleParameter[] objPrm = new OracleParameter[7];
                    objPrm[0] = cmdi.Parameters.Add("NoDashboardItemID", newDashboardItemID); 
                    objPrm[1] = cmdi.Parameters.Add("TextDashboardItemName", TextDashboardItemName.Text);  
                    objPrm[2] = cmdi.Parameters.Add("NoDepartmentID", DepartmentID);
                    objPrm[3] = cmdi.Parameters.Add("NoDivisionID", DivisionID); 
                    objPrm[4] = cmdi.Parameters.Add("c_date", c_date); 
                    objPrm[5] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[6] = cmdi.Parameters.Add("TextIsActive", ISActive);

                    cmdi.ExecuteNonQuery();

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert new Dashboard Item successfully"));
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
              
            DataSet ds = new DataSet();
            string makeSQL = " select * from NRC_DASHBOARD_ITEMS where DASH_ITEM_ID  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                TextDashboardItemID.Text      = dt.Rows[i]["DASH_ITEM_ID"].ToString();
                TextDashboardItemName.Text    = dt.Rows[i]["ITEM_NAME"].ToString();  
                DropDownDepartmentID.Text     = dt.Rows[i]["DEPARTMENT_ID"].ToString();
                DropDownDivisionID.Text       = dt.Rows[i]["DIVISION_ID"].ToString();  
                CheckIsActive.Checked         = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false); 
            }

            conn.Close();
            Display();
            CheckDashboardItemName.Text = "";
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

                DataTable dtEmpTypeID = new DataTable();
                DataSet ds = new DataSet();

                string makeSQL = "";
                if (txtSearchEmp.Text == "")
                {
                    makeSQL = " SELECT NDI.DASH_ITEM_ID, NDI.ITEM_NAME, HED.DIVISION_NAME, HEDP.DEPARTMENT_NAME, NDI.CREATE_DATE, NDI.UPDATE_DATE,  NDI.IS_ACTIVE FROM NRC_DASHBOARD_ITEMS NDI LEFT JOIN HR_EMP_DIVISIONS HED ON HED.DIVISION_ID = NDI.DIVISION_ID LEFT JOIN HR_EMP_DEPARTMENTS HEDP ON HEDP.DEPARTMENT_ID = NDI.DEPARTMENT_ID ORDER BY NDI.ITEM_NAME ";
                }
                else
                {
                    makeSQL = " SELECT NDI.DASH_ITEM_ID, NDI.ITEM_NAME, HED.DIVISION_NAME, HEDP.DEPARTMENT_NAME, NDI.CREATE_DATE, NDI.UPDATE_DATE,  NDI.IS_ACTIVE FROM NRC_DASHBOARD_ITEMS NDI LEFT JOIN HR_EMP_DIVISIONS HED ON HED.DIVISION_ID = NDI.DIVISION_ID LEFT JOIN HR_EMP_DEPARTMENTS HEDP ON HEDP.DEPARTMENT_ID = NDI.DEPARTMENT_ID   where NDI.DASH_ITEM_ID like '" + txtSearchEmp.Text + "%' or NDI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or HED.DIVISION_NAME like '" + txtSearchEmp.Text + "%' or HEDP.DEPARTMENT_NAME like '" + txtSearchEmp.Text + "%' or NDI.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ORDER BY NDI.ITEM_NAME ASC";
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "DASH_ITEM_ID" };
                GridView1.DataBind();
                conn.Close();
                // alert_box.Visible = false;
            }
        }

        protected void GridViewSearchEmp(object sender, EventArgs e)
        {
            this.Display();
        }

        protected void GridViewEmp_PageIndexChanging(object sender, GridViewPageEventArgs e)
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
                int USER_DATA_ID = Convert.ToInt32(TextDashboardItemID.Text);    
                int DepartmentID = Convert.ToInt32(DropDownDepartmentID.Text);
                int DivisionID = Convert.ToInt32(DropDownDivisionID.Text); 
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                 
                string update_user = "update  NRC_DASHBOARD_ITEMS  set ITEM_NAME = :TextDashboardItemName,  DEPARTMENT_ID = :NoDepartmentID, DIVISION_ID = :NoDivisionID,  UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where DASH_ITEM_ID = :NoDashboardItemID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[8]; 
                objPrm[1] = cmdi.Parameters.Add("TextDashboardItemName", TextDashboardItemName.Text);  
                objPrm[2] = cmdi.Parameters.Add("NoDepartmentID", DepartmentID);
                objPrm[3] = cmdi.Parameters.Add("NoDivisionID", DivisionID); 
                objPrm[4] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[5] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[6] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[7] = cmdi.Parameters.Add("NoDashboardItemID", USER_DATA_ID);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Dashboard Item Data Update successfully"));
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
            if (IS_DELETE_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                int USER_DATA_ID = Convert.ToInt32(TextDashboardItemID.Text);
                string delete_user = " delete from HR_EMPLOYEES where EMP_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user, conn);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Employee Data Delete successfully"));
                alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
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
            TextDashboardItemName.Text = ""; 
            CheckDashboardItemName.Text = ""; 
            DropDownDepartmentID.Text = "0";
            DropDownDivisionID.Text = "0"; 
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        { 
            TextDashboardItemName.Text = "";  
            CheckDashboardItemName.Text = ""; 
            DropDownDepartmentID.Text = "0";
            DropDownDivisionID.Text = "0"; 
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public DataSet ExecuteBySqlStringEmpType(string sqlString)
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

        public void TextDashboardItemName_TextChanged(object sender, EventArgs e)
        {
            string DashboardItemName = TextDashboardItemName.Text; 
            if (DashboardItemName != null)
            { 
                    alert_box.Visible = false; 
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "select ITEM_NAME from NRC_DASHBOARD_ITEMS where ITEM_NAME = '" + DashboardItemName + "'";
                    cmd.CommandType = CommandType.Text;

                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        CheckDashboardItemName.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Dashboard Item Name is not available</label>";
                        CheckDashboardItemName.ForeColor = System.Drawing.Color.Red;
                        TextDashboardItemName.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

                      
                    }
                    else
                    {
                        CheckDashboardItemName.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Dashboard Item Name is available</label>";
                        CheckDashboardItemName.ForeColor = System.Drawing.Color.Green;
                        TextDashboardItemName.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
                    }
                }
                 
        }
    }
}