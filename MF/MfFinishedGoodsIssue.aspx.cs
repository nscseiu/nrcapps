using System; 
using System.Configuration;
using System.Data; 
using System.Web.UI; 
using System.Web.UI.WebControls; 
using System.Data.OracleClient;
using System.IO; 
using System.Collections.Generic; 
using System.Data.SqlClient;
using System.Text;
using System.Globalization;

namespace NRCAPPS.MF
{
    public partial class MfFinishedGoodsIssue : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand cmdu, cmdi, cmdl;
        OracleDataAdapter oradata;
        DataTable dt;
        int RowCount;

        string IS_PAGE_ACTIVE = "", IS_ADD_ACTIVE = "", IS_EDIT_ACTIVE = "", IS_DELETE_ACTIVE = "", IS_VIEW_ACTIVE = "", IS_REPORT_ACTIVE = "", IS_PRINT_ACTIVE = "";

        public bool IsLoad { get; set; } 
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

                        DataTable dtBatchID = new DataTable();
                        DataSet db = new DataSet();
                        string makeDropDownBatchSQL = "  SELECT MPBM.BATCH_ID, MPBM.BATCH_NO FROM MF_PRODUCTION_BATCH_MASTER MPBM LEFT JOIN MF_PRODUCTION_ISSUE_FG MPIF ON MPIF.BATCH_NO = MPBM.BATCH_NO WHERE MPBM.PRODUCTION_ISSUE_S = 'Complete' AND MPIF.BATCH_NO IS NULL ORDER BY MPBM.BATCH_ID DESC ";
                        db = ExecuteBySqlString(makeDropDownBatchSQL);
                        dtBatchID = (DataTable)db.Tables[0];
                        DropDownBatchNo.DataSource = dtBatchID;
                        DropDownBatchNo.DataValueField = "BATCH_ID";
                        DropDownBatchNo.DataTextField = "BATCH_NO";
                        DropDownBatchNo.DataBind();
                        DropDownBatchNo.Items.Insert(0, new ListItem("Select  Batch", "0"));


                        DataTable dtBatch1ID = new DataTable();
                        DataSet dbd = new DataSet();
                        string makeDropDownBatch1SQL = "  SELECT MPBM.BATCH_ID, MPBM.BATCH_NO FROM MF_PRODUCTION_BATCH_MASTER MPBM LEFT JOIN MF_PRODUCTION_ISSUE_FG MPIF ON MPIF.BATCH_NO = MPBM.BATCH_NO WHERE MPBM.PRODUCTION_ISSUE_S = 'Complete' ORDER BY MPBM.BATCH_ID DESC ";
                        dbd = ExecuteBySqlString(makeDropDownBatch1SQL);
                        dtBatch1ID = (DataTable)dbd.Tables[0];
                        DropDownBatchNo1.DataSource = dtBatch1ID;
                        DropDownBatchNo1.DataValueField = "BATCH_ID";
                        DropDownBatchNo1.DataTextField = "BATCH_NO";
                        DropDownBatchNo1.DataBind();
                        DropDownBatchNo1.Items.Insert(0, new ListItem("Select  Batch", "0"));


                        DataTable dtFurID = new DataTable();
                        DataSet dc = new DataSet();
                        string makeDropDownFurSQL = " SELECT FURNACES_ID,  FURNACES_ID || ' - ' || FURNACES_NAME || ' - ' || FURNACES_DESCRIPTION  AS FURNACES_NAME FROM MF_PRODUCTION_FURNACES WHERE IS_ACTIVE = 'Enable' ORDER BY FURNACES_NAME ASC";
                        dc = ExecuteBySqlString(makeDropDownFurSQL);
                        dtFurID = (DataTable)dc.Tables[0];
                        DropDownFurnacesID.DataSource = dtFurID;
                        DropDownFurnacesID.DataValueField = "FURNACES_ID";
                        DropDownFurnacesID.DataTextField = "FURNACES_NAME";
                        DropDownFurnacesID.DataBind();
                        DropDownFurnacesID.Items.Insert(0, new ListItem("Select  Furnaces", "0"));
                         
                        DataTable dtSupplierID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeSupplierSQL = " SELECT PARTY_ID, PARTY_ID  || ' - ' || PARTY_NAME || ' - ' || PARTY_VAT_NO AS PARTY_NAME  FROM MF_PARTY WHERE IS_ACTIVE = 'Enable' AND IS_PRODUCTION_ACTIVE = 'Enable' ORDER BY PARTY_NAME ASC";
                        ds = ExecuteBySqlString(makeSupplierSQL);
                        dtSupplierID = (DataTable)ds.Tables[0];
                        DropDownSupplierID.DataSource = dtSupplierID;
                        DropDownSupplierID.DataValueField = "PARTY_ID";
                        DropDownSupplierID.DataTextField = "PARTY_NAME";
                        DropDownSupplierID.DataBind();
                        DropDownSupplierID.Items.Insert(0, new ListItem("Select  Customer", "0"));
                         

                        string get_batch_id = " select LAST_NUMBER from all_sequences where sequence_name = 'MF_PROD_ISSUE_FG_SEQ'";
                        cmdu = new OracleCommand(get_batch_id, conn);
                        int IssueFgID = Convert.ToInt32(cmdu.ExecuteScalar().ToString());
                        TextIssueFgID.Text = Convert.ToString(IssueFgID + 1);
                         

                        alert_box.Visible = false;


                        DropDownFurnacesID.Enabled = false;
                        DropDownSupplierID.Enabled = false;
                        DropDownGradeID.Enabled = false;
                        EntryDate.Enabled = false;
                       
                        string USER_DATA_ID = ""; 
                        ItemDisplay(USER_DATA_ID, new EventArgs());

                        Display();
                        DisplayProductionIssue();
                        BatchPendingProduction();
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
       

        private void ItemDisplay(object sender, EventArgs e)
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                string BatchNo = DropDownBatchNo.SelectedItem.Text;

                List<ClassVariable> allContacts = null;

                string makeSQL = " select  row_number() OVER (ORDER BY MI.ITEM_ID) AS SL_NO, MPII.*, ITEM_CODE || ' : ' || ITEM_NAME AS ITEM_NAME from MF_PRODUCTION_ISSUE_FG_ITEM MPII LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPII.ITEM_ID  WHERE MPII.BATCH_NO = '" + BatchNo + "' ORDER BY MI.ITEM_ID ";

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
                 
