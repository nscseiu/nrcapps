using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OracleClient;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using System.Web.Services;
using System.Text;

namespace NRCAPPS.PF
{
    public partial class PfDailyPurProdStatement : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmd, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt, ds, dp, di, dpr, dsm, dip;
        int RowCount, RowCount1, RowCount2, RowCount3;

        double qtyTotal = 0.00;
        double grQtyTotal = 0.00;
        double ItemVatAmt = 0.0, ItemAmtTotal = 0.0, ItemWtWbTotal = 0.0, TotalInvoiceAmt = 0.0, TotalGarbage = 0.0; string EntryDateSlip = "", PartyArabicName = "", PartyName = "";

 
        string IS_PAGE_ACTIVE = "", IS_ADD_ACTIVE = "", IS_EDIT_ACTIVE = "", IS_DELETE_ACTIVE = "", IS_VIEW_ACTIVE = "", IS_REPORT_ACTIVE = "", IS_PRINT_ACTIVE = "";
        public bool IsLoad { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_NAME"] != null)
            {
                string requestedFile = Path.GetFileName(Request.Path);
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = " SELECT  NUPP.IS_PAGE_ACTIVE, NUPP.IS_ADD_ACTIVE, NUPP.IS_EDIT_ACTIVE, NUPP.IS_DELETE_ACTIVE, NUPP.IS_VIEW_ACTIVE, NUPP.IS_REPORT_ACTIVE, NUPP.IS_PRINT_ACTIVE FROM NRC_USER_PAGE_PERMISSION NUPP LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NUPP.USER_PAGE_ID  WHERE NUPP.USER_ID = '" + Session["USER_ID"] + "' AND NUP.IS_ACTIVE = 'Enable' AND NUP.PAGE_URL = '" + requestedFile + "' ";
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                for (int i = 0; i < RowCount; i++)
                {
                    IS_PAGE_ACTIVE = dt.Rows[i]["IS_PAGE_ACTIVE"].ToString();
                    IS_ADD_ACTIVE = dt.Rows[i]["IS_ADD_ACTIVE"].ToString();
                    IS_EDIT_ACTIVE = dt.Rows[i]["IS_EDIT_ACTIVE"].ToString();
                    IS_DELETE_ACTIVE = dt.Rows[i]["IS_DELETE_ACTIVE"].ToString();
                    IS_VIEW_ACTIVE = dt.Rows[i]["IS_VIEW_ACTIVE"].ToString();
                    IS_REPORT_ACTIVE = dt.Rows[i]["IS_REPORT_ACTIVE"].ToString();
                    IS_PRINT_ACTIVE = dt.Rows[i]["IS_PRINT_ACTIVE"].ToString();
                }

                if (IS_PAGE_ACTIVE == "Enable")
                {
                     
                    if (!IsPostBack)
                    {

                       
                    } 
                    IsLoad = false;
                }
                else {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
                   
                 
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        public void clearTextField(object sender, EventArgs e)
        { 
        }


        public int BusinessDaysUntil(DateTime firstDay, DateTime lastDay)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = (int)firstDay.DayOfWeek;
                int lastDayOfWeek = (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 5)
                {
                    if (lastDayOfWeek >= 5)// Only Friday is in the remaining time interval
                        businessDays -= 1;
                }
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount;

            return businessDays;
        }

        protected void BtnReportPurProd_Click(object sender, EventArgs e)
        { 
            if (IS_REPORT_ACTIVE == "Enable")
            {
               // alert_box.Visible = false;
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string HtmlString = "";
                string StartDateTemp = EntryDate3.Text; 
                string[] StartDateTempSplit = StartDateTemp.Split('-');
                String StartDateFormTemp = StartDateTempSplit[0].Replace("/", "-");
                DateTime StartDateTemp1 = DateTime.ParseExact(StartDateFormTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture); 
                var StartDateTemp2 = new DateTime(StartDateTemp1.Year, StartDateTemp1.Month, 1);
                string StartDateTemp3 = StartDateTemp2.ToString("dd-MM-yyyy");
                string StartDateQuery = StartDateTemp2.ToString("yyyy/MM/dd");
                 
                DateTime StartDateTemp4 = DateTime.ParseExact(StartDateTemp, "dd/mm/yyyy", CultureInfo.InvariantCulture);
                DateTime StartDateTemp5 = new DateTime(StartDateTemp4.Year, StartDateTemp4.Month, 1);

                string EndDate = EntryDate3.Text;
                  
                DateTime EndDateNew = DateTime.ParseExact(EndDate, "dd/mm/yyyy", CultureInfo.InvariantCulture);
                string EndDateQuery = EndDateNew.ToString("yyyy/mm/dd");
                string EndMonthQuery = EndDateNew.ToString("yyyy/mm");
                string EndDateQuery2 = EndDateNew.ToString("dd/mm/yyyy");

                int TotalDaysWithOutFriday = BusinessDaysUntil(StartDateTemp5, EndDateNew);

                string EndDateTemp = EndDate;
                string[] EndDateTempSplit = EndDateTemp.Split('-');
                String EndDateFormTemp = EndDateTempSplit[0].Replace("/", "-");
                DateTime EndMonthNew = DateTime.ParseExact(EndDateFormTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EndMonth = EndMonthNew.ToString("dd-MMMM-yyyy");



                HtmlString += "<div style='float:left;width:715px;height:970px;margin:10px 0 0 40px;padding:10px 20px 20px 20px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";
                HtmlString += "<div style='float:left;width:715px;height:60px;text-align:center;' ><img src='../../image/logo_from.png'/ height='55px;'></div> ";
                HtmlString += "<div style='float:left;width:715px;height:20px;text-align:center;text-decoration: underline;' ><span style='font-family:Times New Roman,Times, serif;font-size:14px;font-weight:700;'>Plastic Factory Division</span></div> ";
                HtmlString += "<div style='float:left;width:715px;height:25px;text-align:center;text-decoration: underline;' ><span style='font-family:Times New Roman,Times, serif;font-size:13px;font-weight:700;'>Daily Report (Purchase & Production) for " + EndMonth + "</span></div> ";

                string makeSQL = " SELECT PI.ITEM_NAME ||' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME, SUM(nvl(PPM.ITEM_WEIGHT, 0)) AS ITEM_WEIGHT, SUM(nvl(PPM.ITEM_AMOUNT, 0)) AS ITEM_AMOUNT, ROUND(SUM(nvl(PPM.ITEM_AMOUNT, 0)) / SUM(nvl(PPM.ITEM_WEIGHT, 0)), 2) AS ITEM_RATE FROM PF_PURCHASE_MASTER PPM LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID WHERE TO_CHAR(TO_DATE(PPM.ENTRY_DATE), 'yyyy/mm/dd') = '" + EndDateQuery + "' GROUP BY PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME ORDER BY PI.ITEM_ID ";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                HtmlString += "<table cellpadding='1px' cellspacing='0' style='font-size: 11px;' width='85%' align='center'>";
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<th colspan=4 style='border:black solid 1px; -webkit-border-top-left-radius:5px;-webkit-border-top-right-radius:5px;'>Purchase List</th> ";
                HtmlString += "</tr> ";
                HtmlString += "<th style='border-left:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>ITEM NAME </th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>WEIGHT IN MT</span></th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>AMOUNT - SR</th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>RATE PER MT</th> ";
  
                for (int i = 0; i < RowCount; i++)
                {
                    ItemWtWbTotal += Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());
                    ItemAmtTotal += Convert.ToDouble(dt.Rows[i]["ITEM_AMOUNT"].ToString()); 

                    HtmlString += "<tr valign='top'> ";

                    HtmlString += "<td style='border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:center;'> ";
                    HtmlString += "" + dt.Rows[i]["ITEM_NAME"].ToString() + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", dt.Rows[i]["ITEM_WEIGHT"]) + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n2}", dt.Rows[i]["ITEM_AMOUNT"]) + "";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n2}", dt.Rows[i]["ITEM_RATE"]) + " ";
                    HtmlString += "</td> ";
 
                    HtmlString += "</tr> ";

                }
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td style='-webkit-border-bottom-left-radius:5px;border-top:black solid 1px;border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "Grand Total ";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-top:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n3}", ItemWtWbTotal) + " ";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-top:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n2}", ItemAmtTotal) + "";
                HtmlString += "</td> ";
                HtmlString += "<td style='-webkit-border-bottom-right-radius:5px;border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n2}", ((ItemAmtTotal / ItemWtWbTotal) * 1000)) + " ";
                HtmlString += "</td> "; 
                HtmlString += "</tr> ";
                    
                HtmlString += "</table> ";
                HtmlString += "</br> ";

                string makeSQL1 = " SELECT  PI.ITEM_NAME ||' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME, SUM(nvl(PPJ.ITEM_WEIGHT, 0)) AS ITEM_WEIGHT FROM PF_PURCHASE_JW PPJ LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPJ.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPJ.SUB_ITEM_ID WHERE TO_CHAR(TO_DATE(PPJ.ENTRY_DATE), 'yyyy/mm/dd') = '" + EndDateQuery + "' GROUP BY PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME ORDER BY PI.ITEM_ID  ";

                cmd = new OracleCommand(makeSQL1);
                oradata = new OracleDataAdapter(cmd.CommandText, conn);
                ds = new DataTable();
                oradata.Fill(ds);
                RowCount1 = ds.Rows.Count;

                double ItemWtJwTotal = 0.0; 
                HtmlString += "<table cellpadding='1px' cellspacing='0' style='font-size: 11px;' width='85%' align='center'>";
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<th colspan=2 style='border:black solid 1px; -webkit-border-top-left-radius:5px;-webkit-border-top-right-radius:5px;'>Job Work (Purchase) List</th> ";
                HtmlString += "</tr> ";
                HtmlString += "<th style='border-left:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>ITEM NAME</th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>WEIGHT IN MT</span></th> "; 

                for (int i = 0; i < RowCount1; i++)
                {
                    ItemWtJwTotal += Convert.ToDouble(ds.Rows[i]["ITEM_WEIGHT"].ToString()); 

                    HtmlString += "<tr valign='top'> ";

                    HtmlString += "<td style='border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:center;'> ";
                    HtmlString += "" + ds.Rows[i]["ITEM_NAME"].ToString() + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", ds.Rows[i]["ITEM_WEIGHT"]) + " ";
                    HtmlString += "</td> "; 

                    HtmlString += "</tr> ";

                }
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td style='-webkit-border-bottom-left-radius:5px;border-top:black solid 1px;border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;;font-weight:700;'> ";
                HtmlString += "Grand Total ";
                HtmlString += "</td> ";
                HtmlString += "<td style='-webkit-border-bottom-right-radius:5px;border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n3}", ItemWtJwTotal) + " ";
                HtmlString += "</td> "; 
                HtmlString += "</tr> ";

                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td colspan=1> ";
                HtmlString += "&nbsp;";
                HtmlString += "</td> ";
                HtmlString += "<td  colspan=1 style='text-align:right;'> ";
                HtmlString += "&nbsp;";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";
                HtmlString += "</table> "; 

