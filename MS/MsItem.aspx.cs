﻿using System;
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
    public partial class MsItem : System.Web.UI.Page
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
                        DataTable dtCatID = new DataTable();
                        DataSet dc = new DataSet();
                        string makeDropDownCatSQL = " SELECT * FROM WP_CATEGORY WHERE IS_ACTIVE = 'Enable' ORDER BY CATEGORY_ID ASC";
                        dc = ExecuteBySqlString(makeDropDownCatSQL);
                        dtCatID = (DataTable)dc.Tables[0];
                        DropDownCategoryID.DataSource = dtCatID;
                        DropDownCategoryID.DataValueField = "CATEGORY_ID";
                        DropDownCategoryID.DataTextField = "CATEGORY_NAME";
                        DropDownCategoryID.DataBind();
                        DropDownCategoryID.Items.Insert(0, new ListItem("Select  Category", "0"));

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
                    int ItemCode = Convert.ToInt32(TextItemCode.Text);
                    int CategoryID = Convert.ToInt32(DropDownCategoryID.Text);
                    string get_customer_id = "select WP_ITEM_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_customer_id, conn);
                    int newItemID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string insert_user = "insert into WP_ITEM (ITEM_ID, ITEM_NAME, ITEM_ARABIC_NAME, ITEM_DESCRIPTION, ITEM_CODE, ITEM_RATE, CATEGORY_ID, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES ( :NoItemID, :TextItemName, :TextItemArabicName, :TextItemDescription, :TextItemCode, :NoItemRate, :NoCategoryID,  :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[10];
                    objPrm[0] = cmdi.Parameters.Add("NoItemID", newItemID);
                    objPrm[1] = cmdi.Parameters.Add("TextItemName", TextItemName.Text);
                    objPrm[2] = cmdi.Parameters.Add("TextItemArabicName", TextItemArabicName.Text);
                    objPrm[3] = cmdi.Parameters.Add("TextItemDescription", TextItemDescription.Text);
                    objPrm[4] = cmdi.Parameters.Add("TextItemCode", ItemCode);
                    objPrm[5] = cmdi.Parameters.Add("NoItemRate", TextItemRate.Text);
                    objPrm[6] = cmdi.Parameters.Add("NoCategoryID", CategoryID);
                    objPrm[7] = cmdi.Parameters.Add("TextIsActive", ISActive); 
                    objPrm[8] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[9] = cmdi.Parameters.Add("NoCuserID", userID);


                    cmdi.ExecuteNonQuery();

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Item successfully"));
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
             string makeSQL = " select *  from WP_ITEM where ITEM_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();
                 TextItemName.Text = dt.Rows[i]["ITEM_NAME"].ToString();
                 TextItemArabicName.Text = dt.Rows[i]["ITEM_ARABIC_NAME"].ToString();
                 TextItemDescription.Text = dt.Rows[i]["ITEM_DESCRIPTION"].ToString();
                 TextItemCode.Text = dt.Rows[i]["ITEM_CODE"].ToString();
                 TextItemRate.Text = dt.Rows[i]["ITEM_RATE"].ToString();
                 DropDownCategoryID.Text = dt.Rows[i]["CATEGORY_ID"].ToString(); 
                 CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                         
             } 
             
             conn.Close();
             Display();
             CheckItemName.Text = "";
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

                string makeSQL = "";
                if (txtSearchUserRole.Text == "")
                {
                    makeSQL = " select   WI.*, WC.CATEGORY_NAME  from WP_ITEM WI LEFT JOIN WP_CATEGORY WC ON WC.CATEGORY_ID = WI.CATEGORY_ID ORDER BY WI.ITEM_ID"; // WI.UPDATE_DATE desc, WI.CREATE_DATE desc
                }
                else
                {
                    makeSQL = " select  * from WP_ITEM where ITEM_ID like '" + txtSearchUserRole.Text + "%' or ITEM_NAME like '" + txtSearchUserRole.Text + "%'  or IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY UPDATE_DATE desc, CREATE_DATE desc";

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
                int USER_DATA_ID = Convert.ToInt32(TextItemID.Text);   
                int CategoryID = Convert.ToInt32(DropDownCategoryID.Text); 
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  WP_ITEM  set ITEM_NAME = :TextItemName,  ITEM_ARABIC_NAME = :TextItemArabicName, ITEM_DESCRIPTION = :TextItemDescription, ITEM_CODE = :TextItemCode, ITEM_RATE = :NoItemRate, CATEGORY_ID =: NoCategoryID, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where ITEM_ID = :NoItemID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[10];
                objPrm[0] = cmdi.Parameters.Add("TextItemName", TextItemName.Text);
                objPrm[1] = cmdi.Parameters.Add("TextItemArabicName", TextItemArabicName.Text);
                objPrm[2] = cmdi.Parameters.Add("TextItemDescription", TextItemDescription.Text);
                objPrm[3] = cmdi.Parameters.Add("TextItemCode", TextItemCode.Text);
                objPrm[4] = cmdi.Parameters.Add("NoItemRate", TextItemRate.Text);
                objPrm[5] = cmdi.Parameters.Add("NoCategoryID", CategoryID);
                objPrm[6] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[7] = cmdi.Parameters.Add("NoItemID", USER_DATA_ID);
                objPrm[8] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[9] = cmdi.Parameters.Add("TextIsActive", ISActive);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();  

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Item Update successfully"));
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

                int USER_DATA_ID = Convert.ToInt32(TextItemID.Text);
                string delete_user_page = " delete from WP_ITEM where ITEM_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Item Delete successfully"));
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
            TextItemID.Text = "";
            TextItemName.Text = "";
            TextItemArabicName.Text = "";
            TextItemDescription.Text = "";
            TextItemCode.Text = ""; 
            CheckItemName.Text = ""; 
            CheckItemCode.Text = ""; 
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            
        }

        public void clearText()
        {
            TextItemID.Text = "";
            TextItemName.Text = "";
            TextItemArabicName.Text = "";
            TextItemDescription.Text = "";
            TextItemCode.Text = ""; 
            CheckItemName.Text = "";
            CheckItemCode.Text = ""; 
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 

        }

      

        public void TextItemName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextItemName.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from WP_ITEM where ITEM_NAME = '" + TextItemName.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckItemName.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Item name is already entry</label>";
                    CheckItemName.ForeColor = System.Drawing.Color.Red;
                    TextItemName.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckItemName.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Item name is available</label>";
                    CheckItemName.ForeColor = System.Drawing.Color.Green;
                    CheckIsActive.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else {
                    CheckItemName.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Item name is not blank</label>";
                    CheckItemName.ForeColor = System.Drawing.Color.Red;
                    TextItemName.Focus();
            }
            
        }


        public void TextItemCode_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextItemCode.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from WP_ITEM where ITEM_CODE = '" + TextItemCode.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckItemCode.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Item Code is already entry</label>";
                    CheckItemCode.ForeColor = System.Drawing.Color.Red;
                    TextItemCode.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckItemCode.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Item Code is available</label>";
                    CheckItemCode.ForeColor = System.Drawing.Color.Green;
                    CheckIsActive.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else {
                CheckItemCode.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Item Code is not blank</label>";
                    CheckItemCode.ForeColor = System.Drawing.Color.Red;
                    TextItemCode.Focus();
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