using System; 
using System.Configuration;
using System.Data;
using System.Linq; 
using System.Web.UI; 
using System.Web.UI.WebControls; 
using System.Data.OracleClient;
using System.IO;  
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization; 


namespace NRCAPPS.MF
{
    public partial class MfMaterialTransfer : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdi, cmdl, cmdsp;
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
      //  public virtual void Select(bool directed, bool forward){}
        
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
                        DataTable dtID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeVehicleSQL = " SELECT * FROM MF_VEHICLE  WHERE IS_ACTIVE = 'Enable' ORDER BY VEHICLE_NO ASC";
                        ds = ExecuteBySqlString(makeVehicleSQL);
                        dtID = (DataTable)ds.Tables[0];
                        DropDownVehicleID.DataSource = dtID;
                        DropDownVehicleID.DataValueField = "VEHICLE_ID";
                        DropDownVehicleID.DataTextField = "VEHICLE_NO";
                        DropDownVehicleID.DataBind();
                        DropDownVehicleID.Items.Insert(0, new ListItem("Select Vehicle Number ", "0"));
                          
                          
                        DataTable dtMaterialID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeMaterialSQL = " SELECT MATERIAL_ID, MATERIAL_NAME || ' - ' || MATERIAL_CODE AS MATERIAL_FULL_NAME FROM MF_MATERIAL WHERE IS_ACTIVE = 'Enable' ORDER BY MATERIAL_ID ASC";
                        dsi = ExecuteBySqlString(makeMaterialSQL);
                        dtMaterialID = (DataTable)dsi.Tables[0];
                        DropDownMaterialID.DataSource = dtMaterialID;
                        DropDownMaterialID.DataValueField = "MATERIAL_ID";
                        DropDownMaterialID.DataTextField = "MATERIAL_FULL_NAME";
                        DropDownMaterialID.DataBind();
                        DropDownMaterialID.Items.Insert(0, new ListItem("Select  Material", "0"));

                        DropDownMaterialID1.DataSource = dtMaterialID;
                        DropDownMaterialID1.DataValueField = "MATERIAL_ID";
                        DropDownMaterialID1.DataTextField = "MATERIAL_FULL_NAME";
                        DropDownMaterialID1.DataBind();
                        DropDownMaterialID1.Items.Insert(0, new ListItem("Select  Material", "0"));                     
                          
                     //   TextWbSlipNo.Focus();
                     //   Page.SetFocus(TextWbSlipNo);
                   //     TextWbSlipNo.Select();
                      //  this.ActiveControl = TextWbSlipNo;
                         TextWbSlipNo.Focus();
                         TextWbSlipNo.Attributes.Add("onfocus", "this.select(); this.onfocus = null;");
                     //   TextWbSlipNo.Attributes.Add("onfocus", "selectText();");

                        Display();
                        DisplaySummary();
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
                    int VehicleID = Convert.ToInt32(DropDownVehicleID.Text);
                    int SlipNo = Convert.ToInt32(TextWbSlipNo.Text); 
                    int MaterialID   = Convert.ToInt32(DropDownMaterialID.Text);  
                  
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                     
                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-"); 
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                     
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                    double MatWeightMf = Convert.ToDouble(TextMatWeightMf.Text.Trim());
                    double MatWeightMs = Convert.ToDouble(TextMatWeightMs.Text.Trim());
                    double WeightDifference = MatWeightMf - MatWeightMs;
                     
                    string get_transfer_id = "select MF_MAT_TRANSFER_MASTERID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_transfer_id, conn);
                    int newTransferID = Int16.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_Transfer = "insert into  MF_MATERIAL_TRANSFER_MASTER (TRANSFER_ID, WB_SLIP_NO, VEHICLE_ID, MATERIAL_ID, WT_AS_PER_MF, WT_AS_PER_MS, DIFFERENCE, REMARKS, ENTRY_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID) values  ( :NoTransferID, :NoSlipID, :NoVehicleID, :NoMaterialID, :TextMatWeightMf, :TextMatWeightMs, :TextWeightDifference, :TextRemarks, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 5)";
                    cmdi = new OracleCommand(insert_Transfer, conn);
                      
