<%@ Page Title="Raw Material Coordinator Approve List - Metal Factory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="MfRmCoordinatorApprove.aspx.cs" Inherits="NRCAPPS.MF.MfRmCoordinatorApprove" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
       Raw Material Coordinator Approve List
        <small>Raw Material Coordinator Approve List: -Update-</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Metal Factory</a></li>
        <li class="active">Raw Material Coordinator Approve</li>
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
          <!-- Horizontal Form -->
            </div> 
          <div class="col-md-5">        
       
        </div>   
      <div class="col-md-12">  
      <div class="box box-solid box-default">
        <div class="box-header">
          <h3 class="box-title"><i class="fa fa-tty margin-r-5"></i> Material Transfer Received From Metal Scrap Yard</h3>
        </div><!-- /.box-header -->
        <div class="box-body"> 
               <div class="box box-default">
            <div class="box-header with-border">
              <h3 class="box-title"> Material Transfer Received  <span class="badge bg-red">Pending</span>  for Approve List</h3>
               <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchGrideView1" Class="form-control" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="GridView1Search1" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridView1Search" 
                        CausesValidation="False" />
                  </div>  
              </div>    
            </div>
            </div>
           
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView1" runat="server"    EnablePersistedSelection="true"        
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "5" 
    OnPageIndexChanging="GridView1_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns> 
                          <asp:BoundField DataField="TRANSFER_ID" HeaderText="Transfer ID" ItemStyle-CssClass="hidden"   HeaderStyle-CssClass="hidden" /> 
                          
                      <asp:TemplateField HeaderText="Action">
                     <ItemTemplate>   
                        <asp:CheckBox ID="IsShipmentCheck" runat="server"  class="flat-red" AutoPostBack = "true" oncheckedchanged="TransferUpdatePendingApporve_Click" />  
                     </ItemTemplate>
                     </asp:TemplateField>    
                     <asp:BoundField DataField="WB_SLIP_NO" HeaderText="WB- Slip No" />
                     <asp:BoundField DataField="VEHICLE_NO" HeaderText="Vehicle No" />
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />
                     <asp:BoundField DataField="ITEM_CODE"  HeaderText="Item Code" />   
                     <asp:BoundField DataField="NET_WT_MS"  HeaderText="Net WT Metal Scarp" DataFormatString="{0:0,0.00}" />                       
                     <asp:BoundField DataField="NET_WT_MF"  HeaderText="Net WT Metal Factory" DataFormatString="{0:0,0.00}" /> 
                     <asp:BoundField DataField="VARIANCE"  HeaderText="Variance" DataFormatString="{0:0,0.00}" />   
                     <asp:BoundField DataField="ITEM_BIN_NAME"  HeaderText="Bin Name" /> 
                      
                     <asp:TemplateField HeaderText="Is Print" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                       <ItemTemplate> 
                            <asp:Label ID="IsPrintedCheck" CssClass="label" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "<span Class=label-success style=Padding:2px >Printed</span> </br></br>" : "<span Class=label-danger style=Padding:2px>Not Printed</span>" %>'  runat="server" /> 
                            <asp:Label ID="IsPrintedDate" class="text-green" Style="font-size:11px;" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? Eval("PRINT_DATE") : "" %>'  runat="server" /> 
                            <asp:Label ID="IsPrintedCheckLink" style="display:none" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "Printed" : "Not_Printed" %>'  runat="server" /> 
                        </ItemTemplate>
                     </asp:TemplateField>  
                          <asp:TemplateField HeaderText="Mat. Coor. Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsCmoCheck" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete<span>" : "<span Class=label-danger style=Padding:2px>Incomplete<span>" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckLink" style="display:none" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>  
                          <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date"  DataFormatString="{0:dd/MM/yyyy}"  />
                     <asp:BoundField DataField="REMARKS"  HeaderText="Remarks" />
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>
                    
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />                         
                      
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                    </asp:GridView> 
                 
        </div>
       </div>
           
        <div class="box box-default">
            <div class="box-header with-border">
              <h3 class="box-title">Material Transfer Received Approve <span class="badge bg-green">Complete</span> List</h3>
               <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchGrideView2" Class="form-control" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="GridView2Search2" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridView2Search" 
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
    PageSize = "5" 
    OnPageIndexChanging="GridView2_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns> 
                    <asp:BoundField DataField="TRANSFER_ID" HeaderText="Transfer ID" ItemStyle-CssClass="hidden"   HeaderStyle-CssClass="hidden" /> 
             
                     <asp:TemplateField HeaderText="Action">
                     <ItemTemplate>   
                        <asp:CheckBox ID="IsCompleteApporve" runat="server"  class="flat-red" AutoPostBack = "true" oncheckedchanged="TransferUpdateCompleteApporve_Click" />  
                     </ItemTemplate>
                     </asp:TemplateField> 
                       
                     <asp:BoundField DataField="WB_SLIP_NO" HeaderText="WB- Slip No" />
                     <asp:BoundField DataField="VEHICLE_NO" HeaderText="Vehicle No" />
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />
                     <asp:BoundField DataField="ITEM_CODE"  HeaderText="Item Code" />   
                     <asp:BoundField DataField="NET_WT_MS"  HeaderText="Net WT Metal Scarp" DataFormatString="{0:0,0.00}" />                       
                     <asp:BoundField DataField="NET_WT_MF"  HeaderText="Net WT Metal Factory" DataFormatString="{0:0,0.00}" /> 
                     <asp:BoundField DataField="VARIANCE"  HeaderText="Variance" DataFormatString="{0:0,0.00}" />   
                     <asp:BoundField DataField="ITEM_BIN_NAME"  HeaderText="Bin Name" /> 
                      
                     <asp:TemplateField HeaderText="Is Print" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                       <ItemTemplate> 
                            <asp:Label ID="IsPrintedCheck" CssClass="label" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "<span Class=label-success style=Padding:2px >Printed</span> </br></br>" : "<span Class=label-danger style=Padding:2px>Not Printed</span>" %>'  runat="server" /> 
                            <asp:Label ID="IsPrintedDate" class="text-green" Style="font-size:11px;" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? Eval("PRINT_DATE") : "" %>'  runat="server" /> 
                            <asp:Label ID="IsPrintedCheckLink" style="display:none" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "Printed" : "Not_Printed" %>'  runat="server" /> 
                        </ItemTemplate>
                     </asp:TemplateField>  
                     <asp:TemplateField HeaderText="Mat. Coor. Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsCmoCheck" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete<span>" : "<span Class=label-danger style=Padding:2px>Incomplete<span>" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckLink" style="display:none" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>  
                          <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date"  DataFormatString="{0:dd/MM/yyyy}"  />
                     <asp:BoundField DataField="REMARKS"  HeaderText="Remarks" />
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>
                    
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  /> 
                      
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

        <div class="box box-solid box-info">
        <div class="box-header">
          <h3 class="box-title"><i class="fa  fa-money margin-r-5"></i> Purchase</h3>
        </div><!-- /.box-header -->
        <div class="box-body"> 
               <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title"> Purchase <span class="badge bg-red">Pending</span>  for Approve List</h3>
               <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchGrideView3" Class="form-control" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="Button1" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridView3Search" 
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
    PageSize = "5" 
    OnPageIndexChanging="GridView3_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns> 
                          <asp:BoundField DataField="PURCHASE_ID" HeaderText="Transfer ID" ItemStyle-CssClass="hidden"   HeaderStyle-CssClass="hidden" /> 
                          
                      <asp:TemplateField HeaderText="Action">
                     <ItemTemplate>   
                        <asp:CheckBox ID="IsShipmentCheck" runat="server"  class="flat-red" AutoPostBack = "true" oncheckedchanged="PurchaseUpdatePendingApporve_Click" />  
                     </ItemTemplate>
                     </asp:TemplateField>    
                     <asp:BoundField DataField="SLIP_NO" HeaderText="WB- Slip No" />
                     <asp:BoundField DataField="PARTY_NAME" HeaderText="Supplier Name" />                         
                     <asp:BoundField DataField="CATEGORY_NAME"  HeaderText="Category" />
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />   
                     <asp:BoundField DataField="ITEM_WEIGHT_WB"  HeaderText="Weight-WB" DataFormatString="{0:0.00}" />
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Weight" DataFormatString="{0:0.00}" />
                     <asp:BoundField DataField="ITEM_RATE"  HeaderText="Rate" />   
                     <asp:BoundField DataField="ITEM_AMOUNT"  HeaderText="Amount" DataFormatString="{0:0,0.00}" />     
                     <asp:BoundField DataField="VAT_AMOUNT"  HeaderText="Vat Amt" DataFormatString="{0:0,0.00}" /> 
                     <asp:BoundField DataField="TOTAL_AMOUNT"  HeaderText="Total Amt" DataFormatString="{0:0,0.00}" />                          
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date"  DataFormatString="{0:dd/MM/yyyy}"  />
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:TemplateField HeaderText="Is Print" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="IsPrintedCheck" CssClass="label" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "<span Class=label-success style=Padding:2px >Printed</span> </br></br>" : "<span Class=label-danger style=Padding:2px>Not Printed</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedDate" class="text-green" Style="font-size:11px;" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? Eval("PRINT_DATE") : "" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedCheckLink" style="display:none" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "Printed" : "Not_Printed" %>'  runat="server" /> 
                           
                        </ItemTemplate>
                     </asp:TemplateField>
                         <asp:TemplateField HeaderText="Mat. Coor. Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsRmcCheck" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete<span>" : "<span Class=label-danger style=Padding:2px>Incomplete<span>" %>'  runat="server" /> 
                             <asp:Label ID="IsRmcCheckLink" style="display:none" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>  
                      <asp:TemplateField HeaderText="CMO Claim Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsCmoCheck" CssClass="label" Text='<%# Eval("IS_CHECK").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete</span>" : "<span Class=label-danger style=Padding:2px>Incomplete</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckLink" style="display:none" CssClass="label" Text='<%# Eval("IS_CHECK").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckClaim"  style="display:none" CssClass="label" Text='<%# Eval("CLAIM_NO").ToString() == "" ? "Available" : "NotAvailable" %>'  runat="server" />                             
                          </ItemTemplate>
                     </asp:TemplateField>                        
                      
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                    </asp:GridView> 
                 
        </div>
       </div>
           
        <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title"> Purchase Approve <span class="badge bg-green">Complete</span> List</h3>
               <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchGrideView4" Class="form-control" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="Button2" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridView4Search" 
                        CausesValidation="False" />
                  </div>  
              </div>    
            </div>
            </div>
           
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView4" runat="server"    EnablePersistedSelection="true"        
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "5" 
    OnPageIndexChanging="GridView4_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns> 
                    <asp:BoundField DataField="PURCHASE_ID" HeaderText="Transfer ID" ItemStyle-CssClass="hidden"   HeaderStyle-CssClass="hidden" /> 
             
                     <asp:TemplateField HeaderText="Action">
                     <ItemTemplate>   
                        <asp:CheckBox ID="IsCompleteApporve" runat="server"  class="flat-red" AutoPostBack = "true" oncheckedchanged="PurchaseUpdateCompleteApporve_Click" />  
                     </ItemTemplate>
                     </asp:TemplateField> 
                     <asp:BoundField DataField="SLIP_NO" HeaderText="WB- Slip No" />
                     <asp:BoundField DataField="PARTY_NAME" HeaderText="Supplier Name" />                         
                     <asp:BoundField DataField="CATEGORY_NAME"  HeaderText="Category" />
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />   
                     <asp:BoundField DataField="ITEM_WEIGHT_WB"  HeaderText="Weight-WB" DataFormatString="{0:0.00}" />
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Weight" DataFormatString="{0:0.00}" />
                     <asp:BoundField DataField="ITEM_RATE"  HeaderText="Rate" />   
                     <asp:BoundField DataField="ITEM_AMOUNT"  HeaderText="Amount" DataFormatString="{0:0,0.00}" />     
                     <asp:BoundField DataField="VAT_AMOUNT"  HeaderText="Vat Amt" DataFormatString="{0:0,0.00}" /> 
                     <asp:BoundField DataField="TOTAL_AMOUNT"  HeaderText="Total Amt" DataFormatString="{0:0,0.00}" />                          
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date"  DataFormatString="{0:dd/MM/yyyy}"  />
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:TemplateField HeaderText="Is Print" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="IsPrintedCheck" CssClass="label" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "<span Class=label-success style=Padding:2px >Printed</span> </br></br>" : "<span Class=label-danger style=Padding:2px>Not Printed</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedDate" class="text-green" Style="font-size:11px;" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? Eval("PRINT_DATE") : "" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedCheckLink" style="display:none" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "Printed" : "Not_Printed" %>'  runat="server" /> 
                           
                        </ItemTemplate>
                     </asp:TemplateField>
                         <asp:TemplateField HeaderText="Mat. Coor. Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsRmcCheck" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete<span>" : "<span Class=label-danger style=Padding:2px>Incomplete<span>" %>'  runat="server" /> 
                             <asp:Label ID="IsRmcCheckLink" style="display:none" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>  
                      <asp:TemplateField HeaderText="CMO Claim Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsCmoCheck" CssClass="label" Text='<%# Eval("IS_CHECK").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete</span>" : "<span Class=label-danger style=Padding:2px>Incomplete</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckLink" style="display:none" CssClass="label" Text='<%# Eval("IS_CHECK").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckClaim"  style="display:none" CssClass="label" Text='<%# Eval("CLAIM_NO").ToString() == "" ? "Available" : "NotAvailable" %>'  runat="server" />                             
                          </ItemTemplate>
                     </asp:TemplateField> 
                      
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

          <div class="box box-solid box-primary">
        <div class="box-header">
          <h3 class="box-title"><i class="fa fa-cubes margin-r-5"></i> Material Receiving (Import)</h3>
        </div><!-- /.box-header -->
        <div class="box-body"> 
               <div class="box box-primary">
            <div class="box-header with-border">
              <h3 class="box-title"> Material Receiving (Import) <span class="badge bg-red">Pending</span>  for Approve List</h3>
               <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchGrideView5" Class="form-control" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="Button3" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridView5Search" 
                        CausesValidation="False" />
                  </div>  
              </div>    
            </div>
            </div>
           
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView5" runat="server"    EnablePersistedSelection="true"        
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "5" 
    OnPageIndexChanging="GridView5_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns> 
                          <asp:BoundField DataField="PURCHASE_IMPROT_ID" HeaderText="Transfer ID" ItemStyle-CssClass="hidden"   HeaderStyle-CssClass="hidden" /> 
                          
                      <asp:TemplateField HeaderText="Action">
                     <ItemTemplate>   
                        <asp:CheckBox ID="IsShipmentCheck" runat="server"  class="flat-red" AutoPostBack = "true" oncheckedchanged="MatRecevingUpdatePendingApporve_Click" />  
                     </ItemTemplate>
                     </asp:TemplateField>    
                    <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date"  DataFormatString="{0:dd/MM/yyyy}"  /> 
                    <asp:TemplateField HeaderText="Container No." ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="ContainerNo"   Text='<%#Bind("CONTAINER_NO")%>'  runat="server" /> 
                            <small class="label bg-blue"> <asp:Label ID="ContainerSize" Text='<%#Bind("CONTAINER_SIZE") %>'  runat="server" /></small> 
                        </ItemTemplate>
                     </asp:TemplateField> 
                     <asp:BoundField DataField="PARTY_NAME" HeaderText="Company Name" />   
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />  
                     <asp:TemplateField HeaderText="Item Bin" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="ItemBin"   Text='<%#Bind("ITEM_BIN_NAME")%>'  runat="server" /> 
                            <small class="label bg-yellow"> <asp:Label ID="CapacityWt" Text='<%#Bind("CAPACITY_WEIGHT") %>'  runat="server" /></small> 
                        </ItemTemplate>
                     </asp:TemplateField> 
                     <asp:BoundField DataField="ITEM_WEIGHT_WB"  HeaderText="Weight WB" />    
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Weight" /> 
                     <asp:BoundField DataField="PACKING_NAME"  HeaderText="Pack Name" />   
                     <asp:BoundField DataField="NUMBER_OF_PACK"  HeaderText="No. Pack "   />   
                     <asp:BoundField DataField="PACK_PER_WEIGHT"  HeaderText="Per Pack WT "   />  
                     <asp:BoundField DataField="TOTAL_WEIGHT"  HeaderText="Total Pack WT"   />
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>  
                     <asp:TemplateField HeaderText="Is Print" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="IsPrintedCheck" CssClass="label" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "<span Class=label-success style=Padding:2px >Printed</span> </br></br>" : "<span Class=label-danger style=Padding:2px>Not Printed</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedDate" class="text-green" Style="font-size:11px;" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? Eval("PRINT_DATE") : "" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedCheckLink" style="display:none" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "Printed" : "Not_Printed" %>'  runat="server" /> 
                        </ItemTemplate>
                     </asp:TemplateField> 
                      <asp:TemplateField HeaderText="Mat Coor. Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsCmoCheck" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete</span>" : "<span Class=label-danger style=Padding:2px>Incomplete</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckLink" style="display:none" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                         </ItemTemplate>
                     </asp:TemplateField> 
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />                       
                      
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                    </asp:GridView> 
                 
        </div>
       </div>
           
        <div class="box box-primary">
            <div class="box-header with-border">
              <h3 class="box-title"> Material Receiving (Import) Approve <span class="badge bg-green">Complete</span> List</h3>
               <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchGrideView6" Class="form-control" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="Button4" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridView6Search" 
                        CausesValidation="False" />
                  </div>  
              </div>    
            </div>
            </div>
           
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView6" runat="server"    EnablePersistedSelection="true"        
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "5" 
    OnPageIndexChanging="GridView6_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns> 
                    <asp:BoundField DataField="PURCHASE_IMPROT_ID" HeaderText="Transfer ID" ItemStyle-CssClass="hidden"   HeaderStyle-CssClass="hidden" /> 
             
                     <asp:TemplateField HeaderText="Action">
                     <ItemTemplate>   
                        <asp:CheckBox ID="IsCompleteApporve" runat="server"  class="flat-red" AutoPostBack = "true" oncheckedchanged="MatRecevingUpdateCompleteApporve_Click" />  
                     </ItemTemplate>
                     </asp:TemplateField> 
                    <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date"  DataFormatString="{0:dd/MM/yyyy}"  /> 
                    <asp:TemplateField HeaderText="Container No." ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="ContainerNo"   Text='<%#Bind("CONTAINER_NO")%>'  runat="server" /> 
                            <small class="label bg-blue"> <asp:Label ID="ContainerSize" Text='<%#Bind("CONTAINER_SIZE") %>'  runat="server" /></small> 
                        </ItemTemplate>
                     </asp:TemplateField> 
                     <asp:BoundField DataField="PARTY_NAME" HeaderText="Company Name" />   
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />  
                     <asp:TemplateField HeaderText="Item Bin" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="ItemBin"   Text='<%#Bind("ITEM_BIN_NAME")%>'  runat="server" /> 
                            <small class="label bg-yellow"> <asp:Label ID="CapacityWt" Text='<%#Bind("CAPACITY_WEIGHT") %>'  runat="server" /></small> 
                        </ItemTemplate>
                     </asp:TemplateField> 
                     <asp:BoundField DataField="ITEM_WEIGHT_WB"  HeaderText="Weight WB" />    
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Weight" /> 
                     <asp:BoundField DataField="PACKING_NAME"  HeaderText="Pack Name" />   
                     <asp:BoundField DataField="NUMBER_OF_PACK"  HeaderText="No. Pack "   />   
                     <asp:BoundField DataField="PACK_PER_WEIGHT"  HeaderText="Per Pack WT "   />  
                     <asp:BoundField DataField="TOTAL_WEIGHT"  HeaderText="Total Pack WT"   />
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>  
                     <asp:TemplateField HeaderText="Is Print" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="IsPrintedCheck" CssClass="label" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "<span Class=label-success style=Padding:2px >Printed</span> </br></br>" : "<span Class=label-danger style=Padding:2px>Not Printed</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedDate" class="text-green" Style="font-size:11px;" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? Eval("PRINT_DATE") : "" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedCheckLink" style="display:none" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "Printed" : "Not_Printed" %>'  runat="server" /> 
                        </ItemTemplate>
                     </asp:TemplateField> 
                      <asp:TemplateField HeaderText="Mat Coor. Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsCmoCheck" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete</span>" : "<span Class=label-danger style=Padding:2px>Incomplete</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckLink" style="display:none" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                         </ItemTemplate>
                     </asp:TemplateField> 
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />    
                      
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
   <asp:Panel ID="PanelPrint" runat="server" ></asp:Panel>  
  
</div> 
</asp:Content> 