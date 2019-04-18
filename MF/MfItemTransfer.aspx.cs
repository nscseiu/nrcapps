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
using System.Web.Services;
using System.Collections.Generic;

namespace NRCAPPS.MF
{
    public partial class MfItemTransfer : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdi, cmdl, cmdsp;
        OracleDataAdapter oradata; 
        DataTable dt;
        int RowCount;
          
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


                        DataTable dtID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeVehicleSQL = " SELECT * FROM MF_VEHICLE  WHERE IS_ACTIVE = 'Enable' ORDER BY VEHICLE_NO ASC";
                        ds = ExecuteBySqlString(makeVehicleSQL);
                        dtID = (DataTable)ds.Tables[0];
                        DropDownVehicleID.DataSource = dtID;
                        DropDownVehicleID.DataValueField = "VEHICLE_ID";
                        DropDownVehicleID.DataTextField = "VEHICLE_NO";
                        DropDownVehicleID.DataBind();
                        DropDownVehicleID.Items.Insert(0, new ListItem("Select Vehicle Number ", "0"));

                        DataTable dtSlipNoID = new DataTable();
                        DataSet dss = new DataSet();
                        string makedtSlipNoIDSQL = " SELECT * FROM MS_SALES_INTER_DIV_MASTER  WHERE IS_ACTIVE = 'Enable' AND IS_MF_RECEIVED_STATUS IS NULL AND INTER_DIVISION_ID = '5' ORDER BY WB_SLIP_NO DESC";
                        dss = ExecuteBySqlString(makedtSlipNoIDSQL);
                        dtSlipNoID = (DataTable)dss.Tables[0];
                        DropDownMsWbSlipNo.DataSource = dtSlipNoID;
                        DropDownMsWbSlipNo.DataValueField = "WB_SLIP_NO";
                        DropDownMsWbSlipNo.DataTextField = "WB_SLIP_NO";
                        DropDownMsWbSlipNo.DataBind();
                        DropDownMsWbSlipNo.Items.Insert(0, new ListItem("Select MS Weight Slip No", "0"));  
                          
                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeItemSQL = " SELECT ITEM_ID, ITEM_NAME || ' - ' || ITEM_CODE AS ITEM_FULL_NAME FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' AND IS_TRANSFER_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        dsi = ExecuteBySqlString(makeItemSQL);
                        dtItemID = (DataTable)dsi.Tables[0];
                        DropDownItemID.DataSource = dtItemID;
                        DropDownItemID.DataValueField = "ITEM_ID";
                        DropDownItemID.DataTextField = "ITEM_FULL_NAME";
                        DropDownItemID.DataBind();
                        DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));

                        DropDownItemID1.DataSource = dtItemID;
                        DropDownItemID1.DataValueField = "ITEM_ID";
                        DropDownItemID1.DataTextField = "ITEM_FULL_NAME";
                        DropDownItemID1.DataBind();
                        DropDownItemID1.Items.Insert(0, new ListItem("Select  Item", "0"));

                        /*      DataTable dtItemBinID = new DataTable();
                              DataSet dsb = new DataSet();
                              string makeItemBinSQL = " SELECT ITEM_BIN_ID, ITEM_BIN_NAME || ' - Capacity: ' || CAPACITY_WEIGHT || ' (KG)'  AS ITEM_FULL_NAME FROM MF_ITEM_BIN WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_BIN_ID ASC";
                              dsb = ExecuteBySqlString(makeItemBinSQL);
                              dtItemBinID = (DataTable)dsb.Tables[0];
                              DropDownItemBinID.DataSource = dtItemBinID;
                              DropDownItemBinID.DataValueField = "ITEM_BIN_ID";
                              DropDownItemBinID.DataTextField = "ITEM_FULL_NAME";
                              DropDownItemBinID.DataBind();
                              DropDownItemBinID.Items.Insert(0, new ListItem("Select  Item Bin", "0")); */

                        DropDownMsWbSlipNo.Focus();
                        Text1stWeightMs.Attributes.Add("readonly", "readonly");
                        Text2ndWeightMs.Attributes.Add("readonly", "readonly");
                        TextMatWeightMs.Attributes.Add("readonly", "readonly"); 
                        TextMatWeightMf.Attributes.Add("readonly", "readonly"); 
                        TextWeightVariance.Enabled = false;
                        DropDownItemBinID.Enabled = false;
                        DropDownItemID.Enabled = false;

                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled");

                        Display();
                        DisplaySummary();
                        PendingReceiving();
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
        // try { 
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    int VehicleID = Convert.ToInt32(DropDownVehicleID.Text);
                    int SlipNo = Convert.ToInt32(TextMfWbSlipNo.Text); 
                    int ItemID   = Convert.ToInt32(DropDownItemID.Text);  
                    int ItemBinID = Convert.ToInt32(Request.Form[DropDownItemBinID.UniqueID]);
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                     
                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-'); 
                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-"); 
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                     
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                    double Mat1stWeightMf = Convert.ToDouble(TextMat1stWeightMf.Text.Trim());
                    double Mat2ndWeightMf = Convert.ToDouble(TextMat2ndWeightMf.Text.Trim());
                    double MatWeightMf = Convert.ToDouble(TextMatWeightMf.Text.Trim());
                    double MatWeightMs = Convert.ToDouble(TextMatWeightMs.Text.Trim());
                    double WeightVariance = MatWeightMf - MatWeightMs;
                     
                    string get_transfer_id = "select MF_PURCHASE_TRANS_MASTERID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_transfer_id, conn);
                    int newTransferID = Int16.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_Transfer = "insert into  MF_PURCHASE_TRANSFER_MASTER (TRANSFER_ID, WB_SLIP_NO, VEHICLE_ID, ITEM_ID, ITEM_BIN_ID, FIRST_WT_MF, SECOND_WT_MF, NET_WT_MF, NET_WT_MS, VARIANCE, REMARKS, WB_SLIP_NO_MS, ENTRY_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID) values  ( :NoTransferID, :NoSlipID, :NoVehicleID, :NoItemID, :NoItemBinID, :TextMat1stWeightMf, :TextMat2ndWeightMf, :TextMatWeightMf, :TextMatWeightMs, :TextWeightVariance, :TextRemarks, :TextMsWbSlipNo, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 5)";
                    cmdi = new OracleCommand(insert_Transfer, conn);
                      
                    OracleParameter[] objPrm = new OracleParameter[16];
                    objPrm[0] = cmdi.Parameters.Add("NoTransferID", newTransferID); 
                    objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNo); 
                    objPrm[2] = cmdi.Parameters.Add("NoVehicleID", VehicleID);  
                    objPrm[3] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[4] = cmdi.Parameters.Add("NoItemBinID", ItemBinID);
                    objPrm[5] = cmdi.Parameters.Add("TextMat1stWeightMf", Mat1stWeightMf);
                    objPrm[6] = cmdi.Parameters.Add("TextMat2ndWeightMf", Mat2ndWeightMf);
                    objPrm[7] = cmdi.Parameters.Add("TextMatWeightMf", MatWeightMf);
                    objPrm[8] = cmdi.Parameters.Add("TextMatWeightMs", MatWeightMs);
                    objPrm[9] = cmdi.Parameters.Add("TextWeightVariance", WeightVariance); 
                    objPrm[10] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[11] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text); 
                    objPrm[12] = cmdi.Parameters.Add("TextMsWbSlipNo", DropDownMsWbSlipNo.Text); 
                    objPrm[13] = cmdi.Parameters.Add("c_date", c_date); 
                    objPrm[14] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[15] = cmdi.Parameters.Add("TextIsActive", ISActive);

                    cmdi.ExecuteNonQuery(); 
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    string update_user = "update  MS_SALES_INTER_DIV_MASTER  set IS_MF_RECEIVED_STATUS = :IsReceived, MF_RECEIVED_ID = :NoCuserID, MF_RECEIVED_DATE = TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM') where WB_SLIP_NO = :TextMsWbSlipNo ";
                    cmdi = new OracleCommand(update_user, conn);

                    OracleParameter[] objPr = new OracleParameter[4];
                    objPr[0] = cmdi.Parameters.Add("IsReceived", "Received"); 
                    objPr[1] = cmdi.Parameters.Add("c_date", c_date);
                    objPr[2] = cmdi.Parameters.Add("NoCuserID", userID); 
                    objPr[3] = cmdi.Parameters.Add("TextMsWbSlipNo", DropDownMsWbSlipNo.Text);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    conn.Close(); 

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Transfer Data successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                     
                    clearText();
                    TextMfWbSlipNo.Focus();
                    Display();
                    DisplaySummary();
                    PendingReceiving();
            }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
            //     }
            //   catch
            //   {
            //       Response.Redirect("~/ParameterError.aspx");
            //   } 
              }


        protected void btnPrint_Click(object sender, EventArgs e)
        {
            if (IS_PRINT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                alert_box.Visible = false;
                string HtmlString = "";
                LinkButton btn = (LinkButton)sender;
                Session["user_data_id"] = btn.CommandArgument;
                string SlipNo = Session["user_data_id"].ToString();

                string makeSQL = "   SELECT MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME, MSIDM.WB_SLIP_NO AS WB_SLIP_NO_MS, MSIDM.FIRST_WT, MSIDM.SECOND_WT, MSIDM.ITEM_WEIGHT, MITM.WB_SLIP_NO AS WB_SLIP_NO_MF, MITM.FIRST_WT_MF, MITM.SECOND_WT_MF, MITM.NET_WT_MF, MITM.VARIANCE,  TO_CHAR(TO_DATE(MITM.ENTRY_DATE), 'dd-MON-YYYY') AS ENTRY_DATE, MIB.ITEM_BIN_NAME, MV.VEHICLE_NO FROM MF_PURCHASE_TRANSFER_MASTER MITM LEFT JOIN MS_SALES_INTER_DIV_MASTER MSIDM ON MSIDM.WB_SLIP_NO = MITM.WB_SLIP_NO_MS LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MITM.ITEM_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MITM.ITEM_BIN_ID LEFT JOIN MF_VEHICLE MV ON MV.VEHICLE_ID = MITM.VEHICLE_ID WHERE MITM.WB_SLIP_NO =  '" + SlipNo + "'";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count; 
                for (int i = 0; i < 1; i++)
                { 
                    HtmlString += "<div style='float:left;width:720px;height:258px;margin:342px 0 0 35px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 12px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;'> ";


                    HtmlString += "<table cellpadding='2px' cellspacing='0' style='font-size: 13px;' width='95%' align='center'>";
                    HtmlString += "<tr valign='top'> ";
                    HtmlString += "<th colspan=4 style='font-size: 14px;text-decoration: underline;'>Material Transfer From Metal Scrap</th> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>Material: " + dt.Rows[i]["ITEM_NAME"].ToString() + "</td> "; 
                    HtmlString += "<td colspan=2 style='text-align:right'>Transfer Date: " + dt.Rows[i]["ENTRY_DATE"].ToString() + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td></td> ";
                    HtmlString += "<td style='border-bottom:black solid 1px;text-align:center'>Metal Scrap</td> ";
                    HtmlString += "<td style='border-bottom:black solid 1px;text-align:center'>Metal Factory</td> "; 
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='text-align:right'>Weight Slip No.:</td> ";
                    HtmlString += "<td style='text-align:center'>" + dt.Rows[i]["WB_SLIP_NO_MS"].ToString() + "</td> ";
                    HtmlString += "<td style='text-align:center'>" + dt.Rows[i]["WB_SLIP_NO_MF"].ToString() + "</td> "; 
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> "; 
                    HtmlString += "<td style='text-align:right'>1st Weight (KG):</td> ";
                    HtmlString += "<td style='text-align:right'>" + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["FIRST_WT"].ToString())) + "</td> "; 
                    HtmlString += "<td style='text-align:right'>" + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["FIRST_WT_MF"].ToString())) + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> "; 
                    HtmlString += "<td style='border-bottom:black solid 1px;text-align:right'>2nd Weight (KG):</td> "; 
                    HtmlString += "<td style='border-bottom:black solid 1px;text-align:right'>" + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["SECOND_WT"].ToString())) + "</td> ";
                    HtmlString += "<td style='border-bottom:black solid 1px;text-align:right'>" + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["SECOND_WT_MF"].ToString())) + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td style='font-weight: 700;text-align:right'>Net Weight (KG):</td> ";
                    HtmlString += "<td style='font-weight: 700;text-align:right'>" + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString())) + "</td> ";
                    HtmlString += "<td style='font-weight: 700;text-align:right'>" + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["NET_WT_MF"].ToString())) + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td></td> ";
                    HtmlString += "<td  colspan=2 style='text-align:center'>Variance (KG): " + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["VARIANCE"].ToString())) + "</td> "; 
                    HtmlString += "</tr> ";

                    HtmlString += "<tr valign='top'> ";
                    HtmlString += "<th colspan=4 style='font-weight: 700;'></th> ";
                    HtmlString += "</tr> ";
                    HtmlString += "</table> ";


                    HtmlString += "</div>";
                }

                // purchase master update for print
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string update_user = "update  MF_PURCHASE_TRANSFER_MASTER  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where WB_SLIP_NO = :NoSlipNo ";
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
                PendingReceiving();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }

     
        [WebMethod]
        public static Boolean MsSlipNoCheck(string MsSlipNo)
        {
            Boolean result = false;
            string query = "select WB_SLIP_NO from MF_PURCHASE_TRANSFER_MASTER where WB_SLIP_NO = '" + MsSlipNo + "'";
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
            string query = " SELECT ITEM_BIN_ID, ITEM_BIN_NAME || ' - Capacity: ' || CAPACITY_WEIGHT || ' (KG)'  AS ITEM_FULL_NAME FROM MF_ITEM_BIN WHERE IS_ACTIVE = 'Enable' AND ITEM_ID = :ItemID  ORDER BY ITEM_BIN_ID ASC ";
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
                                Value = sdr["ITEM_BIN_ID"].ToString(),
                                Text = sdr["ITEM_FULL_NAME"].ToString()
                            });
                        }
                    }
                    conn.Close();
                    return cities;
                }
            }
        }

        protected void GetMsData(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open(); 
            string MsWbSlipNo = DropDownMsWbSlipNo.Text; 
            string makeSQL = " select  ITEM_ID, WB_SLIP_NO, FIRST_WT, SECOND_WT, ITEM_WEIGHT from MS_SALES_INTER_DIV_MASTER where WB_SLIP_NO  = '" + MsWbSlipNo + "'"; 
            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();
                Text1stWeightMs.Text = dt.Rows[i]["FIRST_WT"].ToString();
                Text2ndWeightMs.Text = dt.Rows[i]["SECOND_WT"].ToString();
                TextMatWeightMs.Text = dt.Rows[i]["ITEM_WEIGHT"].ToString();  
            }
            TextMfWbSlipNo.Focus();
            conn.Close();
          
            CheckSlipNo.Text = "";
            alert_box.Visible = false; 

            
        }

        protected void linkSelectClick(object sender, EventArgs e)
        {
        //  try{
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
            int USER_DATA_ID = Convert.ToInt32(Session["user_data_id"]);
                 
            string makeSQL = " select TRANSFER_ID, WB_SLIP_NO,  VEHICLE_ID, ITEM_ID, ITEM_BIN_ID, FIRST_WT_MF, SECOND_WT_MF, NET_WT_MF, NET_WT_MS, VARIANCE, WB_SLIP_NO_MS, REMARKS,  TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, CREATE_DATE,  UPDATE_DATE,  C_USER_ID, U_USER_ID, IS_ACTIVE from MF_PURCHASE_TRANSFER_MASTER where TRANSFER_ID  = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                DataTable dtItemBinID = new DataTable();
                DataSet dsb = new DataSet();
                string makeItemBinSQL = " SELECT ITEM_BIN_ID, ITEM_BIN_NAME || ' - Capacity: ' || CAPACITY_WEIGHT || ' (KG)'  AS ITEM_FULL_NAME FROM MF_ITEM_BIN WHERE IS_ACTIVE = 'Enable' AND ITEM_ID = '"+ Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString()) + "' ORDER BY ITEM_BIN_ID ASC";
                dsb = ExecuteBySqlString(makeItemBinSQL);
                dtItemBinID = (DataTable)dsb.Tables[0];
                DropDownItemBinID.DataSource = dtItemBinID;
                DropDownItemBinID.DataValueField = "ITEM_BIN_ID";
                DropDownItemBinID.DataTextField = "ITEM_FULL_NAME";
                DropDownItemBinID.DataBind();

                TextTransferID.Text = dt.Rows[i]["TRANSFER_ID"].ToString();
                TextMfWbSlipNo.Text = dt.Rows[i]["WB_SLIP_NO"].ToString();
                DropDownVehicleID.Text = dt.Rows[i]["VEHICLE_ID"].ToString();
                DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();
             //   DropDownItemBinID.Text = dt.Rows[i]["ITEM_BIN_ID"].ToString();
                TextMat1stWeightMf.Text = dt.Rows[i]["FIRST_WT_MF"].ToString();
                TextMat2ndWeightMf.Text = dt.Rows[i]["SECOND_WT_MF"].ToString();
                TextMatWeightMf.Text = dt.Rows[i]["NET_WT_MF"].ToString();
                TextMatWeightMs.Text = dt.Rows[i]["NET_WT_MS"].ToString();
                TextWeightVariance.Text = decimal.Parse(dt.Rows[i]["VARIANCE"].ToString()).ToString(".00"); 
                EntryDate.Text = dt.Rows[i]["ENTRY_DATE"].ToString();
                TextRemarks.Text = dt.Rows[i]["REMARKS"].ToString(); 
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);

                DataTable dtSlipNoID = new DataTable();
                DataSet dss = new DataSet();
                string makedtSlipNoIDSQL = " SELECT * FROM MS_SALES_INTER_DIV_MASTER  WHERE IS_ACTIVE = 'Enable' AND INTER_DIVISION_ID = '5' ORDER BY WB_SLIP_NO DESC";
                dss = ExecuteBySqlString(makedtSlipNoIDSQL);
                dtSlipNoID = (DataTable)dss.Tables[0];
                DropDownMsWbSlipNo.DataSource = dtSlipNoID;
                DropDownMsWbSlipNo.DataValueField = "WB_SLIP_NO";
                DropDownMsWbSlipNo.DataTextField = "WB_SLIP_NO";
                DropDownMsWbSlipNo.DataBind();
                DropDownMsWbSlipNo.Items.Insert(0, new ListItem("Select MS Weight Slip No", "0"));
                DropDownMsWbSlipNo.Text = dt.Rows[i]["WB_SLIP_NO_MS"].ToString();

                string makeSQL1 = " select  ITEM_ID, WB_SLIP_NO, FIRST_WT, SECOND_WT, ITEM_WEIGHT from MS_SALES_INTER_DIV_MASTER where WB_SLIP_NO  = '" + dt.Rows[i]["WB_SLIP_NO_MS"].ToString() + "'";
                cmdl = new OracleCommand(makeSQL1);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                for (int j = 0; j < RowCount; j++)
                { 
                    Text1stWeightMs.Text = dt.Rows[j]["FIRST_WT"].ToString();
                    Text2ndWeightMs.Text = dt.Rows[j]["SECOND_WT"].ToString();
                    TextMatWeightMs.Text = dt.Rows[j]["ITEM_WEIGHT"].ToString(); 
                }


        

            }

            conn.Close();
            Display();
            PendingReceiving();
            CheckSlipNo.Text = "";
            alert_box.Visible = false;
            DropDownItemBinID.Enabled = true;
            DropDownMsWbSlipNo.Enabled = true;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

            BtnUpdate.Attributes.Add("aria-disabled", "true");
            BtnUpdate.Attributes.Add("class", "btn btn-success active");
            BtnDelete.Attributes.Add("aria-disabled", "true");
            BtnDelete.Attributes.Add("class", "btn btn-danger active");

         //   }
        //    catch
        //    {
        //     Response.Redirect("~/ParameterError.aspx");
        //  } 
        }


        public void PendingReceiving()
        {
            try
            {
                if (IS_VIEW_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    BatchPending.Text = "";
                    string makeSQL = " SELECT WB_SLIP_NO FROM MS_SALES_INTER_DIV_MASTER  WHERE IS_ACTIVE = 'Enable' AND IS_MF_RECEIVED_STATUS IS NULL AND INTER_DIVISION_ID = '5' ORDER BY WB_SLIP_NO DESC ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;
                    if (RowCount > 0)
                    {
                        for (int i = 0; i < RowCount; i++)
                        {
                            BatchPending.Text += "<span class=\"label label-danger\" >" + dt.Rows[i]["WB_SLIP_NO"].ToString() + "</span>&nbsp;";

                        }
                    }
                    else
                    {
                        BatchPending.Text += "<span class=\"label label-success\" >There are no Pending Weight Slip for Receiving</span>&nbsp;";
                    }

                    conn.Close();

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

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
              {
                if (IS_EDIT_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    int TransferID = Convert.ToInt32(TextTransferID.Text);
                    int SlipNo = Convert.ToInt32(TextMfWbSlipNo.Text);
                    int VehicleID = Convert.ToInt32(DropDownVehicleID.Text);
                    int ItemID = Convert.ToInt32(DropDownItemID.Text);
                    int ItemBinID = Convert.ToInt32(Request.Form[DropDownItemBinID.UniqueID]);
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                     
                    double Mat1stWeightMf = Convert.ToDouble(TextMat1stWeightMf.Text.Trim());
                    double Mat2ndWeightMf = Convert.ToDouble(TextMat2ndWeightMf.Text.Trim());
                    double MatWeightMf = Convert.ToDouble(TextMatWeightMf.Text.Trim());
                    double MatWeightMs = Convert.ToDouble(TextMatWeightMs.Text.Trim());
                    double WeightVariance = MatWeightMf - MatWeightMs;

                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    // Transfer master update

                    string update_user = "update  MF_PURCHASE_TRANSFER_MASTER  set WB_SLIP_NO = :NoSlipID, VEHICLE_ID = :NoVehicleID, ITEM_ID = :NoItemID,  ITEM_BIN_ID = :NoItemBinID, FIRST_WT_MF = :NoMat1stWeightMf, SECOND_WT_MF =:NoMat2ndWeightMf, NET_WT_MF = :NoMatWeightMf, NET_WT_MS = :NoMatWeightMs, VARIANCE = :NoWeightVariance, REMARKS = :TextRemarks, ENTRY_DATE = TO_DATE(:EntryDate, 'DD/MM/YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where TRANSFER_ID = :NoTransferID ";
                    cmdi = new OracleCommand(update_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[15]; 
                    objPrm[0] = cmdi.Parameters.Add("NoSlipID", SlipNo);
                    objPrm[1] = cmdi.Parameters.Add("NoVehicleID", VehicleID);
                    objPrm[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[3] = cmdi.Parameters.Add("NoItemBinID", ItemBinID);
                    objPrm[4] = cmdi.Parameters.Add("NoMat1stWeightMf", Mat1stWeightMf);
                    objPrm[5] = cmdi.Parameters.Add("NoMat2ndWeightMf", Mat2ndWeightMf);
                    objPrm[6] = cmdi.Parameters.Add("NoMatWeightMf", MatWeightMf);
                    objPrm[7] = cmdi.Parameters.Add("NoMatWeightMs", MatWeightMs);
                    objPrm[8] = cmdi.Parameters.Add("NoWeightVariance", WeightVariance);
                    objPrm[9] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[10] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                    objPrm[11] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[12] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[13] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[14] = cmdi.Parameters.Add("NoTransferID", TransferID);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Transfer Data Update successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                    clearText();
                    TextMfWbSlipNo.Focus();
                    Display();
                    DisplaySummary();
                    PendingReceiving();
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
                if (txtSearchEmp.Text == "")
                {
                    makeSQL = " SELECT MMTM.TRANSFER_ID, MMTM.WB_SLIP_NO, MMTM.FIRST_WT_MF, MMTM.SECOND_WT_MF, MMTM.NET_WT_MF, MMTM.NET_WT_MS, MMTM.VARIANCE, MMTM.ENTRY_DATE, MMTM.REMARKS, MMTM.ENTRY_DATE, MMTM.IS_ACTIVE, MMTM.CREATE_DATE, MMTM.UPDATE_DATE, MMTM.FIRST_APPROVED_IS, MMTM.IS_PRINT, TO_CHAR(MMTM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE, MV.VEHICLE_NO, MM.ITEM_NAME, MM.ITEM_CODE, MIB.ITEM_BIN_NAME FROM MF_PURCHASE_TRANSFER_MASTER MMTM LEFT JOIN MF_VEHICLE MV ON MV.VEHICLE_ID = MMTM.VEHICLE_ID LEFT JOIN MF_ITEM MM ON MM.ITEM_ID = MMTM.ITEM_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MMTM.ITEM_BIN_ID ORDER BY MMTM.CREATE_DATE DESC ";
                }
                else
                {
                    if (DropDownItemID1.Text == "0")
                    {
                        makeSQL = " SELECT MMTM.TRANSFER_ID, MMTM.WB_SLIP_NO, MMTM.FIRST_WT_MF, MMTM.SECOND_WT_MF, MMTM.NET_WT_MF, MMTM.NET_WT_MS, MMTM.VARIANCE, MMTM.ENTRY_DATE, MMTM.REMARKS, MMTM.ENTRY_DATE, MMTM.IS_ACTIVE, MMTM.CREATE_DATE, MMTM.UPDATE_DATE, MMTM.FIRST_APPROVED_IS, MMTM.IS_PRINT, TO_CHAR(MMTM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE, MV.VEHICLE_NO, MM.ITEM_NAME, MM.ITEM_CODE, MIB.ITEM_BIN_NAME FROM MF_PURCHASE_TRANSFER_MASTER MMTM LEFT JOIN MF_VEHICLE MV ON MV.VEHICLE_ID = MMTM.VEHICLE_ID LEFT JOIN MF_ITEM MM ON MM.ITEM_ID = MMTM.ITEM_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MMTM.ITEM_BIN_ID where MMTM.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or MV.VEHICLE_NO like '" + txtSearchEmp.Text + "%' or MM.ITEM_NAME like '" + txtSearchEmp.Text + "%' or MMTM.ENTRY_DATE like '" + txtSearchEmp.Text + "%' or MMTM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' or MMTM.FIRST_APPROVED_IS like '" + txtSearchEmp.Text + "%' ORDER BY MMTM.WB_SLIP_NO asc";  
                    }
                    else
                    {
                        makeSQL = "  SELECT MMTM.TRANSFER_ID, MMTM.WB_SLIP_NO, MMTM.FIRST_WT_MF, MMTM.SECOND_WT_MF, MMTM.NET_WT_MF, MMTM.NET_WT_MS, MMTM.VARIANCE, MMTM.ENTRY_DATE, MMTM.REMARKS, MMTM.ENTRY_DATE, MMTM.IS_ACTIVE, MMTM.CREATE_DATE, MMTM.UPDATE_DATE, MMTM.FIRST_APPROVED_IS, MMTM.IS_PRINT, TO_CHAR(MMTM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE, MV.VEHICLE_NO, MM.ITEM_NAME, MM.ITEM_CODE, MIB.ITEM_BIN_NAME FROM MF_PURCHASE_TRANSFER_MASTER MMTM LEFT JOIN MF_VEHICLE MV ON MV.VEHICLE_ID = MMTM.VEHICLE_ID LEFT JOIN MF_ITEM MM ON MM.ITEM_ID = MMTM.ITEM_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MMTM.ITEM_BIN_ID  where  MM.ITEM_ID like '" + DropDownItemID1.Text + "%' AND (MMTM.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or MV.VEHICLE_NO like '" + txtSearchEmp.Text + "%' or MM.ITEM_NAME like '" + txtSearchEmp.Text + "%' or MMTM.ENTRY_DATE like '" + txtSearchEmp.Text + "%' or MMTM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' or MMTM.FIRST_APPROVED_IS like '" + txtSearchEmp.Text + "%') ORDER BY MMTM.WB_SLIP_NO asc";  // PPM.CREATE_DATE desc, PPM.UPDATE_DATE desc
                    }

                    alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView4D.DataSource = dt;
                GridView4D.DataKeyNames = new string[] { "TRANSFER_ID" };
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
                if (isCheck == "Complete")
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
                string CurrentMonth = System.DateTime.Now.ToString("MM/yyyy");
                string makeSQL = "";
                if (TextMonthYear4.Text == "")
                {
                    makeSQL = " SELECT MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME, count(MMTM.WB_SLIP_NO) AS SLIP_NO, sum(MMTM.NET_WT_MF) AS NET_WT_MF, sum(MMTM.NET_WT_MS) AS NET_WT_MS, sum(MMTM.VARIANCE) AS VARIANCE FROM MF_ITEM MI LEFT JOIN MF_PURCHASE_TRANSFER_MASTER MMTM ON MMTM.ITEM_ID = MI.ITEM_ID WHERE to_char(MMTM.ENTRY_DATE, 'mm/yyyy') = '" + CurrentMonth + "' GROUP BY MI.ITEM_ID, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME ORDER BY MI.ITEM_ID ";
                }
                else
                {
                    makeSQL = " SELECT MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME, count(MMTM.WB_SLIP_NO) AS SLIP_NO, sum(MMTM.NET_WT_MF) AS NET_WT_MF, sum(MMTM.NET_WT_MS) AS NET_WT_MS, sum(MMTM.VARIANCE) AS VARIANCE FROM MF_ITEM MI LEFT JOIN MF_PURCHASE_TRANSFER_MASTER MMTM ON MMTM.ITEM_ID = MI.ITEM_ID WHERE to_char(MMTM.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear4.Text + "' GROUP BY MI.ITEM_ID, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME ORDER BY MI.ITEM_ID ";
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

                    decimal total_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("NET_WT_MF"));
                    GridView2.FooterRow.Cells[2].Font.Bold = true;
                    GridView2.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[2].Text = total_wt.ToString("N2");

                    decimal total_amt = dt.AsEnumerable().Sum(row => row.Field<decimal>("NET_WT_MS"));
                    GridView2.FooterRow.Cells[3].Font.Bold = true;
                    GridView2.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[3].Text = total_amt.ToString("N2");

                    decimal total_avg = (total_wt - total_amt);
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
            GridView4D.PageIndex = e.NewPageIndex;
            DisplaySummary();
            alert_box.Visible = false;
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
                int TransferID = Convert.ToInt32(TextTransferID.Text);
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                 string delete_user = " delete from MF_PURCHASE_TRANSFER_MASTER where TRANSFER_ID  = '" + TransferID + "'"; 
                cmdi = new OracleCommand(delete_user, conn); 
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();


                string update_user = "update  MS_SALES_INTER_DIV_MASTER  set IS_MF_RECEIVED_STATUS = :IsReceived, MF_RECEIVED_ID = :NoCuserID, MF_RECEIVED_DATE = TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM') where WB_SLIP_NO = :TextMsWbSlipNo ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPr = new OracleParameter[4];
                objPr[0] = cmdi.Parameters.Add("IsReceived", "");
                objPr[1] = cmdi.Parameters.Add("c_date", u_date);
                objPr[2] = cmdi.Parameters.Add("NoCuserID", userID);
                objPr[3] = cmdi.Parameters.Add("TextMsWbSlipNo", DropDownMsWbSlipNo.Text);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Transfer Data Delete Successfully"));
                alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                clearText(); 
                Display();
                PendingReceiving();
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
            TextMfWbSlipNo.Text = ""; 
            TextMatWeightMf.Text = "";
            TextMatWeightMs.Text = ""; 
            TextMat1stWeightMf.Text = "";
            TextMat2ndWeightMf.Text = "";
            TextWeightVariance.Text = "";
            CheckSlipNo.Text = ""; 
            DropDownVehicleID.Text = "0"; 
            DropDownItemID.Text = "0";
            DropDownItemBinID.Enabled = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        {
            TextMfWbSlipNo.Text = ""; 
            TextMatWeightMf.Text = "";
            TextMatWeightMs.Text = "";
            Text1stWeightMs.Text = "";
            Text2ndWeightMs.Text = "";
            TextMatWeightMs.Text = "";
            TextMat1stWeightMf.Text = "";
            TextMat2ndWeightMf.Text = "";
            TextWeightVariance.Text = "";
            CheckSlipNo.Text = ""; 
            DropDownVehicleID.Text = "0"; 
            DropDownItemID.Text = "0";

            DataTable dtSlipNoID = new DataTable();
            DataSet dss = new DataSet();
            string makedtSlipNoIDSQL = " SELECT * FROM MS_SALES_INTER_DIV_MASTER  WHERE IS_ACTIVE = 'Enable' AND IS_MF_RECEIVED_STATUS = 'Pending' AND INTER_DIVISION_ID = '5' ORDER BY WB_SLIP_NO DESC";
            dss = ExecuteBySqlString(makedtSlipNoIDSQL);
            dtSlipNoID = (DataTable)dss.Tables[0];
            DropDownMsWbSlipNo.DataSource = dtSlipNoID;
            DropDownMsWbSlipNo.DataValueField = "WB_SLIP_NO";
            DropDownMsWbSlipNo.DataTextField = "WB_SLIP_NO";
            DropDownMsWbSlipNo.DataBind();
            DropDownMsWbSlipNo.Items.Insert(0, new ListItem("Select MS Weight Slip No", "0"));
            DropDownMsWbSlipNo.Text = "0";

            DropDownItemBinID.Enabled = false;
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