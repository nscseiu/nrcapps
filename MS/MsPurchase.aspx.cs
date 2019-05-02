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

namespace NRCAPPS.MS
{
    public partial class MsPurchase : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl, cmdsp;
        OracleDataAdapter oradata; 
        DataTable dt;
        int RowCount;
        double ItemVatAmt = 0.0, ItemAmtTotal = 0.0, ItemWtWbTotal = 0.0, ItemWtTotal = 0.0, TotalInvoiceAmt = 0.0, TotalGarbage = 0.0; string EntryDateSlip = "", PartyArabicName = "", PartyName = "";

        string IS_PAGE_ACTIVE = "", IS_ADD_ACTIVE = "",IS_EDIT_ACTIVE = "",IS_DELETE_ACTIVE = "",IS_VIEW_ACTIVE = "", IS_REPORT_ACTIVE = "", IS_PRINT_ACTIVE ="";
        string HtmlString = "";
        public bool IsLoad { get; set; }  public bool IsLoad2 { get; set; } public bool IsLoad3 { get; set; } public bool IsLoad4 { get; set; } public bool IsLoad6 { get; set; } 
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
                        string makeSupplierSQL = " SELECT MP.PARTY_ID,  MP.PARTY_ID || ' : ' || MP.PARTY_NAME || ' - ' || MP.PARTY_VAT_NO AS PARTY_NAME  FROM MS_PARTY MP  WHERE MP.IS_ACTIVE = 'Enable' ORDER BY MP.PARTY_NAME ASC";
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
                        string makeDropDownItemSQL = " SELECT ITEM_ID, ITEM_CODE  || ' : ' || ITEM_NAME AS ITEM_NAME FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' AND IS_PURCHASE_ACTIVE = 'Enable' ORDER BY ITEM_CODE ASC";
                        dsi = ExecuteBySqlString(makeDropDownItemSQL);
                        dtItemID = (DataTable)dsi.Tables[0];
                        DropDownItemList.DataSource = dtItemID;
                        DropDownItemList.DataValueField = "ITEM_ID";
                        DropDownItemList.DataTextField = "ITEM_NAME";
                        DropDownItemList.DataBind();
                        DropDownItemList.Items.Insert(0, new ListItem("All Item", "0"));

                        DropDownItemList2.DataSource = dtItemID;
                        DropDownItemList2.DataValueField = "ITEM_ID";
                        DropDownItemList2.DataTextField = "ITEM_NAME";
                        DropDownItemList2.DataBind();
                        DropDownItemList2.Items.Insert(0, new ListItem("All Item", "0"));

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
                         
                        DataTable dtSupCatID = new DataTable();
                        DataSet dsc = new DataSet();
                        string makeDropDownSupCatSQL = " SELECT * FROM NRC_VEHICLE_MODE WHERE IS_ACTIVE = 'Enable' ORDER BY VEHICLE_MODE_ID ASC";
                        dsc = ExecuteBySqlString(makeDropDownSupCatSQL);
                        dtSupCatID = (DataTable)dsc.Tables[0];
                        DropDownVehicleID.DataSource = dtSupCatID;
                        DropDownVehicleID.DataValueField = "VEHICLE_MODE_ID";
                        DropDownVehicleID.DataTextField = "VEHICLE_MODE_NAME";
                        DropDownVehicleID.DataBind();
                        DropDownVehicleID.Items.Insert(0, new ListItem("Select Vehicle Mode", "0"));
                        DropDownVehicleID.Items.FindByValue("1").Selected = true;

                        DropDownVehicleID2.DataSource = dtSupCatID;
                        DropDownVehicleID2.DataValueField = "VEHICLE_MODE_ID";
                        DropDownVehicleID2.DataTextField = "VEHICLE_MODE_NAME";
                        DropDownVehicleID2.DataBind();
                        DropDownVehicleID2.Items.Insert(0, new ListItem("Select Vehicle Mode", "0"));

                        DataTable dtPgeID = new DataTable();
                        DataSet dsp = new DataSet();
                        string makeDropDownPgeSQL = " SELECT * FROM NRC_VAT WHERE IS_ACTIVE = 'Enable' ORDER BY VAT_ID ASC";
                        dsp = ExecuteBySqlString(makeDropDownPgeSQL);
                        dtPgeID = (DataTable)dsp.Tables[0];
                        DropDownVatID.DataSource = dtPgeID;
                        DropDownVatID.DataValueField = "VAT_ID";
                        DropDownVatID.DataTextField = "VAT_PERCENT";
                        DropDownVatID.DataBind();

                        DataTable dtRepresentativeID = new DataTable();
                        DataSet dsr = new DataSet();
                        string makeDropDownRepresentativeSQL = " SELECT MR.REPRESENTATIVE_ID, MR.NID_NO || ' : ' ||  MR.REPRESENTATIVE_NAME || ' - ' || HC.COUNTRY_NAME AS REPRESENTATIVE_NAME FROM MS_REPRESENTATIVE MR LEFT JOIN HR_COUNTRIES HC ON HC.COUNTRY_ID = MR.COUNTRY_ID  WHERE MR.IS_ACTIVE = 'Enable' ORDER BY MR.REPRESENTATIVE_ID ASC";
                        dsr = ExecuteBySqlString(makeDropDownRepresentativeSQL);
                        dtRepresentativeID = (DataTable)dsr.Tables[0];
                        DropDownRepresentativeID1.DataSource = dtRepresentativeID;
                        DropDownRepresentativeID1.DataValueField = "REPRESENTATIVE_ID";
                        DropDownRepresentativeID1.DataTextField = "REPRESENTATIVE_NAME";
                        DropDownRepresentativeID1.DataBind();
                        DropDownRepresentativeID1.Items.Insert(0, new ListItem("Select Representative", "0"));

                        DataTable dtMarID = new DataTable();
                        DataSet dsm = new DataSet();
                        string makeDropDownMarSQL = " SELECT VEHICLE_NO FROM MS_PURCHASE_MASTER WHERE IS_ACTIVE = 'Enable' AND VEHICLE_NO IS NOT NULL GROUP BY VEHICLE_NO ORDER BY VEHICLE_NO ASC";
                        dsm = ExecuteBySqlString(makeDropDownMarSQL);
                        dtMarID = (DataTable)dsm.Tables[0];
                        DropDownMarketerID.DataSource = dtMarID;
                        DropDownMarketerID.DataValueField = "VEHICLE_NO";
                        DropDownMarketerID.DataTextField = "VEHICLE_NO";
                        DropDownMarketerID.DataBind();
                        DropDownMarketerID.Items.Insert(0, new ListItem("Select Marketer's", "0"));

                        TextMsSlipNo.Focus();
                        //  VatPercent.Visible = false;
                        TextItemAmount.Attributes.Add("readonly", "readonly");
                        TextTotalAmount.Attributes.Add("readonly", "readonly");
                        TextItemVatAmount.Enabled = false;
                        DropDownItemID.Enabled = false;
                    //    TextItemWeightWb.Enabled = false;
                      //  TextItemWeight.Enabled = false;
                     //   TextItemRate.Enabled = false;
                        RadioBtnVat.Enabled = false;

                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled");

                        //  btnPrint.Enabled = false;
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
         //     { 
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    
                    int ItemID = Convert.ToInt32(Request.Form[DropDownItemID.UniqueID]);
                    int SlipNoWp = Convert.ToInt32(TextMsSlipNo.Text); 
                    int CategoryID = Convert.ToInt32(DropDownCategoryID.Text); 
                    int VehicleID = Convert.ToInt32(DropDownVehicleID.Text); 
                 //   int ItemID   = Convert.ToInt32(DropDownItemID.Text);
                    int SupplierID = Convert.ToInt32(DropDownSupplierID.Text); 
                    int RepresentativeID = Convert.ToInt32(Request.Form[DropDownRepresentativeID.UniqueID]);
                    int SupplierCategory = Convert.ToInt32(DropDownVehicleID.Text); 

