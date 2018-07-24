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


namespace NRCAPPS.HR
{
    public partial class HrEmployee : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand  cmdi, cmdl;
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

                        DataTable dtLocationID = new DataTable();
                        DataSet dsl = new DataSet();
                        string makeLocationSQL = " SELECT * FROM HR_EMP_LOCATIONS WHERE IS_ACTIVE = 'Enable' ORDER BY LOCATION_ID ASC";
                        dsl = ExecuteBySqlStringEmpType(makeLocationSQL);
                        dtLocationID = (DataTable)dsl.Tables[0];
                        DropDownLocationID.DataSource = dtLocationID;
                        DropDownLocationID.DataValueField = "LOCATION_ID";
                        DropDownLocationID.DataTextField = "LOCATION_NAME";
                        DropDownLocationID.DataBind();
                        DropDownLocationID.Items.Insert(0, new ListItem("Select  Location", "0"));


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
                    int LocationID   = Convert.ToInt32(DropDownLocationID.Text); 
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                    string DropDownGender = DropDownEmpTitle.Text == "Mr" ? "Male" : "Female";
                     
                     
                    string insert_user = "insert into  HR_EMPLOYEES (EMP_ID, EMP_TITLE, EMP_FNAME, EMP_LNAME, EMP_NAT_ID, GENDER, EMAIL, DEPARTMENT_ID, DIVISION_ID, LOCATION_ID, CREATE_DATE, C_USER_ID, IS_ACTIVE) values ( :NoEmpID, :TextEmpTitle, :TextFname, :TextLname, :TextNid, :TextGender, :TextEMAIL, :NoDepartmentID, :NoDivisionID, :NoLocationID, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive)";
                    cmdi = new OracleCommand(insert_user, conn);
                      
                    OracleParameter[] objPrm = new OracleParameter[13];
                    objPrm[0] = cmdi.Parameters.Add("NoEmpID", TextEmpID.Text);
                    objPrm[1] = cmdi.Parameters.Add("TextEmpTitle", DropDownEmpTitle.Text);
                    objPrm[2] = cmdi.Parameters.Add("TextFname", TextFname.Text);
                    objPrm[3] = cmdi.Parameters.Add("TextLname", TextLname.Text);
                    objPrm[4] = cmdi.Parameters.Add("TextNid", TextNid.Text);
                    objPrm[5] = cmdi.Parameters.Add("TextGender", DropDownGender);
                    objPrm[6] = cmdi.Parameters.Add("TextEMAIL", TextEmail.Text);
                    objPrm[7] = cmdi.Parameters.Add("NoDepartmentID", DepartmentID);
                    objPrm[8] = cmdi.Parameters.Add("NoDivisionID", DivisionID);
                    objPrm[9] = cmdi.Parameters.Add("NoLocationID", LocationID);
                    objPrm[10] = cmdi.Parameters.Add("u_date", u_date); 
                    objPrm[11] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[12] = cmdi.Parameters.Add("TextIsActive", ISActive);

