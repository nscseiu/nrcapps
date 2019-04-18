<%@ Page Title="Sales Documents (Overseas) Form & List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="MfExpSalesInvoice.aspx.cs" Inherits="NRCAPPS.MF.MfExpSalesInvoice" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Sales Documents (Overseas) Form & List
        <small>Sales Documents (Overseas): - Add - Update - Delete - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Plastic Factory</a></li>
        <li class="active">Sales Invoice (Overseas)</li>
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
              <h3 class="box-title">Sales Documents (Overseas) Form</h3>
                <div class="box-tools">
                      <div class="col-sm-8">  
                      <asp:DropDownList ID="DropDownInvPrintFor"  class="form-control input-sm" runat="server" >   
                      </asp:DropDownList> 
                           <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" 
                          ControlToValidate="DropDownInvPrintFor" Display="Dynamic" 
                          ErrorMessage="Select Print For" InitialValue="0"  SetFocusOnError="True"></asp:RequiredFieldValidator>
                      </div>
                      <div class="col-sm-1">  
                       <asp:LinkButton ID="btnPrint" class="btn btn-warning"  runat="server" OnClick="btnPrint_Click" ><span class="fa fa-print"></span> Print</asp:LinkButton>
                    </div>
                      </div>
            </div>
            <!-- /.box-header -->
            <!-- form start -->
            
              <div class="box-body">
                <div class="form-group">   
                    <label class="col-sm-2 control-label">Export Sales Invoice No</label> 
                   <div class="col-sm-2">   
                   <asp:TextBox ID="TextExportSalesID" style="display:none" runat="server"></asp:TextBox>
                    <asp:TextBox ID="TextExportInvoiceNo" class="form-control input-sm"  runat="server" AutoPostBack="True"  ontextchanged="TextExportInvoiceNo_TextChanged" ></asp:TextBox>  
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" 
                          ControlToValidate="TextExportInvoiceNo" Display="Dynamic" 
                          ErrorMessage="Insert Invoice No (Overseas)"  SetFocusOnError="True"></asp:RequiredFieldValidator>
                 </div>
                  <div class="col-sm-3"><asp:Label ID="CheckExInvoiceNo" runat="server"></asp:Label></div>  
               </div>
                <div class="form-group"> 
                    <label class="col-sm-2 control-label">B/L No</label> 
                   <div class="col-sm-2">    
                    <asp:TextBox ID="TextBl" class="form-control input-sm"  runat="server"></asp:TextBox>  
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" 
                          ControlToValidate="TextBl" ErrorMessage="Insert B/L Number" 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                 </div> 
                   <label class="col-sm-1 control-label">PO No.</label> 
                   <div class="col-sm-2">    
                    <asp:TextBox ID="TextPoNo" class="form-control input-sm"  runat="server"></asp:TextBox>   
                 </div> 
                   <label class="col-sm-1 control-label">Ref.</label> 
                   <div class="col-sm-2">    
                    <asp:TextBox ID="TextRef" class="form-control input-sm"  runat="server"></asp:TextBox>  
                 </div> 
                </div> 
                  <div class="form-group">
                  <label  class="col-sm-2 control-label">Party Name</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownPartyID" class="form-control select2 input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" 
                          ControlToValidate="DropDownPartyID" Display="Dynamic" 
                          ErrorMessage="Select Party Name" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                   <label  class="col-sm-2 control-label">Shipping Incoterms</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownShippingIncoID" class="form-control input-sm" runat="server" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="DropDownShippingIncoID" Display="Dynamic" 
                          ErrorMessage="Select Shipping Incoterms" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div> 
                </div>
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Payment Terms</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownPayTermID" class="form-control input-sm" runat="server" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownPayTermID" Display="Dynamic" 
                          ErrorMessage="Select Payment Term" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                   <label  class="col-sm-2 control-label">Payment Terms Des.</label> 
                  <div class="col-sm-3">   
                        <asp:TextBox ID="TextPayTermDes" class="form-control input-sm"  runat="server"></asp:TextBox>  
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server" 
                          ControlToValidate="TextPayTermDes" ErrorMessage="Insert Payment Term Description" 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator> 
                  </div>
                </div>  
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">Trading Vessel</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownTradingVesselID" class="form-control select2 input-sm" runat="server" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="DropDownTradingVesselID" Display="Dynamic" 
                          ErrorMessage="Select Trading Vessel" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                       
                  </div> 
                     <label class="col-sm-2 control-label">Vessel Code</label> 
                   <div class="col-sm-3">    
                    <asp:TextBox ID="TextVesselCode" class="form-control input-sm"  runat="server"></asp:TextBox>   
                 </div> 
                </div> 
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Shipment From & To</label> 
                  <div class="col-sm-3" >   
                   <div class="input-group">  
                    <asp:DropDownList ID="DropDownShipmentFromToID"  class="form-control select2 input-sm" runat="server" >   
                    </asp:DropDownList>  
                      <span class="input-group-addon"><i class="fa fa-fw fa-ship"></i></span>  
                      </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownShipmentFromToID" Display="Dynamic" 
                          ErrorMessage="Select Shipment From & To" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>  
                  </div>   
                    <label class="col-sm-2 control-label">Shipped On Date</label>
                     <div class="col-sm-3">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                     <asp:TextBox  class="form-control input-sm pull-right" ID="EntryDate"  runat="server" ></asp:TextBox>  
                     </div> <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                          ControlToValidate="EntryDate" ErrorMessage="Insert Shipment Date" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator> 
                      </div>  
                  </div>
                  <div class="form-group">
                  <label  class="col-sm-2 control-label">Select WS No. / Container No.</label> 
                  <div class="col-sm-8">    
                  <asp:ListBox runat="server" ID="DropDownSlipNoEx" class="form-control select2"   SelectionMode="multiple"> 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                          ControlToValidate="DropDownSlipNoEx" Display="Dynamic" 
                          ErrorMessage="Select Weight Slip No. / Container No."  SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
                    
                  <div class="form-group">  
                       <label  class="col-sm-2 control-label"></label> 
                        <!-- Button trigger modal -->
                       <div class="col-sm-2">    
                                <asp:Button type="button" id="contract_id" class="btn btn-warning" data-toggle="modal" data-target="#myModal">
                                + Add Sales Order No
                                </asp:Button>
                                <!-- Modal -->
                                <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                  <div class="modal-dialog">
                                    <div class="modal-content">
                                      <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                                        <h4 class="modal-title" id="myModalLabel">Sales Order Form</h4>
                                      </div>
                                      <div class="modal-body">
                                          <div class="form-group">   
                                            <label class="col-sm-4 control-label">Sales Order No.</label> 
                                            <div class="col-sm-4">    
                                              <asp:TextBox ID="TextContractNo" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" 
                                            ControlToValidate="TextContractNo" ErrorMessage="Insert Contract No." 
                                            Display="Dynamic" SetFocusOnError="True" ValidationGroup="VgContract"></asp:RequiredFieldValidator>
                                            </div>   
                                          </div> 
                                            <div class="form-group">  
                                              <label  class="col-sm-4 control-label">Sales Order Serial No.</label> 
                                            <div class="col-sm-4"> 
                                               <asp:TextBox ID="TextContractNoSerial" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                                            </div> 

                                            </div>
                                            <div class="form-group">             
                                              <label class="col-sm-4 control-label">Sales Order Date</label>
                                            <div class="col-sm-4">   
                                                <div class="input-group date">
                                                <div class="input-group-addon">
                                                <i class="fa fa-calendar"></i>
                                                </div>  
                                                <asp:TextBox  class="form-control input-sm pull-right" ID="EntryDate1"  runat="server" ></asp:TextBox>  
                                                </div> <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" 
                                            ControlToValidate="EntryDate1" ErrorMessage="Insert Contarct Date" 
                                            Display="Dynamic" SetFocusOnError="True" ValidationGroup="VgContract"  ></asp:RequiredFieldValidator> 
                                            </div>  
                                            </div> 
                                      </div>
                                      <div class="modal-footer">
                                         <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                                         <asp:LinkButton ID="BtnAddContract" class="btn btn-primary" runat="server" Text="Add New" onclick="BtnAddContract_Click" ValidationGroup="VgContract"><span class="fa fa-plus"></span> Add New</asp:LinkButton>
                                     
                                          
                                       <div class="box-body"> 
                                        <asp:GridView ID="GridView2" runat="server"    EnablePersistedSelection="true"          
                                            SelectedRowStyle-BackColor="Yellow"   AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                                         <Columns>     
                                         <asp:TemplateField HeaderText="Order No" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate> 
                                                 <asp:Label ID="ContractNo"   Text='<%#Bind("CONTRACT_NO")%>'  runat="server" /> 
                                                <small class="label bg-blue"> <asp:Label ID="ContractNoSerial" Text='<%#Bind("CONTRACT_NO_SERIAL") %>'  runat="server" /></small> 
                                            </ItemTemplate>
                                         </asp:TemplateField>
                                         <asp:BoundField DataField="CONTRACT_DATE"  HeaderText="Order Date"  DataFormatString="{0:dd/MM/yyyy}"  />  
                                         <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date"  DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />   
                     
                                          <asp:TemplateField  HeaderText="Action" >
                                           <ItemTemplate> 
                                            <asp:LinkButton ID="DeleteContract" class="btn btn-danger btn-sm" runat="server" CommandArgument='<%#  Eval("EXPORT_CONTRACT_NO_ID")%>' OnClick="DeleteContractClick" CausesValidation="False">Delete</asp:LinkButton> 
                                            </ItemTemplate>
                                           </asp:TemplateField>  
                                         </Columns>
                                            <PagerStyle CssClass="pagination-ys" />
                                            <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                                        </asp:GridView> 
                 
                                     </div>
                                      
                                      </div> 
                                       
                                    </div>
                                  </div>
                                </div>
                           </div>    
                    <label class="col-sm-1 control-label">Invoice Date</label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                     <asp:TextBox  class="form-control input-sm pull-right" ID="EntryDate2"  runat="server" ></asp:TextBox>  
                     </div>  <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" 
                          ControlToValidate="EntryDate2" ErrorMessage="Insert Invoice Date" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator> 
                      </div>
                    <label  class="col-sm-1 control-label">Packing Type</label> 
                  <div class="col-sm-2">   
                    <asp:DropDownList ID="DropDownPackingTypeID" class="form-control input-sm" runat="server" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server" 
                          ControlToValidate="DropDownPackingTypeID" Display="Dynamic" 
                          ErrorMessage="Select Packing Type" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                  </div>   
                  <div class="form-group">
                  <label  class="col-sm-2 control-label">Is Active Status</label> 
                  <div class="col-sm-1" style="padding-top:6px;">    
                        <label>
                            <input type="checkbox" ID="CheckIsActive" class="flat-red" checked runat="server"/>
                        </label>
                  </div>
                  <label  class="col-sm-2 control-label">Shipping Marks Status</label> 
                  <div class="col-sm-1" style="padding-top:6px;">    
                        <label>
                            <input type="checkbox" ID="CheckIsShipMarksActive" class="flat-red" runat="server"/>
                        </label>
                  </div>
                  <label  class="col-sm-2 control-label">Bank Acoount</label> 
                  <div class="col-sm-2">   
                    <asp:DropDownList ID="DropDownAccountID" class="form-control input-sm" runat="server" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                          ControlToValidate="DropDownAccountID" Display="Dynamic" 
                          ErrorMessage="Select Bank Account" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
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
              <h3 class="box-title">Sales Documents (Overseas) List</h3>
              <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchUser" Class="form-control" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="GridView1SearchUser" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridView1Search" 
                        CausesValidation="False" ValidationGroup='GridView1Search' />
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
                        PageSize = "15" 
                        OnPageIndexChanging="GridViewPage_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns> 
                     <asp:BoundField DataField="EXPORT_INVOICE_NO" HeaderText="Invoice No." />
                       <asp:TemplateField  HeaderText="Action" >
                           <ItemTemplate>
                            <asp:LinkButton ID="linkSelect" class="btn btn-info btn-sm" runat="server" CommandArgument='<%#  Eval("EXPORT_SALES_ID")%>' OnClick="LinkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                            </ItemTemplate>
                       </asp:TemplateField>  
                     <asp:BoundField DataField="PAY_TERMS_NAME"  HeaderText="PT"  />  
                     <asp:BoundField DataField="SHIPPING_INCO_NAME" HeaderText="Incoterms" />                       
                     <asp:BoundField DataField="INVOICE_DATE"  HeaderText="Invoice Date"  DataFormatString="{0:dd/MM/yyyy}"  />  
                     <asp:TemplateField HeaderText="Vessel" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="VesselName"   Text='<%#Bind("TRADING_VESSEL_NAME")%>'  runat="server" /> 
                            <small class="label bg-orange"> <asp:Label ID="VesselCode" Text='<%#Bind("TRADING_VESSEL_CODE") %>'  runat="server" /></small> 
                        </ItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="PARTY_NAME" HeaderText="Party Name" />   
                     <asp:BoundField DataField="WB_SLIP_NO" HeaderText="WB Slip No" />    
                     <asp:BoundField DataField="CONTAINER_NO" HeaderText="Con. No" />   
                     <asp:BoundField DataField="ITEM_WEIGHT" HeaderText="Quantity" DataFormatString="{0:0,0.000}"  />                           
                     <asp:BoundField DataField="MAT_PRICE_PER_MT" HeaderText="Price/MT. $USD" DataFormatString="{0:0,0.00}" />                         
                     <asp:BoundField DataField="MATERIAL_AMOUNT" HeaderText="Amount $USD" DataFormatString="{0:0,0.00}" />  
                    
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=\"label label-success\">Enable<span>" : "<span Class=\"label label-danger\">Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField> 
                       <asp:TemplateField HeaderText="Is Print" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="IsPrintedCheck" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "<span Class=\"label label-success\">Printed</span></br>" : "<span Class=\"label label-danger\">Not Printed</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedDate" class="text-green" Style="font-size:11px;" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? Eval("PRINT_DATE") : "" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedCheckLink" style="display:none" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "Printed" : "Not_Printed" %>'  runat="server" /> 
                        </ItemTemplate>
                     </asp:TemplateField>  
                        <asp:TemplateField HeaderText="Sales Order No" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="ContractNo"   Text='<%#Bind("CONTRACT_NO")%>'  runat="server" />  
                        </ItemTemplate>
                     </asp:TemplateField> 
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date"  DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date"  DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                    </asp:GridView> 
                 
        </div>
       </div>
     <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title"> Shipment Statement (Overseas Sales - Month Wise) Parameter</h3>
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
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" 
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
    </div> 
          <!-- /.box --> 

      
          <!-- /.box --> 
        <!--/.col (right) -->


      </div>
      <!-- /.row -->
    </section>
    <!-- /.content -->
    <asp:Panel ID="PanelPrint" runat="server" ></asp:Panel>  
</div>
<style type="text/css"> 
    tr{ 
     border-top: 2px dotted #00c0ef !important;
    } 
 </style>
</asp:Content> 