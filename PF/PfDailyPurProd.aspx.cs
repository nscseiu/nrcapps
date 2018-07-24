using System;
using System.Collections;
using System.Configuration;
using System.Data; 
using System.Text;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.OracleClient;
using System.IO; 
using System.Collections.Generic; 
using System.Data.SqlClient;
using System.Globalization;

namespace NRCAPPS.PF
{
    public partial class PfDailyPurProd : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmd, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt, ds;
        int RowCount;

        double qtyTotal = 0.00;
        double grQtyTotal = 0.00;
        int storid = 0;
        int rowIndex = 1;

        string IS_PAGE_ACTIVE   = "";
        string IS_ADD_ACTIVE    = "";
        string IS_EDIT_ACTIVE   = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE   = "";  
        public DataTable TableData = new DataTable();
        public DataTable TableData2 = new DataTable();
        public DataTable TableData3 = new DataTable(); 

        public bool IsLoad { get; set; }  

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_NAME"] != null)
            {
                string requestedFile = Path.GetFileName(Request.Path);  
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = " SELECT  NUPP.IS_PAGE_ACTIVE, NUPP.IS_ADD_ACTIVE, NUPP.IS_EDIT_ACTIVE, NUPP.IS_DELETE_ACTIVE, NUPP.IS_VIEW_ACTIVE FROM NRC_USER_PAGE_PERMISSION NUPP LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NUPP.USER_PAGE_ID  WHERE NUPP.USER_ID = '" + Session["USER_ID"] + "' AND NUP.IS_ACTIVE = 'Enable' AND NUP.PAGE_URL = '" + requestedFile + "' ";

                cmd = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmd.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                 
                for (int i = 0; i < RowCount; i++)
                {
                    IS_PAGE_ACTIVE   = dt.Rows[i]["IS_PAGE_ACTIVE"].ToString();
                    IS_ADD_ACTIVE    = dt.Rows[i]["IS_ADD_ACTIVE"].ToString();
                    IS_EDIT_ACTIVE   = dt.Rows[i]["IS_EDIT_ACTIVE"].ToString();
                    IS_DELETE_ACTIVE = dt.Rows[i]["IS_DELETE_ACTIVE"].ToString();
                    IS_VIEW_ACTIVE   = dt.Rows[i]["IS_VIEW_ACTIVE"].ToString();  
                }

                if (IS_PAGE_ACTIVE == "Enable")
                {
                     
                    if (!IsPostBack)
                    {

                        DataTable dtPurchaseTypeID = new DataTable();
                        DataSet dsl = new DataSet();
                        string makePurchaseTypeSQL = " SELECT * FROM PF_PURCHASE_TYPE WHERE IS_ACTIVE = 'Enable' ORDER BY PUR_TYPE_ID DESC";
                        dsl = ExecuteBySqlString(makePurchaseTypeSQL);
                        dtPurchaseTypeID = (DataTable)dsl.Tables[0];
                        DropDownPurchaseTypeID.DataSource = dtPurchaseTypeID;
                        DropDownPurchaseTypeID.DataValueField = "PUR_TYPE_ID";
                        DropDownPurchaseTypeID.DataTextField = "PUR_TYPE_NAME";
                        DropDownPurchaseTypeID.DataBind();
                     //   DropDownPurchaseTypeID.Items.Insert(0, new ListItem("Select  Purchase Type", "0"));


                        DisplayPurchase();
                        DisplayProduction();
                         
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


        public void DisplayPurchase()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                GetRmAvailableData();
                //Building an HTML string.
                StringBuilder html = new StringBuilder();

                string Today_Date = "";

                string makeSQL = "";
                if (AsOnDate.Text == "")
                {
                    Today_Date = System.DateTime.Now.ToString("dd/MM/yyyy");
                    makeSQL = "  SELECT  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME_FULL, PI.ITEM_CODE, sum(PPM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PPM.ITEM_AMOUNT) AS ITEM_AMOUNT, ROUND(sum(PPM.ITEM_AMOUNT)/ sum(PPM.ITEM_WEIGHT),2) AS ITEM_AVG_RATE FROM  PF_PURCHASE_MASTER PPM  LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_PURCHASE_TYPE PPT ON PPT.PUR_TYPE_ID = PPM.PUR_TYPE_ID WHERE TO_CHAR(TO_DATE(PPM.ENTRY_DATE),'dd/mm/yyyy') = '" + Today_Date + "' AND PPT.PUR_TYPE_NAME = 'R' GROUP BY  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME, PI.ITEM_CODE ORDER BY PI.ITEM_ID ";
                }
                else
                {
                    makeSQL = " SELECT  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME_FULL, PI.ITEM_CODE, sum(PPM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PPM.ITEM_AMOUNT) AS ITEM_AMOUNT, ROUND(sum(PPM.ITEM_AMOUNT)/ sum(PPM.ITEM_WEIGHT),2) AS ITEM_AVG_RATE FROM  PF_PURCHASE_MASTER PPM  LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_PURCHASE_TYPE PPT ON PPT.PUR_TYPE_ID = PPM.PUR_TYPE_ID WHERE TO_CHAR(TO_DATE(PPM.ENTRY_DATE),'dd/mm/yyyy') = '" + AsOnDate.Text + "' AND PPT.PUR_TYPE_ID = '" + DropDownPurchaseTypeID.Text + "' GROUP BY  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME, PI.ITEM_CODE ORDER BY PI.ITEM_ID  ";
                }

                cmd = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmd.CommandText, conn);
                ds = new DataTable();
                oradata.Fill(ds);
                GridView1.DataSource = ds;
                GridView1.DataBind();

                if (ds.Rows.Count > 0)
                {
                    GridView1.HeaderRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    //Calculate Sum and display in Footer Row 
                    GridView1.FooterRow.Cells[1].Font.Bold = true;
                    GridView1.FooterRow.Cells[1].Text = "Grand Total";
                    GridView1.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Right;

                    decimal total_wt = ds.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_WEIGHT"));
                    GridView1.FooterRow.Cells[2].Font.Bold = true;
                    GridView1.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GridView1.FooterRow.Cells[2].Text = total_wt.ToString("N3");

                    decimal total_amt = ds.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_AMOUNT")); 
                    GridView1.FooterRow.Cells[3].Font.Bold = true;
                    GridView1.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    GridView1.FooterRow.Cells[3].Text = total_amt.ToString("N2");

                    decimal total_avg = (total_amt / total_wt);
                    GridView1.FooterRow.Cells[4].Font.Bold = true;
                    GridView1.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                    GridView1.FooterRow.Cells[4].Text = total_avg.ToString("N2");
                }
                else
                {

                }
                 
                string makeSQL2 = "";
                if (AsOnDate.Text == "")
                {
                    Today_Date = System.DateTime.Now.ToString("dd/MM/yyyy");
                    makeSQL2 = " SELECT PPRS.SHIFT_ID, PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME_FULL, PI.ITEM_CODE, SUM(PPRM.ITEM_WEIGHT_IN_FG) AS ITEM_WEIGHT_IN_FG FROM PF_PRODUCTION_MASTER PPRM  LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_PRODUCTION_MACHINE PPRMA ON PPRMA.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_PRODUCTION_SHIFT PPRS ON PPRS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID WHERE TO_CHAR(TO_DATE(PPRM.ENTRY_DATE),'dd/mm/yyyy') = '" + Today_Date + "' GROUP BY PPRS.SHIFT_ID, PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER,  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME, PI.ITEM_CODE ORDER BY PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER,  PI.ITEM_ID ASC ";
                }
                else
                {
                    makeSQL2 = " SELECT PPRS.SHIFT_ID, PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME_FULL, PI.ITEM_CODE, SUM(PPRM.ITEM_WEIGHT_IN_FG) AS ITEM_WEIGHT_IN_FG FROM PF_PRODUCTION_MASTER PPRM  LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_PRODUCTION_MACHINE PPRMA ON PPRMA.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_PRODUCTION_SHIFT PPRS ON PPRS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID WHERE TO_CHAR(TO_DATE(PPRM.ENTRY_DATE),'dd/mm/yyyy') = '" + AsOnDate.Text + "' GROUP BY PPRS.SHIFT_ID, PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER,  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME, PI.ITEM_CODE ORDER BY PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER,  PI.ITEM_ID ASC ";

                }

                cmdl = new OracleCommand(makeSQL2);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataBind();

                if (dt.Rows.Count > 0)
                { 
                    //Calculate Sum and display in Footer Row 
                    GridView2.FooterRow.Cells[4].Font.Bold = true;
                    GridView2.FooterRow.Cells[4].Text = "Grand Total";
                    GridView2.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;

                    decimal total = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_WEIGHT_IN_FG"));
                    GridView2.FooterRow.Cells[5].Font.Bold = true;
                    GridView2.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[5].Text = total.ToString("N3");
                }
                else
                {

                }


                string makeSQL3 = "";
                if (AsOnDate.Text == "")
                {
                    Today_Date = System.DateTime.Now.ToString("dd/MM/yyyy");
                    makeSQL3 = "  SELECT  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME_FULL, PI.ITEM_CODE, sum(PPM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PPM.ITEM_AMOUNT) AS ITEM_AMOUNT, ROUND(sum(PPM.ITEM_AMOUNT)/ sum(PPM.ITEM_WEIGHT),2) AS ITEM_AVG_RATE FROM  PF_SALES_MASTER PPM  LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_PURCHASE_TYPE PPT ON PPT.PUR_TYPE_ID = PPM.PUR_TYPE_ID WHERE TO_CHAR(TO_DATE(PPM.ENTRY_DATE),'dd/mm/yyyy') = '" + Today_Date + "'  GROUP BY  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME, PI.ITEM_CODE ORDER BY PI.ITEM_ID ";
                }
                else
                {
                    makeSQL3 = " SELECT  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME_FULL, PI.ITEM_CODE, sum(PPM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PPM.ITEM_AMOUNT) AS ITEM_AMOUNT, ROUND(sum(PPM.ITEM_AMOUNT)/ sum(PPM.ITEM_WEIGHT),2) AS ITEM_AVG_RATE FROM  PF_SALES_MASTER PPM  LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_PURCHASE_TYPE PPT ON PPT.PUR_TYPE_ID = PPM.PUR_TYPE_ID WHERE TO_CHAR(TO_DATE(PPM.ENTRY_DATE),'dd/mm/yyyy') = '" + AsOnDate.Text + "' GROUP BY  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME, PI.ITEM_CODE ORDER BY PI.ITEM_ID ";
                }

                cmd = new OracleCommand(makeSQL3);
                oradata = new OracleDataAdapter(cmd.CommandText, conn);
                ds = new DataTable();
                oradata.Fill(ds);
                GridView3.DataSource = ds;
                GridView3.DataBind();

                if (ds.Rows.Count > 0)
                {
                    GridView3.HeaderRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    //Calculate Sum and display in Footer Row 
                    GridView3.FooterRow.Cells[1].Font.Bold = true;
                    GridView3.FooterRow.Cells[1].Text = "Grand Total";
                    GridView3.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Right;

                    decimal total_wt = ds.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_WEIGHT"));
                    GridView3.FooterRow.Cells[2].Font.Bold = true;
                    GridView3.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GridView3.FooterRow.Cells[2].Text = total_wt.ToString("N3");

                    decimal total_amt = ds.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_AMOUNT"));
                    GridView3.FooterRow.Cells[3].Font.Bold = true;
                    GridView3.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    GridView3.FooterRow.Cells[3].Text = total_amt.ToString("N2");

                    decimal total_avg = (total_amt / total_wt);
                    GridView3.FooterRow.Cells[4].Font.Bold = true;
                    GridView3.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                    GridView3.FooterRow.Cells[4].Text = total_avg.ToString("N2");
                }
                else
                {

                }
                string AsOnDateAr = "", CurrentMonth = ""; 
                string makeSQL4 = "";
                if (AsOnDate.Text == "")
                {
                    Today_Date = System.DateTime.Now.ToString("yyyy-MM-dd");
                    CurrentMonth = System.DateTime.Now.ToString("MM-yyyy");

                    makeSQL4 = " SELECT sum(nvl(PPRM.ITEM_WEIGHT_IN_FG,0)) AS ITEM_WEIGHT_IN_FG, sum(nvl(PMT.ITEM_WEIGHT_PROD,0)) AS ITEM_WEIGHT_PROD, sum(nvl((PMT.ITEM_WEIGHT_PROD - PPRM.ITEM_WEIGHT_IN_FG),0)) AS BELOW_TARGET, nvl(PMT.DAYS,0) AS DAYS, sum(nvl(ROUND(((PMT.ITEM_WEIGHT_PROD - PPRM.ITEM_WEIGHT_IN_FG)/PMT.DAYS),3),0)) AS PER_DAY_ACHIV_TAR FROM (SELECT SUM(ITEM_WEIGHT_IN_FG) AS ITEM_WEIGHT_IN_FG  FROM PF_PRODUCTION_MASTER WHERE  TO_CHAR(TO_DATE(ENTRY_DATE),'mm-yyyy') = '" + CurrentMonth + "') PPRM LEFT JOIN (SELECT ITEM_WEIGHT_PROD, TO_CHAR(TO_DATE(MONTH_YEAR),'mm-yyyy') AS MONTH_YEAR,  trunc(last_day(MONTH_YEAR))- to_date('" + Today_Date + "', 'yyyy-mm-dd') AS DAYS FROM PF_MONTHLY_TARGET WHERE TO_CHAR(TO_DATE(MONTH_YEAR),'mm-yyyy') = '" + CurrentMonth + "') PMT ON  PMT.MONTH_YEAR = '" + CurrentMonth + "' GROUP BY   nvl(PMT.DAYS,0)  ";
                }
                else
                {
                    string MakeEntryDate = AsOnDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    AsOnDateAr = EntryDateNewD.ToString("yyyy-MM-dd");

                    DateTime AsOnDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    CurrentMonth = AsOnDateNewD.ToString("MM-yyyy");

                    makeSQL4 = " SELECT sum(nvl(PPRM.ITEM_WEIGHT_IN_FG,0)) AS ITEM_WEIGHT_IN_FG, sum(nvl(PMT.ITEM_WEIGHT_PROD,0)) AS ITEM_WEIGHT_PROD, sum(nvl((PMT.ITEM_WEIGHT_PROD - PPRM.ITEM_WEIGHT_IN_FG),0)) AS BELOW_TARGET, nvl(PMT.DAYS,0) AS DAYS, sum(nvl(ROUND(((PMT.ITEM_WEIGHT_PROD - PPRM.ITEM_WEIGHT_IN_FG)/PMT.DAYS),3),0)) AS PER_DAY_ACHIV_TAR FROM (SELECT SUM(ITEM_WEIGHT_IN_FG) AS ITEM_WEIGHT_IN_FG  FROM PF_PRODUCTION_MASTER WHERE  TO_CHAR(TO_DATE(ENTRY_DATE),'mm-yyyy') = '" + CurrentMonth + "') PPRM LEFT JOIN (SELECT ITEM_WEIGHT_PROD, TO_CHAR(TO_DATE(MONTH_YEAR),'mm-yyyy') AS MONTH_YEAR,  trunc(last_day(MONTH_YEAR))- to_date('" + AsOnDateAr + "', 'yyyy-mm-dd') AS DAYS FROM PF_MONTHLY_TARGET WHERE TO_CHAR(TO_DATE(MONTH_YEAR),'mm-yyyy') = '" + CurrentMonth + "') PMT ON  PMT.MONTH_YEAR = '" + CurrentMonth + "' GROUP BY   nvl(PMT.DAYS,0)   ";
                } 
                this.GetProAchiveData(AsOnDateAr, makeSQL4);
                 
                conn.Close();
                //alert_box.Visible = false;
           
                if (AsOnDate.Text == "")
                {
                    AsOnDateAr = System.DateTime.Now.ToString("dd-MM-yyyy");
                }
                else
                {
                    string MakeEntryDate = AsOnDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    AsOnDateAr = EntryDateNewD.ToString("dd-MM-yyyy"); 
                    
                  //  AsOnDateAr = AsOnDate.Text;
                }
                 
                   DataTable dg = this.PfMatGetData(AsOnDateAr);

                        RowCount = dg.Rows.Count;

                        html.Append("  <div id='container' style=' margin: 10px' ></div>");
                        html.Append("<table id='datatable' style='display:none'>");
                             html.Append("<tr>");
                             html.Append("<th>-</th>");
                             html.Append("<th>");
                             html.Append(AsOnDateAr);
                             html.Append("</th>");
                             html.Append("<th>Target Per Day</th>");
                            html.Append("</tr>");
                        html.Append("<tbody>"); 
                        for (int j = 0; j < RowCount; j++)
                        {
                            

                            html.Append("<tr>");
                                html.Append("<th>Purchase</th>"); 
                                html.Append("<td>");
                                html.Append(dg.Rows[j]["ITEM_WEIGHT_PUR"].ToString());
                                html.Append("</td>");
                                html.Append("<td>");
                                html.Append(dg.Rows[j]["TARGET_PUR_PER_DAY"].ToString());
                                html.Append("</td>");
                           html.Append("</tr>"); 
                           html.Append("<tr>");
                                html.Append("<th>Production</th>"); 
                                html.Append("<td>");
                                html.Append(dg.Rows[j]["ITEM_WEIGHT_PROD"].ToString()); 
                                html.Append("</td>");
                                html.Append("<td>");
                                html.Append(dg.Rows[j]["TARGET_PROD_PER_DAY"].ToString());
                                html.Append("</td>");
                           html.Append("</tr>");
                           html.Append("<tr>");
                                html.Append("<th>Sales</th>"); 
                                html.Append("<td>");
                                html.Append(dg.Rows[j]["ITEM_WEIGHT_SALES"].ToString()); 
                                html.Append("</td>");
                                html.Append("<td>");
                                html.Append(dg.Rows[j]["TARGET_PROD_PER_DAY"].ToString());
                                html.Append("</td>");
                            html.Append("</tr>");
                        }

                        html.Append(" </tbody>");
                        html.Append(" </table>");
  
                     
                     PlaceHolderGraphReport.Controls.Add(new Literal { Text = html.ToString() });
            }
            else
            {
                //   Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        //start Plastic Factory material information current months 
        protected DataTable PfMatGetData(string AsOnDateAr)
        {

             
            DateTime AsOnDateNewD = DateTime.ParseExact(AsOnDateAr, "dd-MM-yyyy", CultureInfo.InvariantCulture); 
            string CurrentMonth = AsOnDateNewD.ToString("MM-yyyy");

            int userID = Convert.ToInt32(Session["USER_ID"]);
            using (var conn = new OracleConnection(strConnString))
            {
                string query = " SELECT PPM.ITEM_WEIGHT_PUR , sum(ROUND(PMT.TARGET_PUR_PER_DAY)) AS TARGET_PUR_PER_DAY, PPRM.ITEM_WEIGHT_PROD, sum(PMT.TARGET_PROD_PER_DAY) AS TARGET_PROD_PER_DAY, PSM.ITEM_WEIGHT_SALES   FROM (SELECT SUM(ITEM_WEIGHT) AS ITEM_WEIGHT_PUR, TO_CHAR(TO_DATE(ENTRY_DATE),'dd/mm/yyyy') AS ENTRY_DATE FROM PF_PURCHASE_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE),'dd-mm-YYYY') = '" + AsOnDateAr + "' AND PUR_TYPE_ID = '" + DropDownPurchaseTypeID.SelectedValue + "' GROUP BY TO_CHAR(TO_DATE(ENTRY_DATE),'dd/mm/yyyy')) PPM LEFT JOIN (SELECT SUM(ITEM_WEIGHT_IN_FG) AS ITEM_WEIGHT_PROD, TO_CHAR(TO_DATE(ENTRY_DATE),'dd/mm/yyyy') AS ENTRY_DATE  FROM PF_PRODUCTION_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE),'dd-mm-YYYY') = '" + AsOnDateAr + "' GROUP BY TO_CHAR(TO_DATE(ENTRY_DATE),'dd/mm/yyyy')) PPRM ON PPM.ENTRY_DATE = PPRM.ENTRY_DATE LEFT JOIN (SELECT SUM(ITEM_WEIGHT) AS ITEM_WEIGHT_SALES, TO_CHAR(TO_DATE(ENTRY_DATE),'dd/mm/yyyy') AS ENTRY_DATE  FROM PF_SALES_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE),'dd-mm-YYYY') = '" + AsOnDateAr + "' GROUP BY TO_CHAR(TO_DATE(ENTRY_DATE),'dd/mm/yyyy')) PSM ON PSM.ENTRY_DATE = PPRM.ENTRY_DATE LEFT JOIN (SELECT  ITEM_WEIGHT_PUR,  ROUND(ITEM_WEIGHT_PUR/(1+trunc(last_day(MONTH_YEAR))-trunc(MONTH_YEAR,'MM')),3) AS TARGET_PUR_PER_DAY,  ITEM_WEIGHT_PROD, ROUND(ITEM_WEIGHT_PROD/(1+trunc(last_day(MONTH_YEAR))-trunc(MONTH_YEAR,'MM')),3) AS TARGET_PROD_PER_DAY, TO_CHAR(TO_DATE(MONTH_YEAR),'mm-YYYY') AS MONTH_YEAR,  1+trunc(last_day(MONTH_YEAR))-trunc(MONTH_YEAR,'MM') AS DAYS FROM PF_MONTHLY_TARGET WHERE TO_CHAR(TO_DATE(MONTH_YEAR),'mm-YYYY') = '" + CurrentMonth + "') PMT ON  PMT.MONTH_YEAR = '" + CurrentMonth + "' GROUP BY PPM.ITEM_WEIGHT_PUR, PPRM.ITEM_WEIGHT_PROD, PSM.ITEM_WEIGHT_SALES ";
                using (var cmd = new OracleCommand(query, conn))
                {
                    //  cmd.Parameters.Add("NoUserID", SqlDbType.Int);
                    //  cmd.Parameters["NoUserID"].Value = userID;
                    using (var sda = new OracleDataAdapter())
                    {
                        cmd.Connection = conn;
                        sda.SelectCommand = cmd;

                        using (TableData)
                        {
                            TableData.Clear();
                            sda.Fill(TableData);
                            return TableData;
                        }
                    }
                }
            }
        }
        //end Plastic Factory material information current months 

        // production per day achive target

        public void GetProAchiveData(string AsOnDateAr, string QueryString)        
        {
            using (var conn = new OracleConnection(strConnString))
            {
                string query = QueryString;
                using (var cmd = new OracleCommand(query, conn))
                {
                    using (var sda = new OracleDataAdapter())
                    {
                        cmd.Connection = conn;
                        sda.SelectCommand = cmd;
                        using (TableData2)
                        {
                            TableData.Clear();
                            sda.Fill(TableData2);

                        }
                    }
                }  
            }
        }


        public void GetRmAvailableData() //Get all the data and bind it in HTLM Table       
        {
            using (var conn = new OracleConnection(strConnString))
            {
                const string query = " SELECT nvl(SUM(FINAL_STOCK_WT),0) AS FINAL_STOCK_WT,  ROUND((nvl(SUM(FINAL_STOCK_WT),0)/25),0) AS AVAIL_DAYS, trunc(LAST_DAY(SYSDATE))-trunc(SYSDATE)  AS DAYS, (trunc(LAST_DAY(SYSDATE))-trunc(SYSDATE)) * 25 AS MAT_REQ FROM PF_RM_STOCK_INVENTORY_MASTER ";
                using (var cmd = new OracleCommand(query, conn))
                {
                    using (var sda = new OracleDataAdapter())
                    {
                        cmd.Connection = conn;
                        sda.SelectCommand = cmd;
                        using (TableData3)
                        {
                            TableData3.Clear();
                            sda.Fill(TableData3);

                        }
                    }
                }  
            }
        }
       

        protected void GridViewSearchPur(object sender, EventArgs e)
        {
            this.DisplayPurchase();
        }
    

        public void DisplayProduction()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                
               
                conn.Close();
                //alert_box.Visible = false;
            }
            else
            {
             //   Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                storid = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "SHIFT_ID").ToString());
                double tmpTotal = Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "ITEM_WEIGHT_IN_FG"));
                qtyTotal += tmpTotal;
                grQtyTotal += tmpTotal;
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                Label lblTotalqty = (Label)e.Row.FindControl("lblTotalqty");
              //  lblTotalqty.Text =  grQtyTotal.ToString("0.000");
            }
        }


        protected void GridView2_RowCreated(object sender, GridViewRowEventArgs e)
        {
            bool newRow = false;
            if ((storid > 0) && (DataBinder.Eval(e.Row.DataItem, "SHIFT_ID") != null))
            {
                if (storid != Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "SHIFT_ID").ToString()))
                    newRow = true;
            }
            if ((storid > 0) && (DataBinder.Eval(e.Row.DataItem, "SHIFT_ID") == null))
            {
                newRow = true;
                rowIndex = 0;
            }
            if (newRow)
            {
                GridView GridView2 = (GridView)sender;
                GridViewRow NewTotalRow = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Insert);
                NewTotalRow.Font.Bold = true;
                NewTotalRow.BackColor = System.Drawing.ColorTranslator.FromHtml("#00c0ef");
                NewTotalRow.ForeColor = System.Drawing.Color.White;
                TableCell HeaderCell = new TableCell();
                HeaderCell.Text = "Sub Total";
                HeaderCell.HorizontalAlign = HorizontalAlign.Left;
                HeaderCell.ColumnSpan = 4;
                NewTotalRow.Cells.Add(HeaderCell);
                HeaderCell = new TableCell();
                HeaderCell.HorizontalAlign = HorizontalAlign.Right;
                HeaderCell.Text = qtyTotal.ToString("0.000");
                NewTotalRow.Cells.Add(HeaderCell);
                GridView2.Controls[0].Controls.AddAt(e.Row.RowIndex + rowIndex, NewTotalRow);
                rowIndex++;
                qtyTotal = 0;
            }
        }

        protected void GridViewSearchProd(object sender, EventArgs e)
        {
            this.DisplayProduction();
        }
  
        public void clearTextField(object sender, EventArgs e)
        { 
            
        }

        public void clearText()
        { 

        }

        public DataSet ExecuteBySqlString(string sqlString)
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            DataSet ds = new DataSet();
            OracleConnection conn = new OracleConnection(connStr);
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(sqlString, conn);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlString;
                bool mustCloseConnection = false;
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    da.Fill(ds);
                    cmd.Parameters.Clear();
                    if (mustCloseConnection)
                    {
                        conn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }
         
   }
}