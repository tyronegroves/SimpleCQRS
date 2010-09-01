<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<NerdDinner.Models.Dinner>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	My Dinners
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>My Dinners</h2>

    <ul class="upcomingdinners">
    
        <% foreach (var dinner in Model) { %>
        
            <li>     
                <%: Html.ActionLink(dinner.Title, "Details", new { id=dinner.DinnerID }) %>
                on 
                <strong><%: dinner.EventDate.ToString("yyyy-MMM-dd")%> 
                <%: dinner.EventDate.ToString("HH:mm tt")%></strong>
                at 
                <%: dinner.Address + " " + dinner.Country %>
            </li>
        
        <% } %>

        <% if (Model.Count() == 0) { %>
           <li>You don't own or aren't registered for any dinners.</li>
        <% } %>


    </ul>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadArea" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MastheadContent" runat="server">
</asp:Content>

