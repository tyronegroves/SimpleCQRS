<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<NerdDinner.Models.Dinner>" %>

<ul id="Details">
    <li>Title: <%: Model.Title %></li>
        <li>When: <%: Model.EventDate.ToShortDateString() %> @ <%: Model.EventDate.ToShortTimeString() %></li>
    <li><a target="_self" href="<%=String.Format("http://maps.google.com/maps?q={0},{1}",Url.Encode(Model.Address),Url.Encode(Model.Country)) %>">Where: <%: Model.Address %>, <%: Model.Country %></a></li>
    <li>Description: <%: Model.Description %></li>
    <li><a target="_self" href="tel://<%: Model.ContactPhone %>">Organizer: <%: Model.HostedBy %> (<%: Model.ContactPhone %>)</a></li>
    <li><a href="#whoscoming">Who's Coming?</a></li>
</ul>

<div id="whoscoming" title="<%: Model.Title %>" class="panel">
    <h2>Who's Coming?</h2>
            <%if (Model.RSVPs.Count == 0){%>
                  <ul><li>No one has registered.</li></ul>
            <% } %>

        <%if(Model.RSVPs.Count > 0) {%>
        <ul id="attendees">
            <%foreach (var RSVP in Model.RSVPs){%>
              <li><%: RSVP.AttendeeName.Replace("@"," at ") %></li>    
            <% } %>
        </ul>
        <%} %>
</div>