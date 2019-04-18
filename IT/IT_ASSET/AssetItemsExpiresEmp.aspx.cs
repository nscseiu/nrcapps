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
using QRCoder; 
using System.Drawing;
using System.Globalization;


namespace NRCAPPS.IT.IT_ASSET
{
    public partial class AssetItemsExpiresEmp : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand  cmdi, cmdl;
        OracleDataAdapter oradata; 
        DataTable dt, ds;
        int RowCount;
        public static int EmpStatus;

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
                        DataTable dtEmpID = new DataTable();
                        DataSet ds = new DataSet();
                        string makeEmpSQL = " SELECT EMP_ID, EMP_ID || ' - ' || EMP_FNAME || ' ' || EMP_LNAME AS EMP_NAME from HR_EMPLOYEES WHERE IS_ACTIVE = 'Enable' ORDER BY EMP_ID ASC";
                        ds = ExecuteBySqlString(makeEmpSQL);
                        dtEmpID = (DataTable)ds.Tables[0];
                        DropDownEmployeeID.DataSource = dtEmpID;
                        DropDownEmployeeID.DataValueField = "EMP_ID";
                        DropDownEmployeeID.DataTextField = "EMP_NAME";
                        DropDownEmployeeID.DataBind();
                        DropDownEmployeeID.Items.Insert(0, new ListItem("Select  Employee", "0"));

