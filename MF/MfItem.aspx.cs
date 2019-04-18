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
 
namespace NRCAPPS.MF
{
    public partial class MfItem : System.Web.UI.Page
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
                        string makeDropDownCatSQL = " SELECT * FROM MF_CATEGORY WHERE IS_ACTIVE = 'Enable' ORDER BY CATEGORY_ID ASC";
                        dc = ExecuteBySqlString(makeDropDownCatSQL);
                        dtCatID = (DataTable)dc.Tables[0];
                        DropDownCategoryID.DataSource = dtCatID;
                        DropDownCategoryID.DataValueField = "CATEGORY_ID";
                        DropDownCategoryID.DataTextField = "CATEGORY_NAME";
                        DropDownCategoryID.DataBind();
                        DropDownCategoryID.Items.Insert(0, new ListItem("Select  Category", "0"));

                        DataTable dtInID = new DataTable();
                        DataSet dcn = new DataSet();
                        string makeDropDownInSQL = " SELECT NATURE_ITEM_ID, NATURE_NAME || '-' || SHORT_CODE AS NATURE_NAME FROM NRC_INVENTORY_NATURE_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY NATURE_ITEM_ID ASC";
                        dcn = ExecuteBySqlString(makeDropDownInSQL);
                        dtInID = (DataTable)dcn.Tables[0];
                        DropDownInNatureID.DataSource = dtInID;
                        DropDownInNatureID.DataValueField = "NATURE_ITEM_ID";
                        DropDownInNatureID.DataTextField = "NATURE_NAME";
                        DropDownInNatureID.DataBind();
                        DropDownInNatureID.Items.Insert(0, new ListItem("Select  Inventory Nature", "0"));

                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled");

                        DropDownCategoryID.Focus();
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
                    int CategoryID = Convert.ToInt32(DropDownCategoryID.Text);
                    int InNatureID = Convert.ToInt32(DropDownInNatureID.Text);
                    string get_customer_id = "select MF_ITEM_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_customer_id, conn);
                    int newItemID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive   = CheckIsActive.Checked ? "Enable" : "Disable";
                    string IsTransferActive = CheckIsTransferActive.Checked ? "Enable" : "Disable";
                    string IsPurchaseActive = CheckIsPurchaseActive.Checked ? "Enable" : "Disable";
                    string IsProductionActive = CheckIsProductionActive.Checked ? "Enable" : "Disable";
                    string IsFgActive = CheckIsFgActive.Checked ? "Enable" : "Disable";
                    string IsSalesActive = CheckIsSalesActive.Checked ? "Enable" : "Disable";

                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string insert_user = "insert into MF_ITEM (ITEM_ID, ITEM_NAME, ITEM_CODE,  IS_ACTIVE, CATEGORY_ID, NATURE_ITEM_ID, IS_TRANSFER_ACTIVE, IS_PURCHASE_ACTIVE, IS_PRODUCTION_ACTIVE, IS_FG_ACTIVE, IS_SALES_ACTIVE, CREATE_DATE, C_USER_ID) VALUES ( :NoItemID, :TextItemName, :TextItemCode,  :TextIsActive, :NoCategoryID, :NoInNatureID, :TextIsTransferActive, :TextIsPurchaseActive, :TextIsProductionActive, :TextIsFgActive, :TextIsSalesActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[13];
                    objPrm[0] = cmdi.Parameters.Add("NoItemID", newItemID);
                    objPrm[1] = cmdi.Parameters.Add("TextItemName", TextItemName.Text);
                    objPrm[2] = cmdi.Parameters.Add("TextItemCode", TextItemCode.Text); 
                    objPrm[3] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[4] = cmdi.Parameters.Add("NoCategoryID", CategoryID);
                    objPrm[5] = cmdi.Parameters.Add("NoInNatureID", InNatureID);
                    objPrm[6] = cmdi.Parameters.Add("TextIsTransferActive", IsTransferActive);
                    objPrm[7] = cmdi.Parameters.Add("TextIsPurchaseActive", IsPurchaseActive);
                    objPrm[8] = cmdi.Parameters.Add("TextIsProductionActive", IsProductionActive);
                    objPrm[9] = cmdi.Parameters.Add("TextIsFgActive", IsFgActive);
                    objPrm[10] = cmdi.Parameters.Add("TextIsSalesActive", IsSalesActive);
                    objPrm[11] = cmdi.Parameters.Add("u_date", c_date);
                    objPrm[12] = cmdi.Parameters.Add("NoCuserID", userID);


                    cmdi.ExecuteNonQuery();

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Item Successfully"));
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
              
