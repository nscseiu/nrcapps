<%@ Page Title="Raw Material Check For Batch Form & List - Metal Factory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="MfBatchRmCheckItem.aspx.cs" Inherits="NRCAPPS.MF.MfBatchRmCheckItem" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Raw Material Check For Batch Form & List
        <small>Raw Material Check For Batch: - Add - Update - Delete - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li> 
        <li><a href="#">Metal Factory</a></li>
        <li class="active">Raw Material Check For Batch</li>
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
              <h3 class="box-title">Raw Material Check For Batch Form</h3> 
                 <div class="box-tools">
                      
                  </div>  
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body"> 
                <div class="form-group">
                  <asp:TextBox ID="TextBatchID" style="display:none" runat="server"></asp:TextBox>
                  <asp:TextBox ID="TextBatchSequence" style="display:none" runat="server"></asp:TextBox>
                  <label  class="col-sm-3 control-label">Selcet Furnace</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownFurnacesID" class="form-control input-sm" runat="server" onclick="GetBatchNo(this)" > 
                    </asp:DropDownList>  
                         <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownFurnacesID" Display="Dynamic" 
                          ErrorMessage="Select Furnaces" InitialValue="0" SetFocusOnError="True"  ValidationGroup="GroupBatch" ></asp:RequiredFieldValidator>
                  </div>
                </div>
                 <div class="form-group">  
                   <label for="User_Name" class="col-sm-3 control-label">Batch Number</label>
                 <div class="col-sm-3">   
                    <asp:TextBox ID="TextBatchNumber" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                         ControlToValidate="TextBatchNumber" ErrorMessage="Insert Batch Number" 
                         SetFocusOnError="True" Display="Dynamic"   ValidationGroup="GroupBatch" ></asp:RequiredFieldValidator> 
                  </div>  
                </div>
                   <div class="form-group">
                  <label  class="col-sm-3 control-label">Target Batch for Customer</label> 
                  <div class="col-sm-6">   
                    <asp:DropDownList ID="DropDownSupplierID" class="form-control select2 input-sm" runat="server" AutoPostBack="True" ontextchanged="GetGradeList"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownSupplierID" Display="Dynamic" 
                          ErrorMessage="Select Customer" InitialValue="0" SetFocusOnError="True"   ValidationGroup="GroupBatch" ></asp:RequiredFieldValidator>
                  </div>
                </div>
                  <div class="form-group">
                  <label  class="col-sm-3 control-label">Grade (Target)</label> 
                  <div class="col-sm-4">   
                    <asp:DropDownList ID="DropDownGradeID" class="form-control select2 input-sm" runat="server"  AutoPostBack="True" ontextchanged="GetGradeTempleteDetails"> 
                    </asp:DropDownList>  
                        
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="DropDownGradeID" Display="Dynamic" 
                          ErrorMessage="Select Grade Template" InitialValue="0" SetFocusOnError="True"  ValidationGroup="GroupBatch" ></asp:RequiredFieldValidator>
                  </div> 
                     <div class="col-sm-1">    
                               <asp:LinkButton type="button" id="BtnGradeDetails" runat="server" class="btn btn-info btn-sm" data-toggle="modal" data-target="#myModal">
                                 <i class="fa fa-fw fa-eye"></i> View
                                </asp:LinkButton>
                                <!-- Modal -->
                                <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                  <div class="modal-dialog">
                                    <div class="modal-content">
                                      <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                                        <h4 class="modal-title" id="myModalLabel">Grade Template Details</h4>
                                      </div>
                                      <div class="modal-body" style="padding-top:0px;">
                                         <asp:PlaceHolder ID = "PlaceHolderGradeDetails" runat="server" />  
                                        <div class="box box-info">
                                        <div class="box-header with-border">
                                          <h3 class="box-title">Grade Items</h3>
                                        </div>
                                       <div class="box-body">  
                                         <asp:GridView ID="GridView3" runat="server"    EnablePersistedSelection="true"          
                                            SelectedRowStyle-BackColor="Yellow"   AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                                         <Columns>     
                                         <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item"   />  
                                         <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Item Weight"   />    
                                         </Columns>
                                            <PagerStyle CssClass="pagination-ys" />
                                            <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                                        </asp:GridView> 
                                      
                                      </div></div> 
                                      <div class="modal-footer"> 
                                        <button type="button" class="btn btn-danger" data-dismiss="modal"><i class="fa fa-fw fa-close"></i> Close</button>
                                     </div>
                                      </div> 
                                       
                                    </div>
                                  </div>
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
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="EntryDate"  runat="server"   ></asp:TextBox>
                     </div>  <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                          ControlToValidate="EntryDate" ErrorMessage="Insert Entry Date" 
                          Display="Dynamic" SetFocusOnError="True"   ValidationGroup="GroupBatch" ></asp:RequiredFieldValidator>
                      </div>  
                    <label class="col-sm-2 control-label">Remarks</label> 
                   <div class="col-sm-3">    
                    <asp:TextBox ID="TextRemarks" class="form-control input-sm"  runat="server" ></asp:TextBox>   
                    </div> 
                  </div> 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Is Active Status</label> 
                  <div class="col-sm-3" style="padding-top:6px;">    
                        <label>
                            <input type="checkbox" ID="CheckIsActive" class="flat-red" checked runat="server"/>
                        </label>
                  </div>

                     <asp:LinkButton ID="BtnBatchItemCheck" class="btn btn-warning" ValidationGroup="GroupBatch" runat="server" OnClick="RmItemCheckBatch" OnClientClick="return confirm('Are you confirm Item availability check for this Batch?')"><span class="fa fa-refresh"></span> Item Availability Check for Batch </asp:LinkButton>
                     <asp:LinkButton ID="BtnBatchItemCheckConfirm" class="btn bg-olive" ValidationGroup="GroupBatch" runat="server" OnClick="BtnBatchItemCheckPost_Click" OnClientClick="return confirm('Are you confirm availability check confirm for this Batch?')"><span class="fa fa-mail-forward"></span> Availability Check Confirm for Batch </asp:LinkButton>
                </div> 
                 <!-- checkbox --> 
             
               <div class="form-group">
                  <div  class="col-sm-3" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" OnClick="ClearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-8">    
                   <asp:Label ID="CheckItemSearch" runat="server"></asp:Label> </BR></BR>
            <asp:GridView ID="Gridview1" runat="server" AutoGenerateColumns="false"
            DataKeyNames="TARGET_ITEM_ID,BATCH_NO,ITEM_ID" CellPadding="10" CellSpacing="0"
            ShowFooter="false"
            CssClass="table table-bordered table-hover table-striped"  HeaderStyle-CssClass="header" RowStyle-CssClass="trow1" 
            AlternatingRowStyle-CssClass="trow2"   > 
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>Item</HeaderTemplate>
                    <ItemTemplate><%#Eval("ITEM_NAME") %></ItemTemplate>  
                </asp:TemplateField> 
                <asp:TemplateField>
                    <HeaderTemplate>Weight - KG</HeaderTemplate>
                    <ItemTemplate><%#Eval("ITEM_WEIGHT_CWT","{0:N2}") %></ItemTemplate>  
                </asp:TemplateField> 
            </Columns>
           </asp:GridView> 
               </div>     
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box --> 
        </div>   
          </div>
      <div class="col-md-4">  
          <!-- Horizontal Form -->
           <div class="callout callout-success">
                <h4>Pending Batch for Inventory Check</h4> 
                  <asp:Label ID="BatchInventoryPending" runat="server"></asp:Label> 
              </div>
           <div class="callout callout-info">
                <h4>Pending Batch Issue for Production</h4> 
                  <asp:Label ID="BatchPending" runat="server"></asp:Label> 
              </div>
          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Today Batch </h3>
            </div>   
             <div class="box-body table-responsive">
               <asp:GridView ID="GridView4" runat="server" EnablePersistedSelection="false"  
                  SelectedRowStyle-BackColor="Yellow"  AutoGenerateColumns="false" ShowHeader="true"  CssClass="table  table-sm table-bordered table-striped"  >
                     <Columns>
                     <asp:BoundField DataField="BATCH_NO"  HeaderText="Batch No" />
                     <asp:BoundField DataField="FURNACES_NAME" HeaderText="Furnace Name"  />
                     <asp:BoundField DataField="ITEM_NAME" HeaderText="Grade (Target)" /> 
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
              <h3 class="box-title">Raw Material Check For Batch List</h3>
              <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchUserRole" Class="form-control input-sm" runat="server" />
                 <div class="input-group-btn">
                    <asp:LinkButton ID="ButtonSearch" Class="btn btn-info"   
                         runat="server" OnClick="GridViewSearchUser" 
                        CausesValidation="False" ><span class="glyphicon glyphicon-search"></span> Search</asp:LinkButton>
                  </div>  
              </div>    
            </div>
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView2" runat="server" EnablePersistedSelection="true"  DataKeyNames="BATCH_NO" 
    OnDataBound="gridViewFileInformation_DataBound"           
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "6" 
    OnPageIndexChanging="GridViewUser_PageIndexChanging" AutoGenerateColumns="false"   CssClass="table table-bordered table-hover table-striped" >
                     <Columns> 
                      <asp:TemplateField HeaderText="Batch No">
                         <ItemTemplate>
                               <asp:LinkButton ID="btnDetails" runat="server"  CommandName="ViewDetails" CommandArgument='<%# Eval("BATCH_NO")  %>' OnCommand="DisplayBatchDetalis" ><%# Eval("BATCH_NO")  %></asp:LinkButton>
                            </ItemTemplate>
                     </asp:TemplateField>  
                       <asp:TemplateField HeaderText="Action">
                       <ItemTemplate>
                        <asp:LinkButton ID="LinkSelectClick" class="btn btn-info btn-sm" runat="server" CommandArgument='<%# Eval("BATCH_ID") %>' OnClick="LinkSelectClick" CausesValidation="False"><i class="fa fa-fw fa-folder-open"></i> Select</asp:LinkButton> 
                         <asp:Label ID="IsBatchPost" style="display:none" CssClass="label" Text='<%# Eval("INVEN_AVAIL_CHECK_S").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                         </ItemTemplate>
                       </asp:TemplateField> 
                     <asp:TemplateField HeaderText="Inventory Check" >
                        <ItemTemplate> 
                            <asp:Label ID="IsInvenCheck" Text='<%# Eval("INVEN_AVAIL_CHECK_S").ToString() == "Complete" ? "<span class=\"label label-success\" >Complete<span>" : "<span class=\"label label-danger\" >Incomplete<span>" %>'  runat="server" /> 
                        </ItemTemplate>
                     </asp:TemplateField>    
                      <asp:TemplateField HeaderText="Is Production">
                        <ItemTemplate>
                             <div class="progress active">
                            <div id="progress_bar" class='<%# Eval("PRODUCTION_ISSUE_S").ToString() == "Ongoing" ? "progress-bar progress-bar-primary progress-bar-striped" : Eval("PRODUCTION_ISSUE_S").ToString() == "Complete" ? "progress-bar progress-bar-success progress-bar-striped" : "progress-bar progress-bar-danger progress-bar-striped" %> ' role="progressbar" aria-valuenow="100%" aria-valuemin="0" aria-valuemax="100%" style="width:100%"  runat="server">
                               <%# Eval("PRODUCTION_ISSUE_S").ToString() == "Ongoing" ? "Ongoing" : Eval("PRODUCTION_ISSUE_S").ToString() == "Complete" ? "Complete" : "Incomplete" %>
                                
                             </div>
                          </div>             
                        </ItemTemplate>
                      </asp:TemplateField>    
                     <asp:BoundField DataField="FURNACES_NAME"  HeaderText="Furnaces Name" /> 
                     <asp:BoundField DataField="PARTY_NAME"  HeaderText="Customer Name" /> 
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Grade (Target)" />                               
                     <asp:TemplateField HeaderText="Status" >
                        <ItemTemplate> 
                            <asp:Label ID="IsActiveGV" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span class=\"label label-success\" >Enable<span>" : "<span class=\"label label-danger\" >Disable<span>" %>'  runat="server" /> 
                        </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date"  DataFormatString="{0:dd/MM/yyyy}"  />
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />   
                   
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                    </asp:GridView>  

                    
           <div class="modal fade" id="myModalDetails" role="dialog">
            <div class="modal-dialog">
                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal"> &times;</button>
                          <h4 class="modal-title"> Batch Details</h4>

                         </div>
                          <div class="modal-body" style="padding-top:0px;">
                            <asp:PlaceHolder ID = "PlaceHolderBatchDetails" runat="server" />   
                        <div class="box box-info">
                        <div class="box-header with-border">
                            <h3 class="box-title">Batch Items</h3>
                        </div>
                        <div class="box-body">   
                            <asp:GridView ID="GridView5" runat="server" AutoGenerateColumns="false"  CssClass="table table-bordered table-hover table-striped">
                               <Columns>
                                 <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />    
                                 <asp:BoundField DataField="ITEM_WEIGHT_CWT"  HeaderText="Item Weight" />   
                               </Columns>
                           </asp:GridView>  
                                      
                        </div>
                    </div>
                    <div class="modal-footer">
                       <button type="button" class="btn btn-danger" data-dismiss="modal"><i class="fa fa-fw fa-close"></i> Close</button>
                    </div>
                </div>
            </div>

        </div>
       </div> 
    </div> 
          <!-- /.box --> 
        <!--/.col (right) -->
      </div>
      <!-- /.row -->
    </section>
    <!-- /.content -->
 
