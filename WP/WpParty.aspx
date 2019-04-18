<%@ Page Title="Party (Supplier & Customer) Form & List - Waste Paper" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="WpParty.aspx.cs" Inherits="NRCAPPS.WP.WpParty" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Party (Supplier & Customer) Form & List
        <small>Party (Supplier & Customer): - Add - Update - Delete - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Waste Paper</a></li>
        <li class="active">Party</li>
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
              <h3 class="box-title">Party (Supplier & Customer) Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
                <div class="form-group">  
                   <label for="User_Name" class="col-sm-2 control-label">Party Name</label>
                 <div class="col-sm-3">  
                    <asp:TextBox ID="TextSupplierID" style="display:none" runat="server"></asp:TextBox>
                    <asp:TextBox ID="TextSupplierName" class="form-control input-sm"  runat="server" AutoPostBack="True" ontextchanged="TextSupplierName_TextChanged"></asp:TextBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                         ControlToValidate="TextSupplierName" ErrorMessage="Insert Party Name" 
                         SetFocusOnError="True" Display="Dynamic"></asp:RequiredFieldValidator> 
                  </div> 
                    <div class="col-sm-3"><asp:Label ID="CheckSupplierName" runat="server"></asp:Label> 
                    </div>
                </div>
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">Party Name Arabic</label> 
                  <div class="col-sm-3">  
                    <asp:TextBox ID="TextSupArabicName" class="form-control input-sm"  runat="server"></asp:TextBox>     
                  </div> 
                </div> 
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">Party Vat No</label> 
                  <div class="col-sm-3">  
                    <asp:TextBox ID="TextSupVatNo" class="form-control input-sm"  runat="server"></asp:TextBox>     
                  </div> 
                </div> 
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Party Address Line 1</label> 
                  <div class="col-sm-3">  
                    <asp:TextBox ID="TextSup_Add_1" class="form-control input-sm"  runat="server"></asp:TextBox>     
                  </div> 
                </div> 
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Party Address Line 2</label> 
                  <div class="col-sm-3">  
                    <asp:TextBox ID="TextSup_Add_2" class="form-control input-sm"  runat="server"></asp:TextBox>    
                  </div> 
                </div> 
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Contact No</label> 
                  <div class="col-sm-3">  
                    <asp:TextBox ID="TextContactNo" class="form-control input-sm"  runat="server"></asp:TextBox>    
                  </div> 
                </div>  
                <div class="form-group">
                  <label  class="col-sm-2 control-label">Address (Add More)</label> 
                       <div class="col-sm-3">    
                                <asp:Button type="button" id="contract_id" class="btn btn-primary" data-toggle="modal" data-target="#myModal">
                                + Add Address
                                </asp:Button>
                                <!-- Modal -->
                                <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                                  <div class="modal-dialog">
                                    <div class="modal-content">
                                      <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                                        <h4 class="modal-title" id="myModalLabel">Address (More) Form</h4>
                                      </div>
                                      <div class="modal-body">
                                          <div class="form-group">   
                                            <label class="col-sm-4 control-label">Address Line 1</label> 
                                            <div class="col-sm-6">    
                                              <asp:TextBox ID="TextAddressLine1" TextMode="multiline" Columns="100" Rows="5" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" 
                                            ControlToValidate="TextAddressLine1" ErrorMessage="Address Line 1" 
                                            Display="Dynamic" SetFocusOnError="True" ValidationGroup="VgContract"></asp:RequiredFieldValidator>
                                            </div>   
                                          </div> 
                                            <div class="form-group">  
                                              <label  class="col-sm-4 control-label">Address Line 2</label> 
                                            <div class="col-sm-6"> 
                                               <asp:TextBox ID="TextAddressLine2" TextMode="multiline" Columns="100" Rows="5" class="form-control input-sm"  runat="server" ></asp:TextBox>  
                                               <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                            ControlToValidate="TextAddressLine2" ErrorMessage="Address Line 2" 
                                            Display="Dynamic" SetFocusOnError="True" ValidationGroup="VgContract"></asp:RequiredFieldValidator>
                                                </div> 

                                            </div>
                                            <div class="form-group">             
                                              <label class="col-sm-4 control-label">Country</label>
                                            <div class="col-sm-6">  
                                                 <asp:DropDownList ID="DropDownCountryID" class="form-control input-sm" runat="server" > 
                                                </asp:DropDownList>  
                                                  <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                                                      ControlToValidate="DropDownCountryID" Display="Dynamic" 
                                                      ErrorMessage="Select Country" InitialValue="0" SetFocusOnError="True"  ValidationGroup="VgContract"></asp:RequiredFieldValidator>
                                            </div>  
                                            </div> 
                                      </div>
                                      <div class="modal-footer">
                                         <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                                         <asp:LinkButton ID="BtnAddContract" class="btn btn-primary" runat="server" Text="Add New" onclick="BtnAddAddress_Click" ValidationGroup="VgContract"><span class="fa fa-plus"></span> Add New</asp:LinkButton>
                                      
                                       <div class="box-body"> 
                                        <asp:GridView ID="GridView2" runat="server"    EnablePersistedSelection="true"          
                                            SelectedRowStyle-BackColor="Yellow"   AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                                         <Columns>      
                                         <asp:BoundField DataField="PARTY_ADD_1"  HeaderText="Party Address Line 1"   />  
                                         <asp:BoundField DataField="PARTY_ADD_2"  HeaderText="Party Address Line 2"  />   
                                           <asp:TemplateField HeaderText="Country" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>  
                                                <small class="label bg-blue"> <asp:Label ID="ContractNoSerial" Text='<%#Bind("COUNTRY_NAME") %>'  runat="server" /></small> 
                                            </ItemTemplate>
                                         </asp:TemplateField>
                                          <asp:TemplateField  HeaderText="Action" >
                                           <ItemTemplate> 
                                            <asp:LinkButton ID="DeleteContract" class="btn btn-danger btn-sm" runat="server" CommandArgument='<%#  Eval("PARTY_ADDRESS_ID")%>' OnClick="DeleteAddressClick" CausesValidation="False">Delete</asp:LinkButton> 
                                            </ItemTemplate>
                                           </asp:TemplateField>  
                                         </Columns>
                                            <PagerStyle CssClass="pagination-ys" />
                                            <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                                        </asp:GridView> 
                 
                                     </div>
                                      
                                      </div> 
                                       
                                    </div>
                                  </div>
                                </div>
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
              <h3 class="box-title">Party (Supplier & Customer) List</h3>
              
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView4D" runat="server" EnablePersistedSelection="true"            
    SelectedRowStyle-BackColor="Yellow"   AutoGenerateColumns="false"   CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="PARTY_ID" HeaderText="Party ID" />  
                     <asp:BoundField DataField="PARTY_NAME"  HeaderText="Party Name" />
                     <asp:BoundField DataField="PARTY_ARABIC_NAME"  HeaderText="Party Arabic Name" /> 
                     <asp:BoundField DataField="PARTY_VAT_NO"  HeaderText="Party Vat No" />    
                     <asp:BoundField DataField="PARTY_ADD_1"  HeaderText="Party Add. Line 1" /> 
                     <asp:BoundField DataField="PARTY_ADD_2"  HeaderText="Party Add. Line 2" /> 
                     <asp:BoundField DataField="CONTACT_NO"  HeaderText="Contact No" /> 
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />   
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("PARTY_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
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