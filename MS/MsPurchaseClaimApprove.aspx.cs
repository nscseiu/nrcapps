using System;
using System.Collections;
using System.Configuration;
using System.Data; 
using System.Web.UI; 
using System.Web.UI.WebControls; 
using System.Data.OracleClient;
using System.IO;  
using System.Data.SqlClient; 


namespace NRCAPPS.MS
{
    public partial class MsPurchaseClaimApprove : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt, ds;
        int RowCount;
        string IS_PAGE_ACTIVE = "", IS_ADD_ACTIVE = "", IS_EDIT_ACTIVE = "", IS_DELETE_ACTIVE = "", IS_VIEW_ACTIVE = "", IS_REPORT_ACTIVE = "", IS_PRINT_ACTIVE = "";
        public bool IsLoad { get; set; }
        public bool IsLoad2 { get; set; } 
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
                        DataTable dtEmployeeID = new DataTable();
                        DataSet dse = new DataSet();
                        string makeEmployeeSQL = " SELECT EMP_ID, EMP_FNAME || ' ' ||EMP_LNAME AS EMP_NAME FROM HR_EMPLOYEES WHERE IS_ACTIVE = 'Enable' ORDER BY EMP_ID ASC";
                        dse = ExecuteBySqlString(makeEmployeeSQL);
                        dtEmployeeID = (DataTable)dse.Tables[0];
                        DropDownEmployeeID.DataSource = dtEmployeeID;
                        DropDownEmployeeID.DataValueField = "EMP_ID";
                        DropDownEmployeeID.DataTextField = "EMP_NAME";
                        DropDownEmployeeID.DataBind();
                        DropDownEmployeeID.Items.Insert(0, new ListItem("Select Petty Cash Holder", "0"));

                        DataTable dtClaimID = new DataTable();
                        DataSet dsc = new DataSet();
                        string makeClaimSQL = " SELECT * FROM MS_PURCHASE_CLAIM WHERE IS_ACTIVE = 'Enable' AND IS_CHECK = 'Incomplete' ORDER BY CLAIM_NO ASC";
                        dsc = ExecuteBySqlString(makeClaimSQL);
                        dtClaimID = (DataTable)dsc.Tables[0];
                        DropDownClaimID.DataSource = dtClaimID;
                        DropDownClaimID.DataValueField = "CLAIM_NO";
                        DropDownClaimID.DataTextField = "CLAIM_NO";
                        DropDownClaimID.DataBind();
                        DropDownClaimID.Items.Insert(0, new ListItem("Select Claim ID ", "0"));

                        DataTable dtPaymentTypeID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeRoleSQL = " SELECT PAYMENT_TYPE_ID, PAYMENT_TYPE_NAME FROM NRC_PAYMENT_TYPE WHERE IS_ACTIVE = 'Enable' ORDER BY PAYMENT_TYPE_ID ASC ";
                        ds = ExecuteBySqlString(makeRoleSQL);
                        dtPaymentTypeID = (DataTable)ds.Tables[0];
                        DropDownPaymentTypeID.DataSource = dtPaymentTypeID;
                        DropDownPaymentTypeID.DataValueField = "PAYMENT_TYPE_ID";
                        DropDownPaymentTypeID.DataTextField = "PAYMENT_TYPE_NAME";
                        DropDownPaymentTypeID.DataBind();
                        //     DropDownPaymentTypeID.Items.Insert(0, new ListItem("Select User Role", "0"));

                        TextClaimNo.Enabled = false;
                        DropDownEmployeeID.Enabled = false;
                        TextMonthYear4.Enabled = false;
                        EntryDate.Enabled = false;
                        DropDownPaymentTypeID.Enabled = false;
                        DropDownSlipNo.Enabled = false;
                        TextTotalAmount.Enabled = false;
                     //   CheckIsActive.Enabled = false;

                        QueryCmo.Visible = false;
                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");

