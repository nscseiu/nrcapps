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

  
namespace NRCAPPS.IT.IT_ASSET.ASSET_Reports
{
    public partial class Asset_Items_Department_Report_View : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
          // string EmployeeID = Request.QueryString["DropDownEmployeeIDReport"].ToString(); 
             
            DataSetAssetClass imageDataSet = new DataSetAssetClass();

            string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; 
            OracleConnection con = new OracleConnection(connStr);
             
            string sqlString = "  SELECT HE.EMP_ID, HE.EMP_FNAME || ' ' || EMP_LNAME AS EMP_NAME, CASE WHEN HEDE.DEPARTMENT_NAME IS NOT NULL  THEN HEDE.DEPARTMENT_NAME WHEN HEDED.DEPARTMENT_NAME IS NOT NULL  THEN HEDED.DEPARTMENT_NAME END AS DEPARTMENT_NAME, CASE WHEN HED.DIVISION_NAME IS NOT NULL  THEN HED.DIVISION_NAME WHEN HEDD.DIVISION_NAME IS NOT NULL  THEN HEDD.DIVISION_NAME END AS DIVISION_NAME, AIC.ITEM_CATEGORY_NAME, AI.ITEM_TYPE, AI.ITEM_BRAND, AEI.EMP_ITEMS_ID, AEI.IMAGE_QR_CODE, AI.ITEM_NAME, HE.EMAIL, IAEIE.ITEM_EXP_ID,  (EXTRACT (DAY FROM (IAEIE.EXPIRES_DATE-SYSDATE)))  AS EXPIRED_DAYS FROM  IT_ASSET_EMP_ITEMS AEI LEFT JOIN IT_ASSET_ITEMS AI ON AI.ITEM_ID = AEI.ITEM_ID LEFT JOIN IT_ASSET_ITEM_CATEGORIES AIC ON AIC.ITEM_CATEGORY_ID = AI.ITEM_CATEGORY_ID LEFT JOIN HR_EMPLOYEES HE  ON HE.EMP_ID = AEI.EMP_ID LEFT JOIN  HR_EMP_DIVISIONS HED ON HED.DIVISION_ID = HE.DIVISION_ID LEFT JOIN HR_EMP_DEPARTMENTS HEDE ON HEDE.DEPARTMENT_ID = HE.DEPARTMENT_ID LEFT JOIN HR_EMP_DEPARTMENTS HEDED ON HEDED.DEPARTMENT_ID = AEI.DEPARTMENT_ID LEFT JOIN  HR_EMP_DIVISIONS HEDD ON HEDD.DIVISION_ID = AEI.DIVISION_ID LEFT JOIN (SELECT * FROM IT_ASSET_EMP_ITEM_EXPIRES)IAEIE ON (IAEIE.EMP_ID = AEI.EMP_ID AND IAEIE.ITEM_ID = AEI.ITEM_ID) OR (IAEIE.DEPARTMENT_ID = AEI.DEPARTMENT_ID AND IAEIE.ITEM_ID = AEI.ITEM_ID) WHERE AI.IS_ACTIVE = 'Enable'  ORDER BY HE.EMP_ID ASC ";
             
            OracleCommand cmd = new OracleCommand(sqlString, con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sqlString; 
            OracleDataAdapter dt = new OracleDataAdapter(cmd);  
            con.Open();
            dt.Fill(imageDataSet.Tables["reportTable"]);  
            con.Close();  
    
            crystalReport.Load(Server.MapPath("~/IT/IT_ASSET/ASSET_Reports/Asset_Items_Department_Report.rpt"));
            crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);

            string datetime = DateTime.Now.ToString("dd-MM-yyyy");
            CrystalReportViewer1.ID = "IT_Asset_Comprehensive_Report_" + datetime + "";  
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