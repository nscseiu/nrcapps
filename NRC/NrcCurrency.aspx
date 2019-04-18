<%@ Page Title="Currency Form & List - Apps & Data Settings " Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="NrcCurrency.aspx.cs" Inherits="NRCAPPS.NRC.NrcCurrency" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Currency Form & List
        <small>Currency: - Add - Update - Delete - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Apps & Data Settings</a></li>
        <li class="active">Currency</li>
      </ol>
    </section>
    
    <!-- Main content -->
    <section class="content">
      <div class="row">
        <!-- left column --> 
            
        <!--/.col (left) -->
        <!-- right column -->
        <div class="col-md-6"> 
             <asp:Panel  id="alert_box" runat="server">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <h4><i class="icon fa fa-check"></i> Alert!</h4>  
           </asp:Panel> 
          <!-- Horizontal Form -->
          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Currency Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
                <div class="form-group">  
                   <label for="TextCurrencySize" class="col-sm-4 control-label">Currency Name</label>
                 <div class="col-sm-4">  
                    <asp:TextBox ID="TextCurrencyID" style="display:none" runat="server"></asp:TextBox>
                    <asp:TextBox ID="TextCurrency" class="form-control input-sm"  runat="server" AutoPostBack="True" ontextchanged="TextCurrency_TextChanged"></asp:TextBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                         ControlToValidate="TextCurrency" ErrorMessage="Insert Currency Name" 
                         SetFocusOnError="True" Display="Dynamic"></asp:RequiredFieldValidator> 
                  </div> 
                    <div class="col-sm-3"><asp:Label ID="CheckCurrency" runat="server"></asp:Label> 
                    </div>
                </div>
                <div class="form-group">  
                   <label for="TextCurrencySymbol" class="col-sm-4 control-label">Currency Symbol</label>
                 <div class="col-sm-4">   
                    <asp:TextBox ID="TextCurrencySymbol" class="form-control input-sm"  runat="server"></asp:TextBox>   
                  </div>  
                </div>
               <div class="form-group">  
                   <label for="TextCurrencyImage" class="col-sm-4 control-label">Currency Image</label>
                 <div class="col-sm-4">   
                    <asp:TextBox ID="TextCurrencyImage" class="form-control input-sm"  runat="server"></asp:TextBox>   
                  </div>  
                </div>
                
                <div class="form-group">
                  <label  class="col-sm-4 control-label">Is Active Status</label> 
                  <div class="col-sm-4" style="padding-top:6px;">    
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
                  <div  class="col-sm-4" style="text-align:right;"> 
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
              <h3 class="box-title">Currency List</h3>
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
    SelectedRowStyle-BackColor="Yellow"   AutoGenerateColumns="false"   CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="CURRENCY_ID" HeaderText="Currency ID" />  
                     <asp:BoundField DataField="CURRENCY_NAME"  HeaderText="Currency Name" /> 
                     <asp:BoundField DataField="CURRENCY_SYMBOL"  HeaderText="Currency Symbol" />                       
                     <asp:BoundField DataField="CURRENCY_IMAGE"  HeaderText="Currency Image" />                              
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />   
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("CURRENCY_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
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
            <div class="col-md-6"> 
             <asp:Panel  id="alert_box_right" runat="server">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <h4><i class="icon fa fa-check"></i> Alert!</h4>  
           </asp:Panel> 
          <!-- Horizontal Form -->
          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Currency Rate Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
                <div class="form-group">  
                   <label for="TextCurrencySize" class="col-sm-4 control-label">Source Currency Name</label>
                 <div class="col-sm-4">  
                    <asp:TextBox ID="TextCurrencyRateID" style="display:none" runat="server"></asp:TextBox> 
                     <asp:DropDownList ID="DropDownSourceCurrencyID" class="form-control input-sm" runat="server"> 
                     </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                         ControlToValidate="DropDownSourceCurrencyID" ErrorMessage="Select Source Currency Name" 
                         SetFocusOnError="True" Display="Dynamic" InitialValue="0"  ValidationGroup='valGroup1'></asp:RequiredFieldValidator> 
                  </div>  
                </div>
                  <div class="form-group">  
                 <label for="TextCurrencySize" class="col-sm-4 control-label">Target Currency Name</label>
                 <div class="col-sm-4">   
                     <asp:DropDownList ID="DropDownTargetCurrencyID" class="form-control input-sm" runat="server"> 
                     </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                         ControlToValidate="DropDownTargetCurrencyID" ErrorMessage="Select Target Currency Name" 
                         SetFocusOnError="True" Display="Dynamic" InitialValue="0"  ValidationGroup='valGroup1'></asp:RequiredFieldValidator> 
                  </div>  
                </div>
                
               <div class="form-group">  
                   <label for="TextExchangeRate" class="col-sm-4 control-label">Exchange Rate</label>
                 <div class="col-sm-4">   
                    <asp:TextBox ID="TextExchangeRate" class="form-control input-sm"  runat="server"></asp:TextBox>  
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                         ControlToValidate="TextExchangeRate" ErrorMessage="Insert Currency Exchange Rate" 
                         SetFocusOnError="True" Display="Dynamic" ValidationGroup='valGroup1'></asp:RequiredFieldValidator> 
                  </div>  
                </div>
                
                <div class="form-group">
                  <label  class="col-sm-4 control-label">Is Active Status</label> 
                  <div class="col-sm-4" style="padding-top:6px;">    
                        <label>
                            <input type="checkbox" ID="Checkbox1" class="flat-red" checked runat="server"/>
                        </label>
                  </div>
                </div> 
                 <!-- checkbox --> 
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-4" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton1" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">    
                    <asp:LinkButton ID="BtnAdd_CRate" class="btn btn-primary" runat="server" Text="Add New" onclick="BtnAdd_CRateClick" ValidationGroup='valGroup1'><span class="fa fa-plus"></span> Add New</asp:LinkButton>
                    <asp:LinkButton ID="BtnUpdate_CRate" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdate_CRateClick" ValidationGroup='valGroup1'><span class="fa fa-edit"></span> Update</asp:LinkButton>
                    <asp:LinkButton ID="BtnDelete_CRate" class="btn btn-danger" runat="server" onclick="BtnDelete_CRateClick" onclientclick="return confirm('Are you sure to delete?');" ValidationGroup='valGroup1' ><span class="fa fa-close"></span> Delete</asp:LinkButton>
                 </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Currency Rate List</h3>
              <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="TextBox5" Class="form-control input-sm" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="Button1" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridViewSearchUser" 
                        CausesValidation="False" />
                  </div>  
              </div>    
            </div>
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView2" runat="server" EnablePersistedSelection="true"            
    SelectedRowStyle-BackColor="Yellow"   AutoGenerateColumns="false"   CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="CURRENCY_RATE_ID" HeaderText="Currency Rate ID" />  
                     <asp:BoundField DataField="SOURCE_CURRENCY_NAME"  HeaderText="Source Currency Name" /> 
                     <asp:BoundField DataField="TARGET_CURRENCY_NAME"  HeaderText="Target Currency Name" />                       
                     <asp:BoundField DataField="EXCHANGE_RATE"  HeaderText="Exchange Rate" />                              
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />   
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectRateClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("CURRENCY_RATE_ID") %>' OnClick="linkSelectRateClick" CausesValidation="False">Select</asp:LinkButton> 
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
      <!-- /.row -->
    </section>
    <!-- /.content -->
 
</div>

</asp:Content> 