<%@ Page Title="Sales Return (FG & RM) List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="PfSalesReturn.aspx.cs" Inherits="NRCAPPS.PF.PfSalesReturn" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Sales Return 
        <small>Sales Return: - Update</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Plastic Factory</a></li>
        <li class="active">Sales Return</li>
      </ol>
    </section>
    
    <!-- Main content -->
    <section class="content">
      <div class="row">
        <!-- left column --> 
            
        <!--/.col (left) -->
        <!-- right column -->
        <div class="col-md-12"> 
             <asp:Panel  id="alert_box" runat="server">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <h4><i class="icon fa fa-check"></i> Alert!</h4>  
           </asp:Panel>  
      
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Sales List</h3>
              <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 550px;">
                <asp:TextBox ID="txtSearchEmp" Class="form-control input-sm" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="ButtonSearchEmp" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridViewSearchEmp" 
                        CausesValidation="False" />
                 <asp:LinkButton ID="LinkButton6" class="btn btn-success" runat="server"  Text="Sales Return (for Finished Goods)"  onclick="BtnUpdateSalesFGCheck_Click" ValidationGroup='valGroup1' ClientIDMode="Static"></asp:LinkButton> 
                 <asp:LinkButton ID="LinkButton1" class="btn btn-danger" runat="server"  Text="Sales Return (for Raw Material)"  onclick="BtnUpdateSalesRMCheck_Click" ValidationGroup='valGroup1' ClientIDMode="Static"></asp:LinkButton> 
                  </div>  
              </div>    
            </div>
            </div>
           
            <!-- /.box-header -->
            <div class="box-body table-responsive">
            <label  class="col-sm-12"> 
              <asp:CustomValidator ID="CustomValidator1" runat="server" ValidationGroup='valGroup1' ErrorMessage="*Please select at least one Sales record."
    ClientValidationFunction="Validate" ForeColor="Red"></asp:CustomValidator></label> 

                    <asp:GridView ID="GridView1" runat="server"    EnablePersistedSelection="true"            
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "10" 
    OnPageIndexChanging="GridViewEmp_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:TemplateField>
                     <ItemTemplate>
                      <asp:CheckBox ID="IschkRowSalesRtn" runat="server"  class="flat-red" checked='<%# Eval("IS_SALES_RETURN").ToString() == "Yes" ? true : false %>' /> 
                     </ItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="INVOICE_NO" HeaderText="Invoice No." />
                     <asp:BoundField DataField="PUR_TYPE_NAME" HeaderText="Sales Type" />
                     <asp:BoundField DataField="PARTY_NAME"  HeaderText="Customer Name" />
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />   
                     <asp:BoundField DataField="SUB_ITEM_NAME"  HeaderText="Sub Item" />                      
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Wet-MT"  DataFormatString="{0:0.000}" />   
                     <asp:BoundField DataField="ITEM_RATE"  HeaderText="Rate" DataFormatString="{0:0.00}" />   
                     <asp:BoundField DataField="ITEM_AMOUNT"  HeaderText="Amount"  DataFormatString="{0:0.00}" />    
                     <asp:BoundField DataField="VAT_PERCENT"  HeaderText="Vat %" />
                     <asp:BoundField DataField="VAT_AMOUNT"  HeaderText="Vat Amount"  DataFormatString="{0:0.00}" /> 
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date" DataFormatString="{0:dd/MM/yyyy}"  />
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />   


                       <asp:TemplateField HeaderText="Sales Return" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveQ" CssClass="label" Text='<%# Eval("IS_SALES_RETURN").ToString() == "FG" ? "<span Class=label-danger style=Padding:2px >Yes-FG<span>" : "<span Class=label-success style=Padding:2px>No<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField> 
                               
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
</asp:GridView> 
<script type="text/javascript">
    function Validate(sender, args) {
        var gridView = document.getElementById("<%=GridView1.ClientID %>");
        var checkBoxes = gridView.getElementsByTagName("input");
        for (var i = 0; i < checkBoxes.length; i++) {
            if (checkBoxes[i].type == "checkbox" && checkBoxes[i].checked) {
                args.IsValid = true;
                return;
            }
        }
        args.IsValid = false;
    }
