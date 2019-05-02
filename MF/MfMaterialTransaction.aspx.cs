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
    public partial class MfMaterialTransaction : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl, cmdsp;
        OracleDataAdapter oradata; 
        DataTable dt;
        int RowCount;
        double ItemVatAmt = 0.0, ItemAmtTotal = 0.0, ItemWtWbTotal = 0.0, TotalInvoiceAmt; string EntryDateSlip = "", TransanctionNo = "", TransanctionFor = "";
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
                        
                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeDropDownItemSQL = " SELECT ITEM_ID, ITEM_CODE  || ' : ' || ITEM_NAME AS ITEM_NAME FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        dsi = ExecuteBySqlString(makeDropDownItemSQL);
                        dtItemID = (DataTable)dsi.Tables[0];
                        DropDownItemID.DataSource = dtItemID;
                        DropDownItemID.DataValueField = "ITEM_ID";
                        DropDownItemID.DataTextField = "ITEM_NAME";
                        DropDownItemID.DataBind();
                        DropDownItemID.Items.Insert(0, new ListItem("All Item", "0"));
                          

                        DropDownSearchItemID.DataSource = dtItemID;
                        DropDownSearchItemID.DataValueField = "ITEM_ID";
                        DropDownSearchItemID.DataTextField = "ITEM_NAME";
                        DropDownSearchItemID.DataBind();
                        DropDownSearchItemID.Items.Insert(0, new ListItem("All Item", "0"));
                         
                        DataTable dtCollecForID = new DataTable();
                        DataSet dsf = new DataSet();
                        string makeDropDownCollecForSQL = " SELECT TRANSACTION_FOR_ID, TRANSACTION_FOR_NAME || ' - ' || SYMBOL AS TRANSACTION_FOR_NAME FROM NRC_MATERIAL_TRANSACTION_FOR WHERE IS_ACTIVE = 'Enable' AND TRANSACTION_FOR_ID != 3 ORDER BY TRANSACTION_FOR_ID ASC";
                        dsf = ExecuteBySqlString(makeDropDownCollecForSQL);
                        dtCollecForID = (DataTable)dsf.Tables[0];
                        DropDownTransactionForID.DataSource = dtCollecForID;
                        DropDownTransactionForID.DataValueField = "TRANSACTION_FOR_ID";
                        DropDownTransactionForID.DataTextField = "TRANSACTION_FOR_NAME";
                        DropDownTransactionForID.DataBind();
                        DropDownTransactionForID.Items.Insert(0, new ListItem("Select Transaction For", "0"));



                        //   DropDownTransactionForID.Focus();
                        //  VatPercent.Visible = false;
                        TextMatTransactionForID.Attributes.Add("readonly", "readonly"); 
                        //    DropDownItemID.Enabled = false; 

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
                    int ItemID   = Convert.ToInt32(DropDownItemID.Text); 
                    int ItemTransferID = 0; 
                    int TransactionForID = Convert.ToInt32(DropDownTransactionForID.Text);
                    string TransInventoryID = DropDownTransInventoryID.Text;
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                      
                    string[] MakeEntryDateSplit = EntryDate.Text.Split('-'); 
                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-"); 
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                     
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt"); 
                    double ItemWeight = Convert.ToDouble(TextItemWeightWP.Text.Trim());    
                  
                    //inventory calculation
                     
                    int InvenItemID = 0; 
                    double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, StockOutWetNew = 0.00, FinalStock = 0.00, StockInWetNew = 0.00, FinalStockNew = 0.00;
                    string makeSQL = "", update_inven_mas = "";
                    if (TransInventoryID == "RM") {
                                makeSQL = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    } else {
                                makeSQL = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    }
                        cmdl = new OracleCommand(makeSQL);
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

                if (TransactionForID == 1) {
                    if(ItemWeight <= FinalStock) {
                    // deduct from the inventory
                    StockOutWetNew = StockOutWet + ItemWeight;
                    FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;

                        if (TransInventoryID == "RM")
                        {
                             update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";

                        }
                        else
                        {
                             update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";

                        }
                         
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

                    string get_id = "select MF_MATERIAL_TRANSACTIONID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_id, conn);
                    int newTransactionID = Int32.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_trans = "insert into  MF_MATERIAL_TRANSACTION (MAT_TRANS_ID, TRANSACTION_FOR_ID, ITEM_ID, ITEM_WEIGHT, ITEM_TRANSFER_ID, ENTRY_DATE, REMARKS, INVENTORY_TYPE, CREATE_DATE, C_USER_ID, IS_ACTIVE) values  ('MT-' || LPAD(:NoTransactionID, 6, '0') , :NoTransactionForID, :NoItemID, :NoItemWeight, :NoItemTransferID, TO_DATE(:EntryDate, 'DD/MM/YYYY'), :TextRemarks, :TextInventoryType, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive)";
                    cmdi = new OracleCommand(insert_trans, conn);

                    OracleParameter[] objPrm = new OracleParameter[11];
                    objPrm[0] = cmdi.Parameters.Add("NoTransactionID", newTransactionID);
                    objPrm[1] = cmdi.Parameters.Add("NoTransactionForID", TransactionForID);
                    objPrm[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[3] = cmdi.Parameters.Add("NoItemWeight", ItemWeight);
                    objPrm[4] = cmdi.Parameters.Add("NoItemTransferID", ItemTransferID);
                    objPrm[5] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[6] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                    objPrm[7] = cmdi.Parameters.Add("TextInventoryType", TransInventoryID);
                    objPrm[8] = cmdi.Parameters.Add("c_date", c_date);
                    objPrm[9] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[10] = cmdi.Parameters.Add("TextIsActive", ISActive);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();

                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Insert New Material Issued Successfully"));
                        alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                    }
                    else
                    {
                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory"));
                        alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                    }

                }
                else if (TransactionForID == 2)
                {
                    // add from the inventory
                    StockInWetNew = StockInWet + ItemWeight;
                    FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;
                    if (TransInventoryID == "RM")
                    {
                        update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";

                    }
                    else
                    {
                        update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";

                    }


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

                    string get_id = "select MF_MATERIAL_TRANSACTIONID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_id, conn);
                    int newTransactionID = Int32.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_trans = "insert into  MF_MATERIAL_TRANSACTION (MAT_TRANS_ID, TRANSACTION_FOR_ID, ITEM_ID, ITEM_WEIGHT, ITEM_TRANSFER_ID, ENTRY_DATE, REMARKS, INVENTORY_TYPE, CREATE_DATE, C_USER_ID, IS_ACTIVE) values  ('TR-' || LPAD(:NoTransactionID, 6, '0') , :NoTransactionForID, :NoItemID, :NoItemWeight, :NoItemTransferID, TO_DATE(:EntryDate, 'DD/MM/YYYY'), :TextRemarks, :TextInventoryType, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive)";
                    cmdi = new OracleCommand(insert_trans, conn);

                    OracleParameter[] objPrm = new OracleParameter[11];
                    objPrm[0] = cmdi.Parameters.Add("NoTransactionID", newTransactionID);
                    objPrm[1] = cmdi.Parameters.Add("NoTransactionForID", TransactionForID);
                    objPrm[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[3] = cmdi.Parameters.Add("NoItemWeight", ItemWeight);
                    objPrm[4] = cmdi.Parameters.Add("NoItemTransferID", ItemTransferID);
                    objPrm[5] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[6] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                    objPrm[7] = cmdi.Parameters.Add("TextInventoryType", TransInventoryID);
                    objPrm[8] = cmdi.Parameters.Add("c_date", c_date);
                    objPrm[9] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[10] = cmdi.Parameters.Add("TextIsActive", ISActive);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Material Received Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                }
            
                    clearText(); 
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
                
            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
            string TransactionForID = Session["user_data_id"].ToString();
                 
            string HtmlString = "";
            string makeSQL = " SELECT WMT.MAT_TRANS_ID, WMT.TRANSACTION_FOR_ID, WMT.INVENTORY_TYPE, NMTF.TRANSACTION_FOR_NAME, WI.ITEM_NAME, WMT.ITEM_WEIGHT, WIT.ITEM_NAME AS ITEM_NAME_TRANSFER, WMT.REMARKS, TO_CHAR(WMT.ENTRY_DATE,'dd-MON-yyyy') AS ENTRY_DATE, WMT.CREATE_DATE, WMT.UPDATE_DATE, WMT.IS_ACTIVE, WMT.IS_PRINT, TO_CHAR(WMT.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE FROM  MF_MATERIAL_TRANSACTION WMT LEFT JOIN NRC_MATERIAL_TRANSACTION_FOR NMTF ON NMTF.TRANSACTION_FOR_ID = WMT.TRANSACTION_FOR_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WMT.ITEM_ID LEFT JOIN MF_ITEM WIT ON WIT.ITEM_ID = WMT.ITEM_TRANSFER_ID WHERE  WMT.MAT_TRANS_ID = '" + TransactionForID + "' ";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;
            
            for (int i = 0; i < RowCount; i++)
            {
            TransanctionNo = dt.Rows[i]["MAT_TRANS_ID"].ToString();
            string InventoryType = dt.Rows[i]["INVENTORY_TYPE"].ToString();
            TransanctionFor = dt.Rows[i]["TRANSACTION_FOR_NAME"].ToString();
            string ItemName = dt.Rows[i]["ITEM_NAME"].ToString();
            string ItemWeight = dt.Rows[i]["ITEM_WEIGHT"].ToString(); 
            EntryDateSlip = dt.Rows[i]["ENTRY_DATE"].ToString();  
            HtmlString += "<div style='float:left;width:775px;height:335px;margin:10px 20px 10px 25px;padding:10px 10px 0 0;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 15px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;border:black solid 1px'> ";
            HtmlString += "<div style='float:left;width:825px;margin-top:5px;height:82px;text-align:center;' ><img src='../../image/logo_from.png'/></div> ";
            HtmlString += "<div style='float:left;width:825px;height:20px;text-align:center;font-family:Times New Roman,Times, serif;font-size:17px;font-weight:700;' >Plastic Factory Division</div> ";
            HtmlString += "<div style='float:left;width:825px;text-align:center;font-family:Times New Roman,Times, serif;font-size:16px;font-weight:700;' >Certificate Of " + TransanctionFor + " ("+ InventoryType + ")</div> ";
         
            HtmlString += "<div style='float:left;width:340px;margin:15px 0 0 20px;text-align:left;' ><span style='font-family:Times New Roman,Times, serif;font-size:15px;font-weight:700;'>Transanction No :</span> <span style='color:Red;font-size:15px;'>" + TransanctionNo + "</span></div> ";
            HtmlString += "<div style='float:left;width:407px;margin-top:15px;text-align:right;'><span style='font-family:Times New Roman;'>Transanction Date :</span>" + EntryDateSlip + "</div> ";
            HtmlString += "<div style='float:left;width:750px;margin:6px 0 0 18px;'> "; 
            HtmlString += "<table border='1px solid black;' width=100%; cellpadding=5px;>";
            HtmlString += "<thead>";
            HtmlString += "<tr>";
            HtmlString += "<th><span style='font-family:Times New Roman;'>For Inventory</span></th>";
            HtmlString += "<th><span style='font-family:Times New Roman;'>" + TransanctionFor + "</span></th>";
               
            HtmlString += "<th><span style='font-family:Times New Roman;'>Material Weight (MT)</span></th>";
            HtmlString += " </tr>";
            HtmlString += "</thead>";
            HtmlString += "<tbody>"; 
            HtmlString += "<tr>"; 
            HtmlString += "<td align='center'>";
            HtmlString += "" + InventoryType + "";
            HtmlString += "</td>";
            HtmlString += "<td align='center'>";
            HtmlString += "" + ItemName + ""; 
            HtmlString += "</td>"; 
            HtmlString += "<td align='center'>";
            HtmlString += "" +  string.Format("{0:n3}", Convert.ToDouble(ItemWeight))  + "";
            HtmlString += "</td>";
             
            HtmlString += "</tr>"; 
            HtmlString += "</tbody>";
            HtmlString += "</table>";
                     
            HtmlString += "</div>";

            HtmlString += "<div style='float:left;width:200px;margin:65 0 0 20px;text-align:left;' ><span style='font-family:Times New Roman,Times, serif;font-size:15px;font-weight:700;'>Prepared By :</span></div> ";
            HtmlString += "<div style='float:left;width:340px;margin-top:65px;text-align:center;'><span style='font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-weight: 600;'>Print Date: " + DateTime.Now.ToString("dd/MM/yyyy h:mm:ss tt") + "</span></div> ";
            HtmlString += "<div style='float:left;width:200px;margin-top:65px;text-align:right;'><span style='font-family:Times New Roman;'>Approved By :</span></div> ";

                } 
            HtmlString += "</div>";
            HtmlString += "</div>";
            HtmlString += "</div>";

            // purchase master update for print
            int userID = Convert.ToInt32(Session["USER_ID"]); 
            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
            string update_user = "update  MF_MATERIAL_TRANSACTION  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where MAT_TRANS_ID = :NoSlipNoWp ";
            cmdi = new OracleCommand(update_user, conn);

            OracleParameter[] objPrm = new OracleParameter[4];
            objPrm[0] = cmdi.Parameters.Add("TextIsPrint", "Printed"); 
            objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
            objPrm[2] = cmdi.Parameters.Add("NoCuserID", userID); 
            objPrm[3] = cmdi.Parameters.Add("NoSlipNoWp", TransactionForID);

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
        public static List<ListItem> GetItemDataTransferList(int ItemID)
        { 
            string query = " SELECT ITEM_ID, ITEM_CODE  || ' : ' || ITEM_NAME  AS ITEM_NAME FROM MF_ITEM WHERE ITEM_ID NOT IN :ItemID AND IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
            string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; 
            using (OracleConnection conn = new OracleConnection(strConnString))
            {
                using (OracleCommand cmd = new OracleCommand(query))
                {
                    List<ListItem> ItemsList = new List<ListItem>();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("ItemID", ItemID);
                    cmd.Connection = conn;
                    conn.Open();
                    using (OracleDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            ItemsList.Add(new ListItem
                            {
                                Value = sdr["ITEM_ID"].ToString(),
                                Text = sdr["ITEM_NAME"].ToString()
                            });
                        }
                    }
                    conn.Close();
                    return ItemsList;
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
            string USER_DATA_ID = Session["user_data_id"].ToString();

            string makeSQL = " SELECT MAT_TRANS_ID, INVENTORY_TYPE, TRANSACTION_FOR_ID, ITEM_ID, ITEM_WEIGHT, ITEM_TRANSFER_ID, REMARKS, TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, C_USER_ID, CREATE_DATE, U_USER_ID, UPDATE_DATE, IS_ACTIVE, IS_PRINT, P_USER_ID, PRINT_DATE FROM MF_MATERIAL_TRANSACTION WHERE MAT_TRANS_ID = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            { 
                TextMatTransactionForID.Text = dt.Rows[i]["MAT_TRANS_ID"].ToString();
                DropDownTransInventoryID.Text = dt.Rows[i]["INVENTORY_TYPE"].ToString(); 
                DropDownTransactionForID.Text = dt.Rows[i]["TRANSACTION_FOR_ID"].ToString(); 
                DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();  
                TextItemWeightWP.Text  = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT"].ToString()).ToString(".00"); 
                EntryDate.Text = dt.Rows[i]["ENTRY_DATE"].ToString(); 
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
               
            }

            conn.Close();
            DropDownItemID.Enabled = false;
            DropDownTransInventoryID.Enabled = false; 
            DropDownTransactionForID.Enabled = false; 

            Display(); 
            alert_box.Visible = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

            BtnUpdate.Attributes.Add("aria-disabled", "true");
            BtnUpdate.Attributes.Add("class", "btn btn-success active"); 
            BtnDelete.Attributes.Add("aria-disabled", "true");
            BtnDelete.Attributes.Add("class", "btn btn-danger active");
              

          //   }
          //  catch
           //    {
        //       Response.Redirect("~/ParameterError.aspx");
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
                    makeSQL = "  SELECT WMT.MAT_TRANS_ID, WMT.INVENTORY_TYPE,  NMTF.TRANSACTION_FOR_NAME, WI.ITEM_CODE  || ' : ' || WI.ITEM_NAME AS ITEM_NAME, WMT.ITEM_WEIGHT, WIT.ITEM_NAME AS ITEM_NAME_TRANSFER, WMT.REMARKS, WMT.ENTRY_DATE, WMT.CREATE_DATE, WMT.UPDATE_DATE, WMT.IS_ACTIVE, WMT.IS_PRINT, TO_CHAR(WMT.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE FROM  MF_MATERIAL_TRANSACTION WMT LEFT JOIN NRC_MATERIAL_TRANSACTION_FOR NMTF ON NMTF.TRANSACTION_FOR_ID = WMT.TRANSACTION_FOR_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WMT.ITEM_ID LEFT JOIN MF_ITEM WIT ON WIT.ITEM_ID = WMT.ITEM_TRANSFER_ID ORDER BY  WMT.MAT_TRANS_ID  ";
                }
                else
                {
                    if (DropDownSearchItemID.Text == "0")
                    {
                        makeSQL = " SELECT WPM.PURCHASE_ID, WPM.SLIP_NO, WPM.PARTY_ID, PP.PARTY_NAME, WSC.SUPPLIER_CAT_NAME, WC.CATEGORY_NAME, WPM.ITEM_ID, WI.ITEM_CODE  || ' : ' || WI.ITEM_NAME AS ITEM_NAME, WD.DRIVER_NAME,  WCF.COLLECTION_FOR_NAME, WPM.SUPERVISOR_ID, WS.SUPERVISOR_NAME, WPM.ITEM_WEIGHT,  WPM.ITEM_RATE, WPM.ITEM_AMOUNT, nvl(WPM.ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, WPM.VAT_AMOUNT, nvl(WPM.TOTAL_AMOUNT,0) AS TOTAL_AMOUNT, WPM.ENTRY_DATE, WPM.CREATE_DATE, WPM.UPDATE_DATE, WPM.IS_ACTIVE , WPC.IS_CHECK, WPM.IS_PRINT, TO_CHAR(WPM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE FROM MF_MATERIAL_TRANSACTION WPM LEFT JOIN WP_PARTY PP ON PP.PARTY_ID = WPM.PARTY_ID LEFT JOIN WP_CATEGORY WC ON WC.CATEGORY_ID = WPM.CATEGORY_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WPM.ITEM_ID LEFT JOIN WP_COLLECTION_FOR WCF ON WCF.COLLECTION_FOR_ID = WPM.COLLECTION_FOR_ID LEFT JOIN WP_SUPERVISOR WS ON WS.SUPERVISOR_ID = WPM.SUPERVISOR_ID LEFT JOIN WP_DRIVER WD ON WD.DRIVER_ID = WPM.DRIVER_ID  LEFT JOIN WP_PURCHASE_CLAIM WPC ON  WPC.CLAIM_NO = WPM.CLAIM_NO LEFT JOIN WP_SUPPLIER_CATEGORY WSC ON WSC.SUPPLIER_CAT_ID = WPM.SUPPLIER_CAT_ID  where WPM.SLIP_NO like '" + txtSearchEmp.Text + "%' or WPM.PARTY_ID like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PS.SUPERVISOR_NAME like '" + txtSearchEmp.Text + "%' or WPM.ITEM_RATE like '" + txtSearchEmp.Text + "%'  or to_char(WPM.ENTRY_DATE, 'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or to_char(WPM.ENTRY_DATE, 'mm/yyyy')  like '" + txtSearchEmp.Text + "%' or WPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' or PPC.IS_CHECK like '" + txtSearchEmp.Text + "%' ORDER BY WPM.SLIP_NO asc";  // WPM.CREATE_DATE desc, WPM.UPDATE_DATE desc
                    }
                    else
                    {
                        makeSQL = " SELECT WPM.PURCHASE_ID, WPM.SLIP_NO, WPM.PARTY_ID, PP.PARTY_NAME, WSC.SUPPLIER_CAT_NAME, WC.CATEGORY_NAME, WPM.ITEM_ID, WI.ITEM_CODE  || ' : ' || WI.ITEM_NAME AS ITEM_NAME, WD.DRIVER_NAME,  WCF.COLLECTION_FOR_NAME, WPM.SUPERVISOR_ID, WS.SUPERVISOR_NAME, WPM.ITEM_WEIGHT,  WPM.ITEM_RATE, WPM.ITEM_AMOUNT, nvl(WPM.ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, WPM.VAT_AMOUNT, nvl(WPM.TOTAL_AMOUNT,0) AS TOTAL_AMOUNT, WPM.ENTRY_DATE, WPM.CREATE_DATE, WPM.UPDATE_DATE, WPM.IS_ACTIVE , WPC.IS_CHECK, WPM.IS_PRINT, TO_CHAR(WPM.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE FROM MF_MATERIAL_TRANSACTION WPM LEFT JOIN WP_PARTY PP ON PP.PARTY_ID = WPM.PARTY_ID LEFT JOIN WP_CATEGORY WC ON WC.CATEGORY_ID = WPM.CATEGORY_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WPM.ITEM_ID LEFT JOIN WP_COLLECTION_FOR WCF ON WCF.COLLECTION_FOR_ID = WPM.COLLECTION_FOR_ID LEFT JOIN WP_SUPERVISOR WS ON WS.SUPERVISOR_ID = WPM.SUPERVISOR_ID LEFT JOIN WP_DRIVER WD ON WD.DRIVER_ID = WPM.DRIVER_ID  LEFT JOIN WP_PURCHASE_CLAIM WPC ON  WPC.CLAIM_NO = WPM.CLAIM_NO LEFT JOIN WP_SUPPLIER_CATEGORY WSC ON WSC.SUPPLIER_CAT_ID = WPM.SUPPLIER_CAT_ID where  PI.ITEM_ID like '" + DropDownSearchItemID.Text + "%' AND (WPM.SLIP_NO like '" + txtSearchEmp.Text + "%' or WPM.PARTY_ID like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PPT.PUR_TYPE_NAME like '" + txtSearchEmp.Text + "%'  or PS.SUPERVISOR_NAME like '" + txtSearchEmp.Text + "%' or WPM.ITEM_RATE like '" + txtSearchEmp.Text + "%'  or to_char(WPM.ENTRY_DATE, 'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or to_char(WPM.ENTRY_DATE, 'mm/yyyy')  like '" + txtSearchEmp.Text + "%' or WPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' or PPC.IS_CHECK like '" + txtSearchEmp.Text + "%') ORDER BY WPM.SLIP_NO asc";  // WPM.CREATE_DATE desc, WPM.UPDATE_DATE desc
                    }

                 //   alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView4D.DataSource = dt;
                GridView4D.DataKeyNames = new string[] { "MAT_TRANS_ID" };
                GridView4D.DataBind();
                conn.Close();
                // alert_box.Visible = false;
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
                    makeSQL = " SELECT WMT.INVENTORY_TYPE, NMTF.TRANSACTION_FOR_NAME, WI.ITEM_NAME, SUM(WMT.ITEM_WEIGHT) AS ITEM_WEIGHT, WIT.ITEM_NAME AS ITEM_NAME_TRANSFER FROM MF_MATERIAL_TRANSACTION WMT LEFT JOIN NRC_MATERIAL_TRANSACTION_FOR NMTF ON NMTF.TRANSACTION_FOR_ID = WMT.TRANSACTION_FOR_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WMT.ITEM_ID LEFT JOIN MF_ITEM WIT ON WIT.ITEM_ID = WMT.ITEM_TRANSFER_ID WHERE to_char(WMT.ENTRY_DATE, 'mm/yyyy') = '" + MonthYear + "' GROUP BY WMT.INVENTORY_TYPE, NMTF.TRANSACTION_FOR_NAME, WI.ITEM_NAME, WIT.ITEM_NAME ORDER BY WI.ITEM_NAME   ";
                }
                else
                {
                    makeSQL = " SELECT WMT.INVENTORY_TYPE, NMTF.TRANSACTION_FOR_NAME, WI.ITEM_NAME, SUM(WMT.ITEM_WEIGHT) AS ITEM_WEIGHT, WIT.ITEM_NAME AS ITEM_NAME_TRANSFER FROM MF_MATERIAL_TRANSACTION WMT LEFT JOIN NRC_MATERIAL_TRANSACTION_FOR NMTF ON NMTF.TRANSACTION_FOR_ID = WMT.TRANSACTION_FOR_ID LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WMT.ITEM_ID LEFT JOIN MF_ITEM WIT ON WIT.ITEM_ID = WMT.ITEM_TRANSFER_ID WHERE to_char(WMT.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear4.Text + "' GROUP BY WMT.INVENTORY_TYPE, NMTF.TRANSACTION_FOR_NAME, WI.ITEM_NAME, WIT.ITEM_NAME ORDER BY WI.ITEM_NAME  ";

                    alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataKeyNames = new string[] { "TRANSACTION_FOR_NAME" };
                GridView2.DataBind();
                if (dt.Rows.Count > 0)
                {
                    GridView2.HeaderRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    //Calculate Sum and display in Footer Row
                    GridView2.FooterRow.Cells[0].Font.Bold = true;
                    GridView2.FooterRow.Cells[0].Text = "Grand Total";
                    GridView2.FooterRow.Cells[0].HorizontalAlign = HorizontalAlign.Right;
                     
                    decimal total_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_WEIGHT"));
                    GridView2.FooterRow.Cells[2].Font.Bold = true;
                    GridView2.FooterRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[2].Text = total_wt.ToString("N3"); 
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
                string TransInventoryID = DropDownTransInventoryID.Text;
                string MatTransactionForID = TextMatTransactionForID.Text;
                int ItemID = Convert.ToInt32(DropDownItemID.Text);
                int ItemTransferID = 0;
                int TransactionForID = Convert.ToInt32(DropDownTransactionForID.Text);
                 
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                string[] MakeEntryDateSplit = EntryDate.Text.Split('-');
                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                double ItemWeight = Math.Abs(Convert.ToDouble(TextItemWeightWP.Text.Trim()));



                // get old purchase item weight
                    int InvenItemID = 0; 
                    double ItemWeightOld = 0.00; int ItemIdOld = 0;
                    double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, StockOutWetNew = 0.00, FinalStock = 0.00, StockInWetNew = 0.00, FinalStockNew = 0.00;
                    string makeSQL = " select  ITEM_ID, nvl(ITEM_WEIGHT,0) AS ITEM_WEIGHT  from MF_MATERIAL_TRANSACTION WHERE MAT_TRANS_ID  = '" + MatTransactionForID + "'";
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
                    double FinalStockCheck = 0.00; string makeSQLCheckRM = "", update_inven_old = "", makeSQLUpdateRM = "", update_inven_mas = "";
                    if (TransInventoryID == "RM")
                    {
                         makeSQLCheckRM = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                    }
                    else
                    {
                        makeSQLCheckRM = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                    }
              
                    cmdl = new OracleCommand(makeSQLCheckRM);
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


                if (TransactionForID == 1)
                {
                    if (ItemWeight <= FinalStock)
                    {

                        // deduct old weight from the inventory
                        StockOutWetNew = Math.Abs(StockOutWet - ItemWeightOld);
                        FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;
                        if (TransInventoryID == "RM")
                        {
                            update_inven_old = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                        }
                        else
                        {
                            update_inven_old = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                        }
                         
                        cmdl = new OracleCommand(update_inven_old, conn);

                        OracleParameter[] objPrmInevenOld = new OracleParameter[5];
                        objPrmInevenOld[0] = cmdl.Parameters.Add("NoStockOut", StockOutWetNew);
                        objPrmInevenOld[1] = cmdl.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenOld[2] = cmdl.Parameters.Add("u_date", u_date);
                        objPrmInevenOld[3] = cmdl.Parameters.Add("NoCuserID", userID);
                        objPrmInevenOld[4] = cmdl.Parameters.Add("NoItemID", InvenItemID);

                        cmdl.ExecuteNonQuery();
                        cmdl.Parameters.Clear();
                        cmdl.Dispose();

                        if (TransInventoryID == "RM")
                        {
                             makeSQLUpdateRM = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                        }
                        else
                        {
                            makeSQLUpdateRM = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                        }
                      
                        cmdl = new OracleCommand(makeSQLUpdateRM);
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

                        // deduct from the inventory
                        StockOutWetNew = StockOutWet + ItemWeight;
                         FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;
                        if (TransInventoryID == "RM")
                        {
                            update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                        }
                        else
                        {
                            update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                        }

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

                        // purchase master update 
                        string update_user = "update  MF_MATERIAL_TRANSACTION  set TRANSACTION_FOR_ID =:NoTransactionForID, ITEM_ID =:NoItemID, ITEM_WEIGHT =:TextItemWeight, ITEM_TRANSFER_ID =:NoItemTransferID, ENTRY_DATE = TO_DATE(:EntryDate, 'DD/MM/YYYY'), REMARKS =:TextRemarks, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID =:NoCuserID, IS_ACTIVE =:TextIsActive WHERE MAT_TRANS_ID =:NoPurchaseID ";
                        cmdi = new OracleCommand(update_user, conn);

                        OracleParameter[] objPrm = new OracleParameter[10];
                        objPrm[0] = cmdi.Parameters.Add("NoTransactionForID", TransactionForID);
                        objPrm[1] = cmdi.Parameters.Add("NoItemID", ItemID);
                        objPrm[2] = cmdi.Parameters.Add("TextItemWeight", ItemWeight);
                        objPrm[3] = cmdi.Parameters.Add("NoItemTransferID", ItemTransferID);
                        objPrm[4] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                        objPrm[5] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                        objPrm[6] = cmdi.Parameters.Add("u_date", u_date);
                        objPrm[7] = cmdi.Parameters.Add("NoCuserID", userID);
                        objPrm[8] = cmdi.Parameters.Add("TextIsActive", ISActive);
                        objPrm[9] = cmdi.Parameters.Add("NoPurchaseID", MatTransactionForID);


                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();
                        conn.Close();

                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Update Dtata for Material Issued Successfully" + ItemWeightOld));
                        alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                    }
                    else
                    {
                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory-1"));
                        alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                    }

                }
                else if (TransactionForID == 2)
                {
                    // add old weight from the inventory
                    StockInWetNew = StockInWet - ItemWeightOld;
                    FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;
                    if (TransInventoryID == "RM")
                    {
                        update_inven_old = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                    }
                    else
                    {
                        update_inven_old = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                    }

                    cmdl = new OracleCommand(update_inven_old, conn);

                    OracleParameter[] objPrmInevenOld = new OracleParameter[5];
                    objPrmInevenOld[0] = cmdl.Parameters.Add("NoStockIn", StockInWetNew);
                    objPrmInevenOld[1] = cmdl.Parameters.Add("NoFinalStock", FinalStockNew);
                    objPrmInevenOld[2] = cmdl.Parameters.Add("u_date", u_date);
                    objPrmInevenOld[3] = cmdl.Parameters.Add("NoCuserID", userID);
                    objPrmInevenOld[4] = cmdl.Parameters.Add("NoItemID", InvenItemID);

                    cmdl.ExecuteNonQuery();
                    cmdl.Parameters.Clear();
                    cmdl.Dispose();
                    if (TransInventoryID == "RM")
                    {
                        makeSQLUpdateRM = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                    }
                    else
                    {
                        makeSQLUpdateRM = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
                    }
                
                    cmdl = new OracleCommand(makeSQLUpdateRM);
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
                     
                    // add from the inventory
                    StockInWetNew = StockInWet + ItemWeight;
                    FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;
                    if (TransInventoryID == "RM")
                    {
                        update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                    }
                    else
                    {
                        update_inven_mas = "update  PF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                    }
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

                    // purchase master update 
                    string update_user = "update  MF_MATERIAL_TRANSACTION  set TRANSACTION_FOR_ID =:NoTransactionForID, ITEM_ID =:NoItemID, ITEM_WEIGHT =:TextItemWeight, ITEM_TRANSFER_ID =:NoItemTransferID, ENTRY_DATE = TO_DATE(:EntryDate, 'DD/MM/YYYY'), REMARKS =:TextRemarks, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID =:NoCuserID, IS_ACTIVE =:TextIsActive WHERE MAT_TRANS_ID =:NoPurchaseID ";
                    cmdi = new OracleCommand(update_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[10];
                    objPrm[0] = cmdi.Parameters.Add("NoTransactionForID", TransactionForID);
                    objPrm[1] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[2] = cmdi.Parameters.Add("TextItemWeight", ItemWeight);
                    objPrm[3] = cmdi.Parameters.Add("NoItemTransferID", ItemTransferID);
                    objPrm[4] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[5] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                    objPrm[6] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[7] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[8] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[9] = cmdi.Parameters.Add("NoPurchaseID", MatTransactionForID);


                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Update Dtata for Material Received Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                }
        
                    clearText();
                    Display();
                    DisplaySummary();
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
                int userID = Convert.ToInt32(Session["USER_ID"]);
                    // check update item weight is available
                int ItemID = Convert.ToInt32(DropDownItemID.Text);
                string TransInventoryID = DropDownTransInventoryID.Text;
                string MatTransactionForID = TextMatTransactionForID.Text; 
                double ItemWeight = Convert.ToDouble(TextItemWeightWP.Text.Trim());  
                double ItemWetInventory = ItemWeight;

                double FinalStockCheck = 0.00; string makeSQLCheckRM = ""; 
                if (TransInventoryID == "RM")
                { 
                    makeSQLCheckRM = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                }
                else
                {
                    makeSQLCheckRM = " select * from PF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                }
              
                cmdl = new OracleCommand(makeSQLCheckRM);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;

                for (int i = 0; i < RowCount; i++)
                {
                    FinalStockCheck = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                }

                if (ItemWetInventory <= FinalStockCheck)
                {



                  
                string delete_user = " delete from MF_MATERIAL_TRANSACTION where MAT_TRANS_ID  = '" + MatTransactionForID + "'"; 
                cmdi = new OracleCommand(delete_user, conn); 
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Treansfer Data Delete successfully"));
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

            TextItemWeightWP.Text = "";  
            DropDownTransactionForID.Text = "0";  
            DropDownItemID.Text = "0"; 
            TextRemarks.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 
        }

        public void clearText()
        { 
            TextItemWeightWP.Text = ""; 
            DropDownTransactionForID.Text = "0"; 
            DropDownItemID.Text = "0"; 
            TextRemarks.Text = "";
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
         
       } 
    }