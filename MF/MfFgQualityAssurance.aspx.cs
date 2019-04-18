using System; 
using System.Configuration;
using System.Data; 
using System.Web.UI; 
using System.Web.UI.WebControls; 
using System.Data.OracleClient;
using System.IO; 
using System.Collections.Generic; 
using System.Data.SqlClient;
using System.Text;
using System.Globalization;

namespace NRCAPPS.MF
{
    public partial class MfFgQualityAssurance : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
        OracleDataAdapter oradata;
        DataTable dt;
        int RowCount;

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
                          
                        alert_box.Visible = false;
                         
                        Display(); 
                        alert_box.Visible = false;

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
        
        
        protected void BtProductionPost_Click(object sender, EventArgs e)
        {
        try
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            LinkButton btn = (LinkButton)sender;
            Session["page_data_id"] = btn.CommandArgument;
            string USER_DATA_ID = Convert.ToString(Session["page_data_id"]);

            int userID = Convert.ToInt32(Session["USER_ID"]); 
            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt"); 
             
            string update_batch = "update  MF_PRODUCTION_BATCH_MASTER  set QUALITY_APPRO_LV_ONE_S = :TextBatchStatus,  QUALITY_APPRO_LV_ONE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , QUALITY_APPRO_LV_ONE_S_ID = :NoU_USER_ID where BATCH_NO = :NoBatchID ";
            cmdi = new OracleCommand(update_batch, conn);

            OracleParameter[] objPr = new OracleParameter[4];
            objPr[0] = cmdi.Parameters.Add("TextBatchStatus", "Approved"); 
            objPr[1] = cmdi.Parameters.Add("u_date", u_date);
            objPr[2] = cmdi.Parameters.Add("NoU_USER_ID", userID);
            objPr[3] = cmdi.Parameters.Add("NoBatchID", USER_DATA_ID);

            cmdi.ExecuteNonQuery();
            cmdi.Parameters.Clear();
            cmdi.Dispose();
             
            conn.Close();

            alert_box.Visible = true;
            alert_box.Controls.Add(new LiteralControl("Quality Assurance Data Post Complete Successfully"));
            alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
              
            Display();   
               }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
            }
            catch
            {
                Response.Redirect("~/ParameterError.aspx");
            }

        }

       

        public void Display()
        {  
          if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                  
                string makeSQL = "";
                if (txtSearchUserRole.Text == "")
                {
                    makeSQL = " SELECT MPIM.PRODUCTION_ID, MPIM.BATCH_NO, MPIM.ENTRY_DATE, MPIM.CREATE_DATE, MPIM.UPDATE_DATE, MPIM.IS_ACTIVE, MPF.FURNACES_NAME, MP.PARTY_NAME, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME, MPBM.INVEN_AVAIL_CHECK_S, MPBM.PRODUCTION_ISSUE_S, MPBM.PRODUCTION_ISSUE_POST_S, QUALITY_APPRO_LV_ONE_S, QUALITY_APPRO_LV_IS_PRINT, TO_CHAR(QUALITY_APPRO_LV_PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS QUALITY_APPRO_LV_PRINT_DATE FROM MF_PRODUCTION_ISSUE_MASTER MPIM LEFT JOIN MF_PRODUCTION_BATCH_MASTER MPBM ON MPBM.BATCH_NO = MPIM.BATCH_NO LEFT JOIN MF_PRODUCTION_FURNACES MPF ON MPF.FURNACES_ID = MPBM.FURNACES_ID LEFT JOIN MF_PARTY MP ON MP.PARTY_ID = MPBM.PARTY_ID LEFT JOIN MF_PRODUCTION_GRADE MPG ON MPG.GRADE_ID = MPBM.GRADE_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID WHERE MPBM.PRODUCTION_ISSUE_S = 'Ongoing' ORDER BY MPIM.PRODUCTION_ID DESC ";
                }
                else
                {
                    makeSQL = " SELECT MPIM.PRODUCTION_ID, MPIM.BATCH_NO, MPIM.ENTRY_DATE, MPIM.CREATE_DATE, MPIM.UPDATE_DATE, MPIM.IS_ACTIVE, MPF.FURNACES_NAME, MP.PARTY_NAME, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME, MPBM.INVEN_AVAIL_CHECK_S, MPBM.PRODUCTION_ISSUE_S, MPBM.PRODUCTION_ISSUE_POST_S, QUALITY_APPRO_LV_ONE_S, QUALITY_APPRO_LV_IS_PRINT, TO_CHAR(QUALITY_APPRO_LV_PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS QUALITY_APPRO_LV_PRINT_DATE FROM MF_PRODUCTION_ISSUE_MASTER MPIM LEFT JOIN MF_PRODUCTION_BATCH_MASTER MPBM ON MPBM.BATCH_NO = MPIM.BATCH_NO LEFT JOIN MF_PRODUCTION_FURNACES MPF ON MPF.FURNACES_ID = MPBM.FURNACES_ID LEFT JOIN MF_PARTY MP ON MP.PARTY_ID = MPBM.PARTY_ID LEFT JOIN MF_PRODUCTION_GRADE MPG ON MPG.GRADE_ID = MPBM.GRADE_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID WHERE MPBM.BATCH_NO like '" + txtSearchUserRole.Text + "%' or MI.ITEM_NAME like '" + txtSearchUserRole.Text + "%' or MPF.FURNACES_NAME like '" + txtSearchUserRole.Text + "%'  or MPBM.IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY MPIM.PRODUCTION_ID desc";

                    alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataKeyNames = new string[] { "BATCH_NO" }; 
                GridView2.DataBind();
                conn.Close(); 
            }
           
       }

        protected void GridViewSearchUser(object sender, EventArgs e)
        {
            this.Display(); 
        }

        protected void GridViewUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView2.PageIndex = e.NewPageIndex;
            Display(); 
            alert_box.Visible = false;
        }

        protected void gridViewFileInformation_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow Row in GridView2.Rows)
            {
                string IsBatchPostCheck = (Row.FindControl("IsProductionPost") as Label).Text; 
                if (IsBatchPostCheck == "Complete")   
                { 
                    (Row.FindControl("BtProductionPost_Click") as LinkButton).Enabled = false;
                }
                 
            }
             
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            if (IS_PRINT_ACTIVE == "Enable")
            {
                alert_box.Visible = false;
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                LinkButton btn = (LinkButton)sender;
                Session["user_data_id"] = btn.CommandArgument;
                string BatchNo = Session["user_data_id"].ToString();

                string HtmlString = "";
                string makeSQL = " SELECT  TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, QUALITY_APPRO_LV_ONE_S FROM MF_PRODUCTION_BATCH_MASTER WHERE BATCH_NO = '" + BatchNo + "' ";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                for (int i = 0; i < RowCount; i++)
                {  
                    string EntryDateSlip = dt.Rows[i]["ENTRY_DATE"].ToString(); 
                    HtmlString += "<div style='float:left;width:775px;height:335px;margin:10px 20px 10px 25px;padding:10px 10px 0 0;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 15px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;border:black solid 1px'> ";
                    HtmlString += "<div style='float:left;width:825px;margin-top:5px;height:82px;text-align:center;' ><img src='../../image/logo_from.png'/></div> ";
                    HtmlString += "<div style='float:left;width:825px;height:20px;text-align:center;font-family:Times New Roman,Times, serif;font-size:17px;font-weight:700;' >Metal Factory Division</div> ";
                    HtmlString += "<div style='float:left;width:825px;text-align:center;font-family:Times New Roman,Times, serif;font-size:16px;font-weight:700;' >Certificate Of Material Quality Assurance</div> ";

                    HtmlString += "<div style='float:left;width:340px;margin:15px 0 0 20px;text-align:left;' ><span style='font-family:Times New Roman,Times, serif;font-size:15px;font-weight:700;'></span> <span style='color:Red;font-size:15px;'></span></div> ";
                    HtmlString += "<div style='float:left;width:407px;margin-top:15px;text-align:right;'><span style='font-family:Times New Roman;'>Print Date :</span>" + DateTime.Now.ToString("dd/MM/yyyy h:mm:ss tt") + "</div> ";
                    HtmlString += "<div style='float:left;width:750px;margin:6px 0 0 18px;'> ";
                     
                    HtmlString += "<table width=100%; cellpadding=5px; cellspacing=0>";
                    HtmlString += "<thead>";
                    HtmlString += "<tr>";
                  
                    HtmlString += "<th style='border:black solid 1px;'><span style='font-family:Times New Roman;'> Batch Number </span></th>"; 
                    HtmlString += "<th style='border:black solid 1px;'><span style='font-family:Times New Roman;'> Date</span></th>";
                    HtmlString += "<th style='border:black solid 1px;'><span style='font-family:Times New Roman;'> Status </span></th>"; 
                    HtmlString += " </tr>";
                    HtmlString += "</thead>";
                    HtmlString += "<tbody>";
                    HtmlString += "<tr>";
                    HtmlString += "<td  style='border:black solid 1px;' align='center'>";
                    HtmlString += "" + BatchNo + "";
                    HtmlString += "</td>";
                    HtmlString += "<td  style='border:black solid 1px;' align='center'>";
                    HtmlString += "" + EntryDateSlip + "";
                    HtmlString += "</td>"; 
                    HtmlString += "<td  style='border:black solid 1px;' align='center'>";
                    HtmlString += "" + dt.Rows[i]["QUALITY_APPRO_LV_ONE_S"].ToString() + "";
                    HtmlString += "</td>";
                     
                    HtmlString += "</tr>";
                    HtmlString += "</tbody>";
                    HtmlString += "</table>"; 
                    HtmlString += "</div>";

                    HtmlString += "<div style='float:left;width:340px;margin:65 0 0 20px;text-align:left;' ><span style='font-family:Times New Roman,Times, serif;font-size:15px;font-weight:700;'>Prepared By :</span></div> ";
                    HtmlString += "<div style='float:left;width:407px;margin-top:65px;text-align:right;'><span style='font-family:Times New Roman;'>Approved By :</span></div> ";

                }
                HtmlString += "</div>";
                HtmlString += "</div>";
                HtmlString += "</div>";

                // purchase master update for print
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string update_user = "update  MF_PRODUCTION_BATCH_MASTER  set QUALITY_APPRO_LV_IS_PRINT = :TextIsPrint, QUALITY_APPRO_LV_PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), QUALITY_APPRO_LV_PRINT_ID = :NoCuserID where BATCH_NO = :NoSlipNoWp ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[4];
                objPrm[0] = cmdi.Parameters.Add("TextIsPrint", "Printed");
                objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[2] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[3] = cmdi.Parameters.Add("NoSlipNoWp", BatchNo);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();

                PanelPrint.Controls.Add(new LiteralControl(HtmlString));
                Session["ctrl"] = PanelPrint;
                ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('Print.aspx','PrintMe','height=900px,width=1200px,scrollbars=1');</script>");
                Display();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }


        protected void DisplayBatchDetalis(object sender, CommandEventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            string BatchNumber =  e.CommandArgument.ToString();

            //Building an HTML string.
            StringBuilder html = new StringBuilder();

            string makeBatchSQL = " select MPBM.BATCH_ID, MPBM.BATCH_NO, MPF.FURNACES_NAME, MP.PARTY_NAME, MPBM.GRADE_ID, TO_CHAR(MPBM.ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, MPIM.IS_ACTIVE, NU.USER_NAME, TO_CHAR(MPIM.CREATE_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS CREATE_DATE  from MF_PRODUCTION_BATCH_MASTER MPBM LEFT JOIN MF_PRODUCTION_ISSUE_MASTER MPIM ON MPIM.BATCH_NO = MPBM.BATCH_NO LEFT JOIN MF_PRODUCTION_FURNACES MPF ON MPF.FURNACES_ID = MPBM.FURNACES_ID LEFT JOIN MF_PARTY MP ON MP.PARTY_ID = MPBM.PARTY_ID  LEFT JOIN NRC_USER NU ON NU.USER_ID = MPIM.C_USER_ID WHERE MPBM.BATCH_NO = '" + BatchNumber + "'";
             
            cmdl = new OracleCommand(makeBatchSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;
            for (int i = 0; i < RowCount; i++)
            {
                html.Append("<div class='box box-info'><div class='box-header with-border'><h3 class='box-title'>Issue Production - Batch General Info</h3></div><div class='box-body'>");

                html.Append("<table class='table table-hover table-bordered table-striped' cellpadding='0' cellspacing='0' width=100%>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Batch No </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["BATCH_NO"].ToString());
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Furnace Name </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["FURNACES_NAME"].ToString());
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Customer Name </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["PARTY_NAME"].ToString());
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Grade ID </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["GRADE_ID"].ToString());
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Entry Date </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["ENTRY_DATE"].ToString());
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Is Active </th>");
                html.Append("<td style='text-align:left;'>");

                if (dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable")
                {
                    html.Append("<span class=\"label label-success\" >Enable</span>");
                }
                else { html.Append("<span class=\"label label-danger\" >Disable</span>"); }

                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Created </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["USER_NAME"].ToString());
                html.Append(" <span class=\"label label-info\" > " + dt.Rows[i]["CREATE_DATE"].ToString() + "</span>");
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("</table>");

                html.Append("</div>");
                html.Append("</div>");

            }
             
            PlaceHolderBatchDetails.Controls.Add(new Literal { Text = html.ToString() });

            string makeSQL = " select  MPII.*, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME from MF_PRODUCTION_ISSUE_ITEM MPII LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPII.ITEM_ID  WHERE  MPII.BATCH_NO = '" + BatchNumber + "' ORDER BY MPII.ITEM_ID ";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            GridView5.DataSource = dt;
            GridView5.DataKeyNames = new string[] { "ITEM_NAME" };
            GridView5.DataBind();
             

            string makeSQL2 = " SELECT row_number() OVER (ORDER BY MI.ITEM_ID) AS SL_NO, MI.ITEM_ID, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME, nvl(MPBTI.ITEM_WEIGHT_CWT,0) AS ITEM_WEIGHT_TARGET, nvl(MPII.ITEM_WEIGHT,0) AS ITEM_WEIGHT_ACTUAL, (nvl(MPBTI.ITEM_WEIGHT_CWT,0) -  nvl(MPII.ITEM_WEIGHT,0)) AS VARIANCE_WT FROM MF_ITEM MI LEFT JOIN(SELECT ITEM_ID, SUM(ITEM_WEIGHT_CWT) AS ITEM_WEIGHT_CWT FROM MF_PRODUCTION_BAT_TARGET_ITEM WHERE BATCH_NO = '" + BatchNumber + "' GROUP BY ITEM_ID)MPBTI ON MPBTI.ITEM_ID = MI.ITEM_ID LEFT JOIN(SELECT ITEM_ID, SUM(ITEM_WEIGHT) AS ITEM_WEIGHT FROM MF_PRODUCTION_ISSUE_ITEM WHERE BATCH_NO = '" + BatchNumber + "' GROUP BY ITEM_ID)MPII ON MPII.ITEM_ID = MI.ITEM_ID WHERE(MPBTI.ITEM_WEIGHT_CWT > 0 OR MPII.ITEM_WEIGHT > 0) ORDER BY MI.ITEM_ID ";

            cmdl = new OracleCommand(makeSQL2);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            GridView3.DataSource = dt;
            GridView3.DataKeyNames = new string[] { "ITEM_ID" };
            GridView3.DataBind();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "myModalDetails", "showPopup();", true);
             
        }


        protected void BtnReport_Click(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad = true;
                alert_box.Visible = false;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }

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
 