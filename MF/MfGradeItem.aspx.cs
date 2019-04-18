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
using System.Text;

namespace NRCAPPS.MF
{
    public partial class MfGradeItem : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
        OracleDataAdapter oradata;
        DataTable dt, dtg;
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

                        DataTable dtItemID = new DataTable();
                        DataSet dsi = new DataSet();
                        string makeItemSQL = " SELECT ITEM_ID, ITEM_CODE || ' : ' || ITEM_NAME AS ITEM_FULL_NAME FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' AND IS_SALES_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        dsi = ExecuteBySqlString(makeItemSQL);
                        dtItemID = (DataTable)dsi.Tables[0];
                        DropDownGradeID.DataSource = dtItemID;
                        DropDownGradeID.DataValueField = "ITEM_ID";
                        DropDownGradeID.DataTextField = "ITEM_FULL_NAME";
                        DropDownGradeID.DataBind();
                        DropDownGradeID.Items.Insert(0, new ListItem("Select  Grade", "0"));

                        DataTable dtGradeTempID = new DataTable();
                        DataSet dsg = new DataSet();
                        string makeGradeTempSQL = " SELECT MPG.GRADE_ID, MPG.GRADE_ID || ' - ' || MI.ITEM_NAME AS ITEM_FULL_NAME FROM MF_PRODUCTION_GRADE MPG LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID WHERE MPG.IS_ACTIVE = 'Enable' ORDER BY MPG.GRADE_ID DESC";
                        dsg = ExecuteBySqlString(makeGradeTempSQL);
                        dtGradeTempID = (DataTable)dsg.Tables[0];
                        DropDownGradeTempID.DataSource = dtGradeTempID;
                        DropDownGradeTempID.DataValueField = "GRADE_ID";
                        DropDownGradeTempID.DataTextField = "ITEM_FULL_NAME";
                        DropDownGradeTempID.DataBind();
                        DropDownGradeTempID.Items.Insert(0, new ListItem("Select  Grade Template", "0"));

                        DataTable dtSupplierID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeSupplierSQL = " SELECT PARTY_ID, PARTY_ID  || ' - ' || PARTY_NAME || ' - ' || PARTY_VAT_NO AS PARTY_NAME  FROM MF_PARTY WHERE IS_ACTIVE = 'Enable' AND IS_PRODUCTION_ACTIVE = 'Enable' ORDER BY PARTY_NAME ASC";
                        ds = ExecuteBySqlString(makeSupplierSQL);
                        dtSupplierID = (DataTable)ds.Tables[0];
                        DropDownCustomerID.DataSource = dtSupplierID;
                        DropDownCustomerID.DataValueField = "PARTY_ID";
                        DropDownCustomerID.DataTextField = "PARTY_NAME";
                        DropDownCustomerID.DataBind();
                        DropDownCustomerID.Items.Insert(0, new ListItem("Select  Customer", "0"));

                        TextGradeID.Enabled = false;
                        string get_batch_id = " select LAST_NUMBER from all_sequences where sequence_name = 'MF_PRODUCTION_GRADEID_SEQ'";
                        cmdu = new OracleCommand(get_batch_id, conn);
                        int GradeID = Convert.ToInt32(cmdu.ExecuteScalar());
                            GradeID = GradeID + 1;
                        TextGradeID.Text = String.Concat("GD-", GradeID.ToString().PadLeft(4, '0'));

                        string USER_DATA_ID = ""; 
                        ItemDisplay(USER_DATA_ID, new EventArgs());

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
         
        //Write function for Populate Contacts for show in Gridview
        private void ItemDisplay(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
              
            string BatchID = TextGradeID.Text;

            List<ClassVariable> allContacts = null;
       
            string makeSQL = " select  MPGI.*, ITEM_CODE || ' : ' || ITEM_NAME AS ITEM_NAME from MF_PRODUCTION_GRADE_ITEM MPGI LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPGI.ITEM_ID  WHERE MPGI.GRADE_ID = '" + BatchID + "' ORDER BY MPGI.ITEM_ID";
          
            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            Gridview1.DataSource = dt;
              
            List<string> mylist = new List<string>();
            foreach (DataRow row1 in dt.Rows) { mylist.Add(dt.Rows.ToString()); }
            

            if (mylist == null || mylist.Count == 0)
                {
                //trick to show footer when there is no data in the gridview 
                    allContacts = new List<ClassVariable>();
                    allContacts.Add(new ClassVariable()); 
                    Gridview1.DataSource = allContacts;
                    Gridview1.DataBind();
                    Gridview1.Rows[0].Visible = false;
                }
                else
                {
                    Gridview1.DataSource = dt;
                    Gridview1.DataBind();
                }

                //Populate & bind country
                if (Gridview1.Rows.Count > 0)
                {
                    DropDownList dd = (DropDownList)Gridview1.FooterRow.FindControl("DropDownItemID");
                    BindItem(dd, ItemData());
                }
   
        }

