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
using System.Web.Services;

namespace NRCAPPS.WP
{
    public partial class WpExpMaterialPricing : System.Web.UI.Page
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
                        string makePageSQL = "   SELECT PEWC.WB_SLIP_NO,  PEWC.CONTAINER_NO || ' - ' || PEWC.WB_SLIP_NO  || '' || ', Item WT(WB) -' || TO_CHAR(PEWC.ITEM_WEIGHT_WB, '999,999,999') AS PARTY_NAME FROM WP_EXPORT_WBSLIP_CON PEWC WHERE PEWC.EXPORT_INVOICE_NO IS NULL AND PEWC.IS_ACTIVE_PRICING IS NULL ORDER BY PEWC.CONTAINER_NO, PEWC.WB_SLIP_NO ASC";
                        dss = ExecuteBySqlString(makePageSQL);
                        dtSlipNo = (DataTable)dss.Tables[0];
                        DropDownWpSlipNoEx.DataSource = dtSlipNo;
                        DropDownWpSlipNoEx.DataValueField = "WB_SLIP_NO";
                        DropDownWpSlipNoEx.DataTextField = "PARTY_NAME";
                        DropDownWpSlipNoEx.DataBind();
                        DropDownWpSlipNoEx.Items.Insert(0, new ListItem("Select Weight / Container", "0"));                         
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
                int SalesItemID = Convert.ToInt32(Request.Form[DropDownSalesItemID.UniqueID]);
                double CurrencyRate = Convert.ToDouble(CurrencyRateIDTemp[1]);
                string[] SlipNo = Request.Form[DropDownItemID.UniqueID].ToString().Split('-');

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                      
                string update_ex_sales = " update  WP_EXPORT_WBSLIP_CON_ITEM set  MAT_PRICE_PER_MT =:NoPricePerMt, MATERIAL_AMOUNT =:NoTotalAmountEx, CURRENCY_RATE_ID =:NoCurrencyRateID, CURRENCY_RATE =:NoCurrencyRate, MATERIAL_CONVERSION_AMOUNT =:TextItemCurrencyAmount, ITEM_SALES_ID =:TextSalesItemID, IS_ACTIVE_PRICING =:TextActivePricing, UPDATE_DATE_PRICING = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID_PRICING = :NoCuserID WHERE EXP_WBCON_ITEM_ID =:TextSlipNoEx ";
                cmdu = new OracleCommand(update_ex_sales, conn); 
                OracleParameter[] objPr = new OracleParameter[10];  
                objPr[0] = cmdu.Parameters.Add("TextSlipNoEx", SlipNo[0]);                   
                objPr[1] = cmdu.Parameters.Add("NoPricePerMt", PricePerMt);
                objPr[2] = cmdu.Parameters.Add("NoTotalAmountEx", TotalAmountEx); 
                objPr[3] = cmdu.Parameters.Add("NoCurrencyRateID", CurrencyRateID);
                objPr[4] = cmdu.Parameters.Add("NoCurrencyRate", CurrencyRate);
                objPr[5] = cmdu.Parameters.Add("TextItemCurrencyAmount", ItemCurrencyAmount); 
                objPr[6] = cmdu.Parameters.Add("TextSalesItemID", SalesItemID);   
                objPr[7] = cmdu.Parameters.Add("u_date", u_date);
                objPr[8] = cmdu.Parameters.Add("NoCuserID", userID);
                objPr[9] = cmdu.Parameters.Add("TextActivePricing", ISActive); 

                cmdu.ExecuteNonQuery(); 
                cmdu.Parameters.Clear();
                cmdu.Dispose();


            //    OracleCommand cmd = new OracleCommand();
           //     cmd.Connection = conn;
            //    cmd.CommandText = "select WB_SLIP_NO from WP_EXPORT_WBSLIP_CON_ITEM WHERE IS_ACTIVE_PRICING IS NULL AND WB_SLIP_NO = '" + SlipNo[0] + "'";
             //   cmd.CommandType = CommandType.Text;

            //    OracleDataReader dr = cmd.ExecuteReader();
            //    if (dr.HasRows)
             //   {
                    string update_ex_price = " update  WP_EXPORT_WBSLIP_CON set  IS_ACTIVE_PRICING =:TextActivePricing WHERE WB_SLIP_NO = '" + DropDownWpSlipNoEx.Text + "' ";
                    cmdu = new OracleCommand(update_ex_price, conn);
                    OracleParameter[] objPrp = new OracleParameter[1]; 
                    objPrp[0] = cmdu.Parameters.Add("TextActivePricing", "Disable"); 
                    cmdu.ExecuteNonQuery();
                    cmdu.Parameters.Clear();
                    cmdu.Dispose(); 
           //     }
                 