                // Target
                string makeSQL2 = " SELECT nvl(sum(ROUND((nvl(PMT.ITEM_WEIGHT_PUR,0)),3)),0) AS ITEM_WEIGHT_PUR,  nvl(sum(ROUND((nvl(PMT.ITEM_WEIGHT_PROD,0)),3)),0) AS ITEM_WEIGHT_PROD FROM PF_MONTHLY_TARGET PMT WHERE  TO_CHAR(TO_DATE(PMT.MONTH_YEAR), 'yyyy/mm') = '" + EndMonthQuery + "'  ";

                cmd = new OracleCommand(makeSQL2);
                oradata = new OracleDataAdapter(cmd.CommandText, conn);
                ds = new DataTable();
                oradata.Fill(ds);
                RowCount1 = ds.Rows.Count;
                 
                HtmlString += "<table cellpadding='1px' cellspacing='0' style='font-size: 11px;' width='85%' align='center'>";
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<th colspan=3 style='border:black solid 1px; -webkit-border-top-left-radius:5px;-webkit-border-top-right-radius:5px;'>Purchase Achievements (Daily)</th> ";
                HtmlString += "</tr> "; 
                HtmlString += "<tr valign='top'> ";
                    HtmlString += "<td style='-webkit-border-bottom-left-radius:5px;border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black solid 1px;text-align:center;'> ";
                    HtmlString += "Target: " + string.Format("{0:n3}", Convert.ToDouble(ds.Rows[0]["ITEM_WEIGHT_PUR"])/26) + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black solid 1px;text-align:center;'> ";
                    HtmlString += "Actual: " + string.Format("{0:n3}", ItemWtWbTotal+ ItemWtJwTotal) + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='-webkit-border-bottom-right-radius:5px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'> ";
                    HtmlString += "Variance: " + string.Format("{0:n3}", (Convert.ToDouble(ds.Rows[0]["ITEM_WEIGHT_PUR"])/26) - (ItemWtWbTotal+ ItemWtJwTotal)) + " ";
                    HtmlString += "</td> ";
                 
