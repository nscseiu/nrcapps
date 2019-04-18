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

namespace NRCAPPS.PF
{
    public partial class PfExpContainerEX : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl, cmdsp;
        OracleDataAdapter oradata; 
        DataTable dt;
        int RowCount;
        double ItemWtTotal=0.0, ItemWtWbTotal=0.0, NoOfBags=0.0, WtPerBag=0.0, TotalBagsWt=0.0, NoOfWp=0.0, WtPerWp=0.0, TotalWpWt=0.0, TotalItemWt=0.0;
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
                        string makeSupplierSQL = " SELECT PARTY_ID, PARTY_NAME || ' - ' || PARTY_ID || ' - ' || PARTY_VAT_NO AS PARTY_NAME  FROM PF_PARTY WHERE IS_ACTIVE = 'Enable' ORDER BY PARTY_NAME ASC";
                        ds = ExecuteBySqlString(makeSupplierSQL);
                        dtSupplierID = (DataTable)ds.Tables[0];
                        DropDownSupplierID.DataSource = dtSupplierID;
                        DropDownSupplierID.DataValueField = "PARTY_ID";
                        DropDownSupplierID.DataTextField = "PARTY_NAME";
                        DropDownSupplierID.DataBind();
                        DropDownSupplierID.Items.Insert(0, new ListItem("Select  Supplier", "0"));
                          
                        DataTable dtSupervisorID = new DataTable();
                        DataSet dsd = new DataSet();
                        string makeSupervisorSQL = " SELECT * FROM PF_SALESMAN WHERE IS_ACTIVE = 'Enable' ORDER BY SALESMAN_NAME ASC";
                        dsd = ExecuteBySqlString(makeSupervisorSQL);
                        dtSupplierID = (DataTable)dsd.Tables[0];
                        DropDownSalesmanID.DataSource = dtSupplierID;
                        DropDownSalesmanID.DataValueField = "SALESMAN_ID";
                        DropDownSalesmanID.DataTextField = "SALESMAN_NAME";
                        DropDownSalesmanID.DataBind();
                        DropDownSalesmanID.Items.Insert(0, new ListItem("Select  Salesman", "0"));
                                              
                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeDropDownItemSQL = " SELECT * FROM PF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
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

                        DataTable dtSubItemID = new DataTable();
                        DataSet dss = new DataSet();
                        string makeDropDownSubItemSQL = " SELECT * FROM PF_SUB_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY SUB_ITEM_ID DESC";
                        dss = ExecuteBySqlString(makeDropDownSubItemSQL);
                        dtSubItemID = (DataTable)dss.Tables[0];
                        DropDownSubItemID.DataSource = dtSubItemID;
                        DropDownSubItemID.DataValueField = "SUB_ITEM_ID";
                        DropDownSubItemID.DataTextField = "SUB_ITEM_NAME";
                        DropDownSubItemID.DataBind();
                        //      DropDownSubItemID.Items.Insert(0, new ListItem("Select Sub Item", "0"));

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
                        string makeSrSQL = " SELECT WB_SLIP_NO FROM PF_EXPORT_WBSLIP_CON WHERE IS_ACTIVE = 'Enable' ORDER BY WB_SLIP_NO DESC";
                        dse = ExecuteBySqlString(makeSrSQL);
                        dtWbSlipID = (DataTable)dse.Tables[0];
                        DropDownSlipID.DataSource = dtWbSlipID;
                        DropDownSlipID.DataValueField = "WB_SLIP_NO";
                        DropDownSlipID.DataTextField = "WB_SLIP_NO";
                        DropDownSlipID.DataBind();
                        DropDownSlipID.Items.Insert(0, new ListItem("Select  Weight Slip", "0"));

                        TextSlipNo.Focus();
                        TextNoOfWoodPall.Text = "0";
                        TextWoodPallPerWt.Text = "0";
                        TextWoodPallPerWtTotal.Text = "0";
                        TextBagsPerWtTotal.Enabled = false;
                        TextWoodPallPerWtTotal.Enabled = false; 
                        DropDownItemID.Enabled = false;
                        DropDownSubItemID.Enabled = false;
                        TextNoOfBags.Enabled = false;
                        TextBagsPerWt.Enabled = false;
                        TextNoOfWoodPall.Enabled = false;
                        TextWoodPallPerWt.Enabled = false;
                        TextItemWeightWBEx.Attributes.Add("readonly", "readonly"); 

                        Display();
                        DisplaySummary();
                        alert_box.Visible = false;

                    //    BtnAdd.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(BtnAdd, null) + ";");

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
                    int SalesmanID = Convert.ToInt32(DropDownSalesmanID.Text);
                    int ContainerSizeID = Convert.ToInt32(DropDownContainerSizeID.Text);

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");   

