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
    public partial class PfSalesJw : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl, cmdsp;
        OracleDataAdapter oradata; 
        DataTable dt, dtr;
        int RowCount;

        string IS_PAGE_ACTIVE = "";
        string IS_ADD_ACTIVE = "";
        string IS_EDIT_ACTIVE = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE = "";
        string IS_REPORT_ACTIVE = "";

        public bool IsLoad { get; set; } public bool IsLoad1 { get; set; } public bool IsLoad2 { get; set; } public bool IsLoad3 { get; set; }
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
                        DataTable dtCustomerID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeCustomerSQL = " SELECT PP.PARTY_ID, PP.PARTY_NAME FROM PF_PURCHASE_JW PPJ LEFT JOIN PF_PARTY PP ON PP.PARTY_ID =  PPJ.PARTY_ID WHERE PP.IS_ACTIVE = 'Enable' GROUP BY  PP.PARTY_ID, PP.PARTY_NAME ORDER BY PP.PARTY_NAME ASC";
                        ds = ExecuteBySqlStringEmpType(makeCustomerSQL);
                        dtCustomerID = (DataTable)ds.Tables[0];
                        DropDownCustomerID.DataSource = dtCustomerID;
                        DropDownCustomerID.DataValueField = "PARTY_ID";
                        DropDownCustomerID.DataTextField = "PARTY_NAME";
                        DropDownCustomerID.DataBind();
                        DropDownCustomerID.Items.Insert(0, new ListItem("Select Customer", "0")); 
                           
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

                        DataTable dtPgeID = new DataTable();
                        DataSet dsp = new DataSet();
                        string makeDropDownPgeSQL = " SELECT * FROM NRC_VAT WHERE IS_ACTIVE = 'Enable' ORDER BY VAT_ID ASC";
                        dsp = ExecuteBySqlStringEmpType(makeDropDownPgeSQL);
                        dtPgeID = (DataTable)dsp.Tables[0];
                        DropDownVatID.DataSource = dtPgeID;
                        DropDownVatID.DataValueField = "VAT_ID";
                        DropDownVatID.DataTextField = "VAT_PERCENT";
                        DropDownVatID.DataBind();
                     //   DropDownVatID.Items.Insert(0, new ListItem("Select Garbage Est. of Prod.", "0"));

                        DropDownCustomerID.Attributes.Add("disabled", "disabled");
                      //  TextItemWeight.Attributes.Add("disabled", "disabled");

                        TextInvoiceNo.Focus();

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
                    string InvoiceNo = TextInvoiceNo.Text; 
                    int CustomerID = Convert.ToInt32(DropDownCustomerID.Text);  
                    int ItemID = Convert.ToInt32(DropDownItemID.Text);
                    double ItemRate = Convert.ToDouble(TextItemRate.Text);
                    double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
                    double ItemAmount = ItemRate * ItemWeight;
                    int VatID = Convert.ToInt32(DropDownVatID.Text);
                    double VatPercent = Convert.ToDouble(DropDownVatID.SelectedItem.Text);
                    double VatAmount = (ItemAmount * VatPercent) / 100; 

                    int SubItemID = Convert.ToInt32(DropDownSubItemID.Text);
                    string ItemName = DropDownItemID.SelectedItem.Text;
                    string SubItemName = ""; 
                    if (SubItemID == 0)  { SubItemID = 0; SubItemName = "";  } else { SubItemID = Convert.ToInt32(DropDownSubItemID.Text); SubItemName = DropDownSubItemID.SelectedItem.Text;  }
                    string Remarks = TextRemarks.Text; 
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable"; 
                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                     
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                       
                    // insert sales data
                    string get_user_production_id = "select PF_SALES_JWID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_user_production_id, conn);
                    int newSalesID = Int16.Parse(cmdu.ExecuteScalar().ToString());
                    string insert_production = "insert into  PF_SALES_JW (SALES_JW_ID, INVOICE_NO, PARTY_ID, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, ITEM_RATE, ITEM_AMOUNT, VAT_ID, VAT_PERCENT, VAT_AMOUNT, REMARKS, ENTRY_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, IS_OBJ_QUERY, DIVISION_ID) values ( :NoSalesID, :NoInvoiceNo, :NoCustomerID, :NoItemID, :NoSubItemID,  :TextItemWeight, :TextItemRate, :TextItemAmount, :NoVatID, :NoVatPercent, :NoVatAmount, :TextRemarks, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, :TextIObjQuery, 3)";
                    cmdi = new OracleCommand(insert_production, conn);

                    OracleParameter[] objPrm = new OracleParameter[17];
                    objPrm[0] = cmdi.Parameters.Add("NoSalesID", newSalesID);
                    objPrm[1] = cmdi.Parameters.Add("NoInvoiceNo", InvoiceNo); 
                    objPrm[2] = cmdi.Parameters.Add("NoCustomerID", CustomerID);
                    objPrm[3] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[4] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                    objPrm[5] = cmdi.Parameters.Add("TextItemWeight", ItemWeight);
                    objPrm[6] = cmdi.Parameters.Add("TextItemRate", ItemRate);
                    objPrm[7] = cmdi.Parameters.Add("TextItemAmount", ItemAmount);
                    objPrm[8] = cmdi.Parameters.Add("NoVatID", VatID);
                    objPrm[9] = cmdi.Parameters.Add("NoVatPercent", VatPercent);
                    objPrm[10] = cmdi.Parameters.Add("NoVatAmount", VatAmount);
                    objPrm[11] = cmdi.Parameters.Add("TextRemarks", Remarks);
                    objPrm[12] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[13] = cmdi.Parameters.Add("c_date", c_date);
                    objPrm[14] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[15] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[16] = cmdi.Parameters.Add("TextIObjQuery", "No");  

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                     
                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Job Work (Sales) Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                     
                    clearText();
                    TextInvoiceNo.Focus();
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
            string USER_DATA_ID = Session["user_data_id"].ToString();


            DataTable dtUserTypeID = new DataTable();
            DataSet ds = new DataSet();
            string makeSQL = " select SALES_JW_ID, INVOICE_NO, PARTY_ID, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, ITEM_RATE, ITEM_AMOUNT, VAT_ID, VAT_PERCENT, VAT_AMOUNT, REMARKS, TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, CREATE_DATE, UPDATE_DATE, IS_ACTIVE from PF_SALES_JW where INVOICE_NO  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            { 
                TextInvoiceNo.Text          = dt.Rows[i]["INVOICE_NO"].ToString();
                DropDownCustomerID.Text     = dt.Rows[i]["PARTY_ID"].ToString(); 
                DropDownItemID.Text         = dt.Rows[i]["ITEM_ID"].ToString();
                DropDownSubItemID.Text      = dt.Rows[i]["SUB_ITEM_ID"].ToString(); 
                TextItemRate.Text           = dt.Rows[i]["ITEM_RATE"].ToString();
                TextItemWeight.Text         = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT"].ToString()).ToString("0.000");
                TextItemAmount.Text         = decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString("0.00"); 
                DropDownVatID.Text          = dt.Rows[i]["VAT_ID"].ToString();
                TextVatAmount.Text          = decimal.Parse(dt.Rows[i]["VAT_AMOUNT"].ToString()).ToString("0.00");
                TextRemarks.Text            = dt.Rows[i]["REMARKS"].ToString();
                EntryDate.Text              = dt.Rows[i]["ENTRY_DATE"].ToString(); 
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false); 
            }

            conn.Close();
            Display();
            CheckItemWeight.Text = "";
            CheckInvoiceNo.Text = "";
            DropDownCustomerID.Attributes.Remove("disabled");
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
                    makeSQL = " SELECT PSM.SALES_JW_ID, PSM.INVOICE_NO, PP.PARTY_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_CHECK, PSM.IS_OBJ_QUERY, PSM.OBJ_QUERY_DES, PSM.OBJ_QUERY_C_DATE FROM PF_SALES_JW PSM LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PSM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID  WHERE PSM.IS_SALES_RETURN IS NULL ORDER BY PSM.CREATE_DATE DESC";
                }
                else
                {
                    if (DropDownItemID1.Text == "0")
                    {
                        makeSQL = " SELECT PSM.SALES_JW_ID, PSM.INVOICE_NO, PC.CUSTOMER_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_CHECK, PSM.IS_OBJ_QUERY, PSM.OBJ_QUERY_DES, PSM.OBJ_QUERY_C_DATE FROM PF_SALES_JW PSM LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PSM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID WHERE  PSM.IS_SALES_RETURN IS NULL AND (PSM.INVOICE_NO like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%' or PC.CUSTOMER_NAME like '" + txtSearchEmp.Text + "%' or PSI.SUB_ITEM_NAME like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'mm/yyyy') like '" + txtSearchEmp.Text + "%' or PSM.IS_ACTIVE like '" + txtSearchEmp.Text + "%'  or PSM.IS_CHECK like '" + txtSearchEmp.Text + "%') ORDER BY PSM.CREATE_DATE desc, PSM.UPDATE_DATE desc";
                    }
                    else
                    {
                        makeSQL = " SELECT PSM.SALES_JW_ID, PSM.INVOICE_NO, PC.CUSTOMER_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_CHECK, PSM.IS_OBJ_QUERY, PSM.OBJ_QUERY_DES, PSM.OBJ_QUERY_C_DATE FROM PF_SALES_JW PSM LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PSM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID WHERE  PSM.IS_SALES_RETURN IS NULL AND PI.ITEM_ID like '" + DropDownItemID1.Text + "%' AND (PSM.INVOICE_NO like '" + txtSearchEmp.Text + "%'  or PC.CUSTOMER_NAME like '" + txtSearchEmp.Text + "%'  or PSI.SUB_ITEM_NAME like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'mm/yyyy') like '" + txtSearchEmp.Text + "%' or PSM.IS_ACTIVE like '" + txtSearchEmp.Text + "%'  or PSM.IS_CHECK like '" + txtSearchEmp.Text + "%') ORDER BY PSM.CREATE_DATE desc, PSM.UPDATE_DATE desc";
                    }
                    alert_box.Visible = false;
               }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "INVOICE_NO" };
                GridView1.DataBind();
                conn.Close(); 
            }
        }

    /*    protected void gridViewFileInformation_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow Row in GridView1.Rows)
            {
                string isCheck = (Row.FindControl("IsActiveCheckLink") as Label).Text;
                if (isCheck == "Complete")
                {
                    (Row.FindControl("linkSelectClick") as LinkButton).Visible = false;
                }
            }
        } */

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
        try {
                
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string InvoiceNo = TextInvoiceNo.Text;
                int CustomerID = Convert.ToInt32(DropDownCustomerID.Text); 
                int ItemID = Convert.ToInt32(DropDownItemID.Text);
                double ItemRate = Convert.ToDouble(TextItemRate.Text);
                double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
                double ItemAmount = ItemRate * ItemWeight;
                int VatID = Convert.ToInt32(DropDownVatID.Text);
                double VatPercent = Convert.ToDouble(DropDownVatID.SelectedItem.Text);
                double VatAmount = (ItemAmount * VatPercent) / 100; 
                int SubItemID = Convert.ToInt32(DropDownSubItemID.Text);
                string ItemName = DropDownItemID.SelectedItem.Text;
                string SubItemName = "";
                if (SubItemID == 0)
                {  SubItemID = 0;  SubItemName = ""; }  else  {  SubItemID = Convert.ToInt32(DropDownSubItemID.Text);  SubItemName = DropDownSubItemID.SelectedItem.Text; }
                string Remarks = TextRemarks.Text;
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string MakeEntryDate = EntryDate.Text;
                string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                 
                // update sales master
                string update_production = "update PF_SALES_JW set PARTY_ID = :NoCustomerID, ITEM_ID = :NoItemID, SUB_ITEM_ID = :NoSubItemID, ITEM_WEIGHT = :NoItemWeight, ITEM_RATE = :NoItemRate, ITEM_AMOUNT = :NoItemAmount, VAT_ID = :NoVatID, VAT_PERCENT = :NoVatPercent, VAT_AMOUNT = :NoVatAmount, REMARKS = :TextRemarks, ENTRY_DATE = TO_DATE(:TextEntryDate, 'DD-MM-YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where INVOICE_NO = :NoInvoiceNo ";
                cmdu = new OracleCommand(update_production, conn);

                OracleParameter[] objPrm = new OracleParameter[15]; 
                objPrm[0] = cmdu.Parameters.Add("NoCustomerID", CustomerID); 
                objPrm[1] = cmdu.Parameters.Add("NoItemID", ItemID);
                objPrm[2] = cmdu.Parameters.Add("NoSubItemID", SubItemID); 
                objPrm[3] = cmdu.Parameters.Add("NoItemWeight", ItemWeight);
                objPrm[4] = cmdu.Parameters.Add("NoItemRate", ItemRate);
                objPrm[5] = cmdu.Parameters.Add("NoItemAmount", ItemAmount); 
                objPrm[6] = cmdu.Parameters.Add("NoVatID", VatID);
                objPrm[7] = cmdu.Parameters.Add("NoVatPercent", VatPercent);
                objPrm[8] = cmdu.Parameters.Add("NoVatAmount", VatAmount);
                objPrm[9] = cmdu.Parameters.Add("TextRemarks", Remarks);
                objPrm[10] = cmdu.Parameters.Add("TextEntryDate", EntryDateNew); 
                objPrm[11] = cmdu.Parameters.Add("u_date", u_date);
                objPrm[12] = cmdu.Parameters.Add("NoCuserID", userID);
                objPrm[13] = cmdu.Parameters.Add("TextIsActive", ISActive);
                objPrm[14] = cmdu.Parameters.Add("NoInvoiceNo", InvoiceNo);

                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose();
                 
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Job Work (Sales) Data Update Successfully"));
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
          try {

            if (IS_DELETE_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string Sales_ID = TextInvoiceNo.Text;  
                int ItemID = Convert.ToInt32(DropDownItemID.Text);
                //  int SubItemID = Convert.ToInt32(DropDownSubItemID.Text);  
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
              
                // delete sales jw
                string delete_production = " delete from PF_SALES_JW where INVOICE_NO  = '" + Sales_ID + "'"; 
                cmdi = new OracleCommand(delete_production, conn); 
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();  

                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Job Work (Sales) Data Delete Successfully"));
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
            TextInvoiceNo.Text = "";  
            TextItemWeight.Text = "";
            TextItemRate.Text = "";
            DropDownSubItemID.Text = "0"; 
            CheckItemWeight.Text = ""; 
            DropDownCustomerID.Text = "0";  
            DropDownItemID.Text = "0";
            CheckEntryDate.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 
        }

        public void clearText()
        {
            TextInvoiceNo.Text = ""; 
            TextItemWeight.Text = "";
            TextItemRate.Text = "";
            DropDownSubItemID.Text = "0"; 
            CheckItemWeight.Text = ""; 
            DropDownCustomerID.Text = "0";  
            DropDownItemID.Text = "0"; 
            CheckEntryDate.Text = "";
            CheckInvoiceNo.Text = "";
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
         

        public void TextSubItem_Changed(object sender, EventArgs e)
        { 
            TextItemWeight.Text = ""; 
        }

  
        public void TextItemRate_Changed(object sender, EventArgs e)
        { 
            if (TextItemRate.Text != "" && TextItemWeight.Text != "")
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

                            string makeSQL = " select nvl(sum(ITEM_WEIGHT),0) AS FINAL_STOCK_WT from PF_PRODUCTION_JW where ITEM_ID  = '" + ItemID + "'  ";
                            cmdl = new OracleCommand(makeSQL);
                            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                            dt = new DataTable();
                            oradata.Fill(dt);
                            RowCount = dt.Rows.Count; 
                            double FinalStock = 0.00;
                            double ItemWeightF = Convert.ToDouble(TextItemWeight.Text);
                            for (int i = 0; i < RowCount; i++)
                            {
                                FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                            }

                            if (ItemWeightF <= FinalStock)
                            {
                                CheckItemWeight.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Material is available</label>";
                                CheckItemWeight.ForeColor = System.Drawing.Color.Green;
                                TextItemRate.Focus();
                                BtnAdd.Attributes.Add("aria-disabled", "true");
                                BtnAdd.Attributes.Add("class", "btn btn-primary active");
                                BtnUpdate.Attributes.Add("aria-disabled", "true");
                                BtnUpdate.Attributes.Add("class", "btn btn-success active");
                            }
                            else
                            {
                                CheckItemWeight.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Material is not available for Sale from Job Work (Purchase). Available Material is <span class='badge bg-yellow'>" + FinalStock + "</span> metric ton (MT)</label>";
                                CheckItemWeight.ForeColor = System.Drawing.Color.Red;
                                TextItemWeight.Focus();
                                BtnAdd.Attributes.Add("aria-disabled", "false");
                                BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                                BtnUpdate.Attributes.Add("aria-disabled", "false");
                                BtnUpdate.Attributes.Add("class", "btn btn-success disabled");

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
                    CheckItemWeight.Text = "";
                    DropDownItemID.Focus();
                } 

                 
                double ItemRate = Convert.ToDouble(TextItemRate.Text);
                double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
                double ItemAmount = ItemRate * ItemWeight;
                string ItemAmountNew = Math.Round(decimal.Parse(ItemAmount.ToString()), 0).ToString();
                double ItemAmountNewD = Convert.ToDouble(ItemAmountNew);
                TextItemAmount.Text = ItemAmountNewD.ToString("0.00");
                double VatPercent = Convert.ToDouble(DropDownVatID.SelectedItem.Text);
                double VatPercentAmt = (ItemAmount * VatPercent) / 100;
                TextVatAmount.Text = VatPercentAmt.ToString("0.00");
                EntryDate.Focus();
            }
        }

     

        public void TextInvoiceNo_TextChanged(object sender, EventArgs e)
        {
            string InvoiceNo = TextInvoiceNo.Text; 
            if (InvoiceNo != null)
            {       alert_box.Visible = false;

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "select INVOICE_NO from PF_SALES_JW where INVOICE_NO = '" + InvoiceNo + "'";
                    cmd.CommandType = CommandType.Text;

                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        CheckInvoiceNo.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Invoice Number is not available</label>";
                        CheckInvoiceNo.ForeColor = System.Drawing.Color.Red;
                        TextInvoiceNo.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                        DropDownCustomerID.Text = "0";
                        TextItemWeight.Text = "";
                        DropDownCustomerID.Attributes.Add("disabled", "disabled");
                        TextItemWeight.Attributes.Add("disabled", "disabled");

                    }
                    else
                    {
                        CheckInvoiceNo.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Invoice Number is available</label>";
                        CheckInvoiceNo.ForeColor = System.Drawing.Color.Green;
                        DropDownCustomerID.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");
                        DropDownCustomerID.Attributes.Remove("disabled");
                        TextItemWeight.Attributes.Remove("disabled");

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
      
         
    }
          
}