                        DataTable dtItemID = new DataTable();
                        DataSet di = new DataSet();
                        string makeItemSQL = " SELECT IAEI.EMP_ITEMS_ID, AI.ITEM_NAME || ' - ' || AI.ITEM_TYPE || ' ' || AI.ITEM_BRAND AS ITEM_NAME_ALL from IT_ASSET_EMP_ITEMS IAEI  LEFT JOIN  IT_ASSET_ITEMS AI ON AI.ITEM_ID = IAEI.ITEM_ID  LEFT JOIN IT_ASSET_ITEM_CATEGORIES AIC ON AIC.ITEM_CATEGORY_ID = AI.ITEM_CATEGORY_ID WHERE AI.IS_ACTIVE = 'Enable' AND (AIC.ITEM_CAT_QR_PRI_CODE = 'CPU' OR AIC.ITEM_CAT_QR_PRI_CODE = 'ITD') ORDER BY AI.ITEM_ID ASC";
                        di = ExecuteBySqlString(makeItemSQL);
                        dtItemID = (DataTable)di.Tables[0];
                        DropDownItemID.DataSource = dtItemID;
                        DropDownItemID.DataValueField = "EMP_ITEMS_ID";
                        DropDownItemID.DataTextField = "ITEM_NAME_ALL";
                        DropDownItemID.DataBind();
                        DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));

                        DataTable dtItemExpID = new DataTable();
                        DataSet die = new DataSet();
                        string makeItemExpSQL = " SELECT AIE.ITEM_EXP_ID, AIE.ITEM_EXP_NAME from IT_ASSET_ITEM_EXPIRES AIE LEFT JOIN IT_ASSET_ITEM_CATEGORIES AIC ON AIC.ITEM_CATEGORY_ID = AIE.ITEM_CATEGORY_ID WHERE AIE.IS_ACTIVE = 'Enable' ORDER BY  AIE.ITEM_EXP_ID ASC";
                        die = ExecuteBySqlString(makeItemExpSQL);
                        dtItemExpID = (DataTable)die.Tables[0];
                        DropDownItemExpID.DataSource = dtItemExpID;
                        DropDownItemExpID.DataValueField = "ITEM_EXP_ID";
                        DropDownItemExpID.DataTextField = "ITEM_EXP_NAME";
                        DropDownItemExpID.DataBind();
                        DropDownItemExpID.Items.Insert(0, new ListItem("Select  Item Expire", "0"));

                        DataTable dtDepartmentID = new DataTable();
                        DataSet dep = new DataSet();
                        string makeDepartmentSQL = " SELECT * FROM HR_EMP_DEPARTMENTS WHERE IS_ACTIVE = 'Enable' ORDER BY DEPARTMENT_ID ASC";
                        dep = ExecuteBySqlString(makeDepartmentSQL);
                        dtDepartmentID = (DataTable)dep.Tables[0];
                        DropDownDepartmentID.DataSource = dtDepartmentID;
                        DropDownDepartmentID.DataValueField = "DEPARTMENT_ID";
                        DropDownDepartmentID.DataTextField = "DEPARTMENT_NAME";
                        DropDownDepartmentID.DataBind();
                        DropDownDepartmentID.Items.Insert(0, new ListItem("Select  Department", "0"));

                        DataTable dtDivisionID = new DataTable();
                        DataSet dsd = new DataSet();
                        string makeDivisionSQL = " SELECT * FROM HR_EMP_DIVISIONS WHERE IS_ACTIVE = 'Enable' ORDER BY DIVISION_ID ASC";
                        dsd = ExecuteBySqlString(makeDivisionSQL);
                        dtDivisionID = (DataTable)dsd.Tables[0];
                        DropDownDivisionID.DataSource = dtDivisionID;
                        DropDownDivisionID.DataValueField = "DIVISION_ID";
                        DropDownDivisionID.DataTextField = "DIVISION_NAME";
                        DropDownDivisionID.DataBind();
                        DropDownDivisionID.Items.Insert(0, new ListItem("Select  Division", "0"));

                        DataTable dtLocationID = new DataTable();
                        DataSet dsl = new DataSet();
                        string makeLocationSQL = " SELECT * FROM HR_EMP_LOCATIONS WHERE IS_ACTIVE = 'Enable' ORDER BY LOCATION_ID ASC";
                        dsl = ExecuteBySqlString(makeLocationSQL);
                        dtLocationID = (DataTable)dsl.Tables[0];
                        DropDownLocationID.DataSource = dtLocationID;
                        DropDownLocationID.DataValueField = "LOCATION_ID";
                        DropDownLocationID.DataTextField = "LOCATION_NAME";
                        DropDownLocationID.DataBind();
                        DropDownLocationID.Items.Insert(0, new ListItem("Select  Location", "0")); 

                        DataTable dtPlacementID = new DataTable();
                        DataSet depl = new DataSet();
                        string makePlacementSQL = " SELECT * FROM IT_ASSET_ITEMS_PLACEMENT WHERE IS_ACTIVE = 'Enable' ORDER BY PLACEMENT_ID ASC";
                        depl = ExecuteBySqlString(makePlacementSQL);
                        dtPlacementID = (DataTable)depl.Tables[0];
                        DropDownPlacementID.DataSource = dtPlacementID;
                        DropDownPlacementID.DataValueField = "PLACEMENT_ID";
                        DropDownPlacementID.DataTextField = "PLACEMENT_NAME";
                        DropDownPlacementID.DataBind();
                        DropDownPlacementID.Items.Insert(0, new ListItem("Select Placement", "0"));

                        //  Display();
                        DropDownItemID.Attributes.Add("disabled", "disabled");
                   //     TextQrImage.Visible = false; 
                        alert_box.Visible = false;
                        ExpDept.Visible = false;

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
         //   try
         //   {
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]);
                 //   string TextQRCodeAll = TextQRPreCode.Text + '-' + TextQRCode.Text; 
                    int ItemID     = Convert.ToInt32(DropDownItemID.Text);
                    int ItemExpID = Convert.ToInt32(DropDownItemExpID.Text);
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt"); 
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                    string StartExpiryDate = TextStartExpiryDate.Text;
                    string[] StartExpiryDateSplit = StartExpiryDate.Split('-');

                    String StartDateTemp = StartExpiryDateSplit[0].Replace("/", "-");
                    String StartDateTemp1 = StartDateTemp.Replace("M ", "M");
                    DateTime StartDate = DateTime.ParseExact(StartDateTemp1, "dd-MM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                    String ExpiryDateTemp = StartExpiryDateSplit[1].Replace("/", "-");
                    String ExpiryDateTemp1 = ExpiryDateTemp.Trim();
                    DateTime ExpiryDate = DateTime.ParseExact(ExpiryDateTemp1, "dd-MM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                string get_user_id = "select IT_ASSET_EMP_ITEM_EXPSID_SEQ.nextval from dual";
                cmdi = new OracleCommand(get_user_id, conn);
                int newExpID = Int16.Parse(cmdi.ExecuteScalar().ToString());

                if (radExpSelect.SelectedValue == "Employee")
                      {
                    
                        string EmployeeID = DropDownEmployeeID.Text;
                        string insert_user = "insert into IT_ASSET_EMP_ITEM_EXPIRES (EMP_ITEM_EXP_ID, EMP_ID, ITEM_EXP_ID, ITEM_ID, SERIAL_NO, ACTIVATION_CODE, START_DATE, EXPIRES_DATE, EXPIRED_DAYS, CREATE_DATE, U_USER_ID, IS_ACTIVE) VALUES ( 'ANT-' || LPAD(:TextEmpItemsExpID, 6, '0'), :NoEmployeeID, :NoItemExpID, :NoItemID, :TextSerialNo, :TextActivationCode, :TextStartDate, :TextExpiryDate, :TextExpiryDays, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoC_USER_ID, :TextIsActive)";
                        cmdi = new OracleCommand(insert_user, conn);

                        OracleParameter[] objPrm = new OracleParameter[12];
                        objPrm[0] = cmdi.Parameters.Add("TextEmpItemsExpID", newExpID);
                        objPrm[1] = cmdi.Parameters.Add("NoEmployeeID", EmployeeID);
                        objPrm[2] = cmdi.Parameters.Add("NoItemExpID", ItemExpID);
                        objPrm[3] = cmdi.Parameters.Add("NoItemID", ItemID);
                        objPrm[4] = cmdi.Parameters.Add("TextSerialNo", TextSerialNo.Text);
                        objPrm[5] = cmdi.Parameters.Add("TextActivationCode", TextActivationCode.Text);
                        objPrm[6] = cmdi.Parameters.Add("TextStartDate", StartDate);
                        objPrm[7] = cmdi.Parameters.Add("TextExpiryDate", ExpiryDate);
                        objPrm[8] = cmdi.Parameters.Add("TextExpiryDays", TextExpiredDays.Text);
                        objPrm[9] = cmdi.Parameters.Add("u_date", u_date);
                        objPrm[10] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                        objPrm[11] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    }
                    else {
                        int DepartmentID = Convert.ToInt32(DropDownDepartmentID.Text);
                        int DivisionID   = Convert.ToInt32(DropDownDivisionID.Text);
                        int LocationID = Convert.ToInt32(DropDownLocationID.Text);
                        int PlacementID   = Convert.ToInt32(DropDownPlacementID.Text);

                        string insert_user = "insert into IT_ASSET_EMP_ITEM_EXPIRES (EMP_ITEM_EXP_ID, DEPARTMENT_ID, DIVISION_ID, LOCATION_ID, PLACEMENT_ID, ITEM_EXP_ID, ITEM_ID, SERIAL_NO, ACTIVATION_CODE, START_DATE, EXPIRES_DATE, EXPIRED_DAYS, CREATE_DATE, U_USER_ID, IS_ACTIVE) VALUES ( 'ANT-' || LPAD(:TextEmpItemsExpID, 6, '0'), :NoDepartmentID, :NoDivisionID, :NoLocationID, :NoPlacementID, :NoItemExpID, :NoItemID, :TextSerialNo, :TextActivationCode, :TextStartDate, :TextExpiryDate, :TextExpiryDays, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoC_USER_ID, :TextIsActive)";
                        cmdi = new OracleCommand(insert_user, conn);

                        OracleParameter[] objPrm = new OracleParameter[15];
                        objPrm[0] = cmdi.Parameters.Add("TextEmpItemsExpID", newExpID);
                        objPrm[1] = cmdi.Parameters.Add("NoDepartmentID", DepartmentID);
                        objPrm[2] = cmdi.Parameters.Add("NoDivisionID", DivisionID);
                        objPrm[3] = cmdi.Parameters.Add("NoLocationID", LocationID);
                        objPrm[4] = cmdi.Parameters.Add("NoPlacementID", PlacementID); 
                        objPrm[5] = cmdi.Parameters.Add("NoItemExpID", ItemExpID);
                        objPrm[6] = cmdi.Parameters.Add("NoItemID", ItemID);
                        objPrm[7] = cmdi.Parameters.Add("TextSerialNo", TextSerialNo.Text);
                        objPrm[8] = cmdi.Parameters.Add("TextActivationCode", TextActivationCode.Text);
                        objPrm[9] = cmdi.Parameters.Add("TextStartDate", StartDate);
                        objPrm[10] = cmdi.Parameters.Add("TextExpiryDate", ExpiryDate);
                        objPrm[11] = cmdi.Parameters.Add("TextExpiryDays", TextExpiredDays.Text);
                        objPrm[12] = cmdi.Parameters.Add("u_date", u_date);
                        objPrm[13] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                        objPrm[14] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    
                    
                    } 

                    cmdi.ExecuteNonQuery();

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New Expire Item successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                    clearText();
                  //  Display();
                    GridViewItem.DataBind();
                }
                else { 
                    Response.Redirect("~/PagePermissionError.aspx");
                }
       //     }
       //     catch
       //     {
       //         Response.Redirect("~/ParameterError.aspx");
       //     }
        }

        protected void linkSelectClick(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
             LinkButton btn = (LinkButton)sender;
             Session["user_page_data_id"] = btn.CommandArgument;
             string USER_DATA_ID = Session["user_page_data_id"].ToString(); 
              
             DataTable dtUserTypeID = new DataTable();
             DataSet ds = new DataSet();
             string makeSQL = " select * from IT_ASSET_EMP_ITEM_EXPIRES where EMP_ITEM_EXP_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count;
             
             for (int i = 0; i < RowCount; i++)
             {
                 TextItemExpID.Text = dt.Rows[i]["EMP_ITEM_EXP_ID"].ToString(); 
                 DropDownItemExpID.Text    = dt.Rows[i]["ITEM_EXP_ID"].ToString();
                
                 if (radExpSelect.SelectedValue == "Employee")
                 {
                    DataTable dtItemID = new DataTable();
                    DataSet di = new DataSet();
                    string makeItemSQL = " SELECT IAEI.EMP_ITEMS_ID, AI.ITEM_NAME || ' - ' || AI.ITEM_TYPE || ' ' || AI.ITEM_BRAND AS ITEM_NAME_ALL from IT_ASSET_EMP_ITEMS IAEI  LEFT JOIN  IT_ASSET_ITEMS AI ON AI.ITEM_ID = IAEI.ITEM_ID LEFT JOIN IT_ASSET_ITEM_CATEGORIES AIC ON AIC.ITEM_CATEGORY_ID = AI.ITEM_CATEGORY_ID WHERE AI.IS_ACTIVE = 'Enable' AND IAEI.EMP_ID = '" + dt.Rows[i]["EMP_ID"].ToString() +"' AND (AIC.ITEM_CAT_QR_PRI_CODE = 'CPU' OR AIC.ITEM_CAT_QR_PRI_CODE = 'ITD') ORDER BY AI.ITEM_ID ASC";
                    di = ExecuteBySqlString(makeItemSQL);
                    dtItemID = (DataTable)di.Tables[0];
                    DropDownItemID.DataSource = dtItemID;
                    DropDownItemID.DataValueField = "EMP_ITEMS_ID";
                    DropDownItemID.DataTextField = "ITEM_NAME_ALL";
                    DropDownItemID.DataBind();
                    DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));

                    DropDownItemID.Text       = dt.Rows[i]["ITEM_ID"].ToString();
                    DropDownEmployeeID.Text = dt.Rows[i]["EMP_ID"].ToString();
                 }
                 else {
                     DropDownDepartmentID.Text = dt.Rows[i]["DEPARTMENT_ID"].ToString();
                     DropDownDivisionID.Text   = dt.Rows[i]["DIVISION_ID"].ToString();
                     DropDownLocationID.Text = dt.Rows[i]["LOCATION_ID"].ToString();
                     DropDownPlacementID.Text = dt.Rows[i]["PLACEMENT_ID"].ToString();

                    string makeItemSQL = " SELECT IAEI.EMP_ITEMS_ID, AI.ITEM_ID, AI.ITEM_NAME || ' - ' || AI.ITEM_TYPE || ' ' || AI.ITEM_BRAND AS ITEM_NAME_ALL from IT_ASSET_EMP_ITEMS IAEI LEFT JOIN IT_ASSET_ITEMS AI ON IAEI.ITEM_ID = AI.ITEM_ID LEFT JOIN IT_ASSET_ITEM_CATEGORIES AIC ON AIC.ITEM_CATEGORY_ID = AI.ITEM_CATEGORY_ID WHERE AI.IS_ACTIVE = 'Enable' AND IAEI.DEPARTMENT_ID = '" + dt.Rows[i]["DEPARTMENT_ID"].ToString() + "'  AND IAEI.DIVISION_ID = '" + dt.Rows[i]["DIVISION_ID"].ToString() + "' AND IAEI.LOCATION_ID = '" + dt.Rows[i]["LOCATION_ID"].ToString() + "' AND IAEI.PLACEMENT_ID = '" + dt.Rows[i]["PLACEMENT_ID"].ToString() + "' AND (AIC.ITEM_CAT_QR_PRI_CODE = 'CPU' OR AIC.ITEM_CAT_QR_PRI_CODE = 'ITD') ORDER BY AI.ITEM_ID ASC";
                    DataTable dtItemdID = new DataTable();
                    DataSet did = new DataSet();
                    did = ExecuteBySqlString(makeItemSQL);
                    dtItemdID = (DataTable)did.Tables[0];
                    DropDownItemID.DataSource = dtItemdID;
                    DropDownItemID.DataValueField = "EMP_ITEMS_ID";
                    DropDownItemID.DataTextField = "ITEM_NAME_ALL";
                    DropDownItemID.DataBind();
                    DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));

                    DropDownItemID.Text = dt.Rows[i]["ITEM_ID"].ToString();
                }
                 TextSerialNo.Text         = dt.Rows[i]["SERIAL_NO"].ToString();
                 TextActivationCode.Text   = dt.Rows[i]["ACTIVATION_CODE"].ToString(); 
                 DateTime StartDateTemp    = DateTime.ParseExact(dt.Rows[i]["START_DATE"].ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                 string StartDate          = String.Format("{0:dd/MM/yyyy h:mm:ss tt}", StartDateTemp);
                 DateTime ExpiryDateTemp   = DateTime.ParseExact(dt.Rows[i]["EXPIRES_DATE"].ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                 string ExpiryDate         = String.Format("{0:dd/MM/yyyy h:mm:ss tt}", ExpiryDateTemp); 
                 TextStartExpiryDate.Text  = StartDate + " - " + ExpiryDate;
                 TextExpiredDays.Text      = dt.Rows[i]["EXPIRED_DAYS"].ToString(); 
                 CheckIsActive.Checked     = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
             } 
             
             conn.Close();
        //     Display(); 
             alert_box.Visible = false;
             BtnAdd.Attributes.Add("aria-disabled", "false");
             BtnAdd.Attributes.Add("class", "btn btn-primary disabled");

         //    GridViewItem.UseAccessibleHeader = true;
        //     GridViewItem.HeaderRow.TableSection = TableRowSection.TableHeader;
          //   TextQrImage.Visible = true;

            // PlaceHolder1.Text = USER_DATA_ID;

        }

    /*    protected void DisplayItemCatQRPriCode(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();

             int USER_DATA_ID = Convert.ToInt32(DropDownItemExpID.Text); 
             

             DataTable dtUserTypeID = new DataTable();
             DataSet ds = new DataSet();
             string makeSQL = " select AIC.ITEM_CAT_QR_PRI_CODE  from IT_ASSET_ITEM_EXPIRES AIE left join IT_ASSET_ITEM_CATEGORIES AIC ON AIC.ITEM_CATEGORY_ID = AIE.ITEM_CATEGORY_ID  where AIE.ITEM_EXP_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count; 

             for (int i = 0; i < RowCount; i++)
             {
                 TextQRPreCode.Text = dt.Rows[i]["ITEM_CAT_QR_PRI_CODE"].ToString();   
             } 
             
             conn.Close();
        //     Display();
             CheckQRCode.Text = "";
             alert_box.Visible = false;
             BtnAdd.Attributes.Add("aria-disabled", "false");
             BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
             
             if (EmpStatus == 0)
             { 
              //   PlaceHolder1.Text = "if";
             }
             else
             {
               //  PlaceHolder1.Text = "else"; 
                 GridViewItem.UseAccessibleHeader = true;
                 GridViewItem.HeaderRow.TableSection = TableRowSection.TableHeader;

                 
             }

             

        } */
    
        
         public void DisplayEmpItem(object sender, EventArgs e)
         { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
                DataTable dtItemID = new DataTable();
                DataSet di = new DataSet();
             string USER_DATA_ID0 = ""; int USER_DATA_ID=0, USER_DATA_ID2 = 0, USER_DATA_ID3 = 0, USER_DATA_ID4=0; string makeSQL = "";
             if (radExpSelect.SelectedValue == "Employee")
             {
                 USER_DATA_ID0 = DropDownEmployeeID.SelectedValue;
                 makeSQL = " select AEIE.*, AIE.ITEM_EXP_NAME, (EXTRACT (DAY FROM (AEIE.EXPIRES_DATE-SYSDATE)))  AS EXPIRED_DAYS_BET,  AEIE.EXPIRED_DAYS , AEIE.IS_ACTIVE from IT_ASSET_EMP_ITEM_EXPIRES AEIE left join IT_ASSET_ITEM_EXPIRES AIE ON AIE.ITEM_EXP_ID = AEIE.ITEM_EXP_ID where AEIE.EMP_ID = '" + USER_DATA_ID0 + "' order by AEIE.EMP_ITEM_EXP_ID";

            
                string makeItemSQL = " SELECT IAEI.EMP_ITEMS_ID, AI.ITEM_NAME || ' - ' || AI.ITEM_TYPE || ' ' || AI.ITEM_BRAND AS ITEM_NAME_ALL from IT_ASSET_EMP_ITEMS IAEI  LEFT JOIN  IT_ASSET_ITEMS AI ON AI.ITEM_ID = IAEI.ITEM_ID  LEFT JOIN IT_ASSET_ITEM_CATEGORIES AIC ON AIC.ITEM_CATEGORY_ID = AI.ITEM_CATEGORY_ID WHERE IAEI.EMP_ID = '" + USER_DATA_ID0 + "' AND AI.IS_ACTIVE = 'Enable' AND (AIC.ITEM_CAT_QR_PRI_CODE = 'CPU' OR AIC.ITEM_CAT_QR_PRI_CODE = 'ITD') ORDER BY AI.ITEM_ID ASC";
                di = ExecuteBySqlString(makeItemSQL);
                dtItemID = (DataTable)di.Tables[0];
                DropDownItemID.DataSource = dtItemID;
                DropDownItemID.DataValueField = "EMP_ITEMS_ID";
                DropDownItemID.DataTextField = "ITEM_NAME_ALL";
                DropDownItemID.DataBind();
                DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                GridViewItem.DataSource = dt;
                GridViewItem.DataKeyNames = new string[] { "EMP_ITEM_EXP_ID" };
                GridViewItem.DataBind();

                
                 
                for (int i = 0; GridViewItemDept.Columns.Count > i;)
                {
                    GridViewItemDept.Columns.RemoveAt(i);
                }

            } else { 
                 USER_DATA_ID  = Convert.ToInt32(DropDownDepartmentID.SelectedValue);
                 USER_DATA_ID2 = Convert.ToInt32(DropDownDivisionID.SelectedValue);
                 USER_DATA_ID3 = Convert.ToInt32(DropDownLocationID.SelectedValue);
                 USER_DATA_ID4 = Convert.ToInt32(DropDownPlacementID.SelectedValue);
                 makeSQL = " select AEIE.*, AIE.ITEM_EXP_NAME, (EXTRACT (DAY FROM (AEIE.EXPIRES_DATE-SYSDATE)))  AS EXPIRED_DAYS_BET,  AEIE.EXPIRED_DAYS , AEIE.IS_ACTIVE, IAIP.PLACEMENT_NAME from IT_ASSET_EMP_ITEM_EXPIRES AEIE left join IT_ASSET_ITEM_EXPIRES AIE ON AIE.ITEM_EXP_ID = AEIE.ITEM_EXP_ID left join IT_ASSET_ITEMS_PLACEMENT IAIP ON IAIP.PLACEMENT_ID = AEIE.PLACEMENT_ID  where AEIE.DEPARTMENT_ID = '" + USER_DATA_ID + "' AND AEIE.DIVISION_ID = '" + USER_DATA_ID2 + "' AND AEIE.LOCATION_ID = '" + USER_DATA_ID3 + "' AND AEIE.PLACEMENT_ID = '" + USER_DATA_ID4 + "' order by AEIE.EMP_ITEM_EXP_ID";
                 
                string makeItemSQL = " SELECT IAEI.EMP_ITEMS_ID, AI.ITEM_NAME || ' - ' || AI.ITEM_TYPE || ' ' || AI.ITEM_BRAND AS ITEM_NAME_ALL from IT_ASSET_EMP_ITEMS IAEI  LEFT JOIN  IT_ASSET_ITEMS AI ON AI.ITEM_ID = IAEI.ITEM_ID  LEFT JOIN IT_ASSET_ITEM_CATEGORIES AIC ON AIC.ITEM_CATEGORY_ID = AI.ITEM_CATEGORY_ID WHERE IAEI.DEPARTMENT_ID = '" + USER_DATA_ID + "' AND IAEI.DIVISION_ID = '" + USER_DATA_ID2 + "' AND IAEI.LOCATION_ID = '" + USER_DATA_ID3 + "'  AND IAEI.PLACEMENT_ID = '" + USER_DATA_ID4 + "'  AND AI.IS_ACTIVE = 'Enable' AND (AIC.ITEM_CAT_QR_PRI_CODE = 'CPU' OR AIC.ITEM_CAT_QR_PRI_CODE = 'ITD') ORDER BY AI.ITEM_ID ASC";
                di = ExecuteBySqlString(makeItemSQL);
                dtItemID = (DataTable)di.Tables[0];
                DropDownItemID.DataSource = dtItemID;
                DropDownItemID.DataValueField = "EMP_ITEMS_ID";
                DropDownItemID.DataTextField = "ITEM_NAME_ALL";
                DropDownItemID.DataBind();
                DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));

                cmdl = new OracleCommand(makeSQL);
                oradata = new OracleDataAdapter(cmdl.CommandText, conn);
                ds = new DataTable();
                oradata.Fill(ds);
                GridViewItemDept.DataSource = ds;
                GridViewItemDept.DataKeyNames = new string[] { "EMP_ITEM_EXP_ID" };
                GridViewItemDept.DataBind();

                
                for (int i = 0; GridViewItem.Columns.Count > i;)
                {
                    GridViewItem.Columns.RemoveAt(i);
                } 
            } 

             DropDownItemID.Attributes.Remove("disabled"); 
             BtnAdd.Attributes.Add("aria-disabled", "true");
             BtnAdd.Attributes.Add("class", "btn btn-primary active");

             TextItemID.Text = ""; 
             DropDownItemID.Text = "0";
             DropDownItemExpID.Text = "0";
             TextSerialNo.Text = "";
             TextActivationCode.Text = "";
             TextExpiredDays.Text = "";
             TextStartExpiryDate.Text = ""; 
             conn.Close(); 
             alert_box.Visible = false; 
         }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);  
                int ItemID = Convert.ToInt32(DropDownItemID.Text);
                int ItemExpID = Convert.ToInt32(DropDownItemExpID.Text);
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                 
                string StartExpiryDate = TextStartExpiryDate.Text;
                string[] StartExpiryDateSplit = StartExpiryDate.Split('-');

                String StartDateTemp = StartExpiryDateSplit[0].Replace("/", "-");
                String StartDateTemp1 = StartDateTemp.Replace("M ", "M"); 
                DateTime StartDate = DateTime.ParseExact(StartDateTemp1, "dd-MM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                String ExpiryDateTemp = StartExpiryDateSplit[1].Replace("/", "-");
                String ExpiryDateTemp1 = ExpiryDateTemp.Trim();
                DateTime ExpiryDate = DateTime.ParseExact(ExpiryDateTemp1, "dd-MM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                 
                if (radExpSelect.SelectedValue == "Employee")
                {

                    string EmployeeID = DropDownEmployeeID.Text; 
                    string update_user = "update  IT_ASSET_EMP_ITEM_EXPIRES  set  EMP_ID = :NoEmployeeID, ITEM_EXP_ID = :NoItemExpID, ITEM_ID = :NoItemID, SERIAL_NO = :TextSerialNo, ACTIVATION_CODE = :TextActivationCode, START_DATE = : TextStartDate, EXPIRES_DATE = : TextExpiryDate, EXPIRED_DAYS = : TextExpiryDays, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where EMP_ITEM_EXP_ID = :TextEmpItemsExpID ";
                    cmdi = new OracleCommand(update_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[12];
                    objPrm[0] = cmdi.Parameters.Add("NoEmployeeID", EmployeeID);
                    objPrm[1] = cmdi.Parameters.Add("NoItemExpID", ItemExpID);
                    objPrm[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[3] = cmdi.Parameters.Add("TextSerialNo", TextSerialNo.Text);
                    objPrm[4] = cmdi.Parameters.Add("TextActivationCode", TextActivationCode.Text);
                    objPrm[5] = cmdi.Parameters.Add("TextStartDate", StartDate);
                    objPrm[6] = cmdi.Parameters.Add("TextExpiryDate", ExpiryDate);
                    objPrm[7] = cmdi.Parameters.Add("TextExpiryDays", TextExpiredDays.Text);
                    objPrm[8] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[9] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                    objPrm[10] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[11] = cmdi.Parameters.Add("TextEmpItemsExpID", TextItemExpID.Text);
                }
                else
                {
                    int DepartmentID = Convert.ToInt32(DropDownDepartmentID.Text);
                    int DivisionID = Convert.ToInt32(DropDownDivisionID.Text); 

                    string update_user = "update  IT_ASSET_EMP_ITEM_EXPIRES  set  DEPARTMENT_ID = :NoDepartmentID, DIVISION_ID =:NoDivisionID, ITEM_EXP_ID = :NoItemExpID, ITEM_ID = :NoItemID, SERIAL_NO = :TextSerialNo, ACTIVATION_CODE = :TextActivationCode, START_DATE = : TextStartDate, EXPIRES_DATE = : TextExpiryDate, EXPIRED_DAYS = : TextExpiryDays, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where EMP_ITEM_EXP_ID = :TextEmpItemsExpID ";
                    cmdi = new OracleCommand(update_user, conn); 
                    OracleParameter[] objPrm = new OracleParameter[13];
                    objPrm[0] = cmdi.Parameters.Add("NoDepartmentID", DepartmentID);
                    objPrm[1] = cmdi.Parameters.Add("NoDivisionID", DivisionID);
                    objPrm[2] = cmdi.Parameters.Add("NoItemExpID", ItemExpID);
                    objPrm[3] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[4] = cmdi.Parameters.Add("TextSerialNo", TextSerialNo.Text);
                    objPrm[5] = cmdi.Parameters.Add("TextActivationCode", TextActivationCode.Text);
                    objPrm[6] = cmdi.Parameters.Add("TextStartDate", StartDate);
                    objPrm[7] = cmdi.Parameters.Add("TextExpiryDate", ExpiryDate);
                    objPrm[8] = cmdi.Parameters.Add("TextExpiryDays", TextExpiredDays.Text);
                    objPrm[9] = cmdi.Parameters.Add("u_date", u_date);
                    objPrm[10] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                    objPrm[11] = cmdi.Parameters.Add("TextIsActive", ISActive);
                    objPrm[12] = cmdi.Parameters.Add("TextEmpItemsExpID", TextItemExpID.Text);  
                }  

                    cmdi.ExecuteNonQuery();
                    cmdi.Parameters.Clear();
                    cmdi.Dispose(); 
                    conn.Close();   

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Items Expire Update successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText();
              //  Display();
            }
                else { 
                    Response.Redirect("~/PagePermissionError.aspx");
                }
        }

        


        protected void BtnDelete_Click(object sender, EventArgs e)
        { 
           
            if (IS_DELETE_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                 
                string delete_user_page = " delete from IT_ASSET_EMP_ITEM_EXPIRES where EMP_ITEM_EXP_ID  = '" + TextItemExpID.Text + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();
                 
                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("Expire Data Delete successfully"));
                alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                clearText();
         

            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
         
        }

        public void Redio_CheckedChanged(object sender, EventArgs e)
        { 
            if (radExpSelect.SelectedValue == "Department")
            {
                ExpDept.Visible = true;
                ExpEmp.Visible = false;  
            }  else 
            {
                ExpDept.Visible = false;
                ExpEmp.Visible = true;
            } 
        }

        public void clearTextField(object sender, EventArgs e)
        {
            TextItemID.Text = ""; 
            DropDownEmployeeID.Text = "0";
            DropDownItemID.Text = "0";
            DropDownItemExpID.Text = "0";
            TextSerialNo.Text = "";
            TextActivationCode.Text = "";
            TextExpiredDays.Text = ""; 
            TextStartExpiryDate.Text = ""; 
            DropDownItemID.Attributes.Add("disabled", "disabled");
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active");
         //   GridViewItem.DataBind();
         //   GridViewItem.UseAccessibleHeader = true; 
            
        }

        public void clearText()
        {
            TextItemID.Text = "";  
            DropDownEmployeeID.Text = "0";
            DropDownItemID.Text = "0";
            DropDownItemExpID.Text = "0";
            TextSerialNo.Text = "";
            TextActivationCode.Text = "";
            TextExpiredDays.Text = ""; 
            TextStartExpiryDate.Text = "";  
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
        //    GridViewItem.DataBind();
         //   GridViewItem.UseAccessibleHeader = true; 

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