             string makeSQL = " select *  from MF_ITEM where ITEM_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();
                 TextItemName.Text = dt.Rows[i]["ITEM_NAME"].ToString();
                 TextItemCode.Text = dt.Rows[i]["ITEM_CODE"].ToString();
                 DropDownCategoryID.Text = dt.Rows[i]["CATEGORY_ID"].ToString();
                 DropDownInNatureID.Text = dt.Rows[i]["NATURE_ITEM_ID"].ToString(); 
                 CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                 CheckIsTransferActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_TRANSFER_ACTIVE"].ToString() == "Enable" ? true : false);
                 CheckIsPurchaseActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_PURCHASE_ACTIVE"].ToString() == "Enable" ? true : false);
                 CheckIsProductionActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_PRODUCTION_ACTIVE"].ToString() == "Enable" ? true : false);
                 CheckIsFgActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_FG_ACTIVE"].ToString() == "Enable" ? true : false);
                 CheckIsSalesActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_SALES_ACTIVE"].ToString() == "Enable" ? true : false);

            } 
             
             conn.Close();
             Display();
             CheckItemName.Text = "";
             alert_box.Visible = false;
             BtnAdd.Attributes.Add("aria-disabled", "false");
             BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
             BtnUpdate.Attributes.Add("aria-disabled", "false");
             BtnUpdate.Attributes.Add("class", "btn btn-success active");
             BtnDelete.Attributes.Add("aria-disabled", "false");
             BtnDelete.Attributes.Add("class", "btn btn-danger active");

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
                int InNatureID = Convert.ToInt32(DropDownInNatureID.Text);
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string IsTransferActive = CheckIsTransferActive.Checked ? "Enable" : "Disable";
                string IsPurchaseActive = CheckIsPurchaseActive.Checked ? "Enable" : "Disable";
                string IsProductionActive = CheckIsProductionActive.Checked ? "Enable" : "Disable";
                string IsFgActive = CheckIsFgActive.Checked ? "Enable" : "Disable";
                string IsSalesActive = CheckIsSalesActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  MF_ITEM  set ITEM_NAME = :TextItemName, ITEM_CODE = :TextItemCode, IS_ACTIVE = :TextIsActive, CATEGORY_ID = :NoCategoryID, NATURE_ITEM_ID =:NoInNatureID, IS_TRANSFER_ACTIVE = :TextIsTransferActive, IS_PURCHASE_ACTIVE = :TextIsPurchaseActive, IS_PRODUCTION_ACTIVE = :TextIsProductionActive, IS_FG_ACTIVE = :TextIsFgActive, IS_SALES_ACTIVE = :TextIsSalesActive,  UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID where ITEM_ID = :NoItemID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[13];
                objPrm[0] = cmdi.Parameters.Add("TextItemName", TextItemName.Text);
                objPrm[1] = cmdi.Parameters.Add("TextItemCode", TextItemCode.Text);
                objPrm[2] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[3] = cmdi.Parameters.Add("NoCategoryID", CategoryID);
                objPrm[4] = cmdi.Parameters.Add("NoInNatureID", InNatureID);
                objPrm[5] = cmdi.Parameters.Add("TextIsTransferActive", IsTransferActive);
                objPrm[6] = cmdi.Parameters.Add("TextIsPurchaseActive", IsPurchaseActive);
                objPrm[7] = cmdi.Parameters.Add("TextIsProductionActive", IsProductionActive);
                objPrm[8] = cmdi.Parameters.Add("TextIsFgActive", IsFgActive);
                objPrm[9] = cmdi.Parameters.Add("TextIsSalesActive", IsSalesActive);
                objPrm[10] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[11] = cmdi.Parameters.Add("NoItemID", USER_DATA_ID);
                objPrm[12] = cmdi.Parameters.Add("NoC_USER_ID", userID);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Item Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearText();
                Display();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
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
                    makeSQL = " select  MI.*, MC.CATEGORY_NAME, NINI.NATURE_NAME || ' (' || SHORT_CODE || ') ' AS NATURE_NAME from MF_ITEM MI LEFT JOIN MF_CATEGORY MC ON MC.CATEGORY_ID = MI.CATEGORY_ID LEFT JOIN NRC_INVENTORY_NATURE_ITEM NINI ON NINI.NATURE_ITEM_ID = MI.NATURE_ITEM_ID ORDER BY MC.CATEGORY_ID, MI.ITEM_ID, MI.UPDATE_DATE desc, MI.CREATE_DATE desc";
                }
                else
                {
                    makeSQL = " select  MI.*, MC.CATEGORY_NAME, NINI.NATURE_NAME || ' (' || SHORT_CODE || ') ' AS NATURE_NAME from MF_ITEM MI LEFT JOIN MF_CATEGORY MC ON MC.CATEGORY_ID = MI.CATEGORY_ID LEFT JOIN NRC_INVENTORY_NATURE_ITEM NINI ON NINI.NATURE_ITEM_ID = MI.NATURE_ITEM_ID where MI.ITEM_ID like '" + txtSearchUserRole.Text + "%' or  MI.ITEM_NAME like '" + txtSearchUserRole.Text + "%' or MI.ITEM_CODE like '" + txtSearchUserRole.Text + "%'   or MI.IS_ACTIVE like '" + txtSearchUserRole.Text + "%'  ORDER BY MC.CATEGORY_ID, MI.ITEM_ID, MI.UPDATE_DATE desc, MI.CREATE_DATE desc ";

                    alert_box.Visible = false;
                }
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "ITEM_ID" }; 
                GridView1.DataBind();
                if (dt.Rows.Count > 0)
                {
                    GroupGridView(GridView1.Rows, 0, 2);
                }
                else
                {

                }
                conn.Close();
                //alert_box.Visible = false;
            }
            else
            {
             //  Response.Redirect("~/PagePermissionError.aspx");
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
         
         
        protected void BtnDelete_Click(object sender, EventArgs e)
        { 
          try
           {
            if (IS_DELETE_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                int USER_DATA_ID = Convert.ToInt32(TextItemID.Text);
                string delete_user_page = " delete from MF_ITEM where ITEM_ID  = '" + USER_DATA_ID + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Item Delete Successfully"));
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
                cmd.CommandText = "select * from MF_ITEM where ITEM_NAME = '" + TextItemName.Text + "'";
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
                    TextItemCode.Focus();
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
                cmd.CommandText = "select * from MF_ITEM where ITEM_CODE = '" + TextItemCode.Text + "'";
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
                    CheckIsTransferActive.Focus();
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