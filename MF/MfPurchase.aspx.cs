using System; 
using System.Configuration;
using System.Data;
using System.Linq; 
using System.Web.UI; 
using System.Web.UI.WebControls; 
using System.Data.OracleClient;
using System.IO;  
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using System.Web.Services;

namespace NRCAPPS.MF
{
    public partial class MfPurchase : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl, cmdsp;
        OracleDataAdapter oradata;
        DataTable dt;
        int RowCount;
        double ItemVatAmt = 0.0, ItemAmtTotal = 0.0, ItemWtWbTotal = 0.0, TotalInvoiceAmt = 0.0; string EntryDateSlip = "", PartyArabicName = "", PartyName = "";

        string IS_PAGE_ACTIVE = "", IS_ADD_ACTIVE = "", IS_EDIT_ACTIVE = "", IS_DELETE_ACTIVE = "", IS_VIEW_ACTIVE = "", IS_REPORT_ACTIVE = "", IS_PRINT_ACTIVE = "";
        string HtmlString = "";
        public bool IsLoad { get; set; }
        public bool IsLoad2 { get; set; }
        public bool IsLoad3 { get; set; }
        public bool IsLoad4 { get; set; }
        public bool IsLoad6 { get; set; }
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
                        DataTable dtSupplierID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeSupplierSQL = " SELECT PARTY_ID,  PARTY_ID || ' - ' || PARTY_NAME || ' - ' || PARTY_VAT_NO AS PARTY_NAME  FROM MF_PARTY WHERE IS_ACTIVE = 'Enable' AND IS_PURCHASE_ACTIVE = 'Enable' ORDER BY PARTY_NAME ASC";
                        ds = ExecuteBySqlString(makeSupplierSQL);
                        dtSupplierID = (DataTable)ds.Tables[0];
                        DropDownSupplierID.DataSource = dtSupplierID;
                        DropDownSupplierID.DataValueField = "PARTY_ID";
                        DropDownSupplierID.DataTextField = "PARTY_NAME";
                        DropDownSupplierID.DataBind();
                        DropDownSupplierID.Items.Insert(0, new ListItem("Select  Supplier", "0"));

                        DropDownSupplierID2.DataSource = dtSupplierID;
                        DropDownSupplierID2.DataValueField = "PARTY_ID";
                        DropDownSupplierID2.DataTextField = "PARTY_NAME";
                        DropDownSupplierID2.DataBind();
                        DropDownSupplierID2.Items.Insert(0, new ListItem("Select  Supplier", "0"));
                          
                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeDropDownItemSQL = " SELECT ITEM_ID, ITEM_NAME || ' - ' || ITEM_CODE AS ITEM_NAME FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' AND IS_PURCHASE_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        dsi = ExecuteBySqlString(makeDropDownItemSQL);
                        dtItemID = (DataTable)dsi.Tables[0];
                        DropDownItemList.DataSource = dtItemID;
                        DropDownItemList.DataValueField = "ITEM_ID";
                        DropDownItemList.DataTextField = "ITEM_NAME";
                        DropDownItemList.DataBind();
                        DropDownItemList.Items.Insert(0, new ListItem("All Item", "0"));  


                        DropDownSearchItemID.DataSource = dtItemID;
                        DropDownSearchItemID.DataValueField = "ITEM_ID";
                        DropDownSearchItemID.DataTextField = "ITEM_NAME";
                        DropDownSearchItemID.DataBind();
                        DropDownSearchItemID.Items.Insert(0, new ListItem("All Item", "0"));

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
                          
                        TextMfSlipNo.Focus();
                        //  VatPercent.Visible = false;
                        TextItemAmountWP.Attributes.Add("readonly", "readonly");
                        TextTotalAmountWP.Attributes.Add("readonly", "readonly");
                        TextItemVatAmountWP.Enabled = false;
                        DropDownItemID.Enabled = false;
                        TextItemWtWbWP.Enabled = false;
                        TextItemWeightWP.Enabled = false;
                        TextItemRateWP.Enabled = false;
                        RadioBtnVatWp.Enabled = false;

                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled");

                        //  btnPrint.Enabled = false;
                        Display();
                        DisplaySummary();
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

