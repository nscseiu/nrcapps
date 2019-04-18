using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.OracleClient;
using System.IO; 
using System.Collections.Generic; 
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization;


namespace NRCAPPS.PF
{
    public partial class PfPurchaseClaim : System.Web.UI.Page
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
                        string makeEmployeeSQL = " SELECT EMP_ID, EMP_FNAME || ' ' ||EMP_LNAME AS EMP_NAME FROM HR_EMPLOYEES WHERE IS_ACTIVE = 'Enable' ORDER BY EMP_ID ASC";
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
                        string makePageSQL = " SELECT PPM.SLIP_NO, PPM.SLIP_NO || ' - ' || PC.PARTY_ID || ' - ' || PC.PARTY_NAME AS PARTY_NAME FROM PF_PURCHASE_MASTER PPM  LEFT JOIN PF_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID  WHERE PPM.CLAIM_NO IS NULL  ORDER BY  PPM.SLIP_NO ASC";
                        dsp = ExecuteBySqlString(makePageSQL);
                        dtSlipNo = (DataTable)dsp.Tables[0];
                        DropDownSlipNo.DataSource = dtSlipNo;
                        DropDownSlipNo.DataValueField = "SLIP_NO";
                        DropDownSlipNo.DataTextField = "PARTY_NAME";
                        DropDownSlipNo.DataBind();
                    
                           
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
                    int EmpID = Convert.ToInt32(DropDownEmployeeID.Text);
                    int ClaimNoFrom = Convert.ToInt32(TextClaimNo.Text);
                    int PaymentTypeID = Convert.ToInt32(DropDownPaymentTypeID.Text);
                    double TotalAmount = Convert.ToDouble(TextTotalAmount.Text);

                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');

                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-"); 
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                    string get_pur_claim_id = "select PF_PURCHASE_CLAIM_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_pur_claim_id, conn);
                    int newPurClaimID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");


                    foreach (ListItem li in DropDownSlipNo.Items)
                    {

                        if (li.Selected == true)
                        {
                            string update_purchase = " update  PF_PURCHASE_MASTER set CLAIM_NO =:NoClaimNo  where SLIP_NO =: NoSlipNo ";
                            cmdi = new OracleCommand(update_purchase, conn);

                            OracleParameter[] objPrm = new OracleParameter[3];
                            objPrm[0] = cmdi.Parameters.Add("NoSlipNo", li.Value);
                            objPrm[1] = cmdi.Parameters.Add("NoClaimNo", ClaimNoFrom);

                            cmdi.ExecuteNonQuery();
                        }
                    }

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    string insert_pur_claim = "insert into PF_PURCHASE_CLAIM (PURCHASE_CLAIM_ID, CLAIM_NO, EMP_ID, CLAIM_DATE, PAYMENT_TYPE_ID, TOTAL_AMOUNT, CREATE_DATE, C_USER_ID, IS_ACTIVE, IS_CHECK, IS_OBJ_QUERY) VALUES ( :NoPurClaimID, :NoClaimNo, :NoEmpID, TO_DATE(:ClaimDate, 'DD/MM/YYYY'), :NoPaymentTypeID, :NoTotalAmount, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID, :TextIsActive, :TextIsCheck, :TextIsObjQuery)";
                    cmdi = new OracleCommand(insert_pur_claim, conn);

