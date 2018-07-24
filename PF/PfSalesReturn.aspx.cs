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
using System.Text.RegularExpressions;
using System.Globalization;


namespace NRCAPPS.PF
{
    public partial class PfSalesReturn : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt, ds;
        int RowCount;

        string IS_PAGE_ACTIVE = "";
        string IS_ADD_ACTIVE = "";
        string IS_EDIT_ACTIVE = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE = "";
        string IS_REPORT_ACTIVE = "";

        public bool IsLoad { get; set; } public bool IsLoad1 { get; set; } public bool IsLoad2 { get; set; } public bool IsLoad3 { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_NAME"] != null)
            {
                string requestedFile = Path.GetFileName(Request.Path);
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = " SELECT  NUPP.IS_PAGE_ACTIVE, NUPP.IS_ADD_ACTIVE, NUPP.IS_EDIT_ACTIVE, NUPP.IS_DELETE_ACTIVE, NUPP.IS_VIEW_ACTIVE, NUPP.IS_REPORT_ACTIVE FROM NRC_USER_PAGE_PERMISSION NUPP LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NUPP.USER_PAGE_ID  WHERE NUPP.USER_ID = '" + Session["USER_ID"] + "' AND NUP.IS_ACTIVE = 'Enable' AND NUP.PAGE_URL = '" + requestedFile + "' ";
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
                }

                if (IS_PAGE_ACTIVE == "Enable")
                {
                     if (!IsPostBack)
                    {
                         
                        Display();
                        DisplaySalesRtn();

                        txtSearchSales.Text = "";

                        alert_box.Visible = false;

                    } IsLoad = false;
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


        public void BtnUpdateSalesCheck_Click(object sender, EventArgs e)
        {
            try
            {
                if (IS_EDIT_ACTIVE == "Enable")
                {

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    int userID = Convert.ToInt32(Session["USER_ID"]);

                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    foreach (GridViewRow gridRow in GridView1.Rows)
                    {
                        CheckBox chkRowIs = (gridRow.Cells[0].FindControl("IschkRowSalesRtn") as CheckBox);
                        string IsSalesRtnCheck = chkRowIs.Checked ? "Yes" : null;
                        if (chkRowIs.Checked)
                        {
                            // update data 
                            string update_user = "update  PF_SALES_MASTER set IS_SALES_RETURN = :NoIsSalesRtnCheck, SALES_RTN_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), SALES_RTN_USER_ID = :NoC_USER_ID  where INVOICE_NO = :NoInvoiceID ";
                            cmdi = new OracleCommand(update_user, conn);
                            OracleParameter[] objPrm = new OracleParameter[5];
                            objPrm[0] = cmdi.Parameters.Add("NoInvoiceID", Convert.ToInt32(gridRow.Cells[1].Text));
                            objPrm[1] = cmdi.Parameters.Add("NoIsSalesRtnCheck", IsSalesRtnCheck);
                            objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                            objPrm[3] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                            cmdi.ExecuteNonQuery();

                            int ItemIdOld = 0; double ItemWeightOld = 0.00;
                            string makeSQLPro = " select ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT from PF_SALES_MASTER where INVOICE_NO  = '" + Convert.ToInt32(gridRow.Cells[1].Text) + "'  ";
                            cmdl = new OracleCommand(makeSQLPro);
                            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                            dt = new DataTable();
                            oradata.Fill(dt);
                            RowCount = dt.Rows.Count;

                            for (int i = 0; i < RowCount; i++)
                            {
                                ItemIdOld = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString()); 
                                ItemWeightOld = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());
                            }

                            //inventory calculation

                            int InvenItemID = 0; 
                            double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, FinalStock = 0.00, StockInWetNew = 0.00, FinalStockNew = 0.00;

                            // check inventory FG
                            string makeSQL = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "' ";
                            cmdl = new OracleCommand(makeSQL);
                            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                            dt = new DataTable();
                            oradata.Fill(dt);
                            RowCount = dt.Rows.Count;
                             
                            for (int i = 0; i < RowCount; i++)
                            {
                                InvenItemID = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString()); 
                                InitialStock = Convert.ToDouble(dt.Rows[i]["INITIAL_STOCK_WT"].ToString());
                                StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                                StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString());
                                FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                            }

                            StockInWetNew = StockInWet + ItemWeightOld;
                            FinalStockNew = InitialStock + StockInWetNew - StockOutWet;

                            
                                // update inventory FG (minus old data)
                                string update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                                cmdu = new OracleCommand(update_inven_mas, conn);

                                OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                                objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                                objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                                objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                                objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                                objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID); 

                                cmdu.ExecuteNonQuery();
                                cmdu.Parameters.Clear();
                                cmdu.Dispose();
                            
                        }

                    }
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();

