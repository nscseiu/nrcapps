<%@ Page Title="Asset Items & Employee Form & List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="AssetItemsEmp.aspx.cs" Inherits="NRCAPPS.IT.IT_ASSET.AssetItemsEmp" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Asset Items & Employee Form & List
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
  <section class="col-lg-6 connectedSortable ui-sortable">  
           <div class="box box-primary">
           <!-- /.box-header -->
            <div class="box-header with-border">
              <h3 class="box-title">IT Asset Item Search</h3> 
            </div>  
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
               <div class="">
                 
                  <div class="col-sm-3"> 
                   <label  class="control-label">Enter QR Code ID</label> 
                  <!-- asp:label ID="PlaceHolder1"  / --><!-- /label --> 
                  <asp:TextBox runat="server" ID="TextQrCodeID" class="form-control input-sm" data-placeholder="QR Code ID No." AutoPostBack="True"  ontextchanged="DisplayItemSearch"> 
                  </asp:TextBox>
                     <asp:LinkButton ID="LinkButton4" runat="server" class="btn btn-default btn-sm" style="margin-top:4px;" OnClick="clearTextFieldSearch" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> 
                  </div>   
                </div> <asp:Label ID="CheckItemSearch" runat="server"></asp:Label>
              </div> 
       </div> 
       
             <asp:Panel  id="alert_box" runat="server">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <h4><i class="icon fa fa-check"></i> Alert!</h4>  
           </asp:Panel> 
          <!-- Horizontal Form -->
          <div class="box box-primary">
            <div class="box-header with-border">
              <h3 class="box-title">Asset Item Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
               <div class="form-group">
                  <label  class="col-sm-3 control-label">Employee</label> 
                  <div class="col-sm-7"> 
                   <asp:TextBox ID="TextEmpItemsID" style="display:none" runat="server"></asp:TextBox>
                  <!-- asp:label ID="PlaceHolder1"  / --><!-- /label --> 
                  <asp:ListBox runat="server" ID="DropDownEmployeeID" class="form-control  input-sm select2" data-placeholder="Select Employee" AutoPostBack="True"  ontextchanged="DisplayEmpItem" > 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="DropDownEmployeeID" Display="Dynamic" 
                          ErrorMessage="select Employee" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Location</label> 
                  <div class="col-sm-4">   
                    <asp:DropDownList ID="DropDownLocationID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" 
                          ControlToValidate="DropDownLocationID" Display="Dynamic" 
                          ErrorMessage="Select Emp Location" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>
                <div class="form-group">  
                   <label for="User_Name" class="col-sm-3 control-label">Select Item</label>
                 <div class="col-sm-7">  
                    <asp:TextBox ID="TextItemID" style="display:none" runat="server"></asp:TextBox>
                    <asp:ListBox runat="server" ID="DropDownItemID" class="form-control  input-sm select2" data-placeholder="Select Item" AutoPostBack="True" ontextchanged="DisplayItemCatQRPriCode"  > 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                          ControlToValidate="DropDownItemID" Display="Dynamic" 
                          ErrorMessage="select Item" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator> 
                  </div>  
                </div> 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Is QR Code</label> 
                     <div class="col-sm-2"> 
                       <asp:RadioButtonList ID="RadioBtnQrCode" runat="server" AutoPostBack = "true"  onselectedindexchanged="Redio_QrCodeChanged" RepeatDirection="Horizontal">
                             <asp:ListItem Value="QrCodeYes" Selected="True">Yes &nbsp;</asp:ListItem>
                             <asp:ListItem Value="QrCodeNo">No </asp:ListItem>  
                        </asp:RadioButtonList>  
                    </div> 
                  <div class="col-sm-5">  
                  <div id="QrCodeBox" style="border:#d2d6de solid 1px;" runat="server">
                    <asp:TextBox ID="TextQRPreCode" class="form-control input-sm"  runat="server"></asp:TextBox>   
                    <asp:TextBox ID="TextQRCode" class="form-control input-sm"  runat="server" AutoPostBack="True" ontextchanged="TextQRCode_TextChanged"></asp:TextBox>   
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextQRCode" ErrorMessage="Insert 6 digit QR ID number" 
                          Display="Dynamic"></asp:RequiredFieldValidator> 
                          <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
                          ControlToValidate="TextQRCode" Display="Dynamic" 
                          ErrorMessage="Insert 6 digit QR ID number" SetFocusOnError="True" 
                          ValidationExpression="^([0-9]{6})$"></asp:RegularExpressionValidator> 
                      <div class="col-sm-7"><asp:Label ID="CheckQRCode" runat="server"></asp:Label></div> 
                    <asp:Panel  id="ImageBox" runat="server">
                     <asp:Image ID="TextQrImage" Height = "125" Width = "125" runat="server" /> 
                     </asp:Panel>  
                      <asp:PlaceHolder ID="plBarCode" runat="server" />  
                     </div>  
                  </div> 
               </div> 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Is Active Status</label> 
                  <div class="col-sm-4" style="padding-top:6px;">    
                        <label>
                            <input type="checkbox" ID="CheckIsActive" class="flat-red" checked runat="server"/>
                        </label>
                  </div>
                </div> 
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-3" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-7">    
                    <asp:LinkButton ID="BtnAdd" class="btn btn-primary" runat="server" Text="Add New" onclick="BtnAdd_Click"><span class="fa fa-plus"></span> Add New</asp:LinkButton>
                    <asp:LinkButton ID="BtnUpdate" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdate_Click"><span class="fa fa-edit"></span> Update</asp:LinkButton>
                    <asp:LinkButton ID="BtnDelete" class="btn btn-danger" runat="server" onclick="BtnDelete_Click" onclientclick="return confirm('Are you sure to delete?');" ><span class="fa fa-close"></span> Delete</asp:LinkButton>
                 </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
   </section>
    <div class="col-md-6"> 
        <asp:Panel  id="alert_box_right" runat="server">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <h4><i class="icon fa fa-check"></i> Alert!</h4>  
           </asp:Panel> 
        <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Asset Item Change (Employee to Employee)</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
               <div class="form-group">
                  <label  class="col-sm-4 control-label">Employee</label> 
                  <div class="col-sm-7"> 
                  <asp:ListBox runat="server" ID="DropDownEmployeeIDChange" class="form-control input-sm select2" data-placeholder="Select Employee" AutoPostBack="True"  ontextchanged="DisplayEmpChangeItem" > 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                          ControlToValidate="DropDownEmployeeIDChange" Display="Dynamic" 
                          ErrorMessage="Select Employee" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup1'></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">  
                   <label for="User_Name" class="col-sm-4 control-label">Select Item</label>
                 <div class="col-sm-7">  
                    <asp:TextBox ID="TextBox1" style="display:none" runat="server"></asp:TextBox>
                    <asp:ListBox runat="server" ID="DropDownItemIDChange" class="form-control  input-sm select2" data-placeholder="Select Item" > 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                          ControlToValidate="DropDownItemIDChange" Display="Dynamic" 
                          ErrorMessage="Select Item" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup1'></asp:RequiredFieldValidator> 
                  </div>  
                </div> 
                <div class="form-group">
                  <label  class="col-sm-4 control-label">Item Change for Employee</label> 
                  <div class="col-sm-5"> 
                  <asp:ListBox runat="server" ID="DropDownEmployeeIDChangeFor" class="form-control select2" data-placeholder="Select Employee" AutoPostBack="True"  ontextchanged="EmpChangeItemBtn" > 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownEmployeeIDChangeFor" Display="Dynamic" 
                          ErrorMessage="select Employee" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup1'></asp:RequiredFieldValidator>
                  </div>
                </div>
                 <div class="form-group">
                  <label  class="col-sm-4 control-label">Location</label> 
                  <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownLocationChangeFor" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" 
                          ControlToValidate="DropDownLocationChangeFor" Display="Dynamic" 
                          ErrorMessage="Select Emp Location" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup1'></asp:RequiredFieldValidator>
                  </div>
                </div>
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-4" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton1" runat="server" class="btn btn-default" OnClick="clearTextChangeField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-7">     
                    <asp:LinkButton ID="BtnUpdateChangeItemEmp" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdateChangeItemEmp_Click" ValidationGroup='valGroup1' ClientIDMode="Static"><span class="fa fa-edit"></span> Update</asp:LinkButton>
                  </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>
         </div> 
    <div class="col-md-6"> 
        <asp:Panel  id="alert_box_right2" runat="server">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <h4><i class="icon fa fa-check"></i> Alert!</h4>  
           </asp:Panel> 
        <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Asset Item Change (Employee to Department)</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
               <div class="form-group">
                  <label  class="col-sm-4 control-label">Employee</label> 
                  <div class="col-sm-7"> 
                  <asp:ListBox runat="server" ID="DropDownEmployeeIDDept" class="form-control input-sm select2" data-placeholder="Select Employee" AutoPostBack="True"  ontextchanged="DisplayEmpDeptChangeItem" > 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownEmployeeIDDept" Display="Dynamic" 
                          ErrorMessage="Select Employee" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup3'></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">  
                   <label for="User_Name" class="col-sm-4 control-label">Select Item</label>
                 <div class="col-sm-7">  
                    <asp:TextBox ID="TextBox2" style="display:none" runat="server"></asp:TextBox>
                    <asp:ListBox runat="server" ID="DropDownItemIDChangeDept" class="form-control  input-sm select2" data-placeholder="Select Item"  AutoPostBack="True"  ontextchanged="DeptChangeItemBtn"> 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" 
                          ControlToValidate="DropDownItemIDChangeDept" Display="Dynamic" 
                          ErrorMessage="Select Item" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup3'></asp:RequiredFieldValidator> 
                  </div>  
                </div> 
                <div class="form-group">
                  <label  class="col-sm-4 control-label">Item Change for Department</label> 
                  <div class="col-sm-5"> 
                  <asp:DropDownList runat="server" ID="DropDownDepartmentID" class="form-control input-sm" data-placeholder="Select Department"  > 
                  </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" 
                          ControlToValidate="DropDownDepartmentID" Display="Dynamic" 
                          ErrorMessage="select Employee" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup3'></asp:RequiredFieldValidator>
                  </div>
                </div> 
                 <div class="form-group">
                  <label  class="col-sm-4 control-label">Select Division</label> 
                  <div class="col-sm-5"> 
                  <asp:DropDownList runat="server" ID="DropDownDivisionID" class="form-control input-sm" data-placeholder="Select Division" > 
                  </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" 
                          ControlToValidate="DropDownDivisionID" Display="Dynamic" 
                          ErrorMessage="Select Division" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup3'></asp:RequiredFieldValidator>
                  </div>
                </div> 
                   <div class="form-group">
                  <label  class="col-sm-4 control-label">Location</label> 
                  <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownChangeDeptLocationID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" 
                          ControlToValidate="DropDownChangeDeptLocationID" Display="Dynamic" 
                          ErrorMessage="Select Location" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup3'></asp:RequiredFieldValidator>
                  </div>
                </div>
                 <div class="form-group">
                  <label  class="col-sm-4 control-label">Placement</label> 
                  <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownChangeDeptPlacementID" class="form-control input-sm" runat="server"  > 
                    </asp:DropDownList>   
                  </div>
                </div>
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-4" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton3" runat="server" class="btn btn-default" OnClick="clearTextChangeField" ValidationGroup='valGroup3' CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-7">     
                    <asp:LinkButton ID="BtnUpdateChangeItemDept" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdateChangeItemDept_Click" ValidationGroup='valGroup3' ClientIDMode="Static"><span class="fa fa-edit"></span> Update</asp:LinkButton>
                  </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>
         </div> 
         </div>
        <!-- .row --> 
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Asset Items & Employee List</h3>
              
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive">  
                    <asp:GridView ID="GridViewItem" runat="server" EnablePersistedSelection="true"  AutoGenerateColumns="false"   CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="EMP_ITEMS_ID" HeaderText="ID" /> 
                     <asp:BoundField DataField="QR_CODE_ID" HeaderText="QR Code ID" /> 
                      <asp:TemplateField HeaderText="QR Code">
                        <ItemTemplate>
                          <asp:Image ID="Image1" Height = "40" Width = "40" runat="server"
                             ImageUrl = '<%# "QRCode/"+Eval("IMAGE_QR_CODE") %>' />
                        </ItemTemplate>
                     </asp:TemplateField>     
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item Name" />    
                     <asp:BoundField DataField="ITEM_TYPE"  HeaderText="Item Type" />  
                      <asp:BoundField DataField="ITEM_BRAND"  HeaderText="Item Brand" /> 
                      <asp:BoundField DataField="LOCATION_NAME"  HeaderText="Location" />                          
                      <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("EMP_ITEMS_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField> 
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                    </asp:GridView>  
        </div>
       </div> 
    
          <!-- /.box --> 
        <!--/.col (right) -->
          <div class="box box-danger">
           <!-- /.box-header -->
            <div class="box-header with-border">
              <h3 class="box-title">IT Asset (Employee Wise) Report</h3> 
            </div>  
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
               <div class="form-group">
                  <label  class="col-sm-2 control-label">Employee</label> 
                  <div class="col-sm-4"> 
                  
                  <!-- asp:label ID="PlaceHolder1"  / --><!-- /label --> 
                  <asp:ListBox runat="server" ID="DropDownEmployeeIDReport" class="form-control select2" data-placeholder="Select Employee"> 
                  </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                          ControlToValidate="DropDownEmployeeIDReport" Display="Dynamic" 
                          ErrorMessage="select Employee" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup2' ></asp:RequiredFieldValidator>
                  </div>
                </div> 
                  
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton2" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-7">     
                    <asp:LinkButton ID="BtnReport" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport_Click" ValidationGroup='valGroup2' ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                 </div>
                </div>

                <asp:Panel ID="Panel1" runat="server">  
                <%if (IsLoad)
                  {%> 
                <div class="panel panel-info">
                    <div class="panel-heading">
                      <h3 class="panel-title">Report View</h3>
                    </div> 
                    <div class="panel-body">         
                       <iframe src="ASSET_Reports/Asset_Items_Employee_Report_View.aspx?DropDownEmployeeIDReport=<%=DropDownEmployeeIDReport.Text %>" width="1350px" id="iframe1"
                    marginheight="0" frameborder="0" scrolling="auto" height="950px">   </iframe> 
                        </div> 
                     </div> 
                   <%} %>   
                </asp:Panel> 
                 
        </div>
       </div> 


           <div class="box box-danger">
           <!-- /.box-header -->
            <div class="box-header with-border">
              <h3 class="box-title">IT Asset - Comprehensive User Data (Department Wise) Report</h3> 
            </div>  
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
                  
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton5" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-7">     
                    <asp:LinkButton ID="LinkButton6" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport1_Click" ValidationGroup='valGroup5' ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                 </div>
                </div>

                <asp:Panel ID="Panel2" runat="server">  
                <%if (IsLoad1)
                  {%> 
                <div class="panel panel-info">
                    <div class="panel-heading">
                      <h3 class="panel-title">Report View</h3>
                    </div> 
                    <div class="panel-body">         
                       <iframe src="ASSET_Reports/Asset_Items_Department_Report_View.aspx" width="1350px" id="iframe2"
                    marginheight="0" frameborder="0" scrolling="auto" height="950px">   </iframe> 
                        </div> 
                     </div> 
                   <%} %>   
                </asp:Panel> 
                 
        </div>
       </div> 
      </div>
      <!-- /.row -->
    </section>
    <!-- /.content -->
 


</asp:Content> 