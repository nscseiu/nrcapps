<%@ Page Title="Change Your Password - NRC Application" Language="C#"  AutoEventWireup="true"  CodeBehind="NrcUserChangePassword.aspx.cs" Inherits="NRCAPPS.NRC.NrcUserChangePassword" %> 
     
 <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head id="Head1" runat="server">
  <meta charset="utf-8">
  <meta http-equiv="X-UA-Compatible" content="IE=edge">
  <title>Nesma Recycling Web Application | Log in</title>
  <link rel='shortcut icon' href='favicon.ico' />
  <!-- Tell the browser to be responsive to screen width -->
  <meta content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" name="viewport">
  <!-- Bootstrap 3.3.6 -->
  <link rel="stylesheet" href="../../bootstrap/css/bootstrap.min.css">
  <!-- Font Awesome -->
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.5.0/css/font-awesome.min.css">
  <!-- Ionicons -->
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/ionicons/2.0.1/css/ionicons.min.css">
  <!-- Theme style -->
  <link rel="stylesheet" href="../../dist/css/AdminLTE.min.css">
  <!-- iCheck -->
  <link rel="stylesheet" href="../../plugins/iCheck/square/blue.css">

  <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
  <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
  <!--[if lt IE 9]>
  <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
  <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
  <![endif]-->
</head>    <!-- Content Wrapper. Contains page content -->


<body class="hold-transition register-page">
<form id="form1" runat="server">
<div class="register-box">
   
  <div class="login-logo">
    <a href="<%= ResolveUrl("~/Default.aspx?") %>"><img src="../../image/logo.png" /></a><br />
     <b>NRC</b> Management Application 
  </div>
  <div class="register-box-body">
   <asp:Panel  id="alert_box" runat="server">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <h4><i class="icon fa fa-check"></i> Alert!</h4>  
           </asp:Panel> 
    <p class="login-box-msg">Change your password</p>

     <div class="form-group has-feedback">
                   
                    <asp:TextBox ID="TextPassword"  class="form-control input-sm" placeholder="Password" runat="server"  onmousedown="this.type='text'" onmouseup="this.type='password'" onmousemove="this.type='password'" ></asp:TextBox> 
                       <span class="glyphicon glyphicon-eye-open form-control-feedback"></span>
          
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="TextPassword" Display="Dynamic" 
                          ErrorMessage="Insert password" SetFocusOnError="True"></asp:RequiredFieldValidator>
                      <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                          ControlToValidate="TextPassword" Display="Dynamic" 
                          ErrorMessage="Password must have min 6 digit and max 12 digit" 
                          ValidationExpression="^[a-zA-Z0-9'@&amp;#.\s]{6,12}$" 
                          SetFocusOnError="True"></asp:RegularExpressionValidator>
                     
                </div> 
                  
                <div class="form-group  has-feedback">
                  
                  <asp:TextBox ID="TextPasswordConfirm"  TextMode="Password" placeholder="Confirm Password" class="form-control input-sm" runat="server"  ></asp:TextBox> 
                  <span class="glyphicon glyphicon-log-in form-control-feedback"></span>
                   <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextPasswordConfirm" Display="Dynamic" 
                          ErrorMessage="Insert Confirm password" SetFocusOnError="True"></asp:RequiredFieldValidator>
                 <asp:CompareValidator ID="CompareValidator1" runat="server"    ControlToValidate="TextPasswordConfirm"  CssClass="ValidationError"  ControlToCompare="TextPassword"
                          ErrorMessage="No Match (Password)" Display="Dynamic" 
                                        ToolTip="Password must be the same" />  
                      <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                          ControlToValidate="TextPasswordConfirm" Display="Dynamic" 
                          ErrorMessage="Password must have min 6 digit and max 12 digit" 
                          ValidationExpression="^[a-zA-Z0-9'@&amp;#.\s]{6,12}$" 
                          SetFocusOnError="True"></asp:RegularExpressionValidator>
                     
                </div> 

      
      <div class="row">
        <div class="col-xs-8">
          <div class="checkbox icheck">
            <label>
               
            </label>
          </div>
        </div>
        <!-- /.col -->
        <div class="col-xs-4">  
          <asp:LinkButton ID="LinkButton1" class="btn btn-primary btn-block btn-flat" runat="server" Text="Update"  onclick="BtnUpdate_Click"><span class="fa fa-edit"></span> Submit</asp:LinkButton>
                     
        </div>
        <!-- /.col -->
      </div>
     
  </div>
  <!-- /.form-box -->
</div>
<!-- /.register-box -->
  </form>
<!-- jQuery 2.2.3 -->
<script src="../../plugins/jQuery/jquery-2.2.3.min.js"></script>
<!-- Bootstrap 3.3.6 -->
<script src="../../bootstrap/js/bootstrap.min.js"></script>
<!-- iCheck -->
<script src="../../plugins/iCheck/icheck.min.js"></script>
<script>
    $(function () {
        $('input').iCheck({
            checkboxClass: 'icheckbox_square-blue',
            radioClass: 'iradio_square-blue',
            increaseArea: '20%' // optional
        });
    });
</script>
</body>
       
   

 