        public void BtnAdd_Click(object sender, EventArgs e)
        {
            //  try
            //     { 
            if (IS_ADD_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                int userID = Convert.ToInt32(Session["USER_ID"]);
                int SupplierID = Convert.ToInt32(DropDownSupplierID.Text);
                int SlipNoWp = Convert.ToInt32(TextMfSlipNo.Text);
                int CategoryID = Convert.ToInt32(DropDownCategoryID.Text);

                //   int ItemID   = Convert.ToInt32(DropDownItemID.Text);

                int ItemID = Convert.ToInt32(Request.Form[DropDownItemID.UniqueID]);

                //     string ItemName = DropDownItemID.SelectedItem.Text;

                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                string[] MakeEntryDateSplit = EntryDate.Text.Split('-');
                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                double ItemRate = Convert.ToDouble(TextItemRateWP.Text.Trim());
                double ItemWtWbWP = Convert.ToDouble(TextItemWtWbWP.Text.Trim());
                double ItemWeight = Convert.ToDouble(TextItemWeightWP.Text.Trim());
                double ItemAmount = Convert.ToDouble(TextItemAmountWP.Text.Trim());
                double TotalAmountWP = 0.00;  

                int VatID = 0; double VatPercent = 0.00, VatAmount = 0.00;
                if (RadioBtnVatWp.SelectedValue == "VatYes")
                {
                    VatID = Convert.ToInt32(DropDownVatID.Text);
                    VatPercent = Convert.ToDouble(DropDownVatID.SelectedItem.Text);
                    VatAmount = (ItemAmount * Convert.ToDouble(DropDownVatID.SelectedItem.Text)) / 100;
                    TotalAmountWP = Math.Round(ItemAmount + VatAmount);
                }
                else
                {
                    TotalAmountWP = Math.Round(ItemAmount);
                }


                string get_user_purchase_id = "select MF_PURCHASE_MASTERID_SEQ.nextval from dual";
                cmdsp = new OracleCommand(get_user_purchase_id, conn);
                int newPurchaseID = Int32.Parse(cmdsp.ExecuteScalar().ToString());
                string insert_purchase = "insert into  MF_PURCHASE_MASTER (PURCHASE_ID, SLIP_NO, PARTY_ID, CATEGORY_ID, ITEM_ID, ITEM_WEIGHT_WB, ITEM_WEIGHT, ITEM_RATE, ITEM_AMOUNT, VAT_ID, VAT_PERCENT, VAT_AMOUNT, TOTAL_AMOUNT, ENTRY_DATE, REMARKS, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID) values  ( :NoPurchaseID, :NoSlipID, :NoSupplierID, :NoCategoryID, :NoItemID,  :TextItemWtWbWP, :TextItemWeightWP, :TextItemRateWP, :TextItemAmountWP, :NoVatID, :NoVatPercent, :NoVatAmount, :TextTotalAmountWP, TO_DATE(:EntryDate, 'DD/MM/YYYY'), :TextRemarks, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 5)";
                cmdi = new OracleCommand(insert_purchase, conn);

                OracleParameter[] objPrm = new OracleParameter[18];
                objPrm[0] = cmdi.Parameters.Add("NoPurchaseID", newPurchaseID);
                objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNoWp);
                objPrm[2] = cmdi.Parameters.Add("NoSupplierID", SupplierID);
                objPrm[3] = cmdi.Parameters.Add("NoCategoryID", CategoryID);
                objPrm[4] = cmdi.Parameters.Add("NoItemID", ItemID);
                objPrm[5] = cmdi.Parameters.Add("TextItemWtWbWP", ItemWtWbWP);
                objPrm[6] = cmdi.Parameters.Add("TextItemWeightWP", ItemWeight);
                objPrm[7] = cmdi.Parameters.Add("TextItemRateWP", ItemRate);
                objPrm[8] = cmdi.Parameters.Add("TextItemAmountWP", ItemAmount);
                objPrm[9] = cmdi.Parameters.Add("TextTotalAmountWP", TotalAmountWP);
                objPrm[10] = cmdi.Parameters.Add("NoVatID", VatID);
                objPrm[11] = cmdi.Parameters.Add("NoVatPercent", VatPercent);
                objPrm[12] = cmdi.Parameters.Add("NoVatAmount", VatAmount); 
                objPrm[13] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                objPrm[14] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                objPrm[15] = cmdi.Parameters.Add("c_date", c_date);
                objPrm[16] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[17] = cmdi.Parameters.Add("TextIsActive", ISActive);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                 
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Insert New Purchase Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                clearText();
                TextMfSlipNo.Focus();
                Display();
                DisplaySummary();

            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
            //  }
            //     catch
            //    {
            //     Response.Redirect("~/ParameterError.aspx");
            //    } 
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            if (IS_PRINT_ACTIVE == "Enable")
            {
                alert_box.Visible = false;
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string SlipNoWp = "";
                if (TextMfSlipNo.Text != "")
                {
                    SlipNoWp = TextMfSlipNo.Text;
                }
                else
                {
                    LinkButton btn = (LinkButton)sender;
                    Session["user_data_id"] = btn.CommandArgument;
                    SlipNoWp = Session["user_data_id"].ToString();
                }


                string makeSQL = " SELECT WPM.PURCHASE_ID, WPM.SLIP_NO, WP.PARTY_NAME, WP.PARTY_ARABIC_NAME, WP.PARTY_ADD_1 || ', ' || WP.PARTY_ADD_2 AS PARTY_ADD, WP.PARTY_VAT_NO, WP.CONTACT_NO, PI.ITEM_CODE, PI.ITEM_NAME, PI.ITEM_ARABIC_NAME, WPM.ITEM_WEIGHT,  nvl(WPM.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, WPM.ITEM_RATE, nvl(WPM.ITEM_AMOUNT,0) AS ITEM_AMOUNT, nvl(WPM.VAT_AMOUNT, 0) AS VAT_AMOUNT, nvl(WPM.TOTAL_AMOUNT, 0) AS TOTAL_AMOUNT, TO_CHAR(WPM.ENTRY_DATE, 'dd-MON-yyyy') AS ENTRY_DATE FROM MF_PURCHASE_MASTER WPM LEFT JOIN MF_PARTY WP ON WP.PARTY_ID = WPM.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = WPM.ITEM_ID WHERE WPM.SLIP_NO = '" + SlipNoWp + "' ORDER BY PI.ITEM_ID";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                HtmlString += "<div style='float:left;width:785px;height:258px;margin-top:200px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";
                HtmlString += "<div style='float:left;width:380px;height:360px;'> ";
                HtmlString += "<div style='float:left;width:260px;height:102px;margin-left:100px;padding-top:10px;text-align:center;' ></div> ";
                HtmlString += "<div style='float:left;width:210px;height:33px; margin:0 0 0 90px;'></div> <div style='float:left;width:210px; margin:0 0 0 90px;'></div> ";
                HtmlString += "</div> ";

                int m = 1;
                HtmlString += "<div style='float:left;width:380px;height:270px;'> ";
                HtmlString += "<div style='float:left;width:240px;height:42px;margin:0 0 0 140px;'> ";

                for (int i = 0; i < 1; i++)
                {
                    PartyArabicName = dt.Rows[i]["PARTY_ARABIC_NAME"].ToString();
                    PartyName = dt.Rows[i]["PARTY_NAME"].ToString();
                    string PartyAdd = dt.Rows[i]["PARTY_ADD"].ToString();
                    string PartyVatNo = dt.Rows[i]["PARTY_VAT_NO"].ToString();
                    EntryDateSlip = dt.Rows[i]["ENTRY_DATE"].ToString();

                    HtmlString += "<div style='float:left;width:380px;height:280px;'> ";
                    HtmlString += "<div style='float:left;width:310px;height:42px;margin-left:20px;padding-top:31px;text-align:left;' > " + PartyName + "</font></div> ";
                    HtmlString += "<div style='float:left;width:310px;height:45px; margin:0 0 0 20px;'>" + PartyArabicName + "</div> <div style='float:left;width:210px;height:60px; margin:0 0 0 17px;'>" + PartyVatNo + " </div>  ";

                    HtmlString += "<div style='float:left;width:380px;'> ";
                    HtmlString += "<div style='float:left;width:200px;margin:60px 0 0 67px;'>" + EntryDateSlip + "</div> ";
                    HtmlString += "</div>";
                    HtmlString += "</div>";
                    HtmlString += "</div>";
                }
                HtmlString += "</div>";
                HtmlString += "<div style='float:left;width:840px;height:220px;margin:0 0 0 15px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 15px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";

                for (int i = 0; i < RowCount; i++)
                {

                    string ItemCode = dt.Rows[i]["ITEM_CODE"].ToString();
                    string ItemName = dt.Rows[i]["ITEM_NAME"].ToString();
                    string ItemArabicName = dt.Rows[i]["ITEM_ARABIC_NAME"].ToString();

                    double ItemWtWb = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString());
                    ItemWtWbTotal += Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString());
                    double VarianceWT = (Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString()) - Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString()));
                    double ItemWt = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());
                    double ItemRate = Convert.ToDouble(dt.Rows[i]["ITEM_RATE"].ToString()) / 1000;
                    double ItemAmt = Convert.ToDouble(decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00"));
                    ItemAmtTotal += Convert.ToDouble(decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00"));
                    ItemVatAmt += Convert.ToDouble(decimal.Parse(dt.Rows[i]["VAT_AMOUNT"].ToString()).ToString(".00"));
                    TotalInvoiceAmt = +Convert.ToDouble(decimal.Parse(dt.Rows[i]["TOTAL_AMOUNT"].ToString()).ToString(".00"));

                    HtmlString += "<div style='float:left;width:60px;height:28px;padding-top:10px;' >" + ItemCode + "</div> ";
                    HtmlString += "<div style='float:left;width:244px;height:28px;padding-top:10px;' >" + ItemName + "-<font size='1px'>" + ItemArabicName + "</font></div> ";
                    HtmlString += "<div style='float:left;width:82px;height:28px;text-align:center;padding-top:10px;'>" + string.Format("{0:n0}", ItemWtWb) + " </div> ";
                    HtmlString += "<div style='float:left;width:89px;height:28px;text-align:center;padding-top:10px;'>" + string.Format("{0:n0}", VarianceWT) + " </div> ";
                    HtmlString += "<div style='float:left;width:112px;height:28px;text-align:right;padding-top:10px;'>" + string.Format("{0:n0}", ItemWt) + " </div> ";
                    HtmlString += "<div style='float:left;width:107px;height:28px;text-align:right;padding-top:10px;'>" + ItemRate.ToString("0.000") + " </div> ";
                    HtmlString += "<div style='float:left;width:142px;height:28px;text-align:right;padding-top:10px;'>" + string.Format("{0:n2}", ItemAmt) + " </div> ";

                }
                HtmlString += "</div>";
                HtmlString += "<div style='float:left;width:840px;height:238px;margin:0 0 0 13px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 15px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'><div style='float:left;width:300px;border:white solid 1px' ></div>";
                HtmlString += "<div style='float:left;width:86px;height:25px;padding-top:8px;text-align:center;margin:0 0 10px 0;'>" + string.Format("{0:n0}", ItemWtWbTotal) + " </div> ";
                HtmlString += "<div style='float:left;width:451px;height:25px;padding-top:8px;text-align:right;margin:0 0 10px 0;'>" + string.Format("{0:n2}", ItemAmtTotal) + " </div> ";
                HtmlString += "<div style='float:left;width:840px;height:25px;padding-top:8px;text-align:right;margin:0 0 10px 0;'>" + string.Format("{0:n2}", ItemVatAmt) + " </div> ";

                HtmlString += "<div style='float:left;width:262px;height:25px;text-align:center;margin:14px 0 0 120px;'>" + EntryDateSlip + "  </div> ";
                HtmlString += "<div style='float:left;width:458px;height:25px;padding-top:5px;text-align:right;margin:0 0 10px 0;'>" + string.Format("{0:n2}", ItemAmtTotal + ItemVatAmt) + "</div> ";

                string NumberToInWord = NumberToInWords.DecimalToWordsSR(Convert.ToDecimal(ItemAmtTotal + ItemVatAmt)).Trim().ToUpper();
                HtmlString += "<div style = 'float:left;width:320px;height:90px;margin:10px 0 0 500px;padding:10px;text-align:left;'>" + NumberToInWord + " </div> ";
                HtmlString += "<div style = 'float:left;width:280px;height:60px;margin:10px 0 0 595px;text-align:left;'><font size='2px'>" + PartyArabicName + "</br>" + PartyName + "</font> </div> ";
                HtmlString += "</div>";
                HtmlString += "</div>";
                HtmlString += "</div>";

                // purchase master update for print
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string update_user = "update  MF_PURCHASE_MASTER  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where SLIP_NO = :NoSlipNoWp ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[4];
                objPrm[0] = cmdi.Parameters.Add("TextIsPrint", "Printed");
                objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[2] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[3] = cmdi.Parameters.Add("NoSlipNoWp", SlipNoWp);

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

        public void Redio_CheckedChanged(object sender, EventArgs e)
        {
            if (radPurDuplicate.SelectedValue == "One")
            {
                alert_box.Visible = false;
                TextMfSlipNo.Focus();
                CheckSlipNoWp.Visible = true;
                BtnAdd.Attributes.Add("aria-disabled", "false");
                BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
            }
            else
            {
                alert_box.Visible = false;
                TextMfSlipNo.Focus();
                CheckSlipNoWp.Visible = false;
                BtnAdd.Attributes.Add("aria-disabled", "true");
                BtnAdd.Attributes.Add("class", "btn btn-primary active");
            }
        }

        [WebMethod]
        public static Boolean WpSlipNoCheck(int WpSlipNo)
        {
            Boolean result = false;
            string query = "select SLIP_NO from MF_PURCHASE_MASTER where SLIP_NO = '" + WpSlipNo + "'";
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

        [WebMethod]
        public static List<ListItem> GetItemDataList(int ItemId)
        {
            string query = " SELECT ITEM_ID, nvl(ITEM_RATE,0) AS ITEM_RATE FROM MF_ITEM WHERE ITEM_ID =:ItemID AND IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
            string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(strConnString))
            {
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    List<ListItem> cities = new List<ListItem>();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("ItemID", ItemId);
                    cmd.Connection = conn;
                    conn.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            cities.Add(new ListItem
                            {
                                Value = sdr["ITEM_ID"].ToString(),
                                Text = sdr["ITEM_RATE"].ToString()
                            });
                        }
                    }
                    conn.Close();
                    return cities;
                }
            }
        }

        [WebMethod]
        public static List<ListItem> GetItemList(int ItemId)
        {
            string query = " SELECT ITEM_ID, ITEM_NAME || ' - ' || ITEM_CODE AS ITEM_NAME FROM MF_ITEM WHERE CATEGORY_ID = :ItemID AND IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC ";
            string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(strConnString))
            {
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    List<ListItem> cities = new List<ListItem>();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("ItemID", ItemId);
                    cmd.Connection = conn;
                    conn.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            cities.Add(new ListItem
                            {
                                Value = sdr["ITEM_ID"].ToString(),
                                Text = sdr["ITEM_NAME"].ToString()
                            });
                        }
                    }
                    conn.Close();
                    return cities;
                }
            }
        }

        protected void linkSelectClick(object sender, EventArgs e)
        {
            // try{
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
            int USER_DATA_ID = Convert.ToInt32(Session["user_data_id"]);

            string makeSQL = " select PURCHASE_ID, SLIP_NO,  PARTY_ID,  ITEM_ID, CATEGORY_ID, SUPERVISOR_ID, ITEM_WEIGHT, nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, ITEM_RATE, nvl(ITEM_AMOUNT,0) AS ITEM_AMOUNT, nvl(VAT_ID,0) AS VAT_ID, nvl(VAT_AMOUNT,0) AS VAT_AMOUNT, nvl(TOTAL_AMOUNT,0) AS TOTAL_AMOUNT, TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, CREATE_DATE,  UPDATE_DATE,  C_USER_ID, U_USER_ID, IS_ACTIVE from MF_PURCHASE_MASTER where PURCHASE_ID  = '" + USER_DATA_ID + "'";

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

                DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));
                TextPurchaseID.Text = dt.Rows[i]["PURCHASE_ID"].ToString();
                TextMfSlipNo.Text = dt.Rows[i]["SLIP_NO"].ToString();
                DropDownSupplierID.Text = dt.Rows[i]["PARTY_ID"].ToString();
                DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();
                DropDownCategoryID.Text = dt.Rows[i]["CATEGORY_ID"].ToString();
                TextItemRateWP.Text = dt.Rows[i]["ITEM_RATE"].ToString();
                TextItemWeightWP.Text = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT"].ToString()).ToString(".00");
                TextItemAmountWP.Text = decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00");
                TextTotalAmountWP.Text = decimal.Parse(dt.Rows[i]["TOTAL_AMOUNT"].ToString()).ToString(".00");
                EntryDate.Text = dt.Rows[i]["ENTRY_DATE"].ToString();
                TextItemWtWbWP.Text = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString()).ToString(".00");
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);

                RadioBtnVatWp.Enabled = true;
                if (dt.Rows[i]["VAT_ID"].ToString() != "0")
                {
                    RadioBtnVatWp.Text = "VatYes";
                    VatPercentBox.Style.Remove("display");
                    TextItemVatAmountWP.Text = decimal.Parse(dt.Rows[i]["VAT_AMOUNT"].ToString()).ToString(".00");
                    DropDownVatID.Text = dt.Rows[i]["VAT_ID"].ToString();
                }
                else
                {
                    RadioBtnVatWp.Text = "VatNo";
                    VatPercentBox.Style.Add("display", "none");
                }



            }

            conn.Close();
            DropDownItemID.Enabled = true;
            TextItemWtWbWP.Enabled = true;
            TextItemWeightWP.Enabled = true;
            TextItemRateWP.Enabled = true;
            TextItemAmountWP.Enabled = true;
            TextItemAmountWP.Attributes.Add("readonly", "readonly");

            Display();
            CheckSlipNoWp.Text = "";
            alert_box.Visible = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

            BtnUpdate.Attributes.Add("aria-disabled", "true");
            BtnUpdate.Attributes.Add("class", "btn btn-success active");
            BtnDelete.Attributes.Add("aria-disabled", "true");
            BtnDelete.Attributes.Add("class", "btn btn-danger active");

            radPurDuplicate.Enabled = false;
            TextMfSlipNo.Enabled = false;


            //     }
            //    catch
            //    {
            //    Response.Redirect("~/ParameterError.aspx");
            //  } 
        }


        public void Display()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = "";
                string DayMonthYear = System.DateTime.Now.ToString("yyyy/MM/dd");
                DateTime ThreeDaysBeforeTemp = System.DateTime.Now.AddDays(-3);
                string ThreeDaysBefore = ThreeDaysBeforeTemp.ToString("yyyy/MM/dd");
                if (txtSearchEmp.Text == "")
                {
                    makeSQL = "  SELECT WPM.PURCHASE_ID, WPM.SLIP_NO, WPM.PARTY_ID, PP.PARTY_NAME, WC.CATEGORY_NAME, WPM.ITEM_ID, WI.ITEM_NAME, WPM.ITEM_WEIGHT,  WPM.ITEM_RATE, WPM.ITEM_AMOUNT, nvl(WPM.ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, WPM.VAT_AMOUNT, nvl(WPM.TOTAL_AMOUNT,0) AS TOTAL_AMOUNT, WPM.ENTRY_DATE, WPM.CREATE_DATE, WPM.UPDATE_DATE, WPM.IS_ACTIVE , WPC.IS_CHECK, WPM.IS_PRINT, TO_CHAR(WPM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE, WPM.CLAIM_NO FROM MF_PURCHASE_MASTER WPM LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = WPM.PARTY_ID LEFT JOIN MF_CATEGORY WC ON WC.CATEGORY_ID = WPM.CATEGORY_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WPM.ITEM_ID  LEFT JOIN WP_PURCHASE_CLAIM WPC ON  WPC.CLAIM_NO = WPM.CLAIM_NO  WHERE to_char(WPM.ENTRY_DATE, 'yyyy/mm/dd') between '" + ThreeDaysBefore + "' AND '" + DayMonthYear + "' ORDER BY WPM.CREATE_DATE DESC  ";

                }
                else
                {
                    if (DropDownSearchItemID.Text == "0")
                    {
                        makeSQL = " SELECT WPM.PURCHASE_ID, WPM.SLIP_NO, WPM.PARTY_ID, PP.PARTY_NAME, WC.CATEGORY_NAME, WPM.ITEM_ID, WI.ITEM_NAME, WPM.ITEM_WEIGHT,  WPM.ITEM_RATE, WPM.ITEM_AMOUNT, nvl(WPM.ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, WPM.VAT_AMOUNT, nvl(WPM.TOTAL_AMOUNT,0) AS TOTAL_AMOUNT, WPM.ENTRY_DATE, WPM.CREATE_DATE, WPM.UPDATE_DATE, WPM.IS_ACTIVE , WPC.IS_CHECK, WPM.IS_PRINT, TO_CHAR(WPM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE, WPM.CLAIM_NO FROM MF_PURCHASE_MASTER WPM LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = WPM.PARTY_ID LEFT JOIN MF_CATEGORY WC ON WC.CATEGORY_ID = WPM.CATEGORY_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WPM.ITEM_ID LEFT JOIN WP_PURCHASE_CLAIM WPC ON  WPC.CLAIM_NO = WPM.CLAIM_NO where WPM.SLIP_NO like '" + txtSearchEmp.Text + "%' or WPM.PARTY_ID like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%'  or WI.ITEM_NAME like '" + txtSearchEmp.Text + "%'  or WPM.ITEM_RATE like '" + txtSearchEmp.Text + "%'  or to_char(WPM.ENTRY_DATE, 'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or to_char(WPM.ENTRY_DATE, 'mm/yyyy')  like '" + txtSearchEmp.Text + "%' or WPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%'  ORDER BY WPM.SLIP_NO asc";  // WPM.CREATE_DATE desc, WPM.UPDATE_DATE desc
                    }
                    else
                    {
                        makeSQL = " SELECT WPM.PURCHASE_ID, WPM.SLIP_NO, WPM.PARTY_ID, PP.PARTY_NAME, WC.CATEGORY_NAME, WPM.ITEM_ID, WI.ITEM_NAME, WPM.ITEM_WEIGHT,  WPM.ITEM_RATE, WPM.ITEM_AMOUNT, nvl(WPM.ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, WPM.VAT_AMOUNT, nvl(WPM.TOTAL_AMOUNT,0) AS TOTAL_AMOUNT, WPM.ENTRY_DATE, WPM.CREATE_DATE, WPM.UPDATE_DATE, WPM.IS_ACTIVE , WPC.IS_CHECK, WPM.IS_PRINT, TO_CHAR(WPM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE, WPM.CLAIM_NO FROM MF_PURCHASE_MASTER WPM LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = WPM.PARTY_ID LEFT JOIN MF_CATEGORY WC ON WC.CATEGORY_ID = WPM.CATEGORY_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WPM.ITEM_ID LEFT JOIN WP_PURCHASE_CLAIM WPC ON  WPC.CLAIM_NO = WPM.CLAIM_NO where  WI.ITEM_ID like '" + DropDownSearchItemID.Text + "%' AND (WPM.SLIP_NO like '" + txtSearchEmp.Text + "%' or WPM.PARTY_ID like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or WPM.ITEM_RATE like '" + txtSearchEmp.Text + "%'  or to_char(WPM.ENTRY_DATE, 'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or to_char(WPM.ENTRY_DATE, 'mm/yyyy')  like '" + txtSearchEmp.Text + "%' or WPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ) ORDER BY WPM.SLIP_NO asc";  // WPM.CREATE_DATE desc, WPM.UPDATE_DATE desc
                    }

                    //   alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView4D.DataSource = dt;
                GridView4D.DataKeyNames = new string[] { "PURCHASE_ID" };
                GridView4D.DataBind();
                conn.Close();
                // alert_box.Visible = false;
            }
        }

        protected void gridViewFileInformation_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow Row in GridView4D.Rows)
            {
                string isCheck = (Row.FindControl("IsCmoCheckLink") as Label).Text;
                string isCheckPrint = (Row.FindControl("IsPrintedCheckLink") as Label).Text;
                string isCheckClaimAvail = (Row.FindControl("IsCmoCheckClaim") as Label).Text;
                if (isCheck == "Complete" || isCheckClaimAvail == "NotAvailable")  // || isCheckPrint == "Printed"
                {
                    (Row.FindControl("linkSelectClick") as LinkButton).Visible = false;
                    (Row.FindControl("linkPrintClick") as LinkButton).Visible = false;
                }
            }
        }

        protected void GridViewSearchEmp(object sender, EventArgs e)
        {
            this.Display();
        }

        protected void GridViewEmp_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView4D.PageIndex = e.NewPageIndex;
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
                    makeSQL = " SELECT  PI.ITEM_NAME, count(WPM.SLIP_NO) AS SLIP_NO, sum(WPM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(WPM.ITEM_AMOUNT) AS ITEM_AMOUNT, (round(sum(WPM.ITEM_AMOUNT)/sum(WPM.ITEM_WEIGHT),2)*1000) AS ITEM_AVG_RATE FROM MF_ITEM PI LEFT JOIN MF_PURCHASE_MASTER WPM ON WPM.ITEM_ID = PI.ITEM_ID WHERE to_char(WPM.ENTRY_DATE, 'mm/yyyy') = '" + MonthYear + "' GROUP BY PI.ITEM_ID, PI.ITEM_NAME ORDER BY PI.ITEM_ID ";
                }
                else
                {
                    makeSQL = " SELECT  PI.ITEM_NAME, count(WPM.SLIP_NO) AS SLIP_NO, sum(WPM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(WPM.ITEM_AMOUNT) AS ITEM_AMOUNT, (round(sum(WPM.ITEM_AMOUNT)/sum(WPM.ITEM_WEIGHT),2)*1000) AS ITEM_AVG_RATE FROM MF_ITEM PI LEFT JOIN MF_PURCHASE_MASTER WPM ON WPM.ITEM_ID = PI.ITEM_ID WHERE to_char(WPM.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear4.Text + "' GROUP BY PI.ITEM_ID, PI.ITEM_NAME ORDER BY PI.ITEM_ID ";

                    //   makeSQL = " SELECT WPM.PURCHASE_ID, WPM.SLIP_NO, WPM.PARTY_ID, PP.PARTY_NAME, WPM.PUR_TYPE_ID, PPT.PUR_TYPE_NAME, WPM.ITEM_ID, PI.ITEM_NAME, WPM.SUB_ITEM_ID, PSI.SUB_ITEM_NAME, WPM.SUPERVISOR_ID, PS.SUPERVISOR_NAME, WPM.ITEM_WEIGHT, WPM.ITEM_RATE, WPM.ITEM_AMOUNT, WPM.ENTRY_DATE, WPM.CREATE_DATE, WPM.UPDATE_DATE, WPM.IS_ACTIVE , PPC.IS_CHECK FROM MF_PURCHASE_MASTER WPM LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = WPM.PARTY_ID LEFT JOIN WP_PURCHASE_TYPE PPT ON PPT.PUR_TYPE_ID = WPM.PUR_TYPE_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = WPM.ITEM_ID LEFT JOIN WP_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = WPM.SUB_ITEM_ID LEFT JOIN WP_SUPERVISOR PS ON PS.SUPERVISOR_ID = WPM.SUPERVISOR_ID  LEFT JOIN WP_PURCHASE_CLAIM PPC ON  PPC.CLAIM_NO = WPM.CLAIM_NO where WPM.SLIP_NO like '" + txtSearchEmp.Text + "%' or WPM.PARTY_ID like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PS.SUPERVISOR_NAME like '" + txtSearchEmp.Text + "%' or WPM.ITEM_RATE like '" + txtSearchEmp.Text + "%' or WPM.ENTRY_DATE like '" + txtSearchEmp.Text + "%' or WPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' or PPC.IS_CHECK like '" + txtSearchEmp.Text + "%' ORDER BY WPM.SLIP_NO asc";  // WPM.CREATE_DATE desc, WPM.UPDATE_DATE desc

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
                    GridView2.HeaderRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                    //Calculate Sum and display in Footer Row
                    GridView2.FooterRow.Cells[0].Font.Bold = true;
                    GridView2.FooterRow.Cells[0].Text = "Grand Total";
                    GridView2.FooterRow.Cells[0].HorizontalAlign = HorizontalAlign.Right;

                    decimal total_prod = dt.AsEnumerable().Sum(row => row.Field<decimal>("SLIP_NO"));
                    GridView2.FooterRow.Cells[1].Font.Bold = true;
                    GridView2.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Center;
                    GridView2.FooterRow.Cells[1].Text = total_prod.ToString("N0");

                    decimal total_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_WEIGHT"));
                    GridView2.FooterRow.Cells[2].Font.Bold = true;
                    GridView2.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[2].Text = total_wt.ToString("N2");

                    decimal total_amt = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_AMOUNT"));
                    GridView2.FooterRow.Cells[3].Font.Bold = true;
                    GridView2.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[3].Text = total_amt.ToString("N2");

                    decimal total_avg = (total_amt / total_wt) * 1000;
                    GridView2.FooterRow.Cells[4].Font.Bold = true;
                    GridView2.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[4].Text = total_avg.ToString("N2");
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
            GridView2.PageIndex = e.NewPageIndex;
            DisplaySummary();
            alert_box.Visible = false;
        }


        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            // try
            //   {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int SupplierID = Convert.ToInt32(DropDownSupplierID.Text);
                int PurchaseID = Convert.ToInt32(TextPurchaseID.Text);
                int SlipNoWp = Convert.ToInt32(TextMfSlipNo.Text);
                int ItemID = Convert.ToInt32(Request.Form[DropDownItemID.UniqueID]);
                int CategoryID = Convert.ToInt32(DropDownCategoryID.Text);

                //  string ItemName = DropDownItemID.SelectedItem.Text;

                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                string[] MakeEntryDateSplit = EntryDate.Text.Split('-');
                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                double ItemRate = Convert.ToDouble(TextItemRateWP.Text.Trim());
                double ItemWeight = Convert.ToDouble(TextItemWeightWP.Text.Trim());
                double ItemAmount = Convert.ToDouble(TextItemAmountWP.Text.Trim());
                double ItemWeightWb = Convert.ToDouble(TextItemWtWbWP.Text.Trim());
                double TotalAmountWP = 0.00;  
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                int VatID = 0; double VatPercent = 0.00, VatAmount = 0.00;
                if (RadioBtnVatWp.SelectedValue == "VatYes")
                {
                    VatID = Convert.ToInt32(DropDownVatID.Text);
                    VatPercent = Convert.ToDouble(DropDownVatID.SelectedItem.Text);
                    VatAmount = (ItemAmount * Convert.ToDouble(DropDownVatID.SelectedItem.Text)) / 100;
                    TotalAmountWP = Math.Round(ItemAmount + VatAmount);
                }
                else
                {
                    TotalAmountWP = Math.Round(ItemAmount);
                }

             

                    // purchase master update 
                    string update_user = "update  MF_PURCHASE_MASTER  set SLIP_NO =:NoSlipID, PARTY_ID =:NoSupplierID,  CATEGORY_ID =:NoCategoryID, ITEM_ID =:NoItemID, SUPERVISOR_ID =:NoSupervisorID, ITEM_WEIGHT_WB =:TextItemWtWbWP, ITEM_WEIGHT =:TextItemWeightWP, ITEM_RATE = :TextItemRateWP, ITEM_AMOUNT =:TextItemAmountWP, VAT_ID =:NoVatID, VAT_PERCENT =:NoVatPercent, VAT_AMOUNT =:NoVatAmount, TOTAL_AMOUNT =:TextTotalAmountWP, ITEM_WET_INVENTORY =:NoItemWetInventory, ENTRY_DATE = TO_DATE(:EntryDate, 'DD/MM/YYYY'), REMARKS =:TextRemarks, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID =:NoCuserID, IS_ACTIVE =:TextIsActive where PURCHASE_ID =:NoPurchaseID ";
                    cmdi = new OracleCommand(update_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[23];
                    objPrm[0] = cmdi.Parameters.Add("NoSlipID", SlipNoWp);
                    objPrm[1] = cmdi.Parameters.Add("NoSupplierID", SupplierID);
                    objPrm[5] = cmdi.Parameters.Add("NoCategoryID", CategoryID);
                    objPrm[6] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[8] = cmdi.Parameters.Add("TextItemWtWbWP", ItemWeightWb);
                    objPrm[9] = cmdi.Parameters.Add("TextItemWeightWP", ItemWeight);
                    objPrm[10] = cmdi.Parameters.Add("TextItemRateWP", ItemRate);
                    objPrm[11] = cmdi.Parameters.Add("TextItemAmountWP", ItemAmount);
                    objPrm[12] = cmdi.Parameters.Add("TextTotalAmountWP", TotalAmountWP);
                    objPrm[13] = cmdi.Parameters.Add("NoVatID", VatID);
                    objPrm[14] = cmdi.Parameters.Add("NoVatPercent", VatPercent);
                    objPrm[15] = cmdi.Parameters.Add("NoVatAmount", VatAmount); 
                    objPrm[17] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[18] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                    objPrm[19] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[20] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[21] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[22] = cmdi.Parameters.Add("NoPurchaseID", PurchaseID);


                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Purchase Data Update Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                    clearText();
                    TextMfSlipNo.Focus();
                    Display();
                    DisplaySummary();
                    VatPercentBox.Style.Add("display", "none");
                 
                 
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
            //  }
            //   catch
            //   {
            //     Response.Redirect("~/ParameterError.aspx");
            //    } 
        }


                protected void BtnDelete_Click(object sender, EventArgs e)
                {
                    //  try
                    //    {
                    if (IS_DELETE_ACTIVE == "Enable")
                    {
                        OracleConnection conn = new OracleConnection(strConnString);
                        conn.Open();
                        // check update item weight is available

                        int PurchaseID = Convert.ToInt32(TextPurchaseID.Text);


                        string delete_user = " delete from MF_PURCHASE_MASTER where PURCHASE_ID  = '" + PurchaseID + "'";
                        cmdi = new OracleCommand(delete_user, conn);
                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();

                        conn.Close();
                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Purchase Data Delete successfully"));
                        alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                        clearText();
                        Display();
                        DisplaySummary();

                    }
                    else
                    {
                        Response.Redirect("~/PagePermissionError.aspx");
                    }
                    //   }
                    //  catch
                    //  {
                    //      Response.Redirect("~/ParameterError.aspx");
                    //  } 
 
                        }




        public void TextMfSlipNo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string SlipNoWp = TextMfSlipNo.Text;
                string MatchEmpIDPattern = "^([0-9]{6})$";
                if (SlipNoWp != null)
                {

                    if (Regex.IsMatch(SlipNoWp, MatchEmpIDPattern))
                    {
                        alert_box.Visible = false;

                        OracleConnection conn = new OracleConnection(strConnString);
                        conn.Open();
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = "select SLIP_NO from MF_PURCHASE_MASTER where SLIP_NO = '" + Convert.ToInt32(SlipNoWp) + "'";
                        cmd.CommandType = CommandType.Text;

                        OracleDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            CheckSlipNoWp.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Slip Number is not available</label>";
                            CheckSlipNoWp.ForeColor = System.Drawing.Color.Red;
                            TextMfSlipNo.Focus();
                            BtnAdd.Attributes.Add("aria-disabled", "false");
                            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");


                        }
                        else
                        {
                            CheckSlipNoWp.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Slip Number is available</label>";
                            CheckSlipNoWp.ForeColor = System.Drawing.Color.Green;
                            //   DropDownCollectionFor.Focus();
                            BtnAdd.Attributes.Add("aria-disabled", "true");
                            BtnAdd.Attributes.Add("class", "btn btn-primary active");

                        }
                    }
                    else
                    {
                        CheckSlipNoWp.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter Slip Number is 6 digit only</label>";
                        CheckSlipNoWp.ForeColor = System.Drawing.Color.Red;
                        TextMfSlipNo.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

                    }
                }
            }
            catch
            {
                Response.Redirect("~/ParameterError.aspx");
            }
        }

        public void clearTextField(object sender, EventArgs e)
        {
            TextMfSlipNo.Text = "";
            TextItemRateWP.Text = "";
            TextItemWeightWP.Text = "";
            DropDownCategoryID.Text = "0";
            TextItemAmountWP.Text = "";
            TextTotalAmountWP.Text = "";
            TextItemVatAmountWP.Text = "";
            CheckSlipNoWp.Text = "";
            DropDownSupplierID.Text = "0";
            DropDownItemID.Text = "0";
            TextItemWtWbWP.Text = "";
            TextRemarks.Text = "";
            RadioBtnVatWp.SelectedIndex = 0;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
        }

        public void clearText()
        {
            TextMfSlipNo.Text = "";
            TextItemRateWP.Text = "";
            TextItemWeightWP.Text = "";
            DropDownCategoryID.Text = "0";
            TextItemAmountWP.Text = "";
            TextTotalAmountWP.Text = "";
            TextItemVatAmountWP.Text = "";
            CheckSlipNoWp.Text = "";
            DropDownSupplierID.Text = "0";
            DropDownItemID.Text = "0";
            TextItemWtWbWP.Text = "";
            TextRemarks.Text = "";
            RadioBtnVatWp.SelectedIndex = 0;
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
        
    }
}