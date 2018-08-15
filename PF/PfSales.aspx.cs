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
    public partial class PfSales : System.Web.UI.Page
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
                        string makeCustomerSQL = " SELECT * FROM PF_PARTY WHERE IS_ACTIVE = 'Enable' ORDER BY PARTY_NAME ASC";
                        ds = ExecuteBySqlStringEmpType(makeCustomerSQL);
                        dtCustomerID = (DataTable)ds.Tables[0];
                        DropDownCustomerID.DataSource = dtCustomerID;
                        DropDownCustomerID.DataValueField = "PARTY_ID";
                        DropDownCustomerID.DataTextField = "PARTY_NAME";
                        DropDownCustomerID.DataBind();
                        DropDownCustomerID.Items.Insert(0, new ListItem("Select Customer", "0"));

                        DropDownCustomerID1.DataSource = dtCustomerID;
                        DropDownCustomerID1.DataValueField = "PARTY_ID";
                        DropDownCustomerID1.DataTextField = "PARTY_NAME";
                        DropDownCustomerID1.DataBind();
                        DropDownCustomerID1.Items.Insert(0, new ListItem("Select Customer", "0"));


                        DataTable dtPurchaseTypeID = new DataTable();
                        DataSet dsl = new DataSet();
                        string makePurchaseTypeSQL = " SELECT * FROM PF_PURCHASE_TYPE WHERE IS_ACTIVE = 'Enable' ORDER BY PUR_TYPE_NAME DESC";
                        dsl = ExecuteBySqlStringEmpType(makePurchaseTypeSQL);
                        dtPurchaseTypeID = (DataTable)dsl.Tables[0];
                        DropDownPurchaseTypeID.DataSource = dtPurchaseTypeID;
                        DropDownPurchaseTypeID.DataValueField = "PUR_TYPE_ID";
                        DropDownPurchaseTypeID.DataTextField = "PUR_TYPE_NAME";
                        DropDownPurchaseTypeID.DataBind();
                     //   DropDownPurchaseTypeID.Items.Insert(0, new ListItem("Select Sales Type", "0"));
                           
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
                        ChangeExport.Visible = false;
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
                    string SalesMode = radSalesMode.SelectedValue;
                    string InvoiceNo = "";
                    if (SalesMode == "Local") { InvoiceNo = TextInvoiceNo.Text; } else { InvoiceNo = TextInvoiceNoEx.Text; } 
                    int CustomerID = Convert.ToInt32(DropDownCustomerID.Text);
                    int SalesTypeID = Convert.ToInt32(DropDownPurchaseTypeID.Text);  
                    int ItemID = Convert.ToInt32(DropDownItemID.Text);
                    double ItemRate = Convert.ToDouble(TextItemRate.Text);
                    double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
                    double ItemAmount = ItemRate * ItemWeight;
                    int VatID = Convert.ToInt32(DropDownVatID.Text);
                    double VatPercent = Convert.ToDouble(DropDownVatID.SelectedItem.Text);
                    double VatAmount = (ItemAmount * VatPercent) / 100; 

                    int SubItemID = Convert.ToInt32(DropDownSubItemID.Text);
                    string ItemName = DropDownItemID.SelectedItem.Text;
                    string SubItemName = "", InventoryType = "";
                    if (SubItemID == 0)  { SubItemID = 0; SubItemName = "";  } else { SubItemID = Convert.ToInt32(DropDownSubItemID.Text); SubItemName = DropDownSubItemID.SelectedItem.Text;  }
                    string Remarks = TextRemarks.Text; 
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable"; 
                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                     
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                      
                    //inventory calculation 
                    int InvenItemID = 0;
                    int InvenSubItemID = 0;
                    double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, FinalStock = 0.00, StockInWetNew = 0.00, StockOutWetNew = 0.00, FinalStockNew = 0.00;

                    string makeSQL1 = " select FINAL_STOCK_WT from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "' ";
                    cmdl = new OracleCommand(makeSQL1);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;
                     
                    for (int i = 0; i < RowCount; i++)
                    {
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    // if sales type D and finished goods inventory is greater than item weight
                    if (SalesTypeID == 1)
                    {

                        string makeSQL2 = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "' "; // AND SUB_ITEM_ID  = '" + SubItemID + "'
                        cmdl = new OracleCommand(makeSQL2);
                        oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                        dt = new DataTable();
                        oradata.Fill(dt);
                        RowCount = dt.Rows.Count;


                        for (int i = 0; i < RowCount; i++)
                        {
                            InvenItemID = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                            InvenSubItemID = Convert.ToInt32(dt.Rows[i]["SUB_ITEM_ID"].ToString());
                            InitialStock = Convert.ToDouble(dt.Rows[i]["INITIAL_STOCK_WT"].ToString());
                            StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                            StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString());
                            FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                        }

                        StockOutWetNew = StockOutWet + ItemWeight;
                        FinalStockNew = InitialStock + StockInWet - StockOutWetNew;

                        if (0 < RowCount)
                        {

                            string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                            cmdu = new OracleCommand(update_inven_mas, conn);

                            OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                            objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetNew);
                            objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                            objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                            objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                            objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);
                        //    objPrmInevenMas[5] = cmdu.Parameters.Add("NoSubItemID", InvenSubItemID);

                            cmdu.ExecuteNonQuery();
                            cmdu.Parameters.Clear();
                            cmdu.Dispose();

                            InventoryType = "RM";
                        }
                    }
                    else
                    {
                        // check inventory FG
                        string makeSQL = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "' ";
                        cmdl = new OracleCommand(makeSQL);
                        oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                        dt = new DataTable();
                        oradata.Fill(dt);
                        RowCount = dt.Rows.Count;

                        for (int i = 0; i < RowCount; i++)
                        {
                            InvenItemID = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                            InvenSubItemID = Convert.ToInt32(dt.Rows[i]["SUB_ITEM_ID"].ToString());
                            InitialStock = Convert.ToDouble(dt.Rows[i]["INITIAL_STOCK_WT"].ToString());
                            StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                            StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString());
                            FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                        }

                        StockOutWetNew = StockOutWet + ItemWeight;
                        FinalStockNew = InitialStock + StockInWet - StockOutWetNew;

                        if (0 < RowCount)
                        {
                            // update inventory FG
                            string update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID "; //AND  SUB_ITEM_ID = :NoSubItemID 
                            cmdu = new OracleCommand(update_inven_mas, conn);

                            OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                            objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetNew);
                            objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                            objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                            objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                            objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);
                        //    objPrmInevenMas[5] = cmdu.Parameters.Add("NoSubItemID", InvenSubItemID);

                            cmdu.ExecuteNonQuery();
                            cmdu.Parameters.Clear();
                            cmdu.Dispose();
                        }

                        InventoryType = "FG";

                    }

                      // insert inventory FG details
                    string InventoryFor = "Sales Orders";
                    string get_inven_mas_des_id = "select PF_FG_STOCK_INVEN_MASDESID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_inven_mas_des_id, conn);
                    int newInvenMasDesFgID = Int16.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_inven_fg_des = "insert into  PF_FG_STOCK_INVENTORY_MAS_DE (FG_INVEN_DE_ID, REF_ID, ITEM_ID, ITEM_NAME, SUB_ITEM_ID, SUB_ITEM_NAME, INVENTORY_FOR, STOCK_IN_WT, STOCK_OUT_WT, CREATE_DATE, C_USER_ID) values ( :NoInvenMasFgID, :NoRefID, :NoItemID, :TextItemName, :NoSubItemID, :TextSubItemName, :TextInventoryFor, :NoStockIn, :NoStockOut, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_inven_fg_des, conn);

                    OracleParameter[] objPrmIrmd = new OracleParameter[11];
                    objPrmIrmd[0] = cmdi.Parameters.Add("NoInvenMasFgID", newInvenMasDesFgID);
                    objPrmIrmd[1] = cmdi.Parameters.Add("NoRefID", InvoiceNo);
                    objPrmIrmd[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrmIrmd[3] = cmdi.Parameters.Add("TextItemName", ItemName);
                    objPrmIrmd[4] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                    objPrmIrmd[5] = cmdi.Parameters.Add("TextSubItemName", SubItemName);
                    objPrmIrmd[6] = cmdi.Parameters.Add("TextInventoryFor", InventoryFor);
                    objPrmIrmd[7] = cmdi.Parameters.Add("NoStockIn", StockInWetNew);
                    objPrmIrmd[8] = cmdi.Parameters.Add("NoStockOut", ItemWeight);
                    objPrmIrmd[9] = cmdi.Parameters.Add("c_date", c_date);
                    objPrmIrmd[10] = cmdi.Parameters.Add("NoCuserID", userID);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    // insert sales data
                    string get_user_production_id = "select PF_SALES_MASTERID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_user_production_id, conn);
                    int newSalesID = Int16.Parse(cmdu.ExecuteScalar().ToString());
                    string insert_production = "insert into  PF_SALES_MASTER (SALES_ID, INVOICE_NO, PUR_TYPE_ID, PARTY_ID, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, ITEM_RATE, ITEM_AMOUNT, VAT_ID, VAT_PERCENT, VAT_AMOUNT, REMARKS, ENTRY_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, IS_OBJ_QUERY, DIVISION_ID, INVENTORY_TYPE, SALES_MODE ) values ( :NoSalesID, :NoInvoiceNo, :NoSalesTypeID, :NoCustomerID, :NoItemID, :NoSubItemID,  :TextItemWeight, :TextItemRate, :TextItemAmount, :NoVatID, :NoVatPercent, :NoVatAmount, :TextRemarks, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, :TextIObjQuery, 3, :TextInventoryType, :TextSalesMode)";
                    cmdi = new OracleCommand(insert_production, conn);

                    OracleParameter[] objPrm = new OracleParameter[20];
                    objPrm[0] = cmdi.Parameters.Add("NoSalesID", newSalesID);
                    objPrm[1] = cmdi.Parameters.Add("NoInvoiceNo", InvoiceNo);
                    objPrm[2] = cmdi.Parameters.Add("NoSalesTypeID", SalesTypeID);
                    objPrm[3] = cmdi.Parameters.Add("NoCustomerID", CustomerID);
                    objPrm[4] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[5] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                    objPrm[6] = cmdi.Parameters.Add("TextItemWeight", ItemWeight);
                    objPrm[7] = cmdi.Parameters.Add("TextItemRate", ItemRate);
                    objPrm[8] = cmdi.Parameters.Add("TextItemAmount", ItemAmount);
                    objPrm[9] = cmdi.Parameters.Add("NoVatID", VatID);
                    objPrm[10] = cmdi.Parameters.Add("NoVatPercent", VatPercent);
                    objPrm[11] = cmdi.Parameters.Add("NoVatAmount", VatAmount);
                    objPrm[12] = cmdi.Parameters.Add("TextRemarks", Remarks);
                    objPrm[13] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[14] = cmdi.Parameters.Add("c_date", c_date);
                    objPrm[15] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[16] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[17] = cmdi.Parameters.Add("TextIObjQuery", "No");
                    objPrm[18] = cmdi.Parameters.Add("TextInventoryType", InventoryType);
                    objPrm[19] = cmdi.Parameters.Add("TextSalesMode", SalesMode);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                     
                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Sales Successfully"));
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
            string USER_DATA_ID = Session["user_data_id"].ToString();


            DataTable dtUserTypeID = new DataTable();
            DataSet ds = new DataSet();
            string makeSQL = " select SALES_ID, INVOICE_NO, PUR_TYPE_ID, PARTY_ID, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, ITEM_RATE, ITEM_AMOUNT, VAT_ID, VAT_PERCENT, VAT_AMOUNT, REMARKS, TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, CREATE_DATE, UPDATE_DATE, IS_ACTIVE, INVENTORY_TYPE, SALES_MODE from PF_SALES_MASTER where INVOICE_NO  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                radSalesMode.SelectedValue  = dt.Rows[i]["SALES_MODE"].ToString();
                TextInvoiceNo.Text          = dt.Rows[i]["INVOICE_NO"].ToString();
                DropDownCustomerID.Text     = dt.Rows[i]["PARTY_ID"].ToString();
                DropDownPurchaseTypeID.Text = dt.Rows[i]["PUR_TYPE_ID"].ToString();
                TextInventoryType.Text      = dt.Rows[i]["INVENTORY_TYPE"].ToString();
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
                    makeSQL = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PPT.PUR_TYPE_NAME, PP.PARTY_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_CHECK, PSM.IS_OBJ_QUERY, PSM.OBJ_QUERY_DES, PSM.OBJ_QUERY_C_DATE FROM PF_SALES_MASTER PSM LEFT JOIN PF_PURCHASE_TYPE PPT ON  PPT.PUR_TYPE_ID = PSM.PUR_TYPE_ID LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PSM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID  WHERE PSM.IS_SALES_RETURN IS NULL ORDER BY PSM.CREATE_DATE DESC";
                }
                else
                {
                    if (DropDownItemID1.Text == "0")
                    {
                        makeSQL = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PPT.PUR_TYPE_NAME, PC.CUSTOMER_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_CHECK, PSM.IS_OBJ_QUERY, PSM.OBJ_QUERY_DES, PSM.OBJ_QUERY_C_DATE FROM PF_SALES_MASTER PSM LEFT JOIN PF_PURCHASE_TYPE PPT ON  PPT.PUR_TYPE_ID = PSM.PUR_TYPE_ID LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PSM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID WHERE  PSM.IS_SALES_RETURN IS NULL AND (PSM.INVOICE_NO like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%' or PC.CUSTOMER_NAME like '" + txtSearchEmp.Text + "%' or PSI.SUB_ITEM_NAME like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'mm/yyyy') like '" + txtSearchEmp.Text + "%' or PSM.IS_ACTIVE like '" + txtSearchEmp.Text + "%'  or PSM.IS_CHECK like '" + txtSearchEmp.Text + "%') ORDER BY PSM.CREATE_DATE desc, PSM.UPDATE_DATE desc";
                    }
                    else
                    {
                        makeSQL = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PPT.PUR_TYPE_NAME, PC.CUSTOMER_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_CHECK, PSM.IS_OBJ_QUERY, PSM.OBJ_QUERY_DES, PSM.OBJ_QUERY_C_DATE FROM PF_SALES_MASTER PSM LEFT JOIN PF_PURCHASE_TYPE PPT ON  PPT.PUR_TYPE_ID = PSM.PUR_TYPE_ID LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PSM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID WHERE  PSM.IS_SALES_RETURN IS NULL AND PI.ITEM_ID like '" + DropDownItemID1.Text + "%' AND (PSM.INVOICE_NO like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%' or PC.CUSTOMER_NAME like '" + txtSearchEmp.Text + "%'  or PSI.SUB_ITEM_NAME like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'mm/yyyy') like '" + txtSearchEmp.Text + "%' or PSM.IS_ACTIVE like '" + txtSearchEmp.Text + "%'  or PSM.IS_CHECK like '" + txtSearchEmp.Text + "%') ORDER BY PSM.CREATE_DATE desc, PSM.UPDATE_DATE desc";
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

        protected void gridViewFileInformation_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow Row in GridView1.Rows)
            {
                string isCheck = (Row.FindControl("IsActiveCheckLink") as Label).Text;
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
        try {
                
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string InvoiceNo = TextInvoiceNo.Text;
                int CustomerID = Convert.ToInt32(DropDownCustomerID.Text);
                int SalesTypeID = Convert.ToInt32(DropDownPurchaseTypeID.Text);
                int ItemID = Convert.ToInt32(DropDownItemID.Text);
                double ItemRate = Convert.ToDouble(TextItemRate.Text);
                double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
                double ItemAmount = ItemRate * ItemWeight;
                int VatID = Convert.ToInt32(DropDownVatID.Text);
                double VatPercent = Convert.ToDouble(DropDownVatID.SelectedItem.Text);
                double VatAmount = (ItemAmount * VatPercent) / 100;
                string InventoryType = TextInventoryType.Text;
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
                

                // check production data
                int ItemIdOld = 0, SubItemIdOld = 0; double ItemWeightOld = 0.00; 
                string makeSQLPro = " select ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT from PF_SALES_MASTER where INVOICE_NO  = '" + InvoiceNo + "'  ";
                cmdl = new OracleCommand(makeSQLPro);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                for (int i = 0; i < RowCount; i++)
                { 
                    ItemIdOld = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                    SubItemIdOld = Convert.ToInt32(dt.Rows[i]["SUB_ITEM_ID"].ToString());
                    ItemWeightOld = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());
                }


                //inventory calculation 
                int InvenItemID = 0;
                int InvenSubItemID = 0;
                double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, StockOutWetDe = 0.00, FinalStock = 0.00, StockOutWetNew = 0.00, FinalStockNew = 0.00;

                if (InventoryType == "RM")
                {
                    // check inventory RM
                    string makeSQL2 = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";  // AND SUB_ITEM_ID  = '" + SubItemIdOld + "'
                    cmdl = new OracleCommand(makeSQL2);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;
                     
                    for (int i = 0; i < RowCount; i++)
                    {
                        InvenItemID = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                        InvenSubItemID = Convert.ToInt32(dt.Rows[i]["SUB_ITEM_ID"].ToString());
                        InitialStock = Convert.ToDouble(dt.Rows[i]["INITIAL_STOCK_WT"].ToString());
                        StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                        StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString());
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    StockOutWetNew = StockOutWet - ItemWeightOld;
                    FinalStockNew = InitialStock + StockInWet - StockOutWetNew;

                    if (0 < RowCount)
                    {
                        // update inventory RM (minus old data)
                        string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";  //  AND  SUB_ITEM_ID = :NoSubItemID
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);
                     //   objPrmInevenMas[5] = cmdu.Parameters.Add("NoSubItemID", InvenSubItemID);

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose(); 
                    }

                    StockOutWetDe = StockOutWet + ItemWeight;
                    FinalStockNew = InitialStock + StockInWet - StockOutWetDe;
                    // update inventory RM (plus new data)
                    string update_inven_rm_new = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                    cmdu = new OracleCommand(update_inven_rm_new, conn);

                    OracleParameter[] objPrmInevenRmNew = new OracleParameter[5];
                    objPrmInevenRmNew[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetDe);
                    objPrmInevenRmNew[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                    objPrmInevenRmNew[2] = cmdu.Parameters.Add("u_date", u_date);
                    objPrmInevenRmNew[3] = cmdu.Parameters.Add("NoCuserID", userID);
                    objPrmInevenRmNew[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);
                 //   objPrmInevenRmNew[5] = cmdu.Parameters.Add("NoSubItemID", InvenSubItemID);

                    cmdu.ExecuteNonQuery();
                    cmdu.Parameters.Clear();
                    cmdu.Dispose();


                } else {

                // check inventory FG
                string makeSQL = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;


                for (int i = 0; i < RowCount; i++)
                {
                    InvenItemID = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                    InvenSubItemID = Convert.ToInt32(dt.Rows[i]["SUB_ITEM_ID"].ToString());
                    InitialStock = Convert.ToDouble(dt.Rows[i]["INITIAL_STOCK_WT"].ToString());
                    StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                    StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString());
                    FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                }

                StockOutWetNew = StockOutWet - ItemWeightOld;
                FinalStockNew = InitialStock + StockInWet - StockOutWetNew;

                if (0 < RowCount)
                {
                    // update inventory FG (minus old data)
                    string update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                    cmdu = new OracleCommand(update_inven_mas, conn);

                    OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                    objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetNew);
                    objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                    objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                    objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                    objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);
                  //  objPrmInevenMas[5] = cmdu.Parameters.Add("NoSubItemID", InvenSubItemID);

                    cmdu.ExecuteNonQuery();
                    cmdu.Parameters.Clear();
                    cmdu.Dispose();
                }
                // update inventory FG (plus new data)

                StockOutWetDe = StockOutWet + ItemWeight;
                FinalStockNew = InitialStock + StockInWet - StockOutWetDe;

                // update inventory FG
                string update_inven_fg_new = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                cmdu = new OracleCommand(update_inven_fg_new, conn);

                OracleParameter[] objPrmInevenFGn = new OracleParameter[5];
                objPrmInevenFGn[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetDe);
                objPrmInevenFGn[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                objPrmInevenFGn[2] = cmdu.Parameters.Add("u_date", u_date);
                objPrmInevenFGn[3] = cmdu.Parameters.Add("NoCuserID", userID);
                objPrmInevenFGn[4] = cmdu.Parameters.Add("NoItemID", ItemID);
            //    objPrmInevenFGn[5] = cmdu.Parameters.Add("NoSubItemID", SubItemID);

                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose();
                }

                // update inventory FG details
                string insert_inven_fg_des = "update  PF_FG_STOCK_INVENTORY_MAS_DE  set ITEM_ID = :NoItemID, ITEM_NAME = :TextItemName, SUB_ITEM_ID = :NoSubItemID, SUB_ITEM_NAME = :TextSubItemName, STOCK_OUT_WT = :NoStockOut, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where REF_ID = :NoRefID ";
                cmdi = new OracleCommand(insert_inven_fg_des, conn);

                OracleParameter[] objPrmIfgd = new OracleParameter[9];
                objPrmIfgd[1] = cmdi.Parameters.Add("NoItemID", ItemID);
                objPrmIfgd[2] = cmdi.Parameters.Add("TextItemName", ItemName);
                objPrmIfgd[3] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                objPrmIfgd[4] = cmdi.Parameters.Add("TextSubItemName", SubItemName);
                objPrmIfgd[5] = cmdi.Parameters.Add("NoStockOut", ItemWeight);
                objPrmIfgd[6] = cmdi.Parameters.Add("u_date", u_date);
                objPrmIfgd[7] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrmIfgd[8] = cmdi.Parameters.Add("NoRefID", TextInvoiceNo.Text);
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                 

                // update sales master
                string update_production = "update PF_SALES_MASTER set PUR_TYPE_ID = :NoSalesTypeID, PARTY_ID = :NoCustomerID, ITEM_ID = :NoItemID, SUB_ITEM_ID = :NoSubItemID, ITEM_WEIGHT = :NoItemWeight, ITEM_RATE = :NoItemRate, ITEM_AMOUNT = :NoItemAmount, VAT_ID = :NoVatID, VAT_PERCENT = :NoVatPercent, VAT_AMOUNT = :NoVatAmount, REMARKS = :TextRemarks, ENTRY_DATE = TO_DATE(:TextEntryDate, 'DD-MM-YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where INVOICE_NO = :NoInvoiceNo ";
                cmdu = new OracleCommand(update_production, conn);

                OracleParameter[] objPrm = new OracleParameter[17];
                objPrm[1] = cmdu.Parameters.Add("NoSalesTypeID", SalesTypeID);
                objPrm[2] = cmdu.Parameters.Add("NoCustomerID", CustomerID); 
                objPrm[3] = cmdu.Parameters.Add("NoItemID", ItemID);
                objPrm[4] = cmdu.Parameters.Add("NoSubItemID", SubItemID); 
                objPrm[5] = cmdu.Parameters.Add("NoItemWeight", ItemWeight);
                objPrm[6] = cmdu.Parameters.Add("NoItemRate", ItemRate);
                objPrm[7] = cmdu.Parameters.Add("NoItemAmount", ItemAmount); 
                objPrm[8] = cmdu.Parameters.Add("NoVatID", VatID);
                objPrm[9] = cmdu.Parameters.Add("NoVatPercent", VatPercent);
                objPrm[10] = cmdu.Parameters.Add("NoVatAmount", VatAmount);
                objPrm[11] = cmdu.Parameters.Add("TextRemarks", Remarks);
                objPrm[12] = cmdu.Parameters.Add("TextEntryDate", EntryDateNew); 
                objPrm[13] = cmdu.Parameters.Add("u_date", u_date);
                objPrm[14] = cmdu.Parameters.Add("NoCuserID", userID);
                objPrm[15] = cmdu.Parameters.Add("TextIsActive", ISActive);
                objPrm[16] = cmdu.Parameters.Add("NoInvoiceNo", InvoiceNo);

                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose();
                 
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Sales Data Update successfully"));
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
            //    int SubItemID = Convert.ToInt32(DropDownSubItemID.Text); 
                string InventoryType = TextInventoryType.Text;
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
             
                // check production data
                int ItemIdOld = 0, SubItemIdOld = 0; double ItemWeightOld = 0.00;
                string makeSQLPro = " select ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT from PF_SALES_MASTER where INVOICE_NO  = '" + Sales_ID + "'  ";
                cmdl = new OracleCommand(makeSQLPro);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                for (int i = 0; i < RowCount; i++)
                {
                    ItemIdOld = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                    SubItemIdOld = Convert.ToInt32(dt.Rows[i]["SUB_ITEM_ID"].ToString());
                    ItemWeightOld = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString()); 
                }
                 
                //inventory calculation
                int InvenItemID = 0;
                int InvenSubItemID = 0;
                double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00,  FinalStock = 0.00, StockOutWetNew = 0.00, FinalStockNew = 0.00;


                if (InventoryType == "RM")
                {
                    // check inventory RM
                    string makeSQL2 = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                    cmdl = new OracleCommand(makeSQL2);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    for (int i = 0; i < RowCount; i++)
                    {
                        InvenItemID = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                        InvenSubItemID = Convert.ToInt32(dt.Rows[i]["SUB_ITEM_ID"].ToString());
                        InitialStock = Convert.ToDouble(dt.Rows[i]["INITIAL_STOCK_WT"].ToString());
                        StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                        StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString());
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    StockOutWetNew = StockOutWet - ItemWeightOld;
                    FinalStockNew = InitialStock + StockInWet - StockOutWetNew;

                    if (0 < RowCount)
                    {
                        // update inventory RM (minus old data)
                        string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);
                    //    objPrmInevenMas[5] = cmdu.Parameters.Add("NoSubItemID", InvenSubItemID);

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }
                     
                }
                else
                {
                    // check inventory FG
                    string makeSQL = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    for (int i = 0; i < RowCount; i++)
                    {
                        InvenItemID = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                        InvenSubItemID = Convert.ToInt32(dt.Rows[i]["SUB_ITEM_ID"].ToString());
                        InitialStock = Convert.ToDouble(dt.Rows[i]["INITIAL_STOCK_WT"].ToString());
                        StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                        StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString());
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    StockOutWetNew = StockOutWet - ItemWeightOld;
                    FinalStockNew = InitialStock + StockInWet - StockOutWetNew;

                    if (0 < RowCount)
                    {
                        // update inventory FG (minus old data)
                        string update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);
                    //    objPrmInevenMas[5] = cmdu.Parameters.Add("NoSubItemID", InvenSubItemID);

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }
                }
                // delete inventory FG details 
                string delete_prod_fg_de = " delete from PF_FG_STOCK_INVENTORY_MAS_DE where REF_ID  = '" + Sales_ID + "'";
                cmdi = new OracleCommand(delete_prod_fg_de, conn);
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();  
                 
                // delete production master
                string delete_production = " delete from PF_SALES_MASTER where INVOICE_NO  = '" + Sales_ID + "'"; 
                cmdi = new OracleCommand(delete_production, conn); 
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();  

                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Sales Data Delete successfully"));
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
            TextInvoiceNoEx.Text = ""; 
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
            TextInvoiceNoEx.Text = ""; 
            TextItemWeight.Text = "";
            TextItemRate.Text = "";
            DropDownSubItemID.Text = "0"; 
            CheckItemWeight.Text = ""; 
            DropDownCustomerID.Text = "0";  
            DropDownItemID.Text = "0";
            CheckInvoiceNo.Text = "";
            CheckEntryDate.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void Redio_CheckedChanged(object sender, EventArgs e)
        {
            if (radSalesMode.SelectedValue == "Local")
            {
                ChangeLocal.Visible = true;
                ChangeExport.Visible = false;
            }
            else
            {
                ChangeLocal.Visible = false;
                ChangeExport.Visible = true;
            }
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

        public void TextCheckDataProcess(object sender, EventArgs e)
        {
            // Check inventory data process in last month
            string MakeAsOnDate = EntryDate.Text;
            string[] MakeAsOnDateSplit = MakeAsOnDate.Split('-');
            String AsOnDateTemp = MakeAsOnDateSplit[0].Replace("/", "-");
            DateTime AsOnDateNewD = DateTime.ParseExact(AsOnDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string AsOnDateNew = AsOnDateNewD.ToString("dd-MM-yyyy");

            DateTime curDate = AsOnDateNewD;
            DateTime startDate = curDate.AddMonths(-1);
            DateTime LastDateTemp = curDate.AddDays(-(curDate.Day));
            string LastDate = LastDateTemp.ToString("dd-MM-yyyy");
            string LastMonthTemp = LastDateTemp.ToString("MM-yyyy");
            DateTime LastMonth = DateTime.ParseExact(LastMonthTemp, "MM-yyyy", CultureInfo.InvariantCulture);
            string CurrentMonthTemp = AsOnDateNewD.ToString("MM-yyyy");
            DateTime CurrentMonth = DateTime.ParseExact(CurrentMonthTemp, "MM-yyyy", CultureInfo.InvariantCulture);
            string SysMonthTemp = System.DateTime.Now.ToString("MM-yyyy");
            DateTime SysMonth = DateTime.ParseExact(SysMonthTemp, "MM-yyyy", CultureInfo.InvariantCulture);
            DateTime SysLastMonth = SysMonth.AddMonths(-1);

            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select ITEM_ID from PF_RM_STOCK_INVENTORY_HISTORY where TO_CHAR(TO_DATE(CREATE_DATE), 'dd-MM-yyyy')   = '" + LastDate + "'";
            cmd.CommandType = CommandType.Text;

            OracleDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                if (CurrentMonth == SysMonth || CurrentMonth == SysLastMonth)
                {
                    CheckEntryDate.Text = "";
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");
                    CheckEntryDate.Text = "<label class='control-label'><i class='fa fa fa-check-circle'></i></label>";
                    CheckEntryDate.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    CheckEntryDate.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Please, Insert Data Current or last months.</label>";
                    CheckEntryDate.ForeColor = System.Drawing.Color.Red;
                    EntryDate.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
            }
            else
            {
                CheckEntryDate.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Please, Complete Data process in last months (" + LastDate + "). It is required for insert current month data. </label>";
                CheckEntryDate.ForeColor = System.Drawing.Color.Red;
                EntryDate.Focus();
                BtnAdd.Attributes.Add("aria-disabled", "false");
                BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
            }
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

                            string makeSQL = " select nvl(FINAL_STOCK_WT,0) AS FINAL_STOCK_WT from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
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

                                CheckItemWeight.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Material is available and Inventory Out from Finished Goods Inventory</label>";
                                CheckItemWeight.ForeColor = System.Drawing.Color.Green;
                                TextItemRate.Focus();
                                BtnAdd.Attributes.Add("aria-disabled", "true");
                                BtnAdd.Attributes.Add("class", "btn btn-primary active");
                                BtnUpdate.Attributes.Add("aria-disabled", "true");
                                BtnUpdate.Attributes.Add("class", "btn btn-success active");

                            }
                            else
                            {
                                CheckItemWeight.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Material is not available for Sale from Finished Goods. Available Material is <span class='badge bg-yellow'>" + FinalStock + "</span> metric ton (MT)</label>";
                                CheckItemWeight.ForeColor = System.Drawing.Color.Red;
                                TextItemWeight.Focus();
                                BtnAdd.Attributes.Add("aria-disabled", "false");
                                BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                                BtnUpdate.Attributes.Add("aria-disabled", "false");
                                BtnUpdate.Attributes.Add("class", "btn btn-success disabled");


                                if (DropDownPurchaseTypeID.Text == "1") {

                                    string makeSQL1 = " select nvl(FINAL_STOCK_WT,0) AS FINAL_STOCK_WT from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "' ";
                                    cmdl = new OracleCommand(makeSQL1);
                                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                                    dtr = new DataTable();
                                    oradata.Fill(dtr);
                                    RowCount = dtr.Rows.Count;

                                    double FinalStockRm = 0.00;
                                    double ItemWeightRm = Convert.ToDouble(TextItemWeight.Text);
                                    for (int i = 0; i < RowCount; i++)
                                    {
                                        FinalStockRm = Convert.ToDouble(dtr.Rows[i]["FINAL_STOCK_WT"].ToString());
                                    }

                                    if (ItemWeightRm <= FinalStockRm)
                                    {

                                        CheckItemWeight.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Material is available & Inventory Out from Raw Material Inventory</label>";
                                        CheckItemWeight.ForeColor = System.Drawing.Color.Green;
                                        TextItemRate.Focus();
                                        BtnAdd.Attributes.Add("aria-disabled", "true");
                                        BtnAdd.Attributes.Add("class", "btn btn-primary active");
                                        BtnUpdate.Attributes.Add("aria-disabled", "true");
                                        BtnUpdate.Attributes.Add("class", "btn btn-success active");

                                    }
                                    else {

                                        CheckItemWeight.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Material is not available for Sale from Raw Material. Available Material is <span class='badge bg-yellow'>" + FinalStockRm + "</span> metric ton (MT)</label>";
                                        CheckItemWeight.ForeColor = System.Drawing.Color.Red;
                                        TextItemWeight.Focus();
                                        BtnAdd.Attributes.Add("aria-disabled", "false");
                                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                                    
                                    }
                                
                                
                                } else { }



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
            string MatchEmpIDPattern = "^([0-9]{5})$";
            if (InvoiceNo != null)
            {

                if (Regex.IsMatch(InvoiceNo, MatchEmpIDPattern))
                {
                    alert_box.Visible = false;

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "select INVOICE_NO from PF_SALES_MASTER where INVOICE_NO = '" + InvoiceNo + "'";
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
                else
                {
                    CheckInvoiceNo.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter Invoice Number is 5 digit only</label>";
                    CheckInvoiceNo.ForeColor = System.Drawing.Color.Red;
                    TextInvoiceNo.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

                }
            }
        }

        public void TextInvoiceNoEx_TextChanged(object sender, EventArgs e)
        {
            string InvoiceNo = TextInvoiceNoEx.Text; 
            if (InvoiceNo != null)
            {       alert_box.Visible = false;

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "select INVOICE_NO from PF_SALES_MASTER where INVOICE_NO = '" + InvoiceNo + "'";
                    cmd.CommandType = CommandType.Text;

                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        CheckInvoiceNoEx.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Invoice Number is not available</label>";
                        CheckInvoiceNoEx.ForeColor = System.Drawing.Color.Red;
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
                        CheckInvoiceNoEx.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Invoice Number is available</label>";
                        CheckInvoiceNoEx.ForeColor = System.Drawing.Color.Green;
                        DropDownCustomerID.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");
                        DropDownCustomerID.Attributes.Remove("disabled");
                        TextItemWeight.Attributes.Remove("disabled");

                    }
                
            }
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

                    string makeSQL = " select nvl(FINAL_STOCK_WT,0) AS FINAL_STOCK_WT from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  AND SUB_ITEM_ID  = '" + SubItemID + "' ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    double FinalStock = 0.00;
                    double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
                    for (int i = 0; i < RowCount; i++)
                    {
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }
                     
                    if (ItemWeight <= FinalStock)
                    {
                        if (TextItemRate.Text != "" && TextItemWeight.Text != "")
                        {
                            double ItemRate = Convert.ToDouble(TextItemRate.Text); 
                            double ItemAmount = ItemRate * ItemWeight;
                            string ItemAmountNew = Math.Round(decimal.Parse(ItemAmount.ToString()), 0).ToString();
                            double ItemAmountNewD = Convert.ToDouble(ItemAmountNew);
                            TextItemAmount.Text = ItemAmountNewD.ToString("0.00");
                            double VatPercent = Convert.ToDouble(DropDownVatID.SelectedItem.Text);
                            double VatPercentAmt = (ItemAmount * VatPercent) / 100;
                            TextVatAmount.Text = VatPercentAmt.ToString("0.00");
                        }
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
                        CheckItemWeight.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Material is not available for Sale. Available Material is <span class='badge bg-yellow'>" + FinalStock + "</span> metric ton (MT)</label>";
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
        protected void BtnReport_Click1(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad1 = true;
                alert_box.Visible = false;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            } 
        }
        protected void BtnReport_Click2(object sender, EventArgs e)
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