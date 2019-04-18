<%@ Page Title="Purchase Form & List - Metal Factory" EnableEventValidation="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="MfPurchase.aspx.cs" Inherits="NRCAPPS.MF.MfPurchase" %>
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
        <li><a href="#">Metal Factory</a></li>
        <li class="active">Purchase</li>
      </ol>
    </section>
    
    <!-- Main content -->
    <section class="content">
        <script language="javascript" type="text/javascript">
                      /* Function to Populate the Category Dropdown*/
                    function GetItemsWp() {   
                  
                              $("[id*=ctl00_ContentPlaceHolder1_RadioBtnVat]").removeAttr("disabled"); 
                                    var ItemId = $("#ctl00_ContentPlaceHolder1_DropDownCategoryID").val();
                                   $("[id*=ctl00_ContentPlaceHolder1_DropDownItemID]").removeAttr("disabled");  
                                    $.ajax({
                                        type: "POST",
                                        url: "MfPurchase.aspx/GetItemList",
                                        data: '{"ItemId": "' + ItemId + '"}',
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        success: function (r) {
                                            var ctl00_ContentPlaceHolder1_DropDownItemID = $("[id*=ctl00_ContentPlaceHolder1_DropDownItemID]");
                                                ctl00_ContentPlaceHolder1_DropDownItemID.empty().append('<option value="0">Please Select Item</option>');
                                            $.each(r.d, function () {
                                                ctl00_ContentPlaceHolder1_DropDownItemID.append($("<option></option>").val(this['Value']).html(this['Text']));

                                            });
                                        }
                                    });
 
                    }
     
                  
               /* Function to get data Item rate */
               function GetItemDataListWp() {
                   var ItemId = $("#ctl00_ContentPlaceHolder1_DropDownItemID").val(); 
                    $.ajax({
                        type: "POST",
                        url: "MfPurchase.aspx/GetItemDataList",
                        data: '{"ItemId": "' + ItemId + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (r) { 
                            $.each(r.d, function () { 
                                $('#ctl00_ContentPlaceHolder1_TextItemRateWP').append().val(this['Text']).html(this['Value']); 
                                
                                   $("[id*=ctl00_ContentPlaceHolder1_DropDownItemID] option").each(function () {
                                    if ($(this).val() == $("[id*=ctl00_ContentPlaceHolder1_DropDownItemID]").val()) {
                                        $(this).attr('selected', 'selected');
                                    }
                                });
                            });
                            $("[id*=ctl00_ContentPlaceHolder1_TextItemRateWP]").removeAttr("disabled");
                            $("[id*=ctl00_ContentPlaceHolder1_TextItemWtWbWP]").removeAttr("disabled");
                            $("[id*=ctl00_ContentPlaceHolder1_TextItemWeightWP]").removeAttr("disabled");

                             var val1 = parseFloat($("#ctl00_ContentPlaceHolder1_TextItemRateWP").val());
                             var val2 = parseFloat($("#ctl00_ContentPlaceHolder1_TextItemWeightWP").val());
                             var val3 = parseFloat($("#ctl00_ContentPlaceHolder1_DropDownVatID option:selected").text()); 
                             if (!isNaN(val1) && val1.length != 0 && !isNaN(val2) && val2.length != 0) {
                                 var sum = Math.round((val1/1000) * val2);
                                 $("#ctl00_ContentPlaceHolder1_TextItemAmountWP").val(sum.toFixed(2));
                                 var VatAmt = Math.round((sum * val3) / 100);
                                 $("#ctl00_ContentPlaceHolder1_TextItemVatAmountWP").val(VatAmt.toFixed(2)); 
                                
                             }

                        }
                    });
                }
         
 </script>
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
              <h3 class="box-title">Purchase (Local) Form</h3><div class="box-tools">
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
                    <asp:TextBox ID="TextMfSlipNo" class="form-control input-sm"  runat="server"  ></asp:TextBox>  
                       <img src="../image/loader.gif" id="loader" style="display:none" height="20px" alt="" />
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextMfSlipNo" ErrorMessage="Insert Slip No." 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator><asp:Label ID="CheckSlipNoWp" runat="server"></asp:Label>
                 </div>  
                    
               </div>
               <div class="form-group">
                  <label  class="col-sm-3 control-label">Supplier Name</label> 
                  <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownSupplierID" class="form-control select2 input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownSupplierID" Display="Dynamic" 
                          ErrorMessage="Select Supplier" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div> 
                  
                </div> 
                 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Category</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownCategoryID" class="form-control input-sm" runat="server" onclick="GetItemsWp()" >  
                    </asp:DropDownList>   
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server" 
                          ControlToValidate="DropDownCategoryID" Display="Dynamic" 
                          ErrorMessage="Select Category" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div> 
                    <label  class="col-sm-2 control-label">Item</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownItemID" class="form-control input-sm" runat="server"  onclick="GetItemDataListWp()" > 
                    </asp:DropDownList>  
                         <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownItemID" Display="Dynamic" 
                          ErrorMessage="Select Item" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>   
                  <div class="form-group">
                  <label  class="col-sm-3 control-label"> Item WT Weigh-Bridge</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextItemWtWbWP" class="form-control input-sm"  runat="server" ></asp:TextBox>   
                    <span class="input-group-addon">KG</span>      
                    </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server"   ControlToValidate="TextItemWtWbWp" ErrorMessage="Insert Item WT  Weigh-Bridge" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                   <label  class="col-sm-2 control-label">Item Weight</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextItemWeightWP" class="form-control input-sm"  runat="server" ></asp:TextBox> <!--  AutoPostBack="True"  ontextchanged="TextItemRate_Changed" -->   
                    <span class="input-group-addon">KG</span>      
                    </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server"   ControlToValidate="TextItemWeightWP" ErrorMessage="Insert Item Weight" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Item Rate</label> 
                  <div class="col-sm-3">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextItemRateWP" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                    <span class="input-group-addon">.00</span>      
                    </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" 
                          ControlToValidate="TextItemRateWP" ErrorMessage="Insert Item Rate" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                  </div>
                     <label  class="col-sm-2 control-label">Item Amount</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextItemAmountWP" class="form-control input-sm"  runat="server"></asp:TextBox>    
                       <span class="input-group-addon">SR</span>      
                    </div>
                  </div>
                </div> 
               <div class="form-group">
                  <label  class="col-sm-3 control-label">Vat Percent</label>
                   <div class="col-sm-2"> 
                       <asp:RadioButtonList ID="RadioBtnVatWp" runat="server"  RepeatDirection="Horizontal">
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
                         <asp:TextBox ID="TextItemVatAmountWP" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                     </div>
                  </div>
                </div>  
               <div class="form-group"> 
                     <label  class="col-sm-3 control-label">Total Amount</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextTotalAmountWP" class="form-control input-sm"  runat="server"></asp:TextBox>    
                       <span class="input-group-addon">SR</span>      
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
                     <asp:BoundField DataField="ITEM_WEIGHT" HeaderText="Total WET" DataFormatString="{0:0,0.00}" ItemStyle-HorizontalAlign="Right" />
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
              <h3 class="box-title">Purchase (Local) List (Current Month - Default)</h3>
              <div class="box-tools">
              <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownSearchItemID" class="form-control input-sm" runat="server"> 
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
                           <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-sm" runat="server" CommandArgument='<%# Eval("PURCHASE_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField> 
                       <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkPrintClick" class="btn btn-warning btn-sm" runat="server" CommandArgument='<%# Eval("SLIP_NO") %>' OnClick="btnPrint_Click" CausesValidation="False">PO Print</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField> 
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
                             <asp:Label ID="IsActiveGV" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=\"label label-success\">Enable<span>" : "<span Class=\"label label-danger\">Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date"  DataFormatString="{0:dd/MM/yyyy}"  />
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:TemplateField HeaderText="Is Print" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="IsPrintedCheck"  Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "<span Class=\"label label-success\">Printed</span> </br>" : "<span Class=\"label label-danger\">Not Printed</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedDate" class="text-green" Style="font-size:11px;" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? Eval("PRINT_DATE") : "" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedCheckLink" style="display:none" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "Printed" : "Not_Printed" %>'  runat="server" /> 
                           
                        </ItemTemplate>
                     </asp:TemplateField> 
                      <asp:TemplateField HeaderText="CMO Claim Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsCmoCheck" Text='<%# Eval("IS_CHECK").ToString() == "Complete" ? "<span Class=\"label label-success\">Complete</span>" : "<span Class=\"label label-danger\">Incomplete</span>" %>'  runat="server" /> 
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
              <h3 class="box-title"> Purchase Statement (Daily Collection Supplier - Month Wise) Parameter</h3>
            </div>
            <!-- /.box-header --> 
              <div class="box-body">
            <!-- form start -->   
              <div class="form-group">
                  <label  class="col-sm-2 control-label">Supplier Name</label> 
                  <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownSupplierID2" class="form-control select2 input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="DropDownSupplierID2" Display="Dynamic" 
                          ErrorMessage="Select Supplier" InitialValue="0" SetFocusOnError="True"  ValidationGroup='valGroup2'></asp:RequiredFieldValidator>
                  </div>
                </div>
                   <div class="form-group">
                    <label class="col-sm-2 control-label">Select Date </label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="EntryDate1"  runat="server" ></asp:TextBox>  
                    </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                         ControlToValidate="EntryDate1" ErrorMessage="Insert Start Date" 
                         SetFocusOnError="True" Display="Dynamic" ValidationGroup='valGroup2'></asp:RequiredFieldValidator> 
                  </div>
                        <label class="col-sm-1 control-label">To</label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="EntryDate2"  runat="server" ></asp:TextBox>  
                    </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                         ControlToValidate="EntryDate2" ErrorMessage="Insert End Date" 
                         SetFocusOnError="True" Display="Dynamic" ValidationGroup='valGroup2'></asp:RequiredFieldValidator> 
                  </div>

                  </div>
                  <div class="form-group">
                  <label  class="col-sm-2 control-label">Select Item</label> 
                  <div class="col-sm-5">   
                    <asp:ListBox ID="DropDownItemList" class="form-control select2 input-sm" SelectionMode="multiple" runat="server"> 
                    </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator23" runat="server" 
                          ControlToValidate="DropDownItemList" Display="Dynamic" 
                          ErrorMessage="Select Item" SetFocusOnError="True"  ValidationGroup='valGroup2'></asp:RequiredFieldValidator>
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
                {   string ItemIDList ="";
                    for(int i=0;i<DropDownItemList.Items.Count;++i)
                    {
                        if(DropDownItemList.Items[i].Selected==true) { 
                        ItemIDList += DropDownItemList.Items[i].Value + "-";
                        }
                    }
               %> 
            <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title"> Purchase Statement (Daily Collection Supplier - Month Wise) Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="WP_Reports/WpPurchaseSupplierWiseReportView.aspx?StartDate=<%=EntryDate1.Text %>&EndDate=<%=EntryDate2.Text %>&SupplierID=<%=DropDownSupplierID2.Text %>&DropDownItemID=<%=ItemIDList%>" width="950px" id="iframe1"
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