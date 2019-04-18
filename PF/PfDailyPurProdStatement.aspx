<%@ Page Title="Daily Purchase, Production & Sales Form & List - Plastic Factory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="PfDailyPurProdStatement.aspx.cs" Inherits="NRCAPPS.PF.PfDailyPurProdStatement" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper"> 

    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Daily Purchase, Production & Sales At a Glance
        <small>: - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Plastic Factory</a></li>
        <li class="active">Daily Purchase, Production & Sales</li>
      </ol>
    </section>
    
    <!-- Main content -->
    <section class="content">
      <div class="row"> 
            <div class="col-md-12"> 
         <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title"> Production Statement (Daily) Parameter</h3>
            </div>
            <!-- /.box-header -->

         <div class="box-body">
            <!-- form start -->    
                   <div class="form-group">
                    <label class="col-sm-2 control-label">Select Date</label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="EntryDate3"  runat="server" ></asp:TextBox>  
                    </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator24" runat="server" 
                         ControlToValidate="EntryDate3" ErrorMessage="Insert Date" 
                         SetFocusOnError="True" Display="Dynamic" ValidationGroup='valGroup6'></asp:RequiredFieldValidator> 
                  </div>
                  </div>
                </div> 
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton8" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">     
                    <asp:LinkButton ID="LinkButton9" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReportPurProd_Click"  ValidationGroup='valGroup6' ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                   </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
           </div>   
           </div>   

      <!-- /.row -->
    </section>
    <!-- /.content -->
  <asp:Panel ID="PanelPrint" runat="server" ></asp:Panel>  
</div>

</asp:Content> 