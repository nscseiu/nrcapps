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
    public partial class NrcUserRolePage : System.Web.UI.Page
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
                        DataTable dtUserRoleID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeRoleSQL = " SELECT USER_ROLE_ID, USER_ROLE_SHORT_NAME || ' - ' ||USER_ROLE_NAME AS USER_ROLE_SNAME FROM NRC_USER_ROLE WHERE IS_ACTIVE = 'Enable' ORDER BY USER_ROLE_ID ASC ";
                        ds = ExecuteBySqlStringUserType(makeRoleSQL);
                        dtUserRoleID = (DataTable)ds.Tables[0];
                        DropDownUserRoleID.DataSource     = dtUserRoleID;
                        DropDownUserRoleID.DataValueField = "USER_ROLE_ID";
                        DropDownUserRoleID.DataTextField  = "USER_ROLE_SNAME";
                        DropDownUserRoleID.DataBind();
                        DropDownUserRoleID.Items.Insert(0, new ListItem("Select User Role", "0"));


                        DataTable dtUserPageID = new DataTable();
                        DataSet dsp = new DataSet();
                        string makePageSQL = " SELECT USER_PAGE_ID, PAGE_NAME || ' - ' ||PAGE_URL AS USER_PAGE_NAME FROM NRC_USER_PAGES WHERE IS_ACTIVE = 'Enable' ORDER BY  USER_PAGE_ID ASC";
                        dsp = ExecuteBySqlStringPages(makePageSQL);
                        dtUserPageID = (DataTable)dsp.Tables[0];
                        DropDownUserPageID.DataSource = dtUserPageID;
                        DropDownUserPageID.DataValueField = "USER_PAGE_ID";
                        DropDownUserPageID.DataTextField = "USER_PAGE_NAME";
                        DropDownUserPageID.DataBind();
                    
                           
                        Display();

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
                int UserRoleID = Convert.ToInt32(DropDownUserRoleID.Text);

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string delete_user = " delete from NRC_USER_ROLE_PAGE where USER_ROLE_ID  = '" + UserRoleID + "'";

                cmdi = new OracleCommand(delete_user, conn);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();


                foreach (ListItem li in DropDownUserPageID.Items)
                {

                    if (li.Selected == true)
                    {
                         
                        string insert_user = "insert into  NRC_USER_ROLE_PAGE (USER_ROLE_ID, USER_PAGE_ID, CREATE_DATE, UPDATE_DATE, C_USER_ID, U_USER_ID) values ( :NoUserRoleID, :NoUserPageID, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID , :NoUuserID)";
                        cmdi = new OracleCommand(insert_user, conn);

                        OracleParameter[] objPrm = new OracleParameter[6];
                        objPrm[0] = cmdi.Parameters.Add("NoUserRoleID", UserRoleID);
                        objPrm[1] = cmdi.Parameters.Add("NoUserPageID", li.Value);
                        objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                        objPrm[3] = cmdi.Parameters.Add("u_date", u_date);
                        objPrm[4] = cmdi.Parameters.Add("NoCuserID", userID);
                        objPrm[5] = cmdi.Parameters.Add("NoUuserID", userID);

                        cmdi.ExecuteNonQuery(); 
                    }

                }
                 
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("User Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                Display();
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
             int USER_DATA_ID = 0;
             if (DropDownUserRoleID.SelectedValue != null && Convert.ToInt32(DropDownUserRoleID.SelectedValue) != 0)
             {
               USER_DATA_ID = Convert.ToInt32(DropDownUserRoleID.SelectedValue); 
             } else { 
               //  LinkButton btn = (LinkButton)sender;
               //  Session["user_data_id"] = btn.CommandArgument;
               //  USER_DATA_ID = Convert.ToInt32(Session["user_data_id"]); 
             }

             DataTable dtUserTypeID = new DataTable();
             DataSet ds = new DataSet();
             string makeSQL = " SELECT NURP.USER_ROLE_ID, NURP.USER_PAGE_ID FROM NRC_USER_ROLE_PAGE NURP where NURP.USER_ROLE_ID  = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 
     
              
              foreach (ListItem li in DropDownUserPageID.Items)
              {
                 
                  li.Selected = false;
                  for (int i = 0; i < RowCount; i++)
                  {
                      DropDownUserRoleID.Text = dt.Rows[i]["USER_ROLE_ID"].ToString();
                      if (li.Value == dt.Rows[i]["USER_PAGE_ID"].ToString())
                      {
                          li.Selected = true;
                        //  lblMessage.Text += "" + "Item value :: " + "</font>" + "<b><font color=red>" + li.Value + "</font></br>";
                      }
                  }
              }

             
             conn.Close();
             Display(); 
             alert_box.Visible = false;   
        }

        public void Display()
        {
            
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            DataTable dtUserTypeID = new DataTable();
            DataSet ds = new DataSet();
       
            string makeSQL = "";
            if (txtSearchUser.Text == "")
            {
                makeSQL = " SELECT NURP.USER_ROLE_ID, NUR.USER_ROLE_NAME, NUR.UR_BG_COLOR, NURP.USER_PAGE_ID, NUP.PAGE_NAME, NURP.CREATE_DATE, NURP.UPDATE_DATE, NURP.C_USER_ID, NURP.U_USER_ID FROM NRC_USER_ROLE_PAGE NURP LEFT JOIN NRC_USER_ROLE NUR ON NUR.USER_ROLE_ID = NURP.USER_ROLE_ID LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NURP.USER_PAGE_ID  ORDER BY NURP.USER_ROLE_ID ASC, NURP.USER_PAGE_ID ASC";
            }
            else
            {
                makeSQL = " SELECT NURP.USER_ROLE_ID, NUR.USER_ROLE_NAME, NUR.UR_BG_COLOR, NURP.USER_PAGE_ID, NUP.PAGE_NAME, NURP.CREATE_DATE, NURP.UPDATE_DATE, NURP.C_USER_ID, NURP.U_USER_ID FROM NRC_USER_ROLE_PAGE NURP LEFT JOIN NRC_USER_ROLE NUR ON NUR.USER_ROLE_ID = NURP.USER_ROLE_ID LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NURP.USER_PAGE_ID where NURP.USER_ROLE_ID like '" + txtSearchUser.Text + "%' or  NUR.USER_ROLE_NAME like '" + txtSearchUser.Text + "%' or NUP.PAGE_NAME like '" + txtSearchUser.Text + "%' ORDER BY NURP.USER_ROLE_ID ASC, NURP.USER_PAGE_ID ASC";
            }

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            GridView1.DataSource = dt;
            GridView1.DataKeyNames = new string[] { "USER_ROLE_ID" }; 
            GridView1.DataBind();
         //   GroupGridView(GridView1.Rows, 0, 3);

            if (dt.Rows.Count > 0)
            {
                GroupGridView(GridView1.Rows, 0, 3);
            }
            else
            {

            }

              
            conn.Close(); 
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




        protected void GridViewSearchUser(object sender, EventArgs e)
        {
            this.Display();
        }
 
         protected void GridViewPage_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            Display();
            alert_box.Visible = false;
        }

      

        public void clearTextField(object sender, EventArgs e)
        {
            
            DropDownUserRoleID.Text = "0";
            DropDownUserPageID.SelectedIndex = -1; 
        }

        public void clearText()
        {
              
            DropDownUserRoleID.Text = "0";
            DropDownUserPageID.SelectedIndex = -1; 

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


        public DataSet ExecuteBySqlStringPages(string sqlString)
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

        public DataSet ExecuteBySqlStringUser(string sqlString)
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