                    Display();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Sales Return Data Update successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
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
                if (txtSearchEmp.Text == "")
                {
                    makeSQL = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PPT.PUR_TYPE_NAME, PC.CUSTOMER_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_SALES_RETURN FROM PF_SALES_MASTER PSM LEFT JOIN PF_PURCHASE_TYPE PPT ON  PPT.PUR_TYPE_ID = PSM.PUR_TYPE_ID LEFT JOIN PF_CUSTOMER PC ON PC.CUSTOMER_ID = PSM.CUSTOMER_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID WHERE PSM.IS_SALES_RETURN IS NULL ";
                }
                else
                {
                    makeSQL = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PPT.PUR_TYPE_NAME, PC.CUSTOMER_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_SALES_RETURN FROM PF_SALES_MASTER PSM LEFT JOIN PF_PURCHASE_TYPE PPT ON  PPT.PUR_TYPE_ID = PSM.PUR_TYPE_ID LEFT JOIN PF_CUSTOMER PC ON PC.CUSTOMER_ID = PSM.CUSTOMER_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID WHERE PSM.IS_SALES_RETURN IS NULL AND PSM.INVOICE_NO like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%' or PC.CUSTOMER_NAME like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PSI.SUB_ITEM_NAME like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'mm/yyyy') like '" + txtSearchEmp.Text + "%' or PSM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ORDER BY PSM.CREATE_DATE desc, PSM.UPDATE_DATE desc";
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "INVOICE_NO" };
                GridView1.DataBind();
                conn.Close();
                // alert_box.Visible = false;
            }
        }

        protected void GridViewSearchEmp(object sender, EventArgs e)
        {
            this.Display();
        }

        protected void GridViewEmp_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            Display();
            alert_box.Visible = false;
        }


        public void DisplaySalesRtn()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                 
                string makeSQL1 = "";
                if (txtSearchSales.Text == "")
                {
                    makeSQL1 = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PPT.PUR_TYPE_NAME, PC.CUSTOMER_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_SALES_RETURN, PSM.SALES_RTN_DATE FROM PF_SALES_MASTER PSM LEFT JOIN PF_PURCHASE_TYPE PPT ON  PPT.PUR_TYPE_ID = PSM.PUR_TYPE_ID LEFT JOIN PF_CUSTOMER PC ON PC.CUSTOMER_ID = PSM.CUSTOMER_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID WHERE PSM.IS_SALES_RETURN = 'Yes' ";
                }
                else
                {
                    makeSQL1 = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PPT.PUR_TYPE_NAME, PC.CUSTOMER_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_SALES_RETURN, PSM.SALES_RTN_DATE FROM PF_SALES_MASTER PSM LEFT JOIN PF_PURCHASE_TYPE PPT ON  PPT.PUR_TYPE_ID = PSM.PUR_TYPE_ID LEFT JOIN PF_CUSTOMER PC ON PC.CUSTOMER_ID = PSM.CUSTOMER_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID WHERE PSM.IS_SALES_RETURN = 'Yes' AND (PSM.INVOICE_NO like '" + txtSearchSales.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchSales.Text + "%' or PC.CUSTOMER_NAME like '" + txtSearchSales.Text + "%' or PI.ITEM_NAME like '" + txtSearchSales.Text + "%' or PSI.SUB_ITEM_NAME like '" + txtSearchSales.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'dd/mm/yyyy') like '" + txtSearchSales.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'mm/yyyy') like '" + txtSearchSales.Text + "%' or PSM.IS_ACTIVE like '" + txtSearchSales.Text + "%') ORDER BY PSM.CREATE_DATE desc, PSM.UPDATE_DATE desc";
                }

                cmdi = new OracleCommand(makeSQL1);
                oradata = new OracleDataAdapter(cmdi.CommandText, conn);
                ds = new DataTable();
                oradata.Fill(ds);
                GridView2.DataSource = ds;
                GridView2.DataKeyNames = new string[] { "INVOICE_NO" };
                GridView2.DataBind();
                conn.Close();
                // alert_box.Visible = false;
            }
        }

        protected void GridViewSearchSalesRtn(object sender, EventArgs e)
        {
            this.DisplaySalesRtn();
        }

        protected void GridViewSalesRtn_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView2.PageIndex = e.NewPageIndex;
            DisplaySalesRtn();
            alert_box.Visible = false;
        }

         
      

         
         
    }
          
}