                conn.Close(); 

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Item Pricing Data Update Successfully"));
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

        [WebMethod]
        public static List<ListItem> GetItemList(int WpSlipNoExID)
        {
            string query = " SELECT WEECI.EXP_WBCON_ITEM_ID || '-' || WEECI.ITEM_WEIGHT || '-' || WI.ITEM_ID AS EXP_WBCON_ITEM_ID, WEECI.WB_SLIP_NO,  WI.ITEM_NAME || ' - ' || WI.ITEM_CODE || '- Item Weight: ' || WEECI.ITEM_WEIGHT || ', Item Bales: ' || WEECI.ITEM_BALES  AS ITEM_NAME FROM WP_EXPORT_WBSLIP_CON_ITEM WEECI LEFT JOIN WP_ITEM WI ON WI.ITEM_ID = WEECI.ITEM_ID WHERE WEECI.WB_SLIP_NO = :WpSlipNoExID AND IS_ACTIVE_PRICING IS NULL ";
            string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(strConnString))
            {
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    List<ListItem> cities = new List<ListItem>();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("WpSlipNoExID", WpSlipNoExID);
                    cmd.Connection = conn;
                    conn.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            cities.Add(new ListItem
                            {
                                Value = sdr["EXP_WBCON_ITEM_ID"].ToString(),
                                Text = sdr["ITEM_NAME"].ToString()
                            });
                        }
                    }
                    conn.Close();
                    return cities;
                }
            }
        }

        [WebMethod]
        public static List<ListItem> GetSalesItemList(int ItemID)
        {
            string query = " SELECT ITEM_SALES_ID, ITEM_SALES_DESCRIPTION, ITEM_ID FROM  WP_SALES_ITEM WHERE ITEM_ID =:ItemID AND IS_ACTIVE = 'Enable' ";
            string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(strConnString))
            {
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    List<ListItem> cities = new List<ListItem>();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("ItemID", ItemID);
                    cmd.Connection = conn;
                    conn.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            cities.Add(new ListItem
                            {
                                Value = sdr["ITEM_SALES_ID"].ToString(),
                                Text = sdr["ITEM_SALES_DESCRIPTION"].ToString()
                            });
                        }
                    }
                    conn.Close();
                    return cities;
                }
            }
        }

        protected void LinkSelectClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             
             LinkButton btn = (LinkButton)sender;
             Session["user_data_id"] = btn.CommandArgument;
            //   string[] USER_DATA_ID = Session["user_data_id"].ToString().Split('-');
            string USER_DATA_ID = Session["user_data_id"].ToString();

            DataTable dtSlipNo = new DataTable();
            DataSet dss = new DataSet();
            string makePageSQL = "  SELECT PEWC.WB_SLIP_NO,  PEWC.CONTAINER_NO || ' - ' || PEWC.WB_SLIP_NO  || '' || ', Item WT(WB) -' || TO_CHAR(PEWC.ITEM_WEIGHT_WB, '999,999,999') AS PARTY_NAME FROM WP_EXPORT_WBSLIP_CON PEWC WHERE  PEWC.EXPORT_INVOICE_NO IS NULL  ORDER BY PEWC.WB_SLIP_NO ASC ";
            dss = ExecuteBySqlString(makePageSQL);
            dtSlipNo = (DataTable)dss.Tables[0];
            DropDownWpSlipNoEx.DataSource = dtSlipNo;
            DropDownWpSlipNoEx.DataValueField = "WB_SLIP_NO";
            DropDownWpSlipNoEx.DataTextField = "PARTY_NAME";
            DropDownWpSlipNoEx.DataBind();
            DropDownWpSlipNoEx.Items.Insert(0, new ListItem("Select Weight / Container", "0"));
             
            string makeSQL = " SELECT PEWCI.EXP_WBCON_ITEM_ID, PEWCI.WB_SLIP_NO, PEWCI.ITEM_ID, PEWCI.ITEM_SALES_ID, PEWCI.MAT_PRICE_PER_MT, PEWCI.MATERIAL_AMOUNT, PEWCI.ITEM_WEIGHT, NCR.CURRENCY_RATE_ID || '-' || NCR.EXCHANGE_RATE AS CURRENCY_RATE_ID, PEWCI.CURRENCY_RATE, PEWCI.MATERIAL_CONVERSION_AMOUNT, PEWCI.IS_ACTIVE_PRICING, PEWCI.UPDATE_DATE_PRICING, PEWCI.U_USER_ID_PRICING FROM WP_EXPORT_WBSLIP_CON_ITEM PEWCI LEFT JOIN  NRC_CURRENCY_RATE NCR ON NCR.CURRENCY_RATE_ID = PEWCI.CURRENCY_RATE_ID WHERE PEWCI.EXP_WBCON_ITEM_ID = '" + USER_DATA_ID + "'";

             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn);
             ds = new DataTable();
             oradata.Fill(ds);
             RowCount = ds.Rows.Count;

             for (int i = 0; i < RowCount; i++)
             {
                DataTable dtItemID = new DataTable();
                DataSet dsi = new DataSet();
                string makeItemSQL = " SELECT WEECI.EXP_WBCON_ITEM_ID || '-' || WEECI.ITEM_WEIGHT || '-' || WI.ITEM_ID AS EXP_WBCON_ITEM_ID, WEECI.WB_SLIP_NO,  WI.ITEM_NAME || ' - ' || WI.ITEM_CODE || '- Item Weight: ' || WEECI.ITEM_WEIGHT || ', Item Bales: ' || WEECI.ITEM_BALES  AS ITEM_NAME FROM WP_EXPORT_WBSLIP_CON_ITEM WEECI LEFT JOIN WP_ITEM WI ON WI.ITEM_ID = WEECI.ITEM_ID WHERE WEECI.WB_SLIP_NO = '" + ds.Rows[i]["WB_SLIP_NO"].ToString() + "' ";
                dsi = ExecuteBySqlString(makeItemSQL);
                dtItemID = (DataTable)dsi.Tables[0];
                DropDownItemID.DataSource = dtItemID;
                DropDownItemID.DataValueField = "EXP_WBCON_ITEM_ID";
                DropDownItemID.DataTextField = "ITEM_NAME";
                DropDownItemID.DataBind();
                DropDownItemID.Items.Insert(0, new ListItem("Please Select Item", "0"));

                DataTable dtSalesItemID = new DataTable();
                DataSet dssi = new DataSet();
                string makeSalesItemSQL = " SELECT ITEM_SALES_ID, ITEM_SALES_DESCRIPTION, ITEM_ID FROM WP_SALES_ITEM WHERE ITEM_ID = '" + ds.Rows[i]["ITEM_ID"].ToString() + "' AND IS_ACTIVE = 'Enable'  ";
                dssi = ExecuteBySqlString(makeSalesItemSQL);
                dtSalesItemID = (DataTable)dssi.Tables[0];
                DropDownSalesItemID.DataSource = dtSalesItemID;
                DropDownSalesItemID.DataValueField = "ITEM_SALES_ID";
                DropDownSalesItemID.DataTextField = "ITEM_SALES_DESCRIPTION";
                DropDownSalesItemID.DataBind();
                DropDownSalesItemID.Items.Insert(0, new ListItem("Please Select Sales Item Name", "0"));

                //  DropDownWpSlipNoEx.Text        = ds.Rows[i]["WB_SLIP_NO"].ToString() + "-"+ ds.Rows[i]["ITEM_WEIGHT"].ToString();
                DropDownWpSlipNoEx.Text      = ds.Rows[i]["WB_SLIP_NO"].ToString();
                DropDownItemID.Text          = ds.Rows[i]["EXP_WBCON_ITEM_ID"].ToString() + "-" + ds.Rows[i]["ITEM_WEIGHT"].ToString() + "-" + ds.Rows[i]["ITEM_ID"].ToString();
                DropDownSalesItemID.Text     = ds.Rows[i]["ITEM_SALES_ID"].ToString();
                             double TotalQty = Convert.ToDouble(ds.Rows[i]["ITEM_WEIGHT"].ToString())/1000;
                TextTotalQty.Text            = decimal.Parse(TotalQty.ToString()).ToString("0.000");
                TextPricePerMt.Text          = decimal.Parse(ds.Rows[i]["MAT_PRICE_PER_MT"].ToString()).ToString(".00");
                TextTotalAmountEx.Text       = decimal.Parse(ds.Rows[i]["MATERIAL_AMOUNT"].ToString()).ToString(".00"); 
                DropDownCurrencyRateID.Text  = ds.Rows[i]["CURRENCY_RATE_ID"].ToString(); 
                TextItemCurrencyAmount.Text  = decimal.Parse(ds.Rows[i]["MATERIAL_CONVERSION_AMOUNT"].ToString()).ToString(".00"); 
                CheckIsActive.Checked        = Convert.ToBoolean(ds.Rows[i]["IS_ACTIVE_PRICING"].ToString() == "Enable" ? true : false);
                 
            }
              
             Display(); 
             conn.Close(); 
             alert_box.Visible = false;

             DropDownWpSlipNoEx.Enabled = false;
          //   DropDownItemID.Enabled = false;
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
                makeSQL = " SELECT PEWC.CONTAINER_NO, PEWCI.EXP_WBCON_ITEM_ID, PEWCI.WB_SLIP_NO, WI.ITEM_NAME, WSI.ITEM_SALES_DESCRIPTION, (PEWCI.ITEM_WEIGHT/1000) AS ITEM_WEIGHT, PEWCI.MAT_PRICE_PER_MT, PEWCI.MATERIAL_AMOUNT, NCS.CURRENCY_NAME AS SOURCE_CURRENCY_NAME, NCT.CURRENCY_NAME AS TARGET_CURRENCY_NAME, NCR.EXCHANGE_RATE, PEWCI.MATERIAL_CONVERSION_AMOUNT, PEWCI.UPDATE_DATE_PRICING, PEWCI.IS_ACTIVE_PRICING, PEWC.EXPORT_INVOICE_NO FROM WP_EXPORT_WBSLIP_CON_ITEM PEWCI LEFT JOIN WP_ITEM WI ON WI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN WP_SALES_ITEM WSI ON WSI.ITEM_SALES_ID = PEWCI.ITEM_SALES_ID LEFT JOIN  WP_EXPORT_WBSLIP_CON PEWC ON PEWC.WB_SLIP_NO = PEWCI.WB_SLIP_NO LEFT JOIN NRC_CURRENCY_RATE NCR ON NCR.CURRENCY_RATE_ID = PEWCI.CURRENCY_RATE_ID LEFT JOIN NRC_CURRENCY NCS ON NCS.CURRENCY_ID = NCR.SOURCE_CURRENCY_ID LEFT JOIN NRC_CURRENCY NCT ON NCT.CURRENCY_ID = NCR.TARGET_CURRENCY_ID WHERE PEWCI.IS_ACTIVE_PRICING IS NOT NULL AND PEWCI.IS_INVENTORY_STATUS = 'Transit'  ORDER BY PEWCI.WB_SLIP_NO DESC "; 
            }
            else
            {
                makeSQL = " SELECT PEWC.CONTAINER_NO, PEWCI.EXP_WBCON_ITEM_ID, PEWCI.WB_SLIP_NO, WI.ITEM_NAME, WSI.ITEM_SALES_DESCRIPTION, (PEWCI.ITEM_WEIGHT/1000) AS ITEM_WEIGHT, PEWCI.MAT_PRICE_PER_MT, PEWCI.MATERIAL_AMOUNT, NCS.CURRENCY_NAME AS SOURCE_CURRENCY_NAME, NCT.CURRENCY_NAME AS TARGET_CURRENCY_NAME, NCR.EXCHANGE_RATE, PEWCI.MATERIAL_CONVERSION_AMOUNT, PEWCI.UPDATE_DATE_PRICING, PEWCI.IS_ACTIVE_PRICING, PEWC.EXPORT_INVOICE_NO FROM WP_EXPORT_WBSLIP_CON_ITEM PEWCI LEFT JOIN WP_ITEM WI ON WI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN WP_SALES_ITEM WSI ON WSI.ITEM_SALES_ID = PEWCI.ITEM_SALES_ID LEFT JOIN  WP_EXPORT_WBSLIP_CON PEWC ON PEWC.WB_SLIP_NO = PEWCI.WB_SLIP_NO LEFT JOIN NRC_CURRENCY_RATE NCR ON NCR.CURRENCY_RATE_ID = PEWCI.CURRENCY_RATE_ID LEFT JOIN NRC_CURRENCY NCS ON NCS.CURRENCY_ID = NCR.SOURCE_CURRENCY_ID LEFT JOIN NRC_CURRENCY NCT ON NCT.CURRENCY_ID = NCR.TARGET_CURRENCY_ID WHERE PEWCI.IS_ACTIVE_PRICING IS NOT NULL AND (PEWC.CONTAINER_NO like '" + txtSearchUser.Text + "%' or  PEWCI.WB_SLIP_NO like '" + txtSearchUser.Text + "%')  ORDER BY PEWCI.WB_SLIP_NO DESC ";
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
                string isCheck = (Row.FindControl("IsPriceCheckLink") as Label).Text; 
                if (isCheck == "No")
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
            DropDownWpSlipNoEx.Text = "0"; 
        }

        public void clearText()
        {  
            DropDownCurrencyRateID.Text = "0";  
            TextPricePerMt.Text = "";
            TextTotalQty.Text = "";
            TextTotalAmountEx.Text = ""; 
            TextItemCurrencyAmount.Text = ""; 
            DropDownWpSlipNoEx.Text = "0"; 
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