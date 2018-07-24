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
    public partial class PfFgStatementReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
          
        DataSetAssetClass imageDataSet = new DataSetAssetClass();

        string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        OracleConnection conn = new OracleConnection(connStr);
        OracleCommand cmdl;
        OracleDataAdapter oradata;
        DataTable dtc;

        string AsOnDate = Request.QueryString["AsOnDate"].ToString();

        string MakeAsOnDate = Request.QueryString["AsOnDate"].ToString(); 
        string[] MakeAsOnDateSplit = MakeAsOnDate.Split('-');
        String AsOnDateTemp = MakeAsOnDateSplit[0].Replace("/", "-");
        DateTime AsOnDateNewD = DateTime.ParseExact(AsOnDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        string AsOnDateNew = AsOnDateNewD.ToString("dd-MM-yyyy");

        DateTime curDate = AsOnDateNewD;
        DateTime startDate = curDate.AddMonths(-1);
        DateTime LastDateTemp = curDate.AddDays(-(curDate.Day));
        string LastDate = LastDateTemp.ToString("dd-MM-yyyy");
        string LastMonth = startDate.ToString("MM-yyyy");
        string CurrentMonth = AsOnDateNewD.ToString("MM-yyyy");
             
       // string sqlString = "  SELECT  PI.ITEM_ID, PI.ITEM_NAME, nvl(BEGPFSIH.FINAL_STOCK_WT,0) AS BEG_FSTOCK_WT, nvl(BEGPFSIH.ITEM_AVG_RATE,0) AS BEG_AVG_RATE, ROUND(nvl(BEGPFSIH.FINAL_STOCK_WT,0) * nvl(BEGPFSIH.ITEM_AVG_RATE,0),3) AS BEG_AMT, nvl(PPMD.ITEM_WEIGHT,0) AS PURCHASE_WTD, ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1))* nvl(PPMD.ITEM_WEIGHT,0),2) AS PURCHASE_AMTD, nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) AS PRODUCTION_WT, ROUND(ROUND(((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1))* nvl(PPMD.ITEM_WEIGHT,0),2)) / ((nvl(BEGPRSIH.FINAL_STOCK_WT,1) + nvl(PPM.ITEM_WEIGHT,0)) - nvl(PPMD.ITEM_WEIGHT,0)),2) * nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) AS PRODUCTION_AMT, ROUND(nvl(nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) * nvl(PPC.COST_RATE,0),0),3) AS PROD_PRO_COST_AMT, ROUND((ROUND(ROUND(((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1))* nvl(PPMD.ITEM_WEIGHT,0),2)) / ((nvl(BEGPRSIH.FINAL_STOCK_WT,1) + nvl(PPM.ITEM_WEIGHT,0)) - nvl(PPMD.ITEM_WEIGHT,0)),2) * nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)) + nvl(nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) * nvl(PPC.COST_RATE,0),0),3) AS PROD_TOTAL_COST_AMT, (nvl(BEGPFSIH.FINAL_STOCK_WT,0) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0))   AS AVAIL_SALE_WT, ROUND((nvl(BEGPFSIH.FINAL_STOCK_WT,0) * nvl(BEGPFSIH.ITEM_AVG_RATE,0))+  ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1))* nvl(PPMD.ITEM_WEIGHT,0),2)+  ((ROUND(ROUND(((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1))* nvl(PPMD.ITEM_WEIGHT,0),2)) / ((nvl(BEGPRSIH.FINAL_STOCK_WT,1) + nvl(PPM.ITEM_WEIGHT,0)) - nvl(PPMD.ITEM_WEIGHT,0)),2) * nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)) + nvl(nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) * nvl(PPC.COST_RATE,0),0)),3) AS AVAIL_SALE_AMT, ROUND(((nvl(BEGPFSIH.FINAL_STOCK_WT,0) * nvl(BEGPFSIH.ITEM_AVG_RATE,0))+  ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1))* nvl(PPMD.ITEM_WEIGHT,0),2)+  ((ROUND(ROUND(((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1))* nvl(PPMD.ITEM_WEIGHT,0),2)) / ((nvl(BEGPRSIH.FINAL_STOCK_WT,1) + nvl(PPM.ITEM_WEIGHT,0)) - nvl(PPMD.ITEM_WEIGHT,0)),2) * nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)) + nvl(nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) * nvl(PPC.COST_RATE,0),0)))/(nvl(BEGPFSIH.FINAL_STOCK_WT,1) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)),2)   AS AVAIL_SALE_AVG_RATE, nvl(PSM.ITEM_WEIGHT,0) AS SLAE_WT, nvl(PSMR.ITEM_WEIGHT,0) AS SALE_RETURN_WET,  (nvl(BEGPFSIH.FINAL_STOCK_WT,0) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)) - (nvl(PSM.ITEM_WEIGHT,0) + nvl(PSMR.ITEM_WEIGHT,0))  AS END_FSTOCK_WT,ROUND(((nvl(BEGPFSIH.FINAL_STOCK_WT,0) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)) - (nvl(PSM.ITEM_WEIGHT,0) + nvl(PSMR.ITEM_WEIGHT,0)))* ROUND(((nvl(BEGPFSIH.FINAL_STOCK_WT,0) * nvl(BEGPFSIH.ITEM_AVG_RATE,0))+  ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1))* nvl(PPMD.ITEM_WEIGHT,0),2)+  ((ROUND(ROUND(((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1))* nvl(PPMD.ITEM_WEIGHT,0),2)) / ((nvl(BEGPRSIH.FINAL_STOCK_WT,1) + nvl(PPM.ITEM_WEIGHT,0)) - nvl(PPMD.ITEM_WEIGHT,0)),2) * nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)) + nvl(nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) * nvl(PPC.COST_RATE,0),0)))/(nvl(BEGPFSIH.FINAL_STOCK_WT,1) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)),2),3) AS END_AMT, ROUND((((nvl(BEGPFSIH.FINAL_STOCK_WT,0) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)) - (nvl(PSM.ITEM_WEIGHT,0) + nvl(PSMR.ITEM_WEIGHT,0)))* ROUND(((nvl(BEGPFSIH.FINAL_STOCK_WT,0) * nvl(BEGPFSIH.ITEM_AVG_RATE,0))+  ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1))* nvl(PPMD.ITEM_WEIGHT,0),2)+  ((ROUND(ROUND(((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1))* nvl(PPMD.ITEM_WEIGHT,0),2)) / ((nvl(BEGPRSIH.FINAL_STOCK_WT,1) + nvl(PPM.ITEM_WEIGHT,0)) - nvl(PPMD.ITEM_WEIGHT,0)),2) * nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)) + nvl(nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) * nvl(PPC.COST_RATE,0),0)))/(nvl(BEGPFSIH.FINAL_STOCK_WT,1) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)),2)) / ((nvl(BEGPFSIH.FINAL_STOCK_WT,1) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)) - (nvl(PSM.ITEM_WEIGHT,0)+nvl(PSMR.ITEM_WEIGHT,0))),3)  AS END_AVG_RATE FROM PF_ITEM PI LEFT JOIN (SELECT ITEM_ID, FINAL_STOCK_WT, ITEM_AVG_RATE FROM PF_RM_STOCK_INVENTORY_HISTORY WHERE TO_CHAR(TO_DATE(CREATE_DATE), 'dd-mm-YYYY') = '" + LastDate + "' ) BEGPRSIH ON PI.ITEM_ID = BEGPRSIH.ITEM_ID LEFT JOIN (SELECT ITEM_ID,  FINAL_STOCK_WT, ITEM_AVG_RATE FROM PF_FG_STOCK_INVENTORY_HISTORY WHERE TO_CHAR(TO_DATE(CREATE_DATE), 'dd-mm-YYYY') = '" + LastDate + "' ) BEGPFSIH ON PI.ITEM_ID = BEGPFSIH.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT,  sum(nvl(ITEM_AMOUNT,0)) AS ITEM_AMOUNT FROM PF_PURCHASE_MASTER  WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + LastMonth + "' GROUP BY ITEM_ID) BEGPPM ON PI.ITEM_ID = BEGPPM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT, sum(ITEM_AMOUNT) AS ITEM_AMOUNT FROM PF_PURCHASE_MASTER  WHERE  TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' GROUP BY ITEM_ID) PPM ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_PURCHASE_MASTER  WHERE PUR_TYPE_ID = 1  AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' GROUP BY ITEM_ID) PPMD ON PI.ITEM_ID = PPMD.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT_IN_FG) AS ITEM_WEIGHT_IN_FG FROM PF_PRODUCTION_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' GROUP BY ITEM_ID) PPMAS ON PI.ITEM_ID = PPMAS.ITEM_ID LEFT JOIN (SELECT ITEM_ID, COST_RATE  FROM PF_PROCESSING_COST WHERE TO_CHAR(TO_DATE(MONTH_YEAR), 'mm-YYYY') = '" + CurrentMonth + "' ) PPC ON PI.ITEM_ID = PPC.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_SALES_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' AND IS_SALES_RETURN IS NULL GROUP BY ITEM_ID) PSM ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_SALES_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' AND IS_SALES_RETURN = 'Yes' GROUP BY ITEM_ID) PSMR ON PI.ITEM_ID = PSMR.ITEM_ID ORDER BY PI.ITEM_ID   ";

        string sqlString = " SELECT  PI.ITEM_ID, PI.ITEM_NAME, nvl(BEGPFSIH.FINAL_STOCK_WT,0) AS BEG_FSTOCK_WT, nvl(BEGPFSIH.ITEM_AVG_RATE,0) AS BEG_AVG_RATE, ROUND(nvl(BEGPFSIH.FINAL_STOCK_WT,0) * nvl(BEGPFSIH.ITEM_AVG_RATE,0),3) AS BEG_AMT, nvl(PPMD.ITEM_WEIGHT,0) AS PURCHASE_WTD, ROUND(nvl((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/  nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1  )* nvl(PPMD.ITEM_WEIGHT,0),0),2) AS PURCHASE_AMTD, nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) AS PRODUCTION_WT, nvl(ROUND(ROUND(((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1 )* nvl(PPMD.ITEM_WEIGHT,0),2)) / (nullif((nvl(BEGPRSIH.FINAL_STOCK_WT,0) + nvl(PPM.ITEM_WEIGHT,0)) - nvl(PPMD.ITEM_WEIGHT,0),0)*1) ,2) * nvl(PPMAS.ITEM_WEIGHT_IN_FG,0),0) AS PRODUCTION_AMT, ROUND(nvl(nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) * nvl(PPC.COST_RATE,0),0),3) AS PROD_PRO_COST_AMT, ROUND(nvl((ROUND(ROUND(((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1)* nvl(PPMD.ITEM_WEIGHT,0),2)) / (nullif((nvl(BEGPRSIH.FINAL_STOCK_WT,0) + nvl(PPM.ITEM_WEIGHT,0)) - nvl(PPMD.ITEM_WEIGHT,0),0)*1),2) * nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)) + nvl(nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) * nvl(PPC.COST_RATE,0),0),0),3) AS PROD_TOTAL_COST_AMT, (nvl(BEGPFSIH.FINAL_STOCK_WT,0) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0))   AS AVAIL_SALE_WT, ROUND(nvl((nvl(BEGPFSIH.FINAL_STOCK_WT,0) * nvl(BEGPFSIH.ITEM_AVG_RATE,0))+  ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1)* nvl(PPMD.ITEM_WEIGHT,0),2)+  ((ROUND(ROUND(((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1)* nvl(PPMD.ITEM_WEIGHT,0),2)) / ((nvl(BEGPRSIH.FINAL_STOCK_WT,1) + nvl(PPM.ITEM_WEIGHT,0)) - nvl(PPMD.ITEM_WEIGHT,0)),2) * nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)) + nvl(nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) * nvl(PPC.COST_RATE,0),0)),0),3) AS AVAIL_SALE_AMT, ROUND(nvl(((nvl(BEGPFSIH.FINAL_STOCK_WT,0) * nvl(BEGPFSIH.ITEM_AVG_RATE,0))+  ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1)* nvl(PPMD.ITEM_WEIGHT,0),2)+  ((ROUND(ROUND(((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1)* nvl(PPMD.ITEM_WEIGHT,0),2)) / ((nvl(BEGPRSIH.FINAL_STOCK_WT,1) + nvl(PPM.ITEM_WEIGHT,0)) - nvl(PPMD.ITEM_WEIGHT,0)),2) * nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)) + nvl(nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) * nvl(PPC.COST_RATE,0),0)))/ nullif((nvl(BEGPFSIH.FINAL_STOCK_WT,1) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)),0) * 1,0),2)   AS AVAIL_SALE_AVG_RATE, nvl(PSM.ITEM_WEIGHT,0) AS SLAE_WT, nvl(PSMR.ITEM_WEIGHT,0) AS SALE_RETURN_WET,  (nvl(BEGPFSIH.FINAL_STOCK_WT,0) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)) - (nvl(PSM.ITEM_WEIGHT,0) + nvl(PSMR.ITEM_WEIGHT,0))  AS END_FSTOCK_WT, ROUND(nvl(((nvl(BEGPFSIH.FINAL_STOCK_WT,0) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)) - (nvl(PSM.ITEM_WEIGHT,0) + nvl(PSMR.ITEM_WEIGHT,0)))* ROUND(((nvl(BEGPFSIH.FINAL_STOCK_WT,0) * nvl(BEGPFSIH.ITEM_AVG_RATE,0))+  ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1)* nvl(PPMD.ITEM_WEIGHT,0),2)+  ((ROUND(ROUND(((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1)* nvl(PPMD.ITEM_WEIGHT,0),2)) / ((nvl(BEGPRSIH.FINAL_STOCK_WT,1) + nvl(PPM.ITEM_WEIGHT,0)) - nvl(PPMD.ITEM_WEIGHT,0)),2) * nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)) + nvl(nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) * nvl(PPC.COST_RATE,0),0)))/(nullif((nvl(BEGPFSIH.FINAL_STOCK_WT,1) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)),0) * 1),2),0),3) AS END_AMT, ROUND(nvl((((nvl(BEGPFSIH.FINAL_STOCK_WT,0) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)) - (nvl(PSM.ITEM_WEIGHT,0) + nvl(PSMR.ITEM_WEIGHT,0)))* ROUND(((nvl(BEGPFSIH.FINAL_STOCK_WT,0) * nvl(BEGPFSIH.ITEM_AVG_RATE,0))+  ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1) * nvl(PPMD.ITEM_WEIGHT,0),2)+  ((ROUND(ROUND(((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/ nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1) * nvl(PPMD.ITEM_WEIGHT,0),2)) / ((nvl(BEGPRSIH.FINAL_STOCK_WT,1) + nvl(PPM.ITEM_WEIGHT,0)) - nvl(PPMD.ITEM_WEIGHT,0)),2) * nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)) + nvl(nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) * nvl(PPC.COST_RATE,0),0)))/(nullif((nvl(BEGPFSIH.FINAL_STOCK_WT,1) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)),0) * 1),2)) / ((nvl(BEGPFSIH.FINAL_STOCK_WT,1) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)) - (nvl(PSM.ITEM_WEIGHT,0)+nvl(PSMR.ITEM_WEIGHT,0))),0),3)  AS END_AVG_RATE FROM PF_ITEM PI LEFT JOIN (SELECT ITEM_ID, FINAL_STOCK_WT, ITEM_AVG_RATE FROM PF_RM_STOCK_INVENTORY_HISTORY WHERE TO_CHAR(TO_DATE(CREATE_DATE), 'dd-mm-YYYY') = '" + LastDate + "' ) BEGPRSIH ON PI.ITEM_ID = BEGPRSIH.ITEM_ID LEFT JOIN (SELECT ITEM_ID,  FINAL_STOCK_WT, ITEM_AVG_RATE FROM PF_FG_STOCK_INVENTORY_HISTORY WHERE TO_CHAR(TO_DATE(CREATE_DATE), 'dd-mm-YYYY') = '" + LastDate + "' ) BEGPFSIH ON PI.ITEM_ID = BEGPFSIH.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT,  sum(nvl(ITEM_AMOUNT,0)) AS ITEM_AMOUNT FROM PF_PURCHASE_MASTER  WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + LastMonth + "' GROUP BY ITEM_ID) BEGPPM ON PI.ITEM_ID = BEGPPM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT, sum(ITEM_AMOUNT) AS ITEM_AMOUNT FROM PF_PURCHASE_MASTER  WHERE  TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' GROUP BY ITEM_ID) PPM ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_PURCHASE_MASTER  WHERE PUR_TYPE_ID = 1  AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' GROUP BY ITEM_ID) PPMD ON PI.ITEM_ID = PPMD.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT_IN_FG) AS ITEM_WEIGHT_IN_FG FROM PF_PRODUCTION_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' GROUP BY ITEM_ID) PPMAS ON PI.ITEM_ID = PPMAS.ITEM_ID LEFT JOIN (SELECT ITEM_ID, COST_RATE  FROM PF_PROCESSING_COST WHERE TO_CHAR(TO_DATE(MONTH_YEAR), 'mm-YYYY') = '" + CurrentMonth + "' ) PPC ON PI.ITEM_ID = PPC.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_SALES_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' AND IS_SALES_RETURN IS NULL GROUP BY ITEM_ID) PSM ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_SALES_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' AND IS_SALES_RETURN = 'Yes' GROUP BY ITEM_ID) PSMR ON PI.ITEM_ID = PSMR.ITEM_ID WHERE ( ((nvl(BEGPFSIH.FINAL_STOCK_WT,0) + nvl(PPMD.ITEM_WEIGHT,0) + nvl(ITEM_WEIGHT_IN_FG,0)) - (nvl(PSM.ITEM_WEIGHT,0) + nvl(PSMR.ITEM_WEIGHT,0))) > 0 OR nvl(BEGPFSIH.FINAL_STOCK_WT,0) > 0 ) ORDER BY PI.ITEM_ID   ";

             
        OracleCommand cmd = new OracleCommand(sqlString, conn);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        conn.Open();
        dt.Fill(imageDataSet.Tables["reportTable"]);  
      

        // for processing cost

        string makeSQL = " SELECT  COST_RATE FROM PF_PROCESSING_COST WHERE  TO_CHAR(TO_DATE(MONTH_YEAR), 'mm-YYYY') =  '" + CurrentMonth + "' ORDER BY ITEM_ID asc ";

        cmdl = new OracleCommand(makeSQL);
        oradata = new OracleDataAdapter(cmdl.CommandText, conn);
        dtc = new DataTable();
        oradata.Fill(dtc);
        int RowCount = dtc.Rows.Count;

        double[] items_cost = new double[100];
        for (int i = 0; i < RowCount; i++)
        {
            items_cost[i] = Convert.ToDouble(dtc.Rows[i]["COST_RATE"]);
        }

        
         double HDPE         = items_cost[0];
         double HD_CAN       = items_cost[1];
         double LDPE         = items_cost[2];
         double PC           = items_cost[3];
         double PET          = items_cost[4];
         double PERTO_RABIGH = items_cost[5]; 
         double PP           = items_cost[6];
         double PVC          = items_cost[7];
         double SABIC        = items_cost[8];
         double SCC          = items_cost[9];

        ReportDocument crystalReport = new ReportDocument();
        crystalReport.Load(Server.MapPath("~/PF/PF_Reports/Pf_Fg_Statement_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsOnDate", AsOnDateNewD);
    //    crystalReport.SetParameterValue("ItemCost", items_cost);
        crystalReport.SetParameterValue("HDPE", HDPE);
        crystalReport.SetParameterValue("HD_CAN", HD_CAN);
        crystalReport.SetParameterValue("HDPE", HDPE);
        crystalReport.SetParameterValue("LDPE", LDPE);
        crystalReport.SetParameterValue("PC", PC);
        crystalReport.SetParameterValue("PET", PET);
        crystalReport.SetParameterValue("PERTO_RABIGH", PERTO_RABIGH);
        crystalReport.SetParameterValue("PP", PP);
        crystalReport.SetParameterValue("PVC", PVC);
        crystalReport.SetParameterValue("SABIC", SABIC);
        crystalReport.SetParameterValue("SCC", SCC); 

        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "FG_Statement_Report_(As_On_Date)_" + datetime + "";

        CrystalReportViewer1.ReportSource = crystalReport;

        conn.Close();

    }

        protected void CrystalReportViewer1_Unload(object sender, EventArgs e)
        {
            crystalReport.Close();
            crystalReport.Dispose();
            CrystalReportViewer1.Dispose();
        }
    }
}