               //     string ItemName = DropDownItemID.SelectedItem.Text;
                   
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                      
                    string[] MakeEntryDateSplit = EntryDate.Text.Split('-'); 
                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-"); 
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                     
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                    double ItemRate = Convert.ToDouble(TextItemRate.Text.Trim());
                    double ItemWtWbWP = Convert.ToDouble(TextItemWeightWb.Text.Trim()); 
                    double ItemWeight = Convert.ToDouble(TextItemWeight.Text.Trim());  
                    double ItemAmount = (ItemRate * ItemWeight);
                    double TotalAmountWP = 0.00;
                  
                    int VatID = 0; double VatPercent = 0.00, VatAmount = 0.00;
                    if (RadioBtnVat.SelectedValue == "VatYes")
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
                      

                    string get_user_purchase_id = "select MS_PURCHASE_MASTERID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_user_purchase_id, conn);
                    int newPurchaseID = Int32.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_purchase = "insert into  MS_PURCHASE_MASTER (PURCHASE_ID, SLIP_NO, PARTY_ID, VEHICLE_MODE_ID, VEHICLE_NO, CATEGORY_ID, ITEM_ID, ITEM_WEIGHT_WB, ITEM_WEIGHT, ITEM_RATE, ITEM_AMOUNT, VAT_ID, VAT_PERCENT, VAT_AMOUNT, TOTAL_AMOUNT, ENTRY_DATE, REMARKS, REPRESENTATIVE_ID, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID) values  ( :NoPurchaseID, :NoSlipID, :NoSupplierID, :VehicleID, :TextVehicleNo, :NoCategoryID, :NoItemID, :TextItemWeightWbWP, :TextItemWeight, :TextItemRate, :TextItemAmountWP, :NoVatID, :NoVatPercent, :NoVatAmount, :TextTotalAmountWP, TO_DATE(:EntryDate, 'DD/MM/YYYY'), :TextRemarks, :TextRepresentative, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 4)";
                    cmdi = new OracleCommand(insert_purchase, conn);
                      
