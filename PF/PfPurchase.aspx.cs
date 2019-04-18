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
using System.Web.Services;

namespace NRCAPPS.PF
{
    public partial class PfPurchase : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl, cmdsp;
        OracleDataAdapter oradata; 
        DataTable dt;
        int RowCount;
        double ItemVatAmt = 0.0, ItemAmtTotal = 0.0, ItemWtWbTotal = 0.0; string EntryDateSlip = "", PartyArabicName = "", PartyName ="";
        string IS_PAGE_ACTIVE = "", IS_ADD_ACTIVE = "",IS_EDIT_ACTIVE = "",IS_DELETE_ACTIVE = "",IS_VIEW_ACTIVE = "", IS_REPORT_ACTIVE = "", IS_PRINT_ACTIVE ="";

        public bool IsLoad { get; set; }  public bool IsLoad2 { get; set; } public bool IsLoad3 { get; set; } public bool IsLoad4 { get; set; } 
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
                    IS_PRINT_ACTIVE = dt.Rows[i]["IS_PRINT_ACTIVE"].ToString();
                }

                if (IS_PAGE_ACTIVE == "Enable")
                {
                     if (!IsPostBack)
                    {
                        DataTable dtSupplierID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeSupplierSQL = " SELECT PARTY_ID,  PARTY_ID || ' - ' || PARTY_NAME || ' - ' || PARTY_VAT_NO AS PARTY_NAME  FROM PF_PARTY WHERE IS_ACTIVE = 'Enable' ORDER BY PARTY_NAME ASC";
                        ds = ExecuteBySqlString(makeSupplierSQL);
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

                        DropDownSupplierID3.DataSource = dtSupplierID;
                        DropDownSupplierID3.DataValueField = "PARTY_ID";
                        DropDownSupplierID3.DataTextField = "PARTY_NAME";
                        DropDownSupplierID3.DataBind();
                        DropDownSupplierID3.Items.Insert(0, new ListItem("Select  Supplier", "0"));

                        DataTable dtSupervisorID = new DataTable();
                        DataSet dsd = new DataSet();
                        string makeSupervisorSQL = " SELECT * FROM PF_SUPERVISOR WHERE IS_ACTIVE = 'Enable' ORDER BY SUPERVISOR_NAME ASC";
                        dsd = ExecuteBySqlString(makeSupervisorSQL);
                        dtSupplierID = (DataTable)dsd.Tables[0];
                        DropDownSupervisorID.DataSource = dtSupplierID;
                        DropDownSupervisorID.DataValueField = "SUPERVISOR_ID";
                        DropDownSupervisorID.DataTextField = "SUPERVISOR_NAME";
                        DropDownSupervisorID.DataBind();
                        DropDownSupervisorID.Items.Insert(0, new ListItem("Select  Supervisor", "0"));

                        DropDownSupervisorID2.DataSource = dtSupplierID;
                        DropDownSupervisorID2.DataValueField = "SUPERVISOR_ID";
                        DropDownSupervisorID2.DataTextField = "SUPERVISOR_NAME";
                        DropDownSupervisorID2.DataBind();
                        DropDownSupervisorID2.Items.Insert(0, new ListItem("Select  Supervisor", "0"));

                        DataTable dtPurchaseTypeID = new DataTable();
                        DataSet dsl = new DataSet();
                        string makePurchaseTypeSQL = " SELECT * FROM PF_PURCHASE_TYPE WHERE IS_ACTIVE = 'Enable' ORDER BY PUR_TYPE_ID DESC";
                        dsl = ExecuteBySqlString(makePurchaseTypeSQL);
                        dtPurchaseTypeID = (DataTable)dsl.Tables[0];
                        DropDownPurchaseTypeID.DataSource = dtPurchaseTypeID;
                        DropDownPurchaseTypeID.DataValueField = "PUR_TYPE_ID";
                        DropDownPurchaseTypeID.DataTextField = "PUR_TYPE_NAME";
                        DropDownPurchaseTypeID.DataBind();
                     //   DropDownPurchaseTypeID.Items.Insert(0, new ListItem("Select  Purchase Type", "0"));

                        DropDownPurchaseTypeID2.DataSource = dtPurchaseTypeID;
                        DropDownPurchaseTypeID2.DataValueField = "PUR_TYPE_ID";
                        DropDownPurchaseTypeID2.DataTextField = "PUR_TYPE_NAME";
                        DropDownPurchaseTypeID2.DataBind();
                     //   DropDownPurchaseTypeID2.Items.Insert(0, new ListItem("Select  Purchase Type", "0"));


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
                        DropDownSubItemID.Items.FindByValue("1").Selected = true;
                        //  DropDownSubItemID.Items.Insert(0, new ListItem("Select Sub Item", "0"));

                        DataTable dtPgeID = new DataTable();
                        DataSet dsp = new DataSet();
                        string makeDropDownPgeSQL = " SELECT * FROM NRC_VAT WHERE IS_ACTIVE = 'Enable' ORDER BY VAT_ID ASC";
                        dsp = ExecuteBySqlString(makeDropDownPgeSQL);
                        dtPgeID = (DataTable)dsp.Tables[0];
                        DropDownVatID.DataSource = dtPgeID;
                        DropDownVatID.DataValueField = "VAT_ID";
                        DropDownVatID.DataTextField = "VAT_PERCENT";
                        DropDownVatID.DataBind();

                        TextSlipNo.Focus(); 
                        TextItemAmount.Attributes.Add("readonly", "readonly");
                        TextItemVatAmount.Attributes.Add("readonly", "readonly");
                        TextTotalAmount.Attributes.Add("readonly", "readonly");
                         
                        //  btnPrint.Enabled = false;
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
                    int SupplierID = Convert.ToInt32(DropDownSupplierID.Text);
                    int SlipNo = Convert.ToInt32(TextSlipNo.Text);
                    int SupervisorID   = Convert.ToInt32(DropDownSupervisorID.Text);
                    int PurchaseTypeID   = Convert.ToInt32(DropDownPurchaseTypeID.Text); 
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
                    double ItemRate = Convert.ToDouble(TextItemRate.Text.Trim());
                    double ItemWeight = Convert.ToDouble(TextItemWeight.Text.Trim());  
                    double ItemAmount = ItemRate * ItemWeight;

                    int VatID = 0; double VatPercent = 0.00, VatAmount = 0.00;
                    if (RadioBtnVat.SelectedValue == "VatYes")
                    { 
                      VatID = Convert.ToInt32(DropDownVatID.Text);
                      VatPercent = Convert.ToDouble(DropDownVatID.SelectedItem.Text); 
                      VatAmount = (ItemAmount * Convert.ToDouble(DropDownVatID.SelectedItem.Text)) / 100;  
                    } else  { }
                     
                    string ItemAmountNew = Math.Round(decimal.Parse(ItemAmount.ToString()), 2).ToString();
                    double ItemAmountNewD = Convert.ToDouble(ItemAmountNew);

                    string get_user_purchase_id = "select PF_PURCHASE_MASTERID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_user_purchase_id, conn);
                    int newPurchaseID = Int16.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_purchase = "insert into  PF_PURCHASE_MASTER (PURCHASE_ID, SLIP_NO, PARTY_ID, PUR_TYPE_ID, ITEM_ID, SUB_ITEM_ID, SUPERVISOR_ID, ITEM_WEIGHT, ITEM_RATE, ITEM_AMOUNT, ENTRY_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID, VAT_ID, VAT_PERCENT, VAT_AMOUNT, ITEM_WEIGHT_WB) values  ( :NoPurchaseID, :NoSlipID, :NoSupplierID, :NoPurchaseTypeID, :NoItemID, :NoSubItemID, :NoSupervisorID, :TextItemWeight, :TextItemRate, :TextItemAmount, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 3, :NoVatID, :NoVatPercent, :NoVatAmount, :TextItemWtWb)";
                    cmdi = new OracleCommand(insert_purchase, conn);
                      
                    OracleParameter[] objPrm = new OracleParameter[18];
                    objPrm[0] = cmdi.Parameters.Add("NoPurchaseID", newPurchaseID); 
                    objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNo); 
                    objPrm[2] = cmdi.Parameters.Add("NoSupplierID", SupplierID); 
                    objPrm[3] = cmdi.Parameters.Add("NoPurchaseTypeID", PurchaseTypeID);
                    objPrm[4] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[5] = cmdi.Parameters.Add("NoSubItemID", SubItemID); 
                    objPrm[6] = cmdi.Parameters.Add("NoSupervisorID", SupervisorID);
                    objPrm[7] = cmdi.Parameters.Add("TextItemWeight", ItemWeight);
                    objPrm[8] = cmdi.Parameters.Add("TextItemRate", ItemRate);
                    objPrm[9] = cmdi.Parameters.Add("TextItemAmount", ItemAmountNewD); 
                    objPrm[10] = cmdi.Parameters.Add("EntryDate", EntryDateNew); 
                    objPrm[11] = cmdi.Parameters.Add("c_date", c_date); 
                    objPrm[12] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[13] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[14] = cmdi.Parameters.Add("NoVatID", VatID);
                    objPrm[15] = cmdi.Parameters.Add("NoVatPercent", VatPercent);
                    objPrm[16] = cmdi.Parameters.Add("NoVatAmount", VatAmount);
                    objPrm[17] = cmdi.Parameters.Add("TextItemWtWb", TextItemWtWb.Text);

                    cmdi.ExecuteNonQuery();

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();


                    //inventory calculation
                     
                    int InvenItemID = 0;
                    int InvenSubItemID = 0;
                    double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, StockOutWetDe = 0.00, FinalStock = 0.00, StockInWetNew = 0.00, FinalStockNew = 0.00;

                    if (PurchaseTypeID == 1)
                    {

                        string makeSQL = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  AND SUB_ITEM_ID  = '" + SubItemID + "' ";
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

                        StockInWetNew = StockInWet + ItemWeight;
                        FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;

                        if (0 < RowCount)
                        {

                            string update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                            cmdu = new OracleCommand(update_inven_mas, conn);

                            OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                            objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                            objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                            objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                            objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                            objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);

                            cmdu.ExecuteNonQuery();
                            cmdu.Parameters.Clear();
                            cmdu.Dispose();
                        }
                        else
                        {

                            FinalStockNew = (InitialStock + ItemWeight) - StockOutWet;

                            string get_inven_mas_id = "select PF_FG_STOCK_INVEN_MASID_SEQ.nextval from dual";
                            cmdsp = new OracleCommand(get_inven_mas_id, conn);
                            int newInvenMasRmID = Int16.Parse(cmdsp.ExecuteScalar().ToString());

                            string insert_inven_rm = "insert into  PF_FG_STOCK_INVENTORY_MASTER (FG_INVENTORY_ID, ITEM_ID, ITEM_NAME, SUB_ITEM_ID, SUB_ITEM_NAME, INITIAL_STOCK_WT, STOCK_IN_WT, STOCK_OUT_WT, FINAL_STOCK_WT, CREATE_DATE, C_USER_ID) values ( :NoInvenMasRmID, :NoItemID, :TextItemName, :NoSubItemID, :TextSubItemName, :NoInitialStock, :NoStockIn, :NoStockOut, :NoFinalStock, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                            cmdi = new OracleCommand(insert_inven_rm, conn);

                            OracleParameter[] objPrmIrm = new OracleParameter[11];
                            objPrmIrm[0] = cmdi.Parameters.Add("NoInvenMasRmID", newInvenMasRmID);
                            objPrmIrm[1] = cmdi.Parameters.Add("NoItemID", ItemID);
                            objPrmIrm[2] = cmdi.Parameters.Add("TextItemName", ItemName);
                            objPrmIrm[3] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                            objPrmIrm[4] = cmdi.Parameters.Add("TextSubItemName", SubItemName);
                            objPrmIrm[5] = cmdi.Parameters.Add("NoInitialStock", InitialStock);
                            objPrmIrm[6] = cmdi.Parameters.Add("NoStockIn", ItemWeight);
                            objPrmIrm[7] = cmdi.Parameters.Add("NoStockOut", StockOutWet);
                            objPrmIrm[8] = cmdi.Parameters.Add("NoFinalStock", FinalStockNew);
                            objPrmIrm[9] = cmdi.Parameters.Add("c_date", c_date);
                            objPrmIrm[10] = cmdi.Parameters.Add("NoCuserID", userID);

                            cmdi.ExecuteNonQuery();

                            cmdi.Parameters.Clear();
                            cmdi.Dispose();

                        }
                            string InventoryFor = "Finished Goods Produced (Direct)";
                            string get_inven_mas_des_id = "select PF_FG_STOCK_INVEN_MASDESID_SEQ.nextval from dual";
                            cmdsp = new OracleCommand(get_inven_mas_des_id, conn);
                            int newInvenMasDesFgID = Int16.Parse(cmdsp.ExecuteScalar().ToString());
                            string insert_inven_fg_des = "insert into  PF_FG_STOCK_INVENTORY_MAS_DE (FG_INVEN_DE_ID, REF_ID, ITEM_ID, ITEM_NAME, SUB_ITEM_ID, SUB_ITEM_NAME, INVENTORY_FOR, STOCK_IN_WT, STOCK_OUT_WT, CREATE_DATE, C_USER_ID) values ( :NoInvenMasFgID, :NoRefID, :NoItemID, :TextItemName, :NoSubItemID, :TextSubItemName, :TextInventoryFor, :NoStockIn, :NoStockOut, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                            cmdi = new OracleCommand(insert_inven_fg_des, conn);

                            OracleParameter[] objPrmIrmd = new OracleParameter[11];
                            objPrmIrmd[0] = cmdi.Parameters.Add("NoInvenMasFgID", newInvenMasDesFgID);
                            objPrmIrmd[1] = cmdi.Parameters.Add("NoRefID", TextSlipNo.Text);
                            objPrmIrmd[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                            objPrmIrmd[3] = cmdi.Parameters.Add("TextItemName", ItemName);
                            objPrmIrmd[4] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                            objPrmIrmd[5] = cmdi.Parameters.Add("TextSubItemName", SubItemName);
                            objPrmIrmd[6] = cmdi.Parameters.Add("TextInventoryFor", InventoryFor);
                            objPrmIrmd[7] = cmdi.Parameters.Add("NoStockIn", ItemWeight);
                            objPrmIrmd[8] = cmdi.Parameters.Add("NoStockOut", StockOutWetDe);
                            objPrmIrmd[9] = cmdi.Parameters.Add("c_date", c_date);
                            objPrmIrmd[10] = cmdi.Parameters.Add("NoCuserID", userID);

                            cmdi.ExecuteNonQuery(); 
                            cmdi.Parameters.Clear();
                            cmdi.Dispose(); 

                    }
                    else {

                        string makeSQL = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
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

                        StockInWetNew = StockInWet + ItemWeight;
                        FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet; 

                        if (0 < RowCount)
                        {

                            string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                            cmdu = new OracleCommand(update_inven_mas, conn);

                            OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                            objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                            objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                            objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                            objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                            objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);

                            cmdu.ExecuteNonQuery();
                            cmdu.Parameters.Clear();
                            cmdu.Dispose();
                        }
                        else {

                            FinalStockNew = (InitialStock + ItemWeight) - StockOutWet;

                            string get_inven_mas_id = "select PF_RM_STOCK_INVEN_MASID_SEQ.nextval from dual";
                            cmdsp = new OracleCommand(get_inven_mas_id, conn);
                            int newInvenMasRmID = Int16.Parse(cmdsp.ExecuteScalar().ToString());

                            string insert_inven_rm = "insert into  PF_RM_STOCK_INVENTORY_MASTER (RM_INVENTORY_ID, ITEM_ID, ITEM_NAME, SUB_ITEM_ID, SUB_ITEM_NAME, INITIAL_STOCK_WT, STOCK_IN_WT, STOCK_OUT_WT, FINAL_STOCK_WT, CREATE_DATE, C_USER_ID) values ( :NoInvenMasRmID, :NoItemID, :TextItemName, :NoSubItemID, :TextSubItemName, :NoInitialStock, :NoStockIn, :NoStockOut, :NoFinalStock, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                            cmdi = new OracleCommand(insert_inven_rm, conn);

                            OracleParameter[] objPrmIrm = new OracleParameter[11];
                            objPrmIrm[0] = cmdi.Parameters.Add("NoInvenMasRmID", newInvenMasRmID);
                            objPrmIrm[1] = cmdi.Parameters.Add("NoItemID", ItemID);
                            objPrmIrm[2] = cmdi.Parameters.Add("TextItemName", ItemName);
                            objPrmIrm[3] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                            objPrmIrm[4] = cmdi.Parameters.Add("TextSubItemName", SubItemName); 
                            objPrmIrm[5] = cmdi.Parameters.Add("NoInitialStock", InitialStock);                       
                            objPrmIrm[6] = cmdi.Parameters.Add("NoStockIn", ItemWeight);
                            objPrmIrm[7] = cmdi.Parameters.Add("NoStockOut", StockOutWet);
                            objPrmIrm[8] = cmdi.Parameters.Add("NoFinalStock", FinalStockNew);
                            objPrmIrm[9] = cmdi.Parameters.Add("c_date", c_date);
                            objPrmIrm[10] = cmdi.Parameters.Add("NoCuserID", userID); 

                            cmdi.ExecuteNonQuery(); 
                            cmdi.Parameters.Clear();
                            cmdi.Dispose(); 
                        }
                        string PurchaseIdString = newPurchaseID.ToString();
                        string InventoryFor = "Raw Material Received";
                        string get_inven_mas_des_id = "select PF_RM_STOCK_INVEN_MASDESID_SEQ.nextval from dual";
                        cmdsp = new OracleCommand(get_inven_mas_des_id, conn);
                        int newInvenMasDesRmID = Int16.Parse(cmdsp.ExecuteScalar().ToString());
                        string insert_inven_rm_des = "insert into  PF_RM_STOCK_INVENTORY_MAS_DE (RM_INVEN_DE_ID, REF_ID, ITEM_ID, ITEM_NAME, SUB_ITEM_ID, SUB_ITEM_NAME, INVENTORY_FOR, STOCK_IN_WT, STOCK_OUT_WT, CREATE_DATE, C_USER_ID, PURCHASE_ID) values ( :NoInvenMasRmID, :NoRefID, :NoItemID, :TextItemName, :NoSubItemID, :TextSubItemName, :TextInventoryFor, :NoStockIn, :NoStockOut, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :NoPurchaseID)";
                        cmdi = new OracleCommand(insert_inven_rm_des, conn);

                        OracleParameter[] objPrmIrmd = new OracleParameter[12];
                        objPrmIrmd[0] = cmdi.Parameters.Add("NoInvenMasRmID", newInvenMasDesRmID);
                        objPrmIrmd[1] = cmdi.Parameters.Add("NoRefID", TextSlipNo.Text);
                        objPrmIrmd[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                        objPrmIrmd[3] = cmdi.Parameters.Add("TextItemName", ItemName);
                        objPrmIrmd[4] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                        objPrmIrmd[5] = cmdi.Parameters.Add("TextSubItemName", SubItemName);
                        objPrmIrmd[6] = cmdi.Parameters.Add("TextInventoryFor", InventoryFor);
                        objPrmIrmd[7] = cmdi.Parameters.Add("NoStockIn", ItemWeight);
                        objPrmIrmd[8] = cmdi.Parameters.Add("NoStockOut", StockOutWetDe); 
                        objPrmIrmd[9] = cmdi.Parameters.Add("c_date", c_date);
                        objPrmIrmd[10] = cmdi.Parameters.Add("NoCuserID", userID);
                        objPrmIrmd[11] = cmdi.Parameters.Add("NoPurchaseID", PurchaseIdString);
                        cmdi.ExecuteNonQuery(); 
                        cmdi.Parameters.Clear();
                        cmdi.Dispose(); 
                    
                    }
                     
                    conn.Close();

                   // System.Threading.Thread.Sleep(4000);
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert new Purchase successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                   
                    clearText();
                    TextSlipNo.Focus();
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

        protected void btnPrint_Click(object sender, EventArgs e)
        {
          if (IS_PRINT_ACTIVE == "Enable")
             {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            string HtmlString = "";
            string makeSQL = "  SELECT PPM.PURCHASE_ID, PPM.SLIP_NO,PP.PARTY_NAME, PP.PARTY_ARABIC_NAME, PP.PARTY_ADD_1 || ', ' || PP.PARTY_ADD_2 AS PARTY_ADD, PP.PARTY_VAT_NO, PI.ITEM_CODE, PI.ITEM_NAME, PI.ITEM_ARABIC_NAME, PPM.ITEM_WEIGHT, nvl(PPM.ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, PPM.ITEM_RATE, ROUND(PPM.ITEM_AMOUNT,2) AS ITEM_AMOUNT, nvl(PPM.VAT_AMOUNT, 0) AS VAT_AMOUNT, TO_CHAR(PPM.ENTRY_DATE, 'dd-MON-yyyy') AS ENTRY_DATE, HE.EMP_LNAME FROM PF_PURCHASE_MASTER PPM LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PPM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_WB_OPERATOR PWO ON PWO.IS_ACTIVE = 'Enable' LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PWO.EMP_ID WHERE PPM.SLIP_NO = '" + TextSlipNo.Text + "' ORDER BY PI.ITEM_ID";

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
                  
                HtmlString += "<div style='float:left;width:785px;height:258px;margin-top:278px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";
                HtmlString += "<div style='float:left;width:380px;'> "; 
                HtmlString += "<div style='float:left;width:260px;height:102px;margin-left:100px;padding-top:10px;text-align:center;' ><font size='1px'> " + PartyArabicName + "</font> </br> " + PartyName + " </div> ";
                HtmlString += "<div style='display:none;float:left;width:210px;height:33px; margin:0 0 0 90px;'>" + PartyAdd + "</div> <div style='float:left;width:210px; margin:33px 0 0 90px;'>" + PartyVatNo + " </div> ";
                HtmlString += "</div> "; 
            }
            int m = 1;
            HtmlString += "<div style='float:left;width:380px;'> ";
            HtmlString += "<div style='float:left;width:240px;height:42px;margin:30px 0 0 140px;'> ";
            for (int i = 0; i < RowCount; i++)
                { 
                    string ItemName = dt.Rows[i]["ITEM_NAME"].ToString();
                //   string ItemArabicName = dt.Rows[i]["ITEM_ARABIC_NAME"].ToString(); 
                if (m == RowCount) {
                    HtmlString += "" + ItemName + "";
                } else {  HtmlString += "" + ItemName + ",";  }
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
                double TotalInvoiceAmt =+ Convert.ToDouble(decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00"));

                HtmlString += "<div style='float:left;width:242px;' >" + ItemCode + "     " + ItemName + "-<font size='1px'>" + ItemArabicName + "</font></div> ";
                HtmlString += "<div style='float:left;width:82px;text-align:center;'>" + string.Format("{0:n0}", ItemWtWb) + " </div> ";
                HtmlString += "<div style='float:left;width:93px;text-align:center;'>" + string.Format("{0:n0}", VarianceWT) + " </div> ";
                HtmlString += "<div style='float:left;width:112px;text-align:right;'>" + string.Format("{0:n0}", ItemWt) + " </div> ";
                HtmlString += "<div style='float:left;width:107px;text-align:right;'>" + ItemRate.ToString("0.000") + " </div> ";
                HtmlString += "<div style='float:left;width:120px;text-align:right;height:25px'>" + string.Format("{0:n2}", ItemAmt)  + " </div> ";
                 
            }
            HtmlString += "</div>";
            HtmlString += "<div style='float:left;width:785px;height:238px;margin:0 0 0 8px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'><div style='float:left;width:242px;border:white solid 1px' ></div>";
            HtmlString += "<div style='float:left;width:82px;text-align:center;margin:0 0 10px 0;'>" + string.Format("{0:n0}", ItemWtWbTotal)  + " </div> ";
            HtmlString += "<div style='float:left;width:431px;text-align:right;margin:0 0 10px 0;'>" + string.Format("{0:n2}", ItemAmtTotal) + " </div> ";
            HtmlString += "<div style='float:left;width:757px;text-align:right;margin:0 0 10px 0;'>" + string.Format("{0:n2}", ItemVatAmt) + " </div> "; 
            HtmlString += "<div style='float:left;width:757px;text-align:right;'>" + string.Format("{0:n2}", ItemAmtTotal +ItemVatAmt)  + " </div> ";
            HtmlString += "<div style='float:left;width:500px;margin:0 0 0 120px;text-align:left;'>" + EntryDateSlip + " </div> "; 
            string NumberToInWord = NumberToInWords.DecimalToWordsSR(Convert.ToDecimal(ItemAmtTotal + ItemVatAmt)).Trim().ToUpper();
            HtmlString += "<div style = 'float:left;width:290px;height:88px;margin:40px 0 0 460px;padding:10px;text-align:left;'>" + NumberToInWord + " </div> ";
            HtmlString += "<div style = 'float:left;width:200px;margin:0 0 0 555px;text-align:left;'><font size='1px'>" + PartyName + "|" + PartyArabicName + "</font> </div> ";
            HtmlString += "</div>";
            HtmlString += "</div>";
            HtmlString += "</div>";

            // purchase master update for print
            int userID = Convert.ToInt32(Session["USER_ID"]);
            string SlipNo = TextSlipNo.Text; 
            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
            string update_user = "update  PF_PURCHASE_MASTER  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where SLIP_NO = :NoSlipNo ";
            cmdi = new OracleCommand(update_user, conn);

            OracleParameter[] objPrm = new OracleParameter[4];
            objPrm[0] = cmdi.Parameters.Add("TextIsPrint", "Printed"); 
            objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
            objPrm[2] = cmdi.Parameters.Add("NoCuserID", userID); 
            objPrm[3] = cmdi.Parameters.Add("NoSlipNo", SlipNo);

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

        public void Redio_CheckedChanged(object sender, EventArgs e)
        {
            if (radPurDuplicate.SelectedValue == "One")
            {
                TextSlipNo.Focus();
                CheckSlipNo.Visible = true;
                BtnAdd.Attributes.Add("aria-disabled", "false");
                BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 
            }
            else
            {
                TextSlipNo.Focus();
                CheckSlipNo.Visible = false;
                BtnAdd.Attributes.Add("aria-disabled", "true");
                BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            }
        }

        public void Redio_VatChanged(object sender, EventArgs e)
        {
            if (RadioBtnVat.SelectedValue == "VatNo")
            {
                if (TextItemWeight.Text != "" && TextItemRate.Text != "")
                {
                    EntryDate.Focus();
                    VatPercentBox.Visible = false;
                    Double ItemAmount = Convert.ToDouble(TextItemWeight.Text) * Convert.ToDouble(TextItemRate.Text);
                    TextItemAmount.Text = decimal.Parse(ItemAmount.ToString()).ToString(".00");
                    TextTotalAmount.Text = decimal.Parse(ItemAmount.ToString()).ToString(".00");
                }
            }
            else
            {
                if (TextItemWeight.Text != "" && TextItemRate.Text != "") {
                    EntryDate.Focus();
                    VatPercentBox.Visible = true; 
                    Double ItemAmount = Convert.ToDouble(TextItemWeight.Text) * Convert.ToDouble(TextItemRate.Text);
                    Double VatAmount = (ItemAmount * Convert.ToDouble(DropDownVatID.SelectedItem.Text))/100;
                    TextItemVatAmount.Text = decimal.Parse(VatAmount.ToString()).ToString(".00");
                    TextItemAmount.Text = decimal.Parse(ItemAmount.ToString()).ToString(".00");
                    double TotalAmt = Math.Round(ItemAmount + VatAmount);
                    TextTotalAmount.Text = decimal.Parse(TotalAmt.ToString()).ToString(".00");
                }
               
            }
        }

        [WebMethod]
        public static Boolean SlipNoCheck(int SlipNo)
        {
            Boolean result = false;
            string query = "select SLIP_NO from PF_PURCHASE_MASTER where SLIP_NO = '" + SlipNo + "'";
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


        protected void linkSelectClick(object sender, EventArgs e)
        {
       //   try{
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
            int USER_DATA_ID = Convert.ToInt32(Session["user_data_id"]);
                 
            string makeSQL = " select PURCHASE_ID, SLIP_NO,  PARTY_ID, PUR_TYPE_ID, ITEM_ID, SUB_ITEM_ID, SUPERVISOR_ID, ITEM_WEIGHT, nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, ITEM_RATE, ROUND(ITEM_AMOUNT,2) AS ITEM_AMOUNT, nvl(VAT_ID,0) AS VAT_ID, ROUND(VAT_AMOUNT,2) AS VAT_AMOUNT, TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, CREATE_DATE,  UPDATE_DATE,  C_USER_ID, U_USER_ID, IS_ACTIVE from PF_PURCHASE_MASTER where PURCHASE_ID  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                TextPurchaseID.Text = dt.Rows[i]["PURCHASE_ID"].ToString();              
                TextSlipNo.Text = dt.Rows[i]["SLIP_NO"].ToString();
                DropDownSupplierID.Text = dt.Rows[i]["PARTY_ID"].ToString();
                DropDownSupervisorID.Text = dt.Rows[i]["SUPERVISOR_ID"].ToString();
                DropDownPurchaseTypeID.Text = dt.Rows[i]["PUR_TYPE_ID"].ToString();
                DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();
                DropDownSubItemID.Text = dt.Rows[i]["SUB_ITEM_ID"].ToString();
                TextItemRate.Text = dt.Rows[i]["ITEM_RATE"].ToString(); 
                TextItemWeight.Text  = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT"].ToString()).ToString(".000");
                TextItemAmount.Text = decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00");
                double ItemAmount = Convert.ToDouble(dt.Rows[i]["ITEM_AMOUNT"]);
                double VatAmount = Convert.ToDouble(dt.Rows[i]["VAT_AMOUNT"]);
                double TotalAmount = Math.Round(ItemAmount + VatAmount);
                TextTotalAmount.Text = decimal.Parse(TotalAmount.ToString()).ToString(".00");
                EntryDate.Text = dt.Rows[i]["ENTRY_DATE"].ToString();
                TextItemWtWb.Text  = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString()).ToString(".000");
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                  
                RadioBtnVat.Enabled = true;
                if (dt.Rows[i]["VAT_ID"].ToString() != "0")
                {
                    RadioBtnVat.Text = "VatYes";
                    VatPercentBox.Style.Remove("display");
                    TextItemVatAmount.Text = decimal.Parse(dt.Rows[i]["VAT_AMOUNT"].ToString()).ToString(".00");
                    DropDownVatID.Text = dt.Rows[i]["VAT_ID"].ToString();
                }
                else
                {
                    RadioBtnVat.Text = "VatNo";
                    VatPercentBox.Style.Add("display", "none");
                }


            }

            conn.Close();
            Display();
            CheckSlipNo.Text = "";
            alert_box.Visible = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

            radPurDuplicate.Enabled = false;
            TextSlipNo.Enabled = false; 
            DropDownPurchaseTypeID.Enabled = false;
          //  DropDownItemID.Enabled = false;
          //  DropDownSubItemID.Enabled = false;
          //  TextItemWeight.Enabled = false;
          //  TextItemWtWb.Enabled = false;

            //  }
            //  catch
            //  {
            //      Response.Redirect("~/ParameterError.aspx");
            //  } 
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
                    makeSQL = " SELECT PPM.PURCHASE_ID, PPM.SLIP_NO, PPM.PARTY_ID, PP.PARTY_NAME, PPM.PUR_TYPE_ID, PPT.PUR_TYPE_NAME, PPM.ITEM_ID, PI.ITEM_NAME, PPM.SUB_ITEM_ID, PSI.SUB_ITEM_NAME, PPM.SUPERVISOR_ID, PS.SUPERVISOR_NAME, PPM.ITEM_WEIGHT,  PPM.ITEM_RATE, PPM.ITEM_AMOUNT, nvl(PPM.ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, PPM.VAT_AMOUNT, ROUND(PPM.ITEM_AMOUNT+PPM.VAT_AMOUNT) AS TOTAL_AMOUNT, PPM.ENTRY_DATE, PPM.CREATE_DATE, PPM.UPDATE_DATE, PPM.IS_ACTIVE , PPC.IS_CHECK, PPM.IS_PRINT, TO_CHAR(PPM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE FROM PF_PURCHASE_MASTER PPM LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PPM.PARTY_ID LEFT JOIN PF_PURCHASE_TYPE PPT ON PPT.PUR_TYPE_ID = PPM.PUR_TYPE_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPM.SUPERVISOR_ID  LEFT JOIN PF_PURCHASE_CLAIM PPC ON  PPC.CLAIM_NO = PPM.CLAIM_NO WHERE to_char(PPM.ENTRY_DATE, 'mm/yyyy') = '" + MonthYear + "' ORDER BY PPM.CREATE_DATE DESC ";
                }
                else
                {
                    if (DropDownItemID1.Text == "0")
                    {
                        makeSQL = " SELECT PPM.PURCHASE_ID, PPM.SLIP_NO, PPM.PARTY_ID, PP.PARTY_NAME, PPM.PUR_TYPE_ID, PPT.PUR_TYPE_NAME, PPM.ITEM_ID, PI.ITEM_NAME, PPM.SUB_ITEM_ID, PSI.SUB_ITEM_NAME, PPM.SUPERVISOR_ID, PS.SUPERVISOR_NAME, PPM.ITEM_WEIGHT, PPM.ITEM_RATE, PPM.ITEM_AMOUNT, nvl(PPM.ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB,  PPM.VAT_AMOUNT, ROUND(PPM.ITEM_AMOUNT+PPM.VAT_AMOUNT) AS TOTAL_AMOUNT, PPM.ENTRY_DATE, PPM.CREATE_DATE, PPM.UPDATE_DATE, PPM.IS_ACTIVE , PPC.IS_CHECK, PPM.IS_PRINT, PPM.PRINT_DATE FROM PF_PURCHASE_MASTER PPM LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PPM.PARTY_ID LEFT JOIN PF_PURCHASE_TYPE PPT ON PPT.PUR_TYPE_ID = PPM.PUR_TYPE_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPM.SUPERVISOR_ID  LEFT JOIN PF_PURCHASE_CLAIM PPC ON  PPC.CLAIM_NO = PPM.CLAIM_NO where PPM.SLIP_NO like '" + txtSearchEmp.Text + "%' or PPM.PARTY_ID like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PS.SUPERVISOR_NAME like '" + txtSearchEmp.Text + "%' or PPM.ITEM_RATE like '" + txtSearchEmp.Text + "%'  or to_char(PPM.ENTRY_DATE, 'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or to_char(PPM.ENTRY_DATE, 'mm/yyyy')  like '" + txtSearchEmp.Text + "%' or PPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' or PPC.IS_CHECK like '" + txtSearchEmp.Text + "%' ORDER BY PPM.SLIP_NO asc";  // PPM.CREATE_DATE desc, PPM.UPDATE_DATE desc
                    }
                    else
                    {
                        makeSQL = " SELECT PPM.PURCHASE_ID, PPM.SLIP_NO, PPM.PARTY_ID, PP.PARTY_NAME, PPM.PUR_TYPE_ID, PPT.PUR_TYPE_NAME, PPM.ITEM_ID, PI.ITEM_NAME, PPM.SUB_ITEM_ID, PSI.SUB_ITEM_NAME, PPM.SUPERVISOR_ID, PS.SUPERVISOR_NAME, PPM.ITEM_WEIGHT, PPM.ITEM_RATE, PPM.ITEM_AMOUNT, nvl(PPM.ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, PPM.VAT_AMOUNT, ROUND(PPM.ITEM_AMOUNT+PPM.VAT_AMOUNT) AS TOTAL_AMOUNT, PPM.ENTRY_DATE, PPM.CREATE_DATE, PPM.UPDATE_DATE, PPM.IS_ACTIVE , PPC.IS_CHECK, PPM.IS_PRINT, PPM.PRINT_DATE FROM PF_PURCHASE_MASTER PPM LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PPM.PARTY_ID LEFT JOIN PF_PURCHASE_TYPE PPT ON PPT.PUR_TYPE_ID = PPM.PUR_TYPE_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPM.SUPERVISOR_ID  LEFT JOIN PF_PURCHASE_CLAIM PPC ON  PPC.CLAIM_NO = PPM.CLAIM_NO where  PI.ITEM_ID like '" + DropDownItemID1.Text + "%' AND (PPM.SLIP_NO like '" + txtSearchEmp.Text + "%' or PPM.PARTY_ID like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%'  or PS.SUPERVISOR_NAME like '" + txtSearchEmp.Text + "%' or PPM.ITEM_RATE like '" + txtSearchEmp.Text + "%'  or to_char(PPM.ENTRY_DATE, 'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or to_char(PPM.ENTRY_DATE, 'mm/yyyy')  like '" + txtSearchEmp.Text + "%' or PPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' or PPC.IS_CHECK like '" + txtSearchEmp.Text + "%') ORDER BY PPM.SLIP_NO asc";  // PPM.CREATE_DATE desc, PPM.UPDATE_DATE desc
                    }

                 //   alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView4D.DataSource = dt;
                GridView4D.DataKeyNames = new string[] { "PURCHASE_ID" };
                GridView4D.DataBind();
                conn.Close();
                // alert_box.Visible = false;
            }
        }

        protected void gridViewFileInformation_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow Row in GridView4D.Rows)
            {
                string isCheck = (Row.FindControl("IsCmoCheckLink") as Label).Text;
                string isCheckPrint = (Row.FindControl("IsPrintedCheckLink") as Label).Text;
                if (isCheck == "Complete")  // || isCheckPrint == "Printed"
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
            GridView4D.PageIndex = e.NewPageIndex;
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
                    makeSQL = " SELECT  PI.ITEM_NAME, count(PPM.SLIP_NO) AS SLIP_NO, sum(PPM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PPM.ITEM_AMOUNT) AS ITEM_AMOUNT, round(sum(PPM.ITEM_AMOUNT)/sum(PPM.ITEM_WEIGHT),2) AS ITEM_AVG_RATE FROM PF_ITEM PI LEFT JOIN PF_PURCHASE_MASTER PPM ON PPM.ITEM_ID = PI.ITEM_ID WHERE to_char(PPM.ENTRY_DATE, 'mm/yyyy') = '" + MonthYear + "' GROUP BY PI.ITEM_ID, PI.ITEM_NAME ORDER BY PI.ITEM_ID ";
                }
                else
                {
                    makeSQL = " SELECT  PI.ITEM_NAME, count(PPM.SLIP_NO) AS SLIP_NO, sum(PPM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PPM.ITEM_AMOUNT) AS ITEM_AMOUNT, round(sum(PPM.ITEM_AMOUNT)/sum(PPM.ITEM_WEIGHT),2) AS ITEM_AVG_RATE FROM PF_ITEM PI LEFT JOIN PF_PURCHASE_MASTER PPM ON PPM.ITEM_ID = PI.ITEM_ID WHERE to_char(PPM.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear4.Text + "' GROUP BY PI.ITEM_ID, PI.ITEM_NAME ORDER BY PI.ITEM_ID ";

                //   makeSQL = " SELECT PPM.PURCHASE_ID, PPM.SLIP_NO, PPM.PARTY_ID, PP.PARTY_NAME, PPM.PUR_TYPE_ID, PPT.PUR_TYPE_NAME, PPM.ITEM_ID, PI.ITEM_NAME, PPM.SUB_ITEM_ID, PSI.SUB_ITEM_NAME, PPM.SUPERVISOR_ID, PS.SUPERVISOR_NAME, PPM.ITEM_WEIGHT, PPM.ITEM_RATE, PPM.ITEM_AMOUNT, PPM.ENTRY_DATE, PPM.CREATE_DATE, PPM.UPDATE_DATE, PPM.IS_ACTIVE , PPC.IS_CHECK FROM PF_PURCHASE_MASTER PPM LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PPM.PARTY_ID LEFT JOIN PF_PURCHASE_TYPE PPT ON PPT.PUR_TYPE_ID = PPM.PUR_TYPE_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPM.SUPERVISOR_ID  LEFT JOIN PF_PURCHASE_CLAIM PPC ON  PPC.CLAIM_NO = PPM.CLAIM_NO where PPM.SLIP_NO like '" + txtSearchEmp.Text + "%' or PPM.PARTY_ID like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PS.SUPERVISOR_NAME like '" + txtSearchEmp.Text + "%' or PPM.ITEM_RATE like '" + txtSearchEmp.Text + "%' or PPM.ENTRY_DATE like '" + txtSearchEmp.Text + "%' or PPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' or PPC.IS_CHECK like '" + txtSearchEmp.Text + "%' ORDER BY PPM.SLIP_NO asc";  // PPM.CREATE_DATE desc, PPM.UPDATE_DATE desc
                    
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
                    GridView2.HeaderRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                    //Calculate Sum and display in Footer Row
                    GridView2.FooterRow.Cells[0].Font.Bold = true;
                    GridView2.FooterRow.Cells[0].Text = "Grand Total";
                    GridView2.FooterRow.Cells[0].HorizontalAlign = HorizontalAlign.Right;

                    decimal total_prod = dt.AsEnumerable().Sum(row => row.Field<decimal>("SLIP_NO"));
                    GridView2.FooterRow.Cells[1].Font.Bold = true;
                    GridView2.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Center;
                    GridView2.FooterRow.Cells[1].Text = total_prod.ToString("N0");

                    decimal total_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_WEIGHT"));
                    GridView2.FooterRow.Cells[2].Font.Bold = true;
                    GridView2.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[2].Text = total_wt.ToString("N3");

                    decimal total_amt = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_AMOUNT"));
                    GridView2.FooterRow.Cells[3].Font.Bold = true;
                    GridView2.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[3].Text = total_amt.ToString("N2");

                    decimal total_avg = (total_amt / total_wt);
                    GridView2.FooterRow.Cells[4].Font.Bold = true;
                    GridView2.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[4].Text = total_avg.ToString("N2");
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
            GridView2.PageIndex = e.NewPageIndex;
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
                int PurchaseID = Convert.ToInt32(TextPurchaseID.Text);
                int SupervisorID = Convert.ToInt32(DropDownSupervisorID.Text);
                int ItemID = Convert.ToInt32(DropDownItemID.Text);
                int SubItemID = Convert.ToInt32(DropDownSubItemID.Text);
                int SupplierID = Convert.ToInt32(DropDownSupplierID.Text);
                int VatID = Convert.ToInt32(DropDownVatID.Text);
                double VatPercent = Convert.ToDouble(DropDownVatID.SelectedItem.Text);
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                string MakeEntryDate = EntryDate.Text;
                string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy"); 
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                 

                string makeSQL = " select  ITEM_ID, ITEM_WEIGHT  from PF_PURCHASE_MASTER where PURCHASE_ID  = '" + PurchaseID + "'";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                double ItemWeightOld = 0.00; int ItemIdOld = 0;
                    //inventory FG update reset 
                    int InvenItemID = 0; 
                    double InitialStock = 0.00, StockInWet = 0.00, StockInWetCurrent = 0.00, StockInWetNew = 0.00, StockOutWet = 0.00, StockOutWetDe = 0.00, FinalStock = 0.00, FinalStockNew = 0.00;
                    for (int i = 0; i < RowCount; i++)
                {
                    ItemWeightOld = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"]);
                    ItemIdOld = Convert.ToInt32(dt.Rows[i]["ITEM_ID"]);
                }
                 
                double ItemRate = Convert.ToDouble(TextItemRate.Text.Trim());
                double ItemWeight = Convert.ToDouble(TextItemWeight.Text.Trim());
                double ItemWeightWb = Convert.ToDouble(TextItemWtWb.Text.Trim());
                double ItemAmount = ItemRate * ItemWeight; 
                string ItemAmountNew = Math.Round(decimal.Parse(ItemAmount.ToString()), 2).ToString();
                double ItemAmountNewD = Convert.ToDouble(ItemAmountNew);

                double VatAmt = 0.00;
                if (RadioBtnVat.SelectedValue == "VatYes")
                {
                    VatID = Convert.ToInt32(DropDownVatID.Text);
                    VatPercent = Convert.ToDouble(DropDownVatID.SelectedItem.Text);
                    VatAmt = (ItemAmount * Convert.ToDouble(DropDownVatID.SelectedItem.Text)) / 100;
                }
                else { VatID = 0; VatPercent = 0; }


                    string makeSQLInvernRM = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                    cmdl = new OracleCommand(makeSQLInvernRM);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    for (int i = 0; i < RowCount; i++)
                    {
                        InvenItemID = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString()); 
                        InitialStock = Convert.ToDouble(dt.Rows[i]["INITIAL_STOCK_WT"].ToString());
                        StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                        StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString());
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    StockInWetCurrent = StockInWet - ItemWeightOld;
                    FinalStockNew = (InitialStock + StockInWetCurrent) - StockOutWet;

                    if (0 < RowCount)
                    {
                        string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetCurrent);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", ItemIdOld); 

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }

                    // inventory RM update insert
                    string makeSQLInvenRMUp = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeSQLInvenRMUp);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;


                    for (int i = 0; i < RowCount; i++)
                    {
                        InvenItemID = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                        InitialStock = Convert.ToDouble(dt.Rows[i]["INITIAL_STOCK_WT"].ToString());
                        StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                        StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString());
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    StockInWetNew = StockInWet + ItemWeight;
                    FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;

                    if (0 < RowCount)
                    {

                        string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID); 

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }

                    // purchase master update

                string update_user = "update  PF_PURCHASE_MASTER  set PARTY_ID =:NoSupplierID, SUPERVISOR_ID =:NoSupervisorID, ITEM_ID =:NoItemID, SUB_ITEM_ID =:NoSubItemID, ITEM_WEIGHT_WB =:NoItemWeightWb, ITEM_WEIGHT =:NoItemWeight , ITEM_RATE = :NoItemRate, ITEM_AMOUNT = :NoItemAmount, VAT_ID = :NoVatID, VAT_PERCENT = :NoVatPercent, VAT_AMOUNT = :NoVatAmt, ENTRY_DATE = TO_DATE(:EntryDate, 'DD/MM/YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where PURCHASE_ID = :NoPurchaseID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[16]; 
                objPrm[0] = cmdi.Parameters.Add("NoSupplierID", SupplierID);
                objPrm[1] = cmdi.Parameters.Add("NoSupervisorID", SupervisorID);
                objPrm[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                objPrm[3] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                objPrm[4] = cmdi.Parameters.Add("NoItemWeight", ItemWeight);
                objPrm[5] = cmdi.Parameters.Add("NoItemWeightWb", ItemWeightWb);
                objPrm[6] = cmdi.Parameters.Add("NoItemRate", ItemRate);
                objPrm[7] = cmdi.Parameters.Add("NoItemAmount", ItemAmountNewD);
                objPrm[8] = cmdi.Parameters.Add("NoVatID", VatID); 
                objPrm[9] = cmdi.Parameters.Add("NoVatPercent", VatPercent); 
                objPrm[10] = cmdi.Parameters.Add("NoVatAmt", VatAmt); 
                objPrm[11] = cmdi.Parameters.Add("EntryDate", EntryDateNew); 
                objPrm[12] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[13] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[14] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[15] = cmdi.Parameters.Add("NoPurchaseID", PurchaseID);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Purchase Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                 
                clearText();
                TextSlipNo.Focus(); 
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
                int PurchaseID = Convert.ToInt32(TextPurchaseID.Text);              
                int SlipNo = Convert.ToInt32(TextSlipNo.Text);
                // purchase check data
                int PurchaseTypeIdOld = 0, ItemIdOld=0, SubItemIdOld=0; double ItemWeightOld = 0.00;
                int InvenItemID = 0;
                int InvenSubItemID = 0;
                double InitialStock = 0.00, StockInWet = 0.00, StockInWetCurrent = 0.00, StockInWetNew = 0.00, StockOutWet = 0.00, FinalStock = 0.00, FinalStockNew = 0.00;
                string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string makeSQL = " select PUR_TYPE_ID, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT from PF_PURCHASE_MASTER where PURCHASE_ID  = '" + PurchaseID + "'  ";
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                 
                for (int i = 0; i < RowCount; i++)
                {
                    PurchaseTypeIdOld = Convert.ToInt32(dt.Rows[i]["PUR_TYPE_ID"].ToString());
                    ItemIdOld = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                    SubItemIdOld = Convert.ToInt32(dt.Rows[i]["SUB_ITEM_ID"].ToString()); 
                    ItemWeightOld = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString()); 
                }

                if (PurchaseTypeIdOld == 1)
                {
                    // inventory update FG delete purchase
                    string makeSQLInvenFGUp = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                    cmdl = new OracleCommand(makeSQLInvenFGUp);
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

                    StockInWetNew = StockInWet - ItemWeightOld;
                    FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;

                    if (0 < RowCount)
                    {
                        string update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID); 

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }

                    // Delete inventoey FG details 
                    string delete_inven_fg_mas_des = " delete from PF_FG_STOCK_INVENTORY_MAS_DE where REF_ID  = '" + TextSlipNo.Text + "'";

                    cmdi = new OracleCommand(delete_inven_fg_mas_des, conn);
                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                } else {

                    // inventory RM update delete purchases
                    string makeSQLInvenRMUp = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                    cmdl = new OracleCommand(makeSQLInvenRMUp);
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

                    StockInWetNew = StockInWet - ItemWeightOld;
                    FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;

                    if (0 < RowCount)
                    {

                        string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID); 

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }

                    // Delete inventoey RM details 
                    string delete_inven_rm_mas_des = " delete from PF_RM_STOCK_INVENTORY_MAS_DE where PURCHASE_ID  = '" + PurchaseID.ToString() + "'";

                    cmdi = new OracleCommand(delete_inven_rm_mas_des, conn);
                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                }

                string delete_user = " delete from PF_PURCHASE_MASTER where PURCHASE_ID  = '" + PurchaseID + "'"; 
                cmdi = new OracleCommand(delete_user, conn); 
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Purchase Data Delete successfully"));
                alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
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
            TextSlipNo.Text = ""; 
            TextItemRate.Text = "";
            TextItemWeight.Text = "";
            DropDownSubItemID.Text = "0";
            TextItemAmount.Text = "";
            TextTotalAmount.Text = "";
            TextItemVatAmount.Text = "";
            CheckSlipNo.Text = ""; 
            DropDownSupplierID.Text = "0";
            DropDownSupervisorID.Text = "0";
            DropDownPurchaseTypeID.Text = "2";
            DropDownItemID.Text = "0";
            TextItemWtWb.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        {
            TextSlipNo.Text = ""; 
            TextItemRate.Text = "";
            TextItemWeight.Text = "";
            DropDownSubItemID.Text = "0";
            TextItemAmount.Text = ""; 
            TextTotalAmount.Text = "";
            TextItemVatAmount.Text = "";
            CheckSlipNo.Text = ""; 
            DropDownSupplierID.Text = "0";
            DropDownSupervisorID.Text = "0";
            DropDownPurchaseTypeID.Text = "2";
            DropDownItemID.Text = "0";
            TextItemWtWb.Text = "";
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

        public void TextItemRate_Changed(object sender, EventArgs e)
        {

            if (TextItemRate.Text != "" && TextItemWeight.Text != "")
            {
            double ItemRate   = Convert.ToDouble(TextItemRate.Text);
            double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
            double ItemAmount = ItemRate * ItemWeight; 
            string ItemAmountNew = Math.Round(decimal.Parse(ItemAmount.ToString()), 0).ToString();
            double ItemAmountNewD = Convert.ToDouble(ItemAmountNew);
            TextItemAmount.Text = ItemAmountNewD.ToString("0.00");
            TextItemWeight.Focus();
            } 
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
        protected void BtnReport3_Click(object sender, EventArgs e)
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
        protected void BtnReport4_Click(object sender, EventArgs e)
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