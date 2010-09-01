<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<NerdDinner.Models.Dinner>" MasterPageFile="~/Views/Shared/Site.Master"  %>
<%@ Import Namespace="NerdDinner.Helpers" %>
<asp:Content ID="Title" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Title %>
</asp:Content>

<asp:Content ID="details" ContentPlaceHolderID="MainContent" runat="server">

    <div id="dinnerDiv" class="vevent">

        <h2 class="summary"><%: Model.Title %></h2>

        <div id="share">
        <strong>Share: </strong>
        <a href="http://twitter.com/home/?source=nerddinner&status=Nerd+Dinner%3A+<%: Model.Title.Truncate(40) %>+on+<%: Model.EventDate.ToString("MMM dd") %>+-+RSVP%3A+http%3A%2F%2Fnrddnr.com/<%: Model.DinnerID %>"  
            title="Tweet this" target="_blank">
            <img src="/Content/Img/icon-twitter.png" alt="Twitter" border="0" />
        </a><a href="http://www.facebook.com/share.php?u=http%3A%2F%2Fnrddnr.com/<%: Model.DinnerID %>"  
            title="Add to Facebook" target="_blank">
            <img src="/Content/Img/icon-facebook.png" alt="Facebook" border="0" />
        </a><a href="http://www.google.com/reader/link?url=http%3A%2F%2Fnrddnr.com/<%: Model.DinnerID %>&title=Nerd+Dinner%3A+<%: Model.Title %>+on+<%: Model.EventDate.ToString("MMM dd, yyyy") %>&snippet=<%: Model.Description %>+%3Cbr+%2F%3E%0D%0A%3Cbr+%2F%3E%3Cbr+%2F%3E%3Cstrong%3EWhere%3F%3C%2Fstrong%3E%3Cbr+%2F%3E<%: Model.Address %>%3C%2Fstrong%3E%3Cbr+%2F%3E%3Cbr+%2F%3E%0D%0A%09%09%09%3Cstrong%3EWhen%3F%3C%2Fstrong%3E%3Cbr+%2F%3E%3Cstrong%3E<%: Model.EventDate.ToString("MMM dd, yyyy") %>%3C%2Fstrong%3E%3Cbr+%2F%3E%3Ca+href%3D%22http%3A%2F%2Fnrddnr.com/<%: Model.DinnerID %>%22++title%3D%22RSVP+here%21%22+%3ERSVP+here%21%3C%2Fa%3E&srcURL=http%3A%2F%2Fnrddnr.com/<%: Model.DinnerID %>&srcTitle=Twtvite"  title="Add to Google Buzz" target="_blank">
            <img src="/Content/Img/icon-google.png" alt="Google Buzz" border="0" />
        </a><input name="share_link" type="text" value="http://nrddnr.com/<%: Model.DinnerID %>" class="widget" onclick="this.select()" size="15"/>
         </div>

        
        <p>
            <%: Html.ActionLink("Add event to your calendar:", "iCal", "Services", new { id = Model.DinnerID }, null) %> 
            <a href="<%:Url.Action("iCal","Services", new { id = Model.DinnerID }, null) %>"><img src="/Content/Img/icon-icalfeed-16x16.png" /></a>
        </p>
        
        <p>
            <strong>When:</strong> 
            <abbr class="dtstart" title="<%: Model.EventDate.ToString("s") %>">
                <%: Model.EventDate.ToString("MMM dd, yyyy") %> 
                <strong>@</strong>
                <%: Model.EventDate.ToShortTimeString() %>
            </abbr>
        </p>
        
        <p>
            <strong>Where:</strong>
            <span class="location adr">
                <span class="extended-address"><%: Model.Address %></span>, 
                <span class="country-name"><%: Model.Country %></span>
                <abbr class="geo" title="<%: Model.Latitude %>;<%: Model.Longitude %>" style="display: none;">Geolocation for hCalendar</abbr>
            </span>
        </p>
        
        <p>
            <strong>Description:</strong> 
            <span class="description"><%: Model.Description %></span>
            <span style="display: none;">
                <%: Html.ActionLink("URL for hCalendar", "iCal", "Services", new { id = Model.DinnerID }, new { @class = "url" })%>
            </span>
        </p>
            
        <p>
            <strong>Organizer:</strong>
            <span class="organizer">
                <span class="vcard">
                    <span class="fn nickname"><%: Model.HostedBy %></span>
                    <span class="tel"> <%: Model.ContactPhone %></span>
                </span>                
            </span>
        </p>
        
        <% Html.RenderPartial("RSVPStatus"); %>
        
        <p id="whoscoming">
            <strong>Who's Coming:</strong>
            <%if (Model.RSVPs.Count == 0){%>
                  No one has registered.
            <% } %>
        </p>
        
        <%if(Model.RSVPs.Count > 0) {%>
           <div id="whoscomingDiv">
            <ul class="attendees">
                <%
                    var RSVPs = Model.RSVPs.Reverse();      
                    foreach (var RSVP in RSVPs){%>
                  <li class="attendee">
                    <span class="vcard">
                        <span class="fn nickname"><%:RSVP.AttendeeName.StartsWith("@") ? RSVP.AttendeeName : RSVP.AttendeeName.Replace("@"," at ").Truncate(30) %></span>
                    </span>
                  </li>
                <% } %>
            </ul>
          </div>
        <%} %>
        
        <% Html.RenderPartial("EditAndDeleteLinks"); %>    
        
    </div>
    
    <div id="mapDiv">
        <% Html.RenderPartial("map"); %>    
        <p>
             <img src="/content/img/microformat_hcalendar.png" alt="hCalendar"/>
        </p>
    </div>   

<div id="disqus_thread"></div>

<script type="text/javascript">
    var disqus_url = '<%: "http://www.nerddinner.com/" + Model.DinnerID.ToString() %>';
    var disqus_developer = 1;

    (function () {
        var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
        dsq.src = 'http://nerddinner.disqus.com/embed.js';
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
    })();
</script>
<noscript>Please enable JavaScript to view the <a href="http://disqus.com/?ref_noscript=nerddinner">comments powered by Disqus.</a></noscript>
<a href="http://disqus.com" class="dsq-brlink">blog comments powered by <span class="logo-disqus">Disqus</span></a>

         
</asp:Content> 

