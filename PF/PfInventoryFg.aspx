<%@ Page Title="Inventory Finished Goods Form & List - Plastic Factory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="PfInventoryFg.aspx.cs" Inherits="NRCAPPS.PF.PfInventoryFg" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Inventory Finished Goods Form & List
        <small>Inventory Finished Goods: - Update - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Plastic Factory</a></li>
        <li class="active">Inventory Finished Goods</li>
      </ol>
    </section>
    
    <!-- Main content -->
    <section class="content">
      <div class="row">
        <!-- left column --> 
            
        <!--/.col (left) -->
        <!-- right column -->       
      <div class="col-md-4">
           <asp:Panel  id="alert_box" runat="server">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <h4><i class="icon fa fa-check"></i> Alert!</h4>  
           </asp:Panel> 
          <!-- Horizontal Form -->
          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Inventory Finished Goods Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
                 
                <div class="form-group">
                  <label  class="col-sm-5 control-label">Item</label> 
                  <div class="col-sm-4">   
                   <asp:TextBox ID="TextInventoryFgID" style="display:none" runat="server"></asp:TextBox>
                    <asp:DropDownList ID="DropDownItemID" class="form-control input-sm" runat="server" disabled = "disabled"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownItemID" Display="Dynamic" 
                          ErrorMessage="Select Item" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>
                 <div class="form-group">
                  <label  class="col-sm-5 control-label">Sub Item</label> 
                  <div class="col-sm-4">   
                    <asp:DropDownList ID="DropDownSubItemID" class="form-control input-sm" runat="server" disabled = "disabled"> 
                    </asp:DropDownList>   
                  </div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-5 control-label">Initial Stock of FG</label> 
                  <div class="col-sm-4"> 
                   <div class="input-group">  
                    <asp:TextBox ID="TextInitialStock" class="form-control input-sm"  runat="server"></asp:TextBox> 
                    <span class="input-group-addon">MT</span>      
                    </div> <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                          ControlToValidate="TextInitialStock" Display="Dynamic" 
                          ErrorMessage="Insert Initial Stock Weight" SetFocusOnError="True"></asp:RequiredFieldValidator>  
                  </div> 
                </div> 
                <div class="form-group">
                  <label  class="col-sm-5 control-label">Stock In of FG</label> 
                  <div class="col-sm-4">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextStockIn" class="form-control input-sm"  runat="server" disabled = "disabled"></asp:TextBox>   
                    <span class="input-group-addon">MT</span>      
                    </div> 
                  </div> 
                </div> 
                 <div class="form-group">
                  <label  class="col-sm-5 control-label">Stock Out of FG</label> 
                  <div class="col-sm-4">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextStockOut" class="form-control input-sm"  runat="server" disabled = "disabled"></asp:TextBox> 
                    <span class="input-group-addon">MT</span>      
                    </div>  
                  </div> 
                </div> 
                <div class="form-group">
                  <label  class="col-sm-5 control-label">Final Stock of FG</label> 
                  <div class="col-sm-4">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextFinalStock" class="form-control input-sm"  runat="server" disabled = "disabled"></asp:TextBox>    
                    <span class="input-group-addon">MT</span>      
                    </div>
                  </div> 
                </div> 
                <div class="form-group">
                  <label  class="col-sm-5 control-label">Average Rate of RM</label> 
                  <div class="col-sm-4"> 
                   <div class="input-group">  
                    <asp:TextBox ID="TextItemAvgRate" class="form-control input-sm"  runat="server"></asp:TextBox> 
                    <span class="input-group-addon">MT</span>      
                    </div> <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextItemAvgRate" Display="Dynamic" 
                          ErrorMessage="Insert Average Rate" SetFocusOnError="True"></asp:RequiredFieldValidator>  
                  </div> 
                </div>              
                 <!-- checkbox --> 
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-5" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">    
                     <asp:LinkButton ID="BtnUpdate" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdate_Click"><span class="fa fa-edit"></span> Update</asp:LinkButton>
                  </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div> 
         </div>  
        <div class="col-md-8">  
        <div class="box box-success">
        
            <div class="box-header with-border">
              <h3 class="box-title">Inventory Finished Goods List</h3>
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
    PageSize = "10" 
    OnPageIndexChanging="GridViewUser_PageIndexChanging" AutoGenerateColumns="false"   CssClass="table table-hover table-bordered table-striped" >
                     <Columns>
                     <%-- asp:BoundField DataField="FG_INVENTORY_ID" HeaderText="Inventory FG ID" / --%>  
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />   
                     <asp:BoundField DataField="SUB_ITEM_NAME"  HeaderText="Sub Item" /> 
                     <asp:BoundField DataField="INITIAL_STOCK_WT"  HeaderText="Initial Stock" DataFormatString="{0:0.000}" ItemStyle-HorizontalAlign="Right"  /> 
                     <asp:BoundField DataField="STOCK_IN_WT"  HeaderText="Stock In"  DataFormatString="{0:0.000}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="STOCK_OUT_WT"  HeaderText="Stock Out"  DataFormatString="{0:0.000}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="FINAL_STOCK_WT"  HeaderText="Final Stock"  DataFormatString="{0:0.000}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="ITEM_AVG_RATE"  HeaderText="AVG. Rate"  DataFormatString="{0:0.00}" ItemStyle-HorizontalAlign="Right"  /> 
                       
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />   
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("FG_INVENTORY_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
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

        <div class="col-md-12">  
        <div class="box box-warning">
        
            <div class="box-header with-border">
              <h3 class="box-title">Inventory Finished Goods Daily History List</h3>
              <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchHistory" Class="form-control input-sm" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="Button1" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridViewSearchHistory" 
                        CausesValidation="False" />
                  </div>  
              </div>    
            </div>
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView2" runat="server" EnablePersistedSelection="true"            
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "10" 
    OnPageIndexChanging="GridViewHistory_PageIndexChanging" AutoGenerateColumns="false"   CssClass="table table-hover table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="IN_FG_HIS_ID" HeaderText="In. FG History ID" />  
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy}"  />     
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />   
                     <asp:BoundField DataField="SUB_ITEM_NAME"  HeaderText="Sub Item" /> 
                     <asp:BoundField DataField="INITIAL_STOCK_WT"  HeaderText="Initial Stock" DataFormatString="{0:0.000}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="STOCK_IN_WT"  HeaderText="Stock In"  DataFormatString="{0:0.000}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="STOCK_OUT_WT"  HeaderText="Stock Out"  DataFormatString="{0:0.000}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="FINAL_STOCK_WT"  HeaderText="Final Stock"  DataFormatString="{0:0.000}" ItemStyle-HorizontalAlign="Right" />  
                     <asp:BoundField DataField="ITEM_AVG_RATE"  HeaderText="AVG. Rate"  DataFormatString="{0:0.00}" ItemStyle-HorizontalAlign="Right"  />  
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