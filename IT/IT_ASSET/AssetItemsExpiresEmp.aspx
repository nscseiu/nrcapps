<%@ Page Title=" Item Expires & Employee Form & List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="AssetItemsExpiresEmp.aspx.cs" Inherits="NRCAPPS.IT.IT_ASSET.AssetItemsExpiresEmp" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
         Item Expires & Employee Form & List
        <small>Asset Items & Employee: - Add - Update - Delete - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">IT</a></li>
        <li><a href="#">IT Asset</a></li>
        <li class="active">Items & Employee</li>
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
              <h3 class="box-title">Asset Item Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
                <div class="form-group">
                  <label  class="col-sm-2 control-label"> </label> 
                  <div class="col-sm-3"> 
                    <asp:RadioButtonList ID="radExpSelect" runat="server" AutoPostBack = "true"  onselectedindexchanged="Redio_CheckedChanged" RepeatDirection="Horizontal">
                         <asp:ListItem Value="Employee" Selected="True">Employee &nbsp;</asp:ListItem>
                         <asp:ListItem Value="Department">Department </asp:ListItem> 
                    </asp:RadioButtonList>     
                  </div>
                 </div> 
               <div id="ExpEmp" runat="server" >  
               <div class="form-group">
                  <label  class="col-sm-2 control-label">Employee</label> 
                  <div class="col-sm-3">  
                  <asp:label ID="PlaceHolder1" runat="server" /></label> 
                  <asp:ListBox runat="server" ID="DropDownEmployeeID" class="form-control  input-sm select2" data-placeholder="Select Employee" AutoPostBack="True"  ontextchanged="DisplayEmpItem" > 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="DropDownEmployeeID" Display="Dynamic" 
                          ErrorMessage="Select Employee" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
                 </div>
                 <div id="ExpDept" runat="server" >
                  <div class="form-group">
                  <label  class="col-sm-2 control-label">Department</label> 
                  <div class="col-sm-3">  
                  <asp:DropDownList runat="server" ID="DropDownDepartmentID" class="form-control  input-sm" data-placeholder="Select Department" > 
                  </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownDepartmentID" Display="Dynamic" 
                          ErrorMessage="Select Department" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">Select Division</label> 
                  <div class="col-sm-3"> 
                  <asp:DropDownList runat="server" ID="DropDownDivisionID" class="form-control input-sm" data-placeholder="Select Division"  AutoPostBack="True"  ontextchanged="DisplayEmpItem"  > 
                  </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                          ControlToValidate="DropDownDivisionID" Display="Dynamic" 
                          ErrorMessage="Select Division"  InitialValue="0" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                </div>  
                 </div>
                <div class="form-group">  
                   <label for="User_Name" class="col-sm-2 control-label">Select Item</label>
                 <div class="col-sm-3">  
                    <asp:TextBox ID="TextItemID" style="display:none" runat="server"></asp:TextBox>
                    <asp:ListBox runat="server" ID="DropDownItemID" class="form-control  input-sm select2" data-placeholder="Select Item"  > 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                          ControlToValidate="DropDownItemID" Display="Dynamic" 
                          ErrorMessage="select Item" SetFocusOnError="True"></asp:RequiredFieldValidator>
                     
                  </div>  
                </div>
             <div class="form-group">  
                   <label class="col-sm-2 control-label">Select Item Expire</label>
                 <div class="col-sm-3">   
                    <asp:ListBox runat="server" ID="DropDownItemExpID" class="form-control  input-sm select2" data-placeholder="Select Item" AutoPostBack="True" ontextchanged="DisplayItemCatQRPriCode"  > 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownItemExpID" Display="Dynamic" 
                          ErrorMessage="Select Item" SetFocusOnError="True"></asp:RequiredFieldValidator> 
                  </div>  
                </div> 
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Item Exp. ID</label> 
                  <div class="col-sm-2">  
                    <asp:TextBox ID="TextQRPreCode" class="form-control input-sm"  runat="server"></asp:TextBox>   
                    <asp:TextBox ID="TextQRCode" class="form-control  input-sm"  runat="server" AutoPostBack="True" ontextchanged="TextQRCode_TextChanged"></asp:TextBox>   
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextQRCode" ErrorMessage="Insert 6 digit QR ID number" 
                          Display="Dynamic"></asp:RequiredFieldValidator> 
                          <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
                          ControlToValidate="TextQRCode" Display="Dynamic" 
                          ErrorMessage="Insert 6 digit QR ID number" SetFocusOnError="True" 
                          ValidationExpression="^([0-9]{6})$"></asp:RegularExpressionValidator>
                       
                  </div> 
                    <div class="col-sm-4"><asp:Label ID="CheckQRCode" runat="server"></asp:Label> 
                    </div> 
               </div>
               <div class="form-group">
                  <label  class="col-sm-2 control-label">Serial No.</label> 
                  <div class="col-sm-2">  
                    <asp:TextBox ID="TextSerialNo" class="form-control input-sm"  runat="server"></asp:TextBox>     
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                          ControlToValidate="TextSerialNo" ErrorMessage="Insert 7 digit Serial number" 
                          Display="Dynamic"></asp:RequiredFieldValidator> 
                          <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                          ControlToValidate="TextSerialNo" Display="Dynamic" 
                          ErrorMessage="Insert 7 digit QR ID number" SetFocusOnError="True" 
                          ValidationExpression="^([0-9]{7})$"></asp:RegularExpressionValidator> 
                  </div>  
               </div>
               <div class="form-group">
                  <label  class="col-sm-2 control-label">Activation Code</label> 
                  <div class="col-sm-2">  
                    <asp:TextBox ID="TextActivationCode" class="form-control input-sm" placeholder="XXXXX-XXXXX-XXXXX-XXXXX" runat="server"></asp:TextBox>     
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" 
                          ControlToValidate="TextActivationCode" ErrorMessage="Insert 23 digit Activation Code" 
                          Display="Dynamic"></asp:RequiredFieldValidator> 
                          <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" 
                          ControlToValidate="TextActivationCode" Display="Dynamic" 
                          ErrorMessage="Insert 23 digit Activation Code" SetFocusOnError="True" 
                          ValidationExpression="^[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}$"></asp:RegularExpressionValidator> 
                  </div>  
               </div>
                <!-- Date and time range -->
              <div class="form-group">
                 <label  class="col-sm-2 control-label">Activation and Expiration Date</label>
              <div class="col-sm-3">  
                <div class="input-group">
                  <div class="input-group-addon">
                    <i class="fa fa-clock-o"></i>
                  </div>
                  <asp:TextBox ID="TextStartExpiryDate" class="form-control  input-sm pull-right"  runat="server"></asp:TextBox> 
                </div>
                  </div>
               </div>
                <!-- /.input group -->
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Expired Days</label> 
                  <div class="col-sm-1">  
                    <asp:TextBox ID="TextExpiredDays" class="form-control input-sm"  runat="server"></asp:TextBox>     
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                          ControlToValidate="TextExpiredDays" ErrorMessage="Insert Expired Days" 
                          Display="Dynamic"></asp:RequiredFieldValidator>  
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
              <h3 class="box-title">Item Expires & Employee List</h3>
              
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive">  
                    <asp:GridView ID="GridViewItem" runat="server" EnablePersistedSelection="true"  AutoGenerateColumns="false"   CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="EMP_ITEM_EXP_ID" HeaderText="Item Exp. ID" /> 
                     <asp:BoundField DataField="ITEM_EXP_NAME"  HeaderText="Item Exp. Name" /> 
                     <asp:BoundField DataField="SERIAL_NO"  HeaderText="Serial No." /> 
                     <asp:BoundField DataField="ACTIVATION_CODE"  HeaderText="Activation Code" />
                     <asp:TemplateField HeaderText="Remaining Days">
                        <ItemTemplate>
                             <div class="progress active">
                            <div id="progress_bar" class="progress-bar progress-bar-info progress-bar-striped" role="progressbar" aria-valuenow='<%# Eval("EXPIRED_DAYS_BET")%>' aria-valuemin="0" aria-valuemax='<%# Eval("EXPIRED_DAYS")%>' style='<%#"width:" + ((Decimal.Parse(Eval("EXPIRED_DAYS_BET").ToString())/(Decimal.Parse(Eval("EXPIRED_DAYS").ToString())))*100)+"%"%>'  runat="server">
                               <%# Eval("EXPIRED_DAYS_BET")%> days in <%# Eval("EXPIRED_DAYS")%> days
                            </div>
                          </div>             
                        </ItemTemplate>
                      </asp:TemplateField>   
                     <asp:BoundField DataField="START_DATE"  HeaderText="Activation Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}" />   
                     <asp:BoundField DataField="EXPIRES_DATE"  HeaderText="Expiration Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}" />    
                      
                      <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("EMP_ITEM_EXP_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
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