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
    public partial class NrcShipmentLocation : System.Web.UI.Page
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
                        DataTable dtShipmentFTID = new DataTable();
                        DataSet dse = new DataSet();
                        string makeDropSQL = " SELECT SHIPMENT_LOC_ID, SHIPMENT_LOC_NAME FROM NRC_SHIPMENT_LOCATION WHERE IS_ACTIVE = 'Enable' ORDER BY SHIPMENT_LOC_ID ASC";
                        dse = ExecuteBySqlString(makeDropSQL);
                        dtShipmentFTID = (DataTable)dse.Tables[0];
                        DropDownShipmentFromID.DataSource = dtShipmentFTID;
                        DropDownShipmentFromID.DataValueField = "SHIPMENT_LOC_ID";
                        DropDownShipmentFromID.DataTextField = "SHIPMENT_LOC_NAME";
                        DropDownShipmentFromID.DataBind();
                        DropDownShipmentFromID.Items.Insert(0, new ListItem("Select Shipment From ", "0"));

                        DropDownShipmentToID.DataSource = dtShipmentFTID;
                        DropDownShipmentToID.DataValueField = "SHIPMENT_LOC_ID";
                        DropDownShipmentToID.DataTextField = "SHIPMENT_LOC_NAME";
                        DropDownShipmentToID.DataBind();
                        DropDownShipmentToID.Items.Insert(0, new ListItem("Select Shipment To ", "0"));
                         
                        Display();
                        DisplayFromTo();
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
                    string get_id = "select NRC_SHIPMENT_LOC_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_id, conn);
                    int newShipmentLocID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string insert_user = "insert into NRC_SHIPMENT_LOCATION (SHIPMENT_LOC_ID, SHIPMENT_LOC_NAME, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES ( :NoShipmentLocID, :TextShipmentLocName, :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[5];
                    objPrm[0] = cmdi.Parameters.Add("NoShipmentLocID", newShipmentLocID);
                    objPrm[1] = cmdi.Parameters.Add("TextShipmentLocName", TextShipmentLocName.Text); 
                    objPrm[2] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[3] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[4] = cmdi.Parameters.Add("NoCuserID", userID);
                     
                    cmdi.ExecuteNonQuery(); 
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Shipment Location Successfully"));
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

        public void BtnAdd_CFromToClick(object sender, EventArgs e)
        {
            try
             {
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    int ShipmentFromID = Convert.ToInt32(DropDownShipmentFromID.Text);
                    int ShipmentToID = Convert.ToInt32(DropDownShipmentToID.Text); 
                    string get_id = "select NRC_SHIPMENT_FROM_TO_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_id, conn);
                    int newShipmentFTID = Int32.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string insert_user = "insert into NRC_SHIPMENT_FROM_TO (SHIPMENT_FT_ID, SHIPMENT_FROM_ID, SHIPMENT_TO_ID, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES ( :NoShipmentFTID, :NoShipmentFromID, :NoShipmentToID, :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[6];
                    objPrm[0] = cmdi.Parameters.Add("NoShipmentFTID", newShipmentFTID);
                    objPrm[1] = cmdi.Parameters.Add("NoShipmentFromID", ShipmentFromID);
                    objPrm[2] = cmdi.Parameters.Add("NoShipmentToID", ShipmentToID); 
                    objPrm[3] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[4] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[5] = cmdi.Parameters.Add("NoCuserID", userID);
                     
                    cmdi.ExecuteNonQuery(); 
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box_right.Visible = true;
                    alert_box_right.Controls.Add(new LiteralControl("Insert New Shipment From To Name Successfully"));
                    alert_box_right.Attributes.Add("class", "alert alert-success alert-dismissible");
                    clearText();
                    DisplayFromTo();
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
             string makeSQL = " select *  from NRC_SHIPMENT_LOCATION where SHIPMENT_LOC_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextShipmentLocID.Text = dt.Rows[i]["SHIPMENT_LOC_ID"].ToString();
                 TextShipmentLocName.Text = dt.Rows[i]["SHIPMENT_LOC_NAME"].ToString(); 
                 CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false); 
             } 
             
             conn.Close();
             Display();
             CheckShipment.Text = "";
             alert_box.Visible = false;
             BtnAdd.Attributes.Add("aria-disabled", "false");
             BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 

        }

        protected void linkSelectFromToClick(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_page_data_id"] = btn.CommandArgument;
            int USER_DATA_ID = Convert.ToInt32(Session["user_page_data_id"]);


            DataTable dtUserTypeID = new DataTable();
            DataSet ds = new DataSet();
            string makeSQL = " select *  from NRC_SHIPMENT_FROM_TO where SHIPMENT_FT_ID = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;

            for (int i = 0; i < RowCount; i++)
            {
                TextShipmentFTID.Text = dt.Rows[i]["SHIPMENT_FT_ID"].ToString();
                DropDownShipmentFromID.Text = dt.Rows[i]["SHIPMENT_FROM_ID"].ToString();
                DropDownShipmentToID.Text = dt.Rows[i]["SHIPMENT_TO_ID"].ToString(); 
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);

            }

            conn.Close();
            DisplayFromTo(); 
            CheckShipment.Text = "";
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
                  
                string makeSQL = "";
                if (txtSearchUserRole.Text == "")
                {
                    makeSQL = " select  * from NRC_SHIPMENT_LOCATION ORDER BY UPDATE_DATE desc, CREATE_DATE desc";
                }
                else
                {
                    makeSQL = " select  * from NRC_SHIPMENT_LOCATION where SHIPMENT_LOC_ID like '" + txtSearchUserRole.Text + "%' or SHIPMENT_LOC_NAME like '" + txtSearchUserRole.Text + "%'  or IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY UPDATE_DATE desc, CREATE_DATE desc";

                    alert_box.Visible = false;
                }
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "SHIPMENT_LOC_ID" };

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


        public void DisplayFromTo()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();  
                string makeSQL = "";
                if (txtSearchUserRole.Text == "")
                {
                    makeSQL = " SELECT NSFT.SHIPMENT_FT_ID, NSLF.SHIPMENT_LOC_NAME AS SHIPMENT_FROM_NAME, NSLT.SHIPMENT_LOC_NAME AS SHIPMENT_TO_NAME, NSFT.IS_ACTIVE, NSFT.CREATE_DATE, NSFT.UPDATE_DATE  FROM NRC_SHIPMENT_FROM_TO NSFT LEFT JOIN NRC_SHIPMENT_LOCATION NSLF ON NSLF.SHIPMENT_LOC_ID = NSFT.SHIPMENT_FROM_ID LEFT JOIN NRC_SHIPMENT_LOCATION NSLT ON NSLT.SHIPMENT_LOC_ID = NSFT.SHIPMENT_TO_ID ORDER BY NSFT.UPDATE_DATE desc, NSFT.CREATE_DATE desc";
                }
                else
                {
                    makeSQL = " SELECT NCR.SHIPMENT_FT_ID, NC.SHIPMENT_LOC_NAME AS SOURCE_SHIPMENT_LOC_NAME,  NCU.SHIPMENT_LOC_NAME AS TARGET_SHIPMENT_LOC_NAME, NCR.EXCHANGE_FromTo, NCR.IS_ACTIVE, NCR.CREATE_DATE, NCR.UPDATE_DATE FROM NRC_SHIPMENT_FROM_TO NCR LEFT JOIN NRC_SHIPMENT_LOCATION NC ON NC.SHIPMENT_LOC_ID = NCR.SHIPMENT_FROM_ID LEFT JOIN NRC_SHIPMENT_LOCATION NCU ON NCU.SHIPMENT_LOC_ID = NCR.SHIPMENT_TO_ID  where NCR.SHIPMENT_FT_ID like '" + txtSearchUserRole.Text + "%' or SHIPMENT_LOC_NAME like '" + txtSearchUserRole.Text + "%'  or IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY UPDATE_DATE desc, CREATE_DATE desc";

                    alert_box.Visible = false;
                    alert_box_right.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataKeyNames = new string[] { "SHIPMENT_FT_ID" };

                GridView2.DataBind();
                conn.Close();
                //alert_box.Visible = false;
            }
            else
            {
                //   Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        protected void GridViewSearchFromTo(object sender, EventArgs e)
        {
            this.DisplayFromTo();
        }

        protected void GridViewFromTo_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView2.PageIndex = e.NewPageIndex;
            DisplayFromTo();
            alert_box_right.Visible = false;
        }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int USER_DATA_ID = Convert.ToInt32(TextShipmentLocID.Text);   
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  NRC_SHIPMENT_LOCATION  set SHIPMENT_LOC_NAME = :TextShipmentLocName, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where SHIPMENT_LOC_ID = :NoShipmentLocID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[7];
                objPrm[0] = cmdi.Parameters.Add("TextShipmentLocName", TextShipmentLocName.Text); 
                objPrm[3] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[4] = cmdi.Parameters.Add("NoShipmentLocID", USER_DATA_ID);
                objPrm[5] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[6] = cmdi.Parameters.Add("TextIsActive", ISActive);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();  

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Shipment Location Data Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                Display();
            }
            else { 
               Response.Redirect("~/PagePermissionError.aspx");
         }
        }
         

        protected void BtnUpdate_CFromToClick(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int USER_DATA_ID = Convert.ToInt32(TextShipmentFTID.Text);
                int ShipmentFromID = Convert.ToInt32(DropDownShipmentFromID.Text);
                int ShipmentToID = Convert.ToInt32(DropDownShipmentToID.Text); 
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  NRC_SHIPMENT_FROM_TO  set SHIPMENT_FROM_ID = :NoShipmentFromID,  SHIPMENT_TO_ID = :NoShipmentToID,  UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where SHIPMENT_FT_ID = :NoShipmentFTID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[7];
                objPrm[0] = cmdi.Parameters.Add("NoShipmentFromID", ShipmentFromID);
                objPrm[1] = cmdi.Parameters.Add("NoShipmentToID", ShipmentToID); 
                objPrm[3] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[4] = cmdi.Parameters.Add("NoShipmentFTID", USER_DATA_ID);
                objPrm[5] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[6] = cmdi.Parameters.Add("TextIsActive", ISActive);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();

                alert_box_right.Visible = true;
                alert_box_right.Controls.Add(new LiteralControl("Shipment From To Data Update Successfully"));
                alert_box_right.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearText();
                DisplayFromTo();
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

                int USER_DATA_ID = Convert.ToInt32(TextShipmentLocID.Text);
                string delete_user_page = " delete from NRC_SHIPMENT_LOCATION where SHIPMENT_LOC_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Shipment Location Delete Successfully"));
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

        protected void BtnDelete_CFromToClick(object sender, EventArgs e)
        { 
          try
           {
            if (IS_DELETE_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                int USER_DATA_ID = Convert.ToInt32(TextShipmentFTID.Text);
                string delete_user_page = " delete from NRC_SHIPMENT_FROM_TO where SHIPMENT_FT_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                alert_box_right.Visible = true;
                alert_box_right.Controls.Add(new LiteralControl("Shipment From To Data Delete Successfully"));
                alert_box_right.Attributes.Add("class", "alert alert-danger alert-dismissible");
                clearText(); 
                DisplayFromTo();

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
            TextShipmentLocID.Text = "";
            TextShipmentLocName.Text = ""; 
            CheckShipment.Text = "";  
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            
        }

        public void clearText()
        {
            TextShipmentLocID.Text = "";
            TextShipmentLocName.Text = ""; 
            CheckShipment.Text = ""; 
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 

        }
         
        public void TextShipmentLocName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextShipmentLocName.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from NRC_SHIPMENT_LOCATION where SHIPMENT_LOC_NAME = '" + TextShipmentLocName.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckShipment.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Shipment name is already entry</label>";
                    CheckShipment.ForeColor = System.Drawing.Color.Red;
                    TextShipmentLocName.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckShipment.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Shipment name is available</label>";
                    CheckShipment.ForeColor = System.Drawing.Color.Green;
                    CheckIsActive.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else {
                    CheckShipment.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Shipment name is not blank</label>";
                    CheckShipment.ForeColor = System.Drawing.Color.Red;
                    TextShipmentLocName.Focus();
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