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
using System.Web.Services;

namespace NRCAPPS.PF
{
    public partial class PfSales : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl, cmdsp;
        OracleDataAdapter oradata; 
        DataTable dt, dtr;
        int RowCount;
         
        double ItemVatAmt = 0.0, ItemAmtTotal = 0.0, ItemWtWbTotal = 0.0; string EntryDateSlip = "", PartyArabicName = "", PartyName = "";
        string IS_PAGE_ACTIVE = "", IS_ADD_ACTIVE = "", IS_EDIT_ACTIVE = "", IS_DELETE_ACTIVE = "", IS_VIEW_ACTIVE = "", IS_REPORT_ACTIVE = "", IS_PRINT_ACTIVE = "";
        public bool IsLoad { get; set; } public bool IsLoad1 { get; set; } public bool IsLoad2 { get; set; } public bool IsLoad3 { get; set; }
        public bool IsLoad4 { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_NAME"] != null)
            {
                string requestedFile = Path.GetFileName(Request.Path);
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = " SELECT  NUPP.IS_PAGE_ACTIVE, NUPP.IS_ADD_ACTIVE, NUPP.IS_EDIT_ACTIVE, NUPP.IS_DELETE_ACTIVE, NUPP.IS_VIEW_ACTIVE, NUPP.IS_REPORT_ACTIVE, NUPP.IS_PRINT_ACTIVE FROM NRC_USER_PAGE_PERMISSION NUPP LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NUPP.USER_PAGE_ID  WHERE NUPP.USER_ID = '" + Session["USER_ID"] + "' AND NUP.IS_ACTIVE = 'Enable' AND NUP.PAGE_URL = '" + requestedFile + "' ";
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
                    IS_PRINT_ACTIVE  = dt.Rows[i]["IS_PRINT_ACTIVE"].ToString();
                }

                if (IS_PAGE_ACTIVE == "Enable")
                {
                     if (!IsPostBack)
                    {
                        DataTable dtCustomerID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeCustomerSQL = "  SELECT PARTY_ID,  PARTY_ID || ' - ' || PARTY_NAME || ' - ' || PARTY_VAT_NO AS PARTY_NAME  FROM PF_PARTY WHERE IS_ACTIVE = 'Enable' ORDER BY PARTY_NAME ASC ";
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
                        DropDownSubItemID.Items.FindByValue("1").Selected = true;
                        //   DropDownSubItemID.Items.Insert(0, new ListItem("Select Sub Item", "0"));

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

                      //  DropDownCustomerID.Attributes.Add("disabled", "disabled");
                      //  TextItemWeight.Attributes.Add("disabled", "disabled");

                        TextInvoiceNo.Focus();
                        TextItemAmount.Enabled = false;
                        TextVatAmount.Enabled = false;
                        TextTotalAmount.Enabled = false;
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
                    string SalesMode = "Local";
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

                        if (ItemWeight <= FinalStock)
                        {


                            StockOutWetNew = StockOutWet + ItemWeight;
                            FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;

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

                            alert_box.Visible = true;
                            alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory"));
                            alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
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
                        if (ItemWeight <= FinalStock)
                        {

                        StockOutWetNew = StockOutWet + ItemWeight;
                        FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;

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
                        else
                        { 
                            alert_box.Visible = true;
                            alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory"));
                            alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                        }

                    }

                    if (ItemWeight <= FinalStock)
                    {
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
                        string insert_production = "insert into  PF_SALES_MASTER (SALES_ID, INVOICE_NO, PUR_TYPE_ID, PARTY_ID, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, ITEM_RATE, ITEM_AMOUNT, VAT_ID, VAT_PERCENT, VAT_AMOUNT, REMARKS, ENTRY_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, IS_OBJ_QUERY, DIVISION_ID, INVENTORY_TYPE, SALES_MODE, ITEM_WEIGHT_WB) values ( :NoSalesID, :NoInvoiceNo, :NoSalesTypeID, :NoCustomerID, :NoItemID, :NoSubItemID,  :TextItemWeight, :TextItemRate, :TextItemAmount, :NoVatID, :NoVatPercent, :NoVatAmount, :TextRemarks, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, :TextIObjQuery, 3, :TextInventoryType, :TextSalesMode, :TextItemWtWb)";
                        cmdi = new OracleCommand(insert_production, conn);

                        OracleParameter[] objPrm = new OracleParameter[21];
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
                        objPrm[20] = cmdi.Parameters.Add("TextItemWtWb", TextItemWtWb.Text);

                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();


                        conn.Close();

                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Insert New Sales Successfully"));
                        alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                        clearText();
                        Display();
                        DisplaySummary();
                    }
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

        [WebMethod]
        public static Boolean InvoiceNoCheck(int InvoiceNo)
        {
            Boolean result = false;
            string query = "select INVOICE_NO from PF_SALES_MASTER where INVOICE_NO = '" + InvoiceNo + "'";
            string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(strConnString))
            {
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;
                    conn.Open();
                    OracleDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        result = true;
                    }
                    conn.Close();
                    return result;
                }
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            if (IS_PRINT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string HtmlString = "";
                string InvoiceNo = TextInvoiceNo.Text;
                string makeSQL = " SELECT PPM.SALES_ID, PPM.INVOICE_NO,PP.PARTY_NAME, PP.PARTY_ARABIC_NAME, PP.PARTY_ADD_1 || ', ' || PP.PARTY_ADD_2 AS PARTY_ADD, PP.PARTY_VAT_NO, PI.ITEM_CODE, PI.ITEM_NAME, PI.ITEM_ARABIC_NAME, PPM.ITEM_WEIGHT, nvl(PPM.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, PPM.ITEM_RATE, ROUND(PPM.ITEM_AMOUNT, 2) AS ITEM_AMOUNT, nvl(PPM.VAT_AMOUNT, 0) AS VAT_AMOUNT, TO_CHAR(PPM.ENTRY_DATE, 'dd-MON-yyyy') AS ENTRY_DATE, HE.EMP_LNAME FROM PF_SALES_MASTER PPM LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PPM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_WB_OPERATOR PWO ON PWO.IS_ACTIVE = 'Enable' LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PWO.EMP_ID WHERE PPM.INVOICE_NO = '" + InvoiceNo + "' ORDER BY PI.ITEM_ID ";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                for (int i = 0; i < 1; i++)
                {
                    PartyArabicName = dt.Rows[i]["PARTY_ARABIC_NAME"].ToString();
                    PartyName = dt.Rows[i]["PARTY_NAME"].ToString();
                    string PartyAdd = dt.Rows[i]["PARTY_ADD"].ToString();
                    string PartyVatNo = dt.Rows[i]["PARTY_VAT_NO"].ToString();
                    int ItemCode = Convert.ToInt32(dt.Rows[i]["ITEM_CODE"].ToString());
                    string ItemName = dt.Rows[i]["ITEM_NAME"].ToString();
                    string ItemArabicName = dt.Rows[i]["ITEM_ARABIC_NAME"].ToString();
                    string EmpWbLname = dt.Rows[i]["EMP_LNAME"].ToString();
                    EntryDateSlip = dt.Rows[i]["ENTRY_DATE"].ToString();

                    HtmlString += "<div style='float:left;width:785px;height:530px;margin-top:0px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";
                    HtmlString += "<div style='float:left;width:685px;height:213px;margin:65px 0 0 111px;font-family: Arial Narrow, Courier, Lucida Sans Typewriter;font-size: 12px;'>Format Number: NRC-PF-FM-SL-04</div> ";
                    HtmlString += "<div style='float:left;width:380px;'> ";
                    HtmlString += "<div style='float:left;width:270px;height:102px;margin-left:110px;padding-top:10px;text-align:center;' ><font size='1px'> " + PartyArabicName + "</font> </br> " + PartyName + " </div> ";
                    HtmlString += "<div style='float:left;width:210px;height:33px; margin:0 0 0 90px;'>" + PartyAdd + "</div> <div style='float:left;width:210px; margin:0 0 0 90px;'>" + PartyVatNo + " </div> ";
                    HtmlString += "</div> ";
                }
                int m = 1;
                HtmlString += "<div style='float:left;width:380px;'> ";
                HtmlString += "<div style='float:left;width:240px;height:42px;margin:30px 0 0 140px;'> ";
                for (int i = 0; i < RowCount; i++)
                {
                    string ItemName = dt.Rows[i]["ITEM_NAME"].ToString();
                    //   string ItemArabicName = dt.Rows[i]["ITEM_ARABIC_NAME"].ToString(); 
                    if (m == RowCount)
                    {
                        HtmlString += "" + ItemName + "";
                    }
                    else { HtmlString += "" + ItemName + ","; }
                    m++;
                }

                HtmlString += "</div>";

                for (int i = 0; i < 1; i++)
                {
                    string EmpWbLname = dt.Rows[i]["EMP_LNAME"].ToString();
                    HtmlString += "<div style='float:left;width:380px;'> ";
                    HtmlString += "<div style='float:left;width:200px;height:76px;margin:0 0 0 175px;'>" + EmpWbLname + "</div><div style='float:left;width:200px;margin:0 0 0 180px;'>" + EntryDateSlip + "</div> ";
                    HtmlString += "</div>";
                    HtmlString += "</div>";
                }
                HtmlString += "</div>";
                HtmlString += "<div style='float:left;width:785px;height:158px;margin:0 0 0 10px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";

                for (int i = 0; i < RowCount; i++)
                {

                    int ItemCode = Convert.ToInt32(dt.Rows[i]["ITEM_CODE"].ToString());
                    string ItemName = dt.Rows[i]["ITEM_NAME"].ToString();
                    string ItemArabicName = dt.Rows[i]["ITEM_ARABIC_NAME"].ToString();

                    double ItemWtWb = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString()) * 1000;
                    ItemWtWbTotal += Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString()) * 1000;
                    double VarianceWT = (Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString()) - Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString())) * 1000;
                    double ItemWt = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString()) * 1000;
                    double ItemRate = Convert.ToDouble(dt.Rows[i]["ITEM_RATE"].ToString()) / 1000;
                    double ItemAmt = Convert.ToDouble(decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00"));
                    ItemAmtTotal += Convert.ToDouble(decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00"));
                    ItemVatAmt += Convert.ToDouble(decimal.Parse(dt.Rows[i]["VAT_AMOUNT"].ToString()).ToString(".00"));
                    double TotalInvoiceAmt = +Convert.ToDouble(decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00"));

                    HtmlString += "<div style='float:left;width:242px;' >" + ItemCode + "     " + ItemName + "-<font size='1px'>" + ItemArabicName + "</font></div> ";
                    HtmlString += "<div style='float:left;width:82px;text-align:center;'>" + string.Format("{0:n0}", ItemWtWb) + " </div> ";
                    HtmlString += "<div style='float:left;width:93px;text-align:center;'>" + string.Format("{0:n0}", VarianceWT) + " </div> ";
                    HtmlString += "<div style='float:left;width:112px;text-align:right;'>" + string.Format("{0:n0}", ItemWt) + " </div> ";
                    HtmlString += "<div style='float:left;width:107px;text-align:right;'>" + ItemRate.ToString("0.000") + " </div> ";
                    HtmlString += "<div style='float:left;width:120px;text-align:right;height:25px'>" + string.Format("{0:n2}", ItemAmt) + " </div> ";

                }
                HtmlString += "</div>";
                HtmlString += "<div style='float:left;width:785px;height:238px;margin:0 0 0 8px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'><div style='float:left;width:242px;border:white solid 1px' ></div>";
                HtmlString += "<div style='float:left;width:82px;text-align:center;margin:0 0 10px 0;'>" + string.Format("{0:n0}", ItemWtWbTotal) + " </div> ";
                HtmlString += "<div style='float:left;width:431px;text-align:right;margin:0 0 10px 0;'>" + string.Format("{0:n2}", ItemAmtTotal) + " </div> ";
                HtmlString += "<div style='float:left;width:757px;text-align:right;margin:0 0 10px 0;'>" + string.Format("{0:n2}", ItemVatAmt) + " </div> ";
                HtmlString += "<div style='float:left;width:757px;text-align:right;'>" + string.Format("{0:n2}", ItemAmtTotal + ItemVatAmt) + " </div> ";
                HtmlString += "<div style='float:left;width:500px;margin:0 0 0 120px;text-align:left;'>" + EntryDateSlip + " </div> ";
                string NumberToInWord = NumberToInWords.DecimalToWordsSR(Convert.ToDecimal(ItemAmtTotal + ItemVatAmt)).Trim().ToUpper();
                //   string NumberToInWord = NumberToWords(ItemAmtTotal + ItemVatAmt).Trim().ToUpper();
                HtmlString += "<div style = 'float:left;width:290px;height:88px;margin:40px 0 0 460px;padding:10px;text-align:left;'>" + NumberToInWord + " </div> ";
                HtmlString += "<div style = 'float:left;width:200px;margin:0 0 0 555px;text-align:left;'><font size='1px'>" + PartyName + "|" + PartyArabicName + "</font> </div> ";
                HtmlString += "</div>";
                HtmlString += "</div>";
                HtmlString += "</div>";

                // sales master update for print
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string update_user = "update  PF_SALES_MASTER  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where INVOICE_NO = :NoSlipNo ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[4];
                objPrm[0] = cmdi.Parameters.Add("TextIsPrint", "Printed");
                objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[2] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[3] = cmdi.Parameters.Add("NoSlipNo", InvoiceNo);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();

                PanelPrint.Controls.Add(new LiteralControl(HtmlString));
                Session["ctrl"] = PanelPrint;
                ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('Print.aspx','PrintMe','height=900px,width=1200px,scrollbars=1');</script>");
                Display();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
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
            string makeSQL = " select SALES_ID, INVOICE_NO, PUR_TYPE_ID, PARTY_ID, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, ITEM_RATE, ITEM_AMOUNT, VAT_ID, VAT_PERCENT, VAT_AMOUNT, (ITEM_AMOUNT+VAT_AMOUNT) AS TOTAL_AMOUNT,  nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, REMARKS, TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, CREATE_DATE, UPDATE_DATE, IS_ACTIVE, INVENTORY_TYPE, SALES_MODE from PF_SALES_MASTER where INVOICE_NO  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            { 
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
                TextTotalAmount.Text        = decimal.Parse(dt.Rows[i]["TOTAL_AMOUNT"].ToString()).ToString("0.00");
                TextRemarks.Text            = dt.Rows[i]["REMARKS"].ToString();
                EntryDate.Text              = dt.Rows[i]["ENTRY_DATE"].ToString(); 
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                TextItemWtWb.Text         = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString()).ToString("0.000");
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
                string MonthYear = System.DateTime.Now.ToString("MM/yyyy");
                if (txtSearchEmp.Text == "")
                {
                    makeSQL = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PPT.PUR_TYPE_NAME, PP.PARTY_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, (PSM.ITEM_AMOUNT + PSM.VAT_AMOUNT) AS TOTAL_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_CHECK, PSM.IS_OBJ_QUERY, PSM.OBJ_QUERY_DES, PSM.OBJ_QUERY_C_DATE, PSM.IS_PRINT, TO_CHAR(PSM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE  FROM PF_SALES_MASTER PSM LEFT JOIN PF_PURCHASE_TYPE PPT ON  PPT.PUR_TYPE_ID = PSM.PUR_TYPE_ID LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PSM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID  WHERE  to_char(PSM.ENTRY_DATE, 'mm/yyyy') = '" + MonthYear + "' AND PSM.IS_SALES_RETURN IS NULL ORDER BY PSM.CREATE_DATE DESC";
                }
                else
                {
                    if (DropDownItemID1.Text == "0")
                    {
                        makeSQL = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PPT.PUR_TYPE_NAME, PP.PARTY_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, (PSM.ITEM_AMOUNT + PSM.VAT_AMOUNT) AS TOTAL_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_CHECK, PSM.IS_OBJ_QUERY, PSM.OBJ_QUERY_DES, PSM.OBJ_QUERY_C_DATE, PSM.IS_PRINT, TO_CHAR(PSM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE  FROM PF_SALES_MASTER PSM LEFT JOIN PF_PURCHASE_TYPE PPT ON  PPT.PUR_TYPE_ID = PSM.PUR_TYPE_ID LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PSM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID WHERE  PSM.IS_SALES_RETURN IS NULL AND (PSM.INVOICE_NO like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PSI.SUB_ITEM_NAME like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'mm/yyyy') like '" + txtSearchEmp.Text + "%' or PSM.IS_ACTIVE like '" + txtSearchEmp.Text + "%'  or PSM.IS_CHECK like '" + txtSearchEmp.Text + "%') ORDER BY PSM.CREATE_DATE desc, PSM.UPDATE_DATE desc";
                    }
                    else
                    {
                        makeSQL = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PPT.PUR_TYPE_NAME, PP.PARTY_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, (PSM.ITEM_AMOUNT + PSM.VAT_AMOUNT) AS TOTAL_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_CHECK, PSM.IS_OBJ_QUERY, PSM.OBJ_QUERY_DES, PSM.OBJ_QUERY_C_DATE, PSM.IS_PRINT, TO_CHAR(PSM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE  FROM PF_SALES_MASTER PSM LEFT JOIN PF_PURCHASE_TYPE PPT ON  PPT.PUR_TYPE_ID = PSM.PUR_TYPE_ID LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PSM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID WHERE  PSM.IS_SALES_RETURN IS NULL AND PI.ITEM_ID like '" + DropDownItemID1.Text + "%' AND (PSM.INVOICE_NO like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%'  or PSI.SUB_ITEM_NAME like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'mm/yyyy') like '" + txtSearchEmp.Text + "%' or PSM.IS_ACTIVE like '" + txtSearchEmp.Text + "%'  or PSM.IS_CHECK like '" + txtSearchEmp.Text + "%') ORDER BY PSM.CREATE_DATE desc, PSM.UPDATE_DATE desc";
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


        public void DisplaySummary()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = "";
                string MonthYear = System.DateTime.Now.ToString("MM/yyyy");
                if (TextMonthYear4.Text == "")
                {
                    makeSQL = "  SELECT PI.ITEM_NAME, count(PSM.SALES_ID) AS SALES_ID, sum(PSM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PSM.ITEM_AMOUNT) AS ITEM_AMOUNT,  sum(PSM.VAT_AMOUNT) AS VAT_AMOUNT,  sum(PSM.ITEM_AMOUNT)+ sum(PSM.VAT_AMOUNT) AS TOTAL_AMOUNT FROM PF_ITEM PI LEFT JOIN PF_SALES_MASTER PSM ON PSM.ITEM_ID = PI.ITEM_ID WHERE to_char(PSM.ENTRY_DATE, 'mm/yyyy') =  '" + MonthYear + "' GROUP BY PI.ITEM_ID, PI.ITEM_NAME ORDER BY PI.ITEM_ID";
                }
                else
                { 
                    makeSQL = "  SELECT PI.ITEM_NAME, count(PSM.SALES_ID) AS SALES_ID, sum(PSM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PSM.ITEM_AMOUNT) AS ITEM_AMOUNT,  sum(PSM.VAT_AMOUNT) AS VAT_AMOUNT,  sum(PSM.ITEM_AMOUNT)+ sum(PSM.VAT_AMOUNT) AS TOTAL_AMOUNT FROM PF_ITEM PI LEFT JOIN PF_SALES_MASTER PSM ON PSM.ITEM_ID = PI.ITEM_ID WHERE to_char(PSM.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear4.Text + "' GROUP BY PI.ITEM_ID, PI.ITEM_NAME ORDER BY PI.ITEM_ID";
                    alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataKeyNames = new string[] { "ITEM_NAME" };
                GridView2.DataBind();
                if (dt.Rows.Count > 0)
                {
                    GridView2.HeaderRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                    //Calculate Sum and display in Footer Row 
                    GridView2.FooterRow.Cells[0].Font.Bold = true;
                    GridView2.FooterRow.Cells[0].Text = "Grand Total";
                    GridView2.FooterRow.Cells[0].HorizontalAlign = HorizontalAlign.Right;

                    decimal total_prod = dt.AsEnumerable().Sum(row => row.Field<decimal>("SALES_ID"));
                    GridView2.FooterRow.Cells[1].Font.Bold = true;
                    GridView2.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Center;
                    GridView2.FooterRow.Cells[1].Text = total_prod.ToString("N0");

                    decimal total_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_WEIGHT"));
                    GridView2.FooterRow.Cells[2].Font.Bold = true;
                    GridView2.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[2].Text = total_wt.ToString("N3");

                    decimal total_pge_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_AMOUNT"));
                    GridView2.FooterRow.Cells[3].Font.Bold = true;
                    GridView2.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[3].Text = total_pge_wt.ToString("N2");

                    decimal total_ag_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("VAT_AMOUNT"));
                    GridView2.FooterRow.Cells[4].Font.Bold = true;
                    GridView2.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[4].Text = total_ag_wt.ToString("N2");

                    decimal total_grand_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("TOTAL_AMOUNT"));
                    GridView2.FooterRow.Cells[5].Font.Bold = true;
                    GridView2.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[5].Text = total_grand_wt.ToString("N2");
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
                    if (ItemWeight <= FinalStock)
                    { 
                    StockOutWetNew = StockOutWet - ItemWeightOld;
                    FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;

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
                    FinalStockNew = (InitialStock + StockInWet) - StockOutWetDe;
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
                        }
                        else
                        {
                            alert_box.Visible = true;
                            alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory"));
                            alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                        }

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
                if (ItemWeight <= FinalStock)
                {

                StockOutWetNew = StockOutWet - ItemWeightOld;
                FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;

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
                        FinalStockNew = (InitialStock + StockInWet) - StockOutWetDe;

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
                        else
                        {
                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory"));
                        alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                    }
                }
                    if (ItemWeight <= FinalStock)
                    {
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
                        string update_production = "update PF_SALES_MASTER set PUR_TYPE_ID = :NoSalesTypeID, PARTY_ID = :NoCustomerID, ITEM_ID = :NoItemID, SUB_ITEM_ID = :NoSubItemID, ITEM_WEIGHT_WB = :NoItemWeightWB, ITEM_WEIGHT = :NoItemWeight, ITEM_RATE = :NoItemRate, ITEM_AMOUNT = :NoItemAmount, VAT_ID = :NoVatID, VAT_PERCENT = :NoVatPercent, VAT_AMOUNT = :NoVatAmount, REMARKS = :TextRemarks, ENTRY_DATE = TO_DATE(:TextEntryDate, 'DD-MM-YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where INVOICE_NO = :NoInvoiceNo ";
                        cmdu = new OracleCommand(update_production, conn);

                        OracleParameter[] objPrm = new OracleParameter[17];
                        objPrm[0] = cmdu.Parameters.Add("NoSalesTypeID", SalesTypeID);
                        objPrm[1] = cmdu.Parameters.Add("NoCustomerID", CustomerID);
                        objPrm[2] = cmdu.Parameters.Add("NoItemID", ItemID);
                        objPrm[3] = cmdu.Parameters.Add("NoSubItemID", SubItemID);
                        objPrm[4] = cmdu.Parameters.Add("NoItemWeightWB", TextItemWtWb.Text);
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
                        DisplaySummary();
                    }
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
                    if (ItemWeightOld <= FinalStock)
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
                    FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;

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

                            alert_box.Visible = true;
                            alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory"));
                            alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
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
                    if (ItemWeightOld <= FinalStock)
                    {

                    StockOutWetNew = StockOutWet - ItemWeightOld;
                    FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;

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
                        else
                        {

                            alert_box.Visible = true;
                            alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory"));
                            alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                        }
                    }
                    if (ItemWeightOld <= FinalStock)
                    {
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
                    }
                clearText(); 
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

        public void clearTextField(object sender, EventArgs e)
        {
            TextInvoiceNo.Text = "";   
            TextItemWeight.Text = "";
            TextItemRate.Text = "";
            TextItemAmount.Text = "";
            TextVatAmount.Text = "";
            TextTotalAmount.Text = "";
            DropDownSubItemID.Text = "0"; 
            CheckItemWeight.Text = ""; 
            DropDownCustomerID.Text = "0";  
            DropDownItemID.Text = "0";
            CheckEntryDate.Text = "";
            TextItemWtWb.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        {
            TextInvoiceNo.Text = ""; 
            TextItemWeight.Text = "";
            TextItemRate.Text = ""; 
            TextItemAmount.Text = "";
            TextVatAmount.Text = "";
            TextTotalAmount.Text = "";
            DropDownSubItemID.Text = "0"; 
            CheckItemWeight.Text = ""; 
            DropDownCustomerID.Text = "0";  
            DropDownItemID.Text = "0";
            CheckInvoiceNo.Text = "";
            CheckEntryDate.Text = "";
            TextItemWtWb.Text = "";
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

                            string makeSQL = " SELECT PFSM.ITEM_ID, (nvl(PFSM.FINAL_STOCK_WT,0) - SUM(nvl(PEWC.ITEM_WEIGHT/1000,0))) AS FINAL_STOCK_WT FROM PF_FG_STOCK_INVENTORY_MASTER PFSM LEFT JOIN PF_EXPORT_WBSLIP_CON PEWC ON PEWC.ITEM_ID = PFSM.ITEM_ID AND PEWC.IS_INVENTORY_STATUS = 'Transit' where PFSM.ITEM_ID = '" + ItemID + "' GROUP BY PFSM.ITEM_ID, PFSM.FINAL_STOCK_WT   ";
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
                TextTotalAmount.Text = (ItemAmount+ VatPercentAmt).ToString("0.00");
                TextVatAmount.Text = VatPercentAmt.ToString("0.00");
                
                EntryDate.Focus();
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

        protected void BtnReport_Click3(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad3 = true;
                alert_box.Visible = false;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        protected void BtnReport_Click4(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad4 = true;
                alert_box.Visible = false;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }


    }

}