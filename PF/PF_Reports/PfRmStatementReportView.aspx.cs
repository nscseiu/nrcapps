﻿using System;
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
    public partial class PfRmStatementReportView : System.Web.UI.Page
    {
        ReportDocument crystalReport = new ReportDocument();
        protected void Page_Init(object sender, EventArgs e)
        {
          
        DataSetAssetClass imageDataSet = new DataSetAssetClass();

        string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        OracleConnection con = new OracleConnection(connStr);

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

           
        string sqlString = " SELECT PI.ITEM_ID, PI.ITEM_NAME, nvl(BEGPRSIH.FINAL_STOCK_WT,0) AS BEG_FSTOCK_WT, nvl(BEGPRSIH.ITEM_AVG_RATE,0) AS BEG_AVG_RATE, (nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) AS BEG_AMT, nvl(PPM.ITEM_WEIGHT,0) AS PURCHASE_WT, nvl(PPM.ITEM_AMOUNT,0) AS PURCHASE_AMT, ROUND(nvl(PPM.ITEM_AMOUNT,0) / nvl(PPM.ITEM_WEIGHT,1),2) PURCHASE_RATE_AMT, nvl(PPMD.ITEM_WEIGHT,0) AS PURCHASE_WTD, ROUND(nvl((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/  nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1  )* nvl(PPMD.ITEM_WEIGHT,0),0),2) AS PURCHASE_AMTD, nvl(PMTI.ITEM_WEIGHT, 0) AS MAT_ISSUED_WT, nvl(PMTR.ITEM_WEIGHT, 0) AS MAT_RECEVIED_WT, (nvl(BEGPRSIH.FINAL_STOCK_WT,0) + nvl(PPM.ITEM_WEIGHT,0)+nvl(PMTR.ITEM_WEIGHT, 0)) - (nvl(PPMD.ITEM_WEIGHT,0)+nvl(PMTI.ITEM_WEIGHT, 0)) AS MAT_AVAIL_PROD_WT,  ((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND(nvl((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/  nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1  )* nvl(PPMD.ITEM_WEIGHT,0),0),2) AS MAT_AVAIL_PROD_AMT, ROUND(nvl((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND(nvl((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/  nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1  )* nvl(PPMD.ITEM_WEIGHT,0),0),2)) / nullif((nvl(BEGPRSIH.FINAL_STOCK_WT,0) + nvl(PPM.ITEM_WEIGHT,0)+nvl(PMTR.ITEM_WEIGHT, 0)) - (nvl(PPMD.ITEM_WEIGHT,0)+nvl(PMTI.ITEM_WEIGHT, 0)),0),0),2) AS MAT_AVAIL_PROD_RATE, nvl(PPMAS.ITEM_WEIGHT_IN_FG,0) AS PRODUCTION_WT, nvl(PGAR.PGE_WEIGHT,0) AS GAR_EST_OF_PROD, nvl(PAGAR.ACTUAL_GAR_WEIGHT,0) AS ACTUAL_GAR_WEIGHT, (((nvl(BEGPRSIH.FINAL_STOCK_WT,0) + nvl(PPM.ITEM_WEIGHT,0)+nvl(PMTR.ITEM_WEIGHT, 0)) - (nvl(PPMD.ITEM_WEIGHT,0)+nvl(PMTI.ITEM_WEIGHT, 0))) -  nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)- nvl(PGAR.PGE_WEIGHT,0) - nvl(PAGAR.ACTUAL_GAR_WEIGHT,0)) AS END_FSTOCK_WT, (((nvl(BEGPRSIH.FINAL_STOCK_WT,0) + nvl(PPM.ITEM_WEIGHT,0)+nvl(PMTR.ITEM_WEIGHT, 0)) - (nvl(PPMD.ITEM_WEIGHT,0)+nvl(PMTI.ITEM_WEIGHT, 0))) -  nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)- nvl(PGAR.PGE_WEIGHT,0) - nvl(PAGAR.ACTUAL_GAR_WEIGHT,0)) *  ROUND(nvl((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND(nvl((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/  nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1  )* nvl(PPMD.ITEM_WEIGHT,0),0),2)) / nullif((nvl(BEGPRSIH.FINAL_STOCK_WT,0) + nvl(PPM.ITEM_WEIGHT,0)+nvl(PMTR.ITEM_WEIGHT, 0)) - (nvl(PPMD.ITEM_WEIGHT,0)+nvl(PMTI.ITEM_WEIGHT, 0)),0),0),2) AS END_AMT, nvl((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) + nvl(PPM.ITEM_WEIGHT,0)+nvl(PMTR.ITEM_WEIGHT, 0)) - (nvl(PPMD.ITEM_WEIGHT,0)+nvl(PMTI.ITEM_WEIGHT, 0))) -  nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)- nvl(PGAR.PGE_WEIGHT,0) - nvl(PAGAR.ACTUAL_GAR_WEIGHT,0)) *  ROUND(nvl((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +  nvl(PPM.ITEM_AMOUNT,0) ) - ROUND(nvl((((nvl(BEGPRSIH.FINAL_STOCK_WT,0) * nvl(BEGPRSIH.ITEM_AVG_RATE,0)) +nvl(PPM.ITEM_AMOUNT,0))/  nullif(nvl( nvl(BEGPRSIH.FINAL_STOCK_WT,1)+  nvl(PPM.ITEM_WEIGHT,0),1),0) * 1  )* nvl(PPMD.ITEM_WEIGHT,0),0),2)) / nullif((nvl(BEGPRSIH.FINAL_STOCK_WT,0) + nvl(PPM.ITEM_WEIGHT,0)+nvl(PMTR.ITEM_WEIGHT, 0)) - (nvl(PPMD.ITEM_WEIGHT,0)+nvl(PMTI.ITEM_WEIGHT, 0)),0),0),2) /   nullif(((nvl(BEGPRSIH.FINAL_STOCK_WT,0) + nvl(PPM.ITEM_WEIGHT,0)+nvl(PMTR.ITEM_WEIGHT, 0)) - (nvl(PPMD.ITEM_WEIGHT,0)+nvl(PMTI.ITEM_WEIGHT, 0))) -  nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)- nvl(PGAR.PGE_WEIGHT,0) - nvl(PAGAR.ACTUAL_GAR_WEIGHT,0),0),0) AS END_AVG_RATE FROM PF_ITEM PI LEFT JOIN (SELECT ITEM_ID, FINAL_STOCK_WT, ITEM_AVG_RATE FROM PF_RM_STOCK_INVENTORY_HISTORY WHERE TO_CHAR(TO_DATE(CREATE_DATE), 'dd-mm-YYYY') = '" + LastDate + "' ) BEGPRSIH ON PI.ITEM_ID = BEGPRSIH.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT,  sum(nvl(ITEM_AMOUNT,0)+nvl(VAT_AMOUNT,0)) AS ITEM_AMOUNT FROM PF_PURCHASE_MASTER  WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + LastMonth + "' GROUP BY ITEM_ID) BEGPPM ON PI.ITEM_ID = BEGPPM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT, sum(nvl(ITEM_AMOUNT,0)+nvl(VAT_AMOUNT,0)) AS ITEM_AMOUNT FROM PF_PURCHASE_MASTER  WHERE  TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' GROUP BY ITEM_ID) PPM ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_SALES_MASTER  WHERE PUR_TYPE_ID = 1 AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' GROUP BY ITEM_ID) PPMD ON PI.ITEM_ID = PPMD.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT_IN_FG FROM PF_PRODUCTION_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' GROUP BY ITEM_ID) PPMAS ON PI.ITEM_ID = PPMAS.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(PGE_WEIGHT) AS PGE_WEIGHT FROM PF_PRODUCTION_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "' GROUP BY ITEM_ID) PGAR ON PI.ITEM_ID = PGAR.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ACTUAL_GAR_WEIGHT) AS ACTUAL_GAR_WEIGHT FROM PF_ACTUAL_GARBAGE WHERE TO_CHAR(TO_DATE(MONTH_YEAR), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) PAGAR ON PI.ITEM_ID = PAGAR.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT FROM PF_MATERIAL_TRANSACTION  WHERE TRANSACTION_FOR_ID = 1 AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) PMTI ON PI.ITEM_ID = PMTI.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT FROM PF_MATERIAL_TRANSACTION  WHERE TRANSACTION_FOR_ID = 2 AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '" + CurrentMonth + "'  GROUP BY ITEM_ID) PMTR ON PI.ITEM_ID = PMTR.ITEM_ID WHERE ( (((nvl(BEGPRSIH.FINAL_STOCK_WT,0) + nvl(PPM.ITEM_WEIGHT,0)+nvl(PMTR.ITEM_WEIGHT, 0)) - (nvl(PPMD.ITEM_WEIGHT,0)+nvl(PMTI.ITEM_WEIGHT, 0))) -  nvl(PPMAS.ITEM_WEIGHT_IN_FG,0)- nvl(PGAR.PGE_WEIGHT,0) - nvl(PAGAR.ACTUAL_GAR_WEIGHT,0)) > 0 OR  nvl(BEGPRSIH.FINAL_STOCK_WT,0)  > 0) ORDER BY PI.ITEM_ID "; //'" + AsOnDate + "' 
             
        OracleCommand cmd = new OracleCommand(sqlString, con);
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = sqlString; 
        OracleDataAdapter dt = new OracleDataAdapter(cmd);  
        con.Open();
        dt.Fill(imageDataSet.Tables["reportTable"]);  
        con.Close();
              
        crystalReport.Load(Server.MapPath("~/PF/PF_Reports/Pf_Rm_Statement_Report.rpt"));
        crystalReport.SetDataSource(imageDataSet.Tables["reportTable"]);
        crystalReport.SetParameterValue("AsOnDate", AsOnDateNewD); 
        string datetime = DateTime.Now.ToString("dd-MM-yyyy");
        CrystalReportViewer1.ID = "Rm_Statement_Report_(As_On_Date)_" + datetime + "";

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