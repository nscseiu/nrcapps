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
    public partial class PfExpContainer : System.Web.UI.Page
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
                        string makeSupplierSQL = " SELECT PARTY_ID, PARTY_NAME || ' - ' || PARTY_ID || ' - ' || PARTY_VAT_NO AS PARTY_NAME  FROM PF_PARTY WHERE IS_ACTIVE = 'Enable' ORDER BY PARTY_NAME ASC";
                        ds = ExecuteBySqlString(makeSupplierSQL);
                        dtSupplierID = (DataTable)ds.Tables[0];
                        DropDownSupplierID.DataSource = dtSupplierID;
                        DropDownSupplierID.DataValueField = "PARTY_ID";
                        DropDownSupplierID.DataTextField = "PARTY_NAME";
                        DropDownSupplierID.DataBind();
                        DropDownSupplierID.Items.Insert(0, new ListItem("Select  Customer", "0"));
                          
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
                        DropDownSubItemID.Items.FindByValue("1").Selected = true;
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

                        DropDownPacking2.DataSource = dtPacking1ID;
                        DropDownPacking2.DataValueField = "PACKING_ID";
                        DropDownPacking2.DataTextField = "PACKING_NAME";
                        DropDownPacking2.DataBind();
                        DropDownPacking2.Items.Insert(0, new ListItem("Select Packing List", "0"));
                        DropDownPacking2.Items.FindByValue("2").Selected = true;

                        TextSlipNo.Focus();
                        TextNoOfPacking2.Text = "0";
                        TextNoPerWtPacking2.Text = "0";
                        TextWtTotalPacking2.Text = "0";
                        TextWtTotalPacking1.Attributes.Add("readonly", "readonly");
                        TextWtTotalPacking2.Attributes.Add("readonly", "readonly"); 

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
                    int SalesmanID = Convert.ToInt32(DropDownSalesmanID.Text);
                    int ContainerSizeID = Convert.ToInt32(DropDownContainerSizeID.Text);

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                  
                        if(DropDownPacking1.Text != "0") { 
                        string insert_packing = "insert into  PF_EXPORT_PACKING_HISTORY (WB_SLIP_NO, PACKING_ID, NUMBER_OF_PACK, PACK_PER_WEIGHT, TOTAL_WEIGHT,  CREATE_DATE, C_USER_ID ) values  ( :NoSlipID, :NoDropDownPacking1, :NoOfPacking1, :NoPerWtPacking1, :NoWtTotalPacking1, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID  )";
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

                        if (DropDownPacking2.Text != "0")
                        {
                            string insert_packing = "insert into  PF_EXPORT_PACKING_HISTORY (WB_SLIP_NO, PACKING_ID, NUMBER_OF_PACK, PACK_PER_WEIGHT, TOTAL_WEIGHT,  CREATE_DATE, C_USER_ID ) values  ( :NoSlipID, :NoDropDownPacking1, :NoOfPacking1, :NoPerWtPacking1, :NoWtTotalPacking1, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID  )";
                            cmdi = new OracleCommand(insert_packing, conn);

                            OracleParameter[] objPr = new OracleParameter[7];
                            objPr[0] = cmdi.Parameters.Add("NoSlipID", SlipNo);
                            objPr[1] = cmdi.Parameters.Add("NoDropDownPacking1", Convert.ToInt32(DropDownPacking2.Text));
                            objPr[2] = cmdi.Parameters.Add("NoOfPacking1", Convert.ToInt32(TextNoOfPacking2.Text));
                            objPr[3] = cmdi.Parameters.Add("NoPerWtPacking1", Convert.ToDouble(TextNoPerWtPacking2.Text));
                            objPr[4] = cmdi.Parameters.Add("NoWtTotalPacking1", Convert.ToDouble(TextWtTotalPacking2.Text));
                            objPr[5] = cmdi.Parameters.Add("c_date", c_date);
                            objPr[6] = cmdi.Parameters.Add("NoCuserID", userID);

                            cmdi.ExecuteNonQuery();
                            cmdi.Parameters.Clear();
                            cmdi.Dispose();
                        }


                    string get_user_purchase_id = "select PF_EXPORT_CONTAINERID_SEQ.nextval from dual";
                    cmdsp = new OracleCommand(get_user_purchase_id, conn);
                    int newPurchaseID = Int32.Parse(cmdsp.ExecuteScalar().ToString());
                    string insert_purchase = "insert into  PF_EXPORT_WBSLIP_CON (EXP_WBCON_ID, WB_SLIP_NO, CONTAINER_NO, CONTAINER_SIZE_ID, SEAL_NO, REL_ORDER_NO, PARTY_ID, SALESMAN_ID, ITEM_WEIGHT_WB, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, DISPATCH_DATE, CREATE_DATE, C_USER_ID, IS_ACTIVE, DIVISION_ID, IS_INVENTORY_STATUS ) values  ( :NoPurchaseID, :NoSlipID, :TextContainerNo, :NoContainerSizeID, :TextSealNo, :TextRelOrderNo, :NoPartyID, :NoSalesmanID, :TextItemWtWb, :NoItemID, :NoSubItemID, :NoItemWeight, TO_DATE(:EntryDate, 'DD/MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, 3, 'Transit'  )";
                    cmdi = new OracleCommand(insert_purchase, conn);

                    OracleParameter[] objPrm = new OracleParameter[16];
                    objPrm[0] = cmdi.Parameters.Add("NoPurchaseID", newPurchaseID);
                    objPrm[1] = cmdi.Parameters.Add("NoSlipID", SlipNo);
                    objPrm[2] = cmdi.Parameters.Add("TextContainerNo", TextContainerNo.Text);
                    objPrm[3] = cmdi.Parameters.Add("NoContainerSizeID", ContainerSizeID);
                    objPrm[4] = cmdi.Parameters.Add("TextSealNo", TextSealNo.Text);
                    objPrm[5] = cmdi.Parameters.Add("TextRelOrderNo", TextRelOrderNo.Text);
                    objPrm[6] = cmdi.Parameters.Add("NoSalesmanID", SalesmanID);
                    objPrm[7] = cmdi.Parameters.Add("NoPartyID", SupplierID);
                    objPrm[8] = cmdi.Parameters.Add("TextItemWtWb", TextItemWtWb.Text);
                    objPrm[9] = cmdi.Parameters.Add("NoItemID", Convert.ToInt32(DropDownItemID.Text));
                    objPrm[10] = cmdi.Parameters.Add("NoSubItemID", Convert.ToInt32(DropDownSubItemID.Text));
                    objPrm[11] = cmdi.Parameters.Add("NoItemWeight", Convert.ToDouble(TextItemWeightEx.Text));
                    objPrm[12] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPrm[13] = cmdi.Parameters.Add("c_date", c_date);
                    objPrm[14] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPrm[15] = cmdi.Parameters.Add("TextIsActive", ISActive);

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
            string HtmlString = "";
            string makeSQL = " SELECT PEWC.EXP_WBCON_ID, PEWC.WB_SLIP_NO, PEWC.CONTAINER_NO, PP.PARTY_NAME, PEWC.SEAL_NO, PEWC.REL_ORDER_NO, nvl(PEWC.ITEM_WEIGHT_WB, 0) AS ITEM_WEIGHT_WB, PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PEWC.ITEM_WEIGHT, NPL.PACKING_NAME, PEPH.NUMBER_OF_PACK, PEPH.PACK_PER_WEIGHT, PEPH.TOTAL_WEIGHT FROM PF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_EXPORT_PACKING_HISTORY PEPH ON PEPH.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = PEPH.PACKING_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWC.SUB_ITEM_ID LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PEWC.PARTY_ID WHERE PEWC.WB_SLIP_NO = '" + TextSlipNo.Text + "' ORDER BY PI.ITEM_ID, NPL.PACKING_ID ";

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

                    HtmlString += "<div style='float:left;width:260px;height:28px;margin:0px 0 0 200px;text-align:center;'> ";
                HtmlString += "" + dt.Rows[i]["ITEM_NAME"].ToString() + "-" + dt.Rows[i]["SUB_ITEM_NAME"].ToString() + "";
                    HtmlString += "</div> ";
            }
          
            for (int i = 0; i < RowCount; i++)
                {     
                           NoOfPacks += Convert.ToDouble(dt.Rows[i]["NUMBER_OF_PACK"].ToString());
                           WtPerPacks += Convert.ToDouble(dt.Rows[i]["PACK_PER_WEIGHT"].ToString());
                           TotalPacksWt += Convert.ToDouble(dt.Rows[i]["TOTAL_WEIGHT"].ToString()); 
                           TotalItemWt = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());

                HtmlString += "<div style='float:left;width:260px;height:20px;margin-left:187px;padding:5px 0 0 5px;text-align:center;border:black solid 1px;'> "; 
                HtmlString += "" + dt.Rows[i]["NUMBER_OF_PACK"].ToString() + " " + dt.Rows[i]["PACKING_NAME"].ToString() + " * " + dt.Rows[i]["PACK_PER_WEIGHT"].ToString() + " = " + dt.Rows[i]["TOTAL_WEIGHT"].ToString() +  ""; 
                HtmlString += "</div> ";
                }
               

                HtmlString += "<div style='float:left;width:260px;height:20px;margin-left:80px;padding:40px 0 0 5px;text-align:center;'> "; 
                HtmlString += "SEAL NO: &nbsp;&nbsp;&nbsp;&nbsp;" + SealNo +  ""; 
                HtmlString += "</div> ";

                HtmlString += "</div> "; 
                HtmlString += "<div style='float:left;width:310px;'> ";

                for (int i = 0; i < RowCount; i++)
                {
                    HtmlString += "<div style='float:left;width:200px;height:20px;margin:0 0 0 50px;'> "+ dt.Rows[i]["PACKING_NAME"].ToString() + ": " + dt.Rows[i]["TOTAL_WEIGHT"].ToString() + "</div> "; 
                }
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
            string SlipNo = TextSlipNo.Text; 
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
                 
            string makeSQL = " select EXP_WBCON_ID, WB_SLIP_NO, CONTAINER_NO, CONTAINER_SIZE_ID, PARTY_ID,  SEAL_NO, REL_ORDER_NO,  SALESMAN_ID, nvl(ITEM_WEIGHT_WB,0) AS ITEM_WEIGHT_WB, ITEM_ID, SUB_ITEM_ID, ITEM_WEIGHT, TO_CHAR(DISPATCH_DATE,'dd/mm/yyyy') AS DISPATCH_DATE, IS_ACTIVE from PF_EXPORT_WBSLIP_CON where WB_SLIP_NO  = '" + USER_DATA_ID + "'";
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
                TextRelOrderNo.Text = dt.Rows[i]["REL_ORDER_NO"].ToString();
                DropDownSalesmanID.Text = dt.Rows[i]["SALESMAN_ID"].ToString(); 
                DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();    
                DropDownSubItemID.Text = dt.Rows[i]["SUB_ITEM_ID"].ToString();
                TextItemWeightEx.Text = dt.Rows[i]["ITEM_WEIGHT"].ToString();                   
                EntryDate.Text = dt.Rows[i]["DISPATCH_DATE"].ToString();
                TextItemWtWb.Text  = dt.Rows[i]["ITEM_WEIGHT_WB"].ToString();
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);

                string makePackSQL = " select * from PF_EXPORT_PACKING_HISTORY where WB_SLIP_NO  = '" + dt.Rows[i]["WB_SLIP_NO"].ToString() + "'";
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
                    else {
                        DropDownPacking2.Text = dt.Rows[j]["PACKING_ID"].ToString();
                        TextNoOfPacking2.Text = dt.Rows[j]["NUMBER_OF_PACK"].ToString();
                        TextNoPerWtPacking2.Text = dt.Rows[j]["PACK_PER_WEIGHT"].ToString();
                        TextWtTotalPacking2.Text = dt.Rows[j]["TOTAL_WEIGHT"].ToString();
                    }                 
                }

             }

            conn.Close();
            Display();
            CheckSlipNo.Text = "";
            alert_box.Visible = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

            TextSlipNo.Attributes.Add("readonly", "readonly");

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
                int ExWbConID = Convert.ToInt32(TextExWbConID.Text);
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                int ContainerSizeID = Convert.ToInt32(DropDownContainerSizeID.Text);
                string MakeEntryDate = EntryDate.Text;
                string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");


                string delete_pack = " delete from PF_EXPORT_PACKING_HISTORY where WB_SLIP_NO  = '" + TextSlipNo.Text + "'";
                cmdi = new OracleCommand(delete_pack, conn);
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                if (DropDownPacking1.Text != "0")
                {
                    string insert_packing = "insert into  PF_EXPORT_PACKING_HISTORY (WB_SLIP_NO, PACKING_ID, NUMBER_OF_PACK, PACK_PER_WEIGHT, TOTAL_WEIGHT,  CREATE_DATE, C_USER_ID ) values  ( :NoSlipID, :NoDropDownPacking1, :NoOfPacking1, :NoPerWtPacking1, :NoWtTotalPacking1, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID  )";
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

                if (DropDownPacking2.Text != "0")
                {
                    string insert_packing = "insert into  PF_EXPORT_PACKING_HISTORY (WB_SLIP_NO, PACKING_ID, NUMBER_OF_PACK, PACK_PER_WEIGHT, TOTAL_WEIGHT,  CREATE_DATE, C_USER_ID ) values  ( :NoSlipID, :NoDropDownPacking1, :NoOfPacking1, :NoPerWtPacking1, :NoWtTotalPacking1, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID  )";
                    cmdi = new OracleCommand(insert_packing, conn);

                    OracleParameter[] objPr = new OracleParameter[7];
                    objPr[0] = cmdi.Parameters.Add("NoSlipID", TextSlipNo.Text);
                    objPr[1] = cmdi.Parameters.Add("NoDropDownPacking1", Convert.ToInt32(DropDownPacking2.Text));
                    objPr[2] = cmdi.Parameters.Add("NoOfPacking1", Convert.ToInt32(TextNoOfPacking2.Text));
                    objPr[3] = cmdi.Parameters.Add("NoPerWtPacking1", Convert.ToDouble(TextNoPerWtPacking2.Text));
                    objPr[4] = cmdi.Parameters.Add("NoWtTotalPacking1", Convert.ToDouble(TextWtTotalPacking2.Text));
                    objPr[5] = cmdi.Parameters.Add("c_date", u_date);
                    objPr[6] = cmdi.Parameters.Add("NoCuserID", userID);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                }

                string update_user = "update  PF_EXPORT_WBSLIP_CON  set WB_SLIP_NO = :NoSlipNo, CONTAINER_NO = :NoContainerNo, CONTAINER_SIZE_ID = :NoContainerSize, SEAL_NO = :TextSealNo, REL_ORDER_NO = :TextRelOrderNo, ITEM_WEIGHT_WB = :NoItemWeightWb, SALESMAN_ID =: NoSalesmanID,  PARTY_ID =:NoPartyID, ITEM_ID =:NoItemID, SUB_ITEM_ID =:NoSubItemID, ITEM_WEIGHT =:NoItemWeight, DISPATCH_DATE = TO_DATE(:DispatchDate, 'DD/MM/YYYY'), UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID, IS_ACTIVE = :TextIsActive where EXP_WBCON_ID = :NoExWbConID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[16];
                objPrm[0] = cmdi.Parameters.Add("NoSlipNo", TextSlipNo.Text);
                objPrm[1] = cmdi.Parameters.Add("NoContainerNo", TextContainerNo.Text);
                objPrm[2] = cmdi.Parameters.Add("NoContainerSize", ContainerSizeID);
                objPrm[3] = cmdi.Parameters.Add("TextSealNo", TextSealNo.Text);
                objPrm[4] = cmdi.Parameters.Add("TextRelOrderNo", TextRelOrderNo.Text);
                objPrm[5] = cmdi.Parameters.Add("NoItemWeightWb", TextItemWtWb.Text);
                objPrm[6] = cmdi.Parameters.Add("NoSalesmanID", DropDownSalesmanID.Text);
                objPrm[7] = cmdi.Parameters.Add("NoPartyID", DropDownSupplierID.Text);
                objPrm[8] = cmdi.Parameters.Add("NoItemID", Convert.ToInt32(DropDownItemID.Text));
                objPrm[9] = cmdi.Parameters.Add("NoSubItemID", Convert.ToInt32(DropDownSubItemID.Text));
                objPrm[10] = cmdi.Parameters.Add("NoItemWeight", Convert.ToDouble(TextItemWeightEx.Text));
                objPrm[11] = cmdi.Parameters.Add("DispatchDate", EntryDateNew);
                objPrm[12] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[13] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[14] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[15] = cmdi.Parameters.Add("NoExWbConID", ExWbConID);

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
                    string delete_user = " delete from PF_EXPORT_WBSLIP_CON where EXP_WBCON_ID  = '" + ExWbConID + "'";
                    cmdi = new OracleCommand(delete_user, conn);
                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    string delete_pack = " delete from PF_EXPORT_PACKING_HISTORY where WB_SLIP_NO  = '" + TextSlipNo.Text + "'";
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
                string MonthYear = System.DateTime.Now.ToString("yyyy/MM");
                DateTime ThreeMonthBeforeTemp = DateTime.Now.AddMonths(-1);
                string ThreeMonthBefore = ThreeMonthBeforeTemp.ToString("yyyy/MM");

                if (txtSearchEmp.Text == "")
                {
                    makeSQL = " SELECT PEWC.WB_SLIP_NO, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.SEAL_NO, PEWC.REL_ORDER_NO, PP.PARTY_NAME,  PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PEWC.ITEM_WEIGHT_WB,  PEWC.ITEM_WEIGHT,  NPL.PACKING_NAME, PEPH.NUMBER_OF_PACK, PEPH.PACK_PER_WEIGHT,  PEPH.TOTAL_WEIGHT, PS.SALESMAN_NAME, PEWC.IS_ACTIVE ,  PEWC.IS_INVENTORY_STATUS,  PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT,  PEWC.PRINT_DATE,  PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO, PEWC.IS_ACTIVE_PRICING FROM PF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_EXPORT_PACKING_HISTORY PEPH ON PEPH.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = PEPH.PACKING_ID LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PEWC.PARTY_ID LEFT JOIN PF_SALESMAN PS ON PS.SALESMAN_ID = PEWC.SALESMAN_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWC.SUB_ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID   WHERE to_char(PEWC.DISPATCH_DATE, 'yyyy/mm') between '" + ThreeMonthBefore + "' AND '" + MonthYear + "'  ORDER BY PEWC.WB_SLIP_NO desc, PEWC.ITEM_ID ASC  "; // OR IS_INVENTORY_STATUS = 'Transit' / WHERE to_char(PEWC.DISPATCH_DATE, 'mm/yyyy') = '" + MonthYear + "'
                }
                else
                {
                    if (DropDownIsInven.Text == "0")
                    {
                        makeSQL = "  SELECT PEWC.WB_SLIP_NO, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.SEAL_NO, PEWC.REL_ORDER_NO, PP.PARTY_NAME,  PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PEWC.ITEM_WEIGHT_WB,  PEWC.ITEM_WEIGHT,  NPL.PACKING_NAME, PEPH.NUMBER_OF_PACK, PEPH.PACK_PER_WEIGHT,  PEPH.TOTAL_WEIGHT, PS.SALESMAN_NAME, PEWC.IS_ACTIVE ,  PEWC.IS_INVENTORY_STATUS,  PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT,  PEWC.PRINT_DATE,  PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO, PEWC.IS_ACTIVE_PRICING FROM PF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_EXPORT_PACKING_HISTORY PEPH ON PEPH.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = PEPH.PACKING_ID LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PEWC.PARTY_ID LEFT JOIN PF_SALESMAN PS ON PS.SALESMAN_ID = PEWC.SALESMAN_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWC.SUB_ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE PEWC.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or PEWC.CONTAINER_NO like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PEWC.SEAL_NO like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PEWC.REL_ORDER_NO like '" + txtSearchEmp.Text + "%' or to_char(PEWC.DISPATCH_DATE, 'dd/mm/yyyy')  like '" + txtSearchEmp.Text + "%' or PEWC.IS_ACTIVE like '" + txtSearchEmp.Text + "%' ORDER BY PEWC.WB_SLIP_NO desc, PEWC.ITEM_ID asc ";   
                    }
                    else
                    {
                        makeSQL = "  SELECT PEWC.WB_SLIP_NO, PEWC.DISPATCH_DATE, PEWC.CONTAINER_NO, NCS.CONTAINER_SIZE, PEWC.SEAL_NO, PEWC.REL_ORDER_NO, PP.PARTY_NAME,  PI.ITEM_NAME, PSI.SUB_ITEM_NAME, PEWC.ITEM_WEIGHT_WB,  PEWC.ITEM_WEIGHT,  NPL.PACKING_NAME, PEPH.NUMBER_OF_PACK, PEPH.PACK_PER_WEIGHT,  PEPH.TOTAL_WEIGHT, PS.SALESMAN_NAME, PEWC.IS_ACTIVE ,  PEWC.IS_INVENTORY_STATUS,  PEWC.CREATE_DATE, PEWC.UPDATE_DATE, PEWC.IS_PRINT,  PEWC.PRINT_DATE,  PEWC.IS_CONFIRM_CHECK, PEWC.EXPORT_INVOICE_NO, PEWC.IS_ACTIVE_PRICING FROM PF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_EXPORT_PACKING_HISTORY PEPH ON PEPH.WB_SLIP_NO = PEWC.WB_SLIP_NO LEFT JOIN NRC_PACKING_LIST NPL ON NPL.PACKING_ID = PEPH.PACKING_ID LEFT JOIN PF_PARTY PP ON PP.PARTY_ID = PEWC.PARTY_ID LEFT JOIN PF_SALESMAN PS ON PS.SALESMAN_ID = PEWC.SALESMAN_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWC.SUB_ITEM_ID LEFT JOIN NRC_CONTAINER_SIZE NCS ON NCS.CONTAINER_SIZE_ID = PEWC.CONTAINER_SIZE_ID WHERE IS_INVENTORY_STATUS = '" + DropDownIsInven.Text + "' AND (PEWC.WB_SLIP_NO like '" + txtSearchEmp.Text + "%' or PEWC.CONTAINER_NO like '" + txtSearchEmp.Text + "%' or PP.PARTY_NAME like '" + txtSearchEmp.Text + "%' or PEWC.SEAL_NO like '" + txtSearchEmp.Text + "%' or PI.ITEM_NAME like '" + txtSearchEmp.Text + "%' or PEWC.REL_ORDER_NO like '" + txtSearchEmp.Text + "%' or to_char(PEWC.DISPATCH_DATE, 'dd/mm/yyyy')  like '" + txtSearchEmp.Text + "%' or PEWC.IS_ACTIVE like '" + txtSearchEmp.Text + "%') ORDER BY PEWC.WB_SLIP_NO desc, PEWC.ITEM_ID asc ";
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
                    GroupGridView(GridView1.Rows, 0, 18);
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
                    makeSQL = " SELECT PI.ITEM_NAME, nvl(sum(PEWC.ITEM_WEIGHT), 0) AS ITEM_WEIGHT FROM PF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWC.SUB_ITEM_ID WHERE IS_INVENTORY_STATUS = 'Transit' AND PI.ITEM_NAME IS NOT NULL GROUP BY PI.ITEM_NAME";
                }
                else
                {
                    makeSQL = " SELECT PI.ITEM_NAME, nvl(sum(PEWC.ITEM_WEIGHT), 0) AS ITEM_WEIGHT FROM PF_EXPORT_WBSLIP_CON PEWC LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PEWC.ITEM_ID LEFT JOIN PF_SUB_ITEM PSI ON PSI.SUB_ITEM_ID = PEWC.SUB_ITEM_ID WHERE IS_INVENTORY_STATUS = '" + DropDownSearchSummary.Text + "' AND PI.ITEM_NAME IS NOT NULL GROUP BY PI.ITEM_NAME ";
                          
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
            TextSealNo.Text = "";
            DropDownSupplierID.Text = "0";
            TextRelOrderNo.Text = "";
            CheckSlipNo.Text = "";
            DropDownSupplierID.Text = "0";
            DropDownSalesmanID.Text = "0";
            DropDownItemID.Text = "0";
            DropDownSubItemID.Text = "0";
            DropDownPacking1.Text = "0";
            TextNoOfPacking1.Text = "0";
            TextNoPerWtPacking1.Text = "0";
            TextWtTotalPacking1.Text = "";
            DropDownPacking2.Text = "0";
            TextNoOfPacking2.Text = "0";
            TextNoPerWtPacking2.Text = "0";
            TextWtTotalPacking2.Text = "0";
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
            TextSealNo.Text = "";
            DropDownSupplierID.Text = "0";
            TextRelOrderNo.Text = ""; 
            CheckSlipNo.Text = ""; 
            DropDownSupplierID.Text = "0";
            DropDownSalesmanID.Text = "0";
            DropDownItemID.Text = "0";
            DropDownSubItemID.Text = "0";
            DropDownPacking1.Text = "0";
            TextNoOfPacking1.Text = "0";
            TextNoPerWtPacking1.Text = "0"; 
            TextWtTotalPacking1.Text = "";
            DropDownPacking2.Text = "0";
            TextNoOfPacking2.Text = "0";
            TextNoPerWtPacking2.Text = "0";
            TextWtTotalPacking2.Text = "0";
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