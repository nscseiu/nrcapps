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
    public partial class PfProduction : System.Web.UI.Page
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
                        DropDownSubItemID.Items.FindByValue("1").Selected = true;
                        //   DropDownSubItemID.Items.Insert(0, new ListItem("Select Sub Item", "0"));

                        DataTable dtPgeID = new DataTable();
                        DataSet dsp = new DataSet();
                        string makeDropDownPgeSQL = " SELECT * FROM PF_PRODUCTION_GARBAGE_EST WHERE IS_ACTIVE = 'Enable' ORDER BY PGE_ID ASC";
                        dsp = ExecuteBySqlString(makeDropDownPgeSQL);
                        dtPgeID = (DataTable)dsp.Tables[0];
                        DropDownPgeID.DataSource = dtPgeID;
                        DropDownPgeID.DataValueField = "PGE_ID";
                        DropDownPgeID.DataTextField = "PGE_PERCENT";
                        DropDownPgeID.DataBind();
                        DropDownPgeID.Items.Insert(0, new ListItem("Select Garbage Est. of Prod.", "0"));

                        TextPgeWet.Enabled = false;
                        Display();
                        DisplaySummary();
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
                    int SupervisorID   = Convert.ToInt32(DropDownSupervisorID.Text);
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
                     
                    int PageID = Convert.ToInt32(DropDownPgeID.Text);
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                     
                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                     
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string get_user_production_id = "select PF_PRODUCTION_MASTERID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_user_production_id, conn);
                    int newProductionID = Int16.Parse(cmdu.ExecuteScalar().ToString()); 
                    double PgePercent = Convert.ToDouble(DropDownPgeID.SelectedItem.Text);
                    double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
                    double ItemPgeWet = (ItemWeight * PgePercent) / 100;
                    double ItemWeightInFg = ItemWeight - ItemPgeWet;

                    string insert_production = "insert into  PF_PRODUCTION_MASTER (PRODUCTION_ID, SHIFT_ID, MACHINE_ID, SUPERVISOR_ID, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, ITEM_WEIGHT_IN_FG, PGE_ID, PGE_PERCENT, PGE_WEIGHT, ENTRY_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID ) values ( 'PD' || LPAD(:NoProductionID, 6, '0'), :NoShiftID, :NoMachineID, :NoSupervisorID, :NoItemID, :NoSubItemID,  :TextItemWeight, :TextItemWeightInFg, :NoPgeID, :NoPgePercent, :NoPgeWeight, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 3)";
                    cmdi = new OracleCommand(insert_production, conn);
                      
                    OracleParameter[] objPrm = new OracleParameter[15];
                    objPrm[0] = cmdi.Parameters.Add("NoProductionID", newProductionID);
                    objPrm[1] = cmdi.Parameters.Add("NoShiftID", ShiftID);  
                    objPrm[2] = cmdi.Parameters.Add("NoMachineID", MachineID);
                    objPrm[3] = cmdi.Parameters.Add("NoSupervisorID", SupervisorID);
                    objPrm[4] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[5] = cmdi.Parameters.Add("NoSubItemID", SubItemID);  
                    objPrm[6] = cmdi.Parameters.Add("TextItemWeight", ItemWeight); 
                    objPrm[7] = cmdi.Parameters.Add("TextItemWeightInFg", ItemWeightInFg); 
                    objPrm[8] = cmdi.Parameters.Add("NoPgeID", PageID);
                    objPrm[9] = cmdi.Parameters.Add("NoPgePercent", PgePercent);
                    objPrm[10] = cmdi.Parameters.Add("NoPgeWeight", ItemPgeWet);
                    objPrm[11] = cmdi.Parameters.Add("EntryDate", EntryDateNew); 
                    objPrm[12] = cmdi.Parameters.Add("c_date", c_date); 
                    objPrm[13] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[14] = cmdi.Parameters.Add("TextIsActive", ISActive);

                    cmdi.ExecuteNonQuery();

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();


                    //inventory calculation

                    int InvenItemID = 0;
                    int InvenSubItemID = 0;
                    double InitialStock = 0.00, StockInWet = 0.00, StockInWetDe = 0.00, StockOutWet = 0.00, StockOutWetDe = 0.00, FinalStock = 0.00, StockInWetNew = 0.00, StockOutWetNew = 0.00, FinalStockNew = 0.00;

                    // check inventory FG
                    string makeSQL = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
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
                        // update inventory FG
                        string update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
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
                        // insert inventory FG
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
                        objPrmIrm[6] = cmdi.Parameters.Add("NoStockIn", ItemWeightInFg);
                        objPrmIrm[7] = cmdi.Parameters.Add("NoStockOut", StockOutWet);
                        objPrmIrm[8] = cmdi.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmIrm[9] = cmdi.Parameters.Add("c_date", c_date);
                        objPrmIrm[10] = cmdi.Parameters.Add("NoCuserID", userID);

                        cmdi.ExecuteNonQuery();

                        cmdi.Parameters.Clear();
                        cmdi.Dispose();

                    }
                    // insert inventory FG details
                    string InventoryFor = "Finished Goods Produced";
                    string get_inven_mas_des_id = "select PF_FG_STOCK_INVEN_MASDESID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_inven_mas_des_id, conn);
                    int newInvenMasDesFgID = Int16.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_inven_fg_des = "insert into  PF_FG_STOCK_INVENTORY_MAS_DE (FG_INVEN_DE_ID, REF_ID, ITEM_ID, ITEM_NAME, SUB_ITEM_ID, SUB_ITEM_NAME, INVENTORY_FOR, STOCK_IN_WT, STOCK_OUT_WT, CREATE_DATE, C_USER_ID) values ( :NoInvenMasFgID, 'PD' || LPAD(:NoRefID, 6, '0'), :NoItemID, :TextItemName, :NoSubItemID, :TextSubItemName, :TextInventoryFor, :NoStockIn, :NoStockOut, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_inven_fg_des, conn);

                    OracleParameter[] objPrmIrmd = new OracleParameter[11];
                    objPrmIrmd[0] = cmdi.Parameters.Add("NoInvenMasFgID", newInvenMasDesFgID);
                    objPrmIrmd[1] = cmdi.Parameters.Add("NoRefID", newProductionID);
                    objPrmIrmd[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrmIrmd[3] = cmdi.Parameters.Add("TextItemName", ItemName);
                    objPrmIrmd[4] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                    objPrmIrmd[5] = cmdi.Parameters.Add("TextSubItemName", SubItemName);
                    objPrmIrmd[6] = cmdi.Parameters.Add("TextInventoryFor", InventoryFor);
                    objPrmIrmd[7] = cmdi.Parameters.Add("NoStockIn", ItemWeightInFg);
                    objPrmIrmd[8] = cmdi.Parameters.Add("NoStockOut", StockOutWetDe);
                    objPrmIrmd[9] = cmdi.Parameters.Add("c_date", c_date);
                    objPrmIrmd[10] = cmdi.Parameters.Add("NoCuserID", userID);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    // check inventory RM master
                    string makeRmInSQL = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "' ";
                    cmdl = new OracleCommand(makeRmInSQL);
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

                    StockOutWetNew = StockOutWet + ItemWeight + ItemPgeWet;
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

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }
                    else
                    {

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
                        objPrmIrm[6] = cmdi.Parameters.Add("NoStockIn", StockInWet);
                        objPrmIrm[7] = cmdi.Parameters.Add("NoStockOut", StockOutWetNew);
                        objPrmIrm[8] = cmdi.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmIrm[9] = cmdi.Parameters.Add("c_date", c_date);
                        objPrmIrm[10] = cmdi.Parameters.Add("NoCuserID", userID);

                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();
                    }

                    // insert Inventory RM details
                    string InventoryRmFor = "Raw Material Issued for Production";
                    string get_inven_mas_rmdes_id = "select PF_RM_STOCK_INVEN_MASDESID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_inven_mas_rmdes_id, conn);
                    int newInvenMasDesRmID = Int16.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_inven_rm_des = "insert into  PF_RM_STOCK_INVENTORY_MAS_DE (RM_INVEN_DE_ID, REF_ID, ITEM_ID, ITEM_NAME, SUB_ITEM_ID, SUB_ITEM_NAME, INVENTORY_FOR, STOCK_IN_WT, STOCK_OUT_WT, CREATE_DATE, C_USER_ID) values ( :NoInvenMasRmID, 'PD' || LPAD(:NoRefID, 6, '0'), :NoItemID, :TextItemName, :NoSubItemID, :TextSubItemName, :TextInventoryFor, :NoStockIn, :NoStockOut, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_inven_rm_des, conn);

                    OracleParameter[] objPrmIrmds = new OracleParameter[11];
                    objPrmIrmds[0] = cmdi.Parameters.Add("NoInvenMasRmID", newInvenMasDesRmID);
                    objPrmIrmds[1] = cmdi.Parameters.Add("NoRefID", newProductionID);
                    objPrmIrmds[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrmIrmds[3] = cmdi.Parameters.Add("TextItemName", ItemName);
                    objPrmIrmds[4] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                    objPrmIrmds[5] = cmdi.Parameters.Add("TextSubItemName", SubItemName);
                    objPrmIrmds[6] = cmdi.Parameters.Add("TextInventoryFor", InventoryRmFor);
                    objPrmIrmds[7] = cmdi.Parameters.Add("NoStockIn", StockInWetDe);
                    objPrmIrmds[8] = cmdi.Parameters.Add("NoStockOut", ItemWeight);
                    objPrmIrmds[9] = cmdi.Parameters.Add("c_date", c_date);
                    objPrmIrmds[10] = cmdi.Parameters.Add("NoCuserID", userID);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose(); 


                    

                    // insert Garbage  
                    string get_garbage_id = "select PF_GARBAGEID_SEQ.nextval from dual";
                    cmdsg = new OracleCommand(get_garbage_id, conn);
                    int newGarbageID = Int16.Parse(cmdsg.ExecuteScalar().ToString());
                    string insert_garbage = "insert into  PF_GARBAGE (GARBAGE_ID, PRODUCTION_ID, ITEM_ID, ITEM_NAME, SUB_ITEM_ID, SUB_ITEM_NAME, PGE_ID, PGE_PERCENT, PGE_WEIGHT, ENTRY_DATE, CREATE_DATE, C_USER_ID) values ( :NoGarbageID, 'PD' || LPAD(:NoProductionID, 6, '0'), :NoItemID, :TextItemName, :NoSubItemID, :TextSubItemName, :NoPgeID, :NoPgePercent, :NoPgeWeight, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_garbage, conn);

                    OracleParameter[] objPrmGr = new OracleParameter[12];
                    objPrmGr[0] = cmdi.Parameters.Add("NoGarbageID", newGarbageID);
                    objPrmGr[1] = cmdi.Parameters.Add("NoProductionID", newProductionID);
                    objPrmGr[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrmGr[3] = cmdi.Parameters.Add("TextItemName", ItemName);
                    objPrmGr[4] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                    objPrmGr[5] = cmdi.Parameters.Add("TextSubItemName", SubItemName);
                    objPrmGr[6] = cmdi.Parameters.Add("NoPgeID", PageID);
                    objPrmGr[7] = cmdi.Parameters.Add("NoPgePercent", PgePercent);
                    objPrmGr[8] = cmdi.Parameters.Add("NoPgeWeight", ItemPgeWet);
                    objPrmGr[9] = cmdi.Parameters.Add("EntryDate", EntryDateNew); 
                    objPrmGr[10] = cmdi.Parameters.Add("c_date", c_date);
                    objPrmGr[11] = cmdi.Parameters.Add("NoCuserID", userID);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose(); 

                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert new Production successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                     
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
            string makeSQL = " SELECT PRODUCTION_ID, SHIFT_ID, MACHINE_ID, SUPERVISOR_ID, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, ITEM_WEIGHT_IN_FG, PGE_ID, PGE_PERCENT, PGE_WEIGHT, TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, CREATE_DATE, UPDATE_DATE, IS_ACTIVE  from PF_PRODUCTION_MASTER where PRODUCTION_ID  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                TextProductionID.Text     = dt.Rows[i]["PRODUCTION_ID"].ToString(); 
                DropDownShiftID.Text      = dt.Rows[i]["SHIFT_ID"].ToString(); 
                DropDownMachineID.Text    = dt.Rows[i]["MACHINE_ID"].ToString();
                DropDownSupervisorID.Text = dt.Rows[i]["SUPERVISOR_ID"].ToString();
                DropDownItemID.Text       = dt.Rows[i]["ITEM_ID"].ToString(); 
                DropDownSubItemID.Text    = dt.Rows[i]["SUB_ITEM_ID"].ToString();
                TextItemWeight.Text       = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT"].ToString()).ToString("0.000"); 
                DropDownPgeID.Text        = dt.Rows[i]["PGE_ID"].ToString();
                TextPgeWet.Text           = decimal.Parse(dt.Rows[i]["PGE_WEIGHT"].ToString()).ToString("0.000"); 
                EntryDate.Text            = dt.Rows[i]["ENTRY_DATE"].ToString();

                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false); 
            }

            conn.Close();
            Display();
            CheckItemWeight.Text = "";
            alert_box.Visible = false;
        //    DropDownItemID.Enabled = false;
        //    DropDownSubItemID.Enabled = false;
       //     TextItemWeight.Enabled = false;
        //    DropDownPgeID.Enabled = false;
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
                    makeSQL = " SELECT PPRM.PRODUCTION_ID, PPS.SHIFT_NAME, PPM.MACHINE_NUMBER,  SUBSTR(PPRM.ENTRY_DATE,0,2) || '-' || PPS.SHIFT_NAME || '-' || PPM.MACHINE_NUMBER  AS SHIFT_MACHINE, PS.SUPERVISOR_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PPRM.ITEM_WEIGHT, PPRM.ITEM_WEIGHT_IN_FG, PPRM.PGE_PERCENT, PPRM.PGE_WEIGHT, PPRM.ENTRY_DATE, PPRM.CREATE_DATE, PPRM.UPDATE_DATE, PPRM.IS_ACTIVE FROM PF_PRODUCTION_MASTER PPRM LEFT JOIN PF_PRODUCTION_SHIFT PPS ON PPS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_PRODUCTION_MACHINE PPM ON PPM.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID WHERE to_char(PPRM.ENTRY_DATE, 'mm/yyyy') = '" + MonthYear + "' ORDER BY PPRM.CREATE_DATE DESC";

                    if (TextMonthYear3.Text != "") {

                      //  makeSQL = " SELECT PPRM.PRODUCTION_ID, PPS.SHIFT_NAME, PPM.MACHINE_NUMBER,  SUBSTR(PPRM.ENTRY_DATE,0,2) || '-' || PPS.SHIFT_NAME || '-' || PPM.MACHINE_NUMBER  AS SHIFT_MACHINE, PS.SUPERVISOR_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PPRM.ITEM_WEIGHT, PPRM.ITEM_WEIGHT_IN_FG, PPRM.PGE_PERCENT, PPRM.PGE_WEIGHT, PPRM.ENTRY_DATE, PPRM.CREATE_DATE, PPRM.UPDATE_DATE, PPRM.IS_ACTIVE FROM PF_PRODUCTION_MASTER PPRM LEFT JOIN PF_PRODUCTION_SHIFT PPS ON PPS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_PRODUCTION_MACHINE PPM ON PPM.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID WHERE to_char(PPRM.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear3.Text + "' ORDER BY PPRM.CREATE_DATE DESC";

                        if (DropDownItemID1.Text == "0")
                        {
                            makeSQL = " SELECT PPRM.PRODUCTION_ID, PPS.SHIFT_NAME, PPM.MACHINE_NUMBER, SUBSTR(PPRM.ENTRY_DATE,0,2) || '-' || PPS.SHIFT_NAME || '-' || PPM.MACHINE_NUMBER  AS SHIFT_MACHINE, PS.SUPERVISOR_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PPRM.ITEM_WEIGHT, PPRM.ITEM_WEIGHT_IN_FG, PPRM.PGE_PERCENT, PPRM.PGE_WEIGHT, PPRM.ENTRY_DATE, PPRM.CREATE_DATE, PPRM.UPDATE_DATE, PPRM.IS_ACTIVE FROM PF_PRODUCTION_MASTER PPRM LEFT JOIN PF_PRODUCTION_SHIFT PPS ON PPS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_PRODUCTION_MACHINE PPM ON PPM.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID where to_char(PPRM.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear3.Text + "' AND (PPRM.PRODUCTION_ID like '" + txtSearchEmp.Text + "%' or PPS.SHIFT_NAME like '" + txtSearchEmp.Text + "%' or PS.SUPERVISOR_NAME  like '" + txtSearchEmp.Text + "%' or PPM.MACHINE_NUMBER like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%'  or  to_char(PPRM.ENTRY_DATE, 'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or PPRM.IS_ACTIVE like '" + txtSearchEmp.Text + "%') ORDER BY PPRM.CREATE_DATE DESC";
                        }
                        else
                        {
                            makeSQL = " SELECT PPRM.PRODUCTION_ID, PPS.SHIFT_NAME, PPM.MACHINE_NUMBER, SUBSTR(PPRM.ENTRY_DATE,0,2) || '-' || PPS.SHIFT_NAME || '-' || PPM.MACHINE_NUMBER  AS SHIFT_MACHINE, PS.SUPERVISOR_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PPRM.ITEM_WEIGHT, PPRM.ITEM_WEIGHT_IN_FG, PPRM.PGE_PERCENT, PPRM.PGE_WEIGHT, PPRM.ENTRY_DATE, PPRM.CREATE_DATE, PPRM.UPDATE_DATE, PPRM.IS_ACTIVE FROM PF_PRODUCTION_MASTER PPRM LEFT JOIN PF_PRODUCTION_SHIFT PPS ON PPS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_PRODUCTION_MACHINE PPM ON PPM.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID where to_char(PPRM.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear3.Text + "' AND PI.ITEM_ID like '" + DropDownItemID1.Text + "%' AND (PPRM.PRODUCTION_ID like '" + txtSearchEmp.Text + "%' or PPS.SHIFT_NAME like '" + txtSearchEmp.Text + "%' or PS.SUPERVISOR_NAME  like '" + txtSearchEmp.Text + "%' or PPM.MACHINE_NUMBER like '" + txtSearchEmp.Text + "%' or  to_char(PPRM.ENTRY_DATE, 'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or PPRM.IS_ACTIVE like '" + txtSearchEmp.Text + "%') ORDER BY PPRM.CREATE_DATE DESC";
                        }
                    }
                }
                else
                {
                    if (DropDownItemID1.Text == "0")
                    {
                        makeSQL = " SELECT PPRM.PRODUCTION_ID, PPS.SHIFT_NAME, PPM.MACHINE_NUMBER, SUBSTR(PPRM.ENTRY_DATE,0,2) || '-' || PPS.SHIFT_NAME || '-' || PPM.MACHINE_NUMBER  AS SHIFT_MACHINE, PS.SUPERVISOR_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PPRM.ITEM_WEIGHT, PPRM.ITEM_WEIGHT_IN_FG, PPRM.PGE_PERCENT, PPRM.PGE_WEIGHT, PPRM.ENTRY_DATE, PPRM.CREATE_DATE, PPRM.UPDATE_DATE, PPRM.IS_ACTIVE FROM PF_PRODUCTION_MASTER PPRM LEFT JOIN PF_PRODUCTION_SHIFT PPS ON PPS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_PRODUCTION_MACHINE PPM ON PPM.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID where PPRM.PRODUCTION_ID like '" + txtSearchEmp.Text + "%' or PPS.SHIFT_NAME like '" + txtSearchEmp.Text + "%' or PS.SUPERVISOR_NAME  like '" + txtSearchEmp.Text + "%' or PPM.MACHINE_NUMBER like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PPRM.ENTRY_DATE like '" + txtSearchEmp.Text + "%'  or  to_char(PPRM.ENTRY_DATE, 'mm/yyyy') like '" + txtSearchEmp.Text + "%' or PPRM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ORDER BY PPRM.CREATE_DATE DESC";
                    }
                    else
                    {
                        makeSQL = " SELECT PPRM.PRODUCTION_ID, PPS.SHIFT_NAME, PPM.MACHINE_NUMBER, SUBSTR(PPRM.ENTRY_DATE,0,2) || '-' || PPS.SHIFT_NAME || '-' || PPM.MACHINE_NUMBER  AS SHIFT_MACHINE, PS.SUPERVISOR_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PPRM.ITEM_WEIGHT, PPRM.ITEM_WEIGHT_IN_FG, PPRM.PGE_PERCENT, PPRM.PGE_WEIGHT, PPRM.ENTRY_DATE, PPRM.CREATE_DATE, PPRM.UPDATE_DATE, PPRM.IS_ACTIVE FROM PF_PRODUCTION_MASTER PPRM LEFT JOIN PF_PRODUCTION_SHIFT PPS ON PPS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_PRODUCTION_MACHINE PPM ON PPM.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID where PI.ITEM_ID like '" + DropDownItemID1.Text + "%' AND (PPRM.PRODUCTION_ID like '" + txtSearchEmp.Text + "%' or PPS.SHIFT_NAME like '" + txtSearchEmp.Text + "%' or PS.SUPERVISOR_NAME  like '" + txtSearchEmp.Text + "%' or PPM.MACHINE_NUMBER like '" + txtSearchEmp.Text + "%'  or PPRM.ENTRY_DATE like '" + txtSearchEmp.Text + "%'  or  to_char(PPRM.ENTRY_DATE, 'mm/yyyy') like '" + txtSearchEmp.Text + "%' or PPRM.IS_ACTIVE like '" + txtSearchEmp.Text + "%') ORDER BY PPRM.CREATE_DATE DESC";
                    } 
                    alert_box.Visible = false; 
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "PRODUCTION_ID" };
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
                    makeSQL = "  SELECT PI.ITEM_NAME, count(PPM.PRODUCTION_ID) AS PRODUCTION_ID, sum(PPM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PGE_WEIGHT) AS PGE_WEIGHT, nvl(PAG.ACTUAL_GAR_WEIGHT,0) AS ACTUAL_GAR_WEIGHT, nvl(sum(ITEM_WEIGHT) + sum(PGE_WEIGHT) + PAG.ACTUAL_GAR_WEIGHT,0) AS TOTAL_WEIGHT FROM PF_ITEM PI LEFT JOIN PF_PRODUCTION_MASTER PPM ON PPM.ITEM_ID = PI.ITEM_ID LEFT JOIN PF_ACTUAL_GARBAGE PAG ON PAG.ITEM_ID = PI.ITEM_ID AND to_char(PAG.MONTH_YEAR, 'mm/yyyy') = '" + MonthYear + "' WHERE to_char(PPM.ENTRY_DATE, 'mm/yyyy') = '" + MonthYear + "' GROUP BY PI.ITEM_ID, PI.ITEM_NAME, PAG.ACTUAL_GAR_WEIGHT ORDER BY PI.ITEM_ID ";
                }
                else
                {
                    makeSQL = "  SELECT PI.ITEM_NAME, count(PPM.PRODUCTION_ID) AS PRODUCTION_ID, sum(PPM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PGE_WEIGHT) AS PGE_WEIGHT, nvl(PAG.ACTUAL_GAR_WEIGHT,0) AS ACTUAL_GAR_WEIGHT, nvl(sum(ITEM_WEIGHT) + sum(PGE_WEIGHT) + PAG.ACTUAL_GAR_WEIGHT,0) AS TOTAL_WEIGHT FROM PF_ITEM PI LEFT JOIN PF_PRODUCTION_MASTER PPM ON PPM.ITEM_ID = PI.ITEM_ID LEFT JOIN PF_ACTUAL_GARBAGE PAG ON PAG.ITEM_ID = PI.ITEM_ID AND to_char(PAG.MONTH_YEAR, 'mm/yyyy') = '" + TextMonthYear4.Text + "' WHERE to_char(PPM.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear4.Text + "' GROUP BY PI.ITEM_ID, PI.ITEM_NAME, PAG.ACTUAL_GAR_WEIGHT ORDER BY PI.ITEM_ID ";
              
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

                    decimal total_prod = dt.AsEnumerable().Sum(row => row.Field<decimal>("PRODUCTION_ID"));
                    GridView2.FooterRow.Cells[1].Font.Bold = true;
                    GridView2.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Center;
                    GridView2.FooterRow.Cells[1].Text = total_prod.ToString("N0");

                    decimal total_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_WEIGHT"));
                    GridView2.FooterRow.Cells[2].Font.Bold = true;
                    GridView2.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[2].Text = total_wt.ToString("N3");

                    decimal total_pge_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("PGE_WEIGHT"));
                    GridView2.FooterRow.Cells[3].Font.Bold = true;
                    GridView2.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[3].Text = total_pge_wt.ToString("N3");

                    decimal total_ag_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("ACTUAL_GAR_WEIGHT"));
                    GridView2.FooterRow.Cells[4].Font.Bold = true;
                    GridView2.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[4].Text = total_ag_wt.ToString("N3");

                    decimal total_grand_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("TOTAL_WEIGHT"));
                    GridView2.FooterRow.Cells[5].Font.Bold = true;
                    GridView2.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[5].Text = total_grand_wt.ToString("N3");
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
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string Production_ID = TextProductionID.Text;    
                int ShiftID = Convert.ToInt32(DropDownShiftID.Text);
                int SupervisorID = Convert.ToInt32(DropDownSupervisorID.Text);
                int MachineID = Convert.ToInt32(DropDownMachineID.Text);
                int ItemID = Convert.ToInt32(DropDownItemID.Text);
                int SubItemID = Convert.ToInt32(DropDownSubItemID.Text);
                string ItemName = DropDownItemID.SelectedItem.Text;
                string SubItemName = "";
                if (SubItemID == 0)
                { SubItemID = 0; SubItemName = "";  }  else  {  SubItemID = Convert.ToInt32(DropDownSubItemID.Text);  SubItemName = DropDownSubItemID.SelectedItem.Text;
                }
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                int PageID = Convert.ToInt32(DropDownPgeID.Text);
                double PgePercent = Convert.ToDouble(DropDownPgeID.SelectedItem.Text);
                double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
                double ItemPgeWet = (ItemWeight * PgePercent) / 100;
                double ItemWeightInFg = ItemWeight - ItemPgeWet;

                string MakeEntryDate = EntryDate.Text;
                string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                //inventory calculation

                int InvenItemID = 0;
                int InvenSubItemID = 0;
                double InitialStock = 0.00, StockInWet = 0.00, StockInWetDe = 0.00, StockOutWet = 0.00, StockOutWetDe = 0.00, FinalStock = 0.00, StockInWetNew = 0.00, StockOutWetNew = 0.00, FinalStockNew = 0.00;

                // check production data
                int ItemIdOld = 0, SubItemIdOld = 0; double ItemWeightOld = 0.00, ItemWeightOldFg = 0.00, ItemPgeWetOld = 0.00;
                string makeSQLPro = " select ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, PGE_WEIGHT, ITEM_WEIGHT_IN_FG from PF_PRODUCTION_MASTER where PRODUCTION_ID  = '" + Production_ID + "'  ";
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
                    ItemWeightOldFg = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_IN_FG"].ToString());
                    ItemPgeWetOld = Convert.ToDouble(dt.Rows[i]["PGE_WEIGHT"].ToString());
                }

               
                    // check inventory RM
                    string makeSQLRmIn = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                    cmdl = new OracleCommand(makeSQLRmIn);
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

                    if ((ItemWeightOld + ItemPgeWetOld) <= FinalStock)
                    {
                     
                    StockOutWetNew = Math.Abs(StockOutWet - (ItemWeightOld + ItemPgeWetOld));  
                    FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;

                    if (0 < RowCount)
                    {
                        // update inventory RM (minus old data)
                        string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }
                   

                    // check inventory RM
                    string makeSQLRmInNew = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeSQLRmInNew);
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
                   // update inventory RM (plus new data)
                    StockOutWetDe = StockOutWet + ItemWeight + ItemPgeWet;
                    FinalStockNew = (InitialStock + StockInWet) - StockOutWetDe;

                    // update inventory RM
                    string update_inven_rm_new = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                    cmdu = new OracleCommand(update_inven_rm_new, conn);

                    OracleParameter[] objPrmInevenRMn = new OracleParameter[5];
                    objPrmInevenRMn[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetDe);
                    objPrmInevenRMn[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                    objPrmInevenRMn[2] = cmdu.Parameters.Add("u_date", u_date);
                    objPrmInevenRMn[3] = cmdu.Parameters.Add("NoCuserID", userID);
                    objPrmInevenRMn[4] = cmdu.Parameters.Add("NoItemID", ItemID);

                    cmdu.ExecuteNonQuery();
                    cmdu.Parameters.Clear();
                    cmdu.Dispose();


            
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

                StockInWetNew = StockInWet - ItemWeightOld;
                FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;

                if (0 < RowCount)
                {
                    // update inventory FG (minus old data)
                    string update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
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


                    // check inventory FG
                    string makeFgSQL = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeFgSQL);
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

                    // update inventory FG (plus new data)

                StockInWetDe = StockInWet + ItemWeight;
                FinalStockNew = (InitialStock + StockInWetDe) - StockOutWet;

                // update inventory FG
                string update_inven_fg_new = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                cmdu = new OracleCommand(update_inven_fg_new, conn);

                OracleParameter[] objPrmInevenFGn = new OracleParameter[5];
                objPrmInevenFGn[0] = cmdu.Parameters.Add("NoStockIn", StockInWetDe);
                objPrmInevenFGn[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                objPrmInevenFGn[2] = cmdu.Parameters.Add("u_date", u_date);
                objPrmInevenFGn[3] = cmdu.Parameters.Add("NoCuserID", userID);
                objPrmInevenFGn[4] = cmdu.Parameters.Add("NoItemID", ItemID); 

                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose();

             
                // update inventory FG details
                string insert_inven_fg_des = "update  PF_FG_STOCK_INVENTORY_MAS_DE  set ITEM_ID = :NoItemID, ITEM_NAME = :TextItemName, SUB_ITEM_ID = :NoSubItemID, SUB_ITEM_NAME = :TextSubItemName, STOCK_IN_WT = :NoStockIn, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where REF_ID = :NoRefID ";
                cmdi = new OracleCommand(insert_inven_fg_des, conn);

                OracleParameter[] objPrmIfgd = new OracleParameter[9];
                objPrmIfgd[1] = cmdi.Parameters.Add("NoItemID", ItemID);
                objPrmIfgd[2] = cmdi.Parameters.Add("TextItemName", ItemName);
                objPrmIfgd[3] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                objPrmIfgd[4] = cmdi.Parameters.Add("TextSubItemName", SubItemName);
                objPrmIfgd[5] = cmdi.Parameters.Add("NoStockIn", ItemWeight);
                objPrmIfgd[6] = cmdi.Parameters.Add("u_date", u_date);
                objPrmIfgd[7] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrmIfgd[8] = cmdi.Parameters.Add("NoRefID", Production_ID);
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();

               


                // update inventory RM details
                string insert_inven_rm_des = "update  PF_RM_STOCK_INVENTORY_MAS_DE  set ITEM_ID = :NoItemID, ITEM_NAME = :TextItemName, SUB_ITEM_ID = :NoSubItemID, SUB_ITEM_NAME = :TextSubItemName, STOCK_IN_WT = :NoStockOut, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where REF_ID = :NoRefID ";
                cmdi = new OracleCommand(insert_inven_rm_des, conn);

                OracleParameter[] objPrmIrmd = new OracleParameter[9];
                objPrmIrmd[1] = cmdi.Parameters.Add("NoItemID", ItemID);
                objPrmIrmd[2] = cmdi.Parameters.Add("TextItemName", ItemName);
                objPrmIrmd[3] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                objPrmIrmd[4] = cmdi.Parameters.Add("TextSubItemName", SubItemName);
                objPrmIrmd[5] = cmdi.Parameters.Add("NoStockOut", ItemWeight);
                objPrmIrmd[6] = cmdi.Parameters.Add("u_date", u_date);
                objPrmIrmd[7] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrmIrmd[8] = cmdi.Parameters.Add("NoRefID", Production_ID);
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                
                // update production master
                string update_production = "update  PF_PRODUCTION_MASTER  set SHIFT_ID = :NoShiftID, MACHINE_ID = :NoMachineID, SUPERVISOR_ID = :NoSupervisorID, ITEM_ID = :NoItemID, SUB_ITEM_ID = :NoSubItemID, ITEM_WEIGHT = :TextItemWeight, ITEM_WEIGHT_IN_FG = :TextItemWeightInFg, PGE_ID = :NoPageID, PGE_PERCENT = :TextPgePercent, PGE_WEIGHT = :TextItemPgeWet, ENTRY_DATE = TO_DATE(:TextEntryDate, 'DD-MM-YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where PRODUCTION_ID = :NoProductionID ";
                cmdu = new OracleCommand(update_production, conn);

                OracleParameter[] objPrm = new OracleParameter[15];
                objPrm[0] = cmdu.Parameters.Add("NoShiftID", ShiftID);
                objPrm[1] = cmdu.Parameters.Add("NoMachineID", MachineID);
                objPrm[2] = cmdu.Parameters.Add("NoSupervisorID", SupervisorID);
                objPrm[3] = cmdu.Parameters.Add("NoItemID", ItemID);
                objPrm[4] = cmdu.Parameters.Add("NoSubItemID", SubItemID); 
                objPrm[5] = cmdu.Parameters.Add("TextItemWeight", ItemWeight);
                objPrm[6] = cmdu.Parameters.Add("TextItemWeightInFg", ItemWeightInFg); 
                objPrm[7] = cmdu.Parameters.Add("NoPageID", PageID);
                objPrm[8] = cmdu.Parameters.Add("TextPgePercent", PgePercent);
                objPrm[9] = cmdu.Parameters.Add("TextItemPgeWet", ItemPgeWet);
                objPrm[10] = cmdu.Parameters.Add("TextEntryDate", EntryDateNew); 
                objPrm[11] = cmdu.Parameters.Add("u_date", u_date);
                objPrm[12] = cmdu.Parameters.Add("NoCuserID", userID);
                objPrm[13] = cmdu.Parameters.Add("TextIsActive", ISActive);
                objPrm[14] = cmdu.Parameters.Add("NoProductionID", Production_ID);

                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose();

                // uodate garbage
                string update_garbage = "update  PF_GARBAGE  set ITEM_ID = :NoItemID, ITEM_NAME = :TextItemName, SUB_ITEM_ID = :NoSubItemID, SUB_ITEM_NAME = :TextSubItemName, PGE_ID = :NoPageID, PGE_PERCENT = :TextPgePercent, PGE_WEIGHT = :TextItemPgeWet, ENTRY_DATE = TO_DATE(:TextEntryDate, 'DD-MM-YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID where PRODUCTION_ID = :NoProductionID ";
                cmdu = new OracleCommand(update_garbage, conn);

                OracleParameter[] objPrmG = new OracleParameter[11]; 
                objPrmG[0] = cmdu.Parameters.Add("NoItemID", ItemID);                
                objPrmG[1] = cmdu.Parameters.Add("TextItemName", ItemName);
                objPrmG[2] = cmdu.Parameters.Add("NoSubItemID", SubItemID); 
                objPrmG[3] = cmdu.Parameters.Add("TextSubItemName", SubItemName);
                objPrmG[4] = cmdu.Parameters.Add("NoPageID", PageID);
                objPrmG[5] = cmdu.Parameters.Add("TextPgePercent", PgePercent);
                objPrmG[6] = cmdu.Parameters.Add("TextItemPgeWet", ItemPgeWet);
                objPrmG[7] = cmdu.Parameters.Add("TextEntryDate", EntryDateNew);
                objPrmG[8] = cmdu.Parameters.Add("u_date", u_date);
                objPrmG[9] = cmdu.Parameters.Add("NoCuserID", userID); 
                objPrmG[10] = cmdu.Parameters.Add("NoProductionID", Production_ID);

                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose(); 

                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Production Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearText();
                Display();
                DisplaySummary();
                }
                else
                {

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the RM Inventory"));
                    alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                }
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
                 //inventory calculation

                int InvenItemID = 0;
                int InvenSubItemID = 0;
                double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00,  FinalStock = 0.00, StockInWetNew = 0.00, StockOutWetNew = 0.00, FinalStockNew = 0.00;
                // check production data
                int ItemIdOld = 0, SubItemIdOld = 0; double ItemWeightOld = 0.00, ItemWeightOldFg = 0.00, ItemPgeWetOld = 0.00;
                string makeSQLPro = " select ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, ITEM_WEIGHT_IN_FG, PGE_WEIGHT from PF_PRODUCTION_MASTER where PRODUCTION_ID  = '" + Production_ID + "'  ";
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
                    ItemWeightOldFg = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_IN_FG"].ToString());
                    ItemPgeWetOld = Convert.ToDouble(dt.Rows[i]["PGE_WEIGHT"].ToString());
                }

                    // check inventory RM
                    string makeSQLRmIn = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                    cmdl = new OracleCommand(makeSQLRmIn);
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
                    if ((ItemWeightOld + ItemPgeWetOld) <= FinalStock)
                    {

                    StockOutWetNew = StockOutWet - (ItemWeightOld + ItemPgeWetOld);
                    FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;

                    if (0 < RowCount)
                    {
                        // update inventory RM (minus old data)
                        string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }

               
            

                // check inventory FG
                string makeSQL = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "' ";
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

              
                StockInWetNew = StockInWet - ItemWeightOld;
                FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;

                if (0 < RowCount)
                {
                    // update inventory FG (minus old data)
                    string update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
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

                // delete inventory FG details 
                string delete_prod_fg_de = " delete from PF_FG_STOCK_INVENTORY_MAS_DE where REF_ID  = '" + Production_ID + "'";
                cmdi = new OracleCommand(delete_prod_fg_de, conn);
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                 
              
                
                // delete inventory RM details
                string delete_prod_rm_de = " delete from PF_RM_STOCK_INVENTORY_MAS_DE where REF_ID  = '" + Production_ID + "'";
                cmdi = new OracleCommand(delete_prod_rm_de, conn);
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                 
                // delete production master
                string delete_production = " delete from PF_PRODUCTION_MASTER where PRODUCTION_ID  = '" + Production_ID + "'"; 
                cmdi = new OracleCommand(delete_production, conn); 
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 

                // delete garbage
                string delete_garbage = " delete from PF_GARBAGE where PRODUCTION_ID  = '" + Production_ID + "'";
                cmdu = new OracleCommand(delete_garbage, conn);
                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose(); 

                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Production Data Delete successfully"));
                alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                clearText(); 
                Display();
                DisplaySummary();
                    }
                    else
                    {

                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the RM Inventory"));
                        alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
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

        public void clearTextField(object sender, EventArgs e)
        {
            TextProductionID.Text = "";
            DropDownPgeID.Text = "0"; 
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
            DropDownPgeID.Text = "0";
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
            TextItemWeight.Focus();
            TextItemWeight.Text = "";
            alert_box.Visible = false;

        }

        public void TextPgeWet_Changed(object sender, EventArgs e)
        {

            if (TextItemWeight.Text != "" && DropDownPgeID.Text != "0")
            {
                int PgePercent = Convert.ToInt32(DropDownPgeID.SelectedItem.Text);
                double ItemWeight = Convert.ToDouble(TextItemWeight.Text);
                double ItemPgeWet = (ItemWeight * PgePercent) / 100;
                TextPgeWet.Text = ItemPgeWet.ToString("0.000");
                EntryDate.Focus();
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

                    string makeSQL = " select nvl(FINAL_STOCK_WT,0) AS FINAL_STOCK_WT from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "' ";
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
                        if (TextItemWeight.Text != "" && DropDownPgeID.Text != "0")
                        {
                            double PgePercent = Convert.ToDouble(DropDownPgeID.SelectedItem.Text); 
                            double ItemPgeWet = (ItemWeight * PgePercent) / 100;
                            TextPgeWet.Text = ItemPgeWet.ToString("0.000");

                        }
                        CheckItemWeight.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Material is available</label>"; 
                        CheckItemWeight.ForeColor = System.Drawing.Color.Green;
                        DropDownPgeID.Focus();
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