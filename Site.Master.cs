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
using System.Globalization;

namespace NRCAPPS
{
    public partial class Site : System.Web.UI.MasterPage
    {
       
        string strConnString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;  
        public DataTable TableData = new DataTable();  
        private Dictionary<string, HtmlGenericControl> ctrls = new Dictionary<string, HtmlGenericControl>();
 

          protected void Page_Load(object sender, EventArgs e)
           {
             
            setSelectedMenuItemClass();    
            int User_ID = Convert.ToInt32(Session["USER_ID"]);
            ImageProfile.ImageUrl = "HR/HandlerProfileImage.ashx?id=" + User_ID.ToString();
            ImageProfile1.ImageUrl = "HR/HandlerProfileImage.ashx?id=" + User_ID.ToString();
            ImageProfile2.ImageUrl = "HR/HandlerProfileImage.ashx?id=" + User_ID.ToString();

            // open data in html 
          //  GetAllData(); 
                 
            }

          /*     
           *     protected DataTable GetDataMainMenu()
             {
                 int userID = Convert.ToInt32(Session["USER_ID"]);
                 using (var conn = new OracleConnection(strConnString))
                 {
                     string query = " SELECT * FROM NRC_MAIN_MENU WHERE IS_ACTIVE = 'Enable' ORDER BY MENU_ORDER ASC "; //'" + Session["USER_ID"] + "'
                     using (var cmd = new OracleCommand(query, conn))
                     {
                         //  cmd.Parameters.Add("NoUserID", SqlDbType.Int);
                         //  cmd.Parameters["NoUserID"].Value = userID;
                         using (var sda = new OracleDataAdapter())
                         {
                             cmd.Connection = conn;
                             sda.SelectCommand = cmd;

                             using (TableData)
                             {
                                 TableData.Clear();
                                 sda.Fill(TableData);
                                 return TableData;
                             }
                         }
                     }
                 }
             }

             protected DataTable GetDataSubMenu(int PageID)
             {
                 int userID = Convert.ToInt32(Session["USER_ID"]);
                 using (var conn = new OracleConnection(strConnString))
                 {
                     string query = " SELECT NUP.MENU_ID, NUPP.USER_PAGE_ID, NUPP.IS_PAGE_ACTIVE, NUP.PAGE_NAME, NUP.PAGE_URL, MENU_PAGE_ID_NAME FROM NRC_USER_PAGE_PERMISSION NUPP LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NUPP.USER_PAGE_ID WHERE IS_PAGE_ACTIVE = 'Enable' AND USER_ID = 1 ORDER BY NUP.MENU_ID, NUPP.USER_PAGE_ID ASC "; //'" + Session["USER_ID"] + "'
                     using (var cmd = new OracleCommand(query, conn))
                     {
                         //  cmd.Parameters.Add("NoUserID", SqlDbType.Int);
                         //  cmd.Parameters["NoUserID"].Value = userID;
                         using (var sda = new OracleDataAdapter())
                         {
                             cmd.Connection = conn;
                             sda.SelectCommand = cmd;

                             using (TableData)
                             {
                                 TableData.Clear();
                                 sda.Fill(TableData);
                                 return TableData;
                             }
                         }
                     }
                 }
             }
           * 
             protected void GetAllData() //Get all the data and bind it in HTLM Table       
             {
                 int userID = Convert.ToInt32(Session["USER_ID"]);
                 using (var conn = new OracleConnection(strConnString))
                 {
                     string query = " SELECT  NUP.PAGE_URL, NUPP.IS_PAGE_ACTIVE FROM NRC_USER_PAGE_PERMISSION NUPP LEFT JOIN NRC_USER_PAGES NUP ON NUP.USER_PAGE_ID = NUPP.USER_PAGE_ID WHERE NUPP.USER_ID = '" + Session["USER_ID"] + "' AND NUP.IS_ACTIVE = 'Enable'  ";

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
           * */

