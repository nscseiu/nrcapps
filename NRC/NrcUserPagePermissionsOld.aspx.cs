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



namespace NRCAPPS.NRC
{
    public partial class NrcUserPagePermissionsOld : System.Web.UI.Page
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
                        dsu = ExecuteBySqlStringUser(makeUserSQL);
                        dtUserID = (DataTable)dsu.Tables[0];
                        DropDownUserID.DataSource = dtUserID;
                        DropDownUserID.DataValueField = "USER_ID";
                        DropDownUserID.DataTextField = "EMP_NAME";
                        DropDownUserID.DataBind();
                        DropDownUserID.Items.Insert(0, new ListItem("Select User", "0"));
                           
                       

                        alert_box.Visible = false;
                        alert_box_right.Visible = false;
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

        
        public void DisplayUserPagePer(object sender, EventArgs e)
        {

            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            int USER_DATA_ID = 0;
            USER_DATA_ID = Convert.ToInt32(DropDownUserID.SelectedValue);
             
            DataTable dtUserTypeID = new DataTable();
            DataSet ds = new DataSet();

            string makeSQL = " SELECT NU.USER_ID, NURP.USER_PAGE_ID, NUP.PAGE_NAME, NUPP.IS_PAGE_ACTIVE, NUPP.IS_ADD_ACTIVE, NUPP.IS_EDIT_ACTIVE, NUPP.IS_DELETE_ACTIVE, NUPP.IS_VIEW_ACTIVE, IS_REPORT_ACTIVE FROM NRC_USER NU LEFT JOIN NRC_USER_ROLE_PAGE NURP ON NURP.USER_ROLE_ID = NU.USER_ROLE_ID LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID  = NURP.USER_PAGE_ID LEFT JOIN NRC_USER_PAGE_PERMISSION NUPP ON NUPP.USER_PAGE_ID  = NURP.USER_PAGE_ID AND  NUPP.USER_ID = '" + USER_DATA_ID + "'   WHERE NU.USER_ID = '" + USER_DATA_ID + "' ORDER BY NUPP.UPDATE_DATE desc";
             

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            GridView3.DataSource = dt;
            GridView3.DataKeyNames = new string[] { "USER_ID" };
            GridView3.DataBind();  
            conn.Close(); 
            alert_box.Visible = false;
            alert_box_right.Visible = false;

          
        }

