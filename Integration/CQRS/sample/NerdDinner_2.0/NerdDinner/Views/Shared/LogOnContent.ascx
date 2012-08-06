<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="DotNetOpenAuth.Mvc" %>
<%@ Import Namespace="DotNetOpenAuth.OpenId.RelyingParty" %>

<% using (Html.BeginForm("LogOnPostAssertion", "Auth", FormMethod.Post, new { target = "_top" })) { %>
<%= Html.AntiForgeryToken() %>
<%= Html.Hidden("ReturnUrl", Request.QueryString["ReturnUrl"], new { id = "ReturnUrl" }) %>
<%= Html.Hidden("openid_openidAuthData") %>
<div id="login-oauth-container">
<%= Html.OpenIdSelector(this.Page, new SelectorButton[] {
	new SelectorProviderButton("https://me.yahoo.com/", Url.Content("~/Content/images/yahoo.gif")),
	new SelectorProviderButton("https://www.google.com/accounts/o8/id", Url.Content("~/Content/images/google.gif")),
	new SelectorOpenIdButton(Url.Content("~/Content/images/openid.gif")),
}) %>

	<div class="helpDoc">
		<p>
			If you have logged in previously, click the same button you did last time.
		</p>
	</div>

</div>
<% } %>
