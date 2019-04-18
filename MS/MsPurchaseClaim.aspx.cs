using System;
using System.Collections;
using System.Configuration;
using System.Data; 
using System.Web.UI; 
using System.Web.UI.WebControls; 
using System.Data.OracleClient;
using System.IO;  
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization;


namespace NRCAPPS.MS
{
    public partial class MsPurchaseClaim : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt, ds;
        int RowCount;
        string IS_PAGE_ACTIVE = "";
        string IS_ADD_ACTIVE = "";
        string IS_EDIT_ACTIVE = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE = "";
        double ItemVatAmt = 0.0, ItemAmtTotal = 0.0, ItemWtWbTotal = 0.0; string EntryDateSlip = "", PartyName = "", FullName ="";
        public bool IsLoad { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_NAME"] != null)
            {
                string requestedFile = Path.GetFileName(Request.Path);
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = " SELECT  NUPP.IS_PAGE_ACTIVE, NUPP.IS_ADD_ACTIVE, NUPP.IS_EDIT_ACTIVE, NUPP.IS_DELETE_ACTIVE, NUPP.IS_VIEW_ACTIVE FROM NRC_USER_PAGE_PERMISSION NUPP LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NUPP.USER_PAGE_ID  WHERE NUPP.USER_ID = '" + Session["USER_ID"] + "' AND NUP.IS_ACTIVE = 'Enable' AND NUP.PAGE_URL = '" + requestedFile + "' ";
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
                }

                if (IS_PAGE_ACTIVE == "Enable")
                {
                      if (!IsPostBack)
                    {
                        DataTable dtEmployeeID = new DataTable();
                        DataSet dse = new DataSet();
                        string makeEmployeeSQL = " SELECT EMP_ID, EMP_ID || ' - ' || EMP_FNAME || ' ' ||EMP_LNAME AS EMP_NAME FROM HR_EMPLOYEES WHERE IS_ACTIVE = 'Enable' ORDER BY EMP_ID ASC";
                        dse = ExecuteBySqlString(makeEmployeeSQL);
                        dtEmployeeID = (DataTable)dse.Tables[0];
                        DropDownEmployeeID.DataSource = dtEmployeeID;
                        DropDownEmployeeID.DataValueField = "EMP_ID";
                        DropDownEmployeeID.DataTextField = "EMP_NAME";
                        DropDownEmployeeID.DataBind();
                        DropDownEmployeeID.Items.Insert(0, new ListItem("Select Petty Cash Holder", "0"));

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


                        DataTable dtSlipNo = new DataTable();
                        DataSet dsp = new DataSet();
                        string makePageSQL = " SELECT PPM.PURCHASE_ID || '-' || PPM.TOTAL_AMOUNT AS PURCHASE_ID, PPM.SLIP_NO || ' - ' || PC.PARTY_ID || ' - ' || PC.PARTY_NAME || ', Item -' || PI.ITEM_NAME || ', Weight- ' || PPM.ITEM_WEIGHT || ', Amount- ' || TO_CHAR(PPM.TOTAL_AMOUNT, '999,999,999.99') AS PARTY_NAME FROM MS_PURCHASE_MASTER PPM  LEFT JOIN MS_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID  WHERE PPM.CLAIM_NO IS NULL  ORDER BY  PPM.SLIP_NO ASC";
                        dsp = ExecuteBySqlString(makePageSQL);
                        dtSlipNo = (DataTable)dsp.Tables[0];
                        DropDownSlipNoWp.DataSource = dtSlipNo;
                        DropDownSlipNoWp.DataValueField = "PURCHASE_ID";
                        DropDownSlipNoWp.DataTextField = "PARTY_NAME";
                        DropDownSlipNoWp.DataBind();

                        TextClaimNo.Enabled = false;
                        TextTotalAmount.Attributes.Add("readonly", "readonly");
                        Display();

                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled");

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
                    string EmpID = DropDownEmployeeID.Text;
                  //  int ClaimNoFrom = Convert.ToInt32(TextClaimNo.Text);
                    int PaymentTypeID = Convert.ToInt32(DropDownPaymentTypeID.Text);
                    double TotalAmount = Convert.ToDouble(TextTotalAmount.Text);

                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-"); 
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                    string ClaimForMonthsGet = TextMonthYear4.Text;
                    string[] MakeClaimForMonthsSplit = ClaimForMonthsGet.Split('/');
                    String ClaimMonths = MakeClaimForMonthsSplit[0];
                    String ClaimYear = MakeClaimForMonthsSplit[1];
                    string ClaimDay = "01";
                    string ClaimForMonthsTemp = ClaimDay +"-" +ClaimMonths+ "-"+ClaimYear;
                    DateTime ClaimForMonthsNew = DateTime.ParseExact(ClaimForMonthsTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string ClaimForMonths = ClaimForMonthsNew.ToString("dd-MM-yyyy");

                    string get_pur_claim_id = "select MS_PURCHASE_CLAIM_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_pur_claim_id, conn);
                    int newPurClaimID = Int32.Parse(cmdu.ExecuteScalar().ToString());
                     
                    string get_pur_claim_no = "select MS_PURCHASE_CLAIMNO_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_pur_claim_no, conn);
                    int newPurClaimNo = Int32.Parse(cmdu.ExecuteScalar().ToString());


                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");


                    foreach (ListItem li in DropDownSlipNoWp.Items)
                    {
                       
                        if (li.Selected == true)
                        {
                            string[] WbSlipNo = li.Value.Split('-');
                            string update_purchase = " update  MS_PURCHASE_MASTER set CLAIM_NO =:NoClaimNo  where PURCHASE_ID =: NoSlipNo ";
                            cmdi = new OracleCommand(update_purchase, conn);

                            OracleParameter[] objPrm = new OracleParameter[3];
                            objPrm[0] = cmdi.Parameters.Add("NoSlipNo", WbSlipNo[0]);
                            objPrm[1] = cmdi.Parameters.Add("NoClaimNo", newPurClaimNo);

                            cmdi.ExecuteNonQuery();
                        }
                    }

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    string insert_pur_claim = "insert into MS_PURCHASE_CLAIM (PURCHASE_CLAIM_ID, CLAIM_NO, EMP_ID, CLAIM_DATE, CLAIM_FOR_MONTH, PAYMENT_TYPE_ID, TOTAL_AMOUNT, CREATE_DATE, C_USER_ID, IS_ACTIVE, IS_CHECK, IS_OBJ_QUERY) VALUES ( :NoPurClaimID, :NoClaimNo, :NoEmpID, TO_DATE(:ClaimDate, 'DD/MM/YYYY'), TO_DATE(:ClaimForMonth, 'DD/MM/YYYY'), :NoPaymentTypeID, :NoTotalAmount, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'),  :NoCuserID, :TextIsActive, :TextIsCheck, :TextIsObjQuery)";
                    cmdi = new OracleCommand(insert_pur_claim, conn);

                    OracleParameter[] objPr = new OracleParameter[12];
                    objPr[0] = cmdi.Parameters.Add("NoPurClaimID", newPurClaimID);
                    objPr[1] = cmdi.Parameters.Add("NoClaimNo", newPurClaimNo);
                    objPr[2] = cmdi.Parameters.Add("NoEmpID", EmpID); 
                    objPr[3] = cmdi.Parameters.Add("ClaimForMonth", ClaimForMonths);
                    objPr[4] = cmdi.Parameters.Add("ClaimDate", EntryDateNew);
                    objPr[5] = cmdi.Parameters.Add("NoPaymentTypeID", PaymentTypeID);
                    objPr[6] = cmdi.Parameters.Add("NoTotalAmount", TotalAmount); 
                    objPr[7] = cmdi.Parameters.Add("c_date", c_date);
                    objPr[8] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPr[9] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPr[10] = cmdi.Parameters.Add("TextIsCheck", "Incomplete");
                    objPr[11] = cmdi.Parameters.Add("TextIsObjQuery", "No");

                    cmdi.ExecuteNonQuery();

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert Claim Data successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                    clearText();
                    Display();
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
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            string HtmlString = "";

            string ClaimNo = "";
            if (TextClaimNo.Text != "")
            {
                ClaimNo = TextClaimNo.Text;
            }
            else
            {
                LinkButton btn = (LinkButton)sender;
                Session["user_data_id"] = btn.CommandArgument;
                ClaimNo = Session["user_data_id"].ToString();
            }

            string makeSQL = " SELECT PPC.CLAIM_NO, TO_CHAR(TO_DATE(CLAIM_DATE),'dd/MON/YYYY') AS CLAIM_DATE, TO_CHAR(TO_DATE(CLAIM_FOR_MONTH),'MON/YYYY') AS CLAIM_MONTH, HE.EMP_FNAME || ' '|| HE.EMP_LNAME AS FULL_NAME,  PP.PARTY_NAME, PPM.SLIP_NO, PPM.ITEM_AMOUNT+nvl(PPM.VAT_AMOUNT,0) AS ITEM_AMOUNT FROM MS_PURCHASE_CLAIM PPC LEFT JOIN MS_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO LEFT JOIN MS_PARTY PP ON PP.PARTY_ID = PPM.PARTY_ID  LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PPC.EMP_ID WHERE PPC.CLAIM_NO = '" + ClaimNo + "' ORDER BY PPM.SLIP_NO ASC";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;


            HtmlString += "<div style='float:left;width:772px;height:auto;margin:20px 0 0 60px;'> ";

            for (int i = 0; i < 1; i++)
            { 
                int ClaimsNo = Convert.ToInt32(dt.Rows[i]["CLAIM_NO"].ToString());
                FullName = dt.Rows[i]["FULL_NAME"].ToString();
                EntryDateSlip = dt.Rows[i]["CLAIM_DATE"].ToString();
                       
                HtmlString += "<div style='float:left;width:770px;height:auto;margin-top:10px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight: 400; line-height: 16px;'> ";
                HtmlString += "<div style='float:left;width:770px;height:115px;text-align:center;' ><img src='../../image/logo_from.png'/></div> ";
                HtmlString += "<div style='float:left;width:770px;height:45px;text-align:center;text-decoration: underline;' ><span style='font-family:Times New Roman,Times, serif;font-size:17px;font-weight:700;'>RAW MATERIAL REIMBURSEMENT REQUEST</span></div> ";
                  
                HtmlString += "<div style='float:left;width:190px;height:40px; margin:0 0 0 40px;'><span style='font-family:Times New Roman,Times, serif;font-size:15px;font-weight:700;'>Claim No.</span><span style='color:red;font-weight:700;font-size:17px;'> " + ClaimsNo + "</span></div> ";
                HtmlString += "<div style='float:left;width:325px;height:40px;text-align:center;font-weight:700;text-decoration:underline;font-size:17px;'>  " + dt.Rows[i]["CLAIM_MONTH"].ToString() + "</div> ";
                HtmlString += "<div style='float:left;width:210px;height:40px;font-weight:700;size:13px;'><span style='font-family:Times New Roman,Times, serif;'>Division:</span><span style='color:#7d4444;'> Metal Factory</span></div>  ";

                HtmlString += "<div style='float:left;width:485px;height:45px;margin:0 0 0 40px;'><div style='float:left;width:45px;font-family:Times New Roman,Times, serif;size:13px;font-weight:700;'>Name:</div><div style='float:left;width:435px;height:15px;padding-left:5px;font-weight:700;border-bottom:black dotted 1px;'>" + FullName + "</div></div> ";
                HtmlString += "<div style='float:left;width:206px;height:45px; margin:0 0 0 30px;'><span style='font-family:Times New Roman,Times, serif;size:13px;font-weight:700;'>Date:</span>  " + EntryDateSlip + "</div> ";


            }
            
             
            HtmlString += "</div>";

            HtmlString += "<div style='border:black solid 1px;float:left;width:740px;height:auto;margin:0 0 0 40px;-webkit-border-top-left-radius:10px;-webkit-border-top-right-radius:10px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight:400; line-height: 16px;'> ";
            HtmlString += "<div style='float:left;width:35px;text-align:center;border-bottom:black solid 1px;padding:5px;'><span style='font-family:Times New Roman,Times, serif;size:13px'>S.No.</span></div> ";
            HtmlString += "<div style='float:left;width:445px;border-bottom:black solid 1px;border-left:black solid 1px;padding:5px;text-align:center;' ><span style='font-family:Times New Roman,Times, serif;size:13px'>Description</span></div> ";
            HtmlString += "<div style='float:left;width:96px;text-align:right;border-bottom:black solid 1px;border-left:black solid 1px;text-align:center;padding:5px;'><span style='font-family:Times New Roman,Times, serif;size:13px'>Ref. No.</span></div> ";
            HtmlString += "<div style='float:left;width:121px;text-align:right;border-bottom:black solid 1px;border-left:black solid 1px;padding:5px;'><span style='font-family:Times New Roman,Times, serif;size:13px'>Amount</span></div> ";
            int l=1;
            for (int i = 0; i < RowCount; i++)
            {
                
                double ItemAmt = Convert.ToDouble(Math.Round(Convert.ToDecimal(dt.Rows[i]["ITEM_AMOUNT"]), 0, MidpointRounding.AwayFromZero));
                ItemAmtTotal += Convert.ToDouble(Math.Round(Convert.ToDecimal(dt.Rows[i]["ITEM_AMOUNT"]), 0, MidpointRounding.AwayFromZero)); 
                double TotalInvoiceAmt = +Convert.ToDouble(decimal.Parse(dt.Rows[i]["ITEM_AMOUNT"].ToString()).ToString(".00")); 
                HtmlString += "<div style='float:left;width:39px;text-align:center;border-bottom:black dotted 1px;padding:3px;'> " + l+"</div> ";
                HtmlString += "<div style='float:left;width:449px;border-bottom:black dotted 1px;border-left:black dotted 1px;padding:3px;' >" + dt.Rows[i]["PARTY_NAME"].ToString() + "</div> ";  
                HtmlString += "<div style='float:left;width:100px;text-align:right;border-bottom:black dotted 1px;border-left:black dotted 1px;text-align:center;padding:3px;'>" + dt.Rows[i]["SLIP_NO"].ToString() + " </div> ";
                HtmlString += "<div style='float:left;width:125px;text-align:right;border-bottom:black dotted 1px;border-left:black dotted 1px;padding:3px;'>" + string.Format("{0:n0}", ItemAmt) + "/-</div> ";
              l++;  
            }
            int CountRow = 25;
            int BlankRow = CountRow - RowCount;
            for (int i = 1; i <= BlankRow; i++) { 
                if (i == BlankRow) {
                    HtmlString += "<div style='float:left;width:39px;height:15px;text-align:center;padding:3px;'></div> ";
                    HtmlString += "<div style='float:left;width:449px;height:15px;border-left:black dotted 1px;padding:3px;' ></div> ";
                    HtmlString += "<div style='float:left;width:100px;height:15px;text-align:right;border-left:black dotted 1px;text-align:center;padding:3px;'></div> ";
                    HtmlString += "<div style='float:left;width:125px;height:15px;text-align:right;border-left:black dotted 1px;padding:3px;'></div> "; }
                else
                {
                    HtmlString += "<div style='float:left;width:39px;height:15px;text-align:center;border-bottom:black dotted 1px;padding:3px;'></div> ";
                    HtmlString += "<div style='float:left;width:449px;height:15px;border-bottom:black dotted 1px;border-left:black dotted 1px;padding:3px;' ></div> ";
                    HtmlString += "<div style='float:left;width:100px;height:15px;text-align:right;border-bottom:black dotted 1px;border-left:black dotted 1px;text-align:center;padding:3px;'></div> ";
                    HtmlString += "<div style='float:left;width:125px;height:15px;text-align:right;border-bottom:black dotted 1px;border-left:black dotted 1px;padding:3px;'></div> ";
                }
                

            }

                HtmlString += "</div>";
            string NumberToInWord = NumberToWords(ItemAmtTotal).Trim().ToUpper();
            HtmlString += "<div style='float:left;width:785px;height:265px;margin:0 0 0 40px;font-family: Courier New, Courier, Lucida Sans Typewriter, Lucida Typewriter, monospace; font-size: 14px; font-style: normal; font-variant: normal; font-weight:400; line-height: 16px;'>";

            HtmlString += "<div style='float:left;width:741px;height:80;'>";
            HtmlString += "<div style='float:left;width:740px;height:auto;border-left:black solid 1px;border-bottom:black solid 1px;-webkit-border-bottom-left-radius:10px;border-right:black solid 1px;border-right:black solid 1px;-webkit-border-bottom-right-radius:10px;'>"; 
            HtmlString += "<div style='float:left;width:104px;padding:5px;font-family:Times New Roman,Times, serif;size:13px'><div style='display:flex;justify-content:center;align-items: center; '>Total Amount SR.</div></div> ";
            HtmlString += "<div style='float:left;width:484px;padding:5px;border-right:black solid 1px;'><span style='font-weight:700;'> " + NumberToInWord + "</span></div> ";
            HtmlString += "<div style='float:left;width:121px;padding:5px;text-align:right;font-weight:700;'>" + string.Format("{0:n0}", ItemAmtTotal) + "/- </div> ";
            HtmlString += "</div>";
            HtmlString += "</div>";

            HtmlString += "<div style='float:left;width:77px;height:15px;'><span style='font-family:Times New Roman,Times, serif;size:13px'>Prepared by:</span></div> ";
            HtmlString += "<div style='float:left;width:230px;height:15px; margin:0 0 0 5px;'><div style='float:left;width:230px;height:12px;border-bottom:black dotted 1px;'></div></div> ";
            HtmlString += "<div style='float:left;width:140px;height:15px; margin:0 0 0 55px;'><span style='font-family:Times New Roman,Times, serif;size:13px'>Checked & Verified by:</span></div> ";
            HtmlString += "<div style='float:left;width:220px;height:15px;'><div style='float:left;width:220px;height:12px;border-bottom:black dotted 1px;'></div></div> ";

            HtmlString += "<div style='float:left;width:152px;height:15px;margin:5px 0 0 0;'><span style='font-family:Times New Roman,Times, serif;size:13px'>Site Accountant / User</span></div> ";
            HtmlString += "<div style='float:left;width:155px;height:15px; margin:5px 0 0 5px;'></div> ";
            HtmlString += "<div style='float:left;width:140px;height:15px; margin:5px 0 0 55px;'><span style='font-family:Times New Roman,Times, serif;size:13px'>Site Manager</span></div> ";
            HtmlString += "<div style='float:left;width:220px;height:15px; margin:5px 0 0 0;'></div> ";

            HtmlString += "<div style='float:left;width:77px;height:15px;margin:25px 0 0 0;'><span style='font-family:Times New Roman,Times, serif;size:13px'>Checked by:</span></div> ";
            HtmlString += "<div style='float:left;width:230px;height:15px; margin:25px 0 0 5px;'><div style='float:left;width:230px;height:12px;border-bottom:black dotted 1px;'></div></div> ";
            HtmlString += "<div style='float:left;width:140px;height:15px; margin:25px 0 0 55px;'><span style='font-family:Times New Roman,Times, serif;size:13px'></span></div> ";
            HtmlString += "<div style='float:left;width:220px;height:15px;margin:25px 0 0 0;'><div style='float:left;width:220px;height:12px;'></div></div> ";

            HtmlString += "<div style='float:left;width:152px;height:15px;margin:5px 0 0 0;'><span style='font-family:Times New Roman,Times, serif;size:13px'>Office Accountant</span></div> ";
            HtmlString += "<div style='float:left;width:155px;height:15px; margin:5px 0 0 5px;'></div> ";
            HtmlString += "<div style='float:left;width:140px;height:15px; margin:5px 0 0 55px;'><span style='font-family:Times New Roman,Times, serif;size:13px'></span></div> ";
            HtmlString += "<div style='float:left;width:220px;height:15px; margin:5px 0 0 0;'></div> ";

            HtmlString += "<div style='float:left;width:155px;height:15px;margin:20px 0 0 0;'><span style='font-family:Times New Roman,Times, serif;size:13px;font-weight:700;'>Approved for Payment</span></div> ";
            HtmlString += "<div style='float:left;width:230px;height:15px; margin:20px 0 0 5px;'><div style='float:left;width:230px;height:12px;'></div></div> ";
            HtmlString += "<div style='float:left;width:140px;height:15px; margin:20px 0 0 55px;'><span style='font-family:Times New Roman,Times, serif;size:13px'></span></div> ";
            HtmlString += "<div style='float:left;width:120px;height:15px;margin:20px 0 0 0;'><div style='float:left;width:220px;height:12px;'></div></div> ";

            HtmlString += "<div style='float:left;width:115px;height:15px;margin:25px 0 0 0;'><span style='font-family:Times New Roman,Times, serif;size:13px'>Accounts Manager</span></div> ";
            HtmlString += "<div style='float:left;width:192px;height:15px; margin:25px 0 0 5px;'><div style='float:left;width:192px;height:12px;border-bottom:black dotted 1px;'></div></div> ";
            HtmlString += "<div style='float:left;width:140px;height:15px; margin:25px 0 0 55px;'><span style='font-family:Times New Roman,Times, serif;size:13px'>Internal Audit Manager</span></div> ";
            HtmlString += "<div style='float:left;width:220px;height:15px; margin:25px 0 0 0;'><div style='float:left;width:220px;height:12px;border-bottom:black dotted 1px;'></div></div> ";

            HtmlString += "</div>";
            HtmlString += "</div>";
            HtmlString += "</div>";

            HtmlString += "</div>";
            // purchase master update for print
            int userID = Convert.ToInt32(Session["USER_ID"]);
            
            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
            string update_user = "update  MS_PURCHASE_CLAIM  set IS_PRINT = :TextIsPrint, PRINT_DATE  = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), P_USER_ID = :NoCuserID where CLAIM_NO = :NoClaimNo ";
            cmdi = new OracleCommand(update_user, conn);

            OracleParameter[] objPrm = new OracleParameter[4];
            objPrm[0] = cmdi.Parameters.Add("TextIsPrint", "Printed");
            objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
            objPrm[2] = cmdi.Parameters.Add("NoCuserID", userID);
            objPrm[3] = cmdi.Parameters.Add("NoClaimNo", ClaimNo);

            cmdi.ExecuteNonQuery();
            cmdi.Parameters.Clear();
            cmdi.Dispose();
            conn.Close();

            PanelPrint.Controls.Add(new LiteralControl(HtmlString));
            Session["ctrl"] = PanelPrint;
            ClientScript.RegisterStartupScript(this.GetType(), "onclick", "<script language=javascript>window.open('Print.aspx','PrintMe','height=900px,width=1200px,scrollbars=1');</script>");
            Display();
        }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
        //   try
        //    {
            if (IS_EDIT_ACTIVE == "Enable")
            { 
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int PurchaseClaimID = Convert.ToInt32(TextPurchaseClaimID.Text);
                string EmpID = DropDownEmployeeID.Text;
                int ClaimNoFrom = Convert.ToInt32(TextClaimNo.Text);
                int PaymentTypeID = Convert.ToInt32(DropDownPaymentTypeID.Text);
                double TotalAmount = Convert.ToDouble(TextTotalAmount.Text);
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                string MakeEntryDate = EntryDate.Text;
                string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-"); 
                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                string ClaimForMonthsGet = TextMonthYear4.Text;
                string[] MakeClaimForMonthsSplit = ClaimForMonthsGet.Split('/');
                String ClaimMonths = MakeClaimForMonthsSplit[0];
                String ClaimYear = MakeClaimForMonthsSplit[1];
                string ClaimDay = "01";
                string ClaimForMonthsTemp = ClaimDay + "-" + ClaimMonths + "-" + ClaimYear;
                DateTime ClaimForMonthsNew = DateTime.ParseExact(ClaimForMonthsTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string ClaimForMonths = ClaimForMonthsNew.ToString("dd-MM-yyyy");

                    string makeSlipSQL = " SELECT PPC.CLAIM_NO, PPM.SLIP_NO FROM MS_PURCHASE_CLAIM PPC LEFT JOIN MS_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO WHERE  PPC.PURCHASE_CLAIM_ID = '" + PurchaseClaimID + "'";

                cmdl = new OracleCommand(makeSlipSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                 
                    for (int i = 0; i < RowCount; i++)
                    {
                        int ClaimNo  = Convert.ToInt32(dt.Rows[i]["CLAIM_NO"]);
                        string update_user = "update  MS_PURCHASE_MASTER set CLAIM_NO =:NoClaimID  where CLAIM_NO ='" + ClaimNo + "' ";
                        cmdi = new OracleCommand(update_user, conn);

                            OracleParameter[] objPrm = new OracleParameter[1]; 
                            objPrm[0] = cmdi.Parameters.Add("NoClaimID", ""); 

                            cmdi.ExecuteNonQuery();
                         
                    }
                  
                foreach (ListItem li in DropDownSlipNoWp.Items)
                {

                    if (li.Selected == true)
                    {
                        string[] WbSlipNo = li.Value.Split('-');
                        string update_purchase = " update  MS_PURCHASE_MASTER set CLAIM_NO =:NoClaimNo  where PURCHASE_ID =: NoSlipNo ";
                        cmdi = new OracleCommand(update_purchase, conn); 
                        OracleParameter[] objPrm = new OracleParameter[2]; 
                        objPrm[0] = cmdi.Parameters.Add("NoSlipNo", WbSlipNo[0]);
                        objPrm[1] = cmdi.Parameters.Add("NoClaimNo", ClaimNoFrom); 

                        cmdi.ExecuteNonQuery(); 
                    } 
                }
                   
                cmdi.Parameters.Clear();
                cmdi.Dispose();


                string update_purchase_claim = " update  MS_PURCHASE_CLAIM set CLAIM_NO =:NoClaimNo, EMP_ID =:NoEmpID,  CLAIM_DATE = TO_DATE(:ClaimDate, 'DD/MM/YYYY'), CLAIM_FOR_MONTH = TO_DATE(:ClaimForMonth, 'DD/MM/YYYY'), PAYMENT_TYPE_ID =:NoPaymentTypeID, TOTAL_AMOUNT =:NoTotalAmount, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where PURCHASE_CLAIM_ID =: NoPurClaimID ";
                cmdu = new OracleCommand(update_purchase_claim, conn);

                OracleParameter[] objPr = new OracleParameter[10];
                objPr[0] = cmdu.Parameters.Add("NoClaimNo", ClaimNoFrom); 
                objPr[1] = cmdu.Parameters.Add("NoEmpID", EmpID);
                objPr[2] = cmdu.Parameters.Add("ClaimForMonth", ClaimForMonths);
                objPr[3] = cmdu.Parameters.Add("ClaimDate", EntryDateNew);
                objPr[4] = cmdu.Parameters.Add("NoPaymentTypeID", PaymentTypeID);
                objPr[5] = cmdu.Parameters.Add("NoTotalAmount", TotalAmount);
                objPr[6] = cmdu.Parameters.Add("u_date", u_date); 
                objPr[7] = cmdu.Parameters.Add("NoC_USER_ID", userID);
                objPr[8] = cmdu.Parameters.Add("TextIsActive", ISActive);
                objPr[9] = cmdu.Parameters.Add("NoPurClaimID", PurchaseClaimID);


                cmdu.ExecuteNonQuery(); 
                cmdu.Parameters.Clear();
                cmdu.Dispose();

                 
                conn.Close(); 

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Purchase Claim Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                DataTable dtSlipNo = new DataTable();
                DataSet dsp = new DataSet();
                string makePageSQL = " SELECT PPM.SLIP_NO, PPM.SLIP_NO || ' - ' || PC.PARTY_ID || ' - ' || PC.PARTY_NAME AS PARTY_NAME FROM MS_PURCHASE_MASTER PPM  LEFT JOIN WP_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID  WHERE PPM.CLAIM_NO IS NULL  ORDER BY  PPM.SLIP_NO ASC";
                dsp = ExecuteBySqlString(makePageSQL);
                dtSlipNo = (DataTable)dsp.Tables[0];
                DropDownSlipNoWp.DataSource = dtSlipNo;
                DropDownSlipNoWp.DataValueField = "SLIP_NO";
                DropDownSlipNoWp.DataTextField = "PARTY_NAME";
                DropDownSlipNoWp.DataBind();

                Display();
                CheckClaimNo.Text = "";

            }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
          //  }
         //  catch
        //   {
        //       Response.Redirect("~/ParameterError.aspx");
        //   }  
        }


        protected void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (IS_DELETE_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                     
                    int PurchaseClaimID = Convert.ToInt32(TextPurchaseClaimID.Text);  

                    string makeSlipSQL = " SELECT PPC.CLAIM_NO, PPM.SLIP_NO FROM MS_PURCHASE_CLAIM PPC LEFT JOIN MS_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO WHERE  PPC.PURCHASE_CLAIM_ID = '" + PurchaseClaimID + "'";

                    cmdl = new OracleCommand(makeSlipSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    for (int i = 0; i < RowCount; i++)
                    {
                        int ClaimNo = Convert.ToInt32(dt.Rows[i]["CLAIM_NO"]);
                        string update_user = "update  MS_PURCHASE_MASTER set CLAIM_NO =:NoClaimID  where CLAIM_NO ='" + ClaimNo + "' ";
                        cmdi = new OracleCommand(update_user, conn);

                        OracleParameter[] objPrm = new OracleParameter[1];
                        objPrm[0] = cmdi.Parameters.Add("NoClaimID", "");

                        cmdi.ExecuteNonQuery();

                    }
                     
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();


                    string delete_pur_claims = " delete from MS_PURCHASE_CLAIM where PURCHASE_CLAIM_ID  = '" + PurchaseClaimID + "'";

                    cmdi = new OracleCommand(delete_pur_claims, conn);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Claim Data Delete successfully"));
                    alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                    clearText();
                    Display();

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
     
        protected void linkSelectClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             
             LinkButton btn = (LinkButton)sender;
             Session["user_data_id"] = btn.CommandArgument;
             int USER_DATA_ID = Convert.ToInt32(Session["user_data_id"]);

             DataTable dtSlipNo = new DataTable();
             DataSet dsp = new DataSet();
             string makePageSQL = " SELECT PPM.PURCHASE_ID || '-' || PPM.TOTAL_AMOUNT AS PURCHASE_ID, PPM.SLIP_NO || ' - ' || PC.PARTY_ID || ' - ' || PC.PARTY_NAME || ', Item -' || PI.ITEM_NAME || ', Weight- ' || PPM.ITEM_WEIGHT || ', Amount- ' || TO_CHAR(PPM.TOTAL_AMOUNT, '999,999,999.99') AS PARTY_NAME FROM MS_PURCHASE_MASTER PPM  LEFT JOIN MS_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID LEFT JOIN MF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID  WHERE (PPM.CLAIM_NO IS NULL OR PPM.CLAIM_NO = '" + USER_DATA_ID + "' ) ORDER BY  PPM.SLIP_NO ASC"; //  
             dsp = ExecuteBySqlString(makePageSQL);
             dtSlipNo = (DataTable)dsp.Tables[0];
             DropDownSlipNoWp.DataSource = dtSlipNo;
             DropDownSlipNoWp.DataValueField = "PURCHASE_ID";
             DropDownSlipNoWp.DataTextField = "PARTY_NAME";
             DropDownSlipNoWp.DataBind();

             string makeSQL = " SELECT PURCHASE_CLAIM_ID, CLAIM_NO, EMP_ID, TO_CHAR(TO_DATE(CLAIM_DATE),'dd/mm/yyyy') AS CLAIM_DATE, TO_CHAR(TO_DATE(CLAIM_FOR_MONTH),'mm/yyyy') CLAIM_FOR_MONTH, PAYMENT_TYPE_ID, TOTAL_AMOUNT, IS_ACTIVE FROM MS_PURCHASE_CLAIM where CLAIM_NO = '" + USER_DATA_ID + "'";

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
                 EntryDate.Text               = ds.Rows[i]["CLAIM_DATE"].ToString();
                 TextMonthYear4.Text          = ds.Rows[i]["CLAIM_FOR_MONTH"].ToString(); 
                 DropDownPaymentTypeID.Text   = ds.Rows[i]["PAYMENT_TYPE_ID"].ToString();
                 TextTotalAmount.Text         = decimal.Parse(ds.Rows[i]["TOTAL_AMOUNT"].ToString()).ToString(".00");
                CheckIsActive.Checked        = Convert.ToBoolean(ds.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false); 
             }

             string makeSlipSQL = " SELECT PPC.CLAIM_NO, PPM.PURCHASE_ID || '-' || PPM.TOTAL_AMOUNT AS PURCHASE_ID FROM MS_PURCHASE_CLAIM PPC LEFT JOIN MS_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO WHERE PPC.IS_CHECK = 'Incomplete' AND  PPC.CLAIM_NO = '" + USER_DATA_ID + "' ORDER BY PPM.SLIP_NO ASC";

             cmdl = new OracleCommand(makeSlipSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count;  
              foreach (ListItem li in DropDownSlipNoWp.Items)
              { 
                  li.Selected = false;
                  for (int i = 0; i < RowCount; i++)
                  {

                    //  DropDownUserRoleID.Text = dt.Rows[i]["USER_ROLE_ID"].ToString();
                      if (li.Value == dt.Rows[i]["PURCHASE_ID"].ToString())
                      {
                          li.Selected = true;
                        //  lblMessage.Text += "" + "Item value :: " + "</font>" + "<b><font color=red>" + li.Value + "</font></br>";
                      }
                  }
              }

              

             Display();
             conn.Close();
             CheckClaimNo.Text = "";
             alert_box.Visible = false;

             BtnUpdate.Attributes.Add("aria-disabled", "true");
             BtnUpdate.Attributes.Add("class", "btn btn-success active");
             BtnDelete.Attributes.Add("aria-disabled", "true");
             BtnDelete.Attributes.Add("class", "btn btn-danger active");

             BtnAdd.Attributes.Add("aria-disabled", "false");
             BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 
        }
        /*
        public void Display()
        {
            
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            DataTable dtUserTypeID = new DataTable();
            DataSet ds = new DataSet();
       
            string makeSQL = "";
            if (txtSearchUser.Text == "")
            {
                makeSQL = " SELECT PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO, HE.EMP_FNAME || ' ' || HE.EMP_LNAME AS EMP_NAME, PPC.CLAIM_FOR_MONTH, PPC.CLAIM_DATE, NPT.PAYMENT_TYPE_NAME, PPC.TOTAL_AMOUNT, PPC.CREATE_DATE, PPC.UPDATE_DATE, PPC.IS_ACTIVE, IS_OBJ_QUERY, PPC.OBJ_QUERY_DES, PPC.OBJ_QUERY_C_DATE, PPM.SLIP_NO, PC.PARTY_ID || ' - ' || PC.PARTY_NAME AS PARTY_NAME, PPC.IS_CHECK, PPC.IS_PRINT, TO_CHAR(PPC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE  FROM MS_PURCHASE_CLAIM PPC LEFT JOIN MS_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PPC.EMP_ID LEFT JOIN NRC_PAYMENT_TYPE NPT ON NPT.PAYMENT_TYPE_ID = PPC.PAYMENT_TYPE_ID LEFT JOIN WP_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID WHERE PPC.IS_CHECK = 'Incomplete' ORDER BY PPC.CLAIM_NO DESC, PPM.CLAIM_NO ASC "; //WHERE PPC.IS_CHECK = 'Incomplete'
            }
            else
            {
                makeSQL = " SELECT PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO, HE.EMP_FNAME || ' ' || HE.EMP_LNAME AS EMP_NAME, PPC.CLAIM_FOR_MONTH, PPC.CLAIM_DATE, NPT.PAYMENT_TYPE_NAME, PPC.TOTAL_AMOUNT, PPC.CREATE_DATE, PPC.UPDATE_DATE, PPC.IS_ACTIVE, IS_OBJ_QUERY, PPC.OBJ_QUERY_DES, PPC.OBJ_QUERY_C_DATE, PPM.SLIP_NO, PC.PARTY_ID || ' - ' || PC.PARTY_NAME AS PARTY_NAME, PPC.IS_CHECK, PPC.IS_PRINT, TO_CHAR(PPC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE FROM MS_PURCHASE_CLAIM PPC LEFT JOIN MS_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PPC.EMP_ID LEFT JOIN NRC_PAYMENT_TYPE NPT ON NPT.PAYMENT_TYPE_ID = PPC.PAYMENT_TYPE_ID LEFT JOIN WP_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID WHERE PPC.CLAIM_NO like '" + txtSearchUser.Text + "%' or  HE.EMP_FNAME like '" + txtSearchUser.Text + "%'  ORDER BY PPC.CLAIM_NO DESC, PPM.SLIP_NO ASC ";
                alert_box.Visible = false;
            }

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            GridView1.DataSource = dt;
            GridView1.DataKeyNames = new string[] { "CLAIM_NO" }; 
            GridView1.DataBind();

            if (dt.Rows.Count > 0)
            {
               GroupGridView(GridView1.Rows, 0, 15);
            }
            else {
                
            }
         
            conn.Close(); 
        }*/

        public void Display()
        {

            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

             
            string makeSQL = "";
            if (txtSearchUser.Text == "")
            {
                 makeSQL = " SELECT PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO, HE.EMP_FNAME || ' ' || HE.EMP_LNAME AS EMP_NAME, PPC.CLAIM_FOR_MONTH, PPC.CLAIM_DATE, NPT.PAYMENT_TYPE_NAME, PPC.TOTAL_AMOUNT, PPC.CREATE_DATE, PPC.UPDATE_DATE, PPC.IS_ACTIVE, IS_OBJ_QUERY, PPC.OBJ_QUERY_DES, PPC.OBJ_QUERY_C_DATE, count(PPM.SLIP_NO) as SLIP_NO, PPC.IS_CHECK, PPC.IS_PRINT, TO_CHAR(PPC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE FROM MS_PURCHASE_CLAIM PPC LEFT JOIN MS_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PPC.EMP_ID LEFT JOIN NRC_PAYMENT_TYPE NPT ON NPT.PAYMENT_TYPE_ID = PPC.PAYMENT_TYPE_ID LEFT JOIN WP_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID WHERE PPC.IS_CHECK = 'Incomplete' AND PPC.IS_ACTIVE = 'Enable' GROUP BY PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO, HE.EMP_FNAME || ' ' || HE.EMP_LNAME, PPC.CLAIM_FOR_MONTH, PPC.CLAIM_DATE, NPT.PAYMENT_TYPE_NAME, PPC.TOTAL_AMOUNT, PPC.CREATE_DATE, PPC.UPDATE_DATE, PPC.IS_ACTIVE, IS_OBJ_QUERY, PPC.OBJ_QUERY_DES, PPC.OBJ_QUERY_C_DATE, PPC.IS_CHECK, PPC.IS_PRINT, TO_CHAR(PPC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') ORDER BY PPC.CLAIM_NO DESC "; //WHERE PPC.IS_CHECK = 'Incomplete'
            }
            else
            {
                 makeSQL = " SELECT PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO, HE.EMP_FNAME || ' ' || HE.EMP_LNAME AS EMP_NAME, PPC.CLAIM_FOR_MONTH, PPC.CLAIM_DATE, NPT.PAYMENT_TYPE_NAME, PPC.TOTAL_AMOUNT, PPC.CREATE_DATE, PPC.UPDATE_DATE, PPC.IS_ACTIVE, IS_OBJ_QUERY, PPC.OBJ_QUERY_DES, PPC.OBJ_QUERY_C_DATE, count(PPM.SLIP_NO) as SLIP_NO, PPC.IS_CHECK, PPC.IS_PRINT, TO_CHAR(PPC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS PRINT_DATE FROM MS_PURCHASE_CLAIM PPC LEFT JOIN MS_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PPC.EMP_ID LEFT JOIN NRC_PAYMENT_TYPE NPT ON NPT.PAYMENT_TYPE_ID = PPC.PAYMENT_TYPE_ID LEFT JOIN WP_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID WHERE PPC.CLAIM_NO like '" + txtSearchUser.Text + "%' GROUP BY PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO, HE.EMP_FNAME || ' ' || HE.EMP_LNAME, PPC.CLAIM_FOR_MONTH, PPC.CLAIM_DATE, NPT.PAYMENT_TYPE_NAME, PPC.TOTAL_AMOUNT, PPC.CREATE_DATE, PPC.UPDATE_DATE, PPC.IS_ACTIVE, IS_OBJ_QUERY, PPC.OBJ_QUERY_DES, PPC.OBJ_QUERY_C_DATE, PPC.IS_CHECK, PPC.IS_PRINT, TO_CHAR(PPC.PRINT_DATE, 'DD/MM/YYYY HH:MI:SS AM') ORDER BY PPC.CLAIM_NO DESC ";
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

        protected void gridViewFileInformation_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow Row in GridView4D.Rows)
            {
                string isCheck = (Row.FindControl("IsActiveCheckLink") as Label).Text; 
                if (isCheck == "Complete")
                {
                    (Row.FindControl("linkSelect") as LinkButton).Visible = false;
                }
            }
        }

  

        public void clearTextField(object sender, EventArgs e)
        {
            
           // DropDownPaymentTypeID.Text = "0";
            TextClaimNo.Text = "";
            EntryDate.Text = "";
            DropDownEmployeeID.Text = "0";
            DropDownSlipNoWp.SelectedIndex = -1;
            CheckClaimNo.Text = "";
            TextTotalAmount.Text = "";
        }

        public void clearText()
        {
              
          //  DropDownPaymentTypeID.Text = "0";
            TextClaimNo.Text = "";
            EntryDate.Text = "";
            DropDownEmployeeID.Text = "0";
            DropDownSlipNoWp.SelectedIndex = -1;
            CheckClaimNo.Text = "";
            TextTotalAmount.Text = "";
        }


        public void TextClaimNo_TextChanged(object sender, EventArgs e)
        {
            string ClaimNo = TextClaimNo.Text;
            string MatchEmpIDPattern = "^([0-9]{4})$";
            if (ClaimNo != null)
            {

                if (Regex.IsMatch(ClaimNo, MatchEmpIDPattern))
                {
                    alert_box.Visible = false;

                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "select CLAIM_NO from MS_PURCHASE_CLAIM where CLAIM_NO = '" + Convert.ToInt32(ClaimNo) + "'";
                    cmd.CommandType = CommandType.Text;

                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        CheckClaimNo.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Claim Number is already used.</label>";
                        CheckClaimNo.ForeColor = System.Drawing.Color.Red;
                        TextClaimNo.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");


                    }
                    else
                    {
                        CheckClaimNo.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Claim Number is available</label>";
                        CheckClaimNo.ForeColor = System.Drawing.Color.Green;
                        TextClaimNo.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");
                         

                    }
                }
                else
                {
                    CheckClaimNo.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter Claim Number is 4 digit only</label>";
                    CheckClaimNo.ForeColor = System.Drawing.Color.Red;
                    TextClaimNo.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

                }
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

        public static string NumberToWords(double doubleNumber)
        {
            var beforeFloatingPoint = (int)Math.Floor(doubleNumber);
            var beforeFloatingPointWord = $"{NumberToWords(beforeFloatingPoint)} ";
            var afterFloatingPointWord = "";
            if ((int)((doubleNumber - beforeFloatingPoint) * 100) == 0)
            {
                afterFloatingPointWord = " only";
            }
            else
            {
                afterFloatingPointWord = $"and halalas {SmallNumberToWord((int)((doubleNumber - beforeFloatingPoint) * 100), "")} only.";
            }

             
            return $"{beforeFloatingPointWord} {afterFloatingPointWord}";
        }

        private static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            var words = "";

            if (number / 1000000000 > 0)
            {
                words += NumberToWords(number / 1000000000) + " billion ";
                number %= 1000000000;
            }

            if (number / 1000000 > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if (number / 1000 > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            words = SmallNumberToWord(number, words);

            return words;
        }

        private static string SmallNumberToWord(int number, string words)
        {
            if (number <= 0) return words;
            if (words != "")
                words += " ";

            var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += "-" + unitsMap[number % 10];
            }
            return words;
        }
    }
}