</div> 
    <script type="text/javascript">
        function showPopup() {
            $('#myModalDetails').modal('show');
        }
    </script> 
  <script language="javascript" type="text/javascript">
        /* Function to Populate the Batch Sequence number */
    function GetBatchNo() {   
        // parseFloat($("#ctl00_ContentPlaceHolder1_DropDownVatID option:selected").text());
        var FurnacesID = $("#ctl00_ContentPlaceHolder1_DropDownFurnacesID option:selected").text().split('-');  
        // var Item_Wt =  $("#ctl00_ContentPlaceHolder1_DropDownItemID").val().split('-'); 
        var FurnacesName = FurnacesID[1].replace(/\s/g,'');  
        var BatchSequence = $("#ctl00_ContentPlaceHolder1_TextBatchSequence").val(); 
        var CurrentYear = new Date().getFullYear();
        var BatchSeqTemp = parseInt(BatchSequence, 10)+1;
        var BatchNo = FurnacesName + "-" + BatchSeqTemp + "-" + CurrentYear;       
            $("#ctl00_ContentPlaceHolder1_TextBatchNumber").val(BatchNo);                   
 
      }

       function ValidateAdd() {
            var isValid = false;
            isValid = Page_ClientValidate('Add');
            if (isValid) {
                isValid = Page_ClientValidate('GroupBatch');
            } 
            return isValid;
      }

       function ValidateUpdate() {
            var isValid = false;
            isValid = Page_ClientValidate('edit');
            if (isValid) {
                isValid = Page_ClientValidate('GroupGrade');
            } 
            return isValid;
        }
           
 </script>
    
</asp:Content> 