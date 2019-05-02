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
    public partial class MfBatchItem : System.Web.UI.Page
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

                        BtnGradeCopyBatch.Style["visibility"] = "hidden";
                        BtnGradeDetails.Style["visibility"] = "hidden";
                        TextBatchNumber.Attributes.Add("readonly", "readonly");



                        string USER_DATA_ID = "";
                        ItemDisplay(USER_DATA_ID, new EventArgs());

                        Display();
                        DisplayGradeTempDetails();
                        DisplayTodayBatch();
                        BatchPendingProduction();
                        alert_box.Visible = false;

                    }
                    IsLoad = false;
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

        public void GetBatchNo(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            string get_batch_id = "", Furnaces = "";
            if (DropDownFurnacesID.Text == "1")
            {
                Furnaces = "A";
                get_batch_id = " select LAST_NUMBER from all_sequences where sequence_name = 'MF_PROD_BATCH_A_MASTERID_SEQ'";
            }
            else if (DropDownFurnacesID.Text == "2")
            {
                Furnaces = "B";
                get_batch_id = " select LAST_NUMBER from all_sequences where sequence_name = 'MF_PROD_BATCH_B_MASTERID_SEQ'";
            }

            cmdu = new OracleCommand(get_batch_id, conn);
            string BatchNo = cmdu.ExecuteScalar().ToString();
            string Year = DateTime.Now.Year.ToString();
            TextBatchNumber.Text = Furnaces + "-" + BatchNo + "-" + Year;


        }


        private void ItemDisplay(object sender, EventArgs e)
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                string BatchID = TextBatchNumber.Text;

                List<ClassVariable> allContacts = null;

                string makeSQL = " select  MPBTI.*, ITEM_CODE || ' : ' || ITEM_NAME AS ITEM_NAME from MF_PRODUCTION_BAT_TARGET_ITEM MPBTI LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPBTI.ITEM_ID  WHERE MPBTI.BATCH_NO = '" + BatchID + "' ORDER BY MPBTI.ITEM_ID";

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
            string sqlString = " SELECT ITEM_ID, ITEM_CODE || ' : ' || ITEM_NAME AS ITEM_NAME FROM MF_ITEM WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
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
            string BatchNumber = TextBatchNumber.Text;
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from MF_PRODUCTION_BAT_TARGET_ITEM where BATCH_NO = '" + BatchNumber + "' AND ITEM_ID = '" + ItemID + "'";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            if (!dr.HasRows)
            {

            }
            else
            {
                var Row = Gridview1.FooterRow;
                DropDownList DropDownItemID = (DropDownList)Row.FindControl("DropDownItemID");
                DropDownItemID.SelectedValue = "0";
                DropDownItemID.Focus();
            }

        }

        public void InsertGradeCopyBatch(object sender, EventArgs e)
        {
            try
            {
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    string BatchNumber = TextBatchNumber.Text;
                    string GradeID = DropDownGradeID.Text;
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                    string MakeEntryDate = EntryDate.Text;
                    string[] MakeEntryDateSplit = MakeEntryDate.Split('-');
                    String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                    DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                    string get_id = "select MF_PROD_BATCH_MASTERID_SEQ.nextval from dual";
                    cmdu = new OracleCommand(get_id, conn);
                    int newBatchID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                    if (DropDownFurnacesID.Text == "1")
                    {
                        string get_no = "select MF_PROD_BATCH_A_MASTERID_SEQ.nextval from dual";
                        cmdu = new OracleCommand(get_no, conn);
                        int newBatchNo = Int16.Parse(cmdu.ExecuteScalar().ToString());
                    }
                    else if (DropDownFurnacesID.Text == "2")
                    {
                        string get_no = "select MF_PROD_BATCH_B_MASTERID_SEQ.nextval from dual";
                        cmdu = new OracleCommand(get_no, conn);
                        int newBatchNo = Int16.Parse(cmdu.ExecuteScalar().ToString());

                    }


                    string insert_grade = "insert into MF_PRODUCTION_BATCH_MASTER (BATCH_ID, BATCH_NO, FURNACES_ID, PARTY_ID, GRADE_ID, ENTRY_DATE, REMARKS, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES (:TextBatchID, :TextBatchNo, :TextFurnacesID, :TextPartyID, :TextGradeID, TO_DATE(:EntryDate, 'DD/MM/YYYY'), :TextRemarks, :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_grade, conn);

                    OracleParameter[] objPr = new OracleParameter[10];
                    objPr[0] = cmdi.Parameters.Add("TextBatchID", newBatchID);
                    objPr[1] = cmdi.Parameters.Add("TextBatchNo", BatchNumber);
                    objPr[2] = cmdi.Parameters.Add("TextFurnacesID", DropDownFurnacesID.Text);
                    objPr[3] = cmdi.Parameters.Add("TextPartyID", DropDownSupplierID.Text);
                    objPr[4] = cmdi.Parameters.Add("TextGradeID", GradeID);
                    objPr[5] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                    objPr[6] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                    objPr[7] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPr[8] = cmdi.Parameters.Add("u_date", u_date);
                    objPr[9] = cmdi.Parameters.Add("NoCuserID", userID);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    string makeSQL = " SELECT ITEM_ID, ITEM_WEIGHT FROM MF_PRODUCTION_GRADE_ITEM  WHERE GRADE_ID = '" + GradeID + "' ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    for (int i = 0; i < RowCount; i++)
                    {
                        // insert item in batch
                        string get_item_cat_id = "select MF_PROD_BAT_TARGET_ITEM_SEQ.nextval from dual";
                        cmdu = new OracleCommand(get_item_cat_id, conn);
                        int newTargetItemID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                        string insert_user = "insert into MF_PRODUCTION_BAT_TARGET_ITEM (TARGET_ITEM_ID, BATCH_NO, ITEM_ID, ITEM_WEIGHT_CWT, CREATE_DATE, C_USER_ID) VALUES ( :NoTargetItemID, :TextBatchNumber, :TextItemID, :TextItemWeight, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                        cmdi = new OracleCommand(insert_user, conn);

                        OracleParameter[] objPrm = new OracleParameter[6];
                        objPrm[0] = cmdi.Parameters.Add("NoTargetItemID", newTargetItemID);
                        objPrm[1] = cmdi.Parameters.Add("TextBatchNumber", BatchNumber);
                        objPrm[2] = cmdi.Parameters.Add("TextItemID", dt.Rows[i]["ITEM_ID"].ToString());
                        objPrm[3] = cmdi.Parameters.Add("TextItemWeight", dt.Rows[i]["ITEM_WEIGHT"].ToString());
                        objPrm[4] = cmdi.Parameters.Add("u_date", u_date);
                        objPrm[5] = cmdi.Parameters.Add("NoCuserID", userID);

                        cmdi.ExecuteNonQuery();
                        cmdi.Parameters.Clear();
                        cmdi.Dispose();

                    }
                    conn.Close();

                    BtnGradeCopyBatch.Style["visibility"] = "hidden";
                    ItemDisplay(BatchNumber, new EventArgs());
                    Display();
                    DisplayGradeTempDetails();
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


        protected void Gridview1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
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
                            string BatchNumber = TextBatchNumber.Text;
                            var Row = Gridview1.FooterRow;
                            DropDownList DropDownItemID = (DropDownList)Row.FindControl("DropDownItemID");
                            TextBox TextItemWeight = (TextBox)Row.FindControl("TextItemWeight");
                            string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = conn;
                            cmd.CommandText = "select * from MF_PRODUCTION_BATCH_MASTER where BATCH_NO = '" + BatchNumber + "'";
                            cmd.CommandType = CommandType.Text;
                            OracleDataReader dr = cmd.ExecuteReader();
                            if (!dr.HasRows)
                            {
                                string GradeID = DropDownGradeID.Text;
                                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                                string MakeEntryDate = EntryDate.Text;
                                string[] MakeEntryDateSplit = MakeEntryDate.Split('-');
                                String EntryDateTemp = MakeEntryDateSplit[0].Replace("/", "-");
                                DateTime EntryDateNewD = DateTime.ParseExact(EntryDateTemp, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                                string EntryDateNew = EntryDateNewD.ToString("dd-MM-yyyy");

                                string get_id = "select MF_PROD_BATCH_MASTERID_SEQ.nextval from dual";
                                cmdu = new OracleCommand(get_id, conn);
                                int newBatchID = Int16.Parse(cmdu.ExecuteScalar().ToString());


                                if (DropDownFurnacesID.Text == "1")
                                {
                                    string get_no = "select MF_PROD_BATCH_A_MASTERID_SEQ.nextval from dual";
                                    cmdu = new OracleCommand(get_no, conn);
                                    int newBatchNo = Int16.Parse(cmdu.ExecuteScalar().ToString());
                                }
                                else if (DropDownFurnacesID.Text == "2")
                                {
                                    string get_no = "select MF_PROD_BATCH_B_MASTERID_SEQ.nextval from dual";
                                    cmdu = new OracleCommand(get_no, conn);
                                    int newBatchNo = Int16.Parse(cmdu.ExecuteScalar().ToString());

                                }

                                string insert_grade = "insert into MF_PRODUCTION_BATCH_MASTER (BATCH_ID, BATCH_NO, FURNACES_ID, PARTY_ID, GRADE_ID, ENTRY_DATE, REMARKS, IS_ACTIVE, CREATE_DATE, C_USER_ID) VALUES (:TextBatchID, :TextBatchNo, :TextFurnacesID, :TextPartyID, :TextGradeID, TO_DATE(:EntryDate, 'DD/MM/YYYY'), :TextRemarks, :TextIsActive, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                                cmdi = new OracleCommand(insert_grade, conn);

                                OracleParameter[] objPr = new OracleParameter[10];
                                objPr[0] = cmdi.Parameters.Add("TextBatchID", newBatchID);
                                objPr[1] = cmdi.Parameters.Add("TextBatchNo", BatchNumber);
                                objPr[2] = cmdi.Parameters.Add("TextFurnacesID", DropDownFurnacesID.Text);
                                objPr[3] = cmdi.Parameters.Add("TextPartyID", DropDownSupplierID.Text);
                                objPr[4] = cmdi.Parameters.Add("TextGradeID", GradeID);
                                objPr[5] = cmdi.Parameters.Add("EntryDate", EntryDateNew);
                                objPr[6] = cmdi.Parameters.Add("TextRemarks", TextRemarks.Text);
                                objPr[7] = cmdi.Parameters.Add("TextIsActive", ISActive);
                                objPr[8] = cmdi.Parameters.Add("u_date", u_date);
                                objPr[9] = cmdi.Parameters.Add("NoCuserID", userID);

                                cmdi.ExecuteNonQuery();
                                cmdi.Parameters.Clear();
                                cmdi.Dispose();
                            }

                            string get_item_cat_id = "select MF_PROD_BAT_TARGET_ITEM_SEQ.nextval from dual";
                            cmdu = new OracleCommand(get_item_cat_id, conn);
                            int newTargetItemID = Int16.Parse(cmdu.ExecuteScalar().ToString());

                            string insert_user = "insert into MF_PRODUCTION_BAT_TARGET_ITEM (TARGET_ITEM_ID, BATCH_NO, ITEM_ID, ITEM_WEIGHT_CWT, CREATE_DATE, C_USER_ID) VALUES ( :NoTargetItemID, :TextBatchNumber, :TextItemID, :TextItemWeight, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                            cmdi = new OracleCommand(insert_user, conn);

                            OracleParameter[] objPrm = new OracleParameter[6];
                            objPrm[0] = cmdi.Parameters.Add("NoTargetItemID", newTargetItemID);
                            objPrm[1] = cmdi.Parameters.Add("TextBatchNumber", BatchNumber);
                            objPrm[2] = cmdi.Parameters.Add("TextItemID", DropDownItemID.Text);
                            objPrm[3] = cmdi.Parameters.Add("TextItemWeight", TextItemWeight.Text);
                            objPrm[4] = cmdi.Parameters.Add("u_date", u_date);
                            objPrm[5] = cmdi.Parameters.Add("NoCuserID", userID);


                            cmdi.ExecuteNonQuery();

                            cmdi.Parameters.Clear();
                            cmdi.Dispose();
                            conn.Close();

                            alert_box.Visible = true;
                            alert_box.Controls.Add(new LiteralControl("Insert New Batch Data Successfully"));
                            alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

                            ItemDisplay(BatchNumber, new EventArgs());
                            Display();
                            DisplayTodayBatch();
                            DisplayGradeTempDetails();
                            //ItemDisplay();
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

                    string BatchID = TextBatchID.Text;

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

                    string update_user = "update  MF_PRODUCTION_BAT_TARGET_ITEM  set BATCH_NO = :TextBatchNumber, ITEM_ID = :TextItemID, ITEM_WEIGHT_CWT = :TextItemWeight, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoU_USER_ID where TARGET_ITEM_ID = :NoTargetItemID ";
                    cmdi = new OracleCommand(update_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[6];
                    objPrm[0] = cmdi.Parameters.Add("TextBatchNumber", BatchNumber);
                    objPrm[1] = cmdi.Parameters.Add("TextItemID", DropDownItemID.Text);
                    objPrm[2] = cmdi.Parameters.Add("TextItemWeight", TextItemWeight.Text);
                    objPrm[3] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[4] = cmdi.Parameters.Add("NoU_USER_ID", userID);
                    objPrm[5] = cmdi.Parameters.Add("NoTargetItemID", TargetItemID);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();

                    string update_batch = "update  MF_PRODUCTION_BATCH_MASTER  set BATCH_NO = :TextBatchNumber, FURNACES_ID = :TextFurnacesID, PARTY_ID = :TextDropDownSupplierID, ENTRY_DATE = TO_DATE(:EntryDate, 'DD/MM/YYYY'), REMARKS = :TextRemarks, IS_ACTIVE = :TextIsActive, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoU_USER_ID where BATCH_ID = :NoBatchID ";
                    cmdi = new OracleCommand(update_batch, conn);

                    OracleParameter[] objPr = new OracleParameter[9];
                    objPr[0] = cmdi.Parameters.Add("TextBatchNumber", BatchNumber);
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
                    DisplayGradeTempDetails();
                    DisplayTodayBatch();
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

        protected void BtBatchPost_Click(object sender, EventArgs e)
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

                    string update_batch = "update  MF_PRODUCTION_BATCH_MASTER  set CREATE_POST_S = :TextBatchStatus,  CREATE_POST_S_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , CREATE_POST_S_ID = :NoU_USER_ID where BATCH_ID = :NoBatchID ";
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
                    alert_box.Controls.Add(new LiteralControl("Batch Data Post Complete Successfully"));
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
            try
            {
                if (IS_DELETE_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int TargetItemID = Convert.ToInt32(Gridview1.DataKeys[e.RowIndex]["TARGET_ITEM_ID"]);
                    string BatchNumber = Convert.ToString(Gridview1.DataKeys[e.RowIndex]["BATCH_NO"]);
                    string delete_user_page = " delete from MF_PRODUCTION_BAT_TARGET_ITEM where TARGET_ITEM_ID  = '" + TargetItemID + "'";

                    cmdi = new OracleCommand(delete_user_page, conn);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();

                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Item Delete Successfully"));
                    alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");

                    ItemDisplay(BatchNumber, new EventArgs());
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

                string makeSQL = "";
                if (txtSearchUserRole.Text == "")
                {
                    makeSQL = " select  MPBM.*, MPF.FURNACES_NAME, MP.PARTY_NAME, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME from MF_PRODUCTION_BATCH_MASTER MPBM LEFT JOIN MF_PRODUCTION_FURNACES MPF ON MPF.FURNACES_ID = MPBM.FURNACES_ID  LEFT JOIN MF_PARTY MP ON MP.PARTY_ID = MPBM.PARTY_ID LEFT JOIN MF_PRODUCTION_GRADE MPG ON MPG.GRADE_ID = MPBM.GRADE_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID ORDER BY MPBM.BATCH_ID desc";
                }
                else
                {
                    makeSQL = " select  MPBM.*, MPF.FURNACES_NAME, MP.PARTY_NAME, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME from MF_PRODUCTION_BATCH_MASTER MPBM LEFT JOIN MF_PRODUCTION_FURNACES MPF ON MPF.FURNACES_ID = MPBM.FURNACES_ID  LEFT JOIN MF_PARTY MP ON MP.PARTY_ID = MPBM.PARTY_ID LEFT JOIN MF_PRODUCTION_GRADE MPG ON MPG.GRADE_ID = MPBM.GRADE_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID   where MPBM.BATCH_NO like '" + txtSearchUserRole.Text + "%' or MI.ITEM_NAME like '" + txtSearchUserRole.Text + "%' or MPF.FURNACES_NAME like '" + txtSearchUserRole.Text + "%'  or MPBM.IS_ACTIVE like '" + txtSearchUserRole.Text + "%' ORDER BY MPBM.BATCH_ID desc";

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
            DisplayGradeTempDetails();
        }

        protected void GridViewUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView2.PageIndex = e.NewPageIndex;
            Display();
            DisplayGradeTempDetails();
            alert_box.Visible = false;
        }

        protected void gridViewFileInformation_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow Row in GridView2.Rows)
            {
                string IsBatchPostCheck = (Row.FindControl("IsBatchPost") as Label).Text;
                //     string isCheckPrint = (Row.FindControl("IsPrintedCheckLink") as Label).Text;
                if (IsBatchPostCheck == "Complete")  // || isCheckPrint == "Printed"
                {
                    (Row.FindControl("LinkSelectClick") as LinkButton).Enabled = false;
                    (Row.FindControl("BtBatchPost_Click") as LinkButton).Enabled = false;
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
                    string makeSQL = "  SELECT MPBM.BATCH_ID, MPBM.BATCH_NO FROM MF_PRODUCTION_BATCH_MASTER MPBM LEFT JOIN MF_PRODUCTION_ISSUE_MASTER MPIM ON MPIM.BATCH_NO = MPBM.BATCH_NO WHERE MPBM.INVEN_AVAIL_CHECK_S = 'Complete' AND MPIM.BATCH_NO IS NULL ORDER BY MPBM.BATCH_ID DESC ";
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
                    else
                    {
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
            TextBatchNumber.Text = "";
            DropDownSupplierID.Text = "0";
            DropDownFurnacesID.Text = "0";

        }

        public void ClearTextField(object sender, EventArgs e)
        {
            EntryDate.Text = "";
            TextRemarks.Text = "";
            DropDownGradeID.Text = "0";
            TextBatchNumber.Text = "";
            DropDownSupplierID.Text = "0";
            DropDownFurnacesID.Text = "0";
            string USER_DATA_ID = "";
            ItemDisplay(USER_DATA_ID, new EventArgs());

        }

        protected void LinkSelectClick(object sender, EventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            LinkButton btn = (LinkButton)sender;
            Session["user_page_data_id"] = btn.CommandArgument;
            string USER_DATA_ID = Convert.ToString(Session["user_page_data_id"]);

            string makeSQL = " select BATCH_ID, BATCH_NO, FURNACES_ID, PARTY_ID, GRADE_ID, TO_CHAR(ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, IS_ACTIVE from MF_PRODUCTION_BATCH_MASTER where BATCH_ID = '" + USER_DATA_ID + "'";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;

            for (int i = 0; i < RowCount; i++)
            {
                TextBatchID.Text = dt.Rows[i]["BATCH_ID"].ToString();
                DropDownFurnacesID.Text = dt.Rows[i]["FURNACES_ID"].ToString();
                TextBatchNumber.Text = dt.Rows[i]["BATCH_NO"].ToString();
                DropDownSupplierID.Text = dt.Rows[i]["PARTY_ID"].ToString();
                EntryDate.Text = dt.Rows[i]["ENTRY_DATE"].ToString();
                CheckIsActive.Checked = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);

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

            }


            conn.Close();
            BtnGradeDetails.Style["visibility"] = "show";
            BtnGradeCopyBatch.Style["visibility"] = "hidden";
            DisplayGradeTempDetails();
            alert_box.Visible = false;
            DropDownFurnacesID.Enabled = false;
            ItemDisplay(TextBatchNumber.Text, new EventArgs());

        }


        public void GetGradeList(object sender, EventArgs e)
        {

            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            string USER_DATA_ID = DropDownSupplierID.Text;

            DataTable dtGradeID = new DataTable();
            DataSet ds = new DataSet();
            string makeGradeSQL = "  SELECT MPG.GRADE_ID, MPG.GRADE_ID || ': ' || MI.ITEM_NAME AS ITEM_NAME FROM MF_PRODUCTION_GRADE_CUSTOMER MPGC LEFT JOIN MF_PRODUCTION_GRADE MPG ON MPG.GRADE_ID = MPGC.GRADE_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID WHERE MPGC.PARTY_ID = '" + USER_DATA_ID + "' AND MPG.IS_ACTIVE = 'Enable' ";
            ds = ExecuteBySqlString(makeGradeSQL);
            dtGradeID = (DataTable)ds.Tables[0];
            DropDownGradeID.DataSource = dtGradeID;
            DropDownGradeID.DataValueField = "GRADE_ID";
            DropDownGradeID.DataTextField = "ITEM_NAME";
            DropDownGradeID.DataBind();
            DropDownGradeID.Items.Insert(0, new ListItem("Select  Grade Template", "0"));

            conn.Close();
            alert_box.Visible = false;

        }

        public void GetGradeTempleteDetails(object sender, EventArgs e)
        {

            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();
            string USER_DATA_ID = TextBatchNumber.Text;

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from MF_PRODUCTION_BATCH_MASTER where BATCH_NO = '" + USER_DATA_ID + "'";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            if (!dr.HasRows)
            {
                BtnGradeCopyBatch.Style["visibility"] = "show";
            }
            else
            {
                BtnGradeCopyBatch.Style["visibility"] = "hidden";
            }

            conn.Close();

            BtnGradeDetails.Style["visibility"] = "show";
            alert_box.Visible = false;
            DisplayGradeTempDetails();
        }

        public void DisplayGradeTempDetails()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();

                //Building an HTML string.
                StringBuilder html = new StringBuilder();

                string USER_DATA_ID = DropDownGradeID.Text;

                string makeGradeSQL = "  SELECT MPG.GRADE_ID, MPG.GRADE_DES, MPG.ITEM_ID, MPG.REMARKS, MPG.IS_ACTIVE, MI.ITEM_NAME, NU.USER_NAME, TO_CHAR(MPG.CREATE_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS CREATE_DATE FROM MF_PRODUCTION_GRADE MPG LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID LEFT JOIN NRC_USER NU ON NU.USER_ID = MPG.C_USER_ID WHERE MPG.GRADE_ID = '" + USER_DATA_ID + "'";

                cmdl = new OracleCommand(makeGradeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                for (int i = 0; i < RowCount; i++)
                {
                    html.Append("<div class='box box-info'><div class='box-header with-border'><h3 class='box-title'>Grade Detalis</h3></div><div class='box-body'>");

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


                string makeSQL = " SELECT MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME, ITEM_WEIGHT FROM MF_PRODUCTION_GRADE_ITEM MPGI LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPGI.ITEM_ID WHERE MPGI.GRADE_ID = '" + USER_DATA_ID + "' ";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView3.DataSource = dt;
                GridView3.DataKeyNames = new string[] { "ITEM_NAME" };
                GridView3.DataBind();

                PlaceHolderGradeDetails.Controls.Add(new Literal { Text = html.ToString() });

                conn.Close();
            }
        }


        public void DisplayTodayBatch()
        {
            if (IS_VIEW_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = "";
                string MonthYear = System.DateTime.Now.ToString("dd/MM/yyyy");

                makeSQL = " select  MPBM.*, MPF.FURNACES_NAME, MP.PARTY_NAME, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME from MF_PRODUCTION_BATCH_MASTER MPBM LEFT JOIN MF_PRODUCTION_FURNACES MPF ON MPF.FURNACES_ID = MPBM.FURNACES_ID  LEFT JOIN MF_PARTY MP ON MP.PARTY_ID = MPBM.PARTY_ID LEFT JOIN MF_PRODUCTION_GRADE MPG ON MPG.GRADE_ID = MPBM.GRADE_ID LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPG.ITEM_ID WHERE to_char(MPBM.ENTRY_DATE, 'dd/mm/yyyy') = '" + MonthYear + "' ORDER BY MPBM.BATCH_ID desc";

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView4.DataSource = dt;
                GridView4.DataKeyNames = new string[] { "BATCH_NO" };
                GridView4.DataBind();

                conn.Close();
                // alert_box.Visible = false;
            }
        }

        protected void GridViewSearchSummary(object sender, EventArgs e)
        {
            this.DisplayTodayBatch();
        }

        protected void GridView2_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView4.PageIndex = e.NewPageIndex;
            DisplayTodayBatch();
            alert_box.Visible = false;
        }

        protected void DisplayBatchDetalis(object sender, CommandEventArgs e)
        {
            OracleConnection conn = new OracleConnection(strConnString);
            conn.Open();

            string BatchNumber = e.CommandArgument.ToString();

            //Building an HTML string.
            StringBuilder html = new StringBuilder();

            string makeBatchSQL = " select MPBM.BATCH_ID, MPBM.BATCH_NO, MPF.FURNACES_NAME, MP.PARTY_NAME, MPBM.GRADE_ID, TO_CHAR(MPBM.ENTRY_DATE,'dd/mm/yyyy') AS ENTRY_DATE, MPBM.IS_ACTIVE, NU.USER_NAME, TO_CHAR(MPBM.CREATE_DATE, 'DD/MM/YYYY HH:MI:SS AM') AS CREATE_DATE  from MF_PRODUCTION_BATCH_MASTER MPBM LEFT JOIN MF_PRODUCTION_FURNACES MPF ON MPF.FURNACES_ID = MPBM.FURNACES_ID LEFT JOIN MF_PARTY MP ON MP.PARTY_ID = MPBM.PARTY_ID  LEFT JOIN NRC_USER NU ON NU.USER_ID = MPBM.C_USER_ID WHERE MPBM.BATCH_NO = '" + BatchNumber + "'";


            cmdl = new OracleCommand(makeBatchSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            RowCount = dt.Rows.Count;
            for (int i = 0; i < RowCount; i++)
            {
                html.Append("<div class='box box-info'><div class='box-header with-border'><h3 class='box-title'>Batch General Info</h3></div><div class='box-body'>");

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

            string makeSQL = " select  MPBTI.ITEM_WEIGHT_CWT, MI.ITEM_CODE || ' : ' || MI.ITEM_NAME AS ITEM_NAME from MF_PRODUCTION_BAT_TARGET_ITEM MPBTI LEFT JOIN MF_ITEM MI ON MI.ITEM_ID = MPBTI.ITEM_ID  WHERE MPBTI.BATCH_NO = '" + BatchNumber + "' ORDER BY MPBTI.ITEM_ID";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);
            GridView5.DataSource = dt;
            GridView5.DataKeyNames = new string[] { "ITEM_NAME" };
            GridView5.DataBind();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "myModalDetails", "showPopup();", true);

            DisplayGradeTempDetails();
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
