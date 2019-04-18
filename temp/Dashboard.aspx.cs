using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.OracleClient;
using System.Data.SqlClient;

namespace NRCAPPS
{
    public partial class Dashboard : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public DataTable TableData = new DataTable();
        OracleCommand cmdl;
        OracleDataAdapter oradata;
        DataTable dtd;
        int RowCount, RowCountj;

        string IS_ITEM_ID = "", IS_ITEM_NAME = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_NAME"] != null)
            {
                //  lb1.Text = "<b><font color=Brown>" + "WELLCOME :: " + "</font>" + "<b><font color=red>" + Session["USER_NAME"] + " User ID:" + Session["USER_ID"] + " U Type: " + Session["NRC_USER_STYPE"] + "</font>";
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                //Building an HTML string.
                StringBuilder html = new StringBuilder();

                string makeSQL = " SELECT NDIU.DASH_ITEM_ID, NDI.ITEM_NAME, NDIU.USER_ID FROM NRC_DASHBOARD_ITEMS_USER NDIU LEFT JOIN NRC_DASHBOARD_ITEMS NDI ON NDI.DASH_ITEM_ID = NDIU.DASH_ITEM_ID WHERE  NDIU.USER_ID  = '" + Session["USER_ID"] + "' AND NDI.IS_ACTIVE = 'Enable' ORDER BY NDIU.ORDER_BY asc ";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dtd = new DataTable();
                oradata.Fill(dtd);
                RowCount = dtd.Rows.Count;

                for (int i = 0; i < RowCount; i++)
                {
                    IS_ITEM_ID = dtd.Rows[i]["DASH_ITEM_ID"].ToString();
                    IS_ITEM_NAME = dtd.Rows[i]["ITEM_NAME"].ToString();

                    if (IS_ITEM_ID == "1")
                    {

                        //start plastic factory material information
                        //Populating a DataTable from database.
                        DataTable dt = this.PfMatGetData();

                        RowCountj = dt.Rows.Count;

                        for (int j = 0; j < RowCountj; j++)
                        {
                            html.Append("<div class='row'>");
                            html.Append("<div class='col-lg-2 col-xs-6'>");
                            html.Append("<div class='small-box bg-yellow'>");
                            html.Append("<div class='inner'>");
                            html.Append("<h3>");
                            html.Append(dt.Rows[j]["PURCHASE_WT"]);
                            html.Append(" <sup style='font-size: 20px'> MT</sup>");
                            html.Append("</h3>");
                            html.Append("<p>Purchase Current Month (WT)</p>");
                            html.Append("</div>");
                            html.Append("<div class='icon' style='padding-top:14px;'>");
                            html.Append("<i class='ion ion-bag'></i>");
                            html.Append("</div>");
                            html.Append("<a href='PF/PfPurchase.aspx' class='small-box-footer'>");
                            html.Append(IS_ITEM_NAME);
                            html.Append(" <i class='fa fa-arrow-circle-right'></i></a>");
                            html.Append("</div>");
                            html.Append("</div>");
                            html.Append("<div class='col-lg-2 col-xs-6'>");
                            html.Append("<div class='small-box bg-yellow'>");
                            html.Append("<div class='inner'>");
                            html.Append("<h3>");
                            html.Append(dt.Rows[j]["PURCHASE_AMT"].ToString());
                            html.Append(" <sup style='font-size: 20px'> SR</sup>");
                            html.Append("</h3>");
                            html.Append("<p>Purchase Current Month (Amount)</p>");
                            html.Append("</div>");
                            html.Append("<div class='icon' style='padding-top:14px;'>");
                            html.Append("<i class='ion  ion-bag'></i>");
                            html.Append("</div>");
                            html.Append("<a href='PF/PfPurchase.aspx' class='small-box-footer'>");
                            html.Append(IS_ITEM_NAME);
                            html.Append(" <i class='fa fa-arrow-circle-right'></i></a>");
                            html.Append("</div>");
                            html.Append("</div>");
                            html.Append("<div class='col-lg-2 col-xs-6'>");
                            html.Append("<div class='small-box bg-light-blue'>");
                            html.Append("<div class='inner'>");
                            html.Append("<h3>");
                            html.Append(dt.Rows[j]["PURCHASE_WTD"].ToString());
                            html.Append(" <sup style='font-size: 20px'> MT</sup>");
                            html.Append("</h3>");
                            html.Append("<p>Direct Sales Current Month (WT)</p>");
                            html.Append("</div>");
                            html.Append("<div class='icon' style='padding-top:14px;'>");
                            html.Append("<i class='ion ion-pie-graph'></i>");
                            html.Append("</div>");
                            html.Append("<a href='PF/PfSales.aspx' class='small-box-footer'>");
                            html.Append(IS_ITEM_NAME);
                            html.Append(" <i class='fa fa-arrow-circle-right'></i></a>");
                            html.Append("</div>");
                            html.Append("</div>");
                            html.Append("<div class='col-lg-2 col-xs-6'>");
                            html.Append("<div class='small-box bg-red'>");
                            html.Append("<div class='inner'>");
                            html.Append("<h3>");
                            html.Append(dt.Rows[j]["PRODUCTION_WT"].ToString());
                            html.Append(" <sup style='font-size: 20px'> MT</sup>");
                            html.Append("</h3>");
                            html.Append("<p>Production Current Month (WT)</p>");
                            html.Append("</div>");
                            html.Append("<div class='icon' style='padding-top:14px;'>");
                            html.Append("<i class='ion ion-pie-graph'></i>");
                            html.Append("</div>");
                            html.Append("<a href='PF/PfProduction.aspx' class='small-box-footer'>");
                            html.Append(IS_ITEM_NAME);
                            html.Append(" <i class='fa fa-arrow-circle-right'></i></a>");
                            html.Append("</div>");
                            html.Append("</div>");
                            html.Append("<div class='col-lg-2 col-xs-6'>");
                            html.Append("<div class='small-box bg-green'>");
                            html.Append("<div class='inner'>");
                            html.Append("<h3>");
                            html.Append(dt.Rows[j]["SALES_WT"].ToString());
                            html.Append(" <sup style='font-size: 20px'> MT</sup>");
                            html.Append("</h3>");
                            html.Append("<p>Sales Current Month (WT)</p>");
                            html.Append("</div>");
                            html.Append("<div class='icon' style='padding-top:14px;'>");
                            html.Append("<i class='ion  ion-stats-bars'></i>");
                            html.Append("</div>");
                            html.Append("<a href='PF/PfSales.aspx' class='small-box-footer'>");
                            html.Append(IS_ITEM_NAME);
                            html.Append(" <i class='fa fa-arrow-circle-right'></i></a>");
                            html.Append("</div>");
                            html.Append("</div>");
                            html.Append("<div class='col-lg-2 col-xs-6'>");
                            html.Append("<div class='small-box bg-green'>");
                            html.Append("<div class='inner'>");
                            html.Append("<h3>");
                            html.Append(dt.Rows[j]["SALES_AMT"].ToString());
                            html.Append(" <sup style='font-size: 20px'> SR</sup>");
                            html.Append("</h3>");
                            html.Append("<p>Sales Current Month (Amount)</p>");
                            html.Append("</div>");
                            html.Append("<div class='icon' style='padding-top:14px;'>");
                            html.Append("<i class='ion  ion-stats-bars'></i>");
                            html.Append("</div>");
                            html.Append("<a href='PF/PfSales.aspx' class='small-box-footer'>");
                            html.Append(IS_ITEM_NAME);
                            html.Append(" <i class='fa fa-arrow-circle-right'></i></a>");
                            html.Append("</div>");
                            html.Append("</div>");

                            html.Append("</div>");
                        }

                    }


                    if (IS_ITEM_ID == "2")
                    {

                        DataTable dt = this.PfChartGetData();

                        RowCountj = dt.Rows.Count;

                        html.Append(" <div class='row'>");
                        html.Append(" <div class='col-md-12'>");
                        html.Append(" <div class='box'>");
                        html.Append("  <div class='box-header with-border'");
                        html.Append("    <h3 class='box-title'><i class='fa fa-bar-chart'></i>");
                        html.Append(IS_ITEM_NAME);
                        html.Append("</h3>");

                        html.Append("    <div class='box-tools pull-right'>");
                        html.Append("     <button type='button' class='btn btn-box-tool' data-widget='collapse'><i class='fa fa-minus'></i>");
                        html.Append("    </button>");
                        html.Append("     <div class='btn-group'>");
                        html.Append("      <button type='button' class='btn btn-box-tool dropdown-toggle' data-toggle='dropdown'>");
                        html.Append("       <i class='fa fa-wrench'></i></button> ");
                        html.Append("   </div>");
                        html.Append("    <button type='button' class='btn btn-box-tool' data-widget='remove'><i class='fa fa-times'></i></button>");
                        html.Append("  </div>");
                        html.Append(" </div> ");
                        html.Append("  <div class='box-body'>");
                        html.Append("   <div class='row'>");
                        html.Append("<div class='col-md-12'>");

                        html.Append("  <div id='container' style=' margin: 10px'></div>");

                        html.Append("<table id='datatable' style='display:none;'>");
                        html.Append("<thead>");
                        html.Append("<tr>");
                        html.Append("<th>ITEM NAME</th>");

                        DataTable dtItem = new DataTable();
                        DataSet di = new DataSet();
                        string makeItemChangeSQL = " SELECT * FROM PF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC ";
                        di = ExecuteBySqlString(makeItemChangeSQL);
                        dtItem = (DataTable)di.Tables[0];

                        for (int l = 0; l < dtItem.Rows.Count; l++)
                        {
                            html.Append("<th>");
                            html.Append(dtItem.Rows[l]["ITEM_NAME"].ToString());
                            html.Append("</th>");
                        }

                        html.Append("</tr>");
                        html.Append("</thead>");
                        html.Append("<tbody>");

                        for (int j = 0; j < RowCountj; j++)
                        {
                            html.Append("<tr>");
                            html.Append("<th>");
                            html.Append(dt.Rows[j]["MONTH_YEAR"].ToString());
                            html.Append("</th>");

                            for (int l = 0; l < dtItem.Rows.Count; l++)
                            {
                                string ItemName = dtItem.Rows[l]["ITEM_NAME"].ToString().Replace(" ", "");
                                html.Append("<td>");
                                html.Append(dt.Rows[j][ItemName].ToString());
                                html.Append("</td>");
                            }

                            html.Append("</tr>");
                        }

                        html.Append(" </tbody>");
                        html.Append(" </table>");
                        html.Append("</div>");
                        html.Append("</div>");
                        html.Append("</div>");
                        html.Append("</div>");
                        html.Append("</div>");
                        html.Append("</div>");

                    }

                    if (IS_ITEM_ID == "3")
                    {

                        DataTable dt = this.PfPieChartGetData();

                        RowCountj = dt.Rows.Count;

                        html.Append(" <div class='row'>");
                        html.Append(" <div class='col-md-12'>");
                        html.Append(" <div class='box'>");
                        html.Append("  <div class='box-header with-border'");
                        html.Append("    <h3 class='box-title'><i class='fa fa-pie-chart'></i>");
                        html.Append(IS_ITEM_NAME);
                        html.Append("</h3>");
                        html.Append(" <div class='box-tools pull-right'>");
                        html.Append(" <button type='button' class='btn btn-box-tool' data-widget='collapse'><i class='fa fa-minus'></i>");
                        html.Append(" </button>");
                        html.Append(" <div class='btn-group'>");
                        html.Append(" <button type='button' class='btn btn-box-tool dropdown-toggle' data-toggle='dropdown'>");
                        html.Append("       <i class='fa fa-wrench'></i></button> ");
                        html.Append("   </div>");
                        html.Append("    <button type='button' class='btn btn-box-tool' data-widget='remove'><i class='fa fa-times'></i></button>");
                        html.Append("  </div>");
                        html.Append(" </div> ");
                        html.Append("  <div class='box-body'>");
                        html.Append("<div class='row'>");

                        html.Append("<div class='col-md-6'>");
                        html.Append("<div id='container_rm_pie' style=' margin: 10px'></div>");
                        html.Append("<table id='datatable_rm_pie' style='display:none'>");
                        html.Append("<thead>");
                        html.Append("<tr>");
                        html.Append("<th>ITEM NAME</th>");
                        html.Append("<th>Raw Material</th>");
                        html.Append("</tr>");
                        html.Append("</thead>");
                        html.Append("<tbody>");
                        for (int j = 0; j < RowCountj; j++)
                        {
                            html.Append("<tr>");
                            html.Append("<th>");
                            html.Append(dt.Rows[j]["ITEM_NAME"].ToString());
                            html.Append("</th>");
                            html.Append("<td>");
                            html.Append(dt.Rows[j]["FINAL_STOCK_WT_RM"].ToString());
                            html.Append("</td>");
                            html.Append("</tr>");
                        }

                        html.Append(" </tbody>");
                        html.Append(" </table>");
                        html.Append("</div>");


                        html.Append("<div class='col-md-6'>");
                        html.Append("<div id='container_fg_pie' style=' margin: 10px'></div>");
                        html.Append("<table id='datatable_fg_pie' style='display:none'>");
                        html.Append("<thead>");
                        html.Append("<tr>");
                        html.Append("<th>ITEM NAME</th>");
                        html.Append("<th>Finished Goods</th>");
                        html.Append("</tr>");
                        html.Append("</thead>");
                        html.Append("<tbody>");
                        for (int j = 0; j < RowCountj; j++)
                        {
                            html.Append("<tr>");
                            html.Append("<th>");
                            html.Append(dt.Rows[j]["ITEM_NAME"].ToString());
                            html.Append("</th>");
                            html.Append("<td>");
                            html.Append(dt.Rows[j]["FINAL_STOCK_WT_FG"].ToString());
                            html.Append("</td>");
                            html.Append("</tr>");
                        }

                        html.Append(" </tbody>");
                        html.Append(" </table>");
                        html.Append("</div>");
                        html.Append("</div>");


                        html.Append("</div>");
                        html.Append("</div>");
                        html.Append("</div>");
                        html.Append("</div>");

                    }


                    if (IS_ITEM_ID == "4")
                    {

                        DataTable dt = this.PfChartGetDataMonthly();

                        RowCountj = dt.Rows.Count;

                        html.Append(" <div class='row'>");
                        html.Append(" <div class='col-md-12'>");
                        html.Append(" <div class='box'>");
                        html.Append("  <div class='box-header with-border'");
                        html.Append("    <h3 class='box-title'><i class='fa fa-bar-chart'></i>");
                        html.Append(IS_ITEM_NAME);
                        html.Append("</h3>");

                        html.Append("    <div class='box-tools pull-right'>");
                        html.Append("     <button type='button' class='btn btn-box-tool' data-widget='collapse'><i class='fa fa-minus'></i>");
                        html.Append("    </button>");
                        html.Append("     <div class='btn-group'>");
                        html.Append("      <button type='button' class='btn btn-box-tool dropdown-toggle' data-toggle='dropdown'>");
                        html.Append("       <i class='fa fa-wrench'></i></button> ");
                        html.Append("   </div>");
                        html.Append("    <button type='button' class='btn btn-box-tool' data-widget='remove'><i class='fa fa-times'></i></button>");
                        html.Append("  </div>");
                        html.Append(" </div> ");
                        html.Append("  <div class='box-body'>");
                        html.Append("   <div class='row'>");
                        html.Append("<div class='col-md-12'>");

                        html.Append("  <div id='container_pf_monthly' style=' margin: 10px'></div>");

                        html.Append("<table id='datatable_pf_monthly' style='display:none;'>");
                        html.Append("<thead>");
                        html.Append("<tr>");
                        html.Append("<th>Months</th>");
                        html.Append("<th>Purchase</th>");  
                        html.Append("<th>Production</th>");   
                        html.Append("<th>Sales</th>");                        
                        html.Append("</tr>");
                        html.Append("</thead>");
                        html.Append("<tbody>");

                        for (int j = 0; j < RowCountj; j++)
                        {
                            html.Append("<tr>");
                            html.Append("<th>");
                            html.Append(dt.Rows[j]["MONTH_YEAR"].ToString());
                            html.Append("</th>"); 
                            html.Append("<td>");
                            html.Append(dt.Rows[j]["PURCHASE_WEIGHT"].ToString());
                            html.Append("</td>");
                            html.Append("<td>");
                            html.Append(dt.Rows[j]["PRODUCTION_WEIGHT"].ToString());
                            html.Append("</td>");
                            html.Append("<td>");
                            html.Append(dt.Rows[j]["SALES_WEIGHT"].ToString());
                            html.Append("</td>");
                            html.Append("</tr>");
                        }

                        html.Append(" </tbody>");
                        html.Append(" </table>");
                        html.Append("</div>");
                        html.Append("</div>");
                        html.Append("</div>");
                        html.Append("</div>");
                        html.Append("</div>");
                        html.Append("</div>");

                    }
                }


                PlaceHolderDashboardReport.Controls.Add(new Literal { Text = html.ToString() });

                /*        //Table start.
                        html.Append("<table border = '1'>");

                        //Building the Header row.
                        html.Append("<tr>");
                        foreach (DataColumn column in dt.Columns)
                        {
                            html.Append("<th>");
                            html.Append(column.ColumnName);
                            html.Append("</th>");
                        }
                        html.Append("</tr>");

                        //Building the Data rows.
                        foreach (DataRow row in dt.Rows)
                        {
                            html.Append("<tr>");
                            foreach (DataColumn column in dt.Columns)
                            {
                                html.Append("<td>");
                                html.Append(row[column.ColumnName]);
                                html.Append("</td>");
                            }
                            html.Append("</tr>");
                        }

                        //Table end.
                        html.Append("</table>");  */

                //Append the HTML string to Placeholder.



            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }

        }

        //start Plastic Factory material information current months 
        protected DataTable PfMatGetData()
        {
            int userID = Convert.ToInt32(Session["USER_ID"]);
            using (var conn = new OracleConnection(strConnString))
            {
                string query = " SELECT TO_CHAR(sum(nvl(PPM.ITEM_WEIGHT,0)),'999,999.999') AS PURCHASE_WT, TO_CHAR(sum(nvl(PPM.ITEM_AMOUNT,0)),'9,999,999,999.99' )  AS PURCHASE_AMT, TO_CHAR(sum(nvl(PPMD.ITEM_WEIGHT,0)),'999,999.99') AS PURCHASE_WTD, TO_CHAR(sum(nvl(PPMAS.ITEM_WEIGHT,0)),'999,999.999') AS PRODUCTION_WT, TO_CHAR(sum(nvl(PSMAS.ITEM_WEIGHT,0)),'999,999.999') AS SALES_WT, TO_CHAR(sum(nvl(PSMAS.ITEM_AMOUNT,0)),'9,999,999,999.99' ) AS SALES_AMT FROM PF_ITEM PI LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT, sum(ITEM_AMOUNT) AS ITEM_AMOUNT FROM PF_PURCHASE_MASTER  WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = TO_CHAR(TO_DATE(sysdate), 'mm-YYYY') GROUP BY ITEM_ID) PPM ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_SALES_MASTER  WHERE PUR_TYPE_ID = 1  AND TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = TO_CHAR(TO_DATE(sysdate), 'mm-YYYY')  GROUP BY ITEM_ID) PPMD ON PI.ITEM_ID = PPMD.ITEM_ID  LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT FROM PF_PRODUCTION_MASTER WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = TO_CHAR(TO_DATE(sysdate), 'mm-YYYY')   GROUP BY ITEM_ID) PPMAS ON PI.ITEM_ID = PPMAS.ITEM_ID LEFT JOIN (SELECT ITEM_ID, sum(ITEM_WEIGHT) AS ITEM_WEIGHT, sum(ITEM_AMOUNT) AS ITEM_AMOUNT FROM PF_SALES_MASTER  WHERE TO_CHAR(TO_DATE(ENTRY_DATE), 'mm-YYYY') = TO_CHAR(TO_DATE(sysdate), 'mm-YYYY')   GROUP BY ITEM_ID) PSMAS ON PI.ITEM_ID = PSMAS.ITEM_ID ";
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

        //start Plastic Factory chart last 12 months (month value is variable)
        protected DataTable PfChartGetData()
        {
            int userID = Convert.ToInt32(Session["USER_ID"]);

            DataTable dtItem = new DataTable();
            DataSet di = new DataSet();
            string makeItemChangeSQL = " SELECT * FROM PF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC ";
            di = ExecuteBySqlString(makeItemChangeSQL);
            dtItem = (DataTable)di.Tables[0];

            string query_str = "SELECT  TO_CHAR(TO_DATE(PY.MONTH_YEAR_NAME), 'mm-YYYY') AS MONTH_YEAR, ";
            string query_str2 = " FROM PF_MONTH_YEAR PY ";
            string query_str3 = "";
            string query_str4 = " WHERE PY.IS_ACTIVE = 'Enable' ORDER BY TO_CHAR(TO_DATE(PY.MONTH_YEAR_NAME), 'YYYY/MM') asc"; 
            for (int i = 0; i < dtItem.Rows.Count; i++)
            {
                query_str += "nvl(" + dtItem.Rows[i]["ITEM_NAME"].ToString().Replace(" ", "") + ".ITEM_AMOUNT,0) AS " + dtItem.Rows[i]["ITEM_NAME"].ToString().Replace(" ", "") + ", ";
                query_str3 += " LEFT JOIN (SELECT PSM.ITEM_ID, PI.ITEM_NAME, sum(nvl(PSM.ITEM_AMOUNT,0)) AS ITEM_AMOUNT, TO_CHAR(TO_DATE(PSM.ENTRY_DATE), 'mm-YYYY') AS ENTRY_DATE FROM PF_SALES_MASTER PSM LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID  WHERE  PSM.ITEM_ID = " + dtItem.Rows[i]["ITEM_ID"].ToString().Replace(" ", "") + " GROUP BY PSM.ITEM_ID, PI.ITEM_NAME, TO_CHAR(TO_DATE(PSM.ENTRY_DATE), 'mm-YYYY')  ) " + dtItem.Rows[i]["ITEM_NAME"].ToString().Replace(" ", "") + " ON TO_CHAR(TO_DATE(PY.MONTH_YEAR_NAME), 'mm-YYYY') = " + dtItem.Rows[i]["ITEM_NAME"].ToString().Replace(" ", "") + ".ENTRY_DATE ";
            }
            query_str = query_str.Remove(query_str.Length - 2) + query_str2 + query_str3 + query_str4;
             
            using (var conn = new OracleConnection(strConnString))
            { 
                using (var cmd = new OracleCommand(query_str, conn))
                { 
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

        //start Plastic Factory chart last 12 months (purchase, poduction & sales )
        protected DataTable PfChartGetDataMonthly()
        {
            int userID = Convert.ToInt32(Session["USER_ID"]);

            string query_str = "SELECT * FROM VIEW_PF_ITEM_WT_MONTHLY_SUM";
            using (var conn = new OracleConnection(strConnString))
            {
                using (var cmd = new OracleCommand(query_str, conn))
                {
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
        //start Plastic Factory chart last 12 months (month value is variable)
        protected DataTable PfPieChartGetData()
        {
            int userID = Convert.ToInt32(Session["USER_ID"]);
            using (var conn = new OracleConnection(strConnString))
            {
                string query = " SELECT  PI.ITEM_NAME,  nvl(PRSIM.FINAL_STOCK_WT,0) AS FINAL_STOCK_WT_RM, nvl(PFSIM.FINAL_STOCK_WT,0) AS FINAL_STOCK_WT_FG FROM PF_ITEM PI LEFT JOIN PF_RM_STOCK_INVENTORY_MASTER PRSIM ON PRSIM.ITEM_ID = PI.ITEM_ID LEFT JOIN PF_FG_STOCK_INVENTORY_MASTER PFSIM ON PFSIM.ITEM_ID = PI.ITEM_ID  ORDER BY PI.ITEM_ID asc"; //WHERE nvl(PRSIM.FINAL_STOCK_WT,0)>0 OR nvl(PFSIM.FINAL_STOCK_WT,0)>0
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