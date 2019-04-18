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

namespace NRCAPPS.WP.WP_Reports
{
    public partial class WpPurchaseSummaryReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
          
        DataSetAssetClass imageDataSet = new DataSetAssetClass();

        string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        OracleConnection con = new OracleConnection(connStr);
             

            string StartDate = Request.QueryString["StartDate"].ToString();
            string EndDate = Request.QueryString["EndDate"].ToString();

            DateTime StartDateNew = DateTime.ParseExact(StartDate, "dd/mm/yyyy", CultureInfo.InvariantCulture);
            string StartDateQuery = StartDateNew.ToString("yyyy/mm/dd");

            DateTime EndDateNew = DateTime.ParseExact(EndDate, "dd/mm/yyyy", CultureInfo.InvariantCulture);
            string EndDateQuery = EndDateNew.ToString("yyyy/mm/dd");

            string StartDateTemp = Request.QueryString["StartDate"].ToString();
            string[] StartDateTempSplit = StartDateTemp.Split('-');
            String StartDateFormTemp = StartDateTempSplit[0].Replace("/", "-");
            DateTime StartDateFormTempNew = DateTime.ParseExact(StartDateFormTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string StartDateForm = StartDateFormTempNew.ToString("dd-MMMM-yyyy");

            string EndDateTemp = Request.QueryString["EndDate"].ToString();
            string[] EndDateTempSplit = EndDateTemp.Split('-');
            String EndDateFormTemp = EndDateTempSplit[0].Replace("/", "-");
            DateTime EndDateFormTempNew = DateTime.ParseExact(EndDateFormTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string EndDateForm = EndDateFormTempNew.ToString("dd-MMMM-yyyy");
             
     
    //    string sqlString = " SELECT WP.PARTY_ID AS SUPPLIER_ID, WP.PARTY_NAME AS SUPPLIER_NAME,  PI.ITEM_NAME, PI.ITEM_CODE, sum(WPM.ITEM_WEIGHT) AS ITEM_WEIGHT, nvl(WPM.ITEM_RATE, 0) AS ITEM_RATE, sum((WPM.ITEM_AMOUNT) + nvl(WPM.VAT_AMOUNT, 0))  AS ITEM_AMOUNT FROM WP_PARTY WP LEFT JOIN WP_PURCHASE_MASTER WPM ON WPM.PARTY_ID = WP.PARTY_ID LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = WPM.ITEM_ID WHERE TO_CHAR(TO_DATE(WPM.ENTRY_DATE),'mm/yyyy')  = '" + AsMonthYear + "'  GROUP BY WP.PARTY_ID, WP.PARTY_NAME, PI.ITEM_NAME, PI.ITEM_CODE, WPM.ITEM_RATE ORDER BY WP.PARTY_NAME ";  
  
         string sqlString = "  SELECT WP.PARTY_ID AS SUPPLIER_ID, WP.PARTY_NAME AS SUPPLIER_NAME,  PI.ITEM_NAME, WPM.ITEM_WEIGHT AS ITEM_WEIGHT, nvl(WPM.ITEM_RATE, 0) AS ITEM_RATE,  (WPM.ITEM_AMOUNT) + nvl(WPM.VAT_AMOUNT, 0)  AS ITEM_AMOUNT FROM WP_PARTY WP LEFT JOIN WP_PURCHASE_MASTER WPM ON WPM.PARTY_ID = WP.PARTY_ID LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = WPM.ITEM_ID WHERE TO_CHAR(WPM.ENTRY_DATE, 'yyyy/mm/dd') between '" + StartDateQuery + "' AND '" + EndDateQuery + "'  ORDER BY WP.PARTY_NAME, WPM.ITEM_ID, WPM.ITEM_RATE  ";  
  
        OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
        dt.Fill(imageDataSet.Tables["reportTable"]);  
        con.Close();
               
        crystalReport.Load(Server.MapPath("~/WP/WP_Reports/Wp_Purchase_Summary_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]); 
        crystalReport.SetParameterValue("StartDate", StartDateForm);
        crystalReport.SetParameterValue("EndDate", EndDateForm);
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Purchase_Summary_(Material_Collection)_Statement_Report_(As_On_Date)_" + datetime + "";

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