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


namespace NRCAPPS.MS
{
    public partial class MsRmStatement : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand  cmdl;
        OracleDataAdapter oradata;
        DataTable dt;
        int RowCount;
        string HtmlString = "";
        string IS_PAGE_ACTIVE   = "";
        string IS_ADD_ACTIVE    = "";
        string IS_EDIT_ACTIVE   = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE   = "";
        string IS_REPORT_ACTIVE = ""; 
 
        public bool IsLoad { get; set; } public bool IsLoad1 { get; set; }
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
                         
                        alert_box.Visible = false;

                        BtnReport.Focus();

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

        public void BtnAdd_Click(object sender, EventArgs e)
        {
             
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open(); 

                    DataTable dtUserTypeID = new DataTable();
                    DataSet ds = new DataSet();
                     
                  
                }
                else { 
                    Response.Redirect("~/PagePermissionError.aspx");
                }
             
        }
        protected void BtnReport1_Click(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad1 = true;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }

        }
         
        protected void BtnReport_Click(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad = true;
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
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
               
                string AsOnDate = EntryDate2.Text; 

                string MakeAsOnDate = EntryDate2.Text;
                string[] MakeAsOnDateSplit = MakeAsOnDate.Split('-');
                String AsOnDateTemp = MakeAsOnDateSplit[0].Replace("/", "-");
                DateTime AsOnDateNewD = DateTime.ParseExact(AsOnDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string AsOnDateNew = AsOnDateNewD.ToString("dd-MM-yyyy");
                string AsOnDateNewFull = AsOnDateNewD.ToString("dd-MMM-yyyy");

                DateTime curDate = AsOnDateNewD;
                DateTime startDate = curDate.AddMonths(-1);
                DateTime LastDateTemp = curDate.AddDays(-(curDate.Day));
                string LastDate = LastDateTemp.ToString("dd-MM-yyyy");
                string LastMonth = startDate.ToString("MM-yyyy");
                string CurrentMonth = AsOnDateNewD.ToString("MM-yyyy");
                string CurrentMonthFull = AsOnDateNewD.ToString("MMM-yyyy");

                //   string sqlString = "  SELECT WI.ITEM_ID, WI.ITEM_NAME, nvl(BEGWRSIH.FINAL_STOCK_WT,0) AS BEG_FSTOCK_WT, nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) AS BEG_AMT, ROUND(nvl(nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) / nullif(BEGWRSIH.FINAL_STOCK_WT, 0), 3)*1000, 0) AS BEG_AVG_RATE, nvl(WPM.ITEM_WEIGHT, 0) AS PURCHASE_WT, nvl(WPM.ITEM_AMOUNT, 0) AS PURCHASE_AMT, nvl(ROUND((nvl(WPM.ITEM_AMOUNT, 0) / nullif(nvl(WPM.ITEM_WEIGHT, 0), 0)) * 1000, 2), 0) AS PURCHASE_AVG_RATE, ROUND((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100, 2) AS GAR_EST_WT, ROUND(nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100), 2) AS PURCHASE_NET_WT, nvl(ROUND(nvl(WPM.ITEM_AMOUNT, 0) / nullif(nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100), 0), 2) * 1000, 0) AS PURCHASE_NET_AVG_RATE, nvl(BEGWRSIH.FINAL_STOCK_WT, 0)+ROUND(nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100), 2) AS PURCHASE_NET_GAR_EST_WT, nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) +nvl(WPM.ITEM_AMOUNT, 0) AS PURCHASE_BEG_AMT, ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100))), 0), 4)*1000 AS PURCHASE_BEG_AVG_RATE, nvl(WMTI.ITEM_WEIGHT, 0) AS MAT_ISSUED_WT, nvl(WMTR.ITEM_WEIGHT, 0) AS MAT_RECEVIED_WT, nvl(WMTRM.ITEM_WEIGHT, 0) AS MAT_TRANSFER_DEDUC_WT, ROUND((((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100))), 0))) * nvl(WMTRM.ITEM_WEIGHT, 0), 2) AS MAT_TRANSFER_DEDUC_AMT, nvl(WMTT.ITEM_WEIGHT, 0) AS MAT_TRANSFER_WT, nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0) AS PURCHASE_TRANS_AVG_RATE, ROUND((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000, 2) AS MAT_TRANSFER_AMT, nvl(BEGWRSIH.FINAL_STOCK_WT, 0)+ROUND(nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0)))) AS SALES_AVAIL_WT, ROUND(nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0) + ROUND((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000, 2) - (((((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100))), 0))) * nvl(WMTRM.ITEM_WEIGHT, 0))), 2) AS SALES_AVAIL_AMT, ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0) + ((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000) - (((((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100))), 0))) * nvl(WMTRM.ITEM_WEIGHT, 0)))) / nullif(nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100)) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0)))), 0) * 1000, 2) AS SALES_AVAIL_AVG_RATE, nvl(WEWCI.ITEM_WEIGHT, 0) AS SALES_OVERSEAS_WT, nvl(WSM.ITEM_WEIGHT, 0) AS SALES_LOCAL_WT, nvl(WSIDM.ITEM_WEIGHT, 0) AS SALES_INTER_DIV_WT, (nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))))) -(nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)) AS END_FSTOCK_WT, ROUND(((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))))) - (nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0))) * (((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0) + ((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000) - (((((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100))), 0))) * nvl(WMTRM.ITEM_WEIGHT, 0)))) / nullif(nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100)) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0)))), 0))), 2) AS END_AMT, ROUND((((((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100), 4) - (nvl(WMTRM.ITEM_WEIGHT, 0) + nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))) - (nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0))) * (ROUND(((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0) + ((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000)) - ((ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100))), 0), 4)) * nvl(WMTRM.ITEM_WEIGHT, 0))) / nullif(nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0)))), 0) * 1000, 2))))) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100), 4) - (nvl(WMTRM.ITEM_WEIGHT, 0) + nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))) - (nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)), 0), 2) AS END_AVG_RATE, nvl(WEWCIT.ITEM_WEIGHT, 0) AS ITEM_WEIGHT_TRANSIT, ((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))))) -(nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)))-nvl(WEWCIT.ITEM_WEIGHT, 0) AS END_AS_PER_BOOK FROM WP_ITEM WI LEFT JOIN(SELECT ITEM_ID, FINAL_STOCK_WT, ITEM_END_AMOUNT FROM WP_RM_STOCK_INVENTORY_HISTORY WHERE TO_CHAR(TO_DATE(CREATE_DATE), 'dd-mm-YYYY') = '" + LastDate + "' ) BEGWRSIH ON WI.ITEM_ID = BEGWRSIH.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT, sum(nvl(ITEM_AMOUNT, 0) + nvl(VAT_AMOUNT, 0)) AS ITEM_AMOUNT FROM WP_PURCHASE_MASTER  WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WPM ON WI.ITEM_ID = WPM.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT FROM WP_MATERIAL_TRANSACTION  WHERE TRANSACTION_FOR_ID = 1 AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WMTI ON WI.ITEM_ID = WMTI.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT FROM WP_MATERIAL_TRANSACTION  WHERE TRANSACTION_FOR_ID = 2 AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WMTR ON WI.ITEM_ID = WMTR.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT FROM WP_MATERIAL_TRANSACTION  WHERE TRANSACTION_FOR_ID = 3 AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WMTRM ON WI.ITEM_ID = WMTRM.ITEM_ID LEFT JOIN(SELECT WMT.ITEM_ID, WMT.ITEM_TRANSFER_ID, sum(nvl(WMT.ITEM_WEIGHT,0)) AS ITEM_WEIGHT, ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100))), 0), 4)*1000 AS PURCHASE_TRANS_AVG_RATE FROM WP_MATERIAL_TRANSACTION  WMT LEFT JOIN(SELECT ITEM_ID, FINAL_STOCK_WT, ITEM_END_AMOUNT FROM WP_RM_STOCK_INVENTORY_HISTORY WHERE TO_CHAR(TO_DATE(CREATE_DATE), 'dd-mm-YYYY') = '" + LastDate + "' ) BEGWRSIH ON WMT.ITEM_ID = BEGWRSIH.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT, sum(nvl(ITEM_AMOUNT, 0) + nvl(VAT_AMOUNT, 0)) AS ITEM_AMOUNT FROM WP_PURCHASE_MASTER  WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WPM ON WMT.ITEM_ID = WPM.ITEM_ID WHERE WMT.TRANSACTION_FOR_ID = 3 AND TO_CHAR(TO_DATE(WMT.ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY WMT.ITEM_ID, WMT.ITEM_TRANSFER_ID, ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100))), 0), 4) * 1000) WMTT ON WI.ITEM_ID = WMTT.ITEM_TRANSFER_ID LEFT JOIN(SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM WP_EXPORT_WBSLIP_CON_ITEM WHERE  IS_INVENTORY_STATUS = 'Complete' AND TO_CHAR(TO_DATE(IS_SHIPMENT_COMPLETE_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WEWCI ON WI.ITEM_ID = WEWCI.ITEM_ID LEFT JOIN(SELECT WEWCI.ITEM_ID, sum(WEWCI.ITEM_WEIGHT) AS ITEM_WEIGHT FROM WP_EXPORT_WBSLIP_CON_ITEM WEWCI LEFT JOIN WP_EXPORT_WBSLIP_CON WEWC ON WEWC.WB_SLIP_NO = WEWCI.WB_SLIP_NO WHERE  WEWCI.IS_INVENTORY_STATUS = 'Transit' AND (TO_CHAR(TO_DATE(SYSDATE), 'mm-YYYY')  = '" + CurrentMonth + "' OR TO_CHAR(TO_DATE(WEWC.DISPATCH_DATE), 'mm-YYYY') <=  '" + CurrentMonth + "') GROUP BY WEWCI.ITEM_ID) WEWCIT ON WI.ITEM_ID = WEWCIT.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT, SUM(nvl(ITEM_AMOUNT, 0) + nvl(VAT_AMOUNT, 0)) AS TOTAL_AMOUNT  FROM WP_SALES_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WSM ON WI.ITEM_ID = WSM.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT  FROM WP_SALES_INTER_DIV_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WSIDM ON WI.ITEM_ID = WSIDM.ITEM_ID   "; //WHERE (nvl(BEGWRSIH.FINAL_STOCK_WT, 0) > 0 OR(nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - ((nvl(WPM.ITEM_WEIGHT, 0) * 2) / 100), 4) - (nvl(WMTRM.ITEM_WEIGHT, 0) + nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))) - (nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)) > 0)
                string sqlString = "   SELECT WI.ITEM_ID, WI.ITEM_CODE || ' : ' || WI.ITEM_NAME AS ITEM_NAME, nvl(BEGWRSIH.FINAL_STOCK_WT,0) AS BEG_FSTOCK_WT, nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) AS BEG_AMT, ROUND(nvl(nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) / nullif(BEGWRSIH.FINAL_STOCK_WT, 0), 3)*1000, 0) AS BEG_AVG_RATE, nvl(WPM.ITEM_WEIGHT, 0) AS PURCHASE_WT, nvl(WPM.ITEM_AMOUNT, 0) AS PURCHASE_AMT, nvl(ROUND((nvl(WPM.ITEM_AMOUNT, 0) / nullif(nvl(WPM.ITEM_WEIGHT, 0), 0)) * 1000, 2), 0) AS PURCHASE_AVG_RATE, ROUND(nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 2) AS GAR_EST_WT, ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 2) AS PURCHASE_NET_WT, nvl(ROUND(nvl(WPM.ITEM_AMOUNT, 0) / nullif(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 0), 2) * 1000, 0) AS PURCHASE_NET_AVG_RATE, nvl(BEGWRSIH.FINAL_STOCK_WT, 0)+ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 2) AS PURCHASE_NET_GAR_EST_WT, nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) +nvl(WPM.ITEM_AMOUNT, 0) AS PURCHASE_BEG_AMT, ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0), 4)*1000 AS PURCHASE_BEG_AVG_RATE, nvl(WMTI.ITEM_WEIGHT, 0) AS MAT_ISSUED_WT, nvl(WMTR.ITEM_WEIGHT, 0) AS MAT_RECEVIED_WT, nvl(WMTRM.ITEM_WEIGHT, 0) AS MAT_TRANSFER_DEDUC_WT, ROUND((((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0))) * nvl(WMTRM.ITEM_WEIGHT, 0), 2) AS MAT_TRANSFER_DEDUC_AMT, nvl(WMTT.ITEM_WEIGHT, 0) AS MAT_TRANSFER_WT, nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0) AS PURCHASE_TRANS_AVG_RATE, ROUND((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000, 2) AS MAT_TRANSFER_AMT, nvl(BEGWRSIH.FINAL_STOCK_WT, 0)+ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0)))) AS SALES_AVAIL_WT, ROUND(nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0) + ROUND((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000, 2) - (((((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0))) * nvl(WMTRM.ITEM_WEIGHT, 0))), 2) AS SALES_AVAIL_AMT, ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0) + ((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000) - (((((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0))) * nvl(WMTRM.ITEM_WEIGHT, 0)))) / nullif(nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0)) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0)))), 0) * 1000, 2) AS SALES_AVAIL_AVG_RATE, nvl(WEWCI.ITEM_WEIGHT, 0) AS SALES_OVERSEAS_WT, nvl(WSM.ITEM_WEIGHT, 0) AS SALES_LOCAL_WT, nvl(WSIDM.ITEM_WEIGHT, 0) AS SALES_INTER_DIV_WT, (nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))))) -(nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)) AS END_FSTOCK_WT, ROUND(((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))))) - (nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0))) * (((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0) + ((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000) - (((((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0))) * nvl(WMTRM.ITEM_WEIGHT, 0)))) / nullif(nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0)) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0)))), 0))), 2) AS END_AMT, ROUND((((((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) - (nvl(WMTRM.ITEM_WEIGHT, 0) + nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))) - (nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0))) * (ROUND(((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0) + ((nvl(WMTT.ITEM_WEIGHT, 0) * nvl(WMTT.PURCHASE_TRANS_AVG_RATE, 0)) / 1000)) - ((ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0), 4)) * nvl(WMTRM.ITEM_WEIGHT, 0))) / nullif(nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0)))), 0) * 1000, 2))))) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) - (nvl(WMTRM.ITEM_WEIGHT, 0) + nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))) - (nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)), 0), 2) AS END_AVG_RATE, nvl(WEWCIT.ITEM_WEIGHT, 0) AS ITEM_WEIGHT_TRANSIT, ((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))))) -(nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)))-nvl(WEWCIT.ITEM_WEIGHT, 0) AS END_AS_PER_BOOK FROM MF_ITEM WI LEFT JOIN(SELECT ITEM_ID, FINAL_STOCK_WT, ITEM_END_AMOUNT FROM MS_RM_STOCK_INVENTORY_HISTORY WHERE TO_CHAR(TO_DATE(CREATE_DATE), 'dd-mm-YYYY') = '" + LastDate + "' ) BEGWRSIH ON WI.ITEM_ID = BEGWRSIH.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT, sum(nvl(ITEM_AMOUNT, 0) + nvl(VAT_AMOUNT, 0)) AS ITEM_AMOUNT FROM MS_PURCHASE_MASTER  WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WPM ON WI.ITEM_ID = WPM.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT FROM MS_MATERIAL_TRANSACTION  WHERE TRANSACTION_FOR_ID = 1 AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WMTI ON WI.ITEM_ID = WMTI.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT FROM MS_MATERIAL_TRANSACTION  WHERE TRANSACTION_FOR_ID = 2 AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WMTR ON WI.ITEM_ID = WMTR.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT FROM MS_MATERIAL_TRANSACTION  WHERE TRANSACTION_FOR_ID = 3 AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WMTRM ON WI.ITEM_ID = WMTRM.ITEM_ID LEFT JOIN(SELECT WMT.ITEM_ID, WMT.ITEM_TRANSFER_ID, sum(nvl(WMT.ITEM_WEIGHT,0)) AS ITEM_WEIGHT, ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0), 4)*1000 AS PURCHASE_TRANS_AVG_RATE FROM MS_MATERIAL_TRANSACTION  WMT LEFT JOIN(SELECT ITEM_ID, FINAL_STOCK_WT, ITEM_END_AMOUNT FROM MS_RM_STOCK_INVENTORY_HISTORY WHERE TO_CHAR(TO_DATE(CREATE_DATE), 'dd-mm-YYYY') = '" + LastDate + "' ) BEGWRSIH ON WMT.ITEM_ID = BEGWRSIH.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT, sum(nvl(ITEM_AMOUNT, 0) + nvl(VAT_AMOUNT, 0)) AS ITEM_AMOUNT FROM MS_PURCHASE_MASTER  WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WPM ON WMT.ITEM_ID = WPM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, nvl(sum(nvl(ACTUAL_GAR_WEIGHT, 0)), 0) AS ACTUAL_GAR_WEIGHT FROM MS_ACTUAL_GARBAGE WHERE TO_CHAR(TO_DATE(MONTH_YEAR), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) MAGAR ON WMT.ITEM_ID = MAGAR.ITEM_ID WHERE WMT.TRANSACTION_FOR_ID = 3 AND TO_CHAR(TO_DATE(WMT.ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY WMT.ITEM_ID, WMT.ITEM_TRANSFER_ID, ROUND((nvl(BEGWRSIH.ITEM_END_AMOUNT, 0) + nvl(WPM.ITEM_AMOUNT, 0)) / nullif((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + (nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0))), 0), 4) * 1000) WMTT ON WI.ITEM_ID = WMTT.ITEM_TRANSFER_ID LEFT JOIN(SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM MS_EXPORT_WBSLIP_CON_ITEM WHERE  IS_INVENTORY_STATUS = 'Complete' AND TO_CHAR(TO_DATE(IS_SHIPMENT_COMPLETE_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WEWCI ON WI.ITEM_ID = WEWCI.ITEM_ID LEFT JOIN(SELECT WEWCI.ITEM_ID, sum(WEWCI.ITEM_WEIGHT) AS ITEM_WEIGHT FROM MS_EXPORT_WBSLIP_CON_ITEM WEWCI LEFT JOIN MS_EXPORT_WBSLIP_CON WEWC ON WEWC.WB_SLIP_NO = WEWCI.WB_SLIP_NO WHERE  WEWCI.IS_INVENTORY_STATUS = 'Transit' AND (TO_CHAR(TO_DATE(SYSDATE), 'mm-YYYY')  = '" + CurrentMonth + "' OR TO_CHAR(TO_DATE(WEWC.DISPATCH_DATE), 'mm-YYYY') <=  '" + CurrentMonth + "') GROUP BY WEWCI.ITEM_ID) WEWCIT ON WI.ITEM_ID = WEWCIT.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT, SUM(nvl(ITEM_AMOUNT, 0) + nvl(VAT_AMOUNT, 0)) AS TOTAL_AMOUNT  FROM MS_SALES_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WSM ON WI.ITEM_ID = WSM.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT  FROM MS_SALES_INTER_DIV_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) WSIDM ON WI.ITEM_ID = WSIDM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, nvl(sum(nvl(ACTUAL_GAR_WEIGHT, 0)), 0) AS ACTUAL_GAR_WEIGHT FROM MS_ACTUAL_GARBAGE WHERE TO_CHAR(TO_DATE(MONTH_YEAR), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) MAGAR ON WI.ITEM_ID = MAGAR.ITEM_ID WHERE ((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))))) -(nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)) > 0 OR nvl(BEGWRSIH.FINAL_STOCK_WT, 0) > 0 OR (nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) - (nvl(WMTRM.ITEM_WEIGHT, 0) + nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))) - (nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)) > 0 ) OR nvl(WPM.ITEM_WEIGHT, 0) > 0   ORDER BY WI.ITEM_ID ";  // WHERE ((nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) + (nvl(WMTR.ITEM_WEIGHT, 0) + nvl(WMTT.ITEM_WEIGHT, 0) - ((nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))))) -(nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)) > 0 OR nvl(BEGWRSIH.FINAL_STOCK_WT, 0) > 0 OR (nvl(BEGWRSIH.FINAL_STOCK_WT, 0) + ROUND(nvl(WPM.ITEM_WEIGHT, 0) - nvl(MAGAR.ACTUAL_GAR_WEIGHT, 0), 4) - (nvl(WMTRM.ITEM_WEIGHT, 0) + nvl(WMTI.ITEM_WEIGHT, 0) + nvl(WMTRM.ITEM_WEIGHT, 0))) - (nvl(WEWCI.ITEM_WEIGHT, 0) + nvl(WSM.ITEM_WEIGHT, 0) + nvl(WSIDM.ITEM_WEIGHT, 0)) > 0 )      



                cmdl = new OracleCommand(sqlString);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                double BegInventoryTotal = 0.0, PurchaseTotal = 0.0, TotalGarbage = 0.0, Adjustment = 0.0, OverseasSales = 0.0, LocalSales = 0.0, SalesInterDiv = 0.0, MatIssued=0.0, MatReceived=0.0,
                       MatTransferDeduc=0.0, MatTransfer=0.0;

                for (int i = 0; i < RowCount; i++)
                {
                    BegInventoryTotal += Convert.ToDouble(dt.Rows[i]["BEG_FSTOCK_WT"].ToString());
                    PurchaseTotal += Convert.ToDouble(dt.Rows[i]["PURCHASE_WT"].ToString());
                    TotalGarbage += Convert.ToDouble(dt.Rows[i]["GAR_EST_WT"].ToString());
                    MatIssued += Convert.ToDouble(dt.Rows[i]["MAT_ISSUED_WT"].ToString());
                    MatReceived += Convert.ToDouble(dt.Rows[i]["MAT_RECEVIED_WT"].ToString());
                    MatTransferDeduc += Convert.ToDouble(dt.Rows[i]["MAT_TRANSFER_DEDUC_WT"].ToString());
                    MatTransfer += Convert.ToDouble(dt.Rows[i]["MAT_TRANSFER_WT"].ToString());

                    OverseasSales += Convert.ToDouble(dt.Rows[i]["SALES_OVERSEAS_WT"].ToString());
                    LocalSales += Convert.ToDouble(dt.Rows[i]["SALES_LOCAL_WT"].ToString());
                    SalesInterDiv += Convert.ToDouble(dt.Rows[i]["SALES_INTER_DIV_WT"].ToString());                    
                }
                Adjustment = (MatReceived+MatTransfer) - (MatIssued + MatTransferDeduc);

                HtmlString += "<div style='float:left;width:725px;height:auto;margin:10px 0 0 50px;padding:10px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 13px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";
                HtmlString += "<div style='float:left;width:725px;height:100px;text-align:center;' ><img src='../../image/logo_from.png'/></div> ";
                HtmlString += "<div style='float:left;width:725px;height:30px;text-align:center;text-decoration: underline;' ><span style='font-family:Times New Roman,Times, serif;font-size:16px;font-weight:700;'>METAL SCRAP DIVISION</span></div> ";
                HtmlString += "<div style='float:left;width:725px;height:35px;text-align:center;text-decoration: underline;' ><span style='font-family:Times New Roman,Times, serif;font-size:15px;font-weight:700;'>STOCK POSITION AS ON " + AsOnDateNewFull.ToUpper() + "</span></div> ";

                HtmlString += "<table cellpadding='6px' cellspacing='0' style='font-size: 14px;' width='85%' align='center'>";
                HtmlString += "<th style='border:black solid 1px; -webkit-border-top-left-radius:10px;width:85px;'>PARTICULARS</th> ";
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;-webkit-border-top-right-radius:10px;'>WEIGHT IN MT</span></th> "; 
                 
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;'> ";
                    HtmlString += "OPENING STOCK AS ON 01-"+ CurrentMonthFull.ToUpper() + "";
                    HtmlString += "</td> ";
                    HtmlString += "<td style='border-right:black solid 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", BegInventoryTotal / 1000) + " ";
                    HtmlString += "</td> "; 
                    HtmlString += "</tr> ";

                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;'> ";
                    HtmlString += "COLLCETION DURING " + CurrentMonthFull.ToUpper() + "";
                    HtmlString += "</td> ";
                    HtmlString += "<td style='border-right:black solid 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", Math.Round(PurchaseTotal / 1000,3))+ " ";
                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";

                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;'> ";
                    HtmlString += "LESS :- GARBAGE OF COLLECTION ";
                    HtmlString += "</td> ";
                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", Math.Round(TotalGarbage / 1000, 3)) + " ";
                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";

                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;'> ";
                    HtmlString += "SUB TOTAL (AFTER GARBAGE DEDUCTION): ";
                    HtmlString += "</td> ";
                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", Math.Round(((BegInventoryTotal+PurchaseTotal) - TotalGarbage) / 1000,3)) + " ";
                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";

                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;'> ";
                    HtmlString += "ADJUSTMENT AS PER PHYSICAL COUNT ";
                    HtmlString += "</td> ";
                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", Math.Round(Adjustment / 1000,3)) + " ";
                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";
                 
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;'> ";
                    HtmlString += "SUB TOTAL (AFTER ADJUSTMENT): ";
                    HtmlString += "</td> ";
                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", Math.Round((((BegInventoryTotal + PurchaseTotal) - TotalGarbage)+ Adjustment) / 1000, 3)) + " ";
                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";

                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;'> ";
                    HtmlString += "EXPORT SALES DURING " + CurrentMonthFull.ToUpper() + "";
                    HtmlString += "</td> ";
                    HtmlString += "<td style='border-right:black solid 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", Math.Round(OverseasSales / 1000,3)) + " ";
                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";

                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;'> ";
                    HtmlString += "LOCAL SALES DURING " + CurrentMonthFull.ToUpper() + "";
                    HtmlString += "</td> ";
                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", Math.Round(LocalSales / 1000,3)) + " ";
                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";

                    HtmlString += "<tr> ";
                    HtmlString += "<td style='-webkit-border-bottom-left-radius:10px;border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;font-weight:700;'> ";
                    HtmlString += "CLOSING STOCK AS ON  " + AsOnDateNewFull.ToUpper() + " (GROSS)";
                    HtmlString += "</td> ";
                    HtmlString += "<td style='-webkit-border-bottom-right-radius:10px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                    HtmlString += "" + string.Format("{0:n3}", Math.Round(((((BegInventoryTotal + PurchaseTotal) - TotalGarbage) + Adjustment)-(OverseasSales+ LocalSales)) / 1000, 3)) + " ";
                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";

                    HtmlString += "<tr> ";
                    HtmlString += "<td> ";
                    HtmlString += "&nbsp;";
                    HtmlString += "</td> ";
                    HtmlString += "<td> ";
                    HtmlString += "&nbsp; ";
                    HtmlString += "</td> ";


                    HtmlString += "<tr> ";
                    HtmlString += "<td style='-webkit-border-top-left-radius:10px;border:black solid 1px;'> ";
                    HtmlString += "MATERIAL TRANSFER DURING " + CurrentMonthFull.ToUpper() + "";
                    HtmlString += "</td> ";
                    HtmlString += "<td style='-webkit-border-top-right-radius:10px;border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", Math.Round(SalesInterDiv / 1000, 3)) + " ";
                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";

                    HtmlString += "<tr> ";
                    HtmlString += "<td style='-webkit-border-bottom-left-radius:10px;border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;font-weight:700;'> ";
                    HtmlString += "CLOSING STOCK AS ON  " + AsOnDateNewFull.ToUpper() + " (NET)";
                    HtmlString += "</td> ";
                    HtmlString += "<td style='-webkit-border-bottom-right-radius:10px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                    HtmlString += "" + string.Format("{0:n3}", Math.Round(((((BegInventoryTotal + PurchaseTotal) - TotalGarbage) + Adjustment) - (OverseasSales + LocalSales + SalesInterDiv)) / 1000, 3)) + " ";
                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";

                HtmlString += "</table> ";
                 
                 

                PanelPrint.Controls.Add(new LiteralControl(HtmlString));
                Session["ctrl"] = PanelPrint;
                ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('Print.aspx','PrintMe','height=900px,width=1200px,scrollbars=1');</script>");
                 
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }

        }        

        public void clearTextField(object sender, EventArgs e)
        {

            AsOnDate.Text = "";  
            
        }

        public void clearText()
        {

            AsOnDate.Text = "";  

        }
         
   }
}