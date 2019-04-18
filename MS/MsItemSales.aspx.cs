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
    public partial class MsItemSales : System.Web.UI.Page
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
                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeDropDownItemSQL = " SELECT ITEM_ID, ITEM_CODE || ' : ' || ITEM_NAME AS ITEM_NAME FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' AND IS_PURCHASE_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        dsi = ExecuteBySqlString(makeDropDownItemSQL);
                        dtItemID = (DataTable)dsi.Tables[0];
                        DropDownItemID.DataSource = dtItemID;
                        DropDownItemID.DataValueField = "ITEM_ID";
                        DropDownItemID.DataTextField = "ITEM_NAME";
                        DropDownItemID.DataBind();
                        DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));

                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled");

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
                    int ItemID = Convert.ToInt32(DropDownItemID.Text);
                    string get_id = "select MS_SALES_ITEM_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_id, conn);
                    int newItemID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string insert_user = "insert into MS_SALES_ITEM (ITEM_SALES_ID, ITEM_SALES_DESCRIPTION, ITEM_ID, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES ( :NoItemSalesID, :TextItemSalesDescription, :NoItemID,  :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[6];
                    objPrm[0] = cmdi.Parameters.Add("NoItemSalesID", newItemID); 
                    objPrm[1] = cmdi.Parameters.Add("TextItemSalesDescription", TextItemSalesDescription.Text); 
                    objPrm[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[3] = cmdi.Parameters.Add("TextIsActive", ISActive); 
                    objPrm[4] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[5] = cmdi.Parameters.Add("NoCuserID", userID);


                    cmdi.ExecuteNonQuery(); 
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Sales Item successfully"));
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
               
             string makeSQL = " select *  from MS_SALES_ITEM where ITEM_SALES_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextItemSalesID.Text = dt.Rows[i]["ITEM_SALES_ID"].ToString(); 
                 TextItemSalesDescription.Text = dt.Rows[i]["ITEM_SALES_DESCRIPTION"].ToString(); 
                 DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString(); 
                 CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                         
             } 
             
             conn.Close();
             Display(); 
             alert_box.Visible = false;
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

            BtnUpdate.Attributes.Add("aria-disabled", "true");
            BtnUpdate.Attributes.Add("class", "btn btn-success active");
            BtnDelete.Attributes.Add("aria-disabled", "true");
            BtnDelete.Attributes.Add("class", "btn btn-danger active");

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
                    makeSQL = " select   WSI.*, WI.ITEM_NAME, WI.ITEM_CODE  from MS_SALES_ITEM WSI LEFT JOIN MF_ITEM WI ON WI.ITEM_ID = WSI.ITEM_ID ORDER BY WSI.ITEM_ID";  
                }
                else
                {
                    makeSQL = " select  * from MS_SALES_ITEM where ITEM_ID like '" + txtSearchUserRole.Text + "%' or ITEM_NAME like '" + txtSearchUserRole.Text + "%'  or IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY UPDATE_DATE desc, CREATE_DATE desc";

                    alert_box.Visible = false;
                }
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "ITEM_ID" };

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
 

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int USER_DATA_ID = Convert.ToInt32(TextItemSalesID.Text);   
                int ItemID = Convert.ToInt32(DropDownItemID.Text); 
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  MS_SALES_ITEM  set ITEM_SALES_DESCRIPTION = :TextItemSalesDescription, ITEM_ID =: NoItemID, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where ITEM_SALES_ID = :NoItemSalesID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[6]; 
                objPrm[0] = cmdi.Parameters.Add("TextItemSalesDescription", TextItemSalesDescription.Text); 
                objPrm[1] = cmdi.Parameters.Add("NoItemID", ItemID);
                objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[3] = cmdi.Parameters.Add("NoItemSalesID", USER_DATA_ID);
                objPrm[4] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[5] = cmdi.Parameters.Add("TextIsActive", ISActive);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();  

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Sales Item Update successfully"));
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

                int USER_DATA_ID = Convert.ToInt32(TextItemSalesID.Text);
                string delete_user_page = " delete from MS_SALES_ITEM where ITEM_SALES_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Sales Item Delete successfully"));
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
            TextItemSalesID.Text = ""; 
            TextItemSalesDescription.Text = ""; 
            CheckItemSalesName.Text = "";  
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            
        }

        public void clearText()
        {
            TextItemSalesID.Text = ""; 
            TextItemSalesDescription.Text = ""; 
            CheckItemSalesName.Text = ""; 
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 

        }

      

        public void TextItemSalesName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextItemSalesDescription.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from MS_SALES_ITEM where ITEM_SALES_DESCRIPTION = '" + TextItemSalesDescription.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckItemSalesName.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Sales Item description is already entry</label>";
                    CheckItemSalesName.ForeColor = System.Drawing.Color.Red;
                    TextItemSalesDescription.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckItemSalesName.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Sales Item description is available</label>";
                    CheckItemSalesName.ForeColor = System.Drawing.Color.Green;
                    CheckIsActive.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else {
                    CheckItemSalesName.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Sales Item description is not blank</label>";
                    CheckItemSalesName.ForeColor = System.Drawing.Color.Red;
                    TextItemSalesDescription.Focus();
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