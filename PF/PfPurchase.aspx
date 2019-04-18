<%@ Page Title="Purchase Form & List - Plastic Factory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="PfPurchase.aspx.cs" Inherits="NRCAPPS.PF.PfPurchase" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Purchase Form & List
        <small>Purchase: - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Plastic Factory</a></li>
        <li class="active">Purchase</li>
      </ol>
    </section>
    
    <!-- Main content -->
    <section class="content">
      <div class="row">
        <!-- left column --> 
            
        <!--/.col (left) -->
        <!-- right column -->
        <div class="col-md-7">  
             <asp:Panel  id="alert_box" runat="server">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <h4><i class="icon fa fa-check"></i> Alert!</h4>  
           </asp:Panel> 
          <!-- Horizontal Form -->
          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Purchase Form</h3>
                <div class="box-tools">
                       <asp:LinkButton ID="btnPrint" class="btn btn-warning"  runat="server" OnClick="btnPrint_Click" ><span class="fa fa-print"></span> Purchase Order Print</asp:LinkButton>
                 </div>
            </div>
                 
            <!-- /.box-header -->
            <!-- form start -->
            
              <div class="box-body">
              <div class="form-group">
                  <label  class="col-sm-3 control-label"> Slip Is</label> 
                  <div class="col-sm-4"> 
                    <asp:RadioButtonList ID="radPurDuplicate" runat="server" AutoPostBack = "true"  onselectedindexchanged="Redio_CheckedChanged" RepeatDirection="Horizontal">
                         <asp:ListItem Value="One" Selected="True">One &nbsp;</asp:ListItem>
                         <asp:ListItem Value="Duplicate">Duplicate </asp:ListItem> 
                    </asp:RadioButtonList>     
                  </div>
                 </div>  
                <div class="form-group">   
                    <label class="col-sm-3 control-label">Slip No</label> 
                   <div class="col-sm-3">  
                    <asp:TextBox ID="TextPurchaseID" class="form-control input-sm" style="display:none"  runat="server"></asp:TextBox>    
                    <asp:TextBox ID="TextSlipNo" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                        <img src="../image/loader.gif" id="loader" style="display:none" height="20px" alt="" />
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextSlipNo" ErrorMessage="Insert Slip No." 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                 </div>
                  <div class="col-sm-4"><asp:Label ID="CheckSlipNo" runat="server"></asp:Label></div>  
               </div>
               <div class="form-group">
                  <label  class="col-sm-3 control-label">Supplier Name</label> 
                  <div class="col-sm-8">   
                    <asp:DropDownList ID="DropDownSupplierID" class="form-control select2 input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownSupplierID" Display="Dynamic" 
                          ErrorMessage="Select Supplier" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Supervisor Name</label> 
                  <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownSupervisorID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                          ControlToValidate="DropDownSupervisorID" Display="Dynamic" 
                          ErrorMessage="Select Supervisor" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>
                 <div class="form-group" style="display:none">
                  <label  class="col-sm-3 control-label">Purchase Type</label> 
                  <div class="col-sm-4">   
                    <asp:DropDownList ID="DropDownPurchaseTypeID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" 
                          ControlToValidate="DropDownPurchaseTypeID" Display="Dynamic" 
                          ErrorMessage="Select Purchase Type" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Item</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownItemID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownItemID" Display="Dynamic" 
                          ErrorMessage="Select Item" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div> 
                    <label  class="col-sm-2 control-label">Sub Item</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownSubItemID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>   
                  </div>
                </div>
                 
                  <div class="form-group">
                  <label  class="col-sm-3 control-label"> Item WT Weigh-Bridge</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextItemWtWb" class="form-control input-sm"  runat="server" ></asp:TextBox>   
                    <span class="input-group-addon">MT</span>      
                    </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server"   ControlToValidate="TextItemWtWb" ErrorMessage="Insert Item WT  Weigh-Bridge" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>  
                      <label  class="col-sm-2 control-label">Item Weight</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextItemWeight" class="form-control input-sm"  runat="server" ></asp:TextBox> <!--  AutoPostBack="True"  ontextchanged="TextItemRate_Changed" -->   
                    <span class="input-group-addon">MT</span>      
                    </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server"   ControlToValidate="TextItemWeight" ErrorMessage="Insert Item Weight" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Item Rate</label> 
                  <div class="col-sm-3">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextItemRate" class="form-control input-sm"  runat="server" ></asp:TextBox> <!--  AutoPostBack="True"  ontextchanged="TextItemRate_Changed" --> 
                    <span class="input-group-addon">.00</span>      
                    </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" 
                          ControlToValidate="TextItemRate" ErrorMessage="Insert Item Rate" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                  </div>
                    <label  class="col-sm-2 control-label">Item Amount</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextItemAmount" class="form-control input-sm"  runat="server"></asp:TextBox>    
                       <span class="input-group-addon">SR</span>      
                    </div>
                  </div>
                </div> 
               <div class="form-group">
                  <label  class="col-sm-3 control-label">Vat Percent</label>
                   <div class="col-sm-2"> 
                       <asp:RadioButtonList ID="RadioBtnVat" runat="server"  RepeatDirection="Horizontal">
                             <asp:ListItem Value="VatNo" Selected="True">No &nbsp;</asp:ListItem>
                             <asp:ListItem Value="VatYes">Yes </asp:ListItem>  
                        </asp:RadioButtonList>  
                    </div> 
                 <div id="VatPercentBox" style="display:none;"  runat="server">
                  <div class="col-sm-2" >   
                   <div class="input-group">  
                    <asp:DropDownList ID="DropDownVatID"  class="form-control input-sm" runat="server" >   
                    </asp:DropDownList>  
                      <span class="input-group-addon">%</span>  
                      </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server" 
                          ControlToValidate="DropDownVatID" Display="Dynamic" 
                          ErrorMessage="Select Vat percent (%)" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator> 
                     
                  </div>   
                   <div class="col-sm-2"> 
                         <asp:TextBox ID="TextItemVatAmount" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                     </div>
                  </div>
                </div> 
                   <div class="form-group"> 
                     <label  class="col-sm-3 control-label">Total Amount</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextTotalAmount" class="form-control input-sm"  runat="server"></asp:TextBox>    
                       <span class="input-group-addon">SR</span>      
                    </div>    
                  </div> 
                </div> 
                 <div class="form-group">
                    <label class="col-sm-3 control-label">Entry Date</label>
                     <div class="col-sm-4">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="EntryDate"  runat="server"   ></asp:TextBox> <!-- AutoPostBack="True"  ontextchanged="TextCheckDataProcess" -->
                    </div>  <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                          ControlToValidate="EntryDate" ErrorMessage="Insert Entry Date" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                      </div> 
                      
                    <!-- /.input group -->
                    <div class="col-sm-4"><asp:Label ID="CheckEntryDate" runat="server"></asp:Label></div>
                  </div>
                 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Is Active Status</label> 
                  <div class="col-sm-4" style="padding-top:6px;">    
                        <label>
                            <input type="checkbox" ID="CheckIsActive" class="flat-red" checked 
                            runat="server" tabindex="1"/>
                        </label>
                  </div>
                </div> 
                 <!-- checkbox -->
              
                 
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-3" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" 
                          OnClick="clearTextField" CausesValidation="False" TabIndex="1"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">    
                    <asp:LinkButton ID="BtnAdd" class="btn btn-primary" runat="server" Text="Add New" onclick="BtnAdd_Click" OnClientClick="return CheckIsRepeat();" ><span class="fa fa-plus"></span> Add New</asp:LinkButton>
                    <asp:LinkButton ID="BtnUpdate" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdate_Click"><span class="fa fa-edit"></span> Update</asp:LinkButton>
                    <asp:LinkButton ID="BtnDelete" class="btn btn-danger" runat="server" onclick="BtnDelete_Click" onclientclick="return confirm('Are you sure to delete?');" ><span class="fa fa-close"></span> Delete</asp:LinkButton>
                  </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>     </div> 
          <div class="col-md-5">        
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Purchase Summary (Current Month-Default)</h3>
              <div class="box-tools"> 
              <div class="input-group input-group-sm" style="width: 200px;"> 
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="TextMonthYear4"  runat="server" ></asp:TextBox>  
                    </div>
                 <div class="input-group-btn">
                      <asp:Button ID="Button1" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridViewSearchSummary" 
                        CausesValidation="False" />
                  </div>  
              </div>    
            </div>
            </div>
           
            <!-- /.box-header -->
            <div class="box-body table-responsive">
               <asp:GridView ID="GridView2" runat="server" EnablePersistedSelection="false"  
    SelectedRowStyle-BackColor="Yellow"  AutoGenerateColumns="false" ShowHeader="true"  ShowFooter="true" CssClass="table  table-sm table-bordered table-striped"  >
                     <Columns>
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item Name" />
                     <asp:BoundField DataField="SLIP_NO" HeaderText="Total Slip" ItemStyle-HorizontalAlign="Center" />
                     <asp:BoundField DataField="ITEM_WEIGHT" HeaderText="Total WET" DataFormatString="{0:0,0.000}" ItemStyle-HorizontalAlign="Right" />
                     <asp:BoundField DataField="ITEM_AMOUNT"  HeaderText="Total Amount" DataFormatString="{0:0,0.00}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="ITEM_AVG_RATE"  HeaderText="Avg. Rate" DataFormatString="{0:0,0.00}" ItemStyle-HorizontalAlign="Right" /> 
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                    </asp:GridView> 
                 
        </div>
       </div>
        
         
      
        </div>   
      <div class="col-md-12">        
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Purchase List</h3>
              <div class="box-tools">
              <div class="col-sm-4">   
                    <asp:DropDownList ID="DropDownItemID1" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>
               </div> 
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchEmp" Class="form-control" runat="server" />
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
              
                    <asp:GridView ID="GridView4D" runat="server"    EnablePersistedSelection="true"  OnDataBound="gridViewFileInformation_DataBound"        
    SelectedRowStyle-BackColor="Yellow"   AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="SLIP_NO" HeaderText="Slip No" />
                     <asp:BoundField DataField="PARTY_NAME" HeaderText="Supplier Name" />
                     <asp:BoundField DataField="PUR_TYPE_NAME"  HeaderText="Type" />
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" /> 
                     <asp:BoundField DataField="SUB_ITEM_NAME"  HeaderText="Sub Item" />
                     <asp:BoundField DataField="SUPERVISOR_NAME"  HeaderText="Supervisor" />  
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Weight" DataFormatString="{0:0.000}" />
                     <asp:BoundField DataField="ITEM_RATE"  HeaderText="Rate" />   
                     <asp:BoundField DataField="ITEM_AMOUNT"  HeaderText="Amount" DataFormatString="{0:0,0.00}" />     
                     <asp:BoundField DataField="VAT_AMOUNT"  HeaderText="Vat Amt" DataFormatString="{0:0,0.00}" />  
                     <asp:BoundField DataField="TOTAL_AMOUNT"  HeaderText="Total Amt" DataFormatString="{0:0,0.00}" />  
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
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("PURCHASE_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField>  
                     <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date"  DataFormatString="{0:dd/MM/yyyy}"  />
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     
                      <asp:TemplateField HeaderText="CMO Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsCmoCheck" CssClass="label" Text='<%# Eval("IS_CHECK").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete</span>" : "<span Class=label-danger style=Padding:2px>Incomplete</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckLink" style="display:none" CssClass="label" Text='<%# Eval("IS_CHECK").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
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
              <h3 class="box-title"> Purchase Statement (Supplier - Month Wise) Parameter</h3>
            </div>
            <!-- /.box-header -->

              <div class="box-body">
            <!-- form start -->   
              <div class="form-group">
                  <label  class="col-sm-2 control-label">Supplier Name</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownSupplierID2" class="form-control select2 input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="DropDownSupplierID2" Display="Dynamic" 
                          ErrorMessage="Select Supplier" InitialValue="0" SetFocusOnError="True"  ValidationGroup='valGroup2'></asp:RequiredFieldValidator>
                  </div>
                </div>
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
              <h3 class="box-title"> Purchase Statement (Supplier - Month Wise) Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="PF_Reports/PfPurchaseSupplierWiseReportView.aspx?MonthYear=<%=TextMonthYear0.Text %>&SupplierID=<%=DropDownSupplierID2.Text %>" width="950px" id="iframe1"
                        marginheight="0" frameborder="0" scrolling="auto" height="1250px">   </iframe>  
                         </div>
                       </div> 
                    
                     <%} %>   
            </asp:Panel>  

             <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title"> Summary of Purchases (Month Wise) Parameter</h3>
            </div>
            <!-- /.box-header -->

              <div class="box-body">
            <!-- form start -->   
              <div class="form-group">
                  <label  class="col-sm-2 control-label">Purchase Type</label> 
                  <div class="col-sm-2">   
                    <asp:DropDownList ID="DropDownPurchaseTypeID2" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                          ControlToValidate="DropDownPurchaseTypeID2" Display="Dynamic" 
                          ErrorMessage="Select Supplier" InitialValue="0" SetFocusOnError="True"  ValidationGroup='valGroup3'></asp:RequiredFieldValidator>
                  </div>
                </div>
                   <div class="form-group">
                    <label class="col-sm-2 control-label">Select Month </label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="TextMonthYear1"  runat="server" ></asp:TextBox>  
                    </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" 
                         ControlToValidate="TextMonthYear1" ErrorMessage="Insert Date" 
                         SetFocusOnError="True" Display="Dynamic" ValidationGroup='valGroup3'></asp:RequiredFieldValidator> 
                  </div>
                  </div>
                </div> 
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton2" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">     
                    <asp:LinkButton ID="LinkButton3" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport2_Click"  ValidationGroup='valGroup3' ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                   </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
        
            <asp:Panel ID="Panel2" runat="server">  
            <%if (IsLoad2)
                {%> 
            <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title"> Summary of Purchases Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="PF_Reports/PfPurchaseSummaryReportView.aspx?MonthYear=<%=TextMonthYear1.Text %>&PurchaseTypeID=<%=DropDownPurchaseTypeID2.Text %>" width="950px" id="iframe2"
                        marginheight="0" frameborder="0" scrolling="auto" height="1250px">   </iframe>  
                         </div>
                       </div> 
                     
                     <%} %>   
            </asp:Panel>  

          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title"> Supervisor Statement (Purchases - Monthly) Parameter</h3>
            </div>
            <!-- /.box-header -->

              <div class="box-body">
            <!-- form start -->   
             <div class="form-group">
                  <label  class="col-sm-2 control-label">Supervisor Name</label> 
                  <div class="col-sm-2">   
                    <asp:DropDownList ID="DropDownSupervisorID2" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" 
                          ControlToValidate="DropDownSupervisorID2" Display="Dynamic" 
                          ErrorMessage="Select Supervisor" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup4'></asp:RequiredFieldValidator>
                  </div>
                </div>
                   <div class="form-group">
                    <label class="col-sm-2 control-label">Select Month </label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="TextMonthYear2"  runat="server" ></asp:TextBox>  
                    </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" 
                         ControlToValidate="TextMonthYear2" ErrorMessage="Insert Date" 
                         SetFocusOnError="True" Display="Dynamic" ValidationGroup='valGroup4'></asp:RequiredFieldValidator> 
                  </div>
                  </div>
                </div> 
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton4" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">     
                    <asp:LinkButton ID="LinkButton5" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport3_Click"  ValidationGroup='valGroup4' ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                   </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->


        </div>   
        
            <asp:Panel ID="Panel3" runat="server">  
            <%if (IsLoad3)
                {%> 
            <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title"> Payment Summary (Purchases - Monthly) Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="PF_Reports/PfPurchaseSupervisorReportView.aspx?MonthYear=<%=TextMonthYear2.Text %>&SupervisorID=<%=DropDownSupervisorID2.Text %>" width="950px" id="iframe3"
                        marginheight="0" frameborder="0" scrolling="auto" height="1250px">   </iframe>  
                         </div>
                       </div> 
                     
                     <%} %>   
            </asp:Panel>  

                  <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title"> Payment Summary (Purchases - Monthly) Parameter</h3>
            </div>
            <!-- /.box-header -->

              <div class="box-body">
            <!-- form start -->   
              <div class="form-group">
                  <label  class="col-sm-2 control-label">Supplier Name</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownSupplierID3" class="form-control select2 input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" 
                          ControlToValidate="DropDownSupplierID3" Display="Dynamic" 
                          ErrorMessage="Select Supplier" InitialValue="0" SetFocusOnError="True"  ValidationGroup='valGroup5'></asp:RequiredFieldValidator>
                  </div>
                </div>
                   <div class="form-group">
                    <label class="col-sm-2 control-label">Select Month </label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="TextMonthYear3"  runat="server" ></asp:TextBox>  
                    </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" 
                         ControlToValidate="TextMonthYear3" ErrorMessage="Insert Date" 
                         SetFocusOnError="True" Display="Dynamic" ValidationGroup='valGroup5'></asp:RequiredFieldValidator> 
                  </div>
                  </div>
                </div> 
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton6" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">     
                    <asp:LinkButton ID="LinkButton7" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport4_Click"  ValidationGroup='valGroup5' ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                   </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->


        </div>   
        
            <asp:Panel ID="Panel4" runat="server">  
            <%if (IsLoad4)
                {%> 
            <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title"> Payment Summary (Purchases - Monthly) Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="PF_Reports/PfPurchasePaymentSummaryReportView.aspx?MonthYear=<%=TextMonthYear3.Text %>&SupplierID=<%=DropDownSupplierID3.Text %>" width="950px" id="iframe4"
                        marginheight="0" frameborder="0" scrolling="auto" height="1250px">   </iframe>  
                         </div>
                       </div> 
                     
                     <%} %>   
            </asp:Panel> 
    
        
          <!-- /.box --> 
        <!--/.col (right) -->
      </div> 
      <!-- /.row -->
    </section>
    <!-- /.content -->
   <asp:Panel ID="PanelPrint" runat="server" ></asp:Panel>  
</div> 
</asp:Content> 