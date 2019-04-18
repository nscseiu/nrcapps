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
    public partial class MfItemTransferMonthWiseReportView : System.Web.UI.Page
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
             
         string sqlString = "  SELECT TO_CHAR(TO_DATE(MMTM.ENTRY_DATE), 'dd-Mon-yyyy') AS ENTRY_DATE, MMTM.WB_SLIP_NO, MV.VEHICLE_NO, MM.ITEM_NAME, MM.ITEM_CODE, MMTM.FIRST_WT_MF, MMTM.SECOND_WT_MF, MMTM.NET_WT_MF, MMTM.NET_WT_MS, MMTM.VARIANCE AS VARIANCE_WT, MIB.ITEM_BIN_NAME, MMTM.REMARKS FROM MF_PURCHASE_TRANSFER_MASTER MMTM LEFT JOIN MF_VEHICLE MV ON MV.VEHICLE_ID = MMTM.VEHICLE_ID LEFT JOIN MF_ITEM MM ON MM.ITEM_ID = MMTM.ITEM_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MMTM.ITEM_BIN_ID WHERE TO_CHAR(TO_DATE(MMTM.ENTRY_DATE), 'mm/yyyy') = '" + AsMonthYear + "' ORDER BY TO_CHAR(TO_DATE(MMTM.ENTRY_DATE), 'yyyy-mm-dd') ASC ";  
   
        OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
        dt.Fill(imageDataSet.Tables["reportTable"]);  
        con.Close();
              
        crystalReport.Load(Server.MapPath("~/MF/MF_Reports/Mf_Item_Trnasfer_Wise_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsMonthYear", AsMonthYearNew); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Mf_Item_Trnasfer_Wise_Report_(As_On_Date)_" + datetime + ""; 
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