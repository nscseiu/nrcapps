using System; 
using System.Configuration;
using System.Data; 
using System.Web.UI; 
using System.Web.UI.WebControls; 
using System.Data.OracleClient;
using System.IO;  
 
namespace NRCAPPS.NRC
{
    public partial class NrcTradingVessel : System.Web.UI.Page
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
                        Display();
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
            try
             {
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]); 
                    string get_id = "select NRC_TRADING_VESSEL_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_id, conn);
                    int newTradingVesselID = Int32.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string insert_user = "insert into NRC_TRADING_VESSEL (TRADING_VESSEL_ID, TRADING_VESSEL_NAME, TRADING_VESSEL_DETAILS, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES ( :NoTradingVesselID, :TextTradingVessel, :TextTradingVesselDetails, :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[6];
                    objPrm[0] = cmdi.Parameters.Add("NoTradingVesselID", newTradingVesselID);
                    objPrm[1] = cmdi.Parameters.Add("TextTradingVessel", TextTradingVessel.Text);
                    objPrm[2] = cmdi.Parameters.Add("TextTradingVesselDetails", TextTradingVesselDetails.Text); 
                    objPrm[3] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[4] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[5] = cmdi.Parameters.Add("NoCuserID", userID);


                    cmdi.ExecuteNonQuery();

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Payment Terms Successfully"));
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

        protected void linkSelectClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             LinkButton btn = (LinkButton)sender;
             Session["user_page_data_id"] = btn.CommandArgument;
             int USER_DATA_ID = Convert.ToInt32(Session["user_page_data_id"]); 
             

             DataTable dtUserTypeID = new DataTable();
             DataSet ds = new DataSet();
             string makeSQL = " select *  from NRC_TRADING_VESSEL where TRADING_VESSEL_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextTradingVesselID.Text = dt.Rows[i]["TRADING_VESSEL_ID"].ToString();
                 TextTradingVessel.Text = dt.Rows[i]["TRADING_VESSEL_NAME"].ToString();
                 TextTradingVesselDetails.Text = dt.Rows[i]["TRADING_VESSEL_DETAILS"].ToString();   
                 CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                         
             } 
             
             conn.Close();
             Display();
             CheckTradingVessel.Text = "";
             alert_box.Visible = false;
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

                
                 string makeSQL = " select  * from NRC_TRADING_VESSEL ORDER BY UPDATE_DATE desc, CREATE_DATE desc";
                 
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView4D.DataSource = dt;
                GridView4D.DataKeyNames = new string[] { "TRADING_VESSEL_ID" };

                GridView4D.DataBind();
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
  
        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int USER_DATA_ID = Convert.ToInt32(TextTradingVesselID.Text);   
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  NRC_TRADING_VESSEL  set TRADING_VESSEL_NAME = :TextTradingVessel,  TRADING_VESSEL_DETAILS = :TextTradingVesselDetails,  UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where TRADING_VESSEL_ID = :NoTradingVesselID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[6];
                objPrm[0] = cmdi.Parameters.Add("TextTradingVessel", TextTradingVessel.Text);
                objPrm[1] = cmdi.Parameters.Add("TextTradingVesselDetails", TextTradingVesselDetails.Text); 
                objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[3] = cmdi.Parameters.Add("NoTradingVesselID", USER_DATA_ID);
                objPrm[4] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[5] = cmdi.Parameters.Add("TextIsActive", ISActive);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();  

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Payment Terms Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                Display();
            }
                else { 
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

                int USER_DATA_ID = Convert.ToInt32(TextTradingVesselID.Text);
                string delete_user_page = " delete from NRC_TRADING_VESSEL where TRADING_VESSEL_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Payment Terms Delete successfully"));
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

        public void clearTextField(object sender, EventArgs e)
        {
            TextTradingVesselID.Text = "";
            TextTradingVessel.Text = "";
            TextTradingVesselDetails.Text = "";
            CheckTradingVessel.Text = "";  
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            
        }

        public void clearText()
        {
            TextTradingVesselID.Text = "";
            TextTradingVessel.Text = "";
            TextTradingVesselDetails.Text = ""; 
            CheckTradingVessel.Text = ""; 
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 

        }

      

        public void TextTradingVessel_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextTradingVessel.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from NRC_TRADING_VESSEL where TRADING_VESSEL_NAME = '" + TextTradingVessel.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckTradingVessel.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Payment Terms name is already entry</label>";
                    CheckTradingVessel.ForeColor = System.Drawing.Color.Red;
                    TextTradingVessel.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckTradingVessel.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Payment Terms name is available</label>";
                    CheckTradingVessel.ForeColor = System.Drawing.Color.Green;
                    CheckIsActive.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else {
                    CheckTradingVessel.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Payment Terms name is not blank</label>";
                    CheckTradingVessel.ForeColor = System.Drawing.Color.Red;
                    TextTradingVessel.Focus();
            }
            
        }
         

    }
}