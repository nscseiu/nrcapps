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
    public partial class MfMaterialShiftFg : System.Web.UI.Page
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
                        string makeDropDownItemSQL = "  SELECT ITEM_ID, ITEM_CODE  || ' : ' || ITEM_NAME AS ITEM_NAME FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' AND IS_SALES_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
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
                         
                     

                        //   DropDownShiftForID.Focus();
                        //  VatPercent.Visible = false;
                        TextMatShiftForID.Attributes.Add("readonly", "readonly"); 
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
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                      
                    string[] MakeEntryDateSplit = EntryDate.Text.Split('-'); 
                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-"); 
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                     
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt"); 
                    double ItemWeight = Convert.ToDouble(TextItemWeightWP.Text.Trim());    
                  
                    //inventory calculation
                     
                    int InvenItemID = 0;
                    int InvenCategoryID = 0;
                    double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, StockOutWetNew = 0.00, FinalStock = 0.00, StockInWetNew = 0.00, FinalStockNew = 0.00;
                     
                    // update raw material

                        string makeSQL = " select * from MF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
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

                
                    if(ItemWeight <= FinalStock) {
                    // deduct from the inventory
                    StockOutWetNew = StockOutWet + ItemWeight;
                    FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;

                    string update_inven_mas = "update  MF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
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

                    // update fg material

                    string makeFgSQL = " select * from MF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeFgSQL);
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



                      // add from the inventory
                        StockInWetNew = StockInWet + ItemWeight;
                        FinalStockNew = (InitialStock + StockInWetNew) - StockOutWetNew;

                        string update_inven_fg = "update  MF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                        cmdu = new OracleCommand(update_inven_fg, conn);

                        OracleParameter[] objPrmInevenFg = new OracleParameter[5];
                        objPrmInevenFg[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                        objPrmInevenFg[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenFg[2] = cmdu.Parameters.Add("u_date", c_date);
                        objPrmInevenFg[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenFg[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();


                        string get_id = "select MF_MATERIAL_SHIFT_FGID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_id, conn);
                    int newShiftID = Int32.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_trans = "insert into  MF_MATERIAL_SHIFT_RM_TO_FG (MAT_SHIFT_ID,  ITEM_ID, ITEM_WEIGHT, ENTRY_DATE, REMARKS, CREATE_DATE, C_USER_ID, IS_ACTIVE) values  ('MS-' || LPAD(:NoShiftID, 6, '0') , :NoItemID, :NoItemWeight, TO_DATE(:EntryDate, 'DD/MM/YYYY'), :TextRemarks, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive)";
                    cmdi = new OracleCommand(insert_trans, conn);

                    OracleParameter[] objPrm = new OracleParameter[8];
                    objPrm[0] = cmdi.Parameters.Add("NoShiftID", newShiftID); 
                    objPrm[1] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[2] = cmdi.Parameters.Add("NoItemWeight", ItemWeight); 
                    objPrm[3] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[4] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                    objPrm[5] = cmdi.Parameters.Add("c_date", c_date);
                    objPrm[6] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[7] = cmdi.Parameters.Add("TextIsActive", ISActive);

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
                        alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Inventory" + ItemWeight +" - "+ FinalStock));
                        alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
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
            string ShiftForID = Session["user_data_id"].ToString();
                 
            string HtmlString = "";
            string makeSQL = " SELECT WMT.MAT_SHIFT_ID,  WI.ITEM_CODE || ' : ' || WI.ITEM_NAME AS ITEM_NAME, WMT.ITEM_WEIGHT, WMT.REMARKS, TO_CHAR(WMT.ENTRY_DATE,'dd-MON-yyyy') AS ENTRY_DATE, WMT.CREATE_DATE, WMT.UPDATE_DATE, WMT.IS_ACTIVE, WMT.IS_PRINT, TO_CHAR(WMT.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE FROM  MF_MATERIAL_SHIFT_RM_TO_FG WMT LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WMT.ITEM_ID  WHERE  WMT.MAT_SHIFT_ID = '" + ShiftForID + "' ";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;
            
            for (int i = 0; i < RowCount; i++)
            {
            TransanctionNo = dt.Rows[i]["MAT_SHIFT_ID"].ToString(); 
            string ItemName = dt.Rows[i]["ITEM_NAME"].ToString();
            string ItemWeight = string.Format("{0:n2}", Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"])); 
            EntryDateSlip = dt.Rows[i]["ENTRY_DATE"].ToString();  
            HtmlString += "<div style='float:left;width:775px;height:335px;margin:10px 20px 10px 25px;padding:10px 10px 0 0;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 15px; font-style: normal; font-variant: normal; font-weight: 700; line-height: 16px;border:black solid 1px'> ";
            HtmlString += "<div style='float:left;width:825px;margin-top:5px;height:82px;text-align:center;' ><img src='../../image/logo_from.png'/></div> ";
            HtmlString += "<div style='float:left;width:825px;height:20px;text-align:center;font-family:Times New Roman,Times, serif;font-size:17px;font-weight:700;' >Metal Factory Division</div> ";
            HtmlString += "<div style='float:left;width:825px;text-align:center;font-family:Times New Roman,Times, serif;font-size:16px;font-weight:700;' >Certificate Of Material Shift RM to FG</div> ";
         
            HtmlString += "<div style='float:left;width:340px;margin:15px 0 0 20px;text-align:left;' ><span style='font-family:Times New Roman,Times, serif;font-size:15px;font-weight:700;'>Shift No :</span> <span style='color:Red;font-size:15px;'>" + TransanctionNo + "</span></div> ";
            HtmlString += "<div style='float:left;width:407px;margin-top:15px;text-align:right;'><span style='font-family:Times New Roman;'>Shift Date :</span>" + EntryDateSlip + "</div> ";
            HtmlString += "<div style='float:left;width:750px;margin:6px 0 0 18px;'> "; 
            HtmlString += "<table border=1px; width=100%; cellpadding=5px;>";
            HtmlString += "<thead>";
            HtmlString += "<tr>";
            HtmlString += "<th><span style='font-family:Times New Roman;'>Item</span></th>";
            HtmlString += "<th><span style='font-family:Times New Roman;'>Material Weight (KG)</span></th>";
               
            HtmlString += " </tr>";
            HtmlString += "</thead>";
            HtmlString += "<tbody>"; 
            HtmlString += "<tr>";
            HtmlString += "<td align='center'>";
            HtmlString += "" + ItemName + ""; 
            HtmlString += "</td>"; 
            HtmlString += "<td align='center'>";
            HtmlString += "" + ItemWeight + "";
            HtmlString += "</td>";
            
            HtmlString += "</tr>"; 
            HtmlString += "</tbody>";
            HtmlString += "</table>";
                     
            HtmlString += "</div>";

            HtmlString += "<div style='float:left;width:340px;margin:65 0 0 20px;text-align:left;' ><span style='font-family:Times New Roman,Times, serif;font-size:15px;font-weight:700;'>Prepared By :</span></div> ";
            HtmlString += "<div style='float:left;width:407px;margin-top:65px;text-align:right;'><span style='font-family:Times New Roman;'>Approved By :</span></div> ";

        } 
            HtmlString += "</div>";
            HtmlString += "</div>";
            HtmlString += "</div>";

            // purchase master update for print
            int userID = Convert.ToInt32(Session["USER_ID"]); 
            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
            string update_user = "update  MF_MATERIAL_SHIFT_RM_TO_FG  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where MAT_SHIFT_ID = :NoSlipNoWp ";
            cmdi = new OracleCommand(update_user, conn);

            OracleParameter[] objPrm = new OracleParameter[4];
            objPrm[0] = cmdi.Parameters.Add("TextIsPrint", "Printed"); 
            objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
            objPrm[2] = cmdi.Parameters.Add("NoCuserID", userID); 
            objPrm[3] = cmdi.Parameters.Add("NoSlipNoWp", ShiftForID);

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
            string query = " SELECT ITEM_ID, ITEM_CODE || ' : ' || ITEM_NAME AS ITEM_NAME FROM MF_ITEM WHERE ITEM_ID NOT IN :ItemID AND IS_ACTIVE = 'Enable' AND IS_PURCHASE_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
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

            string makeSQL = " SELECT MAT_SHIFT_ID,  ITEM_ID, ITEM_WEIGHT, REMARKS, TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, C_USER_ID, CREATE_DATE, U_USER_ID, UPDATE_DATE, IS_ACTIVE, IS_PRINT, P_USER_ID, PRINT_DATE FROM MF_MATERIAL_SHIFT_RM_TO_FG WHERE MAT_SHIFT_ID = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                 
                DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();  
                TextItemWeightWP.Text  = decimal.Parse(dt.Rows[i]["ITEM_WEIGHT"].ToString()).ToString(".00"); 
                EntryDate.Text = dt.Rows[i]["ENTRY_DATE"].ToString(); 
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                 
               
            }

            conn.Close();
            DropDownItemID.Enabled = false; 

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
                    makeSQL = "  SELECT WMT.MAT_SHIFT_ID, WI.ITEM_CODE || ' : ' || WI.ITEM_NAME AS ITEM_NAME, WMT.ITEM_WEIGHT, WMT.REMARKS, WMT.ENTRY_DATE, WMT.CREATE_DATE, WMT.UPDATE_DATE, WMT.IS_ACTIVE, WMT.IS_PRINT, TO_CHAR(WMT.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE FROM  MF_MATERIAL_SHIFT_RM_TO_FG WMT  JOIN MF_ITEM WI ON WI.ITEM_ID = WMT.ITEM_ID ORDER BY  WMT.MAT_SHIFT_ID  ";
                }
                else
                {
                    if (DropDownSearchItemID.Text == "0")
                    {
                        makeSQL = " SELECT WMT.MAT_SHIFT_ID, WI.ITEM_CODE || ' : ' || WI.ITEM_NAME AS ITEM_NAME, WMT.ITEM_WEIGHT, WMT.REMARKS, WMT.ENTRY_DATE, WMT.CREATE_DATE, WMT.UPDATE_DATE, WMT.IS_ACTIVE, WMT.IS_PRINT, TO_CHAR(WMT.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE FROM  MF_MATERIAL_SHIFT_RM_TO_FG WMT LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WMT.ITEM_ID  where WMT.MAT_SHIFT_ID like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or to_char(WPM.ENTRY_DATE, 'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or to_char(WPM.ENTRY_DATE, 'mm/yyyy')  like '" + txtSearchEmp.Text + "%' or WPM.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ORDER BY WMT.MAT_SHIFT_ID asc";  // WPM.CREATE_DATE desc, WPM.UPDATE_DATE desc
                    }
                    else
                    {
                        makeSQL = " SELECT WMT.MAT_SHIFT_ID, WI.ITEM_CODE || ' : ' || WI.ITEM_NAME AS ITEM_NAME, WMT.ITEM_WEIGHT, WMT.REMARKS, WMT.ENTRY_DATE, WMT.CREATE_DATE, WMT.UPDATE_DATE, WMT.IS_ACTIVE, WMT.IS_PRINT, TO_CHAR(WMT.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE FROM  MF_MATERIAL_SHIFT_RM_TO_FG WMT LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WMT.ITEM_ID where  PI.ITEM_ID like '" + DropDownSearchItemID.Text + "%' AND (WMT.MAT_SHIFT_ID like '" + txtSearchEmp.Text + "%' or to_char(WPM.ENTRY_DATE, 'dd/mm/yyyy') like '" + txtSearchEmp.Text + "%' or to_char(WPM.ENTRY_DATE, 'mm/yyyy')  like '" + txtSearchEmp.Text + "%') ORDER BY WMT.MAT_SHIFT_ID asc";  // WPM.CREATE_DATE desc, WPM.UPDATE_DATE desc
                    }

                 //   alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView4D.DataSource = dt;
                GridView4D.DataKeyNames = new string[] { "MAT_SHIFT_ID" };
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
                    makeSQL = " SELECT  WI.ITEM_CODE || ' : ' || WI.ITEM_NAME AS ITEM_NAME, SUM(WMT.ITEM_WEIGHT) AS ITEM_WEIGHT FROM MF_MATERIAL_SHIFT_RM_TO_FG WMT LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WMT.ITEM_ID  WHERE to_char(WMT.ENTRY_DATE, 'mm/yyyy') = '" + MonthYear + "' GROUP BY WI.ITEM_CODE || ' : ' || WI.ITEM_NAME ORDER BY WI.ITEM_CODE || ' : ' || WI.ITEM_NAME   ";
                }
                else
                {
                    makeSQL = " SELECT  WI.ITEM_CODE || ' : ' || WI.ITEM_NAME AS ITEM_NAME, SUM(WMT.ITEM_WEIGHT) AS ITEM_WEIGHT FROM MF_MATERIAL_SHIFT_RM_TO_FG WMT LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WMT.ITEM_ID WHERE to_char(WMT.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear4.Text + "' GROUP BY WI.ITEM_CODE || ' : ' || WI.ITEM_NAME ORDER BY WI.ITEM_CODE || ' : ' || WI.ITEM_NAME  ";

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
                    GridView2.HeaderRow.Cells[2].HorizontalAlign = HorizontalAlign.Right;
                    //Calculate Sum and display in Footer Row
                    GridView2.FooterRow.Cells[0].Font.Bold = true;
                    GridView2.FooterRow.Cells[0].Text = "Grand Total";
                    GridView2.FooterRow.Cells[0].HorizontalAlign = HorizontalAlign.Right;
                     
                    decimal total_wt = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_WEIGHT"));
                    GridView2.FooterRow.Cells[1].Font.Bold = true;
                    GridView2.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[1].Text = total_wt.ToString("N2");

                 /*     decimal total_prod = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_NAME"));
                    GridView2.FooterRow.Cells[1].Font.Bold = true;
                    GridView2.FooterRow.Cells[1].HorizontalAlign = HorizontalAlign.Center;
                    GridView2.FooterRow.Cells[1].Text = total_prod.ToString("N0");  
                    decimal total_amt = dt.AsEnumerable().Sum(row => row.Field<decimal>("ITEM_AMOUNT"));
                    GridView2.FooterRow.Cells[3].Font.Bold = true;
                    GridView2.FooterRow.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[3].Text = total_amt.ToString("N2");

                    decimal total_avg = (total_amt / total_wt);
                    GridView2.FooterRow.Cells[4].Font.Bold = true;
                    GridView2.FooterRow.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                    GridView2.FooterRow.Cells[4].Text = total_avg.ToString("N2"); */
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
                string MatShiftForID = TextMatShiftForID.Text;
                int ItemID = Convert.ToInt32(DropDownItemID.Text);
                int ItemTransferID = 0;
              
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                string[] MakeEntryDateSplit = EntryDate.Text.Split('-');
                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                double ItemWeight = Math.Abs(Convert.ToDouble(TextItemWeightWP.Text.Trim()));



                // get old purchase item weight
                    int InvenItemID = 0;
                    int InvenCategoryID = 0;
                    double ItemWeightOld = 0.00; int ItemIdOld = 0;
                    double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, StockOutWetNew = 0.00, FinalStock = 0.00, StockInWetNew = 0.00, FinalStockNew = 0.00;
                    string makeSQL = " select  ITEM_ID, nvl(ITEM_WEIGHT,0) AS ITEM_WEIGHT  from MF_MATERIAL_SHIFT_RM_TO_FG WHERE MAT_SHIFT_ID  = '" + MatShiftForID + "'";
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
                    string makeSQLCheckRM = " select * from MF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
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


               
                    if (ItemWeight <= FinalStock)
                    {

                        // deduct old weight from the inventory
                        StockOutWetNew = Math.Abs(StockOutWet - ItemWeightOld);
                        FinalStockNew = (InitialStock + StockInWet) - StockOutWetNew;

                        string update_inven_old = "update  MF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
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

                        string makeSQLUpdateRM = " select * from MF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemIdOld + "'  ";
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

                         string update_inven_mas = "update  MF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
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
                        string update_user = "update  MF_MATERIAL_SHIFT_RM_TO_FG  set TRANSACTION_FOR_ID =:NoShiftForID, ITEM_ID =:NoItemID, ITEM_WEIGHT =:TextItemWeight, ITEM_TRANSFER_ID =:NoItemTransferID, ENTRY_DATE = TO_DATE(:EntryDate, 'DD/MM/YYYY'), REMARKS =:TextRemarks, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID =:NoCuserID, IS_ACTIVE =:TextIsActive WHERE MAT_SHIFT_ID =:NoPurchaseID ";
                        cmdi = new OracleCommand(update_user, conn);

                        OracleParameter[] objPrm = new OracleParameter[10]; 
                        objPrm[1] = cmdi.Parameters.Add("NoItemID", ItemID);
                        objPrm[2] = cmdi.Parameters.Add("TextItemWeight", ItemWeight);
                        objPrm[3] = cmdi.Parameters.Add("NoItemTransferID", ItemTransferID);
                        objPrm[4] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                        objPrm[5] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                        objPrm[6] = cmdi.Parameters.Add("u_date", u_date);
                        objPrm[7] = cmdi.Parameters.Add("NoCuserID", userID);
                        objPrm[8] = cmdi.Parameters.Add("TextIsActive", ISActive);
                        objPrm[9] = cmdi.Parameters.Add("NoPurchaseID", MatShiftForID);


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
                string MatShiftForID = TextMatShiftForID.Text; 
                double ItemWeight = Convert.ToDouble(TextItemWeightWP.Text.Trim());  
                double ItemWetInventory = ItemWeight;

                double FinalStockCheck = 0.00;
                string makeSQLCheckRM = " select * from MF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
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
                  
                string delete_user = " delete from MF_MATERIAL_SHIFT_RM_TO_FG where MAT_SHIFT_ID  = '" + MatShiftForID + "'"; 
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
            DropDownItemID.Text = "0"; 
            TextRemarks.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 
        }

        public void clearText()
        { 
            TextItemWeightWP.Text = "";  
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

       
        public void TextCheckDataProcess(object sender, EventArgs e)
        {
            // Check inventory data process in last month
            string MakeAsOnDate = EntryDate.Text;
            string[] MakeAsOnDateSplit = MakeAsOnDate.Split('-');
            String AsOnDateTemp = MakeAsOnDateSplit[0].Replace("/", "-");
            DateTime AsOnDateNewD = DateTime.ParseExact(AsOnDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string AsOnDateNew = AsOnDateNewD.ToString("dd-MM-yyyy");

            DateTime curDate = AsOnDateNewD;
            DateTime startDate = curDate.AddMonths(-1);
            DateTime LastDateTemp = curDate.AddDays(-(curDate.Day));
            string LastDate = LastDateTemp.ToString("dd-MM-yyyy");
            string LastMonthTemp = LastDateTemp.ToString("MM-yyyy");
            DateTime LastMonth = DateTime.ParseExact(LastMonthTemp, "MM-yyyy", CultureInfo.InvariantCulture);
            string CurrentMonthTemp = AsOnDateNewD.ToString("MM-yyyy");
            DateTime CurrentMonth = DateTime.ParseExact(CurrentMonthTemp, "MM-yyyy", CultureInfo.InvariantCulture);
            string SysMonthTemp = System.DateTime.Now.ToString("MM-yyyy");
            DateTime SysMonth = DateTime.ParseExact(SysMonthTemp, "MM-yyyy", CultureInfo.InvariantCulture);
            DateTime SysLastMonth = SysMonth.AddMonths(-1);

            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select ITEM_ID from MF_RM_STOCK_INVENTORY_HISTORY where TO_CHAR(TO_DATE(CREATE_DATE), 'dd-MM-yyyy')   = '" + LastDate + "'";
            cmd.CommandType = CommandType.Text;

            OracleDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                if (CurrentMonth == SysMonth || CurrentMonth == SysLastMonth)
                {
                 //   CheckEntryDate.Text = "";
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");
                 //   CheckEntryDate.Text = "<label class='control-label'><i class='fa fa fa-check-circle'></i></label>";
                 //   CheckEntryDate.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                 //   CheckEntryDate.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Please, Insert Data Current or last months.</label>";
                //    CheckEntryDate.ForeColor = System.Drawing.Color.Red;
                    EntryDate.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
            }
            else
            {
             //   CheckEntryDate.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Please, Complete Data process in last months (" + LastDate + "). It is required for insert current month data. </label>";
            //    CheckEntryDate.ForeColor = System.Drawing.Color.Red;
                EntryDate.Focus();
                BtnAdd.Attributes.Add("aria-disabled", "false");
                BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
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