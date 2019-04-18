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



namespace NRCAPPS.ASSET
{
    public partial class AssetItems : System.Web.UI.Page
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
                        DataTable dtCategoryID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeDepartmentSQL = " SELECT ITEM_CATEGORY_ID, ITEM_CAT_QR_PRI_CODE || ' - ' || ITEM_CATEGORY_NAME AS ITEM_CATEGORY_NAME_CODE FROM IT_ASSET_ITEM_CATEGORIES WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_CATEGORY_ID ASC";
                        ds = ExecuteBySqlStringCatType(makeDepartmentSQL);
                        dtCategoryID = (DataTable)ds.Tables[0];
                        DropDownCategoryID.DataSource = dtCategoryID;
                        DropDownCategoryID.DataValueField = "ITEM_CATEGORY_ID";
                        DropDownCategoryID.DataTextField = "ITEM_CATEGORY_NAME_CODE";
                        DropDownCategoryID.DataBind();
                        DropDownCategoryID.Items.Insert(0, new ListItem("Select  Categorey", "0"));


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

                    string get_item_item_id = "select IT_ASSET_ITEMS_ITEMID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_item_item_id, conn);
                    int newItemID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                    int CategoryID = Convert.ToInt32(DropDownCategoryID.Text);
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string insert_user = "insert into IT_ASSET_ITEMS (ITEM_ID, ITEM_NAME, ITEM_DES, ITEM_TYPE, ITEM_BRAND, ITEM_CATEGORY_ID,  CREATE_DATE, C_USER_ID, IS_ACTIVE) VALUES ( :NoItemID, :TextItemName, :TextItemDes, :TextItemType, :TextItemBrand, :NoCategoryID, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoC_USER_ID, :TextIsActive)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[9];
                    objPrm[0] = cmdi.Parameters.Add("NoItemID", newItemID);  
                    objPrm[1] = cmdi.Parameters.Add("TextItemName", TextItemName.Text);
                    objPrm[2] = cmdi.Parameters.Add("TextItemDes", TextItemDes.Text);
                    objPrm[3] = cmdi.Parameters.Add("TextItemType", TextItemType.Text);
                    objPrm[4] = cmdi.Parameters.Add("TextItemBrand", TextItemBrand.Text);
                    objPrm[5] = cmdi.Parameters.Add("NoCategoryID", CategoryID);
                    objPrm[6] = cmdi.Parameters.Add("u_date", u_date); 
                    objPrm[7] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                    objPrm[8] = cmdi.Parameters.Add("TextIsActive", ISActive);
                     
                    cmdi.ExecuteNonQuery();

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Items successfully"));
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
             string makeSQL = " select *  from IT_ASSET_ITEMS where ITEM_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextItemID.Text         = dt.Rows[i]["ITEM_ID"].ToString();
                 TextItemName.Text       = dt.Rows[i]["ITEM_NAME"].ToString();
                 TextItemDes.Text        = dt.Rows[i]["ITEM_DES"].ToString();
                 TextItemType.Text       = dt.Rows[i]["ITEM_TYPE"].ToString();
                 TextItemBrand.Text      = dt.Rows[i]["ITEM_BRAND"].ToString();
                 DropDownCategoryID.Text = dt.Rows[i]["ITEM_CATEGORY_ID"].ToString();
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
                  
                string makeSQL = "";
            //    if (txtSearchUserRole.Text == "")
             //   {
                    makeSQL = " select  AI.*, AIC.ITEM_CAT_QR_PRI_CODE || ' - ' || AIC.ITEM_CATEGORY_NAME AS ITEM_CATEGORY_NAME_CODE from IT_ASSET_ITEMS AI left join IT_ASSET_ITEM_CATEGORIES AIC ON AIC.ITEM_CATEGORY_ID = AI.ITEM_CATEGORY_ID ORDER BY AI.UPDATE_DATE desc, AI.CREATE_DATE desc";
            //    }
            //    else
            //    {
            //        makeSQL = " select  AI.*, AIC.ITEM_CAT_QR_PRI_CODE || ' - ' || AIC.ITEM_CATEGORY_NAME AS ITEM_CATEGORY_NAME_CODE from IT_ASSET_ITEMS AI left join IT_ASSET_ITEM_CATEGORIES AIC ON AIC.ITEM_CATEGORY_ID = AI.ITEM_CATEGORY_ID where AI.ITEM_NAME like '" + txtSearchUserRole.Text + "%' or AI.ITEM_TYPE like '" + txtSearchUserRole.Text + "%'  ORDER BY AI.UPDATE_DATE desc, AI.CREATE_DATE desc";
             
                    alert_box.Visible = false;
            //    }
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView4D.DataSource = dt;
                GridView4D.DataKeyNames = new string[] { "ITEM_ID" };

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
 
         protected void GridViewUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView4D.PageIndex = e.NewPageIndex;
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

                string update_user = "update  IT_ASSET_ITEMS  set ITEM_NAME = :TextItemName, ITEM_DES = :TextItemDes, ITEM_TYPE = :TextItemType, ITEM_BRAND = :TextItemBrand, ITEM_CATEGORY_ID = :NoCategoryID, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where ITEM_ID = :NoItemID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[9];
                objPrm[0] = cmdi.Parameters.Add("TextItemName", TextItemName.Text);
                objPrm[1] = cmdi.Parameters.Add("TextItemDes", TextItemDes.Text);
                objPrm[2] = cmdi.Parameters.Add("TextItemType", TextItemType.Text);
                objPrm[3] = cmdi.Parameters.Add("TextItemBrand", TextItemBrand.Text);
                objPrm[4] = cmdi.Parameters.Add("NoCategoryID", CategoryID); 
                objPrm[5] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[6] = cmdi.Parameters.Add("NoItemID", USER_DATA_ID);
                objPrm[7] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[8] = cmdi.Parameters.Add("TextIsActive", ISActive);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();  

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Items Update successfully"));
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
                string delete_user_page = " delete from IT_ASSET_ITEMS where ITEM_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Items Delete successfully"));
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
            TextItemDes.Text = "";
            TextItemType.Text = "";
            TextItemBrand.Text = "";
            CheckItemName.Text = "";
            DropDownCategoryID.Text = "0";
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            
        }

        public void clearText()
        {
            TextItemID.Text = "";
            TextItemName.Text = "";
            TextItemDes.Text = "";
            TextItemType.Text = "";
            TextItemBrand.Text = "";
            CheckItemName.Text = "";
            DropDownCategoryID.Text = "0";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 

        }

        public DataSet ExecuteBySqlStringUserType(string sqlString)
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

        public DataSet ExecuteBySqlStringCatType(string sqlString)
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


        public void TextItemName_TextChanged(object sender, EventArgs e)
        {
          try{
              if (!string.IsNullOrEmpty(TextItemName.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from IT_ASSET_ITEMS where ITEM_NAME = '" + TextItemName.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckItemName.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Item name is already exist</label>";
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
          catch
          {
             Response.Redirect("~/ParameterError.aspx");
          }
            
        } 
   }
}