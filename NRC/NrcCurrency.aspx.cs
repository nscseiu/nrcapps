using System; 
using System.Configuration;
using System.Data; 
using System.Web.UI; 
using System.Web.UI.WebControls; 
using System.Data.OracleClient;
using System.IO;
using System.Data.SqlClient;


namespace NRCAPPS.NRC
{
    public partial class NrcCurrency : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt;
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
                        DataTable dtCurrencyRateID = new DataTable();
                        DataSet dse = new DataSet();
                        string makeDropSQL = " SELECT CURRENCY_ID, CURRENCY_SYMBOL || ' - ' || CURRENCY_NAME AS CURRENCY_NAME FROM NRC_CURRENCY WHERE IS_ACTIVE = 'Enable' ORDER BY CURRENCY_ID ASC";
                        dse = ExecuteBySqlString(makeDropSQL);
                        dtCurrencyRateID = (DataTable)dse.Tables[0];
                        DropDownSourceCurrencyID.DataSource = dtCurrencyRateID;
                        DropDownSourceCurrencyID.DataValueField = "CURRENCY_ID";
                        DropDownSourceCurrencyID.DataTextField = "CURRENCY_NAME";
                        DropDownSourceCurrencyID.DataBind();
                        DropDownSourceCurrencyID.Items.Insert(0, new ListItem("Select Source Currency ", "0"));

                        DropDownTargetCurrencyID.DataSource = dtCurrencyRateID;
                        DropDownTargetCurrencyID.DataValueField = "CURRENCY_ID";
                        DropDownTargetCurrencyID.DataTextField = "CURRENCY_NAME";
                        DropDownTargetCurrencyID.DataBind();
                        DropDownTargetCurrencyID.Items.Insert(0, new ListItem("Select Target Currency", "0"));

