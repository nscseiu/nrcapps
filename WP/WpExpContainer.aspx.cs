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

namespace NRCAPPS.WP
{
    public partial class WpExpContainer : System.Web.UI.Page
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


                        /*   DataTable dtItemID = new DataTable();
                         DataSet dsi = new DataSet();
                         string makeDropDownItemSQL = " SELECT * FROM WP_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                         dsi = ExecuteBySqlString(makeDropDownItemSQL);
                         dtItemID = (DataTable)dsi.Tables[0];
                         DropDownItemID.DataSource = dtItemID;
                         DropDownItemID.DataValueField = "ITEM_ID";
                         DropDownItemID.DataTextField = "ITEM_NAME";
                         DropDownItemID.DataBind();
                         DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));

                          DropDownItemID1.DataSource = dtItemID;
                            DropDownItemID1.DataValueField = "ITEM_ID";
                            DropDownItemID1.DataTextField = "ITEM_NAME";
                            DropDownItemID1.DataBind();
                            DropDownItemID1.Items.Insert(0, new ListItem("All Item", "0")); */

                        DataTable dtCategoryID = new DataTable();
                        DataSet dss = new DataSet();
                        string makeDropDownCatgorySQL = " SELECT * FROM WP_CATEGORY WHERE IS_ACTIVE = 'Enable' ORDER BY CATEGORY_ID ASC";
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
                        string makeSrSQL = "   SELECT WEWC.WB_SLIP_NO FROM WP_EXPORT_WBSLIP_CON WEWC WHERE WEWC.IS_ACTIVE = 'Enable' AND WEWC.EXPORT_INVOICE_NO IS NULL AND NOT EXISTS(SELECT WEWCI.WB_SLIP_NO FROM WP_EXPORT_WBSLIP_CON_ITEM WEWCI WHERE WEWCI.WB_SLIP_NO = WEWC.WB_SLIP_NO) ORDER BY WEWC.WB_SLIP_NO DESC ";
                        dse = ExecuteBySqlString(makeSrSQL);
                        dtWbSlipID = (DataTable)dse.Tables[0];
                        DropDownSlipID.DataSource = dtWbSlipID;
                        DropDownSlipID.DataValueField = "WB_SLIP_NO";
                        DropDownSlipID.DataTextField = "WB_SLIP_NO";
                        DropDownSlipID.DataBind();
                        DropDownSlipID.Items.Insert(0, new ListItem("Select  Weight Slip", "0"));

                        TextSlipWbNo.Focus();

                        DropDownItemID.Enabled = false;
                        TextItemWeightEx.Enabled = false;
                        TextBalesNumber.Enabled = false;
                        DropDownCategoryID.Enabled = false;
                        TextItemWeightWBEx.Attributes.Add("readonly", "readonly");

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

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string get_container_id = "select WP_EXPORT_CONTAINERID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_container_id, conn);
                    int newContainerID = Int32.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_purchase = "insert into  WP_EXPORT_WBSLIP_CON (EXP_WBCON_ID, WB_SLIP_NO, DRIVER_NAME, CONTAINER_NO, CONTAINER_SIZE_ID, ITEM_WEIGHT_WB, TARE_WEIGHT, DISPATCH_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID, IS_INVENTORY_STATUS, IS_EDIT ) values  ( :NoContainerID, :NoSlipID, :TextDriverName, :TextContainerNo, :NoContainerSizeID, :TextItemWtWb, :TextTareWeight, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 2, 'Transit', 'Enable' )";
                    cmdi = new OracleCommand(insert_purchase, conn);

