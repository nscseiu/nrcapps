﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="NRCAPPS.Welcome" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 680px">
      <asp:Label ID="lb1" runat="server" Text="Label"></asp:Label>
      <br /> <br />
      <a href="HR/hr_user.aspx">Add User</a>
    </div>
     <br /> <br /> <br />
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Logout" />
    </form>
</body>
</html>
