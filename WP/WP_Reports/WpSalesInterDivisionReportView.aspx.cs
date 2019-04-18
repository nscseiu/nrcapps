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
    public partial class WpSalesInterDivisionReportView : System.Web.UI.Page
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

        string  sqlString = "  SELECT PSM.SALES_INTER_DIV_ID, PSM.WB_SLIP_NO, HED.DIVISION_NAME AS CUSTOMER_NAME, PI.ITEM_NAME, PSI.CATEGORY_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.REMARKS AS SEAL_NO,  TO_CHAR(PSM.ENTRY_DATE, 'DD-Mon-YYYY') AS ENTRY_DATE  FROM WP_SALES_INTER_DIV_MASTER PSM LEFT JOIN HR_EMP_DIVISIONS HED ON HED.DIVISION_ID = PSM.INTER_DIVISION_ID LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN WP_CATEGORY PSI ON PSI.CATEGORY_ID = PSM.CATEGORY_ID  WHERE  to_char(PSM.ENTRY_DATE, 'mm/yyyy') = '" + AsMonthYear + "' ORDER BY PSM.CREATE_DATE DESC ";
             
             
        OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
        dt.Fill(imageDataSet.Tables["reportTable"]);  
        con.Close();
               
        crystalReport.Load(Server.MapPath("~/WP/WP_Reports/Wp_Sales_Inter_Division_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsMonthYear", AsMonthYearNew);
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Sales_Sales_Inter_Division_Report_(As_On_Date)_Waste_Paper_" + datetime + "";

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