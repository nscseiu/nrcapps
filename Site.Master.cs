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
              
            ctrls.Add("WpParty.aspx", wp_party);
            ctrls.Add("WpSupplierCat.aspx", wp_supplier_cat);
            ctrls.Add("WpDriver.aspx", wp_driver);
            ctrls.Add("WpCollectionFor.aspx", wp_collection_for);
            ctrls.Add("WpSupervisor.aspx", wp_supervisor);
            ctrls.Add("WpCategory.aspx", wp_category);
            ctrls.Add("WpItem.aspx", wp_item);  
            ctrls.Add("WpItemSales.aspx", wp_item_sales);  
            ctrls.Add("WpItemSalesWs.aspx", wp_item_sales_ws); 
            ctrls.Add("WpPurchase.aspx", wp_purchase);
            ctrls.Add("WpPurchaseVatAdjustment.aspx", wp_purchase_vat_adjustment);            
            ctrls.Add("WpPurchaseClaim.aspx", wp_purchase_claim);
            ctrls.Add("WpPurchaseClaimApprove.aspx", wp_purchase_claim_approve);
            ctrls.Add("WpMaterialTransaction.aspx", wp_material_transaction); 
            ctrls.Add("WpInventoryRm.aspx", wp_inventory_rm); 
            ctrls.Add("WpDailyPurSales.aspx", wp_daily_purchase_sales); 
            ctrls.Add("WpInventoryMonthlyStatement.aspx", wp_inventory_monthly_state); 
            ctrls.Add("WpSales.aspx", wp_sales);   
            ctrls.Add("WpSalesInterDivision.aspx", wp_sales_inter_division);   
            ctrls.Add("WpExpContainer.aspx", wp_export_container);
            ctrls.Add("WpExpMaterialPricing.aspx", wp_material_pricing);
            ctrls.Add("WpExpSalesInvoice.aspx", wp_export_sales_invoice);
            ctrls.Add("WpExpShipmentCmo.aspx", wp_export_shipment_cmo);
            ctrls.Add("WpRmStatement.aspx", wp_rm_statement); 
             
            ctrls.Add("PfPurchaseType.aspx", pf_purchase_type);
            ctrls.Add("PfBusinessTargetMat.aspx", pf_business_target);
            ctrls.Add("PfSupplier.aspx", pf_supplier);
            ctrls.Add("PfSupervisor.aspx", pf_supervisor);
            ctrls.Add("PfItem.aspx", pf_item);
            ctrls.Add("PfItemSales.aspx", pf_item_sales);
            ctrls.Add("PfSubItem.aspx", pf_sub_item);
            ctrls.Add("PfProductionShift.aspx", pf_prod_shift);
            ctrls.Add("PfMachine.aspx", pf_prod_machine);
            ctrls.Add("PfGaEstProd.aspx", pf_garbage_est);
            ctrls.Add("PfPurchase.aspx", pf_purchase);
            ctrls.Add("PfPurchaseJw.aspx", pf_purchase_jw);
            ctrls.Add("PfPurchaseClaim.aspx", pf_purchase_claim);
            ctrls.Add("PfPurchaseClaimCheck.aspx", pf_purchase_claim_check);
            ctrls.Add("PfInventoryRm.aspx", pf_inventory_rm);
            ctrls.Add("PfProduction.aspx", pf_production);
            ctrls.Add("PfProductionJw.aspx", pf_production_jw);
            ctrls.Add("PfDailyPurProdStatement.aspx", pf_daily_pur_prod_report);
            ctrls.Add("PfDailyPurProd.aspx", pf_daily_pur_prod);
            ctrls.Add("PfActualGarbage.aspx", pf_actual_garbage);
            ctrls.Add("PfMaterialTransaction.aspx", pf_material_transaction);
            ctrls.Add("PfInventoryFg.aspx", pf_inventory_fg);
            ctrls.Add("PfInventoryMonthlyStatement.aspx", pf_inventory_monthly_state);
            ctrls.Add("PfProcessingCost.aspx", pf_processing_cost);
            ctrls.Add("PfSales.aspx", pf_sales);
            ctrls.Add("PfSalesman.aspx", pf_salesman);
            ctrls.Add("PfSalesJw.aspx", pf_sales_jw);
            ctrls.Add("PfSalesReturn.aspx", pf_sales_return);
            ctrls.Add("PfSalesCheck.aspx", pf_sales_check);
            ctrls.Add("PfExpContainer.aspx", pf_export_container);
            ctrls.Add("PfExpMaterialPricing.aspx", pf_export_material_pricing);
            ctrls.Add("PfExpSalesInvoice.aspx", pf_export_sales_invoice);
            ctrls.Add("PfExpShipmentCmo.aspx", pf_export_shipment_cmo);
            ctrls.Add("PfRmStatement.aspx", pf_rm_statement);
            ctrls.Add("PfFgStatement.aspx", pf_fg_statement);

            ctrls.Add("MsParty.aspx", ms_party);
            ctrls.Add("MsSupplierCat.aspx", ms_supplier_cat);
            ctrls.Add("MsDriver.aspx", ms_driver);
            ctrls.Add("MsCollectionFor.aspx", ms_collection_for);
            ctrls.Add("MsSupervisor.aspx", ms_supervisor);
            ctrls.Add("MsCategory.aspx", ms_category);
            ctrls.Add("MsItem.aspx", ms_item);
            ctrls.Add("MsItemSales.aspx", ms_item_sales);
            ctrls.Add("MsItemSalesWs.aspx", ms_item_sales_ws);
            ctrls.Add("MsPurchase.aspx", ms_purchase);
            ctrls.Add("MsPurchaseVatAdjustment.aspx", ms_purchase_vat_adjustment);
            ctrls.Add("MsPurchaseClaim.aspx", ms_purchase_claim);
            ctrls.Add("MsPurchaseClaimApprove.aspx", ms_purchase_claim_approve);
            ctrls.Add("MsMaterialTransaction.aspx", ms_material_transaction);
            ctrls.Add("MsActualGarbage.aspx", ms_material_garbage);
            ctrls.Add("MsInventoryRm.aspx", ms_inventory_rm);
         //   ctrls.Add("MsDailyPurSales.aspx", ms_daily_purchase_sales);
         //   ctrls.Add("MsInventoryMonthlyStatement.aspx", ms_inventory_monthly_state);
            ctrls.Add("MsSales.aspx", ms_sales);
            ctrls.Add("MsSalesInterDivision.aspx", ms_sales_inter_division);
            ctrls.Add("MsExpContainer.aspx", ms_export_container);
            ctrls.Add("MsExpMaterialPricing.aspx", ms_material_pricing);
            ctrls.Add("MsExpSalesInvoice.aspx", ms_export_sales_invoice);
            ctrls.Add("MsExpShipmentCmo.aspx", ms_export_shipment_cmo);
            ctrls.Add("MsRmStatement.aspx", ms_rm_statement);

            ctrls.Add("MfCategory.aspx", mf_category);
            ctrls.Add("MfItem.aspx", mf_item);
            ctrls.Add("MfItemSales.aspx", mf_item_sales);
            ctrls.Add("MfItemBin.aspx", mf_item_bin);
            ctrls.Add("MfVehicle.aspx", mf_vehicle);
            ctrls.Add("MfParty.aspx", mf_party);
            ctrls.Add("MfItemTransfer.aspx", mf_item_transfer);
            ctrls.Add("MfPurchase.aspx", mf_purchase_local);
            ctrls.Add("MfMatReceiving.aspx", mf_material_receiving);
            ctrls.Add("MfRmCoordinatorApprove.aspx", mf_rm_coordinator_approve);
            ctrls.Add("MfFurnaces.aspx", mf_furnaces);
            ctrls.Add("MfGradeItem.aspx", mf_grade_item);
            ctrls.Add("MfBatchItem.aspx", mf_batch_item);
            ctrls.Add("MfBatchRmCheckItem.aspx", mf_batch_rm_check);
            ctrls.Add("MfProductionIssue.aspx", mf_prod_issue);
            ctrls.Add("MfFgQualityAssurance.aspx", mf_fg_qa);
            ctrls.Add("MfFinishedGoodsIssue.aspx", mf_fg_issue);
            ctrls.Add("MfExpContainer.aspx", mf_export_container);
            ctrls.Add("MfExpMaterialPricing.aspx", mf_material_pricing);
            ctrls.Add("MfExpSalesInvoice.aspx", mf_export_sales_invoice);
            ctrls.Add("MfExpShipmentCmo.aspx", mf_export_shipment_cmo);

            ctrls.Add("AssetCategory.aspx", it_asset_category);
            ctrls.Add("AssetItems.aspx", it_asset_items);
            ctrls.Add("AssetItemsPlacement.aspx", it_asset_items_placement);
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

            ctrls.Add("NrcContainer.aspx", nrc_apps_container);
            ctrls.Add("NrcCurrency.aspx", nrc_apps_currency);
            ctrls.Add("NrcPaymentType.aspx", nrc_apps_peyment_type);
            ctrls.Add("NrcPaymentTerms.aspx", nrc_apps_peyment_terms);
            ctrls.Add("NrcShippingIncoterms.aspx", nrc_apps_shipping_incoterms);
            ctrls.Add("NrcShipmentLocation.aspx", nrc_apps_shipment_location);
            ctrls.Add("NrcTradingVessel.aspx", nrc_apps_trading_vessel);
            ctrls.Add("NrcVat.aspx", nrc_apps_vat);
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

                    if (requestedFile == "Dashboard.aspx")
                    {
                        Dashboard.Attributes.Add("class", "active");
                    }
                    // wp menu
                    else if (requestedFile == "WpSupervisor.aspx" || requestedFile == "WpDriver.aspx" ||   requestedFile == "WpCollectionFor.aspx" ||  requestedFile == "WpCategory.aspx" || requestedFile == "WpItem.aspx"  || requestedFile == "WpItemSales.aspx"  || requestedFile == "WpItemSalesWs.aspx" || requestedFile == "WpParty.aspx" ||  requestedFile == "WpSupplierCat.aspx" || requestedFile == "WpPurchase.aspx"  || requestedFile == "WpPurchaseVatAdjustment.aspx" || requestedFile == "WpPurchaseClaim.aspx" || requestedFile == "WpPurchaseClaimApprove.aspx" || requestedFile == "WpMaterialTransaction.aspx"  || requestedFile == "WpInventoryRm.aspx" || requestedFile == "WpExpContainer.aspx" || requestedFile == "WpExpMaterialPricing.aspx" || requestedFile == "WpExpSalesInvoice.aspx" || requestedFile == "WpExpShipmentCmo.aspx" || requestedFile == "WpSales.aspx" || requestedFile == "WpSalesInterDivision.aspx" || requestedFile == "WpRmStatement.aspx"  )
                    {
                        wp_menu.Attributes.Add("class", "active");

                        if (requestedFile == "WpPurchase.aspx" || requestedFile == "WpPurchaseVatAdjustment.aspx" || requestedFile == "WpPurchaseClaim.aspx" || requestedFile == "WpPurchaseClaimApprove.aspx" )
                        {
                            wp_purchase_menu.Attributes.Add("class", "active");
                        }
                        else if (requestedFile == "WpMaterialTransaction.aspx"  || requestedFile == "WpInventoryRm.aspx")
                        { 
                            wp_inventory_menu.Attributes.Add("class", "active");
                        }
                        else if (requestedFile == "WpExpContainer.aspx" || requestedFile == "WpExpMaterialPricing.aspx" || requestedFile == "WpExpSalesInvoice.aspx" || requestedFile == "WpExpShipmentCmo.aspx" || requestedFile == "WpSales.aspx" || requestedFile == "WpSalesInterDivision.aspx")
                        {
                            wp_sales_menu.Attributes.Add("class", "active");
                        }


                    }
                    // pf menu
                    else if (requestedFile == "PfBusinessTargetMat.aspx" || requestedFile == "PfSupplier.aspx" || requestedFile == "PfSupervisor.aspx" || requestedFile == "PfPurchaseType.aspx" || requestedFile == "PfItem.aspx" ||  requestedFile == "PfItemSales.aspx" || requestedFile == "PfSubItem.aspx" || requestedFile == "PfProductionShift.aspx" || requestedFile == "PfMachine.aspx" || requestedFile == "PfGaEstProd.aspx" || requestedFile == "PfPurchase.aspx" || requestedFile == "PfPurchaseJw.aspx" || requestedFile == "PfDailyPurProd.aspx"   || requestedFile == "PfDailyPurProdStatement.aspx" || requestedFile == "PfPurchaseClaim.aspx" || requestedFile == "PfPurchaseClaimCheck.aspx" || requestedFile == "PfInventoryRm.aspx" || requestedFile == "PfInventoryMonthlyStatement.aspx" || requestedFile == "PfProduction.aspx" || requestedFile == "PfProductionJw.aspx" || requestedFile == "PfActualGarbage.aspx" || requestedFile == "PfInventoryFg.aspx" || requestedFile == "PfMaterialTransaction.aspx" || requestedFile == "PfProcessingCost.aspx" || requestedFile == "PfSalesman.aspx" || requestedFile == "PfSales.aspx" || requestedFile == "PfExpContainer.aspx" || requestedFile == "PfExpMaterialPricing.aspx" || requestedFile == "PfExpSalesInvoice.aspx" || requestedFile == "PfExpShipmentCmo.aspx" || requestedFile == "PfSalesJw.aspx" || requestedFile == "PfSalesCheck.aspx" || requestedFile == "PfSalesReturn.aspx" || requestedFile == "PfRmStatement.aspx" || requestedFile == "PfFgStatement.aspx")
                    {
                        pf_menu.Attributes.Add("class", "active");

                        if (requestedFile == "PfPurchaseType.aspx" || requestedFile == "PfPurchase.aspx" || requestedFile == "PfPurchaseJw.aspx" || requestedFile == "PfPurchaseClaim.aspx" || requestedFile == "PfPurchaseClaimCheck.aspx")
                        {
                            pf_purchase_menu.Attributes.Add("class", "active");
                        }
                        else if (requestedFile == "PfProductionShift.aspx" || requestedFile == "PfMachine.aspx" || requestedFile == "PfGaEstProd.aspx" || requestedFile == "PfProduction.aspx" || requestedFile == "PfProductionJw.aspx" || requestedFile == "PfActualGarbage.aspx" || requestedFile == "PfProcessingCost.aspx")
                        {
                            pf_production_menu.Attributes.Add("class", "active");
                        }
                        else if (requestedFile == "PfInventoryRm.aspx" || requestedFile == "PfInventoryFg.aspx"  || requestedFile == "PfMaterialTransaction.aspx" || requestedFile == "PfDailyPurProdStatement.aspx"  || requestedFile == "PfDailyPurProd.aspx" || requestedFile == "PfInventoryMonthlyStatement.aspx")
                        {
                            pf_inventory_menu.Attributes.Add("class", "active");
                        }
                        else if (requestedFile == "PfSalesman.aspx" || requestedFile == "PfSales.aspx" || requestedFile == "PfSalesJw.aspx" || requestedFile == "PfSalesCheck.aspx" || requestedFile == "PfSalesReturn.aspx")
                        {
                            pf_sales_menu.Attributes.Add("class", "active");
                        }
                        else if (requestedFile == "PfExpContainer.aspx" || requestedFile == "PfExpMaterialPricing.aspx" || requestedFile == "PfExpSalesInvoice.aspx" || requestedFile == "PfExpShipmentCmo.aspx")
                        {
                            pf_overseas_menu.Attributes.Add("class", "active");
                        } 
                    }
                    // ms menu
                    else if (requestedFile == "MsSupervisor.aspx" || requestedFile == "MsDriver.aspx" || requestedFile == "MsCollectionFor.aspx" || requestedFile == "MsCategory.aspx" || requestedFile == "MsItem.aspx" || requestedFile == "MsItemSales.aspx" || requestedFile == "MsItemSalesWs.aspx" || requestedFile == "MsParty.aspx" || requestedFile == "MsSupplierCat.aspx" || requestedFile == "MsPurchase.aspx" || requestedFile == "MsPurchaseVatAdjustment.aspx" || requestedFile == "MsPurchaseClaim.aspx" || requestedFile == "MsPurchaseClaimApprove.aspx" || requestedFile == "MsMaterialTransaction.aspx"  | requestedFile == "MsActualGarbage.aspx" || requestedFile == "MsInventoryRm.aspx" || requestedFile == "MsExpContainer.aspx" || requestedFile == "MsExpMaterialPricing.aspx" || requestedFile == "MsExpSalesInvoice.aspx" || requestedFile == "MsExpShipmentCmo.aspx" || requestedFile == "MsSales.aspx" || requestedFile == "MsSalesInterDivision.aspx" || requestedFile == "MsRmStatement.aspx")
                    {
                        ms_menu.Attributes.Add("class", "active");

                        if (requestedFile == "MsPurchase.aspx" || requestedFile == "MsPurchaseVatAdjustment.aspx" || requestedFile == "MsPurchaseClaim.aspx" || requestedFile == "MsPurchaseClaimApprove.aspx")
                        {
                            ms_purchase_menu.Attributes.Add("class", "active");
                        }
                        else if (requestedFile == "MsMaterialTransaction.aspx" || requestedFile == "MsActualGarbage.aspx" || requestedFile == "MsInventoryRm.aspx")
                        {
                            ms_inventory_menu.Attributes.Add("class", "active");
                        }
                        else if (requestedFile == "MsExpContainer.aspx" || requestedFile == "MsExpMaterialPricing.aspx" || requestedFile == "MsExpSalesInvoice.aspx" || requestedFile == "MsExpShipmentCmo.aspx" || requestedFile == "MsSales.aspx" || requestedFile == "MsSalesInterDivision.aspx")
                        {
                            ms_sales_menu.Attributes.Add("class", "active");
                        }
                    }
                    // mf menu
                    else if (requestedFile == "MfCategory.aspx" || requestedFile == "MfItem.aspx" || requestedFile == "MfItemSales.aspx" || requestedFile == "MfItemBin.aspx" || requestedFile == "MfVehicle.aspx" || requestedFile == "MfParty.aspx" || requestedFile == "MfItemTransfer.aspx" || requestedFile == "MfPurchase.aspx" || requestedFile == "MfMatReceiving.aspx" || requestedFile == "MfRmCoordinatorApprove.aspx" || requestedFile == "MfFurnaces.aspx" || requestedFile == "MfGradeItem.aspx" || requestedFile == "MfBatchItem.aspx"  || requestedFile == "MfBatchRmCheckItem.aspx" || requestedFile == "MfProductionIssue.aspx" || requestedFile == "MfFgQualityAssurance.aspx" || requestedFile == "MfFinishedGoodsIssue.aspx" || requestedFile == "MfExpContainer.aspx" || requestedFile == "MfExpMaterialPricing.aspx" || requestedFile == "MfExpSalesInvoice.aspx" || requestedFile == "MfExpShipmentCmo.aspx")
                    {
                        mf_menu.Attributes.Add("class", "active");

                        if (requestedFile == "MfItemTransfer.aspx" || requestedFile == "MfPurchase.aspx" || requestedFile == "MfMatReceiving.aspx" || requestedFile == "MfRmCoordinatorApprove.aspx" )
                        {
                            mf_transfer_purchase_menu.Attributes.Add("class", "active");
                        }

                        else if(requestedFile == "MfFurnaces.aspx" || requestedFile == "MfGradeItem.aspx" || requestedFile == "MfBatchItem.aspx" || requestedFile == "MfBatchRmCheckItem.aspx" || requestedFile == "MfProductionIssue.aspx" || requestedFile == "MfFgQualityAssurance.aspx" || requestedFile == "MfFinishedGoodsIssue.aspx")
                        {
                            mf_production_menu.Attributes.Add("class", "active");
                        }
                        else if (requestedFile == "MfExpContainer.aspx" || requestedFile == "MfExpMaterialPricing.aspx" || requestedFile == "MfExpSalesInvoice.aspx" || requestedFile == "MfExpShipmentCmo.aspx")
                        {
                            mf_sales_menu.Attributes.Add("class", "active");
                        }

                    }
                    else if (requestedFile == "AssetCategory.aspx" || requestedFile == "AssetItems.aspx" || requestedFile == "AssetItemsPlacement.aspx" || requestedFile == "AssetItemsEmp.aspx" || requestedFile == "AssetItemsDept.aspx" || requestedFile == "AssetItemsExpires.aspx" || requestedFile == "AssetItemsExpiresEmp.aspx")
                    {
                        it_menu.Attributes.Add("class", "active");

                        if (requestedFile == "AssetCategory.aspx" || requestedFile == "AssetItems.aspx" || requestedFile == "AssetItemsPlacement.aspx" || requestedFile == "AssetItemsEmp.aspx" || requestedFile == "AssetItemsDept.aspx" || requestedFile == "AssetItemsExpires.aspx" || requestedFile == "AssetItemsExpiresEmp.aspx")
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
                    else if (requestedFile == "NrcVat.aspx" || requestedFile == "NrcPaymentType.aspx" || requestedFile == "NrcDataProcess.aspx" || requestedFile == "NrcContainer.aspx" || requestedFile == "NrcCurrency.aspx" || requestedFile == "NrcPaymentTerms.aspx" || requestedFile == "NrcShippingIncoterms.aspx" || requestedFile == "NrcShipmentLocation.aspx" || requestedFile == "NrcTradingVessel.aspx")
                    {
                        nrc_apps_setting.Attributes.Add("class", "active");
                    }
                }
            }
        }



    }
}