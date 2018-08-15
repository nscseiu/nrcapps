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
    public partial class PfPurchaseSummaryReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
          
        DataSetAssetClass imageDataSet = new DataSetAssetClass();

        string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        OracleConnection con = new OracleConnection(connStr);

         string AsMonthYear     = Request.QueryString["MonthYear"].ToString();
         int PurchaseTypeID  = Convert.ToInt32(Request.QueryString["PurchaseTypeID"]);

         string MakeMonthYear = Request.QueryString["MonthYear"].ToString();
         string[] MakeMonthYearSplit = MakeMonthYear.Split('-');
         String MonthYearTemp = MakeMonthYearSplit[0].Replace("/", "-");
         DateTime AsMonthYearNew = DateTime.ParseExact(MonthYearTemp, "MM-yyyy", CultureInfo.InvariantCulture);
      //  string AsMonthYearNew = AsOnDateNewD.ToString("MM-yyyy");
         string PuschaseTypeIDText = "", PuschaseTypeIDTextNew = "";
          if(PurchaseTypeID == 1){
               PuschaseTypeIDText = "D";
               PuschaseTypeIDTextNew = "Direct";
          } else {
               PuschaseTypeIDText = "R";
               PuschaseTypeIDTextNew = "Reguler";
          }
          string sqlString = " SELECT PP.PARTY_ID AS SUPPLIER_ID, PP.PARTY_NAME AS SUPPLIER_NAME, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME_FULL, PI.ITEM_NAME, PI.ITEM_CODE, sum(PPM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PPM.ITEM_RATE) AS ITEM_RATE, sum(PPM.ITEM_AMOUNT) AS ITEM_AMOUNT FROM PF_PARTY PP LEFT JOIN PF_PURCHASE_MASTER PPM ON PPM.PARTY_ID = PP.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_PURCHASE_TYPE PPT ON PPT.PUR_TYPE_ID = PPM.PUR_TYPE_ID WHERE TO_CHAR(TO_DATE(PPM.ENTRY_DATE),'mm/yyyy') = '" + AsMonthYear + "' AND PPT.PUR_TYPE_NAME =  '" + PuschaseTypeIDText + "' GROUP BY PP.PARTY_ID, PP.PARTY_NAME, PI.ITEM_NAME, PI.ITEM_CODE,  PSI.SUB_ITEM_NAME ";  
  
 
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
        crystalReport.Load(Server.MapPath("~/PF/PF_Reports/Pf_Purchase_Summary_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsMonthYear", AsMonthYearNew); 
        crystalReport.SetParameterValue("PuschaseTypeIDText", PuschaseTypeIDTextNew); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Purchase_Summary_Statement_Report_(As_On_Date)_" + datetime + "";

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