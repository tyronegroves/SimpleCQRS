<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.DateTime>" %>
     <%: Html.TextBox("", String.Format("{0:yyyy-MM-dd HH:mm}",Model)) %>
         <script type="text/javascript">
             $(function () {
                 $('#EventDate').datetime({
                     userLang: 'en',
                     americanMode: true
                 });
             });
            </script>
