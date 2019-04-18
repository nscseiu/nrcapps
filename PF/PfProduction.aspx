<%@ Page Title="Production Form & List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="PfProduction.aspx.cs" Inherits="NRCAPPS.PF.PfProduction" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Production Form & List
        <small>Production: - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Plastic Factory</a></li>
        <li class="active">Production</li>
      </ol>
    </section>
    
    <!-- Main content -->
    <section class="content">
      <div class="row">
        <!-- left column --> 
            
        <!--/.col (left) -->
        <!-- right column -->
        <div class="col-md-7"> 
             <asp:Panel  id="alert_box" runat="server">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                <h4><i class="icon fa fa-check"></i> Alert!</h4>  
           </asp:Panel> 
          <!-- Horizontal Form -->
          <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title">Production Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start -->
            
              <div class="box-body">
                 
               <div class="form-group">
                  <label  class="col-sm-3 control-label">Shift Name</label> 
                  <div class="col-sm-5">   
                   <asp:TextBox ID="TextProductionID" style="display:none" runat="server"></asp:TextBox>
                    <asp:DropDownList ID="DropDownShiftID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownShiftID" Display="Dynamic" 
                          ErrorMessage="Select Shift" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div>
                  <div class="form-group">
                  <label  class="col-sm-3 control-label">Machine Number</label> 
                  <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownMachineID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" 
                          ControlToValidate="DropDownMachineID" Display="Dynamic" 
                          ErrorMessage="Select Machine Number" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Supervisor Name</label> 
                  <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownSupervisorID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                          ControlToValidate="DropDownSupervisorID" Display="Dynamic" 
                          ErrorMessage="Select Supervisor" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Item</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownItemID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownItemID" Display="Dynamic" 
                          ErrorMessage="Select Item" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div> 
                    <label  class="col-sm-3 control-label">Sub Item</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownSubItemID" class="form-control input-sm" runat="server"  > </asp:DropDownList> <!-- AutoPostBack="True"  ontextchanged="TextSubItem_Changed" -->
                   
                </div> 
                </div> 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Item Weight</label> 
                  <div class="col-sm-3"> 
                  <div class="input-group"> 
                    <asp:TextBox ID="TextItemWeight" class="form-control input-sm"  runat="server"  AutoPostBack="True"  ontextchanged="TextItemWeight_TextChanged"></asp:TextBox> 
                    <span class="input-group-addon">MT</span>      
                    </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"  Type="Double"
                          ControlToValidate="TextItemWeight" ErrorMessage="Insert Material Weight" 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                   <div class="col-sm-5"><asp:Label ID="CheckItemWeight" runat="server"></asp:Label></div>  
                </div> 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Garbage Est. of Production</label> 
                  <div class="col-sm-3">  
                   <div class="input-group"> 
                    <asp:DropDownList ID="DropDownPgeID" class="form-control input-sm" runat="server"  AutoPostBack="True"  ontextchanged="TextPgeWet_Changed">   
                    </asp:DropDownList>  
                      <span class="input-group-addon">%</span>  
                      </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" 
                          ControlToValidate="DropDownPgeID" Display="Dynamic" 
                          ErrorMessage="Select Garbage Est. of Production (%)" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator> 
                  </div> 
                    <label  class="col-sm-3 control-label">Garbage Est. of Prod. Wt</label> 
                  <div class="col-sm-3">   
                    <div class="input-group"> 
                         <asp:TextBox ID="TextPgeWet" class="form-control input-sm"  runat="server"></asp:TextBox>    
                         <span class="input-group-addon">MT</span>  
                      </div>
                  </div> 
                </div>  
                 <div class="form-group">
                    <label class="col-sm-3 control-label">Entry Date</label>
                     <div class="col-sm-3">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control input-sm pull-right" ID="EntryDate"  runat="server" ></asp:TextBox>  <!-- AutoPostBack="True"  ontextchanged="TextCheckDataProcess" -->
                    </div>
                      </div> 
                      <div class="col-sm-3"><asp:Label ID="CheckEntryDate" runat="server"></asp:Label></div>  
                    <!-- /.input group -->
                  </div>
                 
                <div class="form-group">
                  <label  class="col-sm-3 control-label">Is Active Status</label> 
                  <div class="col-sm-4" style="padding-top:6px;">    
                        <label>
                            <input type="checkbox" ID="CheckIsActive" class="flat-red" checked 
                            runat="server" tabindex="1"/>
                        </label>
                  </div>
                </div> 
                 <!-- checkbox -->
              
                 
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-3" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" 
                          OnClick="clearTextField" CausesValidation="False" TabIndex="1"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">    
                    <asp:LinkButton ID="BtnAdd" class="btn btn-primary" runat="server" Text="Add New" onclick="BtnAdd_Click" OnClientClick="return CheckIsRepeat();" ><span class="fa fa-plus"></span> Add New</asp:LinkButton>
                    <asp:LinkButton ID="BtnUpdate" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdate_Click"><span class="fa fa-edit"></span> Update</asp:LinkButton>
                    <asp:LinkButton ID="BtnDelete" class="btn btn-danger" runat="server" onclick="BtnDelete_Click" onclientclick="return confirm('Are you sure to delete?');" ><span class="fa fa-close"></span> Delete</asp:LinkButton>
                      
                </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>  
         </div> 

          <div class="col-md-5">        
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Production Summary (Current Month-Default)</h3>
              <div class="box-tools"> 
              <div class="input-group input-group-sm" style="width: 200px;"> 
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="TextMonthYear4"  runat="server" ></asp:TextBox>  
                    </div>
                 <div class="input-group-btn">
                      <asp:Button ID="Button1" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridViewSearchSummary" 
                        CausesValidation="False" />
                  </div>  
              </div>    
            </div>
            </div>
           
            <!-- /.box-header -->
            <div class="box-body table-responsive">
               <asp:GridView ID="GridView2" runat="server" EnablePersistedSelection="false"  
    SelectedRowStyle-BackColor="Yellow"  AutoGenerateColumns="false" ShowHeader="true"  ShowFooter="true" CssClass="table  table-sm table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item Name" />
                     <asp:BoundField DataField="PRODUCTION_ID" HeaderText="Total Slip" ItemStyle-HorizontalAlign="Center" />
                     <asp:BoundField DataField="ITEM_WEIGHT" HeaderText="Item WT" DataFormatString="{0:0.000}" ItemStyle-HorizontalAlign="Right" />
                     <asp:BoundField DataField="PGE_WEIGHT"  HeaderText="PGE WT" DataFormatString="{0:0.000}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="ACTUAL_GAR_WEIGHT"  HeaderText="Actual Garbage" DataFormatString="{0:0.000}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="TOTAL_WEIGHT"  HeaderText="Total WT" DataFormatString="{0:0.000}" ItemStyle-HorizontalAlign="Right" /> 
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                    </asp:GridView> 
                 
        </div>
       </div>
        
         
      
        </div>   
           
    <div class="col-md-12">   
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Production List</h3>
              <div class="box-tools"> 
              <div class="col-sm-3">  
                    <asp:DropDownList ID="DropDownItemID1" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>
               </div> 
                   <div class="col-sm-4">  
                <div class="input-group date" style="width: 150px;">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="TextMonthYear3"  runat="server" ></asp:TextBox>  
                </div>    </div>
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchEmp" Class="form-control input-sm" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="ButtonSearchEmp" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridViewSearchEmp" 
                        CausesValidation="False" />
                  </div>  
              </div>    
            </div>
            </div>
           
            <!-- /.box-header -->
            <div class="box-body table-responsive">
              
                    <asp:GridView ID="GridView1" runat="server"    EnablePersistedSelection="true"            
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "12" 
    OnPageIndexChanging="GridViewEmp_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="PRODUCTION_ID" HeaderText="Prod. ID" />
                     <asp:BoundField DataField="SHIFT_NAME" HeaderText="Shift Name" />
                     <asp:BoundField DataField="MACHINE_NUMBER"  HeaderText="Machine No." />
                     <asp:BoundField DataField="SHIFT_MACHINE"  HeaderText="Shift Machine" />  
                     <asp:BoundField DataField="SUPERVISOR_NAME"  HeaderText="Supervisor" />   
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" /> 
                     <asp:BoundField DataField="SUB_ITEM_NAME"  HeaderText="Sub Item" />  
                     <asp:BoundField DataField="ITEM_WEIGHT"  HeaderText="Wet-MT"  DataFormatString="{0:0.000}" />    
                     <asp:BoundField DataField="PGE_PERCENT"  HeaderText="GEP %" />
                     <asp:BoundField DataField="PGE_WEIGHT"  HeaderText="GPE Wet."  DataFormatString="{0:0.000}" />
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date" DataFormatString="{0:dd/MM/yyyy}"  />
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("PRODUCTION_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField> 
                     </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                    </asp:GridView> 
        </div>
       </div>
  

        <div class="box box-info">
            <div class="box-header with-border">
              <h3 class="box-title"> Prodution Summary (Month Wise) Parameter</h3>
            </div>
            <!-- /.box-header --> 
              <div class="box-body">
            <!-- form start -->   
                   <div class="form-group">
                    <label class="col-sm-2 control-label">Select Month </label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="TextMonthYear0"  runat="server" ></asp:TextBox>  
                    </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                         ControlToValidate="TextMonthYear0" ErrorMessage="Insert Date" 
                         SetFocusOnError="True" Display="Dynamic" ValidationGroup='valGroup2'></asp:RequiredFieldValidator> 
                  </div>
                  </div>
                </div> 
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="LinkButton1" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">     
                    <asp:LinkButton ID="BtnReport" class="btn btn-info" runat="server" Text="View Report"  onclick="BtnReport_Click"  ValidationGroup='valGroup2' ClientIDMode="Static"><span class="fa fa-fax"></span> View Report</asp:LinkButton> 
                   </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>   
        
            <asp:Panel ID="Panel1" runat="server">  
            <%if (IsLoad)
                {%> 
            <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title"> Prodution Summary (Month Wise) Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="PF_Reports/PfProductionSummaryReportView.aspx?MonthYear=<%=TextMonthYear0.Text %>" width="950px" id="iframe1"
                        marginheight="0" frameborder="0" scrolling="auto" height="1250px">   </iframe>  
                         </div>
                       </div> 
                    
                     <%} %>   
            </asp:Panel>  
          <!-- /.box --> 
        <!--/.col (right) -->  
        </div> 
      
      <!-- /.row -->
    </section>
    <!-- /.content -->
 </div>
 
</asp:Content> 