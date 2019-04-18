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

namespace NRCAPPS.MF.MF_Reports
{
    public partial class MfRmIssueReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
          
        DataSetAssetClass imageDataSet = new DataSetAssetClass();

        string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        OracleConnection con = new OracleConnection(connStr);

        string BatchNo = Request.QueryString["BatchNo"].ToString();  

        string sqlString = "   SELECT  MI.ITEM_NAME, MI.ITEM_CODE, CASE  WHEN lag(nvl(MPBTI.ITEM_WEIGHT_CWT,0)) over(order by NULL) = nvl(MPBTI.ITEM_WEIGHT_CWT,0) THEN NULL ELSE nvl(MPBTI.ITEM_WEIGHT_CWT,0)   END AS ITEM_WEIGHT_TARGET, nvl(MPII.FIRST_WT, 0) AS FIRST_WT, nvl(MPII.SECOND_WT, 0) AS SECOND_WT, nvl(MPII.ITEM_WEIGHT, 0) AS ITEM_WEIGHT_ACTUAL, (nvl(MPBTI.ITEM_WEIGHT_CWT, 0) - nvl(MPII.ITEM_WEIGHT, 0)) AS VARIANCE_WT, MPII.TYPE_NAME FROM MF_ITEM MI LEFT JOIN(SELECT ITEM_ID, SUM(ITEM_WEIGHT_CWT) AS ITEM_WEIGHT_CWT FROM MF_PRODUCTION_BAT_TARGET_ITEM WHERE BATCH_NO = '" + BatchNo + "' GROUP BY ITEM_ID)MPBTI ON MPBTI.ITEM_ID = MI.ITEM_ID  LEFT JOIN(SELECT MPI.ITEM_ID, MPI.FIRST_WT, MPI.SECOND_WT, MPI.ITEM_WEIGHT, MPIT.TYPE_ID, MPIT.TYPE_NAME FROM MF_PRODUCTION_ISSUE_ITEM MPI LEFT JOIN MF_PRODUCTION_ISSUE_TYPE MPIT ON MPIT.TYPE_ID = MPI.TYPE_ID WHERE MPI.BATCH_NO = '" + BatchNo + "')MPII ON MPII.ITEM_ID = MI.ITEM_ID WHERE(MPBTI.ITEM_WEIGHT_CWT > 0 OR MPII.ITEM_WEIGHT > 0) ORDER BY MI.ITEM_ID, MPII.TYPE_ID ";
             

        string makeSQL = "  SELECT MPBM.BATCH_NO, MPIM.PRODUCTION_ID, MI.ITEM_NAME, TO_CHAR(MPIM.ENTRY_DATE,'dd-Mon-yyyy') AS ENTRY_DATE FROM MF_PRODUCTION_BATCH_MASTER MPBM LEFT JOIN MF_PRODUCTION_ISSUE_MASTER MPIM ON MPIM.BATCH_NO = MPBM.BATCH_NO LEFT JOIN MF_PRODUCTION_GRADE MPG ON MPG.GRADE_ID = MPBM.GRADE_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID WHERE MPIM.BATCH_NO = '" + BatchNo + "' ";

        OracleConnection conn = new OracleConnection(connStr);
        OracleCommand cmdl;
        OracleDataAdapter oradata;
        DataTable dtc;

        cmdl = new OracleCommand(makeSQL);
        oradata = new OracleDataAdapter(cmdl.CommandText, conn);
        dtc = new DataTable();
        oradata.Fill(dtc);
        int RowCount = dtc.Rows.Count;
        int ProductID = 0; string ItemName = "", EntryDate = "";
        for (int i = 0; i < RowCount; i++)
        {
            ProductID = Convert.ToInt32(dtc.Rows[i]["PRODUCTION_ID"]);
            ItemName = dtc.Rows[i]["ITEM_NAME"].ToString();
            EntryDate = dtc.Rows[i]["ENTRY_DATE"].ToString();
        }
               
        OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
        dt.Fill(imageDataSet.Tables["reportTable"]);  
        con.Close();
              
        crystalReport.Load(Server.MapPath("~/MF/MF_Reports/Mf_Rm_Issue_Production_Wise_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("BatchNo", BatchNo);
        crystalReport.SetParameterValue("ItemName", ItemName);
        crystalReport.SetParameterValue("ProductID", ProductID);
        crystalReport.SetParameterValue("EntryDate", EntryDate);
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Mf_Rm_Issue_Production_Batch_No_"+ BatchNo + "_Report_(As_On_Date)_" + datetime + ""; 
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