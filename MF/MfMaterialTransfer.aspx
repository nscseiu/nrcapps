<%@ Page Title="Material Transfer Form & List - Metal Factory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="MfMaterialTransfer.aspx.cs" Inherits="NRCAPPS.MF.MfMaterialTransfer" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Material Transfer Form & List
        <small>Material Transfer: - Add - Update - Delete</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Metal Factory</a></li>
        <li class="active">Material Transfer</li>
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
              <h3 class="box-title">Material Transfer Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start -->
            
              <div class="box-body">
                <div class="form-group">   
                    <label class="col-sm-4 control-label">Weigh-Bridge Slip No</label> 
                   <div class="col-sm-3">  
                    <asp:TextBox ID="TextTransferID" class="form-control input-sm" style="display:none"  runat="server"></asp:TextBox>    
                    <asp:TextBox ID="TextWbSlipNo" class="form-control input-sm"  runat="server" AutoPostBack="True"  ontextchanged="TextWbSlipNo_TextChanged"></asp:TextBox>  
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                          ControlToValidate="TextWbSlipNo" ErrorMessage="Insert Slip No." 
                          Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                 </div>
                  <div class="col-sm-4"><asp:Label ID="CheckSlipNo" runat="server"></asp:Label></div>  
               </div>
               <div class="form-group">
                  <label  class="col-sm-4 control-label">Vehicle No</label> 
                  <div class="col-sm-4">   
                    <asp:DropDownList ID="DropDownVehicleID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                          ControlToValidate="DropDownVehicleID" Display="Dynamic" 
                          ErrorMessage="Select Vehicle" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-4 control-label">Material</label> 
                  <div class="col-sm-4">   
                    <asp:DropDownList ID="DropDownMaterialID" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                          ControlToValidate="DropDownMaterialID" Display="Dynamic" 
                          ErrorMessage="Select Material" InitialValue="0" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-4 control-label">Weight as Per Metal Factory</label> 
                  <div class="col-sm-4">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextMatWeightMf" class="form-control input-sm"  runat="server" ></asp:TextBox> 
                    <span class="input-group-addon">KG</span>      
                    </div>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server"   ControlToValidate="TextMatWeightMf" ErrorMessage="insert Item Weight" Display="Dynamic" SetFocusOnError="True" ></asp:RequiredFieldValidator>
                  </div>
                </div>
                <div class="form-group">
                  <label  class="col-sm-4 control-label">Weight as Per Metal Scarp</label> 
                  <div class="col-sm-4">  
                   <div class="input-group"> 
                    <asp:TextBox ID="TextMatWeightMs" class="form-control input-sm"  runat="server" ></asp:TextBox> 
                    <span class="input-group-addon">KG</span>      
                    </div> 
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" 
                          ControlToValidate="TextMatWeightMs" ErrorMessage="insert Item Rate" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                  </div>
                </div>
                <div class="form-group">
                  <label  class="col-sm-4 control-label">Weight Diffrence</label> 
                  <div class="col-sm-4">  
                  <div class="input-group"> 
                    <asp:TextBox ID="TextWeightDifference" class="form-control input-sm"  runat="server" disabled="disabled" ></asp:TextBox>    
                       <span class="input-group-addon">KG</span>      
                    </div>
                  </div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-4 control-label">Remarks</label> 
                  <div class="col-sm-4">   
                    <asp:TextBox ID="TextRemarks" class="form-control input-sm"  runat="server" ></asp:TextBox>          
                  </div>
                </div>
                 <div class="form-group">
                    <label class="col-sm-4 control-label">Entry Date</label>
                     <div class="col-sm-4">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="EntryDate"  runat="server"   ></asp:TextBox> <!-- AutoPostBack="True"  ontextchanged="TextCheckDataProcess" -->
                    </div>  <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                          ControlToValidate="EntryDate" ErrorMessage="insert Entry Date" 
                          Display="Dynamic" SetFocusOnError="True"   ></asp:RequiredFieldValidator>
                      </div> 
                      
                    <!-- /.input group -->
                    <div class="col-sm-4"><asp:Label ID="CheckEntryDate" runat="server"></asp:Label></div>
                  </div>
                 
                <div class="form-group">
                  <label  class="col-sm-4 control-label">Is Active Status</label> 
                  <div class="col-sm-4" style="padding-top:6px;">    
                        <label>
                            <input type="checkbox" ID="CheckIsActive" class="flat-red" checked 
                            runat="server" tabindex="2"/>
                        </label>
                  </div>
                </div> 
                 <!-- checkbox -->
              
                 
              </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-4" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" 
                          OnClick="clearTextField" CausesValidation="False" TabIndex="1"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">    
                    <asp:LinkButton ID="BtnAdd" class="btn btn-primary" runat="server" Text="Add New" onclick="BtnAdd_Click"><span class="fa fa-plus"></span> Add New</asp:LinkButton>
                    <asp:LinkButton ID="BtnUpdate" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdate_Click"><span class="fa fa-edit"></span> Update</asp:LinkButton>
                    <asp:LinkButton ID="BtnDelete" class="btn btn-danger" runat="server" onclick="BtnDelete_Click" onclientclick="return confirm('Are you sure to delete?');" ><span class="fa fa-close"></span> Delete</asp:LinkButton>
                      
                </div>
                </div>
              </div>
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>     </div> 
          <div class="col-md-5">        
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Material Transfer Summary (Current Month-Default)</h3>
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
    SelectedRowStyle-BackColor="Yellow"  AutoGenerateColumns="false" ShowHeader="true"  ShowFooter="true" CssClass="table  table-sm table-bordered table-striped"  >
                     <Columns>
                     <asp:BoundField DataField="MATERIAL_NAME"  HeaderText="Mat. Name" />
                     <asp:BoundField DataField="MATERIAL_CODE"  HeaderText="Mat. Code" />
                     <asp:BoundField DataField="SLIP_NO" HeaderText="Total WB-Slip" ItemStyle-HorizontalAlign="Center" />
                     <asp:BoundField DataField="WT_AS_PER_MF" HeaderText="WT as Per Metal Factory" DataFormatString="{0:0,0.00}" ItemStyle-HorizontalAlign="Right" />
                     <asp:BoundField DataField="WT_AS_PER_MS"  HeaderText="WT as Per Metal Scarp" DataFormatString="{0:0,0.00}" ItemStyle-HorizontalAlign="Right" /> 
                     <asp:BoundField DataField="DIFFERENCE"  HeaderText="Diffrence" DataFormatString="{0:0,0.00}" ItemStyle-HorizontalAlign="Right" /> 
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
              <h3 class="box-title">Material Transfer List</h3>
              <div class="box-tools">
              <div class="col-sm-5">   
                    <asp:DropDownList ID="DropDownMaterialID1" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>
               </div> 
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchEmp" Class="form-control" runat="server" />
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
              
                    <asp:GridView ID="GridView1" runat="server"    EnablePersistedSelection="true"  OnDataBound="gridViewFileInformation_DataBound"        
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "12" 
    OnPageIndexChanging="GridViewEmp_PageIndexChanging" AutoGenerateColumns="false" CssClass="table table-hover table-bordered table-striped" >
                     <Columns>
                     <asp:BoundField DataField="WB_SLIP_NO" HeaderText="WB- Slip No" />
                     <asp:BoundField DataField="VEHICLE_NO" HeaderText="Vehicle No" />
                     <asp:BoundField DataField="MATERIAL_NAME"  HeaderText="Material" />
                     <asp:BoundField DataField="MATERIAL_CODE"  HeaderText="Material Code" />    
                     <asp:BoundField DataField="WT_AS_PER_MF"  HeaderText="WT as Per Metal Factory" DataFormatString="{0:0,0.00}" />
                     <asp:BoundField DataField="WT_AS_PER_MS"  HeaderText="WT as Per Metal Scarp" DataFormatString="{0:0,0.00}" />   
                     <asp:BoundField DataField="DIFFERENCE"  HeaderText="Diffrence" DataFormatString="{0:0,0.00}" />     
                     <asp:BoundField DataField="REMARKS"  HeaderText="Remarks" />
                     <asp:TemplateField HeaderText="Status" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsActiveGV" CssClass="label" Text='<%# Eval("IS_ACTIVE").ToString() == "Enable" ? "<span Class=label-success style=Padding:2px >Enable<span>" : "<span Class=label-danger style=Padding:2px>Disable<span>" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField>
                     <asp:BoundField DataField="ENTRY_DATE"  HeaderText="Entry Date"  DataFormatString="{0:dd/MM/yyyy}"  />
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:TemplateField HeaderText="Mat. Coor. Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsCmoCheck" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete<span>" : "<span Class=label-danger style=Padding:2px>Incomplete<span>" %>'  runat="server" /> 
                             <asp:Label ID="IsCmoCheckLink" style="display:none" CssClass="label" Text='<%# Eval("FIRST_APPROVED_IS").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField> 
                     <asp:TemplateField HeaderText="Plant Head Approve" ItemStyle-Width="100">
                        <ItemTemplate> 
                             <asp:Label ID="IsSecondCheck" CssClass="label" Text='<%# Eval("SECOND_APPROVED_IS").ToString() == "Complete" ? "<span Class=label-success style=Padding:2px >Complete<span>" : "<span Class=label-danger style=Padding:2px>Incomplete<span>" %>'  runat="server" /> 
                             <asp:Label ID="IsSecondCheckLink" style="display:none" CssClass="label" Text='<%# Eval("SECOND_APPROVED_IS").ToString() == "Complete" ? "Complete" : "Incomplete" %>'  runat="server" /> 
                          </ItemTemplate>
                     </asp:TemplateField> 
                     <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info btn-xs" runat="server" CommandArgument='<%# Eval("TRANSFER_ID") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
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
              <h3 class="box-title"> Material Transfer Statement (Supplier - Month Wise) Parameter</h3>
            </div>
            <!-- /.box-header -->

              <div class="box-body">
            <!-- form start -->   
              <div class="form-group">
                  <label  class="col-sm-2 control-label">Supplier Name</label> 
                  <div class="col-sm-3">   
                    <asp:DropDownList ID="DropDownSupplierID2" class="form-control input-sm" runat="server"> 
                    </asp:DropDownList>  
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                          ControlToValidate="DropDownSupplierID2" Display="Dynamic" 
                          ErrorMessage="Select Supplier" InitialValue="0" SetFocusOnError="True"  ValidationGroup='valGroup2'></asp:RequiredFieldValidator>
                  </div>
                </div>
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
              <h3 class="box-title"> Material Transfer Statement (Supplier - Month Wise) Report View</h3>
            </div> 
            <!-- /.box-header -->
                    <div class="box-body">       
                           <iframe src="PF_Reports/PfPurchaseSupplierWiseReportView.aspx?MonthYear=<%=TextMonthYear0.Text %>&SupplierID=<%=DropDownSupplierID2.Text %>" width="950px" id="iframe1"
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