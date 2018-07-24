<%@ Page Title="Sales Approve CMO Form & List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="PfSalesCheck.aspx.cs" Inherits="NRCAPPS.PF.PfSalesCheck" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Sales Approve CMO Form & List
        <small>Sales Approve CMO: - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Plastic Factory</a></li>
        <li class="active">Sales Approve CMO</li>
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
          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Sales Approve CMO Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start -->
            
              <div class="box-body">
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">Invoice No</label> 
                  <div class="col-sm-1">    
                        <label>
                         <asp:TextBox ID="TextInvoiceNoDisable" class="form-control input-sm" runat="server"></asp:TextBox>    
                        </label>
                  </div>
                 </div>   
             <div class="form-group">
                  <label  class="col-sm-2 control-label">Query For This Sales</label> 
                  <div class="col-sm-3" style="padding-top:6px;">    
                        <label>
                         <asp:TextBox ID="TextInvoiceNo" class="form-control input-sm" style="display:none;" runat="server"></asp:TextBox>  
                        <asp:CheckBox ID="CheckIsQuery" runat="server" AutoPostBack = "true" oncheckedchanged="Query_CheckedChanged" />  
                        </label>
                  </div>
                 </div> 
                  <div id="QueryCmo" runat="server" >
                   <div class="form-group">   
                        <label class="col-sm-2 control-label">Query Description</label> 
                       <div class="col-sm-5">    
                        <div class="input-group"> 
                        <asp:TextBox ID="TextQueryDescription" class="form-control" runat="server" ></asp:TextBox>  
                        <span class="input-group-addon">Maximum 400 Character</span>      
                       </div>
                         <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                              ControlToValidate="TextQueryDescription" ErrorMessage="Enter Query Description. (Maximum 400 Character)" 
                              Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                     </div>  
                   </div>
                 </div>
                   
                  <div class="form-group">
                  <label  class="col-sm-2 control-label">CMO Approve Status</label> 
                  <div class="col-sm-3" style="padding-top:6px;">    
                        <label>
                            <asp:CheckBox ID="CheckIsCmo" runat="server" AutoPostBack = "true" oncheckedchanged="Cmo_CheckedChanged" />  
                        </label>
                  </div>
                 </div> 
               
                 <!-- checkbox -->
              
                 
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">    
                      <asp:LinkButton ID="BtnUpdate" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdate_Click"><span class="fa fa-edit"></span> Update</asp:LinkButton>
                       
                </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Sales Approve CMO List</h3>
              <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 300px;">
                <asp:TextBox ID="txtSearchEmp" Class="form-control input-sm" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="ButtonSearchEmp" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridViewSearchEmp" 
                        CausesValidation="False" />
                 <asp:LinkButton ID="LinkButton6" class="btn btn-success" runat="server" 
                          Text="Approve"  onclick="BtnUpdateSalesCheck_Click" ValidationGroup='valGroup1' ClientIDMode="Static"></asp:LinkButton> 
                  </div>  
              </div>    
            </div>
            </div>
           
            <!-- /.box-header -->
            <div class="box-body  table-responsive no-padding">
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
                      <asp:CheckBox ID="IschkRowSales" runat="server"  class="flat-red" checked='<%# Eval("IS_CHECK").ToString() == "Complete" ? true : false %>' />
                   
                     </ItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="INVOICE_NO" HeaderText="Invoice No." />
                     <asp:BoundField DataField="PUR_TYPE_NAME" HeaderText="Sales Type" />
                     <asp:BoundField DataField="CUSTOMER_NAME"  HeaderText="Customer Name" />
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />   
                     <asp:BoundField DataField="SUB_ITEM_NAME"  HeaderText="Sub Item" /> 
                     <asp:BoundField DataField="ITEM_RATE"  HeaderText="Rate" DataFormatString="{0:0.00}" />  
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Wet-MT"  DataFormatString="{0:0.000}" />    
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


                       <asp:TemplateField HeaderText="Query" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveQ" CssClass="label" Text='<%# Eval("IS_OBJ_QUERY").ToString() == "No" ? "<span Class=label-success style=Padding:2px >No<span>" : "<span Class=label-danger style=Padding:2px>Yes<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField> 
                     <asp:TemplateField HeaderText="Query Description">
                        <ItemTemplate>  
                             <asp:Label ID="Label1" Text='<%# Eval("OBJ_QUERY_DES")%>' runat="server"></asp:Label> -
                              <asp:Label ID="Label2" Text='<%# Eval("OBJ_QUERY_C_DATE", "{0:d/MM/yyyy h:mm:ss tt}")%>' runat="server"></asp:Label>
                          </ItemTemplate>   
                     </asp:TemplateField>  
                     
                     <asp:TemplateField HeaderText="CMO Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveCheck" CssClass="label" Text='<%# Eval("IS_CHECK").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete<span>" : "<span Class=label-danger style=Padding:2px>Incomplete<span>" %>'  runat="server" /> 
                             <asp:Label ID="IsActiveCheckLink" style="display:none" CssClass="label" Text='<%# Eval("IS_CHECK").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                          </ItemTemplate>
                    </asp:TemplateField> 

                     
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("INVOICE_NO") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
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
        
  
    </div> 
          <!-- /.box --> 
        <!--/.col (right) -->
      </div>
      <!-- /.row -->
    </section>
    <!-- /.content -->
 
</div>
</asp:Content> 