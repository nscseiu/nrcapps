<%@ Page Title="Weight Slip & Container Form & List - Metal Factory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="MfExpContainer.aspx.cs" Inherits="NRCAPPS.MF.MfExpContainer" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Weight Slip & Container Form & List
        <small>Weight Slip & Container: - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Metal Factory</a></li>
        <li class="active">Weight Slip & Container</li>
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
              <h3 class="box-title">Weight Slip & Container Form</h3><div class="box-tools">
                       <asp:LinkButton ID="btnPrint" class="btn btn-warning"  runat="server" OnClick="btnPrint_Click" ><span class="fa fa-print"></span> Weight Slip Print</asp:LinkButton>
                 </div>
            </div>
                 
            <!-- /.box-header -->
            <!-- form start -->
            
              <div class="box-body"> 
                   
                <div class="form-group">   
                    <label class="col-sm-3 control-label">Weight Slip No</label> 
                   <div class="col-sm-3">  
                    <asp:TextBox ID="TextExWbConID" class="form-control input-sm" style="display:none"  runat="server"></asp:TextBox>    
                    <asp:TextBox ID="TextSlipNo" class="form-control input-sm"  runat="server" AutoPostBack="True"  ontextchanged="TextSlipNo_TextChanged"></asp:TextBox>  
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextSlipNo" ErrorMessage="Insert Weight Slip No." 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                 </div>
                  <div class="col-sm-5"><asp:Label ID="CheckSlipNo" runat="server"></asp:Label></div>  
               </div>
               <div class="form-group">
                  <label  class="col-sm-3 control-label"> Container No.</label> 
                  <div class="col-sm-3">   
                    <asp:TextBox ID="TextContainerNo" class="form-control input-sm"  runat="server" ></asp:TextBox>        
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server"   ControlToValidate="TextContainerNo" ErrorMessage="Insert Container Number" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                    <label  class="col-sm-3 control-label">Size</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownContainerSizeID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>   
                  </div>
                </div> 
               <div class="form-group">
                  <label  class="col-sm-3 control-label">Customer Name</label> 
                  <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownSupplierID" class="form-control select2 input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownSupplierID" Display="Dynamic" 
                          ErrorMessage="Select Customer" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                     <label  class="col-sm-2 control-label"> Contract No.</label> 
                  <div class="col-sm-2">   
                    <asp:TextBox ID="TextContractNo" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server"   ControlToValidate="TextContractNo" ErrorMessage="Insert Contract No." Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div> 
                </div> 
               <div class="form-group"> 
                  <label  class="col-sm-3 control-label"> Seal No.</label> 
                  <div class="col-sm-2">   
                    <asp:TextBox ID="TextSealNo" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"   ControlToValidate="TextSealNo" ErrorMessage="Insert Seal Number" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div> 
                   <label  class="col-sm-1 control-label"> Ref.</label> 
                  <div class="col-sm-3">   
                    <asp:TextBox ID="TextRelOrderNo" class="form-control input-sm"  runat="server" ></asp:TextBox>    
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator22" runat="server"   ControlToValidate="TextRelOrderNo" ErrorMessage="Insert Release Order Number" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                     <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextBundle" class="form-control input-sm"  runat="server" ></asp:TextBox>   
                    <span class="input-group-addon">Bundle</span>      
                    </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server"   ControlToValidate="TextBundle" ErrorMessage="Insert Bundle" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div> 
                </div>   
                <div class="form-group">
                  <label  class="col-sm-3 control-label"> Item WT WB (KG)</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextItemFirstWtWb" class="form-control input-sm"  runat="server" AutoPostBack="True"  ontextchanged="GetFirstWbData" ></asp:TextBox>   
                    <span class="input-group-addon">1st WT</span>      
                    </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server"   ControlToValidate="TextItemFirstWtWb" ErrorMessage="Insert 1st WT" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div> 
                 <div class="col-sm-3">  
                     <div class="input-group"> 
                    <asp:TextBox ID="TextItemSecondWtWb" class="form-control input-sm"  runat="server" AutoPostBack="True"  ontextchanged="GetSecondWbData"></asp:TextBox>   
                    <span class="input-group-addon">2nd WT</span>      
                    </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server"   ControlToValidate="TextItemSecondWtWb" ErrorMessage="Insert 2nd WT" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                 <div class="col-sm-3">  
                     <div class="input-group"> 
                    <asp:TextBox ID="TextItemWtWb" class="form-control input-sm"  runat="server" ></asp:TextBox>   
                    <span class="input-group-addon">Net WT</span>      
                    </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"   ControlToValidate="TextItemWtWb" ErrorMessage="Insert WB Net WT" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div> 
                </div> 
                 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Packing List</label>  
                  <div class="col-sm-3">  
                    <asp:DropDownList ID="DropDownPacking1" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="DropDownPacking1" Display="Dynamic" 
                          ErrorMessage="Select Packing List" InitialValue="0" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>  
                       <div class="col-sm-2"> 
                         <div class="input-group"> 
                            <asp:TextBox ID="TextWtTotalPacking1" class="form-control input-sm"  runat="server" AutoPostBack="True"  ontextchanged="GetItemWbNetData" ></asp:TextBox>  
                             <span class="input-group-addon">KG</span>    
                           </div>  
                           <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server"   ControlToValidate="TextWtTotalPacking1" ErrorMessage="Insert Packing Weight" Display="Dynamic" SetFocusOnError="True" ValidationGroup="VItemGroup" ></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-sm-4">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextItemWeightEx" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                    <span class="input-group-addon">Item Net WT</span>      
                    </div>
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server"   ControlToValidate="TextItemWeightEx" ErrorMessage="Insert Item Weight" Display="Dynamic" SetFocusOnError="True"  ></asp:RequiredFieldValidator>
                     <asp:RegularExpressionValidator ID="RequiredFieldValidator25" runat="server" ErrorMessage="Weight must be greater than 0." ControlToValidate="TextItemWeightEx" Display="Dynamic" ValidationExpression="^[1-9][0-9]*$" SetFocusOnError="true"  >
                    </asp:RegularExpressionValidator> </div>
                </div> 
              
                   <div class="form-group">
                  <label  class="col-sm-3 control-label">Item </label> 
                  <div class="col-sm-3">  
                    <asp:DropDownList ID="DropDownItemID" class="form-control input-sm" runat="server" AutoPostBack="True"  ontextchanged="GetItemNetData" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" 
                          ControlToValidate="DropDownItemID" Display="Dynamic" 
                          ErrorMessage="Select Item" InitialValue="0" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                    <div class="col-sm-6">    
                      <asp:Label ID="CheckItemWeight" runat="server"></asp:Label>  
                  </div>   
                </div>  
                 <div class="form-group">
                    <label class="col-sm-3 control-label">Dispatch Date</label>
                     <div class="col-sm-3">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="EntryDate"  runat="server"   ></asp:TextBox> <!-- AutoPostBack="True"  ontextchanged="TextCheckDataProcess" -->
                     </div>  <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                          ControlToValidate="EntryDate" ErrorMessage="Insert Dispatch Date" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                      </div> 
                         <label  class="col-sm-2 control-label">Is Active Status</label> 
                  <div class="col-sm-2" style="padding-top:6px;">    
                        <label>
                            <input type="checkbox" ID="CheckIsActive" class="flat-red" checked 
                            runat="server" tabindex="1"/>
                        </label>
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
                    <asp:LinkButton ID="BtnAdd" class="btn btn-primary" runat="server" Text="Add New" onclick="BtnAdd_Click" OnClientClick="return CheckIsRepeat();"   ><span class="fa fa-plus"></span> Add New</asp:LinkButton>
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
              <h3 class="box-title">WS / Container Summary (Transit-Default)</h3> 
                 <div class="box-tools"> 
              <div class="input-group input-group-sm" style="width: 200px;">     
                  <asp:DropDownList ID="DropDownSearchSummary" class="form-control input-sm" runat="server"> 
                        <asp:ListItem Value="Transit" Text="Transit" />
                        <asp:ListItem Value="Complete" Text="Shipping Complete" />
                    </asp:DropDownList>   
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
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" /> 
                     <asp:BoundField DataField="ITEM_WEIGHT" HeaderText="Total WT - KG" DataFormatString="{0:0,0}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="BUNDLE"  HeaderText="Bundle" ItemStyle-HorizontalAlign="Right" /> 
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
              <h3 class="box-title">Weight Slip & Container List</h3>
              <div class="box-tools">
              <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownIsInven" class="form-control input-sm" runat="server"> 
                        <asp:ListItem Value="0" Text="All" />
                        <asp:ListItem Value="Transit" Text="Transit" />
                        <asp:ListItem Value="Complete" Text="Shipping Complete" />
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
                    <asp:GridView ID="GridView1" runat="server"    EnablePersistedSelection="true"  OnDataBound="gridViewFileInformation_DataBound"        
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "12" 
    OnPageIndexChanging="GridViewEmp_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="WB_SLIP_NO" HeaderText="WB Slip No" />
                      <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-sm" runat="server" CommandArgument='<%# Eval("WB_SLIP_NO") %>' OnClick="LinkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField> 
                     <asp:BoundField DataField="DISPATCH_DATE"  HeaderText="Dispatch Date"  DataFormatString="{0:dd/MM/yyyy}"  /> 
                    <asp:TemplateField HeaderText="Container No." ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="ContainerNo"   Text='<%#Bind("CONTAINER_NO")%>'  runat="server" /> 
                            <small class="label bg-blue"> <asp:Label ID="ContainerSize" Text='<%#Bind("CONTAINER_SIZE") %>'  runat="server" /></small> 
                        </ItemTemplate>
                     </asp:TemplateField> 
                     <asp:BoundField DataField="SEAL_NO"  HeaderText="Seal No." /> 
                     <asp:BoundField DataField="REF_ORDER_NO"  HeaderText="Ref. No." />  
                     <asp:BoundField DataField="CONTRACT_NO"  HeaderText="Contract No" />   
                     <asp:BoundField DataField="BUNDLE"  HeaderText="Bundle" />  
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" /> 
                     <asp:BoundField DataField="FIRST_WT"  HeaderText="1st WT"  />  
                     <asp:BoundField DataField="SECOND_WT"  HeaderText="2nd WT"  />  
                     <asp:BoundField DataField="ITEM_WEIGHT_WB"  HeaderText="WB Net Wt"  />   
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Net Weight" /> 
                     <asp:BoundField DataField="PACKING_NAME"  HeaderText="Pack Name" />       
                     <asp:BoundField DataField="PACKING_WEIGHT"  HeaderText="Pack WT "   />    
                     <asp:BoundField DataField="PARTY_NAME" HeaderText="Customer Name" />    
                     <asp:TemplateField HeaderText="Is Shipment/Inventory Status" > 
                        <ItemTemplate> 
                             <asp:Label ID="IsInvenStatus"  Text='<%# Eval("IS_INVENTORY_STATUS").ToString() == "Transit" ? "<img src=../image/icon/transit.png ><br><br><span Class=\"label label-danger\" style=Padding:2px >------ Transit ------><span>" : "<img src=../image/icon/shipping_complete.png ><br><br><span Class=\"label label-success\" style=Padding:2px> Shpinng Complete<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=\"label label-success\" style=Padding:2px >Enable<span>" : "<span Class=\"label label-danger\">Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField> 

                
                     <asp:TemplateField HeaderText="Is Print" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="IsPrintedCheck"  Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "<span Class=\"label label-success\" style=Padding:2px >Printed</span></br>" : "<span Class=\"label label-danger\" >Not Printed</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedDate" class="text-green" Style="font-size:11px;" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? Eval("PRINT_DATE") : "" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedCheckLink" style="display:none" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "Printed" : "Not_Printed" %>'  runat="server" /> 
                           
                        </ItemTemplate>
                     </asp:TemplateField> 
                      <asp:TemplateField HeaderText="Is Create Invoice" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsCmoCheck" Text='<%# Eval("IS_CONFIRM_CHECK").ToString() == "Complete" ? "<span Class=\"label label-success\" style=Padding:2px >Complete</span>" : "<span Class=\"label label-danger\" >Incomplete</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckLink" style="display:none" CssClass="label" Text='<%# Eval("IS_CONFIRM_CHECK").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                             <asp:Label ID="IsExInvoiceNo" style="display:none" CssClass="label" Text='<%# Eval("EXPORT_INVOICE_NO").ToString() == "" ? "No" : "Yes" %>'  runat="server" /> 
                             <asp:Label ID="IsEditItemPriceCheck" style="display:none" CssClass="label" Text='<%# Eval("IS_ACTIVE_PRICING").ToString() == "Enable" ? "Enable" : "Disable" %>'  runat="server" /> 
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
    
</div> 
</asp:Content> 