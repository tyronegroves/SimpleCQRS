<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="NerdDinner" %>
<%
    if (Request.IsAuthenticated) {
%>
        Welcome <b><%: ((NerdIdentity)Page.User.Identity).FriendlyName %></b>!
        [ <%: Html.ActionLink("Log Off", "LogOff", "Account") %> ]
<%
    }
    else {
%> 
        [ <%: Html.ActionLink("Log On", "LogOn", new { controller = "Account", returnUrl = HttpContext.Current.Request.RawUrl }) %> ]
<%
    }
%>
