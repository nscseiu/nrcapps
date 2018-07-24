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
using System.Text.RegularExpressions;
using System.Globalization;


namespace NRCAPPS.PF
{
    public partial class PfActualGarbage_old : System.Web.UI.Page
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
        public DataTable TableData = new DataTable();  
       
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
                double TextItem0 = Convert.ToDouble(TextItemWet0.Text);
                double TextItem1 = Convert.ToDouble(TextItemWet1.Text);
                double TextItem2 = Convert.ToDouble(TextItemWet2.Text);
                double TextItem3 = Convert.ToDouble(TextItemWet3.Text);
                double TextItem4 = Convert.ToDouble(TextItemWet4.Text);
                double TextItem5 = Convert.ToDouble(TextItemWet5.Text);
                double TextItem6 = Convert.ToDouble(TextItemWet6.Text);
                double TextItem7 = Convert.ToDouble(TextItemWet7.Text);
                double TextItem8 = Convert.ToDouble(TextItemWet8.Text);
                double TextItem9 = Convert.ToDouble(TextItemWet9.Text);

                int InvenItemID = 0, ItemID = 0;
                int InvenSubItemID = 0;
                double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00,  FinalStock = 0.00, StockOutWetNew = 0.00, FinalStockNew = 0.00;

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                double[] TextItem = { TextItem0, TextItem1, TextItem2, TextItem3, TextItem4, TextItem5, TextItem6, TextItem7, TextItem8, TextItem9 };
                int len = TextItem.Length;
                for (int i = 0; i < len; i++)
                {
                    ItemID = i + 1;
                    string get_process_id = "select PF_ACTUAL_GARBAGEID_SEQ.nextval from dual";
                    cmdl = new OracleCommand(get_process_id, conn);
                    int newActualGarbageID = Int16.Parse(cmdl.ExecuteScalar().ToString());
                    string insert_purchase = "insert into  PF_ACTUAL_GARBAGE (ACTUAL_GARBAGE_ID, ITEM_ID, ACTUAL_GAR_WEIGHT, MONTH_YEAR, CREATE_DATE, C_USER_ID ) values (:NoActualGarbageID, :NoItemID, :NoActualGarWeight, TO_DATE(:TextMonthYear, 'MM/YYYY'), TO_DATE(:c_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoCuserID)";
                    cmdi = new OracleCommand(insert_purchase, conn);

                    OracleParameter[] objPrm = new OracleParameter[6];
                    objPrm[0] = cmdi.Parameters.Add("NoActualGarbageID", newActualGarbageID);
                    objPrm[1] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[2] = cmdi.Parameters.Add("NoActualGarWeight", TextItem[i]);
                    objPrm[3] = cmdi.Parameters.Add("TextMonthYear", DataMonthYear);
                    objPrm[4] = cmdi.Parameters.Add("c_date", c_date);
                    objPrm[5] = cmdi.Parameters.Add("NoCuserID", userID);

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose();



                    string makeRmInSQL = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "' ";
                    cmdl = new OracleCommand(makeRmInSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;


                    for (int j = 0; j < RowCount; j++)
                    {
                        InvenItemID = Convert.ToInt32(dt.Rows[j]["ITEM_ID"].ToString());
                        InvenSubItemID = Convert.ToInt32(dt.Rows[j]["SUB_ITEM_ID"].ToString());
                        InitialStock = Convert.ToDouble(dt.Rows[j]["INITIAL_STOCK_WT"].ToString());
                        StockInWet = Convert.ToDouble(dt.Rows[j]["STOCK_IN_WT"].ToString());
                        StockOutWet = Convert.ToDouble(dt.Rows[j]["STOCK_OUT_WT"].ToString());
                        FinalStock = Convert.ToDouble(dt.Rows[j]["FINAL_STOCK_WT"].ToString());
                    }

                    StockOutWetNew = StockOutWet + TextItem[i];
                    FinalStockNew = InitialStock + StockInWet - StockOutWetNew;

                    if (0 < RowCount)
                    {

                        string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", c_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID); 

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }
                    

                }
                 
                conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Insert new Actual Garbage Data successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                 
                clearText();
                Display();
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }
                 

