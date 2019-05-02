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
    public partial class WpClaimSummaryReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
          
        DataSetAssetClass imageDataSet = new DataSetAssetClass();

        string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        OracleConnection con = new OracleConnection(connStr);
             
        string ClaimID = Request.QueryString["ClaimID"].ToString();
        string sqlString = "  SELECT PPC.TOTAL_AMOUNT, PPC.CLAIM_DATE AS ENTRY_DATE,  PC.PARTY_ID, PC.PARTY_NAME AS SUPPLIER_NAME, WI.ITEM_NAME, SUM(PPM.ITEM_WEIGHT) AS ITEM_WEIGHT, SUM(CASE WHEN PPM.VAT_AMOUNT = 0  THEN PPM.ITEM_AMOUNT WHEN PPM.VAT_AMOUNT != 0  THEN NULL END) AS NON_VAT_AMOUNT, SUM(CASE WHEN PPM.VAT_AMOUNT != 0  THEN PPM.ITEM_AMOUNT WHEN PPM.VAT_AMOUNT = 0  THEN NULL END) AS FIVE_PERCENT_VAT_AMOUNT, (PPM.ITEM_RATE / 1000) AS ITEM_RATE, SUM(CASE WHEN PPM.VAT_AMOUNT != 0  THEN PPM.VAT_AMOUNT WHEN PPM.VAT_AMOUNT = 0  THEN NULL END) AS VAT_AMOUNT, SUM(PPM.ITEM_AMOUNT)+SUM(PPM.VAT_AMOUNT) AS END_AMT, TO_CHAR(PPC.CLAIM_FOR_MONTH, 'MON-YYYY') AS CLAIM_FOR_MONTH, PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO FROM WP_PURCHASE_CLAIM PPC LEFT JOIN WP_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PPC.EMP_ID LEFT JOIN WP_ITEM WI ON WI.ITEM_ID = PPM.ITEM_ID LEFT JOIN WP_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID WHERE PPC.IS_ACTIVE = 'Enable' AND PPC.CLAIM_NO = '" + ClaimID + "' GROUP BY PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO, PPC.CLAIM_FOR_MONTH, PPC.CLAIM_DATE,  WI.ITEM_NAME, PPM.ITEM_RATE, PPC.TOTAL_AMOUNT, PC.PARTY_ID, PC.PARTY_NAME ORDER BY PPC.CLAIM_NO DESC, PC.PARTY_ID ASC ";  
   
        OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
        dt.Fill(imageDataSet.Tables["reportTable"]);  
        con.Close(); 

        crystalReport.Load(Server.MapPath("~/WP/WP_Reports/Wp_Claim_Summary_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Claim_Summary_Report_(As_On_Date)_Waste_Paper_" + datetime + "";

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