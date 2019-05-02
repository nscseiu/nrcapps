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



namespace NRCAPPS.MS
{
    public partial class MsParty : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
        OracleDataAdapter oradata;
        DataTable dt, ds;
        int RowCount;

        string IS_PAGE_ACTIVE   = "";
        string IS_ADD_ACTIVE    = "";
        string IS_EDIT_ACTIVE   = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE   = "";  
 
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
                    IS_PAGE_ACTIVE   = dt.Rows[i]["IS_PAGE_ACTIVE"].ToString();
                    IS_ADD_ACTIVE    = dt.Rows[i]["IS_ADD_ACTIVE"].ToString();
                    IS_EDIT_ACTIVE   = dt.Rows[i]["IS_EDIT_ACTIVE"].ToString();
                    IS_DELETE_ACTIVE = dt.Rows[i]["IS_DELETE_ACTIVE"].ToString();
                    IS_VIEW_ACTIVE   = dt.Rows[i]["IS_VIEW_ACTIVE"].ToString();  
                }

                if (IS_PAGE_ACTIVE == "Enable")
                {
                     
                    if (!IsPostBack)
                    {
                        DataTable dtPartyID = new DataTable();
                        DataSet dsp = new DataSet();
                        string makePartySQL = " SELECT * FROM HR_COUNTRIES ORDER BY COUNTRY_NAME ASC";
                        dsp = ExecuteBySqlString(makePartySQL);
                        dtPartyID = (DataTable)dsp.Tables[0];
                        DropDownCountryID.DataSource = dtPartyID;
                        DropDownCountryID.DataValueField = "COUNTRY_ID";
                        DropDownCountryID.DataTextField = "COUNTRY_NAME";
                        DropDownCountryID.DataBind();
                        DropDownCountryID.Items.Insert(0, new ListItem("Select Country Name", "0"));

                        DataTable dtRepID = new DataTable();
                        DataSet dsr = new DataSet();
                        string makeRepSQL = " SELECT REPRESENTATIVE_ID, NID_NO || ' : ' || REPRESENTATIVE_NAME AS REPRESENTATIVE_NAME FROM MS_REPRESENTATIVE ORDER BY REPRESENTATIVE_ID ASC";
                        dsr = ExecuteBySqlString(makeRepSQL);
                        dtRepID = (DataTable)dsr.Tables[0];
                        DropDownRepresentativeID.DataSource = dtRepID;
                        DropDownRepresentativeID.DataValueField = "REPRESENTATIVE_ID";
                        DropDownRepresentativeID.DataTextField = "REPRESENTATIVE_NAME";
                        DropDownRepresentativeID.DataBind();
                        DropDownRepresentativeID.Items.Insert(0, new ListItem("Select Representative Name", "0"));

                        string get_batch_id = " select LAST_NUMBER from all_sequences where sequence_name = 'MS_PARTY_ID_SEQ'";
                        cmdu = new OracleCommand(get_batch_id, conn);
                        TextSupplierID.Text = cmdu.ExecuteScalar().ToString();

                        TextSupplierName.Focus();
                        Display();
                        DisplayAddress();
                        alert_box.Visible = false;

                    } 
                    IsLoad = false;
                }
                else {
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
        //    try
        //    {
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]); 
                    string get_supplier_id = "select MS_PARTY_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_supplier_id, conn);
                    int newSupplierID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string IsPurchaseActive = CheckIsPurchaseActive.Checked ? "Enable" : "Disable";
                    string IsSalesActive = CheckIsSalesActive.Checked ? "Enable" : "Disable";

                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string insert_user = "insert into MS_PARTY (PARTY_ID, PARTY_NAME, PARTY_ARABIC_NAME, PARTY_VAT_NO, PARTY_ADD_1, PARTY_ADD_2, CONTACT_NO, IS_ACTIVE, IS_PURCHASE_ACTIVE, IS_SALES_ACTIVE, CREATE_DATE, C_USER_ID) VALUES ( :NoSupplierID, :TextSupplierName, :TextSupArabicName, :TextSupVatNo, :TextSup_Add_1, :TextSup_Add_2, :TextContactNo, :TextIsActive, :TextIsPurchaseActive, :TextIsSalesActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[12];
                    objPrm[0] = cmdi.Parameters.Add("NoSupplierID", newSupplierID);
                    objPrm[1] = cmdi.Parameters.Add("TextSupplierName", TextSupplierName.Text);
                    objPrm[2] = cmdi.Parameters.Add("TextSupArabicName", TextSupArabicName.Text);
                    objPrm[3] = cmdi.Parameters.Add("TextSupVatNo", TextSupVatNo.Text);
                    objPrm[4] = cmdi.Parameters.Add("TextSup_Add_1", TextSup_Add_1.Text);
                    objPrm[5] = cmdi.Parameters.Add("TextSup_Add_2", TextSup_Add_2.Text);
                    objPrm[6] = cmdi.Parameters.Add("TextContactNo", TextContactNo.Text); 
                    objPrm[7] = cmdi.Parameters.Add("TextIsActive", ISActive); 
                    objPrm[8] = cmdi.Parameters.Add("TextIsPurchaseActive", IsPurchaseActive);
                    objPrm[9] = cmdi.Parameters.Add("TextIsSalesActive", IsSalesActive);
                    objPrm[10] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[11] = cmdi.Parameters.Add("NoCuserID", userID);
                 
                    cmdi.ExecuteNonQuery(); 
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                foreach (ListItem li in DropDownRepresentativeID.Items)
                {

                    if (li.Selected == true)
                    {
                        string RepresentativeID = li.Value;
                        string insert_pur_rep = " insert into MS_PARTY_REPRESENTATIVE (PARTY_ID, REPRESENTATIVE_ID) VALUES ( :NoPartyID, :NoRepresentativeID) ";
                        cmdi = new OracleCommand(insert_pur_rep, conn);

                        OracleParameter[] objPr = new OracleParameter[3];
                        objPr[0] = cmdi.Parameters.Add("NoPartyID", newSupplierID);
                        objPr[1] = cmdi.Parameters.Add("NoRepresentativeID", RepresentativeID); 

                        cmdi.ExecuteNonQuery();
                    }
                }
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Party Data Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                    clearText();
                    TextSupplierName.Focus();
                    Display();
                }
                else { 
                    Response.Redirect("~/PagePermissionError.aspx");
                }
        //    }
        //    catch
        //    {
        //        Response.Redirect("~/ParameterError.aspx");
          //  }
        }

        public void BtnAddAddress_Click(object sender, EventArgs e)
        {
            try
            {
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    int PartyID = Convert.ToInt32(TextSupplierID.Text); 
                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string get_id = "select MS_PARTY_ADDRESS_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_id, conn);
                    int newID = Int32.Parse(cmdu.ExecuteScalar().ToString());

                    string insert_ex_sales = "insert into MS_PARTY_ADDRESS (PARTY_ADDRESS_ID, PARTY_ID, PARTY_ADD_1, PARTY_ADD_2, COUNTRY_ID, CREATE_DATE, C_USER_ID) VALUES (:NoPartyAddID, :TextPartyNo, :TextAddressLine1, :TextAddressLine2, :TextCountryNo, TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'),  :NoCuserID)";
                    cmdi = new OracleCommand(insert_ex_sales, conn);
                    OracleParameter[] objPr = new OracleParameter[7];
                    objPr[0] = cmdi.Parameters.Add("NoPartyAddID", newID);
                    objPr[1] = cmdi.Parameters.Add("TextPartyNo", PartyID);
                    objPr[2] = cmdi.Parameters.Add("TextAddressLine1", TextAddressLine1.Text);
                    objPr[3] = cmdi.Parameters.Add("TextAddressLine2", TextAddressLine2.Text);
                    objPr[4] = cmdi.Parameters.Add("TextCountryNo", DropDownCountryID.Text);
                    objPr[5] = cmdi.Parameters.Add("c_date", c_date);
                    objPr[6] = cmdi.Parameters.Add("NoCuserID", userID);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert Party Address Data Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                    //   clearText();
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
            try{
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             LinkButton btn = (LinkButton)sender;
             Session["user_page_data_id"] = btn.CommandArgument;
             int USER_DATA_ID = Convert.ToInt32(Session["user_page_data_id"]); 
              
             string makeSQL = " select *  from MS_PARTY where PARTY_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextSupplierID.Text = dt.Rows[i]["PARTY_ID"].ToString();
                 TextSupplierName.Text = dt.Rows[i]["PARTY_NAME"].ToString();
                 TextSupArabicName.Text = dt.Rows[i]["PARTY_ARABIC_NAME"].ToString();
                 TextSupVatNo.Text   = dt.Rows[i]["PARTY_VAT_NO"].ToString();
                 TextSup_Add_1.Text = dt.Rows[i]["PARTY_ADD_1"].ToString();
                 TextSup_Add_2.Text = dt.Rows[i]["PARTY_ADD_2"].ToString();
                 TextContactNo.Text = dt.Rows[i]["CONTACT_NO"].ToString();
              //   DropDownRepresentativeID.Text = dt.Rows[i]["REPRESENTATIVE_ID"].ToString(); 
                 CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);


                         
             }
                string makeSlipSQL = " SELECT REPRESENTATIVE_ID FROM MS_PARTY_REPRESENTATIVE  WHERE PARTY_ID = '" + USER_DATA_ID + "' ORDER BY REPRESENTATIVE_ID ASC  ";

                cmdl = new OracleCommand(makeSlipSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                ds = new DataTable();
                oradata.Fill(ds);
                RowCount = ds.Rows.Count;
                foreach (ListItem li in DropDownRepresentativeID.Items)
                {
                    li.Selected = false;
                    for (int i = 0; i < RowCount; i++)
                    {
                        if (li.Value == ds.Rows[i]["REPRESENTATIVE_ID"].ToString())
                        {
                            li.Selected = true;
                        }
                    }
                }

                conn.Close();
             Display();
             TextSupplierName.Focus();
             CheckSupplierName.Text = "";
             alert_box.Visible = false;
             BtnAdd.Attributes.Add("aria-disabled", "false");
             BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
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

                string makeSQL  = " select  MP.* from MS_PARTY MP  ORDER BY MP.UPDATE_DATE desc, MP.CREATE_DATE desc";
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView4D.DataSource = dt;
                GridView4D.DataKeyNames = new string[] { "PARTY_ID" }; 
                GridView4D.DataBind();
                conn.Close(); 
                TextSupplierName.Focus();
                DisplayAddress();
            }
            else
            {
             //   Response.Redirect("~/PagePermissionError.aspx");
            }
        }


        public void DisplayAddress()
        {

            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            string makeSQL = " SELECT WPA.*, HC.COUNTRY_NAME FROM MS_PARTY_ADDRESS WPA LEFT JOIN HR_COUNTRIES HC ON HC.COUNTRY_ID = WPA.COUNTRY_ID WHERE WPA.PARTY_ID = '" + TextSupplierID.Text + "' ";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            GridView2.DataSource = dt;
            GridView2.DataKeyNames = new string[] { "PARTY_ADDRESS_ID" };
            GridView2.DataBind();

            conn.Close();
        }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]); 
                int USER_DATA_ID = Convert.ToInt32(TextSupplierID.Text);   
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string IsPurchaseActive = CheckIsPurchaseActive.Checked ? "Enable" : "Disable";
                string IsSalesActive = CheckIsSalesActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  MS_PARTY  set PARTY_NAME = :TextSupplierName, PARTY_ARABIC_NAME = :TextSupArabicName, PARTY_VAT_NO = :TextSupVatNo, PARTY_ADD_1 = :TextSup_Add_1, PARTY_ADD_2 = :TextSup_Add_2, CONTACT_NO = :TextContactNo, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive, IS_PURCHASE_ACTIVE =:TextIsPurchaseActive, IS_SALES_ACTIVE =:TextIsSalesActive where PARTY_ID = :NoSupplierID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[12];
                objPrm[0] = cmdi.Parameters.Add("TextSupplierName", TextSupplierName.Text);
                objPrm[1] = cmdi.Parameters.Add("TextSupArabicName", TextSupArabicName.Text);
                objPrm[2] = cmdi.Parameters.Add("TextSupVatNo", TextSupVatNo.Text);
                objPrm[3] = cmdi.Parameters.Add("TextSup_Add_1", TextSup_Add_1.Text);
                objPrm[4] = cmdi.Parameters.Add("TextSup_Add_2", TextSup_Add_2.Text); 
                objPrm[5] = cmdi.Parameters.Add("TextContactNo", TextContactNo.Text); 
                objPrm[6] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[7] = cmdi.Parameters.Add("NoSupplierID", USER_DATA_ID);
                objPrm[8] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[9] = cmdi.Parameters.Add("TextIsActive", ISActive); 
                objPrm[10] = cmdi.Parameters.Add("TextIsPurchaseActive", IsPurchaseActive);
                objPrm[11] = cmdi.Parameters.Add("TextIsSalesActive", IsSalesActive);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                string delete_user = " delete from MS_PARTY_REPRESENTATIVE where PARTY_ID  = '" + Convert.ToInt32(TextSupplierID.Text) + "'"; 
                cmdi = new OracleCommand(delete_user, conn); 
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                foreach (ListItem li in DropDownRepresentativeID.Items)
                { 
                    if (li.Selected == true)
                    {
                        string RepresentativeID = li.Value;
                        string insert_pur_rep = " insert into MS_PARTY_REPRESENTATIVE (PARTY_ID, REPRESENTATIVE_ID) VALUES ( :NoPartyID, :NoRepresentativeID) ";
                        cmdi = new OracleCommand(insert_pur_rep, conn);

                        OracleParameter[] objPr = new OracleParameter[3];
                        objPr[0] = cmdi.Parameters.Add("NoPartyID", Convert.ToInt32(TextSupplierID.Text));
                        objPr[1] = cmdi.Parameters.Add("NoRepresentativeID", RepresentativeID);

                        cmdi.ExecuteNonQuery();
                    }
                }
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                conn.Close();  

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Party Data Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                Display();
                TextSupplierName.Focus();
            }
                else { 
                    Response.Redirect("~/PagePermissionError.aspx");
                }
        }


        protected void DeleteAddressClick(object sender, EventArgs e)
        {
            try
            {
                if (IS_DELETE_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    LinkButton btn = (LinkButton)sender;
                    Session["user_data_id"] = btn.CommandArgument;
                    int USER_DATA_ID = Convert.ToInt32(Session["user_data_id"]);

                    string delete_ex_contract = " delete from MS_PARTY_ADDRESS where PARTY_ADDRESS_ID  = '" + USER_DATA_ID + "'";
                    cmdi = new OracleCommand(delete_ex_contract, conn);
                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Party Address Data Delete Successfully"));
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

        protected void BtnDelete_Click(object sender, EventArgs e)
        { 
          try
           {
            if (IS_DELETE_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                int USER_DATA_ID = Convert.ToInt32(TextSupplierID.Text);
                string delete_user_page = " delete from MS_PARTY where PARTY_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                string delete_user = " delete from MS_PARTY_REPRESENTATIVE where PARTY_ID  = '" + USER_DATA_ID + "'";
                cmdi = new OracleCommand(delete_user, conn);
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();

                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Party Delete Successfully"));
                alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                clearText(); 
                Display();
                TextSupplierName.Focus(); 
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
            TextSupplierID.Text = "";
            TextSupplierName.Text = "";
            DropDownRepresentativeID.Text = "0";
            TextSupVatNo.Text = "";
            TextSup_Add_1.Text = "";
            TextSup_Add_2.Text = "";
            CheckSupplierName.Text = ""; 
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            
        }

        public void clearText()
        {
            TextSupplierID.Text = "";
            TextSupplierName.Text = "";
            TextSupArabicName.Text = "";
            DropDownRepresentativeID.Text = "0";
            TextSupVatNo.Text = "";
            TextSup_Add_1.Text = "";
            TextSup_Add_2.Text = "";
            CheckSupplierName.Text = ""; 
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

        public void TextSupplierName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextSupplierName.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from MS_PARTY where PARTY_NAME = '" + TextSupplierName.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckSupplierName.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Party name is already entry</label>";
                    CheckSupplierName.ForeColor = System.Drawing.Color.Red;
                    TextSupplierName.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckSupplierName.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Party name is available</label>";
                    CheckSupplierName.ForeColor = System.Drawing.Color.Green;
                    CheckIsActive.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else {
                CheckSupplierName.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Party name is not blank</label>";
                    CheckSupplierName.ForeColor = System.Drawing.Color.Red;
                    TextSupplierName.Focus();
            }
            
        } 
   }
}