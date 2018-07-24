<%@ Page Title="User Role Pages Form & List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="NrcUserRolePage.aspx.cs" Inherits="NRCAPPS.NRC.NrcUserRolePage" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        User Role Pages Form & List
        <small>User Role & Pages: - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">User Settings</a></li>
        <li class="active">User Role Pages</li>
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
              <h3 class="box-title">User Role & Pages Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start -->
            
              <div class="box-body">
                
               <div class="form-group">
                  <label  class="col-sm-2 control-label">User Role</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownUserRoleID" class="form-control" runat="server"  AutoPostBack="True" 
                        ontextchanged="linkSelectClick" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownUserRoleID" Display="Dynamic" 
                          ErrorMessage="select User Role" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>  
                <div class="form-group">
                  <label  class="col-sm-2 control-label">User Pages</label> 
                  <div class="col-sm-9">    
                  <asp:ListBox runat="server" ID="DropDownUserPageID" class="form-control select2" data-placeholder="Select Pages"  SelectionMode="multiple"> 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                          ControlToValidate="DropDownUserPageID" Display="Dynamic" 
                          ErrorMessage="select User Pages" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>  
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
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
              <h3 class="box-title">User Role & Pages List</h3>
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
            <div class="box-body">
              
                    <asp:GridView ID="GridView1" runat="server"    EnablePersistedSelection="true"            
                        SelectedRowStyle-BackColor="Yellow" 
                        AllowPaging="true" 
                        AllowSorting="true"
                        PageSize = "10" 
                        OnPageIndexChanging="GridViewPage_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="USER_ROLE_ID" HeaderText="Role ID" />
                      <asp:TemplateField HeaderText="User Role">
                        <ItemTemplate> 
                             <asp:Label ID="UroleBGColor"  style='<%# "background-color:" +  Eval("UR_BG_COLOR").ToString() + "; color:white;padding:5px;font-size: 18px;" %>'  Text='<%# Eval("USER_ROLE_NAME").ToString()%>'   runat="server" />
                          </ItemTemplate>
                     </asp:TemplateField> 
                     <%--
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-default btn-xs" runat="server" CommandArgument='<%#  Eval("USER_ROLE_ID")%>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField> 
                      --%>
                     <asp:BoundField DataField="USER_PAGE_ID" HeaderText="Page ID" />
                     <asp:BoundField DataField="PAGE_NAME" HeaderText="User Page Name" />

                      <%-- 
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" />
                        --%> 
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date"  DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                       
                     
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
   <style type="text/css"> 
    tr{ 
     border-top: 2px dotted #00c0ef !important;
    } 
 </style>
</div>
</asp:Content> 