                HtmlString += "</tr> ";
                HtmlString += "</table> ";
                HtmlString += "</br> ";

                // Purchase summary monthly
                string makeSQL3 = " SELECT  nvl(SUM(nvl(PPM.ITEM_WEIGHT,0)),0) AS ITEM_WEIGHT FROM PF_PURCHASE_MASTER PPM WHERE  TO_CHAR(TO_DATE(PPM.ENTRY_DATE), 'yyyy/mm/dd') BETWEEN  '" + StartDateQuery + "' AND '" + EndDateQuery + "'  ";
                cmd = new OracleCommand(makeSQL3);
                oradata = new OracleDataAdapter(cmd.CommandText, conn);
                dp = new DataTable();
                oradata.Fill(dp);
                RowCount1 = dp.Rows.Count;

                // Purchase Jw summary monthly
                string makeSQL4 = " SELECT   nvl(SUM(nvl(PPJ.ITEM_WEIGHT,0)),0) AS ITEM_WEIGHT FROM PF_PURCHASE_JW PPJ WHERE  TO_CHAR(TO_DATE(PPJ.ENTRY_DATE), 'yyyy/mm/dd') BETWEEN  '" + StartDateQuery + "' AND '" + EndDateQuery + "'  ";
                cmd = new OracleCommand(makeSQL4);
                oradata = new OracleDataAdapter(cmd.CommandText, conn);
                di = new DataTable();
                oradata.Fill(di);
                RowCount1 = di.Rows.Count;

                HtmlString += "<table cellpadding='1px' cellspacing='0' style='font-size: 11px;' width='85%' align='center'>";
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<th colspan=2 style='border:black solid 1px; -webkit-border-top-left-radius:5px;-webkit-border-top-right-radius:5px;'>Purchase Achievements Summary</th> ";
                HtmlString += "</tr> ";
                HtmlString += "<tr> ";
                HtmlString += "<td style='border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black dotted 1px;'> ";
                HtmlString += "Monthly Purchase Target (Budgeted) " + EndMonth + "";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                HtmlString += "" + string.Format("{0:n3}", Convert.ToDouble(ds.Rows[0]["ITEM_WEIGHT_PUR"])) + " ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";
                HtmlString += "<tr> ";
                HtmlString += "<td style='border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black dotted 1px;'> ";
                HtmlString += "Total Purchase Till "+ EndMonth + "";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                HtmlString += "" + string.Format("{0:n3}", Convert.ToDouble(dp.Rows[0]["ITEM_WEIGHT"])) + " ";
                HtmlString += "</td> "; 
                HtmlString += "</tr> "; 
                HtmlString += "<tr> ";
                HtmlString += "<td style='border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black dotted 1px;'> ";
                HtmlString += "Total Job Work (Purchase) Till "+ EndMonth + "";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                HtmlString += "" + string.Format("{0:n3}", Convert.ToDouble(di.Rows[0]["ITEM_WEIGHT"])) + " ";
                HtmlString += "</td> "; 
                HtmlString += "</tr> ";
                HtmlString += "<tr> ";
                HtmlString += "<td style='-webkit-border-bottom-left-radius:5px;border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black solid 1px;'> ";
                HtmlString += "Above Than Target ";
                HtmlString += "</td> ";
                HtmlString += "<td style='-webkit-border-bottom-right-radius:5px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;'> ";
                HtmlString += "" + string.Format("{0:n3}", Convert.ToDouble(ds.Rows[0]["ITEM_WEIGHT_PUR"]) - (Convert.ToDouble(dp.Rows[0]["ITEM_WEIGHT"])+ Convert.ToDouble(di.Rows[0]["ITEM_WEIGHT"]))) + " ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";

      
                HtmlString += "</tr> ";
                HtmlString += "</table> ";
                HtmlString += "</br> ";

