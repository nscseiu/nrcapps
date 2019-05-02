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
using System.Globalization;
using System.Text;

namespace NRCAPPS.MS
{
    public partial class MsInventoryRm : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdl;
        OracleDataAdapter oradata;
        DataTable dt;
        int RowCount;

        string IS_PAGE_ACTIVE   = "";
        string IS_ADD_ACTIVE    = "";
        string IS_EDIT_ACTIVE   = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE   = "";  
 
        public bool IsLoad { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_NAME"] != null)
            {
                string requestedFile = Path.GetFileName(Request.Path);  
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = " SELECT  NUPP.IS_PAGE_ACTIVE, NUPP.IS_ADD_ACTIVE, NUPP.IS_EDIT_ACTIVE, NUPP.IS_DELETE_ACTIVE, NUPP.IS_VIEW_ACTIVE FROM NRC_USER_PAGE_PERMISSION NUPP LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NUPP.USER_PAGE_ID  WHERE NUPP.USER_ID = '" + Session["USER_ID"] + "' AND NUP.IS_ACTIVE = 'Enable' AND NUP.PAGE_URL = '" + requestedFile + "' ";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                 
                for (int i = 0; i < RowCount; i++)
                {
                    IS_PAGE_ACTIVE   = dt.Rows[i]["IS_PAGE_ACTIVE"].ToString();
                    IS_ADD_ACTIVE    = dt.Rows[i]["IS_ADD_ACTIVE"].ToString();
                    IS_EDIT_ACTIVE   = dt.Rows[i]["IS_EDIT_ACTIVE"].ToString();
                    IS_DELETE_ACTIVE = dt.Rows[i]["IS_DELETE_ACTIVE"].ToString();
                    IS_VIEW_ACTIVE   = dt.Rows[i]["IS_VIEW_ACTIVE"].ToString();  
                }

                if (IS_PAGE_ACTIVE == "Enable")
                {
                     
                    if (!IsPostBack)
                    {
                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeDropDownItemSQL = " SELECT ITEM_ID, ITEM_CODE || ' : ' || ITEM_NAME AS ITEM_NAME FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        dsi = ExecuteBySqlStringType(makeDropDownItemSQL);
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
                        DropDownItemID1.Items.Insert(0, new ListItem("Select  Item", "0"));

                        DisplayInventory();
                        Display();
                        DisplayRmHistory();

                        alert_box.Visible = false;

                    } 
                    IsLoad = false;
                }
                else {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
                   
                 
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        

        protected void linkSelectClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             LinkButton btn = (LinkButton)sender;
             Session["user_page_data_id"] = btn.CommandArgument;
             int USER_DATA_ID = Convert.ToInt32(Session["user_page_data_id"]); 
             

             DataTable dtUserTypeID = new DataTable();
             DataSet ds = new DataSet();
             string makeSQL = " select *  from MS_RM_STOCK_INVENTORY_MASTER where RM_INVENTORY_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextInventoryRmID.Text = dt.Rows[i]["RM_INVENTORY_ID"].ToString();
                 DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString(); 
                 TextInitialStock.Text = decimal.Parse(dt.Rows[i]["INITIAL_STOCK_WT"].ToString()).ToString("0.00"); 
                 TextStockIn.Text = decimal.Parse(dt.Rows[i]["STOCK_IN_WT"].ToString()).ToString("0.00"); 
                 TextStockOut.Text = decimal.Parse(dt.Rows[i]["STOCK_OUT_WT"].ToString()).ToString("0.00");
                 TextFinalStock.Text = decimal.Parse(dt.Rows[i]["FINAL_STOCK_WT"].ToString()).ToString("0.00");
                 TextItemAvgRate.Text = decimal.Parse(dt.Rows[i]["ITEM_END_AMOUNT"].ToString()).ToString("0.00");             
             } 
             
             conn.Close();
            DisplayInventory();
            Display(); 
             alert_box.Visible = false;  

        }

        protected void linkSelectRmHisClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             LinkButton btn = (LinkButton)sender;
             Session["user_page_data_id"] = btn.CommandArgument;
             int USER_DATA_ID = Convert.ToInt32(Session["user_page_data_id"]); 
             

             DataTable dtUserTypeID = new DataTable();
             DataSet ds = new DataSet();
             string makeSQL = " select * from MS_RM_STOCK_INVENTORY_HISTORY where IN_RM_HIS_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextInventoryRmHisID.Text = dt.Rows[i]["IN_RM_HIS_ID"].ToString();
                 DropDownItemID1.Text = dt.Rows[i]["ITEM_ID"].ToString(); 
                 TextInitialStockHis.Text = decimal.Parse(dt.Rows[i]["INITIAL_STOCK_WT"].ToString()).ToString("0.00"); 
                 TextStockInHis.Text = decimal.Parse(dt.Rows[i]["STOCK_IN_WT"].ToString()).ToString("0.00"); 
                 TextStockOutHis.Text = decimal.Parse(dt.Rows[i]["STOCK_OUT_WT"].ToString()).ToString("0.00");
                 TextFinalStockHis.Text = decimal.Parse(dt.Rows[i]["FINAL_STOCK_WT"].ToString()).ToString("0.00");
                 TextItemAvgRateHis.Text = decimal.Parse(dt.Rows[i]["ITEM_END_AMOUNT"].ToString()).ToString("0.00");             
             } 
             