        public void chkStatus_IsPgaeActiveChanged(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            int userID = Convert.ToInt32(Session["USER_ID"]);  
            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
            int USER_DATA_ID = Convert.ToInt32(DropDownUserID.SelectedValue);

         //   CheckBox chkStatus = (CheckBox)sender;
         //   Int64 nID = Convert.ToInt64(GridView2.DataKeys[((GridViewRow)chkStatus.NamingContainer).RowIndex].Value);
           
                   

                        CheckBox chkStatus = (CheckBox)sender;
                        GridViewRow row = (GridViewRow)chkStatus.NamingContainer;     
            
                       CheckBox chkRowIsPageActive = (row.Cells[3].FindControl("IsPageActive") as CheckBox);
                       string IsPageActive = chkRowIsPageActive.Checked ? "Enable" : "Disable";
    
                        int Page_id = Convert.ToInt32(row.Cells[1].Text);
                        bool status = chkStatus.Checked;

                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = "select * from NRC_USER_PAGE_PERMISSION where USER_ID = '" + USER_DATA_ID + "' and USER_PAGE_ID = '" + Page_id + "'";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            // update data 
                            string update_user = "update  NRC_USER_PAGE_PERMISSION set IS_PAGE_ACTIVE = :NoIsPageActive, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoC_USER_ID  where USER_ID = :NoUserID and USER_PAGE_ID = :NoUserPageID ";
                            cmdi = new OracleCommand(update_user, conn);
                            OracleParameter[] objPrm = new OracleParameter[5];
                            objPrm[0] = cmdi.Parameters.Add("NoUserID", USER_DATA_ID);
                            objPrm[1] = cmdi.Parameters.Add("NoUserPageID", Page_id);
                            objPrm[2] = cmdi.Parameters.Add("NoIsPageActive", IsPageActive);
                            objPrm[3] = cmdi.Parameters.Add("u_date", u_date);
                            objPrm[4] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                            cmdi.ExecuteNonQuery();
                            cmdi.Parameters.Clear();
                            cmdi.Dispose();
                        }
                        else
                        {
                            // insert data
                            string insert_user = "insert into  NRC_USER_PAGE_PERMISSION (USER_ID, USER_PAGE_ID, IS_PAGE_ACTIVE, UPDATE_DATE, U_USER_ID) values ( :NoUserID, :NoUserPageID, :NoIsPageActive,  TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                            cmdi = new OracleCommand(insert_user, conn);

                            OracleParameter[] objPrm = new OracleParameter[3];
                            objPrm[0] = cmdi.Parameters.Add("NoUserID", USER_DATA_ID);
                            objPrm[1] = cmdi.Parameters.Add("NoUserPageID", Page_id);
                            objPrm[2] = cmdi.Parameters.Add("NoIsPageActive", IsPageActive);
                            objPrm[3] = cmdi.Parameters.Add("u_date", u_date);
                            objPrm[4] = cmdi.Parameters.Add("NoC_USER_ID", userID);

                            cmdi.ExecuteNonQuery();
                            cmdi.Parameters.Clear();
                            cmdi.Dispose();
                        }
                 
            
            conn.Close(); 
         
        }

        protected void BtnUpdateUserPagePer_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                int  USER_DATA_ID = Convert.ToInt32(DropDownUserID.SelectedValue); 

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                string delete_user = " delete from NRC_USER_PAGE_PERMISSION where USER_ID  = '" + USER_DATA_ID + "'";

                 cmdl = new OracleCommand(delete_user, conn);

                 cmdl.ExecuteNonQuery();
                 cmdl.Parameters.Clear();
                 cmdl.Dispose();

                foreach (GridViewRow gridRow in GridView3.Rows)
                {
                 
                       CheckBox chkRowIsPageActive = (gridRow.Cells[3].FindControl("IsPageActive") as CheckBox);
                       string IsPageActive = chkRowIsPageActive.Checked ? "Enable" : "Disable";
                       CheckBox chkRowIsAddActive = (gridRow.Cells[4].FindControl("IsAddActive") as CheckBox);
                       string IsAddActive = chkRowIsAddActive.Checked ? "Enable" : "Disable";
                       CheckBox chkRowIsEditActive = (gridRow.Cells[5].FindControl("IsEditActive") as CheckBox);
                       string IsEditActive = chkRowIsEditActive.Checked ? "Enable" : "Disable";
                       CheckBox chkRowIsDelActive = (gridRow.Cells[6].FindControl("IsDelActive") as CheckBox);
                       string IsDelActive = chkRowIsDelActive.Checked ? "Enable" : "Disable";
                       CheckBox chkRowIsViewActive = (gridRow.Cells[7].FindControl("IsViewActive") as CheckBox);
                       string IsViewActive = chkRowIsViewActive.Checked ? "Enable" : "Disable";
                       CheckBox chkRowIsReportActive = (gridRow.Cells[8].FindControl("IsReportActive") as CheckBox);
                       string IsReportActive = chkRowIsReportActive.Checked ? "Enable" : "Disable";


                       string insert_user = "insert into  NRC_USER_PAGE_PERMISSION (USER_ID, USER_PAGE_ID, IS_PAGE_ACTIVE, IS_ADD_ACTIVE, IS_EDIT_ACTIVE, IS_DELETE_ACTIVE, IS_VIEW_ACTIVE, IS_REPORT_ACTIVE) values ( :NoUserID, :NoUserPageID, :NoIsPageActive, :NoIsAddActive, :NoIsEditActive , :NoIsDelActive, :NoIsViewActive, :NoIsReportActive)";
                        cmdi = new OracleCommand(insert_user, conn);

                        OracleParameter[] objPrm = new OracleParameter[8];
                        objPrm[0] = cmdi.Parameters.Add("NoUserID", Convert.ToInt32(gridRow.Cells[0].Text));
                        objPrm[1] = cmdi.Parameters.Add("NoUserPageID", Convert.ToInt32(gridRow.Cells[1].Text));
                        objPrm[2] = cmdi.Parameters.Add("NoIsPageActive", IsPageActive);
                        objPrm[3] = cmdi.Parameters.Add("NoIsAddActive", IsAddActive);
                        objPrm[4] = cmdi.Parameters.Add("NoIsEditActive", IsEditActive);
                        objPrm[5] = cmdi.Parameters.Add("NoIsDelActive", IsDelActive);
                        objPrm[6] = cmdi.Parameters.Add("NoIsViewActive", IsViewActive);
                        objPrm[7] = cmdi.Parameters.Add("NoIsReportActive", IsReportActive);

                        cmdi.ExecuteNonQuery();  
                     
                  }
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();

     

                alert_box_right.Visible = true;
                alert_box_right.Controls.Add(new LiteralControl("User Data Update successfully"));
                alert_box_right.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearText();
             //   Display();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
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
         

        public void clearTextField(object sender, EventArgs e)
        { 
            DropDownUserID.SelectedIndex = -1;
        }

        public void clearText()
        {  
             
          //  DropDownUserID.SelectedIndex = -1;

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