<%@ Page Title="Quality Assurance - Finished Goods List - Metal Factory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="MfFgQualityAssurance.aspx.cs" Inherits="NRCAPPS.MF.MfFgQualityAssurance" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Quality Assurance - Finished Goods List
        <small>Quality Assurance - Finished Goods:  Update - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li> 
        <li><a href="#">Metal Factory</a></li>
        <li class="active">Quality Assurance - Finished Goods</li>
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
      
        <div class="col-md-12">    
             <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Quality Assurance - Finished Goods List</h3>
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
                        <asp:LinkButton ID="BtProductionPost_Click" class="btn bg-maroon btn-sm" runat="server" CommandArgument='<%# Eval("BATCH_NO") %>' OnClientClick="return confirm('Are you confirm Quality Assurance post this batch?')" OnClick="BtProductionPost_Click" CausesValidation="False"><i class="fa fa-fw fa-envelope-o"></i> Quality Assurance Approve Post</asp:LinkButton> 
                        <asp:Label ID="IsBatchPost" style="display:none" CssClass="label" Text='<%# Eval("PRODUCTION_ISSUE_POST_S").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                         </ItemTemplate>
                       </asp:TemplateField> 
                          <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkPrintClick" class="btn btn-warning btn-sm" runat="server" CommandArgument='<%# Eval("BATCH_NO") %>' OnClick="btnPrint_Click" CausesValidation="False"><i class="fa fa-fw fa-print"></i> Print Certificate</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField> 
                       <asp:TemplateField HeaderText="Is Print" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate> 
                             <asp:Label ID="IsPrintedCheck" Text='<%# Eval("QUALITY_APPRO_LV_IS_PRINT").ToString() == "Printed" ? "<span class=\"label label-success\" >Printed</span></br>" : "<span class=\"label label-danger\" >Not Printed</span>" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedDate" class="text-green" Style="font-size:11px;" Text='<%# Eval("QUALITY_APPRO_LV_IS_PRINT").ToString() == "Printed" ? Eval("QUALITY_APPRO_LV_PRINT_DATE") : "" %>'  runat="server" /> 
                             <asp:Label ID="IsPrintedCheckLink" style="display:none" Text='<%# Eval("QUALITY_APPRO_LV_IS_PRINT").ToString() == "Printed" ? "Printed" : "Not_Printed" %>'  runat="server" /> 
                         </ItemTemplate>
                     </asp:TemplateField> 
                     <asp:TemplateField HeaderText="Is QA" >
                        <ItemTemplate> 
                            <asp:Label ID="IsQa" Text='<%# Eval("QUALITY_APPRO_LV_ONE_S").ToString() == "Approved" ? "<span class=\"label label-success\" >Approved<span>" : "<span class=\"label label-danger\" >Pending<span>" %>'  runat="server" /> 
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
                              
                     <asp:BoundField DataField="FURNACES_NAME"  HeaderText="Furnaces Name" /> 
                     <asp:BoundField DataField="PARTY_NAME"  HeaderText="Customer Name" /> 
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Grade (Target)" />                               
                     <asp:TemplateField HeaderText="Status" >
                        <ItemTemplate> 
                            <asp:Label ID="IsActiveGV" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span class=\"label label-success\" >Enable<span>" : "<span class=\"label label-danger\" >Disable<span>" %>'  runat="server" /> 
                        </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date"  DataFormatString="{0:dd/MM/yyyy}"  />  
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
                          <h4 class="modal-title">Issue Production - Batch Details</h4>

                         </div>
                          <div class="modal-body" style="padding-top:0px;">
                            <asp:PlaceHolder ID = "PlaceHolderBatchDetails" runat="server" />   
                        <div class="box box-info">
                        <div class="box-header with-border">
                            <h3 class="box-title">Issue Production - Batch Items</h3>
                        </div>
                        <div class="box-body">   
                            <asp:GridView ID="GridView5" runat="server" AutoGenerateColumns="false"  CssClass="table table-bordered table-hover table-striped">
                               <Columns>
                                 <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />         
                                 <asp:BoundField DataField="FIRST_WT"  HeaderText="Gross WT" />    
                                 <asp:BoundField DataField="SECOND_WT"  HeaderText="Tare WT" />  
                                 <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Net WT" />  
                               </Columns>
                           </asp:GridView>  
                                      
                        </div>
                    </div>
           <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Target & Actual Quantity </h3>
            </div>   
             <div class="box-body table-responsive">
               <asp:GridView ID="GridView3" runat="server" EnablePersistedSelection="false"  
                  SelectedRowStyle-BackColor="Yellow"  AutoGenerateColumns="false" ShowHeader="true"  CssClass="table  table-sm table-bordered table-striped"  >
                     <Columns> 
                     <asp:BoundField DataField="SL_NO"  HeaderText="Sl No." />
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />
                     <asp:BoundField DataField="ITEM_WEIGHT_TARGET" HeaderText="Target WT"  />
                     <asp:BoundField DataField="ITEM_WEIGHT_ACTUAL" HeaderText="Actual WT" /> 
                     <asp:BoundField DataField="VARIANCE_WT" HeaderText="Variance WT" /> 
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
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
                <asp:Panel ID="PanelPrint" runat="server" ></asp:Panel>  
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
   
    
</asp:Content> 