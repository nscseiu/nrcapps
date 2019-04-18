using System; 
using System.Configuration;
using System.Data;
using System.Linq; 
using System.Web.UI; 
using System.Web.UI.WebControls; 
using System.Data.OracleClient;
using System.IO;   


namespace NRCAPPS.MF
{
    public partial class MfRmCoordinatorApprove : System.Web.UI.Page
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
                        
                        DisplayMatTransferPending();
                        DisplayMatTransferApprove();

                        DisplayMatPurchasePending();
                        DisplayMatPurchaseApprove();

                        DisplayMatRecevingPending();
                        DisplayMatRecevingApprove();

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

        ///  Start the Material Transfer Received
       
        public void TransferUpdatePendingApporve_Click(object sender, EventArgs e)
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
                            string makeSQLPro = " select ITEM_ID, NET_WT_MF from MF_PURCHASE_TRANSFER_MASTER where TRANSFER_ID  = '" + gridRow.Cells[0].Text + "'  ";
                            cmdl = new OracleCommand(makeSQLPro);
                            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                            dt = new DataTable();
                            oradata.Fill(dt);
                            RowCount = dt.Rows.Count;

                            for (int i = 0; i < RowCount; i++)
                            {
                                ItemIdOld = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                                ItemWeightOld = Convert.ToDouble(dt.Rows[i]["NET_WT_MF"].ToString());
                            }

                            //inventory calculation

                            int InvenItemID = 0;
                            double InitialStock = 0.00, StockInWet = 0.00, StockInWetNew = 0.00, StockOutWet = 0.00, FinalStock = 0.00, FinalStockNew = 0.00;

                            // check inventory FG
                            string makeSQL = " select * from MF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "' ";
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
                                string update_inven_mas = "update  MF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
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

                                string update_ex_invoice = " update  MF_PURCHASE_TRANSFER_MASTER set FIRST_APPROVED_IS =:IsConfirm,  FIRST_APPROVED_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , FIRST_APPROVED_USER_ID = :NoCuserID  where TRANSFER_ID =:NoWbItemID ";
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
                                DisplayMatTransferPending();
                                DisplayMatTransferApprove();

                                alert_box.Visible = true;
                                alert_box.Controls.Add(new LiteralControl("Material Transfer Received Pending Successfully"));
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

        public void DisplayMatTransferPending()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open(); 
                string makeSQL = "";
                string MonthYear = System.DateTime.Now.ToString("MM/yyyy");
                 
