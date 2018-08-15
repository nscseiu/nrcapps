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
    public partial class PfPurchaseJw : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl, cmdsp;
        OracleDataAdapter oradata; 
        DataTable dt;
        int RowCount;

        string IS_PAGE_ACTIVE = "";
        string IS_ADD_ACTIVE = "";
        string IS_EDIT_ACTIVE = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE = "";
        string IS_REPORT_ACTIVE = "";

        public bool IsLoad { get; set; }  public bool IsLoad2 { get; set; } public bool IsLoad3 { get; set; } public bool IsLoad4 { get; set; } 
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
                        DataTable dtSupplierID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeSupplierSQL = " SELECT * FROM PF_PARTY WHERE IS_ACTIVE = 'Enable' ORDER BY PARTY_NAME ASC";
                        ds = ExecuteBySqlStringEmpType(makeSupplierSQL);
                        dtSupplierID = (DataTable)ds.Tables[0];
                        DropDownSupplierID.DataSource = dtSupplierID;
                        DropDownSupplierID.DataValueField = "PARTY_ID";
                        DropDownSupplierID.DataTextField = "PARTY_NAME";
                        DropDownSupplierID.DataBind();
                        DropDownSupplierID.Items.Insert(0, new ListItem("Select  Supplier", "0"));

                        DropDownSupplierID2.DataSource = dtSupplierID;
                        DropDownSupplierID2.DataValueField = "PARTY_ID";
                        DropDownSupplierID2.DataTextField = "PARTY_NAME";
                        DropDownSupplierID2.DataBind();
                        DropDownSupplierID2.Items.Insert(0, new ListItem("Select  Supplier", "0"));
                           
                        DataTable dtSupervisorID = new DataTable();
                        DataSet dsd = new DataSet();
                        string makeSupervisorSQL = " SELECT * FROM PF_SUPERVISOR WHERE IS_ACTIVE = 'Enable' ORDER BY SUPERVISOR_NAME ASC";
                        dsd = ExecuteBySqlStringEmpType(makeSupervisorSQL);
                        dtSupplierID = (DataTable)dsd.Tables[0];
                        DropDownSupervisorID.DataSource = dtSupplierID;
                        DropDownSupervisorID.DataValueField = "SUPERVISOR_ID";
                        DropDownSupervisorID.DataTextField = "SUPERVISOR_NAME";
                        DropDownSupervisorID.DataBind();
                        DropDownSupervisorID.Items.Insert(0, new ListItem("Select  Supervisor", "0"));
                           
                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeDropDownItemSQL = " SELECT * FROM PF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        dsi = ExecuteBySqlStringEmpType(makeDropDownItemSQL);
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
                        dss = ExecuteBySqlStringEmpType(makeDropDownSubItemSQL);
                        dtSubItemID = (DataTable)dss.Tables[0];
                        DropDownSubItemID.DataSource = dtSubItemID;
                        DropDownSubItemID.DataValueField = "SUB_ITEM_ID";
                        DropDownSubItemID.DataTextField = "SUB_ITEM_NAME";
                        DropDownSubItemID.DataBind();
                        DropDownSubItemID.Items.Insert(0, new ListItem("Select Sub Item", "0"));


                        TextSlipNo.Focus();
                        

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
                    int SupplierID = Convert.ToInt32(DropDownSupplierID.Text);
                    int SlipNo = Convert.ToInt32(TextSlipNo.Text);
                    int SupervisorID   = Convert.ToInt32(DropDownSupervisorID.Text); 
                    int ItemID   = Convert.ToInt32(DropDownItemID.Text);
                    int SubItemID   = Convert.ToInt32(DropDownSubItemID.Text);
                    string ItemName = DropDownItemID.SelectedItem.Text;
                    string SubItemName = "";
                    if (SubItemID == 0)
                    {
                        SubItemID = 0;
                        SubItemName = "";
                    } else { 
                        SubItemID = Convert.ToInt32(DropDownSubItemID.Text);
                        SubItemName =  DropDownSubItemID.SelectedItem.Text;
                    }
                     
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                     
                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-"); 
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                     
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string get_user_purchase_id = "select PF_PURCHASE_JWID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_user_purchase_id, conn);
                    int newPurchaseID = Int16.Parse(cmdsp.ExecuteScalar().ToString());
                    double ItemWeight = Convert.ToDouble(TextItemWeight.Text);

                    string insert_purchase = "insert into  PF_PURCHASE_JW (PURCHASE_JW_ID, SLIP_NO, PARTY_ID, ITEM_ID, SUB_ITEM_ID, SUPERVISOR_ID, ITEM_WEIGHT, ENTRY_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID) values  ( :NoPurchaseID, :NoSlipID, :NoSupplierID, :NoItemID, :NoSubItemID, :NoSupervisorID, :TextItemWeight, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 3)";
                    cmdi = new OracleCommand(insert_purchase, conn);
                      
                    OracleParameter[] objPrm = new OracleParameter[11];
                    objPrm[0] = cmdi.Parameters.Add("NoPurchaseID", newPurchaseID); 
                    objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNo); 
                    objPrm[2] = cmdi.Parameters.Add("NoSupplierID", SupplierID);  
                    objPrm[3] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[4] = cmdi.Parameters.Add("NoSubItemID", SubItemID); 
                    objPrm[5] = cmdi.Parameters.Add("NoSupervisorID", SupervisorID);
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
                    alert_box.Controls.Add(new LiteralControl("Insert new Job Work (Purchase) Successfully"));
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
          try{
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
            int USER_DATA_ID = Convert.ToInt32(Session["user_data_id"]);


            DataTable dtUserTypeID = new DataTable();
            DataSet ds = new DataSet();
            string makeSQL = " select PURCHASE_JW_ID, SLIP_NO,  PARTY_ID, ITEM_ID, SUB_ITEM_ID, SUPERVISOR_ID, ITEM_WEIGHT,  TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, CREATE_DATE,  UPDATE_DATE,  C_USER_ID, U_USER_ID, IS_ACTIVE from PF_PURCHASE_JW where SLIP_NO  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                TextSlipNo.Text = dt.Rows[i]["SLIP_NO"].ToString();
                DropDownSupplierID.Text = dt.Rows[i]["PARTY_ID"].ToString();
                DropDownSupervisorID.Text = dt.Rows[i]["SUPERVISOR_ID"].ToString(); 
                DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();
                DropDownSubItemID.Text = dt.Rows[i]["SUB_ITEM_ID"].ToString(); 
                TextItemWeight.Text  = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT"].ToString()).ToString(".000"); 
                EntryDate.Text = dt.Rows[i]["ENTRY_DATE"].ToString();  
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false); 
            }

            conn.Close();
            Display();
            CheckSlipNo.Text = "";
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
                string makeSQL = "";
                if (txtSearchEmp.Text == "")
                {
                    makeSQL = " SELECT PPM.PURCHASE_JW_ID, PPM.SLIP_NO, PPM.PARTY_ID, PP.PARTY_NAME, PPM.ITEM_ID, PI.ITEM_NAME, PPM.SUB_ITEM_ID, PSI.SUB_ITEM_NAME, PPM.SUPERVISOR_ID, PS.SUPERVISOR_NAME, PPM.ITEM_WEIGHT, PPM.ENTRY_DATE, PPM.CREATE_DATE, PPM.UPDATE_DATE, PPM.IS_ACTIVE , PPC.IS_CHECK FROM PF_PURCHASE_JW PPM LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PPM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPM.SUPERVISOR_ID  LEFT JOIN PF_PURCHASE_CLAIM PPC ON  PPC.CLAIM_NO = PPM.CLAIM_NO ORDER BY PPM.CREATE_DATE DESC ";
                }
                else
                {
                    if (DropDownItemID1.Text == "0")
                    {
                        makeSQL = " SELECT PPM.PURCHASE_JW_ID, PPM.SLIP_NO, PPM.PARTY_ID, PP.PARTY_NAME, PPM.ITEM_ID, PI.ITEM_NAME, PPM.SUB_ITEM_ID, PSI.SUB_ITEM_NAME, PPM.SUPERVISOR_ID, PS.SUPERVISOR_NAME, PPM.ITEM_WEIGHT, PPM.ENTRY_DATE, PPM.CREATE_DATE, PPM.UPDATE_DATE, PPM.IS_ACTIVE , PPC.IS_CHECK FROM PF_PURCHASE_JW PPM LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PPM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPM.SUPERVISOR_ID  LEFT JOIN PF_PURCHASE_CLAIM PPC ON  PPC.CLAIM_NO = PPM.CLAIM_NO where PPM.SLIP_NO like '" + txtSearchEmp.Text + "%' or PPM.PARTY_ID like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PS.SUPERVISOR_NAME like '" + txtSearchEmp.Text + "%'  or PPM.ENTRY_DATE like '" + txtSearchEmp.Text + "%' or PPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' or PPC.IS_CHECK like '" + txtSearchEmp.Text + "%' ORDER BY PPM.SLIP_NO asc";  // PPM.CREATE_DATE desc, PPM.UPDATE_DATE desc
                    }
                    else
                    {
                        makeSQL = " SELECT PPM.PURCHASE_JW_ID, PPM.SLIP_NO, PPM.PARTY_ID, PP.PARTY_NAME, PPM.ITEM_ID, PI.ITEM_NAME, PPM.SUB_ITEM_ID, PSI.SUB_ITEM_NAME, PPM.SUPERVISOR_ID, PS.SUPERVISOR_NAME, PPM.ITEM_WEIGHT, PPM.ENTRY_DATE, PPM.CREATE_DATE, PPM.UPDATE_DATE, PPM.IS_ACTIVE , PPC.IS_CHECK FROM PF_PURCHASE_JW PPM LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PPM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPM.SUPERVISOR_ID  LEFT JOIN PF_PURCHASE_CLAIM PPC ON  PPC.CLAIM_NO = PPM.CLAIM_NO where  PI.ITEM_ID like '" + DropDownItemID1.Text + "%' AND (PPM.SLIP_NO like '" + txtSearchEmp.Text + "%' or PPM.PARTY_ID like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%'  or PS.SUPERVISOR_NAME like '" + txtSearchEmp.Text + "%'  or PPM.ENTRY_DATE like '" + txtSearchEmp.Text + "%' or PPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' or PPC.IS_CHECK like '" + txtSearchEmp.Text + "%') ORDER BY PPM.SLIP_NO asc";  // PPM.CREATE_DATE desc, PPM.UPDATE_DATE desc
                    }

                    alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "PURCHASE_JW_ID" };
                GridView1.DataBind();
                conn.Close();
                // alert_box.Visible = false;
            }
        }

        protected void gridViewFileInformation_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow Row in GridView1.Rows)
            {
                string isCheck = (Row.FindControl("IsCmoCheckLink") as Label).Text;
                if (isCheck == "Complete")
                {
                    (Row.FindControl("linkSelectClick") as LinkButton).Visible = false;
                }
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
          try
           {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int SlipNo = Convert.ToInt32(TextSlipNo.Text); 
                int SupplierID = Convert.ToInt32(DropDownSupplierID.Text);
                int SupervisorID = Convert.ToInt32(DropDownSupervisorID.Text);
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
                double ItemWeight = Convert.ToDouble(TextItemWeight.Text);  
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                
                // purchase master update

                string update_user = "update  PF_PURCHASE_JW  set PARTY_ID = :NoSupplierID, ITEM_ID = :NoItemID, SUB_ITEM_ID = :NoSubItemID, SUPERVISOR_ID = :NoSupervisorID, ITEM_WEIGHT = :NoItemWeight, ENTRY_DATE = TO_DATE(:EntryDate, 'DD/MM/YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where SLIP_NO = :NoSlipNo ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[11];
                objPrm[1] = cmdi.Parameters.Add("NoSupplierID", SupplierID);            
                objPrm[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                objPrm[3] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                objPrm[4] = cmdi.Parameters.Add("NoSupervisorID", SupervisorID); 
                objPrm[5] = cmdi.Parameters.Add("NoItemWeight", ItemWeight);
                objPrm[6] = cmdi.Parameters.Add("EntryDate", EntryDateNew); 
                objPrm[7] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[8] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[9] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[10] = cmdi.Parameters.Add("NoSlipNo", SlipNo);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Job Work (Purchase) Data Update successfully"));
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

         
        protected void BtnDelete_Click(object sender, EventArgs e)
        { 
          try
            {
            if (IS_DELETE_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open(); 
                int userID = Convert.ToInt32(Session["USER_ID"]);                
                int SlipNo = Convert.ToInt32(TextSlipNo.Text);
               
                string delete_user = " delete from PF_PURCHASE_JW where SLIP_NO  = '" + SlipNo + "'"; 
                cmdi = new OracleCommand(delete_user, conn); 
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Job Work (Purchase) Data Delete successfully"));
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
            TextSlipNo.Text = "";  
            TextItemWeight.Text = "";
            DropDownSubItemID.Text = "0"; 
            CheckSlipNo.Text = ""; 
            DropDownSupplierID.Text = "0";  
            DropDownItemID.Text = "0";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        {
            TextSlipNo.Text = "";  
            TextItemWeight.Text = "";
            DropDownSubItemID.Text = "0"; 
            CheckSlipNo.Text = ""; 
            DropDownSupplierID.Text = "0";
            DropDownSupervisorID.Text = "0"; 
            DropDownItemID.Text = "0"; 
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

        
        public void TextSlipNo_TextChanged(object sender, EventArgs e)
        {
            string SlipNo = TextSlipNo.Text;
            string MatchEmpIDPattern = "^([0-9]{6})$";
            if (SlipNo != null)
            {

                if (Regex.IsMatch(SlipNo, MatchEmpIDPattern))
                {
                    alert_box.Visible = false;

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "select SLIP_NO from PF_PURCHASE_JW where SLIP_NO = '" + Convert.ToInt32(SlipNo) + "'";
                    cmd.CommandType = CommandType.Text;

                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        CheckSlipNo.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Slip Number is not available</label>";
                        CheckSlipNo.ForeColor = System.Drawing.Color.Red;
                        TextSlipNo.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

                         
                    }
                    else
                    {
                        CheckSlipNo.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Slip Number is available</label>";
                        CheckSlipNo.ForeColor = System.Drawing.Color.Green;
                        DropDownSupplierID.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");
                         
                    }
                }
                else
                {
                    CheckSlipNo.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter Slip Number is 6 digit only</label>";
                    CheckSlipNo.ForeColor = System.Drawing.Color.Red;
                    TextSlipNo.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

                }
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
        protected void BtnReport2_Click(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad2 = true;
                alert_box.Visible = false;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }

        }
        
        

    }
}