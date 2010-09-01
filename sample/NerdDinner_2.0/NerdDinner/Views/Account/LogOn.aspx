<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="DotNetOpenAuth.Mvc" %>
<%@ Import Namespace="DotNetOpenAuth.OpenId.RelyingParty" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">
    Log On
</asp:Content>

<asp:Content ContentPlaceHolderID=HeadArea runat=server>
	<link rel="Stylesheet" type="text/css" href="<%=Page.ClientScript.GetWebResourceUrl(typeof(DotNetOpenAuth.OpenId.RelyingParty.OpenIdSelector), "DotNetOpenAuth.OpenId.RelyingParty.OpenIdSelector.css")%>" />
	<link rel="Stylesheet" type="text/css" href="<%=Page.ClientScript.GetWebResourceUrl(typeof(DotNetOpenAuth.OpenId.RelyingParty.OpenIdSelector), "DotNetOpenAuth.OpenId.RelyingParty.OpenIdAjaxTextBox.css")%>" />
    
    <script src="/Scripts/jquery.cookie.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Log On</h2>
    <%: Html.ValidationSummary("Login was unsuccessful. Please correct the errors and try again.") %>
    <style type="text/css">
        /* TODO: Move inline styles to Production.css */
        #login-oauth, #login-account { float: left; width: 330px; height: 210px; border: 1px dotted black; padding: 10px; }
        #login-oauth-container { margin-top: 30px; }
        #login-oauth h3, #login-account h3 { font-size: 22px; font-weight: bold; }
        #oauth-buttons { margin-top: 60px; }
        #oauth-buttons a { display: block; height: 32px; width: 111px; border: 1px solid #aaa; float: left; margin-right: 5px; }
        #or { float: left; margin-top: 40px; margin-right: 20px; margin-left: 20px;  font-size: x-large; }
        #openid_identifier { font-size: large; padding-top: 5px; padding-bottom: 5px; }
        .OpenIdAjaxTextBox input[type='button'] { font-size: large; }
        .OpenIdAjaxTextBox img { padding-top: 7px; }
        .classiclogon { font-size: large; padding-top: 2px; padding-bottom: 2px; }
    </style>

    <div id="login-oauth">
        <h3>via 3rd Party (recommended)</h3>
        
        <% Html.RenderPartial("LogOnContent"); %>

    </div>
    <div id="or">OR</div>
    <div id="login-account">
        <h3>using a NerdDinner account</h3>

    <% using (Html.BeginForm()) { %>
        <div>
            <p>
                <label for="username">Username:</label>
                <%: Html.TextBox("username") %>
                <%: Html.ValidationMessage("username", "*") %>
            </p>
            <p>
                <label for="password">Password:</label>
                <%: Html.Password("password") %>
                <%: Html.ValidationMessage("password", "*")%>
            </p>
            <p>
                <%: Html.CheckBox("rememberMe") %> <label class="inline" for="rememberMe">Remember me?</label>
            </p>
            <p>
                <input class="classiclogon" type="submit" value="Log On" />
            </p>
            <p>
                Please enter your username and password. <%: Html.ActionLink("Register", "Register") %> if you don't have an account.
            </p>
        </div>
    </div>
    <% } %>

    <% var options = new OpenIdSelector();
       options.TextBox.LogOnText = "Log On"; %>

    <%= Html.OpenIdSelectorScripts(this.Page, options, null)%>

</asp:Content>
