using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NRCAPPS.PF.PF_Reports
{
    public partial class PfClass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private int Item_Id;
        public int ITEM_ID
        {
            get { return Item_Id; }
            set { Item_Id = value; }
        }
        private string Item_Name;
        public string ITEM_NAME
        {
            get { return Item_Name; }
            set { Item_Name = value; }
        }
        private string Item_Name_Full;
        public string ITEM_NAME_FULL
        {
            get { return Item_Name_Full; }
            set { Item_Name_Full = value; }
        }

        private int Item_Code;
        public int ITEM_CODE
        {
            get { return Item_Code; }
            set { Item_Code = value; }
        }

        private string Sub_Item_Name;
        public string SUB_ITEM_NAME
        {
            get { return Sub_Item_Name; }
            set { Sub_Item_Name = value; }
        }
        private float Item_Weight;
        public float ITEM_WEIGHT
        {
            get { return Item_Weight; }
            set { Item_Weight = value; }
        }
        private float Item_Rate;
        public float ITEM_RATE
        {
            get { return Item_Rate; }
            set { Item_Rate = value; }
        }
        private float Item_Amount;
        public float ITEM_AMOUNT
        {
            get { return Item_Amount; }
            set { Item_Amount = value; }
        } 
        private float Item_Amount_In_Fg;
        public float ITEM_WEIGHT_IN_FG
        {
            get { return Item_Amount_In_Fg; }
            set { Item_Amount_In_Fg = value; }
        } 

        private float Vat_Percent;
        public float VAT_PERCENT
        {
            get { return Vat_Percent; }
            set { Vat_Percent = value; }
        } 
        private float Vat_Amount;
        public float VAT_AMOUNT
        {
            get { return Vat_Amount; }
            set { Vat_Amount = value; }
        } 


        private int Supplier_Id;
        public int SUPPLIER_ID
        {
            get { return Supplier_Id; }
            set { Supplier_Id = value; }
        }
        private string Supplier_Name;
        public string SUPPLIER_NAME
        {
            get { return Supplier_Name; }
            set { Supplier_Name = value; }
        }
        private int Customer_Id;
        public int CUSTOMER_ID
        {
            get { return Customer_Id; }
            set { Customer_Id = value; }
        }
        private string Customer_Name;
        public string CUSTOMER_NAME
        {
            get { return Customer_Name; }
            set { Customer_Name = value; }
        }
        private int Supervisor_Id;
        public int SUPERVISOR_ID
        {
            get { return Supervisor_Id; }
            set { Supervisor_Id = value; }
        }
        private string Supervisor_Name;
        public string SUPERVISOR_NAME
        {
            get { return Supervisor_Name; }
            set { Supervisor_Name = value; }
        }


        private int Slip_No;
        public int SLIP_NO
        {
            get { return Slip_No; }
            set { Slip_No = value; }
        }
        private int Invoice_No;
        public int INVOICE_NO
        {
            get { return Invoice_No; }
            set { Invoice_No = value; }
        }
        private int Machine_No;
        public int MACHINE_NUMBER
        {
            get { return Machine_No; }
            set { Machine_No = value; }
        }
        private string Shift_Name;
        public string SHIFT_NAME
        {
            get { return Shift_Name; }
            set { Shift_Name = value; }
        }

        private float Beg_Fstock_Wt;
        public float BEG_FSTOCK_WT
        {
            get { return Beg_Fstock_Wt; }
            set { Beg_Fstock_Wt = value; }
        }

        private float Beg_Fstock_Wt1;
        public float BEG_FSTOCK_WT1
        {
            get { return Beg_Fstock_Wt1; }
            set { Beg_Fstock_Wt1 = value; }
        }

        private float Beg_Avg_Rate;
        public float BEG_AVG_RATE
        {
            get { return Beg_Avg_Rate; }
            set { Beg_Avg_Rate = value; }
        }
        private float Beg_Amt;
        public float BEG_AMT
        {
            get { return Beg_Amt; }
            set { Beg_Amt = value; }
        }
        private float Purchase_Wt;
        public float PURCHASE_WT
        {
            get { return Purchase_Wt; }
            set { Purchase_Wt = value; }
        }
        private float Purchase_Amt;
        public float PURCHASE_AMT
        {
            get { return Purchase_Amt; }
            set { Purchase_Amt = value; }
        }
        private float Purchase_Wtd;
        public float PURCHASE_WTD
        {
            get { return Purchase_Wtd; }
            set { Purchase_Wtd = value; }
        }
        private float Purchase_Amtd;
        public float PURCHASE_AMTD
        {
            get { return Purchase_Amtd; }
            set { Purchase_Amtd = value; }
        }
        private float Mat_Avail_Prod_Wt;
        public float MAT_AVAIL_PROD_WT
        {
            get { return Mat_Avail_Prod_Wt; }
            set { Mat_Avail_Prod_Wt = value; }
        }
        private float Mat_Avail_Prod_Amt;
        public float MAT_AVAIL_PROD_AMT
        {
            get { return Mat_Avail_Prod_Amt; }
            set { Mat_Avail_Prod_Amt = value; }
        }
        private float Mat_Avail_Prod_Rate;
        public float MAT_AVAIL_PROD_RATE
        {
            get { return Mat_Avail_Prod_Rate; }
            set { Mat_Avail_Prod_Rate = value; }
        }
        private float Production_Wt;
        public float PRODUCTION_WT
        {
            get { return Production_Wt; }
            set { Production_Wt = value; }
        }

        private float Production_Amt;
        public float PRODUCTION_AMT
        {
            get { return Production_Amt; }
            set { Production_Amt = value; }
        }

        private float Pro_Cost_Amt;
       public float PRO_COST_AMT
        {
            get { return Pro_Cost_Amt; }
            set { Pro_Cost_Amt = value; }
        }
        private float Prod_Pro_Cost_Amt;
        public float PROD_PRO_COST_AMT
        {
            get { return Prod_Pro_Cost_Amt; }
            set { Prod_Pro_Cost_Amt = value; }
        }
        private float Prod_Total_Cost_Amt;
        public float PROD_TOTAL_COST_AMT
        {
            get { return Prod_Total_Cost_Amt; }
            set { Prod_Total_Cost_Amt = value; }
        }


        private float Gar_Est_Of_Prod;
        public float GAR_EST_OF_PROD
        {
            get { return Gar_Est_Of_Prod; }
            set { Gar_Est_Of_Prod = value; }
        }
        private float Actual_Gar_Weight;
        public float ACTUAL_GAR_WEIGHT
        {
            get { return Actual_Gar_Weight; }
            set { Actual_Gar_Weight = value; }
        }
        private float Avail_Sale_Wt;
        public float AVAIL_SALE_WT
        {
            get { return Avail_Sale_Wt; }
            set { Avail_Sale_Wt = value; }
        }

        private float Avail_Sale_Amt;
        public float AVAIL_SALE_AMT
        {
            get { return Avail_Sale_Amt; }
            set { Avail_Sale_Amt = value; }
        }

        private float Avail_Sale_Avg_Rate;
        public float AVAIL_SALE_AVG_RATE
        {
            get { return Avail_Sale_Avg_Rate; }
            set { Avail_Sale_Avg_Rate = value; }
        }
        private float Sale_Wt;
        public float SLAE_WT
        {
            get { return Sale_Wt; }
            set { Sale_Wt = value; }
        }
        private float Sale_Return_Wt;
        public float SALE_RETURN_WET
        {
            get { return Sale_Return_Wt; }
            set { Sale_Return_Wt = value; }
        }

        private float End_Fstock_Wt;
        public float END_FSTOCK_WT
        {
            get { return End_Fstock_Wt; }
            set { End_Fstock_Wt = value; }
        }
        private float End_Amt;
        public float END_AMT
        {
            get { return End_Amt; }
            set { End_Amt = value; }
        }
        private float End_Avg_Rate;
        public float END_AVG_RATE
        {
            get { return End_Avg_Rate; }
            set { End_Avg_Rate = value; }
        }

        private DateTime Entry_Date;
        public DateTime ENTRY_DATE
        {
            get { return Entry_Date; }
            set { Entry_Date = value; }
        }

        public PfClass(int ITEM_ID, string ITEM_NAME, string ITEM_NAME_FULL, int ITEM_CODE, string SUB_ITEM_NAME, float ITEM_WEIGHT, float ITEM_RATE, float ITEM_AMOUNT, int VAT_PERCENT, float VAT_AMOUNT, float ITEM_WEIGHT_IN_FG, int SUPPLIER_ID, string SUPPLIER_NAME, int CUSTOMER_ID, string CUSTOMER_NAME, int SUPERVISOR_ID, string SUPERVISOR_NAME, int SLIP_NO, int INVOICE_NO, int MACHINE_NUMBER, string SHIFT_NAME, float BEG_FSTOCK_WT, float BEG_FSTOCK_WT1, float BEG_AVG_RATE, float BEG_AMT, float PURCHASE_WT, float PURCHASE_AMT, float PURCHASE_WTD, float PURCHASE_AMTD, float MAT_AVAIL_PROD_WT, float MAT_AVAIL_PROD_AMT, float MAT_AVAIL_PROD_RATE, float PRODUCTION_WT, float PRODUCTION_AMT, float PRO_COST_AMT, float PROD_PRO_COST_AMT, float PROD_TOTAL_COST_AMT, float GAR_EST_OF_PROD, float ACTUAL_GAR_WEIGHT, float AVAIL_SALE_WT, float AVAIL_SALE_AMT, float AVAIL_SALE_AVG_RATE, float SLAE_WT, float SALE_RETURN_WET, float END_FSTOCK_WT, float END_AMT, float END_AVG_RATE, DateTime ENTRY_DATE)
        {

            this.Item_Id             = ITEM_ID;
            this.Item_Name           = ITEM_NAME;
            this.Item_Name_Full      = ITEM_NAME_FULL;            
            this.Item_Code           = ITEM_CODE;
            this.Sub_Item_Name       = SUB_ITEM_NAME;
            this.Item_Weight         = ITEM_WEIGHT;
            this.Item_Rate           = ITEM_RATE;
            this.Item_Amount         = ITEM_AMOUNT;
            this.Vat_Percent         = VAT_PERCENT;
            this.Vat_Amount          = VAT_AMOUNT;
            this.Item_Amount_In_Fg   = ITEM_WEIGHT_IN_FG; 
            this.Supplier_Id         = SUPPLIER_ID;
            this.Supplier_Name       = SUPPLIER_NAME;
            this.Supervisor_Id       = SUPERVISOR_ID;
            this.Supervisor_Name     = SUPERVISOR_NAME;
            this.Customer_Id         = CUSTOMER_ID;
            this.Customer_Name       = CUSTOMER_NAME;
            this.Slip_No             = SLIP_NO;
            this.Invoice_No          = INVOICE_NO;
            this.Machine_No          = MACHINE_NUMBER;
            this.Shift_Name          = SHIFT_NAME;
            this.Beg_Fstock_Wt       = BEG_FSTOCK_WT;
            this.Beg_Fstock_Wt1      = BEG_FSTOCK_WT1;
            this.Beg_Avg_Rate        = BEG_AVG_RATE;
            this.Beg_Amt             = BEG_AMT;
            this.Purchase_Wt         = PURCHASE_WT;
            this.Purchase_Amt        = PURCHASE_AMT; 
            this.Purchase_Wtd        = PURCHASE_WTD;
            this.Purchase_Amtd       = PURCHASE_AMTD; 
            this.Mat_Avail_Prod_Wt   = MAT_AVAIL_PROD_WT;
            this.Mat_Avail_Prod_Amt  = MAT_AVAIL_PROD_AMT;
            this.Mat_Avail_Prod_Rate = MAT_AVAIL_PROD_RATE;
            this.Production_Wt       = PRODUCTION_WT;
            this.Production_Amt      = PRODUCTION_AMT;
            this.Pro_Cost_Amt        = PRO_COST_AMT;
            this.Prod_Pro_Cost_Amt   = PROD_PRO_COST_AMT;
            this.Prod_Total_Cost_Amt = PROD_TOTAL_COST_AMT;
            this.Gar_Est_Of_Prod     = GAR_EST_OF_PROD;
            this.Actual_Gar_Weight   = ACTUAL_GAR_WEIGHT;
            this.Avail_Sale_Wt       = AVAIL_SALE_WT;
            this.Avail_Sale_Amt      = AVAIL_SALE_AMT;
            this.Avail_Sale_Avg_Rate = AVAIL_SALE_AVG_RATE;
            this.Sale_Wt             = SLAE_WT;
            this.Sale_Return_Wt      = SALE_RETURN_WET;
            this.End_Fstock_Wt       = END_FSTOCK_WT;
            this.End_Amt             = END_AMT;
            this.End_Avg_Rate        = END_AVG_RATE; 

            this.Entry_Date          = ENTRY_DATE;
             
        }
         
    }
}
     