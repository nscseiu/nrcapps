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
using System.Collections;
using System.Collections.Generic;
using System.Web.Services;

namespace NRCAPPS.MS
{
    public partial class MsExpShipmentCmo : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt, ds;
        int RowCount; 
        string IS_PAGE_ACTIVE = "", IS_ADD_ACTIVE = "",IS_EDIT_ACTIVE = "",IS_DELETE_ACTIVE = "",IS_VIEW_ACTIVE = "", IS_REPORT_ACTIVE = "", IS_PRINT_ACTIVE ="";
        private double StockOutWetNew;

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
                        TextWbSlipEx.Enabled = false;
                        Display();
                        DisplayComplete();

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


        public void ShipmentUpdateTransit_Click(object sender, EventArgs e)
        {
            try
              {
                if (IS_EDIT_ACTIVE == "Enable")
                {

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    int userID = Convert.ToInt32(Session["USER_ID"]);

                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    foreach (GridViewRow gridRow in GridView1.Rows)
                    {
                        CheckBox chkRowIs = (gridRow.Cells[1].FindControl("IsShipmentCheck") as CheckBox); 
                        if (chkRowIs.Checked)
                        { 
                            int ItemIdOld = 0; double ItemWeightOld = 0.00;
                            string makeSQLPro = " select ITEM_ID, ITEM_WEIGHT from MS_EXPORT_WBSLIP_CON_ITEM where EXP_WBCON_ITEM_ID  = '" + gridRow.Cells[0].Text + "'  ";
                            cmdl = new OracleCommand(makeSQLPro);
                            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                            dt = new DataTable();
                            oradata.Fill(dt);
                            RowCount = dt.Rows.Count;

                            for (int i = 0; i < RowCount; i++)
                            {
                                ItemIdOld = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                                ItemWeightOld = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());
                            }

                            //inventory calculation

                            int InvenItemID = 0;
                            double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, FinalStock = 0.00, FinalStockNew = 0.00;

                            // check inventory RM
                            string makeSQL = " select * from MS_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "' ";
                            cmdl = new OracleCommand(makeSQL);
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

                            if (ItemWeightOld <= FinalStock)
                            { 
                                StockOutWetNew = StockOutWet + ItemWeightOld;
                                FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;
                                 
                                // update inventory FG (minus old data)
                                string update_inven_mas = "update  MS_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
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

                                // update data 

                                string update_ex_invoice = " update  MS_EXPORT_WBSLIP_CON_ITEM set IS_INVENTORY_STATUS =:IsConfirm,  IS_SHIPMENT_COMPLETE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , IS_SHIPMENT_COMPLETE_C_ID = :NoCuserID  where EXP_WBCON_ITEM_ID =:NoWbItemID ";
                                cmdi = new OracleCommand(update_ex_invoice, conn);

                                OracleParameter[] objPrm = new OracleParameter[5];
                                objPrm[0] = cmdi.Parameters.Add("NoWbItemID", gridRow.Cells[0].Text);
                                objPrm[1] = cmdi.Parameters.Add("IsConfirm", "Complete");
                                objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                                objPrm[3] = cmdi.Parameters.Add("NoCuserID", userID);

                                cmdi.ExecuteNonQuery();
                                cmdi.Parameters.Clear();
                                cmdi.Dispose();

                                conn.Close();
                                Display();
                                DisplayComplete();

                                alert_box.Visible = true;
                            alert_box.Controls.Add(new LiteralControl("Shipment Status Update successfully"));
                            alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                           
                        }
                        else
                        {
                            alert_box.Visible = true;
                            alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory"));
                            alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                        }
                        }
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
         
        public void ShipmentUpdateComplete_Click(object sender, EventArgs e)
        {
            try
              {
                if (IS_EDIT_ACTIVE == "Enable")
                {

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    int userID = Convert.ToInt32(Session["USER_ID"]);

                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    foreach (GridViewRow gridRow in GridView2.Rows)
                    {
                        CheckBox chkRowIs = (gridRow.Cells[1].FindControl("IsShipmentCheck") as CheckBox); 
                        if (chkRowIs.Checked)
                        { 
                            int ItemIdOld = 0; double ItemWeightOld = 0.00;
                            string makeSQLPro = " select ITEM_ID, ITEM_WEIGHT from MS_EXPORT_WBSLIP_CON_ITEM where EXP_WBCON_ITEM_ID  = '" + gridRow.Cells[0].Text + "'  ";
                            cmdl = new OracleCommand(makeSQLPro);
                            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                            dt = new DataTable();
                            oradata.Fill(dt);
                            RowCount = dt.Rows.Count;

                            for (int i = 0; i < RowCount; i++)
                            {
                                ItemIdOld = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                                ItemWeightOld = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());
                            }

                            //inventory calculation

                            int InvenItemID = 0;
                            double InitialStock = 0.00, StockInWet = 0.00, StockInWetNew = 0.00, StockOutWet = 0.00, FinalStock = 0.00, FinalStockNew = 0.00;

                            // check inventory FG
                            string makeSQL = " select * from MS_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "' ";
                            cmdl = new OracleCommand(makeSQL);
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

                            
                                StockInWetNew = StockInWet + ItemWeightOld;
                                FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;
                                 
                                // update inventory FG (minus old data)
                                string update_inven_mas = "update  MS_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
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

                                // update data 

                                string update_ex_invoice = " update  MS_EXPORT_WBSLIP_CON_ITEM set IS_INVENTORY_STATUS =:IsConfirm,  IS_SHIPMENT_COMPLETE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , IS_SHIPMENT_COMPLETE_C_ID = :NoCuserID  where EXP_WBCON_ITEM_ID =:NoWbItemID ";
                                cmdi = new OracleCommand(update_ex_invoice, conn);

                                OracleParameter[] objPrm = new OracleParameter[5];
                                objPrm[0] = cmdi.Parameters.Add("NoWbItemID", gridRow.Cells[0].Text);
                                objPrm[1] = cmdi.Parameters.Add("IsConfirm", "Transit");
                                objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                                objPrm[3] = cmdi.Parameters.Add("NoCuserID", userID);

                                cmdi.ExecuteNonQuery();
                                cmdi.Parameters.Clear();
                                cmdi.Dispose();

                                conn.Close();
                                Display();
                                DisplayComplete();

                                alert_box.Visible = true;
                                alert_box.Controls.Add(new LiteralControl("Shipment Status Update successfully"));
                                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                           
                         
                        }
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
        public void Display()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open(); 
                string makeSQL = "";
                string MonthYear = System.DateTime.Now.ToString("MM/yyyy");
                 
                if (txtSearchGrideView1.Text == "")
                {
                    makeSQL = " SELECT PEWCI.EXP_WBCON_ITEM_ID, PEWC.WB_SLIP_NO, PP.PARTY_NAME, PEWC.SEAL_NO, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.REF_NO, PEWC.IS_EDIT, PEWCI.EXP_WBCON_ITEM_ID, PI.ITEM_NAME, PEWC.ITEM_WEIGHT_WB, PEWC.TARE_WEIGHT, PEWCI.ITEM_WEIGHT, PEWCI.PACKING_WEIGHT, PEWC.IS_ACTIVE, PEWCI.IS_INVENTORY_STATUS, PEWCI.IS_ACTIVE_PRICING, PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT,  PEWC.PRINT_DATE, PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO FROM MS_EXPORT_WBSLIP_CON PEWC LEFT JOIN MS_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN MS_EXPORT_SALES_MASTER WESM ON WESM.EXPORT_INVOICE_NO = PEWC.EXPORT_INVOICE_NO LEFT JOIN MS_PARTY PP ON PP.PARTY_ID = WESM.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE PEWCI.IS_INVENTORY_STATUS = 'Transit' AND PEWC.IS_CONFIRM_CHECK = 'Complete' ORDER BY PEWC.WB_SLIP_NO desc, PEWCI.ITEM_ID ASC "; // to_char(PEWC.DISPATCH_DATE, 'mm/yyyy') = '" + MonthYear + "' AND
                }
                else
                {
                    makeSQL = " SELECT PEWCI.EXP_WBCON_ITEM_ID, PEWC.WB_SLIP_NO, PP.PARTY_NAME, PEWC.SEAL_NO, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.REF_NO, PEWC.IS_EDIT, PEWCI.EXP_WBCON_ITEM_ID, PI.ITEM_NAME, PEWC.ITEM_WEIGHT_WB, PEWC.TARE_WEIGHT, PEWCI.ITEM_WEIGHT, PEWCI.PACKING_WEIGHT, PEWC.IS_ACTIVE, PEWCI.IS_INVENTORY_STATUS, PEWCI.IS_ACTIVE_PRICING, PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT,  PEWC.PRINT_DATE, PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO FROM MS_EXPORT_WBSLIP_CON PEWC LEFT JOIN MS_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN MS_EXPORT_SALES_MASTER WESM ON WESM.EXPORT_INVOICE_NO = PEWC.EXPORT_INVOICE_NO LEFT JOIN MS_PARTY PP ON PP.PARTY_ID = WESM.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE PEWC.IS_INVENTORY_STATUS = 'Transit' AND PEWC.IS_CONFIRM_CHECK = 'Complete' AND (PEWC.CONTAINER_NO like '" + txtSearchGrideView1.Text + "%' OR to_char(PEWC.DISPATCH_DATE, 'dd/mm/yyyy')  like '" + txtSearchGrideView1.Text + "%' or  PEWC.CONTAINER_NO like '" + txtSearchGrideView1.Text + "%')  ORDER BY PEWC.WB_SLIP_NO desc, PEWC.ITEM_ID ASC  ";  

                    alert_box.Visible = false;
                }
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "WB_SLIP_NO" };
                GridView1.DataBind();
                
                conn.Close();
                // alert_box.Visible = false;
            }
        }

         
        protected void GridView1Search(object sender, EventArgs e)
        {
            this.Display();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            Display();
            alert_box.Visible = false;
        }

        public void DisplayComplete()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = "";
                string MonthYear = System.DateTime.Now.ToString("MM/yyyy");

                if (txtSearchGrideView2.Text == "")
                {
                    makeSQL = " SELECT PEWC.WB_SLIP_NO, PP.PARTY_NAME, PEWC.SEAL_NO, PEWCI.IS_SHIPMENT_COMPLETE_DATE, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.REF_NO, PEWC.IS_EDIT, PEWCI.EXP_WBCON_ITEM_ID, PI.ITEM_NAME, PEWC.ITEM_WEIGHT_WB, PEWC.TARE_WEIGHT, PEWCI.ITEM_WEIGHT, PEWCI.PACKING_WEIGHT, PEWC.IS_ACTIVE, PEWCI.IS_INVENTORY_STATUS, PEWCI.IS_ACTIVE_PRICING, PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT,  PEWC.PRINT_DATE, PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO FROM MS_EXPORT_WBSLIP_CON PEWC LEFT JOIN MS_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN MS_EXPORT_SALES_MASTER WESM ON WESM.EXPORT_INVOICE_NO = PEWC.EXPORT_INVOICE_NO LEFT JOIN MS_PARTY PP ON PP.PARTY_ID = WESM.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE PEWCI.IS_INVENTORY_STATUS = 'Complete' AND PEWC.IS_CONFIRM_CHECK = 'Complete' ORDER BY PEWC.WB_SLIP_NO desc, PEWCI.ITEM_ID ASC   ";  // to_char(PEWC.DISPATCH_DATE, 'mm/yyyy') = '" + MonthYear + "' AND
                }
                else
                {
                    makeSQL = " SELECT PEWC.WB_SLIP_NO, PP.PARTY_NAME, PEWC.SEAL_NO, PEWCI.IS_SHIPMENT_COMPLETE_DATE, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.REF_NO, PEWC.IS_EDIT, PEWCI.EXP_WBCON_ITEM_ID, PI.ITEM_NAME, PEWC.ITEM_WEIGHT_WB, PEWC.TARE_WEIGHT, PEWCI.ITEM_WEIGHT, PEWCI.PACKING_WEIGHT, PEWC.IS_ACTIVE, PEWCI.IS_INVENTORY_STATUS, PEWCI.IS_ACTIVE_PRICING, PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT,  PEWC.PRINT_DATE, PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO FROM MS_EXPORT_WBSLIP_CON PEWC LEFT JOIN MS_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN MS_EXPORT_SALES_MASTER WESM ON WESM.EXPORT_INVOICE_NO = PEWC.EXPORT_INVOICE_NO LEFT JOIN MS_PARTY PP ON PP.PARTY_ID = WESM.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE PEWCI.IS_INVENTORY_STATUS = 'Complete' AND PEWC.IS_CONFIRM_CHECK = 'Complete' AND (PEWC.CONTAINER_NO like '" + txtSearchGrideView2.Text + "%' OR to_char(PEWC.DISPATCH_DATE, 'dd/mm/yyyy')  like '" + txtSearchGrideView2.Text + "%' or  PEWC.WB_SLIP_NO like '" + txtSearchGrideView2.Text + "%')  ORDER BY PEWC.WB_SLIP_NO desc, PEWC.ITEM_ID ASC  ";

                    alert_box.Visible = false;
                }


                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataKeyNames = new string[] { "WB_SLIP_NO" };
                GridView2.DataBind();
                
                conn.Close();
                // alert_box.Visible = false;
            }
        }

         
        protected void GridView2Search(object sender, EventArgs e)
        {
            this.DisplayComplete();
        }

        protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView2.PageIndex = e.NewPageIndex;
            DisplayComplete();
            alert_box.Visible = false;
        }

