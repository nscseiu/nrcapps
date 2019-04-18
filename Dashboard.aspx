<%@ Page Title="NESMA Recycling Applications | Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="NRCAPPS.Dashboard" %>
 

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
      
    <div class="content-wrapper">
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Dashboard
        <small>Control panel</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="#"><i class="fa fa-dashboard"></i> Home</a></li>
        <li class="active">Dashboard</li>
      </ol>
    </section>

    <!-- Main content -->
    <section class="content">
      <!-- Small boxes (Stat box) -->
      <asp:PlaceHolder ID = "PlaceHolderDashboardReport" runat="server" />  
      <!-- /.row --> 
      <script type="text/javascript">
          Highcharts.chart('container_pf_monthly', {
              data: {
                  table: 'datatable_pf_monthly'
              },
              chart: {
                  type: 'line'
              },
              title: {
                  text: ''
              },
              yAxis: {
                  allowDecimals: true,
                  title: {
                      text: 'Material Weight (MT)'
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
                      text: 'Amount (SR)'
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

      <script type="text/javascript">
          Highcharts.chart('container_rm_pie', {
              data: {
                  table: 'datatable_rm_pie'
              },
              chart: {
                  type: 'column'
              },
              title: {
                  text: 'Raw Material & Finished Goods (Weight - MT)'
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
                this.point.y + ' ' + this.point.name.toUpperCase(); 
                  }
              },
              plotOptions: {
                  pie: {
                      allowPointSelect: true,
                      cursor: 'pointer'
                  }
              }
          });

		</script>
     
         
      <!-- Main row -->
      <div class="row">
        <!-- Left col -->
        <section class="col-lg-7 connectedSortable">
          <!-- Custom tabs (Charts with tabs)--> 
          <!-- /.nav-tabs-custom --> 
          <!-- /.box (chat box) --> 
          <!-- TO DO List -->
        
          <!-- /.box -->

      
        </section>
        <!-- /.Left col -->
        <!-- right col (We are only adding the ID to make the widgets sortable)-->
        <section class="col-lg-5 connectedSortable">
         

          <!-- Calendar -->
          <!--div class="box box-solid bg-green-gradient">
            <div class="box-header">
              <i class="fa fa-calendar"></i>

              <h3 class="box-title">Calendar</h3>
              <!-- tools box -->
              <!--div class="pull-right box-tools">
                <!-- button with a dropdown -->
                <!--div class="btn-group">
                  <button type="button" class="btn btn-success btn-sm dropdown-toggle" data-toggle="dropdown">
                    <i class="fa fa-bars"></i></button>
                  <ul class="dropdown-menu pull-right" role="menu">
                    <li><a href="#">Add new event</a></li>
                    <li><a href="#">Clear events</a></li>
                    <li class="divider"></li>
                    <li><a href="#">View calendar</a></li>
                  </ul>
                </div>
                <button type="button" class="btn btn-success btn-sm" data-widget="collapse"><i class="fa fa-minus"></i>
                </button>
                <button type="button" class="btn btn-success btn-sm" data-widget="remove"><i class="fa fa-times"></i>
                </button>
              </div>
              <!-- /. tools -->
            <!--/div>
            <!-- /.box-header -->
            <!--div class="box-body no-padding">
              <!--The calendar -->
              <!--div id="calendar" style="width: 100%"></div>
            </div>
            <!-- /.box-body -->
            <!--div class="box-footer text-black">
              <div class="row">
                <div class="col-sm-6">
                  <!-- Progress bars -->
                  <!--div class="clearfix">
                    <span class="pull-left">Task #1</span>
                    <small class="pull-right">90%</small>
                  </div>
                  <div class="progress xs">
                    <div class="progress-bar progress-bar-green" style="width: 90%;"></div>
                  </div>

                  <div class="clearfix">
                    <span class="pull-left">Task #2</span>
                    <small class="pull-right">70%</small>
                  </div>
                  <div class="progress xs">
                    <div class="progress-bar progress-bar-green" style="width: 70%;"></div>
                  </div>
                </div>
                <!-- /.col -->
                <!--div class="col-sm-6">
                  <div class="clearfix">
                    <span class="pull-left">Task #3</span>
                    <small class="pull-right">60%</small>
                  </div>
                  <div class="progress xs">
                    <div class="progress-bar progress-bar-green" style="width: 60%;"></div>
                  </div>

                  <div class="clearfix">
                    <span class="pull-left">Task #4</span>
                    <small class="pull-right">40%</small>
                  </div>
                  <div class="progress xs">
                    <div class="progress-bar progress-bar-green" style="width: 40%;"></div>
                  </div>
                </div>
                <!-- /.col -->
              <!--/div-->
              <!-- /.row -->
            <!--/div-->
          <!--/div-->
          <!-- /.box -->

        </section>
        <!-- right col -->
      </div>
      <!-- /.row (main row) -->

    </section>
    <!-- /.content -->


  </div>
  
 
</asp:Content> 
