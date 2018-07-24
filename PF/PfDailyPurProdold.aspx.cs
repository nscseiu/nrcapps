using System;
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


namespace NRCAPPS.PF
{
    public partial class PfDailyPurProdold : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmd;
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
                        string makePurchaseTypeSQL = " SELECT * FROM PF_PURCHASE_TYPE WHERE IS_ACTIVE = 'Enable' ORDER BY PUR_TYPE_ID ASC";
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

                string Today_Date = "";

                string makeSQL = "";
                if (AsOnDate.Text == "")
                {
                    Today_Date = System.DateTime.Now.ToString("dd/MM/yyyy");
                    makeSQL = "  SELECT  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME_FULL, PI.ITEM_CODE, sum(PPM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PPM.ITEM_AMOUNT) AS ITEM_AMOUNT, ROUND(sum(PPM.ITEM_AMOUNT)/ sum(PPM.ITEM_WEIGHT),2) AS ITEM_AVG_RATE FROM  PF_PURCHASE_MASTER PPM  LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPM.SUB_ITEM_ID LEFT JOIN PF_PURCHASE_TYPE PPT ON PPT.PUR_TYPE_ID = PPM.PUR_TYPE_ID WHERE TO_CHAR(TO_DATE(PPM.ENTRY_DATE),'dd/mm/yyyy') = '" + Today_Date + "' AND PPT.PUR_TYPE_NAME = 'D' GROUP BY  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME, PI.ITEM_CODE ORDER BY PI.ITEM_ID ";
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

                conn.Close();
                //alert_box.Visible = false;
            }
            else
            {
                //   Response.Redirect("~/PagePermissionError.aspx");
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

                string Today_Date = "";

                string makeSQL = "";
                if (EntryDate.Text == "")
                {
                    Today_Date = System.DateTime.Now.ToString("dd/MM/yyyy");
                    makeSQL = " SELECT PPRS.SHIFT_ID, PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME_FULL, PI.ITEM_CODE, SUM(PPRM.ITEM_WEIGHT_IN_FG) AS ITEM_WEIGHT_IN_FG FROM PF_PRODUCTION_MASTER PPRM  LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_PRODUCTION_MACHINE PPRMA ON PPRMA.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_PRODUCTION_SHIFT PPRS ON PPRS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID WHERE TO_CHAR(TO_DATE(PPRM.ENTRY_DATE),'dd/mm/yyyy') = '" + Today_Date  + "' GROUP BY PPRS.SHIFT_ID, PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER,  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME, PI.ITEM_CODE ORDER BY PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER,  PI.ITEM_ID ASC ";
                }
                else
                {
                    makeSQL = " SELECT PPRS.SHIFT_ID, PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME_FULL, PI.ITEM_CODE, SUM(PPRM.ITEM_WEIGHT_IN_FG) AS ITEM_WEIGHT_IN_FG FROM PF_PRODUCTION_MASTER PPRM  LEFT JOIN PF_SUPERVISOR PS ON PS.SUPERVISOR_ID = PPRM.SUPERVISOR_ID LEFT JOIN PF_PRODUCTION_MACHINE PPRMA ON PPRMA.MACHINE_ID = PPRM.MACHINE_ID LEFT JOIN PF_PRODUCTION_SHIFT PPRS ON PPRS.SHIFT_ID = PPRM.SHIFT_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PPRM.SUB_ITEM_ID WHERE TO_CHAR(TO_DATE(PPRM.ENTRY_DATE),'dd/mm/yyyy') = '" + EntryDate.Text + "' GROUP BY PPRS.SHIFT_ID, PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER,  PI.ITEM_ID, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME, PI.ITEM_CODE ORDER BY PPRS.SHIFT_NAME, PPRMA.MACHINE_NUMBER,  PI.ITEM_ID ASC ";
    
                }
                 
                cmd = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmd.CommandText, conn);
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