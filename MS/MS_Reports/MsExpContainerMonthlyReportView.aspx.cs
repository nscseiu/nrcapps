using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OracleClient;
using System.Configuration;
using System.Data.SqlClient; 
using System.IO;
using CrystalDecisions.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Globalization;
using NRCAPPS.PF.PF_Reports;

namespace NRCAPPS.MS.MS_Reports
{
    public partial class MsExpContainerMonthlyReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
         DataSetAssetClass imageDataSet = new DataSetAssetClass();
         string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

         OracleConnection con = new OracleConnection(connStr);

         string AsMonthYear = Request.QueryString["MonthYear"].ToString();
         string GoodsInID = Request.QueryString["GoodsInID"].ToString();
         string IsReport  = Request.QueryString["IsReport"].ToString();

         string MakeMonthYear = Request.QueryString["MonthYear"].ToString(); 
         string[] MakeMonthYearSplit = MakeMonthYear.Split('-');
         String MonthYearTemp = MakeMonthYearSplit[0].Replace("/", "-");
         DateTime AsMonthYearNew = DateTime.ParseExact(MonthYearTemp, "MM-yyyy", CultureInfo.InvariantCulture);
            //  string AsMonthYearNew = AsOnDateNewD.ToString("MM-yyyy");
            string sqlString ="", MonthOf ="";
            if (GoodsInID == "0")
            {
                if (IsReport == "CurrentMonth") {
                    MonthOf = "Month Of";
                    sqlString = "  SELECT PEWC.EXP_WBCON_ID, TO_CHAR(PEWC.DISPATCH_DATE,'dd/mm/yyyy') AS DISPATCH_DATE, PEWC.WB_SLIP_NO, PEWC.CONTAINER_NO,  PEWC.REL_ORDER_NO,  nvl(PEWC.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, PEWCI.IS_INVENTORY_STATUS, PI.ITEM_NAME, PEWCI.ITEM_WEIGHT, PEWCI.ITEM_BALES FROM WP_EXPORT_WBSLIP_CON PEWC LEFT JOIN WP_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID WHERE TO_CHAR(PEWC.DISPATCH_DATE,'mm/yyyy') = '" + AsMonthYear + "'  ORDER BY PEWC.DISPATCH_DATE ASC ";
                 } else {
                    MonthOf = "Till Month Of";
                    sqlString = "  SELECT PEWC.EXP_WBCON_ID, TO_CHAR(PEWC.DISPATCH_DATE,'dd/mm/yyyy') AS DISPATCH_DATE, PEWC.WB_SLIP_NO, PEWC.CONTAINER_NO,  PEWC.REL_ORDER_NO,  nvl(PEWC.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, PEWCI.IS_INVENTORY_STATUS, PI.ITEM_NAME, PEWCI.ITEM_WEIGHT, PEWCI.ITEM_BALES FROM WP_EXPORT_WBSLIP_CON PEWC LEFT JOIN WP_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID WHERE TO_CHAR(PEWC.DISPATCH_DATE,'mm/yyyy') <= '" + AsMonthYear + "'  ORDER BY PEWC.DISPATCH_DATE ASC ";
                 }
            }
            else
            {
                if (IsReport == "CurrentMonth")
                {
                    MonthOf = "Month Of";
                    sqlString = " SELECT PEWC.EXP_WBCON_ID, TO_CHAR(PEWC.DISPATCH_DATE,'dd/mm/yyyy') AS DISPATCH_DATE, PEWC.WB_SLIP_NO, PEWC.CONTAINER_NO,  PEWC.REL_ORDER_NO,  nvl(PEWC.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, PEWCI.IS_INVENTORY_STATUS, PI.ITEM_NAME, PEWCI.ITEM_WEIGHT, PEWCI.ITEM_BALES FROM WP_EXPORT_WBSLIP_CON PEWC LEFT JOIN WP_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID WHERE PEWCI.IS_INVENTORY_STATUS = '" + GoodsInID + "' AND TO_CHAR(PEWC.DISPATCH_DATE,'mm/yyyy') = '" + AsMonthYear + "'  ORDER BY PEWC.DISPATCH_DATE ASC ";
                }
                else
                {
                    MonthOf = "Till Month Of";
                    sqlString = " SELECT PEWC.EXP_WBCON_ID, TO_CHAR(PEWC.DISPATCH_DATE,'dd/mm/yyyy') AS DISPATCH_DATE, PEWC.WB_SLIP_NO, PEWC.CONTAINER_NO,  PEWC.REL_ORDER_NO,  nvl(PEWC.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, PEWCI.IS_INVENTORY_STATUS, PI.ITEM_NAME, PEWCI.ITEM_WEIGHT, PEWCI.ITEM_BALES FROM WP_EXPORT_WBSLIP_CON PEWC LEFT JOIN WP_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID WHERE PEWCI.IS_INVENTORY_STATUS = '" + GoodsInID + "' AND TO_CHAR(PEWC.DISPATCH_DATE,'mm/yyyy') <= '" + AsMonthYear + "'  ORDER BY PEWC.DISPATCH_DATE ASC ";

                }
            }
 
        OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
          dt.Fill(imageDataSet.Tables["reportTable"]);
        con.Close();
               
        crystalReport.Load(Server.MapPath("~/WP/WP_Reports/Wp_ExpContainer_Monthly_Wise_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("MonthOf", MonthOf);
        crystalReport.SetParameterValue("AsMonthYear", AsMonthYearNew); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Purchase_Supplier_Statement_Report_(As_On_Date)_" + datetime + ""; 
        CrystalReportViewer1.ReportSource = crystalReport;
      
    }
   
        protected void CrystalReportViewer1_Unload(object sender, EventArgs e)
        {
            crystalReport.Close();
            crystalReport.Dispose();
            CrystalReportViewer1.Dispose();
        }
    }
} 
