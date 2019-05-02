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

namespace NRCAPPS.MS
{
    public partial class MsSales : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl, cmdsp;
        OracleDataAdapter oradata; 
        DataTable dt, dtr;
        int RowCount;
         
        double ItemVatAmt = 0.0, ItemAmtTotal = 0.0, ItemWtWbTotal = 0.0; string EntryDateSlip = "", PartyArabicName = "", PartyName = "", PartyID="";
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
                        DataTable dtCustomerID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeCustomerSQL = "  SELECT PARTY_ID,  PARTY_ID || ' - ' || PARTY_NAME || ' - ' || PARTY_VAT_NO AS PARTY_NAME  FROM MS_PARTY WHERE IS_ACTIVE = 'Enable' ORDER BY PARTY_NAME ASC ";
                        ds = ExecuteBySqlString(makeCustomerSQL);
                        dtCustomerID = (DataTable)ds.Tables[0];
                        DropDownCustomerID.DataSource = dtCustomerID;
                        DropDownCustomerID.DataValueField = "PARTY_ID";
                        DropDownCustomerID.DataTextField = "PARTY_NAME";
                        DropDownCustomerID.DataBind();
                        DropDownCustomerID.Items.Insert(0, new ListItem("Select Customer", "0"));

                        DropDownCustomerID1.DataSource = dtCustomerID;
                        DropDownCustomerID1.DataValueField = "PARTY_ID";
                        DropDownCustomerID1.DataTextField = "PARTY_NAME";
                        DropDownCustomerID1.DataBind();
                        DropDownCustomerID1.Items.Insert(0, new ListItem("Select Customer", "0"));
                          
                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeDropDownItemSQL = " SELECT * FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
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
                        string makeDropDownSubItemSQL = " SELECT * FROM MF_CATEGORY WHERE IS_ACTIVE = 'Enable' ORDER BY CATEGORY_ID ASC";
                        dss = ExecuteBySqlString(makeDropDownSubItemSQL);
                        dtSubItemID = (DataTable)dss.Tables[0];
                        DropDownCategoryID.DataSource = dtSubItemID;
                        DropDownCategoryID.DataValueField = "CATEGORY_ID";
                        DropDownCategoryID.DataTextField = "CATEGORY_NAME";
                        DropDownCategoryID.DataBind(); 
                        DropDownCategoryID.Items.Insert(0, new ListItem("Select Category", "0"));

                        DataTable dtPgeID = new DataTable();
                        DataSet dsp = new DataSet();
                        string makeDropDownPgeSQL = " SELECT * FROM NRC_VAT WHERE IS_ACTIVE = 'Enable' ORDER BY VAT_ID ASC";
                        dsp = ExecuteBySqlString(makeDropDownPgeSQL);
                        dtPgeID = (DataTable)dsp.Tables[0];
                        DropDownVatID.DataSource = dtPgeID;
                        DropDownVatID.DataValueField = "VAT_ID";
                        DropDownVatID.DataTextField = "VAT_PERCENT";
                        DropDownVatID.DataBind();
                        //   DropDownVatID.Items.Insert(0, new ListItem("Select Garbage Est. of Prod.", "0"));

                        //  DropDownCustomerID.Attributes.Add("disabled", "disabled");
                        //  TextItemWeightWp.Attributes.Add("disabled", "disabled");

                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled");