                        Display();

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
         

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int PurchaseClaimID = Convert.ToInt32(TextPurchaseClaimID.Text);
             

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string IsQuery = CheckIsQuery.Checked ? "Yes" : "No";
                string IsCmoCheck = CheckIsCmo.Checked ? "Complete" : "Incomplete"; 
                 
                if (CheckIsCmo.Checked == false)
                {
                    string update_purchase_claim = " update  MS_PURCHASE_CLAIM set IS_OBJ_QUERY =:TextIsQuery, OBJ_QUERY_DES =:TextQueryDescription,  OBJ_QUERY_C_DATE = TO_DATE(:DateObjQuery, 'DD-MM-YYYY HH:MI:SS AM'), IS_CHECK =:TextIsCmoCheck , CHECK_USER_ID = :NoC_USER_ID where PURCHASE_CLAIM_ID =: NoPurClaimID ";
                    cmdu = new OracleCommand(update_purchase_claim, conn);

                    OracleParameter[] objPr = new OracleParameter[6];
                    objPr[0] = cmdu.Parameters.Add("TextIsQuery", IsQuery);
                    objPr[1] = cmdu.Parameters.Add("TextQueryDescription", TextQueryDescription.Text);
                    objPr[2] = cmdu.Parameters.Add("DateObjQuery", u_date);
                    objPr[3] = cmdu.Parameters.Add("TextIsCmoCheck", IsCmoCheck); 
                    objPr[4] = cmdu.Parameters.Add("NoC_USER_ID", userID);
                    objPr[5] = cmdu.Parameters.Add("NoPurClaimID", PurchaseClaimID);
                     
                }
                else if (CheckIsCmo.Checked == true)
                {

                    string update_purchase_claim = " update  MS_PURCHASE_CLAIM set IS_CHECK =:TextIsCmoCheck,   CHECK_DATE = TO_DATE(:DateCmoCheck, 'DD-MM-YYYY HH:MI:SS AM') , CHECK_USER_ID = :NoC_USER_ID where PURCHASE_CLAIM_ID =: NoPurClaimID ";
                    cmdu = new OracleCommand(update_purchase_claim, conn);

                    OracleParameter[] objPr = new OracleParameter[4]; 
                    objPr[0] = cmdu.Parameters.Add("TextIsCmoCheck", IsCmoCheck);
                    objPr[1] = cmdu.Parameters.Add("DateCmoCheck", u_date);
                    objPr[2] = cmdu.Parameters.Add("NoC_USER_ID", userID);
                    objPr[3] = cmdu.Parameters.Add("NoPurClaimID", PurchaseClaimID);
                   

                }

                 
                cmdu.ExecuteNonQuery(); 
                cmdu.Parameters.Clear();
                cmdu.Dispose();

                 
                conn.Close(); 

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Purchase Claim Check Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                
                Display(); 

            }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
        }

        protected void BtnUpdateQuery_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int PurchaseClaimID = Convert.ToInt32(TextPurchaseClaimID.Text);


                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string IsQuery = CheckIsQuery.Checked ? "Yes" : "No"; 
                 
                    string update_purchase_claim = " update  MS_PURCHASE_CLAIM set IS_OBJ_QUERY =:TextIsQuery, OBJ_QUERY_DES =:TextQueryDescription,  OBJ_QUERY_C_DATE = TO_DATE(:DateObjQuery, 'DD-MM-YYYY HH:MI:SS AM'),  CHECK_USER_ID = :NoC_USER_ID where PURCHASE_CLAIM_ID =: NoPurClaimID ";
                    cmdu = new OracleCommand(update_purchase_claim, conn);

                    OracleParameter[] objPr = new OracleParameter[5];
                    objPr[0] = cmdu.Parameters.Add("TextIsQuery", IsQuery);
                    objPr[1] = cmdu.Parameters.Add("TextQueryDescription", TextQueryDescription.Text);
                    objPr[2] = cmdu.Parameters.Add("DateObjQuery", u_date);
                    objPr[3] = cmdu.Parameters.Add("NoC_USER_ID", userID);
                    objPr[4] = cmdu.Parameters.Add("NoPurClaimID", PurchaseClaimID);
                  
                cmdu.ExecuteNonQuery();
                cmdu.Parameters.Clear();
                cmdu.Dispose();
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Query send successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                Display(); 
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        } 
     
        protected void linkSelectClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             
             LinkButton btn = (LinkButton)sender;
             Session["user_data_id"] = btn.CommandArgument;
             int USER_DATA_ID = Convert.ToInt32(Session["user_data_id"]);

             DataTable dtSlipNo = new DataTable();
             DataSet dsp = new DataSet();
             string makePageSQL = " SELECT PPM.SLIP_NO, PPM.SLIP_NO || ' - ' || PC.PARTY_ID || ' - ' || PC.PARTY_NAME || ', Item -' || PI.ITEM_NAME || ', Weight- ' || PPM.ITEM_WEIGHT || ', Amount- ' ||  TO_CHAR(PPM.TOTAL_AMOUNT, '999,999,999.99')  AS PARTY_NAME, nvl(PPM.TOTAL_AMOUNT,0) AS ITEM_AMOUNT  FROM MS_PURCHASE_MASTER PPM  LEFT JOIN MS_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID  WHERE  PPM.CLAIM_NO = '" + USER_DATA_ID + "' ORDER BY  PPM.SLIP_NO ASC"; //  
             dsp = ExecuteBySqlString(makePageSQL);
             dtSlipNo = (DataTable)dsp.Tables[0];
             DropDownSlipNo.DataSource = dtSlipNo;
             DropDownSlipNo.DataValueField = "SLIP_NO";
             DropDownSlipNo.DataTextField = "PARTY_NAME";
             DropDownSlipNo.DataBind(); 
             RowCount = dtSlipNo.Rows.Count;
             decimal ItemAmount = 0, TotalAmount = 0;
             for (int i = 0; i < RowCount; i++)
             {
                 ItemAmount +=  Math.Round(Convert.ToDecimal(dtSlipNo.Rows[i]["ITEM_AMOUNT"]), 0, MidpointRounding.AwayFromZero);
             }

             string makeSQL = " SELECT PURCHASE_CLAIM_ID, CLAIM_NO, EMP_ID, TO_CHAR(TO_DATE(CLAIM_DATE),'dd/mm/yyyy') AS CLAIM_DATE, TO_CHAR(TO_DATE(CLAIM_FOR_MONTH),'mm/yyyy') AS CLAIM_FOR_MONTH, PAYMENT_TYPE_ID, TOTAL_AMOUNT, IS_ACTIVE, OBJ_QUERY_DES, IS_CHECK, IS_OBJ_QUERY FROM MS_PURCHASE_CLAIM where CLAIM_NO = '" + USER_DATA_ID + "' ORDER BY CLAIM_NO DESC";

             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn);
             ds = new DataTable();
             oradata.Fill(ds);
             RowCount = ds.Rows.Count;

             for (int i = 0; i < RowCount; i++)
             { 
                 TextPurchaseClaimID.Text     = ds.Rows[i]["PURCHASE_CLAIM_ID"].ToString();
                 TextClaimNo.Text             = ds.Rows[i]["CLAIM_NO"].ToString();                 
                 DropDownEmployeeID.Text      = ds.Rows[i]["EMP_ID"].ToString();
                 TextMonthYear4.Text          = ds.Rows[i]["CLAIM_FOR_MONTH"].ToString();
                 EntryDate.Text               = ds.Rows[i]["CLAIM_DATE"].ToString(); 
                 DropDownPaymentTypeID.Text   = ds.Rows[i]["PAYMENT_TYPE_ID"].ToString();
                 TextTotalAmount.Text         = ds.Rows[i]["TOTAL_AMOUNT"].ToString();
                 TextQueryDescription.Text    = ds.Rows[i]["OBJ_QUERY_DES"].ToString(); 
                 CheckIsActive.Checked        = Convert.ToBoolean(ds.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                 CheckIsCmo.Checked           = Convert.ToBoolean(ds.Rows[i]["IS_CHECK"].ToString() == "Complete" ? true : false);
                 CheckIsQuery.Checked         = Convert.ToBoolean(ds.Rows[i]["IS_OBJ_QUERY"].ToString() == "Yes" ? true : false);
             }
             TotalAmount = Convert.ToDecimal(TextTotalAmount.Text);
           //  decimal ItemAmountRound = Math.Round(Convert.ToDecimal(ItemAmount), 0, MidpointRounding.AwayFromZero);
             if (ItemAmount == TotalAmount) 
             {
                 CheckTotalAmount.Text = "";
                 CheckIsCmo.Enabled = true;
            }else{
                CheckTotalAmount.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Claim Amount Mismatch with Invoice Amount is: " + ItemAmount.ToString("N2") + "</label>";
                CheckTotalAmount.ForeColor = System.Drawing.Color.Red;
                TextTotalAmount.Focus();
                BtnUpdate.Attributes.Add("aria-disabled", "false");
                BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                CheckIsCmo.Attributes.Add("disabled", "disabled");
                CheckIsCmo.Enabled = false;
             }

             if (CheckIsQuery.Checked)
             {
                 QueryCmo.Visible = true;
             }
             else { QueryCmo.Visible = false; 
             }

             string makeSlipSQL = " SELECT PPC.CLAIM_NO, PPM.SLIP_NO FROM MS_PURCHASE_CLAIM PPC LEFT JOIN MS_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO WHERE PPC.CLAIM_NO = '" + USER_DATA_ID + "' ORDER BY PPM.SLIP_NO ASC"; // PPC.IS_CHECK = 'Incomplete' AND  

             cmdl = new OracleCommand(makeSlipSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count;  
              foreach (ListItem li in DropDownSlipNo.Items)
              { 
                  li.Selected = false;
                  for (int i = 0; i < RowCount; i++)
                  {
                    //  DropDownUserRoleID.Text = dt.Rows[i]["USER_ROLE_ID"].ToString();
                      if (li.Value == dt.Rows[i]["SLIP_NO"].ToString())
                      {
                          li.Selected = true; 
                      }
                  }
              }
             

             Display();
             conn.Close();
             CheckClaimNo.Text = "";
             alert_box.Visible = false; 
        }

        public void Cmo_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckIsCmo.Checked == false)
            {
                BtnUpdate.Attributes.Add("aria-disabled", "false");
                BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
            }
            else if (CheckIsCmo.Checked == true)
            { 
                BtnUpdate.Attributes.Add("aria-disabled", "true");
                BtnUpdate.Attributes.Add("class", "btn btn-success active");
            }
            
                     }

        public void Query_CheckedChanged(object sender, EventArgs e)
        { 
            if (CheckIsQuery.Checked == false)
            {
                QueryCmo.Visible = false;

                BtnUpdate.Attributes.Add("aria-disabled", "false");
                BtnUpdate.Attributes.Add("class", "btn btn-success disabled");

                if (CheckIsCmo.Checked)
                {
                    BtnUpdate.Attributes.Add("aria-disabled", "true");
                    BtnUpdate.Attributes.Add("class", "btn btn-success active");
                }
            }
            else if (CheckIsQuery.Checked == true)
            { 
                QueryCmo.Visible = true;
                 
            }
             
             
        }

        public void Display()
        {
            
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();


            string makeSQL = "";
            if (txtSearchUser.Text == "")
            {
                makeSQL = " SELECT PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO, HE.EMP_FNAME || ' ' || HE.EMP_LNAME AS EMP_NAME, PPC.CLAIM_FOR_MONTH, PPC.CLAIM_DATE, NPT.PAYMENT_TYPE_NAME, PPC.TOTAL_AMOUNT, PPC.CREATE_DATE, PPC.UPDATE_DATE, PPC.IS_ACTIVE, IS_OBJ_QUERY, PPC.OBJ_QUERY_DES, PPC.OBJ_QUERY_C_DATE, count(PPM.SLIP_NO) as SLIP_NO, PPC.IS_CHECK FROM MS_PURCHASE_CLAIM PPC LEFT JOIN MS_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PPC.EMP_ID LEFT JOIN NRC_PAYMENT_TYPE NPT ON NPT.PAYMENT_TYPE_ID = PPC.PAYMENT_TYPE_ID LEFT JOIN MS_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID WHERE PPC.IS_CHECK = 'Incomplete' AND PPC.IS_ACTIVE = 'Enable' GROUP BY PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO, HE.EMP_FNAME || ' ' || HE.EMP_LNAME, PPC.CLAIM_FOR_MONTH, PPC.CLAIM_DATE, NPT.PAYMENT_TYPE_NAME, PPC.TOTAL_AMOUNT, PPC.CREATE_DATE, PPC.UPDATE_DATE, PPC.IS_ACTIVE, IS_OBJ_QUERY, PPC.OBJ_QUERY_DES, PPC.OBJ_QUERY_C_DATE, PPC.IS_CHECK ORDER BY PPC.CLAIM_NO DESC "; //WHERE PPC.IS_CHECK = 'Incomplete'

            }
            else
            {
                makeSQL = " SELECT PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO, HE.EMP_FNAME || ' ' || HE.EMP_LNAME AS EMP_NAME, PPC.CLAIM_FOR_MONTH, PPC.CLAIM_DATE, NPT.PAYMENT_TYPE_NAME, PPC.TOTAL_AMOUNT, PPC.CREATE_DATE, PPC.UPDATE_DATE, PPC.IS_ACTIVE, IS_OBJ_QUERY, PPC.OBJ_QUERY_DES, PPC.OBJ_QUERY_C_DATE, count(PPM.SLIP_NO) as SLIP_NO, PPC.IS_CHECK, PPC.IS_PRINT, TO_CHAR(PPC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE FROM MS_PURCHASE_CLAIM PPC LEFT JOIN MS_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PPC.EMP_ID LEFT JOIN NRC_PAYMENT_TYPE NPT ON NPT.PAYMENT_TYPE_ID = PPC.PAYMENT_TYPE_ID LEFT JOIN MS_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID WHERE PPC.CLAIM_NO like '" + txtSearchUser.Text + "%' GROUP BY PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO, HE.EMP_FNAME || ' ' || HE.EMP_LNAME, PPC.CLAIM_FOR_MONTH, PPC.CLAIM_DATE, NPT.PAYMENT_TYPE_NAME, PPC.TOTAL_AMOUNT, PPC.CREATE_DATE, PPC.UPDATE_DATE, PPC.IS_ACTIVE, IS_OBJ_QUERY, PPC.OBJ_QUERY_DES, PPC.OBJ_QUERY_C_DATE, PPC.IS_CHECK, PPC.IS_PRINT, TO_CHAR(PPC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') ORDER BY PPC.CLAIM_NO DESC ";
                alert_box.Visible = false;
            }

              
            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            GridView4D.DataSource = dt;
            GridView4D.DataKeyNames = new string[] { "CLAIM_NO" };
            GridView4D.DataBind();
             
            conn.Close(); 
        }

        protected void GridViewSearchUser(object sender, EventArgs e)
        {
            this.Display();
        }
        /*
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
               */




        public void clearTextField(object sender, EventArgs e)
        {
            
         //   DropDownPaymentTypeID.Text = "0";
            DropDownEmployeeID.Text = "0";
            TextQueryDescription.Text = "";
            TextTotalAmount.Text = "";
            DropDownSlipNo.SelectedIndex = -1;
            CheckIsQuery.Checked = false;
            CheckIsCmo.Checked = false;
        }

        public void clearText()
        {
              
          //  DropDownPaymentTypeID.Text = "0";
            TextClaimNo.Text = "";
            EntryDate.Text = "";
            DropDownEmployeeID.Text = "0";
            TextQueryDescription.Text = "";
            TextTotalAmount.Text = "";
            DropDownSlipNo.SelectedIndex = -1;
            CheckIsQuery.Checked = false;
            CheckIsCmo.Checked = false;

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