                    string get_user_purchase_id = "select PF_EXPORT_CONTAINERID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_user_purchase_id, conn);
                    int newPurchaseID = Int32.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_purchase = "insert into  PF_EXPORT_WBSLIP_CON (EXP_WBCON_ID, WB_SLIP_NO, CONTAINER_NO, CONTAINER_SIZE_ID, SEAL_NO, REL_ORDER_NO, PARTY_ID, SALESMAN_ID, ITEM_WEIGHT_WB, DISPATCH_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID, IS_INVENTORY_STATUS ) values  ( :NoPurchaseID, :NoSlipID, :TextContainerNo, :NoContainerSizeID, :TextSealNo, :TextRelOrderNo, :NoSupplierID, :NoSalesmanID, :TextItemWtWb, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 3, 'Transit'  )";
                    cmdi = new OracleCommand(insert_purchase, conn);

                    OracleParameter[] objPrm = new OracleParameter[13];
                    objPrm[0] = cmdi.Parameters.Add("NoPurchaseID", newPurchaseID);
                    objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNo);
                    objPrm[2] = cmdi.Parameters.Add("TextContainerNo", TextContainerNo.Text);
                    objPrm[3] = cmdi.Parameters.Add("NoContainerSizeID", ContainerSizeID);
                    objPrm[4] = cmdi.Parameters.Add("TextSealNo", TextSealNo.Text);
                    objPrm[5] = cmdi.Parameters.Add("TextRelOrderNo", TextRelOrderNo.Text);
                    objPrm[6] = cmdi.Parameters.Add("NoSalesmanID", SalesmanID);
                    objPrm[7] = cmdi.Parameters.Add("NoSupplierID", SupplierID);
                    objPrm[8] = cmdi.Parameters.Add("TextItemWtWb", TextItemWtWb.Text);
                    objPrm[9] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[10] = cmdi.Parameters.Add("c_date", c_date);
                    objPrm[11] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[12] = cmdi.Parameters.Add("TextIsActive", ISActive);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                     
                    DataTable dtWbSlipID = new DataTable();
                    DataSet dse = new DataSet();
                    string makeSrSQL = " SELECT WB_SLIP_NO FROM PF_EXPORT_WBSLIP_CON WHERE IS_ACTIVE = 'Enable' ORDER BY WB_SLIP_NO DESC";
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
                    TextSlipNo.Focus();
                    Display();
                 //   DisplaySummary();
                   
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
            try
              { 
            if (IS_ADD_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open(); 
                int userID = Convert.ToInt32(Session["USER_ID"]); 
                string SlipNo = DropDownSlipID.Text; 
                int ItemID   = Convert.ToInt32(DropDownItemID.Text);
                int SubItemID   = Convert.ToInt32(DropDownSubItemID.Text); 
                int NoOfBags = Convert.ToInt32(TextNoOfBags.Text);
                int NoOfWoodPall = Convert.ToInt32(TextNoOfWoodPall.Text); 
                string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                  double ItemWeightWBEx = Convert.ToDouble(TextItemWeightWBEx.Text);
                  double BagsPerWt = Convert.ToDouble(TextBagsPerWt.Text);
                  double WoodPallPerWt = Convert.ToDouble(TextWoodPallPerWt.Text);

                double BagsTotal = Math.Round(NoOfBags * BagsPerWt, 0, MidpointRounding.AwayFromZero);
                double WoodPallTotal = Math.Round(NoOfWoodPall * WoodPallPerWt, 0, MidpointRounding.AwayFromZero);
                double ItemWeightEx = ItemWeightWBEx - (BagsTotal + WoodPallPerWt);

                string get_id = "select PF_EXPORT_WBS_CON_ITEMID_SEQ.nextval from dual";
                cmdsp = new OracleCommand(get_id, conn);
                int newExWbID = Int16.Parse(cmdsp.ExecuteScalar().ToString());
                string insert_data = "insert into  PF_EXPORT_WBSLIP_CON_ITEM (EXP_WBCON_ITEM_ID, WB_SLIP_NO, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, NO_OF_BAGS, WET_PER_BAG, TOTAL_BAG_WT, NO_OF_WOOD_PALLETS, WET_PER_WP, TOTAL_WP_WT, CREATE_DATE, C_USER_ID) values  ( :NoExpWbConID, :NoSlipID, :NoItemID, :NoSubItemID, :TextItemWeightEx, :NoOfBags, :TextBagsPerWt, :TextBagsWt, :NoOfWoodPall, :TextWoodPallPerWt, :TextWpWt, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                cmdi = new OracleCommand(insert_data, conn);

                OracleParameter[] objPrm = new OracleParameter[13];
                objPrm[0] = cmdi.Parameters.Add("NoExpWbConID", newExWbID);
                objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNo);
                objPrm[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                objPrm[3] = cmdi.Parameters.Add("NoSubItemID", SubItemID);
                objPrm[4] = cmdi.Parameters.Add("TextItemWeightEx", ItemWeightEx);
                objPrm[5] = cmdi.Parameters.Add("NoOfBags", NoOfBags);
                objPrm[6] = cmdi.Parameters.Add("TextBagsPerWt", BagsPerWt);
                objPrm[7] = cmdi.Parameters.Add("TextBagsWt", BagsTotal);
                objPrm[8] = cmdi.Parameters.Add("NoOfWoodPall", NoOfWoodPall);
                objPrm[9] = cmdi.Parameters.Add("TextWoodPallPerWt", WoodPallPerWt);
                objPrm[10] = cmdi.Parameters.Add("TextWpWt", WoodPallTotal);
                objPrm[11] = cmdi.Parameters.Add("c_date", c_date);
                objPrm[12] = cmdi.Parameters.Add("NoCuserID", userID); 
                 
                cmdi.ExecuteNonQuery(); 
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();
                 
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Insert New Weight Slip Material Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                clearTextItem();
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
            string makeSQL = " SELECT PEWC.EXP_WBCON_ID, PEWC.WB_SLIP_NO, PEWC.CONTAINER_NO, PP.PARTY_NAME, PEWC.SEAL_NO, PEWC.REL_ORDER_NO, nvl(PEWC.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PEWCI.ITEM_WEIGHT, PEWCI.NO_OF_BAGS, PEWCI.WET_PER_BAG, PEWCI.TOTAL_BAG_WT, PEWCI.NO_OF_WOOD_PALLETS, PEWCI.WET_PER_WP, PEWCI.TOTAL_WP_WT FROM PF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWCI.SUB_ITEM_ID LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PEWC.PARTY_ID WHERE PEWC.WB_SLIP_NO = '" + TextSlipWbNo.Text + "' ORDER BY PI.ITEM_ID";

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
                       RelOrderNo = dt.Rows[i]["REL_ORDER_NO"].ToString();
                HtmlString += "<div style='float:left;width:785px;height:258px;margin-top:288px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";
                HtmlString += "<div style='float:left;width:470px;'> "; 
                HtmlString += "<div style='float:left;width:260px;margin-left:200px;padding-top:0px;text-align:center;' > Container No.  " + ContainerNo + " </div> ";
                HtmlString += "<div style='float:left;width:260px;height:40px;margin-left:200px;padding-top:10px;text-align:center;' > " + PartyName + " </div> ";
               
            }
            int m = 1, n = 1; 
            HtmlString += "<div style='float:left;width:260px;height:28px;margin:0px 0 0 200px;text-align:center;'> ";
            for (int i = 0; i < RowCount; i++)
                { 
                    string ItemName = dt.Rows[i]["ITEM_NAME"].ToString();
                    string SubItemName = dt.Rows[i]["SUB_ITEM_NAME"].ToString();
                           NoOfBags += Convert.ToDouble(dt.Rows[i]["NO_OF_BAGS"].ToString());
                           WtPerBag = Convert.ToDouble(dt.Rows[i]["WET_PER_BAG"].ToString());
                           TotalBagsWt += Convert.ToDouble(dt.Rows[i]["TOTAL_BAG_WT"].ToString());
                           NoOfWp += Convert.ToDouble(dt.Rows[i]["NO_OF_WOOD_PALLETS"].ToString());
                           WtPerWp = Convert.ToDouble(dt.Rows[i]["WET_PER_WP"].ToString());
                           TotalWpWt += Convert.ToDouble(dt.Rows[i]["TOTAL_WP_WT"].ToString());
                           TotalItemWt += Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());
                //    TotalBagsWt = NoOfBags * WtPerBag;
                //    TotalWpWt   = NoOfWp * WtPerWp;
                 
                if (m == RowCount) {
                    HtmlString += "" + ItemName + "-" + SubItemName + "";
                } else {  HtmlString += "" + ItemName + "-" + SubItemName + ",";  }
                m++;    
                }
                HtmlString += "</div> ";

                HtmlString += "<div style='float:left;width:260px;height:20px;margin-left:187px;padding:5px 0 0 5px;text-align:center;border:black solid 1px;'> "; 
                HtmlString += "" + NoOfBags + " JUMBO BAGS * " + WtPerBag + " = " + TotalBagsWt +  ""; 
                HtmlString += "</div> ";

                HtmlString += "<div style='float:left;width:260px;height:20px;margin-left:187px;padding:5px 0 0 5px;text-align:center;border:black solid 1px;'> "; 
                HtmlString += "" + NoOfWp + " WOODEN PALLETS * " + WtPerWp + " = " + TotalWpWt + ""; 
                HtmlString += "</div> ";

                HtmlString += "<div style='float:left;width:260px;height:20px;margin-left:80px;padding:40px 0 0 5px;text-align:center;'> "; 
                HtmlString += "SEAL NO: &nbsp;&nbsp;&nbsp;&nbsp;" + SealNo +  ""; 
                HtmlString += "</div> ";

                HtmlString += "</div> ";
        
                HtmlString += "<div style='float:left;width:310px;'> "; 
                HtmlString += "<div style='float:left;width:200px;height:20px;margin:0 0 0 50px;'> Less Bags: " + TotalBagsWt + "</div> ";
                HtmlString += "<div style='float:left;width:200px;height:20px;margin:0 0 0 50px;'> Less WOODEN PALLETS: " + TotalWpWt + "</div> ";
                HtmlString += "<div style='float:left;width:200px;height:20px;margin:0 0 0 50px;'> NET WT. (KG): " +  string.Format("{0:n0}", TotalItemWt) + "</div> ";
                HtmlString += "<div style='float:left;width:300px;height:20px;margin:60 0 0 50px;'> Release Order No: " + RelOrderNo + "</div> ";
                HtmlString += "</div>";
                HtmlString += "</div>"; 
          
            HtmlString += "</div>";   
            HtmlString += "</div>";
            HtmlString += "</div>";
            HtmlString += "</div>";

            // weigh-bridge & container update for print
            int userID = Convert.ToInt32(Session["USER_ID"]);
            string SlipNo = TextSlipWbNo.Text; 
            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
            string update_user = "update  PF_EXPORT_WBSLIP_CON  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where WB_SLIP_NO = :NoSlipNo ";
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

     /*   TextBox tb;
        static int i = -1;
        protected void addnewtext_Click(object sender, EventArgs e)
        {
            i++;
            for (int j = 0; j <= i; j++)
            {
                tb = new TextBox();
                tb.ID = j.ToString();
                PlaceHolder1.Controls.Add(tb);

                DropDownList ddl = new DropDownList(); 
                ddl.ID = "txtVersion" + j;
                ddl.Items.Add(new ListItem("--Select--", "")); 
                ddl.Items.Add(new ListItem("HDPE", "1")); 
                ddl.Items.Add(new ListItem("HD CAN", "2")); 
                ddl.Items.Add(new ListItem("LDPE", "3"));

                ddl.AutoPostBack = true;

                ddl.SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);

                pnlDropDownList.Controls.Add(ddl);  
                Literal lt = new Literal(); 
                lt.Text = "<br />"; 
                pnlDropDownList.Controls.Add(lt);
            }

        }

        protected void OnSelectedIndexChanged(object sender, EventArgs e)

        {

            DropDownList ddl = (DropDownList)sender;

            string ID = ddl.ID;

            ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert",

                         "<script type = 'text/javascript'>alert('" + ID +

                          " fired SelectedIndexChanged event');</script>");



            //Place the functionality here

        }
        */
        [WebMethod]
        public static List<ListItem> GetItemList()
        {
          //  OracleConnection conn = new OracleConnection(strConnString);
         //   conn.Open();
            string query = " SELECT * FROM PF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC ";
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
                 
            string makeSQL = " select EXP_WBCON_ID, WB_SLIP_NO, CONTAINER_NO, CONTAINER_SIZE_ID, PARTY_ID,  SEAL_NO, REL_ORDER_NO,  SALESMAN_ID, nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, TO_CHAR(DISPATCH_DATE,'dd/mm/yyyy') AS DISPATCH_DATE, IS_ACTIVE from PF_EXPORT_WBSLIP_CON where WB_SLIP_NO  = '" + USER_DATA_ID + "'";
            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                TextExWbConID.Text = dt.Rows[i]["EXP_WBCON_ID"].ToString();   
                TextSlipNo.Text = dt.Rows[i]["WB_SLIP_NO"].ToString();
                TextSlipWbNo.Text = dt.Rows[i]["WB_SLIP_NO"].ToString();
                DropDownSupplierID.Text = dt.Rows[i]["PARTY_ID"].ToString();
                TextContainerNo.Text = dt.Rows[i]["CONTAINER_NO"].ToString();
                DropDownContainerSizeID.Text = dt.Rows[i]["CONTAINER_SIZE_ID"].ToString();
                TextSealNo.Text = dt.Rows[i]["SEAL_NO"].ToString();
                TextRelOrderNo.Text = dt.Rows[i]["REL_ORDER_NO"].ToString();
                DropDownSalesmanID.Text = dt.Rows[i]["SALESMAN_ID"].ToString();  
                EntryDate.Text = dt.Rows[i]["DISPATCH_DATE"].ToString();
                TextItemWtWb.Text  = dt.Rows[i]["ITEM_WEIGHT_WB"].ToString();
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                   
            }

            conn.Close();
            Display();
            CheckSlipNo.Text = "";
            alert_box.Visible = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
             
            TextSlipNo.Enabled = false; 

         //    }
         //     catch
         //   {
         //      Response.Redirect("~/ParameterError.aspx");
         //   } 
        }

        protected void linkSelectItemClick(object sender, EventArgs e)
        {
            try
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                LinkButton btn = (LinkButton)sender;
                Session["user_data_id"] = btn.CommandArgument;
                int USER_DATA_ID = Convert.ToInt32(Session["user_data_id"]);

                string makeSQL = " select EXP_WBCON_ITEM_ID, WB_SLIP_NO, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, NO_OF_BAGS, WET_PER_BAG, TOTAL_BAG_WT, NO_OF_WOOD_PALLETS, WET_PER_WP, TOTAL_WP_WT from PF_EXPORT_WBSLIP_CON_ITEM where EXP_WBCON_ITEM_ID  = '" + USER_DATA_ID + "'";
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                for (int i = 0; i < RowCount; i++)
                {
                    TextExpWbConItemID.Text = dt.Rows[i]["EXP_WBCON_ITEM_ID"].ToString();
                    DropDownSlipID.Text = dt.Rows[i]["WB_SLIP_NO"].ToString();
                    DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();
                    DropDownSubItemID.Text = dt.Rows[i]["SUB_ITEM_ID"].ToString();
                    TextNoOfBags.Text = dt.Rows[i]["NO_OF_BAGS"].ToString();
                    TextBagsPerWt.Text = dt.Rows[i]["WET_PER_BAG"].ToString(); 
                    TextBagsPerWtTotal.Text = dt.Rows[i]["TOTAL_BAG_WT"].ToString();
                    TextNoOfWoodPall.Text = dt.Rows[i]["NO_OF_WOOD_PALLETS"].ToString();
                    TextWoodPallPerWt.Text = dt.Rows[i]["WET_PER_WP"].ToString();
                    TextWoodPallPerWtTotal.Text = dt.Rows[i]["TOTAL_WP_WT"].ToString();
                    TextItemWeightEx.Text = dt.Rows[i]["ITEM_WEIGHT"].ToString();

                  //  string USER_DATA_ID = DropDownSlipID.Text;
                    string makeSrSQL = " select ITEM_WEIGHT_WB from PF_EXPORT_WBSLIP_CON where WB_SLIP_NO  = '" + dt.Rows[i]["WB_SLIP_NO"].ToString() + "'";
                    cmdl = new OracleCommand(makeSrSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;
                    for (int j = 0; j < RowCount; j++)
                    {
                        TextItemWeightWBEx.Text = dt.Rows[i]["ITEM_WEIGHT_WB"].ToString();
                    }

                }

                

                conn.Close();
                Display();
                CheckSlipNo.Text = "";
                alert_box.Visible = false;
                TextItemWeightEx.Enabled = true;
                DropDownItemID.Enabled = true;
                DropDownSubItemID.Enabled = true;
                TextNoOfBags.Enabled = true;
                TextBagsPerWt.Enabled = true;
                TextNoOfWoodPall.Enabled = true;
                TextWoodPallPerWt.Enabled = true;

                BtnAddItem.Attributes.Add("aria-disabled", "false");
                BtnAddItem.Attributes.Add("class", "btn btn-primary disabled");
                 
                DropDownSlipID.Enabled = false;
            }
            catch
            {
                Response.Redirect("~/ParameterError.aspx");
            }
        }

        protected void GetItemWtWb(object sender, EventArgs e)
        {
          try
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open(); 
                string USER_DATA_ID = DropDownSlipID.Text;
                double ItemWeightWBEx = 0.0, ItemWeight = 0.0;
                string makeSQL = " SELECT nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB from PF_EXPORT_WBSLIP_CON where WB_SLIP_NO  = '" + USER_DATA_ID + "'";
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count; 
                for (int i = 0; i < RowCount; i++)
                {
                   ItemWeightWBEx = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString());  
                }

                string makeSrSQL = " SELECT nvl(sum(ITEM_WEIGHT),0) AS ITEM_WEIGHT, nvl(sum(TOTAL_BAG_WT),0) AS TOTAL_BAG_WT, nvl(sum(TOTAL_WP_WT),0) AS TOTAL_WP_WT from PF_EXPORT_WBSLIP_CON_ITEM where WB_SLIP_NO  = '" + USER_DATA_ID + "'";
                cmdl = new OracleCommand(makeSrSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                for (int i = 0; i < RowCount; i++)
                {
                    ItemWeight = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());
                    TotalBagsWt = Convert.ToDouble(dt.Rows[i]["TOTAL_BAG_WT"].ToString());
                    TotalWpWt = Convert.ToDouble(dt.Rows[i]["TOTAL_WP_WT"].ToString());
                }

                conn.Close();

                TextItemWeightWBEx.Text = Convert.ToString(ItemWeightWBEx - (ItemWeight+TotalBagsWt+TotalWpWt));

                TextItemWeightEx.Text = "";
                TextNoOfBags.Text = "";
                TextBagsPerWt.Text = "";
                DropDownItemID.Enabled = true;
                DropDownSubItemID.Enabled = true;
                TextNoOfBags.Enabled = true;
                TextBagsPerWt.Enabled = true;
                TextNoOfWoodPall.Enabled = true;
                TextWoodPallPerWt.Enabled = true;
                TextItemWeightEx.Enabled = true;
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
                    makeSQL = " SELECT PEWC.WB_SLIP_NO, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.SEAL_NO, PEWC.REL_ORDER_NO, PP.PARTY_NAME, PEWCI.EXP_WBCON_ITEM_ID, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PEWC.ITEM_WEIGHT_WB,  PEWCI.ITEM_WEIGHT, PEWCI.NO_OF_BAGS, PEWCI.WET_PER_BAG,  PEWCI.TOTAL_BAG_WT, PEWCI.NO_OF_WOOD_PALLETS, PEWCI.WET_PER_WP,  PEWCI.TOTAL_WP_WT, PS.SALESMAN_NAME, PEWC.IS_ACTIVE ,  PEWC.IS_INVENTORY_STATUS,  PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT,  PEWC.PRINT_DATE,  PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO  FROM PF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PEWC.PARTY_ID LEFT JOIN PF_SALESMAN PS ON PS.SALESMAN_ID = PEWC.SALESMAN_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWCI.SUB_ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE to_char(PEWC.DISPATCH_DATE, 'mm/yyyy') = '" + MonthYear + "' ORDER BY PEWC.WB_SLIP_NO desc, PEWCI.ITEM_ID  ASC "; // OR IS_INVENTORY_STATUS = 'Transit'
                }
                else
                {
                    if (DropDownIsInven.Text == "0")
                    {
                        makeSQL = "  SELECT PEWC.WB_SLIP_NO, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.SEAL_NO, PEWC.REL_ORDER_NO, PP.PARTY_NAME, PEWCI.EXP_WBCON_ITEM_ID, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PEWC.ITEM_WEIGHT_WB,  PEWCI.ITEM_WEIGHT, PEWCI.NO_OF_BAGS, PEWCI.WET_PER_BAG,  PEWCI.TOTAL_BAG_WT, PEWCI.NO_OF_WOOD_PALLETS, PEWCI.WET_PER_WP,  PEWCI.TOTAL_WP_WT, PS.SALESMAN_NAME, PEWC.IS_ACTIVE ,  PEWC.IS_INVENTORY_STATUS,  PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT,  PEWC.PRINT_DATE,  PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO FROM PF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PEWC.PARTY_ID LEFT JOIN PF_SALESMAN PS ON PS.SALESMAN_ID = PEWC.SALESMAN_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWCI.SUB_ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE PEWC.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or PEWC.CONTAINER_NO like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PEWC.SEAL_NO like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PEWC.REL_ORDER_NO like '" + txtSearchEmp.Text + "%' or to_char(PEWC.DISPATCH_DATE, 'dd/mm/yyyy')  like '" + txtSearchEmp.Text + "%' or PEWC.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ORDER BY PEWC.WB_SLIP_NO desc, PEWCI.ITEM_ID asc ";   
                    }
                    else
                    {
                        makeSQL = "  SELECT PEWC.WB_SLIP_NO, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.SEAL_NO, PEWC.REL_ORDER_NO, PP.PARTY_NAME, PEWCI.EXP_WBCON_ITEM_ID, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PEWC.ITEM_WEIGHT_WB,  PEWCI.ITEM_WEIGHT, PEWCI.NO_OF_BAGS, PEWCI.WET_PER_BAG,  PEWCI.TOTAL_BAG_WT, PEWCI.NO_OF_WOOD_PALLETS, PEWCI.WET_PER_WP,  PEWCI.TOTAL_WP_WT, PS.SALESMAN_NAME, PEWC.IS_ACTIVE ,  PEWC.IS_INVENTORY_STATUS,  PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT,  PEWC.PRINT_DATE,  PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO FROM PF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PEWC.PARTY_ID LEFT JOIN PF_SALESMAN PS ON PS.SALESMAN_ID = PEWC.SALESMAN_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWCI.SUB_ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE IS_INVENTORY_STATUS = '" + DropDownIsInven.Text + "' AND (PEWC.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or PEWC.CONTAINER_NO like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PEWC.SEAL_NO like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PEWC.REL_ORDER_NO like '" + txtSearchEmp.Text + "%' or to_char(PEWC.DISPATCH_DATE, 'dd/mm/yyyy')  like '" + txtSearchEmp.Text + "%' or PEWC.IS_ACTIVE like '" + txtSearchEmp.Text + "%') ORDER BY PEWC.WB_SLIP_NO desc, PEWCI.ITEM_ID asc ";
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
                    GroupGridView(GridView1.Rows, 0, 15);
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

                if (isCheck == "Complete" || isCheckExInvoiceNo == "Yes")  // || isCheckPrint == "Printed"
                {
                    (Row.FindControl("linkSelectClick") as LinkButton).Visible = false; 
                    (Row.FindControl("linkSelectItemClick") as LinkButton).Visible = false; 
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
                    makeSQL = " SELECT PI.ITEM_NAME, nvl(sum(PEWCI.ITEM_WEIGHT), 0) AS ITEM_WEIGHT FROM PF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWCI.SUB_ITEM_ID WHERE IS_INVENTORY_STATUS = 'Transit' AND PI.ITEM_NAME IS NOT NULL GROUP BY PI.ITEM_NAME ";
                }
                else
                {
                    makeSQL = " SELECT PI.ITEM_NAME, nvl(sum(PEWCI.ITEM_WEIGHT), 0) AS ITEM_WEIGHT FROM PF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWCI.SUB_ITEM_ID WHERE IS_INVENTORY_STATUS = '" + DropDownSearchSummary.Text + "' AND PI.ITEM_NAME IS NOT NULL GROUP BY PI.ITEM_NAME ";
                          
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
                   
           //     double ItemRate = Convert.ToDouble(TextItemRate.Text.Trim()); 
           //     double ItemAmount = ItemRate * ItemWeight; 
           //     string ItemAmountNew = Math.Round(decimal.Parse(ItemAmount.ToString()), 2).ToString();
           //     double ItemAmountNewD = Convert.ToDouble(ItemAmountNew); 
                             
                // purchase master update

                string update_user = "update  PF_EXPORT_WBSLIP_CON  set WB_SLIP_NO = :NoSlipNo, CONTAINER_NO = :NoContainerNo, CONTAINER_SIZE_ID = :NoContainerSize, SEAL_NO = :TextSealNo, REL_ORDER_NO = :TextRelOrderNo, ITEM_WEIGHT_WB = :NoItemWeightWb, SALESMAN_ID = : NoSalesmanID,  PARTY_ID = :NoPartyID, DISPATCH_DATE = TO_DATE(:DispatchDate, 'DD/MM/YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where EXP_WBCON_ID = :NoExWbConID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[13]; 
                objPrm[0] = cmdi.Parameters.Add("NoSlipNo", TextSlipNo.Text);
                objPrm[1] = cmdi.Parameters.Add("NoContainerNo", TextContainerNo.Text);
                objPrm[2] = cmdi.Parameters.Add("NoContainerSize", ContainerSizeID);
                objPrm[3] = cmdi.Parameters.Add("TextSealNo", TextSealNo.Text);   
                objPrm[4] = cmdi.Parameters.Add("TextRelOrderNo", TextRelOrderNo.Text);  
                objPrm[5] = cmdi.Parameters.Add("NoItemWeightWb", TextItemWtWb.Text);  
                objPrm[6] = cmdi.Parameters.Add("NoSalesmanID", DropDownSalesmanID.Text);  
                objPrm[7] = cmdi.Parameters.Add("NoPartyID", DropDownSupplierID.Text);  
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
                  
                int ItemID = Convert.ToInt32(DropDownItemID.Text);
                int SubItemID = Convert.ToInt32(DropDownSubItemID.Text);
                int NoOfBags = Convert.ToInt32(TextNoOfBags.Text);
                int NoOfWoodPall = Convert.ToInt32(TextNoOfWoodPall.Text); 

                double ItemWeightWBEx = Convert.ToDouble(TextItemWeightWBEx.Text);
                double BagsPerWt = Convert.ToDouble(TextBagsPerWt.Text);
                double WoodPallPerWt = Convert.ToDouble(TextWoodPallPerWt.Text);

                double BagsTotal = Math.Round(NoOfBags * BagsPerWt, 0, MidpointRounding.AwayFromZero);
                double WoodPallTotal = Math.Round(NoOfWoodPall * WoodPallPerWt, 0, MidpointRounding.AwayFromZero);
                //   double ItemWeightEx = ItemWeightWBEx - (BagsTotal + WoodPallPerWt);
                double ItemWeightEx = Convert.ToDouble(TextItemWeightEx.Text);


                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                 
                string update_user = "update  PF_EXPORT_WBSLIP_CON_ITEM  set ITEM_ID = :NoItemID, SUB_ITEM_ID = :NoSubItemID, ITEM_WEIGHT = :TextItemWeightEx, NO_OF_BAGS = :NoOfBags, WET_PER_BAG = :TextBagsPerWt, TOTAL_BAG_WT = :TextBagsWt, NO_OF_WOOD_PALLETS = :NoOfWoodPall, WET_PER_WP =:TextWoodPallPerWt, TOTAL_WP_WT =:TextWpWt, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where EXP_WBCON_ITEM_ID = :NoExpWbConItemID ";
                cmdu = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[12];
                objPrm[0] = cmdu.Parameters.Add("NoItemID", ItemID);
                objPrm[1] = cmdu.Parameters.Add("NoSubItemID", SubItemID);
                objPrm[2] = cmdu.Parameters.Add("TextItemWeightEx", ItemWeightEx);
                objPrm[3] = cmdu.Parameters.Add("NoOfBags", NoOfBags);
                objPrm[4] = cmdu.Parameters.Add("TextBagsPerWt", BagsPerWt);
                objPrm[5] = cmdu.Parameters.Add("TextBagsWt", BagsTotal);
                objPrm[6] = cmdu.Parameters.Add("NoOfWoodPall", NoOfWoodPall);
                objPrm[7] = cmdu.Parameters.Add("TextWoodPallPerWt", WoodPallPerWt);
                objPrm[8] = cmdu.Parameters.Add("TextWpWt", WoodPallTotal);
                objPrm[9] = cmdu.Parameters.Add("u_date", u_date);
                objPrm[10] = cmdu.Parameters.Add("NoCuserID", userID); 
                objPrm[11] = cmdu.Parameters.Add("NoExpWbConItemID", ExpWbConItemID);

                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose();
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Container & Weight Slip Data Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearTextItem();
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

        protected void BtnDeleteItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (IS_DELETE_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    int ExpWbConItemID = Convert.ToInt32(TextExpWbConItemID.Text); 
                    string delete_user = " delete from PF_EXPORT_WBSLIP_CON_ITEM where EXP_WBCON_ITEM_ID  = '" + ExpWbConItemID + "'";
                    cmdi = new OracleCommand(delete_user, conn);
                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose(); 
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
                int ExWbConID = Convert.ToInt32(TextExWbConID.Text);              
                string SlipNo =  TextSlipNo.Text; 
                string delete_user = " delete from PF_EXPORT_WBSLIP_CON where EXP_WBCON_ID  = '" + ExWbConID + "'"; 
                cmdi = new OracleCommand(delete_user, conn); 
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                string delete_item = " delete from PF_EXPORT_WBSLIP_CON_ITEM where WB_SLIP_NO  = '" + SlipNo + "'";
                cmdu = new OracleCommand(delete_item, conn);
                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose();

                conn.Close();

                DataTable dtWbSlipID = new DataTable();
                DataSet dse = new DataSet();
                string makeSrSQL = " SELECT WB_SLIP_NO FROM PF_EXPORT_WBSLIP_CON WHERE IS_ACTIVE = 'Enable' ORDER BY WB_SLIP_NO DESC";
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
            TextSlipNo.Text = ""; 
         //   TextItemRate.Text = "";
         //   TextItemWeight.Text = "";
          //  DropDownSubItemID.Text = "0";
         //   TextItemAmount.Text = ""; 
            CheckSlipNo.Text = ""; 
            DropDownSupplierID.Text = "0";
            DropDownSalesmanID.Text = "0"; 
         //   DropDownItemID.Text = "0";
            TextItemWtWb.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        {
            TextSlipNo.Text = "";
            TextContainerNo.Text = "";
            TextSealNo.Text = "";
            DropDownSupplierID.Text = "0";
            TextRelOrderNo.Text = ""; 
            CheckSlipNo.Text = ""; 
            DropDownSupplierID.Text = "0";
            DropDownSalesmanID.Text = "0";
            //   DropDownItemID.Text = "0";
            TextItemWtWb.Text = "";
            EntryDate.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearTextItem()
        { 
            DropDownSlipID.Text = "0";  
            DropDownItemID.Text = "0";
            DropDownSubItemID.Text = "0"; 
            TextNoOfBags.Text = "";
            TextBagsPerWt.Text = "";
            TextNoOfWoodPall.Text = "0";
            TextWoodPallPerWt.Text = "0"; 
            TextWoodPallPerWtTotal.Text = "";
            TextItemWeightEx.Text = "";
            TextItemWeightWBEx.Text = "";
            BtnAddItem.Attributes.Add("aria-disabled", "false");
            BtnAddItem.Attributes.Add("class", "btn btn-primary disabled");

        }

     
        public void TextSlipNo_TextChanged(object sender, EventArgs e)
        {
          try
            {
            string SlipNo = TextSlipNo.Text;
            string MatchEmpIDPattern = "^([0-9]{5})$";
            if (SlipNo != null)
            {

                if (Regex.IsMatch(SlipNo, MatchEmpIDPattern))
                {
                    alert_box.Visible = false;

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "select WB_SLIP_NO from PF_EXPORT_WBSLIP_CON where WB_SLIP_NO = '" + SlipNo + "'";
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