        //Function for fetch country from database
        private DataSet ItemData()
        { 

            string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            DataSet ds = new DataSet();
            OracleConnection conn = new OracleConnection(connStr);
             
                conn.Open();
                string sqlString = " SELECT ITEM_ID, ITEM_CODE || ' : ' || ITEM_NAME AS ITEM_NAME FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' AND IS_PRODUCTION_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
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
           
            return ds;
        }

        //function for bind country to dropdown
        private void BindItem(DropDownList DropDownItemID, DataSet ds)
        { 

            DataTable dtCatID = new DataTable();
            DataSet dc = new DataSet();
            string makeDropDownCatSQL = " SELECT ITEM_ID, ITEM_CODE || ' : ' || ITEM_NAME AS ITEM_NAME FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' AND IS_PRODUCTION_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
            dc = ExecuteBySqlString(makeDropDownCatSQL);
            dtCatID = (DataTable)dc.Tables[0];
            DropDownItemID.DataSource = dtCatID;
            DropDownItemID.DataValueField = "ITEM_ID";
            DropDownItemID.DataTextField = "ITEM_NAME";
            DropDownItemID.DataBind();
            DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));
        }

        protected void Check_Item(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            //write code for cascade dropdown
            string ItemID = ((DropDownList)sender).SelectedValue;
            string GradeID = TextGradeID.Text;
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from MF_PRODUCTION_GRADE_ITEM where GRADE_ID = '" + GradeID + "' AND ITEM_ID = '" + ItemID + "'";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            if (!dr.HasRows)
            {
                 
            }
            else {
                var Row = Gridview1.FooterRow;
                DropDownList DropDownItemID = (DropDownList)Row.FindControl("DropDownItemID");
                DropDownItemID.SelectedValue = "0";
                DropDownItemID.Focus(); 
            }
             
        }

        protected void Gridview1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (IS_ADD_ACTIVE == "Enable")
                {
                    //Insert new contact
                    if (e.CommandName == "Insert")
                    {
                        Page.Validate("Add");
                        if (Page.IsValid)
                        {
                            OracleConnection conn = new OracleConnection(strConnString);
                            conn.Open();
                            int userID = Convert.ToInt32(Session["USER_ID"]);
                            string GradeID = TextGradeID.Text;
                            var Row = Gridview1.FooterRow;
                            DropDownList DropDownItemID = (DropDownList)Row.FindControl("DropDownItemID");
                            TextBox TextItemWeight = (TextBox)Row.FindControl("TextItemWeight");

                            //    int ItemID = Convert.ToInt32((TextItemID));
                            //    double ItemWeight = Convert.ToDouble(TextItemWeight);
                            string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = conn;
                            cmd.CommandText = "select * from MF_PRODUCTION_GRADE where GRADE_ID = '" + GradeID + "'";
                            cmd.CommandType = CommandType.Text;
                            OracleDataReader dr = cmd.ExecuteReader();
                            if (!dr.HasRows)
                            {
                                string get_id = "select MF_PRODUCTION_GRADEID_SEQ.nextval from dual";
                                cmdu = new OracleCommand(get_id, conn);
                                int newGradeID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                                string insert_grade = "insert into MF_PRODUCTION_GRADE (GRADE_ID, GRADE_DES, ITEM_ID, REMARKS, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES (:TextGradeID, :TextGradeDes, :TextItemID, :TextRemarks, :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                                cmdi = new OracleCommand(insert_grade, conn);

                                OracleParameter[] objPr = new OracleParameter[7];
                                objPr[0] = cmdi.Parameters.Add("TextGradeID", GradeID);
                                objPr[1] = cmdi.Parameters.Add("TextGradeDes", TextGradeDes.Text);
                                objPr[2] = cmdi.Parameters.Add("TextItemID", DropDownGradeID.Text);
                                objPr[3] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                                objPr[4] = cmdi.Parameters.Add("TextIsActive", ISActive);
                                objPr[5] = cmdi.Parameters.Add("u_date", u_date);
                                objPr[6] = cmdi.Parameters.Add("NoCuserID", userID);
                                cmdi.ExecuteNonQuery();
                                cmdi.Parameters.Clear();
                                cmdi.Dispose();

                                // insert customer id
                                foreach (ListItem li in DropDownCustomerID.Items)
                                {
                                    if (li.Selected == true)
                                    {
                                        string insert_cus = "insert into MF_PRODUCTION_GRADE_CUSTOMER (GRADE_ID, PARTY_ID, CREATE_DATE, C_USER_ID) VALUES (:TextGradeID, :NoPartyID, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                                        cmdi = new OracleCommand(insert_cus, conn);

                                        OracleParameter[] objPrc = new OracleParameter[4];
                                        objPrc[0] = cmdi.Parameters.Add("TextGradeID", GradeID);
                                        objPrc[1] = cmdi.Parameters.Add("NoPartyID", li.Value);
                                        objPrc[2] = cmdi.Parameters.Add("u_date", u_date);
                                        objPrc[3] = cmdi.Parameters.Add("NoCuserID", userID);

                                        cmdi.ExecuteNonQuery();
                                    }
                                }
                                cmdi.Parameters.Clear();
                                cmdi.Dispose();
                            }

                            string get_item_id = "select MF_PROD_GRADE_ITEM_SEQ.nextval from dual";
                            cmdu = new OracleCommand(get_item_id, conn);
                            int newGradeItemID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                            string insert_user = "insert into MF_PRODUCTION_GRADE_ITEM (GRADE_ITEM_ID, GRADE_ID, ITEM_ID, ITEM_WEIGHT, CREATE_DATE, C_USER_ID) VALUES ( :NoGradeItemID, :TextGradeID, :TextItemID, :TextItemWeight, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                            cmdi = new OracleCommand(insert_user, conn);

                            OracleParameter[] objPrm = new OracleParameter[6];
                            objPrm[0] = cmdi.Parameters.Add("NoGradeItemID", newGradeItemID);
                            objPrm[1] = cmdi.Parameters.Add("TextGradeID", GradeID);
                            objPrm[2] = cmdi.Parameters.Add("TextItemID", DropDownItemID.Text);
                            objPrm[3] = cmdi.Parameters.Add("TextItemWeight", TextItemWeight.Text);
                            objPrm[4] = cmdi.Parameters.Add("u_date", u_date);
                            objPrm[5] = cmdi.Parameters.Add("NoCuserID", userID);

                            cmdi.ExecuteNonQuery();
                            cmdi.Parameters.Clear();
                            cmdi.Dispose();

                            conn.Close();

                            alert_box.Visible = true;
                            alert_box.Controls.Add(new LiteralControl("Insert New Item Successfully"));
                            alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                            ItemDisplay(GradeID, new EventArgs());
                            Display();
                        }
                    }
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


        protected void Gridview1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            string GradeID = "";
            //Cancel Edit Mode 
            Gridview1.EditIndex = -1;
            ItemDisplay(GradeID, new EventArgs());
        }

        protected void Gridview1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
        try
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
             
            //Validate Page
            Page.Validate("edit");
            if (!Page.IsValid)
            {
                return;
            }

            string BatchID = TextGradeID.Text;

            //Get TargetItemID
            int GradeItemID = Convert.ToInt32(Gridview1.DataKeys[e.RowIndex]["GRADE_ITEM_ID"]);
            string GradeID = Convert.ToString(Gridview1.DataKeys[e.RowIndex]["GRADE_ID"]);
            //Find Controls  
            TextBox TextItemWeight = (TextBox)Gridview1.Rows[e.RowIndex].FindControl("TextItemWeight");
            DropDownList DropDownItemID = (DropDownList)Gridview1.Rows[e.RowIndex].FindControl("DropDownItemID"); 
              
            int userID = Convert.ToInt32(Session["USER_ID"]);  
            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
            string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

            string update_user = "update  MF_PRODUCTION_GRADE_ITEM  set GRADE_ID = :TextGradeID, ITEM_ID = :TextItemID, ITEM_WEIGHT = :TextItemWeight, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoU_USER_ID where GRADE_ITEM_ID = :NoGradeItemID ";
            cmdi = new OracleCommand(update_user, conn);

            OracleParameter[] objPrm = new OracleParameter[6]; 
            objPrm[0] = cmdi.Parameters.Add("TextGradeID", GradeID);
            objPrm[1] = cmdi.Parameters.Add("TextItemID", DropDownItemID.Text);
            objPrm[2] = cmdi.Parameters.Add("TextItemWeight", TextItemWeight.Text);
            objPrm[3] = cmdi.Parameters.Add("u_date", u_date);
            objPrm[4] = cmdi.Parameters.Add("NoU_USER_ID", userID);
            objPrm[5] = cmdi.Parameters.Add("NoGradeItemID", GradeItemID);

            cmdi.ExecuteNonQuery();
            cmdi.Parameters.Clear();
            cmdi.Dispose();
             
            string update_batch = "update  MF_PRODUCTION_GRADE set GRADE_DES =:TextGradeDes, ITEM_ID =:TextDropDownGradeID, REMARKS =:TextRemarks, IS_ACTIVE =:TextIsActive, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID =:NoU_USER_ID where GRADE_ID =:NoGradeID ";
            cmdi = new OracleCommand(update_batch, conn);

            OracleParameter[] objPr = new OracleParameter[7];
            objPr[0] = cmdi.Parameters.Add("TextGradeDes", TextGradeDes.Text);
            objPr[1] = cmdi.Parameters.Add("TextDropDownGradeID", DropDownGradeID.Text);
            objPr[2] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
            objPr[3] = cmdi.Parameters.Add("TextIsActive", ISActive);
            objPr[4] = cmdi.Parameters.Add("u_date", u_date);
            objPr[5] = cmdi.Parameters.Add("NoU_USER_ID", userID);
            objPr[6] = cmdi.Parameters.Add("NoGradeID", GradeID);

            cmdi.ExecuteNonQuery();
            cmdi.Parameters.Clear();
            cmdi.Dispose();

            // delete customer
            string delete_page = " delete from MF_PRODUCTION_GRADE_CUSTOMER where GRADE_ID  = '" + GradeID + "'"; 
            cmdi = new OracleCommand(delete_page, conn); 
            cmdi.ExecuteNonQuery();
            cmdi.Parameters.Clear();
            cmdi.Dispose();
             
            // insert customer id
            foreach (ListItem li in DropDownCustomerID.Items)
            {
                if (li.Selected == true)
                {  
                    string insert_cus = "insert into MF_PRODUCTION_GRADE_CUSTOMER (GRADE_ID, PARTY_ID, CREATE_DATE, C_USER_ID) VALUES (:TextGradeID, :NoPartyID, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_cus, conn);

                    OracleParameter[] objPrc = new OracleParameter[4];
                    objPrc[0] = cmdi.Parameters.Add("TextGradeID", GradeID);
                    objPrc[1] = cmdi.Parameters.Add("NoPartyID", li.Value); 
                    objPrc[2] = cmdi.Parameters.Add("u_date", u_date);
                    objPrc[3] = cmdi.Parameters.Add("NoCuserID", userID);

                    cmdi.ExecuteNonQuery();
                }
            }
            cmdi.Parameters.Clear();
            cmdi.Dispose();
             
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Grade & Item Data Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
             
                Gridview1.EditIndex = -1;
                ItemDisplay(GradeID, new EventArgs());
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

        protected void Gridview1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //Get Grade ID and Item ID of editable row
            string ItemID = Gridview1.DataKeys[e.NewEditIndex]["ITEM_ID"].ToString();
            string GradeID = Gridview1.DataKeys[e.NewEditIndex]["GRADE_ID"].ToString();
         
            //Open Edit Mode
            Gridview1.EditIndex = e.NewEditIndex;
            ItemDisplay(GradeID, new EventArgs());
            //Populate  Grade and Item and Bind
            DropDownList DropDownItemID = (DropDownList)Gridview1.Rows[e.NewEditIndex].FindControl("DropDownItemID"); 
            if (DropDownItemID != null)
            {
                BindItem(DropDownItemID, ItemData());
                DropDownItemID.SelectedValue = ItemID;  
            }
        }


        protected void Gridview1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
        try
        {
            if (IS_DELETE_ACTIVE == "Enable")
            {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            int GradeItemID = Convert.ToInt32(Gridview1.DataKeys[e.RowIndex]["GRADE_ITEM_ID"]);
            string GradeID = Convert.ToString(Gridview1.DataKeys[e.RowIndex]["GRADE_ID"]);
            string delete_user_page = " delete from MF_PRODUCTION_GRADE_ITEM where GRADE_ITEM_ID  = '" + GradeItemID + "'";

            cmdi = new OracleCommand(delete_user_page, conn);

            cmdi.ExecuteNonQuery();
            cmdi.Parameters.Clear();
            cmdi.Dispose();
            conn.Close();

            alert_box.Visible = true; 
            alert_box.Controls.Add(new LiteralControl("Grade Item Delete Successfully"));
            alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
            
            ItemDisplay(GradeID, new EventArgs());
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
                    makeSQL = " select  MPG.*,  MI.ITEM_NAME from MF_PRODUCTION_GRADE MPG  LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID ORDER BY MPG.GRADE_ID desc";
                }
                else
                {
                    makeSQL = " select  MPG.*,  MI.ITEM_NAME from MF_PRODUCTION_GRADE MPG  LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID  where MPG.GRADE_ID like '" + txtSearchUserRole.Text + "%' or MI.ITEM_NAME like '" + txtSearchUserRole.Text + "%'  or MPG.IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY MPG.GRADE_ID desc";

                    alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataKeyNames = new string[] { "GRADE_ID" };

                GridView2.DataBind();
                conn.Close();
                //alert_box.Visible = false;
            } 
        }

        protected void GridViewSearchUser(object sender, EventArgs e)
        {
            this.Display();
        }

        protected void GridViewUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView2.PageIndex = e.NewPageIndex;
            Display();
            alert_box.Visible = false;
        }


        public void DisplayItemSearch(object sender, EventArgs e)
        {

            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            string USER_DATA_ID = DropDownGradeTempID.Text;

            if (USER_DATA_ID != "")
            {
                string makeSlipSQL = " SELECT  MP.PARTY_ID, MP.PARTY_ID || '-' || MP.PARTY_NAME AS PARTY_NAME FROM MF_PRODUCTION_GRADE_CUSTOMER MPGC LEFT JOIN MF_PARTY MP ON MP.PARTY_ID =  MPGC.PARTY_ID WHERE MPGC.GRADE_ID = '" + USER_DATA_ID + "'  ";
                 
                cmdl = new OracleCommand(makeSlipSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dtg = new DataTable();
                oradata.Fill(dtg); 
                RowCount = dtg.Rows.Count;

                string PartyName = "";  CheckItemSearch.Text = "";
                if (dtg.Rows.Count > 0)
                {

                    CheckItemSearch.Text += "<div class='alert alert-success alert-dismissible'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button><h4> </h4><div style='float:left;padding-right:5px;display:inline;vertical-align: middle;'></div><div style='display:inline;vertical-align: middle;'>";

                for (int i = 0; i < RowCount; i++)
                {
                    PartyName = dtg.Rows[i]["PARTY_NAME"].ToString(); 
                
                    CheckItemSearch.Text  += "<span class=\"label-warning\" >&nbsp;&nbsp; " + PartyName + " &nbsp;&nbsp;</span></br>";
                }
                }
                else
                {

                    CheckItemSearch.Text = "<div class='alert alert-danger alert-dismissible'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button><h4><i class='icon fa fa-ban'></i> Search Result!</h4>Select the Grade Templete.</br></br>";
                 
                }

                CheckItemSearch.Text += "</div></div>";
            }
            else
            { 
                alert_box.Visible = false;
                CheckItemSearch.Text = ""; 
            }
             
            conn.Close();
            alert_box.Visible = false;
           
        }

        protected void DisplayBatchDetalis(object sender, CommandEventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            //Building an HTML string.
            StringBuilder html = new StringBuilder();

            string USER_DATA_ID = e.CommandArgument.ToString();

            string makeGradeSQL = "  SELECT MPG.GRADE_ID, MPG.GRADE_DES, MPG.ITEM_ID, MPG.REMARKS, MPG.IS_ACTIVE, MI.ITEM_NAME, NU.USER_NAME, TO_CHAR(MPG.CREATE_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS CREATE_DATE FROM MF_PRODUCTION_GRADE MPG LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID LEFT JOIN NRC_USER NU ON NU.USER_ID = MPG.C_USER_ID WHERE MPG.GRADE_ID = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeGradeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;
            for (int i = 0; i < RowCount; i++)
            {
                html.Append("<div class='box box-info'><div class='box-header with-border'><h3 class='box-title'>Grade General Information</h3></div><div class='box-body'>");

                html.Append("<table class='table table-hover table-bordered table-striped' cellpadding='0' cellspacing='0' width=100%>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Grade ID </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["GRADE_ID"].ToString());
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Grade </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["ITEM_NAME"].ToString());
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Grade Description </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["GRADE_DES"].ToString());
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Remarks </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["REMARKS"].ToString());
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Is Active </th>");
                html.Append("<td style='text-align:left;'>");

                if (dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable")
                {
                    html.Append("<span class=\"label label-success\" >Enable<span>");
                }
                else { html.Append("<span class=\"label label-danger\" >Disable<span>"); }

                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Created </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["USER_NAME"].ToString());
                html.Append(" <span class=\"label label-info\" > " + dt.Rows[i]["CREATE_DATE"].ToString() + "<span>");
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("</table>");

                html.Append("</div>");
                html.Append("</div>");

            }


            string makeSQL = " SELECT MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME, ITEM_WEIGHT FROM MF_PRODUCTION_GRADE_ITEM MPGI LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPGI.ITEM_ID WHERE MPGI.GRADE_ID = '" + USER_DATA_ID + "' ORDER BY MI.ITEM_ID ";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            GridView5.DataSource = dt;
            GridView5.DataKeyNames = new string[] { "ITEM_NAME" };
            GridView5.DataBind();

            PlaceHolderGradeDetails.Controls.Add(new Literal { Text = html.ToString() });

            conn.Close();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "myModalDetails", "showPopup();", true);

          
        }

        protected void linkSelectClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             LinkButton btn = (LinkButton)sender;
             Session["user_page_data_id"] = btn.CommandArgument;
             string USER_DATA_ID = Convert.ToString(Session["user_page_data_id"]); 
              
             string makeSQL = " SELECT GRADE_ID, GRADE_DES, ITEM_ID, REMARKS, IS_ACTIVE FROM MF_PRODUCTION_GRADE where GRADE_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                TextGradeID.Text = dt.Rows[i]["GRADE_ID"].ToString(); 
                TextGradeDes.Text = dt.Rows[i]["GRADE_DES"].ToString();
                DropDownGradeID.Text = dt.Rows[i]["ITEM_ID"].ToString();
                TextRemarks.Text = dt.Rows[i]["REMARKS"].ToString(); 
                CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
                         
             }

            string makeSlipSQL = " SELECT  MP.PARTY_ID, MP.PARTY_ID || '-' || MP.PARTY_NAME AS PARTY_NAME FROM MF_PRODUCTION_GRADE_CUSTOMER MPGC LEFT JOIN MF_PARTY MP ON MP.PARTY_ID =  MPGC.PARTY_ID WHERE MPGC.GRADE_ID = '" + USER_DATA_ID + "'  ";

            cmdl = new OracleCommand(makeSlipSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;
            foreach (ListItem li in DropDownCustomerID.Items)
            {
                li.Selected = false;
                for (int i = 0; i < RowCount; i++)
                {
                    if (li.Value == dt.Rows[i]["PARTY_ID"].ToString())
                    {
                        li.Selected = true;
                    }
                }
            }

            conn.Close(); 
            alert_box.Visible = false; 
            ItemDisplay(TextGradeID.Text, new EventArgs());

        }
          
        public void ClearTextField(object sender, EventArgs e)
        {
            TextGradeID.Text = ""; 
            
        }

        public void clearText()
        {
            TextGradeID.Text = "";
            TextGradeID.Text = "";  
          //  CheckItemCategoreyName.Text = "";  

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