                if (txtSearchGrideView1.Text == "")
                {
                    makeSQL = " SELECT  MMTM.*, MV.VEHICLE_NO, MM.ITEM_NAME, MM.ITEM_CODE, MIB.ITEM_BIN_NAME FROM MF_PURCHASE_TRANSFER_MASTER MMTM LEFT JOIN MF_VEHICLE MV ON MV.VEHICLE_ID = MMTM.VEHICLE_ID LEFT JOIN MF_ITEM MM ON MM.ITEM_ID = MMTM.ITEM_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MMTM.ITEM_BIN_ID WHERE  (MMTM.FIRST_APPROVED_IS = 'Incomplete' OR MMTM.FIRST_APPROVED_IS IS NULL)  ORDER BY MMTM.WB_SLIP_NO desc, MMTM.ITEM_ID ASC  ";  // to_char(MMTM.ENTRY_DATE, 'mm/yyyy') = '" + MonthYear + "' AND
                }
                else
                {
                    makeSQL = " SELECT  MMTM.*, MV.VEHICLE_NO, MM.ITEM_NAME, MM.ITEM_CODE, MIB.ITEM_BIN_NAME FROM MF_PURCHASE_TRANSFER_MASTER MMTM LEFT JOIN MF_VEHICLE MV ON MV.VEHICLE_ID = MMTM.VEHICLE_ID LEFT JOIN MF_ITEM MM ON MM.ITEM_ID = MMTM.ITEM_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MMTM.ITEM_BIN_ID WHERE (MMTM.FIRST_APPROVED_IS = 'Incomplete' OR MMTM.FIRST_APPROVED_IS IS NULL) AND (MMTM.WB_SLIP_NO like '" + txtSearchGrideView1.Text + "%' OR to_char(MMTM.ENTRY_DATE, 'dd/mm/yyyy')  like '" + txtSearchGrideView1.Text + "%' or MV.VEHICLE_NO like '" + txtSearchGrideView1.Text + "%' or MM.ITEM_NAME like '" + txtSearchGrideView1.Text + "%' or MIB.ITEM_BIN_NAME like '" + txtSearchGrideView1.Text + "%')  ORDER BY MMTM.WB_SLIP_NO desc, MMTM.ITEM_ID ASC  ";
                     
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
            this.DisplayMatTransferPending();
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            DisplayMatTransferPending();
            alert_box.Visible = false;
        }

         
        public void TransferUpdateCompleteApporve_Click(object sender, EventArgs e)
        {
            //  try
            //    {
            if (IS_EDIT_ACTIVE == "Enable")
            {

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                foreach (GridViewRow gridRow in GridView2.Rows)
                {
                    CheckBox chkRowIs = (gridRow.Cells[1].FindControl("IsCompleteApporve") as CheckBox);
                    if (chkRowIs.Checked)
                    {
                        int ItemIdOld = 0; double ItemWeightOld = 0.00;
                        string makeSQLPro = " select ITEM_ID, NET_WT_MF from MF_PURCHASE_TRANSFER_MASTER where TRANSFER_ID  = '" + gridRow.Cells[0].Text + "'  ";
                        cmdl = new OracleCommand(makeSQLPro);
                        oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                        dt = new DataTable();
                        oradata.Fill(dt);
                        RowCount = dt.Rows.Count;

                        for (int i = 0; i < RowCount; i++)
                        {
                            ItemIdOld = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                            ItemWeightOld = Convert.ToDouble(dt.Rows[i]["NET_WT_MF"].ToString());
                        }

                        //inventory calculation

                        int InvenItemID = 0;
                        double InitialStock = 0.00, StockInWet = 0.00, StockInWetNew = 0.0, StockOutWet = 0.00,  FinalStock = 0.00, FinalStockNew = 0.00;

                        // check inventory RM
                        string makeSQL = " select * from MF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "' ";
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
                            StockInWetNew = StockInWet - ItemWeightOld;
                            FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;

                            // update inventory FG (minus old data)
                            string update_inven_mas = "update  MF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
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

                            string update_ex_invoice = " update  MF_PURCHASE_TRANSFER_MASTER set FIRST_APPROVED_IS =:IsConfirm,  FIRST_APPROVED_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , FIRST_APPROVED_USER_ID = :NoCuserID  where TRANSFER_ID =:NoWbItemID ";
                            cmdi = new OracleCommand(update_ex_invoice, conn);

                            OracleParameter[] objPrm = new OracleParameter[5];
                            objPrm[0] = cmdi.Parameters.Add("NoWbItemID", gridRow.Cells[0].Text);
                            objPrm[1] = cmdi.Parameters.Add("IsConfirm", "Incomplete");
                            objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                            objPrm[3] = cmdi.Parameters.Add("NoCuserID", userID);

                            cmdi.ExecuteNonQuery();
                            cmdi.Parameters.Clear();
                            cmdi.Dispose();

                            conn.Close();
                            DisplayMatTransferPending();
                            DisplayMatTransferApprove();

                            alert_box.Visible = true;
                            alert_box.Controls.Add(new LiteralControl("Material Transfer Received Approved Successfully"));
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
            //       }
            //    catch
            //     {
            //       Response.Redirect("~/ParameterError.aspx");
            //  }

        }

        public void DisplayMatTransferApprove()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = "";
                string MonthYear = System.DateTime.Now.ToString("MM/yyyy");

                if (txtSearchGrideView2.Text == "")
                {
                    makeSQL = " SELECT  MMTM.*, MV.VEHICLE_NO, MM.ITEM_NAME, MM.ITEM_CODE, MIB.ITEM_BIN_NAME FROM MF_PURCHASE_TRANSFER_MASTER MMTM LEFT JOIN MF_VEHICLE MV ON MV.VEHICLE_ID = MMTM.VEHICLE_ID LEFT JOIN MF_ITEM MM ON MM.ITEM_ID = MMTM.ITEM_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MMTM.ITEM_BIN_ID WHERE to_char(MMTM.ENTRY_DATE, 'mm/yyyy') = '" + MonthYear + "' AND (MMTM.FIRST_APPROVED_IS = 'Complete')  ORDER BY MMTM.CREATE_DATE DESC    ";   
                }
                else
                {
                    makeSQL = " SELECT  MMTM.*, MV.VEHICLE_NO, MM.ITEM_NAME, MM.ITEM_CODE, MIB.ITEM_BIN_NAME FROM MF_PURCHASE_TRANSFER_MASTER MMTM LEFT JOIN MF_VEHICLE MV ON MV.VEHICLE_ID = MMTM.VEHICLE_ID LEFT JOIN MF_ITEM MM ON MM.ITEM_ID = MMTM.ITEM_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MMTM.ITEM_BIN_ID WHERE MMTM.FIRST_APPROVED_IS = 'Complete' AND (MMTM.WB_SLIP_NO like '" + txtSearchGrideView2.Text + "%' OR to_char(MMTM.ENTRY_DATE, 'dd/mm/yyyy')  like '" + txtSearchGrideView2.Text + "%' or MV.VEHICLE_NO like '" + txtSearchGrideView2.Text + "%' or MM.ITEM_NAME like '" + txtSearchGrideView2.Text + "%' or MIB.ITEM_BIN_NAME like '" + txtSearchGrideView2.Text + "%')  ORDER BY MMTM.WB_SLIP_NO desc, MMTM.ITEM_ID ASC  ";

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
            this.DisplayMatTransferApprove();
        }

        protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView2.PageIndex = e.NewPageIndex;
            DisplayMatTransferApprove();
            alert_box.Visible = false;
        }

        ///  End the Material Transfer Received


        ///  Start the Purchase

        public void PurchaseUpdatePendingApporve_Click(object sender, EventArgs e)
        {
            try
            {
                if (IS_EDIT_ACTIVE == "Enable")
                {

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    int userID = Convert.ToInt32(Session["USER_ID"]);

                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    foreach (GridViewRow gridRow in GridView3.Rows)
                    {
                        CheckBox chkRowIs = (gridRow.Cells[1].FindControl("IsShipmentCheck") as CheckBox);
                        if (chkRowIs.Checked)
                        {
                            int ItemIdOld = 0; double ItemWeightOld = 0.00;
                            string makeSQLPro = " select ITEM_ID, ITEM_WEIGHT from MF_PURCHASE_MASTER where PURCHASE_ID  = '" + gridRow.Cells[0].Text + "'  ";
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
                            string makeSQL = " select * from MF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "' ";
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
                            string update_inven_mas = "update  MF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
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

                            string update_ex_invoice = " update  MF_PURCHASE_MASTER set FIRST_APPROVED_IS =:IsConfirm,  FIRST_APPROVED_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , FIRST_APPROVED_USER_ID = :NoCuserID  where PURCHASE_ID =:NoWbItemID ";
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
                            DisplayMatPurchasePending();
                            DisplayMatPurchaseApprove();

                            alert_box.Visible = true;
                            alert_box.Controls.Add(new LiteralControl("Material Purchase Pending Successfully"));
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

        public void DisplayMatPurchasePending()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = "";
                string MonthYear = System.DateTime.Now.ToString("MM/yyyy");

                if (txtSearchGrideView3.Text == "")
                {
                    makeSQL = "  SELECT WPM.PURCHASE_ID, WPM.SLIP_NO, WPM.PARTY_ID, PP.PARTY_NAME, WC.CATEGORY_NAME, WPM.ITEM_ID, WI.ITEM_NAME, WPM.ITEM_WEIGHT,  WPM.ITEM_RATE, WPM.ITEM_AMOUNT, nvl(WPM.ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, WPM.VAT_AMOUNT, nvl(WPM.TOTAL_AMOUNT,0) AS TOTAL_AMOUNT, WPM.ENTRY_DATE, WPM.CREATE_DATE, WPM.UPDATE_DATE, WPM.IS_ACTIVE , WPC.IS_CHECK, WPM.IS_PRINT, TO_CHAR(WPM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE, WPM.CLAIM_NO, WPM.FIRST_APPROVED_IS FROM MF_PURCHASE_MASTER WPM LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = WPM.PARTY_ID LEFT JOIN MF_CATEGORY WC ON WC.CATEGORY_ID = WPM.CATEGORY_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WPM.ITEM_ID  LEFT JOIN WP_PURCHASE_CLAIM WPC ON  WPC.CLAIM_NO = WPM.CLAIM_NO  WHERE (WPM.FIRST_APPROVED_IS = 'Incomplete' OR WPM.FIRST_APPROVED_IS IS NULL) ORDER BY WPM.CREATE_DATE DESC  ";

                }
                else
                {
                    makeSQL = " SELECT WPM.PURCHASE_ID, WPM.SLIP_NO, WPM.PARTY_ID, PP.PARTY_NAME, WC.CATEGORY_NAME, WPM.ITEM_ID, WI.ITEM_NAME, WPM.ITEM_WEIGHT,  WPM.ITEM_RATE, WPM.ITEM_AMOUNT, nvl(WPM.ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, WPM.VAT_AMOUNT, nvl(WPM.TOTAL_AMOUNT,0) AS TOTAL_AMOUNT, WPM.ENTRY_DATE, WPM.CREATE_DATE, WPM.UPDATE_DATE, WPM.IS_ACTIVE , WPC.IS_CHECK, WPM.IS_PRINT, TO_CHAR(WPM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE, WPM.CLAIM_NO, WPM.FIRST_APPROVED_IS FROM MF_PURCHASE_MASTER WPM LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = WPM.PARTY_ID LEFT JOIN MF_CATEGORY WC ON WC.CATEGORY_ID = WPM.CATEGORY_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WPM.ITEM_ID LEFT JOIN WP_PURCHASE_CLAIM WPC ON  WPC.CLAIM_NO = WPM.CLAIM_NO WHERE  (WPM.FIRST_APPROVED_IS = 'Incomplete' OR WPM.FIRST_APPROVED_IS IS NULL) AND (WPM.SLIP_NO like '" + txtSearchGrideView3.Text + "%' or WPM.PARTY_ID like '" + txtSearchGrideView3.Text + "%' or PP.PARTY_NAME like '" + txtSearchGrideView3.Text + "%'  or WI.ITEM_NAME like '" + txtSearchGrideView3.Text + "%'  or WPM.ITEM_RATE like '" + txtSearchGrideView3.Text + "%'  or to_char(WPM.ENTRY_DATE, 'dd/mm/yyyy') like '" + txtSearchGrideView3.Text + "%' or to_char(WPM.ENTRY_DATE, 'mm/yyyy')  like '" + txtSearchGrideView3.Text + "%' or WPM.IS_ACTIVE like '" + txtSearchGrideView3.Text + "%')  ORDER BY WPM.SLIP_NO asc";

                     alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView3.DataSource = dt;
                GridView3.DataKeyNames = new string[] { "SLIP_NO" };
                GridView3.DataBind();

                conn.Close();
                // alert_box.Visible = false;
            }
        }


        protected void GridView3Search(object sender, EventArgs e)
        {
            this.DisplayMatPurchasePending();
        }

        protected void GridView3_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView3.PageIndex = e.NewPageIndex;
            DisplayMatPurchasePending();
            alert_box.Visible = false;
        }


        public void PurchaseUpdateCompleteApporve_Click(object sender, EventArgs e)
        {
            //  try
            //    {
            if (IS_EDIT_ACTIVE == "Enable")
            {

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                foreach (GridViewRow gridRow in GridView4.Rows)
                {
                    CheckBox chkRowIs = (gridRow.Cells[1].FindControl("IsCompleteApporve") as CheckBox);
                    if (chkRowIs.Checked)
                    {
                        int ItemIdOld = 0; double ItemWeightOld = 0.00;
                        string makeSQLPro = " select ITEM_ID, ITEM_WEIGHT from MF_PURCHASE_MASTER where PURCHASE_ID  = '" + gridRow.Cells[0].Text + "'  ";
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
                        double InitialStock = 0.00, StockInWet = 0.00, StockInWetNew = 0.0, StockOutWet = 0.00, FinalStock = 0.00, FinalStockNew = 0.00;

                        // check inventory RM
                        string makeSQL = " select * from MF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "' ";
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
                            StockInWetNew = StockInWet - ItemWeightOld;
                            FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;

                            // update inventory FG (minus old data)
                            string update_inven_mas = "update  MF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
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

                            string update_ex_invoice = " update  MF_PURCHASE_MASTER set FIRST_APPROVED_IS =:IsConfirm,  FIRST_APPROVED_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , FIRST_APPROVED_USER_ID = :NoCuserID  where PURCHASE_ID =:NoWbItemID ";
                            cmdi = new OracleCommand(update_ex_invoice, conn);

                            OracleParameter[] objPrm = new OracleParameter[5];
                            objPrm[0] = cmdi.Parameters.Add("NoWbItemID", gridRow.Cells[0].Text);
                            objPrm[1] = cmdi.Parameters.Add("IsConfirm", "Incomplete");
                            objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                            objPrm[3] = cmdi.Parameters.Add("NoCuserID", userID);

                            cmdi.ExecuteNonQuery();
                            cmdi.Parameters.Clear();
                            cmdi.Dispose();

                            conn.Close();
                            DisplayMatPurchasePending();
                            DisplayMatPurchaseApprove();

                            alert_box.Visible = true;
                            alert_box.Controls.Add(new LiteralControl("Material Purchase Approved Successfully"));
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
            //       }
            //    catch
            //     {
            //       Response.Redirect("~/ParameterError.aspx");
            //  }

        }

        public void DisplayMatPurchaseApprove()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = "";
                string MonthYear = System.DateTime.Now.ToString("MM/yyyy");

                if (txtSearchGrideView4.Text == "")
                {
                    makeSQL = "  SELECT WPM.PURCHASE_ID, WPM.SLIP_NO, WPM.PARTY_ID, PP.PARTY_NAME, WC.CATEGORY_NAME, WPM.ITEM_ID, WI.ITEM_NAME, WPM.ITEM_WEIGHT,  WPM.ITEM_RATE, WPM.ITEM_AMOUNT, nvl(WPM.ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, WPM.VAT_AMOUNT, nvl(WPM.TOTAL_AMOUNT,0) AS TOTAL_AMOUNT, WPM.ENTRY_DATE, WPM.CREATE_DATE, WPM.UPDATE_DATE, WPM.IS_ACTIVE , WPC.IS_CHECK, WPM.IS_PRINT, TO_CHAR(WPM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE, WPM.CLAIM_NO, WPM.FIRST_APPROVED_IS FROM MF_PURCHASE_MASTER WPM LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = WPM.PARTY_ID LEFT JOIN MF_CATEGORY WC ON WC.CATEGORY_ID = WPM.CATEGORY_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WPM.ITEM_ID  LEFT JOIN WP_PURCHASE_CLAIM WPC ON  WPC.CLAIM_NO = WPM.CLAIM_NO  WHERE WPM.FIRST_APPROVED_IS = 'Complete' ORDER BY WPM.CREATE_DATE DESC  ";

                }
                else
                {
                    makeSQL = " SELECT WPM.PURCHASE_ID, WPM.SLIP_NO, WPM.PARTY_ID, PP.PARTY_NAME, WC.CATEGORY_NAME, WPM.ITEM_ID, WI.ITEM_NAME, WPM.ITEM_WEIGHT,  WPM.ITEM_RATE, WPM.ITEM_AMOUNT, nvl(WPM.ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, WPM.VAT_AMOUNT, nvl(WPM.TOTAL_AMOUNT,0) AS TOTAL_AMOUNT, WPM.ENTRY_DATE, WPM.CREATE_DATE, WPM.UPDATE_DATE, WPM.IS_ACTIVE , WPC.IS_CHECK, WPM.IS_PRINT, TO_CHAR(WPM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE, WPM.CLAIM_NO, WPM.FIRST_APPROVED_IS FROM MF_PURCHASE_MASTER WPM LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = WPM.PARTY_ID LEFT JOIN MF_CATEGORY WC ON WC.CATEGORY_ID = WPM.CATEGORY_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WPM.ITEM_ID LEFT JOIN WP_PURCHASE_CLAIM WPC ON  WPC.CLAIM_NO = WPM.CLAIM_NO WHERE  WPM.FIRST_APPROVED_IS = 'Complete' AND (WPM.SLIP_NO like '" + txtSearchGrideView3.Text + "%' or WPM.PARTY_ID like '" + txtSearchGrideView3.Text + "%' or PP.PARTY_NAME like '" + txtSearchGrideView3.Text + "%'  or WI.ITEM_NAME like '" + txtSearchGrideView3.Text + "%'  or WPM.ITEM_RATE like '" + txtSearchGrideView3.Text + "%'  or to_char(WPM.ENTRY_DATE, 'dd/mm/yyyy') like '" + txtSearchGrideView3.Text + "%' or to_char(WPM.ENTRY_DATE, 'mm/yyyy')  like '" + txtSearchGrideView3.Text + "%' or WPM.IS_ACTIVE like '" + txtSearchGrideView3.Text + "%')  ORDER BY WPM.SLIP_NO asc";

                    alert_box.Visible = false;
                }


                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView4.DataSource = dt;
                GridView4.DataKeyNames = new string[] { "SLIP_NO" };
                GridView4.DataBind();

                conn.Close();
                // alert_box.Visible = false;
            }
        }


        protected void GridView4Search(object sender, EventArgs e)
        {
            this.DisplayMatPurchaseApprove();
        }

        protected void GridView4_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView4.PageIndex = e.NewPageIndex;
            DisplayMatPurchaseApprove();
            alert_box.Visible = false;
        }

        ///  End the Purchase
        ///  

        ///  Start the Material Receiving (Import)

        public void MatRecevingUpdatePendingApporve_Click(object sender, EventArgs e)
        {
            try
            {
                if (IS_EDIT_ACTIVE == "Enable")
                {

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    int userID = Convert.ToInt32(Session["USER_ID"]);

                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    foreach (GridViewRow gridRow in GridView5.Rows)
                    {
                        CheckBox chkRowIs = (gridRow.Cells[1].FindControl("IsShipmentCheck") as CheckBox);
                        if (chkRowIs.Checked)
                        {
                            int ItemIdOld = 0; double ItemWeightOld = 0.00;
                            string makeSQLPro = " select ITEM_ID, ITEM_WEIGHT from MF_PURCHASE_IMPROT where PURCHASE_IMPROT_ID  = '" + gridRow.Cells[0].Text + "'  ";
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
                            string makeSQL = " select * from MF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "' ";
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
                            string update_inven_mas = "update  MF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
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

                            string update_ex_invoice = " update  MF_PURCHASE_IMPROT set FIRST_APPROVED_IS =:IsConfirm,  FIRST_APPROVED_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , FIRST_APPROVED_USER_ID = :NoCuserID  where PURCHASE_IMPROT_ID =:NoWbItemID ";
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
                            DisplayMatRecevingPending();
                            DisplayMatRecevingApprove();

                            alert_box.Visible = true;
                            alert_box.Controls.Add(new LiteralControl("Material Received (Import) Pending Successfully"));
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

        public void DisplayMatRecevingPending()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = "";
                string MonthYear = System.DateTime.Now.ToString("MM/yyyy");

                if (txtSearchGrideView5.Text == "")
                {
                    makeSQL = " SELECT MPI.PURCHASE_IMPROT_ID, MPI.WB_SLIP_NO, MPI.ENTRY_DATE, MPI.CONTAINER_NO, NCS.CONTAINER_SIZE, PP.PARTY_NAME,  PI.ITEM_NAME, MPI.ITEM_WEIGHT_WB,  MPI.ITEM_WEIGHT, MIB.ITEM_BIN_NAME, MIB.CAPACITY_WEIGHT, NPL.PACKING_NAME, MMPH.NUMBER_OF_PACK, MMPH.PACK_PER_WEIGHT,  MMPH.TOTAL_WEIGHT, MPI.IS_ACTIVE, MPI.CREATE_DATE, MPI.UPDATE_DATE, MPI.IS_PRINT, MPI.PRINT_DATE, MPI.FIRST_APPROVED_IS FROM MF_PURCHASE_IMPROT MPI LEFT JOIN MF_MATERIAL_PACKING_HISTORY MMPH ON MMPH.WB_SLIP_NO = MPI.WB_SLIP_NO LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = MMPH.PACKING_ID LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = MPI.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = MPI.ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = MPI.CONTAINER_SIZE_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MPI.ITEM_BIN_ID WHERE (MPI.FIRST_APPROVED_IS = 'Incomplete' OR MPI.FIRST_APPROVED_IS IS NULL) ORDER BY MPI.WB_SLIP_NO desc, MPI.ITEM_ID ASC  ";

                }
                else
                {
                    makeSQL = " SELECT MPI.PURCHASE_IMPROT_ID, MPI.WB_SLIP_NO, MPI.ENTRY_DATE, MPI.CONTAINER_NO, NCS.CONTAINER_SIZE, PP.PARTY_NAME,  PI.ITEM_NAME, MPI.ITEM_WEIGHT_WB,  MPI.ITEM_WEIGHT, MIB.ITEM_BIN_NAME, MIB.CAPACITY_WEIGHT,  NPL.PACKING_NAME, MMPH.NUMBER_OF_PACK, MMPH.PACK_PER_WEIGHT,  MMPH.TOTAL_WEIGHT, MPI.IS_ACTIVE, MPI.CREATE_DATE, MPI.UPDATE_DATE, MPI.IS_PRINT,  MPI.PRINT_DATE,  MPI.FIRST_APPROVED_IS FROM MF_PURCHASE_IMPROT MPI LEFT JOIN MF_MATERIAL_PACKING_HISTORY MMPH ON MMPH.WB_SLIP_NO = MPI.WB_SLIP_NO LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = MMPH.PACKING_ID LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = MPI.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = MPI.ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = MPI.CONTAINER_SIZE_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MPI.ITEM_BIN_ID  WHERE (MPI.FIRST_APPROVED_IS = 'Incomplete' OR MPI.FIRST_APPROVED_IS IS NULL) AND (MPI.WB_SLIP_NO like '" + txtSearchGrideView5.Text + "%' or MPI.CONTAINER_NO like '" + txtSearchGrideView5.Text + "%' or PP.PARTY_NAME like '" + txtSearchGrideView5.Text + "%' or PI.ITEM_NAME like '" + txtSearchGrideView5.Text + "%' or MIB.ITEM_BIN_NAME like '" + txtSearchGrideView5.Text + "%' or to_char(MPI.ENTRY_DATE, 'dd/mm/yyyy')  like '" + txtSearchGrideView5.Text + "%' or MPI.IS_ACTIVE like '" + txtSearchGrideView5.Text + "%') ORDER BY MPI.WB_SLIP_NO desc, MPI.ITEM_ID asc ";

                    alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView5.DataSource = dt;
                GridView5.DataKeyNames = new string[] { "WB_SLIP_NO" };
                GridView5.DataBind();

                conn.Close();
                // alert_box.Visible = false;
            }
        }


        protected void GridView5Search(object sender, EventArgs e)
        {
            this.DisplayMatRecevingPending();
        }

        protected void GridView5_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView5.PageIndex = e.NewPageIndex;
            DisplayMatRecevingPending();
            alert_box.Visible = false;
        }


        public void MatRecevingUpdateCompleteApporve_Click(object sender, EventArgs e)
        {
            //  try
            //    {
            if (IS_EDIT_ACTIVE == "Enable")
            {

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                foreach (GridViewRow gridRow in GridView6.Rows)
                {
                    CheckBox chkRowIs = (gridRow.Cells[1].FindControl("IsCompleteApporve") as CheckBox);
                    if (chkRowIs.Checked)
                    {
                        int ItemIdOld = 0; double ItemWeightOld = 0.00;
                        string makeSQLPro = " select ITEM_ID, ITEM_WEIGHT from MF_PURCHASE_IMPROT where PURCHASE_IMPROT_ID  = '" + gridRow.Cells[0].Text + "'  ";
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
                        double InitialStock = 0.00, StockInWet = 0.00, StockInWetNew = 0.0, StockOutWet = 0.00, FinalStock = 0.00, FinalStockNew = 0.00;

                        // check inventory RM
                        string makeSQL = " select * from MF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "' ";
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
                            StockInWetNew = StockInWet - ItemWeightOld;
                            FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;

                            // update inventory FG (minus old data)
                            string update_inven_mas = "update  MF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
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

                            string update_ex_invoice = " update  MF_PURCHASE_IMPROT set FIRST_APPROVED_IS =:IsConfirm,  FIRST_APPROVED_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , FIRST_APPROVED_USER_ID = :NoCuserID  where PURCHASE_IMPROT_ID =:NoWbItemID ";
                            cmdi = new OracleCommand(update_ex_invoice, conn);

                            OracleParameter[] objPrm = new OracleParameter[5];
                            objPrm[0] = cmdi.Parameters.Add("NoWbItemID", gridRow.Cells[0].Text);
                            objPrm[1] = cmdi.Parameters.Add("IsConfirm", "Incomplete");
                            objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                            objPrm[3] = cmdi.Parameters.Add("NoCuserID", userID);

                            cmdi.ExecuteNonQuery();
                            cmdi.Parameters.Clear();
                            cmdi.Dispose();

                            conn.Close();
                            DisplayMatRecevingPending();
                            DisplayMatRecevingApprove();

                            alert_box.Visible = true;
                            alert_box.Controls.Add(new LiteralControl("Material Received (Import) Approved Successfully"));
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
            //       }
            //    catch
            //     {
            //       Response.Redirect("~/ParameterError.aspx");
            //  }

        }

        public void DisplayMatRecevingApprove()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = "";
                string MonthYear = System.DateTime.Now.ToString("MM/yyyy");

                if (txtSearchGrideView6.Text == "")
                {
                    makeSQL = " SELECT MPI.PURCHASE_IMPROT_ID, MPI.WB_SLIP_NO, MPI.ENTRY_DATE, MPI.CONTAINER_NO, NCS.CONTAINER_SIZE, PP.PARTY_NAME,  PI.ITEM_NAME, MPI.ITEM_WEIGHT_WB,  MPI.ITEM_WEIGHT, MIB.ITEM_BIN_NAME, MIB.CAPACITY_WEIGHT, NPL.PACKING_NAME, MMPH.NUMBER_OF_PACK, MMPH.PACK_PER_WEIGHT,  MMPH.TOTAL_WEIGHT, MPI.IS_ACTIVE, MPI.CREATE_DATE, MPI.UPDATE_DATE, MPI.IS_PRINT, MPI.PRINT_DATE, MPI.FIRST_APPROVED_IS FROM MF_PURCHASE_IMPROT MPI LEFT JOIN MF_MATERIAL_PACKING_HISTORY MMPH ON MMPH.WB_SLIP_NO = MPI.WB_SLIP_NO LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = MMPH.PACKING_ID LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = MPI.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = MPI.ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = MPI.CONTAINER_SIZE_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MPI.ITEM_BIN_ID WHERE MPI.FIRST_APPROVED_IS = 'Complete' ORDER BY MPI.WB_SLIP_NO desc, MPI.ITEM_ID ASC  ";

                }
                else
                {
                    makeSQL = " SELECT MPI.PURCHASE_IMPROT_ID, MPI.WB_SLIP_NO, MPI.ENTRY_DATE, MPI.CONTAINER_NO, NCS.CONTAINER_SIZE, PP.PARTY_NAME,  PI.ITEM_NAME, MPI.ITEM_WEIGHT_WB,  MPI.ITEM_WEIGHT, MIB.ITEM_BIN_NAME, MIB.CAPACITY_WEIGHT,  NPL.PACKING_NAME, MMPH.NUMBER_OF_PACK, MMPH.PACK_PER_WEIGHT,  MMPH.TOTAL_WEIGHT, MPI.IS_ACTIVE, MPI.CREATE_DATE, MPI.UPDATE_DATE, MPI.IS_PRINT,  MPI.PRINT_DATE,  MPI.FIRST_APPROVED_IS FROM MF_PURCHASE_IMPROT MPI LEFT JOIN MF_MATERIAL_PACKING_HISTORY MMPH ON MMPH.WB_SLIP_NO = MPI.WB_SLIP_NO LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = MMPH.PACKING_ID LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = MPI.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = MPI.ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = MPI.CONTAINER_SIZE_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MPI.ITEM_BIN_ID  WHERE MPI.FIRST_APPROVED_IS = 'Complete'  AND (MPI.WB_SLIP_NO like '" + txtSearchGrideView6.Text + "%' or MPI.CONTAINER_NO like '" + txtSearchGrideView6.Text + "%' or PP.PARTY_NAME like '" + txtSearchGrideView6.Text + "%' or PI.ITEM_NAME like '" + txtSearchGrideView6.Text + "%' or MIB.ITEM_BIN_NAME like '" + txtSearchGrideView6.Text + "%' or to_char(MPI.ENTRY_DATE, 'dd/mm/yyyy')  like '" + txtSearchGrideView6.Text + "%' or MPI.IS_ACTIVE like '" + txtSearchGrideView6.Text + "%') ORDER BY MPI.WB_SLIP_NO desc, MPI.ITEM_ID asc ";

                    alert_box.Visible = false;
                }


                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView6.DataSource = dt;
                GridView6.DataKeyNames = new string[] { "WB_SLIP_NO" };
                GridView6.DataBind();

                conn.Close();
                // alert_box.Visible = false;
            }
        }


        protected void GridView6Search(object sender, EventArgs e)
        {
            this.DisplayMatRecevingApprove();
        }

        protected void GridView6_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView6.PageIndex = e.NewPageIndex;
            DisplayMatRecevingApprove();
            alert_box.Visible = false;
        }

        ///  End the Material Receiving (Import)


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