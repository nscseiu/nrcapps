<%@ Page Title="Daily Purchase, Production & Sales Form & List - Plastic Factory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="PfDailyPurProd.aspx.cs" Inherits="NRCAPPS.PF.PfDailyPurProd" %>
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
       <div class="col-md-4"> 
             
          <!-- Horizontal Form -->
          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Search Parameter (By Default is Today Date)</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
                <div class="form-group" style="display:none;">
                  <label  class="col-sm-5 control-label">Purchase Type</label> 
                  <div class="col-sm-4">   
                       <asp:DropDownList ID="DropDownPurchaseTypeID" class="form-control input-sm"   ValidationGroup='valGroup' runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="DropDownPurchaseTypeID" Display="Dynamic" ErrorMessage="Select Purchase Type" SetFocusOnError="True"></asp:RequiredFieldValidator>
               
                  </div> 
                </div>  
                <div class="form-group">
                  <label  class="col-sm-5 control-label">As On Date</label> 
                  <div class="col-sm-4">  
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control input-sm pull-right" ID="AsOnDate"   ValidationGroup='valGroup' runat="server" ></asp:TextBox>  
                    </div> 
                       
                  </div> 
                   
                </div>  
               
                 <!-- checkbox --> 
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-5" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">    
                     <asp:Button ID="Button1" Class="btn btn-info" Text="Search" runat="server" OnClick="GridViewSearchPur"  ValidationGroup='valGroup' CausesValidation="False" />
                 </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
         
        </div>  
       
          <div class="box box-warning"> 
          <% for (var data = 0; data < TableData3.Rows.Count; data++)  { 
                 
                 double final_stock = Convert.ToDouble(TableData3.Rows[data]["FINAL_STOCK_WT"]);
                 double mat_req = Convert.ToDouble(TableData3.Rows[data]["MAT_REQ"]);
                  %>   
                        <table    class="table  table-sm table-bordered" > 
                         <thead>
                                <tr> 
                                <th colspan="2" style="text-align: center">
                                 
                                <% if (final_stock <= mat_req)
                                   { %>   

                                        <div class="alert alert-danger alert-dismissible">
                                        <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                                        <h4><i class="icon fa fa-warning"></i> Alert!</h4>
                                       Raw Material Required for <%=TableData3.Rows[data]["DAYS"]%> Days is <%=TableData3.Rows[data]["MAT_REQ"]%> (MT) 
                                        </div> 
                                <%} else { %>  
                                    <div class="alert alert-success alert-dismissible">
                                        <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                                        <h4><i class="icon fa fa-check"></i> Alert!</h4>
                                       Raw Material Required for <%=TableData3.Rows[data]["DAYS"]%> Days is <%=TableData3.Rows[data]["MAT_REQ"]%> (MT) 
                                        </div>  
                                 <%}%>  
                                </th>
                            </tr>   
                           </thead>
                            <tbody>
                           
                            <tr>
                               <th> Raw Material Final Inventory: </th>
                                <th style="text-align: right"><%=decimal.Parse(TableData3.Rows[data]["FINAL_STOCK_WT"].ToString()).ToString("0.000")%> </th>
                            </tr>
                             <tr>
                                <th>Material Required Per Day (in MT)</th> 
                                <th style="text-align: right">25.000 </th>
                             </tr> 
                            <tr> 
                                <th colspan="2" style="text-align: center">Raw Material Available for <%=TableData3.Rows[data]["AVAIL_DAYS"]%> Days Only.</th>
                            </tr>  
                            
                            </tbody>
                        </table> 
                   
                <%}%> 
           
        </div>  
    </div> 
      <div class="col-md-4">  
      <div class="box box-warning">
            <div class="box-header with-border">
              <h3 class="box-title">Daily Purchase, Production & Sales (Bar Charts)</h3>
              <div class="box-tools"> 
               
            </div>
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
       <asp:PlaceHolder ID = "PlaceHolderGraphReport" runat="server" />  
      <!-- /.row --> 
       
       <script type="text/javascript">
           Highcharts.chart('container', {
               data: {
                   table: 'datatable'
               },
               chart: {
                   type: 'column'
               },
               title: {
                   text: ''
               },
               yAxis: {
                   allowDecimals: false,
                   title: {
                       text: 'Weight (MT)'
                   }
               },
               tooltip: {
                   formatter: function () {
                       return '<b>' + this.series.name + '</b><br/>' +
                this.point.y + ' ' + this.point.name.toLowerCase();
                   }
               }
           }); 
		</script>
              
            </div>
            </div> 
     </div>
       <div class="col-md-4">  
      <div class="box box-warning">
            <div class="box-header with-border">
              <h3 class="box-title">Monthly Production Achive Target (Pie Chart)</h3>
              <div class="box-tools"> 
               
            </div>
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive">  
            <div id='container_achive' style=' margin: 10px' ></div>
      <!-- /.row --> 
       
     
              
            </div>
            </div> 
     </div>
        <!-- left column --> 
      <div class="col-md-4">  
          <!-- Horizontal Form --> 
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Daily Purchase List</h3>
              <div class="box-tools"> 
               
            </div>
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView1" runat="server" EnablePersistedSelection="false"  
    SelectedRowStyle-BackColor="Yellow"  AutoGenerateColumns="false" ShowHeader="true"  ShowFooter="true" CssClass="table  table-sm table-bordered table-striped" >
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
        <div class="col-md-4">  
          <!-- Horizontal Form --> 
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Daily Production List</h3>
              
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView2" runat="server" EnablePersistedSelection="false"  onrowdatabound="GridView2_RowDataBound"  onrowcreated="GridView2_RowCreated"    
    SelectedRowStyle-BackColor="Yellow"  AutoGenerateColumns="false"   ShowFooter="true" CssClass="table table-sm table-bordered table-striped" >
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
          <!-- left column --> 

   <!-- left column --> 
      <div class="col-md-4">  
          <!-- Horizontal Form --> 
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Monthly Production Achive Target</h3>
              <div class="box-tools"> 
               
            </div>
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
              <% for (var data = 0; data < TableData2.Rows.Count; data++)  {  %>   
                        <table id="datatable_achive" style="display:none;" class="table  table-sm table-bordered table-striped" >
                           <thead>
                               <th>Title</th>
                               <th style="text-align: right">Monthly Production Achive Target</th>
                           </thead>
                            <tbody>
                            <tr>
                                <th>Total Production (MT)</th>
                                <td style="text-align: right"><%=decimal.Parse(TableData2.Rows[data]["ITEM_WEIGHT_IN_FG"].ToString()).ToString("0.000")%> </td>
                            </tr>
                             <tr>
                                <th>Monthly Target (MT)</th> 
                                <td style="text-align: right"><%=decimal.Parse(TableData2.Rows[data]["ITEM_WEIGHT_PROD"].ToString()).ToString("0.000")%> </td>
                             </tr> 
                            <tr>
                                <th>Below than Target (MT)</th>
                                <td style="text-align: right"><%=decimal.Parse(TableData2.Rows[data]["BELOW_TARGET"].ToString()).ToString("0.000")%> </td>
                            </tr>  
                             
                            </tbody>
                        </table> 
                   
                <%}%> 
                 <% for (var data = 0; data < TableData2.Rows.Count; data++)  {  %>   
                        <table class="table  table-sm table-bordered table-striped" >
                           <thead>
                               <th>Title</th>
                               <th style="text-align: right">Weight (MT)</th>
                           </thead>
                            <tbody>
                            <tr>
                                <th>Total Production</th>
                                <td style="text-align: right"><%=decimal.Parse(TableData2.Rows[data]["ITEM_WEIGHT_IN_FG"].ToString()).ToString("0.000")%> </td>
                            </tr>
                             <tr>
                                <th>Monthly Target (in MT)</th> 
                                <td style="text-align: right"><%=decimal.Parse(TableData2.Rows[data]["ITEM_WEIGHT_PROD"].ToString()).ToString("0.000")%> </td>
                             </tr> 
                            <tr>
                                <th>Below than Target (in MT)</th>
                                <td style="text-align: right"><%=decimal.Parse(TableData2.Rows[data]["BELOW_TARGET"].ToString()).ToString("0.000")%> </td>
                            </tr>  
                             <tr>
                                <th>Require Production Per Day To Achieve Target <br />[Days Left in This Months: <span class="bg-green">&nbsp; <%=TableData2.Rows[data]["DAYS"]%> &nbsp;</span> ]</th> 
                                <td style="text-align: right"><%=decimal.Parse(TableData2.Rows[data]["PER_DAY_ACHIV_TAR"].ToString()).ToString("0.000")%> </td> 
                            </tr> 
                            </tbody>
                        </table> 
                   
                <%}%> 
                  <script type="text/javascript">
                      Highcharts.chart('container_achive', {
                          data: {
                              table: 'datatable_achive'
                          },
                          chart: {
                              type: 'pie'
                          },
                          title: {
                              text: ''
                          },
                          yAxis: {
                              allowDecimals: false,
                              title: {
                                  text: 'Weight (MT)'
                              }
                          },
                          tooltip: {
                              formatter: function () {
                                  return '<b>' + this.series.name + '</b><br/>' +
                this.point.y + ' ' + this.point.name.toLowerCase();
                              }
                          },
                          plotOptions: {
                              pie: {
                                  allowPointSelect: true,
                                  cursor: 'pointer',
                                  dataLabels: {
                                      enabled: false
                                  },
                                  showInLegend: true
                              }
                          }
                      }); 
		</script>
        </div>
       </div> 
    </div> 
        <!--/.col (left) -->



      <div class="col-md-6">  
          <!-- Horizontal Form --> 
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Daily Sales List</h3>
              <div class="box-tools"> 
               
            </div>
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView3" runat="server" EnablePersistedSelection="false"  
    SelectedRowStyle-BackColor="Yellow"  AutoGenerateColumns="false" ShowHeader="true"  ShowFooter="true" CssClass="table  table-sm table-bordered table-striped" >
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
      </div>


              <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title"> Purchase Statement (Average Purchase Price) Parameter</h3>
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
         
      <!-- /.row -->
    </section>
    <!-- /.content -->
  <asp:Panel ID="PanelPrint" runat="server" ></asp:Panel>  
</div>

</asp:Content> 