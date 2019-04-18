<%@ Page Title="Material Pricing (Overseas) Form & List" Language="C#" MasterPageFile="~/Site.Master" EnableEventValidation="false" AutoEventWireup="true"  CodeBehind="WpExpMaterialPricing.aspx.cs" Inherits="NRCAPPS.WP.WpExpMaterialPricing" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
    <script language="javascript" type="text/javascript">
                      /* Function to Populate the Category Dropdown*/
                    function GetItemsWp() {    
                                   var WpSlipNoExID = $("#ctl00_ContentPlaceHolder1_DropDownWpSlipNoEx").val();
                                   $("[id*=ctl00_ContentPlaceHolder1_DropDownItemID]").removeAttr("disabled");  
                                    $.ajax({
                                        type: "POST",
                                        url: "WpExpMaterialPricing.aspx/GetItemList",
                                        data: '{"WpSlipNoExID": "' + WpSlipNoExID + '"}',
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
                            total = 0; 
                            var Item_Wt =  $("#ctl00_ContentPlaceHolder1_DropDownItemID").val().split('-'); 
                            total = Number(Item_Wt[1]/1000);  
                            $("#ctl00_ContentPlaceHolder1_TextTotalQty").val(total.toFixed(3));

                            var val1 = parseFloat($("#ctl00_ContentPlaceHolder1_TextPricePerMt").val());
                            var val2 = parseFloat($("#ctl00_ContentPlaceHolder1_TextTotalQty").val());  
                            if (!isNaN(val1) && val1.length != 0 && !isNaN(val2) && val2.length != 0) {
                            var sum = val1 * val2;
                            $("#ctl00_ContentPlaceHolder1_TextTotalAmountEx").val(sum.toFixed(2));   
                            } 
                            $("#ctl00_ContentPlaceHolder1_DropDownCurrencyRateID").val(0);   
                            $("#ctl00_ContentPlaceHolder1_TextItemCurrencyAmount").val(0.00); 

                            $.ajax({
                                        type: "POST",
                                        url: "WpExpMaterialPricing.aspx/GetSalesItemList",
                                        data: '{"ItemID": "' + Item_Wt[2] + '"}',
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        success: function (r) {
                                            var ctl00_ContentPlaceHolder1_DropDownSalesItemID = $("[id*=ctl00_ContentPlaceHolder1_DropDownSalesItemID]");
                                                ctl00_ContentPlaceHolder1_DropDownSalesItemID.empty().append('<option value="0">Please Select Sales Item Name</option>');
                                            $.each(r.d, function () {
                                                ctl00_ContentPlaceHolder1_DropDownSalesItemID.append($("<option></option>").val(this['Value']).html(this['Text']));

                                            });
                                        }
                                    });
            }


         
 </script>
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Material Pricing (Overseas) Form & List
        <small>Material Pricing (Overseas): - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Waste Paper</a></li>
        <li class="active">Material Pricing (Overseas)</li>
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
              <h3 class="box-title">Material Pricing (Overseas) Form</h3><div class="box-tools">
                  </div>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
                  <div class="box-body">
                  <div class="form-group">
                  <label  class="col-sm-3 control-label">Select Weight Slip No. / Container No.</label> 
                  <div class="col-sm-6">    
                  <asp:DropDownList runat="server" ID="DropDownWpSlipNoEx" class="form-control select2" onchange="GetItemsWp()" > 
                  </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                          ControlToValidate="DropDownWpSlipNoEx" Display="Dynamic" 
                          ErrorMessage="Select Weight Slip No. / Container No."  SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Select Item</label> 
                  <div class="col-sm-6">    
                  <asp:DropDownList runat="server" ID="DropDownItemID" class="form-control" onclick="GetItemDataListWp()" > 
                  </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="DropDownItemID" Display="Dynamic" 
                          ErrorMessage="Select Item"  SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Select Sales Item Name</label> 
                  <div class="col-sm-6">    
                  <asp:DropDownList runat="server" ID="DropDownSalesItemID" class="form-control" > 
                  </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="DropDownSalesItemID" Display="Dynamic" 
                          ErrorMessage="Select Item"  SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>    
                <div class="form-group">   
                    <label class="col-sm-3 control-label">Price Per Metric Ton.</label> 
                   <div class="col-sm-2"> 
                   <div class="input-group">  
                    <asp:TextBox ID="TextPricePerMt" class="form-control input-sm"  runat="server"></asp:TextBox>  
                             <span class="input-group-addon">$USD</span>  
                      </div> 
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" 
                          ControlToValidate="TextPricePerMt" ErrorMessage="Insert Price Per Metric Ton" 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                 </div> 
                    <label class="col-sm-2 control-label">Total Quantity (Metric Ton)</label> 
                   <div class="col-sm-2">    
                    <div class="input-group">    
                    <asp:TextBox ID="TextTotalQty" class="form-control input-sm"  runat="server"></asp:TextBox>
                        <span class="input-group-addon">MT</span>  
                   </div>
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" 
                          ControlToValidate="TextTotalQty" ErrorMessage="Insert Price Per Metric Ton" 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                 </div> 
               </div>               
               <div class="form-group">   
                    <label class="col-sm-3 control-label">Total Amount</label>  
                   <div class="col-sm-2"> 
                       <div class="input-group">    
                        <asp:TextBox ID="TextTotalAmountEx" class="form-control input-sm"  runat="server"></asp:TextBox>  
                        <span class="input-group-addon">$USD</span>  
                      </div> 
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                          ControlToValidate="TextTotalAmountEx" ErrorMessage="Insert Total Amount of This Invoice No." 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                 </div> 
               </div>  
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Currency Conversion</label> 
                  <div class="col-sm-2" >   
                   <div class="input-group">  
                    <asp:DropDownList ID="DropDownCurrencyRateID"  class="form-control input-sm" runat="server" >   
                    </asp:DropDownList>  
                      <span class="input-group-addon"><i class="fa fa-fw fa-money"></i></span>  
                      </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server" 
                          ControlToValidate="DropDownCurrencyRateID" Display="Dynamic" 
                          ErrorMessage="Select Currency Rate" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator> 
                     
                  </div>   
                       <label  class="col-sm-2 control-label">Conversion Amount</label> 
                   <div class="col-sm-2"> 
                         <asp:TextBox ID="TextItemCurrencyAmount" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                     </div>
                   
                </div>   
                  <div class="form-group">
                  <label  class="col-sm-3 control-label">Is Not WB Slip/Container Edit</label> 
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
                  <div  class="col-sm-3" style="text-align:right;"> 
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
              <h3 class="box-title">Material Pricing (Overseas) List</h3>
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
                    <asp:GridView ID="GridView1" runat="server"    EnablePersistedSelection="true"    OnDataBound="gridViewFileInformation_DataBound"        
                        SelectedRowStyle-BackColor="Yellow" 
                        AllowPaging="true" 
                        AllowSorting="true"
                        PageSize = "15" 
                        OnPageIndexChanging="GridViewPage_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns> 
                     <asp:BoundField DataField="CONTAINER_NO" HeaderText="Container No." /> 
                     <asp:BoundField DataField="WB_SLIP_NO" HeaderText="WB Slip No." /> 
                     <asp:BoundField DataField="ITEM_NAME" HeaderText="Item Name" /> 
                     <asp:BoundField DataField="ITEM_SALES_DESCRIPTION" HeaderText="Sales Item Name" /> 
                     <asp:BoundField DataField="ITEM_WEIGHT" HeaderText="Weight/MT." DataFormatString="{0:0.000}" /> 
                     <asp:BoundField DataField="MAT_PRICE_PER_MT" HeaderText="Price/MT. $USD" DataFormatString="{0:0,0.00}" />                         
                     <asp:BoundField DataField="MATERIAL_AMOUNT" HeaderText="Amount $USD" DataFormatString="{0:0,0.00}" />  
                     <asp:BoundField DataField="MATERIAL_CONVERSION_AMOUNT" HeaderText="Conversion Amount SR" DataFormatString="{0:0,0.00}" />                           
                     <asp:BoundField DataField="UPDATE_DATE_PRICING"  HeaderText="Update Date"  DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:TemplateField HeaderText="Is Not WB Slip/Container Edit Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE_PRICING").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                             <asp:Label ID="IsPriceCheckLink" style="display:none"  CssClass="label" Text='<%# Eval("EXPORT_INVOICE_NO").ToString() == "" ? "Yes" : "No" %>'  runat="server" />
                        </ItemTemplate>
                     </asp:TemplateField> 
                      
                      <asp:TemplateField  HeaderText="Action" >
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelect" class="btn btn-info btn-sm" runat="server" CommandArgument='<%#  Eval("EXP_WBCON_ITEM_ID")%>' OnClick="LinkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField>  
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
    <asp:Panel ID="PanelPrint" runat="server" ></asp:Panel>  

      <style type="text/css"> 
    tr{ 
     border-top: 2px dotted #00c0ef !important;
    } 
 </style>
</div> 
</asp:Content> 