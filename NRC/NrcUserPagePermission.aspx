<%@ Page Title="User Page Permission Form & List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="NrcUserPagePermission.aspx.cs" Inherits="NRCAPPS.NRC.NrcUserPagePermission" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        User Page Permission Form & List
        <small>User Page Permission: - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">User Settings</a></li>
        <li class="active">User Page Permission</li>
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
          <!-- /.box --> 

       <div class="col-md-12"> 
             <asp:Panel  id="alert_box_right" runat="server">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <h4><i class="icon fa fa-check"></i> Alert!</h4>  
           </asp:Panel> 
          <!-- Horizontal Form -->
          <div class="box box-primary">
            <div class="box-header with-border">
              <h3 class="box-title">User Wise Pages Permission Form</h3>
              <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 60px; float:right;"> 
                 <div class="input-group-btn">
                  <asp:LinkButton ID="LinkButton1" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                </div>  
              </div>    
            </div>
            <!-- /.box-header -->
            <!-- form start -->
            
              <div class="box-body">
                    
               <div class="form-group">
                  <label  class="col-sm-2 control-label">Select User</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownUserID" class="form-control" runat="server"  AutoPostBack="True"  ontextchanged="DisplayUserPagePer" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="errorUserID" runat="server" 
                          ControlToValidate="DropDownUserID" Display="Dynamic" 
                          ErrorMessage="select User" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup1' ></asp:RequiredFieldValidator> 
                  </div>
                </div> 
                   
              </div>
              <!-- /.box-body --> 
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
        <div class="box box-danger">
            <div class="box-header with-border">
              <h3 class="box-title">User Wise Pages Permission List</h3>
              <div class="box-tools">
             
              <div class="input-group input-group-sm" style="width: 250px;">
                <asp:TextBox ID="txtSearchUser" Class="form-control" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="ButtonSearchUser" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridViewSearchUser" 
                        CausesValidation="False" />
                      <asp:LinkButton ID="LinkButton2" class="btn btn-success" runat="server" 
                          Text="Update"  onclick="BtnUpdateUserPagePer_Click" ValidationGroup='valGroup1' ClientIDMode="Static"></asp:LinkButton>   
                  </div>  
              </div>    
            </div>
            </div>


            <!-- /.box-header -->
            <div class="box-body table-responsive">
            
     <style type="text/css">
.hidden {display:none;}
</style>
              <asp:GridView ID="GridView2" runat="server"   AutoGenerateColumns="false"   CssClass="table table-sm table-hover table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="USER_ID" HeaderText="User ID"  ItemStyle-CssClass="hidden"   HeaderStyle-CssClass="hidden" />
                   
                     <asp:BoundField DataField="USER_PAGE_ID" HeaderText="Page ID" />
                     
                     <asp:BoundField DataField="PAGE_NAME" HeaderText="User Page Name" />  
                     <asp:TemplateField HeaderText="IS Page" HeaderStyle-HorizontalAlign="center"  ItemStyle-HorizontalAlign="center"> 
                        <ItemTemplate>  
                              <asp:CheckBox type="checkbox" id="IsPageActive"  class="flat-red" checked='<%# Eval("IS_PAGE_ACTIVE").ToString() == "Enable" ? true : false %>'    runat="server"/>
                         </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField HeaderText="IS Add" HeaderStyle-HorizontalAlign="center" ItemStyle-HorizontalAlign="center">
                        <ItemTemplate> 
                             <asp:CheckBox type="checkbox"  id="IsAddActive" class="flat-red" checked='<%# Eval("IS_ADD_ACTIVE").ToString() == "Enable" ? true : false %>'   runat="server"/>
                          </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField HeaderText="IS Edit" HeaderStyle-HorizontalAlign="center" ItemStyle-HorizontalAlign="center">
                        <ItemTemplate> 
                            <asp:CheckBox type="checkbox"   id="IsEditActive"  class="flat-red" checked='<%# Eval("IS_EDIT_ACTIVE").ToString() == "Enable" ? true : false %>'   runat="server"/>
                          </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField HeaderText="IS Del" ItemStyle-HorizontalAlign="center" >
                        <ItemTemplate> 
                            <asp:CheckBox type="checkbox"   id="IsDelActive"  class="flat-red" checked='<%# Eval("IS_DELETE_ACTIVE").ToString() == "Enable" ? true : false %>'   runat="server"/>
                         </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:TemplateField HeaderText="IS View" ItemStyle-HorizontalAlign="center" >
                        <ItemTemplate>  
                            <asp:CheckBox type="checkbox"   id="IsViewActive"  class="flat-red" checked='<%# Eval("IS_VIEW_ACTIVE").ToString() == "Enable" ? true : false %>'   runat="server"/>
                         </ItemTemplate>
                       </asp:TemplateField>   
                    <asp:TemplateField HeaderText="IS Report" ItemStyle-HorizontalAlign="center" >
                         <ItemTemplate>  
                            <asp:CheckBox type="checkbox"   id="IsReportActive"  class="flat-red" checked='<%# Eval("IS_REPORT_ACTIVE").ToString() == "Enable" ? true : false %>'   runat="server"/>
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
        <!--/.col (right) -->


      </div>
      <!-- /.row -->
    </section>
    <!-- /.content -->
 <style>
 th {
    text-align: center;
}
 </style>
</div>
</asp:Content> 