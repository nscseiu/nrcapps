<%@ Page Title="Weight Slip & Container Form & List - Waste Paper" Language="C#" MasterPageFile="~/Site.Master" EnableEventValidation="false"  AutoEventWireup="true"  CodeBehind="WpExpContainer.aspx.cs" Inherits="NRCAPPS.WP.WpExpContainer" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
       <script language="javascript" type="text/javascript">
                      /* Function to Populate the Category Dropdown*/
                    function GetItemsWp() {   
                  
                              $("[id*=ctl00_ContentPlaceHolder1_RadioBtnVat]").removeAttr("disabled"); 
                                    var ItemId = $("#ctl00_ContentPlaceHolder1_DropDownCategoryID").val();
                                   $("[id*=ctl00_ContentPlaceHolder1_DropDownItemID]").removeAttr("disabled");  
                                    $.ajax({
                                        type: "POST",
                                        url: "WpPurchase.aspx/GetItemList",
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

                                    $('[id*=ctl00_ContentPlaceHolder1_BtnUpdateItem]').attr("aria-disabled", "false"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnUpdateItem]').attr("class", "btn btn-success disabled"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnDeleteItem]').attr("aria-disabled", "false"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnDeleteItem]').attr("class", "btn btn-danger disabled"); 
 
                    }
     
                  
               /* Function to get data Item rate */
               function GetItemDataListWp() { 
                   var ItemId = $("#ctl00_ContentPlaceHolder1_DropDownItemID").val(); 
                  
                    $.ajax({
                        type: "POST",
                        url: "WpExpContainer.aspx/GetItemFinalStock",
                        data: '{"ItemId": "' + ItemId + '"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (r) { 
                            $.each(r.d, function () {
                                 var ItemWeightEx = parseInt($("#ctl00_ContentPlaceHolder1_TextItemWeightEx").val()); 
                                 var final_stock = parseInt(this["Text"]);  
                              if (ItemWeightEx <= final_stock ) {  
                                    $('#ctl00_ContentPlaceHolder1_CheckItemWeight').text('Item is Available in the Inventory. ' + final_stock.toLocaleString() + ' kg');
                                    $('[Id*=ctl00_ContentPlaceHolder1_CheckItemWeight]').css("color", "green"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnAdd]').attr("aria-disabled", "true"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnAdd]').attr("class", "btn btn-primary active"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnUpdateItem]').attr("aria-disabled", "true"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnUpdateItem]').attr("class", "btn btn-success active"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnDeleteItem]').attr("aria-disabled", "true"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnDeleteItem]').attr("class", "btn btn-danger active"); 

                            } else {
                                    $('#ctl00_ContentPlaceHolder1_CheckItemWeight').text('Item is Not Available in the Inventory. Available is: ' + final_stock.toLocaleString() + ' kg');
                                    $('[Id*=ctl00_ContentPlaceHolder1_CheckItemWeight]').css("color", "red"); 

                                    $('[id*=ctl00_ContentPlaceHolder1_BtnAdd]').attr("aria-disabled", "false"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnAdd]').attr("class", "btn btn-primary disabled"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnUpdateItem]').attr("aria-disabled", "false"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnUpdateItem]').attr("class", "btn btn-success disabled"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnDeleteItem]').attr("aria-disabled", "false"); 
                                    $('[id*=ctl00_ContentPlaceHolder1_BtnDeleteItem]').attr("class", "btn btn-danger disabled"); 
                            }

                             //   alert(final_stock);
                            });
                          

                        }
                    });
                     
                        $.ajax({
                            type: "POST",
                            url: "WpExpContainer.aspx/GetItemWsList",
                            data: '{"ItemId": "' + ItemId + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (r) {
                                var ctl00_ContentPlaceHolder1_DropDownItemWsID = $("[id*=ctl00_ContentPlaceHolder1_DropDownItemWsID]");
                                 ctl00_ContentPlaceHolder1_DropDownItemWsID.empty();
                                $.each(r.d, function () {
                                    ctl00_ContentPlaceHolder1_DropDownItemWsID.append($("<option></option>").val(this['Value']).html(this['Text']));

                                });
                            }
                        });

                   $("[id*=ctl00_ContentPlaceHolder1_TextItemWeightEx]").removeAttr("disabled");  
                   $("[id*=ctl00_ContentPlaceHolder1_TextBalesNumber]").removeAttr("disabled");  
                }
         
 </script>
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Weight Slip & Container Form & List
        <small>Weight Slip & Container: - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Waste Paper</a></li>
        <li class="active">Weight Slip & Container</li>
      </ol>
    </section>
    
    <!-- Main content -->
    <section class="content">
      <div class="row">
        <!-- left column --> 
            
        <!--/.col (left) -->
        <!-- right column -->
        <div class="col-md-6">  
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
                    <asp:TextBox ID="TextSlipWbNo" class="form-control input-sm" AutoPostBack="True"  ontextchanged="TextSlipWbNo_TextChanged" runat="server"></asp:TextBox>    
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextSlipWbNo" ErrorMessage="Insert Weight Slip No." 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                 </div>
                  <div class="col-sm-4"><asp:Label ID="CheckSlipNo" runat="server"></asp:Label></div>  
               </div>
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Driver Name</label> 
                  <div class="col-sm-4">   
                      <asp:TextBox ID="TextDriverName" class="form-control input-sm"  runat="server" ></asp:TextBox>    
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"   ControlToValidate="TextDriverName" ErrorMessage="Insert Driver Name" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div> 
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
                  <label  class="col-sm-3 control-label"> Item WB Net/ Gross WT</label> 
                  <div class="col-sm-3">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextItemWtWb" class="form-control input-sm"  runat="server" ></asp:TextBox>   
                    <span class="input-group-addon">KG</span>      
                    </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server"   ControlToValidate="TextItemWtWb" ErrorMessage="Insert Item WT  Weigh-Bridge" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                     <label  class="col-sm-3 control-label">Tare Weight</label> 
                    <div class="col-sm-3">
                        <div class="input-group"> 
                      <asp:TextBox ID="TextTareWeight" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                       <span class="input-group-addon">KG</span>      
                    </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server"   ControlToValidate="TextTareWeight" ErrorMessage="Insert Tare Weight" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div> 
                </div> 
                
                 <div class="form-group">
                    <label class="col-sm-3 control-label">Dispatch Date</label>
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
                    <label  class="col-sm-2 control-label">Is Active Status</label> 
                  <div class="col-sm-3" style="padding-top:6px;">    
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
                   <div class="col-sm-8">    
                    <asp:LinkButton ID="BtnAdd" class="btn btn-primary" runat="server" Text="Add New" onclick="BtnAdd_Click" OnClientClick="return CheckIsRepeat();"   ><span class="fa fa-plus"></span> Add New</asp:LinkButton>
                    <asp:LinkButton ID="BtnUpdate" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdate_Click"><span class="fa fa-edit"></span> Update</asp:LinkButton>
                    <asp:LinkButton ID="BtnDelete" class="btn btn-danger" runat="server" onclick="BtnDelete_Click" onclientclick="return confirm('Are you sure to delete?');" ><span class="fa fa-close"></span> Delete</asp:LinkButton>
                  </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>    
            <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Items Form</h3> 
            </div> 
              <div class="box-body"> 
                  <div class="form-group">
                  <label  class="col-sm-3 control-label">Select Weight Slip</label> 
                  <div class="col-sm-3">    
                      <asp:TextBox ID="TextExpWbConItemID" class="form-control input-sm" style="display:none"  runat="server"></asp:TextBox>   
                    <asp:DropDownList ID="DropDownSlipID" class="form-control input-sm select2" runat="server"  AutoPostBack="True"  ontextchanged="GetItemWtWb" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownSlipID" Display="Dynamic" 
                          ErrorMessage="Select Weight Slip" InitialValue="0" SetFocusOnError="True" ValidationGroup="VItemGroup" ></asp:RequiredFieldValidator>
                  </div>  
                      <div class="col-sm-6"> 
                         <div class="input-group"> 
                            <asp:TextBox ID="TextItemWeightWBEx" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                             <span class="input-group-addon">Weight Balance-KG</span>      
                            </div>
                     </div>
                </div>  
                  <div class="form-group">
                  <label  class="col-sm-3 control-label">Category</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownCategoryID" class="form-control input-sm" runat="server" onclick="GetItemsWp()" >  
                    </asp:DropDownList>   
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                          ControlToValidate="DropDownCategoryID" Display="Dynamic" 
                          ErrorMessage="Select Category" InitialValue="0" SetFocusOnError="True"  ValidationGroup="VItemGroup"></asp:RequiredFieldValidator>
                  </div> 
                    <label  class="col-sm-3 control-label">Item (Inventory)</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownItemID" class="form-control input-sm" runat="server"  onclick="GetItemDataListWp()" > 
                    </asp:DropDownList>  
                         <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="DropDownItemID" Display="Dynamic" 
                          ErrorMessage="Select Item" InitialValue="0" SetFocusOnError="True" ValidationGroup="VItemGroup"></asp:RequiredFieldValidator> 
                      <asp:Label ID="CheckItemWeight" runat="server"></asp:Label>  
                  </div>
                       
                </div>    
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Item Net Weight</label> 
                  <div class="col-sm-3">  
                    <div class="input-group"> 
                    <asp:TextBox ID="TextItemWeightEx" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                    <span class="input-group-addon">KG</span>   
                    </div> 
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server"   ControlToValidate="TextItemWeightEx" ErrorMessage="Insert Item Weight" Display="Dynamic" SetFocusOnError="True" ValidationGroup="VItemGroup"  ></asp:RequiredFieldValidator>
                     <asp:RegularExpressionValidator ID="RequiredFieldValidator25" runat="server" ControlToValidate="TextItemWeightEx" ErrorMessage="Weight must be greater than 0."  Display="Dynamic" ValidationExpression="^[1-9][0-9]*$" SetFocusOnError="true" ValidationGroup="VItemGroup" >
                    </asp:RegularExpressionValidator>
                   </div>
                    <label  class="col-sm-3 control-label">Item Weight Slip</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownItemWsID" class="form-control input-sm" runat="server"  > 
                    </asp:DropDownList>  
                         <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" 
                          ControlToValidate="DropDownItemWsID" Display="Dynamic" 
                          ErrorMessage="Select Item Weight Slip" InitialValue="0" SetFocusOnError="True" ValidationGroup="VItemGroup"></asp:RequiredFieldValidator>
                  </div>   
                </div>
                  <div class="form-group">
                  <label  class="col-sm-3 control-label">Bales Number</label> 
                  <div class="col-sm-3">  
                     <div class="input-group">
                      <asp:TextBox ID="TextBalesNumber" class="form-control input-sm"  runat="server" ></asp:TextBox>   
                        <div class="input-group-addon"> <i class="fa fa-th"></i> 
                      </div>
                     </div>  
                  </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server"   ControlToValidate="TextBalesNumber" ErrorMessage="Insert Bales Number" Display="Dynamic" SetFocusOnError="True" ValidationGroup="VItemGroup" ></asp:RequiredFieldValidator>
                 </div>
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-3" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton3" runat="server" class="btn btn-default" 
                          OnClick="clearTextField" CausesValidation="False" TabIndex="1"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-7">    
                    <asp:LinkButton ID="BtnAddItem" class="btn btn-primary" runat="server" Text="Add New" onclick="BtnAddItem_Click" OnClientClick="return CheckIsRepeat();"  ValidationGroup="VItemGroup" ><span class="fa fa-plus"></span> Add New</asp:LinkButton>
                    <asp:LinkButton ID="BtnUpdateItem" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdateItem_Click" ValidationGroup="VItemGroup" ><span class="fa fa-edit"  ></span> Update</asp:LinkButton>
                    <asp:LinkButton ID="BtnDeleteItem" class="btn btn-danger" runat="server" onclick="BtnDeleteItem_Click" onclientclick="return confirm('Are you sure to delete?');" ValidationGroup="VItemGroup" ><span class="fa fa-close"></span> Delete</asp:LinkButton>
                  </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
        </div> 
          <div class="col-md-6">        
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Weight Slip & Container Summary (Transit-Default)</h3> 
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
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item Name" /> 
                     <asp:BoundField DataField="ITEM_WEIGHT" HeaderText="Total WET" DataFormatString="{0:0,0}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="ITEM_BALES" HeaderText="Total Bales" DataFormatString="{0:0,0}" ItemStyle-HorizontalAlign="Right" /> 
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
                        <asp:LinkButton ID="linkPrintClick" class="btn btn-warning btn-sm" runat="server" CommandArgument='<%# Eval("WB_SLIP_NO") %>' OnClick="btnPrint_Click" CausesValidation="False">WS Print</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField> 
                     <asp:BoundField DataField="DISPATCH_DATE"  HeaderText="Dispatch Date"  DataFormatString="{0:dd/MM/yyyy}"  /> 
                     <asp:BoundField DataField="DRIVER_NAME" HeaderText="Driver Name" />
                    <asp:TemplateField HeaderText="Container No." ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="ContainerNo"   Text='<%#Bind("CONTAINER_NO")%>'  runat="server" /> 
                            <small class="label bg-blue"> <asp:Label ID="ContainerSize" Text='<%#Bind("CONTAINER_SIZE") %>'  runat="server" /></small> 
                        </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:BoundField DataField="ITEM_WEIGHT_WB"  HeaderText="WB Gross WT" DataFormatString="{0:0}" />  
                     <asp:BoundField DataField="TARE_WEIGHT"  HeaderText="Tare WT" DataFormatString="{0:0}"  />   
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField> 

                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:TemplateField HeaderText="Is Print" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="IsPrintedCheck" CssClass="label" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "<span Class=label-success style=Padding:2px >Printed</span> </br></br>" : "<span Class=label-danger style=Padding:2px>Not Printed</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedDate" class="text-green" Style="font-size:11px;" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? Eval("PRINT_DATE") : "" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedCheckLink" style="display:none" Text='<%# Eval("IS_PRINT").ToString() == "Printed" ? "Printed" : "Not_Printed" %>'  runat="server" /> 
                           
                        </ItemTemplate>
                     </asp:TemplateField> 
                      <asp:TemplateField HeaderText="Is Create Invoice" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsCmoCheck" CssClass="label" Text='<%# Eval("IS_CONFIRM_CHECK").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete</span>" : "<span Class=label-danger style=Padding:2px>Incomplete</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckLink" style="display:none" CssClass="label" Text='<%# Eval("IS_CONFIRM_CHECK").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                             <asp:Label ID="IsExInvoiceNo" style="display:none" CssClass="label" Text='<%# Eval("EXPORT_INVOICE_NO").ToString() == "" ? "No" : "Yes" %>'  runat="server" /> 

                        </ItemTemplate>
                     </asp:TemplateField> 
                      <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-sm" runat="server" CommandArgument='<%# Eval("WB_SLIP_NO") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                        <asp:Label ID="IsExConEdit" style="display:none" CssClass="label" Text='<%# Eval("IS_EDIT").ToString() == "Enable" ? "Enable" : "Disable" %>'  runat="server" />
                      </ItemTemplate>
                       </asp:TemplateField> 
                        <asp:TemplateField HeaderText="Is Shipment / Inventory Status" > 
                        <ItemTemplate> 
                             <asp:Label ID="IsInvenStatus" CssClass="label" Text='<%# Eval("IS_INVENTORY_STATUS").ToString() == "Transit" ? "<img src=../image/icon/transit.png ><br><br><span Class=label-danger style=Padding:2px >------ Transit ------><span>" : "<img src=../image/icon/shipping_complete.png ><br><br><span Class=label-success style=Padding:2px> Shpinng Complete<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>  
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectItemClick" class="btn btn-info btn-sm" runat="server" CommandArgument='<%# Eval("EXP_WBCON_ITEM_ID") %>' OnClick="linkSelectItemClick" CausesValidation="False">Select</asp:LinkButton> 
                        <asp:Label ID="IsEditItemCheck" style="display:none" CssClass="label" Text='<%# Eval("IS_EDIT").ToString() == "Disable" ? "Enable" : "Disable" %>'  runat="server" />
                        <asp:Label ID="IsEditItemPriceCheck" style="display:none" CssClass="label" Text='<%# Eval("IS_ACTIVE_PRICING").ToString() == "Enable" ? "Enable" : "Disable" %>'  runat="server" />                           
                        </ItemTemplate>
                       </asp:TemplateField> 
                    <asp:TemplateField HeaderText="Item" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="ItemWs"   Text='<%#Bind("ITEM_WS_DESCRIPTION")%>'  runat="server" /> 
                            <small class="label bg-purple"> <asp:Label ID="ItemName" Text='<%#Bind("ITEM_NAME") %>'  runat="server" /></small> 
                        </ItemTemplate>
                     </asp:TemplateField>    
                         
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Weight" />  
                     <asp:BoundField DataField="ITEM_BALES"  HeaderText="Bales" />                           
                   
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
                <div class="form-group">
                  <label  class="col-sm-2 control-label"> Report Is</label> 
                  <div class="col-sm-4"> 
                    <asp:RadioButtonList ID="radMonth" runat="server" RepeatDirection="Horizontal">
                         <asp:ListItem Value="CurrentMonth" Selected="True"> CurrentMonth &nbsp;</asp:ListItem>
                         <asp:ListItem Value="TillMonth"> Till Month</asp:ListItem> 
                    </asp:RadioButtonList>     
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
                           <iframe src="WP_Reports/WpExpContainerMonthlyReportView.aspx?MonthYear=<%=TextMonthYear0.Text %>&GoodsInID=<%=DropDownGoodsIn.Text %>&IsReport=<%=radMonth.SelectedValue %>" width="950px" height="1250px" id="iframe1"
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