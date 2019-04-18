using System; 
using System.Configuration;
using System.Data;
using System.Linq; 
using System.Web.UI; 
using System.Web.UI.WebControls; 
using System.Data.OracleClient;
using System.IO;  
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using System.Web.Services;

namespace NRCAPPS.MS
{
    public partial class MsPurchaseVatAdjustment : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl, cmdsp;
        OracleDataAdapter oradata; 
        DataTable dt;
        int RowCount;
         string IS_PAGE_ACTIVE = "", IS_ADD_ACTIVE = "",IS_EDIT_ACTIVE = "",IS_DELETE_ACTIVE = "",IS_VIEW_ACTIVE = "", IS_REPORT_ACTIVE = "", IS_PRINT_ACTIVE =""; 
        public bool IsLoad { get; set; }  public bool IsLoad2 { get; set; } public bool IsLoad3 { get; set; } public bool IsLoad4 { get; set; } 
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_NAME"] != null)
            {
                string requestedFile = Path.GetFileName(Request.Path);
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = " SELECT  NUPP.IS_PAGE_ACTIVE, NUPP.IS_ADD_ACTIVE, NUPP.IS_EDIT_ACTIVE, NUPP.IS_DELETE_ACTIVE, NUPP.IS_VIEW_ACTIVE, NUPP.IS_REPORT_ACTIVE, NUPP.IS_PRINT_ACTIVE FROM NRC_USER_PAGE_PERMISSION NUPP LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NUPP.USER_PAGE_ID  WHERE NUPP.USER_ID = '" + Session["USER_ID"] + "' AND NUP.IS_ACTIVE = 'Enable' AND NUP.PAGE_URL = '" + requestedFile + "' ";
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
                    IS_REPORT_ACTIVE = dt.Rows[i]["IS_REPORT_ACTIVE"].ToString();
                    IS_PRINT_ACTIVE = dt.Rows[i]["IS_PRINT_ACTIVE"].ToString();
                }

                if (IS_PAGE_ACTIVE == "Enable")
                {
                     if (!IsPostBack)
                    {
                        DataTable dtSupplierID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeSupplierSQL = " SELECT PARTY_ID,  PARTY_ID || ' - ' || PARTY_NAME || ' - ' || PARTY_VAT_NO AS PARTY_NAME  FROM WP_PARTY WHERE IS_ACTIVE = 'Enable' ORDER BY PARTY_NAME ASC";
                        ds = ExecuteBySqlString(makeSupplierSQL);
                        dtSupplierID = (DataTable)ds.Tables[0];
                        DropDownSupplierID.DataSource = dtSupplierID;
                        DropDownSupplierID.DataValueField = "PARTY_ID";
                        DropDownSupplierID.DataTextField = "PARTY_NAME";
                        DropDownSupplierID.DataBind();
                        DropDownSupplierID.Items.Insert(0, new ListItem("Select  Supplier", "0"));

                        DropDownSupplierID2.DataSource = dtSupplierID;
                        DropDownSupplierID2.DataValueField = "PARTY_ID";
                        DropDownSupplierID2.DataTextField = "PARTY_NAME";
                        DropDownSupplierID2.DataBind();
                        DropDownSupplierID2.Items.Insert(0, new ListItem("Select  Supplier", "0"));
                            
                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeDropDownItemSQL = " SELECT ITEM_ID, ITEM_NAME || ' - ' || ITEM_CODE AS ITEM_NAME FROM WP_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        dsi = ExecuteBySqlString(makeDropDownItemSQL);
                        dtItemID = (DataTable)dsi.Tables[0];
                        DropDownItemList.DataSource = dtItemID;
                        DropDownItemList.DataValueField = "ITEM_ID";
                        DropDownItemList.DataTextField = "ITEM_NAME";
                        DropDownItemList.DataBind();
                        DropDownItemList.Items.Insert(0, new ListItem("All Item", "0"));  
                             

                        DataTable dtPgeID = new DataTable();
                        DataSet dsp = new DataSet();
                        string makeDropDownPgeSQL = " SELECT * FROM NRC_VAT WHERE IS_ACTIVE = 'Enable' ORDER BY VAT_ID ASC";
                        dsp = ExecuteBySqlString(makeDropDownPgeSQL);
                        dtPgeID = (DataTable)dsp.Tables[0];
                        DropDownVatID.DataSource = dtPgeID;
                        DropDownVatID.DataValueField = "VAT_ID";
                        DropDownVatID.DataTextField = "VAT_PERCENT";
                        DropDownVatID.DataBind();
                          
                      //  VatPercent.Visible = false;
                     //   TextItemAmountWP.Attributes.Add("readonly", "readonly"); 
                    //    TextItemVatAmountWP.Enabled = false;
                       
                          
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

     
        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
         try
          {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]); 
                int SupplierID = Convert.ToInt32(DropDownSupplierID.Text);   
                 
                string VatStatus = RadioBtnVatWp.SelectedValue;

                int VatID = 0; double VatPercent = 0.00;  
                if (RadioBtnVatWp.SelectedValue == "VatYes")
                {
                    VatID = Convert.ToInt32(DropDownVatID.Text);
                    VatPercent = Convert.ToDouble(DropDownVatID.SelectedItem.Text); 
                  }
                else
                {
                }

                OracleCommand cmd = new OracleCommand("PRO_WP_VAT_ADJUSTMENT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.Parameters.Add(new OracleParameter("NoVatID", OracleType.Number)).Value = VatID;
                cmd.Parameters.Add(new OracleParameter("VatPercent", OracleType.Number)).Value = VatPercent;
                cmd.Parameters.Add(new OracleParameter("NoSupplierID", OracleType.Number)).Value = SupplierID;
                cmd.Parameters.Add(new OracleParameter("RadioBtnVatWp", OracleType.VarChar)).Value = VatStatus;
                cmd.Parameters.Add(new OracleParameter("TextMonthYear0", OracleType.VarChar)).Value = TextMonthYear0.Text;
                 
                cmd.ExecuteNonQuery();
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Purchase Vat Adjusment Data Update Successfully" + VatStatus));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                 
                clearText();  
                VatPercentBox.Style.Add("display", "none");
                 
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

            DropDownSupplierID.Text = "0";
            RadioBtnVatWp.SelectedIndex = 0;
            TextMonthYear0.Text = "";
        }

        public void clearText()
        {
            DropDownSupplierID.Text = "0";
            RadioBtnVatWp.SelectedIndex = 0;
            TextMonthYear0.Text = "";
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

        
        protected void BtnReport_Click(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad = true;
                alert_box.Visible = false;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }

        }
       
         
       } 
    }