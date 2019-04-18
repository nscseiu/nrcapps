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
    public partial class PfJWMonthlyReportView : System.Web.UI.Page
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
         DateTime startDate = AsMonthYearNew.AddMonths(-1); 
         string LastMonth = startDate.ToString("MM/yyyy");

        //    pnlReport.Controls.Add(new LiteralControl(LastMonth));

            string sqlString = "  SELECT PI.ITEM_ID, PI.ITEM_NAME, PI.ITEM_CODE, SUM(NVL(FINAL_STOCK_WT, 0)) AS BEG_FSTOCK_WT, SUM(NVL(PPJ.ITEM_WEIGHT, 0)) AS PURCHASE_WT, SUM(NVL(PPRODJ.ITEM_WEIGHT, 0)) AS PRODUCTION_WT, SUM(NVL(PSJ.ITEM_WEIGHT, 0)) AS SALE_WT, SUM(NVL(PSJ.ITEM_WEIGHT, 0)) - SUM(NVL(PPJ.ITEM_WEIGHT, 0)) AS MAT_AVAIL_PROD_WT, SUM(NVL(FINAL_STOCK_WT, 0)) +(SUM(NVL(PSJ.ITEM_WEIGHT, 0)) - SUM(NVL(PPJ.ITEM_WEIGHT, 0)))  AS END_FSTOCK_WT FROM PF_ITEM PI LEFT JOIN(SELECT ITEM_ID, sum(FINAL_STOCK_WT) AS FINAL_STOCK_WT FROM PF_INVENTORY_JW WHERE TO_CHAR(TO_DATE(CREATE_DATE), 'mm/YYYY') = '" + LastMonth + "'  GROUP BY ITEM_ID) PIJ ON PI.ITEM_ID = PIJ.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_PURCHASE_JW WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm/YYYY') = '" + AsMonthYear + "'  GROUP BY ITEM_ID) PPJ ON PI.ITEM_ID = PPJ.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_PRODUCTION_JW WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm/YYYY') = '" + AsMonthYear + "' GROUP BY ITEM_ID) PPRODJ ON PI.ITEM_ID = PPRODJ.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_SALES_JW WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm/YYYY') = '" + AsMonthYear + "' GROUP BY ITEM_ID) PSJ ON PI.ITEM_ID = PSJ.ITEM_ID GROUP BY PI.ITEM_ID, PI.ITEM_NAME, PI.ITEM_CODE ORDER BY PI.ITEM_ID ";  
  
 
        OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
        dt.Fill(imageDataSet.Tables["reportTable"]);  
        con.Close();
              
        ReportDocument crystalReport = new ReportDocument();
        crystalReport.Load(Server.MapPath("~/PF/PF_Reports/Pf_JW_Monthly.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsMonthYear", AsMonthYearNew); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Pf_JW_Monthly_Report_(As_On_Date)_" + datetime + "";

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