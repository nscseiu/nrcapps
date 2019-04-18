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
    public partial class WpSalesSummaryCustomerReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
          
        DataSetAssetClass imageDataSet = new DataSetAssetClass();

        string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; 
        OracleConnection con = new OracleConnection(connStr);

        string AsMonthYear     = Request.QueryString["MonthYear"].ToString();  
        string MakeMonthYear = Request.QueryString["MonthYear"].ToString();
        string[] MakeMonthYearSplit = MakeMonthYear.Split('-');
        String MonthYearTemp = MakeMonthYearSplit[0].Replace("/", "-");
        DateTime AsMonthYearNew = DateTime.ParseExact(MonthYearTemp, "MM-yyyy", CultureInfo.InvariantCulture); 
          
        string sqlString = " SELECT WSM.INVOICE_NO, PARTY_NAME AS CUSTOMER_NAME, WI.ITEM_NAME, WSM.ITEM_WEIGHT, WSM.ITEM_RATE, WSM.ITEM_AMOUNT, WSM.VAT_AMOUNT, (WSM.ITEM_AMOUNT + WSM.VAT_AMOUNT) AS TOTAL_AMOUNT, TO_CHAR(TO_DATE(WSM.ENTRY_DATE),'dd-MON-yyyy') AS ENTRY_DATE FROM WP_SALES_MASTER WSM LEFT JOIN WP_PARTY WP ON WP.PARTY_ID = WSM.PARTY_ID LEFT JOIN WP_ITEM WI ON WI.ITEM_ID = WSM.ITEM_ID WHERE TO_CHAR(TO_DATE(WSM.ENTRY_DATE),'mm/yyyy') = '" + AsMonthYear + "' ORDER BY TO_CHAR(TO_DATE(WSM.ENTRY_DATE), 'yyyy-mm-dd'), WSM.INVOICE_NO   ";  
  
 
        OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
        dt.Fill(imageDataSet.Tables["reportTable"]);  
        con.Close();
             
        crystalReport.Load(Server.MapPath("~/WP/WP_Reports/Wp_Sales_Summary_Customer_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsMonthYear", AsMonthYearNew); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Wastepaper_Sales_Summary_Statement_Customer_Report_(As_On_Date)_" + datetime + "";
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