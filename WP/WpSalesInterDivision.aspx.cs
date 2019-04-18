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
using System.Web.Services;

namespace NRCAPPS.WP
{
    public partial class WpSalesInterDivision : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl, cmdsp;
        OracleDataAdapter oradata; 
        DataTable dt, dtr;
        int RowCount;
         
        double  ItemAmtTotal = 0.0, ItemWtWbTotal = 0.0; string EntryDateSlip = "", ItemRate = "", PartyName = "";
        string IS_PAGE_ACTIVE = "", IS_ADD_ACTIVE = "", IS_EDIT_ACTIVE = "", IS_DELETE_ACTIVE = "", IS_VIEW_ACTIVE = "", IS_REPORT_ACTIVE = "", IS_PRINT_ACTIVE = "";
        public bool IsLoad { get; set; } public bool IsLoad1 { get; set; } public bool IsLoad2 { get; set; } public bool IsLoad3 { get; set; }
        public bool IsLoad4 { get; set; }
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
                    IS_PRINT_ACTIVE  = dt.Rows[i]["IS_PRINT_ACTIVE"].ToString();
                }

                if (IS_PAGE_ACTIVE == "Enable")
                {
                     if (!IsPostBack)
                    {
                        DataTable dtDivisionID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeCustomerSQL = "  SELECT DIVISION_ID,  DIVISION_ID || ' - ' || DIVISION_NAME AS DIVISION_NAME  FROM HR_EMP_DIVISIONS WHERE IS_ACTIVE = 'Enable' ORDER BY DIVISION_ID ASC ";
                        ds = ExecuteBySqlString(makeCustomerSQL);
                        dtDivisionID = (DataTable)ds.Tables[0];
                        DropDownDivisionID.DataSource = dtDivisionID;
                        DropDownDivisionID.DataValueField = "DIVISION_ID";
                        DropDownDivisionID.DataTextField = "DIVISION_NAME";
                        DropDownDivisionID.DataBind();
                        DropDownDivisionID.Items.Insert(0, new ListItem("Select Division", "0")); 
                          
                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeDropDownItemSQL = " SELECT * FROM WP_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        dsi = ExecuteBySqlString(makeDropDownItemSQL);
                        dtItemID = (DataTable)dsi.Tables[0];
                     /*   DropDownItemID.DataSource = dtItemID;
                        DropDownItemID.DataValueField = "ITEM_ID";
                        DropDownItemID.DataTextField = "ITEM_NAME";
                        DropDownItemID.DataBind();
                        DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0")); */

                        DropDownItemID1.DataSource = dtItemID;
                        DropDownItemID1.DataValueField = "ITEM_ID";
                        DropDownItemID1.DataTextField = "ITEM_NAME";
                        DropDownItemID1.DataBind();
                        DropDownItemID1.Items.Insert(0, new ListItem("All Item", "0"));

                        DataTable dtSubItemID = new DataTable();
                        DataSet dss = new DataSet();
                        string makeDropDownSubItemSQL = " SELECT * FROM WP_CATEGORY WHERE IS_ACTIVE = 'Enable' ORDER BY CATEGORY_ID ASC";
                        dss = ExecuteBySqlString(makeDropDownSubItemSQL);
                        dtSubItemID = (DataTable)dss.Tables[0];
                        DropDownCategoryID.DataSource = dtSubItemID;
                        DropDownCategoryID.DataValueField = "CATEGORY_ID";
                        DropDownCategoryID.DataTextField = "CATEGORY_NAME";
                        DropDownCategoryID.DataBind(); 
                        DropDownCategoryID.Items.Insert(0, new ListItem("Select Category", "0"));

                      

                        //  DropDownDivisionID.Attributes.Add("disabled", "disabled");
                        //  TextItemWeightWp.Attributes.Add("disabled", "disabled");

                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled");

                        TextInvoiceWpNo.Focus();
                        TextItemAmount.Enabled = false; 
                        Display();
                        DisplaySummary(); 
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

        public void BtnAdd_Click(object sender, EventArgs e)
        {
          //  try
          //  {
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]); 
                    string InvoiceNo = TextInvoiceWpNo.Text;
                    int DivisionID = Convert.ToInt32(DropDownDivisionID.Text);
                    int CategoryID = Convert.ToInt32(DropDownCategoryID.Text);
                    int ItemID = Convert.ToInt32(Request.Form[DropDownItemID.UniqueID]);
                    double ItemRate = Convert.ToDouble(TextItemRateWp.Text);
                    double ItemWeight = Convert.ToDouble(TextItemWeightWp.Text);
                    double ItemAmount = (ItemRate * ItemWeight)/1000;  
                      
                    string Remarks = TextRemarks.Text;
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    //inventory calculation 
                    int InvenItemID = 0; 
                    double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, FinalStock = 0.00, StockInWetNew = 0.00, StockOutWetNew = 0.00, FinalStockNew = 0.00;
                     
                    string makeSQL2 = " select * from WP_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "' ";  
                    cmdl = new OracleCommand(makeSQL2);
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

                    if (ItemWeight <= FinalStock)
                    { 
                        StockOutWetNew = StockOutWet + ItemWeight;
                        FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;

                        if (0 < RowCount)
                        { 
                            string update_inven_mas = "update  WP_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                            cmdu = new OracleCommand(update_inven_mas, conn); 
                            OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                            objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetNew);
                            objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                            objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                            objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                            objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID); 

                            cmdu.ExecuteNonQuery();
                            cmdu.Parameters.Clear();
                            cmdu.Dispose();
                             
                        }
                         
                        // insert sales data
                        string get_id = " select WP_SALES_IN_DIV_MASTERID_SEQ.nextval from dual";
                        cmdu = new OracleCommand(get_id, conn);
                        int newSalesID = Int16.Parse(cmdu.ExecuteScalar().ToString());
                        string insert_production = " insert into  WP_SALES_INTER_DIV_MASTER (SALES_INTER_DIV_ID, WB_SLIP_NO, INTER_DIVISION_ID, CATEGORY_ID, ITEM_ID, ITEM_WEIGHT_WB, ITEM_WEIGHT, ITEM_RATE, ITEM_AMOUNT, REMARKS, ENTRY_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID) values ( :NoSalesID, :NoInvoiceNo, :NoDivisionID, :NoCategoryID, :NoItemID, :TextItemWtWb, :TextItemWeightWp, :TextItemRateWp, :TextItemAmount, :TextRemarks, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 2)";
                        cmdi = new OracleCommand(insert_production, conn);

                        OracleParameter[] objPrm = new OracleParameter[14];
                        objPrm[0] = cmdi.Parameters.Add("NoSalesID", newSalesID);
                        objPrm[1] = cmdi.Parameters.Add("NoInvoiceNo", InvoiceNo);
                        objPrm[2] = cmdi.Parameters.Add("NoDivisionID", DivisionID);
                        objPrm[3] = cmdi.Parameters.Add("NoCategoryID", CategoryID);
                        objPrm[4] = cmdi.Parameters.Add("NoItemID", ItemID); 
                        objPrm[5] = cmdi.Parameters.Add("TextItemWtWb", TextItemWtWb.Text);
                        objPrm[6] = cmdi.Parameters.Add("TextItemWeightWp", ItemWeight);
                        objPrm[7] = cmdi.Parameters.Add("TextItemRateWp", ItemRate);
                        objPrm[8] = cmdi.Parameters.Add("TextItemAmount", ItemAmount); 
                        objPrm[9] = cmdi.Parameters.Add("TextRemarks", Remarks);
                        objPrm[10] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                        objPrm[11] = cmdi.Parameters.Add("c_date", c_date);
                        objPrm[12] = cmdi.Parameters.Add("NoCuserID", userID);
                        objPrm[13] = cmdi.Parameters.Add("TextIsActive", ISActive); 
                        
                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();
                    
                        conn.Close();

                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Insert New Sales (Inter Division) Successfully"));
                        alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                        clearText();
                        Display();
                        DisplaySummary();
                    }
                    else
                    {

                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Item Weight is not Available in the Inventory" + ItemID));
                        alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                    }

                    

                       
                     
                }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
           //     }
          //  catch
           //   {
           //     Response.Redirect("~/ParameterError.aspx");
           //   } 
        }

        [WebMethod]
        public static Boolean WpSalesInvoiceNoCheck(int InvoiceNo)
        {
            Boolean result = false;
            string query = "select WB_SLIP_NO from WP_SALES_INTER_DIV_MASTER where WB_SLIP_NO = '" + InvoiceNo + "'";
            string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(strConnString))
            {
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;
                    conn.Open();
                    OracleDataReader sdr = cmd.ExecuteReader();
                    if (sdr.HasRows)
                    {
                        result = true;
                    }
                    conn.Close();
                    return result;
                }
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            if (IS_PRINT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string SlipWbNo = "";
                if (TextInvoiceWpNo.Text != "")
                {
                    SlipWbNo = TextInvoiceWpNo.Text;
                }
                else
                {
                    LinkButton btn = (LinkButton)sender;
                    Session["user_data_id"] = btn.CommandArgument;
                    SlipWbNo = Session["user_data_id"].ToString();
                }


                string HtmlString = "";
                string makeSQL = " SELECT PSM.SALES_INTER_DIV_ID, PSM.WB_SLIP_NO, HED.DIVISION_NAME, PI.ITEM_NAME, PSI.CATEGORY_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE,  PSM.IS_PRINT, TO_CHAR(PSM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE  FROM WP_SALES_INTER_DIV_MASTER PSM LEFT JOIN HR_EMP_DIVISIONS HED ON HED.DIVISION_ID = PSM.INTER_DIVISION_ID LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN WP_CATEGORY PSI ON PSI.CATEGORY_ID = PSM.CATEGORY_ID  WHERE PSM.WB_SLIP_NO = '" + SlipWbNo + "' ";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count; 
                for (int i = 0; i < 1; i++)
                { 
                    HtmlString += "<div style='float:left;width:785px;height:258px;margin-top:220px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 16px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";
                    HtmlString += "<div style='float:left;width:470px;'> ";
                    HtmlString += "<div style='float:left;width:260px;height:38px;margin-left:192px;padding-top:65px;' >" +  dt.Rows[i]["DIVISION_NAME"].ToString() + "</div> ";

                 
                HtmlString += "<div style='float:left;width:300px;height:28px;margin:0px 0 0 192px;'> ";
                  
                HtmlString += ""  + dt.Rows[i]["ITEM_NAME"].ToString() + "";
                     
                HtmlString += "</div> ";

                HtmlString += "<div style='float:left;width:260px;height:20px;margin-left:187px;padding:10px 0 0 5px;'> ";
                
                HtmlString += "</div> ";

                HtmlString += "<div style='float:left;width:260px;height:20px;margin-left:187px;padding:18px 0 0 5px;'> ";
                
                HtmlString += "</div> ";
                 
                HtmlString += "</div> ";

                HtmlString += "<div style='float:left;width:310px;'> ";

                HtmlString += "<div style='float:left;width:305px;margin:0 0 0 40px;'> ";
                HtmlString += "<div style='float:left;width:175px;padding:3px;text-align:right;border:black 1px solid;'> Net Weight (KG)</div> ";
                HtmlString += "<div style='float:left;width:70px;padding:4px;text-align:right;'>" + dt.Rows[i]["ITEM_WEIGHT"].ToString() + "</div> ";
                HtmlString += "</div>";

                HtmlString += "<div style='float:left;width:305px;margin:0 0 0 40px;'> ";
                HtmlString += "<div style='float:left;width:175px;padding:3px;text-align:right;border:black 1px solid;'>  Item Rate</div> ";
                HtmlString += "<div style='float:left;width:70px;padding:4px;text-align:right;'>" + dt.Rows[i]["ITEM_RATE"].ToString() + "</div> ";
                HtmlString += "</div>";

                HtmlString += "<div style='float:left;width:305px;margin:0 0 0 40px;'> ";
                HtmlString += "<div style='float:left;width:175px;padding:3px;text-align:right;border:black 1px solid;'> Item Amount (SR)</div> ";
                HtmlString += "<div style='float:left;width:70px;padding:4px;text-align:right;'>" + dt.Rows[i]["ITEM_AMOUNT"].ToString() + "</div> ";
                HtmlString += "</div>";
                }
                HtmlString += "<div style='float:left;width:300px;height:20px;margin:60 0 0 35px;'>INTER DIVISION TRANSFER</div> ";
                HtmlString += "</div>";
                HtmlString += "</div>";

                HtmlString += "</div>";
                HtmlString += "</div>";
                HtmlString += "</div>";
                HtmlString += "</div>";

                // weigh-bridge & container update for print
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string update_user = "update  WP_SALES_INTER_DIV_MASTER  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where WB_SLIP_NO = :NoSlipNo ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[4];
                objPrm[0] = cmdi.Parameters.Add("TextIsPrint", "Printed");
                objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[2] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[3] = cmdi.Parameters.Add("NoSlipNo", SlipWbNo);

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

        protected void linkSelectClick(object sender, EventArgs e)
        {
        // try{
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
            string USER_DATA_ID = Session["user_data_id"].ToString();
                 
            DataTable dtUserTypeID = new DataTable();
            DataSet ds = new DataSet();
            string makeSQL = " select SALES_INTER_DIV_ID, WB_SLIP_NO, INTER_DIVISION_ID, ITEM_ID, CATEGORY_ID, ITEM_WEIGHT, ITEM_RATE, ITEM_AMOUNT, nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, REMARKS, TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, CREATE_DATE, UPDATE_DATE, IS_ACTIVE from WP_SALES_INTER_DIV_MASTER where WB_SLIP_NO  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                DataTable dtItemID = new DataTable();
                DataSet dsi = new DataSet();
                string makeDropDownItemSQL = " SELECT ITEM_ID, ITEM_NAME || ' - ' || ITEM_CODE AS ITEM_NAME FROM WP_ITEM WHERE CATEGORY_ID = '" + Convert.ToInt32(dt.Rows[i]["CATEGORY_ID"].ToString()) + "' AND IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                dsi = ExecuteBySqlString(makeDropDownItemSQL);
                dtItemID = (DataTable)dsi.Tables[0];
                DropDownItemID.DataSource = dtItemID;
                DropDownItemID.DataValueField = "ITEM_ID";
                DropDownItemID.DataTextField = "ITEM_NAME";
                DropDownItemID.DataBind();

                TextInvoiceWpNo.Text          = dt.Rows[i]["WB_SLIP_NO"].ToString();
                DropDownDivisionID.Text     = dt.Rows[i]["INTER_DIVISION_ID"].ToString();  
                DropDownItemID.Text         = dt.Rows[i]["ITEM_ID"].ToString(); 
                DropDownCategoryID.Text      = dt.Rows[i]["CATEGORY_ID"].ToString();
                TextItemRateWp.Text           = dt.Rows[i]["ITEM_RATE"].ToString();
                TextItemWeightWp.Text         = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT"].ToString()).ToString("0.00");
                TextItemAmount.Text         = decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString("0.00");  
                TextRemarks.Text            = dt.Rows[i]["REMARKS"].ToString();
                EntryDate.Text              = dt.Rows[i]["ENTRY_DATE"].ToString(); 
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                TextItemWtWb.Text         = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString()).ToString("0.00");
            }

            conn.Close();
            Display();
            CheckItemWeight.Text = "";
            CheckInvoiceNo.Text = "";
            DropDownDivisionID.Attributes.Remove("disabled");
            alert_box.Visible = false;
         //   DropDownItemID.Attributes.Add("readonly", "readonly");
         //   DropDownCategoryID.Enabled = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
            BtnUpdate.Attributes.Add("aria-disabled", "true");
            BtnUpdate.Attributes.Add("class", "btn btn-success active");
            BtnDelete.Attributes.Add("aria-disabled", "true");
            BtnDelete.Attributes.Add("class", "btn btn-danger active");
       //     }
      //   catch
      //   {
     //          Response.Redirect("~/ParameterError.aspx");
      //   } 
        }

     

        public void Display()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open(); 
                string makeSQL = "";
                string MonthYear = System.DateTime.Now.ToString("MM/yyyy");
                if (txtSearchEmp.Text == "")
                {
                    makeSQL = " SELECT PSM.SALES_INTER_DIV_ID, PSM.WB_SLIP_NO, HED.DIVISION_NAME, PI.ITEM_NAME, PSI.CATEGORY_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE,  PSM.IS_PRINT, TO_CHAR(PSM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE  FROM WP_SALES_INTER_DIV_MASTER PSM LEFT JOIN HR_EMP_DIVISIONS HED ON HED.DIVISION_ID = PSM.INTER_DIVISION_ID LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN WP_CATEGORY PSI ON PSI.CATEGORY_ID = PSM.CATEGORY_ID  WHERE  to_char(PSM.ENTRY_DATE, 'mm/yyyy') = '" + MonthYear + "' ORDER BY PSM.CREATE_DATE DESC";
                }
                else
                {
                    if (DropDownItemID1.Text == "0")
                    {
                        makeSQL = " SELECT PSM.SALES_INTER_DIV_ID, PSM.WB_SLIP_NO, HED.DIVISION_NAME, PI.ITEM_NAME, PSI.CATEGORY_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE,  PSM.IS_PRINT, TO_CHAR(PSM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE  FROM WP_SALES_INTER_DIV_MASTER PSM  LEFT JOIN HR_EMP_DIVISIONS HED ON HED.DIVISION_ID = PSM.INTER_DIVISION_ID LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN WP_CATEGORY PSI ON PSI.CATEGORY_ID = PSM.CATEGORY_ID  WHERE (PSM.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or HED.DIVISION_NAME like '" + txtSearchEmp.Text + "%' or PSI.CATEGORY_NAME like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'mm/yyyy') like '" + txtSearchEmp.Text + "%' or PSM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ) ORDER BY PSM.CREATE_DATE desc, PSM.UPDATE_DATE desc";
                    }
                    else
                    {
                        makeSQL = " SELECT PSM.SALES_INTER_DIV_ID, PSM.WB_SLIP_NO, HED.DIVISION_NAME, PI.ITEM_NAME, PSI.CATEGORY_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE,  PSM.IS_PRINT, TO_CHAR(PSM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE  FROM WP_SALES_INTER_DIV_MASTER PSM  LEFT JOIN HR_EMP_DIVISIONS HED ON HED.DIVISION_ID = PSM.INTER_DIVISION_ID LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN WP_CATEGORY PSI ON PSI.CATEGORY_ID = PSM.CATEGORY_ID  WHERE PI.ITEM_ID like '" + DropDownItemID1.Text + "%'  AND (PSM.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or HED.DIVISION_NAME like '" + txtSearchEmp.Text + "%' or PSI.CATEGORY_NAME like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'mm/yyyy') like '" + txtSearchEmp.Text + "%' or PSM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ) ORDER BY PSM.CREATE_DATE desc, PSM.UPDATE_DATE desc";

                     }
                    alert_box.Visible = false;
               }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "WB_SLIP_NO" };
                GridView1.DataBind();
                conn.Close(); 
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


        public void DisplaySummary()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = "";
                string MonthYear = System.DateTime.Now.ToString("MM/yyyy");
                if (TextMonthYear4.Text == "")
                {
                    makeSQL = "  SELECT PI.ITEM_NAME, count(PSM.SALES_INTER_DIV_ID) AS SALES_INTER_DIV_ID, sum(PSM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PSM.ITEM_AMOUNT) AS ITEM_AMOUNT  FROM WP_ITEM PI LEFT JOIN WP_SALES_INTER_DIV_MASTER PSM ON PSM.ITEM_ID = PI.ITEM_ID WHERE to_char(PSM.ENTRY_DATE, 'mm/yyyy') =  '" + MonthYear + "' GROUP BY PI.ITEM_ID, PI.ITEM_NAME ORDER BY PI.ITEM_ID";
                }
                else
                { 
                    makeSQL = "  SELECT PI.ITEM_NAME, count(PSM.SALES_INTER_DIV_ID) AS SALES_INTER_DIV_ID, sum(PSM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PSM.ITEM_AMOUNT) AS ITEM_AMOUNT FROM WP_ITEM PI LEFT JOIN WP_SALES_INTER_DIV_MASTER PSM ON PSM.ITEM_ID = PI.ITEM_ID WHERE to_char(PSM.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear4.Text + "' GROUP BY PI.ITEM_ID, PI.ITEM_NAME ORDER BY PI.ITEM_ID";
                    alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataKeyNames = new string[] { "ITEM_NAME" };
                GridView2.DataBind();
                if (dt.Rows.Count > 0)
                {
                    GridView2.HeaderRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    //Calculate Sum and display in Footer Row 
                    GridView2.FooterRow.Cells[0].Font.Bold = true;
                    GridView2.FooterRow.Cells[0].Text = "Grand Total";
                    GridView2.FooterRow.Cells[0].HorizontalAlign = HorizontalAlign.Right;

                    decimal total_prod = dt.AsEnumerable().Sum(row => row.Field<decimal>("SALES_INTER_DIV_ID"));
                    GridView2.FooterRow.Cells[1].Font.Bold = true;
                    GridView2.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Center;
                    GridView2.FooterRow.Cells[1].Text = total_prod.ToString("N0");

                    decimal total_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_WEIGHT"));
                    GridView2.FooterRow.Cells[2].Font.Bold = true;
                    GridView2.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[2].Text = total_wt.ToString("N2");

                    decimal total_pge_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_AMOUNT"));
                    GridView2.FooterRow.Cells[3].Font.Bold = true;
                    GridView2.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[3].Text = total_pge_wt.ToString("N2");
                    
                }
                else
                {

                }
                conn.Close();
                // alert_box.Visible = false;
            }
        }

        protected void GridViewSearchSummary(object sender, EventArgs e)
        {
            this.DisplaySummary();
        }

        protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            DisplaySummary();
            alert_box.Visible = false;
        }


        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
      //  try {

                if (IS_EDIT_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    string InvoiceNo = TextInvoiceWpNo.Text;
                    int DivisionID = Convert.ToInt32(DropDownDivisionID.Text);
                    int ItemID = Convert.ToInt32(Request.Form[DropDownItemID.UniqueID]);
                    double ItemRate = Convert.ToDouble(TextItemRateWp.Text);
                    double ItemWeight = Convert.ToDouble(TextItemWeightWp.Text);
                    double ItemAmount = (ItemRate * ItemWeight)/1000;  
                    int CategoryID = Convert.ToInt32(DropDownCategoryID.Text);  
                    string Remarks = TextRemarks.Text;
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-'); 
                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                 
                    // check production data
                    int ItemIdOld = 0, SubItemIdOld = 0; double ItemWeightOld = 0.00;
                    string makeSQLPro = " select ITEM_ID, ITEM_WEIGHT from WP_SALES_INTER_DIV_MASTER where WB_SLIP_NO  = '" + InvoiceNo + "'  ";
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
                    double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, StockOutWetDe = 0.00, FinalStock = 0.00, StockOutWetNew = 0.00, FinalStockNew = 0.00;


                    // check inventory RM
                    string makeSQL2 = " select * from WP_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";   
                    cmdl = new OracleCommand(makeSQL2);
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
                    if (ItemWeight <= FinalStock)
                    {
                        StockOutWetNew = StockOutWet - ItemWeightOld;
                        FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;

                        if (0 < RowCount)
                        {
                            // update inventory RM (minus old data)
                            string update_inven_mas = "update  WP_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";  
                            cmdu = new OracleCommand(update_inven_mas, conn);

                            OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                            objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetNew);
                            objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                            objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                            objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                            objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID); 

                            cmdu.ExecuteNonQuery();
                            cmdu.Parameters.Clear();
                            cmdu.Dispose();
                        }
                         
                        // check inventory RM
                        string makeSQL3 = " select * from WP_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                        cmdl = new OracleCommand(makeSQL3);
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

                        StockOutWetDe = StockOutWet + ItemWeight;
                        FinalStockNew = (InitialStock + StockInWet) - StockOutWetDe;
                        // update inventory RM (plus new data)
                        string update_inven_rm_new = "update  WP_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                        cmdu = new OracleCommand(update_inven_rm_new, conn);

                        OracleParameter[] objPrmInevenRmNew = new OracleParameter[5];
                        objPrmInevenRmNew[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetDe);
                        objPrmInevenRmNew[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenRmNew[2] = cmdu.Parameters.Add("u_date", u_date);
                        objPrmInevenRmNew[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenRmNew[4] = cmdu.Parameters.Add("NoItemID", ItemID); 

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                     
                        // update sales master
                        string update_sales = " update WP_SALES_INTER_DIV_MASTER set  INTER_DIVISION_ID = :NoDivisionID, ITEM_ID = :NoItemID, CATEGORY_ID = :NoCategoryID, ITEM_WEIGHT_WB = :NoItemWeightWB, ITEM_WEIGHT = :NoItemWeight, ITEM_RATE = :NoItemRate, ITEM_AMOUNT = :NoItemAmount, REMARKS = :TextRemarks, ENTRY_DATE = TO_DATE(:TextEntryDate, 'DD-MM-YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where WB_SLIP_NO = :NoInvoiceNo ";
                        cmdu = new OracleCommand(update_sales, conn);

                        OracleParameter[] objPrm = new OracleParameter[13];
                        objPrm[0] = cmdu.Parameters.Add("NoDivisionID", DivisionID);
                        objPrm[1] = cmdu.Parameters.Add("NoItemID", ItemID);
                        objPrm[2] = cmdu.Parameters.Add("NoCategoryID", CategoryID);
                        objPrm[3] = cmdu.Parameters.Add("NoItemWeightWB", TextItemWtWb.Text);
                        objPrm[4] = cmdu.Parameters.Add("NoItemWeight", ItemWeight);
                        objPrm[5] = cmdu.Parameters.Add("NoItemRate", ItemRate);
                        objPrm[6] = cmdu.Parameters.Add("NoItemAmount", ItemAmount); 
                        objPrm[7] = cmdu.Parameters.Add("TextRemarks", Remarks);
                        objPrm[8] = cmdu.Parameters.Add("TextEntryDate", EntryDateNew);
                        objPrm[9] = cmdu.Parameters.Add("u_date", u_date);
                        objPrm[10] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrm[11] = cmdu.Parameters.Add("TextIsActive", ISActive);
                        objPrm[12] = cmdu.Parameters.Add("NoInvoiceNo", InvoiceNo);

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();

                        conn.Close();

                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Sales (Inter Division) Data Update Successfully"));
                        alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                        clearText();
                        Display();
                        DisplaySummary();

                }
                else
                {
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory"));
                    alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                }

             
             }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
          //    }
         //   catch
         //   {
         //       Response.Redirect("~/ParameterError.aspx");
          //  } 
        }


        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {

                if (IS_DELETE_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    string SALES_INTER_DIV_ID = TextInvoiceWpNo.Text;
                    int ItemID = Convert.ToInt32(Request.Form[DropDownItemID.UniqueID]);
                    //    int SubItemID = Convert.ToInt32(DropDownCategoryID.Text);  
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    // check production data
                    int ItemIdOld = 0, SubItemIdOld = 0; double ItemWeightOld = 0.00;
                    string makeSQLPro = " select ITEM_ID, ITEM_WEIGHT from WP_SALES_INTER_DIV_MASTER where WB_SLIP_NO  = '" + SALES_INTER_DIV_ID + "'  ";
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
                    double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, FinalStock = 0.00, StockOutWetNew = 0.00, FinalStockNew = 0.00;
                   
                        // check inventory RM
                        string makeSQL2 = " select * from WP_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                        cmdl = new OracleCommand(makeSQL2);
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

                        StockOutWetNew = StockOutWet - ItemWeightOld;
                        FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;
   
                       if (ItemWeightOld <= FinalStock)
                         {
                        if (0 < RowCount)
                        {
                            // update inventory RM (minus old data)
                            string update_inven_mas = "update  WP_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                            cmdu = new OracleCommand(update_inven_mas, conn);

                            OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                            objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetNew);
                            objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                            objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                            objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                            objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID); 

                            cmdu.ExecuteNonQuery();
                            cmdu.Parameters.Clear();
                            cmdu.Dispose();
                        }

                        // delete sales master
                        string delete_production = " delete from WP_SALES_INTER_DIV_MASTER where WB_SLIP_NO  = '" + SALES_INTER_DIV_ID + "'";
                        cmdi = new OracleCommand(delete_production, conn);
                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();

                        conn.Close();
                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Sales (Inter Division) Data Delete Successfully"));
                        alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");

                        clearText();
                        Display();
                        DisplaySummary();
                    }
                    else
                    {

                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory"));
                        alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                    } 
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
            TextInvoiceWpNo.Text = "";   
            TextItemWeightWp.Text = "";
            TextItemRateWp.Text = "";
            TextItemAmount.Text = ""; 
            DropDownCategoryID.Text = "0"; 
            CheckItemWeight.Text = ""; 
            DropDownDivisionID.Text = "0";  
         //   DropDownItemID.Text = "0";
            CheckEntryDate.Text = "";
            TextItemWtWb.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        {
            TextInvoiceWpNo.Text = ""; 
            TextItemWeightWp.Text = "";
            TextItemRateWp.Text = ""; 
            TextItemAmount.Text = ""; 
            DropDownCategoryID.Text = "0"; 
            CheckItemWeight.Text = ""; 
            DropDownDivisionID.Text = "0";  
          //  DropDownItemID.Text = "0";
            CheckInvoiceNo.Text = "";
            CheckEntryDate.Text = "";
            TextItemWtWb.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

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

        public void TextCheckDataProcess(object sender, EventArgs e)
        {
            // Check inventory data process in last month
            string MakeAsOnDate = EntryDate.Text;
            string[] MakeAsOnDateSplit = MakeAsOnDate.Split('-');
            String AsOnDateTemp = MakeAsOnDateSplit[0].Replace("/", "-");
            DateTime AsOnDateNewD = DateTime.ParseExact(AsOnDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string AsOnDateNew = AsOnDateNewD.ToString("dd-MM-yyyy");

            DateTime curDate = AsOnDateNewD;
            DateTime startDate = curDate.AddMonths(-1);
            DateTime LastDateTemp = curDate.AddDays(-(curDate.Day));
            string LastDate = LastDateTemp.ToString("dd-MM-yyyy");
            string LastMonthTemp = LastDateTemp.ToString("MM-yyyy");
            DateTime LastMonth = DateTime.ParseExact(LastMonthTemp, "MM-yyyy", CultureInfo.InvariantCulture);
            string CurrentMonthTemp = AsOnDateNewD.ToString("MM-yyyy");
            DateTime CurrentMonth = DateTime.ParseExact(CurrentMonthTemp, "MM-yyyy", CultureInfo.InvariantCulture);
            string SysMonthTemp = System.DateTime.Now.ToString("MM-yyyy");
            DateTime SysMonth = DateTime.ParseExact(SysMonthTemp, "MM-yyyy", CultureInfo.InvariantCulture);
            DateTime SysLastMonth = SysMonth.AddMonths(-1);

            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select ITEM_ID from WP_RM_STOCK_INVENTORY_HISTORY where TO_CHAR(TO_DATE(CREATE_DATE), 'dd-MM-yyyy')   = '" + LastDate + "'";
            cmd.CommandType = CommandType.Text;

            OracleDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                if (CurrentMonth == SysMonth || CurrentMonth == SysLastMonth)
                {
                    CheckEntryDate.Text = "";
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");
                    CheckEntryDate.Text = "<label class='control-label'><i class='fa fa fa-check-circle'></i></label>";
                    CheckEntryDate.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    CheckEntryDate.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Please, Insert Data Current or last months.</label>";
                    CheckEntryDate.ForeColor = System.Drawing.Color.Red;
                    EntryDate.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
            }
            else
            {
                CheckEntryDate.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Please, Complete Data process in last months (" + LastDate + "). It is required for insert current month data. </label>";
                CheckEntryDate.ForeColor = System.Drawing.Color.Red;
                EntryDate.Focus();
                BtnAdd.Attributes.Add("aria-disabled", "false");
                BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
            }
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
        protected void BtnReport_Click1(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad1 = true;
                alert_box.Visible = false;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            } 
        }
        protected void BtnReport_Click2(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad2 = true;
                alert_box.Visible = false;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        protected void BtnReport_Click3(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad3 = true;
                alert_box.Visible = false;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        protected void BtnReport_Click4(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad4 = true;
                alert_box.Visible = false;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }


    }

}