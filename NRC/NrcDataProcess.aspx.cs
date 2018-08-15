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
    public partial class NrcDataProcess : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
        OracleDataAdapter oradata;
        DataTable dt;
        int RowCount;
        string IS_PAGE_ACTIVE = "";
        string IS_ADD_ACTIVE = "";
        string IS_EDIT_ACTIVE = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE = "";

        public bool IsLoad { get; set; }
        public DataTable TableData = new DataTable();
        public DataTable TableData2 = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["USER_NAME"] != null)
            {
                string requestedFile = Path.GetFileName(Request.Path);
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = " SELECT  NUPP.IS_PAGE_ACTIVE, NUPP.IS_ADD_ACTIVE, NUPP.IS_EDIT_ACTIVE, NUPP.IS_DELETE_ACTIVE, NUPP.IS_VIEW_ACTIVE FROM NRC_USER_PAGE_PERMISSION NUPP LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NUPP.USER_PAGE_ID  WHERE NUPP.USER_ID = '" + Session["USER_ID"] + "' AND NUP.IS_ACTIVE = 'Enable' AND NUP.PAGE_URL = '" + requestedFile + "' ";

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
                }

                if (IS_PAGE_ACTIVE == "Enable")
                {

                    if (!IsPostBack)
                    {

                        alert_box.Visible = false;

                    }
                    IsLoad = false;
                }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }


            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }


        public void BtnDataProcPf_Click(object sender, EventArgs e)
        {
          try {
            if (IS_ADD_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);

                int userID = Convert.ToInt32(Session["USER_ID"]);
                string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string MakeAsOnDate = TextMonthYear0.Text;
                string[] MakeAsOnDateSplit = MakeAsOnDate.Split('-');
                String AsOnDateTemp = MakeAsOnDateSplit[0].Replace("/", "-");
                DateTime AsOnDateNewD = DateTime.ParseExact(AsOnDateTemp, "MM-yyyy", CultureInfo.InvariantCulture);
                string AsOnDateNew = AsOnDateNewD.ToString("dd-MM-yyyy");

                DateTime curDate = AsOnDateNewD;
                DateTime startDate = curDate.AddMonths(-1);
                DateTime LastDateTemp = curDate.AddDays(-(curDate.Day));
                string LastDate = LastDateTemp.ToString("dd-MM-yyyy");
                string LastMonth = startDate.ToString("MM-yyyy");
                string CurrentMonth = AsOnDateNewD.ToString("MM-yyyy");
                DateTime LastDateCurrentMonthTemp = AsOnDateNewD.AddMonths(1).AddDays(-1);
                string LastDateCurrentMonth = LastDateCurrentMonthTemp.ToString("dd-MM-yyyy");

                OracleCommand cmd = new OracleCommand("PRO_PF_DATA_PROCESS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("TextLastDate", OracleType.VarChar)).Value = LastDate;
                cmd.Parameters.Add(new OracleParameter("TextLastMonth", OracleType.VarChar)).Value = LastMonth;
                cmd.Parameters.Add(new OracleParameter("TextCurrentMonth", OracleType.VarChar)).Value = CurrentMonth;
                cmd.Parameters.Add(new OracleParameter("TextLastDateCurrentMonth", OracleType.VarChar)).Value = LastDateCurrentMonth;

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Plastic Factory Data Process Successfully " + LastDateCurrentMonth));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                clearText();
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

        public void clearTextField(object sender, EventArgs e)
        {

            CheckMonthYear.Text = "";
            CheckMonthYear.Text = "";
            TextMonthYear0.Text = "";
            alert_box.Visible = false;
        }

        public void clearText()
        {
            CheckMonthYear.Text = "";
            TextMonthYear0.Text = "";

        }

        public void TextItem_TextChanged(object sender, EventArgs e)
        {
            TextMonthYear0.Text = "";
            CheckMonthYear.Text = "";
        }

        public void CheckDataProcess_PF_Monthly(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextMonthYear0.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;

                string MakeAsOnDate = TextMonthYear0.Text;
                string[] MakeAsOnDateSplit = MakeAsOnDate.Split('-');
                String AsOnDateTemp = MakeAsOnDateSplit[0].Replace("/", "-");

                DateTime AsOnDateNewD = DateTime.ParseExact(AsOnDateTemp, "MM-yyyy", CultureInfo.InvariantCulture);
                DateTime LastDateTemp = AsOnDateNewD.AddMonths(1).AddDays(-1);
                string LastDate = LastDateTemp.ToString("dd-MM-yyyy");

                cmd.CommandText = "select ITEM_ID from PF_RM_STOCK_INVENTORY_HISTORY where  TO_CHAR(TO_DATE(CREATE_DATE), 'dd-mm-YYYY') = '" + LastDate + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckMonthYear.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Already Data Process for this Month (" + TextMonthYear0.Text + ")</label>";
                    CheckMonthYear.ForeColor = System.Drawing.Color.Red;
                    TextMonthYear0.Focus();
                    BtnDataProcPf.Attributes.Add("aria-disabled", "false");
                    BtnDataProcPf.Attributes.Add("class", "btn btn-warning disabled");
                }
                else
                {
                    CheckMonthYear.Text = "<label class='control-label'><i class='fa fa fa-check'></i> No Data Process for this Month  (" + TextMonthYear0.Text + ")</label>";
                    CheckMonthYear.ForeColor = System.Drawing.Color.Green;
                    BtnDataProcPf.Attributes.Add("aria-disabled", "true");
                    BtnDataProcPf.Attributes.Add("class", "btn btn-warning active");

                }
            }
            else
            {
                CheckMonthYear.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Month is not blank</label>";
                CheckMonthYear.ForeColor = System.Drawing.Color.Red;
                TextMonthYear0.Focus();
            }

        }

    }
}