                    OracleParameter[] objPr = new OracleParameter[11];
                    objPr[0] = cmdi.Parameters.Add("NoPurClaimID", newPurClaimID);
                    objPr[1] = cmdi.Parameters.Add("NoClaimNo", ClaimNoFrom);
                    objPr[2] = cmdi.Parameters.Add("NoEmpID", EmpID);
                    objPr[3] = cmdi.Parameters.Add("ClaimDate", EntryDateNew);
                    objPr[4] = cmdi.Parameters.Add("NoPaymentTypeID", PaymentTypeID);
                    objPr[5] = cmdi.Parameters.Add("NoTotalAmount", TotalAmount); 
                    objPr[6] = cmdi.Parameters.Add("c_date", c_date);
                    objPr[7] = cmdi.Parameters.Add("NoCuserID", userID);
                    objPr[8] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPr[9] = cmdi.Parameters.Add("TextIsCheck", "Incomplete");
                    objPr[10] = cmdi.Parameters.Add("TextIsObjQuery", "No");

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


        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
           try
            {
            if (IS_EDIT_ACTIVE == "Enable")
            { 
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int PurchaseClaimID = Convert.ToInt32(TextPurchaseClaimID.Text);
                int EmpID = Convert.ToInt32(DropDownEmployeeID.Text);
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

                string makeSlipSQL = " SELECT PPC.CLAIM_NO, PPM.SLIP_NO FROM PF_PURCHASE_CLAIM PPC LEFT JOIN PF_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO WHERE  PPC.PURCHASE_CLAIM_ID = '" + PurchaseClaimID + "'";

                cmdl = new OracleCommand(makeSlipSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                 
                    for (int i = 0; i < RowCount; i++)
                    {
                        int ClaimNo  = Convert.ToInt32(dt.Rows[i]["CLAIM_NO"]);
                        string update_user = "update  PF_PURCHASE_MASTER set CLAIM_NO =:NoClaimID  where CLAIM_NO ='" + ClaimNo + "' ";
                        cmdi = new OracleCommand(update_user, conn);

                            OracleParameter[] objPrm = new OracleParameter[1]; 
                            objPrm[0] = cmdi.Parameters.Add("NoClaimID", ""); 

                            cmdi.ExecuteNonQuery();
                         
                    }
                  
                foreach (ListItem li in DropDownSlipNo.Items)
                {

                    if (li.Selected == true)
                    {
                        string update_purchase = " update  PF_PURCHASE_MASTER set CLAIM_NO =:NoClaimNo  where SLIP_NO =: NoSlipNo ";
                        cmdi = new OracleCommand(update_purchase, conn);

                        OracleParameter[] objPrm = new OracleParameter[3]; 
                        objPrm[0] = cmdi.Parameters.Add("NoSlipNo", li.Value);
                        objPrm[1] = cmdi.Parameters.Add("NoClaimNo", ClaimNoFrom); 

                        cmdi.ExecuteNonQuery(); 
                    } 
                }
                   
                cmdi.Parameters.Clear();
                cmdi.Dispose();


                string update_purchase_claim = " update  PF_PURCHASE_CLAIM set CLAIM_NO =:NoClaimNo, EMP_ID =:NoEmpID,  CLAIM_DATE = TO_DATE(:ClaimDate, 'DD/MM/YYYY'), PAYMENT_TYPE_ID =:NoPaymentTypeID, TOTAL_AMOUNT =:NoTotalAmount, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where PURCHASE_CLAIM_ID =: NoPurClaimID ";
                cmdu = new OracleCommand(update_purchase_claim, conn);

                OracleParameter[] objPr = new OracleParameter[9];
                objPr[0] = cmdu.Parameters.Add("NoClaimNo", ClaimNoFrom); 
                objPr[1] = cmdu.Parameters.Add("NoEmpID", EmpID);
                objPr[2] = cmdu.Parameters.Add("ClaimDate", EntryDateNew);
                objPr[3] = cmdu.Parameters.Add("NoPaymentTypeID", PaymentTypeID);
                objPr[4] = cmdu.Parameters.Add("NoTotalAmount", TotalAmount);
                objPr[5] = cmdu.Parameters.Add("u_date", u_date); 
                objPr[6] = cmdu.Parameters.Add("NoC_USER_ID", userID);
                objPr[7] = cmdu.Parameters.Add("TextIsActive", ISActive);
                objPr[8] = cmdu.Parameters.Add("NoPurClaimID", PurchaseClaimID);


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
                string makePageSQL = " SELECT PPM.SLIP_NO, PPM.SLIP_NO || ' - ' || PC.PARTY_ID || ' - ' || PC.PARTY_NAME AS PARTY_NAME FROM PF_PURCHASE_MASTER PPM  LEFT JOIN PF_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID  WHERE PPM.CLAIM_NO IS NULL  ORDER BY  PPM.SLIP_NO ASC";
                dsp = ExecuteBySqlString(makePageSQL);
                dtSlipNo = (DataTable)dsp.Tables[0];
                DropDownSlipNo.DataSource = dtSlipNo;
                DropDownSlipNo.DataValueField = "SLIP_NO";
                DropDownSlipNo.DataTextField = "PARTY_NAME";
                DropDownSlipNo.DataBind();

                Display();
                CheckClaimNo.Text = "";

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
                     
                    int PurchaseClaimID = Convert.ToInt32(TextPurchaseClaimID.Text);  

                    string makeSlipSQL = " SELECT PPC.CLAIM_NO, PPM.SLIP_NO FROM PF_PURCHASE_CLAIM PPC LEFT JOIN PF_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO WHERE  PPC.PURCHASE_CLAIM_ID = '" + PurchaseClaimID + "'";

                    cmdl = new OracleCommand(makeSlipSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    for (int i = 0; i < RowCount; i++)
                    {
                        int ClaimNo = Convert.ToInt32(dt.Rows[i]["CLAIM_NO"]);
                        string update_user = "update  PF_PURCHASE_MASTER set CLAIM_NO =:NoClaimID  where CLAIM_NO ='" + ClaimNo + "' ";
                        cmdi = new OracleCommand(update_user, conn);

                        OracleParameter[] objPrm = new OracleParameter[1];
                        objPrm[0] = cmdi.Parameters.Add("NoClaimID", "");

                        cmdi.ExecuteNonQuery();

                    }
                     
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();


                    string delete_pur_claims = " delete from PF_PURCHASE_CLAIM where PURCHASE_CLAIM_ID  = '" + PurchaseClaimID + "'";

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
             string makePageSQL = " SELECT PPM.SLIP_NO, PPM.SLIP_NO || ' - ' || PC.PARTY_ID || ' - ' || PC.PARTY_NAME || ', Item -' || PI.ITEM_NAME || ', Weight- ' || PPM.ITEM_WEIGHT || ', Amount- ' || TO_CHAR(PPM.ITEM_AMOUNT, '999,999,999.99') AS PARTY_NAME FROM PF_PURCHASE_MASTER PPM  LEFT JOIN PF_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPM.ITEM_ID  WHERE (PPM.CLAIM_NO IS NULL OR PPM.CLAIM_NO = '" + USER_DATA_ID + "' ) ORDER BY  PPM.SLIP_NO ASC"; //  
             dsp = ExecuteBySqlString(makePageSQL);
             dtSlipNo = (DataTable)dsp.Tables[0];
             DropDownSlipNo.DataSource = dtSlipNo;
             DropDownSlipNo.DataValueField = "SLIP_NO";
             DropDownSlipNo.DataTextField = "PARTY_NAME";
             DropDownSlipNo.DataBind();

             string makeSQL = " SELECT PURCHASE_CLAIM_ID, CLAIM_NO, EMP_ID, TO_CHAR(TO_DATE(CLAIM_DATE),'dd/mm/yyyy') AS CLAIM_DATE, PAYMENT_TYPE_ID, TOTAL_AMOUNT, IS_ACTIVE FROM PF_PURCHASE_CLAIM where CLAIM_NO = '" + USER_DATA_ID + "'";

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
                 DropDownPaymentTypeID.Text   = ds.Rows[i]["PAYMENT_TYPE_ID"].ToString();
                 TextTotalAmount.Text         = ds.Rows[i]["TOTAL_AMOUNT"].ToString();
                 CheckIsActive.Checked        = Convert.ToBoolean(ds.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false); 
             }

             string makeSlipSQL = " SELECT PPC.CLAIM_NO, PPM.SLIP_NO FROM PF_PURCHASE_CLAIM PPC LEFT JOIN PF_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO WHERE PPC.IS_CHECK = 'Incomplete' AND  PPC.CLAIM_NO = '" + USER_DATA_ID + "' ORDER BY PPM.SLIP_NO ASC";

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

        public void Display()
        {
            
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            DataTable dtUserTypeID = new DataTable();
            DataSet ds = new DataSet();
       
            string makeSQL = "";
            if (txtSearchUser.Text == "")
            {
                makeSQL = " SELECT PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO, HE.EMP_FNAME || ' ' || HE.EMP_LNAME AS EMP_NAME, PPC.CLAIM_DATE, NPT.PAYMENT_TYPE_NAME, PPC.TOTAL_AMOUNT, PPC.CREATE_DATE, PPC.UPDATE_DATE, PPC.IS_ACTIVE, IS_OBJ_QUERY, PPC.OBJ_QUERY_DES, PPC.OBJ_QUERY_C_DATE, PPM.SLIP_NO, PC.PARTY_ID || ' - ' || PC.PARTY_NAME AS PARTY_NAME, PPC.IS_CHECK FROM PF_PURCHASE_CLAIM PPC LEFT JOIN PF_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PPC.EMP_ID LEFT JOIN NRC_PAYMENT_TYPE NPT ON NPT.PAYMENT_TYPE_ID = PPC.PAYMENT_TYPE_ID LEFT JOIN PF_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID  ORDER BY PPC.CLAIM_NO DESC, PPM.CLAIM_NO ASC "; //WHERE PPC.IS_CHECK = 'Incomplete'
            }
            else
            {
                makeSQL = " SELECT PPC.PURCHASE_CLAIM_ID, PPC.CLAIM_NO, HE.EMP_FNAME || ' ' || HE.EMP_LNAME AS EMP_NAME, PPC.CLAIM_DATE, NPT.PAYMENT_TYPE_NAME, PPC.TOTAL_AMOUNT, PPC.CREATE_DATE, PPC.UPDATE_DATE, PPC.IS_ACTIVE, IS_OBJ_QUERY, PPC.OBJ_QUERY_DES, PPC.OBJ_QUERY_C_DATE, PPM.SLIP_NO, PC.PARTY_ID || ' - ' || PC.PARTY_NAME AS PARTY_NAME, PPC.IS_CHECK FROM PF_PURCHASE_CLAIM PPC LEFT JOIN PF_PURCHASE_MASTER PPM ON PPM.CLAIM_NO = PPC.CLAIM_NO LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = PPC.EMP_ID LEFT JOIN NRC_PAYMENT_TYPE NPT ON NPT.PAYMENT_TYPE_ID = PPC.PAYMENT_TYPE_ID LEFT JOIN PF_PARTY PC ON PC.PARTY_ID = PPM.PARTY_ID WHERE PPC.CLAIM_NO like '" + txtSearchUser.Text + "%' or  HE.EMP_FNAME like '" + txtSearchUser.Text + "%'  ORDER BY PPC.CLAIM_NO DESC, PPM.SLIP_NO ASC ";
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
        }

     

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

         
        protected void GridViewSearchUser(object sender, EventArgs e)
        {
            this.Display();
        }
 
         protected void GridViewPage_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            
            try
            {
                GridView1.PageIndex = e.NewPageIndex;
                Display();
            }
            catch
            {
            }
          
            alert_box.Visible = false;
        }

      

        public void clearTextField(object sender, EventArgs e)
        {
            
           // DropDownPaymentTypeID.Text = "0";
            TextClaimNo.Text = "";
            EntryDate.Text = "";
            DropDownEmployeeID.Text = "0";
            DropDownSlipNo.SelectedIndex = -1;
            CheckClaimNo.Text = "";
        }

        public void clearText()
        {
              
          //  DropDownPaymentTypeID.Text = "0";
            TextClaimNo.Text = "";
            EntryDate.Text = "";
            DropDownEmployeeID.Text = "0";
            DropDownSlipNo.SelectedIndex = -1;
            CheckClaimNo.Text = "";
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
                    cmd.CommandText = "select CLAIM_NO from PF_PURCHASE_CLAIM where CLAIM_NO = '" + Convert.ToInt32(ClaimNo) + "'";
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

 
   }
}