</script>
        </div>
       </div>
        
         <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Sales Return List (for Finished Goods)</h3>
              <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchSales" Class="form-control input-sm" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="Button1" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridViewSearchSalesRtn" 
                        CausesValidation="False" />
                
                  </div>  
              </div>    
            </div>
            </div>
           
            <!-- /.box-header -->
            <div class="box-body table-responsive">
           

                    <asp:GridView ID="GridView2" runat="server"    EnablePersistedSelection="true"            
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "10" 
    OnPageIndexChanging="GridViewSalesRtn_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:TemplateField>
                     
                     </asp:TemplateField>
                     <asp:BoundField DataField="INVOICE_NO" HeaderText="Invoice No." />
                     <asp:BoundField DataField="PUR_TYPE_NAME" HeaderText="Sales Type" />
                     <asp:BoundField DataField="PARTY_NAME"  HeaderText="Customer Name" />
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />   
                     <asp:BoundField DataField="SUB_ITEM_NAME"  HeaderText="Sub Item" />
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Wet-MT"  DataFormatString="{0:0.000}" /> 
                     <asp:BoundField DataField="ITEM_RATE"  HeaderText="Rate" DataFormatString="{0:0.00}" />   
                     <asp:BoundField DataField="ITEM_AMOUNT"  HeaderText="Amount"  DataFormatString="{0:0.00}" />    
                     <asp:BoundField DataField="VAT_PERCENT"  HeaderText="Vat %" />
                     <asp:BoundField DataField="VAT_AMOUNT"  HeaderText="Vat Amount"  DataFormatString="{0:0.00}" /> 
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date" DataFormatString="{0:dd/MM/yyyy}"  />
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />   


                       <asp:TemplateField HeaderText="Sales Return" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveQ" CssClass="label" Text='<%# Eval("IS_SALES_RETURN").ToString() == "FG" ? "<span Class=label-danger style=Padding:2px >Yes-FG<span>" : "<span Class=label-success style=Padding:2px>No<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField> 
                      <asp:BoundField DataField="SALES_RTN_DATE"  HeaderText="Return Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                    
                     
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
</asp:GridView> 
 
        </div>


       </div>

               <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Sales Return List (for Raw Material)</h3>
              <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="TextBox1" Class="form-control input-sm" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="Button2" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridViewSearchSalesRtn" 
                        CausesValidation="False" />
                
                  </div>  
              </div>    
            </div>
            </div>
           
            <!-- /.box-header -->
            <div class="box-body table-responsive">
           

                    <asp:GridView ID="GridView3" runat="server"    EnablePersistedSelection="true"            
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "10" 
    OnPageIndexChanging="GridViewSalesRmRtn_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:TemplateField>
                     
                     </asp:TemplateField>
                     <asp:BoundField DataField="INVOICE_NO" HeaderText="Invoice No." />
                     <asp:BoundField DataField="PUR_TYPE_NAME" HeaderText="Sales Type" />
                     <asp:BoundField DataField="PARTY_NAME"  HeaderText="Customer Name" />
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />   
                     <asp:BoundField DataField="SUB_ITEM_NAME"  HeaderText="Sub Item" />                     
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Wet-MT"  DataFormatString="{0:0.000}" /> 
                     <asp:BoundField DataField="ITEM_RATE"  HeaderText="Rate" DataFormatString="{0:0.00}" />  
                     <asp:BoundField DataField="ITEM_AMOUNT"  HeaderText="Amount"  DataFormatString="{0:0.00}" />    
                     <asp:BoundField DataField="VAT_PERCENT"  HeaderText="Vat %" />
                     <asp:BoundField DataField="VAT_AMOUNT"  HeaderText="Vat Amount"  DataFormatString="{0:0.00}" /> 
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date" DataFormatString="{0:dd/MM/yyyy}"  />
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />   


                       <asp:TemplateField HeaderText="Sales Return" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveQ" CssClass="label" Text='<%# Eval("IS_SALES_RETURN").ToString() == "RM" ? "<span Class=label-danger style=Padding:2px >Yes-RM<span>" : "<span Class=label-success style=Padding:2px>No<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField> 
                      <asp:BoundField DataField="SALES_RTN_DATE"  HeaderText="Return Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                    
                     
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
</asp:GridView> 
 
        </div>


       </div>
    </div> 
          <!-- /.box --> 
        <!--/.col (right) -->
      </div>
      <!-- /.row -->
    </section>
    <!-- /.content -->
 
</div>
</asp:Content> 