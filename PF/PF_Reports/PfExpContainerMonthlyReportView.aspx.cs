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
    public partial class PfExpContainerMonthlyReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
         DataSetAssetClass imageDataSet = new DataSetAssetClass();
         string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

         OracleConnection con = new OracleConnection(connStr);

         string AsMonthYear = Request.QueryString["MonthYear"].ToString();
         string GoodsInID = Request.QueryString["GoodsInID"].ToString();

         string MakeMonthYear = Request.QueryString["MonthYear"].ToString();
         string[] MakeMonthYearSplit = MakeMonthYear.Split('-');
         String MonthYearTemp = MakeMonthYearSplit[0].Replace("/", "-");
         DateTime AsMonthYearNew = DateTime.ParseExact(MonthYearTemp, "MM-yyyy", CultureInfo.InvariantCulture);
            //  string AsMonthYearNew = AsOnDateNewD.ToString("MM-yyyy");
            string sqlString ="";
            if (GoodsInID == "0")
            {
                sqlString = " SELECT PEWC.EXP_WBCON_ID, TO_CHAR(PEWC.DISPATCH_DATE,'dd/mm/yyyy') AS DISPATCH_DATE, PEWC.WB_SLIP_NO, PEWC.CONTAINER_NO, PP.PARTY_NAME AS CUSTOMER_NAME, PEWC.SEAL_NO, PEWC.REL_ORDER_NO, nvl(PEWC.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, PEWC.IS_INVENTORY_STATUS, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME_FULL, (PEWC.ITEM_WEIGHT) AS ITEM_WEIGHT, PS.SALESMAN_NAME FROM PF_EXPORT_WBSLIP_CON PEWC  LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWC.SUB_ITEM_ID LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PEWC.PARTY_ID LEFT JOIN PF_SALESMAN PS ON PS.SALESMAN_ID = PEWC.SALESMAN_ID WHERE TO_CHAR(PEWC.DISPATCH_DATE,'mm/yyyy') = '" + AsMonthYear + "' ORDER BY PEWC.DISPATCH_DATE ASC ";
            }
            else
            {
                sqlString = " SELECT PEWC.EXP_WBCON_ID, TO_CHAR(PEWC.DISPATCH_DATE,'dd/mm/yyyy') AS DISPATCH_DATE, PEWC.WB_SLIP_NO, PEWC.CONTAINER_NO, PP.PARTY_NAME AS CUSTOMER_NAME, PEWC.SEAL_NO, PEWC.REL_ORDER_NO, nvl(PEWC.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, PEWC.IS_INVENTORY_STATUS, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME_FULL, (PEWC.ITEM_WEIGHT) AS ITEM_WEIGHT, PS.SALESMAN_NAME FROM PF_EXPORT_WBSLIP_CON PEWC  LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWC.SUB_ITEM_ID LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PEWC.PARTY_ID LEFT JOIN PF_SALESMAN PS ON PS.SALESMAN_ID = PEWC.SALESMAN_ID WHERE PEWC.IS_INVENTORY_STATUS = '" + GoodsInID + "' AND TO_CHAR(PEWC.DISPATCH_DATE,'mm/yyyy') = '" + AsMonthYear + "' ORDER BY PEWC.DISPATCH_DATE ASC ";
            }
 
        OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
          dt.Fill(imageDataSet.Tables["reportTable"]);
        con.Close();
              
        ReportDocument crystalReport = new ReportDocument();
        crystalReport.Load(Server.MapPath("~/PF/PF_Reports/Pf_ExpContainer_Monthly_Wise_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsMonthYear", AsMonthYearNew); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Purchase_Supplier_Statement_Report_(As_On_Date)_" + datetime + ""; 
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
