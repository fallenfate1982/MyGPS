<%@ Page Title="" Language="C#" MasterPageFile="~/Login.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="MyGPS.Login" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="MainContent">
    
    <asp:Login RenderOuterTable="false" ID="loginPanel" runat="server" DestinationPageUrl="~\Dashboard.aspx">
        <LayoutTemplate>

            <div class="form-group">
							<label>Username</label>
							<asp:TextBox runat="server" id="UserName" placeholder="Username" cssclass="form-control input-sm bounceIn animation-delay2" />
						</div>
						<div class="form-group">
							<label>Password</label>
							<asp:TextBox runat="server" id="Password" TextMode="Password" placeholder="Password" cssclass="form-control input-sm bounceIn animation-delay4"/>
						</div>
						<div class="form-group">
							<label class="label-checkbox inline">
								
                                <input type="checkbox" id="RememberMe" runat="server" Text="" cssclass="regular-checkbox chk-delete"/>
								<span class="custom-checkbox info bounceIn animation-delay4"></span>
							</label>
							Remember me
                           
						</div>
		                     <span style="color:red;"><asp:Literal id="FailureText" runat="server"></asp:Literal>	</span>	
						<div class="seperator"></div>
						<div class="form-group">
							Forgot your password?<br/>
							Click <a href="#">here</a> to reset your password
						</div>

						<hr/>
							<asp:LinkButton id="Login" CommandName="Login" runat="server" Text="Login" CssClass="btn btn-success btn-sm bounceIn login-Link animation-delay5 pull-right ">
                                <i class="fa fa-sign-in"></i> Sign in
							</asp:LinkButton>

        </LayoutTemplate>
    </asp:Login>
                                   

</asp:Content>