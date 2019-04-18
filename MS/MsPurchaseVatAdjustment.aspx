<%@ Page Title="Purchase Vat Adjustment Form & List - Waste Paper" EnableEventValidation="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="MsPurchaseVatAdjustment.aspx.cs" Inherits="NRCAPPS.MS.MsPurchaseVatAdjustment" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Purchase Vat Adjustment Form & List
        <small>Purchase Vat Adjustment: - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Waste Paper</a></li>
        <li class="active">Purchase Vat Adjustment</li>
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
              <h3 class="box-title">Purchase Vat Adjustment Form</h3><div class="box-tools">
               </div>
            </div>
                 
            <!-- /.box-header -->
            <!-- form start -->
            
              <div class="box-body"> 
               <div class="form-group">
                  <label  class="col-sm-3 control-label">Supplier Name</label> 
                  <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownSupplierID" class="form-control select2 input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownSupplierID" Display="Dynamic" 
                          ErrorMessage="Select Supplier" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div> 
                 
                </div>  
               <div class="form-group">
                  <label  class="col-sm-3 control-label">Vat Percent</label>
                   <div class="col-sm-2"> 
                       <asp:RadioButtonList ID="RadioBtnVatWp" runat="server"  RepeatDirection="Horizontal">
                             <asp:ListItem Value="VatNo" Selected="True">No &nbsp;</asp:ListItem>
                             <asp:ListItem Value="VatYes">Yes </asp:ListItem>  
                        </asp:RadioButtonList>  
                    </div> 
                 <div id="VatPercentBox" style="display:none;"  runat="server">
                  <div class="col-sm-2" >   
                   <div class="input-group">  
                    <asp:DropDownList ID="DropDownVatID"  class="form-control input-sm" runat="server" >   
                    </asp:DropDownList>  
                      <span class="input-group-addon">%</span>  
                      </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server" 
                          ControlToValidate="DropDownVatID" Display="Dynamic" 
                          ErrorMessage="Select Vat percent (%)" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator> 
                     
                  </div>    
                  </div>
                </div>   
                 
                 <div class="form-group">
                    <label class="col-sm-3 control-label">Select Month</label>
                     <div class="col-sm-3">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="TextMonthYear0"  runat="server"   ></asp:TextBox> <!-- AutoPostBack="True"  ontextchanged="TextCheckDataProcess" -->
                     </div>  <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                          ControlToValidate="TextMonthYear0" ErrorMessage="Insert Entry Date" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                      </div>   
                  </div>  
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-3" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" 
                          OnClick="clearTextField" CausesValidation="False" TabIndex="1"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">    
                    <asp:LinkButton ID="BtnUpdate" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdate_Click"><span class="fa fa-edit"></span> Update</asp:LinkButton>
                    </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>     </div> 
         
      <div class="col-md-12">        
         
          <div class="box box-sucsses">
            <div class="box-header with-border">
              <h3 class="box-title"> Purchase Statement (Daily Collection Supplier - Month Wise) Parameter</h3>
            </div>
            <!-- /.box-header -->

              <div class="box-body">
            <!-- form start -->   
              <div class="form-group">
                  <label  class="col-sm-2 control-label">Supplier Name</label> 
                  <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownSupplierID2" class="form-control select2 input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="DropDownSupplierID2" Display="Dynamic" 
                          ErrorMessage="Select Supplier" InitialValue="0" SetFocusOnError="True"  ValidationGroup='valGroup2'></asp:RequiredFieldValidator>
                  </div>
                </div>
                   <div class="form-group">
                    <label class="col-sm-2 control-label">Select Date </label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="EntryDate1"  runat="server" ></asp:TextBox>  
                    </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                         ControlToValidate="EntryDate1" ErrorMessage="Insert Start Date" 
                         SetFocusOnError="True" Display="Dynamic" ValidationGroup='valGroup2'></asp:RequiredFieldValidator> 
                  </div>
                        <label class="col-sm-1 control-label">To</label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="EntryDate2"  runat="server" ></asp:TextBox>  
                    </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                         ControlToValidate="EntryDate2" ErrorMessage="Insert End Date" 
                         SetFocusOnError="True" Display="Dynamic" ValidationGroup='valGroup2'></asp:RequiredFieldValidator> 
                  </div>

                  </div>
                  <div class="form-group">
                  <label  class="col-sm-2 control-label">Select Item</label> 
                  <div class="col-sm-5">   
                    <asp:ListBox ID="DropDownItemList" class="form-control select2 input-sm" SelectionMode="multiple" runat="server"> 
                    </asp:ListBox>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator23" runat="server" 
                          ControlToValidate="DropDownItemList" Display="Dynamic" 
                          ErrorMessage="Select Item" SetFocusOnError="True"  ValidationGroup='valGroup2'></asp:RequiredFieldValidator>
                  </div>
                </div>
                </div> 
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton1" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">     
                    <asp:LinkButton ID="BtnReport" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport_Click"  ValidationGroup='valGroup2' ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                   </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
        
            <asp:Panel ID="Panel1" runat="server">  
            <%if (IsLoad)
                {   string ItemIDList ="";
                    for(int i=0;i<DropDownItemList.Items.Count;++i)
                    {
                        if(DropDownItemList.Items[i].Selected==true) { 
                        ItemIDList += DropDownItemList.Items[i].Value + "-";
                        }
                    }
               %> 
            <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title"> Purchase Statement (Daily Collection Supplier - Month Wise) Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="WP_Reports/WpPurchaseSupplierWiseReportView.aspx?StartDate=<%=EntryDate1.Text %>&EndDate=<%=EntryDate2.Text %>&SupplierID=<%=DropDownSupplierID2.Text %>&DropDownItemID=<%=ItemIDList%>" width="950px" id="iframe1"
                        marginheight="0" frameborder="0" scrolling="auto" height="1250px">   </iframe>  
                         </div>
                       </div> 
                    
                     <%} %>   
            </asp:Panel>  
           
        
          <!-- /.box --> 
        <!--/.col (right) -->
      </div> 
      <!-- /.row -->
    </section>
    <!-- /.content -->
         

  
   <asp:Panel ID="PanelPrint" runat="server" ></asp:Panel>  
</div> 
</asp:Content> 