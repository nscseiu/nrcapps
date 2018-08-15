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
    public partial class PfPurchaseSupervisorReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
          
        DataSetAssetClass imageDataSet = new DataSetAssetClass();

        string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        OracleConnection con = new OracleConnection(connStr);

         string AsMonthYear  = Request.QueryString["MonthYear"].ToString();
         string SupervisorID = Request.QueryString["SupervisorID"].ToString();

         string MakeMonthYear = Request.QueryString["MonthYear"].ToString();
         string[] MakeMonthYearSplit = MakeMonthYear.Split('-');
         String MonthYearTemp = MakeMonthYearSplit[0].Replace("/", "-");
         DateTime AsMonthYearNew = DateTime.ParseExact(MonthYearTemp, "MM-yyyy", CultureInfo.InvariantCulture);
      //  string AsMonthYearNew = AsOnDateNewD.ToString("MM-yyyy");


         string sqlString = "  SELECT PS.SUPERVISOR_ID, PS.SUPERVISOR_NAME, PI.ITEM_NAME, PI.ITEM_CODE, PP.PARTY_ID SUPPLIER_ID, PP.PARTY_NAME AS SUPPLIER_NAME, TO_CHAR(TO_DATE(PPM.ENTRY_DATE),'dd/mm/yyyy') AS ENTRY_DATE, sum(PPM.ITEM_WEIGHT) AS ITEM_WEIGHT, ROUND(sum(PPM.ITEM_AMOUNT),0) AS ITEM_AMOUNT FROM PF_PARTY PP LEFT JOIN PF_PURCHASE_MASTER PPM ON PPM.PARTY_ID = PP.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID  LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPM.SUPERVISOR_ID WHERE TO_CHAR(TO_DATE(PPM.ENTRY_DATE),'mm/yyyy') = '" + AsMonthYear + "' AND PPM.SUPERVISOR_ID = '" + SupervisorID + "' GROUP BY PS.SUPERVISOR_ID, PS.SUPERVISOR_NAME, PP.PARTY_ID, PP.PARTY_NAME, PI.ITEM_ID, PI.ITEM_NAME, PI.ITEM_CODE, TO_CHAR(TO_DATE(PPM.ENTRY_DATE),'dd/mm/yyyy') ORDER BY PI.ITEM_ID ASC";  
  
 
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
        crystalReport.Load(Server.MapPath("~/PF/PF_Reports/Pf_Purchase_Supervisor_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsMonthYear", AsMonthYearNew); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Purchase_Supervosor_Statement_Report_(As_On_Date)_" + datetime + "";

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