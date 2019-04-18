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
    public partial class PfPurchasePaymentSummaryReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
          
        DataSetAssetClass imageDataSet = new DataSetAssetClass();

        string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        OracleConnection con = new OracleConnection(connStr);

         string AsMonthYear = Request.QueryString["MonthYear"].ToString();
         string SupplierID  = Request.QueryString["SupplierID"].ToString();

         string MakeMonthYear = Request.QueryString["MonthYear"].ToString();
         string[] MakeMonthYearSplit = MakeMonthYear.Split('-');
         String MonthYearTemp = MakeMonthYearSplit[0].Replace("/", "-");
         DateTime AsMonthYearNew = DateTime.ParseExact(MonthYearTemp, "MM-yyyy", CultureInfo.InvariantCulture);
      //  string AsMonthYearNew = AsOnDateNewD.ToString("MM-yyyy");


         string sqlString = " SELECT PP.PARTY_ID AS SUPPLIER_ID, PP.PARTY_NAME AS SUPPLIER_NAME, PI.ITEM_NAME, PI.ITEM_CODE, PPM.SLIP_NO, PPM.ITEM_WEIGHT, round((PPM.ITEM_AMOUNT)/(PPM.ITEM_WEIGHT),2) AS ITEM_RATE, PPM.ITEM_AMOUNT FROM PF_PARTY PP LEFT JOIN PF_PURCHASE_MASTER PPM ON PPM.PARTY_ID = PP.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID  WHERE PPM.PARTY_ID = '" + SupplierID + "'  AND TO_CHAR(TO_DATE(PPM.ENTRY_DATE),'mm/yyyy') = '" + AsMonthYear + "' ORDER BY PI.ITEM_ID ";  
  
 
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
        crystalReport.Load(Server.MapPath("~/PF/PF_Reports/Pf_Purchase_Payment_Summary_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsMonthYear", AsMonthYearNew); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Purchase_Payment_Summary_Statement_Report_(As_On_Date)_" + datetime + "";

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