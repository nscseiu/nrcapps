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



namespace NRCAPPS.IT.IT_ASSET
{
    public partial class AssetItemsEmp : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand  cmdi, cmdl;
        OracleDataAdapter oradata;
        DataTable dt;
        int RowCount;
        public static int EmpStatus;

        string IS_PAGE_ACTIVE   = "";
        string IS_ADD_ACTIVE    = "";
        string IS_EDIT_ACTIVE   = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE   = "";
        string IS_REPORT_ACTIVE = "";

        public bool IsLoad { get; set; }  public bool IsLoad1 { get; set; }  
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_NAME"] != null)
            {
                string requestedFile = Path.GetFileName(Request.Path);  
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                string makeSQL = " SELECT  NUPP.IS_PAGE_ACTIVE, NUPP.IS_ADD_ACTIVE, NUPP.IS_EDIT_ACTIVE, NUPP.IS_DELETE_ACTIVE, NUPP.IS_VIEW_ACTIVE, NUPP.IS_REPORT_ACTIVE FROM NRC_USER_PAGE_PERMISSION NUPP LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NUPP.USER_PAGE_ID  WHERE NUPP.USER_ID = '" + Session["USER_ID"] + "' AND NUP.IS_ACTIVE = 'Enable' AND NUP.PAGE_URL = '" + requestedFile + "' ";

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
                    IS_REPORT_ACTIVE = dt.Rows[i]["IS_REPORT_ACTIVE"].ToString();  
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

                        DataTable dtEmpChangeID = new DataTable(); 
                        dtEmpChangeID = (DataTable)ds.Tables[0];
                        DropDownEmployeeIDChange.DataSource = dtEmpChangeID;
                        DropDownEmployeeIDChange.DataValueField = "EMP_ID";
                        DropDownEmployeeIDChange.DataTextField = "EMP_NAME";
                        DropDownEmployeeIDChange.DataBind();
                        DropDownEmployeeIDChange.Items.Insert(0, new ListItem("Select  Employee", "0"));

                        DataTable dtEmpChangeDeptID = new DataTable();
                        dtEmpChangeDeptID = (DataTable)ds.Tables[0];
                        DropDownEmployeeIDDept.DataSource = dtEmpChangeDeptID;
                        DropDownEmployeeIDDept.DataValueField = "EMP_ID";
                        DropDownEmployeeIDDept.DataTextField = "EMP_NAME";
                        DropDownEmployeeIDDept.DataBind();
                        DropDownEmployeeIDDept.Items.Insert(0, new ListItem("Select  Employee", "0"));

                        DataTable dtEmpReportID = new DataTable();
                        dtEmpReportID = (DataTable)ds.Tables[0];
                        DropDownEmployeeIDReport.DataSource = dtEmpReportID;
                        DropDownEmployeeIDReport.DataValueField = "EMP_ID";
                        DropDownEmployeeIDReport.DataTextField = "EMP_NAME";
                        DropDownEmployeeIDReport.DataBind();
                        DropDownEmployeeIDReport.Items.Insert(0, new ListItem("All  Employee", "1"));

                         
                        DataTable dtItemID = new DataTable();
                        DataSet di = new DataSet();
                        string makeItemSQL = " SELECT ITEM_ID, ITEM_NAME || ' - ' || ITEM_TYPE || ' ' || ITEM_BRAND AS ITEM_NAME_ALL from IT_ASSET_ITEMS WHERE IS_ACTIVE = 'Enable' ORDER BY ITEM_ID ASC";
                        di = ExecuteBySqlString(makeItemSQL);
                        dtItemID = (DataTable)di.Tables[0];
                        DropDownItemID.DataSource = dtItemID;
                        DropDownItemID.DataValueField = "ITEM_ID";
                        DropDownItemID.DataTextField = "ITEM_NAME_ALL";
                        DropDownItemID.DataBind();
                        DropDownItemID.Items.Insert(0, new ListItem("Select  Item", "0"));

                         

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

                         
                        DropDownItemID.Attributes.Add("disabled", "disabled");
                        DropDownItemIDChange.Attributes.Add("disabled", "disabled");
                        DropDownEmployeeIDChangeFor.Attributes.Add("disabled", "disabled");
                        DropDownItemIDChangeDept.Attributes.Add("disabled", "disabled");

                        TextQrImage.Visible = false; 
                        alert_box.Visible = false;
                        alert_box_right.Visible = false;
                        alert_box_right2.Visible = false;


                        BtnAdd.Attributes.Add("aria-disabled", "false");
                        BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
                        BtnUpdate.Attributes.Add("aria-disabled", "false");
                        BtnUpdate.Attributes.Add("class", "btn btn-success disabled");
                        BtnDelete.Attributes.Add("aria-disabled", "false");
                        BtnDelete.Attributes.Add("class", "btn btn-danger disabled");
                        BtnUpdateChangeItemEmp.Attributes.Add("aria-disabled", "false");
                        BtnUpdateChangeItemEmp.Attributes.Add("class", "btn btn-success disabled");
                        BtnUpdateChangeItemDept.Attributes.Add("aria-disabled", "false");
                        BtnUpdateChangeItemDept.Attributes.Add("class", "btn btn-success disabled");
                          
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
        //    {
                if (IS_ADD_ACTIVE == "Enable")
                {
                    OracleConnection conn = new OracleConnection(strConnString);
                    conn.Open();

                    int userID = Convert.ToInt32(Session["USER_ID"]);
                    string TextQRCodeAll = TextQRPreCode.Text + '-' + TextQRCode.Text;
                    int EmployeeID = Convert.ToInt32(DropDownEmployeeID.Text);
                    int ItemID     = Convert.ToInt32(DropDownItemID.Text); 
                    string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt"); 
                    string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";

                    string code = TextQRCodeAll;
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
                    System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
                    imgBarCode.Height = 125;
                    imgBarCode.Width = 125;
                    using (Bitmap bitMap = qrCode.GetGraphic(20))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);


                            bitMap.Save(Server.MapPath("~/IT/IT_ASSET/QRCode/" + code + ".png"));
                        }
                        plBarCode.Controls.Add(imgBarCode);
                    }
                     string ImageUrl = code+"."+"png";

                     string insert_user = "insert into IT_ASSET_EMP_ITEMS (EMP_ITEMS_ID, EMP_ID, ITEM_ID, IMAGE_QR_CODE, CREATE_DATE, U_USER_ID, IS_ACTIVE) VALUES ( :TextEmpItemsID, :NoEmployeeID, :NoItemID, :TextImageQRCode, TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM'), :NoC_USER_ID, :TextIsActive)";
                    cmdi = new OracleCommand(insert_user, conn);

                    OracleParameter[] objPrm = new OracleParameter[7];
                    objPrm[0] = cmdi.Parameters.Add("TextEmpItemsID", TextQRCodeAll);
                    objPrm[1] = cmdi.Parameters.Add("NoEmployeeID", EmployeeID);
                    objPrm[2] = cmdi.Parameters.Add("NoItemID", ItemID);
                    objPrm[3] = cmdi.Parameters.Add("TextImageQRCode", ImageUrl);
                    objPrm[4] = cmdi.Parameters.Add("u_date", u_date); 
                    objPrm[5] = cmdi.Parameters.Add("NoC_USER_ID", userID); 
                    objPrm[6] = cmdi.Parameters.Add("TextIsActive", ISActive);
                     
                    cmdi.ExecuteNonQuery();

                    cmdi.Parameters.Clear();
                    cmdi.Dispose();
                    conn.Close();
                    alert_box.Visible = true;
                    alert_box.Controls.Add(new LiteralControl("Insert New IT Asset Item successfully"));
                    alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");
                    clearText(); 
                    GridViewItem.DataBind();
                }
                else { 
                    Response.Redirect("~/PagePermissionError.aspx");
                }
          //  }
          //  catch
         //   {
         //       Response.Redirect("~/ParameterError.aspx");
         //   }
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
             string makeSQL = " select * from IT_ASSET_EMP_ITEMS where EMP_ITEMS_ID = '" + USER_DATA_ID + "'";
             
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn); 
             dt = new DataTable();
             oradata.Fill(dt);
             RowCount = dt.Rows.Count;
             
             for (int i = 0; i < RowCount; i++)
             {
                 string[] TextQRPreCodeAll = dt.Rows[i]["EMP_ITEMS_ID"].ToString().Split('-');
                 TextQRPreCode.Text        = TextQRPreCodeAll[0];
                 TextQRCode.Text           = TextQRPreCodeAll[1];
                 DropDownItemID.Text       = dt.Rows[i]["ITEM_ID"].ToString();
                 DropDownEmployeeID.Text   = dt.Rows[i]["EMP_ID"].ToString();
                 TextQrImage.ImageUrl      = "QRCode/"+ dt.Rows[i]["IMAGE_QR_CODE"].ToString();
                 CheckIsActive.Checked     = Convert.ToBoolean(dt.Rows[i]["IS_ACTIVE"].ToString() == "Enable" ? true : false);
             } 
             
             conn.Close(); 
             CheckQRCode.Text = "";
             alert_box.Visible = false;
             BtnAdd.Attributes.Add("aria-disabled", "false");
             BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
             
             BtnUpdate.Attributes.Add("aria-disabled", "true");
             BtnUpdate.Attributes.Add("class", "btn btn-success active");
             BtnDelete.Attributes.Add("aria-disabled", "true");
             BtnDelete.Attributes.Add("class", "btn btn-danger active");
              
             alert_box_right.Visible = false;
             alert_box_right2.Visible = false;
             GridViewItem.UseAccessibleHeader = true;
             GridViewItem.HeaderRow.TableSection = TableRowSection.TableHeader;
             TextQrImage.Visible = true;
             
        }

        public void DisplayItemSearch(object sender, EventArgs e)
        {

        OracleConnection conn = new OracleConnection(strConnString);
        conn.Open(); 
        string USER_DATA_ID = TextEmpItemsID.Text;

        if (USER_DATA_ID != "")
        {
            string makeSQL = " select CASE WHEN AEI.EMP_ID IS NOT NULL  THEN HE.EMP_ID || '-' || HE.EMP_FNAME || ' ' || HE.EMP_LNAME WHEN AEI.EMP_ID IS NULL  THEN HEDE.DEPARTMENT_NAME || ' Department' || ' in ' ||HEDI.DIVISION_NAME || ' Division'  END AS ASSET_USER_NAME, AEI.EMP_ITEMS_ID, AEI.IMAGE_QR_CODE, AEI.IS_ACTIVE, AI.ITEM_NAME, AI.ITEM_TYPE, AI.ITEM_BRAND from IT_ASSET_EMP_ITEMS AEI left join IT_ASSET_ITEMS AI ON AI.ITEM_ID = AEI.ITEM_ID left join HR_EMPLOYEES HE ON HE.EMP_ID = AEI.EMP_ID left join HR_EMP_DEPARTMENTS HEDE ON HEDE.DEPARTMENT_ID = AEI.DEPARTMENT_ID left join HR_EMP_DIVISIONS HEDI ON HEDI.DIVISION_ID = AEI.DIVISION_ID where AEI.EMP_ITEMS_ID = '" + USER_DATA_ID + "' order by AEI.EMP_ITEMS_ID";

            cmdl = new OracleCommand(makeSQL);
            oradata = new OracleDataAdapter(cmdl.CommandText, conn);
            dt = new DataTable();
            oradata.Fill(dt);

            RowCount = dt.Rows.Count;
            string AssetUserName = "", EmpItemID = "", ItemName = "", ItemType = "", ItemBrand = "", QrCodeID = "";
            for (int i = 0; i < RowCount; i++)
            {
                 AssetUserName = dt.Rows[i]["ASSET_USER_NAME"].ToString();
                 EmpItemID     = dt.Rows[i]["EMP_ITEMS_ID"].ToString();
                 ItemName      = dt.Rows[i]["ITEM_NAME"].ToString();
                 ItemType      = dt.Rows[i]["ITEM_TYPE"].ToString();
                 ItemBrand     = dt.Rows[i]["ITEM_BRAND"].ToString();
                 QrCodeID      = dt.Rows[i]["IMAGE_QR_CODE"].ToString();
            }

            if (dt.Rows.Count > 0)
            {
                CheckItemSearch.Text = "<div class='alert alert-success alert-dismissible'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button><h4><i class='icon fa fa-check'></i> Search Result!</h4><div style='float:clear;'><div style='float:clear;'><div style='float:left;padding-right:5px;display:inline;vertical-align: middle;'><img src='QRCode/" + QrCodeID + "' style='height:45px;width:45px;border-width:0px;'></div><div style='display:inline;vertical-align: middle;'>" + EmpItemID + " - " + ItemName + " - " + ItemType + " - " + ItemBrand + ". </br><span class='bg-yellow'> &nbsp; Using -" + AssetUserName + " &nbsp;</span></div></div></div></div>";
             }
            else
            {
                CheckItemSearch.Text = "<div class='alert alert-danger alert-dismissible'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button><h4><i class='icon fa fa-ban'></i> Search Result!</h4>This QR Code ID Asset Item not available.</br></br></div>";
             }

        }
        else
        {

            alert_box.Visible = false;
            CheckItemSearch.Text = "";
            TextEmpItemsID.Focus(); 
        } 

            TextQrImage.Visible = false;
            TextQrImage.ImageUrl = null;

            TextQRPreCode.Text = "";
            TextQRCode.Text = "";
            conn.Close();
            alert_box.Visible = false;
            alert_box_right.Visible = false;
            alert_box_right2.Visible = false;
        }

        protected void DisplayItemCatQRPriCode(object sender, EventArgs e) 
        { 
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();

             int USER_DATA_ID = Convert.ToInt32(DropDownItemID.Text); 
              
             DataTable dtUserTypeID = new DataTable();
             DataSet ds = new DataSet();
             string makeSQL = " select AIC.ITEM_CAT_QR_PRI_CODE  from IT_ASSET_ITEMS AI left join IT_ASSET_ITEM_CATEGORIES AIC ON AIC.ITEM_CATEGORY_ID = AI.ITEM_CATEGORY_ID  where AI.ITEM_ID = '" + USER_DATA_ID + "'";
             
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
             CheckQRCode.Text = "";
             alert_box.Visible = false;
             alert_box_right.Visible = false;
             alert_box_right2.Visible = false;
             BtnAdd.Attributes.Add("aria-disabled", "false");
             BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
             
             if (EmpStatus == 0)
             { 
              //   PlaceHolder1.Text = "if";
             }
             else
             { 
                 GridViewItem.UseAccessibleHeader = true;
                 GridViewItem.HeaderRow.TableSection = TableRowSection.TableHeader; 
             }
             
        }
    
        
         public void DisplayEmpItem(object sender, EventArgs e)
         {

             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();

             int USER_DATA_ID = 0;
             USER_DATA_ID = Convert.ToInt32(DropDownEmployeeID.SelectedValue);
              
             string makeSQL = " select AEI.EMP_ITEMS_ID, AEI.ITEM_ID, AEI.IMAGE_QR_CODE, AEI.IS_ACTIVE, AI.ITEM_NAME, AI.ITEM_TYPE, AI.ITEM_BRAND from IT_ASSET_EMP_ITEMS AEI left join IT_ASSET_ITEMS AI ON AI.ITEM_ID = AEI.ITEM_ID where AEI.EMP_ID = '" + USER_DATA_ID + "' order by AEI.EMP_ITEMS_ID";
               
             cmdl = new OracleCommand(makeSQL);
             oradata = new OracleDataAdapter(cmdl.CommandText, conn);
             dt = new DataTable();
             oradata.Fill(dt);
             GridViewItem.DataSource = dt;
             GridViewItem.DataKeyNames = new string[] { "EMP_ITEMS_ID" };
             GridViewItem.DataBind();

             if (dt.Rows.Count == 0)
             {
                 EmpStatus = 0;
             }
             else
             { 
                 GridViewItem.UseAccessibleHeader = true;
                 GridViewItem.HeaderRow.TableSection = TableRowSection.TableHeader;
                 EmpStatus = 1;
                 
             } 
             DropDownItemID.Attributes.Remove("disabled");
              
             TextQrImage.Visible = false; 
             TextQrImage.ImageUrl = null; 

             TextQRPreCode.Text = "";
             TextQRCode.Text = ""; 
             conn.Close(); 
             alert_box.Visible = false;
             alert_box_right.Visible = false;
             alert_box_right2.Visible = false;
         }

         public void DisplayEmpChangeItem(object sender, EventArgs e)
         {

             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
            
             if (EmpStatus == 1)
             {
                 GridViewItem.UseAccessibleHeader = true;
                 GridViewItem.HeaderRow.TableSection = TableRowSection.TableHeader;
             }

             int USER_DATA_ID = 0;
             USER_DATA_ID = Convert.ToInt32(DropDownEmployeeIDChange.SelectedValue);

             DataTable dtItemChangeID = new DataTable();
             DataSet di = new DataSet();
             string makeItemChangeSQL = " SELECT AEI.EMP_ITEMS_ID, AEI.EMP_ITEMS_ID || ' - ' || AI.ITEM_NAME || ' - ' || AI.ITEM_TYPE || ' ' || AI.ITEM_BRAND AS ITEM_NAME_ALL from   IT_ASSET_EMP_ITEMS AEI LEFT JOIN IT_ASSET_ITEMS AI ON AI.ITEM_ID = AEI.ITEM_ID WHERE AEI.IS_ACTIVE = 'Enable' AND AEI.EMP_ID = '" + USER_DATA_ID + "' ORDER BY AI.ITEM_ID ASC";
             di = ExecuteBySqlString(makeItemChangeSQL);
             dtItemChangeID = (DataTable)di.Tables[0];
             DropDownItemIDChange.DataSource = dtItemChangeID;
             DropDownItemIDChange.DataValueField = "EMP_ITEMS_ID";
             DropDownItemIDChange.DataTextField = "ITEM_NAME_ALL";
             DropDownItemIDChange.DataBind();
             DropDownItemIDChange.Items.Insert(0, new ListItem("Select  Item", "0"));

             if (di.Tables[0].Rows.Count == 0)
             {
                BtnUpdateChangeItemEmp.Attributes.Add("aria-disabled", "false");
                BtnUpdateChangeItemEmp.Attributes.Add("class", "btn btn-success disabled");
                DropDownItemIDChange.Attributes.Add("disabled", "disabled");
                DropDownEmployeeIDChangeFor.Attributes.Add("disabled", "disabled");
             }
             else {   
                DropDownItemIDChange.Attributes.Remove("disabled");
                DropDownEmployeeIDChangeFor.Attributes.Remove("disabled");
             }
              
             DataTable dtEmpChangeForID = new DataTable();
             DataSet ds = new DataSet();
             string makeEmpSQL = " SELECT EMP_ID, EMP_ID || ' - ' || EMP_FNAME || ' ' || EMP_LNAME AS EMP_NAME from HR_EMPLOYEES WHERE IS_ACTIVE = 'Enable' AND EMP_ID != '" + USER_DATA_ID + "'   ORDER BY EMP_ID ASC";
             ds = ExecuteBySqlString(makeEmpSQL);
             dtEmpChangeForID = (DataTable)ds.Tables[0];
             DropDownEmployeeIDChangeFor.DataSource = dtEmpChangeForID;
             DropDownEmployeeIDChangeFor.DataValueField = "EMP_ID";
             DropDownEmployeeIDChangeFor.DataTextField = "EMP_NAME";
             DropDownEmployeeIDChangeFor.DataBind();
             DropDownEmployeeIDChangeFor.Items.Insert(0, new ListItem("Select  Employee", "0"));
               
             conn.Close(); 
             alert_box.Visible = false;
             alert_box_right.Visible = false;
             alert_box_right2.Visible = false;
         }

         public void EmpChangeItemBtn(object sender, EventArgs e)
         {
             BtnUpdateChangeItemEmp.Attributes.Add("aria-disabled", "true");
             BtnUpdateChangeItemEmp.Attributes.Add("class", "btn btn-success active");
              
             alert_box.Visible = false;
             alert_box_right.Visible = false;
             alert_box_right2.Visible = false;
         }

         public void DisplayEmpDeptChangeItem(object sender, EventArgs e)
         {

             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();
              
             int USER_DATA_ID = 0;
             USER_DATA_ID = Convert.ToInt32(DropDownEmployeeIDDept.SelectedValue);

             DataTable dtItemChangeID = new DataTable();
             DataSet di = new DataSet();
             string makeItemChangeSQL = " SELECT AEI.EMP_ITEMS_ID, AEI.EMP_ITEMS_ID || ' - ' || AI.ITEM_NAME || ' - ' || AI.ITEM_TYPE || ' ' || AI.ITEM_BRAND AS ITEM_NAME_ALL from   IT_ASSET_EMP_ITEMS AEI LEFT JOIN IT_ASSET_ITEMS AI ON AI.ITEM_ID = AEI.ITEM_ID WHERE AEI.IS_ACTIVE = 'Enable' AND AEI.EMP_ID = '" + USER_DATA_ID + "' ORDER BY AI.ITEM_ID ASC";
             di = ExecuteBySqlString(makeItemChangeSQL);
             dtItemChangeID = (DataTable)di.Tables[0];
             DropDownItemIDChangeDept.DataSource = dtItemChangeID;
             DropDownItemIDChangeDept.DataValueField = "EMP_ITEMS_ID";
             DropDownItemIDChangeDept.DataTextField = "ITEM_NAME_ALL";
             DropDownItemIDChangeDept.DataBind();
             DropDownItemIDChangeDept.Items.Insert(0, new ListItem("Select  Item", "0"));

             if (di.Tables[0].Rows.Count == 0)
             {
                 BtnUpdateChangeItemDept.Attributes.Add("aria-disabled", "false");
                 BtnUpdateChangeItemDept.Attributes.Add("class", "btn btn-success disabled");
                 DropDownItemIDChangeDept.Attributes.Add("disabled", "disabled");
                 DropDownEmployeeIDChangeFor.Attributes.Add("disabled", "disabled");
             }
             else
             {
                 DropDownItemIDChangeDept.Attributes.Remove("disabled"); 
             }

             DataTable dtEmpChangeForID = new DataTable();
             DataSet ds = new DataSet();
             string makeEmpSQL = " SELECT EMP_ID, EMP_ID || ' - ' || EMP_FNAME || ' ' || EMP_LNAME AS EMP_NAME from HR_EMPLOYEES WHERE IS_ACTIVE = 'Enable' AND EMP_ID != '" + USER_DATA_ID + "'   ORDER BY EMP_ID ASC";
             ds = ExecuteBySqlString(makeEmpSQL);
             dtEmpChangeForID = (DataTable)ds.Tables[0];
             DropDownEmployeeIDChangeFor.DataSource = dtEmpChangeForID;
             DropDownEmployeeIDChangeFor.DataValueField = "EMP_ID";
             DropDownEmployeeIDChangeFor.DataTextField = "EMP_NAME";
             DropDownEmployeeIDChangeFor.DataBind();
             DropDownEmployeeIDChangeFor.Items.Insert(0, new ListItem("Select  Employee", "0"));

             conn.Close();
             alert_box.Visible = false;
             alert_box_right.Visible = false;

         }

         public void DeptChangeItemBtn(object sender, EventArgs e)
         {
             BtnUpdateChangeItemDept.Attributes.Add("aria-disabled", "true");
             BtnUpdateChangeItemDept.Attributes.Add("class", "btn btn-success active");
              
             alert_box.Visible = false;
             alert_box_right.Visible = false;
             alert_box_right2.Visible = false;
         }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]); 
                string TextQRCodeAll = TextQRPreCode.Text + '-' + TextQRCode.Text;
                int EmployeeID = Convert.ToInt32(DropDownEmployeeID.Text);
                int ItemID = Convert.ToInt32(DropDownItemID.Text);
                string ISActive = CheckIsActive.Checked ? "Enable" : "Disable";
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");
                  
                string update_user = "update  IT_ASSET_EMP_ITEMS  set  EMP_ID = :NoEmployeeID, ITEM_ID = :NoItemID,  UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID, IS_ACTIVE = :TextIsActive where EMP_ITEMS_ID = :TextEmpItemsID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[6];
                objPrm[0] = cmdi.Parameters.Add("NoEmployeeID", EmployeeID);
                objPrm[1] = cmdi.Parameters.Add("NoItemID", ItemID); 
                objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[3] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[4] = cmdi.Parameters.Add("TextIsActive", ISActive);
                objPrm[5] = cmdi.Parameters.Add("TextEmpItemsID", TextQRCodeAll);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();  

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("IT Asset Items Update Successfully"));
                alert_box.Attributes.Add("class", "alert alert-success alert-dismissible"); 
                clearText(); 
            }
                else { 
                    Response.Redirect("~/PagePermissionError.aspx");
                }
        }

        protected void BtnUpdateChangeItemEmp_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);  
                int EmployeeIDChangeFor = Convert.ToInt32(DropDownEmployeeIDChangeFor.Text); 
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");


                string update_user = "update  IT_ASSET_EMP_ITEMS  set  EMP_ID = :NoEmployeeIDChangeFor, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID where EMP_ITEMS_ID = :TextEmpItemsID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[4];
                objPrm[0] = cmdi.Parameters.Add("NoEmployeeIDChangeFor", EmployeeIDChangeFor); 
                objPrm[1] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[2] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[3] = cmdi.Parameters.Add("TextEmpItemsID", DropDownItemIDChange.Text);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();  

                alert_box.Visible = false;
                alert_box_right2.Visible = false;
                alert_box_right.Visible = true;
                alert_box_right.Controls.Add(new LiteralControl("IT Asset Item Change Employee to Employee Successfully"));
                alert_box_right.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearTextChange();

                BtnUpdateChangeItemEmp.Attributes.Add("aria-disabled", "false");
                BtnUpdateChangeItemEmp.Attributes.Add("class", "btn btn-success disabled");

               
            }
            else { 
                Response.Redirect("~/PagePermissionError.aspx");
            }
        }

        protected void BtnUpdateChangeItemDept_Click(object sender, EventArgs e)
        {
            if (IS_EDIT_ACTIVE == "Enable")
            {
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                int userID = Convert.ToInt32(Session["USER_ID"]);
                string EmpItemsID = DropDownItemIDChangeDept.Text; 
                string u_date = System.DateTime.Now.ToString("dd-MM-yyyy h:mm:ss tt");


                string update_user = "update  IT_ASSET_EMP_ITEMS  set  EMP_ID = NULL, DEPARTMENT_ID = :NoDepartmentID, DIVISION_ID = :NoDivisionID, UPDATE_DATE = TO_DATE(:u_date, 'DD-MM-YYYY HH:MI:SS AM') , U_USER_ID = :NoC_USER_ID where EMP_ITEMS_ID = :TextEmpItemsID ";
                cmdi = new OracleCommand(update_user, conn);  

                OracleParameter[] objPrm = new OracleParameter[5]; 
                objPrm[0] = cmdi.Parameters.Add("NoDepartmentID", DropDownDepartmentID.Text);
                objPrm[1] = cmdi.Parameters.Add("NoDivisionID", DropDownDivisionID.Text); 
                objPrm[2] = cmdi.Parameters.Add("u_date", u_date);
                objPrm[3] = cmdi.Parameters.Add("NoC_USER_ID", userID);
                objPrm[4] = cmdi.Parameters.Add("TextEmpItemsID", EmpItemsID);

                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose(); 
                conn.Close();  

                alert_box.Visible = false;
                alert_box_right.Visible = false;
                alert_box_right2.Visible = true;
                alert_box_right2.Controls.Add(new LiteralControl("Item Change Employee to Department Successfully"));
                alert_box_right2.Attributes.Add("class", "alert alert-success alert-dismissible");
                clearTextChange();

                BtnUpdateChangeItemDept.Attributes.Add("aria-disabled", "false");
                BtnUpdateChangeItemDept.Attributes.Add("class", "btn btn-success disabled");

               
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

                string TextQRCodeAll = TextQRPreCode.Text + '-' + TextQRCode.Text; 
                string delete_user_page = " delete from IT_ASSET_EMP_ITEMS where EMP_ITEMS_ID  = '" + TextQRCodeAll + "'";

                cmdi = new OracleCommand(delete_user_page, conn);
            
                cmdi.ExecuteNonQuery();
                cmdi.Parameters.Clear();
                cmdi.Dispose();
                conn.Close();

                string filePath = Server.MapPath("~/IT/IT_ASSET/"+TextQrImage.ImageUrl);
                if (System.IO.File.Exists(filePath))
                { 
                  System.IO.File.Delete(filePath); 
                }

                alert_box.Visible = true;
                alert_box.Controls.Add(new LiteralControl("IT Asset Items Delete Successfully"));
                alert_box.Attributes.Add("class", "alert alert-danger alert-dismissible");
                clearText();
                TextQrImage.Visible = false; 

            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
         
        }
        protected void BtnReport_Click(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad = true;
                TextQrImage.Visible = false;
                alert_box.Visible = false;
                alert_box_right.Visible = false;
                alert_box_right2.Visible = false;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
         
        }

        protected void BtnReport1_Click(object sender, EventArgs e)
        {

            if (IS_REPORT_ACTIVE == "Enable")
            {

                IsLoad1 = true;
                TextQrImage.Visible = false;
                alert_box.Visible = false;
                alert_box_right.Visible = false;
                alert_box_right2.Visible = false;
            }
            else
            {
                Response.Redirect("~/PagePermissionError.aspx");
            }
         
        }         
        public void clearTextField(object sender, EventArgs e)
        {
            TextItemID.Text = "";  
            TextQRCode.Text = ""; 
            DropDownEmployeeID.Text = "0";
            DropDownItemID.Text = "0";
            DropDownItemID.Attributes.Add("disabled", "disabled");
            BtnAdd.Attributes.Add("aria-disabled", "true");
            BtnAdd.Attributes.Add("class", "btn btn-primary active");
        //  GridViewItem.DataBind();
            GridViewItem.UseAccessibleHeader = true;

            DataTable ds = new DataTable();
            ds = null;
            GridViewItem.DataSource = ds;
            GridViewItem.DataBind();
            
        }
        public void clearTextFieldSearch(object sender, EventArgs e)
        {
            TextEmpItemsID.Text = "";
            CheckItemSearch.Text = "";
        }
        public void clearTextChangeField(object sender, EventArgs e)
        {
            DropDownEmployeeIDChange.Text = "0"; 
            DropDownItemIDChange.Text = "0";
            DropDownEmployeeIDChangeFor.Text = "0";
            DropDownEmployeeIDDept.Text = "0";
            DropDownItemIDChangeDept.Text = "0";
            DropDownDepartmentID.Text = "0";
            DropDownDivisionID.Text = "0";
          
        }

        public void clearText()
        {
            TextItemID.Text = "";  
            TextQRCode.Text = "";
            TextQRPreCode.Text = "";
            CheckQRCode.Text = "";
            DropDownEmployeeID.Text = "0";
            DropDownItemID.Text = "0"; 
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
            GridViewItem.DataBind();
            GridViewItem.UseAccessibleHeader = true; 

        }

        public void clearTextChange()
        {
            TextItemID.Text = "";
            TextQRCode.Text = "";
            DropDownEmployeeID.Text = "0";
            DropDownItemID.Text = "0";
            DropDownEmployeeIDChange.Text = "0";
            DropDownItemIDChange.Text = "0";
            DropDownEmployeeIDChangeFor.Text = "0";
            BtnAdd.Attributes.Add("aria-disabled", "false");
            BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
            if (EmpStatus == 1)
            {
                DataTable ds = new DataTable();
                ds = null;
                GridViewItem.DataSource = ds;
                GridViewItem.DataBind();

             //   GridViewItem.DataBind();
             //   GridViewItem.UseAccessibleHeader = true;
           //   GridViewItem.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            DropDownEmployeeIDDept.Text = "0";
            DropDownItemIDChangeDept.Text = "0";
            DropDownDepartmentID.Text = "0";
            DropDownDivisionID.Text = "0";  
            DropDownItemIDChange.Attributes.Add("disabled", "disabled");
            DropDownItemIDChangeDept.Attributes.Add("disabled", "disabled");
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
         
        public void TextQRCode_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TextQRCode.Text))
            {
                alert_box.Visible = false; 
                if (EmpStatus == 1) { 
                    GridViewItem.UseAccessibleHeader = true;
                    GridViewItem.HeaderRow.TableSection = TableRowSection.TableHeader; 
                }
                
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                 
                string TextQRCodeAll = TextQRPreCode.Text+"-"+TextQRCode.Text;

                cmd.CommandText = "select * from IT_ASSET_EMP_ITEMS where EMP_ITEMS_ID = '" + TextQRCodeAll + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    CheckQRCode.Text = "<label class='control-label'><i class='fa fa-times-circle-o'></i> Item name is already exist</label>";
                    CheckQRCode.ForeColor = System.Drawing.Color.Red;
                    TextQRCode.Focus();
                    BtnAdd.Attributes.Add("aria-disabled", "false");
                    BtnAdd.Attributes.Add("class", "btn btn-primary disabled");
   
                }
                else
                {
                    CheckQRCode.Text = "<label class='control-label'><i class='fa fa fa-check'></i> Item name is available</label>";
                    CheckQRCode.ForeColor = System.Drawing.Color.Green;
                   
                    BtnAdd.Attributes.Add("aria-disabled", "true");
                    BtnAdd.Attributes.Add("class", "btn btn-primary active");

                    string code = TextQRPreCode.Text + '-' + TextQRCode.Text;
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
                    System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
                    imgBarCode.Height = 125;
                    imgBarCode.Width = 125;
                    using (Bitmap bitMap = qrCode.GetGraphic(20))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                             
                          //  bitMap.Save(Server.MapPath("~/ASSET/QRCode/" + code + ".png"));
                        }
                        plBarCode.Controls.Add(imgBarCode);
                    }
                   // PlaceHolder1.Text = code + "." + "png";
                    TextQrImage.Visible = false;
                    TextQrImage.ImageUrl = null; 
                }
            }
            else {
                    CheckQRCode.Text = "<label class='control-label'><i class='fa fa-hand-o-left'></i> Item name is not blank</label>";
                    CheckQRCode.ForeColor = System.Drawing.Color.Red;
                    TextQRCode.Focus();
            }
            
        } 
   }
}