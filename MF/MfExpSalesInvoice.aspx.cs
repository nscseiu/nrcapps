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

namespace NRCAPPS.MF
{
    public partial class MfExpSalesInvoice : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt, ds, dc, di;
        int RowCount, RowCount1, RowCount2, RowCount3; 
        double ItemVatAmt = 0.0, TotalItemWtWb = 0.0, TotalItemBundle=0.0, TotalItemPackWt=0.0, TotalItemWt = 0.0,  TotalInvoiceAmt = 0.0, TotalInvoiceSrAmt = 0.0, TotalInvoiceQty = 0.0, MatWeight = 0.0; string EntryDateSlip = "", PartyName = "", FullName =""; 
        string IS_PAGE_ACTIVE = "", IS_ADD_ACTIVE = "", IS_EDIT_ACTIVE = "", IS_DELETE_ACTIVE = "", IS_VIEW_ACTIVE = "", IS_REPORT_ACTIVE = "", IS_PRINT_ACTIVE = "";
        public bool IsLoad { get; set; }
        public bool IsLoad1 { get; set; }
        public bool IsLoad2 { get; set; }
        public bool IsLoad3 { get; set; }
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
                        DataTable dtPrintForID = new DataTable();
                        DataSet dip = new DataSet();
                        string makePrintForSQL = " SELECT INV_PRINT_FOR_ID, INV_PRINT_FOR_NAME FROM NRC_INVOICE_PRINT_FOR WHERE IS_ACTIVE = 'Enable' ORDER BY INV_PRINT_FOR_ID ASC";
                        dip = ExecuteBySqlString(makePrintForSQL);
                        dtPrintForID = (DataTable)dip.Tables[0];
                        DropDownInvPrintFor.DataSource = dtPrintForID;
                        DropDownInvPrintFor.DataValueField = "INV_PRINT_FOR_ID";
                        DropDownInvPrintFor.DataTextField = "INV_PRINT_FOR_NAME";
                        DropDownInvPrintFor.DataBind();
                  //      DropDownInvPrintFor.Items.Insert(0, new ListItem("Select Print For", "0"));

                        DataTable dtPackingTypeID = new DataTable();
                        DataSet dit = new DataSet();
                        string makePackingTypeSQL = " SELECT PACKING_TYPE_ID, PACKING_TYPE_NAME FROM NRC_PACKING_TYPE WHERE IS_ACTIVE = 'Enable' ORDER BY PACKING_TYPE_ID ASC";
                        dit = ExecuteBySqlString(makePackingTypeSQL);
                        dtPackingTypeID = (DataTable)dit.Tables[0];
                        DropDownPackingTypeID.DataSource = dtPackingTypeID;
                        DropDownPackingTypeID.DataValueField = "PACKING_TYPE_ID";
                        DropDownPackingTypeID.DataTextField = "PACKING_TYPE_NAME";
                        DropDownPackingTypeID.DataBind();

                        DataTable dtPartyID = new DataTable();
                        DataSet dsp = new DataSet();
                        string makePartySQL = " SELECT PARTY_ID,  PARTY_ID || ' - ' || PARTY_NAME || ' - ' || PARTY_VAT_NO AS PARTY_NAME  FROM MF_PARTY WHERE IS_ACTIVE = 'Enable' AND IS_PRODUCTION_ACTIVE = 'Enable' ORDER BY PARTY_NAME ASC";
                        dsp = ExecuteBySqlString(makePartySQL);
                        dtPartyID = (DataTable)dsp.Tables[0];
                        DropDownPartyID.DataSource = dtPartyID;
                        DropDownPartyID.DataValueField = "PARTY_ID";
                        DropDownPartyID.DataTextField = "PARTY_NAME";
                        DropDownPartyID.DataBind();
                        DropDownPartyID.Items.Insert(0, new ListItem("Select  Party Name", "0"));

                        DataTable dtPayTermID = new DataTable();
                        DataSet dse = new DataSet();
                        string makePayTermIDSQL = " SELECT PAY_TERMS_ID, PAY_TERMS_NAME || ' - ' || PAY_TERMS_DETAILS AS PAY_TERMS_NAME FROM NRC_PAYMENT_TERMS WHERE IS_ACTIVE = 'Enable' ORDER BY PAY_TERMS_ID ASC";
                        dse = ExecuteBySqlString(makePayTermIDSQL);
                        dtPayTermID = (DataTable)dse.Tables[0];
                        DropDownPayTermID.DataSource = dtPayTermID;
                        DropDownPayTermID.DataValueField = "PAY_TERMS_ID";
                        DropDownPayTermID.DataTextField = "PAY_TERMS_NAME";
                        DropDownPayTermID.DataBind();
                        DropDownPayTermID.Items.Insert(0, new ListItem("Select Payment Term", "0"));

                        DataTable dtShippingIncoID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeRoleSQL = " SELECT SHIPPING_INCO_ID, SHIPPING_INCO_NAME || ' - ' || SHIPPING_INCO_DETAILS AS SHIPPING_INCO_NAME FROM NRC_SHIPPING_INCOTERMS WHERE IS_ACTIVE = 'Enable' ORDER BY SHIPPING_INCO_ID ASC ";
                        ds = ExecuteBySqlString(makeRoleSQL);
                        dtShippingIncoID = (DataTable)ds.Tables[0];
                        DropDownShippingIncoID.DataSource = dtShippingIncoID;
                        DropDownShippingIncoID.DataValueField = "SHIPPING_INCO_ID";
                        DropDownShippingIncoID.DataTextField = "SHIPPING_INCO_NAME";
                        DropDownShippingIncoID.DataBind();
                        DropDownShippingIncoID.Items.Insert(0, new ListItem("Select Shipping Incoterms", "0"));
                        DropDownShippingIncoID.Items.FindByValue("2").Selected = true;

                        DataTable dtTradingVesselID = new DataTable();
                        DataSet dvs = new DataSet();
                        string makeTvSQL = " SELECT TRADING_VESSEL_ID, TRADING_VESSEL_NAME || ' - ' || TRADING_VESSEL_DETAILS AS TRADING_VESSEL_NAME FROM NRC_TRADING_VESSEL WHERE IS_ACTIVE = 'Enable' ORDER BY TRADING_VESSEL_NAME ASC ";
                        dvs = ExecuteBySqlString(makeTvSQL);
                        dtTradingVesselID = (DataTable)dvs.Tables[0];
                        DropDownTradingVesselID.DataSource = dtTradingVesselID;
                        DropDownTradingVesselID.DataValueField = "TRADING_VESSEL_ID";
                        DropDownTradingVesselID.DataTextField = "TRADING_VESSEL_NAME";
                        DropDownTradingVesselID.DataBind();
                        DropDownTradingVesselID.Items.Insert(0, new ListItem("Select Trading Vessel", "0"));
  
                        DataTable dtSlipNo = new DataTable();
                        DataSet dss = new DataSet();
                        string makePageSQL = " SELECT PEWC.WB_SLIP_NO, PEWC.WB_SLIP_NO || ' - ' || PC.PARTY_ID || ' - ' || PC.PARTY_NAME || ', Container No. - ' || PEWC.CONTAINER_NO || ', Item - ' || PI.ITEM_CODE || ' : ' || PI.ITEM_NAME || ', Item WT(WB) -' || TO_CHAR(PEWC.ITEM_WEIGHT_WB, '999,999,999') || ', Item WT -' || TO_CHAR((PEWC.ITEM_WEIGHT/1000), '999999.999') AS PARTY_NAME FROM MF_EXPORT_WBSLIP_CON PEWC LEFT JOIN MF_PARTY PC ON PC.PARTY_ID = PEWC.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PEWC.ITEM_ID WHERE PEWC.EXPORT_INVOICE_NO IS NULL AND PEWC.IS_ACTIVE_PRICING = 'Enable' ORDER BY PEWC.WB_SLIP_NO ASC";
                        dss = ExecuteBySqlString(makePageSQL);
                        dtSlipNo = (DataTable)dss.Tables[0];
                        DropDownSlipNoEx.DataSource = dtSlipNo;
                        DropDownSlipNoEx.DataValueField = "WB_SLIP_NO";
                        DropDownSlipNoEx.DataTextField = "PARTY_NAME";
                        DropDownSlipNoEx.DataBind();


                        DataTable dtShipmentFTID = new DataTable();
                        DataSet dsft = new DataSet();
                        string makeDropSQL = "  SELECT NSFT.SHIPMENT_FT_ID, NSLF.SHIPMENT_LOC_NAME || ' - ' || NSLT.SHIPMENT_LOC_NAME AS SHIPMENT_LOC_NAME FROM NRC_SHIPMENT_FROM_TO NSFT LEFT JOIN NRC_SHIPMENT_LOCATION NSLF ON NSLF.SHIPMENT_LOC_ID = NSFT.SHIPMENT_FROM_ID LEFT JOIN NRC_SHIPMENT_LOCATION NSLT ON NSLT.SHIPMENT_LOC_ID = NSFT.SHIPMENT_TO_ID WHERE NSFT.IS_ACTIVE = 'Enable' ORDER BY NSFT.SHIPMENT_FT_ID ASC";
                        dsft = ExecuteBySqlString(makeDropSQL);
                        dtShipmentFTID = (DataTable)dsft.Tables[0];
                        DropDownShipmentFromToID.DataSource = dtShipmentFTID;
                        DropDownShipmentFromToID.DataValueField = "SHIPMENT_FT_ID";
                        DropDownShipmentFromToID.DataTextField = "SHIPMENT_LOC_NAME";
                        DropDownShipmentFromToID.DataBind();
                        DropDownShipmentFromToID.Items.Insert(0, new ListItem("Select Shipment From & To", "0"));

                        DataTable dtAccountID = new DataTable();
                        DataSet dsa = new DataSet();
                        string makeDropAccSQL = "  SELECT ACCOUNT_ID, ACCOUNT_NO || ' - ' || BANK_NAME AS BANK_NAME FROM NRC_COMPANY_ACCOUNT_INFO WHERE IS_ACTIVE = 'Enable' ORDER BY ACCOUNT_ID ASC";
                        dsa = ExecuteBySqlString(makeDropAccSQL);
                        dtAccountID = (DataTable)dsa.Tables[0];
                        DropDownAccountID.DataSource = dtAccountID;
                        DropDownAccountID.DataValueField = "ACCOUNT_ID";
                        DropDownAccountID.DataTextField = "BANK_NAME";
                        DropDownAccountID.DataBind();
                    //    DropDownAccountID.Items.Insert(0, new ListItem("Select Bank Account", "0"));

