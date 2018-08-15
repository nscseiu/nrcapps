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
  
namespace NRCAPPS.PF.PF_Reports
{
    public partial class PfInventoryMonthlyStatementReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
          
        DataSetAssetClass imageDataSet = new DataSetAssetClass();

        string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        OracleConnection con = new OracleConnection(connStr);

        string AsMonthYear = Request.QueryString["MonthYear"].ToString();

        string MakeMonthYear = Request.QueryString["MonthYear"].ToString();
        string[] MakeMonthYearSplit = MakeMonthYear.Split('-');
        String MonthYearTemp = MakeMonthYearSplit[0].Replace("/", "-");
        DateTime AsMonthYearNew = DateTime.ParseExact(MonthYearTemp, "MM-yyyy", CultureInfo.InvariantCulture);

        DateTime curDate = AsMonthYearNew;
        DateTime startDate = curDate.AddMonths(-1).AddDays(1 - curDate.Day);
        DateTime endDate = startDate.AddMonths(1).AddDays(-1);
        string LastDate = endDate.ToString("dd-MM-yyyy");
        string LastMonth = endDate.ToString("MM-yyyy");
        string CurrentMonth = AsMonthYearNew.ToString("MM-yyyy");

        string sqlString = " SELECT SUM(nvl(BEGPRSIH.FINAL_STOCK_WT,0)) AS BEG_FSTOCK_WT, SUM(nvl(PPM.ITEM_WEIGHT,0)) AS PURCHASE_WT, SUM(nvl(PPMD.ITEM_WEIGHT,0)) AS PURCHASE_WTD, SUM((nvl(BEGPRSIH.FINAL_STOCK_WT,0) + nvl(PPM.ITEM_WEIGHT,0)) - nvl(PPMD.ITEM_WEIGHT,0)) AS MAT_AVAIL_PROD_WT, SUM(nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)) AS PRODUCTION_WT, SUM(nvl(PGAR.PGE_WEIGHT,0)) AS GAR_EST_OF_PROD, SUM(nvl(PAGAR.ACTUAL_GAR_WEIGHT,0)) AS ACTUAL_GAR_WEIGHT, SUM(nvl(BEGPFSIH.FINAL_STOCK_WT,0)) AS BEG_FSTOCK_WT1, SUM(nvl(PPMD.ITEM_WEIGHT,0)) AS PURCHASE_WTD1, SUM(nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)) AS PRODUCTION_WT1, SUM((nvl(BEGPFSIH.FINAL_STOCK_WT,0) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)))   AS AVAIL_SALE_WT, SUM(nvl(PSM.ITEM_WEIGHT,0)) AS SALE_WT, SUM(nvl(PSML.ITEM_WEIGHT,0)) AS SALE_LOCAL_WT, SUM(nvl(PPW.ITEM_WEIGHT,0)) AS PURCHASE_JW_WT, SUM(nvl(PPROW.ITEM_WEIGHT,0)) AS PRODUCTION_JW_WT, SUM(nvl(PSW.ITEM_WEIGHT,0)) AS SALES_JW_WT FROM PF_ITEM PI LEFT JOIN (SELECT ITEM_ID, FINAL_STOCK_WT, ITEM_AVG_RATE FROM PF_RM_STOCK_INVENTORY_HISTORY WHERE TO_CHAR(TO_DATE(CREATE_DATE), 'dd-mm-YYYY') = '" + LastDate + "' ) BEGPRSIH ON PI.ITEM_ID = BEGPRSIH.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT,  sum(nvl(ITEM_AMOUNT,0)) AS ITEM_AMOUNT FROM PF_PURCHASE_MASTER  WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + LastMonth + "' GROUP BY ITEM_ID) BEGPPM ON PI.ITEM_ID = BEGPPM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT, sum(ITEM_AMOUNT) AS ITEM_AMOUNT FROM PF_PURCHASE_MASTER  WHERE  TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' GROUP BY ITEM_ID) PPM ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_SALES_MASTER  WHERE PUR_TYPE_ID = 1  AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' GROUP BY ITEM_ID) PPMD ON PI.ITEM_ID = PPMD.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT_IN_FG FROM PF_PRODUCTION_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' GROUP BY ITEM_ID) PPMAS ON PI.ITEM_ID = PPMAS.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(PGE_WEIGHT) AS PGE_WEIGHT  FROM PF_GARBAGE WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) PGAR ON PI.ITEM_ID = PGAR.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ACTUAL_GAR_WEIGHT) AS ACTUAL_GAR_WEIGHT FROM PF_ACTUAL_GARBAGE WHERE TO_CHAR(TO_DATE(MONTH_YEAR), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) PAGAR ON PI.ITEM_ID = PAGAR.ITEM_ID LEFT JOIN (SELECT ITEM_ID,  FINAL_STOCK_WT, ITEM_AVG_RATE FROM PF_FG_STOCK_INVENTORY_HISTORY WHERE TO_CHAR(TO_DATE(CREATE_DATE), 'dd-mm-YYYY') = '" + LastDate + "' ) BEGPFSIH ON PI.ITEM_ID = BEGPFSIH.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_SALES_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' AND SALES_MODE = 'Local'  GROUP BY ITEM_ID) PSML ON PI.ITEM_ID = PSML.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_SALES_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' AND SALES_MODE = 'Export'  GROUP BY ITEM_ID) PSM ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_PURCHASE_JW WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') =  '" + CurrentMonth + "'  GROUP BY ITEM_ID) PPW ON PI.ITEM_ID = PPW.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_PRODUCTION_JW WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) PPROW ON PI.ITEM_ID = PPROW.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_SALES_JW WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' GROUP BY ITEM_ID) PSW ON PI.ITEM_ID = PSW.ITEM_ID "; //'" + AsOnDate + "' 
  
 
        OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
        dt.Fill(imageDataSet.Tables["reportTable"]);  
        con.Close();
             
             
        ReportDocument crystalReport = new ReportDocument();
        crystalReport.Load(Server.MapPath("~/PF/PF_Reports/Pf_Inventory_Monthly_Statement_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsMonthYear", AsMonthYearNew); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Inventory_Monthly_Statement_Report_(As_On_Date)_" + datetime + "";

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