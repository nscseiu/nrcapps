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
    public partial class PfProductionSummaryReportView : System.Web.UI.Page
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
      //  string AsMonthYearNew = AsOnDateNewD.ToString("MM-yyyy");


         string sqlString = " SELECT PS.SUPERVISOR_ID, PS.SUPERVISOR_NAME, PPRMA.MACHINE_NUMBER, PPRS.SHIFT_NAME, PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME_FULL, SUM(PPRM.ITEM_WEIGHT) AS ITEM_WEIGHT_IN_FG FROM PF_PRODUCTION_MASTER PPRM LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_PRODUCTION_MACHINE PPRMA ON PPRMA.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_PRODUCTION_SHIFT PPRS ON PPRS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID WHERE TO_CHAR(TO_DATE(PPRM.ENTRY_DATE),'mm/yyyy') = '" + AsMonthYear + "' GROUP BY PS.SUPERVISOR_ID, PS.SUPERVISOR_NAME, PPRMA.MACHINE_NUMBER, PPRS.SHIFT_NAME, PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME ORDER BY PS.SUPERVISOR_NAME, PPRMA.MACHINE_NUMBER,  PPRS.SHIFT_NAME, PI.ITEM_ID ASC";  
  
 
        OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
        dt.Fill(imageDataSet.Tables["reportTable"]);  
        con.Close();
              
        ReportDocument crystalReport = new ReportDocument();
        crystalReport.Load(Server.MapPath("~/PF/PF_Reports/Pf_Production_Summary_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsMonthYear", AsMonthYearNew); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Production_Summary_Report_(As_On_Date)_" + datetime + "";

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