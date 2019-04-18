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
    public partial class MfMatReceiving : System.Web.UI.Page
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
                        string makeSupplierSQL = " SELECT PARTY_ID, PARTY_ID  || ' - ' || PARTY_NAME || ' - ' || PARTY_VAT_NO AS PARTY_NAME  FROM MF_PARTY WHERE IS_ACTIVE = 'Enable' AND IS_PURCHASE_ACTIVE = 'Enable' ORDER BY PARTY_NAME ASC";
                        ds = ExecuteBySqlString(makeSupplierSQL);
                        dtSupplierID = (DataTable)ds.Tables[0];
                        DropDownSupplierID.DataSource = dtSupplierID;
                        DropDownSupplierID.DataValueField = "PARTY_ID";
                        DropDownSupplierID.DataTextField = "PARTY_NAME";
                        DropDownSupplierID.DataBind();
                        DropDownSupplierID.Items.Insert(0, new ListItem("Select  Supplier", "0"));


                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeItemSQL = " SELECT ITEM_ID, ITEM_CODE || ' : ' || ITEM_NAME AS ITEM_FULL_NAME FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' AND IS_PURCHASE_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        dsi = ExecuteBySqlString(makeItemSQL);
                        dtItemID = (DataTable)dsi.Tables[0];
                        DropDownItemID.DataSource = dtItemID;
                        DropDownItemID.DataValueField = "ITEM_ID";
                        DropDownItemID.DataTextField = "ITEM_FULL_NAME";
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
                         
                        TextSlipNo.Focus(); 
                        TextWtTotalPacking1.Attributes.Add("readonly", "readonly"); 

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
           //   { 
                if (IS_ADD_ACTIVE == "Enable")
                {  
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    int SupplierID = Convert.ToInt32(DropDownSupplierID.Text);
                    string SlipNo = TextSlipNo.Text; 
                    int ContainerSizeID = Convert.ToInt32(DropDownContainerSizeID.Text);
                    int ItemID = Convert.ToInt32(DropDownItemID.Text);
                    int ItemBinID = Convert.ToInt32(Request.Form[DropDownItemBinID.UniqueID]);
                    double Mat1stWeightMf = Convert.ToDouble(TextMat1stWeightMf.Text.Trim());
                    double Mat2ndWeightMf = Convert.ToDouble(TextMat2ndWeightMf.Text.Trim()); 
                    double ItemWtWb = Mat1stWeightMf - Mat2ndWeightMf;

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
  
                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-'); 
                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                  
                        if(DropDownPacking1.Text != "0") { 
                        string insert_packing = " insert into MF_MATERIAL_PACKING_HISTORY (WB_SLIP_NO, PACKING_ID, NUMBER_OF_PACK, PACK_PER_WEIGHT, TOTAL_WEIGHT,  CREATE_DATE, C_USER_ID ) values  ( :NoSlipID, :NoDropDownPacking1, :NoOfPacking1, :NoPerWtPacking1, :NoWtTotalPacking1, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID  )";
                        cmdi = new OracleCommand(insert_packing, conn);

                        OracleParameter[] objPr = new OracleParameter[7]; 
                        objPr[0] = cmdi.Parameters.Add("NoSlipID", SlipNo);   
                        objPr[1] = cmdi.Parameters.Add("NoDropDownPacking1", Convert.ToInt32(DropDownPacking1.Text));
                        objPr[2] = cmdi.Parameters.Add("NoOfPacking1", Convert.ToInt32(TextNoOfPacking1.Text));
                        objPr[3] = cmdi.Parameters.Add("NoPerWtPacking1", Convert.ToDouble(TextNoPerWtPacking1.Text));
                        objPr[4] = cmdi.Parameters.Add("NoWtTotalPacking1", Convert.ToDouble(TextWtTotalPacking1.Text));
                        objPr[5] = cmdi.Parameters.Add("c_date", c_date);
                        objPr[6] = cmdi.Parameters.Add("NoCuserID", userID); 

                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();
                        }
                         
                    string get_user_purchase_id = "select MF_PURCHASE_IMPROTID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_user_purchase_id, conn);
                    int newPurchaseID = Int32.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_purchase = "insert into  MF_PURCHASE_IMPROT (PURCHASE_IMPROT_ID, WB_SLIP_NO, PARTY_ID, CONTAINER_NO, CONTAINER_SIZE_ID, ITEM_ID, ITEM_BIN_ID, FIRST_WT_MF, SECOND_WT_MF, ITEM_WEIGHT_WB, ITEM_WEIGHT, ENTRY_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID ) values  ( :NoPurchaseID, :NoSlipID, :NoPartyID, :TextContainerNo, :NoContainerSizeID, :NoItemID, :NoItemBinID, :NoMat1stWeightMf, :NoMat2ndWeightMf, :NoItemWtWb,  :NoItemWeight, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 5)";
                    cmdi = new OracleCommand(insert_purchase, conn);

                    OracleParameter[] objPrm = new OracleParameter[15];
                    objPrm[0] = cmdi.Parameters.Add("NoPurchaseID", newPurchaseID);
                    objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNo);
                    objPrm[2] = cmdi.Parameters.Add("NoPartyID", SupplierID);
                    objPrm[3] = cmdi.Parameters.Add("TextContainerNo", TextContainerNo.Text);
                    objPrm[4] = cmdi.Parameters.Add("NoContainerSizeID", ContainerSizeID); 
                    objPrm[5] = cmdi.Parameters.Add("NoItemID", ItemID); 
                    objPrm[6] = cmdi.Parameters.Add("NoItemBinID", ItemBinID);  
                    objPrm[7] = cmdi.Parameters.Add("NoMat1stWeightMf", Mat1stWeightMf);   
                    objPrm[8] = cmdi.Parameters.Add("NoMat2ndWeightMf", Mat2ndWeightMf);                 
                    objPrm[9] = cmdi.Parameters.Add("NoItemWtWb", ItemWtWb); 
                    objPrm[10] = cmdi.Parameters.Add("NoItemWeight", Convert.ToDouble(TextItemWeightEx.Text));
                    objPrm[11] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[12] = cmdi.Parameters.Add("c_date", c_date);
                    objPrm[13] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[14] = cmdi.Parameters.Add("TextIsActive", ISActive); 

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
           //     }
           // catch
          //    {
          //     Response.Redirect("~/ParameterError.aspx");
          //  } 
          }

       
        protected void btnPrint_Click(object sender, EventArgs e)
        {
          if (IS_PRINT_ACTIVE == "Enable")
             {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_data_id"] = btn.CommandArgument;
                string SlipNo = Session["user_data_id"].ToString();
                string HtmlString = ""; 
                string makeSQL = " SELECT MPI.PURCHASE_IMPROT_ID, MPI.WB_SLIP_NO, MPI.CONTAINER_NO,  PP.PARTY_NAME, MPI.FIRST_WT_MF, MPI.SECOND_WT_MF, nvl(MPI.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, PI.ITEM_CODE || ' : ' || PI.ITEM_NAME AS ITEM_NAME, MPI.ITEM_WEIGHT, NPL.PACKING_NAME, MMPH.NUMBER_OF_PACK, MMPH.PACK_PER_WEIGHT, MMPH.TOTAL_WEIGHT, TO_CHAR(TO_DATE(MPI.ENTRY_DATE), 'dd-MON-YYYY') AS ENTRY_DATE FROM MF_PURCHASE_IMPROT MPI LEFT JOIN MF_MATERIAL_PACKING_HISTORY MMPH ON MMPH.WB_SLIP_NO = MPI.WB_SLIP_NO LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = MMPH.PACKING_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = MPI.ITEM_ID  LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = MPI.PARTY_ID WHERE MPI.WB_SLIP_NO = '" + SlipNo + "' ORDER BY PI.ITEM_ID, NPL.PACKING_ID ";

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
                    HtmlString += "<th colspan=4 style='font-size: 14px;text-decoration: underline;'>Material Receiving</th> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>Container No: " + dt.Rows[i]["CONTAINER_NO"].ToString() + "</td> ";
                    HtmlString += "<td colspan=2 style='text-align:right'>Received Date: " + dt.Rows[i]["ENTRY_DATE"].ToString() + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>Material: " + dt.Rows[i]["ITEM_NAME"].ToString() + "</td> ";
                    HtmlString += "<td style='text-align:right'></td> ";
                    HtmlString += "<td style='text-align:right'></td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td></td> ";
                    HtmlString += "<td style='text-align:right'>1st Weight (KG):</td> "; 
                    HtmlString += "<td style='text-align:right'>" + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["FIRST_WT_MF"].ToString())) + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "<td style='border-bottom:black solid 1px;text-align:right'>2nd Weight (KG):</td> "; 
                    HtmlString += "<td style='border-bottom:black solid 1px;text-align:right'>" + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["SECOND_WT_MF"].ToString())) + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "<td style='text-align:right'>Weigh-Bridge Net Weight (KG):</td> ";
                    HtmlString += "<td style='text-align:right'>" + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT_WB"].ToString())) + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "<td style='border-bottom:solid black 1px;text-align:right'>Less " + dt.Rows[i]["PACKING_NAME"].ToString() + ": " + dt.Rows[i]["NUMBER_OF_PACK"].ToString() + "x" + dt.Rows[i]["PACK_PER_WEIGHT"].ToString() + " (KG):</td> ";
                    HtmlString += "<td style='border-bottom:solid black 1px;text-align:right'>" + dt.Rows[i]["TOTAL_WEIGHT"].ToString() + "</td> ";
                    HtmlString += "</tr> ";
                    HtmlString += "<tr> ";
                    HtmlString += "<td>&nbsp;</td> ";
                    HtmlString += "<td style='font-weight: 700;text-align:right'>Net Weight:</td> "; 
                    HtmlString += "<td style='font-weight: 700;text-align:right'>" + string.Format("{0:n0}", Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString())) + "</td> ";
                    HtmlString += "</tr> "; 

                    HtmlString += "<tr valign='top'> ";
                    HtmlString += "<th colspan=4 style='font-weight: 700;'></th> ";
                    HtmlString += "</tr> ";
                    HtmlString += "</table> ";


                    HtmlString += "</div>";
                }

                // weigh-bridge & container update for print
                int userID = Convert.ToInt32(Session["USER_ID"]); 
            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
            string update_user = "update  MF_PURCHASE_IMPROT  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where WB_SLIP_NO = :NoSlipNo ";
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

   
        [WebMethod]
        public static List<ListItem> GetItemList()
        {
          //  OracleConnection conn = new OracleConnection(strConnString);
         //   conn.Open();
            string query = " SELECT * FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC ";
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
                 
            string makeSQL = " select PURCHASE_IMPROT_ID, WB_SLIP_NO, CONTAINER_NO, CONTAINER_SIZE_ID, PARTY_ID, FIRST_WT_MF, SECOND_WT_MF, nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, ITEM_ID, ITEM_BIN_ID, ITEM_WEIGHT, TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, IS_ACTIVE from MF_PURCHASE_IMPROT where WB_SLIP_NO  = '" + USER_DATA_ID + "'";
            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count; 

            for (int i = 0; i < RowCount; i++)
            {
                TextPrchaseImportID.Text = dt.Rows[i]["PURCHASE_IMPROT_ID"].ToString();   
                TextSlipNo.Text = dt.Rows[i]["WB_SLIP_NO"].ToString(); 
                DropDownSupplierID.Text = dt.Rows[i]["PARTY_ID"].ToString();
                TextContainerNo.Text = dt.Rows[i]["CONTAINER_NO"].ToString();
                DropDownContainerSizeID.Text = dt.Rows[i]["CONTAINER_SIZE_ID"].ToString(); 
                DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();

                DataTable dtItemID = new DataTable();
                DataSet dsi = new DataSet();
                string makeItemSQL = " SELECT ITEM_BIN_ID, ITEM_BIN_NAME || ' - Capacity: ' || CAPACITY_WEIGHT || ' (KG)'  AS ITEM_FULL_NAME FROM MF_ITEM_BIN WHERE IS_ACTIVE = 'Enable' AND ITEM_ID = '"+ Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString()) + "'  ORDER BY ITEM_BIN_ID ASC ";
                dsi = ExecuteBySqlString(makeItemSQL);
                dtItemID = (DataTable)dsi.Tables[0];
                DropDownItemBinID.DataSource = dtItemID;
                DropDownItemBinID.DataValueField = "ITEM_BIN_ID";
                DropDownItemBinID.DataTextField = "ITEM_FULL_NAME";
                DropDownItemBinID.DataBind();
                DropDownItemBinID.Items.Insert(0, new ListItem("Select  Item Bin ", "0"));

             //   DropDownItemBinID.Text = dt.Rows[i]["ITEM_BIN_ID"].ToString();                 
                TextItemWeightEx.Text = dt.Rows[i]["ITEM_WEIGHT"].ToString();                   
                EntryDate.Text = dt.Rows[i]["ENTRY_DATE"].ToString();
                TextMat1stWeightMf.Text  = dt.Rows[i]["FIRST_WT_MF"].ToString();
                TextMat2ndWeightMf.Text  = dt.Rows[i]["SECOND_WT_MF"].ToString(); 
                TextItemWtWb.Text  = dt.Rows[i]["ITEM_WEIGHT_WB"].ToString();
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);

                string makePackSQL = " select * from MF_MATERIAL_PACKING_HISTORY where WB_SLIP_NO  = '" + dt.Rows[i]["WB_SLIP_NO"].ToString() + "'";
                cmdl = new OracleCommand(makePackSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount2 = dt.Rows.Count;
                for (int j = 0; j < RowCount2; j++)
                {
                    if (j<1) {
                        DropDownPacking1.Text = dt.Rows[j]["PACKING_ID"].ToString();
                        TextNoOfPacking1.Text = dt.Rows[j]["NUMBER_OF_PACK"].ToString();
                        TextNoPerWtPacking1.Text = dt.Rows[j]["PACK_PER_WEIGHT"].ToString();
                        TextWtTotalPacking1.Text = dt.Rows[j]["TOTAL_WEIGHT"].ToString();
                    }
                                  
                }

             }

            conn.Close();
            Display();
            CheckSlipNo.Text = "";
            alert_box.Visible = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

          //  TextSlipNo.Attributes.Add("readonly", "readonly");

            //    }
            //     catch
            //   {
            //      Response.Redirect("~/ParameterError.aspx");
            //   } 
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
                 
                int PrchaseImportID = Convert.ToInt32(TextPrchaseImportID.Text); 
                int SupplierID = Convert.ToInt32(DropDownSupplierID.Text);
                string SlipNo = TextSlipNo.Text;
                int ContainerSizeID = Convert.ToInt32(DropDownContainerSizeID.Text);
                int ItemID = Convert.ToInt32(DropDownItemID.Text);
                int ItemBinID = Convert.ToInt32(Request.Form[DropDownItemBinID.UniqueID]);
                double Mat1stWeightMf = Convert.ToDouble(TextMat1stWeightMf.Text.Trim());
                double Mat2ndWeightMf = Convert.ToDouble(TextMat2ndWeightMf.Text.Trim());
                double ItemWtWb = Mat1stWeightMf - Mat2ndWeightMf;
                      
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                  
                string MakeEntryDate = EntryDate.Text;
                string[] MakeEntryDateSplit = MakeEntryDate.Split('-'); 
                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");


                string delete_pack = " delete from MF_MATERIAL_PACKING_HISTORY where WB_SLIP_NO  = '" + TextSlipNo.Text + "'";
                cmdi = new OracleCommand(delete_pack, conn);
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                if (DropDownPacking1.Text != "0")
                {
                    string insert_packing = "insert into  MF_MATERIAL_PACKING_HISTORY (WB_SLIP_NO, PACKING_ID, NUMBER_OF_PACK, PACK_PER_WEIGHT, TOTAL_WEIGHT,  CREATE_DATE, C_USER_ID ) values  ( :NoSlipID, :NoDropDownPacking1, :NoOfPacking1, :NoPerWtPacking1, :NoWtTotalPacking1, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID  )";
                    cmdi = new OracleCommand(insert_packing, conn);

                    OracleParameter[] objPr = new OracleParameter[7];
                    objPr[0] = cmdi.Parameters.Add("NoSlipID", TextSlipNo.Text);
                    objPr[1] = cmdi.Parameters.Add("NoDropDownPacking1", Convert.ToInt32(DropDownPacking1.Text));
                    objPr[2] = cmdi.Parameters.Add("NoOfPacking1", Convert.ToInt32(TextNoOfPacking1.Text));
                    objPr[3] = cmdi.Parameters.Add("NoPerWtPacking1", Convert.ToDouble(TextNoPerWtPacking1.Text));
                    objPr[4] = cmdi.Parameters.Add("NoWtTotalPacking1", Convert.ToDouble(TextWtTotalPacking1.Text));
                    objPr[5] = cmdi.Parameters.Add("c_date", u_date);
                    objPr[6] = cmdi.Parameters.Add("NoCuserID", userID);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                }

                

                string update_user = "update  MF_PURCHASE_IMPROT set WB_SLIP_NO =:NoSlipID, PARTY_ID =:NoPartyID, CONTAINER_NO =:NoContainerNo, CONTAINER_SIZE_ID =:NoContainerSizeID, ITEM_ID =:NoItemID, ITEM_BIN_ID =:NoItemBinID, FIRST_WT_MF =:NoMat1stWeightMf, SECOND_WT_MF =:NoMat2ndWeightMf, ITEM_WEIGHT_WB =:NoItemWtWb, ITEM_WEIGHT =:NoItemWeight, ENTRY_DATE = TO_DATE(:EntryDate, 'DD/MM/YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID =:NoCuserID, IS_ACTIVE = :TextIsActive where PURCHASE_IMPROT_ID = :NoPrchaseImportID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[15]; 
                objPrm[0] = cmdi.Parameters.Add("NoSlipID", SlipNo);
                objPrm[1] = cmdi.Parameters.Add("NoPartyID", SupplierID);
                objPrm[2] = cmdi.Parameters.Add("NoContainerNo", TextContainerNo.Text);
                objPrm[3] = cmdi.Parameters.Add("NoContainerSizeID", ContainerSizeID);
                objPrm[4] = cmdi.Parameters.Add("NoItemID", ItemID);
                objPrm[5] = cmdi.Parameters.Add("NoItemBinID", ItemBinID);
                objPrm[6] = cmdi.Parameters.Add("NoMat1stWeightMf", Mat1stWeightMf);
                objPrm[7] = cmdi.Parameters.Add("NoMat2ndWeightMf", Mat2ndWeightMf);
                objPrm[8] = cmdi.Parameters.Add("NoItemWtWb", ItemWtWb);
                objPrm[9] = cmdi.Parameters.Add("NoItemWeight", Convert.ToDouble(TextItemWeightEx.Text));
                objPrm[10] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                objPrm[11] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[12] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[13] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[14] = cmdi.Parameters.Add("NoPrchaseImportID", PrchaseImportID);

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
                    int PrchaseImportID = Convert.ToInt32(TextPrchaseImportID.Text); 
                    string delete_user = " delete from MF_PURCHASE_IMPROT where PURCHASE_IMPROT_ID  = '" + PrchaseImportID + "'";
                    cmdi = new OracleCommand(delete_user, conn);
                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    string delete_pack = " delete from MF_MATERIAL_PACKING_HISTORY where WB_SLIP_NO  = '" + TextSlipNo.Text + "'";
                    cmdi = new OracleCommand(delete_pack, conn);
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
                string MonthYear = System.DateTime.Now.ToString("MM/yyyy");
                if (txtSearchEmp.Text == "")
                {
                    makeSQL = " SELECT MPI.WB_SLIP_NO, MPI.ENTRY_DATE, MPI.CONTAINER_NO, NCS.CONTAINER_SIZE, PP.PARTY_NAME, PI.ITEM_CODE || ' : ' || PI.ITEM_NAME AS ITEM_NAME, MPI.ITEM_WEIGHT_WB,  MPI.ITEM_WEIGHT, MIB.ITEM_BIN_NAME, MIB.CAPACITY_WEIGHT, NPL.PACKING_NAME, MMPH.NUMBER_OF_PACK, MMPH.PACK_PER_WEIGHT,  MMPH.TOTAL_WEIGHT, MPI.IS_ACTIVE, MPI.CREATE_DATE, MPI.UPDATE_DATE, MPI.IS_PRINT, TO_CHAR(MPI.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE,  MPI.FIRST_APPROVED_IS FROM MF_PURCHASE_IMPROT MPI LEFT JOIN MF_MATERIAL_PACKING_HISTORY MMPH ON MMPH.WB_SLIP_NO = MPI.WB_SLIP_NO LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = MMPH.PACKING_ID LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = MPI.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = MPI.ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = MPI.CONTAINER_SIZE_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MPI.ITEM_BIN_ID ORDER BY MPI.WB_SLIP_NO desc, MPI.ITEM_ID ASC  "; // OR IS_INVENTORY_STATUS = 'Transit' / WHERE to_char(MPI.ENTRY_DATE, 'mm/yyyy') = '" + MonthYear + "'
                }
                else
                {
                    
                    makeSQL = " SELECT MPI.WB_SLIP_NO, MPI.ENTRY_DATE, MPI.CONTAINER_NO, NCS.CONTAINER_SIZE, PP.PARTY_NAME, PI.ITEM_CODE || ' : ' || PI.ITEM_NAME AS ITEM_NAME, MPI.ITEM_WEIGHT_WB,  MPI.ITEM_WEIGHT, MIB.ITEM_BIN_NAME, MIB.CAPACITY_WEIGHT,  NPL.PACKING_NAME, MMPH.NUMBER_OF_PACK, MMPH.PACK_PER_WEIGHT,  MMPH.TOTAL_WEIGHT, MPI.IS_ACTIVE, MPI.CREATE_DATE, MPI.UPDATE_DATE, MPI.IS_PRINT, TO_CHAR(MPI.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE,  MPI.FIRST_APPROVED_IS FROM MF_PURCHASE_IMPROT MPI LEFT JOIN MF_MATERIAL_PACKING_HISTORY MMPH ON MMPH.WB_SLIP_NO = MPI.WB_SLIP_NO LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = MMPH.PACKING_ID LEFT JOIN MF_PARTY PP ON PP.PARTY_ID = MPI.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = MPI.ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = MPI.CONTAINER_SIZE_ID LEFT JOIN MF_ITEM_BIN MIB ON MIB.ITEM_BIN_ID = MPI.ITEM_BIN_ID  WHERE MPI.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or MPI.CONTAINER_NO like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or MIB.ITEM_BIN_NAME like '" + txtSearchEmp.Text + "%' or to_char(MPI.ENTRY_DATE, 'dd/mm/yyyy')  like '" + txtSearchEmp.Text + "%' or MPI.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ORDER BY MPI.WB_SLIP_NO desc, MPI.ITEM_ID asc ";   
                     

                    alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "WB_SLIP_NO" };
                GridView1.DataBind();
              
                // alert_box.Visible = false;
            }
        }
         
        protected void gridViewFileInformation_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow Row in GridView1.Rows)
            {
                string isCheck = (Row.FindControl("IsCmoCheckLink") as Label).Text;
                string isCheckPrint = (Row.FindControl("IsPrintedCheckLink") as Label).Text;  

                if (isCheck == "Complete" )  // || isCheckPrint == "Printed"
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
                string CurrentMonth = System.DateTime.Now.ToString("MM/yyyy");
                if (TextMonthYear4.Text == "")
                {
                    makeSQL = " SELECT PI.ITEM_CODE || ' : ' || PI.ITEM_NAME AS ITEM_NAME, nvl(sum(MPI.ITEM_WEIGHT), 0) AS ITEM_WEIGHT FROM MF_PURCHASE_IMPROT MPI LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = MPI.ITEM_ID  WHERE PI.ITEM_NAME IS NOT NULL AND to_char(MPI.ENTRY_DATE, 'mm/yyyy') = '" + CurrentMonth + "' GROUP BY  PI.ITEM_CODE || ' : ' || PI.ITEM_NAME ";
                }
                else
                {
                    makeSQL = " SELECT PI.ITEM_CODE || ' : ' || PI.ITEM_NAME AS ITEM_NAME, nvl(sum(MPI.ITEM_WEIGHT), 0) AS ITEM_WEIGHT FROM MF_PURCHASE_IMPROT MPI LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = MPI.ITEM_ID  WHERE PI.ITEM_NAME IS NOT NULL AND to_char(MPI.ENTRY_DATE, 'mm/yyyy') = '" + TextMonthYear4.Text + "' GROUP BY  PI.ITEM_CODE || ' : ' || PI.ITEM_NAME ";
                          
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


    
        public void clearTextField(object sender, EventArgs e)
        {
            TextSlipNo.Text = "";
            TextContainerNo.Text = ""; 
            DropDownSupplierID.Text = "0"; 
            CheckSlipNo.Text = "";
            DropDownSupplierID.Text = "0"; 
            DropDownItemID.Text = "0"; 
            DropDownPacking1.Text = "0";
            TextNoOfPacking1.Text = "0";
            TextNoPerWtPacking1.Text = "0";
            TextWtTotalPacking1.Text = ""; 
            TextItemWtWb.Text = "";
            TextItemWeightEx.Text = "";
            EntryDate.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void clearText()
        {
            TextSlipNo.Text = "";
            TextContainerNo.Text = ""; 
            DropDownSupplierID.Text = "0"; 
            CheckSlipNo.Text = ""; 
            DropDownSupplierID.Text = "0"; 
            DropDownItemID.Text = "0"; 
            DropDownPacking1.Text = "0";
            TextNoOfPacking1.Text = "0";
            TextNoPerWtPacking1.Text = "0"; 
            TextWtTotalPacking1.Text = ""; 
            TextItemWtWb.Text = "";
            TextItemWeightEx.Text = "";
            EntryDate.Text = "";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

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
                    cmd.CommandText = "select WB_SLIP_NO from MF_PURCHASE_IMPROT where WB_SLIP_NO = '" + SlipNo + "'";
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