        public void clearTextField(object sender, EventArgs e)
        {
            TextWbSlipEx.Text = "";
            TextWbConItemEx.Text = "";
            EntryDate.Text = "";

        }
        public void clearText()
        {
            TextWbSlipEx.Text = "";
            TextWbConItemEx.Text = "";
            EntryDate.Text = "";

        }
        protected void linkSelectClick(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
            string USER_DATA_ID = Session["user_data_id"].ToString();
              
            string makeSQL = "  SELECT WB_SLIP_NO, EXP_WBCON_ITEM_ID, TO_CHAR(IS_SHIPMENT_COMPLETE_DATE, 'DD/MM/YYYY') AS IS_SHIPMENT_COMPLETE_DATE FROM MS_EXPORT_WBSLIP_CON_ITEM  WHERE EXP_WBCON_ITEM_ID = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            ds = new DataTable();
            oradata.Fill(ds);
            RowCount = ds.Rows.Count;

            for (int i = 0; i < RowCount; i++)
            {
                TextWbSlipEx.Text = ds.Rows[i]["WB_SLIP_NO"].ToString();
                TextWbConItemEx.Text = ds.Rows[i]["EXP_WBCON_ITEM_ID"].ToString();
                EntryDate.Text = ds.Rows[i]["IS_SHIPMENT_COMPLETE_DATE"].ToString(); 
            }

            Display();
            conn.Close();
            alert_box.Visible = false;
             

        }


        public void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (IS_EDIT_ACTIVE == "Enable")
                {

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-'); 
                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                    // update data  
                    string update_ex_invoice = " update  MS_EXPORT_WBSLIP_CON_ITEM set IS_SHIPMENT_COMPLETE_DATE = TO_DATE(:EntryDate, 'DD/MM/YYYY'), IS_SHIPMENT_COMPLETE_C_ID = :NoCuserID  where EXP_WBCON_ITEM_ID =: NoWbSlipNo ";
                    cmdi = new OracleCommand(update_ex_invoice, conn);

                    OracleParameter[] objPrm = new OracleParameter[3]; 
                    objPrm[0] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[1] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[2] = cmdi.Parameters.Add("NoWbSlipNo", TextWbConItemEx.Text);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    conn.Close();
                    Display();
                    DisplayComplete();
                    clearText();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Shipment Complete Status Date Update successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                          
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