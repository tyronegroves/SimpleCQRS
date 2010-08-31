<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Commands.AddProductToCartCommand>"
    MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content runat="server" ID="Content" ContentPlaceHolderID="TitleContent">
</asp:Content>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="MainContent">
<%using (Html.BeginForm()) %>
<%{%>
<%=Html.EditorForModel()%>
<input type="submit" value="Submit" />
<%}%>
</asp:Content>