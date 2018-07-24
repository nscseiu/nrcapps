using System;
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
    public partial class PfBusinessTargetMatold : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt;
        int RowCount, ItemCount;
        TextBox[] TextItemID;  
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
                        // open data in html 
                        GetAllData();

                         
                         
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
               
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");


                int len = ItemCount;
                for (int i = 0; i < len; i++)
                {

                    string get_process_id = "select PF_PROCESSING_COSTID_SEQ.nextval from dual";
                    cmdl = new OracleCommand(get_process_id, conn);
                    int newPorcessCostID = Int16.Parse(cmdl.ExecuteScalar().ToString());
                    string insert_purchase = "insert into  PF_PROCESSING_COST (PROCESSING_COST_ID, ITEM_ID, COST_RATE, MONTH_YEAR, CREATE_DATE, C_USER_ID ) values (:NoProcessCostID, :NoItemID, :NoCostRate, TO_DATE(:TextMonthYear, 'MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_purchase, conn);

                    OracleParameter[] objPrm = new OracleParameter[6];
                    objPrm[0] = cmdi.Parameters.Add("NoProcessCostID", newPorcessCostID);
                    objPrm[1] = cmdi.Parameters.Add("NoItemID", i+1);
                    objPrm[2] = cmdi.Parameters.Add("NoCostRate", TextItemID[i].Text);
                    objPrm[3] = cmdi.Parameters.Add("TextMonthYear", DataMonthYear);
                    objPrm[4] = cmdi.Parameters.Add("c_date", c_date);
                    objPrm[5] = cmdi.Parameters.Add("NoCuserID", userID);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();  
                }
                 
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
             

             DataTable dtUserTypeID = new DataTable();
             DataSet ds = new DataSet();
             string makeSQL = " select PI.ITEM_ID, PI.ITEM_NAME, PPC.COST_RATE, TO_CHAR(PPC.MONTH_YEAR, 'mm/yyyy') AS MONTH_YEAR  from PF_PROCESSING_COST PPC LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPC.ITEM_ID where TO_CHAR(PPC.MONTH_YEAR,'mm/yyyy') = '" + USER_DATA_ID + "' order by PI.ITEM_ID asc ";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt); 
             RowCount = dt.Rows.Count;

             //Building an HTML string.
             StringBuilder html = new StringBuilder();
             StringBuilder dynTxtBoxs = new StringBuilder();

             TextBox[] TextMonthYear = new TextBox[] { TextMonthYear0 };

             for (int i = 0; i < dt.Rows.Count; i++)
             {
                     Label lbl = new Label();
                   
                     lbl.CssClass = "col-sm-2 control-label";
                     lbl.Text = dt.Rows[i]["ITEM_NAME"].ToString();
                     TextBox txtNewTextBox = new TextBox();
                     txtNewTextBox.ID = "TextItemID"+i;
                     txtNewTextBox.Text = decimal.Parse(dt.Rows[i]["COST_RATE"].ToString()).ToString("0.00");
                    
                       pnlContainer.Controls.Add(lbl);
                       pnlContainer.Controls.Add(txtNewTextBox);  
                   
                    //     dynTxtBoxs.Append(@"<div class='form-group'>");
                     //    dynTxtBoxs.Append(@"<label class='col-sm-2 control-label'>" + dt.Rows[i]["ITEM_ID"].ToString() +". "+ dt.Rows[i]["ITEM_NAME"].ToString() + ""); 
                     //    dynTxtBoxs.Append(@"</label>");
                    //     dynTxtBoxs.Append(@"<div class='col-sm-2'>");   
                    //     dynTxtBoxs.Append(@"<div class='input-group'>"); 
                     //    dynTxtBoxs.Append(@"<input type='Text' runat='server' class='form-control input-sm'  id='TextItemID" + i + "' value='"+ decimal.Parse(dt.Rows[i]["COST_RATE"].ToString()).ToString("0.00") + "' />");
                     //    dynTxtBoxs.Append(@"<span class='input-group-addon'>.00</span>");      
                     //    dynTxtBoxs.Append(@"</div>");
                     //    dynTxtBoxs.Append(@"</div>");
                     //    dynTxtBoxs.Append(@"</div>");
                     //    dynamicContent.InnerHtml = Convert.ToString(dynTxtBoxs);
                     
                  
             }
             for (int i = 0; i < 1; i++)
             { 
                 TextMonthYear[i].Text = dt.Rows[i]["MONTH_YEAR"].ToString();
             }

          
               
             conn.Close();
             Display(); 
             alert_box.Visible = false;
             CheckMonthYear.Text = "";
             
        }

        private DataTable GetAllDataSelect(string USER_DATA_ID) //Get all the data and bind it in HTLM Table       
        {
            using (var conn = new OracleConnection(strConnString))
            {
                string query = " select ITEM_ID, COST_RATE, TO_CHAR(MONTH_YEAR, 'mm/yyyy') AS MONTH_YEAR  from PF_PROCESSING_COST where TO_CHAR(MONTH_YEAR,'mm/yyyy') = '" + USER_DATA_ID + "' order by ITEM_ID asc ";
             
                using (var cmd = new OracleCommand(query, conn))
                {
                    using (var sda = new OracleDataAdapter())
                    {
                        cmd.Connection = conn;
                        sda.SelectCommand = cmd;
                        using (TableData2)
                        { 
                            TableData.Clear();
                            sda.Fill(TableData2);
                        }
                    }
                } return TableData2;
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
                    makeSQL = " SELECT TO_CHAR(PPRC.MONTH_YEAR,'mm/yyyy') AS MONTH_YEAR, PPRC.PROCESSING_COST_ID,  PPRC.ITEM_ID, PI.ITEM_NAME, PI.ITEM_CODE, PPRC.COST_RATE, PPRC.MONTH_YEAR, PPRC.CREATE_DATE, PPRC.UPDATE_DATE FROM PF_PROCESSING_COST PPRC LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRC.ITEM_ID ORDER BY TO_CHAR(PPRC.MONTH_YEAR,'mm/yyyy') desc, PPRC.ITEM_ID asc";
                }
                else
                {
                    makeSQL = " select  * from PF_FG_STOCK_INVENTORY_MASTER where CUSTOMER_ID like '" + txtSearchUserRole.Text + "%' or CUSTOMER_NAME like '" + txtSearchUserRole.Text + "%'  or IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY UPDATE_DATE desc, CREATE_DATE desc";

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
 

        public void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string DataMonthYear = TextMonthYear0.Text; 
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                 

                int len = ItemCount; 
                 for (int i = 0; i < 10; i++)
                {
                   string ItemRate = TextItemID[i].Text;

                    string update_inven_mas = "update  PF_PROCESSING_COST  set COST_RATE = :NoCostRate, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID =:NoItemID AND To_CHAR(MONTH_YEAR,'mm/yyyy') = :TextMonthYear ";
                    cmdu = new OracleCommand(update_inven_mas, conn);

                    OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                    objPrmInevenMas[0] = cmdu.Parameters.Add("NoItemID", i+1);
                    objPrmInevenMas[1] = cmdu.Parameters.Add("NoCostRate", Convert.ToDouble(ItemRate)); 
                    objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                    objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                    objPrmInevenMas[4] = cmdu.Parameters.Add("TextMonthYear", DataMonthYear); 

                    cmdu.ExecuteNonQuery();
                    cmdu.Parameters.Clear();
                    cmdu.Dispose();
                }

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Processing Cost Data Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
                Display();
            }
                else { 
                    Response.Redirect("~/PagePermissionError.aspx");
                }
        }

         
         
        public void clearTextField(object sender, EventArgs e)
        {
            
            CheckMonthYear.Text = "";
        }

        public void clearText()
        {
           
            CheckMonthYear.Text = "";

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
                cmd.CommandText = "select MONTH_YEAR from PF_PROCESSING_COST where TO_CHAR(TO_DATE(MONTH_YEAR), 'mm/YYYY') = '" + TextMonthYear0.Text + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckMonthYear.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Month Data is already entry</label>";
                    CheckMonthYear.ForeColor = System.Drawing.Color.Red;
                    TextMonthYear0.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                }
                else
                {
                    CheckMonthYear.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Month Data is not entry</label>";
                    CheckMonthYear.ForeColor = System.Drawing.Color.Green; 
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                }
            }
            else
            {
                CheckMonthYear.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Customer name is not blank</label>";
                CheckMonthYear.ForeColor = System.Drawing.Color.Red;
                TextMonthYear0.Focus();
            }

        } 
   }
}