<%@ Page Title="Item Transfer Received Form & List - Metal Factory" Language="C#" MasterPageFile="~/Site.Master" EnableEventValidation="false" AutoEventWireup="true"  CodeBehind="MfItemTransfer.aspx.cs" Inherits="NRCAPPS.MF.MfItemTransfer" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Item Transfer Form & List
        <small>Item Transfer Received: - Add - Update - Delete - View - Report</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Metal Factory</a></li>
        <li class="active">Item Transfer</li>
      </ol>
    </section>
    
    <!-- Main content -->
    <section class="content">
      <div class="row">
        <!-- left column --> 
            
        <!--/.col (left) -->
        <!-- right column -->
        <div class="col-md-8"> 
             <asp:Panel  id="alert_box" runat="server">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <h4><i class="icon fa fa-check"></i> Alert!</h4>  
           </asp:Panel> 
          <!-- Horizontal Form -->
          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Item Transfer Received Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start -->
            
       <div class="box-body">
                  <div class="box box-solid box-default">
        <div class="box-header">
          <h3 class="box-title"><i class="fa fa-legal margin-r-5"></i> Matal Scrap - Weight Slip</h3>
        </div><!-- /.box-header -->
        <div class="box-body">
            <div class="form-group">
                  <label  class="col-sm-3 control-label">MS WB Slip No</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownMsWbSlipNo" class="form-control select2 input-sm" runat="server"  AutoPostBack="True" ontextchanged="GetMsData" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="DropDownMsWbSlipNo" Display="Dynamic" 
                          ErrorMessage="Select Metal Scrap Weight Slip" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>  
                  </div>
            <div class="form-group">
                 <label  class="col-sm-3 control-label">1st WT</label> 
                  <div class="col-sm-2">  
                   <div class="input-group"> 
                    <asp:TextBox ID="Text1stWeightMs" class="form-control input-sm"  runat="server" ></asp:TextBox> 
                    <span class="input-group-addon">KG</span>      
                    </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" 
                          ControlToValidate="Text1stWeightMs" ErrorMessage="Insert Item 1st Weight" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                  </div>
                <label  class="col-sm-1 control-label">2nd WT</label> 
                  <div class="col-sm-2">  
                   <div class="input-group"> 
                    <asp:TextBox ID="Text2ndWeightMs" class="form-control input-sm"  runat="server" ></asp:TextBox> 
                    <span class="input-group-addon">KG</span>      
                    </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" 
                          ControlToValidate="Text2ndWeightMs" ErrorMessage="Insert Item 2nd Weight" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                  </div>
                  <label  class="col-sm-1 control-label">Net WT</label> 
                  <div class="col-sm-2">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextMatWeightMs" class="form-control input-sm"  runat="server" ></asp:TextBox> 
                    <span class="input-group-addon">KG</span>      
                    </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" 
                          ControlToValidate="TextMatWeightMs" ErrorMessage="Insert Item Weight" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                  </div>
             </div>
        </div><!-- /.box-body -->
      </div>
                       
        <div class="box box-solid box-info">
        <div class="box-header">
          <h3 class="box-title"><i class="fa fa-tty margin-r-5"></i> Matal Factory - Purchase Order</h3>
        </div><!-- /.box-header -->
        <div class="box-body"> 
                <div class="form-group">   
                    <label class="col-sm-3 control-label">WB Slip No</label> 
                   <div class="col-sm-3">  
                    <asp:TextBox ID="TextTransferID" class="form-control input-sm" style="display:none"  runat="server"></asp:TextBox>    
                    <asp:TextBox ID="TextMfWbSlipNo" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextMfWbSlipNo" ErrorMessage="Insert Slip No." 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                 </div>
                  <div class="col-sm-4"><asp:Label ID="CheckSlipNo" runat="server"></asp:Label></div>  
               </div>
             <div class="form-group"> 
                  <label  class="col-sm-3 control-label">Vehicle No</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownVehicleID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" 
                          ControlToValidate="DropDownVehicleID" Display="Dynamic" 
                          ErrorMessage="Select Vehicle" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
               <div class="form-group">
                   <label  class="col-sm-3 control-label">Item</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownItemID" class="form-control input-sm" runat="server"  onclick="GetItemBinDataList()" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownItemID" Display="Dynamic" 
                          ErrorMessage="Select Item" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                  <label  class="col-sm-2 control-label">Item Bin</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownItemBinID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownItemBinID" Display="Dynamic" 
                          ErrorMessage="Select Item Bin" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
                   
                <div class="form-group">
                  <label  class="col-sm-3 control-label">1st WT</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextMat1stWeightMf" class="form-control input-sm"  runat="server" ></asp:TextBox> 
                    <span class="input-group-addon">KG</span>      
                    </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"   ControlToValidate="TextMat1stWeightMf" ErrorMessage="Insert Item Weight" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                    <label  class="col-sm-2 control-label">2nd WT</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextMat2ndWeightMf" class="form-control input-sm"  runat="server" ></asp:TextBox>    
                       <span class="input-group-addon">KG</span>      
                    </div>
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"   ControlToValidate="TextMat2ndWeightMf" ErrorMessage="Insert Item Weight" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                </div>
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Net WT</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextMatWeightMf" class="form-control input-sm"  runat="server"  ></asp:TextBox> 
                    <span class="input-group-addon">KG</span>      
                    </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server"   ControlToValidate="TextMatWeightMf" ErrorMessage="Insert Item Weight" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                    <label  class="col-sm-2 control-label">Variance</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextWeightVariance" class="form-control input-sm"  runat="server" ></asp:TextBox>    
                       <span class="input-group-addon">KG</span>      
                    </div>
                  </div>
                </div>
                 
                  <div class="form-group">
                    <label class="col-sm-3 control-label">Entry Date</label>
                     <div class="col-sm-3">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="EntryDate"  runat="server"   ></asp:TextBox> <!-- AutoPostBack="True"  ontextchanged="TextCheckDataProcess" -->
                     </div>  <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                          ControlToValidate="EntryDate" ErrorMessage="Insert Entry Date" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                      </div>  
                    <label class="col-sm-2 control-label">Remarks</label> 
                   <div class="col-sm-3">    
                    <asp:TextBox ID="TextRemarks" class="form-control input-sm"  runat="server" ></asp:TextBox>   
                    </div> 
                  </div>  
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Is Active Status</label> 
                  <div class="col-sm-4" style="padding-top:6px;">    
                        <label>
                            <input type="checkbox" ID="CheckIsActive" class="flat-red" checked 
                            runat="server" tabindex="2"/>
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
                   <div class="col-sm-7">    
                    <asp:LinkButton ID="BtnAdd" class="btn btn-primary" runat="server" Text="Add New" onclick="BtnAdd_Click" OnClientClick="return CheckIsRepeat();"><span class="fa fa-plus"></span> Add New</asp:LinkButton>
                    <asp:LinkButton ID="BtnUpdate" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdate_Click"><span class="fa fa-edit"></span> Update</asp:LinkButton>
                    <asp:LinkButton ID="BtnDelete" class="btn btn-danger" runat="server" onclick="BtnDelete_Click" onclientclick="return confirm('Are you sure to delete?');" ><span class="fa fa-close"></span> Delete</asp:LinkButton>
                      
                </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   </div>   

          </div> </div> 

      <div class="col-md-4">      
           <div class="callout callout-info">
                <h4>Pending Receving Material</h4> 
                  <asp:Label ID="BatchPending" runat="server"></asp:Label> 
              </div>
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Item Trans. Summary (Current Month-Default)</h3>
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
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" /> 
                     <asp:BoundField DataField="SLIP_NO" HeaderText="Total WS" ItemStyle-HorizontalAlign="Center" />                     
                     <asp:BoundField DataField="NET_WT_MS"  HeaderText="Net WT Metal Scarp" DataFormatString="{0:0,0.00}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="NET_WT_MF" HeaderText="Net WT Metal Factory" DataFormatString="{0:0,0.00}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="VARIANCE"  HeaderText="Variance" DataFormatString="{0:0,0.00}" ItemStyle-HorizontalAlign="Right" /> 
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
              <h3 class="box-title">Item Transfer Received List</h3>
              <div class="box-tools">
              <div class="col-sm-5">   
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
                     <asp:BoundField DataField="WB_SLIP_NO" HeaderText="WB- Slip No" />
                            <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-sm" runat="server" CommandArgument='<%# Eval("TRANSFER_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField> 
                       <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkPrintClick" class="btn btn-warning btn-sm" runat="server" CommandArgument='<%# Eval("WB_SLIP_NO") %>' OnClick="btnPrint_Click" CausesValidation="False">PO Print</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField> 
                     <asp:BoundField DataField="VEHICLE_NO" HeaderText="Vehicle No" />
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />
                     <asp:BoundField DataField="ITEM_CODE"  HeaderText="Item Code" />   
                     <asp:BoundField DataField="NET_WT_MS"  HeaderText="Net WT Metal Scarp" DataFormatString="{0:0,0.00}" />                       
                     <asp:BoundField DataField="NET_WT_MF"  HeaderText="Net WT Metal Factory" DataFormatString="{0:0,0.00}" /> 
                     <asp:BoundField DataField="VARIANCE"  HeaderText="Variance" DataFormatString="{0:0,0.00}" />   
                     <asp:BoundField DataField="ITEM_BIN_NAME"  HeaderText="Bin Name" /> 
                       
                     <asp:TemplateField HeaderText="Is Print" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                       <ItemTemplate> 
                           <asp:Label ID="IsPrintedCheck"  Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "<span Class=\"label label-success\" >Printed</span></br>" : "<span Class=\"label label-danger\">Not Printed</span>" %>'  runat="server" /> 
                           <asp:Label ID="IsPrintedDate" class="text-green" Style="font-size:11px;" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? Eval("PRINT_DATE") : "" %>'  runat="server" /> 
                            <asp:Label ID="IsPrintedCheckLink" style="display:none" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "Printed" : "Not_Printed" %>'  runat="server" /> 
                        </ItemTemplate>
                     </asp:TemplateField> 
                      
                          <asp:TemplateField HeaderText="Mat. Coor. Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsCmoCheck" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "<span Class=\"label label-success\">Complete<span>" : "<span Class=\"label label-danger\">Incomplete<span>" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckLink" style="display:none" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>  
                          <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date"  DataFormatString="{0:dd/MM/yyyy}"  />
                     <asp:BoundField DataField="REMARKS"  HeaderText="Remarks" />
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                              <asp:Label ID="IsActiveGV" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=\"label label-success\">Enable<span>" : "<span Class=\"label label-danger\">Disable<span>" %>'  runat="server" /> 
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
              <h3 class="box-title"> Item Transfer Statement (MS To MF - Month Wise) Parameter</h3>
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
              <h3 class="box-title"> Item Transfer Statement (MS To MF - Month Wise) Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="MF_Reports/MfItemTransferMonthWiseReportView.aspx?MonthYear=<%=TextMonthYear0.Text %>" width="1250px" id="iframe1"
                        marginheight="0" frameborder="0" scrolling="auto" height="950px">   </iframe>  
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
        <script language="javascript" type="text/javascript">

              function GetItemBinDataList() {   
                  
                              $("[id*=ctl00_ContentPlaceHolder1_RadioBtnVat]").removeAttr("disabled"); 
                                    var ItemId = $("#ctl00_ContentPlaceHolder1_DropDownItemID").val();
                                   
                                    $.ajax({
                                        type: "POST",
                                        url: "MfItemTransfer.aspx/GetItemDataList",
                                        data: '{"ItemId": "' + ItemId + '"}',
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        success: function (r) {
                                            
                                             var ctl00_ContentPlaceHolder1_DropDownItemBinID = $("[id*=ctl00_ContentPlaceHolder1_DropDownItemBinID]");
                                            ctl00_ContentPlaceHolder1_DropDownItemBinID.empty();
                                            $.each(r.d, function () {
                                                  ctl00_ContentPlaceHolder1_DropDownItemBinID.append($("<option></option>").val(this['Value']).html(this['Text']));

                                            });
                                            $("[id*=ctl00_ContentPlaceHolder1_DropDownItemBinID]").removeAttr("disabled"); 
                                        }
                                    });
 
                    }
                   
         
 </script>
         <asp:Panel ID="PanelPrint" runat="server" ></asp:Panel>  
</div> 
</asp:Content> 