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
using System.Globalization;


namespace NRCAPPS.PF
{
    public partial class PfProductionJw : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl, cmdsp, cmdsg;
        OracleDataAdapter oradata; 
        DataTable dt;
        int RowCount;

        string IS_PAGE_ACTIVE = "";
        string IS_ADD_ACTIVE = "";
        string IS_EDIT_ACTIVE = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE = "";
        string IS_REPORT_ACTIVE = "";

        public bool IsLoad { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_NAME"] != null)
            {
                string requestedFile = Path.GetFileName(Request.Path);
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = " SELECT  NUPP.IS_PAGE_ACTIVE, NUPP.IS_ADD_ACTIVE, NUPP.IS_EDIT_ACTIVE, NUPP.IS_DELETE_ACTIVE, NUPP.IS_VIEW_ACTIVE, NUPP.IS_REPORT_ACTIVE FROM NRC_USER_PAGE_PERMISSION NUPP LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NUPP.USER_PAGE_ID  WHERE NUPP.USER_ID = '" + Session["USER_ID"] + "' AND NUP.IS_ACTIVE = 'Enable' AND NUP.PAGE_URL = '" + requestedFile + "' ";
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
                    IS_REPORT_ACTIVE = dt.Rows[i]["IS_REPORT_ACTIVE"].ToString();
                }

                if (IS_PAGE_ACTIVE == "Enable")
                {
                     if (!IsPostBack)
                    {
                        DataTable dtShiftID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeShiftSQL = " SELECT * FROM PF_PRODUCTION_SHIFT WHERE IS_ACTIVE = 'Enable' ORDER BY SHIFT_NAME ASC";
                        ds = ExecuteBySqlString(makeShiftSQL);
                        dtShiftID = (DataTable)ds.Tables[0];
                        DropDownShiftID.DataSource = dtShiftID;
                        DropDownShiftID.DataValueField = "SHIFT_ID";
                        DropDownShiftID.DataTextField = "SHIFT_NAME";
                        DropDownShiftID.DataBind();
                        DropDownShiftID.Items.Insert(0, new ListItem("Select  Shift", "0"));

                        DataTable dtMachineID = new DataTable();
                        DataSet dsl = new DataSet();
                        string makeMachineSQL = " SELECT * FROM PF_PRODUCTION_MACHINE WHERE IS_ACTIVE = 'Enable' ORDER BY MACHINE_ID ASC";
                        dsl = ExecuteBySqlString(makeMachineSQL);
                        dtMachineID = (DataTable)dsl.Tables[0];
                        DropDownMachineID.DataSource = dtMachineID;
                        DropDownMachineID.DataValueField = "MACHINE_ID";
                        DropDownMachineID.DataTextField = "MACHINE_NUMBER";
                        DropDownMachineID.DataBind();
                        DropDownMachineID.Items.Insert(0, new ListItem("Select Machine Number", "0"));

                        DataTable dtSupervisorID = new DataTable();
                        DataSet dsd = new DataSet();
                        string makeSupervisorSQL = " SELECT * FROM PF_SUPERVISOR WHERE IS_ACTIVE = 'Enable' ORDER BY SUPERVISOR_NAME ASC";
                        dsd = ExecuteBySqlString(makeSupervisorSQL);
                        dtSupervisorID = (DataTable)dsd.Tables[0];
                        DropDownSupervisorID.DataSource = dtSupervisorID;
                        DropDownSupervisorID.DataValueField = "SUPERVISOR_ID";
                        DropDownSupervisorID.DataTextField = "SUPERVISOR_NAME";
                        DropDownSupervisorID.DataBind();
                        DropDownSupervisorID.Items.Insert(0, new ListItem("Select  Supervisor", "0"));

                   

                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeDropDownItemSQL = " SELECT * FROM PF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        dsi = ExecuteBySqlString(makeDropDownItemSQL);
                        dtItemID = (DataTable)dsi.Tables[0];
                        DropDownItemID.DataSource = dtItemID;
                        DropDownItemID.DataValueField = "ITEM_ID";
                        DropDownItemID.DataTextField = "ITEM_NAME";
                        DropDownItemID.DataBind();
                        DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));

                        DropDownItemID1.DataSource = dtItemID;
                        DropDownItemID1.DataValueField = "ITEM_ID";
                        DropDownItemID1.DataTextField = "ITEM_NAME";
                        DropDownItemID1.DataBind();
                        DropDownItemID1.Items.Insert(0, new ListItem("All Item", "0"));
                          
                        DataTable dtSubItemID = new DataTable();
                        DataSet dss = new DataSet();
                        string makeDropDownSubItemSQL = " SELECT * FROM PF_SUB_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY SUB_ITEM_ID ASC";
                        dss = ExecuteBySqlString(makeDropDownSubItemSQL);
                        dtSubItemID = (DataTable)dss.Tables[0];
                        DropDownSubItemID.DataSource = dtSubItemID;
                        DropDownSubItemID.DataValueField = "SUB_ITEM_ID";
                        DropDownSubItemID.DataTextField = "SUB_ITEM_NAME";
                        DropDownSubItemID.DataBind();
                        DropDownSubItemID.Items.Insert(0, new ListItem("Select Sub Item", "0"));
 

                        Display();
                        DropDownShiftID.Focus();
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
                    int ShiftID = Convert.ToInt32(DropDownShiftID.Text);                   
                    int MachineID   = Convert.ToInt32(DropDownMachineID.Text);
                    int SupervisorID = Convert.ToInt32(DropDownSupervisorID.Text);
                    double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
                    int ItemID = Convert.ToInt32(DropDownItemID.Text);
                    int SubItemID = Convert.ToInt32(DropDownSubItemID.Text);
                    string ItemName = DropDownItemID.SelectedItem.Text;
                    string SubItemName = "";
                    if (SubItemID == 0)
                    {
                        SubItemID = 0;
                        SubItemName = "";
                    }
                    else
                    {
                        SubItemID = Convert.ToInt32(DropDownSubItemID.Text);
                        SubItemName = DropDownSubItemID.SelectedItem.Text;
                    }
                      
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                     
                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                     
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string get_user_production_id = "select PF_PRODUCTION_JWID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_user_production_id, conn);
                    int newProductionID = Int16.Parse(cmdu.ExecuteScalar().ToString());  

                    string insert_production = "insert into  PF_PRODUCTION_JW (PRODUCTION_JW_ID, SHIFT_ID, MACHINE_ID, SUPERVISOR_ID, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, ENTRY_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID ) values ( 'JW' || LPAD(:NoProductionID, 6, '0'), :NoShiftID, :NoMachineID, :NoSupervisorID, :NoItemID, :NoSubItemID,  :TextItemWeight, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 3)";
                    cmdi = new OracleCommand(insert_production, conn);
                      
                    OracleParameter[] objPrm = new OracleParameter[11];
                    objPrm[0] = cmdi.Parameters.Add("NoProductionID", newProductionID);
                    objPrm[1] = cmdi.Parameters.Add("NoShiftID", ShiftID);  
                    objPrm[2] = cmdi.Parameters.Add("NoMachineID", MachineID);
                    objPrm[3] = cmdi.Parameters.Add("NoSupervisorID", SupervisorID);
                    objPrm[4] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[5] = cmdi.Parameters.Add("NoSubItemID", SubItemID);  
                    objPrm[6] = cmdi.Parameters.Add("TextItemWeight", ItemWeight);  
                    objPrm[7] = cmdi.Parameters.Add("EntryDate", EntryDateNew); 
                    objPrm[8] = cmdi.Parameters.Add("c_date", c_date); 
                    objPrm[9] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[10] = cmdi.Parameters.Add("TextIsActive", ISActive);

                    cmdi.ExecuteNonQuery();

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                     
                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Job Work (Production) Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                     
                    clearText();
                    Display();
                    DropDownShiftID.Focus();
                     
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
           try{
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
            string USER_DATA_ID = Session["user_data_id"].ToString();


            DataTable dtUserTypeID = new DataTable();
            DataSet ds = new DataSet();
            string makeSQL = " SELECT PRODUCTION_JW_ID, SHIFT_ID, MACHINE_ID, SUPERVISOR_ID, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, CREATE_DATE, UPDATE_DATE, IS_ACTIVE  from PF_PRODUCTION_JW where PRODUCTION_JW_ID  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                TextProductionID.Text     = dt.Rows[i]["PRODUCTION_JW_ID"].ToString(); 
                DropDownShiftID.Text      = dt.Rows[i]["SHIFT_ID"].ToString(); 
                DropDownMachineID.Text    = dt.Rows[i]["MACHINE_ID"].ToString();
                DropDownSupervisorID.Text = dt.Rows[i]["SUPERVISOR_ID"].ToString();
                DropDownItemID.Text       = dt.Rows[i]["ITEM_ID"].ToString(); 
                DropDownSubItemID.Text    = dt.Rows[i]["SUB_ITEM_ID"].ToString();
                TextItemWeight.Text       = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT"].ToString()).ToString("0.000");  
                EntryDate.Text            = dt.Rows[i]["ENTRY_DATE"].ToString(); 
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false); 
            }