                string makeSQL5 = "  SELECT PPL.PROD_LINE_NAME, PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME_FULL, PI.ITEM_CODE, nvl(SUM(PPRM.ITEM_WEIGHT),0) AS ITEM_WEIGHT  FROM PF_PRODUCTION_MASTER PPRM LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_PRODUCTION_MACHINE PPRMA ON PPRMA.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_PRODUCTION_LINE PPL ON PPL.PRODUCTION_LINE_ID = PPRMA.PRODUCTION_LINE_ID LEFT JOIN PF_PRODUCTION_SHIFT PPRS ON PPRS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID WHERE  TO_CHAR(TO_DATE(PPRM.ENTRY_DATE), 'yyyy/mm/dd') = '" + EndDateQuery + "' GROUP BY PPL.PROD_LINE_NAME, PPRS.SHIFT_ID, PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER,  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME, PI.ITEM_CODE ORDER BY PPL.PROD_LINE_NAME, PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER,  PI.ITEM_ID ASC  ";

                cmdl = new OracleCommand(makeSQL5);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dpr = new DataTable();
                oradata.Fill(dpr);
                RowCount = dpr.Rows.Count;
                 
                HtmlString += "<table cellpadding='1px' cellspacing='0' style='font-size: 11px;' width='85%' align='center'>";
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<th colspan=5 style='border:black solid 1px; -webkit-border-top-left-radius:5px;-webkit-border-top-right-radius:5px;'>Production List</th> ";
                HtmlString += "</tr> ";
                HtmlString += "<th style='border-left:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>PRODUCTION LINE</th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>SHIFT</span></th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>MACHINE</th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>ITEM NAME</th> "; 
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>WEIGHT IN MT</th> ";