             conn.Close();
            DisplayInventory();
            Display();
            DisplayRmHistory();
            alert_box.Visible = false;  

        } 

        public void Display()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                DataTable dtUserTypeID = new DataTable();
                DataSet ds = new DataSet();

                string   makeSQL = " select  PRSIM.RM_INVENTORY_ID, PI.ITEM_CODE || ' : ' || PI.ITEM_NAME AS ITEM_NAME, PRSIM.INITIAL_STOCK_WT, PRSIM.STOCK_IN_WT, PRSIM.STOCK_OUT_WT, PRSIM.FINAL_STOCK_WT, PRSIM.ITEM_END_AMOUNT  from MS_RM_STOCK_INVENTORY_MASTER PRSIM LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PRSIM.ITEM_ID WHERE  nvl(PRSIM.INITIAL_STOCK_WT,0)>0 OR nvl(PRSIM.STOCK_IN_WT,0)>0 OR nvl(PRSIM.STOCK_OUT_WT,0)>0 OR nvl(PRSIM.FINAL_STOCK_WT,0)>0  ORDER BY PI.ITEM_CODE asc";
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView5D.DataSource = dt;
                GridView5D.DataKeyNames = new string[] { "RM_INVENTORY_ID" }; 
                GridView5D.DataBind();
                conn.Close();
                //alert_box.Visible = false;
            }
            else
            {
             //   Response.Redirect("~/PagePermissionError.aspx");
            }
        }
        
     

        public void BtnDataCheckMs_Click(object sender, EventArgs e)
        {
       //   try {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);

                int userID = Convert.ToInt32(Session["USER_ID"]);
                string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string MakeAsOnDate = TextMonthYear1.Text;
                string[] MakeAsOnDateSplit = MakeAsOnDate.Split('-');
                String AsOnDateTemp = MakeAsOnDateSplit[0].Replace("/", "-");
                DateTime AsOnDateNewD = DateTime.ParseExact(AsOnDateTemp, "MM-yyyy", CultureInfo.InvariantCulture);
                string AsOnDateNew = AsOnDateNewD.ToString("dd-MM-yyyy");

                DateTime curDate = AsOnDateNewD;
                DateTime startDate = curDate.AddMonths(-1);
                DateTime LastDateTemp = curDate.AddDays(-(curDate.Day));
                string LastDate = LastDateTemp.ToString("dd-MM-yyyy");
                string LastMonth = startDate.ToString("MM-yyyy");
                string CurrentMonth = AsOnDateNewD.ToString("MM-yyyy");
                DateTime LastDateCurrentMonthTemp = AsOnDateNewD.AddMonths(1).AddDays(-1);
                string LastDateCurrentMonth = LastDateCurrentMonthTemp.ToString("dd-MM-yyyy");

                OracleCommand cmd = new OracleCommand("PRO_MS_DATA_CHECK_INVENTORY", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("TextLastDate", OracleType.VarChar)).Value = LastDate;
                cmd.Parameters.Add(new OracleParameter("TextLastMonth", OracleType.VarChar)).Value = LastMonth;
                cmd.Parameters.Add(new OracleParameter("TextCurrentMonth", OracleType.VarChar)).Value = CurrentMonth;
                cmd.Parameters.Add(new OracleParameter("TextLastDateCurrentMonth", OracleType.VarChar)).Value = LastDateCurrentMonth;

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Waste Paper Chcek Process Successfully " + LastDateCurrentMonth));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                clearText();
                DisplayInventory();
                Display();
                DisplayRmHistory();

            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
         //   }
       //  catch
        //   {
         //     Response.Redirect("~/ParameterError.aspx");
       //  } 
        }

        public void DisplayRmHistory()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                DataTable dtUserTypeID = new DataTable();
                DataSet ds = new DataSet();

                string makeSQL = "", c_date = System.DateTime.Now.ToString("MM-yyyy");
                if (txtSearchHistory.Text == "")
                {
                    makeSQL = " select  * from MS_RM_STOCK_INVENTORY_HISTORY ORDER BY CREATE_DATE desc, ITEM_ID asc"; //where TO_CHAR(TO_DATE(CREATE_DATE), 'MM-yyyy') =  '" + c_date + "' 
                }
                else
                {
                    makeSQL = " select  * from MS_RM_STOCK_INVENTORY_HISTORY where TO_CHAR(TO_DATE(CREATE_DATE), 'dd/mm/yyyy') like '" + txtSearchHistory.Text + "%' or ITEM_NAME like '" + txtSearchHistory.Text + "%'  ORDER BY CREATE_DATE desc, ITEM_ID asc";

                    alert_box.Visible = false;
                }
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataKeyNames = new string[] { "IN_RM_HIS_ID" };

                GridView2.DataBind();
                conn.Close();
                //alert_box.Visible = false;
            }
            else
            {
             //   Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        protected void GridViewSearchHistory(object sender, EventArgs e)
        {
            this.DisplayRmHistory();
        }

        protected void GridViewHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView2.PageIndex = e.NewPageIndex;
            DisplayRmHistory();
            alert_box.Visible = false;
        }

        public void DisplayInventory()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                //Building an HTML string.
                StringBuilder html = new StringBuilder();
                string AsOnDate = "";
                if (EntryDate.Text == "") {  AsOnDate = System.DateTime.Now.ToString("dd/MM/yyyy"); } else {  AsOnDate = EntryDate.Text; }
               

                string MakeAsOnDate = AsOnDate;
                string[] MakeAsOnDateSplit = MakeAsOnDate.Split('-');
                String AsOnDateTemp = MakeAsOnDateSplit[0].Replace("/", "-");
                DateTime AsOnDateNewD = DateTime.ParseExact(AsOnDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string AsOnDateNew = AsOnDateNewD.ToString("dd-MM-yyyy");

                DateTime curDate = AsOnDateNewD;
                DateTime startDate = curDate.AddMonths(-1);
                DateTime LastDateTemp = curDate.AddDays(-(curDate.Day));
                string LastDate = LastDateTemp.ToString("dd-MM-yyyy");
                string LastMonth = startDate.ToString("MM-yyyy");
                string CurrentMonth = AsOnDateNewD.ToString("MM-yyyy");
                string CurrentMonthTitle = AsOnDateNewD.ToString("MMMM-yyyy");

                string sqlString = "   SELECT WI.ITEM_ID, WI.ITEM_CODE || ' : ' || WI.ITEM_NAME AS ITEM_NAME, nvl(BEGWRSIH.FINAL_STOCK_WT,0) AS BEG_FSTOCK_WT, nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) AS BEG_AMT, ROUND(nvl(nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) / nullif(BEGWRSIH.FINAL_STOCK_WT, 0), 3)*1000, 0) AS BEG_AVG_RATE, nvl(WPM.ITEM_WEIGHT, 0) AS PURCHASE_WT, nvl(WPM.ITEM_AMOUNT, 0) AS PURCHASE_AMT, nvl(ROUND((nvl(WPM.ITEM_AMOUNT, 0) / nullif(nvl(WPM.ITEM_WEIGHT, 0), 0)) * 1000, 2), 0) AS PURCHASE_AVG_RATE, ROUND(nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 2) AS GAR_EST_WT, ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 2) AS PURCHASE_NET_WT, nvl(ROUND(nvl(WPM.ITEM_AMOUNT, 0) / nullif(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 0), 2) * 1000, 0) AS PURCHASE_NET_AVG_RATE, nvl(BEGWRSIH.FINAL_STOCK_WT, 0)+ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 2) AS PURCHASE_NET_GAR_EST_WT, nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) +nvl(WPM.ITEM_AMOUNT, 0) AS PURCHASE_BEG_AMT, ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0), 4)*1000 AS PURCHASE_BEG_AVG_RATE, nvl(WMTI.ITEM_WEIGHT, 0) AS MAT_ISSUED_WT, nvl(WMTR.ITEM_WEIGHT, 0) AS MAT_RECEVIED_WT, nvl(WMTRM.ITEM_WEIGHT, 0) AS MAT_TRANSFER_DEDUC_WT, ROUND((((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0))) * nvl(WMTRM.ITEM_WEIGHT, 0), 2) AS MAT_TRANSFER_DEDUC_AMT, nvl(WMTT.ITEM_WEIGHT, 0) AS MAT_TRANSFER_WT, nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0) AS PURCHASE_TRANS_AVG_RATE, ROUND((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000, 2) AS MAT_TRANSFER_AMT, nvl(BEGWRSIH.FINAL_STOCK_WT, 0)+ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0)))) AS SALES_AVAIL_WT, ROUND(nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0) + ROUND((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000, 2) - (((((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0))) * nvl(WMTRM.ITEM_WEIGHT, 0))), 2) AS SALES_AVAIL_AMT, ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0) + ((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000) - (((((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0))) * nvl(WMTRM.ITEM_WEIGHT, 0)))) / nullif(nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0)) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0)))), 0) * 1000, 2) AS SALES_AVAIL_AVG_RATE, nvl(WEWCI.ITEM_WEIGHT, 0) AS SALES_OVERSEAS_WT, nvl(WSM.ITEM_WEIGHT, 0) AS SALES_LOCAL_WT, nvl(WSIDM.ITEM_WEIGHT, 0) AS SALES_INTER_DIV_WT, (nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))))) -(nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)) AS END_FSTOCK_WT, ROUND(((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))))) - (nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0))) * (((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0) + ((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000) - (((((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0))) * nvl(WMTRM.ITEM_WEIGHT, 0)))) / nullif(nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0)) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0)))), 0))), 2) AS END_AMT, ROUND((((((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) - (nvl(WMTRM.ITEM_WEIGHT, 0) + nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))) - (nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0))) * (ROUND(((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0) + ((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000)) - ((ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0), 4)) * nvl(WMTRM.ITEM_WEIGHT, 0))) / nullif(nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0)))), 0) * 1000, 2))))) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) - (nvl(WMTRM.ITEM_WEIGHT, 0) + nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))) - (nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)), 0), 2) AS END_AVG_RATE, nvl(WEWCIT.ITEM_WEIGHT, 0) AS ITEM_WEIGHT_TRANSIT, ((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))))) -(nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)))-nvl(WEWCIT.ITEM_WEIGHT, 0) AS END_AS_PER_BOOK FROM MF_ITEM WI LEFT JOIN(SELECT ITEM_ID, FINAL_STOCK_WT, ITEM_END_AMOUNT FROM MS_RM_STOCK_INVENTORY_HISTORY WHERE TO_CHAR(TO_DATE(CREATE_DATE), 'dd-mm-YYYY') = '" + LastDate + "' ) BEGWRSIH ON WI.ITEM_ID = BEGWRSIH.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT, sum(nvl(ITEM_AMOUNT, 0) + nvl(VAT_AMOUNT, 0)) AS ITEM_AMOUNT FROM MS_PURCHASE_MASTER  WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WPM ON WI.ITEM_ID = WPM.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT FROM MS_MATERIAL_TRANSACTION  WHERE TRANSACTION_FOR_ID = 1 AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WMTI ON WI.ITEM_ID = WMTI.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT FROM MS_MATERIAL_TRANSACTION  WHERE TRANSACTION_FOR_ID = 2 AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WMTR ON WI.ITEM_ID = WMTR.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT FROM MS_MATERIAL_TRANSACTION  WHERE TRANSACTION_FOR_ID = 3 AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WMTRM ON WI.ITEM_ID = WMTRM.ITEM_ID LEFT JOIN(SELECT WMT.ITEM_ID, WMT.ITEM_TRANSFER_ID, sum(nvl(WMT.ITEM_WEIGHT,0)) AS ITEM_WEIGHT, ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0), 4)*1000 AS PURCHASE_TRANS_AVG_RATE FROM MS_MATERIAL_TRANSACTION  WMT LEFT JOIN(SELECT ITEM_ID, FINAL_STOCK_WT, ITEM_END_AMOUNT FROM MS_RM_STOCK_INVENTORY_HISTORY WHERE TO_CHAR(TO_DATE(CREATE_DATE), 'dd-mm-YYYY') = '" + LastDate + "' ) BEGWRSIH ON WMT.ITEM_ID = BEGWRSIH.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT, sum(nvl(ITEM_AMOUNT, 0) + nvl(VAT_AMOUNT, 0)) AS ITEM_AMOUNT FROM MS_PURCHASE_MASTER  WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WPM ON WMT.ITEM_ID = WPM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, nvl(sum(nvl(ACTUAL_GAR_WEIGHT, 0)), 0) AS ACTUAL_GAR_WEIGHT FROM MS_ACTUAL_GARBAGE WHERE TO_CHAR(TO_DATE(MONTH_YEAR), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) MAGAR ON WMT.ITEM_ID = MAGAR.ITEM_ID WHERE WMT.TRANSACTION_FOR_ID = 3 AND TO_CHAR(TO_DATE(WMT.ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY WMT.ITEM_ID, WMT.ITEM_TRANSFER_ID, ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0), 4) * 1000) WMTT ON WI.ITEM_ID = WMTT.ITEM_TRANSFER_ID LEFT JOIN(SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM MS_EXPORT_WBSLIP_CON_ITEM WHERE  IS_INVENTORY_STATUS = 'Complete' AND TO_CHAR(TO_DATE(IS_SHIPMENT_COMPLETE_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WEWCI ON WI.ITEM_ID = WEWCI.ITEM_ID LEFT JOIN(SELECT WEWCI.ITEM_ID, sum(WEWCI.ITEM_WEIGHT) AS ITEM_WEIGHT FROM MS_EXPORT_WBSLIP_CON_ITEM WEWCI LEFT JOIN MS_EXPORT_WBSLIP_CON WEWC ON WEWC.WB_SLIP_NO = WEWCI.WB_SLIP_NO WHERE  WEWCI.IS_INVENTORY_STATUS = 'Transit' AND (TO_CHAR(TO_DATE(SYSDATE), 'mm-YYYY')  = '" + CurrentMonth + "' OR TO_CHAR(TO_DATE(WEWC.DISPATCH_DATE), 'mm-YYYY') <=  '" + CurrentMonth + "') GROUP BY WEWCI.ITEM_ID) WEWCIT ON WI.ITEM_ID = WEWCIT.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT, SUM(nvl(ITEM_AMOUNT, 0) + nvl(VAT_AMOUNT, 0)) AS TOTAL_AMOUNT  FROM MS_SALES_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WSM ON WI.ITEM_ID = WSM.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT  FROM MS_SALES_INTER_DIV_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WSIDM ON WI.ITEM_ID = WSIDM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, nvl(sum(nvl(ACTUAL_GAR_WEIGHT, 0)), 0) AS ACTUAL_GAR_WEIGHT FROM MS_ACTUAL_GARBAGE WHERE TO_CHAR(TO_DATE(MONTH_YEAR), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) MAGAR ON WI.ITEM_ID = MAGAR.ITEM_ID WHERE ((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))))) -(nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)) > 0 OR nvl(BEGWRSIH.FINAL_STOCK_WT, 0) > 0 OR (nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) - (nvl(WMTRM.ITEM_WEIGHT, 0) + nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))) - (nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)) > 0 ) OR nvl(WPM.ITEM_WEIGHT, 0) > 0   ORDER BY WI.ITEM_ID ";  // WHERE ((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))))) -(nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)) > 0 OR nvl(BEGWRSIH.FINAL_STOCK_WT, 0) > 0 OR (nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) - (nvl(WMTRM.ITEM_WEIGHT, 0) + nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))) - (nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)) > 0 )      

                cmdl = new OracleCommand(sqlString);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                 
                html.Append("  <div class='box-body'>");
                html.Append("<div class='row'>");

                html.Append("<div class='col-md-12'>");
                html.Append("<div  style='text-align:center;border: solid 1px #c1c1c1;font-size:16px;font-weight:400;'>" + CurrentMonthTitle + "</div>");
                html.Append("<table id='GridViewItem' width='100%'>");
                html.Append("<thead>");
                html.Append("<tr>");
                html.Append("<th rowspan='2' align='center'>MATERIAL</th>");
                html.Append("<th rowspan='2' align='center'>BEG. INVENTORY</th>");
                html.Append("<th rowspan='2' align='center'>CURRENT PURCHASES</th>");
                html.Append("<th rowspan='2' align='center'>GARBAGE</th>");
                html.Append("<th rowspan='2' align='center'>NET PURCHASES</th>");
                html.Append("<th rowspan='2' align='center'>TOTAL QTY. AVAIL. FOR SALE BEFORE ADJUSTMENT</th>");
                html.Append("<th colspan='4' align='center'>ADJUSTMENTS / TRANSANCTION </th>");
                html.Append("<th rowspan='2' align='center'>NET TOTAL QTY. AVAIL. FOR SALE</th>");
                html.Append("<th colspan='3' align='center'>TOTAL SALES</th>");
                html.Append("<th rowspan='2' align='center'>END INV. AS PER BOOK</th>");
                html.Append("<th rowspan='2' align='center'>TRANSIT</th>");
                html.Append("<th rowspan='2' align='center'>END INV. FACTORY</th>");
                html.Append("</tr>");
                html.Append("<tr>"); 
                html.Append("<th align='center'>ISSUED (-)</th>");
                html.Append("<th align='center'>RECEVIED (+)</th>");
                html.Append("<th align='center'>TRANS.(-)</th>");
                html.Append("<th align='center'>TRANS.(+)</th>"); 
                html.Append("<th align='center'>OVERSEAS</th>");
                html.Append("<th align='center'>LOCAL</th>");
                html.Append("<th align='center'>INTER. DIV.</th>"); 
                html.Append("</tr>");
                html.Append("</thead>");
                html.Append("<tbody>");
                double BegFStock = 0, PurchaseWt = 0, GarEstWt = 0, PurchaseNetWt = 0, PurchaseNetGarEstWt = 0, MatIssuedWt = 0, MatReceviedWt = 0, MatTransferDeducWt = 0, MatTransferWt = 0, SalesAvailWt = 0, SalesOverseasWt = 0, SalesLocalWt = 0, SalesInterDivWt = 0, EndFstock = 0, ItemWtTransit = 0, EndAsPerFactory = 0; 
                for (int j = 0; j < RowCount; j++)
                {
                    BegFStock += Convert.ToDouble(dt.Rows[j]["BEG_FSTOCK_WT"].ToString());
                    PurchaseWt += Convert.ToDouble(dt.Rows[j]["PURCHASE_WT"].ToString());
                    GarEstWt += Convert.ToDouble(dt.Rows[j]["GAR_EST_WT"].ToString());
                    PurchaseNetWt += Convert.ToDouble(dt.Rows[j]["PURCHASE_NET_WT"].ToString());
                    PurchaseNetGarEstWt += Convert.ToDouble(dt.Rows[j]["PURCHASE_NET_GAR_EST_WT"].ToString());
                    MatIssuedWt += Convert.ToDouble(dt.Rows[j]["MAT_ISSUED_WT"].ToString());
                    MatReceviedWt += Convert.ToDouble(dt.Rows[j]["MAT_RECEVIED_WT"].ToString());
                    MatTransferDeducWt += Convert.ToDouble(dt.Rows[j]["MAT_TRANSFER_DEDUC_WT"].ToString());
                    MatTransferWt += Convert.ToDouble(dt.Rows[j]["MAT_TRANSFER_WT"].ToString());
                    SalesAvailWt += Convert.ToDouble(dt.Rows[j]["SALES_AVAIL_WT"].ToString());
                    SalesOverseasWt += Convert.ToDouble(dt.Rows[j]["SALES_OVERSEAS_WT"].ToString());
                    SalesLocalWt += Convert.ToDouble(dt.Rows[j]["SALES_LOCAL_WT"].ToString());
                    SalesInterDivWt += Convert.ToDouble(dt.Rows[j]["SALES_INTER_DIV_WT"].ToString());
                    EndFstock += Convert.ToDouble(dt.Rows[j]["END_FSTOCK_WT"].ToString());
                    ItemWtTransit += Convert.ToDouble(dt.Rows[j]["ITEM_WEIGHT_TRANSIT"].ToString());
                    EndAsPerFactory += Convert.ToDouble(dt.Rows[j]["END_AS_PER_BOOK"].ToString());

                    html.Append("<tr>");
                    html.Append("<th style='text-align:left;'>");
                    html.Append(dt.Rows[j]["ITEM_NAME"].ToString());
                    html.Append("</th>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["BEG_FSTOCK_WT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["PURCHASE_WT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["GAR_EST_WT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["PURCHASE_NET_WT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["PURCHASE_NET_GAR_EST_WT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["MAT_ISSUED_WT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["MAT_RECEVIED_WT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["MAT_TRANSFER_DEDUC_WT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["MAT_TRANSFER_WT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["SALES_AVAIL_WT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["SALES_OVERSEAS_WT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["SALES_LOCAL_WT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["SALES_INTER_DIV_WT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["END_FSTOCK_WT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["ITEM_WEIGHT_TRANSIT"].ToString())));
                    html.Append("</td>");
                    html.Append("<td align='right'>");
                    html.Append(string.Format("{0:n2}", Convert.ToDouble(dt.Rows[j]["END_AS_PER_BOOK"].ToString())));
                    html.Append("</td>");
                    html.Append("</tr>");
                }
                 
                html.Append(" </tbody>");
                html.Append("<tfoot>");
                html.Append("<tr>");
                html.Append("<th align='right'>GRAND TOTAL</th>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", BegFStock) + "</td>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", PurchaseWt) + "</td>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", GarEstWt) + "</td>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", PurchaseNetWt) + "</td>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", PurchaseNetGarEstWt) + "</td>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", MatIssuedWt) + "</td>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", MatReceviedWt) + "</td>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", MatTransferDeducWt) + "</td>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", MatTransferWt) + "</td>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", SalesAvailWt) + "</td>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", SalesOverseasWt) + "</td>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", SalesLocalWt) + "</td>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", SalesInterDivWt) + "</td>");  
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", EndFstock) + "</td>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", ItemWtTransit) + "</td>");
                html.Append("<td style='text-align:right;font-weight:700;'>" + string.Format("{0:n2}", EndAsPerFactory) + "</td>");
                html.Append("</tr>"); 
                html.Append("</tfoot>");

                html.Append(" </table>");
                html.Append("</div>");
                  
                html.Append("</div>");
                html.Append("</div>");
               
                PlaceHolderInventoryReport.Controls.Add(new Literal { Text = html.ToString() });

                conn.Close();
                
            }
            else
            {
                //   Response.Redirect("~/PagePermissionError.aspx");
            }
        }
        protected void DisplayInventorySearch(object sender, EventArgs e)
        {
            this.DisplayInventory();
        }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int Data_ID = Convert.ToInt32(TextInventoryRmID.Text);
                double InitialStock = Convert.ToDouble(TextInitialStock.Text);
                double ItemAvgRate = Convert.ToDouble(TextItemAvgRate.Text);
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                double StockInWet = 0.00, StockOutWet = 0.00, FinalStockNew = 0.00;

                // inventory RM update select
                string makeSQLInvenRMUp = " select * from MS_RM_STOCK_INVENTORY_MASTER where RM_INVENTORY_ID  = '" + Data_ID + "' ";
                cmdl = new OracleCommand(makeSQLInvenRMUp);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                 
                for (int i = 0; i < RowCount; i++)
                { 
                    StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                    StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString()); 
                }
                 
                FinalStockNew = InitialStock + StockInWet - StockOutWet;

                if (0 < RowCount)
                {
                    // inventory RM update
                    string update_inven_mas = "update  MS_RM_STOCK_INVENTORY_MASTER  set INITIAL_STOCK_WT = :NoInitialStock, FINAL_STOCK_WT = :NoFinalStock, ITEM_END_AMOUNT = :NoItemAvgRate, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where RM_INVENTORY_ID = :NoRmInventoryID ";
                    cmdu = new OracleCommand(update_inven_mas, conn);

                    OracleParameter[] objPrmInevenMas = new OracleParameter[6];
                    objPrmInevenMas[0] = cmdu.Parameters.Add("NoInitialStock", InitialStock);
                    objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                    objPrmInevenMas[2] = cmdu.Parameters.Add("NoItemAvgRate", ItemAvgRate);
                    objPrmInevenMas[3] = cmdu.Parameters.Add("u_date", u_date);
                    objPrmInevenMas[4] = cmdu.Parameters.Add("NoCuserID", userID);
                    objPrmInevenMas[5] = cmdu.Parameters.Add("NoRmInventoryID", Data_ID); 

                    cmdu.ExecuteNonQuery();
                    cmdu.Parameters.Clear();
                    cmdu.Dispose();
                }

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Raw Material Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                Display();
                DisplayInventory();
            }
                else { 
                    Response.Redirect("~/PagePermissionError.aspx");
                }
        }

        protected void BtnUpdateHis_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int Data_ID = Convert.ToInt32(TextInventoryRmHisID.Text);
                double FinalStock = Convert.ToDouble(TextFinalStockHis.Text.Trim());
                double ItemAvgRate = Convert.ToDouble(TextItemAvgRateHis.Text.Trim());
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt"); 
 
                // inventory RM inventory history update
                string update_inven_mas = "update  MS_RM_STOCK_INVENTORY_HISTORY  set  FINAL_STOCK_WT = :NoFinalStock, ITEM_END_AMOUNT = :NoItemAvgRate, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where IN_RM_HIS_ID = :NoRmInventoryHisID ";
                cmdu = new OracleCommand(update_inven_mas, conn);

                OracleParameter[] objPrmInevenMas = new OracleParameter[5]; 
                objPrmInevenMas[0] = cmdu.Parameters.Add("NoFinalStock", FinalStock);
                objPrmInevenMas[1] = cmdu.Parameters.Add("NoItemAvgRate", ItemAvgRate);
                objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                objPrmInevenMas[4] = cmdu.Parameters.Add("NoRmInventoryHisID", Data_ID); 

                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose();
               

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Raw Material Inventory History Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                Display();
                DisplayInventory();
            }
                else { 
                    Response.Redirect("~/PagePermissionError.aspx");
                }
        }
         
        public void clearTextField(object sender, EventArgs e)
        {
            TextInventoryRmID.Text = "";
            TextInitialStock.Text = "";
            TextStockIn.Text = "";
            TextItemAvgRate.Text = "";
            TextFinalStockHis.Text = "";
            TextItemAvgRateHis.Text = "";
            DropDownItemID.Text = "0";
            
        }

        public void clearText()
        {
            TextInventoryRmID.Text = "";
            TextInitialStock.Text = "";
            TextStockIn.Text = "";
            TextItemAvgRate.Text = ""; 
            TextFinalStockHis.Text = "";
            TextItemAvgRateHis.Text = "";
            DropDownItemID.Text = "0";

        }

        public DataSet ExecuteBySqlStringType(string sqlString)
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

       
   }
}