                    OracleParameter[] objPrm = new OracleParameter[11];
                    objPrm[0] = cmdi.Parameters.Add("NoContainerID", newContainerID);
                    objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNo);
                    objPrm[2] = cmdi.Parameters.Add("TextDriverName", TextDriverName.Text);
                    objPrm[3] = cmdi.Parameters.Add("TextContainerNo", TextContainerNo.Text);
                    objPrm[4] = cmdi.Parameters.Add("NoContainerSizeID", ContainerSizeID);
                    objPrm[5] = cmdi.Parameters.Add("TextItemWtWb", TextItemWtWb.Text);
                    objPrm[6] = cmdi.Parameters.Add("TextTareWeight", TextTareWeight.Text);
                    objPrm[7] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[8] = cmdi.Parameters.Add("c_date", c_date);
                    objPrm[9] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[10] = cmdi.Parameters.Add("TextIsActive", ISActive);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();

                    DataTable dtWbSlipID = new DataTable();
                    DataSet dse = new DataSet();
                    string makeSrSQL = " SELECT WB_SLIP_NO FROM WP_EXPORT_WBSLIP_CON WHERE IS_ACTIVE = 'Enable' AND EXPORT_INVOICE_NO IS NULL ORDER BY WB_SLIP_NO DESC";
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
                int BalesNumber = Convert.ToInt32(TextBalesNumber.Text);
                string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                double ItemWeightEx = Convert.ToDouble(TextItemWeightEx.Text);

                string get_id = "select WP_EXPORT_WBS_CON_ITEMID_SEQ.nextval from dual";
                cmdsp = new OracleCommand(get_id, conn);
                int newExWbID = Int32.Parse(cmdsp.ExecuteScalar().ToString());
                string insert_data = "insert into  WP_EXPORT_WBSLIP_CON_ITEM (EXP_WBCON_ITEM_ID, WB_SLIP_NO, CATEGORY_ID, ITEM_ID, ITEM_WS_ID, ITEM_WEIGHT, ITEM_BALES, IS_INVENTORY_STATUS, CREATE_DATE, C_USER_ID) values  ( :NoExpWbConID, :NoSlipID, :NoCategoryID, :NoItemID, :NoItemWsID, :TextItemWeightEx, :NoBalesNumber, :TextInvenStatus, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                cmdi = new OracleCommand(insert_data, conn);

                OracleParameter[] objPrm = new OracleParameter[10];
                objPrm[0] = cmdi.Parameters.Add("NoExpWbConID", newExWbID);
                objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNo);
                objPrm[2] = cmdi.Parameters.Add("NoCategoryID", CategoryID);
                objPrm[3] = cmdi.Parameters.Add("NoItemID", ItemID);
                objPrm[4] = cmdi.Parameters.Add("NoItemWsID", ItemWsID);
                objPrm[5] = cmdi.Parameters.Add("TextItemWeightEx", ItemWeightEx);
                objPrm[6] = cmdi.Parameters.Add("NoBalesNumber", BalesNumber);
                objPrm[7] = cmdi.Parameters.Add("TextInvenStatus", "Transit");
                objPrm[8] = cmdi.Parameters.Add("c_date", c_date);
                objPrm[9] = cmdi.Parameters.Add("NoCuserID", userID);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();


                string update_user = "update  WP_EXPORT_WBSLIP_CON  set IS_EDIT =:TextIsEdit where WB_SLIP_NO =:NoSlipNo ";
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
            string query = " SELECT ITEM_WS_ID, ITEM_WS_DESCRIPTION FROM WP_WS_ITEM WHERE ITEM_ID = :ItemID AND IS_ACTIVE = 'Enable' ORDER BY ITEM_WS_ID ASC ";
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
                string makeSQL = " SELECT PEWC.EXP_WBCON_ID, PEWC.WB_SLIP_NO, PEWC.DRIVER_NAME, PEWC.CONTAINER_NO, PEWC.REL_ORDER_NO, nvl(PEWC.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, TARE_WEIGHT, PI.ITEM_NAME, WWI.ITEM_WS_DESCRIPTION, PEWCI.ITEM_WEIGHT, PEWCI.ITEM_BALES FROM WP_EXPORT_WBSLIP_CON PEWC LEFT JOIN WP_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN WP_WS_ITEM WWI ON WWI.ITEM_WS_ID = PEWCI.ITEM_WS_ID  WHERE PEWC.WB_SLIP_NO = '" + SlipWbNo + "' ORDER BY PI.ITEM_ID ";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                string ContainerNo = "";
                for (int i = 0; i < 1; i++)
                {
                    ContainerNo = dt.Rows[i]["CONTAINER_NO"].ToString();
                    string DriverName = dt.Rows[i]["DRIVER_NAME"].ToString();
                    RelOrderNo = dt.Rows[i]["REL_ORDER_NO"].ToString();
                    HtmlString += "<div style='float:left;width:785px;height:258px;margin-top:220px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 16px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";
                    HtmlString += "<div style='float:left;width:470px;'> ";
                    HtmlString += "<div style='float:left;width:260px;height:38px;margin-left:192px;padding-top:65px;' > " + DriverName + " </div> ";

                }
                int m = 1;
                HtmlString += "<div style='float:left;width:300px;height:28px;margin:0px 0 0 192px;'> ";
                for (int i = 0; i < RowCount; i++)
                {
                    string ItemName = dt.Rows[i]["ITEM_NAME"].ToString();
                    string ItemNameWs = dt.Rows[i]["ITEM_WS_DESCRIPTION"].ToString();
                    ItemWbWeight = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString());
                    TareWeight = Convert.ToDouble(dt.Rows[i]["TARE_WEIGHT"].ToString());
                    TotalBales += Convert.ToDouble(dt.Rows[i]["ITEM_BALES"].ToString());
                    TotalItemWt += Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());

                    if (m == RowCount)
                    {
                        HtmlString += "" + ItemNameWs + " ";  // ("+ ItemName + ")
                    }
                    else { HtmlString += "" + ItemName + " , "; } // (" + ItemName + ")
                    m++;
                }
                HtmlString += "</div> ";

                HtmlString += "<div style='float:left;width:260px;height:20px;margin-left:187px;padding:10px 0 0 5px;'> ";
                HtmlString += "" + ContainerNo + "";
                HtmlString += "</div> ";

                HtmlString += "<div style='float:left;width:260px;height:20px;margin-left:187px;padding:18px 0 0 5px;'> ";
                int n = 1;
                for (int i = 0; i < RowCount; i++)
                {
                    double BalesNumber = Convert.ToDouble(dt.Rows[i]["ITEM_BALES"].ToString());

                    if (n == RowCount)
                    {
                        HtmlString += "" + BalesNumber + " Bales";
                    }
                    else { HtmlString += "" + BalesNumber + " Bales, "; }
                    n++;
                }

                HtmlString += "</div> ";



                HtmlString += "</div> ";

                HtmlString += "<div style='float:left;width:310px;'> ";

                HtmlString += "<div style='float:left;width:305px;margin:0 0 0 40px;'> ";
                HtmlString += "<div style='float:left;width:175px;padding:3px;text-align:right;border:black 1px solid;'> Gross Weight (KG)</div> ";
                HtmlString += "<div style='float:left;width:70px;padding:4px;text-align:right;'>" + ItemWbWeight + "</div> ";
                HtmlString += "</div>";

                HtmlString += "<div style='float:left;width:305px;margin:0 0 0 40px;'> ";
                HtmlString += "<div style='float:left;width:175px;padding:3px;text-align:right;border:black 1px solid;'>  Tare Weight (KG)</div> ";
                HtmlString += "<div style='float:left;width:70px;padding:4px;text-align:right;'>" + TareWeight + "</div> ";
                HtmlString += "</div>";

                HtmlString += "<div style='float:left;width:305px;margin:0 0 0 40px;'> ";
                HtmlString += "<div style='float:left;width:175px;padding:3px;text-align:right;border:black 1px solid;'>  Net Weight (KG)</div> ";
                HtmlString += "<div style='float:left;width:70px;padding:4px;text-align:right;'>" + TotalItemWt + "</div> ";
                HtmlString += "</div>";

                HtmlString += "<div style='float:left;width:300px;height:20px;margin:60 0 0 35px;text-align:center'> FOR EXPORTS</div> ";
                HtmlString += "</div>";
                HtmlString += "</div>";

                HtmlString += "</div>";
                HtmlString += "</div>";
                HtmlString += "</div>";
                HtmlString += "</div>";

                // weigh-bridge & container update for print
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string update_user = "update  WP_EXPORT_WBSLIP_CON  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where WB_SLIP_NO = :NoSlipNo ";
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
        public static List<ListItem> GetItemFinalStock(int ItemId)
        {
            string query = " SELECT WRSM.ITEM_ID, ROUND(nvl(WRSM.FINAL_STOCK_WT,0) - SUM(nvl(WEWCI.ITEM_WEIGHT,0)),2) AS FINAL_STOCK_WT FROM WP_RM_STOCK_INVENTORY_MASTER WRSM LEFT JOIN WP_EXPORT_WBSLIP_CON_ITEM WEWCI ON WEWCI.ITEM_ID = WRSM.ITEM_ID AND IS_INVENTORY_STATUS = 'Transit' where WRSM.ITEM_ID =:ItemID GROUP BY WRSM.ITEM_ID, WRSM.FINAL_STOCK_WT ";
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
            string query = " SELECT * FROM WP_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC ";
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

            string makeSQL = " select EXP_WBCON_ID, WB_SLIP_NO, DRIVER_NAME, CONTAINER_NO, CONTAINER_SIZE_ID, nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, nvl(TARE_WEIGHT,0) AS TARE_WEIGHT, TO_CHAR(DISPATCH_DATE,'dd/mm/yyyy') AS DISPATCH_DATE, IS_ACTIVE from WP_EXPORT_WBSLIP_CON where WB_SLIP_NO  = '" + USER_DATA_ID + "'";
            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;

            for (int i = 0; i < RowCount; i++)
            {
                TextExWbConID.Text = dt.Rows[i]["EXP_WBCON_ID"].ToString();
                TextSlipWbNo.Text = dt.Rows[i]["WB_SLIP_NO"].ToString();
                TextDriverName.Text = dt.Rows[i]["DRIVER_NAME"].ToString();
                TextContainerNo.Text = dt.Rows[i]["CONTAINER_NO"].ToString();
                DropDownContainerSizeID.Text = dt.Rows[i]["CONTAINER_SIZE_ID"].ToString();
                EntryDate.Text = dt.Rows[i]["DISPATCH_DATE"].ToString();
                TextItemWtWb.Text = dt.Rows[i]["ITEM_WEIGHT_WB"].ToString();
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
            string makeSriSQL = " SELECT WB_SLIP_NO FROM WP_EXPORT_WBSLIP_CON WHERE IS_ACTIVE = 'Enable' AND EXPORT_INVOICE_NO IS NULL ORDER BY WB_SLIP_NO DESC";
            dse = ExecuteBySqlString(makeSriSQL);
            dtWbSlipID = (DataTable)dse.Tables[0];
            DropDownSlipID.DataSource = dtWbSlipID;
            DropDownSlipID.DataValueField = "WB_SLIP_NO";
            DropDownSlipID.DataTextField = "WB_SLIP_NO";
            DropDownSlipID.DataBind();
            DropDownSlipID.Items.Insert(0, new ListItem("Select  Weight Slip", "0"));

            string makeSQL = " select EXP_WBCON_ITEM_ID, WB_SLIP_NO, ITEM_ID, ITEM_WS_ID, CATEGORY_ID, ITEM_WEIGHT, ITEM_BALES from WP_EXPORT_WBSLIP_CON_ITEM where EXP_WBCON_ITEM_ID  = '" + USER_DATA_ID + "'";
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

                DataTable dtItemWsID = new DataTable();
                DataSet dsis = new DataSet();
                string makeDropDownItemWsSQL = " SELECT ITEM_WS_ID, ITEM_WS_DESCRIPTION FROM WP_WS_ITEM WHERE ITEM_ID = '"+ Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString()) + "' AND IS_ACTIVE = 'Enable' ORDER BY ITEM_WS_ID ASC ";
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
                TextBalesNumber.Text = dt.Rows[i]["ITEM_BALES"].ToString();
                TextItemWeightEx.Text = dt.Rows[i]["ITEM_WEIGHT"].ToString();

                //  string USER_DATA_ID = DropDownSlipID.Text;
                string makeSrSQL = " select ITEM_WEIGHT_WB from WP_EXPORT_WBSLIP_CON where WB_SLIP_NO  = '" + dt.Rows[i]["WB_SLIP_NO"].ToString() + "'";
                cmdl = new OracleCommand(makeSrSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                for (int j = 0; j < RowCount; j++)
                {
                    TextItemWeightWBEx.Text = dt.Rows[j]["ITEM_WEIGHT_WB"].ToString();
                }

            }

            conn.Close();
            Display();
            CheckSlipNo.Text = "";
            alert_box.Visible = false;
            TextItemWeightEx.Enabled = true;
            TextBalesNumber.Enabled = true;
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

        protected void GetItemWtWb(object sender, EventArgs e)
        {
            try
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string USER_DATA_ID = DropDownSlipID.Text;
                double ItemWeightWBEx = 0.0, ItemWeight = 0.0, TareWeight = 0.0;
                string makeSQL = " SELECT nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, nvl(TARE_WEIGHT,0) AS TARE_WEIGHT from WP_EXPORT_WBSLIP_CON where WB_SLIP_NO  = '" + USER_DATA_ID + "'";
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

                string makeSrSQL = " SELECT nvl(sum(ITEM_WEIGHT),0) AS ITEM_WEIGHT from WP_EXPORT_WBSLIP_CON_ITEM where WB_SLIP_NO  = '" + USER_DATA_ID + "'";
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
                TextItemWeightWBEx.Text = Convert.ToString(ItemWeightWBEx - (TareWeight + ItemWeight));
                TextItemWeightEx.Text = Convert.ToString(ItemWeightWBEx - (TareWeight + ItemWeight));

                if ((ItemWeightWBEx - (TareWeight + ItemWeight)) <= 0)
                {
                    BtnAddItem.Attributes.Add("aria-disabled", "false");
                    BtnAddItem.Attributes.Add("class", "btn btn-primary disabled");
                    DropDownCategoryID.Enabled = false;
                    TextItemWeightEx.Enabled = false;
                    TextBalesNumber.Enabled = false;
                }
                else
                {
                    BtnAddItem.Attributes.Add("aria-disabled", "true");
                    BtnAddItem.Attributes.Add("class", "btn btn-primary active");
                    DropDownCategoryID.Enabled = true;
                    TextItemWeightEx.Enabled = true;
                    TextBalesNumber.Enabled = true;
                }


                clearText();
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
                    makeSQL = " SELECT PEWC.WB_SLIP_NO, PEWC.DRIVER_NAME, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.REL_ORDER_NO, PEWC.IS_EDIT, PEWCI.EXP_WBCON_ITEM_ID, PI.ITEM_NAME, WWI.ITEM_WS_DESCRIPTION, PEWC.ITEM_WEIGHT_WB, PEWC.TARE_WEIGHT, PEWCI.ITEM_WEIGHT, PEWCI.ITEM_BALES, PEWC.IS_ACTIVE, PEWCI.IS_INVENTORY_STATUS, PEWCI.IS_ACTIVE_PRICING, PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT, TO_CHAR(PEWC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE,  PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO  FROM WP_EXPORT_WBSLIP_CON PEWC LEFT JOIN WP_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO  LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN WP_WS_ITEM WWI ON WWI.ITEM_WS_ID = PEWCI.ITEM_WS_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE PEWCI.IS_INVENTORY_STATUS != 'Complete'  ORDER BY PEWC.WB_SLIP_NO desc, PEWCI.ITEM_ID  ASC "; //WHERE to_char(PEWC.DISPATCH_DATE, 'mm/yyyy') = '" + MonthYear + "' OR IS_INVENTORY_STATUS = 'Transit'
                }
                else
                {
                    if (DropDownIsInven.Text == "0")
                    {
                        makeSQL = "  SELECT PEWC.WB_SLIP_NO, PEWC.DRIVER_NAME, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.REL_ORDER_NO, PEWC.IS_EDIT, PEWCI.EXP_WBCON_ITEM_ID, PI.ITEM_NAME, WWI.ITEM_WS_DESCRIPTION, PEWC.ITEM_WEIGHT_WB, PEWC.TARE_WEIGHT, PEWCI.ITEM_WEIGHT, PEWCI.ITEM_BALES, PEWC.IS_ACTIVE, PEWCI.IS_INVENTORY_STATUS, PEWCI.IS_ACTIVE_PRICING, PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT, TO_CHAR(PEWC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE,  PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO  FROM WP_EXPORT_WBSLIP_CON PEWC LEFT JOIN WP_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO  LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN WP_WS_ITEM WWI ON WWI.ITEM_WS_ID = PEWCI.ITEM_WS_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE PEWC.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or PEWC.CONTAINER_NO like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%'  or to_char(PEWC.DISPATCH_DATE, 'dd/mm/yyyy')  like '" + txtSearchEmp.Text + "%' or PEWC.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ORDER BY PEWC.WB_SLIP_NO desc, PEWCI.ITEM_ID asc ";
                    }
                    else
                    {
                        makeSQL = "  SELECT PEWC.WB_SLIP_NO, PEWC.DRIVER_NAME, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.REL_ORDER_NO, PEWC.IS_EDIT, PEWCI.EXP_WBCON_ITEM_ID, PI.ITEM_NAME, WWI.ITEM_WS_DESCRIPTION, PEWC.ITEM_WEIGHT_WB, PEWC.TARE_WEIGHT, PEWCI.ITEM_WEIGHT, PEWCI.ITEM_BALES, PEWC.IS_ACTIVE, PEWCI.IS_INVENTORY_STATUS, PEWCI.IS_ACTIVE_PRICING, PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT, TO_CHAR(PEWC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE,  PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO  FROM WP_EXPORT_WBSLIP_CON PEWC LEFT JOIN WP_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO  LEFT JOIN WP_ITEM PI ON PI.ITEM_ID = PEWCI.ITEM_ID LEFT JOIN WP_WS_ITEM WWI ON WWI.ITEM_WS_ID = PEWCI.ITEM_WS_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE PEWCI.IS_INVENTORY_STATUS = '" + DropDownIsInven.Text + "' AND (PEWC.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or PEWC.CONTAINER_NO like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or to_char(PEWC.DISPATCH_DATE, 'dd/mm/yyyy')  like '" + txtSearchEmp.Text + "%' or PEWC.IS_ACTIVE like '" + txtSearchEmp.Text + "%') ORDER BY PEWC.WB_SLIP_NO desc, PEWCI.ITEM_ID asc ";
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
                    makeSQL = " SELECT WI.ITEM_NAME, nvl(sum(PEWCI.ITEM_WEIGHT), 0) AS ITEM_WEIGHT, SUM(PEWCI.ITEM_BALES) AS ITEM_BALES FROM WP_EXPORT_WBSLIP_CON PEWC LEFT JOIN WP_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN WP_ITEM WI ON WI.ITEM_ID = PEWCI.ITEM_ID WHERE PEWCI.IS_INVENTORY_STATUS = 'Transit' AND WI.ITEM_NAME IS NOT NULL GROUP BY WI.ITEM_ID, WI.ITEM_NAME ORDER BY WI.ITEM_ID";
                }
                else
                {
                    makeSQL = " SELECT WI.ITEM_NAME, nvl(sum(PEWCI.ITEM_WEIGHT), 0) AS ITEM_WEIGHT, SUM(PEWCI.ITEM_BALES) AS ITEM_BALES FROM WP_EXPORT_WBSLIP_CON PEWC LEFT JOIN WP_EXPORT_WBSLIP_CON_ITEM PEWCI ON PEWCI.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN WP_ITEM WI ON WI.ITEM_ID = PEWCI.ITEM_ID WHERE PEWCI.IS_INVENTORY_STATUS = '" + DropDownSearchSummary.Text + "' AND WI.ITEM_NAME IS NOT NULL GROUP BY WI.ITEM_ID, WI.ITEM_NAME ORDER BY WI.ITEM_ID";

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
                    decimal total_bales = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_BALES"));
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

                string update_user = "update  WP_EXPORT_WBSLIP_CON  set WB_SLIP_NO =:NoSlipNo, DRIVER_NAME =:TextDriverName, CONTAINER_NO =:NoContainerNo, CONTAINER_SIZE_ID =:NoContainerSize, ITEM_WEIGHT_WB =:NoItemWeightWb, TARE_WEIGHT =:TextTareWeight, DISPATCH_DATE = TO_DATE(:DispatchDate, 'DD/MM/YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE =:TextIsActive where EXP_WBCON_ID =:NoExWbConID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[11];
                objPrm[0] = cmdi.Parameters.Add("NoSlipNo", TextSlipWbNo.Text);
                objPrm[1] = cmdi.Parameters.Add("TextDriverName", TextDriverName.Text);
                objPrm[2] = cmdi.Parameters.Add("NoContainerNo", TextContainerNo.Text);
                objPrm[3] = cmdi.Parameters.Add("NoContainerSize", ContainerSizeID);
                objPrm[4] = cmdi.Parameters.Add("NoItemWeightWb", TextItemWtWb.Text);
                objPrm[5] = cmdi.Parameters.Add("TextTareWeight", TextTareWeight.Text);
                objPrm[6] = cmdi.Parameters.Add("DispatchDate", EntryDateNew);
                objPrm[7] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[8] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[9] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[10] = cmdi.Parameters.Add("NoExWbConID", ExWbConID);

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
                int BalesNumber = Convert.ToInt32(TextBalesNumber.Text);
                double ItemWeightEx = Convert.ToDouble(TextItemWeightEx.Text);

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  WP_EXPORT_WBSLIP_CON_ITEM  set ITEM_ID =:NoItemID, ITEM_WS_ID =:NoItemWsID, CATEGORY_ID =:NoCategoryID, ITEM_WEIGHT =:TextItemWeightEx, ITEM_BALES =:NoBalesNumber, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where EXP_WBCON_ITEM_ID = :NoExpWbConItemID ";
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
                string delete_user = " delete from WP_EXPORT_WBSLIP_CON_ITEM where EXP_WBCON_ITEM_ID  = '" + ExpWbConItemID + "'";
                cmdi = new OracleCommand(delete_user, conn);
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();


                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select WB_SLIP_NO from WP_EXPORT_WBSLIP_CON_ITEM where WB_SLIP_NO = '" + SlipNo + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (!dr.HasRows)
                {
                    string update_user = "update  WP_EXPORT_WBSLIP_CON  set IS_EDIT =:TextIsEdit where WB_SLIP_NO =:NoSlipNo ";
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
                    string delete_user = " delete from WP_EXPORT_WBSLIP_CON where EXP_WBCON_ID  = '" + ExWbConID + "'";
                    cmdi = new OracleCommand(delete_user, conn);
                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    string delete_item = " delete from WP_EXPORT_WBSLIP_CON_ITEM where WB_SLIP_NO  = '" + SlipNo + "'";
                    cmdu = new OracleCommand(delete_item, conn);
                    cmdu.ExecuteNonQuery();
                    cmdu.Parameters.Clear();
                    cmdu.Dispose();

                    conn.Close();

                    DataTable dtWbSlipID = new DataTable();
                    DataSet dse = new DataSet();
                    string makeSrSQL = " SELECT WB_SLIP_NO FROM WP_EXPORT_WBSLIP_CON WHERE IS_ACTIVE = 'Enable' ORDER BY WB_SLIP_NO DESC";
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
            TextDriverName.Text = "";
            TextContainerNo.Text = "";
            CheckSlipNo.Text = "";
            TextItemWtWb.Text = "";
            TextBalesNumber.Text = "";
            EntryDate.Text = "";
            TextTareWeight.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        {
            TextSlipWbNo.Text = "";
            TextDriverName.Text = "";
            TextContainerNo.Text = "";
            CheckSlipNo.Text = "";
            TextItemWtWb.Text = "";
            TextBalesNumber.Text = "";
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
            TextItemWeightWBEx.Text = "";
            TextBalesNumber.Text = "";
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
                string MatchEmpIDPattern = "^([0-9]{4})$";
                if (SlipNo != null)
                {

                    if (Regex.IsMatch(SlipNo, MatchEmpIDPattern))
                    {
                        alert_box.Visible = false;

                        OracleConnection conn = new OracleConnection(strConnString);
                        conn.Open();
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = "select WB_SLIP_NO from WP_EXPORT_WBSLIP_CON where WB_SLIP_NO = '" + SlipNo + "'";
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
                            TextDriverName.Focus();
                            BtnAdd.Attributes.Add("aria-disabled", "true");
                            BtnAdd.Attributes.Add("class", "btn btn-primary active");

                        }
                    }
                    else
                    {
                        CheckSlipNo.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter Weight Slip Number is 4 digit only</label>";
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