                //Populate & bind item
                if (Gridview1.Rows.Count > 0)
                {
                    DropDownList dd = (DropDownList)Gridview1.FooterRow.FindControl("DropDownItemID"); 
                    BindItem(dd, ItemData());
                }
            }  

        }

        //Function for fetch item from database
        private DataSet ItemData()
        {  
            string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            DataSet ds = new DataSet();
            OracleConnection conn = new OracleConnection(connStr);
             
                conn.Open();
                string sqlString = " SELECT MI.ITEM_ID || '-' || NINI.SHORT_CODE || '-' || MI.NATURE_ITEM_ID AS ITEM_ID, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME FROM MF_ITEM MI LEFT JOIN NRC_INVENTORY_NATURE_ITEM NINI ON NINI.NATURE_ITEM_ID = MI.NATURE_ITEM_ID WHERE MI.IS_ACTIVE = 'Enable' AND MI.IS_SALES_ACTIVE = 'Enable' ORDER BY MI.ITEM_ID ASC";
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
        private void BindItem(DropDownList DropDownItemID,  DataSet ds)
        { 

            DataTable dtCatID = new DataTable();
            DataSet dc = new DataSet();
            string makeDropDownCatSQL = " SELECT MI.ITEM_ID || '-' || NINI.SHORT_CODE || '-' || MI.NATURE_ITEM_ID AS ITEM_ID, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME FROM MF_ITEM MI LEFT JOIN NRC_INVENTORY_NATURE_ITEM NINI ON NINI.NATURE_ITEM_ID = MI.NATURE_ITEM_ID WHERE MI.IS_ACTIVE = 'Enable' AND MI.IS_SALES_ACTIVE = 'Enable' ORDER BY MI.ITEM_ID ASC";
            dc = ExecuteBySqlString(makeDropDownCatSQL);
            dtCatID = (DataTable)dc.Tables[0];
            DropDownItemID.DataSource = dtCatID;
            DropDownItemID.DataValueField = "ITEM_ID";
            DropDownItemID.DataTextField = "ITEM_NAME";
            DropDownItemID.DataBind();
            DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));

            
          //  DropDownTypeID.Items.Insert(0, new ListItem("Select  Item", "0"));
        }

   
        protected void Check_First_Wt(object sender, EventArgs e)
        {   
            var Row = Gridview1.FooterRow; 
            TextBox TextFirstWeight = (TextBox)Row.FindControl("TextFirstWeight");
            TextBox TextSecondWeight = (TextBox)Row.FindControl("TextSecondWeight");
            TextBox TextItemWeight = (TextBox)Row.FindControl("TextItemWeight");
            double FirstWeight = Convert.ToDouble(TextFirstWeight.Text);
            double SecondWeight = Convert.ToDouble(TextSecondWeight.Text);

            if (FirstWeight <= 0 || FirstWeight <= SecondWeight)
            { 
                TextFirstWeight.Text = "0";
                TextFirstWeight.Focus();
                TextItemWeight.Text = "";
            }
            else { 
                double NetWeight = FirstWeight - SecondWeight;
                TextItemWeight.Text = NetWeight.ToString();
                TextSecondWeight.Focus();
            }
         
            TextItemWeight.Enabled = false; 
            alert_box.Visible = false;

        }

   
        protected void Check_Second_Wt(object sender, EventArgs e)
        {   
            var Row = Gridview1.FooterRow; 
            TextBox TextFirstWeight = (TextBox)Row.FindControl("TextFirstWeight");
            TextBox TextSecondWeight = (TextBox)Row.FindControl("TextSecondWeight");
            TextBox TextItemWeight = (TextBox)Row.FindControl("TextItemWeight");
            double FirstWeight = Convert.ToDouble(TextFirstWeight.Text);
            double SecondWeight = Convert.ToDouble(TextSecondWeight.Text);

            if (SecondWeight <= 0 || FirstWeight <= SecondWeight)
            {
                TextSecondWeight.Text = "0";
                TextSecondWeight.Focus();
                TextItemWeight.Text = "";
            }
            else {
                
                double NetWeight = FirstWeight - SecondWeight;
                TextItemWeight.Text = NetWeight.ToString();
               // btnInsert.Focus();
            }
         
            TextItemWeight.Enabled = false; 
            alert_box.Visible = false;

        }

        protected void Gridview1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
        // try { 
            if (IS_ADD_ACTIVE == "Enable")
            { 
                if (e.CommandName == "Insert")
                {
                    Page.Validate("Add");
                    if (Page.IsValid)
                    {
                            OracleConnection conn = new OracleConnection(strConnString);
                            conn.Open();
                            int userID = Convert.ToInt32(Session["USER_ID"]);
                            string BatchNumber = DropDownBatchNo.SelectedItem.Text;
                            int ActualGradeID = Convert.ToInt32(DropDownActualGradeID.Text);
                            int IssueFgID = Convert.ToInt32(TextIssueFgID.Text);
                    
                            var Row = Gridview1.FooterRow;
                            DropDownList DropDownItemID = (DropDownList)Row.FindControl("DropDownItemID"); 
                            TextBox TextItemWeight = (TextBox)Row.FindControl("TextItemWeight");
                            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                            double ItemWeight = Convert.ToDouble(TextItemWeight.Text);   
                            string ItemDataID = Convert.ToString(DropDownItemID.Text);
                            string[] ItemID = ItemDataID.Split('-');

                        //inventory calculation

                        int InvenItemID = 0;
                        double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, FinalStock = 0.00, StockInWetNew = 0.00, FinalStockNew = 0.00;

                        if (ItemID[1] == "FG")
                        {
                            // check inventory FG master
                            string makeRmInSQL = " select * from MF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID[0] + "' ";
                            cmdl = new OracleCommand(makeRmInSQL);
                            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                            dt = new DataTable();
                            oradata.Fill(dt);
                            RowCount = dt.Rows.Count;


                            for (int i = 0; i < RowCount; i++)
                            {
                                InvenItemID = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                                InitialStock = Convert.ToDouble(dt.Rows[i]["INITIAL_STOCK_WT"].ToString());
                                StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                                StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString());
                                FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                            }

                            if (InvenItemID != 0)
                            {

                                StockInWetNew = StockInWet + ItemWeight;
                                FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;

                                if (0 < RowCount)
                                {

                                    string update_inven_mas = "update  MF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                                    cmdu = new OracleCommand(update_inven_mas, conn);

                                    OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                                    objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                                    objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                                    objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                                    objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                                    objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);

                                    cmdu.ExecuteNonQuery();
                                    cmdu.Parameters.Clear();
                                    cmdu.Dispose();
                                }

                                 
                                alert_box.Visible = true;
                                alert_box.Controls.Add(new LiteralControl("Insert New Issue For Finished Goods Data Successfully"));
                                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                                // Finished goods Item
                                string get_item_cat_id = "select MF_PROD_ISSUE_FG_ITEM_SEQ.nextval from dual";
                                cmdu = new OracleCommand(get_item_cat_id, conn);
                                int newIssueItemID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                                string insert_user = "insert into MF_PRODUCTION_ISSUE_FG_ITEM (ISSUE_FG_ITEM_ID, BATCH_NO, ISSUE_FG_ID, ITEM_ID, ITEM_WEIGHT, CREATE_DATE, C_USER_ID) VALUES ( :NoIssueFgItemID, :TextBatchNo, :TextIssueFgID, :TextItemID, :TextItemWeight, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                                cmdi = new OracleCommand(insert_user, conn);

                                OracleParameter[] objPrm = new OracleParameter[7];
                                objPrm[0] = cmdi.Parameters.Add("NoIssueFgItemID", newIssueItemID);
                                objPrm[1] = cmdi.Parameters.Add("TextIssueFgID", IssueFgID);
                                objPrm[2] = cmdi.Parameters.Add("TextBatchNo", BatchNumber);
                                objPrm[3] = cmdi.Parameters.Add("TextItemID", ItemID[0]);
                                objPrm[4] = cmdi.Parameters.Add("TextItemWeight", ItemWeight);
                                objPrm[5] = cmdi.Parameters.Add("u_date", u_date);
                                objPrm[6] = cmdi.Parameters.Add("NoCuserID", userID);


                                cmdi.ExecuteNonQuery();
                                cmdi.Parameters.Clear();
                                cmdi.Dispose();
                              

                                // Finished goods master
                                OracleCommand cmd = new OracleCommand();
                                cmd.Connection = conn;
                                cmd.CommandText = "select * from MF_PRODUCTION_ISSUE_FG where BATCH_NO = '" + BatchNumber + "'";
                                cmd.CommandType = CommandType.Text;
                                OracleDataReader dr = cmd.ExecuteReader();
                                if (!dr.HasRows)
                                {
                                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                                    string MakeEntryDate = EntryDate.Text;
                                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');
                                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                                    string get_id = "select MF_PROD_ISSUE_FG_SEQ.nextval from dual";
                                    cmdu = new OracleCommand(get_id, conn);
                                    int newProdID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                                    string insert_grade = "insert into MF_PRODUCTION_ISSUE_FG (ISSUE_FG_ID, BATCH_NO, ACTUAL_GRADE_ID, ENTRY_DATE, REMARKS, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES (:TextProdID, :TextBatchNo, :TextActualGradeID, TO_DATE(:EntryDate, 'DD/MM/YYYY'), :TextRemarks, :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                                    cmdi = new OracleCommand(insert_grade, conn);

                                    OracleParameter[] objPr = new OracleParameter[8];
                                    objPr[0] = cmdi.Parameters.Add("TextProdID", newProdID);
                                    objPr[1] = cmdi.Parameters.Add("TextBatchNo", BatchNumber);
                                    objPr[2] = cmdi.Parameters.Add("TextActualGradeID", ActualGradeID);
                                    objPr[3] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                                    objPr[4] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                                    objPr[5] = cmdi.Parameters.Add("TextIsActive", ISActive);
                                    objPr[6] = cmdi.Parameters.Add("u_date", u_date);
                                    objPr[7] = cmdi.Parameters.Add("NoCuserID", userID);

                                    cmdi.ExecuteNonQuery();
                                    cmdi.Parameters.Clear();
                                    cmdi.Dispose();

                                    string update_batch = "update  MF_PRODUCTION_BATCH_MASTER  set ISSUED_FG_S = :TextBatchStatus,  ISSUED_FG_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , ISSUED_FG_S_ID = :NoU_USER_ID where BATCH_NO = :NoBatchNo ";
                                    cmdi = new OracleCommand(update_batch, conn);

                                    OracleParameter[] objPrp = new OracleParameter[4];
                                    objPrp[0] = cmdi.Parameters.Add("TextBatchStatus", "Ongoing");
                                    objPrp[1] = cmdi.Parameters.Add("u_date", u_date);
                                    objPrp[2] = cmdi.Parameters.Add("NoU_USER_ID", userID);
                                    objPrp[3] = cmdi.Parameters.Add("NoBatchNo", BatchNumber);

                                    cmdi.ExecuteNonQuery();
                                    cmdi.Parameters.Clear();
                                    cmdi.Dispose();

                                }



                                Display();
                                DisplayProductionIssue();
                                BatchPendingProduction();
                                ItemDisplay(IssueFgID, new EventArgs());

                            }
                            else
                            {
                                alert_box.Visible = true;
                                alert_box.Controls.Add(new LiteralControl("Item is not available in the Finished Goods Inventory, ID: " + InvenItemID));
                                alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                            }
                        }
                        else {

                            // check inventory RM master
                            string makeRmInSQL = " select * from MF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID[0] + "' ";
                            cmdl = new OracleCommand(makeRmInSQL);
                            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                            dt = new DataTable();
                            oradata.Fill(dt);
                            RowCount = dt.Rows.Count;


                            for (int i = 0; i < RowCount; i++)
                            {
                                InvenItemID = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                                InitialStock = Convert.ToDouble(dt.Rows[i]["INITIAL_STOCK_WT"].ToString());
                                StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                                StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString());
                                FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                            }

                            if (InvenItemID != 0)
                            {

                                StockInWetNew = StockInWet + ItemWeight;
                                FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;

                                if (0 < RowCount)
                                {

                                    string update_inven_mas = "update  MF_RM_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                                    cmdu = new OracleCommand(update_inven_mas, conn);

                                    OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                                    objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                                    objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                                    objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                                    objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                                    objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);

                                    cmdu.ExecuteNonQuery();
                                    cmdu.Parameters.Clear();
                                    cmdu.Dispose();
                                }


                                alert_box.Visible = true;
                                alert_box.Controls.Add(new LiteralControl("Insert New Issue For Recyclable Material Data Successfully"));
                                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                                // Finished goods Item
                                string get_item_cat_id = "select MF_PROD_ISSUE_FG_ITEM_SEQ.nextval from dual";
                                cmdu = new OracleCommand(get_item_cat_id, conn);
                                int newIssueItemID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                                string insert_user = "insert into MF_PRODUCTION_ISSUE_FG_ITEM (ISSUE_FG_ITEM_ID, BATCH_NO, ISSUE_FG_ID, ITEM_ID, ITEM_WEIGHT, CREATE_DATE, C_USER_ID) VALUES ( :NoIssueFgItemID, :TextBatchNo, :TextIssueFgID, :TextItemID, :TextItemWeight, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                                cmdi = new OracleCommand(insert_user, conn);

                                OracleParameter[] objPrm = new OracleParameter[7];
                                objPrm[0] = cmdi.Parameters.Add("NoIssueFgItemID", newIssueItemID);
                                objPrm[1] = cmdi.Parameters.Add("TextIssueFgID", IssueFgID);
                                objPrm[2] = cmdi.Parameters.Add("TextBatchNo", BatchNumber);
                                objPrm[3] = cmdi.Parameters.Add("TextItemID", ItemID[0]);
                                objPrm[4] = cmdi.Parameters.Add("TextItemWeight", ItemWeight);
                                objPrm[5] = cmdi.Parameters.Add("u_date", u_date);
                                objPrm[6] = cmdi.Parameters.Add("NoCuserID", userID);


                                cmdi.ExecuteNonQuery();
                                cmdi.Parameters.Clear();
                                cmdi.Dispose();
                               

                                // Finished goods master
                                OracleCommand cmd = new OracleCommand();
                                cmd.Connection = conn;
                                cmd.CommandText = "select * from MF_PRODUCTION_ISSUE_FG where BATCH_NO = '" + BatchNumber + "'";
                                cmd.CommandType = CommandType.Text;
                                OracleDataReader dr = cmd.ExecuteReader();
                                if (!dr.HasRows)
                                {
                                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                                    string MakeEntryDate = EntryDate.Text;
                                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');
                                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                                    string get_id = "select MF_PROD_ISSUE_FG_SEQ.nextval from dual";
                                    cmdu = new OracleCommand(get_id, conn);
                                    int newProdID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                                    string insert_grade = "insert into MF_PRODUCTION_ISSUE_FG (ISSUE_FG_ID, BATCH_NO, ACTUAL_GRADE_ID, ENTRY_DATE, REMARKS, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES (:TextProdID, :TextBatchNo, :TextActualGradeID, TO_DATE(:EntryDate, 'DD/MM/YYYY'), :TextRemarks, :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                                    cmdi = new OracleCommand(insert_grade, conn);

                                    OracleParameter[] objPr = new OracleParameter[8];
                                    objPr[0] = cmdi.Parameters.Add("TextProdID", newProdID);
                                    objPr[1] = cmdi.Parameters.Add("TextBatchNo", BatchNumber);
                                    objPr[2] = cmdi.Parameters.Add("TextActualGradeID", ActualGradeID);
                                    objPr[3] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                                    objPr[4] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                                    objPr[5] = cmdi.Parameters.Add("TextIsActive", ISActive);
                                    objPr[6] = cmdi.Parameters.Add("u_date", u_date);
                                    objPr[7] = cmdi.Parameters.Add("NoCuserID", userID);

                                    cmdi.ExecuteNonQuery();
                                    cmdi.Parameters.Clear();
                                    cmdi.Dispose();

                                    string update_batch = "update  MF_PRODUCTION_BATCH_MASTER  set ISSUED_FG_S = :TextBatchStatus,  ISSUED_FG_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , ISSUED_FG_S_ID = :NoU_USER_ID where BATCH_NO = :NoBatchNo ";
                                    cmdi = new OracleCommand(update_batch, conn);

                                    OracleParameter[] objPrp = new OracleParameter[4];
                                    objPrp[0] = cmdi.Parameters.Add("TextBatchStatus", "Ongoing");
                                    objPrp[1] = cmdi.Parameters.Add("u_date", u_date);
                                    objPrp[2] = cmdi.Parameters.Add("NoU_USER_ID", userID);
                                    objPrp[3] = cmdi.Parameters.Add("NoBatchNo", BatchNumber);

                                    cmdi.ExecuteNonQuery();
                                    cmdi.Parameters.Clear();
                                    cmdi.Dispose();

                                }

                                conn.Close();
                                Display();
                                DisplayProductionIssue();
                                BatchPendingProduction();
                                ItemDisplay(IssueFgID, new EventArgs());

                            }
                            else
                            {
                                alert_box.Visible = true;
                                alert_box.Controls.Add(new LiteralControl("Item is not available in the Recyclable Material Inventory, ID: " + InvenItemID));
                                alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                            }


                        }

                         


                        }
                }
            }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
          //  }
         //   catch
         //   {
        //        Response.Redirect("~/ParameterError.aspx");
        //    }

        }

        protected void Gridview1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            string BatchNumber = "";
            //Cancel Edit Mode 
            Gridview1.EditIndex = -1;
            ItemDisplay(BatchNumber, new EventArgs());
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

            string BatchID = TextIssueFgID.Text;

            //Get TargetItemID
            int TargetItemID = Convert.ToInt32(Gridview1.DataKeys[e.RowIndex]["TARGET_ITEM_ID"]);
            string BatchNumber = Convert.ToString(Gridview1.DataKeys[e.RowIndex]["BATCH_NO"]);
            //Find Controls  
            TextBox TextItemWeight = (TextBox)Gridview1.Rows[e.RowIndex].FindControl("TextItemWeight");
            DropDownList DropDownItemID = (DropDownList)Gridview1.Rows[e.RowIndex].FindControl("DropDownItemID"); 
              
            int userID = Convert.ToInt32(Session["USER_ID"]);
            string MakeEntryDate = EntryDate.Text;
            string[] MakeEntryDateSplit = MakeEntryDate.Split('-');
            String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
            DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
            string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

            string update_user = "update  MF_PRODUCTION_BAT_TARGET_ITEM  set BATCH_NO = :TextIssueFgID, ITEM_ID = :TextItemID, ITEM_WEIGHT_CWT = :TextItemWeight, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoU_USER_ID where TARGET_ITEM_ID = :NoTargetItemID ";
            cmdi = new OracleCommand(update_user, conn);

            OracleParameter[] objPrm = new OracleParameter[6]; 
            objPrm[0] = cmdi.Parameters.Add("TextIssueFgID", BatchNumber);
            objPrm[1] = cmdi.Parameters.Add("TextItemID", DropDownItemID.Text);
            objPrm[2] = cmdi.Parameters.Add("TextItemWeight", TextItemWeight.Text);
            objPrm[3] = cmdi.Parameters.Add("u_date", u_date);
            objPrm[4] = cmdi.Parameters.Add("NoU_USER_ID", userID);
            objPrm[5] = cmdi.Parameters.Add("NoTargetItemID", TargetItemID);

            cmdi.ExecuteNonQuery();
            cmdi.Parameters.Clear();
            cmdi.Dispose();
             
            string update_batch = "update  MF_PRODUCTION_BATCH_MASTER  set BATCH_NO = :TextIssueFgID, FURNACES_ID = :TextFurnacesID, PARTY_ID = :TextDropDownSupplierID, ENTRY_DATE = TO_DATE(:EntryDate, 'DD/MM/YYYY'), REMARKS = :TextRemarks, IS_ACTIVE = :TextIsActive, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoU_USER_ID where BATCH_ID = :NoBatchID ";
            cmdi = new OracleCommand(update_batch, conn);

            OracleParameter[] objPr = new OracleParameter[9];
            objPr[0] = cmdi.Parameters.Add("TextIssueFgID", BatchNumber);
            objPr[1] = cmdi.Parameters.Add("TextFurnacesID", DropDownFurnacesID.Text);
            objPr[2] = cmdi.Parameters.Add("TextDropDownSupplierID", DropDownSupplierID.Text);
            objPr[3] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
            objPr[4] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
            objPr[5] = cmdi.Parameters.Add("TextIsActive", ISActive); 
            objPr[6] = cmdi.Parameters.Add("u_date", u_date);
            objPr[7] = cmdi.Parameters.Add("NoU_USER_ID", userID);
            objPr[8] = cmdi.Parameters.Add("NoBatchID", BatchID);

            cmdi.ExecuteNonQuery();
            cmdi.Parameters.Clear();
            cmdi.Dispose();
             
            conn.Close();

            alert_box.Visible = true;
            alert_box.Controls.Add(new LiteralControl("Batch Data Update Successfully"));
            alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
             
            Gridview1.EditIndex = -1;
            ItemDisplay(BatchNumber, new EventArgs());
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

        protected void BtProductionPost_Click(object sender, EventArgs e)
        {
        try
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            LinkButton btn = (LinkButton)sender;
            Session["page_data_id"] = btn.CommandArgument;
            string USER_DATA_ID = Convert.ToString(Session["page_data_id"]);

            int userID = Convert.ToInt32(Session["USER_ID"]); 
            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt"); 
             
            string update_batch = "update  MF_PRODUCTION_BATCH_MASTER  set PRODUCTION_ISSUE_S = :TextBatchStatus,  PRODUCTION_ISSUE_S_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , PRODUCTION_ISSUE_S_ID = :NoU_USER_ID where BATCH_NO = :NoBatchID ";
            cmdi = new OracleCommand(update_batch, conn);

            OracleParameter[] objPr = new OracleParameter[4];
            objPr[0] = cmdi.Parameters.Add("TextBatchStatus", "Complete"); 
            objPr[1] = cmdi.Parameters.Add("u_date", u_date);
            objPr[2] = cmdi.Parameters.Add("NoU_USER_ID", userID);
            objPr[3] = cmdi.Parameters.Add("NoBatchID", USER_DATA_ID);

            cmdi.ExecuteNonQuery();
            cmdi.Parameters.Clear();
            cmdi.Dispose();
             
            conn.Close();

            alert_box.Visible = true;
            alert_box.Controls.Add(new LiteralControl("Production Data Post Complete Successfully"));
            alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
             
            Gridview1.EditIndex = -1;
            Display(); 
            ClearText(); 
            string USER_DATA_ID1 = "";
            ItemDisplay(USER_DATA_ID1, new EventArgs()); 
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
            //Get Item ID of editable row
            string ItemID = Gridview1.DataKeys[e.NewEditIndex]["ITEM_ID"].ToString();
            string BatchNumber = Gridview1.DataKeys[e.NewEditIndex]["BATCH_NO"].ToString(); 
            //Open Edit Mode
            Gridview1.EditIndex = e.NewEditIndex;
            ItemDisplay(BatchNumber, new EventArgs());
            //Populate item
            DropDownList DropDownItemID = (DropDownList)Gridview1.Rows[e.NewEditIndex].FindControl("DropDownItemID"); 
            if (DropDownItemID != null)
            {
                BindItem(DropDownItemID, ItemData());
                DropDownItemID.SelectedValue = ItemID;  
            }
        }


        protected void Gridview1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
     //   try
     //   {
          if (IS_DELETE_ACTIVE == "Enable")
            {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            int userID = Convert.ToInt32(Session["USER_ID"]);
            int IssueItemID = Convert.ToInt32(Gridview1.DataKeys[e.RowIndex]["ISSUE_FG_ITEM_ID"]);
            int IssueFgID = Convert.ToInt32(Gridview1.DataKeys[e.RowIndex]["ISSUE_FG_ID"]);
            int ItemID = Convert.ToInt32(Gridview1.DataKeys[e.RowIndex]["ITEM_ID"]);
            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                    // check production data
                    int ItemIdOld = 0; double ItemWeightOld = 0.00; 
                    string makeSQLPro = " select ITEM_ID, ITEM_WEIGHT from MF_PRODUCTION_ISSUE_FG_ITEM where ISSUE_FG_ITEM_ID  = '" + IssueItemID + "'  ";
                    cmdl = new OracleCommand(makeSQLPro);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    for (int i = 0; i < RowCount; i++)
                    {
                        ItemIdOld = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                        ItemWeightOld = Convert.ToDouble(dt.Rows[i]["ITEM_WEIGHT"].ToString());
                    }

                    //inventory calculation
                    int InvenItemID = 0;
                    double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, FinalStock = 0.00, StockInWetNew = 0.00, FinalStockNew = 0.00;

                    // check inventory RM
                    string makeSQL2 = " select * from MF_FG_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeSQL2);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    for (int i = 0; i < RowCount; i++)
                    {
                        InvenItemID = Convert.ToInt32(dt.Rows[i]["ITEM_ID"].ToString());
                        InitialStock = Convert.ToDouble(dt.Rows[i]["INITIAL_STOCK_WT"].ToString());
                        StockInWet = Convert.ToDouble(dt.Rows[i]["STOCK_IN_WT"].ToString());
                        StockOutWet = Convert.ToDouble(dt.Rows[i]["STOCK_OUT_WT"].ToString());
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    StockInWetNew = StockInWet - ItemWeightOld;
                    FinalStockNew = (InitialStock + StockInWetNew) - StockOutWet;

                    if (ItemWeightOld <= FinalStock)
                    {
                        if (0 < RowCount)
                        {
                            // update inventory RM (minus old data)
                            string update_inven_mas = "update  MF_FG_STOCK_INVENTORY_MASTER  set STOCK_IN_WT = :NoStockIn, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID  ";
                            cmdu = new OracleCommand(update_inven_mas, conn);

                            OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                            objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockIn", StockInWetNew);
                            objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                            objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                            objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                            objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);

                            cmdu.ExecuteNonQuery();
                            cmdu.Parameters.Clear();
                            cmdu.Dispose();
                        }
                         
                        string delete_user_page = " delete from MF_PRODUCTION_ISSUE_FG_ITEM where ISSUE_FG_ITEM_ID  = '" + IssueItemID + "'";

                        cmdi = new OracleCommand(delete_user_page, conn);

                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();
                        conn.Close();

                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Item Delete Successfully"));
                        alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                        DisplayProductionIssue();
                        ItemDisplay(IssueFgID, new EventArgs());
                         
                    }
                    else
                    { 
                        alert_box.Visible = true;
                        alert_box.Controls.Add(new LiteralControl("Item Weight is not available in the Raw Material Inventory"));
                        alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                    }
                     
                }
                else
                {
                    Response.Redirect("~/PagePermissionError.aspx");
                }
          //  }
        //    catch
        //    {
        //       Response.Redirect("~/ParameterError.aspx");
        //    }
        
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
                    makeSQL = " SELECT MPIM.ISSUE_FG_ID, MPIM.BATCH_NO, MPIM.ENTRY_DATE, MPIM.CREATE_DATE, MPIM.UPDATE_DATE, MPIM.IS_ACTIVE, MPF.FURNACES_NAME, MP.PARTY_NAME, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME, MPBM.INVEN_AVAIL_CHECK_S, MPBM.PRODUCTION_ISSUE_S, MPBM.PRODUCTION_ISSUE_POST_S, QUALITY_APPRO_LV_ONE_S, ISSUED_FG_S FROM MF_PRODUCTION_ISSUE_FG MPIM LEFT JOIN MF_PRODUCTION_BATCH_MASTER MPBM ON MPBM.BATCH_NO = MPIM.BATCH_NO LEFT JOIN MF_PRODUCTION_FURNACES MPF ON MPF.FURNACES_ID = MPBM.FURNACES_ID LEFT JOIN MF_PARTY MP ON MP.PARTY_ID = MPBM.PARTY_ID LEFT JOIN MF_PRODUCTION_GRADE MPG ON MPG.GRADE_ID = MPBM.GRADE_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID ORDER BY MPIM.ISSUE_FG_ID DESC ";
                }
                else
                {
                    makeSQL = " SELECT MPIM.ISSUE_FG_ID, MPIM.BATCH_NO, MPIM.ENTRY_DATE, MPIM.CREATE_DATE, MPIM.UPDATE_DATE, MPIM.IS_ACTIVE, MPF.FURNACES_NAME, MP.PARTY_NAME, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME, MPBM.INVEN_AVAIL_CHECK_S, MPBM.PRODUCTION_ISSUE_S, MPBM.PRODUCTION_ISSUE_POST_S, QUALITY_APPRO_LV_ONE_S, ISSUED_FG_S FROM MF_PRODUCTION_ISSUE_FG MPIM LEFT JOIN MF_PRODUCTION_BATCH_MASTER MPBM ON MPBM.BATCH_NO = MPIM.BATCH_NO LEFT JOIN MF_PRODUCTION_FURNACES MPF ON MPF.FURNACES_ID = MPBM.FURNACES_ID LEFT JOIN MF_PARTY MP ON MP.PARTY_ID = MPBM.PARTY_ID LEFT JOIN MF_PRODUCTION_GRADE MPG ON MPG.GRADE_ID = MPBM.GRADE_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID WHERE MPBM.BATCH_NO like '" + txtSearchUserRole.Text + "%' or MI.ITEM_NAME like '" + txtSearchUserRole.Text + "%' or MPF.FURNACES_NAME like '" + txtSearchUserRole.Text + "%'  or MPBM.IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY MPIM.ISSUE_FG_ID desc";

                    alert_box.Visible = false;
                }

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView2.DataSource = dt;
                GridView2.DataKeyNames = new string[] { "BATCH_NO" };

                GridView2.DataBind();
                conn.Close(); 
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

        protected void gridViewFileInformation_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow Row in GridView2.Rows)
            {
                string IsBatchPostCheck = (Row.FindControl("IsFgPost") as Label).Text; 
                string IsQaCheck = (Row.FindControl("IsQaCheck") as Label).Text; 
                if (IsBatchPostCheck == "Complete")   
                {
                    (Row.FindControl("LinkSelectClick") as LinkButton).Enabled = false; 
                }

                if (IsBatchPostCheck == "Complete" || IsQaCheck == "Pending")
                {
                    (Row.FindControl("BtProductionPost_Click") as LinkButton).Enabled = false;
                }

            }
        }


        public void BatchPendingProduction()
        {
          try
           {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                BatchPending.Text = "";
                string makeSQL = "  SELECT MPBM.BATCH_ID, MPBM.BATCH_NO FROM MF_PRODUCTION_BATCH_MASTER MPBM LEFT JOIN MF_PRODUCTION_ISSUE_FG MPIF ON MPIF.BATCH_NO = MPBM.BATCH_NO WHERE MPBM.PRODUCTION_ISSUE_S = 'Complete' AND MPIF.BATCH_NO IS NULL ORDER BY MPBM.BATCH_ID DESC ";
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                    if (RowCount > 0)
                    {
                        for (int i = 0; i < RowCount; i++)
                        {
                            BatchPending.Text += "<span class=\"label label-danger\" >" + dt.Rows[i]["BATCH_NO"].ToString() + "</span>&nbsp;";

                        }
                    }
                    else {
                            BatchPending.Text += "<span class=\"label label-success\" >There are no Pending Batch issue for production</span>&nbsp;";
                    }

                    conn.Close();
                  
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

        public void ClearText()
        {
            EntryDate.Text = "";
            TextRemarks.Text = "";
            DropDownGradeID.Text = "0";
            TextIssueFgID.Text = "";
            DropDownSupplierID.Text = "0";
            DropDownFurnacesID.Text = "0";
       
        }

        public void ClearTextField(object sender, EventArgs e)
        {
            EntryDate.Text = "";
            TextRemarks.Text = "";
            DropDownGradeID.Text = "0";
            TextIssueFgID.Text = "";
            DropDownSupplierID.Text = "0";
            DropDownFurnacesID.Text = "0";
            string USER_DATA_ID = "";
            ItemDisplay(USER_DATA_ID, new EventArgs());

        }

        protected void LinkSelectClick(object sender, EventArgs e) 
        {
         //   try {  
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             LinkButton btn = (LinkButton)sender;
             Session["user_page_data_id"] = btn.CommandArgument;
             string USER_DATA_ID = Convert.ToString(Session["user_page_data_id"]); 
              
             string makeSQL = " SELECT MPIM.ISSUE_FG_ID, MPBM.BATCH_ID, MPBM.BATCH_NO, MPBM.FURNACES_ID, MPBM.PARTY_ID, MPBM.GRADE_ID, TO_CHAR(MPIM.ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, MPIM.IS_ACTIVE, MPIM.REMARKS, MPIM.ACTUAL_GRADE_ID FROM MF_PRODUCTION_ISSUE_FG MPIM LEFT JOIN MF_PRODUCTION_BATCH_MASTER MPBM ON MPBM.BATCH_NO = MPIM.BATCH_NO WHERE MPIM.ISSUE_FG_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                TextIssueFgID.Text = dt.Rows[i]["ISSUE_FG_ID"].ToString();
                DropDownFurnacesID.Text = dt.Rows[i]["FURNACES_ID"].ToString(); 
                DropDownSupplierID.Text = dt.Rows[i]["PARTY_ID"].ToString();
                EntryDate.Text = dt.Rows[i]["ENTRY_DATE"].ToString(); 
                CheckIsActive.Checked   = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);

                DataTable dtBatchID = new DataTable();
                DataSet db = new DataSet();
                string makeDropDownBatchSQL = " SELECT MPBM.BATCH_ID, MPBM.BATCH_NO FROM MF_PRODUCTION_BATCH_MASTER MPBM LEFT JOIN MF_PRODUCTION_ISSUE_FG MPIM ON MPIM.BATCH_NO = MPBM.BATCH_NO WHERE MPBM.ISSUED_FG_S = 'Ongoing' AND MPIM.BATCH_NO = '" + dt.Rows[i]["BATCH_NO"].ToString() + "' ORDER BY MPBM.BATCH_ID DESC";
                db = ExecuteBySqlString(makeDropDownBatchSQL);
                dtBatchID = (DataTable)db.Tables[0];
                DropDownBatchNo.DataSource = dtBatchID;
                DropDownBatchNo.DataValueField = "BATCH_ID";
                DropDownBatchNo.DataTextField = "BATCH_NO";
                DropDownBatchNo.DataBind();
                DropDownBatchNo.Items.Insert(0, new ListItem("Select  Batch", "0"));
                DropDownBatchNo.Text = dt.Rows[i]["BATCH_ID"].ToString();

                DataTable dtGradeID = new DataTable();
                DataSet ds = new DataSet();
                string makeGradeSQL = "  SELECT MPG.GRADE_ID, MPG.GRADE_ID || ': ' || MI.ITEM_NAME AS ITEM_NAME FROM MF_PRODUCTION_GRADE_CUSTOMER MPGC LEFT JOIN MF_PRODUCTION_GRADE MPG ON MPG.GRADE_ID = MPGC.GRADE_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID WHERE MPGC.PARTY_ID = '" + dt.Rows[i]["PARTY_ID"].ToString() + "' ";
                ds = ExecuteBySqlString(makeGradeSQL);
                dtGradeID = (DataTable)ds.Tables[0];
                DropDownGradeID.DataSource = dtGradeID;
                DropDownGradeID.DataValueField = "GRADE_ID";
                DropDownGradeID.DataTextField = "ITEM_NAME";
                DropDownGradeID.DataBind();
                DropDownGradeID.Items.Insert(0, new ListItem("Select  Grade Template", "0")); 
                DropDownGradeID.Text = dt.Rows[i]["GRADE_ID"].ToString();


                DataTable dtActGradeID = new DataTable();
                DataSet dsc = new DataSet();
                string makeActGradeSQL = "  SELECT MI.ITEM_ID, MI.ITEM_CODE || ': ' || MI.ITEM_NAME AS ITEM_NAME FROM MF_ITEM MI WHERE  MI.IS_ACTIVE = 'Enable' AND MI.IS_SALES_ACTIVE = 'Enable' ";
                dsc = ExecuteBySqlString(makeActGradeSQL);
                dtActGradeID = (DataTable)dsc.Tables[0];
                DropDownActualGradeID.DataSource = dtActGradeID;
                DropDownActualGradeID.DataValueField = "ITEM_ID";
                DropDownActualGradeID.DataTextField = "ITEM_NAME";
                DropDownActualGradeID.DataBind();
                DropDownActualGradeID.Items.Insert(0, new ListItem("Select Actual Grade", "0"));

                DropDownActualGradeID.Text = dt.Rows[i]["ACTUAL_GRADE_ID"].ToString();
            }

           
            conn.Close();   
            alert_box.Visible = false;
            DropDownBatchNo.Enabled = false;
            DropDownFurnacesID.Enabled = false;
            DisplayProductionIssue();
            ItemDisplay(TextIssueFgID.Text, new EventArgs());
         //   }
         //   catch
         //   {
        //        Response.Redirect("~/ParameterError.aspx");
        //    }

        }


        public void GetBatchData(object sender, EventArgs e)
        {

            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open(); 
            string USER_DATA_ID = DropDownBatchNo.Text;

            string makeSQL = " SELECT MPBM.BATCH_ID, MPBM.BATCH_NO, MPBM.FURNACES_ID, MPBM.PARTY_ID, MPBM.GRADE_ID, TO_CHAR(MPBM.ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, MPBM.IS_ACTIVE, MPG.ITEM_ID FROM MF_PRODUCTION_BATCH_MASTER MPBM LEFT JOIN MF_PRODUCTION_GRADE MPG ON MPG.GRADE_ID = MPBM.GRADE_ID WHERE MPBM.BATCH_ID = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;

            for (int i = 0; i < RowCount; i++)
            { 
                DropDownFurnacesID.Text = dt.Rows[i]["FURNACES_ID"].ToString(); 
                DropDownSupplierID.Text = dt.Rows[i]["PARTY_ID"].ToString();
                EntryDate.Text = dt.Rows[i]["ENTRY_DATE"].ToString();
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);

                DataTable dtGradeID = new DataTable();
                DataSet ds = new DataSet();
                string makeGradeSQL = "  SELECT MPG.GRADE_ID, MPG.GRADE_ID || ' : ' || MI.ITEM_NAME AS ITEM_NAME FROM MF_PRODUCTION_GRADE_CUSTOMER MPGC LEFT JOIN MF_PRODUCTION_GRADE MPG ON MPG.GRADE_ID = MPGC.GRADE_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID WHERE MPGC.PARTY_ID = '" + dt.Rows[i]["PARTY_ID"].ToString() + "' ";
                ds = ExecuteBySqlString(makeGradeSQL);
                dtGradeID = (DataTable)ds.Tables[0];
                DropDownGradeID.DataSource = dtGradeID;
                DropDownGradeID.DataValueField = "GRADE_ID";
                DropDownGradeID.DataTextField = "ITEM_NAME";
                DropDownGradeID.DataBind();
                DropDownGradeID.Items.Insert(0, new ListItem("Select  Grade Template", "0"));
                DropDownGradeID.Text = dt.Rows[i]["GRADE_ID"].ToString();

                DataTable dtActGradeID = new DataTable();
                DataSet dsc = new DataSet();
                string makeActGradeSQL = "  SELECT MI.ITEM_ID, MI.ITEM_CODE || ': ' || MI.ITEM_NAME AS ITEM_NAME FROM MF_ITEM MI WHERE  MI.IS_ACTIVE = 'Enable' AND MI.IS_SALES_ACTIVE = 'Enable' ";
                dsc = ExecuteBySqlString(makeActGradeSQL);
                dtActGradeID = (DataTable)dsc.Tables[0];
                DropDownActualGradeID.DataSource = dtActGradeID;
                DropDownActualGradeID.DataValueField = "ITEM_ID";
                DropDownActualGradeID.DataTextField = "ITEM_NAME";
                DropDownActualGradeID.DataBind();
                DropDownActualGradeID.Items.Insert(0, new ListItem("Select Actual Grade", "0"));
                 
                DropDownActualGradeID.Text = dt.Rows[i]["ITEM_ID"].ToString();


            }

            var Row = Gridview1.FooterRow;
            DropDownList DropDownItemID = (DropDownList)Row.FindControl("DropDownItemID"); 
            DropDownItemID.Focus();
            DisplayProductionIssue();
            conn.Close();
            alert_box.Visible = false;

        }
         
        public void DisplayProductionIssue()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string BatchNo = DropDownBatchNo.SelectedItem.Text;

                string makeSQL = " SELECT row_number() OVER (ORDER BY MI.ITEM_ID) AS SL_NO, MI.ITEM_ID, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME, nvl(MPBTI.ITEM_WEIGHT_CWT,0) AS ITEM_WEIGHT_TARGET, nvl(MPII.ITEM_WEIGHT,0) AS ITEM_WEIGHT_ACTUAL, (nvl(MPBTI.ITEM_WEIGHT_CWT,0) -  nvl(MPII.ITEM_WEIGHT,0)) AS VARIANCE_WT FROM MF_ITEM MI LEFT JOIN(SELECT ITEM_ID, SUM(ITEM_WEIGHT_CWT) AS ITEM_WEIGHT_CWT FROM MF_PRODUCTION_BAT_TARGET_ITEM WHERE BATCH_NO = '" + BatchNo + "' GROUP BY ITEM_ID)MPBTI ON MPBTI.ITEM_ID = MI.ITEM_ID LEFT JOIN(SELECT ITEM_ID, SUM(ITEM_WEIGHT) AS ITEM_WEIGHT FROM MF_PRODUCTION_ISSUE_ITEM WHERE BATCH_NO = '" + BatchNo + "' GROUP BY ITEM_ID)MPII ON MPII.ITEM_ID = MI.ITEM_ID WHERE(MPBTI.ITEM_WEIGHT_CWT > 0 OR MPII.ITEM_WEIGHT > 0) ORDER BY MI.ITEM_ID ";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView4.DataSource = dt;
                GridView4.DataKeyNames = new string[] { "ITEM_ID" };
                GridView4.DataBind();
              
                conn.Close();
                alert_box.Visible = false;
            }
        }


        protected void DisplayBatchDetalis(object sender, CommandEventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            string BatchNumber =  e.CommandArgument.ToString();

            //Building an HTML string.
            StringBuilder html = new StringBuilder();

            string makeBatchSQL = " select MPBM.BATCH_ID, MPBM.BATCH_NO, MPF.FURNACES_NAME, MP.PARTY_NAME, MPBM.GRADE_ID, TO_CHAR(MPBM.ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, MPIM.IS_ACTIVE, NU.USER_NAME, TO_CHAR(MPIM.CREATE_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS CREATE_DATE  from MF_PRODUCTION_BATCH_MASTER MPBM LEFT JOIN MF_PRODUCTION_ISSUE_FG MPIM ON MPIM.BATCH_NO = MPBM.BATCH_NO LEFT JOIN MF_PRODUCTION_FURNACES MPF ON MPF.FURNACES_ID = MPBM.FURNACES_ID LEFT JOIN MF_PARTY MP ON MP.PARTY_ID = MPBM.PARTY_ID  LEFT JOIN NRC_USER NU ON NU.USER_ID = MPIM.C_USER_ID WHERE MPBM.BATCH_NO = '" + BatchNumber + "'";
             
            cmdl = new OracleCommand(makeBatchSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;
            for (int i = 0; i < RowCount; i++)
            {
                html.Append("<div class='box box-info'><div class='box-header with-border'><h3 class='box-title'>Issue For Finished Goods - Batch General Info</h3></div><div class='box-body'>");

                html.Append("<table class='table table-hover table-bordered table-striped' cellpadding='0' cellspacing='0' width=100%>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Batch No </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["BATCH_NO"].ToString());
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Furnace Name </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["FURNACES_NAME"].ToString());
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Customer Name </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["PARTY_NAME"].ToString());
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Grade ID </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["GRADE_ID"].ToString());
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Entry Date </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["ENTRY_DATE"].ToString());
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Is Active </th>");
                html.Append("<td style='text-align:left;'>");

                if (dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable")
                {
                    html.Append("<span class=\"label label-success\" >Enable</span>");
                }
                else { html.Append("<span class=\"label label-danger\" >Disable</span>"); }

                html.Append("</td>");
                html.Append("</tr>");
                html.Append("<tr valign='top'>");
                html.Append("<th style='text-align:right;'> Created </th>");
                html.Append("<td style='text-align:left;'>");
                html.Append(dt.Rows[i]["USER_NAME"].ToString());
                html.Append(" <span class=\"label label-info\" > " + dt.Rows[i]["CREATE_DATE"].ToString() + "</span>");
                html.Append("</td>");
                html.Append("</tr>");
                html.Append("</table>");

                html.Append("</div>");
                html.Append("</div>");

            }
             
            PlaceHolderBatchDetails.Controls.Add(new Literal { Text = html.ToString() });

            string makeSQL = " select  MPII.*, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME from MF_PRODUCTION_ISSUE_FG_ITEM MPII LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPII.ITEM_ID WHERE  MPII.BATCH_NO = '" + BatchNumber + "' ORDER BY MI.ITEM_ID ";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            GridView5.DataSource = dt;
            GridView5.DataKeyNames = new string[] { "ITEM_NAME" };
            GridView5.DataBind();
              

            ScriptManager.RegisterStartupScript(this, this.GetType(), "myModalDetails", "showPopup();", true);
             
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
 