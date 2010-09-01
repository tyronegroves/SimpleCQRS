<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<NerdDinner.Models.Dinner>>" ContentType="text/html" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html401/strict.dtd">

<html>
<head>
    <title><%: ViewData["Title"] %><</title>
</head>
<body style="margin: 0">
    <div class="hslice" id="webslice" style="width: 320px">
        <div class="entry-content">
            <center><h2 class="entry-title"><%: ViewData["Title"] %></h2></center>
            <div>
                <ul>
                    <% foreach (var dinner in Model) { %>
                    <li style="list-style-type: none;">
                        <%: Html.ActionLink(dinner.Title, "Details", new { id=dinner.DinnerID }) %>
                        on <strong>
                            <%: dinner.EventDate.ToString("yyyy-MMM-dd")%>
                            <%: dinner.EventDate.ToString("HH:mm tt")%></strong> at
                        <%: dinner.Address + " " + dinner.Country %>
                    </li>
                    <% } %>
                </ul>
            </div>
        </div>
        <a rel="Bookmark" href="http://www.nerddinner.com" style="display:none;"></a>
    </div>
</body>
</html>
