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

namespace NRCAPPS.MS.MS_Reports
{
    public partial class MsPurchaseSupplierWiseReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
          
        DataSetAssetClass imageDataSet = new DataSetAssetClass();

        string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        OracleConnection con = new OracleConnection(connStr);

            string StartDate = Request.QueryString["StartDate"].ToString();
            string EndDate = Request.QueryString["EndDate"].ToString();
             
            DateTime StartDateNew = DateTime.ParseExact(StartDate, "dd/mm/yyyy", CultureInfo.InvariantCulture);
            string StartDateQuery = StartDateNew.ToString("yyyy/mm/dd");

            DateTime EndDateNew = DateTime.ParseExact(EndDate, "dd/mm/yyyy", CultureInfo.InvariantCulture);
            string EndDateQuery = EndDateNew.ToString("yyyy/mm/dd");

            string StartDateTemp = Request.QueryString["StartDate"].ToString();
            string[] StartDateTempSplit = StartDateTemp.Split('-');
            String StartDateFormTemp = StartDateTempSplit[0].Replace("/", "-");
            DateTime StartDateFormTempNew = DateTime.ParseExact(StartDateFormTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string StartDateForm = StartDateFormTempNew.ToString("dd-MMMM-yyyy");

            string EndDateTemp = Request.QueryString["EndDate"].ToString();
            string[] EndDateTempSplit = EndDateTemp.Split('-');
            String EndDateFormTemp = EndDateTempSplit[0].Replace("/", "-");
            DateTime EndDateFormTempNew = DateTime.ParseExact(EndDateFormTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string EndDateForm = EndDateFormTempNew.ToString("dd-MMMM-yyyy");

            string SupplierID  = Request.QueryString["SupplierID"].ToString(); 
            string DropDownItem = Request.QueryString["DropDownItemID"].ToString();
            string DropDownItemID = DropDownItem.Remove(DropDownItem.Length - 1, 1);
            string[] ItemID =  DropDownItemID.Split('-');
            string sqlString = "";
            if (ItemID[0] == "0")
            { 
                sqlString = "   SELECT PP.PARTY_ID AS SUPPLIER_ID, PP.PARTY_NAME AS SUPPLIER_NAME, PI.ITEM_NAME,  PI.ITEM_CODE, TO_CHAR(TO_DATE(WPM.ENTRY_DATE),'dd/mm/yyyy') AS ENTRY_DATE, WPM.SLIP_NO,  WPM.ITEM_WEIGHT, WPM.ITEM_RATE, WPM.ITEM_AMOUNT, WPM.VAT_PERCENT, WPM.VAT_AMOUNT,  (WPM.ITEM_AMOUNT+WPM.VAT_AMOUNT) as TOTAL_AMOUNT FROM WP_PARTY PP LEFT JOIN WP_PURCHASE_MASTER WPM ON WPM.PARTY_ID = PP.PARTY_ID LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = WPM.ITEM_ID WHERE PP.PARTY_ID = '" + SupplierID + "' AND TO_CHAR(WPM.ENTRY_DATE, 'yyyy/mm/dd') between '" + StartDateQuery + "' AND '" + EndDateQuery + "' ORDER BY  TO_CHAR(TO_DATE(WPM.ENTRY_DATE),'dd/mm/yyyy') ASC, WPM.SLIP_NO ASC ";
            }
            else
            {
                sqlString = "   SELECT PP.PARTY_ID AS SUPPLIER_ID, PP.PARTY_NAME AS SUPPLIER_NAME, PI.ITEM_NAME,  PI.ITEM_CODE, TO_CHAR(TO_DATE(WPM.ENTRY_DATE),'dd/mm/yyyy') AS ENTRY_DATE, WPM.SLIP_NO,  WPM.ITEM_WEIGHT, WPM.ITEM_RATE, WPM.ITEM_AMOUNT, WPM.VAT_PERCENT, WPM.VAT_AMOUNT, (WPM.ITEM_AMOUNT+WPM.VAT_AMOUNT) as TOTAL_AMOUNT FROM WP_PARTY PP LEFT JOIN WP_PURCHASE_MASTER WPM ON WPM.PARTY_ID = PP.PARTY_ID LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = WPM.ITEM_ID WHERE PP.PARTY_ID = '" + SupplierID + "' AND TO_CHAR(WPM.ENTRY_DATE, 'yyyy/mm/dd') between '" + StartDateQuery + "' AND '" + EndDateQuery + "' AND ( ";
                 
                for (int i = 0; i < ItemID.Length; i++)
                {
                    sqlString += " WPM.ITEM_ID = '" + ItemID[i] + "' OR";
                }
                sqlString = sqlString.Remove(sqlString.Length - 2, 2) + " ) ORDER BY  TO_CHAR(TO_DATE(WPM.ENTRY_DATE),'dd/mm/yyyy') ASC, WPM.SLIP_NO ASC ";
            }
            //  pnlReport.GroupingText = sqlString; 

            OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
        dt.Fill(imageDataSet.Tables["reportTable"]);  
        con.Close();
               
        crystalReport.Load(Server.MapPath("~/WP/WP_Reports/Wp_Purchase_Supplier_Wise_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("StartDate", StartDateForm); 
        crystalReport.SetParameterValue("EndDate", EndDateForm); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Purchase_Supplier_Statement_Report_(As_On_Date)_Waste_Paper_" + datetime + "";

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