using System;
using System.Collections;
using System.Configuration;
using System.Data; 
using System.Web.UI; 
using System.Web.UI.WebControls; 
using System.Data.OracleClient;
using System.IO;  
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;

namespace NRCAPPS.PF
{
    public partial class PfExpMaterialPricing : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt, ds, dc;
        int RowCount, RowCount1, RowCount2;
        string IS_PAGE_ACTIVE = "";
        string IS_ADD_ACTIVE = "";
        string IS_EDIT_ACTIVE = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE = "";
        double ItemVatAmt = 0.0, TotalInvoiceAmt = 0.0, MatWeight = 0.0; string EntryDateSlip = "", PartyName = "", FullName ="";
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
                        
                        DataTable dtCurrencyRateID = new DataTable();
                        DataSet drs = new DataSet();
                        string makeRateSQL = " SELECT  NCR.CURRENCY_RATE_ID || '-' || NCR.EXCHANGE_RATE AS CURRENCY_RATE_ID,  NCS.CURRENCY_SYMBOL || ' ' ||   NCS.CURRENCY_NAME || ' - ' || NCT.CURRENCY_SYMBOL || ' ' || NCT.CURRENCY_NAME || ', Rate - ' || NCR.EXCHANGE_RATE AS CURRENCY_NAME FROM NRC_CURRENCY_RATE NCR LEFT JOIN NRC_CURRENCY NCS ON NCS.CURRENCY_ID = NCR.SOURCE_CURRENCY_ID LEFT JOIN NRC_CURRENCY NCT ON NCT.CURRENCY_ID = NCR.TARGET_CURRENCY_ID WHERE NCR.IS_ACTIVE = 'Enable' ORDER BY NCR.CURRENCY_RATE_ID ASC ";
                        drs = ExecuteBySqlString(makeRateSQL);
                        dtCurrencyRateID = (DataTable)drs.Tables[0];
                        DropDownCurrencyRateID.DataSource = dtCurrencyRateID;
                        DropDownCurrencyRateID.DataValueField = "CURRENCY_RATE_ID";
                        DropDownCurrencyRateID.DataTextField = "CURRENCY_NAME";
                        DropDownCurrencyRateID.DataBind();
                        DropDownCurrencyRateID.Items.Insert(0, new ListItem("Select Currency Conversion", "0"));


                        DataTable dtSlipNo = new DataTable();
                        DataSet dss = new DataSet();
                        string makePageSQL = " SELECT PEWS.WB_SLIP_NO || '-' || PEWCI.ITEM_WEIGHT AS WB_SLIP_NO, PEWS.WB_SLIP_NO || ' - ' || PC.PARTY_ID || ' - ' || PC.PARTY_NAME || ', Container No. - ' || PEWS.CONTAINER_NO || ', Item - ' || PI.ITEM_NAME || ', Sub Item - ' || PSI.SUB_ITEM_NAME || ', Item WT(WB) -' || TO_CHAR(PEWS.ITEM_WEIGHT_WB, '999,999,999') || ', Item WT -' || TO_CHAR((PEWCI.ITEM_WEIGHT/1000), '999999.999') AS PARTY_NAME FROM PF_EXPORT_WBSLIP_CON PEWS LEFT JOIN PF_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWS.WB_SLIP_NO LEFT JOIN PF_PARTY PC ON PC.PARTY_ID = PEWS.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWCI.SUB_ITEM_ID WHERE PEWS.EXPORT_INVOICE_NO IS NULL AND PEWS.IS_ACTIVE_PRICING IS NULL ORDER BY PEWS.WB_SLIP_NO ASC";
                        dss = ExecuteBySqlString(makePageSQL);
                        dtSlipNo = (DataTable)dss.Tables[0];
                        DropDownSlipNoEx.DataSource = dtSlipNo;
                        DropDownSlipNoEx.DataValueField = "WB_SLIP_NO";
                        DropDownSlipNoEx.DataTextField = "PARTY_NAME";
                        DropDownSlipNoEx.DataBind();
                        DropDownSlipNoEx.Items.Insert(0, new ListItem("Select Weight / Container", "0"));                         
                        //   TextExportInvoiceNo.Enabled = false;
                        Display(); 
                         