                    cmdi.ExecuteNonQuery();

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert new Employee successfully"));
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
            string makeSQL = " select * from HR_EMPLOYEES where EMP_ID  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                TextEmpID.Text            = dt.Rows[i]["EMP_ID"].ToString();
                DropDownEmpTitle.Text     = dt.Rows[i]["EMP_TITLE"].ToString();
                TextFname.Text            = dt.Rows[i]["EMP_FNAME"].ToString();
                TextLname.Text            = dt.Rows[i]["EMP_LNAME"].ToString();
                DropDownGender.Text       = dt.Rows[i]["GENDER"].ToString();
                TextNid.Text              = dt.Rows[i]["EMP_NAT_ID"].ToString();
                TextEmail.Text            = dt.Rows[i]["EMAIL"].ToString(); 
                DropDownDepartmentID.Text = dt.Rows[i]["DEPARTMENT_ID"].ToString();
                DropDownDivisionID.Text   = dt.Rows[i]["DIVISION_ID"].ToString();
                DropDownLocationID.Text   = dt.Rows[i]["LOCATION_ID"].ToString();

                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                 
            }

            conn.Close();
            Display();
            CheckEmpID.Text = "";
            alert_box.Visible = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void ChangeGender(object sender, EventArgs e)
        {

            if (DropDownEmpTitle.SelectedValue == "Mr")
            {
                DropDownGender.Text = "Male";    
            }
            else if (DropDownEmpTitle.SelectedValue == "Ms")
            {
                DropDownGender.Text = "Female";
            }
            
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
                    makeSQL = " select HE.*, HED.DEPARTMENT_NAME, HEDIV.DIVISION_NAME, HEL.LOCATION_NAME from HR_EMPLOYEES HE left join HR_EMP_DEPARTMENTS HED ON HED.DEPARTMENT_ID =  HE.DEPARTMENT_ID left join HR_EMP_DIVISIONS HEDIV ON HEDIV.DIVISION_ID = HE.DIVISION_ID left join HR_EMP_LOCATIONS HEL ON HEL.LOCATION_ID = HE.LOCATION_ID ORDER BY HE.CREATE_DATE desc, HE.UPDATE_DATE desc";
                }
                else
                {
                    makeSQL = " select HE.*, HED.DEPARTMENT_NAME, HEDIV.DIVISION_NAME, HEL.LOCATION_NAME from HR_EMPLOYEES HE left join HR_EMP_DEPARTMENTS HED ON HED.DEPARTMENT_ID =  HE.DEPARTMENT_ID left join HR_EMP_DIVISIONS HEDIV ON HEDIV.DIVISION_ID = HE.DIVISION_ID left join HR_EMP_LOCATIONS HEL ON HEL.LOCATION_ID = HE.LOCATION_ID where HE.EMP_ID like '" + txtSearchEmp.Text + "%' or HE.EMP_TITLE like '" + txtSearchEmp.Text + "%' or HE.EMP_FNAME like '" + txtSearchEmp.Text + "%' or HE.EMP_LNAME like '" + txtSearchEmp.Text + "%' or HE.EMP_NAT_ID like '" + txtSearchEmp.Text + "%' or HE.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ORDER BY HE.CREATE_DATE desc, HE.UPDATE_DATE desc";
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "EMP_ID" };
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
                int USER_DATA_ID = Convert.ToInt32(TextEmpID.Text);    
                int DepartmentID = Convert.ToInt32(DropDownDepartmentID.Text);
                int DivisionID = Convert.ToInt32(DropDownDivisionID.Text);
                int LocationID = Convert.ToInt32(DropDownLocationID.Text);
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string DropDownGender = DropDownEmpTitle.Text == "Mr" ? "Male" : "Female";


                string update_user = "update  HR_EMPLOYEES  set EMP_TITLE = :TextEmpTitle, EMP_FNAME = :TextFname, EMP_LNAME = :TextLname, EMP_NAT_ID = :TextNid, GENDER = :TextGender, EMAIL = :TextEMAIL, DEPARTMENT_ID = :NoDepartmentID, DIVISION_ID = :NoDivisionID, LOCATION_ID = :NoLocationID, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where EMP_ID = :NoEmpID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[13];
                objPrm[0] = cmdi.Parameters.Add("TextEmpTitle", DropDownEmpTitle.Text);
                objPrm[1] = cmdi.Parameters.Add("TextFname", TextFname.Text);
                objPrm[2] = cmdi.Parameters.Add("TextLname", TextLname.Text);
                objPrm[3] = cmdi.Parameters.Add("TextNid", TextNid.Text);
                objPrm[4] = cmdi.Parameters.Add("TextGender", DropDownGender);
                objPrm[5] = cmdi.Parameters.Add("TextEMAIL", TextEmail.Text);
                objPrm[6] = cmdi.Parameters.Add("NoDepartmentID", DepartmentID);
                objPrm[7] = cmdi.Parameters.Add("NoDivisionID", DivisionID);
                objPrm[8] = cmdi.Parameters.Add("NoLocationID", LocationID);
                objPrm[9] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[10] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[11] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[12] = cmdi.Parameters.Add("NoEmpID", USER_DATA_ID);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Employee Data Update successfully"));
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

                int USER_DATA_ID = Convert.ToInt32(TextEmpID.Text);
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
            TextEmpID.Text = "";
            DropDownEmpTitle.Text = "0";
            TextFname.Text = "";
            TextLname.Text = "";
            TextEmail.Text = "";
            TextNid.Text = "";
            CheckEmpID.Text = "";
            DropDownGender.Text = "0";
            DropDownDepartmentID.Text = "0";
            DropDownDivisionID.Text = "0";
            DropDownLocationID.Text = "0";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        {
            TextEmpID.Text = "";
            DropDownEmpTitle.Text = "0";
            TextFname.Text = "";
            TextLname.Text = "";
            TextEmail.Text = "";
            TextNid.Text = "";
            CheckEmpID.Text = "";
            DropDownGender.Text = "0";
            DropDownDepartmentID.Text = "0";
            DropDownDivisionID.Text = "0";
            DropDownLocationID.Text = "0";
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

        public void TextEmpID_TextChanged(object sender, EventArgs e)
        {
            string EmpID = TextEmpID.Text;
            string MatchEmpIDPattern = "^([0-9]{4})$";
            if (EmpID != null)
            {
                 
                if (Regex.IsMatch(EmpID, MatchEmpIDPattern))
                {
                    alert_box.Visible = false;

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from HR_EMPLOYEES where EMP_ID = '" + Convert.ToInt32(EmpID) + "'";
                    cmd.CommandType = CommandType.Text;

                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        CheckEmpID.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Emp ID is not available</label>";
                        CheckEmpID.ForeColor = System.Drawing.Color.Red;
                        TextEmpID.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

                        if (DropDownEmpTitle.SelectedValue == "Mr")
                        {
                            DropDownGender.Text = "Male";
                        }
                        else if (DropDownEmpTitle.SelectedValue == "Ms")
                        {
                            DropDownGender.Text = "Female";
                        }
                    }
                    else
                    {
                        CheckEmpID.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Emp ID is available</label>";
                        CheckEmpID.ForeColor = System.Drawing.Color.Green;
                        TextEmpID.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");

                        if (DropDownEmpTitle.SelectedValue == "Mr")
                        {
                            DropDownGender.Text = "Male";
                        }
                        else if (DropDownEmpTitle.SelectedValue == "Ms")
                        {
                            DropDownGender.Text = "Female";
                        }

                    }
                }
                else
                {
                    CheckEmpID.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter Emp ID is 4 digit only</label>";
                    CheckEmpID.ForeColor = System.Drawing.Color.Red;
                    TextEmpID.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
            }
        }
    }
}