          protected override void OnInit(EventArgs e)
        {
              ctrls.Add("Dashboard.aspx", Dashboard);

              ctrls.Add("PfBusinessTargetMat.aspx", pf_business_target);
              ctrls.Add("PfSupplier.aspx", pf_supplier);
              ctrls.Add("PfSupervisor.aspx", pf_supervisor);              
              ctrls.Add("PfPurchaseType.aspx", pf_purchase_type);
              ctrls.Add("PfItem.aspx", pf_item);
              ctrls.Add("PfSubItem.aspx", pf_sub_item);
              ctrls.Add("PfProductionShift.aspx", pf_prod_shift);
              ctrls.Add("PfMachine.aspx", pf_prod_machine);
              ctrls.Add("PfGaEstProd.aspx", pf_garbage_est);

              ctrls.Add("", pf_purchase_menu);
              ctrls.Add("PfPurchase.aspx", pf_purchase);
              ctrls.Add("PfPurchaseClaim.aspx", pf_purchase_claim);
              ctrls.Add("PfPurchaseClaimCheck.aspx", pf_purchase_claim_check);
              ctrls.Add("PfInventoryRm.aspx", pf_inventory_rm);
              ctrls.Add("PfProduction.aspx", pf_production);
              ctrls.Add("PfDailyPurProd.aspx", pf_daily_pur_prod);
              ctrls.Add("PfActualGarbage.aspx", pf_actual_garbage);
              ctrls.Add("PfInventoryFg.aspx", pf_inventory_fg);
              ctrls.Add("PfInventoryMonthlyStatement.aspx", pf_inventory_monthly_state);
              ctrls.Add("PfProcessingCost.aspx", pf_processing_cost);
              ctrls.Add("PfCustomer.aspx", pf_customer);
              ctrls.Add("PfSales.aspx", pf_sales);
              ctrls.Add("PfSalesReturn.aspx", pf_sales_return);
              ctrls.Add("PfSalesCheck.aspx", pf_sales_check);
              ctrls.Add("PfRmStatement.aspx", pf_rm_statement);
              ctrls.Add("PfFgStatement.aspx", pf_fg_statement);


              ctrls.Add("AssetCategory.aspx", it_asset_category);
              ctrls.Add("AssetItems.aspx", it_asset_items); 
              ctrls.Add("AssetItemsEmp.aspx", it_asset_items_emp);
              ctrls.Add("AssetItemsDept.aspx", it_asset_items_dept);
              ctrls.Add("AssetItemsExpires.aspx", it_asset_items_expires);
              ctrls.Add("AssetItemsExpiresEmp.aspx", it_asset_items_expires_emp);
              ctrls.Add("HrEmpLocation.aspx", hr_emp_location);
              ctrls.Add("HrEmpDivision.aspx", hr_emp_division);
              ctrls.Add("HrEmpDepartment.aspx", hr_emp_department);
              ctrls.Add("HrEmployee.aspx", hr_employee);
              ctrls.Add("NrcUser.aspx", nrc_user);
              ctrls.Add("NrcUserProfile.aspx", nrc_user_profile);
              ctrls.Add("NrcUserRole.aspx", nrc_user_role);
              ctrls.Add("NrcUserPages.aspx", nrc_user_pages);
              ctrls.Add("NrcUserRolePage.aspx", nrc_user_role_pages);
              ctrls.Add("NrcUserPagePermission.aspx", nrc_user_pages_permission);
              ctrls.Add("NrcDashboardItem.aspx", nrc_dashboard_items);
              ctrls.Add("NrcDashboardItemUser.aspx", nrc_dashboard_items_user);
              ctrls.Add("NrcDashboardItemOrder.aspx", nrc_dashboard_items_order);
              ctrls.Add("NrcVat.aspx", nrc_apps_vat);
              ctrls.Add("NrcPaymentType.aspx", nrc_apps_peyment_type);
              ctrls.Add("NrcDataProcess.aspx", nrc_data_process);
              ctrls.Add("NrcMainMenu.aspx", nrc_main_menu);

            base.OnInit(e);
        }
        
         
        protected void ButtonLogOut_Click(object sender, EventArgs e)
        {
            Session.Remove("USER_NAME"); 
            Session.Clear();
            Response.Redirect("~/Default.aspx");
        }

      
      
