﻿<%@ Page Title="Job Work (Sales) Form & List - Plastic Factory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="PfSalesJw.aspx.cs" Inherits="NRCAPPS.PF.PfSalesJw" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Job Work (Sales) Form & List
        <small>Job Work (Sales): - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Plastic Factory</a></li>
        <li class="active">Job Work (Sales)</li>
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
              <h3 class="box-title">Job Work (Sales) Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
               <div class="box-body">
                  <div class="form-group">   
                        <label class="col-sm-2 control-label">Invoice No.</label> 
                       <div class="col-sm-2">    
                        <asp:TextBox ID="TextInvoiceNo" class="form-control input-sm"  runat="server" AutoPostBack="True"  ontextchanged="TextInvoiceNo_TextChanged"></asp:TextBox>  
                         <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" 
                              ControlToValidate="TextInvoiceNo" ErrorMessage="Insert Invoice No." 
                              Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                     </div>
                      <div class="col-sm-3"><asp:Label ID="CheckInvoiceNo" runat="server"></asp:Label></div>  
                   </div> 
               <div class="form-group">
                  <label  class="col-sm-2 control-label">Customer Name</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownCustomerID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownCustomerID" Display="Dynamic" 
                          ErrorMessage="Select Customer" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>  
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Item</label> 
                  <div class="col-sm-2">   
                    <asp:DropDownList ID="DropDownItemID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownItemID" Display="Dynamic" 
                          ErrorMessage="Select Item" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">Sub Item</label> 
                  <div class="col-sm-2">   
                    <asp:DropDownList ID="DropDownSubItemID" class="form-control input-sm" runat="server" >  </asp:DropDownList>  <!-- AutoPostBack="True"  ontextchanged="TextSubItem_Changed" -->
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server" 
                          ControlToValidate="DropDownSubItemID" Display="Dynamic" 
                          ErrorMessage="Select Sub Item" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>  
                  </div>
                </div>
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">Item Weight</label> 
                  <div class="col-sm-2"> 
                  <div class="input-group"> 
                    <asp:TextBox ID="TextItemWeight" class="form-control input-sm"  runat="server"  ></asp:TextBox> <!-- AutoPostBack="True"  ontextchanged="TextItemWeight_TextChanged" -->
                    <span class="input-group-addon">MT</span>      
                    </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextItemWeight" ErrorMessage="Insert Material Weight" 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                   <div class="col-sm-5"><asp:Label ID="CheckItemWeight" runat="server"></asp:Label></div>  
                </div>  
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">Item Rate</label> 
                  <div class="col-sm-2">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextItemRate" class="form-control input-sm"  runat="server"  AutoPostBack="True"  ontextchanged="TextItemRate_Changed"></asp:TextBox>   
                    <span class="input-group-addon">.00</span>      
                    </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" 
                          ControlToValidate="TextItemRate" ErrorMessage="insert Item Rate" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Item Amount</label> 
                  <div class="col-sm-2">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextItemAmount" class="form-control input-sm"  runat="server" disabled="disabled"></asp:TextBox>    
                       <span class="input-group-addon">SR</span>      
                    </div>
                  </div>
                </div>  
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Vat Percent</label> 
                  <div class="col-sm-2">  
                   <div class="input-group"> 
                    <asp:DropDownList ID="DropDownVatID" class="form-control input-sm" runat="server" >   
                    </asp:DropDownList>  
                      <span class="input-group-addon">%</span>  
                      </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" 
                          ControlToValidate="DropDownVatID" Display="Dynamic" 
                          ErrorMessage="Select Vat percent (%)" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator> 
                  </div> 
                </div>  
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Vat Amount</label> 
                  <div class="col-sm-2">   
                    <div class="input-group"> 
                         <asp:TextBox ID="TextVatAmount" class="form-control input-sm"  runat="server" disabled="disabled"></asp:TextBox>    
                         <span class="input-group-addon">SR</span>  
                      </div>
                  </div> 
                </div>  
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Remarks</label> 
                  <div class="col-sm-2">    
                         <asp:TextBox ID="TextRemarks" class="form-control input-sm"  runat="server" ></asp:TextBox>     
                      </div>
                  </div> 
               
                 <div class="form-group">
                    <label class="col-sm-2 control-label">Entry Date</label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control input-sm pull-right" ID="EntryDate"  runat="server"   ></asp:TextBox>  <!-- AutoPostBack="True"  ontextchanged="TextCheckDataProcess" -->
                    </div>
                         <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                          ControlToValidate="EntryDate" ErrorMessage="insert Entry Date" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                      </div> 
                      <div class="col-sm-3"><asp:Label ID="CheckEntryDate" runat="server"></asp:Label></div>
                    <!-- /.input group -->
                  </div>
                 
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Is Active Status</label> 
                  <div class="col-sm-3" style="padding-top:6px;">    
                        <label>
                            <input type="checkbox" ID="CheckIsActive" class="flat-red" checked runat="server"/>
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
              <h3 class="box-title">Job Work (Sales) List</h3>
              <div class="box-tools">
               <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownItemID1" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>
               </div> 
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchEmp" Class="form-control input-sm" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="ButtonSearchEmp" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridViewSearchEmp" 
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
    PageSize = "10" 
    OnPageIndexChanging="GridViewEmp_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="INVOICE_NO" HeaderText="Invoice No." /> 
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
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("INVOICE_NO") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
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
              <h3 class="box-title"> Job Work Summary (Item Wise - Monthly) Parameter</h3>
            </div>
            <!-- /.box-header --> 
              <div class="box-body">
            <!-- form start -->   
                   <div class="form-group">
                    <label class="col-sm-2 control-label">Select Month </label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="TextMonthYear0"  runat="server" ></asp:TextBox>  
                    </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                         ControlToValidate="TextMonthYear0" ErrorMessage="Insert Date" 
                         SetFocusOnError="True" Display="Dynamic" ValidationGroup='valGroup2'></asp:RequiredFieldValidator> 
                  </div>
                  </div>
                </div> 
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton1" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">     
                    <asp:LinkButton ID="BtnReport" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport_Click"  ValidationGroup='valGroup2' ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                   </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
        
            <asp:Panel ID="Panel1" runat="server">  
            <%if (IsLoad)
                {%> 
            <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title"> Job Work Summary (Item Wise - Monthly) Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="PF_Reports/PfJWMonthlyReportView.aspx?MonthYear=<%=TextMonthYear0.Text %>" width="1350px" height="950px" id="iframe1"
                        marginheight="0" frameborder="0" scrolling="auto" >   </iframe>  
                         </div>
                       </div> 
                    
                     <%} %>   
            </asp:Panel>  
             
    </div> 
          <!-- /.box --> 
        <!--/.col (right) -->
      </div>
      <!-- /.row -->
    </section>
    <!-- /.content -->
 
</div>
</asp:Content> 