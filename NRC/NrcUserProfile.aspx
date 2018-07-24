<%@ Page Title="User Form & List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="NrcUserProfile.aspx.cs" Inherits="NRCAPPS.NRC.NrcUserProfile" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
 <div class="content-wrapper">
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        User Profile
      </h1>
      <ol class="breadcrumb">
        <li><a href="#"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">User Setting</a></li>
        <li class="active">User profile</li>
      </ol>
    </section>

    <!-- Main content -->
    <section class="content">

      <div class="row">
        <div class="col-md-3">
         
          <!-- Profile Image -->
          <div class="box box-primary">
            <% for (var data = 0; data < TableData.Rows.Count; data++)
                {  %>  
            <div class="box-body box-profile"> 
            <asp:Image class="profile-user-img img-responsive img-circle" ID="ImageProfile" style="border:3px solid #d2d6de" Runat="server" />   

              <h3 class="profile-username text-center"><%=Session["EMP_NAME_TWO"]%></h3>

              <p class="text-muted text-center"><%=Session["NRC_USER_ROLE_NAME"]%></p>

              <ul class="list-group list-group-unbordered">
                <li class="list-group-item">
                  <b>Department</b> <a class="pull-right"><%=TableData.Rows[data]["DEPARTMENT_NAME"]%></a>
                </li>
                <li class="list-group-item">
                  <b>Division</b> <a class="pull-right"><%=TableData.Rows[data]["DIVISION_NAME"]%></a>
                </li>
                <li class="list-group-item">
                  <b>Location</b> <a class="pull-right"><%=TableData.Rows[data]["LOCATION_NAME"]%></a>
                </li>
              </ul>

              <a href="<%= ResolveUrl("~/NRC/NrcUserChangePassword.aspx") %>" class="btn btn-primary btn-block"><b>Change Password</b></a>
            </div>
           
            <!-- /.box-body -->
          </div>
          <!-- /.box -->

          <!-- About Me Box -->
          <!-- div class="box box-primary">
            <div class="box-header with-border">
              <h3 class="box-title">About Me</h3>
            </div>
            <!-- /.box-header -->
            <!--div class="box-body">
              <strong><i class="fa fa-book margin-r-5"></i> Education</strong>

              <p class="text-muted">
                B.S. in Computer Science from the University of Tennessee at Knoxville
              </p>

              <hr>

              <strong><i class="fa fa-map-marker margin-r-5"></i> Location</strong>

              <p class="text-muted">Malibu, California</p>

              <hr>

              <strong><i class="fa fa-pencil margin-r-5"></i> Skills</strong>

              <p>
                <span class="label label-danger">UI Design</span>
                <span class="label label-success">Coding</span>
                <span class="label label-info">Javascript</span>
                <span class="label label-warning">PHP</span>
                <span class="label label-primary">Node.js</span>
              </p>

              <hr>

              <strong><i class="fa fa-file-text-o margin-r-5"></i> Notes</strong>

              <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam fermentum enim neque.</p>
            </div>
            <!-- /.box-body -->
          <!-- /div -->
          <!-- /.box -->
        </div>
        <!-- /.col -->
        <div class="col-md-9">
          <asp:Panel  id="alert_box" runat="server">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <h4><i class="icon fa fa-check"></i> Alert!</h4>  
           </asp:Panel> 
          <div class="nav-tabs-custom">
            <ul class="nav nav-tabs">
              <li class="active"><a href="#activity" data-toggle="tab">User Details</a></li> 
              <li><a href="#settings" data-toggle="tab">Picture Settings</a></li>
            </ul>
            <div class="tab-content">
              <div class="active tab-pane" id="activity">
                  <div class="box">
            <div class="box-header">
              
            </div>
            <!-- /.box-header -->
            <div class="box-body  no-padding">
              <table class="table table-striped">
                <tr>
                  <th style="width: 200px">Subject</th>
                  <th>Details</th> 
                </tr>
                <tr>
                  <td>Employee ID</td>
                  <td><%=TableData.Rows[data]["EMP_ID"]%></td> 
                </tr>
                  <td>Name</td>
                  <td><%=TableData.Rows[data]["EMP_FNAME"]%>  <%=TableData.Rows[data]["EMP_LNAME"]%></td> 
                </tr>
                <tr>
                  <td>Email</td>
                  <td><%=TableData.Rows[data]["EMAIL"]%></td> 
                </tr>
                 <tr>
                  <td>User Create Date</td>
                  <td><%=TableData.Rows[data]["CREATE_DATE"]%></td> 
                </tr>
                
              </table>
            </div>
            <!-- /.box-body -->
          </div>
              </div>
              <!-- /.tab-pane -->
             <% } %>  
              <!-- /.tab-pane -->

              <div class="tab-pane" id="settings"> 
                  <div class="form-group">
                    <label for="inputSkills" class="col-sm-2 control-label">Choose your profile picture:</label>

                    <div class="col-sm-10">
                          <asp:FileUpload ID="FileUpload1" runat="server" />     
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                         ControlToValidate="FileUpload1" ErrorMessage="Insert your profile picture" 
                         SetFocusOnError="True" Display="Dynamic"></asp:RequiredFieldValidator>  <br />Image Dimensions: 128px X 128px 
                    </div>
              
                  </div>
                   
                  <div class="form-group">
                    <div class="col-sm-offset-2 col-sm-10">
                        <asp:Button ID="BtnUpload" runat="server" Text="Upload" OnClick="Upload" CssClass="btn btn-danger" /> 
                    </div>
                  </div> 
              </div>
              <!-- /.tab-pane -->
            </div>
            <!-- /.tab-content -->
          </div>
          <!-- /.nav-tabs-custom -->
        </div>
        <!-- /.col -->
      </div>
      <!-- /.row -->

    </section>
    <!-- /.content -->
  </div>
    
</asp:Content> 