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

namespace NRCAPPS
{
    public partial class _Default : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        OracleCommand com;
        OracleDataAdapter oradata;
        DataTable dt;
        int RowCount;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_NAME"] != null)
            {
                Response.Redirect("Dashboard.aspx");
            }

            TextUserName.Focus();

        }

        protected void Button_Click_Login(object sender, EventArgs e)
        {
            try
            {
                string UserName = TextUserName.Text.Trim();
                string Password = TextPassword.Text.Trim();
                OracleConnection conn = new OracleConnection(strConnString);
                conn.Open();
               if(conn.State.ToString() == "Open")
                {
                string checkuser = "select count(*) AS temp from NRC_USER where IS_ACTIVE = 'Enable' AND USER_NAME = '" + UserName + "'";
                com = new OracleCommand(checkuser);
                oradata = new OracleDataAdapter(com.CommandText, conn);
                dt = new DataTable();
                oradata.Fill(dt);
                RowCount = dt.Rows.Count;
                string temp = "";

                for (int i = 0; i < RowCount; i++)
                {
                    temp = dt.Rows[i]["temp"].ToString();
                }

                conn.Close();
                if (temp == "1")
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT NU.USER_ID, NU.PASSWORD,  NUR.USER_ROLE_SHORT_NAME, NUR.USER_ROLE_NAME, NU.CREATE_DATE, HE.EMP_FNAME || ' ' || HE.EMP_LNAME AS EMP_NAME, nvl(NU.IS_PASSWORD_CHANGE,0), HD.DIVISION_NAME, HD.DIV_SHORT_NAME from NRC_USER NU LEFT JOIN HR_EMPLOYEES HE ON HE.EMP_ID = NU.EMP_ID LEFT JOIN HR_EMP_DIVISIONS HD ON HD.DIVISION_ID = HE.DIVISION_ID LEFT JOIN NRC_USER_ROLE  NUR ON NUR.USER_ROLE_ID = NU.USER_ROLE_ID  WHERE  NU.USER_NAME = '" + UserName + "'";
                    cmd.CommandType = CommandType.Text;

                    OracleDataReader dr = cmd.ExecuteReader();
                    int userID = 0, NrcIsPasswordChange = 0;
                    string passDB = "", NrcUserSrole = "", NrcUserRoleName = "", NrcUserName = "", HrDivisionName = "", HrDivShortName = "";

                    DateTime NrcUserCdate = DateTime.Today;

                    while (dr.Read())
                    {
                        if (!dr.IsDBNull(0))
                        {
                            userID = dr.GetInt32(0);
                            passDB = dr.GetString(1);
                            NrcUserSrole = dr.GetString(2);
                            NrcUserRoleName = dr.GetString(3);
                            NrcUserCdate = dr.GetDateTime(4);
                            NrcUserName = dr.GetString(5);
                            NrcIsPasswordChange = dr.GetInt32(6);
                            HrDivisionName = dr.GetString(7);
                            HrDivShortName = dr.GetString(8);
                        }
                    }

                    string pass_add = "1234567891234560";

                    OracleCommand cmdep = new OracleCommand();
                    cmdep.Connection = conn;
                    cmdep.CommandText = " SELECT get_dec_val ('" + passDB + "','" + pass_add + "') FROM dual";
                    cmdep.CommandType = CommandType.Text;
                    OracleDataReader dre = cmdep.ExecuteReader();

                    string EncryptoPassword = "";
                    while (dre.Read())
                    {
                        if (!dre.IsDBNull(0))
                        {
                            EncryptoPassword = dre.GetString(0);
                        }
                    }

                    if (EncryptoPassword == Password)
                    {
                        Session["USER_NAME"] = UserName;
                        Session["USER_ID"] = userID;
                        Session["NRC_USER_ROLE_NAME"] = NrcUserRoleName;
                        Session["EMP_NAME"] = NrcUserName;
                        Session["NRC_USER_SROLE"] = NrcUserSrole;
                        Session["NRC_USER_C_DATE"] = NrcUserCdate.ToLongDateString();
                        Session["NRC_DIVISION_NAME"] = HrDivisionName;
                        Session["NRC_DIV_SHORT_NAME"] = HrDivShortName;

                        string Emp_name_temp = Convert.ToString(Session["EMP_NAME"]);
                        string[] EmpName = Emp_name_temp.Split(' ');
                        Session["EMP_NAME_TWO"] = EmpName[0] + " " + EmpName[1];

                        if (NrcIsPasswordChange > 0) {
                            Response.Redirect("Dashboard.aspx"); 
                          }
                        else { Response.Redirect("NRC/NrcUserChangePassword.aspx");  }
                        
                    }
                    else
                    {
                        TextPassword.Focus();
                        LabelUserName.Text = "<p class='text-green'>User Name is correct!</p>";
                        LabelPassword.Text = ("<p class='text-red'>Password is not  correct!</p>");
                    }

                    conn.Close();
                }
                else
                {
                    LabelUserName.Text = "<p class='text-red'>User Name is not  correct!</p>";
                }

            } else {
                 Response.Redirect("~/ParameterError.aspx");
               } }
            catch (System.Data.SqlClient.SqlException ex)
            {
                Response.Redirect("~/ParameterError.aspx"); 
            }
        }
    }
}
