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



namespace NRCAPPS.NRC
{
    public partial class NrcDashboardItemOrder : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdi, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt;
        int RowCount;
        string IS_PAGE_ACTIVE = "";
        string IS_ADD_ACTIVE = "";
        string IS_EDIT_ACTIVE = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE = "";  
 
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
                    IS_PAGE_ACTIVE = dt.Rows[i]["IS_PAGE_ACTIVE"].ToString();
                    IS_ADD_ACTIVE = dt.Rows[i]["IS_ADD_ACTIVE"].ToString();
                    IS_EDIT_ACTIVE = dt.Rows[i]["IS_EDIT_ACTIVE"].ToString();
                    IS_DELETE_ACTIVE = dt.Rows[i]["IS_DELETE_ACTIVE"].ToString();
                    IS_VIEW_ACTIVE = dt.Rows[i]["IS_VIEW_ACTIVE"].ToString();
                }

                if (IS_PAGE_ACTIVE == "Enable")
                {
                      if (!IsPostBack)
                    {
                        DataTable dtUserID = new DataTable();
                        DataSet dsu = new DataSet();
                        string makeUserSQL = " select NU.USER_ID, NU.EMP_ID || ' - ' || HE.EMP_FNAME || ' ' || HE.EMP_LNAME || ' - ' || NUR.USER_ROLE_NAME AS EMP_NAME, NUR.USER_ROLE_NAME, NUR.UR_BG_COLOR from NRC_USER NU left join HR_EMPLOYEES HE ON HE.EMP_ID = NU.EMP_ID left join NRC_USER_ROLE  NUR ON NUR.USER_ROLE_ID =  NU.USER_ROLE_ID WHERE NU.IS_ACTIVE = 'Enable' ORDER BY  NU.EMP_ID ASC";
                        dsu = ExecuteBySqlString(makeUserSQL);
                        dtUserID = (DataTable)dsu.Tables[0];
                        DropDownUserID.DataSource = dtUserID;
                        DropDownUserID.DataValueField = "USER_ID";
                        DropDownUserID.DataTextField = "EMP_NAME";
                        DropDownUserID.DataBind();
                        DropDownUserID.Items.Insert(0, new ListItem("Select User", "0"));
                           
                     //   Display();

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
            if (IS_EDIT_ACTIVE == "Enable")
            {

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int DropUserID = Convert.ToInt32(DropDownUserID.Text);
                int DropItemID = Convert.ToInt32(DropDownUserItemID.Text);
                
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string update_user = "update  NRC_DASHBOARD_ITEMS_USER  set ORDER_BY = :TextOrderBy,  UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID where USER_ID = :NoDropUserID and DASH_ITEM_ID = :NoDropItemID ";
                cmdi = new OracleCommand(update_user, conn);

                OracleParameter[] objPrm = new OracleParameter[5];
                objPrm[0] = cmdi.Parameters.Add("TextOrderBy", TextOrderBy.Text); 
                objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[2] = cmdi.Parameters.Add("NoCuserID", userID);
                objPrm[3] = cmdi.Parameters.Add("NoDropUserID", DropUserID);
                objPrm[4] = cmdi.Parameters.Add("NoDropItemID", DropItemID);
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Employee Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearText();

                GridView1.DataSource = null;
                GridView1.DataBind(); 
            }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
        }

     
        protected void linkSelectClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             int USER_DATA_ID = Convert.ToInt32(DropDownUserID.SelectedValue); 
              
             DataTable dtUserPageID = new DataTable();
             DataSet dsp = new DataSet();
             string makePageSQL = " SELECT NDIU.DASH_ITEM_ID, NDIU.ORDER_BY ||' - '|| NDI.ITEM_NAME AS ITEM_NAME, NDIU.USER_ID, HEDIV.DIV_BG_COLOR, NDIU.ORDER_BY FROM NRC_DASHBOARD_ITEMS_USER NDIU LEFT JOIN NRC_DASHBOARD_ITEMS NDI ON NDI.DASH_ITEM_ID = NDIU.DASH_ITEM_ID  LEFT JOIN HR_EMP_DIVISIONS HEDIV ON HEDIV.DIVISION_ID  =  NDI.DIVISION_ID WHERE  NDIU.USER_ID  = '" + USER_DATA_ID + "' AND NDI.IS_ACTIVE = 'Enable' ORDER BY NDIU.ORDER_BY ";
             dsp = ExecuteBySqlString(makePageSQL);
             dtUserPageID = (DataTable)dsp.Tables[0];
             DropDownUserItemID.DataSource = dtUserPageID;
             DropDownUserItemID.DataValueField = "DASH_ITEM_ID";
             DropDownUserItemID.DataTextField = "ITEM_NAME";
             DropDownUserItemID.DataBind();
             DropDownUserItemID.Items.Insert(0, new ListItem("Select Dashboard Item", "0"));

          //   dt = new DataTable();
             oradata.Fill(dtUserPageID);
             GridView1.DataSource = dtUserPageID;
             GridView1.DataKeyNames = new string[] { "DASH_ITEM_ID" };
             GridView1.DataBind(); 
             
             conn.Close();
             TextOrderBy.Text = "";   
             alert_box.Visible = false;   
        }

        protected void ItemSelectClick(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            int USER_DATA_ID = Convert.ToInt32(DropDownUserID.SelectedValue);
            int User_Item_ID = Convert.ToInt32(DropDownUserItemID.SelectedValue);
            TextOrderBy.Text = "";
            DataTable dtUserTypeID = new DataTable();
            DataSet ds = new DataSet();
            string makeSQL = " SELECT  ORDER_BY FROM NRC_DASHBOARD_ITEMS_USER WHERE USER_ID = '" + USER_DATA_ID + "'  AND DASH_ITEM_ID = '" + User_Item_ID + "' ";
           
            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;

            for (int i = 0; i < RowCount; i++)
            {
                TextOrderBy.Text = dt.Rows[i]["ORDER_BY"].ToString(); 
            }

            conn.Close(); 

            alert_box.Visible = false;

            if (TextOrderBy.Text == "")
            {
                BtnUpdate.Attributes.Add("aria-disabled", "false");
                BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
            }
            else { 
                BtnUpdate.Attributes.Add("aria-disabled", "true");
                BtnUpdate.Attributes.Add("class", "btn btn-success active");
            }

        }
         
        public void clearTextField(object sender, EventArgs e)
        {
            
            DropDownUserID.Text = "0";
            TextOrderBy.Text = "";
            DropDownUserItemID.SelectedIndex = -1; 
        }

        public void clearText()
        {
            TextOrderBy.Text = "";  
            DropDownUserID.Text = "0";
            DropDownUserItemID.SelectedIndex = -1; 

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