﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
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
    public partial class PfInventoryRm : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdl;
        OracleDataAdapter oradata;
        DataTable dt;
        int RowCount;

        string IS_PAGE_ACTIVE   = "";
        string IS_ADD_ACTIVE    = "";
        string IS_EDIT_ACTIVE   = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE   = "";  
 
        public bool IsLoad { get; set; }
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
                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeDropDownItemSQL = " SELECT * FROM PF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        dsi = ExecuteBySqlStringType(makeDropDownItemSQL);
                        dtItemID = (DataTable)dsi.Tables[0];
                        DropDownItemID.DataSource = dtItemID;
                        DropDownItemID.DataValueField = "ITEM_ID";
                        DropDownItemID.DataTextField = "ITEM_NAME";
                        DropDownItemID.DataBind();
                        DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));

                        DropDownItemID1.DataSource = dtItemID;
                        DropDownItemID1.DataValueField = "ITEM_ID";
                        DropDownItemID1.DataTextField = "ITEM_NAME";
                        DropDownItemID1.DataBind();
                        DropDownItemID1.Items.Insert(0, new ListItem("Select  Item", "0"));
  
                        Display();
                        DisplayRmHistory();

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

        

        protected void linkSelectClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             LinkButton btn = (LinkButton)sender;
             Session["user_page_data_id"] = btn.CommandArgument;
             int USER_DATA_ID = Convert.ToInt32(Session["user_page_data_id"]); 
             

             DataTable dtUserTypeID = new DataTable();
             DataSet ds = new DataSet();
             string makeSQL = " select *  from PF_RM_STOCK_INVENTORY_MASTER where RM_INVENTORY_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextInventoryRmID.Text = dt.Rows[i]["RM_INVENTORY_ID"].ToString();
                 DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();
               //  DropDownSubItemID.Text = dt.Rows[i]["SUB_ITEM_ID"].ToString();
                 TextInitialStock.Text = decimal.Parse(dt.Rows[i]["INITIAL_STOCK_WT"].ToString()).ToString("0.000"); 
                 TextStockIn.Text = decimal.Parse(dt.Rows[i]["STOCK_IN_WT"].ToString()).ToString("0.000"); 
                 TextStockOut.Text = decimal.Parse(dt.Rows[i]["STOCK_OUT_WT"].ToString()).ToString("0.000");
                 TextFinalStock.Text = decimal.Parse(dt.Rows[i]["FINAL_STOCK_WT"].ToString()).ToString("0.000");
                 TextItemAvgRate.Text = decimal.Parse(dt.Rows[i]["ITEM_AVG_RATE"].ToString()).ToString("0.00");             
             } 
             
             conn.Close();
             Display(); 
             alert_box.Visible = false;  

        }

        protected void linkSelectRmHisClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             LinkButton btn = (LinkButton)sender;
             Session["user_page_data_id"] = btn.CommandArgument;
             int USER_DATA_ID = Convert.ToInt32(Session["user_page_data_id"]); 
             

             DataTable dtUserTypeID = new DataTable();
             DataSet ds = new DataSet();
             string makeSQL = " select * from PF_RM_STOCK_INVENTORY_HISTORY where IN_RM_HIS_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextInventoryRmHisID.Text = dt.Rows[i]["IN_RM_HIS_ID"].ToString();
                 DropDownItemID1.Text = dt.Rows[i]["ITEM_ID"].ToString(); 
                 TextInitialStockHis.Text = decimal.Parse(dt.Rows[i]["INITIAL_STOCK_WT"].ToString()).ToString("0.000"); 
                 TextStockInHis.Text = decimal.Parse(dt.Rows[i]["STOCK_IN_WT"].ToString()).ToString("0.000"); 
                 TextStockOutHis.Text = decimal.Parse(dt.Rows[i]["STOCK_OUT_WT"].ToString()).ToString("0.000");
                 TextFinalStockHis.Text = decimal.Parse(dt.Rows[i]["FINAL_STOCK_WT"].ToString()).ToString("0.000");
                 TextItemAvgRateHis.Text = decimal.Parse(dt.Rows[i]["ITEM_AVG_RATE"].ToString()).ToString("0.00");             
             } 
             
             conn.Close();
             Display(); 
             alert_box.Visible = false;  

        } 

        public void Display()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open(); 

                string makeSQL   = " select  * from PF_RM_STOCK_INVENTORY_MASTER ORDER BY ITEM_ID asc";
                  
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "RM_INVENTORY_ID" };

                GridView1.DataBind();
                conn.Close();
                //alert_box.Visible = false;
            }
            else
            {
             //   Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        public void BtnDataCheckPf_Click(object sender, EventArgs e)
        {
            try
            {
                if (IS_EDIT_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);

                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string MakeAsOnDate = TextMonthYear1.Text;
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

                    OracleCommand cmd = new OracleCommand("PRO_PF_DATA_CHECK_INVENTORY", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("TextLastDate", OracleType.VarChar)).Value = LastDate;
                    cmd.Parameters.Add(new OracleParameter("TextLastMonth", OracleType.VarChar)).Value = LastMonth;
                    cmd.Parameters.Add(new OracleParameter("TextCurrentMonth", OracleType.VarChar)).Value = CurrentMonth;
                    cmd.Parameters.Add(new OracleParameter("TextLastDateCurrentMonth", OracleType.VarChar)).Value = LastDateCurrentMonth;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Plastic Factory Chcek Process Successfully " + LastDateCurrentMonth));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                    clearText(); 
                    Display();
                    DisplayRmHistory();

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


        public void DisplayRmHistory()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                DataTable dtUserTypeID = new DataTable();
                DataSet ds = new DataSet();

                string makeSQL = "", c_date = System.DateTime.Now.ToString("MM-yyyy");
                if (txtSearchHistory.Text == "")
                {
                    makeSQL = " select  * from PF_RM_STOCK_INVENTORY_HISTORY ORDER BY CREATE_DATE desc, ITEM_ID asc"; //where TO_CHAR(TO_DATE(CREATE_DATE), 'MM-yyyy') =  '" + c_date + "' 
                }
                else
                {
                    makeSQL = " select  * from PF_RM_STOCK_INVENTORY_HISTORY where TO_CHAR(TO_DATE(CREATE_DATE), 'dd/mm/yyyy') like '" + txtSearchHistory.Text + "%' or ITEM_NAME like '" + txtSearchHistory.Text + "%'  or SUB_ITEM_NAME like '" + txtSearchHistory.Text + "%'  ORDER BY CREATE_DATE desc, ITEM_ID asc";

                    alert_box.Visible = false;
                }
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataKeyNames = new string[] { "IN_RM_HIS_ID" };

                GridView2.DataBind();
                conn.Close();
                //alert_box.Visible = false;
            }
            else
            {
             //   Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        protected void GridViewSearchHistory(object sender, EventArgs e)
        {
            this.DisplayRmHistory();
        }

        protected void GridViewHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView2.PageIndex = e.NewPageIndex;
            DisplayRmHistory();
            alert_box.Visible = false;
        }
 

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int Data_ID = Convert.ToInt32(TextInventoryRmID.Text);
                double InitialStock = Convert.ToDouble(TextInitialStock.Text);
                double ItemAvgRate = Convert.ToDouble(TextItemAvgRate.Text);
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                double StockInWet = 0.00, StockOutWet = 0.00, FinalStockNew = 0.00;

                // inventory RM update select
                string makeSQLInvenRMUp = " select * from PF_RM_STOCK_INVENTORY_MASTER where RM_INVENTORY_ID  = '" + Data_ID + "' ";
                cmdl = new OracleCommand(makeSQLInvenRMUp);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                 
                for (int i = 0; i < RowCount; i++)
                { 
                    StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                    StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString()); 
                }
                 
                FinalStockNew = InitialStock + StockInWet - StockOutWet;

                if (0 < RowCount)
                {
                    // inventory RM update
                    string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set INITIAL_STOCK_WT = :NoInitialStock, FINAL_STOCK_WT = :NoFinalStock, ITEM_AVG_RATE = :NoItemAvgRate, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where RM_INVENTORY_ID = :NoRmInventoryID ";
                    cmdu = new OracleCommand(update_inven_mas, conn);

                    OracleParameter[] objPrmInevenMas = new OracleParameter[6];
                    objPrmInevenMas[0] = cmdu.Parameters.Add("NoInitialStock", InitialStock);
                    objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                    objPrmInevenMas[2] = cmdu.Parameters.Add("NoItemAvgRate", ItemAvgRate);
                    objPrmInevenMas[3] = cmdu.Parameters.Add("u_date", u_date);
                    objPrmInevenMas[4] = cmdu.Parameters.Add("NoCuserID", userID);
                    objPrmInevenMas[5] = cmdu.Parameters.Add("NoRmInventoryID", Data_ID); 

                    cmdu.ExecuteNonQuery();
                    cmdu.Parameters.Clear();
                    cmdu.Dispose();
                }

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Raw Material Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                Display();
            }
                else { 
                    Response.Redirect("~/PagePermissionError.aspx");
                }
        }

        protected void BtnUpdateHis_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int Data_ID = Convert.ToInt32(TextInventoryRmHisID.Text);
                double FinalStock = Convert.ToDouble(TextFinalStockHis.Text.Trim());
                double ItemAvgRate = Convert.ToDouble(TextItemAvgRateHis.Text.Trim());
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt"); 
 
                // inventory RM inventory history update
                string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_HISTORY  set  FINAL_STOCK_WT = :NoFinalStock, ITEM_AVG_RATE = :NoItemAvgRate, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where IN_RM_HIS_ID = :NoRmInventoryHisID ";
                cmdu = new OracleCommand(update_inven_mas, conn);

                OracleParameter[] objPrmInevenMas = new OracleParameter[5]; 
                objPrmInevenMas[0] = cmdu.Parameters.Add("NoFinalStock", FinalStock);
                objPrmInevenMas[1] = cmdu.Parameters.Add("NoItemAvgRate", ItemAvgRate);
                objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                objPrmInevenMas[4] = cmdu.Parameters.Add("NoRmInventoryHisID", Data_ID); 

                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose();
               

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Raw Material Inventory History Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                Display();
            }
                else { 
                    Response.Redirect("~/PagePermissionError.aspx");
                }
        }
         
        public void clearTextField(object sender, EventArgs e)
        {
            TextInventoryRmID.Text = "";
            TextInitialStock.Text = "";
            TextStockIn.Text = "";
            TextItemAvgRate.Text = "";
            TextFinalStockHis.Text = "";
            TextItemAvgRateHis.Text = "";
            DropDownItemID.Text = "0";
            
        }

        public void clearText()
        {
            TextInventoryRmID.Text = "";
            TextInitialStock.Text = "";
            TextStockIn.Text = "";
            TextItemAvgRate.Text = ""; 
            TextFinalStockHis.Text = "";
            TextItemAvgRateHis.Text = "";
            DropDownItemID.Text = "0";

        }

        public DataSet ExecuteBySqlStringType(string sqlString)
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