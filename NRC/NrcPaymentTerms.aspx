<%@ Page Title="Payment Terms Form & List - Apps & Data Settings " Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="NrcPaymentTerms.aspx.cs" Inherits="NRCAPPS.NRC.NrcPaymentTerms" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Payment Terms Form & List
        <small>Payment Terms: - Add - Update - Delete - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Apps & Data Settings</a></li>
        <li class="active">Payment Terms</li>
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
              <h3 class="box-title">Payment Terms Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
                <div class="form-group">  
                   <label for="TextPayTerms" class="col-sm-2 control-label">Payment Terms </label>
                 <div class="col-sm-3">  
                    <asp:TextBox ID="TextPayTermsID" style="display:none" runat="server"></asp:TextBox>
                    <asp:TextBox ID="TextPayTerms" class="form-control input-sm"  runat="server" AutoPostBack="True" ontextchanged="TextPayTerms_TextChanged"></asp:TextBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                         ControlToValidate="TextPayTerms" ErrorMessage="Insert Payment Terms Name" 
                         SetFocusOnError="True" Display="Dynamic"></asp:RequiredFieldValidator> 
                  </div> 
                    <div class="col-sm-3"><asp:Label ID="CheckPayTerms" runat="server"></asp:Label> 
                    </div>
                </div>
                <div class="form-group">  
                   <label for="TextPayTermsDetails" class="col-sm-2 control-label">Payment Terms Details</label>
                 <div class="col-sm-3">   
                    <asp:TextBox ID="TextPayTermsDetails" class="form-control input-sm"  runat="server"></asp:TextBox>   
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
              <h3 class="box-title">Payment Terms List</h3>
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
                    <asp:GridView ID="GridView4D" runat="server" EnablePersistedSelection="true"            
    SelectedRowStyle-BackColor="Yellow"   AutoGenerateColumns="false"   CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="PAY_TERMS_ID" HeaderText="Payment Terms ID" />  
                     <asp:BoundField DataField="PAY_TERMS_NAME"  HeaderText="Payment Terms" /> 
                     <asp:BoundField DataField="PAY_TERMS_DETAILS"  HeaderText="Payment Terms In Words" />                                
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>   
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />   
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("PAY_TERMS_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
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