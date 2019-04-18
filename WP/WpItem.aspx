<%@ Page Title="Item Form & List - Waste Paper" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="WpItem.aspx.cs" Inherits="NRCAPPS.WP.WpItem" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Item Form & List
        <small>Item: - Add - Update - Delete - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Waste Paper</a></li>
        <li class="active">Item</li>
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
              <h3 class="box-title">Item Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">Category</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownCategoryID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                         <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownCategoryID" Display="Dynamic" 
                          ErrorMessage="Select Category" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>
                <div class="form-group">  
                   <label for="TextItemName" class="col-sm-2 control-label">Item Name</label>
                 <div class="col-sm-3">  
                    <asp:TextBox ID="TextItemID" style="display:none" runat="server"></asp:TextBox>
                    <asp:TextBox ID="TextItemName" class="form-control input-sm"  runat="server" AutoPostBack="True" ontextchanged="TextItemName_TextChanged"></asp:TextBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                         ControlToValidate="TextItemName" ErrorMessage="Insert Item Name" 
                         SetFocusOnError="True" Display="Dynamic"></asp:RequiredFieldValidator> 
                  </div> 
                    <div class="col-sm-3"><asp:Label ID="CheckItemName" runat="server"></asp:Label> 
                    </div>
                </div>
                <div class="form-group">  
                   <label for="TextItemArabicName" class="col-sm-2 control-label">Item Arabic Name</label>
                 <div class="col-sm-3">   
                    <asp:TextBox ID="TextItemArabicName" class="form-control input-sm"  runat="server"></asp:TextBox>   
                  </div>  
                </div>
               <div class="form-group">  
                   <label for="TextItemDescription" class="col-sm-2 control-label">Description</label>
                 <div class="col-sm-3">   
                    <asp:TextBox ID="TextItemDescription" class="form-control input-sm"  runat="server"></asp:TextBox>   
                  </div>  
                </div>
                 <div class="form-group">  
                   <label for="User_Name" class="col-sm-2 control-label">Item Code</label>
                 <div class="col-sm-3">   
                    <asp:TextBox ID="TextItemCode" class="form-control input-sm"  runat="server" AutoPostBack="True" ontextchanged="TextItemCode_TextChanged"></asp:TextBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                         ControlToValidate="TextItemCode" ErrorMessage="Insert Item Code" 
                         SetFocusOnError="True" Display="Dynamic"></asp:RequiredFieldValidator> 
                  </div> 
                    <div class="col-sm-3"><asp:Label ID="CheckItemCode" runat="server"></asp:Label> 
                    </div>
                </div>
                    <div class="form-group">
                  <label  class="col-sm-2 control-label">Item Rate</label> 
                  <div class="col-sm-2">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextItemRate" class="form-control input-sm"  runat="server" ></asp:TextBox> <!--  AutoPostBack="True"  ontextchanged="TextItemRate_Changed" --> 
                    <span class="input-group-addon">.00</span>      
                    </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" 
                          ControlToValidate="TextItemRate" ErrorMessage="Insert Item Rate" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
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
              <h3 class="box-title">Item List</h3>
              <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchUserRole" Class="form-control input-sm" runat="server" />
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
                    <asp:GridView ID="GridView1" runat="server" EnablePersistedSelection="true"            
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "20" 
    OnPageIndexChanging="GridViewUser_PageIndexChanging" AutoGenerateColumns="false"   CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="ITEM_ID" HeaderText="Item ID" />  
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item Name" /> 
                     <asp:BoundField DataField="ITEM_ARABIC_NAME"  HeaderText="Item Arabic Name" />                       
                     <asp:BoundField DataField="ITEM_CODE"  HeaderText="Item Code" />  
                     <asp:BoundField DataField="ITEM_RATE"  HeaderText="Item Rate" />                          
                     <asp:BoundField DataField="ITEM_DESCRIPTION"  HeaderText="Item Description" />   
                     <asp:BoundField DataField="CATEGORY_NAME"  HeaderText="Category" />                            
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />   
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("ITEM_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
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