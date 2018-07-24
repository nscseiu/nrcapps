<%@ Page Title="User Form & List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="NrcUser.aspx.cs" Inherits="NRCAPPS.NRC.NrcUser" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        User Form & List
        <small>User: - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">User Settings</a></li>
        <li class="active">Users</li>
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
              <h3 class="box-title">User Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start -->
            
              <div class="box-body">
              <div class="form-group">
                  <label  class="col-sm-2 control-label">Select Employee*</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownEmployeeID" class="form-control  input-sm select2" runat="server"  AutoPostBack="True"  ontextchanged="TextEmpID_TextChanged"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownEmployeeID" Display="Dynamic" 
                          ErrorMessage="Select Employee" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                    <div class="col-sm-3"><asp:Label ID="CheckEmpID" runat="server"></asp:Label> 
                    </div>
                </div>
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">User Role*</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownUserRoleID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownUserRoleID" Display="Dynamic" 
                          ErrorMessage="Select User Role" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>
                <div class="form-group"> 
                   <label for="User_Name" class="col-sm-2 control-label">User Name*</label>
                 <div class="col-sm-2">  
                    <asp:TextBox ID="TextUserID" style="display:none" runat="server"></asp:TextBox>
                    <asp:TextBox ID="TextUserName" class="form-control input-sm" placeholder="User Name" runat="server" AutoPostBack="True" 
                        ontextchanged="TextUserName_TextChanged"></asp:TextBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                         ControlToValidate="TextUserName" ErrorMessage="Insert User Name" 
                         SetFocusOnError="True" Display="Dynamic"></asp:RequiredFieldValidator> 
                  </div> 
                    <div class="col-sm-3"><asp:Label ID="CheckUsername" runat="server"></asp:Label> 
                    </div>
                </div> 
              <div class="form-group has-feedback">
                  <label  class="col-sm-2 control-label">Password*</label> 
                  <div class="col-sm-2">  
                    <asp:TextBox ID="TextPassword"  class="form-control input-sm" placeholder="Password" runat="server"  onmousedown="this.type='text'" onmouseup="this.type='password'" onmousemove="this.type='password'" ></asp:TextBox> 
                       <span class="glyphicon glyphicon-eye-open form-control-feedback"></span>
          
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="TextPassword" Display="Dynamic" 
                          ErrorMessage="Insert password" SetFocusOnError="True"></asp:RequiredFieldValidator>
                      <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                          ControlToValidate="TextPassword" Display="Dynamic" 
                          ErrorMessage="Password must have min 6 digit and max 12 digit" 
                          ValidationExpression="^[a-zA-Z0-9'@&amp;#.\s]{6,12}$" 
                          SetFocusOnError="True"></asp:RegularExpressionValidator>
                    </div>
                </div> 
                  
                <div class="form-group  has-feedback">
                  <label  class="col-sm-2 control-label">Confirm Password*</label> 
                  <div class="col-sm-2">   
                  <asp:TextBox ID="TextPasswordConfirm"  TextMode="Password" placeholder="Confirm Password" class="form-control input-sm" runat="server"  ></asp:TextBox> 
                  <span class="glyphicon glyphicon-log-in form-control-feedback"></span>
                   <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextPasswordConfirm" Display="Dynamic" 
                          ErrorMessage="Insert Confirm password" SetFocusOnError="True"></asp:RequiredFieldValidator>
                 <asp:CompareValidator ID="CompareValidator1" runat="server"    ControlToValidate="TextPasswordConfirm"  CssClass="ValidationError"  ControlToCompare="TextPassword"
                          ErrorMessage="No Match (Password)" Display="Dynamic" 
                                        ToolTip="Password must be the same" />  
                      <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                          ControlToValidate="TextPasswordConfirm" Display="Dynamic" 
                          ErrorMessage="Password must have min 6 digit and max 12 digit" 
                          ValidationExpression="^[a-zA-Z0-9'@&amp;#.\s]{6,12}$" 
                          SetFocusOnError="True"></asp:RegularExpressionValidator>
                    </div> 
                </div> 
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Is Active Status</label> 
                  <div class="col-sm-3" style="padding-top:6px;">    
                        <label>
                            <input type="checkbox" ID="CheckIsActive" class="flat-red" checked runat="server"/>
                        </label>
                  </div>
                </div>

                 <!-- checkbox -->
               
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">    
                    <asp:LinkButton ID="BtnAdd" class="btn btn-primary" runat="server" Text="Add New" onclick="BtnAdd_Click"><span class="fa fa-plus"></span> Add New</asp:LinkButton>
                    <asp:LinkButton ID="BtnUpdate" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdate_Click"><span class="fa fa-edit"></span> Update</asp:LinkButton>
                    <asp:LinkButton ID="BtnDelete" class="btn btn-danger" runat="server" onclick="BtnDelete_Click" onclientclick="return confirm('Are you sure to delete?');" ><span class="fa fa-close"></span> Delete</asp:LinkButton>
                      
                </div>
                </div>
              </div>

              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">User List</h3>
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
    PageSize = "5" 
    OnPageIndexChanging="GridViewUser_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="USER_ID" HeaderText="User ID" />
                       <asp:TemplateField>
                            <ItemTemplate>
                             <asp:Image ID="Image1" class="profile-user-img img-responsive img-circle" style="Width:50px;border:1px solid #d2d6de !important;" runat="server" ImageUrl='<%# "HandlerProfileImage.ashx?id=" + Eval("USER_ID") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                     <asp:BoundField DataField="USER_NAME" HeaderText="User Name" />
                     <asp:BoundField DataField="EMP_ID" HeaderText="Emp. ID" />
                     <asp:BoundField DataField="EMP_NAME"  HeaderText="Emp. Name" /> 
                      <asp:TemplateField HeaderText="User Role">
                        <ItemTemplate> 
                             <asp:Label ID="UroleBGColor" CssClass="label" style='<%# "background-color:" +  Eval("UR_BG_COLOR").ToString() + "" %>'  Text='<%# Eval("USER_ROLE_NAME").ToString()%>'   runat="server" />
                          </ItemTemplate>
                     </asp:TemplateField>  
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                       
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-sm" runat="server" CommandArgument='<%# Eval("USER_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
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
 
</div>
</asp:Content> 