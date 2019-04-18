<%@ Page Title="Inventory & Valuation Statement - Waste Paper" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="WpRmStatement.aspx.cs" Inherits="NRCAPPS.WP.WpRmStatement" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Inventory & Valuation Statement
        <small>Inventory & Valuation Statement: - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Waste Paper</a></li>
        <li class="active">Inventory & Valuation Statement</li>
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
              <h3 class="box-title">Inventory Statement Parameter</h3>
            </div>
            <!-- /.box-header -->

              <div class="box-body">
            <!-- form start -->  
               <div class="form-group">
                    <label class="col-sm-2 control-label">Select Date (As On)</label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control input-sm pull-right" ID="EntryDate" runat="server"></asp:TextBox>  
                             
                    </div> <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                         ControlToValidate="EntryDate" ErrorMessage="Insert Date" 
                         SetFocusOnError="True" Display="Dynamic" ValidationGroup="Vgroup1" ></asp:RequiredFieldValidator> 
                      </div>  
                  </div>
                </div> 
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">     
                    <asp:LinkButton ID="BtnReport" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport1_Click"  ValidationGroup="Vgroup1" ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                   </div>
                </div>
              </div>
              <!-- /.box-footer -->
           </div> 
        
            <asp:Panel ID="Panel1" runat="server">  
            <%if (IsLoad1)
                {%> 
            <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title"> Inventory Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="WP_Reports/WpInventoryValuationStatementReportView.aspx?AsOnDate=<%=EntryDate.Text %>&IsReport=1" width="1450px" height="950px" id="iframe1"
                        marginheight="0" frameborder="0" scrolling="auto" >   </iframe>  
                         </div>
                       </div> 
                    
                <%} %>   
            </asp:Panel>  


          <!-- Horizontal Form -->
          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Inventory & Valuation Statement Parameter</h3>
            </div>
            <!-- /.box-header -->

              <div class="box-body">
            <!-- form start -->  
               <div class="form-group">
                    <label class="col-sm-2 control-label">Select Date (As On)</label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control input-sm pull-right" ID="AsOnDate" runat="server"></asp:TextBox>  
                             
                    </div> <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                         ControlToValidate="AsOnDate" ErrorMessage="Insert Date" 
                         SetFocusOnError="True" Display="Dynamic"></asp:RequiredFieldValidator> 
                      </div>  
                  </div>
                </div> 
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton1" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">     
                    <asp:LinkButton ID="LinkButton2" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport_Click"  ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                   </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
        
            <asp:Panel ID="Panel3" runat="server">  
            <%if (IsLoad)
                {%> 
            <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title"> Inventory & Valuation Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="WP_Reports/WpInventoryValuationStatementReportView.aspx?AsOnDate=<%=AsOnDate.Text %>&IsReport=2" width="1450px" height="950px" id="iframe1"
                        marginheight="0" frameborder="0" scrolling="auto" >   </iframe>  
                         </div>
                       </div> 
                     
                <%} %>   
            </asp:Panel>  
          <!-- /.box --> 
        <!--/.col (right) -->
            
          <!-- Horizontal Form -->
          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Stock Position - Parameter</h3>
            </div>
            <!-- /.box-header -->

              <div class="box-body">
            <!-- form start -->  
               <div class="form-group">
                    <label class="col-sm-2 control-label">Select Date (As On)</label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control input-sm pull-right" ID="EntryDate2" runat="server"></asp:TextBox>  
                             
                    </div> <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                         ControlToValidate="EntryDate2" ErrorMessage="Insert Date" 
                         SetFocusOnError="True" Display="Dynamic" ValidationGroup="Vgroup2" ></asp:RequiredFieldValidator> 
                      </div>  
                  </div>
                </div> 
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton3" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">     
                    <asp:LinkButton ID="LinkButton4" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport2_Click" ValidationGroup="Vgroup2"  ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                   </div>
                </div>
                   <asp:Panel ID="PanelPrint" runat="server" ></asp:Panel>  
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>  
      <!-- /.row -->
    </section>
    <!-- /.content -->
  </div> 
</div>

</asp:Content> 