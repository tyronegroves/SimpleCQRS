<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<NerdDinner.Models.Dinner>" %>

<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">
	Delete Confirmation: <%:Model.Title %>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <h2>
        Delete Confirmation
    </h2>

    <div>
        <p>Please confirm you want to cancel the dinner titled: 
        <i> <%:Model.Title %>? </i> </p>
    </div>
    
    <% using (Html.BeginForm()) { %>

        <input name="confirmButton" type="submit" value="Delete" />        

    <% } %>

</asp:Content>