                        //   TextExportInvoiceNo.Enabled = false;
                        Display();
                        DisplayContract();

                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled");
                     //   contract_id.Enabled = false;

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
        //       {  
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]); 
                    int PayTermID = Convert.ToInt32(DropDownPayTermID.Text); 
                    int ShippingIncoID = Convert.ToInt32(DropDownShippingIncoID.Text);
                    int TradingVesselID = Convert.ToInt32(DropDownTradingVesselID.Text);
                    int ShipmentFromToID = Convert.ToInt32(DropDownShipmentFromToID.Text);
                    int PartyID = Convert.ToInt32(DropDownPartyID.Text);  
                    int PackingTypeID = Convert.ToInt32(DropDownPackingTypeID.Text);  
                    int AccountID = Convert.ToInt32(DropDownAccountID.Text);  
                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-'); 
                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-"); 
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                    string MakeEntryDate2 = EntryDate2.Text;
                    string[] MakeEntryDateSplit2 = MakeEntryDate2.Split('-');
                    String EntryDateTemp2 = MakeEntryDateSplit2[0].Replace("/", "-");
                    DateTime EntryDateNewD2 = DateTime.ParseExact(EntryDateTemp2, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew2 = EntryDateNewD2.ToString("dd-MM-yyyy");

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string IsShipMarksActive = CheckIsShipMarksActive.Checked ? "Enable" : "Disable";
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string get_id = "select MF_EXPORT_SALES_MASTER_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_id, conn);
                    int newID = Int32.Parse(cmdu.ExecuteScalar().ToString()); 

                    foreach (ListItem li in DropDownSlipNoEx.Items)
                    {

                        if (li.Selected == true)
                        {
                            string[] WbSlipNo = li.Value.Split('-');
                            string update_ex_invoice = " update  MF_EXPORT_WBSLIP_CON set EXPORT_INVOICE_NO =:NoExportInvoiceNo, IS_CONFIRM_CHECK =:IsConfirm,  IS_CONFIRM_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , IS_CONFIRM_ID = :NoCuserID  where WB_SLIP_NO =: NoWbSlipNo ";
                            cmdi = new OracleCommand(update_ex_invoice, conn);

                            OracleParameter[] objPrm = new OracleParameter[5];
                            objPrm[0] = cmdi.Parameters.Add("NoWbSlipNo", WbSlipNo[0]);
                            objPrm[1] = cmdi.Parameters.Add("NoExportInvoiceNo", TextExportInvoiceNo.Text);
                            objPrm[2] = cmdi.Parameters.Add("IsConfirm", "Complete");
                            objPrm[3] = cmdi.Parameters.Add("u_date", c_date);
                            objPrm[4] = cmdi.Parameters.Add("NoCuserID", userID);

                            cmdi.ExecuteNonQuery();
                        }
                    } 
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    string insert_ex_sales = "insert into MF_EXPORT_SALES_MASTER (EXPORT_SALES_ID, EXPORT_INVOICE_NO, BL_NO, PO_NO, REF_NO, PACKING_TYPE_ID, PAY_TERMS_ID, SHIPPING_INCO_ID, TRADING_VESSEL_ID, TRADING_VESSEL_CODE, SHIPMENT_DATE, SHIPMENT_FROM_TO_ID, PARTY_ID, INVOICE_DATE, PAYMENT_TERMS_DES, ACCOUNT_ID, CREATE_DATE,  C_USER_ID, IS_SHIPPING_MARKS_ACTIVE, IS_ACTIVE) VALUES ( :NoExportSalesID, :TextExportInvoiceNo, :TextBl, :TextPoNo, :TextRef, :NoPackingTypeID, :NoPayTermID, :NoShippingIncoID, :NoTradingVesselID, :TextVesselCode, TO_DATE(:ShipmentDate, 'DD/MM/YYYY'), :NoShipmentFromToID, :NoPartyID, TO_DATE(:InvoiceDate, 'DD/MM/YYYY'), :TextPayTermDes, :NoAccountID, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'),  :NoCuserID, :TextIsShipMarksActive, :TextIsActive)";
                    cmdi = new OracleCommand(insert_ex_sales, conn); 
                    OracleParameter[] objPr = new OracleParameter[21];
                    objPr[0] = cmdi.Parameters.Add("NoExportSalesID", newID);
                    objPr[1] = cmdi.Parameters.Add("TextExportInvoiceNo", TextExportInvoiceNo.Text);
                    objPr[2] = cmdi.Parameters.Add("TextBl", TextBl.Text);
                    objPr[3] = cmdi.Parameters.Add("TextPoNo", TextPoNo.Text);
                    objPr[4] = cmdi.Parameters.Add("TextRef", TextRef.Text);
                    objPr[5] = cmdi.Parameters.Add("NoPackingTypeID", PackingTypeID); 
                    objPr[6] = cmdi.Parameters.Add("NoPayTermID", PayTermID);
                    objPr[7] = cmdi.Parameters.Add("TextPayTermDes", TextPayTermDes.Text);
                    objPr[8] = cmdi.Parameters.Add("NoShippingIncoID", ShippingIncoID);
                    objPr[9] = cmdi.Parameters.Add("NoTradingVesselID", TradingVesselID);
                    objPr[10] = cmdi.Parameters.Add("TextVesselCode", TextVesselCode.Text); 
                    objPr[11] = cmdi.Parameters.Add("ShipmentDate", EntryDateNew); 
                    objPr[12] = cmdi.Parameters.Add("NoShipmentFromToID", ShipmentFromToID); 
                    objPr[13] = cmdi.Parameters.Add("NoPartyID", PartyID);
                    objPr[14] = cmdi.Parameters.Add("InvoiceDate", EntryDateNew2); 
                    objPr[15] = cmdi.Parameters.Add("TextPayTermDes", TextPayTermDes.Text); 
                    objPr[16] = cmdi.Parameters.Add("c_date", c_date); 
                    objPr[17] = cmdi.Parameters.Add("NoAccountID", AccountID);
                    objPr[18] = cmdi.Parameters.Add("TextIsShipMarksActive", IsShipMarksActive);
                    objPr[19] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPr[20] = cmdi.Parameters.Add("NoCuserID", userID);
                    cmdi.ExecuteNonQuery(); 
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();  
                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert Export Sales Invoice Data Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                    clearText();
                    Display();
                }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
            //    }
          //   catch
          //    {
         //     Response.Redirect("~/ParameterError.aspx");
         //    }  
        }


        public void BtnAddContract_Click(object sender, EventArgs e)
        {
          try
               {  
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open(); 
                    int userID = Convert.ToInt32(Session["USER_ID"]);  
                      
                    string MakeEntryDate1 = EntryDate1.Text;
                    string[] MakeEntryDateSplit1 = MakeEntryDate1.Split('-'); 
                    String EntryDateTemp1 = MakeEntryDateSplit1[0].Replace("/", "-"); 
                    DateTime EntryDateNewD1 = DateTime.ParseExact(EntryDateTemp1, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew1 = EntryDateNewD1.ToString("dd-MM-yyyy");
                     
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string get_id = "select MF_EXPORT_CONTRACT_NO_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_id, conn);
                    int newID = Int32.Parse(cmdu.ExecuteScalar().ToString()); 
                     
                    string insert_ex_sales = "insert into MF_EXPORT_CONTRACT_NO (EXPORT_CONTRACT_NO_ID, CONTRACT_NO, CONTRACT_NO_SERIAL, CONTRACT_DATE, EXPORT_INVOICE_NO, CREATE_DATE, C_USER_ID) VALUES ( :NoContractNoID,  :TextContractNo, :TextContractNoSerial, TO_DATE(:ContractDate, 'DD/MM/YYYY'), :TextExportInvoiceNo, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'),  :NoCuserID)";
                    cmdi = new OracleCommand(insert_ex_sales, conn); 
                    OracleParameter[] objPr = new OracleParameter[7];
                    objPr[0] = cmdi.Parameters.Add("NoContractNoID", newID); 
                    objPr[1] = cmdi.Parameters.Add("TextContractNo", TextContractNo.Text);
                    objPr[2] = cmdi.Parameters.Add("TextContractNoSerial", TextContractNoSerial.Text);
                    objPr[3] = cmdi.Parameters.Add("ContractDate", EntryDateNew1); 
                    objPr[4] = cmdi.Parameters.Add("TextExportInvoiceNo", TextExportInvoiceNo.Text); 
                    objPr[5] = cmdi.Parameters.Add("c_date", c_date);
                    objPr[6] = cmdi.Parameters.Add("NoCuserID", userID); 

                    cmdi.ExecuteNonQuery(); 
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();  
                    conn.Close(); 

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert Contract Data Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                 //   clearText();
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


        protected void btnPrint_Click(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            string HtmlString = "";
            string makeSQL = " SELECT  PESM.EXPORT_SALES_ID, PESM.EXPORT_INVOICE_NO, PESM.BL_NO, PESM.PO_NO, PESM.REF_NO, PESM.PAYMENT_TERMS_DES, PESM.IS_SHIPPING_MARKS_ACTIVE, NPT.PAY_TERMS_NAME, NSI.SHIPPING_INCO_NAME, NTV.TRADING_VESSEL_NAME, PESM.TRADING_VESSEL_CODE,  TO_CHAR(PESM.SHIPMENT_DATE,'dd-MON-yyyy') AS SHIPMENT_DATE, TO_CHAR(PESM.INVOICE_DATE,'dd-Mon-yyyy') AS INVOICE_DATE,  NSLF.SHIPMENT_LOC_NAME AS SHIPMENT_FROM_NAME, NSLT.SHIPMENT_LOC_NAME AS SHIPMENT_TO_NAME, PESM.CREATE_DATE, PESM.UPDATE_DATE, PESM.IS_ACTIVE, PESM.IS_PRINT, PESM.PRINT_DATE, PP.PARTY_NAME, PP.PARTY_ADD_1, PP.PARTY_ADD_2, PP.PARTY_VAT_NO, PP.CONTACT_NO, NCAI.ACCOUNT_NO, NCAI.ACCOUNT_NAME, NCAI.BANK_NAME, NCAI.IBAN, NCAI.SWIFT, NPTY.PACKING_TYPE_NAME FROM MF_EXPORT_SALES_MASTER PESM LEFT JOIN NRC_PAYMENT_TERMS NPT ON NPT.PAY_TERMS_ID = PESM.PAY_TERMS_ID LEFT JOIN NRC_SHIPPING_INCOTERMS NSI ON NSI.SHIPPING_INCO_ID = PESM.SHIPPING_INCO_ID LEFT JOIN NRC_TRADING_VESSEL NTV ON NTV.TRADING_VESSEL_ID = PESM.TRADING_VESSEL_ID  LEFT JOIN NRC_SHIPMENT_FROM_TO NSRT ON NSRT.SHIPMENT_FT_ID = PESM.SHIPMENT_FROM_TO_ID LEFT JOIN NRC_SHIPMENT_LOCATION NSLF ON NSLF.SHIPMENT_LOC_ID = NSRT.SHIPMENT_FROM_ID LEFT JOIN NRC_SHIPMENT_LOCATION NSLT ON NSLT.SHIPMENT_LOC_ID = NSRT.SHIPMENT_TO_ID  LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = PESM.PARTY_ID LEFT JOIN NRC_COMPANY_ACCOUNT_INFO NCAI ON NCAI.ACCOUNT_ID = PESM.ACCOUNT_ID LEFT JOIN NRC_PACKING_TYPE NPTY ON NPTY.PACKING_TYPE_ID = PESM.PACKING_TYPE_ID WHERE PESM.EXPORT_INVOICE_NO = '" + TextExportInvoiceNo.Text + "' ORDER BY PESM.EXPORT_INVOICE_NO DESC ";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;

            string makeContractSQL = " SELECT CONTRACT_NO, CONTRACT_NO_SERIAL, TO_CHAR(CONTRACT_DATE, 'DD-MON-YYYY') AS CONTRACT_DATE FROM MF_EXPORT_CONTRACT_NO WHERE EXPORT_INVOICE_NO = '" + TextExportInvoiceNo.Text + "' ";
            cmdl = new OracleCommand(makeContractSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dc = new DataTable();
            oradata.Fill(dc);
            RowCount1 = dc.Rows.Count;

            string makeContainerSQL = " SELECT PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, NCS.CONTAINER_SIZE_INWORDS, PSI.ITEM_SALES_DESCRIPTION, (PEWC.ITEM_WEIGHT / 1000) AS ITEM_WEIGHT, PEWC.MAT_PRICE_PER_MT, PEWC.MATERIAL_AMOUNT, (PEWC.CURRENCY_RATE * PEWC.MAT_PRICE_PER_MT) AS MAT_CONVERSION_PRICE_PER_MT, PEWC.MATERIAL_CONVERSION_AMOUNT FROM MF_EXPORT_WBSLIP_CON PEWC LEFT JOIN MF_ITEM_SALES PSI ON PSI.ITEM_SALES_ID = PEWC.ITEM_SALES_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID LEFT JOIN MF_EXPORT_SALES_MASTER PESM ON PESM.EXPORT_INVOICE_NO = PEWC.EXPORT_INVOICE_NO LEFT JOIN NRC_CURRENCY_RATE NCR ON NCR.CURRENCY_RATE_ID = PEWC.CURRENCY_RATE_ID LEFT JOIN NRC_CURRENCY NCS ON NCS.CURRENCY_ID = NCR.SOURCE_CURRENCY_ID LEFT JOIN NRC_CURRENCY NCT ON NCT.CURRENCY_ID = NCR.TARGET_CURRENCY_ID WHERE PESM.EXPORT_INVOICE_NO = '" + TextExportInvoiceNo.Text + "' ";
            cmdl = new OracleCommand(makeContainerSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            ds = new DataTable();
            oradata.Fill(ds);
            RowCount2 = ds.Rows.Count;

            //string makeItemSQL = " SELECT PSI.ITEM_SALES_DESCRIPTION, SUM((PEWC.ITEM_WEIGHT / 1000)) AS ITEM_WEIGHT, PEWC.MAT_PRICE_PER_MT, SUM(PEWC.MATERIAL_AMOUNT) AS MATERIAL_AMOUNT, PEWC.CURRENCY_RATE, SUM((PEWC.CURRENCY_RATE * PEWC.MAT_PRICE_PER_MT)) AS MAT_CONVERSION_PRICE_PER_MT, SUM(PEWC.MATERIAL_CONVERSION_AMOUNT) AS MATERIAL_CONVERSION_AMOUNT FROM MF_EXPORT_WBSLIP_CON PEWC LEFT JOIN MF_ITEM_SALES PSI ON PSI.ITEM_SALES_ID = PEWC.ITEM_SALES_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID LEFT JOIN MF_EXPORT_SALES_MASTER PESM ON PESM.EXPORT_INVOICE_NO = PEWC.EXPORT_INVOICE_NO LEFT JOIN NRC_CURRENCY_RATE NCR ON NCR.CURRENCY_RATE_ID = PEWC.CURRENCY_RATE_ID LEFT JOIN NRC_CURRENCY NCS ON NCS.CURRENCY_ID = NCR.SOURCE_CURRENCY_ID LEFT JOIN NRC_CURRENCY NCT ON NCT.CURRENCY_ID = NCR.TARGET_CURRENCY_ID WHERE PESM.EXPORT_INVOICE_NO = '" + TextExportInvoiceNo.Text + "' GROUP BY PSI.ITEM_SALES_DESCRIPTION, PEWC.MAT_PRICE_PER_MT, PEWC.CURRENCY_RATE  ";

            string makeItemSQL = " SELECT PSI.ITEM_SALES_DESCRIPTION, NCS.CONTAINER_SIZE, (PEWC.ITEM_WEIGHT_WB / 1000) AS ITEM_WEIGHT_WB, (PEWC.PACKING_WEIGHT / 1000) AS PACKING_WEIGHT, (PEWC.ITEM_WEIGHT / 1000) AS ITEM_WEIGHT, PEWC.BUNDLE, PEWC.MAT_PRICE_PER_MT, PEWC.MATERIAL_AMOUNT AS MATERIAL_AMOUNT, PEWC.CURRENCY_RATE, (PEWC.CURRENCY_RATE * PEWC.MAT_PRICE_PER_MT) AS MAT_CONVERSION_PRICE_PER_MT, PEWC.MATERIAL_CONVERSION_AMOUNT AS MATERIAL_CONVERSION_AMOUNT, NPL.PACKING_NAME FROM MF_EXPORT_WBSLIP_CON PEWC LEFT JOIN MF_ITEM_SALES PSI ON PSI.ITEM_SALES_ID = PEWC.ITEM_SALES_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID LEFT JOIN MF_EXPORT_SALES_MASTER PESM ON PESM.EXPORT_INVOICE_NO = PEWC.EXPORT_INVOICE_NO LEFT JOIN NRC_CURRENCY_RATE NCR ON NCR.CURRENCY_RATE_ID = PEWC.CURRENCY_RATE_ID LEFT JOIN NRC_CURRENCY NCS ON NCS.CURRENCY_ID = NCR.SOURCE_CURRENCY_ID LEFT JOIN NRC_CURRENCY NCT ON NCT.CURRENCY_ID = NCR.TARGET_CURRENCY_ID LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = PEWC.PACKING_ID WHERE PESM.EXPORT_INVOICE_NO = '" + TextExportInvoiceNo.Text + "'   ";
            cmdl = new OracleCommand(makeItemSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            di = new DataTable();
            oradata.Fill(di);
            RowCount3 = di.Rows.Count;

            List<string> ItemDeslist = new List<string>();
            List<string> ContainerSizelist = new List<string>();
            List<string> ItempRatePer = new List<string>();
            for (int x = 0; x < RowCount2; x++)
            {
                ItemDeslist.Add(di.Rows[x]["ITEM_SALES_DESCRIPTION"].ToString());
                ContainerSizelist.Add(di.Rows[x]["CONTAINER_SIZE"].ToString());
                //   ItempRatePer.Add(ds.Rows[x]["MAT_PRICE_PER_MT"].ToString());
                MatWeight += Convert.ToDouble(ds.Rows[x]["ITEM_WEIGHT"].ToString());

            }
            List<string> ItemDeslistDis = ItemDeslist.Distinct().ToList();
            List<string> ContainerSizelistDis = ContainerSizelist.Distinct().ToList();
            List<string> ItemRatelistPer = ItempRatePer.Distinct().ToList();
            int ItemDeslistDisCount = ItemDeslistDis.Count;
            int ContainerSizelistDisCount = ContainerSizelistDis.Count;
            int ItemRatelistPerCount = ItemRatelistPer.Count;

            HtmlString += "<div style='float:left;width:785px;height:auto;margin:30px 0 0 40px;'> ";

            if (DropDownInvPrintFor.Text == "1")
            {
                for (int i = 0; i < RowCount; i++)
                {

                    HtmlString += "<div style='float:left;width:757px;height:auto;margin-top:50px;font-family: Tahoma, Arial, Courier, Lucida Typewriter, monospace; font-size: 12px; font-style: normal; font-variant: normal; font-weight: 400; line-height: 16px;'> ";

                    HtmlString += "<div style='float:left;width:720px;height:20px; margin:30px 5px 0 40px;text-align:center;border-bottom: black solid 1px;' ><span style='font-family:Tahoma;font-size:15px;font-weight:700;'>INVOICE</span></div> ";
                    HtmlString += "<div style='float:left;width:375px;height:20px; margin:5px 0 0 40px;'><span style='font-family:Tahoma, Arial,Times, serif;font-size:12px;'></span></div> ";
                    HtmlString += "<div style='float:left;width:340px;height:20px; margin-top:5px;text-align:right;'><span style='font-family:Tahoma;font-size:12px;'>VAT NO.: 300507825100003</span></div> ";

                    HtmlString += "<div style='float:left;width:375px;height:40px; margin:0 0 0 40px;'>INVOICE NO: <span style='font-weight:700;font-size:12px;'>" + dt.Rows[i]["EXPORT_INVOICE_NO"].ToString() + " </span></div> ";
                    HtmlString += "<div style='float:left;width:340px;height:40px;text-align:right;'><span style='font-size:12px;'>DATE: <span style='font-weight:700;'>" + dt.Rows[i]["INVOICE_DATE"].ToString() + "</span></span></div> ";

                    HtmlString += "<div style='float:left;width:720px;height:auto;margin:0 0 0 40px;font-size: 11px; font-style: normal; font-variant: normal; font-weight:400; line-height: 16px;'> ";
                    HtmlString += "<table cellpadding='2px' cellspacing='0' style='font-size:11px;'>";
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px'><span style='font-weight:700;line-height: 1.6;'>" + dt.Rows[i]["PARTY_NAME"].ToString() + "</br>" + dt.Rows[i]["PARTY_ADD_1"].ToString() + "</br>" + dt.Rows[i]["PARTY_ADD_2"].ToString() + "</br>TEL NO: " + dt.Rows[i]["CONTACT_NO"].ToString() + "</span></td> ";
                    HtmlString += "<td></td> ";
                    HtmlString += "<td></td> ";
                    HtmlString += "<td></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>&nbsp;</td>";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "</table>";

                    HtmlString += "<table cellpadding='2px' cellspacing='0' style='font-size:11px;' width=100%>";
                    HtmlString += "<tr> ";
                    HtmlString += "<td  style='border-left:black solid 1px'><span style='font-weight:700;'>BL NO:</span></td> ";
                    HtmlString += "<td  style=''>" + dt.Rows[i]["BL_NO"].ToString() + "</td> ";
                    HtmlString += "<td  style='border-left:black solid 1px'><span style='font-weight:700;'>SALES ORDER NO:</span></td> ";
                    HtmlString += "<td  style=''>";
                    int m = 1;
                    for (int n = 0; n < RowCount1; n++)
                    {
                        string ContractNo = dc.Rows[n]["CONTRACT_NO"].ToString();
                        string ContractDate = dc.Rows[n]["CONTRACT_DATE"].ToString();
                        if (m == RowCount1)
                        {
                            HtmlString += "" + ContractNo + "";
                        }
                        else { HtmlString += "" + ContractNo + " AND "; }
                        m++;
                    }
                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td  style='border-left:black solid 1px'><span style='font-weight:700;'>VESSEL NAME:</span></td> ";
                    HtmlString += "<td  style=''>" + dt.Rows[i]["TRADING_VESSEL_NAME"].ToString() + " " + dt.Rows[i]["TRADING_VESSEL_CODE"].ToString() + "</td> ";
                    HtmlString += "<td  style='border-left:black solid 1px'><span style='font-weight:700;'>SALES ORDER DATE:</span></td> ";
                    HtmlString += "<td  style=''>";
                    int o = 1;
                    for (int n = 0; n < RowCount1; n++)
                    {
                        string ContractNo = dc.Rows[n]["CONTRACT_NO"].ToString();
                        string ContractDate = dc.Rows[n]["CONTRACT_DATE"].ToString();
                        if (o == RowCount1)
                        {
                            HtmlString += "" + ContractDate + "";
                        }
                        else { HtmlString += "" + ContractDate + " AND "; }
                        o++;
                    }
                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td  style='border-left:black solid 1px'><span style='font-weight:700;'>POL & POD:</span></td> ";
                    HtmlString += "<td  style=''>" + dt.Rows[i]["SHIPMENT_FROM_NAME"].ToString() + " TO " + dt.Rows[i]["SHIPMENT_TO_NAME"].ToString() + "</td> ";
                    HtmlString += "<td  style='border-left:black solid 1px'><span style='font-weight:700;'>PO NO:</span></td> ";
                    HtmlString += "<td  style=''>" + dt.Rows[i]["PO_NO"].ToString() + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td  style='border-left:black solid 1px'><span style='font-weight:700;'>SHIPPED ON:</span></td> ";
                    HtmlString += "<td  style=''>" + dt.Rows[i]["SHIPMENT_DATE"].ToString() + "</td> ";
                    HtmlString += "<td  style='border-left:black solid 1px'><span style='font-weight:700;'>REF.:</span></td> ";
                    HtmlString += "<td  style=''>" + dt.Rows[i]["REF_NO"].ToString() + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td  style='border-left:black solid 1px'><span style='font-weight:700;'>UNITS:</span></td> ";
                    HtmlString += "<td  style=''>" + decimal.Parse(RowCount2.ToString()).ToString("00.") + "x";
                    for (int l = 0; l < 1; l++)
                    {
                        HtmlString += "" + ds.Rows[l]["CONTAINER_SIZE"].ToString() + "'</br> ";
                    }
                    HtmlString += "</td> ";
                    HtmlString += "<td  style='border-left:black solid 1px'><span style='font-weight:700;'></span></td> ";
                    HtmlString += "<td  style=''></td> ";
                    HtmlString += "</tr> ";

                    HtmlString += "</table>";
                    HtmlString += "</BR></BR>";
                    HtmlString += "</div>";


                    HtmlString += "<div style='float:left;width:720px;height:auto;margin:0 0 0 40px;-webkit-border-top-left-radius:10px;-webkit-border-top-right-radius:10px;font-family: Tahoma, Arial Narrow, Courier, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight:400; line-height: 16px;'> ";

                    HtmlString += "<table cellpadding='3px' cellspacing='0' style='font-size:11px'  width=100%>";
                    HtmlString += "<th style='border:black solid 1px;'>CONTAINER (S)</th> ";
                    HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>SIZE</th> ";
                    HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>DESCRIPTION OF MATERIALS</th> ";
                    HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'> GROSS WEIGHT (MT)</th> ";
                    HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'> NET. WEIGHT (MT)</th> ";
                    HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;'>PRICE PER MT (USD)</th> ";
                    HtmlString += "<th style='border:black solid 1px;'>AMOUNT (USD) </BR>" + dt.Rows[i]["SHIPPING_INCO_NAME"].ToString() + " " + dt.Rows[i]["SHIPMENT_TO_NAME"].ToString() + "</th> ";


                    HtmlString += "<tr valign='top'> ";

                    HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;'> ";
                    for (int l = 0; l < RowCount2; l++)
                    {
                        HtmlString += "" + ds.Rows[l]["CONTAINER_NO"].ToString() + "</br> ";
                    }
                    HtmlString += "</td> ";

                    HtmlString += "<td rowspan='1' style='border-right:black solid 1px;text-align:center;vertical-align: middle;'> ";

                    for (int d = 0; d < ContainerSizelistDisCount; d++)
                    {
                        HtmlString += "" + ContainerSizelistDis[d].ToString() + "'</br>";
                    }

                    HtmlString += "</td> ";

                    HtmlString += "<td rowspan='1' style='border-right:black solid 1px;text-align:center;vertical-align: middle;'> ";  // " + Convert.ToInt32(RowCount2 - 1) + "

                    for (int d = 0; d < ItemDeslistDisCount; d++)
                    {
                        HtmlString += "" + ItemDeslistDis[d].ToString() + "</br>";
                    }

                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black solid 1px;text-align:center;'> ";
                    for (int p = 0; p < RowCount3; p++)
                    {
                        HtmlString += "" + decimal.Parse(di.Rows[p]["ITEM_WEIGHT_WB"].ToString()).ToString(".000") + "</br> ";
                    }
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black solid 1px;text-align:center;'> ";
                    for (int p = 0; p < RowCount3; p++)
                    {
                        HtmlString += "" + decimal.Parse(di.Rows[p]["ITEM_WEIGHT"].ToString()).ToString(".000") + "</br> ";
                    }
                    HtmlString += "</td> ";

                    HtmlString += "<td style='text-align:center;'> ";
                    for (int p = 0; p < RowCount3; p++)
                    {
                        HtmlString += "" + di.Rows[p]["MAT_PRICE_PER_MT"].ToString() + "</br> ";
                    }
                    HtmlString += "</td> ";
                    HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;text-align:center;'> ";
                    for (int p = 0; p < RowCount3; p++)
                    {
                        HtmlString += "" + string.Format("{0:n2}", Convert.ToDouble(di.Rows[p]["MATERIAL_AMOUNT"])) + "</br> ";
                        TotalItemWtWb += Convert.ToDouble(di.Rows[p]["ITEM_WEIGHT_WB"].ToString());
                        TotalItemWt += Convert.ToDouble(di.Rows[p]["ITEM_WEIGHT"].ToString());
                        TotalInvoiceAmt += Convert.ToDouble(di.Rows[p]["MATERIAL_AMOUNT"].ToString());
                        TotalInvoiceSrAmt = Convert.ToDouble(di.Rows[p]["CURRENCY_RATE"].ToString()) * TotalInvoiceAmt;
                    }
                    HtmlString += "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td colspan='3' style='border-right:black solid 1px;text-align:center;border-top:black solid 1px;border-bottom:black solid 1px;border-left:black solid 1px; text-align:right;font-weight:700;'> TOTAL: </td> ";
                    HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'>" + decimal.Parse(TotalItemWtWb.ToString()).ToString(".000") + "</td>";
                    HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;font-weight:700;'>" + decimal.Parse(TotalItemWt.ToString()).ToString(".000") + "</td> ";
                    HtmlString += "<td style='border-top:black solid 1px;border-bottom:black solid 1px;text-align:center;'>&nbsp;</td>";
                    HtmlString += "<td  style='border-top:black solid 1px;border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;font-weight:700;'> " + string.Format("{0:n2}", TotalInvoiceAmt) + " </td>";
                    HtmlString += "</tr> ";
                    string NumberToInWordUSD = NumberToInWords.DecimalToWordsUSD(Convert.ToDecimal(TotalInvoiceAmt)).Trim().ToUpper();
                    HtmlString += "<tr> ";
                    HtmlString += "<td colspan='7' style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'> " + NumberToInWordUSD + " </td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr>";
                    HtmlString += "<td colspan='6' style=''></td> ";
                    HtmlString += "<td style='text-align:center;'>SAR " + string.Format("{0:n2}", TotalInvoiceSrAmt) + "</td> ";
                    HtmlString += "</tr>";
                    HtmlString += "</table>";
                    HtmlString += "</BR>";
                    HtmlString += "<table cellpadding='2px' cellspacing='0' style='font-size:11px'>";
                    //if shipping marks is enable start
                    if (dt.Rows[i]["IS_SHIPPING_MARKS_ACTIVE"].ToString() == "Enable") {
                        HtmlString += "<tr> ";
                        HtmlString += "<td style='border-left:black solid 1px'><span style='font-weight:700;'>SHIPPING MARKS:</span></td> ";
                        HtmlString += "<td><span style='line-height: 1.6;'>";
                        for (int d = 0; d < ItemDeslistDisCount; d++)
                        {
                            HtmlString += "" + ItemDeslistDis[d].ToString() + "</br>";
                        }
                        HtmlString += " ORIGIN OF SAUDI ARABIA</span></td> ";
                        HtmlString += "</tr> ";
                    }
                    //if shipping marks is enable end
                    HtmlString += "<tr> ";
                    HtmlString += "<td>&nbsp;</td>";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px'><span style='font-weight:700;'>PAYMENT TERMS:</span></td>";
                    HtmlString += "<td><span style='line-height: 1.6;'>" + dt.Rows[i]["PAYMENT_TERMS_DES"].ToString() + "</span></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>&nbsp;</td>";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px'><span style='font-weight:700;'>OUR BANK DETAILS:</span></td>";
                    HtmlString += "<td></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px'>BANK:</span></td>";
                    HtmlString += "<td>" + dt.Rows[i]["BANK_NAME"].ToString() + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px'>ACCOUNT NAME:</span></td>";
                    HtmlString += "<td>" + dt.Rows[i]["ACCOUNT_NAME"].ToString() + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px'>ACCOUNT NO:</span></td>";
                    HtmlString += "<td>" + dt.Rows[i]["ACCOUNT_NO"].ToString() + "</td>";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px'>IBAN:</span></td>";
                    HtmlString += "<td>" + dt.Rows[i]["IBAN"].ToString() + "</td>";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px'>SWIFT:</span></td>";
                    HtmlString += "<td>" + dt.Rows[i]["SWIFT"].ToString() + "</td>";
                    HtmlString += "</tr> ";

                    HtmlString += "<tr> ";
                    HtmlString += "<td colspan='2' style=''></BR></BR></BR></BR><span style='font-size:12px;font-weight:700;'>NESMA RECYCLING CO. LTD.,</span> </BR> P.O. BOX 54744, JEDDAH 21524, KSA </BR></BR></BR></BR> PREPARED BY</td> ";
                    HtmlString += "</tr> ";

                    HtmlString += "</table>";
                    HtmlString += "</div>";

                    HtmlString += "</div>";
                    HtmlString += "</div>";
                    HtmlString += "</div>";

                    HtmlString += "</div>";
                }
            }
            else if (DropDownInvPrintFor.Text == "2")
            {
                for (int i = 0; i < RowCount; i++)
                {

                    HtmlString += "<div style='float:left;width:757px;height:auto;margin-top:50px;font-family: Tahoma, Arial, Courier, Lucida Typewriter, monospace; font-size: 12px; font-style: normal; font-variant: normal; font-weight: 400; line-height: 16px;'> ";

                    HtmlString += "<div style='float:left;width:720px;height:20px; margin:30px 5px 0 40px;text-align:center;border-bottom: black solid 1px;' ><span style='font-family:Tahoma;font-size:15px;font-weight:700;'>PACKING LIST</span></div> ";
                    HtmlString += "<div style='float:left;width:375px;height:20px; margin:5px 0 0 40px;'><span style='font-family:Tahoma, Arial,Times, serif;font-size:12px;'></span></div> ";
                    HtmlString += "<div style='float:left;width:340px;height:20px; margin-top:5px;text-align:right;'><span style='font-family:Tahoma;font-size:12px;'>VAT NO.: 300507825100003</span></div> ";

                    HtmlString += "<div style='float:left;width:375px;height:40px; margin:0 0 0 40px;'></div> ";
                    HtmlString += "<div style='float:left;width:340px;height:40px;text-align:right;'><span style='font-size:12px;'>DATE: <span style='font-weight:700;'>" + dt.Rows[i]["INVOICE_DATE"].ToString() + "</span></span></div> ";

                    HtmlString += "<div style='float:left;width:720px;height:auto;margin:0 0 0 40px;font-size: 11px; font-style: normal; font-variant: normal; font-weight:400; line-height: 16px;'> ";
                    HtmlString += "<table cellpadding='2px' cellspacing='0' style='font-size:11px'>";
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-left:black solid 1px'><span style='font-weight:700;line-height: 1.6;'>" + dt.Rows[i]["PARTY_NAME"].ToString() + "</br>" + dt.Rows[i]["PARTY_ADD_1"].ToString() + "</br>" + dt.Rows[i]["PARTY_ADD_2"].ToString() + "</br>TEL NO: " + dt.Rows[i]["CONTACT_NO"].ToString() + "</span></td> ";
                    HtmlString += "<td></td> ";
                    HtmlString += "<td></td> ";
                    HtmlString += "<td></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>&nbsp;</td>";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "</table>";

                    HtmlString += "<table cellpadding='2px' cellspacing='0' style='font-size:11px' width=100%>";
                    HtmlString += "<tr> ";
                    HtmlString += "<td  style='border-left:black solid 1px'><span style='font-weight:700;'>BL NO:</span></td> ";
                    HtmlString += "<td  style=''>" + dt.Rows[i]["BL_NO"].ToString() + "</td> "; 
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td  style='border-left:black solid 1px'><span style='font-weight:700;'>VESSEL NAME:</span></td> ";
                    HtmlString += "<td  style=''>" + dt.Rows[i]["TRADING_VESSEL_NAME"].ToString() + " " + dt.Rows[i]["TRADING_VESSEL_CODE"].ToString() + "</td> "; 
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td  style='border-left:black solid 1px'><span style='font-weight:700;'>POL & POD:</span></td> ";
                    HtmlString += "<td  style=''>" + dt.Rows[i]["SHIPMENT_FROM_NAME"].ToString() + " TO " + dt.Rows[i]["SHIPMENT_TO_NAME"].ToString() + "</td> "; 
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td  style='border-left:black solid 1px'><span style='font-weight:700;'>REF INVOICE NO:</span></td> ";
                    HtmlString += "<td  style=''>" + dt.Rows[i]["EXPORT_INVOICE_NO"].ToString() + "</td> "; 
                    HtmlString += "</tr> ";  
                    HtmlString += "</table>";
                    HtmlString += "</BR></BR>";
                    HtmlString += "</div>";


                    HtmlString += "<div style='float:left;width:720px;height:auto;margin:0 0 0 40px;-webkit-border-top-left-radius:10px;-webkit-border-top-right-radius:10px;font-family: Tahoma, Arial Narrow, Courier, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight:400; line-height: 16px;'> ";

                    HtmlString += "<table cellpadding='5px' cellspacing='0' style='font-size:11px'  width=100%>";
                    HtmlString += "<th style='border:black solid 1px;width:100px'>CONTAINER (S)</th> ";
                    HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>SIZE</th> ";
                    HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>DESCRIPTION OF MATERIALS</th> ";
                    HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>QTY</th> ";
                    HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>PACKAGE TYPE</th> ";
                    HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>GROSS WEIGHT (KG)</th> ";
                    HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>LESS ";
                    for (int p = 0; p < 1; p++)
                    {
                        HtmlString += "" + di.Rows[p]["PACKING_NAME"].ToString().ToUpper() + "(MT)</th> ";
                    } 
                    HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>NET. WEIGHT (MT)</th> ";
             

                    HtmlString += "<tr valign='top'> ";

                    HtmlString += "<td style='border-left:black solid 1px;border-right:black solid 1px;'> ";
                    for (int l = 0; l < RowCount2; l++)
                    {
                        HtmlString += "" + ds.Rows[l]["CONTAINER_NO"].ToString() + "</br> ";
                    }
                    HtmlString += "</td> ";

                    HtmlString += "<td rowspan='1' style='border-right:black solid 1px;text-align:center;vertical-align: middle;'> ";

                    for (int d = 0; d < ContainerSizelistDisCount; d++)
                    {
                        HtmlString += "" + ContainerSizelistDis[d].ToString() + "'</br>";
                    }

                    HtmlString += "</td> ";

                    HtmlString += "<td rowspan='1' style='border-right:black solid 1px;text-align:center;vertical-align: middle;'> ";  // " + Convert.ToInt32(RowCount2 - 1) + "

                    for (int d = 0; d < ItemDeslistDisCount; d++)
                    {
                        HtmlString += "" + ItemDeslistDis[d].ToString() + "</br>";
                    }

                    HtmlString += "</td> ";

                    HtmlString += "<td  rowspan='1' style='border-right:black solid 1px;text-align:center;'> ";
                    for (int p = 0; p < RowCount3; p++)
                    {
                        HtmlString += "" + di.Rows[p]["BUNDLE"].ToString() + "</br> ";
                    }
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black solid 1px;text-align:center;'> ";
                   
                    HtmlString += "" + dt.Rows[i]["PACKING_TYPE_NAME"].ToString() + "";
                    
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black solid 1px;text-align:center;'> ";
                    for (int p = 0; p < RowCount3; p++)
                    {
                        HtmlString += "" + decimal.Parse(di.Rows[p]["ITEM_WEIGHT_WB"].ToString()).ToString(".000") + "</br> ";
                    }
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black solid 1px;text-align:center;'> ";
                    for (int p = 0; p < RowCount3; p++)
                    {
                        HtmlString += "" + decimal.Parse(di.Rows[p]["PACKING_WEIGHT"].ToString()).ToString("0.000") + "</br> ";
                    }
                    HtmlString += "</td> ";

                    HtmlString += "<td style='border-right:black solid 1px;text-align:center;'> ";
                    for (int p = 0; p < RowCount3; p++)
                    {
                        HtmlString += "" + decimal.Parse(di.Rows[p]["ITEM_WEIGHT"].ToString()).ToString(".000") + "</br> ";
                    }
                    HtmlString += "</td> "; 
                    for (int p = 0; p < RowCount3; p++)
                    { 
                        TotalItemBundle += Convert.ToDouble(di.Rows[p]["BUNDLE"].ToString());
                        TotalItemWtWb += Convert.ToDouble(di.Rows[p]["ITEM_WEIGHT_WB"].ToString());
                        TotalItemPackWt += Convert.ToDouble(di.Rows[p]["PACKING_WEIGHT"].ToString());
                        TotalItemWt += Convert.ToDouble(di.Rows[p]["ITEM_WEIGHT"].ToString());
                        TotalInvoiceAmt += Convert.ToDouble(di.Rows[p]["MATERIAL_AMOUNT"].ToString());
                        TotalInvoiceSrAmt = Convert.ToDouble(di.Rows[p]["CURRENCY_RATE"].ToString()) * TotalInvoiceAmt;
                    } 
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td colspan='3' style='border-right:black solid 1px;text-align:center;border-top:black solid 1px;border-bottom:black solid 1px;border-left:black solid 1px; text-align:right;font-weight:700;'> TOTAL: </td> ";
                    HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'>" + TotalItemBundle.ToString() + "</td>";
                    HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'></td>";
                    HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'>" + decimal.Parse(TotalItemWtWb.ToString()).ToString(".000") + "</td>";
                    HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'>" + decimal.Parse(TotalItemPackWt.ToString()).ToString("0.000") + "</td> ";
                    HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;font-weight:700;'>" + decimal.Parse(TotalItemWt.ToString()).ToString(".000") + "</td> ";
                    HtmlString += "</tr> ";  
                    HtmlString += "</table>";
                    HtmlString += "</BR>";
                    HtmlString += "<table cellpadding='2px' cellspacing='0' style='font-size:11px'>";
                
                    HtmlString += "<tr> ";
                    HtmlString += "<td colspan='2' style=''></BR></BR></BR></BR></BR></BR><span style='font-size:12px;font-weight:700;'>NESMA RECYCLING CO. LTD.,</span> </BR> P.O. BOX 54744, JEDDAH 21524, KSA </BR></BR></BR></BR> PREPARED BY</td> ";
                    HtmlString += "</tr> ";

                    HtmlString += "</table>";
                    HtmlString += "</div>";

                    HtmlString += "</div>";
                    HtmlString += "</div>";
                    HtmlString += "</div>";

                    HtmlString += "</div>";
                }
            }
            else if(DropDownInvPrintFor.Text == "3")
            {
                for (int i = 0; i < RowCount; i++)
                {

                    HtmlString += "<div style='float:left;width:757px;height:auto;margin:70px 0 0 40px;font-family: Tahoma, Arial, Courier, Lucida Typewriter, monospace; font-size: 12px; font-style: normal; font-variant: normal; font-weight: 400; line-height: 16px;'> ";
                     
                    HtmlString += "<div style='float:left;width:684px;height:30px;text-align:center;'><span style='font-size:15px;font-weight:700;'>CERTIFICATE OF ORIGIN</span></div> ";
                    HtmlString += "<div style='float:left;width:680px;height:auto; border:black dotted 2px;'> ";

                    HtmlString += "<table cellpadding='6px' cellspacing='0' style='font-size:12px' width=100%>";
                    HtmlString += "<tr> ";
                    HtmlString += "<td bgcolor='#d1e0e0' colspan='2' style='border-right:black solid 1px'><span style='font-weight:700;line-height: 1.6;font-size:13px;'>CONSIGNEE / IMPORTER</span></td> ";
                    HtmlString += "<td bgcolor='#d1e0e0' colspan='2' rowspan='2' style='text-align:center;vertical-align: middle;'><span style='font-size:15px;font-weight:700;line-height: 1.6;'>ORIGINAL</span></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td colspan='2' style='border-top:black solid 1px;border-right:black solid 1px'><span style='font-weight:700;line-height: 1.6;'>" + dt.Rows[i]["PARTY_NAME"].ToString() + "</span></br>" + dt.Rows[i]["PARTY_ADD_1"].ToString() + "</br>" + dt.Rows[i]["PARTY_ADD_2"].ToString() + "</br>TEL NO: " + dt.Rows[i]["CONTACT_NO"].ToString() + "</td> ";
       
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td bgcolor='#d1e0e0' colspan='2' style='border-top:black solid 1px;border-right:black solid 1px;vertical-align: middle;'><span style='font-size:13px;font-weight:700;line-height: 1.6;'>DESCRIPTION</span></td> ";
                    HtmlString += "<td bgcolor='#d1e0e0' colspan='2' style='border-top:black solid 1px;vertical-align: middle;'><span style='font-size:13px;font-weight:700;line-height: 1.6;t'>COUNTRY OF ORIGIN</span></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-top:black solid 1px;vertical-align: middle;'></BR><span style='line-height: 1.6;'>MATERIAL: </span></BR></BR></td> ";
                    HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;vertical-align: middle;'></BR>";
                    for (int d = 0; d < ItemDeslistDisCount; d++)
                    {
                        HtmlString += "<span style='font-size:13px;line-height: 1.6;'>" + ItemDeslistDis[d].ToString() + "</span> </br>";
                    }
                    HtmlString += "</BR></td> ";
                    HtmlString += "<td colspan='2' style='border-top:black solid 1px;text-align:center;vertical-align: middle;'><span style='font-size:13px;font-weight:700;line-height: 1.6;t'>SAUDI ARABIA</br></span></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td colspan='2' style='border-top:black solid 1px;border-right:black solid 1px;'></td> ";
                    HtmlString += "<td bgcolor='#d1e0e0' colspan='2' style='border-top:black solid 1px;border-bottom:black solid 1px;vertical-align: middle;'><span style='font-size:13px;font-weight:700;line-height: 1.6;t'>SHIPMENT DETAILS</span></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td  style='vertical-align: top;'><span style='line-height: 1.6;'>CONTAINER:</span></td>";
                    HtmlString += "<td  style='border-right:black solid 1px;vertical-align: middle;'>";
                    for (int l = 0; l < RowCount2; l++)
                    {
                        HtmlString += "<span style='line-height: 1.6;'>" + ds.Rows[l]["CONTAINER_NO"].ToString() + "</span></BR> ";
                    }

                    for (int p = 0; p < RowCount3; p++)
                    {
                      //  HtmlString += "" + string.Format("{0:n2}", Convert.ToDouble(di.Rows[p]["MATERIAL_AMOUNT"])) + "</br> ";
                        TotalItemWtWb += Convert.ToDouble(di.Rows[p]["ITEM_WEIGHT_WB"].ToString());
                        TotalItemWt += Convert.ToDouble(di.Rows[p]["ITEM_WEIGHT"].ToString());
                        TotalInvoiceAmt += Convert.ToDouble(di.Rows[p]["MATERIAL_AMOUNT"].ToString());
                        TotalInvoiceSrAmt = Convert.ToDouble(di.Rows[p]["CURRENCY_RATE"].ToString()) * TotalInvoiceAmt;
                    }
                    HtmlString += "</td> ";
                    HtmlString += "<td style='vertical-align: middle;line-height: 1.6;'>B/L NO: </BR>VESSEL:</td> ";
                    HtmlString += "<td style='vertical-align: middle;line-height: 1.6;'>" + dt.Rows[i]["BL_NO"].ToString() + "</BR>" + dt.Rows[i]["TRADING_VESSEL_NAME"].ToString() + " " + dt.Rows[i]["TRADING_VESSEL_CODE"].ToString() + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='vertical-align: middle;'><span style='line-height: 1.6;'>PACKING:</span></td> ";
                    HtmlString += "<td style='border-right:black solid 1px;vertical-align: middle;'><span style='line-height: 1.6;'>" + dt.Rows[i]["PACKING_TYPE_NAME"].ToString() + "</span></td> ";
                    HtmlString += "<td style='vertical-align: middle;'><span style='line-height: 1.6;'>DATE:</span></td> ";
                    HtmlString += "<td style='vertical-align: middle;'><span style='line-height: 1.6;'>" + dt.Rows[i]["SHIPMENT_DATE"].ToString() + "</span></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td colspan='2' style='border-right:black solid 1px;vertical-align: middle;'></td> "; 
                    HtmlString += "<td style='vertical-align: middle;'><span style='line-height: 1.6;'>PLACE OF LOADING:</BR>PLACE OF DELIVERY:</span></td> ";
                    HtmlString += "<td style='vertical-align: middle;'><span style='line-height: 1.6;'>" + dt.Rows[i]["SHIPMENT_FROM_NAME"].ToString() + "</BR>" + dt.Rows[i]["SHIPPING_INCO_NAME"].ToString() + " " + dt.Rows[i]["SHIPMENT_TO_NAME"].ToString() + "</span></td> ";
                    HtmlString += "</tr> "; 
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='vertical-align: middle;'><span style='line-height: 1.6;'>NET WEIGHT: </span></BR></BR></td> "; 
                    HtmlString += "<td style='border-right:black solid 1px;vertical-align: middle;'><span style='line-height: 1.6;'>" + TotalItemWt + " MT</span></BR></BR></td> "; 
                    HtmlString += "<td style='vertical-align: middle;'><span style='font-size:13px;line-height: 1.6;'></span></td> ";
                    HtmlString += "<td style='vertical-align: middle;'><span style='font-size:13px;line-height: 1.6;'></span></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td bgcolor='#d1e0e0' colspan='2' style='border-top:black solid 1px;border-right:black solid 1px;vertical-align: middle;'><span style='font-size:13px;font-weight:700;line-height: 1.6;'>INVOICE</span></td> ";
                    HtmlString += "<td bgcolor='#d1e0e0' colspan='2' style='border-top:black solid 1px;vertical-align: middle;'><span style='font-size:13px;font-weight:700;line-height: 1.6;'>REMARKS</span></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='border-top:black solid 1px;vertical-align: middle;'></BR><span style='line-height: 1.6;'>NO.:</BR>DATE:</BR>AMOUNT:</span></BR></BR></td> ";
                    HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;vertical-align: middle;'></BR><span style='line-height: 1.6;'>" + dt.Rows[i]["EXPORT_INVOICE_NO"].ToString() + "</BR>" + dt.Rows[i]["INVOICE_DATE"].ToString() + "</BR>USD " + string.Format("{0:n2}", TotalInvoiceAmt) + "</span></BR></BR></td> ";
                    HtmlString += "<td colspan='2' style='border-top:black solid 1px;vertical-align: middle;text-align:center;'><span style='font-weight:700;line-height: 1.6;'>______________________</span></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td colspan='4' style='border-top:black solid 1px;vertical-align: middle;'><span style='line-height: 1.6;'></BR>IT IS HEREBY CERTIFIED THAT THE DETAILS GIVEN ABOVE ARE TRUE AND CORRECT.</BR></BR></span></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td colspan='2' style='border-top:black solid 1px;vertical-align: middle;'></BR></BR></BR></BR></BR><span style='font-size:12px;font-weight:700;'>NESMA RECYCLING CO. LTD.,</span> </BR> P.O. BOX 54744, JEDDAH 21524, KSA.</BR></BR></td> ";
                    HtmlString += "<td colspan='2' style='border-top:black solid 1px;vertical-align: middle;text-align:center;'></BR></BR></BR></BR><span style='font-size:12px;font-weight:700;'>_________________</BR>COMPANY'S SEAL</td> ";

                    HtmlString += "</tr> "; 
                    HtmlString += "</table>";
                    HtmlString += "<div style='float:left;width:720px;height:30px;text-align:center;'><span style='font-size:11px;'>VAT NO.: 300507825100003</span></div> ";
                    HtmlString += "</div>"; 
                    HtmlString += "</div>";
                    HtmlString += "</div>"; 
                     
                }
            }

            
            // purchase master update for print
            int userID = Convert.ToInt32(Session["USER_ID"]);
            int ExportSalesID = Convert.ToInt32(TextExportSalesID.Text);
            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
            string update_user = "update  MF_EXPORT_SALES_MASTER  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where EXPORT_SALES_ID = :NoExportSalesID ";
            cmdi = new OracleCommand(update_user, conn);

            OracleParameter[] objPrm = new OracleParameter[4];
            objPrm[0] = cmdi.Parameters.Add("TextIsPrint", "Printed");
            objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
            objPrm[2] = cmdi.Parameters.Add("NoCuserID", userID);
            objPrm[3] = cmdi.Parameters.Add("NoExportSalesID", ExportSalesID);

            cmdi.ExecuteNonQuery();
            cmdi.Parameters.Clear();
            cmdi.Dispose();
            conn.Close();

            PanelPrint.Controls.Add(new LiteralControl(HtmlString));
            Session["ctrl"] = PanelPrint;
            ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('Print.aspx','PrintMe','height=900px,width=1200px,scrollbars=1');</script>");
            Display();
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
                int ExportSalesID =  Convert.ToInt32(TextExportSalesID.Text);
                int PartyID = Convert.ToInt32(DropDownPartyID.Text);
                int PayTermID = Convert.ToInt32(DropDownPayTermID.Text);
                int ShippingIncoID = Convert.ToInt32(DropDownShippingIncoID.Text);
                int TradingVesselID = Convert.ToInt32(DropDownTradingVesselID.Text);
                int ShipmentFromToID = Convert.ToInt32(DropDownShipmentFromToID.Text);
                int PackingTypeID = Convert.ToInt32(DropDownPackingTypeID.Text);
                int AccountID = Convert.ToInt32(DropDownAccountID.Text);

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string IsShipMarksActive = CheckIsShipMarksActive.Checked ? "Enable" : "Disable";
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                string MakeEntryDate = EntryDate.Text;
                string[] MakeEntryDateSplit = MakeEntryDate.Split('-'); 
                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-"); 
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                  
                string MakeEntryDate2 = EntryDate2.Text;
                string[] MakeEntryDateSplit2 = MakeEntryDate2.Split('-');
                String EntryDateTemp2 = MakeEntryDateSplit2[0].Replace("/", "-");
                DateTime EntryDateNewD2 = DateTime.ParseExact(EntryDateTemp2, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew2 = EntryDateNewD2.ToString("dd-MM-yyyy");

                string makeSlipSQL = " SELECT PEWC.EXPORT_INVOICE_NO FROM MF_EXPORT_SALES_MASTER PESM LEFT JOIN MF_EXPORT_WBSLIP_CON PEWC ON PEWC.EXPORT_INVOICE_NO = PESM.EXPORT_INVOICE_NO WHERE  PESM.EXPORT_SALES_ID = '" + ExportSalesID + "'";

                cmdl = new OracleCommand(makeSlipSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                for (int i = 0; i < RowCount; i++)
                {
                    string ExInvoiceNo = dt.Rows[i]["EXPORT_INVOICE_NO"].ToString();
                    string update_user = "update  MF_EXPORT_WBSLIP_CON set EXPORT_INVOICE_NO =:NoExportInvoiceNo, IS_CONFIRM_CHECK =:IsConfirm,  IS_CONFIRM_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , IS_CONFIRM_ID = :NoCuserID  where EXPORT_INVOICE_NO ='" + ExInvoiceNo + "' ";
                    cmdi = new OracleCommand(update_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[4]; 
                    objPrm[0] = cmdi.Parameters.Add("NoExportInvoiceNo", "");
                    objPrm[1] = cmdi.Parameters.Add("IsConfirm", "");
                    objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[3] = cmdi.Parameters.Add("NoCuserID", userID);

                    cmdi.ExecuteNonQuery();
                }

                cmdi.Parameters.Clear();
                cmdi.Dispose();

                foreach (ListItem li in DropDownSlipNoEx.Items)
                {
                        if (li.Selected == true)
                        {
                            string[] WbSlipNo = li.Value.Split('-');
                            string update_ex_invoice = " update  MF_EXPORT_WBSLIP_CON set EXPORT_INVOICE_NO =:NoExportInvoiceNo, IS_CONFIRM_CHECK =:IsConfirm,  IS_CONFIRM_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , IS_CONFIRM_ID = :NoCuserID  where WB_SLIP_NO =: NoWbSlipNo ";
                            cmdi = new OracleCommand(update_ex_invoice, conn);

                            OracleParameter[] objPrm = new OracleParameter[5];
                            objPrm[0] = cmdi.Parameters.Add("NoWbSlipNo", WbSlipNo[0]);
                            objPrm[1] = cmdi.Parameters.Add("NoExportInvoiceNo", TextExportInvoiceNo.Text);
                            objPrm[2] = cmdi.Parameters.Add("IsConfirm", "Complete");
                            objPrm[3] = cmdi.Parameters.Add("u_date", u_date);
                            objPrm[4] = cmdi.Parameters.Add("NoCuserID", userID);

                            cmdi.ExecuteNonQuery();
                        }
                    }
                cmdi.Parameters.Clear();
                cmdi.Dispose();


                string update_ex_sales = " update  MF_EXPORT_SALES_MASTER set  EXPORT_INVOICE_NO =:TextExportInvoiceNo, BL_NO =:TextBl, PO_NO =:TextPoNo, REF_NO =:TextRef, PACKING_TYPE_ID =:NoPackingTypeID, PAY_TERMS_ID =:NoPayTermID, PAYMENT_TERMS_DES =:TextPayTermDes, SHIPPING_INCO_ID =:NoShippingIncoID, TRADING_VESSEL_ID =:NoTradingVesselID, TRADING_VESSEL_CODE =:TextVesselCode, SHIPMENT_DATE = TO_DATE(:ShipmentDate, 'DD/MM/YYYY'),  SHIPMENT_FROM_TO_ID =:NoShipmentFromToID, PARTY_ID =:NoPartyID, INVOICE_DATE = TO_DATE(:InvoiceDate, 'DD/MM/YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, ACCOUNT_ID =:NoAccountID, IS_SHIPPING_MARKS_ACTIVE =:TextIsShipMarksActive, IS_ACTIVE = :TextIsActive WHERE EXPORT_SALES_ID =: NoExportSalesID ";
                cmdi = new OracleCommand(update_ex_sales, conn); 
                OracleParameter[] objPr = new OracleParameter[20];
                objPr[0] = cmdi.Parameters.Add("TextExportInvoiceNo", TextExportInvoiceNo.Text); 
                objPr[1] = cmdi.Parameters.Add("TextBl", TextBl.Text); 
                objPr[2] = cmdi.Parameters.Add("TextPoNo", TextPoNo.Text); 
                objPr[3] = cmdi.Parameters.Add("TextRef", TextRef.Text); 
                objPr[4] = cmdi.Parameters.Add("NoPackingTypeID", PackingTypeID); 
                objPr[5] = cmdi.Parameters.Add("NoPayTermID", PayTermID); 
                objPr[6] = cmdi.Parameters.Add("TextPayTermDes", TextPayTermDes.Text); 
                objPr[7] = cmdi.Parameters.Add("NoShippingIncoID", ShippingIncoID);
                objPr[8] = cmdi.Parameters.Add("NoTradingVesselID", TradingVesselID);  
                objPr[9] = cmdi.Parameters.Add("TextVesselCode", TextVesselCode.Text);  
                objPr[10] = cmdi.Parameters.Add("ShipmentDate", EntryDateNew); 
                objPr[11] = cmdi.Parameters.Add("NoShipmentFromToID", ShipmentFromToID);  
                objPr[12] = cmdi.Parameters.Add("NoPartyID", PartyID); 
                objPr[13] = cmdi.Parameters.Add("InvoiceDate", EntryDateNew2);  
                objPr[14] = cmdi.Parameters.Add("u_date", u_date); 
                objPr[15] = cmdi.Parameters.Add("NoCuserID", userID);  
                objPr[16] = cmdi.Parameters.Add("NoAccountID", AccountID); 
                objPr[17] = cmdi.Parameters.Add("TextIsShipMarksActive", IsShipMarksActive);
                objPr[18] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPr[19] = cmdi.Parameters.Add("NoExportSalesID", ExportSalesID);  

                cmdi.ExecuteNonQuery(); 
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close(); 

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Export Sales Invoice Data Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                 

                Display();
                CheckExInvoiceNo.Text = "";

            }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
          //    }
          //   catch
          //    {
          //      Response.Redirect("~/ParameterError.aspx");
          //   }  
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
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt"); 
                    string ExportSalesID = TextExportSalesID.Text;  

                    string makeSlipSQL = " SELECT PEWC.EXPORT_INVOICE_NO FROM MF_EXPORT_SALES_MASTER PESM LEFT JOIN MF_EXPORT_WBSLIP_CON PEWC ON PEWC.EXPORT_INVOICE_NO = PESM.EXPORT_INVOICE_NO WHERE  PESM.EXPORT_SALES_ID = '" + ExportSalesID + "'";

                    cmdl = new OracleCommand(makeSlipSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    for (int i = 0; i < RowCount; i++)
                    {
                        string ExInvoiceNo = dt.Rows[i]["EXPORT_INVOICE_NO"].ToString();
                        string update_user = "update  MF_EXPORT_WBSLIP_CON set EXPORT_INVOICE_NO =:NoExportInvoiceNo, IS_CONFIRM_CHECK =:IsConfirm,  IS_CONFIRM_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , IS_CONFIRM_ID = :NoCuserID  where EXPORT_INVOICE_NO ='" + ExInvoiceNo + "' ";
                        cmdi = new OracleCommand(update_user, conn);

                        OracleParameter[] objPrm = new OracleParameter[4];
                        objPrm[0] = cmdi.Parameters.Add("NoExportInvoiceNo", "");
                        objPrm[1] = cmdi.Parameters.Add("IsConfirm", "");
                        objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                        objPrm[3] = cmdi.Parameters.Add("NoCuserID", userID);

                        cmdi.ExecuteNonQuery();
                    }
                     
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                     
                    string delete_ex_sales = " delete from MF_EXPORT_SALES_MASTER where EXPORT_SALES_ID  = '" + ExportSalesID + "'"; 
                    cmdi = new OracleCommand(delete_ex_sales, conn); 
                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();


                    string delete_ex_contract = " delete from MF_EXPORT_CONTRACT_NO where EXPORT_INVOICE_NO  = '" + TextExportInvoiceNo.Text + "'";
                    cmdi = new OracleCommand(delete_ex_contract, conn);
                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();

                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Invoice Data Delete Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
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
     

        protected void DeleteContractClick(object sender, EventArgs e)
        {
            try
            {
                if (IS_DELETE_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    LinkButton btn = (LinkButton)sender;
                    Session["user_data_id"] = btn.CommandArgument;
                    int USER_DATA_ID = Convert.ToInt32(Session["user_data_id"]);
                      
                    string delete_ex_contract = " delete from MF_EXPORT_CONTRACT_NO where EXPORT_CONTRACT_NO_ID  = '" + USER_DATA_ID + "'"; 
                    cmdi = new OracleCommand(delete_ex_contract, conn); 
                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Contract Data Delete Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
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
         
        protected void LinkSelectClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             
             LinkButton btn = (LinkButton)sender;
             Session["user_data_id"] = btn.CommandArgument;
             int USER_DATA_ID = Convert.ToInt32(Session["user_data_id"]); 

            DataTable dtSlipNo = new DataTable();
            DataSet dsp = new DataSet();
            string makePageSQL = " SELECT PEWC.WB_SLIP_NO, PEWC.WB_SLIP_NO || ' - ' || PC.PARTY_ID || ' - ' || PC.PARTY_NAME || ', Container No. - ' || PEWC.CONTAINER_NO || ', Item - ' || PI.ITEM_CODE || ' : ' || PI.ITEM_NAME || ', Item WT(WB) -' || TO_CHAR(PEWC.ITEM_WEIGHT_WB, '999,999,999') || ', Item WT -' || TO_CHAR((PEWC.ITEM_WEIGHT/1000), '999999.999') AS PARTY_NAME FROM MF_EXPORT_WBSLIP_CON PEWC LEFT JOIN MF_EXPORT_SALES_MASTER PESM  ON PEWC.EXPORT_INVOICE_NO = PESM.EXPORT_INVOICE_NO LEFT JOIN MF_PARTY PC ON PC.PARTY_ID = PEWC.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PEWC.ITEM_ID WHERE  PEWC.IS_ACTIVE_PRICING = 'Enable' AND ((PEWC.IS_CONFIRM_CHECK IS NULL AND PEWC.EXPORT_INVOICE_NO IS NULL) OR  PESM.EXPORT_SALES_ID = '" + USER_DATA_ID + "')  ORDER BY PEWC.WB_SLIP_NO ASC";
            dsp = ExecuteBySqlString(makePageSQL);
            dtSlipNo = (DataTable)dsp.Tables[0];
            DropDownSlipNoEx.DataSource = dtSlipNo;
            DropDownSlipNoEx.DataValueField = "WB_SLIP_NO";
            DropDownSlipNoEx.DataTextField = "PARTY_NAME";
            DropDownSlipNoEx.DataBind();

             
            string makeSQL = " SELECT EXPORT_SALES_ID, EXPORT_INVOICE_NO, BL_NO, PO_NO, REF_NO, ACCOUNT_ID, IS_SHIPPING_MARKS_ACTIVE, PACKING_TYPE_ID, PAY_TERMS_ID,  SHIPPING_INCO_ID, TRADING_VESSEL_ID, TRADING_VESSEL_CODE, TO_CHAR(SHIPMENT_DATE,'dd/mm/yyyy') AS SHIPMENT_DATE, TO_CHAR(INVOICE_DATE,'dd/mm/yyyy') AS INVOICE_DATE, PARTY_ID,  SHIPMENT_FROM_TO_ID, TO_CHAR(SHIPMENT_DATE,'dd/mm/yyyy') AS SHIPMENT_DATE, CREATE_DATE, UPDATE_DATE, IS_ACTIVE, IS_PRINT, PRINT_DATE, PAYMENT_TERMS_DES FROM MF_EXPORT_SALES_MASTER WHERE EXPORT_SALES_ID = '" + USER_DATA_ID + "'";

             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn);
             ds = new DataTable();
             oradata.Fill(ds);
             RowCount = ds.Rows.Count;

             for (int i = 0; i < RowCount; i++)
             { 
                 TextExportSalesID.Text       = ds.Rows[i]["EXPORT_SALES_ID"].ToString();
                 TextExportInvoiceNo.Text     = ds.Rows[i]["EXPORT_INVOICE_NO"].ToString();
                 TextBl.Text                  = ds.Rows[i]["BL_NO"].ToString();
                 TextPoNo.Text                = ds.Rows[i]["PO_NO"].ToString();
                 TextRef.Text                 = ds.Rows[i]["REF_NO"].ToString();
                 DropDownPackingTypeID.Text   = ds.Rows[i]["PACKING_TYPE_ID"].ToString();
                 DropDownPartyID.Text         = ds.Rows[i]["PARTY_ID"].ToString();                 
                 DropDownPayTermID.Text       = ds.Rows[i]["PAY_TERMS_ID"].ToString();
                 EntryDate.Text               = ds.Rows[i]["SHIPMENT_DATE"].ToString(); 
                 EntryDate2.Text              = ds.Rows[i]["INVOICE_DATE"].ToString(); 
                 DropDownShippingIncoID.Text  = ds.Rows[i]["SHIPPING_INCO_ID"].ToString();
                 DropDownTradingVesselID.Text = ds.Rows[i]["TRADING_VESSEL_ID"].ToString();
                 TextVesselCode.Text          = ds.Rows[i]["TRADING_VESSEL_CODE"].ToString(); 
                 DropDownShipmentFromToID.Text= ds.Rows[i]["SHIPMENT_FROM_TO_ID"].ToString();
                 DropDownAccountID.Text        = ds.Rows[i]["ACCOUNT_ID"].ToString();
                 TextPayTermDes.Text          = ds.Rows[i]["PAYMENT_TERMS_DES"].ToString(); 
                 CheckIsActive.Checked        = Convert.ToBoolean(ds.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                 CheckIsShipMarksActive.Checked= Convert.ToBoolean(ds.Rows[i]["IS_SHIPPING_MARKS_ACTIVE"].ToString() == "Enable" ? true : false); 
             }
              
             string makeSlipSQL = " SELECT PEWC.WB_SLIP_NO, PEWC.WB_SLIP_NO || ' - ' || PC.PARTY_ID || ' - ' || PC.PARTY_NAME || ', Container No. - ' || PEWC.CONTAINER_NO || ', Item - ' ||  PI.ITEM_CODE || ' : ' || PI.ITEM_NAME  || ', Item WT(WB) -' || TO_CHAR(PEWC.ITEM_WEIGHT_WB, '999,999,999') || ', Item WT -' || TO_CHAR((PEWC.ITEM_WEIGHT/1000), '999999.999') AS PARTY_NAME FROM MF_EXPORT_SALES_MASTER PESM LEFT JOIN MF_EXPORT_WBSLIP_CON PEWC ON PEWC.EXPORT_INVOICE_NO = PESM.EXPORT_INVOICE_NO  LEFT JOIN MF_PARTY PC ON PC.PARTY_ID = PEWC.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PEWC.ITEM_ID WHERE PESM.EXPORT_SALES_ID = '" + USER_DATA_ID + "' ORDER BY PEWC.WB_SLIP_NO ASC ";

             cmdl = new OracleCommand(makeSlipSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count;  
              foreach (ListItem li in DropDownSlipNoEx.Items)
              { 
                  li.Selected = false;
                  for (int i = 0; i < RowCount; i++)
                  { 
                      if (li.Value == dt.Rows[i]["WB_SLIP_NO"].ToString())
                      {
                          li.Selected = true; 
                      }
                  }
              }
          //   contract_id.Enabled = true;
             Display();
             DisplayContract();
             conn.Close();
             CheckExInvoiceNo.Text = "";
             TextExportInvoiceNo.Enabled = false; 
             alert_box.Visible = false;

             BtnUpdate.Attributes.Add("aria-disabled", "true");
             BtnUpdate.Attributes.Add("class", "btn btn-success active");
             BtnDelete.Attributes.Add("aria-disabled", "true");
             BtnDelete.Attributes.Add("class", "btn btn-danger active");

             BtnAdd.Attributes.Add("aria-disabled", "false");
             BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 
        }

        public void Display()
        {
            
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open(); 

            string makeSQL = "";
            if (txtSearchUser.Text == "")
            {
                makeSQL = "SELECT  PESM.EXPORT_SALES_ID, PESM.EXPORT_INVOICE_NO, NPT.PAY_TERMS_NAME, NSI.SHIPPING_INCO_NAME, NTV.TRADING_VESSEL_NAME, PESM.TRADING_VESSEL_CODE, PEWC.MAT_PRICE_PER_MT, PEWC.MATERIAL_AMOUNT, PESM.SHIPMENT_DATE, PESM.INVOICE_DATE, NCS.CURRENCY_NAME AS SOURCE_CURRENCY_NAME, NCT.CURRENCY_NAME AS TARGET_CURRENCY_ID, NCR.EXCHANGE_RATE, PEWC.MATERIAL_CONVERSION_AMOUNT, NSLF.SHIPMENT_LOC_NAME AS SHIPMENT_FROM_NAME, NSLT.SHIPMENT_LOC_NAME AS SHIPMENT_TO_NAME, PESM.CREATE_DATE, PESM.UPDATE_DATE, PESM.IS_ACTIVE, PESM.IS_PRINT, TO_CHAR(PESM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE,  PP.PARTY_NAME, PECN.CONTRACT_NO, (PEWC.ITEM_WEIGHT/1000) AS ITEM_WEIGHT, PEWC.WB_SLIP_NO, PEWC.CONTAINER_NO FROM MF_EXPORT_SALES_MASTER PESM LEFT JOIN MF_EXPORT_WBSLIP_CON PEWC ON PEWC.EXPORT_INVOICE_NO = PESM.EXPORT_INVOICE_NO LEFT JOIN NRC_PAYMENT_TERMS NPT ON NPT.PAY_TERMS_ID = PESM.PAY_TERMS_ID LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = PESM.PARTY_ID LEFT JOIN NRC_SHIPPING_INCOTERMS NSI ON NSI.SHIPPING_INCO_ID = PESM.SHIPPING_INCO_ID LEFT JOIN NRC_TRADING_VESSEL NTV ON NTV.TRADING_VESSEL_ID = PESM.TRADING_VESSEL_ID LEFT JOIN NRC_CURRENCY_RATE NCR ON NCR.CURRENCY_RATE_ID = PEWC.CURRENCY_RATE_ID LEFT JOIN NRC_CURRENCY NCS ON NCS.CURRENCY_ID = NCR.SOURCE_CURRENCY_ID LEFT JOIN NRC_CURRENCY NCT ON NCT.CURRENCY_ID = NCR.TARGET_CURRENCY_ID LEFT JOIN NRC_SHIPMENT_FROM_TO NSRT ON NSRT.SHIPMENT_FT_ID = PESM.SHIPMENT_FROM_TO_ID LEFT JOIN NRC_SHIPMENT_LOCATION NSLF ON NSLF.SHIPMENT_LOC_ID = NSRT.SHIPMENT_FROM_ID LEFT JOIN NRC_SHIPMENT_LOCATION NSLT ON NSLT.SHIPMENT_LOC_ID = NSRT.SHIPMENT_TO_ID LEFT JOIN(SELECT EXPORT_INVOICE_NO, COUNT(CONTRACT_NO) AS CONTRACT_NO FROM MF_EXPORT_CONTRACT_NO GROUP BY EXPORT_INVOICE_NO) PECN ON PECN.EXPORT_INVOICE_NO = PESM.EXPORT_INVOICE_NO WHERE PEWC.IS_INVENTORY_STATUS = 'Transit'  GROUP BY PESM.EXPORT_SALES_ID, PESM.EXPORT_INVOICE_NO, NPT.PAY_TERMS_NAME, NSI.SHIPPING_INCO_NAME, NTV.TRADING_VESSEL_NAME, PESM.TRADING_VESSEL_CODE, PEWC.MAT_PRICE_PER_MT, PEWC.MATERIAL_AMOUNT, PESM.SHIPMENT_DATE, PESM.INVOICE_DATE, NCS.CURRENCY_NAME, NCT.CURRENCY_NAME, NCR.EXCHANGE_RATE, PEWC.MATERIAL_CONVERSION_AMOUNT, NSLF.SHIPMENT_LOC_NAME, NSLT.SHIPMENT_LOC_NAME , PESM.CREATE_DATE, PESM.UPDATE_DATE, PESM.IS_ACTIVE, PESM.IS_PRINT, PESM.PRINT_DATE, PP.PARTY_NAME, PECN.CONTRACT_NO, PEWC.ITEM_WEIGHT, PEWC.WB_SLIP_NO, PEWC.CONTAINER_NO ORDER BY PESM.EXPORT_INVOICE_NO DESC"; 
            }
            else
            {
                makeSQL = "SELECT  PESM.EXPORT_SALES_ID, PESM.EXPORT_INVOICE_NO, NPT.PAY_TERMS_NAME, NSI.SHIPPING_INCO_NAME, NTV.TRADING_VESSEL_NAME, PESM.TRADING_VESSEL_CODE, PEWC.MAT_PRICE_PER_MT, PEWC.MATERIAL_AMOUNT, PESM.SHIPMENT_DATE, PESM.INVOICE_DATE, NCS.CURRENCY_NAME AS SOURCE_CURRENCY_NAME, NCT.CURRENCY_NAME AS TARGET_CURRENCY_ID, NCR.EXCHANGE_RATE, PEWC.MATERIAL_CONVERSION_AMOUNT, NSLF.SHIPMENT_LOC_NAME AS SHIPMENT_FROM_NAME, NSLT.SHIPMENT_LOC_NAME AS SHIPMENT_TO_NAME, PESM.CREATE_DATE, PESM.UPDATE_DATE, PESM.IS_ACTIVE, PESM.IS_PRINT, TO_CHAR(PESM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE,  PP.PARTY_NAME, PECN.CONTRACT_NO, (PEWC.ITEM_WEIGHT/1000) AS ITEM_WEIGHT, PEWC.WB_SLIP_NO, PEWC.CONTAINER_NO FROM MF_EXPORT_SALES_MASTER PESM LEFT JOIN MF_EXPORT_WBSLIP_CON PEWC ON PEWC.EXPORT_INVOICE_NO = PESM.EXPORT_INVOICE_NO LEFT JOIN NRC_PAYMENT_TERMS NPT ON NPT.PAY_TERMS_ID = PESM.PAY_TERMS_ID LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = PESM.PARTY_ID LEFT JOIN NRC_SHIPPING_INCOTERMS NSI ON NSI.SHIPPING_INCO_ID = PESM.SHIPPING_INCO_ID LEFT JOIN NRC_TRADING_VESSEL NTV ON NTV.TRADING_VESSEL_ID = PESM.TRADING_VESSEL_ID LEFT JOIN NRC_CURRENCY_RATE NCR ON NCR.CURRENCY_RATE_ID = PEWC.CURRENCY_RATE_ID LEFT JOIN NRC_CURRENCY NCS ON NCS.CURRENCY_ID = NCR.SOURCE_CURRENCY_ID LEFT JOIN NRC_CURRENCY NCT ON NCT.CURRENCY_ID = NCR.TARGET_CURRENCY_ID LEFT JOIN NRC_SHIPMENT_FROM_TO NSRT ON NSRT.SHIPMENT_FT_ID = PESM.SHIPMENT_FROM_TO_ID LEFT JOIN NRC_SHIPMENT_LOCATION NSLF ON NSLF.SHIPMENT_LOC_ID = NSRT.SHIPMENT_FROM_ID LEFT JOIN NRC_SHIPMENT_LOCATION NSLT ON NSLT.SHIPMENT_LOC_ID = NSRT.SHIPMENT_TO_ID LEFT JOIN(SELECT EXPORT_INVOICE_NO, COUNT(CONTRACT_NO) AS CONTRACT_NO FROM MF_EXPORT_CONTRACT_NO GROUP BY EXPORT_INVOICE_NO) PECN ON PECN.EXPORT_INVOICE_NO = PESM.EXPORT_INVOICE_NO  WHERE PESM.EXPORT_INVOICE_NO like '" + txtSearchUser.Text + "%'  GROUP BY PESM.EXPORT_SALES_ID, PESM.EXPORT_INVOICE_NO, NPT.PAY_TERMS_NAME, NSI.SHIPPING_INCO_NAME, NTV.TRADING_VESSEL_NAME, PESM.TRADING_VESSEL_CODE, PEWC.MAT_PRICE_PER_MT, PEWC.MATERIAL_AMOUNT, PESM.SHIPMENT_DATE, PESM.INVOICE_DATE, NCS.CURRENCY_NAME, NCT.CURRENCY_NAME, NCR.EXCHANGE_RATE, PEWC.MATERIAL_CONVERSION_AMOUNT, NSLF.SHIPMENT_LOC_NAME, NSLT.SHIPMENT_LOC_NAME , PESM.CREATE_DATE, PESM.UPDATE_DATE, PESM.IS_ACTIVE, PESM.IS_PRINT, PESM.PRINT_DATE, PP.PARTY_NAME, PECN.CONTRACT_NO, PEWC.ITEM_WEIGHT, PEWC.WB_SLIP_NO, PEWC.CONTAINER_NO ORDER BY PESM.EXPORT_INVOICE_NO DESC";

                alert_box.Visible = false;
            }

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            GridView1.DataSource = dt;
            GridView1.DataKeyNames = new string[] { "EXPORT_SALES_ID" }; 
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

         
        protected void GridView1Search(object sender, EventArgs e)
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



        public void DisplayContract()
           {

            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
             
            string makeSQL = " SELECT * FROM MF_EXPORT_CONTRACT_NO WHERE EXPORT_INVOICE_NO = '"+ TextExportInvoiceNo.Text  + "' ";
             
            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            GridView2.DataSource = dt;
            GridView2.DataKeyNames = new string[] { "EXPORT_CONTRACT_NO_ID" };
            GridView2.DataBind();
              
            conn.Close();
         }



        public void clearTextField(object sender, EventArgs e)
        {
            DropDownShippingIncoID.Text = "0";
            DropDownTradingVesselID.Text = "0";
            DropDownShipmentFromToID.Text = "0"; 
            DropDownPartyID.Text = "0";
            TextExportInvoiceNo.Text = "";
            TextVesselCode.Text = ""; 
            TextContractNo.Text = "";
            TextContractNoSerial.Text = ""; 
            EntryDate.Text = "";
            EntryDate1.Text = "";
            DropDownPayTermID.Text = "0";
            DropDownSlipNoEx.SelectedIndex = -1;
            CheckExInvoiceNo.Text = "";
        }

        public void clearText()
        { 
            DropDownShippingIncoID.Text = "0";
            DropDownTradingVesselID.Text = "0";
            DropDownShipmentFromToID.Text = "0"; 
            DropDownPartyID.Text = "0";
            TextExportInvoiceNo.Text = "";
            TextBl.Text = "";
            TextPoNo.Text = "";
            TextRef.Text = "";            
            TextVesselCode.Text = ""; 
            TextContractNo.Text = "";
            TextContractNoSerial.Text = ""; 
            EntryDate.Text = "";
            EntryDate1.Text = "";
            DropDownPayTermID.Text = "0";
            DropDownSlipNoEx.SelectedIndex = -1;
            CheckExInvoiceNo.Text = "";
        }


        public void TextExportInvoiceNo_TextChanged(object sender, EventArgs e)
        {
            string ExportInvoiceNo = TextExportInvoiceNo.Text; 
            if (ExportInvoiceNo != null)
            {
 
                    alert_box.Visible = false;

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "select EXPORT_INVOICE_NO from MF_EXPORT_SALES_MASTER where EXPORT_INVOICE_NO = '" + ExportInvoiceNo + "'";
                    cmd.CommandType = CommandType.Text;

                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        CheckExInvoiceNo.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Export Sales Invoice Number is already used.</label>";
                        CheckExInvoiceNo.ForeColor = System.Drawing.Color.Red;
                        TextExportInvoiceNo.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");


                    }
                    else
                    {
                        CheckExInvoiceNo.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Export Sales Invoice Number is available</label>";
                        CheckExInvoiceNo.ForeColor = System.Drawing.Color.Green;
                        TextBl.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");
                         

                    }
                }
                else
                {
                    CheckExInvoiceNo.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter Export Sales Invoice Number is 4 digit only</label>";
                    CheckExInvoiceNo.ForeColor = System.Drawing.Color.Red;
                    TextExportInvoiceNo.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

                } 
        }


        protected void BtnReport_Click(object sender, EventArgs e)
        {
            if (IS_REPORT_ACTIVE == "Enable")
            {


                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string[] MakeEntryDateSplit1 = TextMonthYear0.Text.Split('-');
                String ShipmentDate = MakeEntryDateSplit1[0].Replace("/", "-");
                DateTime ShipmentDateNew = DateTime.ParseExact(ShipmentDate, "MM-yyyy", CultureInfo.InvariantCulture);

                string HtmlString = "";
                string makeSQL = " SELECT  PESM.EXPORT_SALES_ID, PESM.EXPORT_INVOICE_NO, NPT.PAY_TERMS_NAME, NSI.SHIPPING_INCO_NAME, NTV.TRADING_VESSEL_NAME, PESM.TRADING_VESSEL_CODE,  TO_CHAR(PESM.SHIPMENT_DATE,'dd-MON-yyyy') AS SHIPMENT_DATE, TO_CHAR(PESM.INVOICE_DATE,'dd-mm-yyyy') AS INVOICE_DATE,  NSLF.SHIPMENT_LOC_NAME AS SHIPMENT_FROM_NAME, NSLT.SHIPMENT_LOC_NAME AS SHIPMENT_TO_NAME, PESM.CREATE_DATE, PESM.UPDATE_DATE, PESM.IS_ACTIVE, PESM.IS_PRINT, PESM.PRINT_DATE, PP.PARTY_NAME, PP.PARTY_ADD_1, PP.PARTY_ADD_2, PP.PARTY_VAT_NO FROM MF_EXPORT_SALES_MASTER PESM LEFT JOIN NRC_PAYMENT_TERMS NPT ON NPT.PAY_TERMS_ID = PESM.PAY_TERMS_ID LEFT JOIN NRC_SHIPPING_INCOTERMS NSI ON NSI.SHIPPING_INCO_ID = PESM.SHIPPING_INCO_ID LEFT JOIN NRC_TRADING_VESSEL NTV ON NTV.TRADING_VESSEL_ID = PESM.TRADING_VESSEL_ID  LEFT JOIN NRC_SHIPMENT_FROM_TO NSRT ON NSRT.SHIPMENT_FT_ID = PESM.SHIPMENT_FROM_TO_ID LEFT JOIN NRC_SHIPMENT_LOCATION NSLF ON NSLF.SHIPMENT_LOC_ID = NSRT.SHIPMENT_FROM_ID LEFT JOIN NRC_SHIPMENT_LOCATION NSLT ON NSLT.SHIPMENT_LOC_ID = NSRT.SHIPMENT_TO_ID  LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = PESM.PARTY_ID  WHERE TO_CHAR(PESM.SHIPMENT_DATE ,'mm-yyyy') = '" + ShipmentDate + "' ORDER BY PESM.EXPORT_INVOICE_NO ASC ";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;



                HtmlString += "<div style='float:left;width:1020px;height:auto;margin:15px 0 0 30px;'> ";

                HtmlString += "<div style='float:left;width:1020px;height:auto; font-family: Arial Narrow, Courier, Tahoma, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight:400; line-height: 16px;'> ";
                HtmlString += "<div style='float:left;width:1020px;height:62px; text-align:center;' ><img src='../../image/logo_from.png' height='98%'/></div> ";
                HtmlString += "<div style='float:left;width:1020px;height:20px; text-align:center;'><span style='font-size:14px;font-weight:700;'>Plastic Factory Division</span></div> ";
                HtmlString += "<div style='float:left;width:1020px;height:20px;margin-bottom:15px; text-align:center;border-bottom: black solid 1px;' ><span style='font-size:13px;font-weight:700;'>Shipment of Plastic Material During The Month of " + string.Format("{0: MMMM yyyy}", ShipmentDateNew) + "</span></div> ";

                HtmlString += "<table cellpadding='2px' cellspacing='0' style='font-size:11px;' width='100%'>";
                HtmlString += "<th style='border:black solid 1px; -webkit-border-top-left-radius:10px;'>INVOICE NO.</th> ";
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>PAYMENT TERMS</th> ";
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>SHIPMENT DATE</th> "; 
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>VESSEL NAME</th> ";
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>PARTY NAME</th> ";
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>CONTRACT</th> ";
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>MATERIAL</th> ";
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>QUANTITY</th> ";
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>PRICE/MT $USD</th> ";
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px;'>AMOUNT-USD</th> ";
                HtmlString += "<th style='border-top:black solid 1px;border-bottom:black solid 1px;border-right:black solid 1px; -webkit-border-top-right-radius:10px;'>CONTAINERS</th> ";

                int TotalCotainer = 0;
                for (int i = 0; i < RowCount; i++)
                {
                    string makeContractSQL = " SELECT CONTRACT_NO || '-' || CONTRACT_NO_SERIAL AS CONTRACT_NO, CONTRACT_NO_SERIAL, TO_CHAR(CONTRACT_DATE, 'DD-MON-YYYY') AS CONTRACT_DATE FROM MF_EXPORT_CONTRACT_NO WHERE EXPORT_INVOICE_NO = '" + dt.Rows[i]["EXPORT_INVOICE_NO"].ToString() + "' ";
                    cmdl = new OracleCommand(makeContractSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dc = new DataTable();
                    oradata.Fill(dc);
                    RowCount1 = dc.Rows.Count;

                  //  string makeContainerSQL = " SELECT PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, NCS.CONTAINER_SIZE_INWORDS, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME, PSIO.ITEM_SALES_DESCRIPTION, (PEWC.ITEM_WEIGHT / 1000) AS ITEM_WEIGHT, PEWC.MAT_PRICE_PER_MT, PEWC.MATERIAL_AMOUNT, (PEWC.CURRENCY_RATE * PEWC.MAT_PRICE_PER_MT) AS MAT_CONVERSION_PRICE_PER_MT, PEWC.MATERIAL_CONVERSION_AMOUNT FROM MF_EXPORT_WBSLIP_CON PEWC LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN MF_ITEM_SALES PSIO ON PSIO.ITEM_ID = PEWC.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWC.SUB_ITEM_ID    LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID LEFT JOIN MF_EXPORT_SALES_MASTER PESM ON PESM.EXPORT_INVOICE_NO = PEWC.EXPORT_INVOICE_NO LEFT JOIN NRC_CURRENCY_RATE NCR ON NCR.CURRENCY_RATE_ID = PEWC.CURRENCY_RATE_ID LEFT JOIN NRC_CURRENCY NCS ON NCS.CURRENCY_ID = NCR.SOURCE_CURRENCY_ID LEFT JOIN NRC_CURRENCY NCT ON NCT.CURRENCY_ID = NCR.TARGET_CURRENCY_ID WHERE PESM.EXPORT_INVOICE_NO = '" + dt.Rows[i]["EXPORT_INVOICE_NO"].ToString() + "' ";

                    string makeContainerSQL = " SELECT PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME AS ITEM_NAME , SUM((PEWC.ITEM_WEIGHT / 1000)) AS ITEM_WEIGHT, PEWC.MAT_PRICE_PER_MT, SUM(PEWC.MATERIAL_AMOUNT) AS MATERIAL_AMOUNT, SUM((PEWC.CURRENCY_RATE * PEWC.MAT_PRICE_PER_MT)) AS MAT_CONVERSION_PRICE_PER_MT, SUM(PEWC.MATERIAL_CONVERSION_AMOUNT) AS MATERIAL_CONVERSION_AMOUNT FROM MF_EXPORT_WBSLIP_CON PEWC LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID LEFT JOIN MF_EXPORT_SALES_MASTER PESM ON PESM.EXPORT_INVOICE_NO = PEWC.EXPORT_INVOICE_NO LEFT JOIN NRC_CURRENCY_RATE NCR ON NCR.CURRENCY_RATE_ID = PEWC.CURRENCY_RATE_ID LEFT JOIN NRC_CURRENCY NCS ON NCS.CURRENCY_ID = NCR.SOURCE_CURRENCY_ID LEFT JOIN NRC_CURRENCY NCT ON NCT.CURRENCY_ID = NCR.TARGET_CURRENCY_ID WHERE PESM.EXPORT_INVOICE_NO = '" + dt.Rows[i]["EXPORT_INVOICE_NO"].ToString() + "' GROUP BY PI.ITEM_NAME || ' ' || PSI.SUB_ITEM_NAME , PEWC.MAT_PRICE_PER_MT, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE  ";


                    cmdl = new OracleCommand(makeContainerSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    ds = new DataTable();
                    oradata.Fill(ds);
                    RowCount2 = ds.Rows.Count;
                    TotalCotainer += RowCount2;
                    List<string> ItemDeslist = new List<string>();
                    List<string> ItempRatePer = new List<string>();
                    for (int x = 0; x < RowCount2; x++)
                    {
                        ItemDeslist.Add(ds.Rows[x]["ITEM_NAME"].ToString());
                        ItempRatePer.Add(ds.Rows[x]["MAT_PRICE_PER_MT"].ToString());
                        MatWeight += Convert.ToDouble(ds.Rows[x]["ITEM_WEIGHT"].ToString());

                    }
                    List<string> ItemDeslistDis = ItemDeslist.Distinct().ToList();
                    List<string> ItemRatelistPer = ItempRatePer.Distinct().ToList();
                    int ItemDeslistDisCount = ItemDeslistDis.Count;
                    int ItemRatelistPerCount = ItemRatelistPer.Count;

                    HtmlString += "<tr> ";
                    HtmlString += "<td rowspan=" + RowCount2 + " style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px; font-size: 12px;text-align:center;'>" + dt.Rows[i]["EXPORT_INVOICE_NO"].ToString() + "</td>";
                    HtmlString += "<td rowspan=" + RowCount2 + " style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'>" + dt.Rows[i]["PAY_TERMS_NAME"].ToString() + "</td>";
                    HtmlString += "<td rowspan=" + RowCount2 + " style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'>" + dt.Rows[i]["SHIPMENT_DATE"].ToString() + "</td>"; 
                    HtmlString += "<td rowspan=" + RowCount2 + " style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'>" + dt.Rows[i]["TRADING_VESSEL_NAME"].ToString() + " " + dt.Rows[i]["TRADING_VESSEL_CODE"].ToString() + " </td> ";
                    HtmlString += "<td rowspan=" + RowCount2 + " style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'>" + dt.Rows[i]["PARTY_NAME"].ToString() + "</td> ";

                    HtmlString += "<td rowspan=" + RowCount2 + " style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'>";
                    int m = 1;
                    for (int j = 0; j < RowCount1; j++)
                    {
                        string ContractNo = dc.Rows[j]["CONTRACT_NO"].ToString();
                        if (m == RowCount1)
                        {
                            HtmlString += "" + dc.Rows[j]["CONTRACT_NO"].ToString() + " ";
                        }
                        else { HtmlString += "" + dc.Rows[j]["CONTRACT_NO"].ToString() + "</br>"; }
                        m++;
                    }
                    HtmlString += "</td>";

                    for (int l = 0; l < RowCount2; l++)
                    {

                        double MatPricePerMt = Convert.ToDouble(Convert.ToDecimal(ds.Rows[l]["MAT_PRICE_PER_MT"].ToString()));
                        double ItemAmt = Convert.ToDouble(Convert.ToDecimal(ds.Rows[l]["MATERIAL_AMOUNT"]));

                        /*  if (l < ItemDeslistDisCount)
                           {
                               if (ds.Rows[l].ToString() == ItemDeslistDis[l].ToString())
                               {
                                   HtmlString += "<td style='border-right:black solid 1px;'>  </td> ";
                               }
                               else { HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'>" + ds.Rows[l]["ITEM_NAME"].ToString() + " </td> "; }
                           }
                           else { HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;'>  </td> "; } */
                        if (l < RowCount2)
                        {
                            HtmlString += "<td style='border-right:black solid 1px;text-align:right;border-bottom:black solid 1px;'>" + ds.Rows[l]["ITEM_NAME"].ToString() + " </td> ";

                        }

                        if (l < RowCount2)
                        {
                            TotalInvoiceQty += Convert.ToDouble(decimal.Parse(ds.Rows[l]["ITEM_WEIGHT"].ToString()));
                            HtmlString += "<td style='border-right:black solid 1px;text-align:right;border-bottom:black solid 1px;'>" + string.Format("{0:n3}", Convert.ToDouble(ds.Rows[l]["ITEM_WEIGHT"].ToString())) + " </td> ";

                        }


                        if (l < RowCount2)
                        {
                            HtmlString += "<td style='border-right:black solid 1px;text-align:right;border-bottom:black solid 1px;'>" + string.Format("{0:n2}", Convert.ToDouble(ds.Rows[l]["MAT_PRICE_PER_MT"].ToString())) + " </td> ";

                        }


                        /*    else { HtmlString += "<td style='border-right:black solid 1px;text-align:right;'>  </td> "; }
                            if (l < ItemRatelistPerCount)
                            {
                                if (ds.Rows[l].ToString() == ItemRatelistPer[l].ToString())
                                {
                                    HtmlString += "<td style='border-bottom:black solid 1px;border-right:black solid 1px;'>  </td> ";
                                }
                                else { HtmlString += "<td style='text-align:center;border-bottom:black solid 1px;border-right:black solid 1px;'>" + string.Format("{0:n2}", Convert.ToDouble(ds.Rows[l]["MAT_PRICE_PER_MT"].ToString())) + " </td> "; }

                            }
                            else { HtmlString += "<td >  </td> "; } */

                        if (l < RowCount2)
                        {
                            HtmlString += "<td style='border-right:black solid 1px;text-align:right;border-bottom:black solid 1px;'>" + string.Format("{0:n2}", Convert.ToDouble(ds.Rows[l]["MATERIAL_AMOUNT"].ToString())) + "</td> ";
                            TotalInvoiceAmt += Convert.ToDouble(Math.Round(decimal.Parse(ds.Rows[l]["MATERIAL_AMOUNT"].ToString()), 2));
                        }
                        else
                        {
                            HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;'>  </td> ";
                        }

                        HtmlString += "<td style='border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;'>" + ds.Rows[l]["CONTAINER_NO"].ToString() + "</td> ";


                        HtmlString += "</tr> ";
                    }
                }
                HtmlString += "<tr> ";
                HtmlString += "<td colspan='7' style='border-top:black solid 1px;border-bottom:black solid 1px;border-left:black solid 1px;border-right:black solid 1px; text-align:right;font-weight:700;'> TOTAL: </td> ";
                HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> " + string.Format("{0:n3}", TotalInvoiceQty) + " </td>";
                HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;'> </td> ";
                HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:right;font-weight:700;'> " + string.Format("{0:n2}", TotalInvoiceAmt) + " </td>";
                HtmlString += "<td style='border-top:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;text-align:center;font-weight:700;'> " + TotalCotainer + "</td> ";
                HtmlString += "</tr> ";
                HtmlString += "</tr> ";

                string NumberToInWordUSD = NumberToInWords.DecimalToWordsUSD(Convert.ToDecimal(TotalInvoiceAmt)).Trim().ToUpper();
                HtmlString += "<tr> ";
                HtmlString += "<td colspan='12' style='border-left:black solid 1px;border-right:black solid 1px;border-bottom:black solid 1px;-webkit-border-bottom-left-radius:10px;-webkit-border-bottom-right-radius:10px;'>TOTAL VALUE  " + NumberToInWordUSD + " </td> ";
                HtmlString += "</tr> ";
                HtmlString += "<tr> ";
                HtmlString += "<td colspan='12' style=''>&nbsp;</td> ";
                HtmlString += "</tr> ";
                HtmlString += "<tr> ";
                HtmlString += "<td colspan='12' style=''>&nbsp;</td> ";
                HtmlString += "</tr> ";
                HtmlString += "<tr> ";
                HtmlString += "<td colspan='6' style='text-align:left;'>PREPARED BY:______________________</td> ";
                HtmlString += "<td colspan='6' style='text-align:right;'>APPROVED BY:_______________________ </td> ";

                HtmlString += "</tr> ";

                HtmlString += "</table>";


                HtmlString += "</div>";



                HtmlString += "</div>";
                HtmlString += "</div>";
                HtmlString += "</div>";

                HtmlString += "</div>";

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