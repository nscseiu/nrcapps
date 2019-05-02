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
    public partial class MsPurchaseMarketersReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
          
        DataSetAssetClass imageDataSet = new DataSetAssetClass();

        string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        OracleConnection con = new OracleConnection(connStr);
             
        string MarketerID = Request.QueryString["MarketerID"].ToString();
        string AsMonthYear = Request.QueryString["MonthYear"].ToString();  
        string MakeMonthYear = Request.QueryString["MonthYear"].ToString();
        string[] MakeMonthYearSplit = MakeMonthYear.Split('-');
        String MonthYearTemp = MakeMonthYearSplit[0].Replace("/", "-");
        DateTime AsMonthYearNew = DateTime.ParseExact(MonthYearTemp, "MM-yyyy", CultureInfo.InvariantCulture);

        string sqlString = "  SELECT PI.ITEM_CODE || ' : ' || PI.ITEM_NAME AS ITEM_NAME, VEHICLE_NO AS SALESMAN_NAME, nvl(SUM(WPM.ITEM_WEIGHT),0) AS ITEM_WEIGHT, nvl(SUM(WPM.ITEM_AMOUNT),0) AS ITEM_AMOUNT, nvl(SUM(WPM.VAT_AMOUNT),0) AS VAT_AMOUNT, nvl(SUM(WPM.ITEM_AMOUNT),0)+nvl(SUM(WPM.VAT_AMOUNT),0) AS TOTAL_AMOUNTFROM  MS_PURCHASE_MASTER WPM LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = WPM.ITEM_ID WHERE TO_CHAR(WPM.ENTRY_DATE, 'mm/yyyy') = '" + AsMonthYear + "' AND VEHICLE_NO = '" + MarketerID + "' GROUP BY PI.ITEM_CODE || ' : ' || PI.ITEM_NAME, VEHICLE_NO ORDER BY PI.ITEM_CODE || ' : ' || PI.ITEM_NAME ASC ";  
   
        OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
        dt.Fill(imageDataSet.Tables["reportTable"]);  
        con.Close(); 

        crystalReport.Load(Server.MapPath("~/MS/MS_Reports/Ms_Market_Wise_Summary_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsMonthYear", AsMonthYearNew); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Metal_Scrap_Market_Wise_Summary_Report_(As_On_Date)_" + datetime + "";

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