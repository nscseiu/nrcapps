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
    public partial class NrcUserProfile : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand  cmdl;
        OracleDataAdapter oradata;
        DataTable dt;
        int RowCount;

        string IS_PAGE_ACTIVE = "";
        string IS_ADD_ACTIVE = "";
        string IS_EDIT_ACTIVE = "";
        string IS_DELETE_ACTIVE = "";
        string IS_VIEW_ACTIVE = "";  
 
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
                   
                        int User_ID = Convert.ToInt32(Session["USER_ID"]);
                        ImageProfile.ImageUrl = "HandlerProfileImage.ashx?id="+User_ID.ToString();
                          
                        GetAllData();  

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

        protected void GetAllData() //Get all the data and bind it in HTLM Table       
        {
            int userID = Convert.ToInt32(Session["USER_ID"]);
            using (var conn = new OracleConnection(strConnString))
            {
                string query = "SELECT NU.USER_NAME, NU.CREATE_DATE, NU.UPDATE_DATE, HE.*, HED.DEPARTMENT_NAME, HEDIV.DIVISION_NAME, HEL.LOCATION_NAME FROM NRC_USER NU LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = NU.EMP_ID LEFT JOIN HR_EMP_DEPARTMENTS HED ON HED.DEPARTMENT_ID = HE.DEPARTMENT_ID LEFT JOIN HR_EMP_DIVISIONS HEDIV ON HEDIV.DIVISION_ID = HE.DIVISION_ID LEFT JOIN HR_EMP_LOCATIONS HEL ON HEL.LOCATION_ID = HE.LOCATION_ID WHERE NU.USER_ID = :NoUserID ";
                using (var cmd = new OracleCommand(query, conn))
                {
                     cmd.Parameters.Add("NoUserID", SqlDbType.Int);
                     cmd.Parameters["NoUserID"].Value = userID;
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


        public void Upload(object sender, EventArgs e)
         {
             OracleConnection conn = new OracleConnection(strConnString);
             conn.Open();

             int userID = Convert.ToInt32(Session["USER_ID"]);
             string filename = Path.GetFileName(FileUpload1.PostedFile.FileName);
             string contentType = FileUpload1.PostedFile.ContentType;
               
               using (Stream fs = FileUpload1.PostedFile.InputStream)
                 {
                     using (BinaryReader br = new BinaryReader(fs))
                     {
                         byte[] bytes = br.ReadBytes((Int32)fs.Length);
                         using (OracleConnection con = new OracleConnection(strConnString))
                         {
                             string query = "update  NRC_USER set USER_IMAGE_NAME = :Name, IMAGE_CONTENT_TYPE = :ContentType, IMAGE_DATA = :Data where USER_ID = :NoUserID ";
                             using (OracleCommand cmd = new OracleCommand(query))
                             {
                                 cmd.Connection = con;  
                                 OracleParameter[] objPrm = new OracleParameter[4];
                                 objPrm[0] = cmd.Parameters.Add("Name", filename);
                                 objPrm[1] = cmd.Parameters.Add("ContentType", contentType);
                                 objPrm[2] = cmd.Parameters.Add("Data", bytes);
                                 objPrm[3] = cmd.Parameters.Add("NoUserID", userID); 

                                 con.Open();
                                 cmd.ExecuteNonQuery();
                                 con.Close();
                             }
                         }  
                     }
                 }
           //  Response.Redirect(Request.Url.AbsoluteUri); 
             alert_box.Visible = true;
             alert_box.Controls.Add(new LiteralControl("Profile picture successfully uploaded."));
             alert_box.Attributes.Add("class", "alert alert-success alert-dismissible");

             GetAllData();
         }

         
                  
         

        public void clearTextField(object sender, EventArgs e)
        {
          //  TextUserID.Text         = "";
          //  TextUserName.Text       = ""; 
         //   DropDownEmployeeID.Text  = "0";
         //   TextPassword.Text       = "";
         //   CheckUsername.Text      = "";
          //  CheckEmpID.Text = "";
         //   DropDownUserRoleID.Text = "0";
         //   DropDownEmployeeID.Attributes.Remove("disabled");
         //   BtnAdd.Attributes.Add("aria-disabled", "true");
         //   BtnAdd.Attributes.Add("class", "btn btn-primary active"); 
            
        }

        public void clearText()
        {
         //   TextUserID.Text = "";
        //    TextUserName.Text = ""; 
          //  DropDownEmployeeID.Text = "0";
        //    TextPassword.Text = "";
        //    CheckUsername.Text = "";
         //   CheckEmpID.Text = "";
        //    DropDownUserRoleID.Text = "0";
          //  DropDownEmployeeID.Attributes.Remove("disabled");
        //    BtnAdd.Attributes.Add("aria-disabled", "false");
        //    BtnAdd.Attributes.Add("class", "btn btn-primary disabled"); 

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
         
        
   }
}