<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<FlairViewModel>" %>
<%@ Import Namespace="NerdDinner.Helpers" %>
<%@ Import Namespace="NerdDinner.Models" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <title>Nerd Dinner</title>
        <link href="/Content/Flair.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <div id="nd-wrapper">
            <h2 id="nd-header">NerdDinner.com</h2>
            <div id="nd-outer">
                <% if (Model.Dinners.Count == 0) { %>
                <div id="nd-bummer">
                    Looks like there's no Nerd Dinners near
                    <%:Model.LocationName %>
                    in the near future. Why not <a target="_blank" href="http://www.nerddinner.com/Dinners/Create">host one</a>?</div>
                <% } else { %>
                <h3>
                    Dinners Near You</h3>
                <ul>
                    <% foreach (var item in Model.Dinners) { %>
                    <li>
                        <%: Html.ActionLink(String.Format("{0} with {1} on {2}", item.Title.Truncate(20), item.HostedBy, item.EventDate.ToShortDateString()), "Details", "Dinners", new { id = item.DinnerID }, new { target = "_blank" })%></li>
                    <% } %>
                </ul>
                <% } %>
                <div id="nd-footer">
                    More dinners and fun at <a target="_blank" href="http://nrddnr.com">http://nrddnr.com</a></div>
            </div>
        </div>
    </body>
</html>
