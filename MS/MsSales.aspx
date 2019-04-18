<%@ Page Title="Sales (Local) Form & List - Metal Scrap" Language="C#" MasterPageFile="~/Site.Master" EnableEventValidation="false" AutoEventWireup="true"  CodeBehind="MsSales.aspx.cs" Inherits="NRCAPPS.MS.MsSales" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
 <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
    <script language="javascript" type="text/javascript">
                      /* Function to Populate the Category Dropdown*/
                    function GetItemsWp() {   
                                   var ItemId = $("#ctl00_ContentPlaceHolder1_DropDownCategoryID").val();
                                                $("[id*=ctl00_ContentPlaceHolder1_DropDownItemID]").removeAttr("disabled");  
                                                $("#ctl00_ContentPlaceHolder1_TextItemWeightWp ").val("");
                                    $.ajax({
                                        type: "POST",
                                        url: "MsPurchase.aspx/GetItemList",
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
                        url: "MsExpContainer.aspx/GetItemFinalStock",
                        data: '{"ItemId": "' + ItemId + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (r) { 
                            $.each(r.d, function () {
                                 var ItemWeightEx = parseInt($("#ctl00_ContentPlaceHolder1_TextItemWeightWp").val()); 
                                 var final_stock = this['Text'];  
                              if (ItemWeightEx <= final_stock ) {  
                                    $('#ctl00_ContentPlaceHolder1_CheckItemWeight').text('Item is Available in the Inventory. ' + final_stock );
                                    $('[Id*=ctl00_ContentPlaceHolder1_CheckItemWeight]').css("color", "green"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnAdd]').attr("aria-disabled", "true"); 
                                  $('[id*=ctl00_ContentPlaceHolder1_BtnAdd]').attr("class", "btn btn-primary active");

                                        var val1 = parseFloat($("#ctl00_ContentPlaceHolder1_TextItemRateWp").val());
                                        var val2 = parseFloat($("#ctl00_ContentPlaceHolder1_TextItemWeightWp").val());
                                        var val3 = parseFloat($("#ctl00_ContentPlaceHolder1_DropDownVatID option:selected").text()); 
                                        if (!isNaN(val1) && val1.length != 0 && !isNaN(val2) && val2.length != 0) {
                                            var item_amt = (val1 * val2)/1000;
                                            $("#ctl00_ContentPlaceHolder1_TextItemAmount").val(item_amt.toFixed(2));  
                                            var RadioBtnVatVal = $('#ctl00_ContentPlaceHolder1_RadioBtnVat input:checked').val();
                                            var vat_amt = 0;   
                                            vat_amt = (item_amt * val3) / 100;
                                            $("#ctl00_ContentPlaceHolder1_TextVatAmount").val(vat_amt.toFixed(2));  
                                            var total_amt =  item_amt + vat_amt;
                                            $("#ctl00_ContentPlaceHolder1_TextTotalAmount").val(total_amt.toFixed(2)); 
                                        }
                            } else {
                                    $('#ctl00_ContentPlaceHolder1_CheckItemWeight').text('Item is Not Available in the Inventory. Available is: ' + final_stock);
                                    $('[Id*=ctl00_ContentPlaceHolder1_CheckItemWeight]').css("color", "red"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnAdd]').attr("aria-disabled", "false"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnAdd]').attr("class", "btn btn-primary disabled"); 
                            }

                             //   alert(final_stock);
                            });
                           
                        }
                    });
                    
                }
      
         
 </script>
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Sales (Local) Form & List
        <small>Sales (Local): - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Metal Scrap</a></li>
        <li class="active">Sales (Local)</li>
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
              <h3 class="box-title">Sales (Local) Form</h3>
                <div class="box-tools">
                       <asp:LinkButton ID="btnPrint" class="btn btn-warning"  runat="server" OnClick="btnPrint_Click" ><span class="fa fa-print"></span> Sales (Local) Invoice Print</asp:LinkButton>
                 </div>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">    
                  <div class="form-group">   
                        <label class="col-sm-3 control-label">Invoice No.</label> 
                       <div class="col-sm-3">   
                        <asp:TextBox ID="TextInvoiceMsNo" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                            <img src="../image/loader.gif" id="loader" style="display:none" height="20px" alt="" />
                         <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                              ControlToValidate="TextInvoiceMsNo" ErrorMessage="Insert Invoice No." 
                              Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                     </div>
                      <div class="col-sm-3"><asp:Label ID="CheckInvoiceNo" runat="server"></asp:Label></div>  
                   </div> 
               <div class="form-group">
                  <label  class="col-sm-3 control-label">Customer Name</label> 
                  <div class="col-sm-8">   
                    <asp:DropDownList ID="DropDownCustomerID" class="form-control select2 input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownCustomerID" Display="Dynamic" 
                          ErrorMessage="Select Customer" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>   
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Category</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownCategoryID" class="form-control input-sm" runat="server" onclick="GetItemsWp()" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownCategoryID" Display="Dynamic" 
                          ErrorMessage="Select Category" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                  <label  class="col-sm-2 control-label">Item</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownItemID" class="form-control select2 input-sm" runat="server" >  </asp:DropDownList>  
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" 
                          ControlToValidate="DropDownItemID" Display="Dynamic" 
                          ErrorMessage="Select Item" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
                   <div class="form-group">
                  <label  class="col-sm-3 control-label"> Item WT Weigh-Bridge</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextItemWtWb" class="form-control input-sm"  runat="server" ></asp:TextBox>   
                    <span class="input-group-addon">KG</span>      
                    </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server"   ControlToValidate="TextItemWtWb" ErrorMessage="Insert Item WT  Weigh-Bridge" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>  
                       <label  class="col-sm-2 control-label">Item Weight</label> 
                  <div class="col-sm-3"> 
                  <div class="input-group"> 
                    <asp:TextBox ID="TextItemWeightWp" class="form-control input-sm"  runat="server"  onChange="GetItemDataListWp()" ></asp:TextBox>  
                    <span class="input-group-addon">KG</span>      
                    </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextItemWeightWp" ErrorMessage="Insert Material Weight" 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                    
                  </div>
                    <label  class="col-sm-3 control-label"></label> 
                  <div class="col-sm-9">    
                      <asp:Label ID="CheckItemWeight" runat="server"></asp:Label>  
                  </div>   
                </div>   
                 <div class="form-group">
                  <label  class="col-sm-3 control-label">Item Rate</label> 
                  <div class="col-sm-3">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextItemRateWp" class="form-control input-sm"  runat="server" ></asp:TextBox>   
                    <span class="input-group-addon">.00</span>      
                    </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" 
                          ControlToValidate="TextItemRateWp" ErrorMessage="insert Item Rate" 
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
                  <div class="col-sm-3">  
                   <div class="input-group"> 
                    <asp:DropDownList ID="DropDownVatID" class="form-control input-sm" runat="server" >   
                    </asp:DropDownList>  
                      <span class="input-group-addon">%</span>  
                      </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" 
                          ControlToValidate="DropDownVatID" Display="Dynamic" 
                          ErrorMessage="Select Vat percent (%)" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator> 
                  </div> 
                     <label  class="col-sm-2 control-label">Vat Amount</label> 
                  <div class="col-sm-3">   
                    <div class="input-group"> 
                         <asp:TextBox ID="TextVatAmount" class="form-control input-sm"  runat="server"></asp:TextBox>    
                         <span class="input-group-addon">SR</span>  
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
                     <div class="col-sm-3">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control input-sm pull-right" ID="EntryDate"  runat="server"   ></asp:TextBox>  <!-- AutoPostBack="True"  ontextchanged="TextCheckDataProcess" -->
                    
                    </div>     <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" 
                          ControlToValidate="EntryDate" ErrorMessage="Insert Entry Date"  Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                      </div>
                     <label  class="col-sm-2 control-label">Remarks</label> 
                          <div class="col-sm-3">    
                         <asp:TextBox ID="TextRemarks" class="form-control input-sm"  runat="server" ></asp:TextBox>     
                      </div> 
                      <div class="col-sm-4"><asp:Label ID="CheckEntryDate" runat="server"></asp:Label></div>
                    <!-- /.input group -->
                  </div>
                 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Is Active Status</label> 
                  <div class="col-sm-3" style="padding-top:6px;">    
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
        </div> 
         </div> 
            <div class="col-md-5">        
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Sales (Local) Summary (Current Month-Default)</h3>
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
    SelectedRowStyle-BackColor="Yellow"  AutoGenerateColumns="false" ShowHeader="true"  ShowFooter="true" CssClass="table  table-sm table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />
                     <asp:BoundField DataField="SALES_ID" HeaderText="Total Slip" ItemStyle-HorizontalAlign="Center" />
                     <asp:BoundField DataField="ITEM_WEIGHT" HeaderText="Item WT" DataFormatString="{0:0,0.00}" ItemStyle-HorizontalAlign="Right" />
                     <asp:BoundField DataField="ITEM_AMOUNT"  HeaderText="Amount" DataFormatString="{0:0,0.00}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="VAT_AMOUNT"  HeaderText="VAT Amt" DataFormatString="{0:0,0.00}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="TOTAL_AMOUNT"  HeaderText="Total Amt" DataFormatString="{0:0,0.00}" ItemStyle-HorizontalAlign="Right" /> 
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
              <h3 class="box-title">Sales (Local) List</h3>
              <div class="box-tools">
               <div class="col-sm-4">   
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
                       <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-sm" runat="server" CommandArgument='<%# Eval("INVOICE_NO") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField> 
                     <asp:BoundField DataField="PARTY_NAME"  HeaderText="Customer Name" /> 
                     <asp:BoundField DataField="CATEGORY_NAME"  HeaderText="Category" />   
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Weight-KG"  DataFormatString="{0:0.00}" />   
                     <asp:BoundField DataField="ITEM_RATE"  HeaderText="Rate" DataFormatString="{0:0.00}" />   
                     <asp:BoundField DataField="ITEM_AMOUNT"  HeaderText="Amount"  DataFormatString="{0:0.00}" />    
                     <asp:BoundField DataField="VAT_PERCENT"  HeaderText="Vat %" />
                     <asp:BoundField DataField="VAT_AMOUNT"  HeaderText="Vat Amount"  DataFormatString="{0:0.00}" /> 
                     <asp:BoundField DataField="TOTAL_AMOUNT"  HeaderText="Total Amt"  DataFormatString="{0:0.00}" /> 
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date" DataFormatString="{0:dd/MM/yyyy}"  />  
                   
                   <asp:TemplateField HeaderText="Is Print" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="IsPrintedCheck" CssClass="label" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "<span Class=label-success style=Padding:2px >Printed</span> </br></br>" : "<span Class=label-danger style=Padding:2px>Not Printed</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedDate" class="text-green" Style="font-size:11px;" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? Eval("PRINT_DATE") : "" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedCheckLink" style="display:none" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "Printed" : "Not_Printed" %>'  runat="server" /> 
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
              <h3 class="box-title"> Sales (Local) Summary (Monthly) Parameter</h3>
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
              <h3 class="box-title"> Sales (Local) Summary (Monthly)  Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="PF_Reports/PfSalesSummaryReportView.aspx?MonthYear=<%=TextMonthYear0.Text %>" width="950px" id="iframe1"
                        marginheight="0" frameborder="0" scrolling="auto" height="1250px">   </iframe>  
                         </div>
                       </div> 
                    
                     <%} %>   
            </asp:Panel>  

           <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title"> Sales (Local) Summary (Item Wise - Monthly) Parameter</h3>
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
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="TextMonthYear1"  runat="server" ></asp:TextBox>  
                    </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
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
                    <asp:LinkButton ID="LinkButton3" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport_Click1"  ValidationGroup='valGroup3' ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                   </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
        
            <asp:Panel ID="Panel2" runat="server">  
            <%if (IsLoad1)
                {%> 
            <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title"> Sales Summary (Item Wise - Monthly)  Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="PF_Reports/PfSalesSummaryItemWiseReportView.aspx?MonthYear=<%=TextMonthYear1.Text %>" width="950px" id="iframe2"
                        marginheight="0" frameborder="0" scrolling="auto" height="1250px">   </iframe>  
                         </div>
                       </div> 
                    
                     <%} %>   
            </asp:Panel>  

          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title"> Sales Statement (Local Sales: Customer Wise - Monthly) Parameter</h3>
            </div>
            <!-- /.box-header --> 
              <div class="box-body">
            <!-- form start --> 
                <div class="form-group">
                      <label  class="col-sm-2 control-label">Customer Name</label> 
                      <div class="col-sm-4">   
                        <asp:DropDownList ID="DropDownCustomerID1" class="form-control input-sm select2" runat="server"> 
                        </asp:DropDownList>  
                          <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                              ControlToValidate="DropDownCustomerID1" Display="Dynamic" 
                              ErrorMessage="Select Customer" InitialValue="0" SetFocusOnError="True"  ValidationGroup='valGroup4'></asp:RequiredFieldValidator>
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
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
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
                    <asp:LinkButton ID="LinkButton5" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport_Click2"  ValidationGroup='valGroup4' ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                   </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
        
            <asp:Panel ID="Panel3" runat="server">  
            <%if (IsLoad2)
                {%> 
            <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title"> Sales Statement (Local Sales: Customer Wise - Monthly) Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="PF_Reports/PfSalesSummaryCustomerWiseReportView.aspx?MonthYear=<%=TextMonthYear2.Text %>&CustomerID=<%=DropDownCustomerID1.Text %>" width="950px" id="iframe3"
                        marginheight="0" frameborder="0" scrolling="auto" height="1250px">   </iframe>  
                         </div>
                       </div> 
                    
                     <%} %>   
            </asp:Panel>  


             <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title"> Sales Statement (All Customer - Monthly) Parameter</h3>
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
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="TextMonthYear3"  runat="server" ></asp:TextBox>  
                    </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" 
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
                    <asp:LinkButton ID="LinkButton7" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport_Click3"  ValidationGroup='valGroup5' ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                   </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
               <asp:Panel ID="Panel4" runat="server">  
            <%if (IsLoad3)
                {%> 
            <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title"> Sales Statement (All Customer - Monthly) Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="WP_Reports/WpSalesSummaryCustomerReportView.aspx?MonthYear=<%=TextMonthYear3.Text %>" width="1250px" id="iframe3"
                        marginheight="0" frameborder="0" scrolling="auto" height="950px">   </iframe>  
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
 <asp:Panel ID="PanelPrint" runat="server" ></asp:Panel>  
</div>
</asp:Content> 