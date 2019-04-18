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
using System.Collections;
using System.Collections.Generic;
using System.Web.Services;

namespace NRCAPPS.MS
{
    public partial class MsExpContainer : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl, cmdsp;
        OracleDataAdapter oradata;
        DataTable dt;
        int RowCount;
        double ItemWtTotal = 0.0, ItemWtWbTotal = 0.0, ItemWbWeight = 0.0, TareWeight = 0.0, TotalBales = 0.0, NoOfWp = 0.0, WtPerWp = 0.0, TotalWpWt = 0.0, TotalItemWt = 0.0;
        string SealNo = "", RelOrderNo = "";
        string IS_PAGE_ACTIVE = "", IS_ADD_ACTIVE = "", IS_EDIT_ACTIVE = "", IS_DELETE_ACTIVE = "", IS_VIEW_ACTIVE = "", IS_REPORT_ACTIVE = "", IS_PRINT_ACTIVE = "";

        public bool IsLoad { get; set; }
        public bool IsLoad2 { get; set; }
        public bool IsLoad3 { get; set; }
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
                    IS_PRINT_ACTIVE = dt.Rows[i]["IS_PRINT_ACTIVE"].ToString();
                }

                if (IS_PAGE_ACTIVE == "Enable")
                {
                    if (!IsPostBack)
                    {
                         
                        DataTable dtCategoryID = new DataTable();
                        DataSet dss = new DataSet();
                        string makeDropDownCatgorySQL = " SELECT * FROM MF_CATEGORY WHERE IS_ACTIVE = 'Enable' ORDER BY CATEGORY_ID ASC";
                        dss = ExecuteBySqlString(makeDropDownCatgorySQL);
                        dtCategoryID = (DataTable)dss.Tables[0];
                        DropDownCategoryID.DataSource = dtCategoryID;
                        DropDownCategoryID.DataValueField = "CATEGORY_ID";
                        DropDownCategoryID.DataTextField = "CATEGORY_NAME";
                        DropDownCategoryID.DataBind();
                        DropDownCategoryID.Items.Insert(0, new ListItem("Select Category", "0"));

                        DataTable dtContainerID = new DataTable();
                        DataSet dsc = new DataSet();
                        string makeDropDownContainerSQL = " SELECT CONTAINER_SIZE_ID, CONTAINER_SIZE || '- (' || CONTAINER_SIZE_INWORDS || ')' AS CONTAINER_SIZE FROM NRC_CONTAINER_SIZE WHERE IS_ACTIVE = 'Enable' ORDER BY CONTAINER_SIZE_ID ASC";
                        dsc = ExecuteBySqlString(makeDropDownContainerSQL);
                        dtContainerID = (DataTable)dsc.Tables[0];
                        DropDownContainerSizeID.DataSource = dtContainerID;
                        DropDownContainerSizeID.DataValueField = "CONTAINER_SIZE_ID";
                        DropDownContainerSizeID.DataTextField = "CONTAINER_SIZE";
                        DropDownContainerSizeID.DataBind();

                        DataTable dtWbSlipID = new DataTable();
                        DataSet dse = new DataSet();
                        string makeSrSQL = "   SELECT WEWC.WB_SLIP_NO FROM MS_EXPORT_WBSLIP_CON WEWC WHERE WEWC.IS_ACTIVE = 'Enable' AND WEWC.EXPORT_INVOICE_NO IS NULL AND NOT EXISTS(SELECT WEWCI.WB_SLIP_NO FROM MS_EXPORT_WBSLIP_CON_ITEM WEWCI WHERE WEWCI.WB_SLIP_NO = WEWC.WB_SLIP_NO) ORDER BY WEWC.WB_SLIP_NO DESC ";
                        dse = ExecuteBySqlString(makeSrSQL);
                        dtWbSlipID = (DataTable)dse.Tables[0];
                        DropDownSlipID.DataSource = dtWbSlipID;
                        DropDownSlipID.DataValueField = "WB_SLIP_NO";
                        DropDownSlipID.DataTextField = "WB_SLIP_NO";
                        DropDownSlipID.DataBind();
                        DropDownSlipID.Items.Insert(0, new ListItem("Select  Weight Slip", "0"));

                        DataTable dtPacking1ID = new DataTable();
                        DataSet dsp = new DataSet();
                        string makeDropDownPacking1SQL = " SELECT * FROM NRC_PACKING_LIST WHERE IS_ACTIVE = 'Enable' ORDER BY PACKING_ID ASC";
                        dsp = ExecuteBySqlString(makeDropDownPacking1SQL);
                        dtPacking1ID = (DataTable)dsp.Tables[0];
                        DropDownPacking1.DataSource = dtPacking1ID;
                        DropDownPacking1.DataValueField = "PACKING_ID";
                        DropDownPacking1.DataTextField = "PACKING_NAME";
                        DropDownPacking1.DataBind();
                        DropDownPacking1.Items.Insert(0, new ListItem("Select Packing List", "0"));
                        DropDownPacking1.Items.FindByValue("1").Selected = true;

                        DataTable dtPartyID = new DataTable();
                        DataSet dpt = new DataSet();
                        string makeDropDowndPartyIDSQL = " SELECT MP.PARTY_ID,  MP.PARTY_ID || ' : ' || MP.PARTY_NAME || ' - ' || MP.PARTY_VAT_NO  AS PARTY_NAME  FROM MS_PARTY MP WHERE MP.IS_ACTIVE = 'Enable' AND MP.IS_SALES_ACTIVE = 'Enable' ORDER BY MP.PARTY_NAME ASC";
                        dpt = ExecuteBySqlString(makeDropDowndPartyIDSQL);
                        dtPartyID = (DataTable)dpt.Tables[0];
                        DropDownPartyID.DataSource = dtPartyID;
                        DropDownPartyID.DataValueField = "PARTY_ID";
                        DropDownPartyID.DataTextField = "PARTY_NAME";
                        DropDownPartyID.DataBind();
                        DropDownPartyID.Items.Insert(0, new ListItem("Select Customer", "0"));

                        TextSlipWbNo.Focus();

                        DropDownItemID.Enabled = false;
                        TextItemWeightEx.Enabled = false; 
                        DropDownCategoryID.Enabled = false; 
                        TextItemWtWb.Attributes.Add("readonly", "readonly"); 

                        Display();
                        DisplaySummary();
                        alert_box.Visible = false;

                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled");
                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");

                        BtnDeleteItem.Attributes.Add("aria-disabled", "false");
                        BtnDeleteItem.Attributes.Add("class", "btn btn-danger disabled");
                        BtnUpdateItem.Attributes.Add("aria-disabled", "false");
                        BtnUpdateItem.Attributes.Add("class", "btn btn-success disabled");

                        //    BtnAdd.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(BtnAdd, null) + ";");

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


        public void Redio_MultipleChanged(object sender, EventArgs e)
        {
            if (radPurDuplicate.SelectedValue == "One")
            {
                 
            }
            else
            {
                DataTable dtWbSlipID = new DataTable();
                DataSet dse = new DataSet();
                string makeSrSQL = "   SELECT WEWC.WB_SLIP_NO FROM MS_EXPORT_WBSLIP_CON WEWC WHERE WEWC.IS_ACTIVE = 'Enable' AND WEWC.EXPORT_INVOICE_NO IS NULL ORDER BY WEWC.WB_SLIP_NO DESC ";
                dse = ExecuteBySqlString(makeSrSQL);
                dtWbSlipID = (DataTable)dse.Tables[0];
                DropDownSlipID.DataSource = dtWbSlipID;
                DropDownSlipID.DataValueField = "WB_SLIP_NO";
                DropDownSlipID.DataTextField = "WB_SLIP_NO";
                DropDownSlipID.DataBind();
                DropDownSlipID.Items.Insert(0, new ListItem("Select  Weight Slip", "0"));

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
                    string SlipNo = TextSlipWbNo.Text;
                    int ContainerSizeID = Convert.ToInt32(DropDownContainerSizeID.Text);
                    int PartyID = Convert.ToInt32(DropDownPartyID.Text);
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string get_container_id = "select MS_EXPORT_CONTAINERID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_container_id, conn);
                    int newContainerID = Int32.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_purchase = "insert into  MS_EXPORT_WBSLIP_CON (EXP_WBCON_ID, WB_SLIP_NO, PARTY_ID, SEAL_NO, REF_NO, BOOKING_NO, CONTAINER_NO, CONTAINER_SIZE_ID, ITEM_WEIGHT_WB, TARE_WEIGHT, DISPATCH_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID, IS_INVENTORY_STATUS, IS_EDIT ) values  ( :NoContainerID, :NoSlipID, :NoPartyID, :TextSealNo, :TextRefNo, :TextBookingNo, :TextContainerNo, :NoContainerSizeID, :TextItemWeightWb, :TextTareWeight, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 2, 'Transit', 'Enable' )";
                    cmdi = new OracleCommand(insert_purchase, conn);

                    OracleParameter[] objPrm = new OracleParameter[14];
                    objPrm[0] = cmdi.Parameters.Add("NoContainerID", newContainerID);
                    objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNo);
                    objPrm[2] = cmdi.Parameters.Add("NoPartyID", PartyID);
                    objPrm[3] = cmdi.Parameters.Add("TextSealNo", TextSealNo.Text);
                    objPrm[4] = cmdi.Parameters.Add("TextRefNo", TextRefNo.Text);
                    objPrm[5] = cmdi.Parameters.Add("TextBookingNo", TextBookingNo.Text);
                    objPrm[6] = cmdi.Parameters.Add("TextContainerNo", TextContainerNo.Text);
                    objPrm[7] = cmdi.Parameters.Add("NoContainerSizeID", ContainerSizeID);
                    objPrm[8] = cmdi.Parameters.Add("TextItemWeightWb", TextItemWeightWb.Text);//TextItemWtWb
                    objPrm[9] = cmdi.Parameters.Add("TextTareWeight", TextTareWeight.Text);
                    objPrm[10] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[11] = cmdi.Parameters.Add("c_date", c_date);
                    objPrm[12] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[13] = cmdi.Parameters.Add("TextIsActive", ISActive);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();

                    DataTable dtWbSlipID = new DataTable();
                    DataSet dse = new DataSet();
                    string makeSrSQL = " SELECT WB_SLIP_NO FROM MS_EXPORT_WBSLIP_CON WHERE IS_ACTIVE = 'Enable' AND EXPORT_INVOICE_NO IS NULL ORDER BY WB_SLIP_NO DESC";
                    dse = ExecuteBySqlString(makeSrSQL);
                    dtWbSlipID = (DataTable)dse.Tables[0];
                    DropDownSlipID.DataSource = dtWbSlipID;
                    DropDownSlipID.DataValueField = "WB_SLIP_NO";
                    DropDownSlipID.DataTextField = "WB_SLIP_NO";
                    DropDownSlipID.DataBind();
                    DropDownSlipID.Items.Insert(0, new ListItem("Select  Weight Slip", "0"));

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Weight Slip Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                    clearText();
                    TextSlipWbNo.Focus();
                    Display();
                    //  DisplaySummary();

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
         
        public void BtnAddItem_Click(object sender, EventArgs e)
        {
            //   try
            //  {
            if (IS_ADD_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string SlipNo = DropDownSlipID.Text;
                // int ItemID = Convert.ToInt32(DropDownItemID.Text);
                int ItemID = Convert.ToInt32(Request.Form[DropDownItemID.UniqueID]);
                int ItemWsID = Convert.ToInt32(Request.Form[DropDownItemWsID.UniqueID]);
                int CategoryID = Convert.ToInt32(DropDownCategoryID.Text);
                int BalesNumber = Convert.ToInt32(DropDownPacking1.Text);
                string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                double ItemWeightEx = Convert.ToDouble(TextItemWeightEx.Text);

                string get_id = "select MS_EXPORT_WBS_CON_ITEMID_SEQ.nextval from dual";
                cmdsp = new OracleCommand(get_id, conn);
                int newExWbID = Int32.Parse(cmdsp.ExecuteScalar().ToString());
                string insert_data = "insert into  MS_EXPORT_WBSLIP_CON_ITEM (EXP_WBCON_ITEM_ID, WB_SLIP_NO, CATEGORY_ID, ITEM_ID, ITEM_WS_ID, ITEM_WEIGHT, PACKING_ID, NUMBER_OF_PACK, PACK_PER_WEIGHT, PACKING_WEIGHT, IS_INVENTORY_STATUS, CREATE_DATE, C_USER_ID) values  ( :NoExpWbConID, :NoSlipID, :NoCategoryID, :NoItemID, :NoItemWsID, :TextItemWeightEx, :NoDropDownPacking1, :NoOfPacking1, :NoPerWtPacking1, :NoWtTotalPacking1, :TextInvenStatus, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                cmdi = new OracleCommand(insert_data, conn);

                OracleParameter[] objPrm = new OracleParameter[13];
                objPrm[0] = cmdi.Parameters.Add("NoExpWbConID", newExWbID);
                objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNo);
                objPrm[2] = cmdi.Parameters.Add("NoCategoryID", CategoryID);
                objPrm[3] = cmdi.Parameters.Add("NoItemID", ItemID);
                objPrm[4] = cmdi.Parameters.Add("NoItemWsID", ItemWsID);
                objPrm[5] = cmdi.Parameters.Add("TextItemWeightEx", ItemWeightEx);
                objPrm[6] = cmdi.Parameters.Add("NoDropDownPacking1", Convert.ToInt32(DropDownPacking1.Text));
                objPrm[7] = cmdi.Parameters.Add("NoOfPacking1", Convert.ToInt32(TextNoOfPacking1.Text));
                objPrm[8] = cmdi.Parameters.Add("NoPerWtPacking1", Convert.ToDouble(TextNoPerWtPacking1.Text));
                objPrm[9] = cmdi.Parameters.Add("NoWtTotalPacking1", Convert.ToDouble(TextWtTotalPacking1.Text)); 
                objPrm[10] = cmdi.Parameters.Add("TextInvenStatus", "Transit");
                objPrm[11] = cmdi.Parameters.Add("c_date", c_date);
                objPrm[12] = cmdi.Parameters.Add("NoCuserID", userID);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();


                string update_user = "update  MS_EXPORT_WBSLIP_CON  set IS_EDIT =:TextIsEdit where WB_SLIP_NO =:NoSlipNo ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPr = new OracleParameter[2];
                objPr[0] = cmdi.Parameters.Add("TextIsEdit", "Disable");
                objPr[1] = cmdi.Parameters.Add("NoSlipNo", SlipNo);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Insert New Weight Slip Material Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                clearTextItem();
                TextSlipWbNo.Focus();
                Display();
                DisplaySummary();
                DropDownSlipID.Enabled = false;

            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
            //    }
            //    catch
            //   {
            //      Response.Redirect("~/ParameterError.aspx");
            //   }
        }

        [WebMethod]
        public static List<ListItem> GetItemWsList(int ItemId)
        {
            string query = " SELECT ITEM_WS_ID, ITEM_WS_DESCRIPTION FROM MS_WS_ITEM WHERE ITEM_ID = :ItemID AND IS_ACTIVE = 'Enable' ORDER BY ITEM_WS_ID ASC ";
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
                                Value = sdr["ITEM_WS_ID"].ToString(),
                                Text = sdr["ITEM_WS_DESCRIPTION"].ToString()
                            });
                        }
                    }
                    conn.Close();
                    return cities;
                }
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            if (IS_PRINT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                LinkButton btn = (LinkButton)sender;
                Session["user_data_id"] = btn.CommandArgument;
                string SlipWbNo = Session["user_data_id"].ToString();


                string HtmlString = "";
                string makeSQL = " SELECT PEWC.EXP_WBCON_ID, PEWC.WB_SLIP_NO, PEWC.SEAL_NO, PEWC.CONTAINER_NO, PEWC.REF_NO, PEWC.BOOKING_NO, nvl(PEWC.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, TARE_WEIGHT, PI.ITEM_NAME, WWI.ITEM_WS_DESCRIPTION, PEWCI.ITEM_WEIGHT, PEWCI.NUMBER_OF_PACK, PEWCI.PACK_PER_WEIGHT, NPL.PACKING_NAME FROM MS_EXPORT_WBSLIP_CON PEWC LEFT JOIN MS_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN MS_WS_ITEM WWI ON WWI.ITEM_WS_ID = PEWCI.ITEM_WS_ID LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = PEWCI.PACKING_ID WHERE PEWC.WB_SLIP_NO = '" + SlipWbNo + "' ORDER BY PI.ITEM_ID ";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                string ContainerNo = "", SealNo = "", BookingNo = "";
                for (int i = 0; i < 1; i++)
                {
                    ContainerNo = dt.Rows[i]["CONTAINER_NO"].ToString();
                    SealNo = dt.Rows[i]["SEAL_NO"].ToString();
                    RelOrderNo = dt.Rows[i]["REF_NO"].ToString();
                    BookingNo = dt.Rows[i]["BOOKING_NO"].ToString();
                    HtmlString += "<div style='float:left;width:785px;height:258px;margin-top:275px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 13px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";
                    HtmlString += "<div style='float:left;width:470px;'> ";
                    HtmlString += "<div style='float:left;width:460px;height:50px;margin-left:5px;padding:35px 0 0 5px;font-size: 16px;'> ";
                    HtmlString += "#################&nbsp;CONTAINER NO.:&nbsp;&nbsp;" + ContainerNo + "";
                    HtmlString += "</div> ";
                 

                }
             //   int m = 1;
                
                double PakingWt = 0.0, PakingPerWt = 0.0;
                for (int i = 0; i < RowCount; i++)
                {
                    string ItemName = dt.Rows[i]["ITEM_NAME"].ToString();
                    string ItemNameWs = dt.Rows[i]["ITEM_WS_DESCRIPTION"].ToString();
                  
                    ItemWbWeight = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString());
                    PakingWt += Convert.ToDouble(dt.Rows[i]["NUMBER_OF_PACK"].ToString());
                    PakingPerWt = Convert.ToDouble(dt.Rows[i]["PACK_PER_WEIGHT"].ToString());
                    TareWeight = Convert.ToDouble(dt.Rows[i]["TARE_WEIGHT"].ToString()); 
                    TotalItemWt += Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());

                   /* if (m == RowCount)
                    {
                        HtmlString += "" + ItemNameWs + " ";  // ("+ ItemName + ")
                    }
                    else { HtmlString += "" + ItemName + ", "; } // (" + ItemName + ")
                    m++;*/
                }
                 

                
                HtmlString += "<div style='float:left;width:380px;height:130px;margin-left:182px;padding:5px 0 0 5px;'> ";
                int n = 1;
                string PackingName = "";
               
                for (int i = 0; i < RowCount; i++)
                { 
                    PackingName = dt.Rows[i]["PACKING_NAME"].ToString(); 
                    HtmlString += "" + dt.Rows[i]["NUMBER_OF_PACK"].ToString() + " " + dt.Rows[i]["PACKING_NAME"].ToString() + "-" + dt.Rows[i]["ITEM_WS_DESCRIPTION"].ToString() + " = " + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString())) + " KG</br>";
                 
                }

                HtmlString += "</div> ";
                HtmlString += "<div style='float:left;width:260px;height:38px;margin-left:103px;padding-top:0px;font-size: 16px;' >SEAL NO:&nbsp; " + SealNo + " </div> ";


                HtmlString += "</div> ";

                HtmlString += "<div style='float:left;width:310px;'> ";

                HtmlString += "<table cellpadding='3' cellspacing='0' style='font-size:13px;font-weight: 700;' width=100%>";
                HtmlString += "<tr>";
                HtmlString += "<td style='text-align:right;'>Less Tare Wt. (KG)</td> ";
                HtmlString += "<td style='text-align:right;'>" + string.Format("{0:n0}", TareWeight) + "</td> ";
                HtmlString += "</tr>"; 
                HtmlString += "<tr>";
                HtmlString += "<td style='text-align:right;'>Net Wt. (KG)</td> ";
                HtmlString += "<td style='text-align:right;border-top:black solid 1px;'>" + string.Format("{0:n0}", (ItemWbWeight - TareWeight)) + "</td> ";
                HtmlString += "</tr>"; 
                HtmlString += "<tr>";
                HtmlString += "<td style='text-align:right;'>Less " + PackingName + " (" + PakingWt + "x" + PakingPerWt + ")= (KG)</td> ";
                HtmlString += "<td style='text-align:right;'>" + string.Format("{0:n0}", (PakingWt * PakingPerWt)) + "</td> ";
                HtmlString += "</tr>"; 
                HtmlString += "<tr>";
                HtmlString += "<td style='text-align:right;'>Final Net Wt. (KG)</td> ";
                HtmlString += "<td style='text-align:right;border-top:black solid 1px;'>" + string.Format("{0:n0}", ((ItemWbWeight - TareWeight) - (PakingWt * PakingPerWt))) + "</td> ";
                HtmlString += "</tr>"; 
                HtmlString += "<tr>";
                HtmlString += "<td colspan=2 style='text-align:center;'>&nbsp;</td> ";
                HtmlString += "</tr>";
                HtmlString += "<tr>";
                HtmlString += "<td colspan=2 style='text-align:center;'>&nbsp;</td> ";
                HtmlString += "</tr>";
                HtmlString += "<tr>";
                HtmlString += "<td colspan=2 style='text-align:center;'>&nbsp;</td> ";
                HtmlString += "</tr>";
                HtmlString += "<tr>";
                HtmlString += "<td colspan=2 style='text-align:center;'>&nbsp;</td> ";
                HtmlString += "</tr>";
                HtmlString += "<tr>";
                HtmlString += "<td colspan=2 style='text-align:center;font-size:15px;'>BOOKING# " + BookingNo + "</td> ";
                HtmlString += "</tr>";
                HtmlString += "<tr>";
                HtmlString += "<td colspan=2 style='text-align:center;font-size:15px;'>REF#: " + RelOrderNo + "</td> ";
                HtmlString += "</tr>";
                HtmlString += "<tr>";
                HtmlString += "<td colspan=2 style='text-align:center;font-size:15px;'>FOR EXPORTS ONLY</td> "; 
                HtmlString += "</tr>";

                HtmlString += "</table>";

            
                HtmlString += "</div>";
                HtmlString += "</div>";

                HtmlString += "</div>";
                HtmlString += "</div>";
                HtmlString += "</div>";
                HtmlString += "</div>";

                // weigh-bridge & container update for print
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string update_user = "update  MS_EXPORT_WBSLIP_CON  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where WB_SLIP_NO = :NoSlipNo ";
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

       
        [WebMethod]
        public static List<ListItem> GetItemFinalStock(int ItemId)
        {
            string query = " SELECT WRSM.ITEM_ID, ROUND(nvl(WRSM.FINAL_STOCK_WT,0) - SUM(nvl(WEWCI.ITEM_WEIGHT,0)),2) AS FINAL_STOCK_WT FROM MS_RM_STOCK_INVENTORY_MASTER WRSM LEFT JOIN MS_EXPORT_WBSLIP_CON_ITEM WEWCI ON WEWCI.ITEM_ID = WRSM.ITEM_ID AND IS_INVENTORY_STATUS = 'Transit' where WRSM.ITEM_ID =:ItemID GROUP BY WRSM.ITEM_ID, WRSM.FINAL_STOCK_WT ";
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
                                Text = sdr["FINAL_STOCK_WT"].ToString()
                            });
                        }
                    }
                    conn.Close();
                    return cities;
                }
            }
        }

        [WebMethod]
        public static List<ListItem> GetItemList()
        {
            //  OracleConnection conn = new OracleConnection(strConnString);
            //   conn.Open();
            string query = " SELECT * FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' AND IS_PURCHASE_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC ";
            string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            //      string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (OracleConnection conn = new OracleConnection(strConnString))
            {
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    List<ListItem> cities = new List<ListItem>();
                    cmd.CommandType = CommandType.Text;
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
            //   try{
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
            string USER_DATA_ID = Session["user_data_id"].ToString();

            string makeSQL = " select EXP_WBCON_ID, WB_SLIP_NO, SEAL_NO, REF_NO, BOOKING_NO, CONTAINER_NO, CONTAINER_SIZE_ID, nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, nvl(TARE_WEIGHT,0) AS TARE_WEIGHT, TO_CHAR(DISPATCH_DATE,'dd/mm/yyyy') AS DISPATCH_DATE, IS_ACTIVE from MS_EXPORT_WBSLIP_CON where WB_SLIP_NO  = '" + USER_DATA_ID + "'";
            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;

            for (int i = 0; i < RowCount; i++)
            {
                TextExWbConID.Text = dt.Rows[i]["EXP_WBCON_ID"].ToString();
                TextSlipWbNo.Text = dt.Rows[i]["WB_SLIP_NO"].ToString();
                TextSealNo.Text = dt.Rows[i]["SEAL_NO"].ToString();
                TextRefNo.Text = dt.Rows[i]["REF_NO"].ToString();
                TextBookingNo.Text = dt.Rows[i]["BOOKING_NO"].ToString();
                TextContainerNo.Text = dt.Rows[i]["CONTAINER_NO"].ToString();
                DropDownContainerSizeID.Text = dt.Rows[i]["CONTAINER_SIZE_ID"].ToString();
                EntryDate.Text = dt.Rows[i]["DISPATCH_DATE"].ToString();
                TextItemWeightWb.Text = dt.Rows[i]["ITEM_WEIGHT_WB"].ToString();
                TextTareWeight.Text = dt.Rows[i]["TARE_WEIGHT"].ToString();
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);

            }

            conn.Close();
            Display();
            CheckSlipNo.Text = "";
            alert_box.Visible = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

            BtnDelete.Attributes.Add("aria-disabled", "true");
            BtnDelete.Attributes.Add("class", "btn btn-danger active");
            BtnUpdate.Attributes.Add("aria-disabled", "true");
            BtnUpdate.Attributes.Add("class", "btn btn-success active");

            TextSlipWbNo.Attributes.Add("readonly", "readonly");
            //    }
            //     catch
            //   {
            //      Response.Redirect("~/ParameterError.aspx");
            //   } 
        }

        protected void linkSelectItemClick(object sender, EventArgs e)
        {
            //   try
            //   {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
            int USER_DATA_ID = Convert.ToInt32(Session["user_data_id"]);

            DataTable dtWbSlipID = new DataTable();
            DataSet dse = new DataSet();
            string makeSriSQL = " SELECT WB_SLIP_NO FROM MS_EXPORT_WBSLIP_CON WHERE IS_ACTIVE = 'Enable' AND EXPORT_INVOICE_NO IS NULL ORDER BY WB_SLIP_NO DESC";
            dse = ExecuteBySqlString(makeSriSQL);
            dtWbSlipID = (DataTable)dse.Tables[0];
            DropDownSlipID.DataSource = dtWbSlipID;
            DropDownSlipID.DataValueField = "WB_SLIP_NO";
            DropDownSlipID.DataTextField = "WB_SLIP_NO";
            DropDownSlipID.DataBind();
            DropDownSlipID.Items.Insert(0, new ListItem("Select  Weight Slip", "0"));

            string makeSQL = " select EXP_WBCON_ITEM_ID, WB_SLIP_NO, ITEM_ID, ITEM_WS_ID, CATEGORY_ID, ITEM_WEIGHT, PACKING_ID, NUMBER_OF_PACK, PACK_PER_WEIGHT, PACKING_WEIGHT from MS_EXPORT_WBSLIP_CON_ITEM where EXP_WBCON_ITEM_ID  = '" + USER_DATA_ID + "'";
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

                DataTable dtItemWsID = new DataTable();
                DataSet dsis = new DataSet();
                string makeDropDownItemWsSQL = " SELECT ITEM_WS_ID, ITEM_WS_DESCRIPTION FROM MS_WS_ITEM WHERE ITEM_ID = '"+ Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString()) + "' AND IS_ACTIVE = 'Enable' ORDER BY ITEM_WS_ID ASC ";
                dsis = ExecuteBySqlString(makeDropDownItemWsSQL);
                dtItemWsID = (DataTable)dsis.Tables[0];
                DropDownItemWsID.DataSource = dtItemWsID;
                DropDownItemWsID.DataValueField = "ITEM_WS_ID";
                DropDownItemWsID.DataTextField = "ITEM_WS_DESCRIPTION";
                DropDownItemWsID.DataBind();
                 
                TextExpWbConItemID.Text = dt.Rows[i]["EXP_WBCON_ITEM_ID"].ToString();
                DropDownSlipID.Text = dt.Rows[i]["WB_SLIP_NO"].ToString();
                DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();
                DropDownItemWsID.Text = dt.Rows[i]["ITEM_WS_ID"].ToString();
                DropDownCategoryID.Text = dt.Rows[i]["CATEGORY_ID"].ToString();
                DropDownPacking1.Text = dt.Rows[i]["PACKING_ID"].ToString();
                TextNoOfPacking1.Text = dt.Rows[i]["NUMBER_OF_PACK"].ToString();
                TextNoPerWtPacking1.Text = dt.Rows[i]["PACK_PER_WEIGHT"].ToString();
                TextWtTotalPacking1.Text = dt.Rows[i]["PACKING_WEIGHT"].ToString();
                TextItemWeightEx.Text = dt.Rows[i]["ITEM_WEIGHT"].ToString();

                //  string USER_DATA_ID = DropDownSlipID.Text;
                string makeSrSQL = " select ITEM_WEIGHT_WB from MS_EXPORT_WBSLIP_CON where WB_SLIP_NO  = '" + dt.Rows[i]["WB_SLIP_NO"].ToString() + "'";
                cmdl = new OracleCommand(makeSrSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                for (int j = 0; j < RowCount; j++)
                {
                    TextItemWtWb.Text = dt.Rows[j]["ITEM_WEIGHT_WB"].ToString();
                }

            }

            conn.Close();
            Display();
            CheckSlipNo.Text = "";
            alert_box.Visible = false;
            TextItemWeightEx.Enabled = true;
            DropDownPacking1.Enabled = true;
            DropDownItemID.Enabled = true;
            DropDownItemWsID.Enabled = true; 
            DropDownCategoryID.Enabled = true;
            BtnAddItem.Style["visibility"] = "hidden";
            BtnAddItem.Attributes.Add("aria-disabled", "false");
            BtnAddItem.Attributes.Add("class", "btn btn-primary disabled");

            BtnDeleteItem.Attributes.Add("aria-disabled", "true");
            BtnDeleteItem.Attributes.Add("class", "btn btn-danger active");
            BtnUpdateItem.Attributes.Add("aria-disabled", "true");
            BtnUpdateItem.Attributes.Add("class", "btn btn-success active");

            DropDownSlipID.Enabled = false;
            //  }
            //  catch
            //  {
            //      Response.Redirect("~/ParameterError.aspx");
            //  }
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
                int ExWbConID = Convert.ToInt32(TextExWbConID.Text);
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                int ContainerSizeID = Convert.ToInt32(DropDownContainerSizeID.Text);
                string MakeEntryDate = EntryDate.Text;
                string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  MS_EXPORT_WBSLIP_CON  set WB_SLIP_NO =:NoSlipNo, SEAL_NO =:TextSealNo, REF_NO =:TextRefNo, BOOKING_NO =:TextBookingNo, CONTAINER_NO =:NoContainerNo, CONTAINER_SIZE_ID =:NoContainerSize, ITEM_WEIGHT_WB =:NoItemWeightWb, TARE_WEIGHT =:TextTareWeight, DISPATCH_DATE = TO_DATE(:DispatchDate, 'DD/MM/YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE =:TextIsActive where EXP_WBCON_ID =:NoExWbConID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[13];
                objPrm[0] = cmdi.Parameters.Add("NoSlipNo", TextSlipWbNo.Text);
                objPrm[1] = cmdi.Parameters.Add("TextSealNo", TextSealNo.Text);
                objPrm[2] = cmdi.Parameters.Add("TextRefNo", TextRefNo.Text);
                objPrm[3] = cmdi.Parameters.Add("TextBookingNo", TextBookingNo.Text);
                objPrm[4] = cmdi.Parameters.Add("NoContainerNo", TextContainerNo.Text);
                objPrm[5] = cmdi.Parameters.Add("NoContainerSize", ContainerSizeID);
                objPrm[6] = cmdi.Parameters.Add("NoItemWeightWb", TextItemWeightWb.Text);
                objPrm[7] = cmdi.Parameters.Add("TextTareWeight", TextTareWeight.Text);
                objPrm[8] = cmdi.Parameters.Add("DispatchDate", EntryDateNew);
                objPrm[9] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[10] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[11] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[12] = cmdi.Parameters.Add("NoExWbConID", ExWbConID);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Container & Weight Slip Data Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearText();
                TextSlipWbNo.Focus();
                Display();
                DisplaySummary();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
            //     }
            //   catch
            //   {
            //       Response.Redirect("~/ParameterError.aspx");
            //  } 
        }

        protected void BtnUpdateItem_Click(object sender, EventArgs e)
        {
            // try
            //   {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int ExpWbConItemID = Convert.ToInt32(TextExpWbConItemID.Text);
                int ItemID = 0, ItemWsID = 0;
                if (Request.Form[DropDownItemID.UniqueID] == "0")
                {
                    ItemID = Convert.ToInt32(DropDownItemID.Text);
                }
                else
                {
                    ItemID = Convert.ToInt32(Request.Form[DropDownItemID.UniqueID]);
                }
                if (Request.Form[DropDownItemID.UniqueID] == "0")
                {
                    ItemWsID = Convert.ToInt32(DropDownItemWsID.Text);
                }
                else
                {
                    ItemWsID = Convert.ToInt32(Request.Form[DropDownItemWsID.UniqueID]);
                }


                //   

                int CategoryID = Convert.ToInt32(DropDownCategoryID.Text);
                int BalesNumber = Convert.ToInt32(DropDownPacking1.Text);
                double ItemWeightEx = Convert.ToDouble(TextItemWeightEx.Text);

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  MS_EXPORT_WBSLIP_CON_ITEM  set ITEM_ID =:NoItemID, ITEM_WS_ID =:NoItemWsID, CATEGORY_ID =:NoCategoryID, ITEM_WEIGHT =:TextItemWeightEx, NUMBER_OF_PACK =:NoBalesNumber, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where EXP_WBCON_ITEM_ID = :NoExpWbConItemID ";
                cmdu = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[8];
                objPrm[0] = cmdu.Parameters.Add("NoItemID", ItemID);
                objPrm[1] = cmdu.Parameters.Add("NoItemWsID", ItemWsID);
                objPrm[2] = cmdu.Parameters.Add("NoCategoryID", CategoryID);
                objPrm[3] = cmdu.Parameters.Add("TextItemWeightEx", ItemWeightEx);
                objPrm[4] = cmdu.Parameters.Add("NoBalesNumber", BalesNumber);
                objPrm[5] = cmdu.Parameters.Add("u_date", u_date);
                objPrm[6] = cmdu.Parameters.Add("NoCuserID", userID);
                objPrm[7] = cmdu.Parameters.Add("NoExpWbConItemID", ExpWbConItemID);

                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose();
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Container & Weight Slip Data Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearTextItem();
                TextSlipWbNo.Focus();
                Display();
                DisplaySummary();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
            //     }
            //   catch
            //   {
            //       Response.Redirect("~/ParameterError.aspx");
            //  } 
        }

        protected void GetItemWtWb(object sender, EventArgs e)
        {
          //  try
         //   {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string USER_DATA_ID = DropDownSlipID.Text;
                double ItemWeightWBEx = 0.0, ItemWeight = 0.0, TareWeight = 0.0;
                string makeSQL = " SELECT nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, nvl(TARE_WEIGHT,0) AS TARE_WEIGHT from MS_EXPORT_WBSLIP_CON where WB_SLIP_NO  = '" + USER_DATA_ID + "'";
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                for (int i = 0; i < RowCount; i++)
                {
                    ItemWeightWBEx = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString());
                    TareWeight = Convert.ToDouble(dt.Rows[i]["TARE_WEIGHT"].ToString());
                }

                string makeSrSQL = " SELECT nvl(sum(ITEM_WEIGHT),0) AS ITEM_WEIGHT from MS_EXPORT_WBSLIP_CON_ITEM where WB_SLIP_NO  = '" + USER_DATA_ID + "'";
                cmdl = new OracleCommand(makeSrSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                for (int i = 0; i < RowCount; i++)
                {
                    ItemWeight = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());

                }

                conn.Close();
                TextItemWtWb.Text = Convert.ToString(ItemWeightWBEx - (TareWeight + ItemWeight));
                TextItemWeightEx.Text = Convert.ToString(ItemWeightWBEx - (TareWeight + ItemWeight));

                if ((ItemWeightWBEx - (TareWeight + ItemWeight)) <= 0)
                {
                    BtnAddItem.Attributes.Add("aria-disabled", "false");
                    BtnAddItem.Attributes.Add("class", "btn btn-primary disabled");
                    DropDownCategoryID.Enabled = false;
                    TextItemWeightEx.Enabled = false;
                    DropDownPacking1.Enabled = false;
                
                }
                else
                {
                    BtnAddItem.Attributes.Add("aria-disabled", "true");
                    BtnAddItem.Attributes.Add("class", "btn btn-primary active");
                    DropDownCategoryID.Enabled = true;
                    TextItemWeightEx.Enabled = true;
                    DropDownPacking1.Enabled = true;
                }
                   TextItemWeightEx.Enabled = true;

                clearText();
        //    }
       //     catch
       //     {
        //        Response.Redirect("~/ParameterError.aspx");
        //    }
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
                    makeSQL = " SELECT PEWC.WB_SLIP_NO, PEWC.SEAL_NO, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.REF_NO, PEWC.BOOKING_NO, PEWC.IS_EDIT, PEWCI.EXP_WBCON_ITEM_ID, PI.ITEM_CODE  || ' : ' || PI.ITEM_NAME AS ITEM_NAME, WWI.ITEM_WS_DESCRIPTION, PEWC.ITEM_WEIGHT_WB, PEWC.TARE_WEIGHT, PEWCI.ITEM_WEIGHT, PEWCI.NUMBER_OF_PACK, PEWC.IS_ACTIVE, PEWCI.IS_INVENTORY_STATUS, PEWCI.IS_ACTIVE_PRICING, PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT, TO_CHAR(PEWC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE,  PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO  FROM MS_EXPORT_WBSLIP_CON PEWC LEFT JOIN MS_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO  LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN MS_WS_ITEM WWI ON WWI.ITEM_WS_ID = PEWCI.ITEM_WS_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE PEWC.IS_INVENTORY_STATUS = 'Transit'  ORDER BY PEWC.WB_SLIP_NO desc, PEWCI.ITEM_ID  ASC "; //WHERE to_char(PEWC.DISPATCH_DATE, 'mm/yyyy') = '" + MonthYear + "' OR IS_INVENTORY_STATUS = 'Transit'
                }
                else
                {
                    if (DropDownIsInven.Text == "0")
                    {
                        makeSQL = "  SELECT PEWC.WB_SLIP_NO, PEWC.SEAL_NO, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.REF_NO, PEWC.BOOKING_NO, PEWC.IS_EDIT, PEWCI.EXP_WBCON_ITEM_ID, PI.ITEM_CODE  || ' : ' || PI.ITEM_NAME AS ITEM_NAME, WWI.ITEM_WS_DESCRIPTION, PEWC.ITEM_WEIGHT_WB, PEWC.TARE_WEIGHT, PEWCI.ITEM_WEIGHT, PEWCI.NUMBER_OF_PACK, PEWC.IS_ACTIVE, PEWCI.IS_INVENTORY_STATUS, PEWCI.IS_ACTIVE_PRICING, PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT, TO_CHAR(PEWC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE,  PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO  FROM MS_EXPORT_WBSLIP_CON PEWC LEFT JOIN MS_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO  LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN MS_WS_ITEM WWI ON WWI.ITEM_WS_ID = PEWCI.ITEM_WS_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE PEWC.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or PEWC.CONTAINER_NO like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%'  or to_char(PEWC.DISPATCH_DATE, 'dd/mm/yyyy')  like '" + txtSearchEmp.Text + "%' or PEWC.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ORDER BY PEWC.WB_SLIP_NO desc, PEWCI.ITEM_ID asc ";
                    }
                    else
                    {
                        makeSQL = "  SELECT PEWC.WB_SLIP_NO, PEWC.SEAL_NO, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.REF_NO, PEWC.BOOKING_NO, PEWC.IS_EDIT, PEWCI.EXP_WBCON_ITEM_ID, PI.ITEM_CODE  || ' : ' || PI.ITEM_NAME AS ITEM_NAME, WWI.ITEM_WS_DESCRIPTION, PEWC.ITEM_WEIGHT_WB, PEWC.TARE_WEIGHT, PEWCI.ITEM_WEIGHT, PEWCI.NUMBER_OF_PACK, PEWC.IS_ACTIVE, PEWCI.IS_INVENTORY_STATUS, PEWCI.IS_ACTIVE_PRICING, PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT, TO_CHAR(PEWC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE,  PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO  FROM MS_EXPORT_WBSLIP_CON PEWC LEFT JOIN MS_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO  LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN MS_WS_ITEM WWI ON WWI.ITEM_WS_ID = PEWCI.ITEM_WS_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE PEWCI.IS_INVENTORY_STATUS = '" + DropDownIsInven.Text + "' AND (PEWC.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or PEWC.CONTAINER_NO like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or to_char(PEWC.DISPATCH_DATE, 'dd/mm/yyyy')  like '" + txtSearchEmp.Text + "%' or PEWC.IS_ACTIVE like '" + txtSearchEmp.Text + "%') ORDER BY PEWC.WB_SLIP_NO desc, PEWCI.ITEM_ID asc ";
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
                if (dt.Rows.Count > 0)
                {
                    GroupGridView(GridView1.Rows, 0, 14);
                }
                else
                {

                }
                conn.Close();
                // alert_box.Visible = false;
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

        protected void gridViewFileInformation_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow Row in GridView1.Rows)
            {
                string isCheck = (Row.FindControl("IsCmoCheckLink") as Label).Text;
                string isCheckPrint = (Row.FindControl("IsPrintedCheckLink") as Label).Text;
                string isCheckExInvoiceNo = (Row.FindControl("IsExInvoiceNo") as Label).Text;
                string isCheckExConEdit = (Row.FindControl("IsExConEdit") as Label).Text;
                string IsEditItemCheck = (Row.FindControl("IsEditItemCheck") as Label).Text;
                string IsEditItemPriceCheck = (Row.FindControl("IsEditItemPriceCheck") as Label).Text;
                if (isCheck == "Complete" || isCheckExInvoiceNo == "Yes")  // || isCheckPrint == "Printed"
                {
                    (Row.FindControl("linkSelectClick") as LinkButton).Visible = false;
                    (Row.FindControl("linkSelectItemClick") as LinkButton).Visible = false;
                }

                if (isCheckExConEdit == "Disable")
                {
                    (Row.FindControl("linkSelectClick") as LinkButton).Visible = false;
                }

                if (IsEditItemCheck == "Disable")
                {
                    (Row.FindControl("linkSelectItemClick") as LinkButton).Visible = false;
                    (Row.FindControl("IsInvenStatus") as Label).Visible = false;
                    (Row.FindControl("linkPrintClick") as LinkButton).Visible = false;
                }
                if (IsEditItemPriceCheck == "Enable")
                {
                    (Row.FindControl("linkSelectItemClick") as LinkButton).Visible = false;
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
                if (DropDownSearchSummary.Text == "")
                {
                    makeSQL = " SELECT WI.ITEM_CODE  || ' : ' || WI.ITEM_NAME AS ITEM_NAME, nvl(sum(PEWCI.ITEM_WEIGHT), 0) AS ITEM_WEIGHT, SUM(PEWCI.NUMBER_OF_PACK) AS NUMBER_OF_PACK FROM MS_EXPORT_WBSLIP_CON PEWC LEFT JOIN MS_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = PEWCI.ITEM_ID WHERE PEWCI.IS_INVENTORY_STATUS = 'Transit' AND WI.ITEM_NAME IS NOT NULL GROUP BY WI.ITEM_CODE  || ' : ' || WI.ITEM_NAME  ORDER BY WI.ITEM_CODE  || ' : ' || WI.ITEM_NAME ";
                }
                else
                {
                    makeSQL = " SELECT WI.ITEM_CODE  || ' : ' || WI.ITEM_NAME AS ITEM_NAME, nvl(sum(PEWCI.ITEM_WEIGHT), 0) AS ITEM_WEIGHT, SUM(PEWCI.NUMBER_OF_PACK) AS NUMBER_OF_PACK FROM MS_EXPORT_WBSLIP_CON PEWC LEFT JOIN MS_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = PEWCI.ITEM_ID WHERE PEWCI.IS_INVENTORY_STATUS = '" + DropDownSearchSummary.Text + "' AND WI.ITEM_NAME IS NOT NULL GROUP BY WI.ITEM_CODE  || ' : ' || WI.ITEM_NAME  ORDER BY WI.ITEM_CODE  || ' : ' || WI.ITEM_NAME ";

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
                    GridView2.HeaderRow.Cells[1].HorizontalAlign = HorizontalAlign.Right;
                    //Calculate Sum and display in Footer Row
                    GridView2.FooterRow.Cells[0].Font.Bold = true;
                    GridView2.FooterRow.Cells[0].Text = "Grand Total";
                    GridView2.FooterRow.Cells[0].HorizontalAlign = HorizontalAlign.Right;

                    decimal total_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_WEIGHT"));
                    GridView2.FooterRow.Cells[1].Font.Bold = true;
                    GridView2.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[1].Text = total_wt.ToString("N0");
                    decimal total_bales = dt.AsEnumerable().Sum(row => row.Field<decimal>("NUMBER_OF_PACK"));
                    GridView2.FooterRow.Cells[2].Font.Bold = true;
                    GridView2.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[2].Text = total_bales.ToString("N0");

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


     

        protected void BtnDeleteItem_Click(object sender, EventArgs e)
        {
            //   try
            //   {
            if (IS_DELETE_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string SlipNo = DropDownSlipID.Text;
                int ExpWbConItemID = Convert.ToInt32(TextExpWbConItemID.Text);
                string delete_user = " delete from MS_EXPORT_WBSLIP_CON_ITEM where EXP_WBCON_ITEM_ID  = '" + ExpWbConItemID + "'";
                cmdi = new OracleCommand(delete_user, conn);
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();


                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select WB_SLIP_NO from MS_EXPORT_WBSLIP_CON_ITEM where WB_SLIP_NO = '" + SlipNo + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (!dr.HasRows)
                {
                    string update_user = "update  MS_EXPORT_WBSLIP_CON  set IS_EDIT =:TextIsEdit where WB_SLIP_NO =:NoSlipNo ";
                    cmdi = new OracleCommand(update_user, conn);
                    OracleParameter[] objPr = new OracleParameter[2];
                    objPr[0] = cmdi.Parameters.Add("TextIsEdit", "Enable");
                    objPr[1] = cmdi.Parameters.Add("NoSlipNo", SlipNo);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                }

                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Container & Weight Slip Items Data Delete Successfully"));
                alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                clearTextItem();
                Display();
                DisplaySummary();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
            //   }
            //    catch
            //    {
            //        Response.Redirect("~/ParameterError.aspx");
            //    }

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
                    int ExWbConID = Convert.ToInt32(TextExWbConID.Text);
                    string SlipNo = TextSlipWbNo.Text;
                    string delete_user = " delete from MS_EXPORT_WBSLIP_CON where EXP_WBCON_ID  = '" + ExWbConID + "'";
                    cmdi = new OracleCommand(delete_user, conn);
                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    string delete_item = " delete from MS_EXPORT_WBSLIP_CON_ITEM where WB_SLIP_NO  = '" + SlipNo + "'";
                    cmdu = new OracleCommand(delete_item, conn);
                    cmdu.ExecuteNonQuery();
                    cmdu.Parameters.Clear();
                    cmdu.Dispose();

                    conn.Close();

                    DataTable dtWbSlipID = new DataTable();
                    DataSet dse = new DataSet();
                    string makeSrSQL = " SELECT WB_SLIP_NO FROM MS_EXPORT_WBSLIP_CON WHERE IS_ACTIVE = 'Enable' ORDER BY WB_SLIP_NO DESC";
                    dse = ExecuteBySqlString(makeSrSQL);
                    dtWbSlipID = (DataTable)dse.Tables[0];
                    DropDownSlipID.DataSource = dtWbSlipID;
                    DropDownSlipID.DataValueField = "WB_SLIP_NO";
                    DropDownSlipID.DataTextField = "WB_SLIP_NO";
                    DropDownSlipID.DataBind();
                    DropDownSlipID.Items.Insert(0, new ListItem("Select  Weight Slip", "0"));

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Container & Weight Slip Data Delete Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                    clearText();
                    Display();
                    DisplaySummary();
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
            TextSlipWbNo.Text = "";
            TextSealNo.Text = "";
            TextContainerNo.Text = "";
            CheckSlipNo.Text = "";
            TextItemWeightWb.Text = "";
            DropDownPacking1.Text = "1";
            EntryDate.Text = "";
            TextTareWeight.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        {
            TextSlipWbNo.Text = "";
            TextSealNo.Text = "";
            TextContainerNo.Text = "";
            CheckSlipNo.Text = "";
            TextItemWeightWb.Text = "";
            DropDownPacking1.Text = "1";
            TextTareWeight.Text = "";
            EntryDate.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearTextItem()
        {
            DropDownSlipID.Text = "0";
            //  DropDownItemID.Text = "0";
            DropDownCategoryID.Text = "0";
            TextItemWeightEx.Text = "";
            TextItemWtWb.Text = "";
            DropDownPacking1.Text = "1";
            BtnDeleteItem.Attributes.Add("aria-disabled", "false");
            BtnDeleteItem.Attributes.Add("class", "btn btn-danger disabled");
            BtnUpdateItem.Attributes.Add("aria-disabled", "false");
            BtnUpdateItem.Attributes.Add("class", "btn btn-success disabled");

        }


        public void TextSlipWbNo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string SlipNo = TextSlipWbNo.Text;
                string MatchEmpIDPattern = "^([0-9]{6})$";
                if (SlipNo != null)
                {

                    if (Regex.IsMatch(SlipNo, MatchEmpIDPattern))
                    {
                        alert_box.Visible = false;

                        OracleConnection conn = new OracleConnection(strConnString);
                        conn.Open();
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = "select WB_SLIP_NO from MS_EXPORT_WBSLIP_CON where WB_SLIP_NO = '" + SlipNo + "'";
                        cmd.CommandType = CommandType.Text;

                        OracleDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            CheckSlipNo.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Weight Slip Number is not available</label>";
                            CheckSlipNo.ForeColor = System.Drawing.Color.Red;
                            TextSlipWbNo.Focus();
                            BtnAdd.Attributes.Add("aria-disabled", "false");
                            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");


                        }
                        else
                        {
                            CheckSlipNo.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Weight Slip Number is available</label>";
                            CheckSlipNo.ForeColor = System.Drawing.Color.Green;
                            TextSealNo.Focus();
                            BtnAdd.Attributes.Add("aria-disabled", "true");
                            BtnAdd.Attributes.Add("class", "btn btn-primary active");

                        }
                    }
                    else
                    {
                        CheckSlipNo.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter Weight Slip Number is 6 digit only</label>";
                        CheckSlipNo.ForeColor = System.Drawing.Color.Red;
                        TextSlipWbNo.Focus();
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