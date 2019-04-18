<%@ Page Title="Inventory Raw Material Form & List - Metal Scrap" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="MsInventoryRm.aspx.cs" Inherits="NRCAPPS.MS.MsInventoryRm" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Inventory Raw Material Form & List
        <small>Inventory Raw Material: - Update - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Metal Scrap</a></li>
        <li class="active">Inventory Raw Material</li>
      </ol>
    </section>
    
    <!-- Main content -->
    <section class="content">
      <div class="row">
        <!-- left column --> 
            
        <!--/.col (left) -->
        <!-- right column --> 
           <div class='col-md-12'> 
           <div class='box'> 
            <div class='box-header with-border'> 
              <h3 class='box-title'><i class='fa fa-building-o'></i> 
                 Inventory Statement-(Default Current Month) 
                     
                </h3>   
                <div class='box-tools'>
                    
                   <div class="input-group date" style="width: 300px;">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control pull-right" ID="EntryDate"  runat="server"   ></asp:TextBox> 
                         <div class="input-group-btn">
                      <asp:Button ID="Button2" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="DisplayInventorySearch" 
                        CausesValidation="False" />
                  </div>  
                     </div> 
                     </div> 

            </div> 
                    <asp:PlaceHolder ID = "PlaceHolderInventoryReport" runat="server" />  
             
               </div> 
               </div> 
      <div class="col-md-4">
           <asp:Panel  id="alert_box" runat="server">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <h4><i class="icon fa fa-check"></i> Alert!</h4>  
           </asp:Panel> 
          <!-- Horizontal Form --> 
          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Inventory Raw Material Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
                 
                <div class="form-group">
                  <label  class="col-sm-5 control-label">Item</label> 
                  <div class="col-sm-4">   
                   <asp:TextBox ID="TextInventoryRmID" style="display:none" runat="server"></asp:TextBox>
                    <asp:DropDownList ID="DropDownItemID" class="form-control input-sm" runat="server" disabled = "disabled"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownItemID" Display="Dynamic" 
                          ErrorMessage="Select Item" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-5 control-label">Initial Stock of RM</label> 
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
                  <label  class="col-sm-5 control-label">Stock In of RM</label> 
                  <div class="col-sm-4">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextStockIn" class="form-control input-sm"  runat="server" disabled = "disabled"></asp:TextBox>   
                    <span class="input-group-addon">MT</span>      
                    </div> 
                  </div> 
                </div> 
                 <div class="form-group">
                  <label  class="col-sm-5 control-label">Stock Out of RM</label> 
                  <div class="col-sm-4">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextStockOut" class="form-control input-sm"  runat="server" disabled = "disabled"></asp:TextBox> 
                    <span class="input-group-addon">MT</span>      
                    </div>  
                  </div> 
                </div> 
                <div class="form-group">
                  <label  class="col-sm-5 control-label">Final Stock of RM</label> 
                  <div class="col-sm-4">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextFinalStock" class="form-control input-sm"  runat="server" disabled = "disabled"></asp:TextBox>    
                    <span class="input-group-addon">MT</span>      
                    </div>
                  </div> 
                </div>
                <div class="form-group">
                  <label  class="col-sm-5 control-label">Amount of RM</label> 
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
              <h3 class="box-title">Inventory Raw Material List</h3>
              <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 300px;">
                <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="TextMonthYear1"  runat="server" ></asp:TextBox>  <%-- AutoPostBack="True" ontextchanged="CheckDataProcess_PF_Monthly"  --%>
                    </div>
                 <div class="input-group-btn">
                      <asp:Button ID="BtnDataCheckWp" Class="btn btn-warning"   
                        Text="Check Inventory" runat="server" OnClick="BtnDataCheckMs_Click" 
                        CausesValidation="False" />
                  </div>  
              </div>    
            </div>
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView5D" runat="server" EnablePersistedSelection="true"            
    SelectedRowStyle-BackColor="Yellow" 
    AutoGenerateColumns="false"   CssClass="table table-hover table-bordered table-striped" >
                     <Columns>
                     <%-- asp:BoundField DataField="RM_INVENTORY_ID" HeaderText="Inventory RM ID" / --%>  
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />    
                     <asp:BoundField DataField="INITIAL_STOCK_WT"  HeaderText="Initial Stock" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right"  /> 
                     <asp:BoundField DataField="STOCK_IN_WT"  HeaderText="Stock In"  DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="STOCK_OUT_WT"  HeaderText="Stock Out"  DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right"  /> 
                     <asp:BoundField DataField="FINAL_STOCK_WT"  HeaderText="Final Stock"  DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="ITEM_END_AMOUNT"  HeaderText="Amount"  DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right"  />   
                     <%-- asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  /--%>  
                     <%-- asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  / --%>   
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("RM_INVENTORY_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
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
        <div class="col-md-4">
          
          <!-- Horizontal Form -->
          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Inventory Raw Material Monthly History Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
                 
                <div class="form-group">
                  <label  class="col-sm-5 control-label">Item</label> 
                  <div class="col-sm-4">   
                   <asp:TextBox ID="TextInventoryRmHisID" style="display:none" runat="server"></asp:TextBox>
                    <asp:DropDownList ID="DropDownItemID1" class="form-control input-sm" runat="server" disabled = "disabled"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                          ControlToValidate="DropDownItemID1" Display="Dynamic" 
                          ErrorMessage="Select Item" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup1'></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-5 control-label">Initial Stock of RM</label> 
                  <div class="col-sm-4"> 
                   <div class="input-group">  
                    <asp:TextBox ID="TextInitialStockHis" class="form-control input-sm"  runat="server" disabled = "disabled"></asp:TextBox> 
                    <span class="input-group-addon">MT</span>      
                    </div>  
                  </div> 
                </div> 
                <div class="form-group">
                  <label  class="col-sm-5 control-label">Stock In of RM</label> 
                  <div class="col-sm-4">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextStockInHis" class="form-control input-sm"  runat="server" disabled = "disabled"></asp:TextBox>   
                    <span class="input-group-addon">MT</span>      
                    </div> 
                  </div> 
                </div> 
                 <div class="form-group">
                  <label  class="col-sm-5 control-label">Stock Out of RM</label> 
                  <div class="col-sm-4">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextStockOutHis" class="form-control input-sm"  runat="server" disabled = "disabled"></asp:TextBox> 
                    <span class="input-group-addon">MT</span>      
                    </div>  
                  </div> 
                </div> 
                <div class="form-group">
                  <label  class="col-sm-5 control-label">Final Stock of RM</label> 
                  <div class="col-sm-4">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextFinalStockHis" class="form-control input-sm"  runat="server" ></asp:TextBox>    
                    <span class="input-group-addon">MT</span> 
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                          ControlToValidate="TextFinalStockHis" Display="Dynamic" 
                          ErrorMessage="Insert Final Stock" InitialValue="0" SetFocusOnError="True" ValidationGroup='valGroup1'></asp:RequiredFieldValidator>     
                    </div>
                  </div> 
                </div>
                <div class="form-group">
                  <label  class="col-sm-5 control-label">Amount of RM</label> 
                  <div class="col-sm-4"> 
                   <div class="input-group">  
                    <asp:TextBox ID="TextItemAvgRateHis" class="form-control input-sm"  runat="server"></asp:TextBox> 
                    <span class="input-group-addon">MT</span>      
                    </div> <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="TextItemAvgRateHis" Display="Dynamic" 
                          ErrorMessage="Insert Average Rate" SetFocusOnError="True" ValidationGroup='valGroup1'></asp:RequiredFieldValidator>  
                  </div> 
                </div>            
                 <!-- checkbox --> 
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-5" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton1" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">    
                     <asp:LinkButton ID="BtnUpdateHis" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdateHis_Click" ValidationGroup='valGroup1'><span class="fa fa-edit"></span> Update</asp:LinkButton>
                  </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div> 
         </div>  
        <div class="col-md-8">  
        <div class="box box-warning">
        
            <div class="box-header with-border">
              <h3 class="box-title">Inventory Raw Material Monthly History List</h3>
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
    PageSize = "12" 
    OnPageIndexChanging="GridViewHistory_PageIndexChanging" AutoGenerateColumns="false"   CssClass="table table-hover table-bordered table-striped" >
                     <Columns>  
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy}"  />     
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />     
                     <asp:BoundField DataField="INITIAL_STOCK_WT"  HeaderText="Initial Stock" DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right"  /> 
                     <asp:BoundField DataField="STOCK_IN_WT"  HeaderText="Stock In"  DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="STOCK_OUT_WT"  HeaderText="Stock Out"  DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="FINAL_STOCK_WT"  HeaderText="Final Stock"  DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />  
                     <asp:BoundField DataField="ITEM_END_AMOUNT"  HeaderText="Amount"  DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right"  /> 
                      <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectRmHisClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("IN_RM_HIS_ID") %>' OnClick="linkSelectRmHisClick" CausesValidation="False">Select</asp:LinkButton> 
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
 <style>
 
  #GridViewItem {background-color: #fff; margin: 0 0 10px 0; border: solid 1px #c1c1c1; border-collapse:collapse; font-family:Calibri; color: #474747;}

  #GridViewItem td { 
      padding: 4px; 
      border: solid 1px #c1c1c1; }

  #GridViewItem th  { 
      padding:4px; 
      text-align: center;  
      color: #fff; 
      background: #ba8d26; 
      border: solid 1px #c1c1c1; 
      font-size: 0.9em; }

  #GridViewItem .alt { 
      background: #fcfcfc; } 
  #GridViewItem .pgr {background: #363670; } 
  #GridViewItem .pgr table { margin: 3px 0; } 
  #GridViewItem .pgr td { border-width: 0; padding: 0 6px; border-left: solid 1px #666; font-weight: bold; color: #fff; line-height: 12px; }   
  #GridViewItem .pgr a { color: Gray; text-decoration: none; } 
  #GridViewItem .pgr a:hover { color: #000; text-decoration: none; }
 </style>
</div>

</asp:Content> 