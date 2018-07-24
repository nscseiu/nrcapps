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
    public partial class PfPurchase : System.Web.UI.Page
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
        {  TextSlipNo.Focus();
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
                     
                    // SetFocus(TextSlipNo); 

                    if (!IsPostBack)
                    {
                        DataTable dtSupplierID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeSupplierSQL = " SELECT SUPPLIER_ID, SUPPLIER_ID || ' - ' || SUPPLIER_NAME AS SUPPLIER_NAME_FULL FROM PF_SUPPLIER WHERE IS_ACTIVE = 'Enable' ORDER BY SUPPLIER_NAME ASC";
                        ds = ExecuteBySqlStringEmpType(makeSupplierSQL);
                        dtSupplierID = (DataTable)ds.Tables[0];
                        DropDownSupplierID.DataSource = dtSupplierID;
                        DropDownSupplierID.DataValueField = "SUPPLIER_ID";
                        DropDownSupplierID.DataTextField = "SUPPLIER_NAME_FULL";
                        DropDownSupplierID.DataBind();
                        DropDownSupplierID.Items.Insert(0, new ListItem("Select  Supplier", "0"));

                        DropDownSupplierID2.DataSource = dtSupplierID;
                        DropDownSupplierID2.DataValueField = "SUPPLIER_ID";
                        DropDownSupplierID2.DataTextField = "SUPPLIER_NAME_FULL";
                        DropDownSupplierID2.DataBind();
                        DropDownSupplierID2.Items.Insert(0, new ListItem("Select  Supplier", "0"));

                        DropDownSupplierID3.DataSource = dtSupplierID;
                        DropDownSupplierID3.DataValueField = "SUPPLIER_ID";
                        DropDownSupplierID3.DataTextField = "SUPPLIER_NAME_FULL";
                        DropDownSupplierID3.DataBind();
                        DropDownSupplierID3.Items.Insert(0, new ListItem("Select  Supplier", "0"));

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

                        DropDownSupervisorID2.DataSource = dtSupplierID;
                        DropDownSupervisorID2.DataValueField = "SUPERVISOR_ID";
                        DropDownSupervisorID2.DataTextField = "SUPERVISOR_NAME";
                        DropDownSupervisorID2.DataBind();
                        DropDownSupervisorID2.Items.Insert(0, new ListItem("Select  Supervisor", "0"));

                        DataTable dtPurchaseTypeID = new DataTable();
                        DataSet dsl = new DataSet();
                        string makePurchaseTypeSQL = " SELECT * FROM PF_PURCHASE_TYPE WHERE IS_ACTIVE = 'Enable' ORDER BY PUR_TYPE_ID DESC";
                        dsl = ExecuteBySqlStringEmpType(makePurchaseTypeSQL);
                        dtPurchaseTypeID = (DataTable)dsl.Tables[0];
                        DropDownPurchaseTypeID.DataSource = dtPurchaseTypeID;
                        DropDownPurchaseTypeID.DataValueField = "PUR_TYPE_ID";
                        DropDownPurchaseTypeID.DataTextField = "PUR_TYPE_NAME";
                        DropDownPurchaseTypeID.DataBind();
                      //  DropDownPurchaseTypeID.Items.Insert(0, new ListItem("Select  Purchase Type", "0"));

                        DropDownPurchaseTypeID2.DataSource = dtPurchaseTypeID;
                        DropDownPurchaseTypeID2.DataValueField = "PUR_TYPE_ID";
                        DropDownPurchaseTypeID2.DataTextField = "PUR_TYPE_NAME";
                        DropDownPurchaseTypeID2.DataBind();
                     //  DropDownPurchaseTypeID2.Items.Insert(0, new ListItem("Select  Purchase Type", "0"));


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

                    string get_user_purchase_id = "select PF_PURCHASE_MASTERID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_user_purchase_id, conn);
                    int newPurchaseID = Int16.Parse(cmdsp.ExecuteScalar().ToString());
                    double ItemRate = Convert.ToDouble(TextItemRate.Text); 
                    double ItemWeight = Convert.ToDouble(TextItemWeight.Text);  
                    double ItemAmount = ItemRate * ItemWeight;
                     
                    string ItemAmountNew = Math.Round(decimal.Parse(ItemAmount.ToString()), 0).ToString();
                    double ItemAmountNewD = Convert.ToDouble(ItemAmountNew);

                    string insert_purchase = "insert into  PF_PURCHASE_MASTER (PURCHASE_ID, SLIP_NO, SUPPLIER_ID, PUR_TYPE_ID, ITEM_ID, SUB_ITEM_ID, SUPERVISOR_ID, ITEM_WEIGHT, ITEM_RATE, ITEM_AMOUNT, ENTRY_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID) values  ( :NoPurchaseID, :NoSlipID, :NoSupplierID, :NoPurchaseTypeID, :NoItemID, :NoSubItemID, :NoSupervisorID, :TextItemWeight, :TextItemRate, :TextItemAmount, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 3)";
                    cmdi = new OracleCommand(insert_purchase, conn);
                      
                    OracleParameter[] objPrm = new OracleParameter[14];
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
                        FinalStockNew = InitialStock + StockInWetNew - StockOutWet;

                        if (0 < RowCount)
                        {

                            string update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID AND  SUB_ITEM_ID = :NoSubItemID ";
                            cmdu = new OracleCommand(update_inven_mas, conn);

                            OracleParameter[] objPrmInevenMas = new OracleParameter[6];
                            objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                            objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                            objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                            objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                            objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);
                            objPrmInevenMas[5] = cmdu.Parameters.Add("NoSubItemID", InvenSubItemID);

                            cmdu.ExecuteNonQuery();
                            cmdu.Parameters.Clear();
                            cmdu.Dispose();
                        }
                        else
                        {

                            FinalStockNew = InitialStock + ItemWeight - StockOutWet;

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

                        string makeSQL = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  AND SUB_ITEM_ID  = '" + SubItemID + "' ";
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
                        FinalStockNew = InitialStock + StockInWetNew - StockOutWet; 

                        if (0 < RowCount)
                        {

                            string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID AND  SUB_ITEM_ID = :NoSubItemID ";
                            cmdu = new OracleCommand(update_inven_mas, conn);

                            OracleParameter[] objPrmInevenMas = new OracleParameter[6];
                            objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                            objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                            objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                            objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                            objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);
                            objPrmInevenMas[5] = cmdu.Parameters.Add("NoSubItemID", InvenSubItemID);

                            cmdu.ExecuteNonQuery();
                            cmdu.Parameters.Clear();
                            cmdu.Dispose();
                        }
                        else {

                            FinalStockNew = InitialStock + ItemWeight - StockOutWet;

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

                        string InventoryFor = "Raw Material Received";
                        string get_inven_mas_des_id = "select PF_RM_STOCK_INVEN_MASDESID_SEQ.nextval from dual";
                        cmdsp = new OracleCommand(get_inven_mas_des_id, conn);
                        int newInvenMasDesRmID = Int16.Parse(cmdsp.ExecuteScalar().ToString());
                        string insert_inven_rm_des = "insert into  PF_RM_STOCK_INVENTORY_MAS_DE (RM_INVEN_DE_ID, REF_ID, ITEM_ID, ITEM_NAME, SUB_ITEM_ID, SUB_ITEM_NAME, INVENTORY_FOR, STOCK_IN_WT, STOCK_OUT_WT, CREATE_DATE, C_USER_ID) values ( :NoInvenMasRmID, :NoRefID, :NoItemID, :TextItemName, :NoSubItemID, :TextSubItemName, :TextInventoryFor, :NoStockIn, :NoStockOut, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                        cmdi = new OracleCommand(insert_inven_rm_des, conn);

                        OracleParameter[] objPrmIrmd = new OracleParameter[11];
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

                        cmdi.ExecuteNonQuery(); 
                        cmdi.Parameters.Clear();
                        cmdi.Dispose(); 
                    
                    }
                     
                    conn.Close(); 

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert new Purchase successfully"));
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
            string makeSQL = " select PURCHASE_ID, SLIP_NO,  SUPPLIER_ID, PUR_TYPE_ID, ITEM_ID, SUB_ITEM_ID, SUPERVISOR_ID, ITEM_WEIGHT, ITEM_RATE, ROUND(ITEM_AMOUNT,2) AS ITEM_AMOUNT,  TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, CREATE_DATE,  UPDATE_DATE,  C_USER_ID, U_USER_ID, IS_ACTIVE from PF_PURCHASE_MASTER where SLIP_NO  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                TextSlipNo.Text = dt.Rows[i]["SLIP_NO"].ToString();
                DropDownSupplierID.Text = dt.Rows[i]["SUPPLIER_ID"].ToString();
                DropDownSupervisorID.Text = dt.Rows[i]["SUPERVISOR_ID"].ToString();
                DropDownPurchaseTypeID.Text = dt.Rows[i]["PUR_TYPE_ID"].ToString();
                DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();
                DropDownSubItemID.Text = dt.Rows[i]["SUB_ITEM_ID"].ToString();
                TextItemRate.Text = dt.Rows[i]["ITEM_RATE"].ToString(); 
                TextItemWeight.Text  = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT"].ToString()).ToString(".000");
                TextItemAmount.Text = decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00"); 
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
                    makeSQL = " SELECT PPM.PURCHASE_ID, PPM.SLIP_NO, PPM.SUPPLIER_ID, PC.SUPPLIER_NAME, PPM.PUR_TYPE_ID, PPT.PUR_TYPE_NAME, PPM.ITEM_ID, PI.ITEM_NAME, PPM.SUB_ITEM_ID, PSI.SUB_ITEM_NAME, PPM.SUPERVISOR_ID, PS.SUPERVISOR_NAME, PPM.ITEM_WEIGHT, PPM.ITEM_RATE, PPM.ITEM_AMOUNT, PPM.ENTRY_DATE, PPM.CREATE_DATE, PPM.UPDATE_DATE, PPM.IS_ACTIVE , PPC.IS_CHECK FROM PF_PURCHASE_MASTER PPM LEFT JOIN PF_SUPPLIER PC ON PC.SUPPLIER_ID = PPM.SUPPLIER_ID LEFT JOIN PF_PURCHASE_TYPE PPT ON PPT.PUR_TYPE_ID = PPM.PUR_TYPE_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPM.SUPERVISOR_ID  LEFT JOIN PF_PURCHASE_CLAIM PPC ON  PPC.CLAIM_NO = PPM.CLAIM_NO ORDER BY PPM.CREATE_DATE DESC ";
                }
                else
                {
                    makeSQL = "  SELECT PPM.PURCHASE_ID, PPM.SLIP_NO, PPM.SUPPLIER_ID, PC.SUPPLIER_NAME, PPM.PUR_TYPE_ID, PPT.PUR_TYPE_NAME, PPM.ITEM_ID, PI.ITEM_NAME, PPM.SUB_ITEM_ID, PSI.SUB_ITEM_NAME, PPM.SUPERVISOR_ID, PS.SUPERVISOR_NAME, PPM.ITEM_WEIGHT, PPM.ITEM_RATE, PPM.ITEM_AMOUNT, PPM.ENTRY_DATE, PPM.CREATE_DATE, PPM.UPDATE_DATE, PPM.IS_ACTIVE , PPC.IS_CHECK FROM PF_PURCHASE_MASTER PPM LEFT JOIN PF_SUPPLIER PC ON PC.SUPPLIER_ID = PPM.SUPPLIER_ID LEFT JOIN PF_PURCHASE_TYPE PPT ON PPT.PUR_TYPE_ID = PPM.PUR_TYPE_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPM.SUPERVISOR_ID  LEFT JOIN PF_PURCHASE_CLAIM PPC ON  PPC.CLAIM_NO = PPM.CLAIM_NO where PPM.SLIP_NO like '" + txtSearchEmp.Text + "%' or PPM.SUPPLIER_ID like '" + txtSearchEmp.Text + "%' or PC.SUPPLIER_NAME like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PS.SUPERVISOR_NAME like '" + txtSearchEmp.Text + "%' or PPM.ITEM_RATE like '" + txtSearchEmp.Text + "%' or PPM.ENTRY_DATE like '" + txtSearchEmp.Text + "%' or PPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' or PPC.IS_CHECK like '" + txtSearchEmp.Text + "%' ORDER BY PPM.CREATE_DATE desc, PPM.UPDATE_DATE desc";
                    alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "PURCHASE_ID" };
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
                int PurchaseTypeID = Convert.ToInt32(DropDownPurchaseTypeID.Text);
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

                string get_user_purchase_id = "select PF_PURCHASE_MASTERID_SEQ.nextval from dual";
                cmdsp = new OracleCommand(get_user_purchase_id, conn);
                int newPurchaseID = Int16.Parse(cmdsp.ExecuteScalar().ToString());
                double ItemRate = Convert.ToDouble(TextItemRate.Text);
                double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
                double ItemAmount = ItemRate * ItemWeight;

                string ItemAmountNew = Math.Round(decimal.Parse(ItemAmount.ToString()), 0).ToString();
                double ItemAmountNewD = Convert.ToDouble(ItemAmountNew); 

                string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");  
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                // purchase check data
                int PurchaseTypeIdOld = 0, ItemIdOld=0, SubItemIdOld=0; double ItemWeightOld = 0.00;
                string makeSQL = " select PUR_TYPE_ID, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT from PF_PURCHASE_MASTER where SLIP_NO  = '" + SlipNo + "'  ";
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
                 
                //inventory FG update reset 
                int InvenItemID = 0;
                int InvenSubItemID = 0;
                double InitialStock = 0.00, StockInWet = 0.00, StockInWetCurrent = 0.00, StockInWetNew = 0.00, StockOutWet = 0.00, StockOutWetDe = 0.00, FinalStock = 0.00,FinalStockNew = 0.00;

                if (PurchaseTypeIdOld == 1)
                { 
                    string makeSQLInvernFG = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  AND SUB_ITEM_ID  = '" + SubItemIdOld + "' ";
                    cmdl = new OracleCommand(makeSQLInvernFG);
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

                    StockInWetCurrent = StockInWet - ItemWeightOld;
                    FinalStockNew     = InitialStock + StockInWetCurrent - StockOutWet;

                    if (0 < RowCount)
                    { 
                        string update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID AND  SUB_ITEM_ID = :NoSubItemID ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[6];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetCurrent);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", ItemIdOld);
                        objPrmInevenMas[5] = cmdu.Parameters.Add("NoSubItemID", SubItemIdOld);

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    } 
                }
                else
                {  //inventory RM update reset 
                    string makeSQLInvernRM = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  AND SUB_ITEM_ID  = '" + SubItemIdOld + "' ";
                    cmdl = new OracleCommand(makeSQLInvernRM);
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

                    StockInWetCurrent = StockInWet - ItemWeightOld;
                    FinalStockNew = InitialStock + StockInWetCurrent - StockOutWet;

                    if (0 < RowCount)
                    { 
                        string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID AND  SUB_ITEM_ID = :NoSubItemID ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[6];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetCurrent);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", ItemIdOld);
                        objPrmInevenMas[5] = cmdu.Parameters.Add("NoSubItemID", SubItemIdOld);

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    } 
                }


                if (PurchaseTypeID == 1)
                { 
                    // inventory update FG insert
                    string makeSQLInvenFGUp = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  AND SUB_ITEM_ID  = '" + SubItemID + "' ";
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

                    StockInWetNew = StockInWet + ItemWeight;
                    FinalStockNew = InitialStock + StockInWetNew - StockOutWet;

                    if (0 < RowCount)
                    { 
                        string update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID AND  SUB_ITEM_ID = :NoSubItemID ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[6];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);
                        objPrmInevenMas[5] = cmdu.Parameters.Add("NoSubItemID", InvenSubItemID);

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }
                    else
                    {

                        FinalStockNew = InitialStock + ItemWeight - StockOutWet;

                        string get_inven_mas_id = "select PF_FG_STOCK_INVEN_MASID_SEQ.nextval from dual";
                        cmdsp = new OracleCommand(get_inven_mas_id, conn);
                        int newInvenMasFgID = Int16.Parse(cmdsp.ExecuteScalar().ToString());

                        string insert_inven_rm = "insert into  PF_FG_STOCK_INVENTORY_MASTER (FG_INVENTORY_ID, ITEM_ID, ITEM_NAME, SUB_ITEM_ID, SUB_ITEM_NAME, INITIAL_STOCK_WT, STOCK_IN_WT, STOCK_OUT_WT, FINAL_STOCK_WT, CREATE_DATE, C_USER_ID) values ( :NoInvenMasFgID, :NoItemID, :TextItemName, :NoSubItemID, :TextSubItemName, :NoInitialStock, :NoStockIn, :NoStockOut, :NoFinalStock, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                        cmdi = new OracleCommand(insert_inven_rm, conn);

                        OracleParameter[] objPrmIrm = new OracleParameter[11];
                        objPrmIrm[0] = cmdi.Parameters.Add("NoInvenMasFgID", newInvenMasFgID);
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
                }
                else { 
                    // inventory RM update insert
                    string makeSQLInvenRMUp = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  AND SUB_ITEM_ID  = '" + SubItemID + "' ";
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

                    StockInWetNew = StockInWet + ItemWeight;
                    FinalStockNew = InitialStock + StockInWetNew - StockOutWet;

                    if (0 < RowCount)
                    {

                        string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID AND  SUB_ITEM_ID = :NoSubItemID ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[6];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);
                        objPrmInevenMas[5] = cmdu.Parameters.Add("NoSubItemID", InvenSubItemID);

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }
                    else
                    {

                        FinalStockNew = InitialStock + ItemWeight - StockOutWet;

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
                }

                if(PurchaseTypeIdOld == PurchaseTypeID){

                    if (PurchaseTypeID == 1)
                    {
                        // Update inventoey FG details
                        string insert_inven_fg_des = "update  PF_FG_STOCK_INVENTORY_MAS_DE  set ITEM_ID = :NoItemID, ITEM_NAME = :TextItemName, SUB_ITEM_ID = :NoSubItemID, SUB_ITEM_NAME = :TextSubItemName, STOCK_IN_WT = :NoStockIn, UPDATE_DATE = TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where REF_ID = :NoRefID ";
                        cmdi = new OracleCommand(insert_inven_fg_des, conn);

                        OracleParameter[] objPrmIrmd = new OracleParameter[9];
                        objPrmIrmd[1] = cmdi.Parameters.Add("NoItemID", ItemID);
                        objPrmIrmd[2] = cmdi.Parameters.Add("TextItemName", ItemName);
                        objPrmIrmd[3] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                        objPrmIrmd[4] = cmdi.Parameters.Add("TextSubItemName", SubItemName);
                        objPrmIrmd[5] = cmdi.Parameters.Add("NoStockIn", ItemWeight);
                        objPrmIrmd[6] = cmdi.Parameters.Add("c_date", c_date);
                        objPrmIrmd[7] = cmdi.Parameters.Add("NoCuserID", userID);
                        objPrmIrmd[8] = cmdi.Parameters.Add("NoRefID", TextSlipNo.Text);
                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();


                    }
                    else {
                        // Update inventoey RM details
                        string insert_inven_fg_des = "update  PF_RM_STOCK_INVENTORY_MAS_DE  set ITEM_ID = :NoItemID, ITEM_NAME = :TextItemName, SUB_ITEM_ID = :NoSubItemID, SUB_ITEM_NAME = :TextSubItemName, STOCK_IN_WT = :NoStockIn, UPDATE_DATE = TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where REF_ID = :NoRefID ";
                        cmdi = new OracleCommand(insert_inven_fg_des, conn);

                        OracleParameter[] objPrmIrmd = new OracleParameter[9];
                        objPrmIrmd[1] = cmdi.Parameters.Add("NoItemID", ItemID);
                        objPrmIrmd[2] = cmdi.Parameters.Add("TextItemName", ItemName);
                        objPrmIrmd[3] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                        objPrmIrmd[4] = cmdi.Parameters.Add("TextSubItemName", SubItemName);
                        objPrmIrmd[5] = cmdi.Parameters.Add("NoStockIn", ItemWeight);
                        objPrmIrmd[6] = cmdi.Parameters.Add("c_date", c_date);
                        objPrmIrmd[7] = cmdi.Parameters.Add("NoCuserID", userID);
                        objPrmIrmd[8] = cmdi.Parameters.Add("NoRefID", TextSlipNo.Text);
                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();
                    
                    }

                } else {
                    if (PurchaseTypeIdOld == 1)
                    {
                        // Delete inventoey FG details 
                        string delete_user = " delete from PF_FG_STOCK_INVENTORY_MAS_DE where REF_ID  = '" + TextSlipNo.Text + "'";

                        cmdi = new OracleCommand(delete_user, conn); 
                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();


                        // insert inventory RM details
                        string InventoryFor = "Raw Material Received";
                        string get_inven_mas_des_id = "select PF_RM_STOCK_INVEN_MASDESID_SEQ.nextval from dual";
                        cmdsp = new OracleCommand(get_inven_mas_des_id, conn);
                        int newInvenMasDesRmID = Int16.Parse(cmdsp.ExecuteScalar().ToString());
                        string insert_inven_rm_des = "insert into  PF_RM_STOCK_INVENTORY_MAS_DE (RM_INVEN_DE_ID, REF_ID, ITEM_ID, ITEM_NAME, SUB_ITEM_ID, SUB_ITEM_NAME, INVENTORY_FOR, STOCK_IN_WT, STOCK_OUT_WT, CREATE_DATE, C_USER_ID) values ( :NoInvenMasRmID, :NoRefID, :NoItemID, :TextItemName, :NoSubItemID, :TextSubItemName, :TextInventoryFor, :NoStockIn, :NoStockOut, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                        cmdi = new OracleCommand(insert_inven_rm_des, conn);

                        OracleParameter[] objPrmIrmd = new OracleParameter[11];
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

                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose(); 
                    }
                    else
                    {
                        // Delete inventoey RM details 
                        string delete_user = " delete from PF_RM_STOCK_INVENTORY_MAS_DE where REF_ID  = '" + TextSlipNo.Text + "'";

                        cmdi = new OracleCommand(delete_user, conn); 
                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();

                        // insert inventory FG details
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

                }

                // purchase master update

                string update_user = "update  PF_PURCHASE_MASTER  set SUPPLIER_ID = :NoSupplierID, PUR_TYPE_ID = :NoPurchaseTypeID, ITEM_ID = :NoItemID, SUB_ITEM_ID = :NoSubItemID, SUPERVISOR_ID = :NoSupervisorID, ITEM_WEIGHT = :NoItemWeight, ITEM_RATE = :NoItemRate, ITEM_AMOUNT = :NoItemAmount, ENTRY_DATE = TO_DATE(:EntryDate, 'DD/MM/YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where SLIP_NO = :NoSlipNo ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[14];
                objPrm[1] = cmdi.Parameters.Add("NoSupplierID", SupplierID);               
                objPrm[2] = cmdi.Parameters.Add("NoPurchaseTypeID", PurchaseTypeID);
                objPrm[3] = cmdi.Parameters.Add("NoItemID", ItemID);
                objPrm[4] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                objPrm[5] = cmdi.Parameters.Add("NoSupervisorID", SupervisorID); 
                objPrm[6] = cmdi.Parameters.Add("NoItemWeight", ItemWeight);
                objPrm[7] = cmdi.Parameters.Add("NoItemRate", ItemRate);
                objPrm[8] = cmdi.Parameters.Add("NoItemAmount", ItemAmountNewD);
                objPrm[9] = cmdi.Parameters.Add("EntryDate", EntryDateNew); 
                objPrm[10] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[11] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[12] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[13] = cmdi.Parameters.Add("NoSlipNo", SlipNo);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Purchase Data Update successfully"));
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
                // purchase check data
                int PurchaseTypeIdOld = 0, ItemIdOld=0, SubItemIdOld=0; double ItemWeightOld = 0.00;
                int InvenItemID = 0;
                int InvenSubItemID = 0;
                double InitialStock = 0.00, StockInWet = 0.00, StockInWetCurrent = 0.00, StockInWetNew = 0.00, StockOutWet = 0.00, FinalStock = 0.00, FinalStockNew = 0.00;
                string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");  

                string makeSQL = " select PUR_TYPE_ID, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT from PF_PURCHASE_MASTER where SLIP_NO  = '" + SlipNo + "'  ";
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
                    string makeSQLInvenFGUp = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  AND SUB_ITEM_ID  = '" + SubItemIdOld + "' ";
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
                    FinalStockNew = InitialStock + StockInWetNew - StockOutWet;

                    if (0 < RowCount)
                    {
                        string update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID AND  SUB_ITEM_ID = :NoSubItemID ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[6];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);
                        objPrmInevenMas[5] = cmdu.Parameters.Add("NoSubItemID", InvenSubItemID);

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
                    string makeSQLInvenRMUp = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  AND SUB_ITEM_ID  = '" + SubItemIdOld + "' ";
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
                    FinalStockNew = InitialStock + StockInWetNew - StockOutWet;

                    if (0 < RowCount)
                    {

                        string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID AND  SUB_ITEM_ID = :NoSubItemID ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[6];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);
                        objPrmInevenMas[5] = cmdu.Parameters.Add("NoSubItemID", InvenSubItemID);

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }

                    // Delete inventoey RM details 
                    string delete_inven_rm_mas_des = " delete from PF_RM_STOCK_INVENTORY_MAS_DE where REF_ID  = '" + TextSlipNo.Text + "'";

                    cmdi = new OracleCommand(delete_inven_rm_mas_des, conn);
                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                }
                 
                string delete_user = " delete from PF_PURCHASE_MASTER where SLIP_NO  = '" + SlipNo + "'"; 
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
            CheckSlipNo.Text = ""; 
            DropDownSupplierID.Text = "0";
            DropDownSupervisorID.Text = "0";
            DropDownPurchaseTypeID.Text = "2";
            DropDownItemID.Text = "0";
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
            CheckSlipNo.Text = ""; 
            DropDownSupplierID.Text = "0";
            DropDownSupervisorID.Text = "0";
            DropDownPurchaseTypeID.Text = "2";
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
                    cmd.CommandText = "select SLIP_NO from PF_PURCHASE_MASTER where SLIP_NO = '" + Convert.ToInt32(SlipNo) + "'";
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

                        if (TextItemRate.Text != "" && TextItemWeight.Text != "")
                        {
                            double ItemRate = Convert.ToDouble(TextItemRate.Text);
                            double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
                            double ItemAmount = ItemRate * ItemWeight;
                            string ItemAmountNew = Math.Round(decimal.Parse(ItemAmount.ToString()), 0).ToString();
                            double ItemAmountNewD = Convert.ToDouble(ItemAmountNew);
                            TextItemAmount.Text = ItemAmountNewD.ToString("0.00");
                        } 

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