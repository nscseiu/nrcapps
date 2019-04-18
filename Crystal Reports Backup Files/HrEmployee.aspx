<%@ Page Title="Employee Form & List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="HrEmployee.aspx.cs" Inherits="NRCAPPS.HR.HrEmployee" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Employee Form & List
        <small>Employee: - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Employee Settings</a></li>
        <li class="active">Employees</li>
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
              <h3 class="box-title">Employee Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start -->
            
              <div class="box-body">
                <div class="form-group">   
                    <label class="col-sm-2 control-label">Employee ID</label> 
                   <div class="col-sm-1">   
                    <asp:TextBox ID="TextEmpID" class="form-control"  runat="server" AutoPostBack="True" 
                        ontextchanged="TextEmpID_TextChanged"></asp:TextBox>  
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" 
                          ControlToValidate="TextEmpID" ErrorMessage="insert Employee ID" 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                 </div>
                  <div class="col-sm-3"><asp:Label ID="CheckEmpID" runat="server"></asp:Label></div>  
               </div> 
              <div class="form-group">  
                      <label class="col-sm-2 control-label">Employee Title</label> 
                  <div class="col-sm-2">   
                    <asp:DropDownList ID="DropDownEmpTitle" class="form-control" runat="server" AutoPostBack="True"  ontextchanged="ChangeGender" >  
                       <asp:ListItem Text="Select name title" Value="0" />
                        <asp:ListItem Text="Mr" Value="Mr" />
                        <asp:ListItem Text="Ms" Value="Ms" />
                    </asp:DropDownList>   
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                          ControlToValidate="DropDownEmpTitle" Display="Dynamic" EnableTheming="True" 
                          ErrorMessage="Select name title" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div> 
                    
                </div>
                <div class="form-group">
                  <label  class="col-sm-2 control-label">First Name</label> 
                  <div class="col-sm-3">  
                    <asp:TextBox ID="TextFname" class="form-control"  runat="server"></asp:TextBox>   
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextFname" ErrorMessage="insert First name" 
                          Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>

                <div class="form-group">
                  <label  class="col-sm-2 control-label">Last Name</label> 
                  <div class="col-sm-3">   
                    <asp:TextBox ID="TextLname" class="form-control"  runat="server"></asp:TextBox> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                          ControlToValidate="TextLname" ErrorMessage="insert Last name" 
                          Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>
                   </div>
                </div>
                <div class="form-group">  
                      <label class="col-sm-2 control-label">Gender</label> 
                  <div class="col-sm-2">   
                    <asp:DropDownList ID="DropDownGender" class="form-control" runat="server" disabled="disabled">  
                       <asp:ListItem Text="Select name Gender" Value="0" />
                        <asp:ListItem Text="Male" Value="Male" />
                        <asp:ListItem Text="Female" Value="Female" />
                    </asp:DropDownList> 
                      
                 </div> 
               </div>      
               <div class="form-group">
                  <label  class="col-sm-2 control-label">Email</label> 
                  <div class="col-sm-3">   
                    <asp:TextBox ID="TextEmail" class="form-control"  runat="server"></asp:TextBox> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                          ControlToValidate="TextEmail" Display="Dynamic" 
                          ErrorMessage="insert Email adress" SetFocusOnError="True"></asp:RequiredFieldValidator>
                      <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                          ControlToValidate="TextEmail" ErrorMessage="insert correct email adress" 
                          ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                          Display="Dynamic" SetFocusOnError="True"></asp:RegularExpressionValidator>
                   </div>
                </div>
              <div class="form-group">
                  <label  class="col-sm-2 control-label">Emp. National/Iqama ID</label> 
                  <div class="col-sm-3">  
                  <asp:TextBox ID="TextNid" class="form-control" runat="server"></asp:TextBox>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="TextNid" Display="Dynamic" 
                          ErrorMessage="Insert National ID / Iqama Number" SetFocusOnError="True"></asp:RequiredFieldValidator>
                      <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                          ControlToValidate="TextNid" Display="Dynamic" 
                          ErrorMessage="National ID / Iqama Number must have 10 digit" 
                          ValidationExpression="^([0-9]{10})$" 
                          SetFocusOnError="True"></asp:RegularExpressionValidator>
                    </div>
                </div>
               <div class="form-group">
                  <label  class="col-sm-2 control-label">Department</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownDepartmentID" class="form-control" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownDepartmentID" Display="Dynamic" 
                          ErrorMessage="Select Emp Department" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">Division</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownDivisionID" class="form-control" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                          ControlToValidate="DropDownDivisionID" Display="Dynamic" 
                          ErrorMessage="Select Emp Division" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Location</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownLocationID" class="form-control" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownLocationID" Display="Dynamic" 
                          ErrorMessage="Select Emp Location" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
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
              <h3 class="box-title">Employee List</h3>
              <div class="box-tools">
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
            <div class="box-body">
              
                    <asp:GridView ID="GridView1" runat="server"    EnablePersistedSelection="true"            
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "5" 
    OnPageIndexChanging="GridViewEmp_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="EMP_ID" HeaderText="Employee ID" />
                     <asp:BoundField DataField="EMP_TITLE" HeaderText="Title" />
                     <asp:BoundField DataField="EMP_FNAME"  HeaderText="Fast Name" />
                     <asp:BoundField DataField="EMP_LNAME"  HeaderText="Last Name" /> 
                     <asp:BoundField DataField="EMP_NAT_ID"  HeaderText="NID" />
                     <asp:BoundField DataField="DEPARTMENT_NAME"  HeaderText="Deprtment" /> 
                     <asp:BoundField DataField="DIVISION_NAME"  HeaderText="Division" />  
                     <asp:BoundField DataField="LOCATION_NAME"  HeaderText="Location" />    
                     <asp:BoundField DataField="EMAIL"  HeaderText="Email" />
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                       
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-default btn-xs" runat="server" CommandArgument='<%# Eval("EMP_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
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