                        TextTotalQty.Attributes.Add("readonly", "readonly");
                        TextTotalAmountEx.Attributes.Add("readonly", "readonly");
                        TextItemCurrencyAmount.Attributes.Add("readonly", "readonly"); 

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
         
        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
        //  try
       //    {
            if (IS_EDIT_ACTIVE == "Enable")
            { 
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]); 
                double PricePerMt = Convert.ToDouble(TextPricePerMt.Text);
                double TotalAmountEx = Convert.ToDouble(TextTotalAmountEx.Text);
                double ItemCurrencyAmount = Convert.ToDouble(TextItemCurrencyAmount.Text);
                string[] CurrencyRateIDTemp = DropDownCurrencyRateID.Text.Split('-');
                int CurrencyRateID = Convert.ToInt32(CurrencyRateIDTemp[0]);
                double CurrencyRate = Convert.ToDouble(CurrencyRateIDTemp[1]);
                    string[] SlipNo = DropDownSlipNoEx.Text.ToString().Split('-');

                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                      
                string update_ex_sales = " update  PF_EXPORT_WBSLIP_CON set  MAT_PRICE_PER_MT =:NoPricePerMt, MATERIAL_AMOUNT =:NoTotalAmountEx, CURRENCY_RATE_ID =:NoCurrencyRateID, CURRENCY_RATE =:NoCurrencyRate, MATERIAL_CONVERSION_AMOUNT =:TextItemCurrencyAmount,  UPDATE_DATE_PRICING = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID_PRICING = :NoCuserID, IS_ACTIVE_PRICING = :TextIsActive WHERE WB_SLIP_NO =:TextSlipNoEx ";
                cmdu = new OracleCommand(update_ex_sales, conn); 
                OracleParameter[] objPr = new OracleParameter[9];  
                objPr[0] = cmdu.Parameters.Add("TextSlipNoEx", SlipNo[0]);                   
                objPr[1] = cmdu.Parameters.Add("NoPricePerMt", PricePerMt);
                objPr[2] = cmdu.Parameters.Add("NoTotalAmountEx", TotalAmountEx); 
                objPr[3] = cmdu.Parameters.Add("NoCurrencyRateID", CurrencyRateID);
                objPr[4] = cmdu.Parameters.Add("NoCurrencyRate", CurrencyRate);
                objPr[5] = cmdu.Parameters.Add("TextItemCurrencyAmount", ItemCurrencyAmount); 
                objPr[6] = cmdu.Parameters.Add("u_date", u_date);
                objPr[7] = cmdu.Parameters.Add("NoCuserID", userID);
                objPr[8] = cmdu.Parameters.Add("TextIsActive", ISActive); 

                cmdu.ExecuteNonQuery(); 
                cmdu.Parameters.Clear();
                cmdu.Dispose(); 
                conn.Close(); 

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Export Sales Invoice Data Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                 

                Display(); 

            }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
         //     }
         //    catch
          //    {
            //    Response.Redirect("~/ParameterError.aspx");
           //  }  
        }

         
        protected void linkSelectClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             
             LinkButton btn = (LinkButton)sender;
             Session["user_data_id"] = btn.CommandArgument;
             string[] USER_DATA_ID = Session["user_data_id"].ToString().Split('-');

            DataTable dtSlipNo = new DataTable();
            DataSet dss = new DataSet();
            string makePageSQL = " SELECT PEWS.WB_SLIP_NO || '-' || PEWCI.ITEM_WEIGHT AS WB_SLIP_NO, PEWS.WB_SLIP_NO || ' - ' || PC.PARTY_ID || ' - ' || PC.PARTY_NAME || ', Container No. - ' || PEWS.CONTAINER_NO || ', Item - ' || PI.ITEM_NAME || ', Sub Item - ' || PSI.SUB_ITEM_NAME || ', Item WT(WB) -' || TO_CHAR(PEWS.ITEM_WEIGHT_WB, '999,999,999') || ', Item WT -' || TO_CHAR((PEWCI.ITEM_WEIGHT/1000), '999999.999') AS PARTY_NAME FROM PF_EXPORT_WBSLIP_CON PEWS LEFT JOIN PF_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWS.WB_SLIP_NO LEFT JOIN PF_PARTY PC ON PC.PARTY_ID = PEWS.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWCI.SUB_ITEM_ID WHERE PEWS.EXPORT_INVOICE_NO IS NULL AND PEWS.WB_SLIP_NO = '" + USER_DATA_ID[0] + "' ORDER BY PEWS.WB_SLIP_NO ASC";
            dss = ExecuteBySqlString(makePageSQL);
            dtSlipNo = (DataTable)dss.Tables[0];
            DropDownSlipNoEx.DataSource = dtSlipNo;
            DropDownSlipNoEx.DataValueField = "WB_SLIP_NO";
            DropDownSlipNoEx.DataTextField = "PARTY_NAME";
            DropDownSlipNoEx.DataBind();
            DropDownSlipNoEx.Items.Insert(0, new ListItem("Select Weight / Container", "0"));

            string makeSQL = "  SELECT PEWC.WB_SLIP_NO, PEWC.MAT_PRICE_PER_MT, PEWC.MATERIAL_AMOUNT, PEWCI.ITEM_WEIGHT, NCR.CURRENCY_RATE_ID || '-' || NCR.EXCHANGE_RATE AS CURRENCY_RATE_ID, PEWC.CURRENCY_RATE, PEWC.MATERIAL_CONVERSION_AMOUNT, PEWC.IS_ACTIVE_PRICING, PEWC.UPDATE_DATE_PRICING, PEWC.U_USER_ID_PRICING FROM PF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN  NRC_CURRENCY_RATE NCR ON NCR.CURRENCY_RATE_ID  = PEWC.CURRENCY_RATE_ID WHERE PEWC.WB_SLIP_NO = '" + USER_DATA_ID[0] + "'";

             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn);
             ds = new DataTable();
             oradata.Fill(ds);
             RowCount = ds.Rows.Count;

             for (int i = 0; i < RowCount; i++)
             {
                 DropDownSlipNoEx.Text        = ds.Rows[i]["WB_SLIP_NO"].ToString() + "-"+ ds.Rows[i]["ITEM_WEIGHT"].ToString();
                 TextTotalQty.Text            = Convert.ToString(decimal.Parse(ds.Rows[i]["ITEM_WEIGHT"].ToString())/1000);
                 TextPricePerMt.Text          = decimal.Parse(ds.Rows[i]["MAT_PRICE_PER_MT"].ToString()).ToString(".00");
                 TextTotalAmountEx.Text       = decimal.Parse(ds.Rows[i]["MATERIAL_AMOUNT"].ToString()).ToString(".00"); 
                 DropDownCurrencyRateID.Text  = ds.Rows[i]["CURRENCY_RATE_ID"].ToString(); 
                 TextItemCurrencyAmount.Text  = decimal.Parse(ds.Rows[i]["MATERIAL_CONVERSION_AMOUNT"].ToString()).ToString(".00"); 
                 CheckIsActive.Checked        = Convert.ToBoolean(ds.Rows[i]["IS_ACTIVE_PRICING"].ToString() == "Enable" ? true : false); 
             }
              
             Display(); 
             conn.Close(); 
             alert_box.Visible = false;

             DropDownSlipNoEx.Enabled = false; 
             BtnUpdate.Attributes.Add("aria-disabled", "true");
             BtnUpdate.Attributes.Add("class", "btn btn-success active"); 
             
        }

        public void Display()
        {
            
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open(); 

            string makeSQL = "";
            if (txtSearchUser.Text == "")
            {
                makeSQL = " SELECT PEWC.WB_SLIP_NO, PEWC.MAT_PRICE_PER_MT, PEWC.MATERIAL_AMOUNT,   NCS.CURRENCY_NAME AS SOURCE_CURRENCY_NAME, NCT.CURRENCY_NAME AS TARGET_CURRENCY_NAME, NCR.EXCHANGE_RATE, PEWC.MATERIAL_CONVERSION_AMOUNT,  PEWC.UPDATE_DATE_PRICING, PEWC.IS_ACTIVE_PRICING FROM PF_EXPORT_WBSLIP_CON PEWC LEFT JOIN NRC_CURRENCY_RATE NCR ON NCR.CURRENCY_RATE_ID = PEWC.CURRENCY_RATE_ID LEFT JOIN NRC_CURRENCY NCS ON NCS.CURRENCY_ID = NCR.SOURCE_CURRENCY_ID LEFT JOIN NRC_CURRENCY NCT ON NCT.CURRENCY_ID = NCR.TARGET_CURRENCY_ID WHERE PEWC.IS_ACTIVE_PRICING IS NOT NULL ORDER BY PEWC.WB_SLIP_NO DESC"; 
            }
            else
            {
                makeSQL = " SELECT PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO, HE.EMP_FNAME || ' ' || HE.EMP_LNAME AS EMP_NAME, PPC.CLAIM_FOR_MONTH, PPC.CLAIM_DATE, NPT.PAYMENT_TYPE_NAME, PPC.TOTAL_AMOUNT, PPC.CREATE_DATE, PPC.UPDATE_DATE, PPC.IS_ACTIVE, IS_OBJ_QUERY, PPC.OBJ_QUERY_DES, PPC.OBJ_QUERY_C_DATE, PPM.SLIP_NO, PC.PARTY_ID || ' - ' || PC.PARTY_NAME AS PARTY_NAME, PPC.IS_CHECK, PPC.IS_PRINT, TO_CHAR(PPC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE FROM PF_EXPORT_SALES_MASTER PPC LEFT JOIN PF_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PPC.EMP_ID LEFT JOIN NRC_PAYMENT_TYPE NPT ON NPT.PAYMENT_TYPE_ID = PPC.PAYMENT_TYPE_ID LEFT JOIN PF_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID WHERE PPC.CLAIM_NO like '" + txtSearchUser.Text + "%' or  HE.EMP_FNAME like '" + txtSearchUser.Text + "%'  ORDER BY PPC.CLAIM_NO DESC, PPM.SLIP_NO ASC ";
                alert_box.Visible = false;
            }

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            GridView1.DataSource = dt;
            GridView1.DataKeyNames = new string[] { "WB_SLIP_NO" }; 
            GridView1.DataBind();

            if (dt.Rows.Count > 0)
            {
               GroupGridView(GridView1.Rows, 0, 17);
            }
            else {
                
            }
         
            conn.Close(); 
        }

     

        protected void gridViewFileInformation_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow Row in GridView1.Rows)
            {
                string isCheck = (Row.FindControl("IsActiveCheckLink") as Label).Text; 
                if (isCheck == "Complete")
                {
                    (Row.FindControl("linkSelect") as LinkButton).Visible = false;
                }
            }
        }


        void GroupGridView(GridViewRowCollection gvrc, int startIndex, int total)
        {
            if (total == 0) return;
            int i, count = 1;
            ArrayList lst = new ArrayList(); 
            lst.Add(gvrc[0]);
            var ctrl = gvrc[0].Cells[startIndex];
            for (i = 1; i < gvrc.Count; i++)
            {
                TableCell nextCell = gvrc[i].Cells[startIndex];
                if (ctrl.Text == nextCell.Text)
                {
                    count++;
                    nextCell.Visible = false;
                    lst.Add(gvrc[i]);
                }
                else
                {
                    if (count > 1)
                    {
                        ctrl.RowSpan = count;
                        GroupGridView(new GridViewRowCollection(lst), startIndex + 1, total - 1);
                    }
                    count = 1;
                    lst.Clear();
                    ctrl = gvrc[i].Cells[startIndex];
                    lst.Add(gvrc[i]);
                }
            }
            if (count > 1)
            {
                ctrl.RowSpan = count;
                GroupGridView(new GridViewRowCollection(lst), startIndex + 1, total - 1);
            }
            count = 1;
            lst.Clear();
        }

         
        protected void GridViewSearchUser(object sender, EventArgs e)
        {
            this.Display();
        }
 
        protected void GridViewPage_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            
            try
            {
                GridView1.PageIndex = e.NewPageIndex;
                Display();
            }
            catch
            {
            }
          
            alert_box.Visible = false;
        }
         
        public void clearTextField(object sender, EventArgs e)
        { 
            DropDownCurrencyRateID.Text = "0"; 
            TextPricePerMt.Text = "";
            TextTotalQty.Text = "";
            TextTotalAmountEx.Text = ""; 
            TextItemCurrencyAmount.Text = ""; 
            DropDownSlipNoEx.Text = "0"; 
        }

        public void clearText()
        {  
            DropDownCurrencyRateID.Text = "0";  
            TextPricePerMt.Text = "";
            TextTotalQty.Text = "";
            TextTotalAmountEx.Text = ""; 
            TextItemCurrencyAmount.Text = ""; 
            DropDownSlipNoEx.Text = "0"; 
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