                        Display();
                        DisplayRate();
                        alert_box.Visible = false;
                        alert_box_right.Visible = false;
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
            try
             {
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]); 
                    string get_customer_id = "select NRC_CURRENCY_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_customer_id, conn);
                    int newCurrencyID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string insert_user = "insert into NRC_CURRENCY (CURRENCY_ID, CURRENCY_NAME, CURRENCY_SYMBOL, CURRENCY_IMAGE, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES ( :NoCurrencyID, :TextCurrency, :TextCurrencySymbol, :TextCurrencyImage, :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[7];
                    objPrm[0] = cmdi.Parameters.Add("NoCurrencyID", newCurrencyID);
                    objPrm[1] = cmdi.Parameters.Add("TextCurrency", TextCurrency.Text);
                    objPrm[2] = cmdi.Parameters.Add("TextCurrencySymbol", TextCurrencySymbol.Text);
                    objPrm[3] = cmdi.Parameters.Add("TextCurrencyImage", TextCurrencyImage.Text); 
                    objPrm[4] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[5] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[6] = cmdi.Parameters.Add("NoCuserID", userID);
                     
                    cmdi.ExecuteNonQuery(); 
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Currency Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                    clearText();
                    Display();
                }
                else { 
                    Response.Redirect("~/PagePermissionError.aspx");
                }
             }
             catch
             {
               Response.Redirect("~/ParameterError.aspx");
            }
        }

        public void BtnAdd_CRateClick(object sender, EventArgs e)
        {
            try
             {
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    int SourceCurrencyID = Convert.ToInt32(DropDownSourceCurrencyID.Text);
                    int TargetCurrencyID = Convert.ToInt32(DropDownTargetCurrencyID.Text);
                    double ExchangeRate = Convert.ToDouble(TextExchangeRate.Text.Trim());
                    string get_customer_id = "select NRC_CURRENCY_RATE_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_customer_id, conn);
                    int newCurrencyRateID = Int32.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string insert_user = "insert into NRC_CURRENCY_RATE (CURRENCY_RATE_ID, SOURCE_CURRENCY_ID, TARGET_CURRENCY_ID, EXCHANGE_RATE, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES ( :NoCurrencyRateID, :NoSourceCurrencyID, :NoTargetCurrencyID, :NoExchangeRate, :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[7];
                    objPrm[0] = cmdi.Parameters.Add("NoCurrencyRateID", newCurrencyRateID);
                    objPrm[1] = cmdi.Parameters.Add("NoSourceCurrencyID", SourceCurrencyID);
                    objPrm[2] = cmdi.Parameters.Add("NoTargetCurrencyID", TargetCurrencyID);
                    objPrm[3] = cmdi.Parameters.Add("NoExchangeRate", ExchangeRate); 
                    objPrm[4] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[5] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[6] = cmdi.Parameters.Add("NoCuserID", userID);
                     
                    cmdi.ExecuteNonQuery(); 
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box_right.Visible = true;
                    alert_box_right.Controls.Add(new LiteralControl("Insert New Currency Rate Successfully"));
                    alert_box_right.Attributes.Add("class", "alert alert-success alert-dismissible");
                    clearText();
                    DisplayRate();
                }
                else { 
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
             Session["user_page_data_id"] = btn.CommandArgument;
             int USER_DATA_ID = Convert.ToInt32(Session["user_page_data_id"]); 
             

             DataTable dtUserTypeID = new DataTable();
             DataSet ds = new DataSet();
             string makeSQL = " select *  from NRC_CURRENCY where CURRENCY_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextCurrencyID.Text = dt.Rows[i]["CURRENCY_ID"].ToString();
                 TextCurrency.Text = dt.Rows[i]["CURRENCY_NAME"].ToString();
                 TextCurrencySymbol.Text = dt.Rows[i]["CURRENCY_SYMBOL"].ToString();
                 TextCurrencyImage.Text = dt.Rows[i]["CURRENCY_IMAGE"].ToString(); 
                 CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                         
             } 
             
             conn.Close();
             Display();
             CheckCurrency.Text = "";
             alert_box.Visible = false;
             BtnAdd.Attributes.Add("aria-disabled", "false");
             BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 

        }

        protected void linkSelectRateClick(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_page_data_id"] = btn.CommandArgument;
            int USER_DATA_ID = Convert.ToInt32(Session["user_page_data_id"]);


            DataTable dtUserTypeID = new DataTable();
            DataSet ds = new DataSet();
            string makeSQL = " select *  from NRC_CURRENCY_RATE where CURRENCY_RATE_ID = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;

            for (int i = 0; i < RowCount; i++)
            {
                TextCurrencyRateID.Text = dt.Rows[i]["CURRENCY_RATE_ID"].ToString();
                DropDownSourceCurrencyID.Text = dt.Rows[i]["SOURCE_CURRENCY_ID"].ToString();
                DropDownTargetCurrencyID.Text = dt.Rows[i]["TARGET_CURRENCY_ID"].ToString();
                TextExchangeRate.Text = dt.Rows[i]["EXCHANGE_RATE"].ToString();
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);

            }

            conn.Close();
            DisplayRate(); 
            CheckCurrency.Text = "";
            alert_box_right.Visible = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

        }

        public void Display()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                DataTable dtUserTypeID = new DataTable();
                DataSet ds = new DataSet();

                string makeSQL = "";
                if (txtSearchUserRole.Text == "")
                {
                    makeSQL = " select  * from NRC_CURRENCY ORDER BY UPDATE_DATE desc, CREATE_DATE desc";
                }
                else
                {
                    makeSQL = " select  * from NRC_CURRENCY where CURRENCY_ID like '" + txtSearchUserRole.Text + "%' or CURRENCY_NAME like '" + txtSearchUserRole.Text + "%'  or IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY UPDATE_DATE desc, CREATE_DATE desc";

                    alert_box.Visible = false;
                }
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "CURRENCY_ID" };

                GridView1.DataBind();
                conn.Close();
                //alert_box.Visible = false;
            }
            else
            {
             //   Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        protected void GridViewSearchUser(object sender, EventArgs e)
        {
            this.Display();
        }
 
         protected void GridViewUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            Display();
            alert_box.Visible = false;
        }


        public void DisplayRate()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();  
                string makeSQL = "";
                if (txtSearchUserRole.Text == "")
                {
                    makeSQL = " SELECT NCR.CURRENCY_RATE_ID, NC.CURRENCY_NAME AS SOURCE_CURRENCY_NAME,  NCU.CURRENCY_NAME AS TARGET_CURRENCY_NAME, NCR.EXCHANGE_RATE, NCR.IS_ACTIVE, NCR.CREATE_DATE, NCR.UPDATE_DATE FROM NRC_CURRENCY_RATE NCR LEFT JOIN NRC_CURRENCY NC ON NC.CURRENCY_ID = NCR.SOURCE_CURRENCY_ID LEFT JOIN NRC_CURRENCY NCU ON NCU.CURRENCY_ID = NCR.TARGET_CURRENCY_ID ORDER BY NCR.UPDATE_DATE desc, NCR.CREATE_DATE desc";
                }
                else
                {
                    makeSQL = " SELECT NCR.CURRENCY_RATE_ID, NC.CURRENCY_NAME AS SOURCE_CURRENCY_NAME,  NCU.CURRENCY_NAME AS TARGET_CURRENCY_NAME, NCR.EXCHANGE_RATE, NCR.IS_ACTIVE, NCR.CREATE_DATE, NCR.UPDATE_DATE FROM NRC_CURRENCY_RATE NCR LEFT JOIN NRC_CURRENCY NC ON NC.CURRENCY_ID = NCR.SOURCE_CURRENCY_ID LEFT JOIN NRC_CURRENCY NCU ON NCU.CURRENCY_ID = NCR.TARGET_CURRENCY_ID  where NCR.CURRENCY_RATE_ID like '" + txtSearchUserRole.Text + "%' or CURRENCY_NAME like '" + txtSearchUserRole.Text + "%'  or IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY UPDATE_DATE desc, CREATE_DATE desc";

                    alert_box.Visible = false;
                    alert_box_right.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataKeyNames = new string[] { "CURRENCY_RATE_ID" };

                GridView2.DataBind();
                conn.Close();
                //alert_box.Visible = false;
            }
            else
            {
                //   Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        protected void GridViewSearchRate(object sender, EventArgs e)
        {
            this.DisplayRate();
        }

        protected void GridViewRate_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            DisplayRate();
            alert_box_right.Visible = false;
        }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int USER_DATA_ID = Convert.ToInt32(TextCurrencyID.Text);   
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  NRC_CURRENCY  set CURRENCY_NAME = :TextCurrency,  CURRENCY_SYMBOL = :TextCurrencySymbol, CURRENCY_IMAGE = :TextCurrencyImage, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where CURRENCY_ID = :NoCurrencyID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[7];
                objPrm[0] = cmdi.Parameters.Add("TextCurrency", TextCurrency.Text);
                objPrm[1] = cmdi.Parameters.Add("TextCurrencySymbol", TextCurrencySymbol.Text);
                objPrm[2] = cmdi.Parameters.Add("TextCurrencyImage", TextCurrencyImage.Text); 
                objPrm[3] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[4] = cmdi.Parameters.Add("NoCurrencyID", USER_DATA_ID);
                objPrm[5] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[6] = cmdi.Parameters.Add("TextIsActive", ISActive);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();  

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Currency Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                Display();
            }
            else { 
               Response.Redirect("~/PagePermissionError.aspx");
         }
        }
         

        protected void BtnUpdate_CRateClick(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int USER_DATA_ID = Convert.ToInt32(TextCurrencyRateID.Text);
                int SourceCurrencyID = Convert.ToInt32(DropDownSourceCurrencyID.Text);
                int TargetCurrencyID = Convert.ToInt32(DropDownTargetCurrencyID.Text);
                double ExchangeRate = Convert.ToDouble(TextExchangeRate.Text.Trim());
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  NRC_CURRENCY_RATE  set SOURCE_CURRENCY_ID = :NoSourceCurrencyID,  TARGET_CURRENCY_ID = :NoTargetCurrencyID, EXCHANGE_RATE = :NoExchangeRate, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where CURRENCY_RATE_ID = :NoCurrencyRateID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[7];
                objPrm[0] = cmdi.Parameters.Add("NoSourceCurrencyID", SourceCurrencyID);
                objPrm[1] = cmdi.Parameters.Add("NoTargetCurrencyID", TargetCurrencyID);
                objPrm[2] = cmdi.Parameters.Add("NoExchangeRate", ExchangeRate);
                objPrm[3] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[4] = cmdi.Parameters.Add("NoCurrencyRateID", USER_DATA_ID);
                objPrm[5] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[6] = cmdi.Parameters.Add("TextIsActive", ISActive);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();

                alert_box_right.Visible = true;
                alert_box_right.Controls.Add(new LiteralControl("Currency Rate Update Successfully"));
                alert_box_right.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearText();
                DisplayRate();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
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

                int USER_DATA_ID = Convert.ToInt32(TextCurrencyID.Text);
                string delete_user_page = " delete from NRC_CURRENCY where CURRENCY_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Currency Delete Successfully"));
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

        protected void BtnDelete_CRateClick(object sender, EventArgs e)
        { 
          try
           {
            if (IS_DELETE_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                int USER_DATA_ID = Convert.ToInt32(TextCurrencyID.Text);
                string delete_user_page = " delete from NRC_CURRENCY_RATE where CURRENCY_RATE_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                alert_box_right.Visible = true;
                alert_box_right.Controls.Add(new LiteralControl("Currency Rate Delete Successfully"));
                alert_box_right.Attributes.Add("class", "alert alert-danger alert-dismissible");
                clearText(); 
                DisplayRate();

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
            TextCurrencyID.Text = "";
            TextCurrency.Text = "";
            TextCurrencySymbol.Text = "";
            TextCurrencyImage.Text = ""; 
            CheckCurrency.Text = "";  
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            
        }

        public void clearText()
        {
            TextCurrencyID.Text = "";
            TextCurrency.Text = "";
            TextCurrencySymbol.Text = "";
            TextCurrencyImage.Text = ""; 
            CheckCurrency.Text = ""; 
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 

        }
         
        public void TextCurrency_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextCurrency.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from NRC_CURRENCY where CURRENCY_NAME = '" + TextCurrency.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckCurrency.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Currency name is already entry</label>";
                    CheckCurrency.ForeColor = System.Drawing.Color.Red;
                    TextCurrency.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckCurrency.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Currency name is available</label>";
                    CheckCurrency.ForeColor = System.Drawing.Color.Green;
                    CheckIsActive.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else {
                    CheckCurrency.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Currency name is not blank</label>";
                    CheckCurrency.ForeColor = System.Drawing.Color.Red;
                    TextCurrency.Focus();
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