                    OracleParameter[] objPrm = new OracleParameter[21];
                    objPrm[0] = cmdi.Parameters.Add("NoPurchaseID", newPurchaseID); 
                    objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNoWp); 
                    objPrm[2] = cmdi.Parameters.Add("NoSupplierID", SupplierID); 
                    objPrm[3] = cmdi.Parameters.Add("VehicleID", VehicleID);   
                    objPrm[4] = cmdi.Parameters.Add("TextVehicleNo", VehicleNo.Text);   
                    objPrm[5] = cmdi.Parameters.Add("NoCategoryID", CategoryID); 
                    objPrm[6] = cmdi.Parameters.Add("NoItemID", ItemID);  
                    objPrm[7] = cmdi.Parameters.Add("TextItemWeightWbWP", ItemWtWbWP);
                    objPrm[8] = cmdi.Parameters.Add("TextItemWeight", ItemWeight);
                    objPrm[9] = cmdi.Parameters.Add("TextItemRate", ItemRate);
                    objPrm[10] = cmdi.Parameters.Add("TextItemAmountWP", ItemAmount);
                    objPrm[11] = cmdi.Parameters.Add("TextTotalAmountWP", TotalAmountWP);
                    objPrm[12] = cmdi.Parameters.Add("NoVatID", VatID);
                    objPrm[13] = cmdi.Parameters.Add("NoVatPercent", VatPercent);
                    objPrm[14] = cmdi.Parameters.Add("NoVatAmount", VatAmount); 
                    objPrm[15] = cmdi.Parameters.Add("EntryDate", EntryDateNew); 
                    objPrm[16] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text); 
                    objPrm[17] = cmdi.Parameters.Add("TextRepresentative", RepresentativeID);
                    objPrm[18] = cmdi.Parameters.Add("c_date", c_date); 
                    objPrm[19] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[20] = cmdi.Parameters.Add("TextIsActive", ISActive);
                  
                    cmdi.ExecuteNonQuery(); 
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                     
                    //inventory calculation
                     
                    int InvenItemID = 0;
                    int InvenCategoryID = 0;
                    double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, StockOutWetDe = 0.00, FinalStock = 0.00, StockInWetNew = 0.00, FinalStockNew = 0.00;
                     

                        string makeSQL = " select * from MS_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                        cmdl = new OracleCommand(makeSQL);
                        oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                        dt = new DataTable();
                        oradata.Fill(dt);
                        RowCount = dt.Rows.Count;


                        for (int i = 0; i < RowCount; i++)
                        {
                            InvenCategoryID = Convert.ToInt32(dt.Rows[i]["CATEGORY_ID"].ToString());
                            InvenItemID = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString()); 
                            InitialStock = Convert.ToDouble(dt.Rows[i]["INITIAL_STOCK_WT"].ToString());
                            StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                            StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString());
                            FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                        }

                        StockInWetNew = StockInWet + ItemWeight;
                        FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet; 

                        if (0 < RowCount)
                        {

                            string update_inven_mas = "update  MS_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                            cmdu = new OracleCommand(update_inven_mas, conn);

                            OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                            objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                            objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                            objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                            objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                            objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);

                            cmdu.ExecuteNonQuery();
                            cmdu.Parameters.Clear();
                            cmdu.Dispose();
                        }
                        
                    conn.Close();
                     
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Purchase Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                     
                    clearText();
                    TextMsSlipNo.Focus();
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
                if (TextMsSlipNo.Text != "")
                {
                    SlipNoWp = TextMsSlipNo.Text;
                }
                else
                {
                    LinkButton btn = (LinkButton)sender;
                    Session["user_data_id"] = btn.CommandArgument;
                    SlipNoWp = Session["user_data_id"].ToString();
                }
                 
                string makeSQL = "  SELECT PPM.PURCHASE_ID, PPM.SLIP_NO, PP.PARTY_ID, PP.PARTY_NAME, PP.PARTY_ARABIC_NAME, PP.PARTY_ADD_1 || ', ' || PP.PARTY_ADD_2 AS PARTY_ADD, PP.PARTY_VAT_NO, PI.ITEM_CODE, PI.ITEM_NAME, PI.ITEM_ARABIC_NAME, PPM.ITEM_WEIGHT, nvl(PPM.ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, PPM.ITEM_RATE, ROUND(PPM.ITEM_AMOUNT,2) AS ITEM_AMOUNT, nvl(PPM.VAT_AMOUNT, 0) AS VAT_AMOUNT, TO_CHAR(PPM.ENTRY_DATE, 'dd-MON-yyyy') AS ENTRY_DATE, HE.EMP_LNAME FROM MS_PURCHASE_MASTER PPM LEFT JOIN MS_PARTY PP ON PP.PARTY_ID = PPM.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID LEFT JOIN MS_WB_OPERATOR PWO ON PWO.IS_ACTIVE = 'Enable' LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PWO.EMP_ID WHERE PPM.SLIP_NO = '" + SlipNoWp + "' ORDER BY PI.ITEM_ID";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                for (int i = 0; i < 1; i++)
                {
                    PartyArabicName = dt.Rows[i]["PARTY_ARABIC_NAME"].ToString();
                    PartyName = dt.Rows[i]["PARTY_NAME"].ToString();
                    string PartyAdd = dt.Rows[i]["PARTY_ADD"].ToString();
                    string PartyVatNo = dt.Rows[i]["PARTY_VAT_NO"].ToString();
                    string ItemCode = dt.Rows[i]["ITEM_CODE"].ToString();
                    string ItemName = dt.Rows[i]["ITEM_NAME"].ToString();
                    string ItemArabicName = dt.Rows[i]["ITEM_ARABIC_NAME"].ToString();
                    string EmpWbLname = dt.Rows[i]["EMP_LNAME"].ToString();
                    EntryDateSlip = dt.Rows[i]["ENTRY_DATE"].ToString();

                    HtmlString += "<div style='float:left;width:785px;height:240px;margin-top:278px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";
                    HtmlString += "<div style='float:left;width:380px;'> ";
                    HtmlString += "<div style='float:left;width:270px;height:85px;margin-left:100px;padding-top:15px;text-align:center;' >"+ dt.Rows[i]["PARTY_ID"].ToString() + ":" + PartyName + "|<font size='1px'>" +  PartyArabicName + "</font></div> ";
                    HtmlString += "<div style='float:left;width:210px;height:33px; margin:0 0 0 90px;'>" + PartyAdd + "</div> <div style='float:left;width:210px; margin:0px 0 0 90px;'>" + PartyVatNo + " </div> ";
                    HtmlString += "</div> ";
                }
                int m = 1;
                HtmlString += "<div style='float:left;width:380px;'> ";
                HtmlString += "<div style='float:left;width:240px;height:42px;margin:20px 0 0 140px;'><font size='1px'>";
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

                HtmlString += "</font></div>";

                for (int i = 0; i < 1; i++)
                {
                    string EmpWbLname = dt.Rows[i]["EMP_LNAME"].ToString();
                    HtmlString += "<div style='float:left;width:380px;'> ";
                    HtmlString += "<div style='float:left;width:200px;height:71px;margin:0 0 0 175px;'>" + EmpWbLname + "</div><div style='float:left;width:200px;margin:0 0 0 180px;'>" + EntryDateSlip + "</div> ";
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

                    double ItemWtWb = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString());
                    ItemWtWbTotal += Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString());
                    double VarianceWT = (Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString()) - Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString()));
                    double ItemWt = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());
                    double ItemRate = Convert.ToDouble(dt.Rows[i]["ITEM_RATE"].ToString());
                    double ItemAmt = Convert.ToDouble(decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00"));
                    ItemAmtTotal += Convert.ToDouble(decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00"));
                    ItemVatAmt += Convert.ToDouble(decimal.Parse(dt.Rows[i]["VAT_AMOUNT"].ToString()).ToString(".00"));
                    double TotalInvoiceAmt = +Convert.ToDouble(decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00"));

                    HtmlString += "<div style='float:left;width:242px;' >" + ItemCode + "     " + ItemName + " <font size='1px'>" + ItemArabicName + "</font></div> ";
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
                HtmlString += "<div style = 'float:left;width:290px;height:88px;margin:60px 0 0 460px;padding:10px;text-align:left;'>" + NumberToInWord + " </div> ";
                HtmlString += "<div style = 'float:left;width:200px;margin:0 0 0 555px;text-align:left;'><font size='1px'>" + PartyName + "|" + PartyArabicName + "</font> </div> ";
                HtmlString += "</div>";
                HtmlString += "</div>";
                HtmlString += "</div>";

                // purchase master update for print
                int userID = Convert.ToInt32(Session["USER_ID"]); 
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string update_user = "update  MS_PURCHASE_MASTER  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where SLIP_NO = :NoSlipNo ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[4];
                objPrm[0] = cmdi.Parameters.Add("TextIsPrint", "Printed");
                objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[2] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[3] = cmdi.Parameters.Add("NoSlipNo", SlipNoWp);

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
                TextMsSlipNo.Focus();
                CheckSlipNoWp.Visible = true;
                BtnAdd.Attributes.Add("aria-disabled", "false");
                BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 
            }
            else
            {
                alert_box.Visible = false;
                TextMsSlipNo.Focus();
                CheckSlipNoWp.Visible = false;
                BtnAdd.Attributes.Add("aria-disabled", "true");
                BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            }
        }

        [WebMethod]
        public static Boolean MsSlipNoCheck(int WpSlipNo)
        {
            Boolean result = false;
            string query = "select SLIP_NO from MS_PURCHASE_MASTER where SLIP_NO = '" + WpSlipNo + "'";
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
            string query = " SELECT ITEM_ID, ITEM_CODE || ' : ' || ITEM_NAME AS ITEM_NAME FROM MF_ITEM WHERE CATEGORY_ID = :ItemID AND IS_ACTIVE = 'Enable' AND IS_PURCHASE_ACTIVE = 'Enable' ORDER BY ITEM_CODE ASC ";
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

        [WebMethod]
        public static List<ListItem> GetRepresentativeList(string PartyId)
        {
            string[] PartyID = PartyId.Split('-');
            string query = " SELECT MR.REPRESENTATIVE_ID, MR.NID_NO || ' : ' ||  MR.REPRESENTATIVE_NAME || ' - ' || HC.COUNTRY_NAME AS REPRESENTATIVE_NAME FROM MS_PARTY_REPRESENTATIVE MPR LEFT JOIN MS_REPRESENTATIVE MR ON MR.REPRESENTATIVE_ID =  MPR.REPRESENTATIVE_ID LEFT JOIN HR_COUNTRIES HC ON HC.COUNTRY_ID = MR.COUNTRY_ID  WHERE MPR.PARTY_ID = :PartyId AND MR.IS_ACTIVE = 'Enable' ORDER BY MR.REPRESENTATIVE_ID ASC ";
            string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; 
            using (OracleConnection conn = new OracleConnection(strConnString))
            {
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    List<ListItem> cities = new List<ListItem>();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("PartyId", Convert.ToInt32(PartyID[0]));
                    cmd.Connection = conn;
                    conn.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            cities.Add(new ListItem
                            {
                                Value = sdr["REPRESENTATIVE_ID"].ToString(),
                                Text = sdr["REPRESENTATIVE_NAME"].ToString()
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
        // try{
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
            int USER_DATA_ID = Convert.ToInt32(Session["user_data_id"]);

            string makeSQL = " select PURCHASE_ID, SLIP_NO,  PARTY_ID, REPRESENTATIVE_ID, VEHICLE_MODE_ID, VEHICLE_NO, ITEM_ID, CATEGORY_ID, ITEM_WEIGHT, nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, ITEM_RATE, nvl(ITEM_AMOUNT,0) AS ITEM_AMOUNT, nvl(VAT_ID,0) AS VAT_ID, nvl(VAT_AMOUNT,0) AS VAT_AMOUNT, nvl(TOTAL_AMOUNT,0) AS TOTAL_AMOUNT, TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, CREATE_DATE,  UPDATE_DATE,  C_USER_ID, U_USER_ID, IS_ACTIVE from MS_PURCHASE_MASTER where PURCHASE_ID  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                DataTable dtItemID = new DataTable();
                DataSet dsi = new DataSet();
                string makeDropDownItemSQL = " SELECT ITEM_ID, ITEM_CODE || ' : ' || ITEM_NAME AS ITEM_NAME FROM MF_ITEM WHERE CATEGORY_ID = '" + Convert.ToInt32(dt.Rows[i]["CATEGORY_ID"].ToString()) + "' AND IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                dsi = ExecuteBySqlString(makeDropDownItemSQL);
                dtItemID = (DataTable)dsi.Tables[0];
                DropDownItemID.DataSource = dtItemID;
                DropDownItemID.DataValueField = "ITEM_ID";
                DropDownItemID.DataTextField = "ITEM_NAME";
                DropDownItemID.DataBind();

                DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0")); 
                TextPurchaseID.Text = dt.Rows[i]["PURCHASE_ID"].ToString();              
                TextMsSlipNo.Text = dt.Rows[i]["SLIP_NO"].ToString();
                DropDownVehicleID.Text = dt.Rows[i]["VEHICLE_MODE_ID"].ToString();
                VehicleNo.Text = dt.Rows[i]["VEHICLE_NO"].ToString(); 
                DropDownSupplierID.Text = dt.Rows[i]["PARTY_ID"].ToString();
            
                DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();
                DropDownCategoryID.Text = dt.Rows[i]["CATEGORY_ID"].ToString();
                TextItemRate.Text = dt.Rows[i]["ITEM_RATE"].ToString(); 
                TextItemWeight.Text  = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT"].ToString()).ToString(".00");
                TextItemAmount.Text = decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00");
                TextTotalAmount.Text = decimal.Parse(dt.Rows[i]["TOTAL_AMOUNT"].ToString()).ToString(".00"); 
                EntryDate.Text = dt.Rows[i]["ENTRY_DATE"].ToString();
                TextItemWeightWb.Text  = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString()).ToString(".00");
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);

                DataTable dtRepresentativeID = new DataTable();
                DataSet dsr = new DataSet();
                string makeRepresentativeIDSQL = " SELECT MR.REPRESENTATIVE_ID, MR.NID_NO || ' : ' ||  MR.REPRESENTATIVE_NAME || ' - ' || HC.COUNTRY_NAME AS REPRESENTATIVE_NAME FROM MS_PARTY_REPRESENTATIVE MPR LEFT JOIN MS_REPRESENTATIVE MR ON MR.REPRESENTATIVE_ID =  MPR.REPRESENTATIVE_ID LEFT JOIN HR_COUNTRIES HC ON HC.COUNTRY_ID = MR.COUNTRY_ID  WHERE MPR.PARTY_ID = '"+ Convert.ToInt32(dt.Rows[i]["PARTY_ID"].ToString()) + "' AND MR.IS_ACTIVE = 'Enable' ORDER BY MR.REPRESENTATIVE_ID ASC";
                dsr = ExecuteBySqlString(makeRepresentativeIDSQL);
                dtRepresentativeID = (DataTable)dsr.Tables[0];
                DropDownRepresentativeID.DataSource = dtRepresentativeID;
                DropDownRepresentativeID.DataValueField = "REPRESENTATIVE_ID";
                DropDownRepresentativeID.DataTextField = "REPRESENTATIVE_NAME";
                DropDownRepresentativeID.DataBind();
                DropDownRepresentativeID.Text = dt.Rows[i]["REPRESENTATIVE_ID"].ToString(); 

                RadioBtnVat.Enabled = true; 
                if (dt.Rows[i]["VAT_ID"].ToString() != "0")
                {
                    RadioBtnVat.Text = "VatYes"; 
                    VatPercentBox.Style.Remove("display");  
                    TextItemVatAmount.Text  = decimal.Parse(dt.Rows[i]["VAT_AMOUNT"].ToString()).ToString(".00");
                    DropDownVatID.Text =  dt.Rows[i]["VAT_ID"].ToString();
                }
                else { RadioBtnVat.Text = "VatNo";
                    VatPercentBox.Style.Add("display", "none");
                }

 

            }

            conn.Close();
            DropDownItemID.Enabled = true; 
            TextItemWeight.Enabled = true;
            TextItemRate.Enabled = true;
            TextItemAmount.Enabled = true;
            TextItemAmount.Attributes.Add("readonly", "readonly");

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
            TextMsSlipNo.Enabled = false;
             

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
                DateTime ThreeDaysBeforeTemp = System.DateTime.Now.AddDays(-15);
                string ThreeDaysBefore = ThreeDaysBeforeTemp.ToString("yyyy/MM/dd");
                if (txtSearchEmp.Text == "")
                {
                    makeSQL = " SELECT WPM.PURCHASE_ID, WPM.SLIP_NO, WPM.PARTY_ID, PP.PARTY_NAME, WSC.VEHICLE_MODE_NAME, WPM.VEHICLE_NO, WC.CATEGORY_NAME, WPM.ITEM_ID, WI.ITEM_CODE  || ' : ' || WI.ITEM_NAME AS ITEM_NAME,  WCF.REPRESENTATIVE_NAME, WPM.ITEM_WEIGHT,  WPM.ITEM_RATE, WPM.ITEM_AMOUNT, nvl(WPM.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, WPM.VAT_AMOUNT, nvl(WPM.TOTAL_AMOUNT, 0) AS TOTAL_AMOUNT, WPM.ENTRY_DATE, WPM.CREATE_DATE, WPM.UPDATE_DATE, WPM.IS_ACTIVE , WPC.IS_CHECK, WPM.IS_PRINT, TO_CHAR(WPM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE, WPM.CLAIM_NO FROM MS_PURCHASE_MASTER WPM LEFT JOIN MS_PARTY PP ON PP.PARTY_ID = WPM.PARTY_ID LEFT JOIN MF_CATEGORY WC ON WC.CATEGORY_ID = WPM.CATEGORY_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WPM.ITEM_ID LEFT JOIN MS_REPRESENTATIVE WCF ON WCF.REPRESENTATIVE_ID = PP.REPRESENTATIVE_ID LEFT JOIN MS_PURCHASE_CLAIM WPC ON WPC.CLAIM_NO = WPM.CLAIM_NO LEFT JOIN NRC_VEHICLE_MODE WSC ON WSC.VEHICLE_MODE_ID = WPM.VEHICLE_MODE_ID  WHERE to_char(WPM.ENTRY_DATE, 'yyyy/mm/dd') between '" + ThreeDaysBefore  + "' AND '" + DayMonthYear + "' ORDER BY WPM.CREATE_DATE DESC  ";

                }
                else
                {
                    if (DropDownSearchItemID.Text == "0")
                    {
                        makeSQL = " SELECT WPM.PURCHASE_ID, WPM.SLIP_NO, WPM.PARTY_ID, PP.PARTY_NAME, WSC.VEHICLE_MODE_NAME, WPM.VEHICLE_NO, WC.CATEGORY_NAME, WPM.ITEM_ID, WI.ITEM_CODE  || ' : ' || WI.ITEM_NAME AS ITEM_NAME,  WCF.REPRESENTATIVE_NAME, WPM.ITEM_WEIGHT,  WPM.ITEM_RATE, WPM.ITEM_AMOUNT, nvl(WPM.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, WPM.VAT_AMOUNT, nvl(WPM.TOTAL_AMOUNT, 0) AS TOTAL_AMOUNT, WPM.ENTRY_DATE, WPM.CREATE_DATE, WPM.UPDATE_DATE, WPM.IS_ACTIVE , WPC.IS_CHECK, WPM.IS_PRINT, TO_CHAR(WPM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE, WPM.CLAIM_NO FROM MS_PURCHASE_MASTER WPM LEFT JOIN MS_PARTY PP ON PP.PARTY_ID = WPM.PARTY_ID LEFT JOIN MF_CATEGORY WC ON WC.CATEGORY_ID = WPM.CATEGORY_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WPM.ITEM_ID LEFT JOIN MS_REPRESENTATIVE WCF ON WCF.REPRESENTATIVE_ID = PP.REPRESENTATIVE_ID LEFT JOIN MS_PURCHASE_CLAIM WPC ON WPC.CLAIM_NO = WPM.CLAIM_NO LEFT JOIN NRC_VEHICLE_MODE WSC ON WSC.VEHICLE_MODE_ID = WPM.VEHICLE_MODE_ID where WPM.SLIP_NO like '" + txtSearchEmp.Text + "%' or WPM.PARTY_ID like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%'  or WI.ITEM_NAME like '" + txtSearchEmp.Text + "%'  or WPM.ITEM_RATE like '" + txtSearchEmp.Text + "%'  or to_char(WPM.ENTRY_DATE, 'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or to_char(WPM.ENTRY_DATE, 'mm/yyyy')  like '" + txtSearchEmp.Text + "%' or WPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%'  ORDER BY WPM.SLIP_NO asc";  // WPM.CREATE_DATE desc, WPM.UPDATE_DATE desc
                    }
                    else
                    {
                        makeSQL = " SELECT WPM.PURCHASE_ID, WPM.SLIP_NO, WPM.PARTY_ID, PP.PARTY_NAME, WSC.VEHICLE_MODE_NAME, WPM.VEHICLE_NO, WC.CATEGORY_NAME, WPM.ITEM_ID, WI.ITEM_CODE  || ' : ' || WI.ITEM_NAME AS ITEM_NAME,  WCF.REPRESENTATIVE_NAME, WPM.ITEM_WEIGHT,  WPM.ITEM_RATE, WPM.ITEM_AMOUNT, nvl(WPM.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, WPM.VAT_AMOUNT, nvl(WPM.TOTAL_AMOUNT, 0) AS TOTAL_AMOUNT, WPM.ENTRY_DATE, WPM.CREATE_DATE, WPM.UPDATE_DATE, WPM.IS_ACTIVE , WPC.IS_CHECK, WPM.IS_PRINT, TO_CHAR(WPM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE, WPM.CLAIM_NO FROM MS_PURCHASE_MASTER WPM LEFT JOIN MS_PARTY PP ON PP.PARTY_ID = WPM.PARTY_ID LEFT JOIN MF_CATEGORY WC ON WC.CATEGORY_ID = WPM.CATEGORY_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WPM.ITEM_ID LEFT JOIN MS_REPRESENTATIVE WCF ON WCF.REPRESENTATIVE_ID = PP.REPRESENTATIVE_ID LEFT JOIN MS_PURCHASE_CLAIM WPC ON WPC.CLAIM_NO = WPM.CLAIM_NO LEFT JOIN NRC_VEHICLE_MODE WSC ON WSC.VEHICLE_MODE_ID = WPM.VEHICLE_MODE_ID where  WI.ITEM_ID like '" + DropDownSearchItemID.Text + "%' AND (WPM.SLIP_NO like '" + txtSearchEmp.Text + "%' or WPM.PARTY_ID like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or WPM.ITEM_RATE like '" + txtSearchEmp.Text + "%'  or to_char(WPM.ENTRY_DATE, 'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or to_char(WPM.ENTRY_DATE, 'mm/yyyy')  like '" + txtSearchEmp.Text + "%' or WPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ) ORDER BY WPM.SLIP_NO asc";  // WPM.CREATE_DATE desc, WPM.UPDATE_DATE desc
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
                    (Row.FindControl("LinkSelectClick") as LinkButton).Visible = false;
                    (Row.FindControl("LinkPrintClick") as LinkButton).Visible = false;
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
                    makeSQL = " SELECT PI.ITEM_CODE || ' : ' || PI.ITEM_NAME AS ITEM_NAME, count(WPM.SLIP_NO) AS SLIP_NO, sum(WPM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(WPM.ITEM_AMOUNT) AS ITEM_AMOUNT, (round(sum(WPM.ITEM_AMOUNT)/sum(WPM.ITEM_WEIGHT),2)*1000) AS ITEM_AVG_RATE FROM MF_ITEM PI LEFT JOIN MS_PURCHASE_MASTER WPM ON WPM.ITEM_ID = PI.ITEM_ID WHERE to_char(WPM.ENTRY_DATE, 'mm/yyyy') = '" + MonthYear + "' GROUP BY PI.ITEM_CODE || ' : ' || PI.ITEM_NAME  ORDER BY PI.ITEM_CODE || ' : ' || PI.ITEM_NAME ";
                }
                else
                {
                    makeSQL = " SELECT PI.ITEM_CODE || ' : ' || PI.ITEM_NAME AS ITEM_NAME, count(WPM.SLIP_NO) AS SLIP_NO, sum(WPM.ITEM_WEIGHT) AS ITEM_WEIGHT, sum(WPM.ITEM_AMOUNT) AS ITEM_AMOUNT, (round(sum(WPM.ITEM_AMOUNT)/sum(WPM.ITEM_WEIGHT),2)*1000) AS ITEM_AVG_RATE FROM MF_ITEM PI LEFT JOIN MS_PURCHASE_MASTER WPM ON WPM.ITEM_ID = PI.ITEM_ID WHERE to_char(WPM.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear4.Text + "' GROUP BY PI.ITEM_CODE || ' : ' || PI.ITEM_NAME ORDER BY PI.ITEM_CODE || ' : ' || PI.ITEM_NAME ";
                     
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

                    decimal total_avg = (total_amt / total_wt)*1000;
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
                int PurchaseID = Convert.ToInt32(TextPurchaseID.Text);
                int SlipNoWp = Convert.ToInt32(TextMsSlipNo.Text); 
                int ItemID = Convert.ToInt32(Request.Form[DropDownItemID.UniqueID]);
                int CategoryID = Convert.ToInt32(DropDownCategoryID.Text);
                int VehicleID = Convert.ToInt32(DropDownVehicleID.Text);
                int SupplierID = Convert.ToInt32(DropDownSupplierID.Text);
                int RepresentativeID = Convert.ToInt32(Request.Form[DropDownRepresentativeID.UniqueID]);
                //  string ItemName = DropDownItemID.SelectedItem.Text;

                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                string[] MakeEntryDateSplit = EntryDate.Text.Split('-');
                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                 
                double ItemRate = Convert.ToDouble(TextItemRate.Text.Trim());
                double ItemWeight = Convert.ToDouble(TextItemWeight.Text.Trim());
                double ItemAmount = (ItemRate * ItemWeight);              
                double ItemWeightWb = Convert.ToDouble(TextItemWeightWb.Text.Trim()); 
                double TotalAmountWP = 0.00;   
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                int VatID = 0; double VatPercent = 0.00, VatAmount = 0.00;
                if (RadioBtnVat.SelectedValue == "VatYes")
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

                    // get old purchase item weight
                    double ItemWeightOld = 0.00; int ItemIdOld = 0;
                    string makeSQL = " select  ITEM_ID, nvl(ITEM_WEIGHT,0) AS ITEM_WEIGHT  from MS_PURCHASE_MASTER where PURCHASE_ID  = '" + PurchaseID + "'";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;
                    for (int i = 0; i < RowCount; i++)
                    {
                        ItemIdOld = Convert.ToInt32(dt.Rows[i]["ITEM_ID"]);
                        ItemWeightOld = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"]);
                    }


                    // check update item weight is available
                    double FinalStockCheck = 0.00;
                    string makeSQLCheckRM = " select * from MS_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                    cmdl = new OracleCommand(makeSQLCheckRM);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    for (int i = 0; i < RowCount; i++)
                    { 
                        FinalStockCheck = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                if (ItemWeightOld <= FinalStockCheck) { 
             
                int InvenItemID = 0; 
                double InitialStock = 0.00, StockInWet = 0.00, StockInWetCurrent = 0.00, StockInWetNew = 0.00, StockOutWet = 0.00, FinalStock = 0.00, FinalStockNew = 0.00; 
                
                // deduction old item weight
                string makeSQLInvernRM = " select * from MS_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                cmdl = new OracleCommand(makeSQLInvernRM);
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

                    StockInWetCurrent = StockInWet - ItemWeightOld;
                    FinalStockNew = (InitialStock + StockInWetCurrent) - StockOutWet;

                 
                    if (0 < RowCount)
                    {
                        string update_inven_mas = "update  MS_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetCurrent);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", ItemIdOld); 

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }

                    // inventory RM update insert
                    string makeSQLInvenRMUp = " select * from MS_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeSQLInvenRMUp);
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

                    StockInWetNew = StockInWet + ItemWeight;
                    FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;

                    if (0 < RowCount)
                    {

                        string update_inven_mas = "update  MS_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
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

                // purchase master update 
                string update_user = "update  MS_PURCHASE_MASTER  set SLIP_NO =:NoSlipID, PARTY_ID =:NoSupplierID, VEHICLE_MODE_ID =:VehicleID, VEHICLE_NO =:TextVehicleNo, CATEGORY_ID =:NoCategoryID, ITEM_ID =:NoItemID, ITEM_WEIGHT_WB =:TextItemWeightWbWP, ITEM_WEIGHT =:TextItemWeight, ITEM_RATE = :TextItemRate, ITEM_AMOUNT =:TextItemAmountWP, VAT_ID =:NoVatID, VAT_PERCENT =:NoVatPercent, VAT_AMOUNT =:NoVatAmount, TOTAL_AMOUNT =:TextTotalAmountWP, ENTRY_DATE = TO_DATE(:EntryDate, 'DD/MM/YYYY'), REMARKS =:TextRemarks, REPRESENTATIVE_ID =:TextRepresentative, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID =:NoCuserID, IS_ACTIVE =:TextIsActive where PURCHASE_ID =:NoPurchaseID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[21];
                objPrm[0] = cmdi.Parameters.Add("NoSlipID", SlipNoWp);
                objPrm[1] = cmdi.Parameters.Add("NoSupplierID", SupplierID);
                objPrm[2] = cmdi.Parameters.Add("VehicleID", VehicleID);
                objPrm[3] = cmdi.Parameters.Add("TextVehicleNo", VehicleNo.Text);
                objPrm[4] = cmdi.Parameters.Add("NoCategoryID", CategoryID);
                objPrm[5] = cmdi.Parameters.Add("NoItemID", ItemID); 
                objPrm[6] = cmdi.Parameters.Add("TextItemWeightWbWP", ItemWeightWb);
                objPrm[7] = cmdi.Parameters.Add("TextItemWeight", ItemWeight);
                objPrm[8] = cmdi.Parameters.Add("TextItemRate", ItemRate);
                objPrm[9] = cmdi.Parameters.Add("TextItemAmountWP", ItemAmount); 
                objPrm[10] = cmdi.Parameters.Add("TextTotalAmountWP", TotalAmountWP);
                objPrm[11] = cmdi.Parameters.Add("NoVatID", VatID);
                objPrm[12] = cmdi.Parameters.Add("NoVatPercent", VatPercent);
                objPrm[13] = cmdi.Parameters.Add("NoVatAmount", VatAmount);  
                objPrm[14] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                objPrm[15] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                objPrm[16] = cmdi.Parameters.Add("TextRepresentative", RepresentativeID);
                objPrm[17] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[18] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[19] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[20] = cmdi.Parameters.Add("NoPurchaseID", PurchaseID);


                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();
                     
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Purchase Data Update Successfully" ));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                 
                clearText();
                TextMsSlipNo.Focus(); 
                Display();
                DisplaySummary();
                VatPercentBox.Style.Add("display", "none");
                }  else {

                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory"));
                        alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
               }
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
                int ItemID = Convert.ToInt32(DropDownItemID.Text); 
                double ItemWeight = Convert.ToDouble(TextItemWeight.Text.Trim());  

                double FinalStockCheck = 0.00;
                string makeSQLCheckRM = " select * from MS_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                cmdl = new OracleCommand(makeSQLCheckRM);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                for (int i = 0; i < RowCount; i++)
                {
                    FinalStockCheck = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                }

                if (ItemWeight <= FinalStockCheck)
                {
                int userID = Convert.ToInt32(Session["USER_ID"]); 
                int PurchaseID = Convert.ToInt32(TextPurchaseID.Text);    
                // purchase check data
                int InvenItemID = 0, ItemIdOld = 0;  double ItemWeightOld = 0.00; 
                double InitialStock = 0.00, StockInWet = 0.00,   StockInWetNew = 0.00, StockOutWet = 0.00, FinalStock = 0.00, FinalStockNew = 0.00;
                string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string makeSQL = " select ITEM_ID, ITEM_WEIGHT from MS_PURCHASE_MASTER where PURCHASE_ID  = '" + PurchaseID + "'  ";
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                 
                for (int i = 0; i < RowCount; i++)
                { 
                    ItemIdOld = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString()); 
                    ItemWeightOld = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString()); 
                }
                 
                    // inventory RM update delete purchases
                    string makeSQLInvenRMUp = " select ITEM_ID, nvl(INITIAL_STOCK_WT,0) AS INITIAL_STOCK_WT, nvl(STOCK_IN_WT,0) AS STOCK_IN_WT, nvl(STOCK_OUT_WT,0) AS STOCK_OUT_WT, nvl(FINAL_STOCK_WT,0) AS FINAL_STOCK_WT  from MS_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                    cmdl = new OracleCommand(makeSQLInvenRMUp);
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

                    StockInWetNew = StockInWet - ItemWeightOld;
                    FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;

                    if (0 < RowCount)
                    {

                        string update_inven_mas = "update  MS_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID); 

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }

                    
                string delete_user = " delete from MS_PURCHASE_MASTER where PURCHASE_ID  = '" + PurchaseID + "'"; 
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
                        string script = "alert('Item Weight is not available in the Inventory');";
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert", script, true);

                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory"));
                        alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                    }
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

         

        public void clearTextField(object sender, EventArgs e)
        {
            TextMsSlipNo.Text = ""; 
            TextItemRate.Text = "";
            TextItemWeight.Text = "";
            DropDownCategoryID.Text = "0";
            TextItemAmount.Text = "";
            TextTotalAmount.Text = "";
            TextItemVatAmount.Text = "";
            CheckSlipNoWp.Text = ""; 
            DropDownSupplierID.Text = "0";
            DropDownVehicleID.Text = "1"; 
            VehicleNo.Text = "0"; 
            DropDownItemID.Text = "0";
            TextItemWeight.Text = "";
            TextRemarks.Text = "";
            RadioBtnVat.SelectedIndex = 0;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 
        }

        public void clearText()
        {
            TextMsSlipNo.Text = ""; 
            TextItemRate.Text = "";
            TextItemWeight.Text = "";
            DropDownCategoryID.Text = "0";
            TextItemAmount.Text = "";
            TextTotalAmount.Text = "";
            TextItemVatAmount.Text = "";
            CheckSlipNoWp.Text = "";
            DropDownSupplierID.Text = "0";
            DropDownVehicleID.Text = "1";
            VehicleNo.Text = "0";
            DropDownItemID.Text = "0"; 
            TextRemarks.Text = ""; 
            RadioBtnVat.SelectedIndex = 0;
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
        protected void BtnReport2_Click(object sender, EventArgs e)
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
        protected void BtnReport3_Click(object sender, EventArgs e)
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
        protected void BtnReport4_Click(object sender, EventArgs e)
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

        protected void BtnReport6_Click(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad6 = true;
                alert_box.Visible = false;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }

        }


        public int BusinessDaysUntil(DateTime firstDay, DateTime lastDay)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = (int)firstDay.DayOfWeek;
                int lastDayOfWeek = (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 5)
                {
                    if (lastDayOfWeek >= 5)// Only Friday is in the remaining time interval
                        businessDays -= 1;
                }
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount;

            return businessDays;
        }

        protected void BtnReport5_Click(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {
                alert_box.Visible = false;
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                // string HtmlString = "";
                string StartDateTemp = EntryDate3.Text;
                DateTime StartDateTemp1 = DateTime.ParseExact(StartDateTemp, "dd/mm/yyyy", CultureInfo.InvariantCulture);
                DateTime StartDateTemp2 = new DateTime(StartDateTemp1.Year, StartDateTemp1.Month, 1);

                string[] StartDateTempSplit = StartDateTemp.Split('-');
                String StartDateFormTemp = StartDateTempSplit[0].Replace("/", "-");
                DateTime StartDateTempChange = DateTime.ParseExact(StartDateFormTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var StartDateTemp4 = new DateTime(StartDateTempChange.Year, StartDateTempChange.Month, 1);
                string StartDateTemp3 = StartDateTemp4.ToString("dd-MM-yyyy");
                string StartDateQuery = StartDateTemp4.ToString("yyyy/MM/dd");

                string EndDate = EntryDate3.Text;
                  
                DateTime EndDateNew = DateTime.ParseExact(EndDate, "dd/mm/yyyy", CultureInfo.InvariantCulture);
                string EndDateQuery = EndDateNew.ToString("yyyy/mm/dd");

                int TotalDaysWithOutFriday = BusinessDaysUntil(StartDateTemp2, EndDateNew);

                string EndDateTemp = EndDate;
                string[] EndDateTempSplit = EndDateTemp.Split('-');
                String EndDateFormTemp = EndDateTempSplit[0].Replace("/", "-");
                DateTime EndMonthNew = DateTime.ParseExact(EndDateFormTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EndMonth = EndMonthNew.ToString("dd-MMMM-yyyy");

                string makeSQL = " SELECT WI.ITEM_CODE || ' : ' || WI.ITEM_NAME AS ITEM_NAME, SUM(nvl(WPM.ITEM_WEIGHT,0)) AS ITEM_WEIGHT, SUM(nvl(WPM.ITEM_AMOUNT,0)) AS ITEM_AMOUNT, ROUND(nvl((SUM(WPM.ITEM_AMOUNT)/SUM(WPM.ITEM_WEIGHT))*1000,0),2) AS ITEM_RATE, SUM(nvl(MAG.ACTUAL_GAR_WEIGHT, 0)) GARBAGE_WT FROM MF_ITEM WI LEFT JOIN  MS_PURCHASE_MASTER WPM LEFT JOIN MS_ACTUAL_GARBAGE MAG ON MAG.ITEM_ID = WPM.ITEM_ID ON WPM.ITEM_ID = WI.ITEM_ID AND TO_CHAR(TO_DATE(WPM.ENTRY_DATE), 'YYYY/mm/dd') BETWEEN '" + StartDateQuery + "' AND '" + EndDateQuery + "' WHERE (nvl(WPM.ITEM_WEIGHT,0) > 0) GROUP BY WI.ITEM_CODE || ' : ' || WI.ITEM_NAME ORDER BY WI.ITEM_CODE || ' : ' || WI.ITEM_NAME";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                HtmlString += "<div style='float:left;width:725px;height:auto;margin:10px 0 0 40px;padding:10px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;border:black solid 1px;'> ";
                HtmlString += "<div style='float:left;width:725px;height:100px;text-align:center;' ><img src='../../image/logo_from.png'/></div> ";
                HtmlString += "<div style='float:left;width:725px;height:25px;text-align:center;text-decoration: underline;' ><span style='font-family:Times New Roman,Times, serif;font-size:17px;font-weight:700;'>Metal Scrap Division</span></div> ";
                HtmlString += "<div style='float:left;width:725px;height:25px;text-align:center;text-decoration: underline;' ><span style='font-family:Times New Roman,Times, serif;font-size:16px;font-weight:700;'>Average Purchase Price : " + EndMonth + "</span></div> ";
                HtmlString += "<table cellpadding='4px' cellspacing='0' style='font-size: 15px;' width=100%>";
                HtmlString += "<th style='border:black solid 1px; -webkit-border-top-left-radius:10px;width:85px;'>ITEM NAME</th> ";
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>WEIGHT IN KG</span></th> ";
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>AMOUNT - SR</th> ";
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>RATE PER MT</th> ";
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;-webkit-border-top-right-radius:10px;'><span style='size:12px'>DAILY PURCHASE AVG. IN KG</th> ";

                for (int i = 0; i < RowCount; i++)
                {
                    ItemWtWbTotal += Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());
                    ItemAmtTotal += Convert.ToDouble(dt.Rows[i]["ITEM_AMOUNT"].ToString());
                    TotalGarbage += Convert.ToDouble(dt.Rows[i]["GARBAGE_WT"].ToString());

                    HtmlString += "<tr valign='top'> ";

                    HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;'> ";
                    HtmlString += "" + dt.Rows[i]["ITEM_NAME"].ToString() + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n2}", dt.Rows[i]["ITEM_WEIGHT"]) + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;'> ";
                    HtmlString += "" + string.Format("{0:n2}", dt.Rows[i]["ITEM_AMOUNT"]) + "";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'> ";
                    HtmlString += "" + string.Format("{0:n2}", dt.Rows[i]["ITEM_RATE"]) + " ";
                    HtmlString += "</td> ";

                    HtmlString += "<td style='text-align:center;border-bottom:black solid 1px;border-right:black solid 1px;'> ";
                    HtmlString += "" + string.Format("{0:n0}", dt.Rows[i]["ITEM_WEIGHT"]) + "</br> ";

                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";

                }
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;;font-weight:700;'> ";
                HtmlString += "Grand Total ";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n2}", ItemWtWbTotal) + " ";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n2}", ItemAmtTotal) + "";
                HtmlString += "</td> ";
                HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n2}", ((ItemAmtTotal / ItemWtWbTotal) * 1000)) + " ";
                HtmlString += "</td> ";
                HtmlString += "<td style='text-align:center;border-bottom:black solid 1px;border-right:black solid 1px;;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n0}", ItemWtWbTotal) + "</br> ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";

                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td colspan=4 style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;;font-weight:700;'> ";
                HtmlString += "Less - Garbage ";
                HtmlString += "</td> ";
                HtmlString += "<td style='text-align:center;border-bottom:black solid 1px;border-right:black solid 1px;;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n2}", TotalGarbage) + "</br> ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";

                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td colspan=4 style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;;font-weight:700;'> ";
                HtmlString += "Net Total";
                HtmlString += "</td> ";
                HtmlString += "<td style='text-align:center;border-bottom:black solid 1px;border-right:black solid 1px;;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n2}", (ItemWtWbTotal - TotalGarbage)) + "</br> ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";

                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td colspan=4 style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;;font-weight:700;'> ";
                HtmlString += "Net Average Rate After Deduction of Garbage ";
                HtmlString += "</td> ";
                HtmlString += "<td style='text-align:center;border-bottom:black solid 1px;border-right:black solid 1px;;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n2}", (ItemAmtTotal / (ItemWtWbTotal - TotalGarbage)) * 1000) + "</br> ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";

                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td colspan=4 style='-webkit-border-bottom-left-radius:10px;border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;;font-weight:700;'> ";
                HtmlString += "Daily Average Collection (Total Working Days: " + TotalDaysWithOutFriday + ")";
                HtmlString += "</td> ";
                HtmlString += "<td style='-webkit-border-bottom-right-radius:10px;text-align:center;border-bottom:black solid 1px;border-right:black solid 1px;;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n2}", (ItemWtWbTotal / TotalDaysWithOutFriday)) + "</br> ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";

                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td colspan=2> ";
                HtmlString += "&nbsp;";
                HtmlString += "</td> ";
                HtmlString += "<td  colspan=3> ";
                HtmlString += "&nbsp; ";
                HtmlString += "</td> ";

                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td colspan=2> ";
                HtmlString += "Prepared By:";
                HtmlString += "</td> ";
                HtmlString += "<td  colspan=3 style='text-align:right;'> ";
                HtmlString += "Approved By:";
                HtmlString += "</td> ";

                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td colspan=2> ";
                HtmlString += "&nbsp;";
                HtmlString += "</td> ";
                HtmlString += "<td  colspan=3> ";
                HtmlString += "&nbsp; ";
                HtmlString += "</td> ";

                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td colspan=2> ";
                HtmlString += "&nbsp;";
                HtmlString += "</td> ";
                HtmlString += "<td  colspan=3> ";
                HtmlString += "&nbsp; ";
                HtmlString += "</td> ";

                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td colspan=2> ";
                HtmlString += "---";
                HtmlString += "</td> ";
                HtmlString += "<td  colspan=3 style='text-align:right;'> ";
                HtmlString += "---";
                HtmlString += "</td> ";

                HtmlString += "</tr> ";

                HtmlString += "</table> ";
                HtmlString += "</div> ";
                HtmlString += "</div> ";
                HtmlString += "</div> ";

                PanelPrint.Controls.Add(new LiteralControl(HtmlString));
                Session["ctrl"] = PanelPrint;
                ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('Print.aspx','PrintMe','height=900px,width=1200px,scrollbars=1');</script>");

            }
        }

        protected void BtnReport7_Click(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {
                alert_box.Visible = false;
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open(); 

                string EndDate = EntryDate10.Text;

                DateTime EndDateNew = DateTime.ParseExact(EndDate, "dd/mm/yyyy", CultureInfo.InvariantCulture);
                string EndDateQuery = EndDateNew.ToString("yyyy/mm/dd");
                string EndDateMonthQuery = EndDateNew.ToString("yyyy/mm");
                 
                string EndDateTemp = EndDate;
                string[] EndDateTempSplit = EndDateTemp.Split('-');
                String EndDateFormTemp = EndDateTempSplit[0].Replace("/", "-");
                DateTime EndMonthNew = DateTime.ParseExact(EndDateFormTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EndMonth = EndMonthNew.ToString("dd-MMMM-yyyy");
                string EndMonthName = EndMonthNew.ToString("MMMM-yyyy");

                HtmlString += "<div style='float:left;width:725px;height:auto;margin:0 0 0 40px;padding:10px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";
                HtmlString += "<div style='float:left;width:725px;height:80px;text-align:center;' ><img src='../../image/logo_from.png'/></div> ";
                HtmlString += "<div style='float:left;width:725px;height:25px;text-align:center;text-decoration: underline;' ><span style='font-family:Times New Roman,Times, serif;font-size:17px;font-weight:700;'>Metal Scrap Division</span></div> ";
                HtmlString += "<div style='float:left;width:725px;height:25px;text-align:center;text-decoration: underline;' ><span style='font-family:Times New Roman,Times, serif;font-size:16px;font-weight:700;'>Daily Material Collection Report : " + EndMonth + "</span></div> ";

              
                HtmlString += "</br></br> ";

                string makeSQL1 = " SELECT  WI.ITEM_CODE || ' : ' || WI.ITEM_NAME AS ITEM_NAME, sum(WPM.ITEM_WEIGHT) AS ITEM_WEIGHT FROM MS_PURCHASE_MASTER WPM LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WPM.ITEM_ID WHERE TO_CHAR(TO_DATE(WPM.ENTRY_DATE),'yyyy/mm/dd') = '" + EndDateQuery + "' GROUP BY WI.ITEM_CODE || ' : ' || WI.ITEM_NAME ORDER BY WI.ITEM_CODE || ' : ' || WI.ITEM_NAME ";

                cmdl = new OracleCommand(makeSQL1);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                HtmlString += "<table cellpadding='3px' cellspacing='0' style='font-size: 14px;' width=80% align='center'>";
                HtmlString += "<th style='border:black solid 1px; -webkit-border-top-left-radius:10px;width:85px;'>ITEM NAME</th> "; 
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;-webkit-border-top-right-radius:10px;'><span style='size:12px'>WEIGHT IN KG</th> ";

                for (int i = 0; i < RowCount; i++)
                {
                    ItemWtWbTotal += Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString()); 

                    HtmlString += "<tr valign='top'> ";

                    HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;'> ";
                    HtmlString += "" + dt.Rows[i]["ITEM_NAME"].ToString() + " ";
                    HtmlString += "</td> "; 
                    HtmlString += "<td style='text-align:right;border-bottom:black solid 1px;border-right:black solid 1px;'> ";
                    HtmlString += "" + string.Format("{0:n2}", dt.Rows[i]["ITEM_WEIGHT"]) + "</br> ";

                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";

                }
                HtmlString += "<tr valign='top'> ";
                HtmlString += "<td style='-webkit-border-bottom-left-radius:10px;border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;;font-weight:700;'> ";
                HtmlString += "Grand Total ";
                HtmlString += "</td> "; 
                HtmlString += "<td style='-webkit-border-bottom-right-radius:10px;text-align:right;border-bottom:black solid 1px;border-right:black solid 1px;;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n2}", ItemWtWbTotal) + "</br> ";
                HtmlString += "</td> ";
                HtmlString += "</tr> "; 
                HtmlString += "</table> "; 
                HtmlString += "</br></br> ";

                string makeSQL = " SELECT  WI.ITEM_CODE || ' : ' || WI.ITEM_NAME AS ITEM_NAME, sum(WPM.ITEM_WEIGHT) AS ITEM_WEIGHT FROM MS_PURCHASE_MASTER WPM LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WPM.ITEM_ID WHERE TO_CHAR(TO_DATE(WPM.ENTRY_DATE),'yyyy/mm') = '" + EndDateMonthQuery + "' GROUP BY WI.ITEM_CODE || ' : ' || WI.ITEM_NAME ORDER BY WI.ITEM_CODE || ' : ' || WI.ITEM_NAME  ";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                HtmlString += "<table cellpadding='3px' cellspacing='0' style='font-size: 14px;' width=80% align='center'>";
                HtmlString += "<th style='border:black solid 1px; -webkit-border-top-left-radius:10px;width:85px;'>ITEM NAME</th> "; 
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;-webkit-border-top-right-radius:10px;'><span style='size:12px'>WEIGHT IN KG</th> ";

                for (int i = 0; i < RowCount; i++)
                {
                    ItemWtTotal += Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());

                    HtmlString += "<tr valign='top'> "; 
                    HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;'> ";
                    HtmlString += "" + dt.Rows[i]["ITEM_NAME"].ToString() + " ";
                    HtmlString += "</td> ";  
                    HtmlString += "<td style='text-align:right;border-bottom:black solid 1px;border-right:black solid 1px;'> ";
                    HtmlString += "" + string.Format("{0:n2}", dt.Rows[i]["ITEM_WEIGHT"]) + "</br> "; 
                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";

                }
                HtmlString += "<tr valign='top'> ";  
                HtmlString += "<td style='-webkit-border-bottom-left-radius:10px;border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;;font-weight:700;'> ";
                HtmlString += "Grand Total in " + EndMonthName + " months:";
                HtmlString += "</td> ";

                HtmlString += "<td style='-webkit-border-bottom-right-radius:10px;text-align:right;border-bottom:black solid 1px;border-right:black solid 1px;;font-weight:700;'> ";
                HtmlString += "" + string.Format("{0:n2}", ItemWtTotal) + "</br> ";
                HtmlString += "</td> ";
                HtmlString += "</tr> ";
                HtmlString += "</table> ";
                HtmlString += "</br>  ";
                 

                HtmlString += "</div> ";
                HtmlString += "</div> ";
                HtmlString += "</div> ";

                PanelPrint.Controls.Add(new LiteralControl(HtmlString));
                Session["ctrl"] = PanelPrint;
                ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('Print.aspx','PrintMe','height=900px,width=1200px,scrollbars=1');</script>");

            }
        }
    } 
    }