       private void setSelectedMenuItemClass()
        {
            
             string requestedFile = Path.GetFileName(Request.Path);
            if (!string.IsNullOrEmpty(requestedFile))
            {
                foreach (KeyValuePair<string, HtmlGenericControl> ctrl in ctrls)
                {
                    HtmlGenericControl aCtrl = ctrl.Value;
                    aCtrl.Attributes.Remove("class");
                }

                HtmlGenericControl selectedMenuItem;
                if (ctrls.TryGetValue(requestedFile, out selectedMenuItem))
                {
                    selectedMenuItem.Attributes.Add("class", "active"); 

                    if(requestedFile == "Dashboard.aspx"){
                    Dashboard.Attributes.Add("class", "active");
                    }
                     // pf menu
                    else if (requestedFile == "PfBusinessTargetMat.aspx" || requestedFile == "PfSupplier.aspx" || requestedFile == "PfSupervisor.aspx" || requestedFile == "PfPurchaseType.aspx" || requestedFile == "PfItem.aspx" || requestedFile == "PfSubItem.aspx" || requestedFile == "PfProductionShift.aspx" || requestedFile == "PfMachine.aspx" || requestedFile == "PfGaEstProd.aspx" || requestedFile == "PfPurchase.aspx" || requestedFile == "PfDailyPurProd.aspx" || requestedFile == "PfPurchaseClaim.aspx" || requestedFile == "PfPurchaseClaimCheck.aspx" || requestedFile == "PfInventoryRm.aspx" || requestedFile == "PfInventoryMonthlyStatement.aspx" || requestedFile == "PfProduction.aspx" || requestedFile == "PfActualGarbage.aspx" || requestedFile == "PfInventoryFg.aspx" || requestedFile == "PfProcessingCost.aspx" || requestedFile == "PfCustomer.aspx" || requestedFile == "PfSales.aspx" || requestedFile == "PfSalesCheck.aspx" || requestedFile == "PfSalesReturn.aspx" || requestedFile == "PfRmStatement.aspx" || requestedFile == "PfFgStatement.aspx")
                     {
                      plastic.Attributes.Add("class", "active");

                      if (requestedFile == "PfSupplier.aspx" || requestedFile == "PfPurchaseType.aspx" || requestedFile == "PfPurchase.aspx" || requestedFile == "PfPurchaseClaim.aspx" || requestedFile == "PfPurchaseClaimCheck.aspx")
                      {
                          pf_purchase_menu.Attributes.Add("class", "active");
                      }
                      else if (requestedFile == "PfProductionShift.aspx" || requestedFile == "PfMachine.aspx" || requestedFile == "PfGaEstProd.aspx" || requestedFile == "PfProduction.aspx" || requestedFile == "PfActualGarbage.aspx" || requestedFile == "PfProcessingCost.aspx")
                      {
                          pf_production_menu.Attributes.Add("class", "active");
                      }
                      else if (requestedFile == "PfInventoryRm.aspx" || requestedFile == "PfInventoryFg.aspx" || requestedFile == "PfDailyPurProd.aspx" || requestedFile == "PfInventoryMonthlyStatement.aspx")
                      {
                          pf_inventory_menu.Attributes.Add("class", "active");
                      }
                      else if (requestedFile == "PfCustomer.aspx" || requestedFile == "PfSales.aspx" || requestedFile == "PfSalesCheck.aspx" || requestedFile == "PfSalesReturn.aspx")
                      {
                          pf_sales_menu.Attributes.Add("class", "active");
                      }

                    }
                    else if (requestedFile == "AssetCategory.aspx" || requestedFile == "AssetItems.aspx" || requestedFile == "AssetItemsEmp.aspx" || requestedFile == "AssetItemsDept.aspx" || requestedFile == "AssetItemsExpires.aspx" || requestedFile == "AssetItemsExpiresEmp.aspx")
                    {
                      it.Attributes.Add("class", "active");

                      if (requestedFile == "AssetCategory.aspx" || requestedFile == "AssetItems.aspx" || requestedFile == "AssetItemsEmp.aspx" || requestedFile == "AssetItemsDept.aspx" || requestedFile == "AssetItemsExpires.aspx" || requestedFile == "AssetItemsExpiresEmp.aspx")
                      {
                         it_asset.Attributes.Add("class", "active");
                       }

                    } 
                    else if (requestedFile == "HrEmpLocation.aspx" || requestedFile == "HrEmpDivision.aspx" || requestedFile == "HrEmpDepartment.aspx" || requestedFile == "HrEmployee.aspx")
                    {
                      emp_setting.Attributes.Add("class", "active");
                    }
                    else if (requestedFile == "NrcUser.aspx" || requestedFile == "NrcUserProfile.aspx" || requestedFile == "NrcUserRole.aspx" || requestedFile == "NrcUserPages.aspx" || requestedFile == "NrcUserRolePage.aspx" || requestedFile == "NrcUserPagePermission.aspx" || requestedFile == "NrcMainMenu.aspx")
                    {
                      user_setting.Attributes.Add("class", "active"); 
                    }
                    else if (requestedFile == "NrcDashboardItem.aspx" || requestedFile == "NrcDashboardItemUser.aspx" || requestedFile == "NrcDashboardItemOrder.aspx")
                    {
                      nrc_dashboard_setting.Attributes.Add("class", "active"); 
                    }
                    else if (requestedFile == "NrcVat.aspx" || requestedFile == "NrcPaymentType.aspx" || requestedFile == "NrcDataProcess.aspx")
                    {
                      nrc_apps_setting.Attributes.Add("class", "active"); 
                    } 
                } 
            }  
        }


         
      }
    }
 