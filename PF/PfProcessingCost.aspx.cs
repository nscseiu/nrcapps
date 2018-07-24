﻿using System;
using System.Collections;
using System.Configuration;
using System.Data; 
using System.Text;
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
using System.Globalization;


namespace NRCAPPS.PF
{
    public partial class PfProcessingCost : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt;
        int RowCount, ItemCount; 
        string IS_PAGE_ACTIVE   = "";
        string IS_ADD_ACTIVE    = "";
        string IS_EDIT_ACTIVE   = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE   = "";  
 
        public bool IsLoad { get; set; }          
        public DataTable TableData = new DataTable();  
        public DataTable TableData2 = new DataTable();  

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
                        string makeDropDownItemSQL = " SELECT * FROM PF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        dsi = ExecuteBySqlString(makeDropDownItemSQL);
                        dtItemID = (DataTable)dsi.Tables[0];
                        DropDownItemID.DataSource = dtItemID;
                        DropDownItemID.DataValueField = "ITEM_ID";
                        DropDownItemID.DataTextField = "ITEM_NAME";
                        DropDownItemID.DataBind();
                        DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));
                        
                         
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

            if (IS_ADD_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                int userID = Convert.ToInt32(Session["USER_ID"]);   
                string c_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt"); 
                string DataMonthYear = TextMonthYear0.Text; 
                  
                    string get_process_id = "select PF_PROCESSING_COSTID_SEQ.nextval from dual";
                    cmdl = new OracleCommand(get_process_id, conn);
                    int newPorcessCostID = Int16.Parse(cmdl.ExecuteScalar().ToString());
                    string insert_purchase = "insert into  PF_PROCESSING_COST (PROCESSING_COST_ID, ITEM_ID, COST_RATE, MONTH_YEAR, CREATE_DATE, C_USER_ID ) values (:NoProcessCostID, :NoItemID, :NoCostRate, TO_DATE(:TextMonthYear, 'MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_purchase, conn);

                    OracleParameter[] objPrm = new OracleParameter[6];
                    objPrm[0] = cmdi.Parameters.Add("NoProcessCostID", newPorcessCostID);
                    objPrm[1] = cmdi.Parameters.Add("NoItemID", DropDownItemID.Text);
                    objPrm[2] = cmdi.Parameters.Add("NoCostRate", TextItemCostRate.Text);
                    objPrm[3] = cmdi.Parameters.Add("TextMonthYear", DataMonthYear);
                    objPrm[4] = cmdi.Parameters.Add("c_date", c_date);
                    objPrm[5] = cmdi.Parameters.Add("NoCuserID", userID);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();  
                
                 
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Insert new Processing Cost Data successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                 
                clearText();
                Display();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }


        public void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string DataMonthYear = TextMonthYear0.Text;
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                 

                    string update_inven_mas = "update  PF_PROCESSING_COST  set COST_RATE = :NoCostRate, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID =:NoItemID AND To_CHAR(MONTH_YEAR,'mm/yyyy') = :TextMonthYear ";
                    cmdu = new OracleCommand(update_inven_mas, conn);

                    OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                    objPrmInevenMas[0] = cmdu.Parameters.Add("NoItemID", DropDownItemID.Text);
                    objPrmInevenMas[1] = cmdu.Parameters.Add("NoCostRate", TextItemCostRate.Text);
                    objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                    objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                    objPrmInevenMas[4] = cmdu.Parameters.Add("TextMonthYear", DataMonthYear);

                    cmdu.ExecuteNonQuery();
                    cmdu.Parameters.Clear();
                    cmdu.Dispose();
                
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Processing Cost Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearText();
                Display();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        public void GetAllData() //Get all the data and bind it in HTLM Table       
        {
            using (var conn = new OracleConnection(strConnString))  
            {
                const string query = "SELECT * FROM PF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                using (var cmd = new OracleCommand(query, conn))  
                {
                    using (var sda = new OracleDataAdapter())    
                    {  
                        cmd.Connection = conn;  
                        sda.SelectCommand = cmd;  
                        using (TableData)  
                        {  
                            TableData.Clear();  
                            sda.Fill(TableData); 
                            
                        }  
                    }  
                } ItemCount = TableData.Rows.Count;
            }  
        }



      
        

        public void linkSelectClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             LinkButton btn = (LinkButton)sender;
             Session["user_page_data_id"] = btn.CommandArgument;
             string USER_DATA_ID = (Session["user_page_data_id"]).ToString();

             string[] arg = new string[2];
             arg = btn.CommandArgument.ToString().Split(';');
             Session["MONTH_YEAR"] = arg[0];
             Session["ITEM_CODE"] = arg[1];
              
             DataTable dtUserTypeID = new DataTable();
             DataSet ds = new DataSet();
             string makeSQL = " select PI.ITEM_ID, PI.ITEM_NAME, PPC.COST_RATE, TO_CHAR(PPC.MONTH_YEAR, 'mm/yyyy') AS MONTH_YEAR  from PF_PROCESSING_COST PPC LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPC.ITEM_ID where PI.ITEM_CODE = '"+ Session["ITEM_CODE"] + "' AND TO_CHAR(PPC.MONTH_YEAR,'mm/yyyy') = '" + Session["MONTH_YEAR"] + "' order by PI.ITEM_ID asc ";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt); 
             RowCount = dt.Rows.Count;
              

             for (int i = 0; i < dt.Rows.Count; i++)
             {
                 DropDownItemID.Text   = dt.Rows[i]["ITEM_ID"].ToString();
                 TextItemCostRate.Text = dt.Rows[i]["COST_RATE"].ToString();
                 TextMonthYear0.Text   = dt.Rows[i]["MONTH_YEAR"].ToString();  
             }
             
               
             conn.Close();
             Display(); 
             alert_box.Visible = false;
             CheckMonthYear.Text = "";
             
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
                    makeSQL = " SELECT TO_CHAR(PPRC.MONTH_YEAR,'mm/yyyy') AS MONTH_YEAR, PPRC.PROCESSING_COST_ID,  PPRC.ITEM_ID, PI.ITEM_NAME, PI.ITEM_CODE, PPRC.COST_RATE, PPRC.MONTH_YEAR, PPRC.CREATE_DATE, PPRC.UPDATE_DATE FROM PF_PROCESSING_COST PPRC LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRC.ITEM_ID ORDER BY TO_CHAR(PPRC.MONTH_YEAR,'mm/yyyy') desc, PPRC.ITEM_ID asc";
                }
                else
                {
                    makeSQL = " SELECT TO_CHAR(PPRC.MONTH_YEAR,'mm/yyyy') AS MONTH_YEAR, PPRC.PROCESSING_COST_ID,  PPRC.ITEM_ID, PI.ITEM_NAME, PI.ITEM_CODE, PPRC.COST_RATE, PPRC.MONTH_YEAR, PPRC.CREATE_DATE, PPRC.UPDATE_DATE FROM PF_PROCESSING_COST PPRC LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRC.ITEM_ID where TO_CHAR(PPRC.MONTH_YEAR,'mm/yyyy') like '" + txtSearchUserRole.Text + "%' or PI.ITEM_NAME like '" + txtSearchUserRole.Text + "%'  or PI.ITEM_CODE like '" + txtSearchUserRole.Text + "%' ORDER BY TO_CHAR(PPRC.MONTH_YEAR,'mm/yyyy') desc, PPRC.ITEM_ID asc";

                    alert_box.Visible = false;
                }
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "MONTH_YEAR" }; 
                GridView1.DataBind();                
           //     GroupGridView(GridView1.Rows, 0, 1);
                if (dt.Rows.Count > 0)
                {
                    GroupGridView(GridView1.Rows, 0, 1);
                }
                else
                {

                }

                conn.Close();
                //alert_box.Visible = false;
            }
            else
            {
             //   Response.Redirect("~/PagePermissionError.aspx");
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
 
         
         
        public void clearTextField(object sender, EventArgs e)
        {
            
            CheckMonthYear.Text = "";
        }

        public void clearText()
        { 
            CheckMonthYear.Text = "";
            TextItemCostRate.Text = "";
            DropDownItemID.Text = "0";
            TextMonthYear0.Text = "";

        }

        public void TextItem_TextChanged(object sender, EventArgs e)
        {
            TextMonthYear0.Text = "";
            alert_box.Visible = false;
        }

        public void TextMonthYear0_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextMonthYear0.Text))
            {
                alert_box.Visible = false;

                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select MONTH_YEAR from PF_PROCESSING_COST where ITEM_ID = '" + DropDownItemID.Text + "' and TO_CHAR(TO_DATE(MONTH_YEAR), 'mm/YYYY') = '" + TextMonthYear0.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckMonthYear.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Item Cost already insert for this Month</label>";
                    CheckMonthYear.ForeColor = System.Drawing.Color.Red;
                    TextMonthYear0.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckMonthYear.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Item Cost this Month Data is not entry</label>";
                    CheckMonthYear.ForeColor = System.Drawing.Color.Green; 
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else
            {
                CheckMonthYear.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> This Item Cost is not blank</label>";
                CheckMonthYear.ForeColor = System.Drawing.Color.Red;
                TextMonthYear0.Focus();
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