                for (int i = 0; i < RowCount; i++)
                {
                    ItemWtWbTotal += Convert.ToDouble(dpr.Rows[i]["ITEM_WEIGHT"].ToString()); 

                    HtmlString += "<tr valign='top'> ";

                    HtmlString += "<td style='border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:center;'> ";
                    HtmlString += "" + dpr.Rows[i]["PROD_LINE_NAME"].ToString() + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:center;'> ";
                    HtmlString += "" + dpr.Rows[i]["SHIFT_NAME"].ToString() + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:center;'> ";
                    HtmlString += "" + dpr.Rows[i]["MACHINE_NUMBER"].ToString() + "";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:center;'> ";
                    HtmlString += "" + dpr.Rows[i]["ITEM_NAME_FULL"].ToString() + "";
                    HtmlString += "</td> ";
                     
                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", dpr.Rows[i]["ITEM_WEIGHT"]) + " ";
                    HtmlString += "</td> ";

                    HtmlString += "</tr> ";

                }
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td style='-webkit-border-bottom-left-radius:5px;border-top:black solid 1px;border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;;font-weight:700;'> ";
                HtmlString += "Grand Total ";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += " ";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "";
                HtmlString += "</td> ";
                HtmlString += "<td style='-webkit-border-bottom-right-radius:5px;border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n3}",  ItemWtWbTotal) + " ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";

                HtmlString += "</table> ";
                HtmlString += "</br> ";

                string makeSQL6 = " SELECT  PI.ITEM_NAME ||' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME, nvl(SUM(nvl(PPJ.ITEM_WEIGHT, 0)),0) AS ITEM_WEIGHT FROM PF_PRODUCTION_JW PPJ LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPJ.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPJ.SUB_ITEM_ID WHERE TO_CHAR(TO_DATE(PPJ.ENTRY_DATE), 'yyyy/mm/dd') = '" + EndDateQuery + "' GROUP BY PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME ORDER BY PI.ITEM_ID  ";

                cmd = new OracleCommand(makeSQL6);
                oradata = new OracleDataAdapter(cmd.CommandText, conn);
                dsm = new DataTable();
                oradata.Fill(dsm);
                RowCount1 = dsm.Rows.Count;

                double ItemWtProdJwTotal = 0.0;
                HtmlString += "<table cellpadding='1px' cellspacing='0' style='font-size: 11px;' width='85%' align='center'>";
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<th colspan=2 style='border:black solid 1px; -webkit-border-top-left-radius:5px;-webkit-border-top-right-radius:5px;'>Job Work (Production) List</th> ";
                HtmlString += "</tr> ";
                HtmlString += "<th style='border-left:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>ITEM NAME</th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>WEIGHT IN MT</span></th> ";

                for (int i = 0; i < RowCount1; i++)
                {
                    ItemWtProdJwTotal += Convert.ToDouble(dsm.Rows[i]["ITEM_WEIGHT"].ToString());

                    HtmlString += "<tr valign='top'> ";

                    HtmlString += "<td style='border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:center;'> ";
                    HtmlString += "" + dsm.Rows[i]["ITEM_NAME"].ToString() + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", dsm.Rows[i]["ITEM_WEIGHT"]) + " ";
                    HtmlString += "</td> ";

                    HtmlString += "</tr> ";

                }
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td style='-webkit-border-bottom-left-radius:5px;border-top:black solid 1px;border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;;font-weight:700;'> ";
                HtmlString += "Grand Total ";
                HtmlString += "</td> ";
                HtmlString += "<td style='-webkit-border-bottom-right-radius:5px;border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n3}", ItemWtProdJwTotal) + " ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";
                  
                HtmlString += "</table> ";
                HtmlString += "</br> ";

                // Production summary monthly
                string makeSQL7 = "  SELECT  nvl(SUM(nvl(PPRM.ITEM_WEIGHT,0)),0) AS ITEM_WEIGHT FROM PF_PRODUCTION_MASTER PPRM WHERE  TO_CHAR(TO_DATE(PPRM.ENTRY_DATE), 'yyyy/mm/dd') BETWEEN  '" + StartDateQuery + "' AND '" + EndDateQuery + "'  ";
                cmd = new OracleCommand(makeSQL7);
                oradata = new OracleDataAdapter(cmd.CommandText, conn);
                dp = new DataTable();
                oradata.Fill(dp);
                RowCount1 = dp.Rows.Count;

                // Production Jw summary monthly
                string makeSQL8 = " SELECT   nvl(SUM(nvl(PPJ.ITEM_WEIGHT,0)),0) AS ITEM_WEIGHT FROM PF_PRODUCTION_JW PPJ WHERE  TO_CHAR(TO_DATE(PPJ.ENTRY_DATE), 'yyyy/mm/dd') BETWEEN  '" + StartDateQuery + "' AND '" + EndDateQuery + "'  ";
                cmd = new OracleCommand(makeSQL8);
                oradata = new OracleDataAdapter(cmd.CommandText, conn);
                dip = new DataTable();
                oradata.Fill(dip);
                RowCount1 = dip.Rows.Count;

                HtmlString += "<table cellpadding='1px' cellspacing='0' style='font-size: 11px;' width='85%' align='center'>";
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<th colspan=2 style='border:black solid 1px; -webkit-border-top-left-radius:5px;-webkit-border-top-right-radius:5px;'>Production Achievements Summary</th> ";
                HtmlString += "</tr> ";
                HtmlString += "<tr> ";
                HtmlString += "<td style='border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black dotted 1px;'> ";
                HtmlString += "Monthly Production Target (Budgeted) " + EndMonth + "";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                HtmlString += "" + string.Format("{0:n3}", Convert.ToDouble(ds.Rows[0]["ITEM_WEIGHT_PROD"])) + " ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";
                HtmlString += "<tr> ";
                HtmlString += "<td style='border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black dotted 1px;'> ";
                HtmlString += "Total Production Till " + EndMonth + "";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                HtmlString += "" + string.Format("{0:n3}", Convert.ToDouble(dp.Rows[0]["ITEM_WEIGHT"])) + " ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";
                HtmlString += "<tr> ";
                HtmlString += "<td style='border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black dotted 1px;'> ";
                HtmlString += "Total Job Work (Purchase) Till " + EndMonth + "";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                HtmlString += "" + string.Format("{0:n3}", Convert.ToDouble(dip.Rows[0]["ITEM_WEIGHT"])) + " ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";
                HtmlString += "<tr> ";
                HtmlString += "<td style='-webkit-border-bottom-left-radius:5px;border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black solid 1px;'> ";
                HtmlString += "Above Than Target ";
                HtmlString += "</td> ";
                HtmlString += "<td style='-webkit-border-bottom-right-radius:5px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;'> ";
                HtmlString += "" + string.Format("{0:n3}", Convert.ToDouble(ds.Rows[0]["ITEM_WEIGHT_PROD"]) - (Convert.ToDouble(dp.Rows[0]["ITEM_WEIGHT"]) + Convert.ToDouble(dip.Rows[0]["ITEM_WEIGHT"]))) + " ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";


               
                HtmlString += "</table> ";
                HtmlString += "</br> ";

                HtmlString += "</div> ";
                HtmlString += "</div> ";

                HtmlString += "<table cellpadding='0' cellspacing='0' style='font-size: 11px;' width='96%' align='center'>"; 
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td> ";
                HtmlString += " ";
                HtmlString += "</td> ";
                HtmlString += "<td  style='text-align:right;'> ";
                HtmlString += "Report Print Date: "+ DateTime.Now +"";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";
                HtmlString += "</table> ";
                HtmlString += "</br> "; 
                HtmlString += "<div style='float:left;width:715px;height:920px;margin:10px 0 0 40px;padding:10px 20px 20px 20px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";
                HtmlString += "<div style='float:left;width:715px;height:60px;text-align:center;' ><img src='../../image/logo_from.png'/ height='55px;'></div> ";
                HtmlString += "<div style='float:left;width:715px;height:20px;text-align:center;text-decoration: underline;' ><span style='font-family:Times New Roman,Times, serif;font-size:14px;font-weight:700;'>Plastic Factory Division</span></div> ";
                HtmlString += "<div style='float:left;width:715px;height:25px;text-align:center;text-decoration: underline;' ><span style='font-family:Times New Roman,Times, serif;font-size:13px;font-weight:700;'>Daily Report (Sales & Stock In Hand - Raw Material) for " + EndMonth + "</span></div> ";

                string makeSQL9 = " SELECT PP.PARTY_ID, PP.PARTY_NAME, CASE WHEN  PI.ITEM_NAME IS NOT NULL  THEN PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME   WHEN PI.ITEM_NAME IS  NULL THEN PIC.ITEM_NAME || ' ' || PSIC.SUB_ITEM_NAME END AS ITEM_NAME_FULL, CASE WHEN  PI.ITEM_NAME IS NOT NULL  THEN PI.ITEM_NAME WHEN PI.ITEM_NAME IS  NULL THEN PIC.ITEM_NAME END AS ITEM_NAME, sum( CASE WHEN  PPM.ITEM_WEIGHT IS NOT NULL  THEN  PPM.ITEM_WEIGHT WHEN PPM.ITEM_WEIGHT IS  NULL THEN PEWC.ITEM_WEIGHT / 1000 END) AS ITEM_WEIGHT, round((sum(CASE WHEN  PPM.ITEM_AMOUNT IS NOT NULL  THEN  PPM.ITEM_AMOUNT   WHEN PPM.ITEM_AMOUNT IS  NULL THEN PEWC.MATERIAL_CONVERSION_AMOUNT END) / sum(CASE WHEN  PPM.ITEM_WEIGHT IS NOT NULL  THEN  PPM.ITEM_WEIGHT   WHEN PPM.ITEM_WEIGHT IS  NULL THEN PEWC.ITEM_WEIGHT / 1000 END)), 1) AS ITEM_RATE, sum( CASE WHEN  PPM.ITEM_AMOUNT IS NOT NULL  THEN  PPM.ITEM_AMOUNT   WHEN PPM.ITEM_AMOUNT IS  NULL THEN PEWC.MATERIAL_CONVERSION_AMOUNT END) AS ITEM_AMOUNT FROM PF_PARTY PP LEFT JOIN PF_SALES_MASTER PPM ON PPM.PARTY_ID = PP.PARTY_ID AND TO_CHAR(TO_DATE(PPM.ENTRY_DATE),'yyyy/mm/dd') = '" + EndDateQuery + "' LEFT JOIN PF_EXPORT_WBSLIP_CON PEWC ON PEWC.PARTY_ID = PP.PARTY_ID AND TO_CHAR(TO_DATE(PEWC.IS_SHIPMENT_COMPLETE_DATE),'yyyy/mm/dd') = '" + EndDateQuery + "' LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_ITEM PIC ON PIC.ITEM_ID = PEWC.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_SUB_ITEM PSIC ON PSIC.SUB_ITEM_ID = PEWC.SUB_ITEM_ID WHERE PPM.ITEM_WEIGHT IS NOT NULL OR PEWC.ITEM_WEIGHT IS NOT NULL GROUP BY PP.PARTY_ID, PP.PARTY_NAME, CASE WHEN  PI.ITEM_NAME IS NOT NULL  THEN PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME   WHEN PI.ITEM_NAME IS  NULL THEN PIC.ITEM_NAME || ' ' || PSIC.SUB_ITEM_NAME END, CASE WHEN  PI.ITEM_NAME IS NOT NULL  THEN PI.ITEM_NAME WHEN PI.ITEM_NAME IS  NULL THEN PIC.ITEM_NAME END ORDER BY PP.PARTY_ID    ";

                cmdl = new OracleCommand(makeSQL9);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                HtmlString += "<table cellpadding='2px' cellspacing='0' style='font-size: 11px;' width='85%' align='center'>";
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<th colspan=6 style='border:black solid 1px; -webkit-border-top-left-radius:5px;-webkit-border-top-right-radius:5px;'>Purchase List</th> ";
                HtmlString += "</tr> ";

                HtmlString += "<th style='border-left:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>PARTY ID</th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>PARTEY NAME</span></th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>ITEM NAME</th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>WEIGHT IN MT</span></th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>AMOUNT - SR</th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>RATE PER MT</th> ";

                for (int i = 0; i < RowCount; i++)
                {
                    ItemWtWbTotal += Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());
                    ItemAmtTotal += Convert.ToDouble(dt.Rows[i]["ITEM_AMOUNT"].ToString());

                    HtmlString += "<tr valign='top'> ";
                    HtmlString += "<td style='border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:center;'> ";
                    HtmlString += "" + dt.Rows[i]["PARTY_ID"].ToString() + " ";
                    HtmlString += "</td> ";
                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;'> ";
                    HtmlString += "" + dt.Rows[i]["PARTY_NAME"].ToString() + " ";
                    HtmlString += "</td> ";
                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:center;'> ";
                    HtmlString += "" + dt.Rows[i]["ITEM_NAME"].ToString() + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", dt.Rows[i]["ITEM_WEIGHT"]) + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n2}", dt.Rows[i]["ITEM_AMOUNT"]) + "";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n2}", dt.Rows[i]["ITEM_RATE"]) + " ";
                    HtmlString += "</td> ";

                    HtmlString += "</tr> ";

                }
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td colspan=3 style='-webkit-border-bottom-left-radius:5px;border-top:black solid 1px;border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "Grand Total ";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-top:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n3}", ItemWtWbTotal) + " ";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-top:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n2}", ItemAmtTotal) + "";
                HtmlString += "</td> ";
                HtmlString += "<td style='-webkit-border-bottom-right-radius:5px;border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n2}", ((ItemAmtTotal / ItemWtWbTotal) * 1000)) + " ";
                HtmlString += "</td> ";
                HtmlString += "</tr> "; 
                HtmlString += "</table> ";
                HtmlString += "</br> ";

                string makeSQL10 = "  SELECT  PI.ITEM_ID, PI.ITEM_NAME,  nvl(PSM.ITEM_WEIGHT,0)+nvl(PPMD.ITEM_WEIGHT,0) AS SALE_LOCAL_WT, nvl(PSJ.ITEM_WEIGHT,0) AS SALE_JOB_WORK_WT,  nvl(PEWCC.ITEM_WEIGHT,0) AS SALE_OVERSEAS_WT, nvl(PEWC.ITEM_WEIGHT, 0) AS ITEM_WEIGHT_TRANSIT, nvl(PSM.ITEM_WEIGHT,0)+nvl(PPMD.ITEM_WEIGHT,0) +nvl(PSJ.ITEM_WEIGHT,0)+nvl(PEWCC.ITEM_WEIGHT,0)+nvl(PEWC.ITEM_WEIGHT, 0) AS ITEM_WEIGHT_TOTAL FROM PF_ITEM PI LEFT JOIN(SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_SALES_MASTER WHERE PUR_TYPE_ID = 1  AND TO_CHAR(TO_DATE(ENTRY_DATE), 'yyyy/mm/dd') BETWEEN '" + StartDateQuery + "' AND '" + EndDateQuery + "'   GROUP BY ITEM_ID) PPMD ON PI.ITEM_ID = PPMD.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(nvl(ITEM_WEIGHT,0)) AS ITEM_WEIGHT, SUM(CASE WHEN  SALES_MODE = 'Local'  THEN  nvl(ITEM_AMOUNT, 0) + nvl(VAT_AMOUNT, 0)  WHEN SALES_MODE = 'Export' THEN nvl(ITEM_AMOUNT, 0) END) AS ITEM_AMOUNT FROM PF_SALES_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'yyyy/mm/dd') BETWEEN '" + StartDateQuery + "' AND '" + EndDateQuery + "'   AND IS_SALES_RETURN IS NULL GROUP BY ITEM_ID) PSM ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_SALES_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = '09-2018'  AND IS_SALES_RETURN = 'FG'  GROUP BY ITEM_ID) PSMR ON PI.ITEM_ID = PSMR.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(ITEM_WEIGHT/ 1000) AS ITEM_WEIGHT FROM PF_EXPORT_WBSLIP_CON WHERE IS_INVENTORY_STATUS = 'Transit' AND(TO_CHAR(TO_DATE(DISPATCH_DATE), 'dd/mm/yyyy') = '" + EndDateQuery2 + "' OR TO_CHAR(TO_DATE(SYSDATE), 'dd/mm/yyyy') <= '" + EndDateQuery2 + "')  GROUP BY ITEM_ID) PEWC ON PI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(ITEM_WEIGHT/ 1000) AS ITEM_WEIGHT, SUM(nvl(MATERIAL_CONVERSION_AMOUNT, 0)) AS ITEM_AMOUNT FROM PF_EXPORT_WBSLIP_CON WHERE IS_INVENTORY_STATUS = 'Complete' AND TO_CHAR(TO_DATE(IS_SHIPMENT_COMPLETE_DATE), 'yyyy/mm/dd') BETWEEN '" + StartDateQuery + "' AND '" + EndDateQuery + "'  GROUP BY ITEM_ID) PEWCC ON PI.ITEM_ID = PEWCC.ITEM_ID LEFT JOIN(SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_SALES_JW WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'yyyy/mm/dd') BETWEEN '" + StartDateQuery + "' AND '" + EndDateQuery + "'   GROUP BY ITEM_ID) PSJ ON PI.ITEM_ID = PSJ.ITEM_ID WHERE(nvl(PSM.ITEM_WEIGHT, 0) + nvl(PPMD.ITEM_WEIGHT, 0) > 0 OR nvl(PSJ.ITEM_WEIGHT, 0) > 0 OR nvl(PEWCC.ITEM_WEIGHT, 0) > 0 OR nvl(PEWC.ITEM_WEIGHT, 0) > 0) ORDER BY PI.ITEM_ID    ";

                cmdl = new OracleCommand(makeSQL10);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                HtmlString += "<table cellpadding='1px' cellspacing='0' style='font-size: 11px;' width='85%' align='center'>";
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<th colspan=7 style='border:black solid 1px; -webkit-border-top-left-radius:5px;-webkit-border-top-right-radius:5px;text-align:center;'>Slaes (Local, Job Work, Overseas & Goods-In-Transit Till : " +  EndMonth  + ")</th> ";
                HtmlString += "</tr> ";
                HtmlString += "<th style='border-left:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>SL. No.</th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>ITEM NAME </span></th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>LOCAL</span></th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>JOB WORK</span></th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>OVERSEAS</th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>TRANSIT</th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>TOTAL</th> ";

                int count = 1;
                double SalesLocal = 0.0, SalesJoWork = 0.0, SalesOverseas = 0.0, SalesTransit = 0.0, SalesTotal = 0.0;  
                for (int i = 0; i < RowCount; i++)
                {
                    SalesLocal += Convert.ToDouble(dt.Rows[i]["SALE_LOCAL_WT"].ToString());
                    SalesJoWork += Convert.ToDouble(dt.Rows[i]["SALE_JOB_WORK_WT"].ToString());
                    SalesOverseas += Convert.ToDouble(dt.Rows[i]["SALE_OVERSEAS_WT"].ToString());
                    SalesTransit += Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_TRANSIT"].ToString()); 
                    SalesTotal += Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_TOTAL"].ToString());

                    HtmlString += "<tr valign='top'> ";

                    HtmlString += "<td style='border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:center;'> ";
                    HtmlString += "" + count + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;'> ";
                    HtmlString += "" + string.Format("{0:n3}", dt.Rows[i]["ITEM_NAME"]) + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", dt.Rows[i]["SALE_LOCAL_WT"]) + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", dt.Rows[i]["SALE_JOB_WORK_WT"]) + "";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", dt.Rows[i]["SALE_OVERSEAS_WT"]) + "";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", dt.Rows[i]["ITEM_WEIGHT_TRANSIT"]) + "";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", dt.Rows[i]["ITEM_WEIGHT_TOTAL"]) + " ";
                    HtmlString += "</td> ";

                    HtmlString += "</tr> ";
                    count++;
                }
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td colspan=2 style='-webkit-border-bottom-left-radius:5px;border-top:black solid 1px;border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "Grand Total ";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-top:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n3}", SalesLocal) + " ";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-top:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n3}", SalesJoWork) + "";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-top:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n3}", SalesOverseas) + "";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-top:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n3}", SalesTransit) + "";
                HtmlString += "</td> "; 
                HtmlString += "<td style='-webkit-border-bottom-right-radius:5px;border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n3}", SalesTotal) + " ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";

                HtmlString += "</table> ";
                HtmlString += "</br> ";
                HtmlString += "</br> "; 


                string makeSQL11 = "  select  * from PF_RM_STOCK_INVENTORY_MASTER where FINAL_STOCK_WT > 0 ORDER BY ITEM_ID  asc ";

                cmdl = new OracleCommand(makeSQL11);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                 
                HtmlString += "<table cellpadding='1px' cellspacing='0' style='font-size: 11px;' width='85%' align='center'>";
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<th colspan=7 style='border:black solid 1px; -webkit-border-top-left-radius:5px;-webkit-border-top-right-radius:5px;text-align:center;'>Raw Material Stock-In-Hand</th> ";
                HtmlString += "</tr> ";
                HtmlString += "<th style='border-left:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>SL. No.</th> ";
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>ITEM NAME </span></th> "; 
                HtmlString += "<th style='border-bottom:black solid 1px;border-right:black solid 1px;'>TOTAL</th> ";

                int count_stock = 1;
                double TotalStock = 0.0, TotalStockTwoItem=0.0;
                for (int i = 0; i < RowCount; i++)
                { 
                    TotalStock += Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    if (dt.Rows[i]["ITEM_ID"].ToString() == "1" || dt.Rows[i]["ITEM_ID"].ToString() == "3")
                    {
                        TotalStockTwoItem += Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    HtmlString += "<tr valign='top'> ";

                    HtmlString += "<td style='border-left:black solid 1px;border-right:black dotted 1px;border-bottom:black dotted 1px;text-align:center;'> ";
                    HtmlString += "" + count_stock + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black dotted 1px;border-bottom:black dotted 1px;'> ";
                    HtmlString += "" + string.Format("{0:n3}", dt.Rows[i]["ITEM_NAME"]) +"";
                    HtmlString += "</td> ";
                      
                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black dotted 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n3}", dt.Rows[i]["FINAL_STOCK_WT"]) + " ";
                    HtmlString += "</td> ";

                    HtmlString += "</tr> ";
                    count_stock++;
                }
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td colspan=2 style='-webkit-border-bottom-left-radius:5px;border-top:black solid 1px;border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "Grand Total ";
                HtmlString += "</td> "; 
                HtmlString += "<td style='-webkit-border-bottom-right-radius:5px;border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n3}", TotalStock) + " ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";

                HtmlString += "</table> ";
                HtmlString += "</br> ";

                HtmlString += "<table cellpadding='1px' cellspacing='0' style='font-size: 11px;' width='85%' align='center'>";
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<th style='border:black solid 1px; -webkit-border-top-left-radius:5px;-webkit-border-top-right-radius:5px;text-align:center;'>Raw Material Stock-In-Hand</th> ";
                HtmlString += "</tr> "; 

                string makeSQL12 = "  select  nvl(sum(ITEM_WEIGHT),0) as ITEM_WEIGHT from PF_PRODUCTION_MASTER where TO_CHAR(TO_DATE(ENTRY_DATE), 'yyyy/mm/dd') BETWEEN  '" + StartDateQuery + "' AND '" + EndDateQuery + "'  AND (ITEM_ID = 1 OR ITEM_ID = 3)";

                cmdl = new OracleCommand(makeSQL12);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                double ProductionPerDay = Convert.ToDouble(dt.Rows[0]["ITEM_WEIGHT"].ToString()) / TotalDaysWithOutFriday;
                int TotalAvailDays = Convert.ToInt32(TotalStockTwoItem / ProductionPerDay);
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td style='-webkit-border-bottom-left-radius:5px;-webkit-border-bottom-right-radius:5px;border-top:black solid 1px;border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;font-weight:700;'> ";
                HtmlString += "Raw Material Available " + TotalAvailDays + " Days Only (Consider Only HDPE & LDPE Material) ";
                HtmlString += "</td> "; 
                HtmlString += "</tr> ";

                HtmlString += "</table> ";
                HtmlString += "</br> ";
                HtmlString += "</br> ";
                HtmlString += "</div> ";
                  
                HtmlString += "<div style='float:left;width:745px;height:auto;margin:0 0 0 40px;'> ";

                HtmlString += "<table cellpadding='0' cellspacing='0' style='font-size: 11px;' width='98%' align='center'>";
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td> ";
                HtmlString += "Prepared By: ";
                HtmlString += "</td> ";
                HtmlString += "<td  style='text-align:center;'> ";
                HtmlString += "Reviewed By:";
                HtmlString += "</td> ";
                HtmlString += "<td  style='text-align:right;'> ";
                HtmlString += "Verified By:";
                HtmlString += "</td> ";
                HtmlString += "</tr> "; 
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td colspan=3 style='text-align:right;'> ";
                HtmlString += "&nbsp;";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";
                HtmlString += "<tr> ";
                HtmlString += "<td colspan=3 style='text-align:right;'> ";
                HtmlString += "Report Print Date: " + DateTime.Now + "";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";
                HtmlString += "</table> ";
                HtmlString += "</div> ";

                PanelPrint.Controls.Add(new LiteralControl(HtmlString));
                Session["ctrl"] = PanelPrint;
                ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('Print.aspx','PrintMe','height=900px,width=1200px,scrollbars=1');</script>");

            }
        }


     

    }
}