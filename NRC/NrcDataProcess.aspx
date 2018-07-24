<%@ Page Title="Data Process" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="NrcDataProcess.aspx.cs" Inherits="NRCAPPS.PF.NrcDataProcess" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Data Process 
        <small>Data Process: - Update - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Apps & Data Settings</a></li>
        <li class="active">Data Process</li>
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
              <h3 class="box-title">Data Process (Plastic Factory - Monthly)</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body"> 
              <div class="form-group">
                    <label class="col-sm-3 control-label">Select Month</label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="TextMonthYear0"  runat="server" AutoPostBack="True" ontextchanged="CheckDataProcess_PF_Monthly" ></asp:TextBox>  
                    </div>
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                          ControlToValidate="TextMonthYear0" ErrorMessage="Insert Month Year" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                      </div>   <div class="col-sm-3"><asp:Label ID="CheckMonthYear" runat="server"></asp:Label> 
                  </div>
                  </div> 
                  </div> 
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-3" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">   
                     <asp:LinkButton ID="BtnDataProcPf" class="btn btn-warning" runat="server" Text="Add New" onclick="BtnDataProcPf_Click"><span class="fa fa-refresh"></span> Data Process</asp:LinkButton>  
                  </div>
                </div>
              </div>
             
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
       
    </div> 
          <!-- /.box --> 
        <!--/.col (right) -->
      </div>
      <!-- /.row -->
    </section>
    <!-- /.content -->
 
</div>
<style type="text/css"> 
    tr{ 
     border-top: 2px dotted #00c0ef !important;
    } 
 </style>
</asp:Content> 