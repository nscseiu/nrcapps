﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WpPurchaseSupplierWiseReportView.aspx.cs" Inherits="NRCAPPS.WP.WP_Reports.WpPurchaseSupplierWiseReportView" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
   <form id="Form1" runat="server">
        <asp:Panel ID="pnlReport" runat="server">  
             <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
                 AutoDataBind="true" onunload="CrystalReportViewer1_Unload" 
                 ToolPanelView="None" />
        </asp:Panel> 
   </form>  
</body>
</html>