                    OracleParameter[] objPrm = new OracleParameter[12];
                    objPrm[0] = cmdi.Parameters.Add("NoTransferID", newTransferID); 
                    objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNo); 
                    objPrm[2] = cmdi.Parameters.Add("NoVehicleID", VehicleID);  
                    objPrm[3] = cmdi.Parameters.Add("NoMaterialID", MaterialID); 
                    objPrm[4] = cmdi.Parameters.Add("TextMatWeightMf", MatWeightMf);
                    objPrm[5] = cmdi.Parameters.Add("TextMatWeightMs", MatWeightMs);
                    objPrm[6] = cmdi.Parameters.Add("TextWeightDifference", WeightDifference); 
                    objPrm[7] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[8] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text); 
                    objPrm[9] = cmdi.Parameters.Add("c_date", c_date); 
                    objPrm[10] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[11] = cmdi.Parameters.Add("TextIsActive", ISActive);

                    cmdi.ExecuteNonQuery(); 
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                     
                    conn.Close(); 

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Transfer Data successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                     
                    clearText();
                    TextWbSlipNo.Focus();
                    Display();
                    DisplaySummary();
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
            string makeSQL = " select TRANSFER_ID, WB_SLIP_NO,  VEHICLE_ID, MATERIAL_ID, WT_AS_PER_MF, WT_AS_PER_MS, DIFFERENCE, REMARKS,  TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, CREATE_DATE,  UPDATE_DATE,  C_USER_ID, U_USER_ID, IS_ACTIVE from MF_MATERIAL_TRANSFER_MASTER where TRANSFER_ID  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                TextTransferID.Text = dt.Rows[i]["TRANSFER_ID"].ToString();
                TextWbSlipNo.Text = dt.Rows[i]["WB_SLIP_NO"].ToString();
                DropDownVehicleID.Text = dt.Rows[i]["VEHICLE_ID"].ToString();
                DropDownMaterialID.Text = dt.Rows[i]["MATERIAL_ID"].ToString();
                TextMatWeightMf.Text = dt.Rows[i]["WT_AS_PER_MF"].ToString();
                TextMatWeightMs.Text = dt.Rows[i]["WT_AS_PER_MS"].ToString();
                TextWeightDifference.Text = decimal.Parse(dt.Rows[i]["DIFFERENCE"].ToString()).ToString(".00"); 
                EntryDate.Text = dt.Rows[i]["ENTRY_DATE"].ToString();
                TextRemarks.Text = dt.Rows[i]["REMARKS"].ToString();  
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
                    makeSQL = " SELECT  MMTM.*, MV.VEHICLE_NO, MM.MATERIAL_NAME, MM.MATERIAL_CODE FROM MF_MATERIAL_TRANSFER_MASTER MMTM LEFT JOIN MF_VEHICLE MV ON MV.VEHICLE_ID = MMTM.VEHICLE_ID LEFT JOIN MF_MATERIAL MM ON MM.MATERIAL_ID = MMTM.MATERIAL_ID  ORDER BY MMTM.CREATE_DATE DESC ";
                }
                else
                {
                    if (DropDownMaterialID1.Text == "0")
                    {
                        makeSQL = " SELECT  MMTM.*, MV.VEHICLE_NO, MM.MATERIAL_NAME, MM.MATERIAL_CODE FROM MF_MATERIAL_TRANSFER_MASTER MMTM LEFT JOIN MF_VEHICLE MV ON MV.VEHICLE_ID = MMTM.VEHICLE_ID LEFT JOIN MF_MATERIAL MM ON MM.MATERIAL_ID = MMTM.MATERIAL_ID  where MMTM.WB_SLIP like '" + txtSearchEmp.Text + "%' or PV.VEHICLE_NO like '" + txtSearchEmp.Text + "%' or MM.MATERIAL_NAME like '" + txtSearchEmp.Text + "%' or MMTM.ENTRY_DATE like '" + txtSearchEmp.Text + "%' or MMTM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' or MMTM.FIRST_APPROVED_IS like '" + txtSearchEmp.Text + "%' ORDER BY MMTM.WB_SLIP asc";  // MMTM.CREATE_DATE desc, MMTM.UPDATE_DATE desc
                    }
                    else
                    {
                     //  makeSQL = " SELECT MMTM.TRANSFER_ID, MMTM.SLIP_NO, MMTM.VEHICLE_ID, PP.PARTY_NAME, MMTM.PUR_TYPE_ID, PPT.PUR_TYPE_NAME, MMTM.ITEM_ID, PI.ITEM_NAME, MMTM.SUB_ITEM_ID, PSI.SUB_ITEM_NAME, MMTM.SUPERVISOR_ID, PS.SUPERVISOR_NAME, MMTM.ITEM_WEIGHT, MMTM.ITEM_RATE, MMTM.ITEM_AMOUNT, MMTM.ENTRY_DATE, MMTM.CREATE_DATE, MMTM.UPDATE_DATE, MMTM.IS_ACTIVE , PPC.IS_CHECK FROM MF_MATERIAL_TRANSFER_MASTER MMTM LEFT JOIN PF_PARTY PP ON PP.VEHICLE_ID = MMTM.VEHICLE_ID LEFT JOIN PF_Transfer_TYPE PPT ON PPT.PUR_TYPE_ID = MMTM.PUR_TYPE_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = MMTM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = MMTM.SUB_ITEM_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = MMTM.SUPERVISOR_ID  LEFT JOIN PF_Transfer_CLAIM PPC ON  PPC.CLAIM_NO = MMTM.CLAIM_NO where  PI.ITEM_ID like '" + DropDownMaterialID1.Text + "%' AND (MMTM.SLIP_NO like '" + txtSearchEmp.Text + "%' or MMTM.VEHICLE_ID like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%'  or PS.SUPERVISOR_NAME like '" + txtSearchEmp.Text + "%' or MMTM.ITEM_RATE like '" + txtSearchEmp.Text + "%' or PPM.ENTRY_DATE like '" + txtSearchEmp.Text + "%' or PPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' or PPC.IS_CHECK like '" + txtSearchEmp.Text + "%') ORDER BY PPM.SLIP_NO asc";  // PPM.CREATE_DATE desc, PPM.UPDATE_DATE desc
                    }

                    alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "TRANSFER_ID" };
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


        public void DisplaySummary()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string CurrentMonth = System.DateTime.Now.ToString("MM/yyyy");
                string makeSQL = "";
                if (TextMonthYear4.Text == "")
                {
                    makeSQL = " SELECT  MM.MATERIAL_NAME, MM.MATERIAL_CODE, count(MMTM.WB_SLIP_NO) AS SLIP_NO, sum(MMTM.WT_AS_PER_MF) AS WT_AS_PER_MF, sum(MMTM.WT_AS_PER_MS) AS WT_AS_PER_MS, sum(MMTM.DIFFERENCE) AS DIFFERENCE FROM MF_MATERIAL MM LEFT JOIN MF_MATERIAL_TRANSFER_MASTER MMTM ON MMTM.MATERIAL_ID = MM.MATERIAL_ID WHERE to_char(MMTM.ENTRY_DATE, 'mm/yyyy') = '" + CurrentMonth + "' GROUP BY MM.MATERIAL_ID, MM.MATERIAL_NAME, MM.MATERIAL_CODE ORDER BY MM.MATERIAL_ID ";
                }
                else
                {
                    makeSQL = " SELECT  MM.MATERIAL_NAME, MM.MATERIAL_CODE, count(MMTM.WB_SLIP_NO) AS SLIP_NO, sum(MMTM.WT_AS_PER_MF) AS WT_AS_PER_MF, sum(MMTM.WT_AS_PER_MS) AS WT_AS_PER_MS, sum(MMTM.DIFFERENCE) AS DIFFERENCE FROM MF_MATERIAL MM LEFT JOIN MF_MATERIAL_TRANSFER_MASTER MMTM ON MMTM.MATERIAL_ID = MM.MATERIAL_ID WHERE to_char(MMTM.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear4.Text + "' GROUP BY MM.MATERIAL_ID, MM.MATERIAL_NAME, MM.MATERIAL_CODE ORDER BY MM.MATERIAL_ID ";
                    alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataKeyNames = new string[] { "MATERIAL_NAME" };
                GridView2.DataBind();
                if (dt.Rows.Count > 0)
                {
                    GridView2.HeaderRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                    //Calculate Sum and display in Footer Row
                    GridView2.FooterRow.Cells[1].Font.Bold = true;
                    GridView2.FooterRow.Cells[1].Text = "Grand Total";
                    GridView2.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Right;

                    decimal total_prod = dt.AsEnumerable().Sum(row => row.Field<decimal>("SLIP_NO"));
                    GridView2.FooterRow.Cells[2].Font.Bold = true;
                    GridView2.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Center;
                    GridView2.FooterRow.Cells[2].Text = total_prod.ToString("N0");

                    decimal total_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("WT_AS_PER_MF"));
                    GridView2.FooterRow.Cells[3].Font.Bold = true;
                    GridView2.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[3].Text = total_wt.ToString("N3");

                    decimal total_amt = dt.AsEnumerable().Sum(row => row.Field<decimal>("WT_AS_PER_MS"));
                    GridView2.FooterRow.Cells[4].Font.Bold = true;
                    GridView2.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[4].Text = total_amt.ToString("N2");

                    decimal total_avg = (total_wt - total_amt);
                    GridView2.FooterRow.Cells[5].Font.Bold = true;
                    GridView2.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[5].Text = total_avg.ToString("N2");
                }
                else
                {

                }
                conn.Close();
                // alert_box.Visible = false;
            }
        }

        protected void GridViewSearchSummary(object sender, EventArgs e)
        {
            this.DisplaySummary();
        }

        protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            DisplaySummary();
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
                int TransferID = Convert.ToInt32(TextTransferID.Text); 
                int VehicleID = Convert.ToInt32(DropDownVehicleID.Text); 
                int MaterialID = Convert.ToInt32(DropDownMaterialID.Text);  
                 
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                string MakeEntryDate = EntryDate.Text;
                string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                double MatWeightMf = Convert.ToDouble(TextMatWeightMf.Text.Trim());
                double MatWeightMs = Convert.ToDouble(TextMatWeightMs.Text.Trim());
                double WeightDifference = MatWeightMf - MatWeightMs;
                    
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                 
                // Transfer master update

                string update_user = "update  MF_MATERIAL_TRANSFER_MASTER  set WB_SLIP_NO = :NoSlipID, VEHICLE_ID = :NoVehicleID, MATERIAL_ID = :NoMaterialID, WT_AS_PER_MF = :NoMatWeightMf, WT_AS_PER_MS = :NoMatWeightMs, DIFFERENCE = :NoWeightDifference, REMARKS = :TextRemarks, ENTRY_DATE = TO_DATE(:EntryDate, 'DD/MM/YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where TRANSFER_ID = :NoTransferID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[12];
                objPrm[0] = cmdi.Parameters.Add("NoSlipID", TextWbSlipNo.Text); 
                objPrm[1] = cmdi.Parameters.Add("NoVehicleID", VehicleID);                 
                objPrm[2] = cmdi.Parameters.Add("NoMaterialID", MaterialID); 
                objPrm[3] = cmdi.Parameters.Add("NoMatWeightMf", MatWeightMf);
                objPrm[4] = cmdi.Parameters.Add("NoMatWeightMs", MatWeightMs);
                objPrm[5] = cmdi.Parameters.Add("NoWeightDifference", WeightDifference);
                objPrm[6] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                objPrm[7] = cmdi.Parameters.Add("EntryDate", EntryDateNew); 
                objPrm[8] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[9] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[10] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[11] = cmdi.Parameters.Add("NoTransferID", TransferID);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Transfer Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearText();
                TextWbSlipNo.Focus();
                Display();
                DisplaySummary();
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
                int TransferID = Convert.ToInt32(TextTransferID.Text);  
                 
                string delete_user = " delete from MF_MATERIAL_TRANSFER_MASTER where TRANSFER_ID  = '" + TransferID + "'"; 
                cmdi = new OracleCommand(delete_user, conn); 
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Transfer Data Delete successfully"));
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
            TextWbSlipNo.Text = ""; 
            TextMatWeightMf.Text = "";
            TextMatWeightMs.Text = ""; 
            TextWeightDifference.Text = "";
            CheckSlipNo.Text = ""; 
            DropDownVehicleID.Text = "0"; 
            DropDownMaterialID.Text = "0";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        {
            TextWbSlipNo.Text = ""; 
            TextMatWeightMf.Text = "";
            TextMatWeightMs.Text = ""; 
            TextWeightDifference.Text = "";
            CheckSlipNo.Text = ""; 
            DropDownVehicleID.Text = "0"; 
            DropDownMaterialID.Text = "0"; 
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void TextWbSlipNo_TextChanged(object sender, EventArgs e)
        {
            string SlipNo = TextWbSlipNo.Text;
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
                    cmd.CommandText = "select WB_SLIP_NO from MF_MATERIAL_TRANSFER_MASTER where WB_SLIP_NO = '" + Convert.ToInt32(SlipNo) + "'";
                    cmd.CommandType = CommandType.Text;

                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        CheckSlipNo.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Slip Number is not available</label>";
                        CheckSlipNo.ForeColor = System.Drawing.Color.Red;
                        TextWbSlipNo.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                    }
                    else
                    {
                        CheckSlipNo.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Slip Number is available</label>";
                        CheckSlipNo.ForeColor = System.Drawing.Color.Green;
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");

                    }
                }
                else
                {
                    CheckSlipNo.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter Slip Number is 6 digit only</label>";
                    CheckSlipNo.ForeColor = System.Drawing.Color.Red;
                    TextWbSlipNo.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

                }
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