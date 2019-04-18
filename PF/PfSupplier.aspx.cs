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



namespace NRCAPPS.PF
{
    public partial class PfSupplier : System.Web.UI.Page
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
                        TextSupplierName.Focus();
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

                    string get_supplier_id = "select PF_PARTY_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_supplier_id, conn);
                    int newSupplierID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string insert_user = "insert into PF_PARTY (PARTY_ID, PARTY_NAME, PARTY_ARABIC_NAME, PARTY_VAT_NO, PARTY_ADD_1, PARTY_ADD_2, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES ( :NoSupplierID, :TextSupplierName, :TextSupArabicName, :TextSupVatNo, :TextSup_Add_1, :TextSup_Add_2, :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[9];
                    objPrm[0] = cmdi.Parameters.Add("NoSupplierID", newSupplierID);
                    objPrm[1] = cmdi.Parameters.Add("TextSupplierName", TextSupplierName.Text);
                    objPrm[2] = cmdi.Parameters.Add("TextSupArabicName", TextSupArabicName.Text);
                    objPrm[3] = cmdi.Parameters.Add("TextSupVatNo", TextSupVatNo.Text);
                    objPrm[4] = cmdi.Parameters.Add("TextSup_Add_1", TextSup_Add_1.Text);
                    objPrm[5] = cmdi.Parameters.Add("TextSup_Add_2", TextSup_Add_2.Text);
                    objPrm[6] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[7] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[8] = cmdi.Parameters.Add("NoCuserID", userID);


                    cmdi.ExecuteNonQuery();

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
              
             string makeSQL = " select *  from PF_PARTY where PARTY_ID = '" + USER_DATA_ID + "'";
             
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
                 CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                         
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

                DataTable dtUserTypeID = new DataTable();
                DataSet ds = new DataSet();

                string makeSQL = "";
                if (txtSearchUserRole.Text == "")
                {
                    makeSQL = " select  * from PF_PARTY ORDER BY UPDATE_DATE desc, CREATE_DATE desc";
                }
                else
                {
                    makeSQL = " select  * from PF_PARTY where PARTY_ID like '" + txtSearchUserRole.Text + "%' or PARTY_NAME like '" + txtSearchUserRole.Text + "%' or PARTY_ARABIC_NAME like '" + txtSearchUserRole.Text + "%' or PARTY_VAT_NO like '" + txtSearchUserRole.Text + "%' or IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY UPDATE_DATE desc, CREATE_DATE desc";

                    alert_box.Visible = false;
                }
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "PARTY_ID" };

                GridView1.DataBind();
                conn.Close();
                //alert_box.Visible = false;
                TextSupplierName.Focus();
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
                int USER_DATA_ID = Convert.ToInt32(TextSupplierID.Text);   
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  PF_PARTY  set PARTY_NAME = :TextSupplierName, PARTY_ARABIC_NAME = :TextSupArabicName, PARTY_VAT_NO = :TextSupVatNo, PARTY_ADD_1 = :TextSup_Add_1, PARTY_ADD_2 = :TextSup_Add_2, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where PARTY_ID = :NoSupplierID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[9];
                objPrm[0] = cmdi.Parameters.Add("TextSupplierName", TextSupplierName.Text);
                objPrm[1] = cmdi.Parameters.Add("TextSupArabicName", TextSupArabicName.Text);
                objPrm[2] = cmdi.Parameters.Add("TextSupVatNo", TextSupVatNo.Text);
                objPrm[3] = cmdi.Parameters.Add("TextSup_Add_1", TextSup_Add_1.Text);
                objPrm[4] = cmdi.Parameters.Add("TextSup_Add_2", TextSup_Add_2.Text); 
                objPrm[5] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[6] = cmdi.Parameters.Add("NoSupplierID", USER_DATA_ID);
                objPrm[7] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[8] = cmdi.Parameters.Add("TextIsActive", ISActive);

                cmdi.ExecuteNonQuery();
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

        


        protected void BtnDelete_Click(object sender, EventArgs e)
        { 
          try
           {
            if (IS_DELETE_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                int USER_DATA_ID = Convert.ToInt32(TextSupplierID.Text);
                string delete_user_page = " delete from PF_PARTY where PARTY_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
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
            TextSupVatNo.Text = "";
            TextSup_Add_1.Text = "";
            TextSup_Add_2.Text = "";
            CheckSupplierName.Text = ""; 
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

        public void TextSupplierName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextSupplierName.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from PF_PARTY where PARTY_NAME = '" + TextSupplierName.Text + "'";
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