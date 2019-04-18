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

namespace NRCAPPS.MF
{
    public partial class MfExpContainer : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl, cmdsp;
        OracleDataAdapter oradata; 
        DataTable dt;
        int RowCount, RowCount2;
        double ItemWtTotal=0.0, ItemWtWbTotal=0.0, NoOfPacks = 0.0, WtPerPacks = 0.0, TotalPacksWt = 0.0, TotalWpWt=0.0, TotalItemWt=0.0; 

        string SealNo = "", RelOrderNo="";
        string IS_PAGE_ACTIVE = "", IS_ADD_ACTIVE = "",IS_EDIT_ACTIVE = "",IS_DELETE_ACTIVE = "",IS_VIEW_ACTIVE = "", IS_REPORT_ACTIVE = "", IS_PRINT_ACTIVE ="";

        public bool IsLoad { get; set; }  public bool IsLoad2 { get; set; } public bool IsLoad3 { get; set; } public bool IsLoad4 { get; set; } 
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
                        string makeSupplierSQL = " SELECT PARTY_ID, PARTY_NAME || ' - ' || PARTY_ID || ' - ' || PARTY_VAT_NO AS PARTY_NAME  FROM MF_PARTY WHERE IS_ACTIVE = 'Enable' AND IS_PRODUCTION_ACTIVE = 'Enable' ORDER BY PARTY_NAME ASC";
                        ds = ExecuteBySqlString(makeSupplierSQL);
                        dtSupplierID = (DataTable)ds.Tables[0];
                        DropDownSupplierID.DataSource = dtSupplierID;
                        DropDownSupplierID.DataValueField = "PARTY_ID";
                        DropDownSupplierID.DataTextField = "PARTY_NAME";
                        DropDownSupplierID.DataBind();
                        DropDownSupplierID.Items.Insert(0, new ListItem("Select  Customer", "0"));
                                   
                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeDropDownItemSQL = " SELECT ITEM_ID, ITEM_CODE || ' : ' || ITEM_NAME AS ITEM_NAME FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' AND IS_SALES_ACTIVE =  'Enable' ORDER BY ITEM_ID ASC";
                        dsi = ExecuteBySqlString(makeDropDownItemSQL);
                        dtItemID = (DataTable)dsi.Tables[0];
                        DropDownItemID.DataSource = dtItemID;
                        DropDownItemID.DataValueField = "ITEM_ID";
                        DropDownItemID.DataTextField = "ITEM_NAME";
                        DropDownItemID.DataBind();
                        DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));
                         
                     /*   DropDownItemID1.DataSource = dtItemID;
                        DropDownItemID1.DataValueField = "ITEM_ID";
                        DropDownItemID1.DataTextField = "ITEM_NAME";
                        DropDownItemID1.DataBind();
                        DropDownItemID1.Items.Insert(0, new ListItem("All Item", "0")); */

                     

                        DataTable dtContainerID = new DataTable();
                        DataSet dsc = new DataSet();
                        string makeDropDownContainerSQL = " SELECT CONTAINER_SIZE_ID, CONTAINER_SIZE || '- (' || CONTAINER_SIZE_INWORDS || ')' AS CONTAINER_SIZE FROM NRC_CONTAINER_SIZE WHERE IS_ACTIVE = 'Enable' ORDER BY CONTAINER_SIZE_ID ASC";
                        dsc = ExecuteBySqlString(makeDropDownContainerSQL);
                        dtContainerID = (DataTable)dsc.Tables[0];
                        DropDownContainerSizeID.DataSource = dtContainerID;
                        DropDownContainerSizeID.DataValueField = "CONTAINER_SIZE_ID";
                        DropDownContainerSizeID.DataTextField = "CONTAINER_SIZE";
                        DropDownContainerSizeID.DataBind();
                        DropDownContainerSizeID.Items.FindByValue("2").Selected = true;

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
                        DropDownPacking1.Items.FindByValue("3").Selected = true;
                         

                        TextSlipNo.Focus();
                        TextItemWtWb.Attributes.Add("readonly", "readonly");
                        TextItemWeightEx.Attributes.Add("readonly", "readonly");

                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled");

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
                    int SupplierID = Convert.ToInt32(DropDownSupplierID.Text);
                    string SlipNo = TextSlipNo.Text; 
                    int ContainerSizeID = Convert.ToInt32(DropDownContainerSizeID.Text);

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-'); 
                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                   
                    string get_user_id = "select MF_EXPORT_CONTAINERID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_user_id, conn);
                    int newID = Int32.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_purchase = "insert into  MF_EXPORT_WBSLIP_CON (EXP_WBCON_ID, WB_SLIP_NO, CONTAINER_NO, CONTAINER_SIZE_ID, SEAL_NO, REF_ORDER_NO, CONTRACT_NO, BUNDLE, PARTY_ID, FIRST_WT, SECOND_WT, ITEM_WEIGHT_WB, ITEM_ID, ITEM_WEIGHT, PACKING_ID, PACKING_WEIGHT, DISPATCH_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID, IS_INVENTORY_STATUS ) values  ( :NoPurchaseID, :NoSlipID, :TextContainerNo, :NoContainerSizeID, :TextSealNo, :TextRelOrderNo, :TextContractNo, :TextBundle, :NoPartyID,  :TextItemFirstWtWb, :TextItemSecondWtWb, :TextItemWtWb, :NoItemID, :NoItemWeight, :NoPacking1, :TextWtTotalPacking1, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 3, 'Transit'  )";
                    cmdi = new OracleCommand(insert_purchase, conn);

                    OracleParameter[] objPrm = new OracleParameter[20];
                    objPrm[0] = cmdi.Parameters.Add("NoPurchaseID", newID);
                    objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNo);
                    objPrm[2] = cmdi.Parameters.Add("TextContainerNo", TextContainerNo.Text);
                    objPrm[3] = cmdi.Parameters.Add("NoContainerSizeID", ContainerSizeID);
                    objPrm[4] = cmdi.Parameters.Add("TextSealNo", TextSealNo.Text);
                    objPrm[5] = cmdi.Parameters.Add("TextRelOrderNo", TextRelOrderNo.Text);
                    objPrm[6] = cmdi.Parameters.Add("TextContractNo", TextContractNo.Text);
                    objPrm[7] = cmdi.Parameters.Add("TextBundle", Convert.ToInt32(TextBundle.Text));
                    objPrm[8] = cmdi.Parameters.Add("NoPartyID", SupplierID);
                    objPrm[9] = cmdi.Parameters.Add("TextItemFirstWtWb", Convert.ToDouble(TextItemFirstWtWb.Text));
                    objPrm[10] = cmdi.Parameters.Add("TextItemSecondWtWb", Convert.ToDouble(TextItemSecondWtWb.Text));
                    objPrm[11] = cmdi.Parameters.Add("TextItemWtWb", Convert.ToDouble(TextItemWtWb.Text));
                    objPrm[12] = cmdi.Parameters.Add("NoItemID", Convert.ToInt32(DropDownItemID.Text)); 
                    objPrm[13] = cmdi.Parameters.Add("NoItemWeight", Convert.ToDouble(TextItemWeightEx.Text));
                    objPrm[14] = cmdi.Parameters.Add("NoPacking1", Convert.ToInt32(DropDownPacking1.Text));
                    objPrm[15] = cmdi.Parameters.Add("TextWtTotalPacking1", Convert.ToDouble(TextWtTotalPacking1.Text));
                    objPrm[16] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[17] = cmdi.Parameters.Add("c_date", c_date);
                    objPrm[18] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[19] = cmdi.Parameters.Add("TextIsActive", ISActive);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                      
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Weight Slip Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                    clearText();
                    TextSlipNo.Focus();
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

       
        protected void btnPrint_Click(object sender, EventArgs e)
        {
          if (IS_PRINT_ACTIVE == "Enable")
             {
            OracleConnection conn = new OracleConnection(strConnString); 
            conn.Open();
            string HtmlString = "";
            string makeSQL = " SELECT PEWC.EXP_WBCON_ID, PEWC.WB_SLIP_NO, PEWC.CONTAINER_NO, PP.PARTY_NAME, PEWC.SEAL_NO, PEWC.REF_ORDER_NO, PEWC.CONTRACT_NO, PEWC.BUNDLE, FIRST_WT, SECOND_WT, nvl(PEWC.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, MI.ITEM_NAME,  PEWC.ITEM_WEIGHT, NPL.PACKING_NAME,  PEWC.PACKING_WEIGHT, NCS.CONTAINER_SIZE FROM MF_EXPORT_WBSLIP_CON PEWC  LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = PEWC.PACKING_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = PEWC.PARTY_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE PEWC.WB_SLIP_NO = '" + TextSlipNo.Text + "' ORDER BY MI.ITEM_ID ";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;

            for (int i = 0; i < 1; i++)
            { 
                string ContainerNo = dt.Rows[i]["CONTAINER_NO"].ToString();
                string PartyName = dt.Rows[i]["PARTY_NAME"].ToString();
                       SealNo = dt.Rows[i]["SEAL_NO"].ToString();
                       RelOrderNo = dt.Rows[i]["REF_ORDER_NO"].ToString(); //border:solid black 1px;
                    HtmlString += "<div style='float:left;width:720px;height:258px;margin:342px 0 0 35px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 12px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";


                    HtmlString += "<table cellpadding='2px' cellspacing='0' style='font-size: 13px;' width='100%' align='center'>";
                    HtmlString += "<tr valign='top'> ";
                    HtmlString += "<th colspan=4 style='font-size: 14px;text-decoration: underline;'>Export (Sales)</th> ";
                    HtmlString += "</tr> "; 
                     
                    HtmlString += "<tr> "; 
                    HtmlString += "<td>Material:</td> ";
                    HtmlString += "<td>" + dt.Rows[i]["ITEM_NAME"].ToString() + "</td> "; 
                    HtmlString += "<td></td> ";
                    HtmlString += "<td></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>Container No.:</td> ";
                    HtmlString += "<td>" + ContainerNo + " Size: " + dt.Rows[i]["CONTAINER_SIZE"].ToString() + "'</td> ";
                    HtmlString += "<td></td> ";
                    HtmlString += "<td></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>Customer Name:</td> ";
                    HtmlString += "<td>" + PartyName +  "</td> ";
                    HtmlString += "<td></td> ";
                    HtmlString += "<td></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>Seal No.:</td> ";
                    HtmlString += "<td>" + SealNo + "</td> ";
                    HtmlString += "<td>1st Weight (KG):</td> ";
                    HtmlString += "<td style='text-align:right'>" + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["FIRST_WT"].ToString())) + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>Refrence:</td> ";
                    HtmlString += "<td>" + dt.Rows[i]["REF_ORDER_NO"].ToString() + "</td> ";
                    HtmlString += "<td>2nd Weight (KG):</td> ";
                    HtmlString += "<td style='text-align:right'>" + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["SECOND_WT"].ToString())) + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>Contract No.:</td> ";
                    HtmlString += "<td>" + dt.Rows[i]["CONTRACT_NO"].ToString() + "</td> ";
                    HtmlString += "<td style='border-bottom:solid black 1px;'>Less " + dt.Rows[i]["PACKING_NAME"].ToString() + " (KG):</td> ";
                    HtmlString += "<td style='border-bottom:solid black 1px;text-align:right'>" + dt.Rows[i]["PACKING_WEIGHT"].ToString() + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>Bundle:</td> ";
                    HtmlString += "<td>" + dt.Rows[i]["BUNDLE"].ToString() + "</td> ";
                    HtmlString += "<td style='font-weight:700;'>Net Weight (KG):</td> ";
                    HtmlString += "<td style='font-weight:700;text-align:right'> " + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString())) + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr valign='top'> ";
                    HtmlString += "<th colspan=4 style='font-weight: 700;'>For Export Only</th> ";
                    HtmlString += "</tr> ";
                    HtmlString += "</table> ";
                     

                HtmlString += "</div>"; 
             } 

            // weigh-bridge & container update for print
            int userID = Convert.ToInt32(Session["USER_ID"]);
            string SlipNo = TextSlipNo.Text; 
            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
            string update_user = "update  MF_EXPORT_WBSLIP_CON  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where WB_SLIP_NO = :NoSlipNo ";
            cmdi = new OracleCommand(update_user, conn);

            OracleParameter[] objPrm = new OracleParameter[4];
            objPrm[0] = cmdi.Parameters.Add("TextIsPrint", "Printed"); 
            objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
            objPrm[2] = cmdi.Parameters.Add("NoCuserID", userID); 
            objPrm[3] = cmdi.Parameters.Add("NoSlipNo", SlipNo);

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
         
        protected void LinkSelectClick(object sender, EventArgs e)
        {
         try{
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
            string USER_DATA_ID = Session["user_data_id"].ToString();
                 
            string makeSQL = " select EXP_WBCON_ID, WB_SLIP_NO, CONTAINER_NO, CONTAINER_SIZE_ID, PARTY_ID,  SEAL_NO, REF_ORDER_NO,  CONTRACT_NO, BUNDLE, FIRST_WT, SECOND_WT, nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, ITEM_ID, ITEM_WEIGHT, PACKING_ID, PACKING_WEIGHT, TO_CHAR(DISPATCH_DATE,'dd/mm/yyyy') AS DISPATCH_DATE, IS_ACTIVE from MF_EXPORT_WBSLIP_CON where WB_SLIP_NO  = '" + USER_DATA_ID + "'";
            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                TextExWbConID.Text = dt.Rows[i]["EXP_WBCON_ID"].ToString();   
                TextSlipNo.Text = dt.Rows[i]["WB_SLIP_NO"].ToString(); 
                DropDownSupplierID.Text = dt.Rows[i]["PARTY_ID"].ToString();
                TextContainerNo.Text = dt.Rows[i]["CONTAINER_NO"].ToString();
                DropDownContainerSizeID.Text = dt.Rows[i]["CONTAINER_SIZE_ID"].ToString();
                TextSealNo.Text = dt.Rows[i]["SEAL_NO"].ToString();
                TextRelOrderNo.Text = dt.Rows[i]["REF_ORDER_NO"].ToString();
                TextContractNo.Text = dt.Rows[i]["CONTRACT_NO"].ToString();
                TextBundle.Text = dt.Rows[i]["BUNDLE"].ToString();
                DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();
                TextItemFirstWtWb.Text  = dt.Rows[i]["FIRST_WT"].ToString();
                TextItemSecondWtWb.Text  = dt.Rows[i]["SECOND_WT"].ToString();                        
                TextItemWtWb.Text  = dt.Rows[i]["ITEM_WEIGHT_WB"].ToString();
                TextItemWeightEx.Text = dt.Rows[i]["ITEM_WEIGHT"].ToString();
                DropDownPacking1.Text = dt.Rows[i]["PACKING_ID"].ToString();
                TextWtTotalPacking1.Text = dt.Rows[i]["PACKING_WEIGHT"].ToString();                  
                EntryDate.Text = dt.Rows[i]["DISPATCH_DATE"].ToString();
               
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
 

             }

            conn.Close();
            Display();
            CheckSlipNo.Text = "";
            alert_box.Visible = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

            BtnUpdate.Attributes.Add("aria-disabled", "true");
            BtnUpdate.Attributes.Add("class", "btn btn-success active"); 
            BtnDelete.Attributes.Add("aria-disabled", "true");
            BtnDelete.Attributes.Add("class", "btn btn-danger active");

            TextSlipNo.Attributes.Add("readonly", "readonly");

                }
             catch
              {
               Response.Redirect("~/ParameterError.aspx");
            } 
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
                int SupplierID = Convert.ToInt32(DropDownSupplierID.Text);
                string MakeEntryDate = EntryDate.Text;
                string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                 

                string update_user = "update  MF_EXPORT_WBSLIP_CON  set WB_SLIP_NO =:NoSlipNo, CONTAINER_NO =:NoContainerNo, CONTAINER_SIZE_ID =:NoContainerSizeID, SEAL_NO =:TextSealNo, REF_ORDER_NO =:TextRelOrderNo, CONTRACT_NO =:TextContractNo, BUNDLE =:TextBundle, PARTY_ID =:NoPartyID, FIRST_WT =:TextItemFirstWtWb, SECOND_WT =:TextItemSecondWtWb, ITEM_WEIGHT_WB =:TextItemWtWb, ITEM_ID =:NoItemID, ITEM_WEIGHT =:NoItemWeight, PACKING_ID =:NoPacking1, PACKING_WEIGHT =:TextWtTotalPacking1, DISPATCH_DATE = TO_DATE(:DispatchDate, 'DD/MM/YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where EXP_WBCON_ID = :NoExWbConID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[20];
                objPrm[0] = cmdi.Parameters.Add("NoSlipNo", TextSlipNo.Text);
                objPrm[1] = cmdi.Parameters.Add("NoContainerNo", TextContainerNo.Text);
                objPrm[2] = cmdi.Parameters.Add("NoContainerSizeID", ContainerSizeID);
                objPrm[3] = cmdi.Parameters.Add("TextSealNo", TextSealNo.Text);
                objPrm[4] = cmdi.Parameters.Add("TextRelOrderNo", TextRelOrderNo.Text);
                objPrm[5] = cmdi.Parameters.Add("TextContractNo", TextContractNo.Text);
                objPrm[6] = cmdi.Parameters.Add("TextBundle", Convert.ToInt32(TextBundle.Text));
                objPrm[7] = cmdi.Parameters.Add("NoPartyID", SupplierID);
                objPrm[8] = cmdi.Parameters.Add("TextItemFirstWtWb", Convert.ToDouble(TextItemFirstWtWb.Text));
                objPrm[9] = cmdi.Parameters.Add("TextItemSecondWtWb", Convert.ToDouble(TextItemSecondWtWb.Text));
                objPrm[10] = cmdi.Parameters.Add("TextItemWtWb", Convert.ToDouble(TextItemWtWb.Text));
                objPrm[11] = cmdi.Parameters.Add("NoItemID", Convert.ToInt32(DropDownItemID.Text));
                objPrm[12] = cmdi.Parameters.Add("NoItemWeight", Convert.ToDouble(TextItemWeightEx.Text));
                objPrm[13] = cmdi.Parameters.Add("NoPacking1", Convert.ToInt32(DropDownPacking1.Text));
                objPrm[14] = cmdi.Parameters.Add("TextWtTotalPacking1", Convert.ToDouble(TextWtTotalPacking1.Text));
                objPrm[15] = cmdi.Parameters.Add("DispatchDate", EntryDateNew);
                objPrm[16] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[17] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[18] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[19] = cmdi.Parameters.Add("NoExWbConID", ExWbConID);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Container & Weight Slip Data Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearText();
                TextSlipNo.Focus();
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
                    string delete_user = " delete from MF_EXPORT_WBSLIP_CON where EXP_WBCON_ID  = '" + ExWbConID + "'";
                    cmdi = new OracleCommand(delete_user, conn);
                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                     
                    conn.Close();
                     
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



        public void Display()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open(); 
                string makeSQL = "";
                string MonthYear = System.DateTime.Now.ToString("yyyy/MM");
                DateTime ThreeMonthBeforeTemp = DateTime.Now.AddMonths(-1);
                string ThreeMonthBefore = ThreeMonthBeforeTemp.ToString("yyyy/MM");

                if (txtSearchEmp.Text == "")
                {
                    makeSQL = " SELECT PEWC.*,  NCS.CONTAINER_SIZE,  PP.PARTY_NAME,  MI.ITEM_NAME,  NPL.PACKING_NAME, PEWC.PRINT_DATE FROM MF_EXPORT_WBSLIP_CON PEWC LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = PEWC.PACKING_ID LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = PEWC.PARTY_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID   WHERE to_char(PEWC.DISPATCH_DATE, 'yyyy/mm') between '" + ThreeMonthBefore + "' AND '" + MonthYear + "'  ORDER BY PEWC.WB_SLIP_NO desc, PEWC.ITEM_ID ASC  "; // OR IS_INVENTORY_STATUS = 'Transit' / WHERE to_char(PEWC.DISPATCH_DATE, 'mm/yyyy') = '" + MonthYear + "'
                }
                else
                {
                    if (DropDownIsInven.Text == "0")
                    {
                        makeSQL = "  SELECT PEWC.WB_SLIP_NO, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.SEAL_NO, PEWC.REF_ORDER_NO, PP.PARTY_NAME,  MI.ITEM_NAME, PEWC.ITEM_WEIGHT_WB,  PEWC.ITEM_WEIGHT,  NPL.PACKING_NAME,  PEWC.PACKING_WEIGHT, PEWC.CONTRACT_NO, PEWC.IS_ACTIVE ,  PEWC.IS_INVENTORY_STATUS,  PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT,  PEWC.PRINT_DATE,  PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO, PEWC.IS_ACTIVE_PRICING FROM MF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_EXPORT_PACKING_HISTORY PEPH ON PEPH.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = PEPH.PACKING_ID LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = PEWC.PARTY_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE PEWC.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or PEWC.CONTAINER_NO like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PEWC.SEAL_NO like '" + txtSearchEmp.Text + "%' or MI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PEWC.REL_ORDER_NO like '" + txtSearchEmp.Text + "%' or to_char(PEWC.DISPATCH_DATE, 'dd/mm/yyyy')  like '" + txtSearchEmp.Text + "%' or PEWC.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ORDER BY PEWC.WB_SLIP_NO desc, PEWC.ITEM_ID asc ";   
                    }
                    else
                    {
                        makeSQL = "  SELECT PEWC.WB_SLIP_NO, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.SEAL_NO, PEWC.REF_ORDER_NO, PP.PARTY_NAME,  MI.ITEM_NAME, PEWC.ITEM_WEIGHT_WB,  PEWC.ITEM_WEIGHT,  NPL.PACKING_NAME,  PEWC.PACKING_WEIGHT, PEWC.CONTRACT_NO, PEWC.IS_ACTIVE ,  PEWC.IS_INVENTORY_STATUS,  PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT,  PEWC.PRINT_DATE,  PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO, PEWC.IS_ACTIVE_PRICING FROM MF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_EXPORT_PACKING_HISTORY PEPH ON PEPH.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = PEPH.PACKING_ID LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = PEWC.PARTY_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE IS_INVENTORY_STATUS = '" + DropDownIsInven.Text + "' AND (PEWC.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or PEWC.CONTAINER_NO like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PEWC.SEAL_NO like '" + txtSearchEmp.Text + "%' or MI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PEWC.REL_ORDER_NO like '" + txtSearchEmp.Text + "%' or to_char(PEWC.DISPATCH_DATE, 'dd/mm/yyyy')  like '" + txtSearchEmp.Text + "%' or PEWC.IS_ACTIVE like '" + txtSearchEmp.Text + "%') ORDER BY PEWC.WB_SLIP_NO desc, PEWC.ITEM_ID asc ";
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
                // alert_box.Visible = false;
            }
        }

         

        protected void gridViewFileInformation_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow Row in GridView1.Rows)
            {
                string isCheck = (Row.FindControl("IsCmoCheckLink") as Label).Text;
                string isCheckPrint = (Row.FindControl("IsPrintedCheckLink") as Label).Text;
                string isCheckExInvoiceNo = (Row.FindControl("IsExInvoiceNo") as Label).Text;
                string isEditItemPriceCheck = (Row.FindControl("IsEditItemPriceCheck") as Label).Text;

                if (isCheck == "Complete" || isCheckExInvoiceNo == "Yes")  // || isCheckPrint == "Printed"
                {
                    (Row.FindControl("linkSelectClick") as LinkButton).Visible = false;  
                }
                if (isEditItemPriceCheck == "Enable")   
                {
                    (Row.FindControl("linkSelectClick") as LinkButton).Visible = false;
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
                    makeSQL = " SELECT  MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME, nvl(sum(PEWC.ITEM_WEIGHT), 0) AS ITEM_WEIGHT, sum(PEWC.BUNDLE) AS BUNDLE FROM MF_EXPORT_WBSLIP_CON PEWC LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = PEWC.ITEM_ID WHERE IS_INVENTORY_STATUS = 'Transit' AND MI.ITEM_NAME IS NOT NULL GROUP BY MI.ITEM_CODE || ' : ' || MI.ITEM_NAME  ";
                }
                else
                {
                    makeSQL = " SELECT MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME, nvl(sum(PEWC.ITEM_WEIGHT), 0) AS ITEM_WEIGHT, sum(PEWC.BUNDLE) AS BUNDLE FROM MF_EXPORT_WBSLIP_CON PEWC LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = PEWC.ITEM_ID WHERE IS_INVENTORY_STATUS = '" + DropDownSearchSummary.Text + "' AND MI.ITEM_NAME IS NOT NULL GROUP BY MI.ITEM_CODE || ' : ' || MI.ITEM_NAME   ";
                          
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

                    decimal total_bundle = dt.AsEnumerable().Sum(row => row.Field<decimal>("BUNDLE"));
                    GridView2.FooterRow.Cells[2].Font.Bold = true;
                    GridView2.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[2].Text = total_bundle.ToString("N0");

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


    
        public void clearTextField(object sender, EventArgs e)
        {
            TextSlipNo.Text = "";
            TextBundle.Text = "";
            TextContainerNo.Text = "";
            TextSealNo.Text = "";
            DropDownSupplierID.Text = "0";
            TextRelOrderNo.Text = "";
            CheckSlipNo.Text = "";
            DropDownSupplierID.Text = "0";
            DropDownItemID.Text = "0";
            DropDownPacking1.Text = "0";
            TextWtTotalPacking1.Text = "";
            TextContractNo.Text = "";
            TextItemFirstWtWb.Text = "";
            TextItemSecondWtWb.Text = "";
            TextItemWtWb.Text = "";
            TextItemWeightEx.Text = "";
            EntryDate.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        {
            TextSlipNo.Text = "";
            TextBundle.Text = "";
            TextContainerNo.Text = "";
            TextSealNo.Text = "";
            DropDownSupplierID.Text = "0";
            TextRelOrderNo.Text = ""; 
            CheckSlipNo.Text = ""; 
            DropDownSupplierID.Text = "0"; 
            DropDownItemID.Text = "0"; 
            DropDownPacking1.Text = "0";  
            TextWtTotalPacking1.Text = "";
            TextContractNo.Text = "";
            TextItemFirstWtWb.Text = "";
            TextItemSecondWtWb.Text = "";
            TextItemWtWb.Text = ""; 
            TextItemWeightEx.Text = "";
            // EntryDate.Text = "";
            CheckItemWeight.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

     public void GetFirstWbData(object sender, EventArgs e)
        {
            if (TextItemFirstWtWb.Text != "")
            {
                if (TextItemFirstWtWb.Text != "" && TextItemSecondWtWb.Text != "" && TextWtTotalPacking1.Text != "")
                {
                    double ItemFirstWtWb = Convert.ToDouble(TextItemFirstWtWb.Text);
                    double ItemSecondWtWb = Convert.ToDouble(TextItemSecondWtWb.Text);
                    double WtTotalPacking1 = Convert.ToDouble(TextWtTotalPacking1.Text);

                    if (ItemFirstWtWb < ItemSecondWtWb)
                    {
                        TextItemWtWb.Text = Convert.ToString(ItemSecondWtWb - ItemFirstWtWb);
                        TextItemWeightEx.Text = Convert.ToString(ItemSecondWtWb - (ItemFirstWtWb + WtTotalPacking1));
                        TextItemSecondWtWb.Focus();
                    }
                    else
                    {
                        TextItemFirstWtWb.Text = "";
                        TextItemFirstWtWb.Focus();
                    }
                }
               TextItemSecondWtWb.Focus();
            }
            else {
                TextItemFirstWtWb.Focus();
            }
        }

        public void GetSecondWbData(object sender, EventArgs e)
        {
            if (TextItemFirstWtWb.Text != "")
            {
                if (TextItemSecondWtWb.Text != "")
                {
                    if (TextItemFirstWtWb.Text != "" && TextItemSecondWtWb.Text != "" && TextWtTotalPacking1.Text != "")
                    {
                        double ItemFirstWtWb = Convert.ToDouble(TextItemFirstWtWb.Text);
                        double ItemSecondWtWb = Convert.ToDouble(TextItemSecondWtWb.Text);
                        double WtTotalPacking1 = Convert.ToDouble(TextWtTotalPacking1.Text);

                        if (ItemFirstWtWb < ItemSecondWtWb)
                        {
                            TextItemWtWb.Text = Convert.ToString(ItemSecondWtWb - ItemFirstWtWb);
                            TextItemWeightEx.Text = Convert.ToString(ItemSecondWtWb - (ItemFirstWtWb + WtTotalPacking1));
                            TextWtTotalPacking1.Focus();
                        }
                        else
                        {
                            TextItemSecondWtWb.Text = "";
                            TextItemSecondWtWb.Focus(); 
                        }
                    }
                    TextWtTotalPacking1.Focus();
                }
                else
                {
                   TextItemSecondWtWb.Focus();
                }

            }
            else {
                TextItemFirstWtWb.Focus();
            }
        }

        public void GetItemWbNetData(object sender, EventArgs e)
        {

            if (TextItemFirstWtWb.Text != "" && TextItemSecondWtWb.Text != "" && TextWtTotalPacking1.Text != "")
            {
                double ItemFirstWtWb = Convert.ToDouble(TextItemFirstWtWb.Text);
                double ItemSecondWtWb = Convert.ToDouble(TextItemSecondWtWb.Text);
                double WtTotalPacking1 = Convert.ToDouble(TextWtTotalPacking1.Text);

                if (ItemFirstWtWb < ItemSecondWtWb && WtTotalPacking1 < ItemFirstWtWb)
                {
                    TextItemWtWb.Text = Convert.ToString(ItemSecondWtWb - ItemFirstWtWb);
                    TextItemWeightEx.Text = Convert.ToString(ItemSecondWtWb - (ItemFirstWtWb + WtTotalPacking1));
                    DropDownItemID.Focus();
                }
                else
                {
                    TextWtTotalPacking1.Text = "";
                    TextWtTotalPacking1.Focus();
                }
            }
        }

        public void GetItemNetData(object sender, EventArgs e)
        {
            alert_box.Visible = false; 
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            int ItemID = Convert.ToInt32(DropDownItemID.Text);
            if (TextItemFirstWtWb.Text != "" && TextItemSecondWtWb.Text != "" && TextWtTotalPacking1.Text != "")
            {

                double ItemFirstWtWb = Convert.ToDouble(TextItemFirstWtWb.Text);
                double ItemSecondWtWb = Convert.ToDouble(TextItemSecondWtWb.Text);
                double WtTotalPacking1 = Convert.ToDouble(TextWtTotalPacking1.Text);
                double ItemNetWt = ItemSecondWtWb - (ItemFirstWtWb + WtTotalPacking1);
                TextItemFirstWtWb.Enabled = false;
                TextItemSecondWtWb.Enabled = false;
                TextWtTotalPacking1.Enabled = false;

                string makeSQL = " SELECT MFSM.ITEM_ID, (nvl(MFSM.FINAL_STOCK_WT,0) - SUM(nvl(MEWC.ITEM_WEIGHT,0))) AS FINAL_STOCK_WT FROM MF_FG_STOCK_INVENTORY_MASTER MFSM LEFT JOIN MF_EXPORT_WBSLIP_CON MEWC ON MEWC.ITEM_ID = MFSM.ITEM_ID AND MEWC.IS_INVENTORY_STATUS = 'Transit' where MFSM.ITEM_ID = '" + ItemID + "' GROUP BY MFSM.ITEM_ID, MFSM.FINAL_STOCK_WT ";
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count; 
                double FinalStock = 0.00; 
                for (int i = 0; i < RowCount; i++)
                {
                    FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                }

                if (ItemNetWt <= FinalStock)
                {
                    CheckItemWeight.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Item Weight is available in Finished Goods Inventory</label>";
                    CheckItemWeight.ForeColor = System.Drawing.Color.Green; 
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");
                    BtnUpdate.Attributes.Add("aria-disabled", "true");
                    BtnUpdate.Attributes.Add("class", "btn btn-success active");

                    EntryDate.Focus();
                }
                else
                {
                    CheckItemWeight.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Item Weight is not available in Finished Goods Inventory. Available Weight is <span class='badge bg-yellow'>" + FinalStock + "</span> KG</label>";
                    CheckItemWeight.ForeColor = System.Drawing.Color.Red;
                    DropDownItemID.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                    BtnUpdate.Attributes.Add("aria-disabled", "false");
                    BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                     
                }
            }
            else {
                DropDownItemID.Text = "0";
            }
        }

        public void TextSlipNo_TextChanged(object sender, EventArgs e)
        {
          try
            {
            string SlipNo = TextSlipNo.Text;
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
                    cmd.CommandText = "select WB_SLIP_NO from MF_EXPORT_WBSLIP_CON where WB_SLIP_NO = '" + SlipNo + "'";
                    cmd.CommandType = CommandType.Text;

                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        CheckSlipNo.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Weight Slip Number is not available</label>";
                        CheckSlipNo.ForeColor = System.Drawing.Color.Red;
                        TextSlipNo.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

                         
                    }
                    else
                    {
                        CheckSlipNo.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Weight Slip Number is available</label>";
                        CheckSlipNo.ForeColor = System.Drawing.Color.Green;
                        TextContainerNo.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");
 
                    }
                }
                else
                {
                    CheckSlipNo.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter Weight Slip Number is 5 digit only</label>";
                    CheckSlipNo.ForeColor = System.Drawing.Color.Red;
                    TextSlipNo.Focus();
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