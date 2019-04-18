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
    public partial class PfSalesSummaryDirectReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
          
        DataSetAssetClass imageDataSet = new DataSetAssetClass();

        string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        OracleConnection con = new OracleConnection(connStr);

         string AsMonthYear     = Request.QueryString["MonthYear"].ToString();
      //   int PurchaseTypeID  = Convert.ToInt32(Request.QueryString["PurchaseTypeID"]);

         string MakeMonthYear = Request.QueryString["MonthYear"].ToString();
         string[] MakeMonthYearSplit = MakeMonthYear.Split('-');
         String MonthYearTemp = MakeMonthYearSplit[0].Replace("/", "-");
         DateTime AsMonthYearNew = DateTime.ParseExact(MonthYearTemp, "MM-yyyy", CultureInfo.InvariantCulture);
      //  string AsMonthYearNew = AsOnDateNewD.ToString("MM-yyyy");
         string PuschaseTypeIDText = "", PuschaseTypeIDTextNew = "";
          
          string sqlString = "  SELECT PSM.INVOICE_NO, PP.PARTY_ID AS SUPPLIER_ID, PP.PARTY_NAME AS SUPPLIER_NAME, PI.ITEM_NAME AS ITEM_NAME_FULL, PSI.SUB_ITEM_NAME AS ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ENTRY_DATE FROM PF_SALES_MASTER PSM LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PSM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID WHERE PSM.PUR_TYPE_ID = 1 AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm/yyyy')  = '" + AsMonthYear + "'   ";  
  
 
        OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
        dt.Fill(imageDataSet.Tables["reportTable"]);  
        con.Close();

      //  string AsOnDateGet = Request.QueryString["AsOnDate"].ToString();
     //   var AsOnDateGet = DateTime.Parse(AsOnDate);
             
        ReportDocument crystalReport = new ReportDocument();
        crystalReport.Load(Server.MapPath("~/PF/PF_Reports/Pf_Sales_Direct_Summary_Customer_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsMonthYear", AsMonthYearNew); 
        crystalReport.SetParameterValue("PuschaseTypeIDText", PuschaseTypeIDTextNew); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Sales_Direct_Summary_Statement_Report_(As_On_Date)_" + datetime + "";

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