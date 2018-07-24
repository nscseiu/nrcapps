<%@ Page Title="Actual Garbage Form & List - Plastic Factory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="PfActualGarbage_old.aspx.cs" Inherits="NRCAPPS.PF.PfActualGarbage_old" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="ContentPlaceHolder1"> 
    
     <!-- Content Wrapper. Contains page content -->
  <div class="content-wrapper">
   
    <!-- Content Header (Page header) -->
    <section class="content-header">
      <h1>
        Actual Garbage Form & List
        <small>Actual Garbage: - Update - View</small>
      </h1>
      <ol class="breadcrumb">
        <li><a href="../Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
        <li><a href="#">Plastic Factory</a></li>
        <li class="active">Actual Garbage</li>
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
              <h3 class="box-title">Actual Garbage Form</h3>
            </div>
            <!-- /.box-header -->
            <!-- form start --> 
              <div class="box-body">
                
             <!-- % for (var data = 0; data < TableData.Rows.Count; data++)
                {
                    TextItemID.ID = "TextItemID" + data; %>  

                   <div class="form-group">
                  <label  class="col-sm-2 control-label"><!--%=TableData.Rows[data]["ITEM_ID"]%> . <!--%=TableData.Rows[data]["ITEM_NAME"]%> </label> 
                  <div class="col-sm-2">   
                  <div class="input-group">  
                   <asp:TextBox ID="TextItemID" class="form-control input-sm" runat="server"></asp:TextBox>
                   <span class="input-group-addon">.00</span>      
                    </div>  
                  </div>
                </div--> 
                <!--%}-->
                <div class="form-group">
                  <label  class="col-sm-2 control-label">1. HDPE</label> 
                     <div class="col-sm-2">   
                  <div class="input-group"> 
                  <asp:TextBox ID="TextItemID0"  value="1" runat="server"  style="display:none;" ></asp:TextBox> 
                 <asp:TextBox ID="TextItemWet0" class="form-control input-sm"  runat="server"  AutoPostBack="True" ontextchanged="TextHDPE_TextChanged" ></asp:TextBox> 
                 <span class="input-group-addon">.00</span> 
                    </div>     
                  <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextItemWet0" ErrorMessage="Insert HDPE Cost Rate" Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                   <div class="col-sm-6"><asp:Label ID="Check_HDPE" runat="server"></asp:Label></div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-2 control-label">2. HD CAN</label> 
                     <div class="col-sm-2">   
                  <div class="input-group"> 
                 <asp:TextBox ID="TextItemID1"  runat="server" value="2"  style="display:none;" ></asp:TextBox> 
                 <asp:TextBox ID="TextItemWet1" class="form-control input-sm"  runat="server"   AutoPostBack="True" ontextchanged="TextHDCAN_TextChanged" ></asp:TextBox> 
                 <span class="input-group-addon">.00</span>      
                    </div>  
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextItemWet1" ErrorMessage="Insert HD CAN Cost Rate" Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>
                   </div>
                    <div class="col-sm-6"><asp:Label ID="Check_HDCAN" runat="server"></asp:Label></div>
                </div> 
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">3. LDPE</label> 
                     <div class="col-sm-2">   
                  <div class="input-group"> 
                 <asp:TextBox ID="TextItemID2"  runat="server" value="3"  style="display:none;" ></asp:TextBox> 
                 <asp:TextBox ID="TextItemWet2" class="form-control input-sm"  runat="server"   AutoPostBack="True" ontextchanged="TextLDPE_TextChanged"></asp:TextBox> 
                 <span class="input-group-addon">.00</span>      
                    </div>  
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextItemWet2" ErrorMessage="Insert LDPE Cost Rate" Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>
                   </div> 
                   <div class="col-sm-6"><asp:Label ID="Check_LDPE" runat="server"></asp:Label></div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-2 control-label">4. PC</label> 
                     <div class="col-sm-2">   
                  <div class="input-group"> 
                 <asp:TextBox ID="TextItemID3"  runat="server" value="4" style="display:none;" ></asp:TextBox> 
                 <asp:TextBox ID="TextItemWet3" class="form-control input-sm"  runat="server"  AutoPostBack="True" ontextchanged="TextPC_TextChanged"></asp:TextBox> 
                 <span class="input-group-addon">.00</span>      
                    </div>  
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TextItemWet3" ErrorMessage="Insert PC Cost Rate" Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                  <div class="col-sm-6"><asp:Label ID="Check_PC" runat="server"></asp:Label></div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-2 control-label">5. PET</label> 
                     <div class="col-sm-2">   
                  <div class="input-group"> 
                 <asp:TextBox ID="TextItemID4"  runat="server" value="5" style="display:none;" ></asp:TextBox> 
                 <asp:TextBox ID="TextItemWet4" class="form-control input-sm"  runat="server"  AutoPostBack="True" ontextchanged="TextPET_TextChanged"></asp:TextBox> 
                 <span class="input-group-addon">.00</span>      
                    </div>  
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TextItemWet4" ErrorMessage="Insert PET Cost Rate" Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                  <div class="col-sm-6"><asp:Label ID="Check_PET" runat="server"></asp:Label></div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-2 control-label">6. PETRO RABIGH</label> 
                     <div class="col-sm-2">   
                  <div class="input-group"> 
                 <asp:TextBox ID="TextItemID5"  runat="server" value="6" style="display:none;" ></asp:TextBox> 
                 <asp:TextBox ID="TextItemWet5" class="form-control input-sm"  runat="server" AutoPostBack="True"  ontextchanged="TextPETRO_RABIGH_TextChanged"></asp:TextBox> 
                 <span class="input-group-addon">.00</span>      
                    </div>  
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="TextItemWet5" ErrorMessage="Insert PETRO RABIGH Cost Rate" Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                  <div class="col-sm-6"><asp:Label ID="Check_PETRO_RABIGH" runat="server"></asp:Label></div>
                </div> 
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">7. PP</label> 
                     <div class="col-sm-2">   
                  <div class="input-group">
                 <asp:TextBox ID="TextItemID6"  runat="server"  value="7"  style="display:none;" ></asp:TextBox>  
                 <asp:TextBox ID="TextItemWet6" class="form-control input-sm"  runat="server" AutoPostBack="True"  ontextchanged="TextPP_TextChanged"></asp:TextBox> 
                 <span class="input-group-addon">.00</span>      
                    </div>  
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="TextItemWet6" ErrorMessage="Insert PP Cost Rate" Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                   <div class="col-sm-6"><asp:Label ID="Check_PP" runat="server"></asp:Label></div>
                </div> 
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">8. PVC</label> 
                     <div class="col-sm-2">   
                  <div class="input-group"> 
                 <asp:TextBox ID="TextItemID7"  runat="server"  value="8"  style="display:none;" ></asp:TextBox>  
                 <asp:TextBox ID="TextItemWet7" class="form-control input-sm"  runat="server" AutoPostBack="True"  ontextchanged="TextPVC_TextChanged"></asp:TextBox> 
                 <span class="input-group-addon">.00</span>      
                    </div> 
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="TextItemWet7" ErrorMessage="Insert PVC Cost Rate" Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator> 
                  </div>
                   <div class="col-sm-6"><asp:Label ID="Check_PVC" runat="server"></asp:Label></div>
                </div> 
                 <div class="form-group">
                  <label  class="col-sm-2 control-label">9. SABIC</label> 
                     <div class="col-sm-2">   
                  <div class="input-group"> 
                 <asp:TextBox ID="TextItemID8"  runat="server"  value="9"  style="display:none;" ></asp:TextBox>  
                 <asp:TextBox ID="TextItemWet8" class="form-control input-sm"  runat="server"  AutoPostBack="True"  ontextchanged="TextSABIC_TextChanged"></asp:TextBox> 
                 <span class="input-group-addon">.00</span>      
                    </div>  
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="TextItemWet8" ErrorMessage="Insert SABIC Cost Rate" Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>
                  </div>
                   <div class="col-sm-6"><asp:Label ID="Check_SABIC" runat="server"></asp:Label></div>
                </div> 
                <div class="form-group">
                  <label  class="col-sm-2 control-label">10. SCC</label> 
                     <div class="col-sm-2">   
                  <div class="input-group"> 
                 <asp:TextBox ID="TextItemID9"  runat="server"  value="10"  style="display:none;" ></asp:TextBox> 
                 <asp:TextBox ID="TextItemWet9" class="form-control input-sm"  runat="server" AutoPostBack="True"  ontextchanged="TextSCC_TextChanged"></asp:TextBox> 
                 <span class="input-group-addon">.00</span>      
                    </div> 
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="TextItemWet9" ErrorMessage="Insert SCC Cost Rate" Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator> 
                  </div>
                   <div class="col-sm-6"><asp:Label ID="Check_SCC" runat="server"></asp:Label></div>
                </div>  
              <div class="form-group">
                    <label class="col-sm-2 control-label">Month</label>
                     <div class="col-sm-2">   
                    <div class="input-group date">
                      <div class="input-group-addon">
                        <i class="fa fa-calendar"></i>
                      </div>  
                       <asp:TextBox  class="form-control  input-sm pull-right" ID="TextMonthYear0"  runat="server" AutoPostBack="True" ontextchanged="TextMonthYear0_TextChanged" ></asp:TextBox>  
                    </div>
                       
                      </div>   <div class="col-sm-3"><asp:Label ID="CheckMonthYear" runat="server"></asp:Label> 
                  </div>
                  </div>  
                   </div>
              <!-- /.box-body -->
              <div class="box-footer">
               <div class="form-group">
                  <div  class="col-sm-2" style="text-align:right;"> 
                      <asp:LinkButton ID="ClearFiled" runat="server" class="btn btn-default" OnClick="clearTextField" CausesValidation="False"><span class="fa fa-reply"></span> Reset</asp:LinkButton> </div>
                   <div class="col-sm-6">   
                     <asp:LinkButton ID="BtnAdd" class="btn btn-primary" runat="server" Text="Add New" onclick="BtnAdd_Click"><span class="fa fa-plus"></span> Add New</asp:LinkButton> 
                     <asp:LinkButton ID="BtnUpdate" class="btn btn-success" runat="server" Text="Update"  onclick="BtnUpdate_Click"><span class="fa fa-edit"></span> Update</asp:LinkButton>
                  </div>
                </div> 
              <!-- /.box-footer -->
         <!-- /.box -->
        </div>  
      </div>   
        <div class="box box-success">
            <div class="box-header with-border">
              <h3 class="box-title">Actual Garbage List</h3>
              <div class="box-tools">
              <div class="input-group input-group-sm" style="width: 200px;">
                <asp:TextBox ID="txtSearchUser" Class="form-control input-sm" runat="server" />
                 <div class="input-group-btn">
                      <asp:Button ID="ButtonSearchUser" Class="btn btn-info"   
                        Text="Search" runat="server" OnClick="GridViewSearchUser" 
                        CausesValidation="False" />
                  </div>  
              </div>    
            </div>
            </div> 
            <!-- /.box-header -->
            <div class="box-body table-responsive"> 
                    <asp:GridView ID="GridView1" runat="server" EnablePersistedSelection="true"            
    SelectedRowStyle-BackColor="Yellow" 
    AllowPaging="true" 
    AllowSorting="true"
    PageSize = "10" 
    OnPageIndexChanging="GridViewUser_PageIndexChanging" AutoGenerateColumns="false"   CssClass="table table-bordered table-striped" >
                     <Columns> 
                     <asp:BoundField DataField="MONTH_YEAR"  HeaderText="Month" DataFormatString="{0:MM/yyyy}"  />  
                      <asp:TemplateField>
                       <ItemTemplate>
                        <asp:LinkButton ID="linkSelectClick" class="btn btn-info" runat="server" CommandArgument='<%# Eval("MONTH_YEAR") %>' OnClick="linkSelectClick" CausesValidation="False">Select</asp:LinkButton> 
                        </ItemTemplate>
                       </asp:TemplateField>   
                     <asp:BoundField DataField="ITEM_NAME"  HeaderText="Item" />  
                     <asp:BoundField DataField="ITEM_CODE" HeaderText="Item Code" />  
                     <asp:BoundField DataField="ACTUAL_GAR_WEIGHT"  HeaderText="Actual Garbage Weight - MT" />   
                     <asp:BoundField DataField="CREATE_DATE"  HeaderText="Create Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />  
                     <asp:BoundField DataField="UPDATE_DATE"  HeaderText="Update Date" DataFormatString="{0:dd/MM/yyyy h:mm:ss tt}"  />    
                     </Columns> 
                        <PagerStyle CssClass="pagination-ys" />
                        <SelectedRowStyle BackColor="Yellow"></SelectedRowStyle>
                    </asp:GridView>  
        </div>
       </div> 
    </div> 
          <!-- /.box --> 
        <!--/.col (right) -->
      </div>
      <!-- /.row -->
    </section>
    <!-- /.content -->
 
</div>
    
</asp:Content> 