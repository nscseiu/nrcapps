<%@ Page Title="Issue For Finished Goods Form & List - Metal Factory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="MfFinishedGoodsIssue.aspx.cs" Inherits="NRCAPPS.MF.MfFinishedGoodsIssue" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Issue For Finished Goods Form & List
        <small>Issue For Finished Goods: - Add - Update - Delete - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li> 
        <li><a href="#">Metal Factory</a></li>
        <li class="active">Issue For Finished Goods</li>
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
              <h3 class="box-title">Issue For Finished Goods Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">  
                 <div class="form-group">  
                   <label for="User_Name" class="col-sm-2 control-label">Batch Number</label>
                 <div class="col-sm-3">    
                   <asp:DropDownList ID="DropDownBatchNo" class="form-control select2 input-sm" runat="server" AutoPostBack="True" ontextchanged="GetBatchData" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                         ControlToValidate="DropDownBatchNo" ErrorMessage="Insert Batch Number" 
                         SetFocusOnError="True" Display="Dynamic"   ValidationGroup="GroupBatch" ></asp:RequiredFieldValidator> 
                  </div> 
                     <asp:TextBox ID="TextIssueFgID" class="form-control input-sm" style="display:none" runat="server" ></asp:TextBox> 
                  <label  class="col-sm-2 control-label"> Furnace</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownFurnacesID" class="form-control input-sm" runat="server"  > 
                    </asp:DropDownList>  
                         <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownFurnacesID" Display="Dynamic" 
                          ErrorMessage="Select Furnaces" InitialValue="0" SetFocusOnError="True"  ValidationGroup="GroupBatch" ></asp:RequiredFieldValidator>
                  </div>
                </div>
                   <div class="form-group">
                  <label  class="col-sm-2 control-label">Customer</label> 
                  <div class="col-sm-7">   
                    <asp:DropDownList ID="DropDownSupplierID" class="form-control select2 input-sm" runat="server" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownSupplierID" Display="Dynamic" 
                          ErrorMessage="Select Customer" InitialValue="0" SetFocusOnError="True"   ValidationGroup="GroupBatch" ></asp:RequiredFieldValidator>
                  </div>
                </div>
                  <div class="form-group">
                  <label  class="col-sm-2 control-label">Grade (Target)</label> 
                  <div class="col-sm-4">   
                    <asp:DropDownList ID="DropDownGradeID" class="form-control select2 input-sm" runat="server" > 
                    </asp:DropDownList>   
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="DropDownGradeID" Display="Dynamic" 
                          ErrorMessage="Select Grade Template" InitialValue="0" SetFocusOnError="True"  ValidationGroup="GroupBatch" ></asp:RequiredFieldValidator>
                  </div> 
                       <label  class="col-sm-2 control-label">Grade (Actual)</label> 
                   <div class="col-sm-4">   
                    <asp:DropDownList ID="DropDownActualGradeID" class="form-control select2 input-sm" runat="server" > 
                    </asp:DropDownList>   
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                          ControlToValidate="DropDownActualGradeID" Display="Dynamic" 
                          ErrorMessage="Select Actual Grade" InitialValue="0" SetFocusOnError="True"  ValidationGroup="GroupBatch" ></asp:RequiredFieldValidator>
                  </div> 
                    
                </div>
                    <div class="form-group">
                    <label class="col-sm-2 control-label">Entry Date</label>
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
                    <label class="col-sm-1 control-label">Remarks</label> 
                   <div class="col-sm-3">    
                    <asp:TextBox ID="TextRemarks" class="form-control input-sm"  runat="server" ></asp:TextBox>   
                    </div> 
                   <label  class="col-sm-2 control-label">Is Active Status</label> 
                  <div class="col-sm-1" style="padding-top:6px;">    
                        <label>
                            <input type="checkbox" ID="CheckIsActive" class="flat-red" checked runat="server"/>
                        </label>
                  </div>
                  </div>  
                 <!-- checkbox --> 
             
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" OnClick="ClearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-10">    
                  
            <asp:GridView ID="Gridview1" runat="server" AutoGenerateColumns="false"
            DataKeyNames="ISSUE_FG_ITEM_ID,ISSUE_FG_ID, BATCH_NO,ITEM_ID" CellPadding="10" CellSpacing="0"
            ShowFooter="true"
            CssClass="table table-bordered table-hover table-striped"  HeaderStyle-CssClass="header" RowStyle-CssClass="trow1" 
            AlternatingRowStyle-CssClass="trow2" 
            OnRowCommand="Gridview1_RowCommand" 
            OnRowCancelingEdit="Gridview1_RowCancelingEdit"
            OnRowEditing="Gridview1_RowEditing"
            OnRowUpdating="Gridview1_RowUpdating"
            OnRowDeleting="Gridview1_RowDeleting" > 
            <Columns>
                <asp:BoundField DataField="SL_NO"  HeaderText="Sl No." />
                <asp:TemplateField>
                    <HeaderTemplate>Item</HeaderTemplate>
                    <ItemTemplate><%#Eval("ITEM_NAME") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="DropDownItemID" class="form-control select2 input-sm"  runat="server" >
                            <asp:ListItem Text="Select Item" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfCEdit" runat="server" ErrorMessage="*"
                            ForeColor="Red" Display="Dynamic" ValidationGroup="edit" ControlToValidate="DropDownItemID" InitialValue="0">
                            Select Item
                        </asp:RequiredFieldValidator>
                     </EditItemTemplate>
                    <FooterTemplate>
                        <asp:DropDownList ID="DropDownItemID" class="form-control select2 input-sm"  runat="server" >
                            <asp:ListItem Text="Select Item" Value="0"></asp:ListItem>
                        </asp:DropDownList> 
                        <asp:RequiredFieldValidator ID="rfC" runat="server" ErrorMessage="*"
                            ForeColor="Red" Display="Dynamic" ValidationGroup="Add" ControlToValidate="DropDownItemID" InitialValue="0">Select Item</asp:RequiredFieldValidator>
                    </FooterTemplate>
                </asp:TemplateField> 
                 
                <asp:TemplateField>
                    <HeaderTemplate>Weight - KG</HeaderTemplate>
                    <ItemTemplate><%#Eval("ITEM_WEIGHT","{0:N2}") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TextItemWeight" runat="server" class="form-control input-sm" Text='<%#Bind("ITEM_WEIGHT") %>'  />
                        <asp:RequiredFieldValidator ID="rfCPEdit" runat="server" ForeColor="Red" ErrorMessage="*"
                             Display="Dynamic" ValidationGroup="edit" ControlToValidate="TextItemWeight">Insert Weight</asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="TextItemWeight" class="form-control input-sm"  runat="server"  ></asp:TextBox> 
                        <asp:RequiredFieldValidator ID="rfCP" runat="server" ErrorMessage="*"
                            ForeColor="Red" Display="Dynamic" ValidationGroup="Add" ControlToValidate="TextItemWeight">Insert Weight</asp:RequiredFieldValidator>
                    </FooterTemplate>
                      </asp:TemplateField>
                 
                <asp:TemplateField>
                      <HeaderTemplate>Action</HeaderTemplate>
                    <ItemTemplate>
                        <asp:LinkButton ID="lbEdit" Style="display:none;" class="btn btn-labeled btn-warning btn-sm" runat="server" CommandName="Edit"><span aria-hidden="true" class="fa fa-edit"></span> Edit</asp:LinkButton>
                         <asp:LinkButton ID="lbDelete" class="btn btn-danger btn-sm" runat="server" CommandName="Delete" OnClientClick="return confirm('Are you confirm to delete this item?')"><span aria-hidden="true" class="fa fa-close"></span>  Delete</asp:LinkButton>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:LinkButton ID="lbUpdate" class="btn btn-success btn-sm" runat="server" CommandName="Update" OnClientClick="return ValidateUpdate()"  ><span aria-hidden="true" class="fa fa-save"></span> Update</asp:LinkButton>
                         <asp:LinkButton ID="lbCancel" class="btn btn-default btn-sm" runat="server" CommandName="Cancel"><span aria-hidden="true" class="fa fa-close"></span> Cancel</asp:LinkButton>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:LinkButton ID="btnInsert" class="btn btn-primary btn-sm" runat="server" Text="Add" CommandName="Insert" OnClientClick="return ValidateAdd()"  ValidationGroup="Add" >
                          <span aria-hidden="true" class="fa fa-plus"></span> Add
                        </asp:LinkButton>
                    </FooterTemplate>
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
              <div class="callout callout-warning">
                <h4>Pending Batch Issue for Finished Goods</h4> 
                  <asp:Label ID="BatchPending" runat="server"></asp:Label> 
              </div>
      
          <!-- Horizontal Form -->
       

        </div>  
        <div class="col-md-12">    
             <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Issue For Finished Goods List</h3>
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
                        <asp:LinkButton ID="LinkSelectClick" class="btn btn-info btn-sm" runat="server" CommandArgument='<%# Eval("ISSUE_FG_ID") %>' OnClick="LinkSelectClick" CausesValidation="False"><i class="fa fa-fw fa-folder-open"></i> Select</asp:LinkButton> 
                        <asp:LinkButton ID="BtProductionPost_Click" class="btn bg-maroon btn-sm" runat="server" CommandArgument='<%# Eval("BATCH_NO") %>' OnClientClick="return confirm('Are you confirm post this batch?')" OnClick="BtProductionPost_Click" CausesValidation="False"><i class="fa fa-fw fa-envelope-o"></i> Finished Goods Post</asp:LinkButton> 
                        <asp:Label ID="IsBatchPost" style="display:none" CssClass="label" Text='<%# Eval("PRODUCTION_ISSUE_POST_S").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
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
                               <asp:Label ID="IsProductionPost" style="display:none" CssClass="label" Text='<%# Eval("PRODUCTION_ISSUE_S").ToString() == "Ongoing" ? "Ongoing" : Eval("PRODUCTION_ISSUE_S").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" />
                            </div>
                          </div>             
                        </ItemTemplate>
                      </asp:TemplateField>  
                     <asp:TemplateField HeaderText="Is QA" >
                        <ItemTemplate> 
                            <asp:Label ID="IsQa" Text='<%# Eval("QUALITY_APPRO_LV_ONE_S").ToString() == "Approved" ? "<span class=\"label label-success\" >Approved<span>" : "<span class=\"label label-danger\" >Pending<span>" %>'  runat="server" /> 
                            <asp:Label ID="IsQaCheck" style="display:none" CssClass="label" Text='<%# Eval("QUALITY_APPRO_LV_ONE_S").ToString() == "Approved" ? "Approved" : "Pending" %>'  runat="server" />
                          </ItemTemplate>
                     </asp:TemplateField>   
                       <asp:TemplateField HeaderText="Is FG">
                        <ItemTemplate>
                             <div class="progress active">
                            <div id="progress_bar1" class='<%# Eval("ISSUED_FG_S").ToString() == "Ongoing" ? "progress-bar progress-bar-primary progress-bar-striped" : Eval("ISSUED_FG_S").ToString() == "Complete" ? "progress-bar progress-bar-success progress-bar-striped" : "progress-bar progress-bar-danger progress-bar-striped" %> ' role="progressbar" aria-valuenow="100%" aria-valuemin="0" aria-valuemax="100%" style="width:100%"  runat="server">
                               <%# Eval("ISSUED_FG_S").ToString() == "Ongoing" ? "Ongoing" : Eval("ISSUED_FG_S").ToString() == "Complete" ? "Complete" : "Incomplete" %>
                               <asp:Label ID="IsFgPost" style="display:none" CssClass="label" Text='<%# Eval("ISSUED_FG_S").ToString() == "Ongoing" ? "Ongoing" : Eval("ISSUED_FG_S").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" />
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
                          <h4 class="modal-title">Issue For Finished Goods - Batch Details</h4>

                         </div>
                          <div class="modal-body" style="padding-top:0px;">
                            <asp:PlaceHolder ID = "PlaceHolderBatchDetails" runat="server" />   
                        <div class="box box-info">
                        <div class="box-header with-border">
                            <h3 class="box-title">Issue For Finished Goods in this Batch</h3>
                        </div>
                        <div class="box-body">   
                            <asp:GridView ID="GridView5" runat="server" AutoGenerateColumns="false"  CssClass="table table-bordered table-hover table-striped">
                               <Columns>
                                 <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />       
                                 <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Weight - KG" />                        
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
    <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title"> Issue Raw Material for Production (NRC-MF-FM-PRD-02) Report- Parameter</h3>
            </div>
            <!-- /.box-header -->

              <div class="box-body">
            <!-- form start -->   
             <div class="form-group">
                  <label  class="col-sm-2 control-label">Batch Number</label> 
                  <div class="col-sm-2">   
                    <asp:DropDownList ID="DropDownBatchNo1" class="form-control input-sm select2" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" 
                          ControlToValidate="DropDownBatchNo1" Display="Dynamic" 
                          ErrorMessage="Select Batch Number" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup1'></asp:RequiredFieldValidator>
                  </div>
                </div>
                   
                </div> 
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton4" runat="server" class="btn btn-default" OnClick="ClearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">     
                    <asp:LinkButton ID="LinkButton5" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport_Click"  ValidationGroup='valGroup1' ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
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
              <h3 class="box-title"> Issue Raw Material for Production (NRC-MF-FM-PRD-02) Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="MF_Reports/MfRmIssueReportView.aspx?BatchNo=<%=DropDownBatchNo1.SelectedItem.Text %>" width="950px" height="1250px" id="iframe3"
                        marginheight="0" frameborder="0" scrolling="auto" >   </iframe>  
                         </div>
                       </div> 
                     
                     <%} %>   
            </asp:Panel>  
             
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