<%@ Page Title="Dashboard Item & User Form & List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="NrcDashboardItemUser.aspx.cs" Inherits="NRCAPPS.NRC.NrcDashboardItemUser" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Dashboard Item & User Form & List
        <small>Dashboard Item & User: - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Dashboard Settings</a></li>
        <li class="active">Dashboard Item & User</li>
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
              <h3 class="box-title">Dashboard Item & User Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start -->
            
              <div class="box-body">
                
               <div class="form-group">
                  <label  class="col-sm-2 control-label">User </label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownUserID" class="form-control select2" runat="server"  AutoPostBack="True" 
                        ontextchanged="linkSelectClick" > 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownUserID" Display="Dynamic" 
                          ErrorMessage="select User" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>  
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Dashboard Items</label> 
                  <div class="col-sm-9">    
                  <asp:ListBox runat="server" ID="DropDownUserItemID" class="form-control select2" data-placeholder="Select Dashboard Items"  SelectionMode="multiple"> 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                          ControlToValidate="DropDownUserItemID" Display="Dynamic" 
                          ErrorMessage="select Dashboard Items" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
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
              <h3 class="box-title">Dashboard Item & User List</h3>
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
              
                    <asp:GridView ID="GridView1" runat="server"    EnablePersistedSelection="true"            
                        SelectedRowStyle-BackColor="Yellow" 
                        AllowPaging="true" 
                        AllowSorting="true"
                        PageSize = "10" 
                        OnPageIndexChanging="GridViewPage_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="USER_ID" HeaderText="User ID" />
                     <asp:BoundField DataField="EMP_ID" HeaderText="User Emp. ID" />
                     <asp:BoundField DataField="EMP_FULL_NAME" HeaderText="User Full Name" /> 
                      <asp:TemplateField HeaderText="User Role">
                        <ItemTemplate> 
                             <asp:Label ID="UroleBGColor"  style='<%# "background-color:" +  Eval("UR_BG_COLOR").ToString() + "; color:white;padding:3px 6px 3px 6px;font-size: 14px;" %>'  Text='<%# Eval("USER_ROLE_NAME").ToString()%>'   runat="server" />
                          </ItemTemplate>
                     </asp:TemplateField> 
                      <asp:TemplateField HeaderText="Dashboard Item Name">
                        <ItemTemplate> 
                             <asp:Label ID="UroleBGColor"  style='<%# "background-color:" +  Eval("DIV_BG_COLOR").ToString() + "; color:white;padding:3px 6px 3px 6px;font-size: 14px;" %>'  Text='<%# Eval("ITEM_NAME").ToString()%>'   runat="server" />
                          </ItemTemplate>
                     </asp:TemplateField> 
                     <%--
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-default btn-xs" runat="server" CommandArgument='<%#  Eval("USER_ROLE_ID")%>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField> 
                      --%>
                     
                     

                      <%-- 
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" />
                        --%>  
                       
                     
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
 
</div>
</asp:Content> 