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
    public partial class MfItemBin : System.Web.UI.Page
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
                        string makeDropDownCatSQL = " SELECT ITEM_ID, ITEM_NAME || ' - ' || ITEM_CODE AS ITEM_NAME FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        dc = ExecuteBySqlString(makeDropDownCatSQL);
                        dtCatID = (DataTable)dc.Tables[0];
                        DropDownItemID.DataSource = dtCatID;
                        DropDownItemID.DataValueField = "ITEM_ID";
                        DropDownItemID.DataTextField = "ITEM_NAME";
                        DropDownItemID.DataBind();
                        DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));

                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled");

                        DropDownItemID.Focus();
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
                    int CategoryID = Convert.ToInt32(DropDownItemID.Text);
                    string get_customer_id = "select MF_ITEM_BIN_ID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_customer_id, conn);
                    int newItemID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                    string ISActive   = CheckIsActive.Checked ? "Enable" : "Disable"; 

                    string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                    string insert_user = "insert into MF_ITEM_BIN (ITEM_BIN_ID, ITEM_BIN_NAME, CAPACITY_WEIGHT,  IS_ACTIVE, ITEM_ID, CREATE_DATE, C_USER_ID) VALUES ( :NoItemID, :TextItemName, :TextCapacityWeight,  :TextIsActive, :NoCategoryID, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[7];
                    objPrm[0] = cmdi.Parameters.Add("NoItemID", newItemID);
                    objPrm[1] = cmdi.Parameters.Add("TextItemName", TextItemName.Text);
                    objPrm[2] = cmdi.Parameters.Add("TextCapacityWeight", TextCapacityWeight.Text); 
                    objPrm[3] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[4] = cmdi.Parameters.Add("NoCategoryID", CategoryID); 
                    objPrm[5] = cmdi.Parameters.Add("u_date", c_date);
                    objPrm[6] = cmdi.Parameters.Add("NoCuserID", userID);


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
              
             string makeSQL = " select *  from MF_ITEM_BIN where ITEM_BIN_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextItemID.Text = dt.Rows[i]["ITEM_BIN_ID"].ToString();
                 TextItemName.Text = dt.Rows[i]["ITEM_BIN_NAME"].ToString();
                 TextCapacityWeight.Text = dt.Rows[i]["CAPACITY_WEIGHT"].ToString();
                 DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString(); 
                 CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false); 
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
                int CategoryID = Convert.ToInt32(DropDownItemID.Text);
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable"; 
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  MF_ITEM_BIN  set ITEM_BIN_NAME = :TextItemName, CAPACITY_WEIGHT = :TextCapacityWeight, IS_ACTIVE = :TextIsActive, ITEM_ID = :NoCategoryID, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID where ITEM_BIN_ID = :NoItemID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[7];
                objPrm[0] = cmdi.Parameters.Add("TextItemName", TextItemName.Text);
                objPrm[1] = cmdi.Parameters.Add("TextCapacityWeight", TextCapacityWeight.Text);
                objPrm[2] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[3] = cmdi.Parameters.Add("NoCategoryID", CategoryID); 
                objPrm[4] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[5] = cmdi.Parameters.Add("NoItemID", USER_DATA_ID);
                objPrm[6] = cmdi.Parameters.Add("NoC_USER_ID", userID);

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
                    makeSQL = " select  MI.*, MC.ITEM_NAME from MF_ITEM_BIN MI LEFT JOIN MF_ITEM MC ON MC.ITEM_ID = MI.ITEM_ID ORDER BY MC.ITEM_ID, MI.ITEM_BIN_ID, MI.UPDATE_DATE desc, MI.CREATE_DATE desc";
                }
                else
                {
                    makeSQL = " select  MI.*, MC.ITEM_NAME from MF_ITEM_BIN MI LEFT JOIN MF_ITEM MC ON MC.ITEM_ID = MI.ITEM_ID where MI.ITEM_BIN_ID like '" + txtSearchUserRole.Text + "%' or MI.ITEM_BIN_NAME like '" + txtSearchUserRole.Text + "%'  or MI.IS_ACTIVE like '" + txtSearchUserRole.Text + "%'  ORDER BY MC.ITEM_ID, MI.ITEM_BIN_ID, MI.UPDATE_DATE desc, MI.CREATE_DATE desc ";

                    alert_box.Visible = false;
                }
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "ITEM_BIN_ID" }; 
                GridView1.DataBind();
                
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

     
         
        protected void BtnDelete_Click(object sender, EventArgs e)
        { 
          try
           {
            if (IS_DELETE_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                int USER_DATA_ID = Convert.ToInt32(TextItemID.Text);
                string delete_user_page = " delete from MF_ITEM_BIN where ITEM_BIN_ID  = '" + USER_DATA_ID + "'";

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
            TextCapacityWeight.Text = ""; 
            CheckItemName.Text = ""; 
            CheckItemCode.Text = ""; 
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            
        }

        public void clearText()
        {
            TextItemID.Text = "";
            TextItemName.Text = "";
            TextCapacityWeight.Text = ""; 
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
                cmd.CommandText = "select * from MF_ITEM_BIN where ITEM_BIN_NAME = '" + TextItemName.Text + "'";
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
                    TextCapacityWeight.Focus();
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