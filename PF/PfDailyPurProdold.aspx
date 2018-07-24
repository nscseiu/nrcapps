<%@ Page Title="Daily Purchase &  Production Form & List - Plastic Factory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="PfDailyPurProdold.aspx.cs" Inherits="NRCAPPS.PF.PfDailyPurProdold" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Daily Purchase &  Production List
        <small>Customer: - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Plastic Fctory</a></li>
        <li class="active">Daily Purchase &  Production</li>
      </ol>
    </section>
    
    <!-- Main content -->
    <section class="content">
      <div class="row">
        <!-- left column --> 
                 <div class="col-md-6">  
          <!-- Horizontal Form --> 
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Daily Purchase List (By Default Today Date)</h3>
              <div class="box-tools"> 
              <div class="input-group input-group-sm" style="width: 295px;">
                 
                  <div class="col-sm-5">   
                 
                    <asp:DropDownList ID="DropDownPurchaseTypeID" class="form-control input-sm"   ValidationGroup='valGroup' runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="DropDownPurchaseTypeID" Display="Dynamic" ErrorMessage="Select Purchase Type" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
               
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control input-sm pull-right" ID="AsOnDate"   ValidationGroup='valGroup' runat="server" ></asp:TextBox>  
                    </div> 
                 <div class="input-group-btn">
                      <asp:Button ID="Button1" Class="btn btn-info" Text="Search" runat="server" OnClick="GridViewSearchPur"  ValidationGroup='valGroup' CausesValidation="False" />
                  </div>  
              </div>    
            </div>
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView1" runat="server" EnablePersistedSelection="false"  
    SelectedRowStyle-BackColor="Yellow"  AutoGenerateColumns="false" ShowHeader="true"  ShowFooter="true" CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="ITEM_NAME_FULL" HeaderText="Item Full Name" /> 
                     <asp:BoundField DataField="ITEM_CODE" HeaderText="Item Code" />  
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Weight - MT"  DataFormatString="{0:0.000}" ItemStyle-HorizontalAlign="Right"/>   
                     <asp:BoundField DataField="ITEM_AMOUNT"  HeaderText="Amount"  DataFormatString="{0:0.00}" ItemStyle-HorizontalAlign="Right"/> 
                     <asp:BoundField DataField="ITEM_AVG_RATE"  HeaderText="Average Rate"  DataFormatString="{0:0.00}" ItemStyle-HorizontalAlign="Right" />      
                     
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                    </asp:GridView>  
        </div>
       </div> 
    </div> 
        <!--/.col (left) -->
        <!-- right column -->
        <div class="col-md-6">  
          <!-- Horizontal Form --> 
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Daily Production List (By Default Today Date)</h3>
              <div class="box-tools"> 
              <div class="input-group input-group-sm" style="width: 200px;">
               
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control input-sm pull-right" ID="EntryDate" ValidationGroup='valGroup1' runat="server" ></asp:TextBox>  
                    </div> 
                 <div class="input-group-btn">
                      <asp:Button ID="ButtonSearchUser" Class="btn btn-info"  Text="Search" runat="server" OnClick="GridViewSearchProd"  ValidationGroup='valGroup1' CausesValidation="False" />
                  </div>  
              </div>    
            </div>
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView2" runat="server" EnablePersistedSelection="false"  onrowdatabound="GridView2_RowDataBound"  onrowcreated="GridView2_RowCreated"    
    SelectedRowStyle-BackColor="Yellow"  AutoGenerateColumns="false"   ShowFooter="true" CssClass="table table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="SHIFT_ID" HeaderText="Shift ID" visible="false" /> 
                     <asp:BoundField DataField="SHIFT_NAME" HeaderText="Shift Name" />  
                     <asp:BoundField DataField="MACHINE_NUMBER"  HeaderText="Machine No" />   
                     <asp:BoundField DataField="ITEM_NAME_FULL"  HeaderText="Item Full Name" /> 
                     <asp:BoundField DataField="ITEM_CODE"  HeaderText="Item Code" />      
                   

                   	  <asp:TemplateField HeaderText="Weight - MT" ItemStyle-HorizontalAlign="Right" >
			             <ItemTemplate>
				            <asp:Label ID="lblqty" runat="server" Text='<%# Eval("ITEM_WEIGHT_IN_FG","{0:n3}") %>' />
			             </ItemTemplate>
			             <FooterTemplate>
				            <div style="text-align: right;">
				            <asp:Label ID="lblTotalqty" runat="server"  Font-Bold=true   />
				            </div>
			             </FooterTemplate>
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