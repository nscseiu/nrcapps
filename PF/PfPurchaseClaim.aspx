<%@ Page Title="Purchase Claim Form & List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="PfPurchaseClaim.aspx.cs" Inherits="NRCAPPS.PF.PfPurchaseClaim" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Purchase Claim Form & List
        <small>Purchase Claim: - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Plastic Factory</a></li>
        <li class="active">Purchase Claim</li>
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
              <h3 class="box-title">Purchase Claim Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start -->
            
              <div class="box-body">
                <div class="form-group">   
                    <label class="col-sm-2 control-label">Claim No</label> 
                   <div class="col-sm-1">   
                   <asp:TextBox ID="TextPurchaseClaimID" style="display:none" runat="server"></asp:TextBox>
                    <asp:TextBox ID="TextClaimNo" class="form-control"  runat="server" AutoPostBack="True"  ontextchanged="TextClaimNo_TextChanged"></asp:TextBox>  
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextClaimNo" ErrorMessage="Insert Slip No." 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                 </div>
                  <div class="col-sm-3"><asp:Label ID="CheckClaimNo" runat="server"></asp:Label></div>  
               </div>
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Select Petty Cash Holder</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownEmployeeID" class="form-control select2" runat="server" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownEmployeeID" Display="Dynamic" 
                          ErrorMessage="Select Petty Cash Holder" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                    <div class="col-sm-3"><asp:Label ID="CheckEmpID" runat="server"></asp:Label> 
                    </div>
                </div>
                <div class="form-group">
                    <label class="col-sm-2 control-label">Claim Date</label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control pull-right" ID="EntryDate"  runat="server" ></asp:TextBox>  
                    </div>
                      </div> 
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                          ControlToValidate="EntryDate" ErrorMessage="Insert Claim Date" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                    <!-- /.input group -->
                  </div>
               <div class="form-group">
                  <label  class="col-sm-2 control-label">Pyement Type</label> 
                  <div class="col-sm-2">   
                    <asp:DropDownList ID="DropDownPaymentTypeID" class="form-control" runat="server" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownPaymentTypeID" Display="Dynamic" 
                          ErrorMessage="Select Payment Type" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>   
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Select Purchase Order Invoice No</label> 
                  <div class="col-sm-9">    
                  <asp:ListBox runat="server" ID="DropDownSlipNo" class="form-control select2"   SelectionMode="multiple"> 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                          ControlToValidate="DropDownSlipNo" Display="Dynamic" 
                          ErrorMessage="Select Purchase Order Invoice No."  SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">   
                    <label class="col-sm-2 control-label">Total Amount</label> 
                   <div class="col-sm-2">    
                    <asp:TextBox ID="TextTotalAmount" class="form-control"  runat="server"></asp:TextBox>  
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                          ControlToValidate="TextTotalAmount" ErrorMessage="Insert Total Amount of This Claim" 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                 </div> 
               </div> 
                  <div class="form-group">
                  <label  class="col-sm-2 control-label">Is Active Status</label> 
                  <div class="col-sm-3" style="padding-top:6px;">    
                        <label>
                            <input type="checkbox" ID="CheckIsActive" class="flat-red" checked runat="server"/>
                        </label>
                  </div>
                </div> 
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">     
                    <asp:LinkButton ID="BtnAdd" class="btn btn-primary" runat="server" Text="Add New" onclick="BtnAdd_Click"><span class="fa fa-plus"></span> Add New</asp:LinkButton>
                    <asp:LinkButton ID="BtnUpdate" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdate_Click"><span class="fa fa-edit"></span> Update</asp:LinkButton>
                    <asp:LinkButton ID="BtnDelete" class="btn btn-danger" runat="server" onclick="BtnDelete_Click" onclientclick="return confirm('Are you sure to delete?');" ><span class="fa fa-close"></span> Delete</asp:LinkButton>
                   
                </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Purchase Claim List</h3>
              <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchUser" Class="form-control" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="ButtonSearchUser" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridViewSearchUser" 
                        CausesValidation="False" />
                  </div>  
              </div>    
            </div>
            </div>
           
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView1" runat="server"    EnablePersistedSelection="true"   OnDataBound="gridViewFileInformation_DataBound"         
                        SelectedRowStyle-BackColor="Yellow" 
                        AllowPaging="true" 
                        AllowSorting="true"
                        PageSize = "10" 
                        OnPageIndexChanging="GridViewPage_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="CLAIM_NO" HeaderText="Claim No" />
                     <asp:BoundField DataField="EMP_NAME" HeaderText="Petty Cash Holder" />
                     <asp:BoundField DataField="CLAIM_DATE"  HeaderText="Claim Date"  DataFormatString="{0:dd/MM/yyyy}"  /> 
                     <asp:BoundField DataField="PAYMENT_TYPE_NAME" HeaderText="Payment Type" />  
                     <asp:BoundField DataField="TOTAL_AMOUNT" HeaderText="Total Amt." DataFormatString="{0:0,0.00}" />  
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date"  DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date"  DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField HeaderText="Query" ItemStyle-Width="100">
                        <ItemTemplate>  
                             <asp:Label ID="IsActiveQ" CssClass="label" Text='<%# Eval("IS_OBJ_QUERY").ToString() == "No" ? "<span Class=label-success style=Padding:2px >No<span>" : "<span Class=label-danger style=Padding:2px>Yes<span>" %>'  runat="server" /></asp:Label> 
                           </ItemTemplate> 
                     </asp:TemplateField>   
                      <asp:TemplateField HeaderText="Query Description">
                        <ItemTemplate>  
                             <asp:Label ID="Label1" Text='<%# Eval("OBJ_QUERY_DES")%>' runat="server"></asp:Label> -
                             <asp:Label ID="Label2" Text='<%# Eval("OBJ_QUERY_C_DATE", "{0:d/MM/yyyy h:mm:ss tt}")%>' runat="server"></asp:Label>
                          </ItemTemplate>   
                     </asp:TemplateField>    
                       
                      <asp:TemplateField  HeaderText="Action" >
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelect" class="btn btn-info btn-sm" runat="server" CommandArgument='<%#  Eval("CLAIM_NO")%>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField> 
                       <asp:TemplateField HeaderText="CMO Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveCheck" CssClass="label" Text='<%# Eval("IS_CHECK").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete<span>" : "<span Class=label-danger style=Padding:2px>Incomplete<span>" %>'  runat="server" /> 
                             <asp:Label ID="IsActiveCheckLink" style="display:none" CssClass="label" Text='<%# Eval("IS_CHECK").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                          </ItemTemplate>
                    </asp:TemplateField> 
                     <asp:BoundField DataField="SLIP_NO" HeaderText="Slip No" />
                     <asp:BoundField DataField="SUPPLIER_NAME" HeaderText="Supplier Details" />
                     
                    
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                    </asp:GridView> 
                 
        </div>
       </div>
  
    </div> 
          <!-- /.box --> 

      
          <!-- /.box --> 
        <!--/.col (right) -->


      </div>
      <!-- /.row -->
    </section>
    <!-- /.content -->
   
</div>
<style type="text/css"> 
    tr{ 
     border-top: 2px dotted #00c0ef !important;
    } 
 </style>
</asp:Content> 