<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NRCAPPS._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head id="Head1" runat="server">
  <meta charset="utf-8">
  <meta http-equiv="X-UA-Compatible" content="IE=edge">
  <title>Nesma Recycling Company Application | Log in</title>
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
</head>
<body class="hold-transition login-page">
<div class="login-box">
  <div class="login-logo">
    <img src="image/logo.png" /><br />
     <b>NRC</b> Application 
  </div>
  <!-- /.login-logo --> 
  <div class="login-box-body">
    <p class="login-box-msg">Sign in to start your session</p> 
    <form id="form1" runat="server"> 

      <div class="form-group has-feedback">

      <asp:TextBox ID="TextUserName" class="form-control" placeholder="User Name" runat="server" ></asp:TextBox>
      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
              ControlToValidate="TextUserName"   ErrorMessage="Please Enter your User Name" 
              CssClass="control-label" Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>
      <asp:Label ID="LabelUserName" runat="server" Text=""></asp:Label> 
        <span class="glyphicon glyphicon-envelope form-control-feedback"></span>
      </div>
      <div class="form-group has-feedback"> 
                <asp:TextBox ID="TextPassword" TextMode="Password" class="form-control" placeholder="Password"  runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                    ControlToValidate="TextPassword" ErrorMessage="Please Enter your Password" 
                    Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>
                    <asp:Label ID="LabelPassword" runat="server" Text=""></asp:Label> 
        <span class="glyphicon glyphicon-lock form-control-feedback"></span>
           
      </div>
      <div class="row">
        <div class="col-xs-8">
          <div class="checkbox icheck">
            <label>
              <input type="checkbox" /> Remember Me
            </label>
          </div>
        </div>
        <!-- /.col -->
        <div class="col-xs-4">
         <asp:Button ID="ButtonLogin" runat="server" onclick="Button_Click_Login"  type="submit" class="btn btn-primary btn-block btn-flat"  Text="Login" />
          
        </div>
        <!-- /.col -->
      </div>
    </form>

     
    <!-- /.social-auth-links -->

    <a href="#">I forgot my password</a><br> 

  </div>
  <!-- /.login-box-body -->
</div>
<!-- /.login-box -->

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
</html>
    
 

 
