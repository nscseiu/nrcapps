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
    public partial class PfSalesSummaryCustomerReportView : System.Web.UI.Page
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
          
          string sqlString = "  SELECT PP.PARTY_ID AS SUPPLIER_ID, PP.PARTY_NAME AS SUPPLIER_NAME, CASE WHEN  PI.ITEM_NAME IS NOT NULL  THEN PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME   WHEN PI.ITEM_NAME IS  NULL THEN PIC.ITEM_NAME || ' ' || PSIC.SUB_ITEM_NAME END AS ITEM_NAME_FULL, CASE WHEN  PI.ITEM_NAME IS NOT NULL  THEN PI.ITEM_NAME WHEN PI.ITEM_NAME IS  NULL THEN PIC.ITEM_NAME END AS ITEM_NAME, sum( CASE WHEN  PPM.ITEM_WEIGHT IS NOT NULL  THEN  PPM.ITEM_WEIGHT   WHEN PPM.ITEM_WEIGHT IS  NULL THEN PEWC.ITEM_WEIGHT / 1000 END) AS ITEM_WEIGHT,  round((sum(CASE WHEN  PPM.ITEM_AMOUNT IS NOT NULL  THEN  PPM.ITEM_AMOUNT   WHEN PPM.ITEM_AMOUNT IS  NULL THEN PEWC.MATERIAL_CONVERSION_AMOUNT END) / sum(CASE WHEN  PPM.ITEM_WEIGHT IS NOT NULL  THEN  PPM.ITEM_WEIGHT   WHEN PPM.ITEM_WEIGHT IS  NULL THEN PEWC.ITEM_WEIGHT / 1000 END)), 1) AS ITEM_RATE, sum( CASE WHEN  PPM.ITEM_AMOUNT IS NOT NULL  THEN  PPM.ITEM_AMOUNT   WHEN PPM.ITEM_AMOUNT IS  NULL THEN PEWC.MATERIAL_CONVERSION_AMOUNT END) AS ITEM_AMOUNT FROM PF_PARTY PP LEFT JOIN PF_SALES_MASTER PPM ON PPM.PARTY_ID = PP.PARTY_ID AND TO_CHAR(TO_DATE(PPM.ENTRY_DATE),'mm/yyyy') = '" + AsMonthYear + "' LEFT JOIN PF_EXPORT_WBSLIP_CON PEWC ON PEWC.PARTY_ID = PP.PARTY_ID AND TO_CHAR(TO_DATE(PEWC.IS_SHIPMENT_COMPLETE_DATE),'mm/yyyy') = '" + AsMonthYear + "' LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_ITEM PIC ON PIC.ITEM_ID = PEWC.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_SUB_ITEM PSIC ON PSIC.SUB_ITEM_ID = PEWC.SUB_ITEM_ID WHERE PPM.ITEM_WEIGHT IS NOT NULL OR PEWC.ITEM_WEIGHT IS NOT NULL GROUP BY PP.PARTY_ID, PP.PARTY_NAME, CASE WHEN  PI.ITEM_NAME IS NOT NULL  THEN PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME   WHEN PI.ITEM_NAME IS  NULL THEN PIC.ITEM_NAME || ' ' || PSIC.SUB_ITEM_NAME END, CASE WHEN  PI.ITEM_NAME IS NOT NULL  THEN PI.ITEM_NAME WHEN PI.ITEM_NAME IS  NULL THEN PIC.ITEM_NAME END ORDER BY PP.PARTY_ID   ";  
  
 
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
        crystalReport.Load(Server.MapPath("~/PF/PF_Reports/Pf_Sales_Summary_Customer_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsMonthYear", AsMonthYearNew); 
        crystalReport.SetParameterValue("PuschaseTypeIDText", PuschaseTypeIDTextNew); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Sales_Summary_Statement_Customer_Report_(As_On_Date)_" + datetime + "";

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