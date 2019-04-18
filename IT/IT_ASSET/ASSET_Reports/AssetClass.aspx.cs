using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NRCAPPS.ASSET.ASSET_Reports
{
    public partial class AssetClass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
         
        private string Emp_Items_Id;
        public string EMP_ITEMS_ID
        {
            get { return Emp_Items_Id; }
            set { Emp_Items_Id = value; }
        }
            

        private string Emp_Name;
        public string EMP_NAME
        {
            get { return Emp_Name; }
            set { Emp_Name = value; }
        }
        private string Department_Name;
        public string DEPARTMENT_NAME
        {
            get { return Department_Name; }
            set { Department_Name = value; }
        }
        private string Division_Name;
        public string DIVISION_NAME
        {
            get { return Division_Name; }
            set { Division_Name = value; }
        }
        private string Location_Name;
        public string LOCATION_NAME
        {
            get { return Location_Name; }
            set { Location_Name = value; }
        }
        private string Item_Category_Name;
        public string ITEM_CATEGORY_NAME
        {
            get { return Item_Category_Name; }
            set { Item_Category_Name = value; }
        }
        private string Item_Type;
        public string ITEM_TYPE
        {
            get { return Item_Type; }
            set { Item_Type = value; }
        }
        private string Item_Brand;
        public string ITEM_BRAND
        {
            get { return Item_Brand; }
            set { Item_Brand = value; }
        }
        private string Item_Name;
        public string ITEM_NAME
        {
            get { return Item_Name; }
            set { Item_Name = value; }
        }
        private string Email;
        public string EMAIL
        {
            get { return Email; }
            set { Email = value; }
        }
        private string Items_Exp_Id;
        public string ITEM_EXP_ID
        {
            get { return Items_Exp_Id; }
            set { Items_Exp_Id = value; }
        }
        private int Expired_Days;
        public int EXPIRED_DAYS
        {
            get { return Expired_Days; }
            set { Expired_Days = value; }
        }

        private byte[] Image_Qr_Code;
        public byte[] IMAGE_QR_CODE
        {
            get { return Image_Qr_Code; }
            set { Image_Qr_Code = value; }
        }

        private float Limit_Amt;
        public float LIMIT_AMT
        {
            get { return Limit_Amt; }
            set { Limit_Amt = value; }
        }
        private string Qr_Code_ID;
        public string QR_CODE_ID
        {
            get { return Qr_Code_ID; }
            set { Qr_Code_ID = value; }
        }
        private DateTime Effective_Dt;
        public DateTime EFFECTIVE_DT
        {
            get { return Effective_Dt; }
            set { Effective_Dt = value; }
        }

        public AssetClass(string EMP_ITEMS_ID, string EMP_NAME, string DEPARTMENT_NAME, string DIVISION_NAME, string LOCATION_NAME, string ITEM_CATEGORY_NAME, string QR_CODE_ID, string ITEM_TYPE, string ITEM_BRAND, string ITEM_NAME, string ITEM_EXP_ID, string EMAIL, int EXPIRED_DAYS, byte[] IMAGE_QR_CODE, float LIMIT_AMT, string BRANCH_NM, DateTime EFFECTIVE_DT)
        {

            this.Emp_Items_Id       = EMP_ITEMS_ID;
            this.Emp_Name           = EMP_NAME;
            this.Department_Name    = DEPARTMENT_NAME;
            this.Division_Name      = DIVISION_NAME;
            this.Location_Name      = LOCATION_NAME;
            this.Item_Category_Name = ITEM_CATEGORY_NAME;
            this.Item_Type          = ITEM_TYPE;
            this.Item_Name          = ITEM_NAME;
            this.Email              = EMAIL;
            this.Items_Exp_Id       = ITEM_EXP_ID;
            this.Item_Brand         = ITEM_BRAND;
            this.Expired_Days       = EXPIRED_DAYS;
            this.Image_Qr_Code      = IMAGE_QR_CODE; 
            this.Limit_Amt          = LIMIT_AMT; 
            this.Qr_Code_ID         = QR_CODE_ID; 
            this.Effective_Dt       = EFFECTIVE_DT;
             
        }
         
    }
}
     