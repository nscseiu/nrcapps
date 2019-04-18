<%@ Page Title="Shipment Status List - Plastic Factory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="PfExpShipmentCmo.aspx.cs" Inherits="NRCAPPS.PF.PfExpShipmentCmo" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Shipment Status List
        <small>Shipment Status: - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Plastic Factory</a></li>
        <li class="active">Shipment Status</li>
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
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Shipment Status Transit List</h3>
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
    PageSize = "12" 
    OnPageIndexChanging="GridView1_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns> 
                      <asp:BoundField DataField="WB_SLIP_NO" HeaderText="WB Slip No" />
                       <asp:TemplateField HeaderText="Container No." ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="ContainerNo"   Text='<%#Bind("CONTAINER_NO")%>'  runat="server" /> 
                            <small class="label bg-blue"> <asp:Label ID="ContainerSize" Text='<%#Bind("CONTAINER_SIZE") %>'  runat="server" /></small> 
                        </ItemTemplate>
                     </asp:TemplateField>
                         <asp:TemplateField HeaderText="Action">
                     <ItemTemplate>   
                        <asp:CheckBox ID="IsShipmentCheck" runat="server"  class="flat-red" AutoPostBack = "true" onclientclick="return confirm('Are you sure to Shipment is Complete?');" oncheckedchanged="ShipmentUpdateTransit_Click" />  
                     </ItemTemplate>
                     </asp:TemplateField> 
                           <asp:TemplateField HeaderText="Is Shipment/Inventory Status" > 
                        <ItemTemplate> 
                             <asp:Label ID="IsInvenStatus" CssClass="label" Text='<%# Eval("IS_INVENTORY_STATUS").ToString() == "Transit" ? "<img src=../image/icon/transit.png ><br><br><span Class=label-danger style=Padding:2px >------ Transit ------><span>" : "<img src=../image/icon/shipping_complete.png ><br><br><span Class=label-success style=Padding:2px> Shpinng Complete<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:BoundField DataField="DISPATCH_DATE"  HeaderText="Dispatch Date"  DataFormatString="{0:dd/MM/yyyy}"  /> 
                     <asp:BoundField DataField="SEAL_NO"  HeaderText="Seal No." /> 
                     <asp:BoundField DataField="REL_ORDER_NO"  HeaderText="Rel Order No." /> 
                     <asp:BoundField DataField="PARTY_NAME" HeaderText="Company Name" /> 
                     <asp:BoundField DataField="ITEM_WEIGHT_WB"  HeaderText="Weight WB" DataFormatString="{0:0.0}" />   
                     
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField> 

                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                      
                      <asp:TemplateField HeaderText="Is Create Invoice" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsCmoCheck" CssClass="label" Text='<%# Eval("IS_CONFIRM_CHECK").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete</span>" : "<span Class=label-danger style=Padding:2px>Incomplete</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckLink" style="display:none" CssClass="label" Text='<%# Eval("IS_CONFIRM_CHECK").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                             <asp:Label ID="IsExInvoiceNo" style="display:none" CssClass="label" Text='<%# Eval("EXPORT_INVOICE_NO").ToString() == "" ? "No" : "Yes" %>'  runat="server" /> 
                        </ItemTemplate>
                     </asp:TemplateField> 
                       
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" /> 
                     <asp:BoundField DataField="SUB_ITEM_NAME"  HeaderText="Sub Item" /> 
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Weight" /> 
                     <asp:BoundField DataField="PACKING_NAME"  HeaderText="Pack Name" />   
                     <asp:BoundField DataField="NUMBER_OF_PACK"  HeaderText="No. Pack "   />   
                     <asp:BoundField DataField="PACK_PER_WEIGHT"  HeaderText="Per Pack WT "   />  
                     <asp:BoundField DataField="TOTAL_WEIGHT"  HeaderText="Total Pack WT"   />                                
                      
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                    </asp:GridView> 
                 
        </div>
       </div>

  <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Change Shipment Status Complete Date Form</h3>
                <div class="box-tools"> 
                 </div>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body"> 
                   <div class="form-group">
                    <label class="col-sm-3 control-label">WB Slip No</label>
                     <div class="col-sm-2">      
                       <asp:TextBox  class="form-control input-sm pull-right" ID="TextWbSlipEx"  runat="server" ></asp:TextBox>   
                      </div> 
                  </div> 
                 <div class="form-group">
                    <label class="col-sm-3 control-label">Shipment Status Complete Date</label>
                     <div class="col-sm-3">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>   
                       <asp:TextBox  class="form-control input-sm pull-right" ID="EntryDate"  runat="server"   ></asp:TextBox>  
                    </div> 
                      </div> 
                  </div> 
                  
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-3" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" 
                          OnClick="clearTextField" CausesValidation="False" TabIndex="1"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
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
              <h3 class="box-title">Shipment Status Complete List</h3>
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
    PageSize = "12" 
    OnPageIndexChanging="GridView2_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns> 
                     <asp:BoundField DataField="WB_SLIP_NO" HeaderText="WB Slip No" />
                     <asp:TemplateField HeaderText="Container No." ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="ContainerNo"   Text='<%#Bind("CONTAINER_NO")%>'  runat="server" /> 
                            <small class="label bg-blue"> <asp:Label ID="ContainerSize" Text='<%#Bind("CONTAINER_SIZE") %>'  runat="server" /></small> 
                        </ItemTemplate>
                     </asp:TemplateField>
                    <asp:TemplateField HeaderText="Action">
                     <ItemTemplate>   
                        <asp:CheckBox ID="IsShipmentCheck" runat="server"  class="flat-red" AutoPostBack = "true" onclientclick="return confirm('Are you sure to Shipment is Complete?');" oncheckedchanged="ShipmentUpdateComplete_Click" />  
                     </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField HeaderText="Is Shipment/Inventory Status" > 
                        <ItemTemplate> 
                             <asp:Label ID="IsInvenStatus" CssClass="label" Text='<%# Eval("IS_INVENTORY_STATUS").ToString() == "Transit" ? "<img src=../image/icon/transit.png ><br><br><span Class=label-danger style=Padding:2px >------ Transit ------><span>" : "<img src=../image/icon/shipping_complete.png ><br><br><span Class=label-success style=Padding:2px> Shpinng Complete<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:BoundField DataField="IS_SHIPMENT_COMPLETE_DATE"  HeaderText="Shipment Status Complete Date"  DataFormatString="{0:dd/MM/yyyy}"  /> 
                       <asp:TemplateField  HeaderText="Action" >
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelect" class="btn btn-info btn-sm" runat="server" CommandArgument='<%#  Eval("WB_SLIP_NO")%>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField>  
                     <asp:BoundField DataField="DISPATCH_DATE"  HeaderText="Dispatch Date"  DataFormatString="{0:dd/MM/yyyy}"  />  
                     <asp:BoundField DataField="SEAL_NO"  HeaderText="Seal No." /> 
                     <asp:BoundField DataField="REL_ORDER_NO"  HeaderText="Rel Order No." /> 
                     <asp:BoundField DataField="PARTY_NAME" HeaderText="Company Name" /> 
                     <asp:BoundField DataField="ITEM_WEIGHT_WB"  HeaderText="Weight WB" DataFormatString="{0:0.0}" />   
                     
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField> 

                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                      
                      <asp:TemplateField HeaderText="Is Create Invoice" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsCmoCheck" CssClass="label" Text='<%# Eval("IS_CONFIRM_CHECK").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete</span>" : "<span Class=label-danger style=Padding:2px>Incomplete</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckLink" style="display:none" CssClass="label" Text='<%# Eval("IS_CONFIRM_CHECK").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                             <asp:Label ID="IsExInvoiceNo" style="display:none" CssClass="label" Text='<%# Eval("EXPORT_INVOICE_NO").ToString() == "" ? "No" : "Yes" %>'  runat="server" /> 
                        </ItemTemplate>
                     </asp:TemplateField> 
                       
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" /> 
                     <asp:BoundField DataField="SUB_ITEM_NAME"  HeaderText="Sub Item" /> 
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Weight" /> 
                     <asp:BoundField DataField="PACKING_NAME"  HeaderText="Pack Name" />   
                     <asp:BoundField DataField="NUMBER_OF_PACK"  HeaderText="No. Pack "   />   
                     <asp:BoundField DataField="PACK_PER_WEIGHT"  HeaderText="Per Pack WT "   />  
                     <asp:BoundField DataField="TOTAL_WEIGHT"  HeaderText="Total Pack WT"   />                                
                      
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                    </asp:GridView> 
                 
        </div>
       </div>
          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title"> Weight Slip & Container Statement (Goods In Transit/Shipment Complete - Month Wise) Parameter</h3>
            </div>
            <!-- /.box-header -->

              <div class="box-body">
            <!-- form start -->   
              <div class="form-group">
                  <label  class="col-sm-2 control-label">Select Goods In Transit/Shipment</label> 
                  <div class="col-sm-2">   
                       <asp:DropDownList ID="DropDownGoodsIn" class="form-control input-sm" runat="server"> 
                        <asp:ListItem Value="0" Text="All" />
                        <asp:ListItem Value="Transit" Text="Transit" />
                        <asp:ListItem Value="Complete" Text="Shipping Complete" />
                       </asp:DropDownList>
                   
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
              <h3 class="box-title"> Weight Slip & Container Statement (Goods In Transit/Shipment Complete - Month Wise) Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="PF_Reports/PfExpContainerMonthlyReportView.aspx?MonthYear=<%=TextMonthYear0.Text %>&GoodsInID=<%=DropDownGoodsIn.Text %>" width="1350px" height="950px" id="iframe1"
                        marginheight="0" frameborder="0" scrolling="auto" >   </iframe>  
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
    <style type="text/css"> 
    tr{ 
     border-top: 2px dotted #00c0ef !important;
    } 
 </style>
 
 
       
</div> 
</asp:Content> 