        private void GetAllData() //Get all the data and bind it in HTLM Table       
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
                }  
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
             string makeSQL = " select ITEM_ID, nvl(ACTUAL_GAR_WEIGHT,0) AS ACTUAL_GAR_WEIGHT, TO_CHAR(MONTH_YEAR, 'mm/yyyy') AS MONTH_YEAR from PF_ACTUAL_GARBAGE where TO_CHAR(MONTH_YEAR,'mm/yyyy') = '" + USER_DATA_ID + "' order by ITEM_ID asc ";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt); 
             RowCount = dt.Rows.Count;

             TextBox[] TextItemID = new TextBox[] { TextItemID0, TextItemID1, TextItemID2, TextItemID3, TextItemID4, TextItemID5, TextItemID6, TextItemID7, TextItemID8, TextItemID9 };
             TextBox[] TextItemWet = new TextBox[] { TextItemWet0, TextItemWet1, TextItemWet2, TextItemWet3, TextItemWet4, TextItemWet5, TextItemWet6, TextItemWet7, TextItemWet8, TextItemWet9 };
             
            TextBox[] TextMonthYear = new TextBox[] { TextMonthYear0 };

             for (int i = 0; i < RowCount; i++)
             {
                 TextItemID[i].Text   =  dt.Rows[i]["ITEM_ID"].ToString().ToString();
                 TextItemWet[i].Text = decimal.Parse(dt.Rows[i]["ACTUAL_GAR_WEIGHT"].ToString()).ToString("0.000"); 
             }
             for (int i = 0; i < 1; i++)
             { 
                 TextMonthYear[i].Text = dt.Rows[i]["MONTH_YEAR"].ToString();
             }
               
             conn.Close();
             Display(); 
             alert_box.Visible = false;

             Check_HDPE.Text = "";
             Check_HDCAN.Text = "";
             Check_LDPE.Text = "";
             Check_PC.Text = "";
             Check_PET.Text = "";
             Check_PETRO_RABIGH.Text = "";
             Check_PP.Text = "";
             Check_PVC.Text = "";
             Check_SABIC.Text = "";
             Check_SCC.Text = "";
             
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
                if (txtSearchUser.Text == "")
                {
                    makeSQL = " SELECT TO_CHAR(PPRC.MONTH_YEAR,'mm/yyyy') AS MONTH_YEAR, PPRC.ACTUAL_GARBAGE_ID,  PPRC.ITEM_ID, PI.ITEM_NAME, PI.ITEM_CODE, PPRC.ACTUAL_GAR_WEIGHT, PPRC.MONTH_YEAR, PPRC.CREATE_DATE, PPRC.UPDATE_DATE FROM PF_ACTUAL_GARBAGE PPRC LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRC.ITEM_ID ORDER BY TO_CHAR(PPRC.MONTH_YEAR,'mm/yyyy') desc, PPRC.ITEM_ID asc";
                }
                else
                {
                    makeSQL = " SELECT TO_CHAR(PPRC.MONTH_YEAR,'mm/yyyy') AS MONTH_YEAR, PPRC.ACTUAL_GARBAGE_ID,  PPRC.ITEM_ID, PI.ITEM_NAME, PI.ITEM_CODE, PPRC.ACTUAL_GAR_WEIGHT, PPRC.MONTH_YEAR, PPRC.CREATE_DATE, PPRC.UPDATE_DATE FROM PF_ACTUAL_GARBAGE PPRC LEFT JOIN PF_ITEM PI ON PI.ITEM_ID = PPRC.ITEM_ID where TO_CHAR(PPRC.MONTH_YEAR,'mm/yyyy')  like '" + txtSearchUser.Text + "%' ORDER BY TO_CHAR(PPRC.MONTH_YEAR,'mm/yyyy') desc, PPRC.ITEM_ID asc";

                    alert_box.Visible = false;
                }
                 
                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataKeyNames = new string[] { "MONTH_YEAR" }; 
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
 

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string DataMonthYear = TextMonthYear0.Text;
                double TextItem0 = Convert.ToDouble(TextItemWet0.Text);
                double TextItem1 = Convert.ToDouble(TextItemWet1.Text);
                double TextItem2 = Convert.ToDouble(TextItemWet2.Text);
                double TextItem3 = Convert.ToDouble(TextItemWet3.Text);
                double TextItem4 = Convert.ToDouble(TextItemWet4.Text);
                double TextItem5 = Convert.ToDouble(TextItemWet5.Text);
                double TextItem6 = Convert.ToDouble(TextItemWet6.Text);
                double TextItem7 = Convert.ToDouble(TextItemWet7.Text);
                double TextItem8 = Convert.ToDouble(TextItemWet8.Text);
                double TextItem9 = Convert.ToDouble(TextItemWet9.Text);

                int InvenItemID = 0, ItemID = 0;
                int InvenSubItemID = 0;
                double InitialStock = 0.00, StockInWet = 0.00, StockOutWet = 0.00, FinalStock = 0.00, StockOutWetNew = 0.00, FinalStockNew = 0.00, ItemWeightOld = 0.00;

                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");

                 double[] TextItem = { TextItem0, TextItem1, TextItem2, TextItem3, TextItem4, TextItem5, TextItem6, TextItem7, TextItem8, TextItem9}; 
                 int len = TextItem.Length; 
                 for (int i = 0; i < len; i++)
                {
                    ItemID = i + 1;

                    // previous actual garbage data

                    string makeSQLAcGar = " select * from PF_ACTUAL_GARBAGE where ITEM_ID  = '" + ItemID + "' and TO_CHAR(TO_DATE(MONTH_YEAR), 'mm/YYYY') = '" + DataMonthYear + "' ";
                    cmdl = new OracleCommand(makeSQLAcGar);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;


                    for (int j = 0; j < RowCount; j++)
                    {
                        ItemWeightOld = Convert.ToDouble(dt.Rows[j]["ACTUAL_GAR_WEIGHT"].ToString()); 
                    }

                    // check inventory RM
                    string makeSQLRmIn = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "' ";
                    cmdl = new OracleCommand(makeSQLRmIn);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;


                    for (int k = 0; k < RowCount; k++)
                    {
                        InvenItemID = Convert.ToInt32(dt.Rows[k]["ITEM_ID"].ToString());
                        InvenSubItemID = Convert.ToInt32(dt.Rows[k]["SUB_ITEM_ID"].ToString());
                        InitialStock = Convert.ToDouble(dt.Rows[k]["INITIAL_STOCK_WT"].ToString());
                        StockInWet = Convert.ToDouble(dt.Rows[k]["STOCK_IN_WT"].ToString());
                        StockOutWet = Convert.ToDouble(dt.Rows[k]["STOCK_OUT_WT"].ToString());
                        FinalStock = Convert.ToDouble(dt.Rows[k]["FINAL_STOCK_WT"].ToString());
                    }

                    StockOutWetNew = StockOutWet - ItemWeightOld;
                    FinalStockNew = InitialStock + StockInWet - StockOutWetNew;

                    if (0 < RowCount)
                    {
                        // update inventory RM (minus old data)
                        string update_inven_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                        cmdu = new OracleCommand(update_inven_mas, conn);

                        OracleParameter[] objPrmInevenMas = new OracleParameter[5];
                        objPrmInevenMas[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetNew);
                        objPrmInevenMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInevenMas[2] = cmdu.Parameters.Add("u_date", u_date);
                        objPrmInevenMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInevenMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID); 

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }
                      
                    string makeRmInSQL = " select * from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "' ";
                    cmdl = new OracleCommand(makeRmInSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;


                    for (int j = 0; j < RowCount; j++)
                    {
                        InvenItemID = Convert.ToInt32(dt.Rows[j]["ITEM_ID"].ToString());
                        InvenSubItemID = Convert.ToInt32(dt.Rows[j]["SUB_ITEM_ID"].ToString());
                        InitialStock = Convert.ToDouble(dt.Rows[j]["INITIAL_STOCK_WT"].ToString());
                        StockInWet = Convert.ToDouble(dt.Rows[j]["STOCK_IN_WT"].ToString());
                        StockOutWet = Convert.ToDouble(dt.Rows[j]["STOCK_OUT_WT"].ToString());
                        FinalStock = Convert.ToDouble(dt.Rows[j]["FINAL_STOCK_WT"].ToString());
                    }

                    StockOutWetNew = StockOutWet + TextItem[i];
                    FinalStockNew = InitialStock + StockInWet - StockOutWetNew;

                    if (0 < RowCount)
                    {

                        string update_in_mas = "update  PF_RM_STOCK_INVENTORY_MASTER  set STOCK_OUT_WT = :NoStockOut, FINAL_STOCK_WT = :NoFinalStock, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID = :NoItemID ";
                        cmdu = new OracleCommand(update_in_mas, conn);

                        OracleParameter[] objPrmInMas = new OracleParameter[5];
                        objPrmInMas[0] = cmdu.Parameters.Add("NoStockOut", StockOutWetNew);
                        objPrmInMas[1] = cmdu.Parameters.Add("NoFinalStock", FinalStockNew);
                        objPrmInMas[2] = cmdu.Parameters.Add("u_date", u_date);
                        objPrmInMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                        objPrmInMas[4] = cmdu.Parameters.Add("NoItemID", InvenItemID);

                        cmdu.ExecuteNonQuery();
                        cmdu.Parameters.Clear();
                        cmdu.Dispose();
                    }

                    string update_inve_mas = "update  PF_ACTUAL_GARBAGE  set ACTUAL_GAR_WEIGHT = :NoActualGarWeight, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), U_USER_ID = :NoCuserID  where ITEM_ID =:NoItemID AND To_CHAR(MONTH_YEAR,'mm/yyyy') = :TextMonthYear ";
                    cmdu = new OracleCommand(update_inve_mas, conn);

                    OracleParameter[] objPrmInevMas = new OracleParameter[5];
                    objPrmInevMas[0] = cmdu.Parameters.Add("NoItemID", ItemID);
                    objPrmInevMas[1] = cmdu.Parameters.Add("NoActualGarWeight", TextItem[i]);
                    objPrmInevMas[2] = cmdu.Parameters.Add("u_date", u_date);
                    objPrmInevMas[3] = cmdu.Parameters.Add("NoCuserID", userID);
                    objPrmInevMas[4] = cmdu.Parameters.Add("TextMonthYear", DataMonthYear);

                    cmdu.ExecuteNonQuery();
                    cmdu.Parameters.Clear();
                    cmdu.Dispose();
                        

                }

                 conn.Close();

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Actual Garbage Data Update successfully"));
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
            TextItemWet0.Text = "";
            TextItemWet1.Text = "";
            TextItemWet2.Text = "";
            TextItemWet3.Text = "";
            TextItemWet4.Text = "";
            TextItemWet5.Text = "";
            TextItemWet6.Text = "";
            TextItemWet7.Text = "";
            TextItemWet8.Text = "";
            TextItemWet9.Text = "";
            CheckMonthYear.Text = "";

            Check_HDPE.Text = "";
            Check_HDCAN.Text = "";
            Check_LDPE.Text = "";
            Check_PC.Text = "";
            Check_PET.Text = "";
            Check_PETRO_RABIGH.Text = "";
            Check_PP.Text = "";
            Check_PVC.Text = "";
            Check_SABIC.Text = "";
            Check_SCC.Text = "";
        }

        public void clearText()
        {
            TextItemWet0.Text = "";
            TextItemWet1.Text = "";
            TextItemWet2.Text = "";
            TextItemWet3.Text = "";
            TextItemWet4.Text = "";
            TextItemWet5.Text = "";
            TextItemWet6.Text = "";
            TextItemWet7.Text = "";
            TextItemWet8.Text = "";
            TextItemWet9.Text = "";
            CheckMonthYear.Text = "";

        }

        public void TextHDPE_TextChanged(object sender, EventArgs e)
        {
            int ItemID =Convert.ToInt32(TextItemID0.Text);
            string ItemWeightCheck = TextItemWet0.Text;
            string MatchPattern = "([0-9.]+)";
            if (ItemWeightCheck != null)
            { 
                if (Regex.IsMatch(ItemWeightCheck, MatchPattern))
                {
                    alert_box.Visible = false; 
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open(); 
                    string makeSQL = " select FINAL_STOCK_WT from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    double FinalStock = 0.00;
                    double ItemWeight = Convert.ToDouble(TextItemWet0.Text);
                    for (int i = 0; i < RowCount; i++)
                    {
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    if (ItemWeight <= FinalStock)
                    {

                        Check_HDPE.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Actual Garbage HDPE Value is Correct </label>";
                        Check_HDPE.ForeColor = System.Drawing.Color.Green; 
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");
                        TextItemWet1.Focus();

                    }
                    else
                    {
                        Check_HDPE.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Value is Incorrect. Available  HDPE Material is <span class='badge bg-yellow'>" + FinalStock + "</span> metric ton (MT) below the Actual Garbage</label>";
                        Check_HDPE.ForeColor = System.Drawing.Color.Red; 
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                        TextItemWet0.Text = "";
                        TextItemWet0.Focus();

                    }
                }
                else
                {
                    Check_HDPE.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter HDPE Material Weight</label>";
                    Check_HDPE.ForeColor = System.Drawing.Color.Red; 
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                    TextItemWet0.Text = "";
                    TextItemWet0.Focus(); 
                }
            } 
        }

        public void TextHDCAN_TextChanged(object sender, EventArgs e)
        {
            int ItemID = Convert.ToInt32(TextItemID1.Text);
            string ItemWeightCheck = TextItemWet1.Text;
            string MatchPattern = "([0-9.]+)";
            if (ItemWeightCheck != null)
            {
                if (Regex.IsMatch(ItemWeightCheck, MatchPattern))
                {
                    alert_box.Visible = false;
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    string makeSQL = " select FINAL_STOCK_WT from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    double FinalStock = 0.00;
                    double ItemWeight = Convert.ToDouble(TextItemWet1.Text);
                    for (int i = 0; i < RowCount; i++)
                    {
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    if (ItemWeight <= FinalStock)
                    {

                        Check_HDCAN.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Actual Garbage HD CAN Value is Correct </label>";
                        Check_HDCAN.ForeColor = System.Drawing.Color.Green;
                        TextItemWet2.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");

                    }
                    else
                    {
                        Check_HDCAN.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Value is Incorrect. Available  HD CAN Material is <span class='badge bg-yellow'>" + FinalStock + "</span> metric ton (MT) below the Actual Garbage</label>";
                        Check_HDCAN.ForeColor = System.Drawing.Color.Red; 
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                        TextItemWet1.Focus();
                        TextItemWet1.Text = "";

                    }
                }
                else
                {
                    Check_HDCAN.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter HD CAN Material Weight</label>";
                    Check_HDCAN.ForeColor = System.Drawing.Color.Red; 
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                    TextItemWet1.Focus();
                    TextItemWet1.Text = "";

                }
            }
        }

        public void TextLDPE_TextChanged(object sender, EventArgs e)
        {
            int ItemID = Convert.ToInt32(TextItemID2.Text);
            string ItemWeightCheck = TextItemWet2.Text;
            string MatchPattern = "([0-9.]+)";
            if (ItemWeightCheck != null)
            {
                if (Regex.IsMatch(ItemWeightCheck, MatchPattern))
                {
                    alert_box.Visible = false;
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    string makeSQL = " select FINAL_STOCK_WT from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    double FinalStock = 0.00;
                    double ItemWeight = Convert.ToDouble(TextItemWet2.Text);
                    for (int i = 0; i < RowCount; i++)
                    {
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    if (ItemWeight <= FinalStock)
                    {

                        Check_LDPE.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Actual Garbage LDPE Value is Correct </label>";
                        Check_LDPE.ForeColor = System.Drawing.Color.Green;
                        TextItemWet3.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");

                    }
                    else
                    {
                        Check_LDPE.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Value is Incorrect. Available  LDPE Material is <span class='badge bg-yellow'>" + FinalStock + "</span> metric ton (MT) below the Actual Garbage</label>";
                        Check_LDPE.ForeColor = System.Drawing.Color.Red; 
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                        TextItemWet2.Text = "";
                        TextItemWet2.Focus();

                    }
                }
                else
                {
                    Check_LDPE.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter LDPE Material Weight</label>";
                    Check_LDPE.ForeColor = System.Drawing.Color.Red; 
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                    TextItemWet2.Text = "";
                    TextItemWet2.Focus();

                }
            }
        }

        public void TextPC_TextChanged(object sender, EventArgs e)
        {
            int ItemID = Convert.ToInt32(TextItemID3.Text);
            string ItemWeightCheck = TextItemWet3.Text;
            string MatchPattern = "([0-9.]+)";
            if (ItemWeightCheck != null)
            {
                if (Regex.IsMatch(ItemWeightCheck, MatchPattern))
                {
                    alert_box.Visible = false;
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    string makeSQL = " select FINAL_STOCK_WT from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    double FinalStock = 0.00;
                    double ItemWeight = Convert.ToDouble(TextItemWet3.Text);
                    for (int i = 0; i < RowCount; i++)
                    {
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    if (ItemWeight <= FinalStock)
                    {

                        Check_PC.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Actual Garbage PC Value is Correct </label>";
                        Check_PC.ForeColor = System.Drawing.Color.Green;
                        TextItemWet4.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");

                    }
                    else
                    {
                        Check_PC.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Value is Incorrect. Available  PC Material is <span class='badge bg-yellow'>" + FinalStock + "</span> metric ton (MT) below the Actual Garbage</label>";
                        Check_PC.ForeColor = System.Drawing.Color.Red;
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                        TextItemWet3.Text = "";
                        TextItemWet3.Focus();

                    }
                }
                else
                {
                    Check_PC.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter PC Material Weight</label>";
                    Check_PC.ForeColor = System.Drawing.Color.Red;
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                    TextItemWet3.Text = "";
                    TextItemWet3.Focus();

                }
            }
        }
      
        public void TextPET_TextChanged(object sender, EventArgs e)
        {
            int ItemID = Convert.ToInt32(TextItemID4.Text);
            string ItemWeightCheck = TextItemWet4.Text;
            string MatchPattern = "([0-9.]+)";
            if (ItemWeightCheck != null)
            {
                if (Regex.IsMatch(ItemWeightCheck, MatchPattern))
                {
                    alert_box.Visible = false;
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    string makeSQL = " select FINAL_STOCK_WT from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    double FinalStock = 0.00;
                    double ItemWeight = Convert.ToDouble(TextItemWet4.Text);
                    for (int i = 0; i < RowCount; i++)
                    {
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    if (ItemWeight <= FinalStock)
                    {

                        Check_PET.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Actual Garbage PET Value is Correct </label>";
                        Check_PET.ForeColor = System.Drawing.Color.Green;
                        TextItemWet5.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");

                    }
                    else
                    {
                        Check_PET.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Value is Incorrect. Available  PET Material is <span class='badge bg-yellow'>" + FinalStock + "</span> metric ton (MT) below the Actual Garbage</label>";
                        Check_PET.ForeColor = System.Drawing.Color.Red;
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                        TextItemWet4.Text = "";
                        TextItemWet4.Focus();

                    }
                }
                else
                {
                    Check_PET.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter PET Material Weight</label>";
                    Check_PET.ForeColor = System.Drawing.Color.Red;
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                    TextItemWet4.Text = "";
                    TextItemWet4.Focus();

                }
            }
        }     
        public void TextPETRO_RABIGH_TextChanged(object sender, EventArgs e)
        {
            int ItemID = Convert.ToInt32(TextItemID5.Text);
            string ItemWeightCheck = TextItemWet5.Text;
            string MatchPattern = "([0-9.]+)";
            if (ItemWeightCheck != null)
            {
                if (Regex.IsMatch(ItemWeightCheck, MatchPattern))
                {
                    alert_box.Visible = false;
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    string makeSQL = " select FINAL_STOCK_WT from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    double FinalStock = 0.00;
                    double ItemWeight = Convert.ToDouble(TextItemWet5.Text);
                    for (int i = 0; i < RowCount; i++)
                    {
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    if (ItemWeight <= FinalStock)
                    {

                        Check_PETRO_RABIGH.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Actual Garbage PETRO RABIGH Value is Correct </label>";
                        Check_PETRO_RABIGH.ForeColor = System.Drawing.Color.Green;
                        TextItemWet6.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");

                    }
                    else
                    {
                        Check_PETRO_RABIGH.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Value is Incorrect. Available  PETRO RABIGH Material is <span class='badge bg-yellow'>" + FinalStock + "</span> metric ton (MT) below the Actual Garbage</label>";
                        Check_PETRO_RABIGH.ForeColor = System.Drawing.Color.Red;
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                        TextItemWet5.Text = "";
                        TextItemWet5.Focus();

                    }
                }
                else
                {
                    Check_PETRO_RABIGH.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter PETRO RABIGH Material Weight</label>";
                    Check_PETRO_RABIGH.ForeColor = System.Drawing.Color.Red;
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                    TextItemWet5.Text = "";
                    TextItemWet5.Focus();

                }
            }
        }
        public void TextPP_TextChanged(object sender, EventArgs e)
        {
            int ItemID = Convert.ToInt32(TextItemID6.Text);
            string ItemWeightCheck = TextItemWet6.Text;
            string MatchPattern = "([0-9.]+)";
            if (ItemWeightCheck != null)
            {
                if (Regex.IsMatch(ItemWeightCheck, MatchPattern))
                {
                    alert_box.Visible = false;
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    string makeSQL = " select FINAL_STOCK_WT from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    double FinalStock = 0.00;
                    double ItemWeight = Convert.ToDouble(TextItemWet6.Text);
                    for (int i = 0; i < RowCount; i++)
                    {
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    if (ItemWeight <= FinalStock)
                    {

                        Check_PP.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Actual Garbage PP Value is Correct </label>";
                        Check_PP.ForeColor = System.Drawing.Color.Green;
                        TextItemWet7.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");

                    }
                    else
                    {
                        Check_PP.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Value is Incorrect. Available  PP Material is <span class='badge bg-yellow'>" + FinalStock + "</span> metric ton (MT) below the Actual Garbage</label>";
                        Check_PP.ForeColor = System.Drawing.Color.Red;
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                        TextItemWet6.Text = "";
                        TextItemWet6.Focus();

                    }
                }
                else
                {
                    Check_PP.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter PP Material Weight</label>";
                    Check_PP.ForeColor = System.Drawing.Color.Red;
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                    TextItemWet6.Text = "";
                    TextItemWet6.Focus();

                }
            }
        }
        public void TextPVC_TextChanged(object sender, EventArgs e)
        {
            int ItemID = Convert.ToInt32(TextItemID7.Text);
            string ItemWeightCheck = TextItemWet7.Text;
            string MatchPattern = "([0-9.]+)";
            if (ItemWeightCheck != null)
            {
                if (Regex.IsMatch(ItemWeightCheck, MatchPattern))
                {
                    alert_box.Visible = false;
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    string makeSQL = " select FINAL_STOCK_WT from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    double FinalStock = 0.00;
                    double ItemWeight = Convert.ToDouble(TextItemWet7.Text);
                    for (int i = 0; i < RowCount; i++)
                    {
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    if (ItemWeight <= FinalStock)
                    {

                        Check_PVC.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Actual Garbage PVC Value is Correct </label>";
                        Check_PVC.ForeColor = System.Drawing.Color.Green;
                        TextItemWet8.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");

                    }
                    else
                    {
                        Check_PVC.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Value is Incorrect. Available  PVC Material is <span class='badge bg-yellow'>" + FinalStock + "</span> metric ton (MT) below the Actual Garbage</label>";
                        Check_PVC.ForeColor = System.Drawing.Color.Red;
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                        TextItemWet7.Text = "";
                        TextItemWet7.Focus();

                    }
                }
                else
                {
                    Check_PVC.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter PVC Material Weight</label>";
                    Check_PVC.ForeColor = System.Drawing.Color.Red;
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                    TextItemWet7.Text = "";
                    TextItemWet7.Focus();

                }
            }
        }

        public void TextSABIC_TextChanged(object sender, EventArgs e)
        {
            int ItemID = Convert.ToInt32(TextItemID8.Text);
            string ItemWeightCheck = TextItemWet8.Text;
            string MatchPattern = "([0-9.]+)";
            if (ItemWeightCheck != null)
            {
                if (Regex.IsMatch(ItemWeightCheck, MatchPattern))
                {
                    alert_box.Visible = false;
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    string makeSQL = " select FINAL_STOCK_WT from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    double FinalStock = 0.00;
                    double ItemWeight = Convert.ToDouble(TextItemWet8.Text);
                    for (int i = 0; i < RowCount; i++)
                    {
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    if (ItemWeight <= FinalStock)
                    {

                        Check_SABIC.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Actual Garbage SABIC Value is Correct </label>";
                        Check_SABIC.ForeColor = System.Drawing.Color.Green;
                        TextItemWet9.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");

                    }
                    else
                    {
                        Check_SABIC.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Value is Incorrect. Available  SABIC Material is <span class='badge bg-yellow'>" + FinalStock + "</span> metric ton (MT) below the Actual Garbage</label>";
                        Check_SABIC.ForeColor = System.Drawing.Color.Red;
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                        TextItemWet8.Text = "";
                        TextItemWet8.Focus();

                    }
                }
                else
                {
                    Check_SABIC.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter SABIC Material Weight</label>";
                    Check_SABIC.ForeColor = System.Drawing.Color.Red;
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                    TextItemWet8.Text = "";
                    TextItemWet8.Focus();

                }
            }
        }
        public void TextSCC_TextChanged(object sender, EventArgs e)
        {
            int ItemID = Convert.ToInt32(TextItemID9.Text);
            string ItemWeightCheck = TextItemWet9.Text;
            string MatchPattern = "([0-9.]+)";
            if (ItemWeightCheck != null)
            {
                if (Regex.IsMatch(ItemWeightCheck, MatchPattern))
                {
                    alert_box.Visible = false;
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();
                    string makeSQL = " select FINAL_STOCK_WT from PF_RM_STOCK_INVENTORY_MASTER where ITEM_ID  = '" + ItemID + "'  ";
                    cmdl = new OracleCommand(makeSQL);
                    oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                    dt = new DataTable();
                    oradata.Fill(dt);
                    RowCount = dt.Rows.Count;

                    double FinalStock = 0.00;
                    double ItemWeight = Convert.ToDouble(TextItemWet9.Text);
                    for (int i = 0; i < RowCount; i++)
                    {
                        FinalStock = Convert.ToDouble(dt.Rows[i]["FINAL_STOCK_WT"].ToString());
                    }

                    if (ItemWeight <= FinalStock)
                    {

                        Check_SCC.Text = "<label class='control-label'><i class='fa fa fa-check'></i> This Actual Garbage SCC Value is Correct </label>";
                        Check_SCC.ForeColor = System.Drawing.Color.Green;
                        BtnAdd.Focus();
                        BtnAdd.Attributes.Add("aria-disabled", "true");
                        BtnAdd.Attributes.Add("class", "btn btn-primary active");

                    }
                    else
                    {
                        Check_SCC.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> This Value is Incorrect. Available  SCC Material is <span class='badge bg-yellow'>" + FinalStock + "</span> metric ton (MT) below the Actual Garbage</label>";
                        Check_SCC.ForeColor = System.Drawing.Color.Red;
                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                        TextItemWet9.Text = "";
                        TextItemWet9.Focus();

                    }
                }
                else
                {
                    Check_SCC.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Enter SCC Material Weight</label>";
                    Check_SCC.ForeColor = System.Drawing.Color.Red;
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                    TextItemWet9.Text = "";
                    TextItemWet9.Focus();

                }
            }
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
                cmd.CommandText = "select MONTH_YEAR from PF_ACTUAL_GARBAGE where TO_CHAR(TO_DATE(MONTH_YEAR), 'mm/YYYY') = '" + TextMonthYear0.Text + "'";
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