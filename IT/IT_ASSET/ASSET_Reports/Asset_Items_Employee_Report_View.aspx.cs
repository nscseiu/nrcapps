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
    public partial class Asset_Items_Employee_Report_View : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
           string EmployeeID = Request.QueryString["DropDownEmployeeIDReport"].ToString(); 

             /* 
                string makeSQL = " select AEI.EMP_ITEMS_ID, HE.EMP_FNAME || ' ' || HE.EMP_LNAME AS EMP_NAME, AEI.ITEM_ID, AEI.IMAGE_QR_CODE, AEI.IS_ACTIVE, AI.ITEM_NAME, AI.ITEM_TYPE, AI.ITEM_BRAND from ASSET_EMP_ITEMS AEI left join ASSET_ITEMS AI ON AI.ITEM_ID = AEI.ITEM_ID left join HR_EMPLOYEES HE ON HE.EMP_ID = AEI.EMP_ID order by AEI.EMP_ITEMS_ID";
             
                var dt = ExecuteBySqlString(makeSQL);
                crystalReport.Load(Server.MapPath("~/ASSET/ASSET_Reports/Asset_Items_Employee_Report.rpt"));
                crystalReport.SetDataSource(dt);
             
                //  crystalReport.SetParameterValue("AsOnDate", EmployeeID);
                //  crystalReport.SetParameterValue("branchName", EmployeeID);
           
            string datetime = DateTime.Now.ToString("dd-MM-yyyy");
            CrystalReportViewer1.ID = "Asset_Items_Employee_Report_" + datetime + "";
            CrystalReportViewer1.ReportSource = crystalReport; 
            */
 
            DataSetAssetClass imageDataSet = new DataSetAssetClass();

            string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; 
            OracleConnection con = new OracleConnection(connStr);
             
            string sqlString = " SELECT HE.EMP_ID, HE.EMP_FNAME || ' ' || EMP_LNAME AS EMP_NAME, CASE WHEN HEDE.DEPARTMENT_NAME IS NOT NULL  THEN HEDE.DEPARTMENT_NAME WHEN HEDED.DEPARTMENT_NAME IS NOT NULL  THEN HEDED.DEPARTMENT_NAME END AS DEPARTMENT_NAME, CASE WHEN HED.DIVISION_NAME IS NOT NULL  THEN HED.DIVISION_NAME WHEN HEDD.DIVISION_NAME IS NOT NULL  THEN HEDD.DIVISION_NAME END AS DIVISION_NAME, AIC.ITEM_CATEGORY_NAME, AI.ITEM_TYPE, AI.ITEM_BRAND, AEI.EMP_ITEMS_ID, AEI.IMAGE_QR_CODE, AI.ITEM_NAME FROM  IT_ASSET_EMP_ITEMS AEI LEFT JOIN IT_ASSET_ITEMS AI ON AI.ITEM_ID = AEI.ITEM_ID LEFT JOIN IT_ASSET_ITEM_CATEGORIES AIC ON AIC.ITEM_CATEGORY_ID = AI.ITEM_CATEGORY_ID LEFT JOIN HR_EMPLOYEES HE  ON HE.EMP_ID = AEI.EMP_ID LEFT JOIN  HR_EMP_DIVISIONS HED ON HED.DIVISION_ID = HE.DIVISION_ID LEFT JOIN HR_EMP_DEPARTMENTS HEDE ON HEDE.DEPARTMENT_ID = HE.DEPARTMENT_ID LEFT JOIN HR_EMP_DEPARTMENTS HEDED ON HEDED.DEPARTMENT_ID = AEI.DEPARTMENT_ID LEFT JOIN  HR_EMP_DIVISIONS HEDD ON HEDD.DIVISION_ID = AEI.DIVISION_ID WHERE AI.IS_ACTIVE = 'Enable'  ORDER BY HE.EMP_ID ASC ";
             
            OracleCommand cmd = new OracleCommand(sqlString, con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sqlString; 
            OracleDataAdapter dt = new OracleDataAdapter(cmd);  
            con.Open();
            dt.Fill(imageDataSet.Tables["reportTable"]);  
            con.Close();  
  
            for (int rowNumber = 0; rowNumber < imageDataSet.Tables["reportTable"].Rows.Count; rowNumber++) 
            { 
                string imgName = Server.MapPath("~/IT/IT_ASSET/QRCode/"+imageDataSet.Tables["reportTable"].Rows[rowNumber]["IMAGE_QR_CODE"].ToString()); 
                DisplayImages(imageDataSet.Tables["reportTable"].Rows[rowNumber],"Image",imgName); 
            }
     
            crystalReport.Load(Server.MapPath("~/IT/IT_ASSET/ASSET_Reports/Asset_Items_Employee_Report.rpt"));
            crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);

            string datetime = DateTime.Now.ToString("dd-MM-yyyy");
            CrystalReportViewer1.ID = "IT_Asset_Items_&_Employee_Report_" + datetime + "";  
            CrystalReportViewer1.ReportSource = crystalReport;     
       }
 
        private void DisplayImages(DataRow row, string img, string ImagePath) 
        { 
            FileStream stream = new FileStream(ImagePath, FileMode.Open, FileAccess.Read); 
            byte[] ImgData = new byte[stream.Length]; 
            stream.Read(ImgData, 0, Convert.ToInt32(stream.Length)); 
            stream.Close(); 
            row[img] = ImgData; 
        } 
         
        protected void CrystalReportViewer1_Unload(object sender, EventArgs e)
        {
            crystalReport.Close();
            crystalReport.Dispose();
            CrystalReportViewer1.Dispose();
        }
    }
}