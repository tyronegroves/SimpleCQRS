<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<NerdDinner.Models.Dinner>" %>

<script src="/Scripts/MicrosoftAjax.js" type="text/javascript"></script>
<script src="/Scripts/MicrosoftMvcAjax.js" type="text/javascript"></script>    

<script type="text/javascript">

    function AnimateRSVPMessage() {
        $("#rsvpmsg").animate({ fontSize: "1.5em" }, 400);
    }

</script>
    
<div id="rsvpmsg">

<% if (Request.IsAuthenticated) { %>

    <% if (Model.IsHostedBy(Context.User.Identity.Name)) { %>

        <p>You are the host for this event!</p>

    <% } else if (Model.IsUserRegistered(Context.User.Identity.Name)) { %>        
    
        <p>You are registered for this event!</p>
    
    <% }
       else
       { %>  
    
        <%: Ajax.ActionLink("RSVP for this event",
                             "Register", "RSVP",
                             new { id = Model.DinnerID },
                             new AjaxOptions { UpdateTargetId = "rsvpmsg", OnSuccess = "AnimateRSVPMessage" })%>         
    <% } %>
    
<% } else { %>

    <strong>RSVP for this event:</strong>
    <a href="<%= Url.Action("RsvpTwitterBegin", "RSVP", new { id = Model.DinnerID }) %>"><img alt="Twitter" src="/Content/Img/icon-twitter.png" border="0" /></a>
    <a href="<%= Url.Action("RsvpBegin", "RSVP", new { id = Model.DinnerID, identifier = "https://www.google.com/accounts/o8/id" }) %>"><img alt="Google" src="/Content/Img/icon-google.png" border="0" /></a>
    <a href="<%= Url.Action("RsvpBegin", "RSVP", new { id = Model.DinnerID, identifier = "https://me.yahoo.com/" }) %>"><img alt="Yahoo!" src="/Content/Img/icon-yahoo.png" border="0" /></a>
<% } %>
    
</div>    