            conn.Close();
            Display();
            CheckItemWeight.Text = "";
            alert_box.Visible = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
           }
           catch
           {
               Response.Redirect("~/ParameterError.aspx");
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
                    makeSQL = " SELECT PPRM.PRODUCTION_JW_ID, PPS.SHIFT_NAME, PPM.MACHINE_NUMBER,  SUBSTR(PPRM.ENTRY_DATE,0,2) || '-' || PPS.SHIFT_NAME || '-' || PPM.MACHINE_NUMBER  AS SHIFT_MACHINE, PS.SUPERVISOR_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PPRM.ITEM_WEIGHT, PPRM.ENTRY_DATE, PPRM.CREATE_DATE, PPRM.UPDATE_DATE, PPRM.IS_ACTIVE FROM PF_PRODUCTION_JW PPRM LEFT JOIN PF_PRODUCTION_SHIFT PPS ON PPS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_PRODUCTION_MACHINE PPM ON PPM.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID ORDER BY PPRM.CREATE_DATE DESC";
                }
                else
                {
                    if (DropDownItemID1.Text == "0")
                    {
                        makeSQL = " SELECT PPRM.PRODUCTION_JW_ID, PPS.SHIFT_NAME, PPM.MACHINE_NUMBER, SUBSTR(PPRM.ENTRY_DATE,0,2) || '-' || PPS.SHIFT_NAME || '-' || PPM.MACHINE_NUMBER  AS SHIFT_MACHINE, PS.SUPERVISOR_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PPRM.ITEM_WEIGHT, PPRM.ENTRY_DATE, PPRM.CREATE_DATE, PPRM.UPDATE_DATE, PPRM.IS_ACTIVE FROM PF_PRODUCTION_JW PPRM LEFT JOIN PF_PRODUCTION_SHIFT PPS ON PPS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_PRODUCTION_MACHINE PPM ON PPM.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID where PPRM.PRODUCTION_JW_ID like '" + txtSearchEmp.Text + "%' or PPS.SHIFT_NAME like '" + txtSearchEmp.Text + "%' or PS.SUPERVISOR_NAME  like '" + txtSearchEmp.Text + "%' or PPM.MACHINE_NUMBER like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PPRM.ENTRY_DATE like '" + txtSearchEmp.Text + "%' or PPRM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ORDER BY PPRM.CREATE_DATE DESC";
                    }
                    else
                    {
                        makeSQL = " SELECT PPRM.PRODUCTION_JW_ID, PPS.SHIFT_NAME, PPM.MACHINE_NUMBER, SUBSTR(PPRM.ENTRY_DATE,0,2) || '-' || PPS.SHIFT_NAME || '-' || PPM.MACHINE_NUMBER  AS SHIFT_MACHINE, PS.SUPERVISOR_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PPRM.ITEM_WEIGHT, PPRM.ENTRY_DATE, PPRM.CREATE_DATE, PPRM.UPDATE_DATE, PPRM.IS_ACTIVE FROM PF_PRODUCTION_JW PPRM LEFT JOIN PF_PRODUCTION_SHIFT PPS ON PPS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_PRODUCTION_MACHINE PPM ON PPM.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID where PI.ITEM_ID like '" + DropDownItemID1.Text + "%' AND (PPRM.PRODUCTION_JW_ID like '" + txtSearchEmp.Text + "%' or PPS.SHIFT_NAME like '" + txtSearchEmp.Text + "%' or PS.SUPERVISOR_NAME  like '" + txtSearchEmp.Text + "%' or PPM.MACHINE_NUMBER like '" + txtSearchEmp.Text + "%'  or PPRM.ENTRY_DATE like '" + txtSearchEmp.Text + "%' or PPRM.IS_ACTIVE like '" + txtSearchEmp.Text + "%') ORDER BY PPRM.CREATE_DATE DESC";
                    }
                    alert_box.Visible = false; 
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "PRODUCTION_JW_ID" };
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
                string Production_ID = TextProductionID.Text;    
                int ShiftID = Convert.ToInt32(DropDownShiftID.Text);
                int SupervisorID = Convert.ToInt32(DropDownSupervisorID.Text);
                int MachineID = Convert.ToInt32(DropDownMachineID.Text);
                double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
                int ItemID = Convert.ToInt32(DropDownItemID.Text);
                int SubItemID = Convert.ToInt32(DropDownSubItemID.Text);
                string ItemName = DropDownItemID.SelectedItem.Text;
                string SubItemName = "";
                if (SubItemID == 0)
                { SubItemID = 0; SubItemName = "";  }  else  {  SubItemID = Convert.ToInt32(DropDownSubItemID.Text);  SubItemName = DropDownSubItemID.SelectedItem.Text;
                }
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
               
                string MakeEntryDate = EntryDate.Text;
                string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                 
                // update  production jw master
                string update_production = "update  PF_PRODUCTION_JW  set SHIFT_ID = :NoShiftID, MACHINE_ID = :NoMachineID, SUPERVISOR_ID = :NoSupervisorID, ITEM_ID = :NoItemID, SUB_ITEM_ID = :NoSubItemID, ITEM_WEIGHT = :TextItemWeight, ENTRY_DATE = TO_DATE(:TextEntryDate, 'DD-MM-YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where PRODUCTION_JW_ID = :NoProductionID ";
                cmdu = new OracleCommand(update_production, conn);

                OracleParameter[] objPrm = new OracleParameter[11];
                objPrm[0] = cmdu.Parameters.Add("NoShiftID", ShiftID);
                objPrm[1] = cmdu.Parameters.Add("NoMachineID", MachineID);
                objPrm[2] = cmdu.Parameters.Add("NoSupervisorID", SupervisorID);
                objPrm[3] = cmdu.Parameters.Add("NoItemID", ItemID);
                objPrm[4] = cmdu.Parameters.Add("NoSubItemID", SubItemID); 
                objPrm[5] = cmdu.Parameters.Add("TextItemWeight", ItemWeight); 
                objPrm[6] = cmdu.Parameters.Add("TextEntryDate", EntryDateNew); 
                objPrm[7] = cmdu.Parameters.Add("u_date", u_date);
                objPrm[8] = cmdu.Parameters.Add("NoCuserID", userID);
                objPrm[9] = cmdu.Parameters.Add("TextIsActive", ISActive);
                objPrm[10] = cmdu.Parameters.Add("NoProductionID", Production_ID);

                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose();
 
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Job Work (Production) Data Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearText();
                DropDownShiftID.Focus();
                Display();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }

 
        protected void BtnDelete_Click(object sender, EventArgs e)
        { 
         try{ 
            if (IS_DELETE_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string Production_ID = TextProductionID.Text;  
                int ItemID = Convert.ToInt32(DropDownItemID.Text);
                int SubItemID = Convert.ToInt32(DropDownSubItemID.Text); 
                 
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
              
                // delete production master
                string delete_production = " delete from PF_PRODUCTION_JW where PRODUCTION_JW_ID  = '" + Production_ID + "'"; 
                cmdi = new OracleCommand(delete_production, conn); 
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                 
                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Job Work (Production) Data Delete Successfully"));
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
            TextProductionID.Text = ""; 
            TextItemWeight.Text = "";
            DropDownSubItemID.Text = "0"; 
            CheckItemWeight.Text = ""; 
            DropDownShiftID.Text = "0";
            DropDownSupervisorID.Text = "0";
            DropDownMachineID.Text = "0";
            DropDownItemID.Text = "0";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        {
            TextProductionID.Text = "";  
            TextItemWeight.Text = "";
            DropDownSubItemID.Text = "0"; 
            CheckItemWeight.Text = ""; 
            DropDownShiftID.Text = "0";
            DropDownSupervisorID.Text = "0";
            DropDownMachineID.Text = "0";
            DropDownItemID.Text = "0"; 
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

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

   
        public void TextSubItem_Changed(object sender, EventArgs e)
        {
            TextItemWeight.Focus();
            TextItemWeight.Text = "";
            alert_box.Visible = false;

        }

       

        public void TextItemWeight_TextChanged(object sender, EventArgs e)
        {
            int ItemID = Convert.ToInt32(DropDownItemID.Text);
            if (ItemID != 0)
            { 
            int SubItemID = Convert.ToInt32(DropDownSubItemID.Text);
            string ItemWeightCheck = TextItemWeight.Text;
            string MatchPattern = "([0-9.]+)";
            if (ItemWeightCheck != null)
            {

                if (Regex.IsMatch(ItemWeightCheck, MatchPattern))
                {
                    alert_box.Visible = false;

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    string makeSQL = " select nvl(sum(ITEM_WEIGHT),0) AS ITEM_WEIGHT from PF_PURCHASE_JW where ITEM_ID  = '" + ItemID + "'  AND SUB_ITEM_ID  = '" + SubItemID + "' ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    double FinalStock = 0.00;
                    double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
                    for (int i = 0; i < RowCount; i++)
                    {
                        FinalStock = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());
                    }
                     
                    if (ItemWeight <= FinalStock)
                    { 
                        CheckItemWeight.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Material is available</label>"; 
                        CheckItemWeight.ForeColor = System.Drawing.Color.Green;
                        EntryDate.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");  

                    }
                    else
                    {
                        CheckItemWeight.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Material is not available. Available Material is <span class='badge bg-yellow'>" + FinalStock + "</span> metric ton (MT)</label>";
                        CheckItemWeight.ForeColor = System.Drawing.Color.Red;
                        TextItemWeight.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

                    }
                }
                else
                {
                    CheckItemWeight.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter Material Weight</label>";
                    CheckItemWeight.ForeColor = System.Drawing.Color.Red;
                    TextItemWeight.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

                }
            }
            }
            else
            {
                alert_box.Visible = false;
                TextItemWeight.Text = "";
                CheckItemWeight.Text = "";
                DropDownItemID.Focus();
            } 

        }

        protected void BtnReport_Click(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad = true;
                alert_box.Visible = false;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }

        }
         
    }
          
}