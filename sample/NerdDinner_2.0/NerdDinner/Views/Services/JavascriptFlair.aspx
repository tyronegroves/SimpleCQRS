<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<NerdDinner.Models.FlairViewModel>" ContentType="application/x-javascript" %>
<%@ Import Namespace="NerdDinner.Helpers" %>
<%@ Import Namespace="NerdDinner.Models" %>
document.write('<script>var link = document.createElement(\"link\");link.href = \"http://nerddinner.com/content/Flair.css\";link.rel = \"stylesheet\";link.type = \"text/css\";var head = document.getElementsByTagName(\"head\")[0];head.appendChild(link);</script>');
document.write('<div id=\"nd-wrapper\"><h2 id=\"nd-header\">NerdDinner.com</h2><div id=\"nd-outer\">');
<% if (Model.Dinners.Count == 0) { %>
    document.write('<div id=\"nd-bummer\">Looks like there\'s no Nerd Dinners near <%:Model.LocationName %> in the near future. Why not <a target=\"_blank\" href=\"http://www.nerddinner.com/Dinners/Create\">host one</a>?</div>');
<% } else { %>
document.write('<h3>  Dinners Near You</h3><ul>');
    <% foreach (var item in Model.Dinners) { %>
document.write('<li><a target=\"_blank\" href=\"http://nrddnr.com/<%: item.DinnerID %>\"><%: item.Title.Truncate(20) %> with <%: item.HostedBy %> on <%: item.EventDate.ToShortDateString() %></a></li>');
    <% } %>
document.write('</ul>');
<% } %>
document.write('<div id=\"nd-footer\">  More dinners and fun at <a target=\"_blank\" href=\"http://nrddnr.com\">http://nrddnr.com</a></div></div></div>');