                        TextInvoiceMsNo.Focus();
                        TextItemAmount.Enabled = false;
                        TextVatAmount.Enabled = false;
                        TextTotalAmount.Enabled = false;
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
           try
            {
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]); 
                    string InvoiceNo = TextInvoiceMsNo.Text;
                    int CustomerID = Convert.ToInt32(DropDownCustomerID.Text);
                    int CategoryID = Convert.ToInt32(DropDownCategoryID.Text);
                    int ItemID = Convert.ToInt32(Request.Form[DropDownItemID.UniqueID]);
                    double ItemRate = Convert.ToDouble(TextItemRateWp.Text);
                    double ItemWeight = Convert.ToDouble(TextItemWeightWp.Text);
                    double ItemAmount = (ItemRate * ItemWeight)/1000;
                    int VatID = Convert.ToInt32(DropDownVatID.Text);
                    double VatPercent = Convert.ToDouble(DropDownVatID.SelectedItem.Text);
                    double VatAmount = (ItemAmount * VatPercent) / 100;
                      
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
                     
                    string makeSQL2 = " select * from MS_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "' ";  
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
                            string update_inven_mas = "update  MS_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
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
                        string get_id = "select MS_SALES_MASTERID_SEQ.nextval from dual";
                        cmdu = new OracleCommand(get_id, conn);
                        int newSalesID = Int16.Parse(cmdu.ExecuteScalar().ToString());
                        string insert_production = "insert into  MS_SALES_MASTER (SALES_ID, INVOICE_NO, PARTY_ID, CATEGORY_ID, ITEM_ID, ITEM_WEIGHT_WB, ITEM_WEIGHT, ITEM_RATE, ITEM_AMOUNT, VAT_ID, VAT_PERCENT, VAT_AMOUNT, REMARKS, ENTRY_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID) values ( :NoSalesID, :NoInvoiceNo, :NoCustomerID, :NoCategoryID, :NoItemID, :TextItemWtWb, :TextItemWeightWp, :TextItemRateWp, :TextItemAmount, :NoVatID, :NoVatPercent, :NoVatAmount, :TextRemarks, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 4)";
                        cmdi = new OracleCommand(insert_production, conn);

                        OracleParameter[] objPrm = new OracleParameter[17];
                        objPrm[0] = cmdi.Parameters.Add("NoSalesID", newSalesID);
                        objPrm[1] = cmdi.Parameters.Add("NoInvoiceNo", InvoiceNo);
                        objPrm[2] = cmdi.Parameters.Add("NoCustomerID", CustomerID);
                        objPrm[3] = cmdi.Parameters.Add("NoCategoryID", CategoryID);
                        objPrm[4] = cmdi.Parameters.Add("NoItemID", ItemID); 
                        objPrm[5] = cmdi.Parameters.Add("TextItemWtWb", TextItemWtWb.Text);
                        objPrm[6] = cmdi.Parameters.Add("TextItemWeightWp", ItemWeight);
                        objPrm[7] = cmdi.Parameters.Add("TextItemRateWp", ItemRate);
                        objPrm[8] = cmdi.Parameters.Add("TextItemAmount", ItemAmount);
                        objPrm[9] = cmdi.Parameters.Add("NoVatID", VatID);
                        objPrm[10] = cmdi.Parameters.Add("NoVatPercent", VatPercent);
                        objPrm[11] = cmdi.Parameters.Add("NoVatAmount", VatAmount);
                        objPrm[12] = cmdi.Parameters.Add("TextRemarks", Remarks);
                        objPrm[13] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                        objPrm[14] = cmdi.Parameters.Add("c_date", c_date);
                        objPrm[15] = cmdi.Parameters.Add("NoCuserID", userID);
                        objPrm[16] = cmdi.Parameters.Add("TextIsActive", ISActive); 
                        
                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();


                        conn.Close();

                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Insert New Sales Successfully"));
                        alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                        clearText();
                        Display();
                        DisplaySummary();
                    }
                    else
                    {

                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory" + ItemID));
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

        [WebMethod]
        public static Boolean MsSalesInvoiceNoCheck(string InvoiceNo)
        {
            Boolean result = false;
            string query = "select INVOICE_NO from MS_SALES_MASTER where INVOICE_NO = '" + InvoiceNo + "'";
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
                string HtmlString = "";
                string InvoiceNo = TextInvoiceMsNo.Text;
                string makeSQL = " SELECT PPM.SALES_ID, PPM.INVOICE_NO, PP.PARTY_ID, PP.PARTY_NAME, PP.PARTY_ARABIC_NAME, PP.PARTY_ADD_1 || ', ' || PP.PARTY_ADD_2 AS PARTY_ADD, PP.PARTY_VAT_NO, PI.ITEM_CODE, PI.ITEM_NAME, PI.ITEM_ARABIC_NAME, PPM.ITEM_WEIGHT, nvl(PPM.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, PPM.ITEM_RATE, ROUND(PPM.ITEM_AMOUNT, 2) AS ITEM_AMOUNT, nvl(PPM.VAT_AMOUNT, 0) AS VAT_AMOUNT, TO_CHAR(PPM.ENTRY_DATE, 'dd-MON-yyyy') AS ENTRY_DATE, HE.EMP_LNAME FROM MS_SALES_MASTER PPM LEFT JOIN MS_PARTY PP ON PP.PARTY_ID = PPM.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN MS_WB_OPERATOR PWO ON PWO.IS_ACTIVE = 'Enable' LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PWO.EMP_ID WHERE PPM.INVOICE_NO = '" + InvoiceNo + "' ORDER BY PI.ITEM_ID ";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                for (int i = 0; i < 1; i++)
                {
                    PartyArabicName = dt.Rows[i]["PARTY_ARABIC_NAME"].ToString();
                    PartyID = dt.Rows[i]["PARTY_ID"].ToString();
                    PartyName = dt.Rows[i]["PARTY_NAME"].ToString();
                    string PartyAdd = dt.Rows[i]["PARTY_ADD"].ToString();
                    string PartyVatNo = dt.Rows[i]["PARTY_VAT_NO"].ToString();
                    string ItemCode = dt.Rows[i]["ITEM_CODE"].ToString();
                    string ItemName = dt.Rows[i]["ITEM_NAME"].ToString();
                    string ItemArabicName = dt.Rows[i]["ITEM_ARABIC_NAME"].ToString();
                    string EmpWbLname = dt.Rows[i]["EMP_LNAME"].ToString();
                    EntryDateSlip = dt.Rows[i]["ENTRY_DATE"].ToString();

                    HtmlString += "<div style='float:left;width:785px;height:535px;margin-top:0px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";
                    HtmlString += "<div style='float:left;width:685px;height:213px;margin:65px 0 0 111px;font-family: Arial Narrow, Courier, Lucida Sans Typewriter;font-size: 12px;'></div> ";
                    HtmlString += "<div style='float:left;width:380px;'> ";
                    HtmlString += "<div style='float:left;width:285px;height:102px;margin-left:110px;padding-top:10px;text-align:center;' >"+ PartyID +":" + PartyName  + "</br><font size='1px'>" + PartyArabicName + "</font></div> ";
                    HtmlString += "<div style='float:left;width:210px;height:33px; margin:0 0 0 90px;'>" + PartyAdd + "</div> <div style='float:left;width:210px; margin:0 0 0 90px;'>" + PartyVatNo + " </div> ";
                    HtmlString += "</div> ";
                }
                int m = 1;
                HtmlString += "<div style='float:left;width:380px;'> ";
                HtmlString += "<div style='float:left;width:240px;height:42px;margin:30px 0 0 140px;'> ";
                for (int i = 0; i < RowCount; i++)
                {
                    string ItemName = dt.Rows[i]["ITEM_NAME"].ToString();
                    //   string ItemArabicName = dt.Rows[i]["ITEM_ARABIC_NAME"].ToString(); 
                    if (m == RowCount)
                    {
                        HtmlString += "" + ItemName + "";
                    }
                    else { HtmlString += "" + ItemName + ","; }
                    m++;
                }

                HtmlString += "</div>";

                for (int i = 0; i < 1; i++)
                {
                    string EmpWbLname = dt.Rows[i]["EMP_LNAME"].ToString();
                    HtmlString += "<div style='float:left;width:380px;'> ";
                    HtmlString += "<div style='float:left;width:200px;height:76px;margin:0 0 0 175px;'>" + EmpWbLname + "</div><div style='float:left;width:200px;margin:0 0 0 180px;'>" + EntryDateSlip + "</div> ";
                    HtmlString += "</div>";
                    HtmlString += "</div>";
                }
                HtmlString += "</div>";
                HtmlString += "<div style='float:left;width:785px;height:158px;margin:0 0 0 10px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";

                for (int i = 0; i < RowCount; i++)
                {

                    string ItemCode = dt.Rows[i]["ITEM_CODE"].ToString();
                    string ItemName = dt.Rows[i]["ITEM_NAME"].ToString();
                    string ItemArabicName = dt.Rows[i]["ITEM_ARABIC_NAME"].ToString();

                    double ItemWtWb = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString()) * 1000;
                    ItemWtWbTotal += Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString()) * 1000;
                    double VarianceWT = (Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString()) - Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString())) * 1000;
                    double ItemWt = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString()) * 1000;
                    double ItemRate = Convert.ToDouble(dt.Rows[i]["ITEM_RATE"].ToString()) / 1000;
                    double ItemAmt = Convert.ToDouble(decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00"));
                    ItemAmtTotal += Convert.ToDouble(decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00"));
                    ItemVatAmt += Convert.ToDouble(decimal.Parse(dt.Rows[i]["VAT_AMOUNT"].ToString()).ToString(".00"));
                    double TotalInvoiceAmt = +Convert.ToDouble(decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00"));

                    HtmlString += "<div style='float:left;width:242px;' >" + ItemCode + "     " + ItemName + "-<font size='1px'>" + ItemArabicName + "</font></div> ";
                    HtmlString += "<div style='float:left;width:82px;text-align:center;'>" + string.Format("{0:n0}", ItemWtWb) + " </div> ";
                    HtmlString += "<div style='float:left;width:93px;text-align:center;'>" + string.Format("{0:n0}", VarianceWT) + " </div> ";
                    HtmlString += "<div style='float:left;width:112px;text-align:right;'>" + string.Format("{0:n0}", ItemWt) + " </div> ";
                    HtmlString += "<div style='float:left;width:107px;text-align:right;'>" + ItemRate.ToString("0.000") + " </div> ";
                    HtmlString += "<div style='float:left;width:120px;text-align:right;height:25px'>" + string.Format("{0:n2}", ItemAmt) + " </div> ";

                }
                HtmlString += "</div>";
                HtmlString += "<div style='float:left;width:785px;height:238px;margin:0 0 0 8px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'><div style='float:left;width:242px;border:white solid 1px' ></div>";
                HtmlString += "<div style='float:left;width:82px;text-align:center;margin:0 0 10px 0;'>" + string.Format("{0:n0}", ItemWtWbTotal) + " </div> ";
                HtmlString += "<div style='float:left;width:431px;text-align:right;margin:0 0 10px 0;'>" + string.Format("{0:n2}", ItemAmtTotal) + " </div> ";
                HtmlString += "<div style='float:left;width:757px;text-align:right;margin:0 0 10px 0;'>" + string.Format("{0:n2}", ItemVatAmt) + " </div> ";
                HtmlString += "<div style='float:left;width:757px;text-align:right;'>" + string.Format("{0:n2}", ItemAmtTotal + ItemVatAmt) + " </div> ";
                HtmlString += "<div style='float:left;width:500px;margin:0 0 0 120px;text-align:left;'>" + EntryDateSlip + " </div> ";
                string NumberToInWord = NumberToInWords.DecimalToWordsSR(Convert.ToDecimal(ItemAmtTotal + ItemVatAmt)).Trim().ToUpper();
                //   string NumberToInWord = NumberToWords(ItemAmtTotal + ItemVatAmt).Trim().ToUpper();
                HtmlString += "<div style = 'float:left;width:290px;height:88px;margin:40px 0 0 460px;padding:10px;text-align:left;'>" + NumberToInWord + " </div> ";
                HtmlString += "<div style = 'float:left;width:200px;margin:0 0 0 555px;text-align:left;'><font size='1px'>" + PartyName + "|" + PartyArabicName + "</font> </div> ";
                HtmlString += "</div>";
                HtmlString += "</div>";
                HtmlString += "</div>";

                // sales master update for print
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string update_user = "update  MS_SALES_MASTER  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where INVOICE_NO = :NoSlipNo ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[4];
                objPrm[0] = cmdi.Parameters.Add("TextIsPrint", "Printed");
                objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[2] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[3] = cmdi.Parameters.Add("NoSlipNo", InvoiceNo);

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
         try{
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
            string USER_DATA_ID = Session["user_data_id"].ToString();
                 
            DataTable dtUserTypeID = new DataTable();
            DataSet ds = new DataSet();
            string makeSQL = " select SALES_ID, INVOICE_NO, PARTY_ID, ITEM_ID, CATEGORY_ID, ITEM_WEIGHT, ITEM_RATE, ITEM_AMOUNT, VAT_ID, VAT_PERCENT, VAT_AMOUNT, (ITEM_AMOUNT+VAT_AMOUNT) AS TOTAL_AMOUNT,  nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, REMARKS, TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, CREATE_DATE, UPDATE_DATE, IS_ACTIVE from MS_SALES_MASTER where INVOICE_NO  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                DataTable dtItemID = new DataTable();
                DataSet dsi = new DataSet();
                string makeDropDownItemSQL = " SELECT ITEM_ID, ITEM_NAME || ' - ' || ITEM_CODE AS ITEM_NAME FROM MF_ITEM WHERE CATEGORY_ID = '" + Convert.ToInt32(dt.Rows[i]["CATEGORY_ID"].ToString()) + "' AND IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                dsi = ExecuteBySqlString(makeDropDownItemSQL);
                dtItemID = (DataTable)dsi.Tables[0];
                DropDownItemID.DataSource = dtItemID;
                DropDownItemID.DataValueField = "ITEM_ID";
                DropDownItemID.DataTextField = "ITEM_NAME";
                DropDownItemID.DataBind();

                TextInvoiceMsNo.Text          = dt.Rows[i]["INVOICE_NO"].ToString();
                DropDownCustomerID.Text     = dt.Rows[i]["PARTY_ID"].ToString();  
                DropDownItemID.Text         = dt.Rows[i]["ITEM_ID"].ToString(); 
                DropDownCategoryID.Text      = dt.Rows[i]["CATEGORY_ID"].ToString();
                TextItemRateWp.Text           = dt.Rows[i]["ITEM_RATE"].ToString();
                TextItemWeightWp.Text         = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT"].ToString()).ToString("0.00");
                TextItemAmount.Text         = decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString("0.00"); 
                DropDownVatID.Text          = dt.Rows[i]["VAT_ID"].ToString();
                TextVatAmount.Text          = decimal.Parse(dt.Rows[i]["VAT_AMOUNT"].ToString()).ToString("0.00");
                TextTotalAmount.Text        = decimal.Parse(dt.Rows[i]["TOTAL_AMOUNT"].ToString()).ToString("0.00");
                TextRemarks.Text            = dt.Rows[i]["REMARKS"].ToString();
                EntryDate.Text              = dt.Rows[i]["ENTRY_DATE"].ToString(); 
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                TextItemWtWb.Text         = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString()).ToString("0.00");
            }

            conn.Close();
            Display();
            CheckItemWeight.Text = "";
            CheckInvoiceNo.Text = "";
            TextInvoiceMsNo.Enabled = false;
            DropDownCustomerID.Attributes.Remove("disabled");
            alert_box.Visible = false;
         //   DropDownItemID.Attributes.Add("readonly", "readonly");
         //   DropDownCategoryID.Enabled = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
            BtnUpdate.Attributes.Add("aria-disabled", "true");
            BtnUpdate.Attributes.Add("class", "btn btn-success active");
            BtnDelete.Attributes.Add("aria-disabled", "true");
            BtnDelete.Attributes.Add("class", "btn btn-danger active");
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
                string MonthYear = System.DateTime.Now.ToString("MM/yyyy");
                if (txtSearchEmp.Text == "")
                {
                    makeSQL = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PP.PARTY_NAME, PI.ITEM_CODE  || ' : ' || PI.ITEM_NAME AS ITEM_NAME, PSI.CATEGORY_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, (PSM.ITEM_AMOUNT + PSM.VAT_AMOUNT) AS TOTAL_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE,  PSM.IS_PRINT, TO_CHAR(PSM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE  FROM MS_SALES_MASTER PSM LEFT JOIN MS_PARTY PP ON PP.PARTY_ID = PSM.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN MF_CATEGORY PSI ON PSI.CATEGORY_ID = PSM.CATEGORY_ID  WHERE  to_char(PSM.ENTRY_DATE, 'mm/yyyy') = '" + MonthYear + "' AND PSM.IS_SALES_RETURN IS NULL ORDER BY PSM.CREATE_DATE DESC";
                }
                else
                {
                    if (DropDownItemID1.Text == "0")
                    {
                        makeSQL = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PP.PARTY_NAME, PI.ITEM_CODE  || ' : ' || PI.ITEM_NAME AS ITEM_NAME, PSI.CATEGORY_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, (PSM.ITEM_AMOUNT + PSM.VAT_AMOUNT) AS TOTAL_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE,  PSM.IS_PRINT, TO_CHAR(PSM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE  FROM MS_SALES_MASTER PSM LEFT JOIN MS_PARTY PP ON PP.PARTY_ID = PSM.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN MF_CATEGORY PSI ON PSI.CATEGORY_ID = PSM.CATEGORY_ID  WHERE PSM.IS_SALES_RETURN IS NULL  AND (PSM.INVOICE_NO like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PSI.CATEGORY_NAME like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'mm/yyyy') like '" + txtSearchEmp.Text + "%' or PSM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ) ORDER BY PSM.CREATE_DATE desc, PSM.UPDATE_DATE desc";
                    }
                    else
                    {
                        makeSQL = " SELECT PSM.SALES_ID, PSM.INVOICE_NO, PP.PARTY_NAME, PI.ITEM_CODE  || ' : ' || PI.ITEM_NAME AS ITEM_NAME, PSI.CATEGORY_NAME, PSM.ITEM_WEIGHT, PSM.ITEM_RATE, PSM.ITEM_AMOUNT, PSM.VAT_ID, PSM.VAT_PERCENT, PSM.VAT_AMOUNT, (PSM.ITEM_AMOUNT + PSM.VAT_AMOUNT) AS TOTAL_AMOUNT, PSM.REMARKS, PSM.ENTRY_DATE, PSM.CREATE_DATE, PSM.UPDATE_DATE, PSM.IS_ACTIVE,  PSM.IS_PRINT, TO_CHAR(PSM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE  FROM MS_SALES_MASTER PSM LEFT JOIN MS_PARTY PP ON PP.PARTY_ID = PSM.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PSM.ITEM_ID LEFT JOIN MF_CATEGORY PSI ON PSI.CATEGORY_ID = PSM.CATEGORY_ID  WHERE PSM.IS_SALES_RETURN IS NULL AND PI.ITEM_ID like '" + DropDownItemID1.Text + "%'  AND (PSM.INVOICE_NO like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PSI.CATEGORY_NAME like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or TO_CHAR(TO_DATE(PSM.ENTRY_DATE),'mm/yyyy') like '" + txtSearchEmp.Text + "%' or PSM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ) ORDER BY PSM.CREATE_DATE desc, PSM.UPDATE_DATE desc";

                     }
                    alert_box.Visible = false;
               }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "INVOICE_NO" };
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
                    makeSQL = "  SELECT PI.ITEM_CODE  || ' : ' || PI.ITEM_NAME AS ITEM_NAME, count(PSM.SALES_ID) AS SALES_ID, sum(PSM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PSM.ITEM_AMOUNT) AS ITEM_AMOUNT,  sum(nvl(PSM.VAT_AMOUNT,0)) AS VAT_AMOUNT,  sum(PSM.ITEM_AMOUNT)+ sum(nvl(PSM.VAT_AMOUNT,0)) AS TOTAL_AMOUNT FROM MF_ITEM PI LEFT JOIN MS_SALES_MASTER PSM ON PSM.ITEM_ID = PI.ITEM_ID WHERE to_char(PSM.ENTRY_DATE, 'mm/yyyy') =  '" + MonthYear + "' GROUP BY PI.ITEM_CODE  || ' : ' || PI.ITEM_NAME ORDER BY PI.ITEM_CODE  || ' : ' || PI.ITEM_NAME ";
                }
                else
                { 
                    makeSQL = "  SELECT PI.ITEM_CODE  || ' : ' || PI.ITEM_NAME AS ITEM_NAME, count(PSM.SALES_ID) AS SALES_ID, sum(PSM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(PSM.ITEM_AMOUNT) AS ITEM_AMOUNT,  sum(PSM.VAT_AMOUNT) AS VAT_AMOUNT,  sum(PSM.ITEM_AMOUNT)+ sum(PSM.VAT_AMOUNT) AS TOTAL_AMOUNT FROM MF_ITEM PI LEFT JOIN MS_SALES_MASTER PSM ON PSM.ITEM_ID = PI.ITEM_ID WHERE to_char(PSM.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear4.Text + "' GROUP BY PI.ITEM_CODE  || ' : ' || PI.ITEM_NAME ORDER BY PI.ITEM_CODE  || ' : ' || PI.ITEM_NAME ";
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
                    GridView2.HeaderRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                    //Calculate Sum and display in Footer Row 
                    GridView2.FooterRow.Cells[0].Font.Bold = true;
                    GridView2.FooterRow.Cells[0].Text = "Grand Total";
                    GridView2.FooterRow.Cells[0].HorizontalAlign = HorizontalAlign.Right;

                    decimal total_prod = dt.AsEnumerable().Sum(row => row.Field<decimal>("SALES_ID"));
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

                    decimal total_ag_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("VAT_AMOUNT"));
                    GridView2.FooterRow.Cells[4].Font.Bold = true;
                    GridView2.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[4].Text = total_ag_wt.ToString("N2");

                    decimal total_grand_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("TOTAL_AMOUNT"));
                    GridView2.FooterRow.Cells[5].Font.Bold = true;
                    GridView2.FooterRow.Cells[5].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[5].Text = total_grand_wt.ToString("N2");
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
         try {

                if (IS_EDIT_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    string InvoiceNo = TextInvoiceMsNo.Text;
                    int CustomerID = Convert.ToInt32(DropDownCustomerID.Text);
                    int ItemID = Convert.ToInt32(Request.Form[DropDownItemID.UniqueID]);
                    double ItemRate = Convert.ToDouble(TextItemRateWp.Text);
                    double ItemWeight = Convert.ToDouble(TextItemWeightWp.Text);
                    double ItemAmount = (ItemRate * ItemWeight)/1000;
                    int VatID = Convert.ToInt32(DropDownVatID.Text);
                    double VatPercent = Convert.ToDouble(DropDownVatID.SelectedItem.Text);
                    double VatAmount = (ItemAmount * VatPercent) / 100;
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
                    string makeSQLPro = " select ITEM_ID, ITEM_WEIGHT from MS_SALES_MASTER where INVOICE_NO  = '" + InvoiceNo + "'  ";
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
                    string makeSQL2 = " select * from MS_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";   
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
                            string update_inven_mas = "update  MS_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";  
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
                        string makeSQL3 = " select * from MS_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
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
                        string update_inven_rm_new = "update  MS_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
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
                        string update_sales = "update MS_SALES_MASTER set  PARTY_ID = :NoCustomerID, ITEM_ID = :NoItemID, CATEGORY_ID = :NoCategoryID, ITEM_WEIGHT_WB = :NoItemWeightWB, ITEM_WEIGHT = :NoItemWeight, ITEM_RATE = :NoItemRate, ITEM_AMOUNT = :NoItemAmount, VAT_ID = :NoVatID, VAT_PERCENT = :NoVatPercent, VAT_AMOUNT = :NoVatAmount, REMARKS = :TextRemarks, ENTRY_DATE = TO_DATE(:TextEntryDate, 'DD-MM-YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where INVOICE_NO = :NoInvoiceNo ";
                        cmdu = new OracleCommand(update_sales, conn);

                        OracleParameter[] objPrm = new OracleParameter[17];
                        objPrm[1] = cmdu.Parameters.Add("NoCustomerID", CustomerID);
                        objPrm[2] = cmdu.Parameters.Add("NoItemID", ItemID);
                        objPrm[3] = cmdu.Parameters.Add("NoCategoryID", CategoryID);
                        objPrm[4] = cmdu.Parameters.Add("NoItemWeightWB", TextItemWtWb.Text);
                        objPrm[5] = cmdu.Parameters.Add("NoItemWeight", ItemWeight);
                        objPrm[6] = cmdu.Parameters.Add("NoItemRate", ItemRate);
                        objPrm[7] = cmdu.Parameters.Add("NoItemAmount", ItemAmount);
                        objPrm[8] = cmdu.Parameters.Add("NoVatID", VatID);
                        objPrm[9] = cmdu.Parameters.Add("NoVatPercent", VatPercent);
                        objPrm[10] = cmdu.Parameters.Add("NoVatAmount", VatAmount);
                        objPrm[11] = cmdu.Parameters.Add("TextRemarks", Remarks);
                        objPrm[12] = cmdu.Parameters.Add("TextEntryDate", EntryDateNew);
                        objPrm[13] = cmdu.Parameters.Add("u_date", u_date);
                        objPrm[14] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrm[15] = cmdu.Parameters.Add("TextIsActive", ISActive);
                        objPrm[16] = cmdu.Parameters.Add("NoInvoiceNo", InvoiceNo);

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();

                        conn.Close();

                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Sales (Local) Data Update Successfully"));
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
               }
              catch
              {
                Response.Redirect("~/ParameterError.aspx");
             } 
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
                    string Sales_ID = TextInvoiceMsNo.Text;
                    int ItemID = Convert.ToInt32(Request.Form[DropDownItemID.UniqueID]);
                    //    int SubItemID = Convert.ToInt32(DropDownCategoryID.Text);  
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    // check production data
                    int ItemIdOld = 0, SubItemIdOld = 0; double ItemWeightOld = 0.00;
                    string makeSQLPro = " select ITEM_ID, ITEM_WEIGHT from MS_SALES_MASTER where INVOICE_NO  = '" + Sales_ID + "'  ";
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
                        string makeSQL2 = " select * from MS_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
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
                            string update_inven_mas = "update  MS_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
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
                        string delete_production = " delete from MS_SALES_MASTER where INVOICE_NO  = '" + Sales_ID + "'";
                        cmdi = new OracleCommand(delete_production, conn);
                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();

                        conn.Close();
                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Sales (Local) Data Delete Successfully"));
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
            TextInvoiceMsNo.Text = "";   
            TextItemWeightWp.Text = "";
            TextItemRateWp.Text = "";
            TextItemAmount.Text = "";
            TextVatAmount.Text = "";
            TextTotalAmount.Text = "";
            DropDownCategoryID.Text = "0"; 
            CheckItemWeight.Text = ""; 
            DropDownCustomerID.Text = "0";  
         //   DropDownItemID.Text = "0";
            CheckEntryDate.Text = "";
            TextItemWtWb.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        {
            TextInvoiceMsNo.Text = ""; 
            TextItemWeightWp.Text = "";
            TextItemRateWp.Text = ""; 
            TextItemAmount.Text = "";
            TextVatAmount.Text = "";
            TextTotalAmount.Text = "";
            DropDownCategoryID.Text = "0"; 
            CheckItemWeight.Text = ""; 
            DropDownCustomerID.Text = "0";  
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