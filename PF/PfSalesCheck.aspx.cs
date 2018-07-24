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
    public partial class PfSalesCheck : System.Web.UI.Page
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
                        QueryCmo.Visible = false;

                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");

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
          
            
        protected void linkSelectClick(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
            string USER_DATA_ID = Session["user_data_id"].ToString();


            DataTable dtUserTypeID = new DataTable();
            DataSet ds = new DataSet();
            string makeSQL = " select  INVOICE_NO, OBJ_QUERY_DES, IS_CHECK, IS_OBJ_QUERY from PF_SALES_MASTER where INVOICE_NO  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                TextInvoiceNo.Text          = dt.Rows[i]["INVOICE_NO"].ToString();
                TextInvoiceNoDisable.Text   = dt.Rows[i]["INVOICE_NO"].ToString();
                TextQueryDescription.Text   = dt.Rows[i]["OBJ_QUERY_DES"].ToString(); 
                CheckIsCmo.Checked          = Convert.ToBoolean(dt.Rows[i]["IS_CHECK"].ToString() == "Complete" ? true : false);
                CheckIsQuery.Checked        = Convert.ToBoolean(dt.Rows[i]["IS_OBJ_QUERY"].ToString() == "Yes" ? true : false); 
            }
            if (CheckIsQuery.Checked)
            {
                QueryCmo.Visible = true;
            }
            else { QueryCmo.Visible = false; }

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

                DataTable dtEmpTypeID = new DataTable();
                DataSet ds = new DataSet();

                string makeSQL = "";
                if (txtSearchEmp.Text == "")
                {
                    makeSQL = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PPT.PUR_TYPE_NAME, PC.CUSTOMER_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_CHECK, PSM.IS_OBJ_QUERY, PSM.OBJ_QUERY_DES, PSM.OBJ_QUERY_C_DATE, PSM.CHECK_DATE FROM PF_SALES_MASTER PSM LEFT JOIN PF_PURCHASE_TYPE PPT ON  PPT.PUR_TYPE_ID = PSM.PUR_TYPE_ID LEFT JOIN PF_CUSTOMER PC ON PC.CUSTOMER_ID = PSM.CUSTOMER_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID WHERE PSM.IS_CHECK IS NULL ";
                }
                else
                {
                    makeSQL = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PPT.PUR_TYPE_NAME, PC.CUSTOMER_NAME, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE, PSM.IS_CHECK FROM PF_SALES_MASTER PSM LEFT JOIN PF_PURCHASE_TYPE PPT ON  PPT.PUR_TYPE_ID = PSM.PUR_TYPE_ID LEFT JOIN PF_CUSTOMER PC ON PC.CUSTOMER_ID = PSM.CUSTOMER_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PSM.SUB_ITEM_ID WHERE PSM.INVOICE_NO like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%' or PC.CUSTOMER_NAME like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PSI.SUB_ITEM_NAME like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'mm/yyyy') like '" + txtSearchEmp.Text + "%' or PSM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ORDER BY PSM.CREATE_DATE desc, PSM.UPDATE_DATE desc";
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


        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
        try {
                
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);  
                int InvoiceNo = Convert.ToInt32(TextInvoiceNo.Text);
                 
                string IsQuery = CheckIsQuery.Checked ? "Yes" : "No";
                string IsCmoCheck = CheckIsCmo.Checked ? "Complete" : ""; 
                
             
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                 
                // update sales master 
                string update_sales_mas = " update  PF_SALES_MASTER set IS_OBJ_QUERY =:TextIsQuery, OBJ_QUERY_DES =:TextQueryDescription,  OBJ_QUERY_C_DATE = TO_DATE(:DateObjQuery, 'DD-MM-YYYY HH:MI:SS AM'), IS_CHECK =:TextIsCmoCheck , CHECK_USER_ID = :NoC_USER_ID where INVOICE_NO =: NoInvoiceNo ";
                cmdu = new OracleCommand(update_sales_mas, conn);

                OracleParameter[] objPr = new OracleParameter[6];
                objPr[0] = cmdu.Parameters.Add("TextIsQuery", IsQuery);
                objPr[1] = cmdu.Parameters.Add("TextQueryDescription", TextQueryDescription.Text);
                objPr[2] = cmdu.Parameters.Add("DateObjQuery", u_date);
                objPr[3] = cmdu.Parameters.Add("TextIsCmoCheck", IsCmoCheck);
                objPr[4] = cmdu.Parameters.Add("NoC_USER_ID", userID);
                objPr[5] = cmdu.Parameters.Add("NoInvoiceNo", InvoiceNo);


                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose();

                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Sales Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearText();
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
         

        public void BtnUpdateSalesCheck_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            int userID = Convert.ToInt32(Session["USER_ID"]); 

            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
             
            foreach (GridViewRow gridRow in GridView1.Rows)
            { 
                CheckBox chkRowIs = (gridRow.Cells[0].FindControl("IschkRowSales") as CheckBox);
                string IsSalesCheck = chkRowIs.Checked ? "Complete" : "";
                if (chkRowIs.Checked)
                {
                    // update data 
                    string update_user = "update  PF_SALES_MASTER set IS_CHECK = :NoIsSalesCheck, CHECK_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), CHECK_USER_ID = :NoC_USER_ID  where INVOICE_NO = :NoInvoiceID ";
                    cmdi = new OracleCommand(update_user, conn);
                    OracleParameter[] objPrm = new OracleParameter[5];
                    objPrm[0] = cmdi.Parameters.Add("NoInvoiceID", Convert.ToInt32(gridRow.Cells[1].Text));
                    objPrm[1] = cmdi.Parameters.Add("NoIsSalesCheck", IsSalesCheck);
                    objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[3] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                    cmdi.ExecuteNonQuery();
                }    

            }
            cmdi.Parameters.Clear();
            cmdi.Dispose();
            conn.Close();

            Display();

            alert_box.Visible = true;
            alert_box.Controls.Add(new LiteralControl("Sales Data Update successfully"));
            alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
            clearText();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        public void clearTextField(object sender, EventArgs e)
        {
            TextInvoiceNo.Text = "";  
            

        }

        public void clearText()
        {
            TextInvoiceNo.Text = "";  
            
        }


        public void Cmo_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckIsCmo.Checked == false)
            {
                BtnUpdate.Attributes.Add("aria-disabled", "false");
                BtnUpdate.Attributes.Add("class", "btn btn-primary disabled");
            }
            else if (CheckIsCmo.Checked == true)
            {
                BtnUpdate.Attributes.Add("aria-disabled", "true");
                BtnUpdate.Attributes.Add("class", "btn btn-success active");
            }


        }

        public void Query_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckIsQuery.Checked == false)
            {
                QueryCmo.Visible = false;

                BtnUpdate.Attributes.Add("aria-disabled", "false");
                BtnUpdate.Attributes.Add("class", "btn btn-success disabled");

                if (CheckIsCmo.Checked)
                {
                    BtnUpdate.Attributes.Add("aria-disabled", "true");
                    BtnUpdate.Attributes.Add("class", "btn btn-success active");
                }
            }
            else if (CheckIsQuery.Checked == true)
            {
                QueryCmo.Visible = true;
                BtnUpdate.Attributes.Add("aria-disabled", "true");
                BtnUpdate.Attributes.Add("class", "btn btn-success active");
            }


        }
       
         
    }
          
}