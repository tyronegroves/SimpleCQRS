<%@ Language="C#" Inherits="System.Web.Mvc.ViewUserControl<NerdDinner.Models.DinnerFormViewModel>" %>

<script src="/Scripts/MicrosoftAjax.js" type="text/javascript"></script>
<script src="/Scripts/MicrosoftMvcAjax.js" type="text/javascript"></script>
<script src="/Scripts/MicrosoftMvcValidation.js" type="text/javascript"></script>

<% Html.EnableClientValidation(); %>
<%: Html.ValidationSummary("Please correct the errors and try again.") %>

<% using (Html.BeginForm()) { %>
    <fieldset>

        <div id="dinnerDiv">

        <p>
            <label for="Title">Dinner Title:</label>
            <%: Html.EditorFor(m => Model.Dinner.Title) %>
        </p>
        <p>
            <label for="EventDate">Event Date:</label>
            <%: Html.EditorFor(m => m.Dinner.EventDate) %>
        </p>
        <p>
            <label for="Description">Description:</label>
            <%: Html.TextAreaFor(m => Model.Dinner.Description, 6, 35, null) %>
        </p>
        <p>
            <label for="Address">Address:</label>
            <%: Html.EditorFor(m => Model.Dinner.Address)%>
        </p>
        <p>
            <label for="Country">Country:</label>
            <%: Html.DropDownList("Dinner.Country", Model.Countries) %>                
        </p>
        <p>
            <label for="ContactPhone">Contact Info:</label>
            <%: Html.EditorFor(m => Model.Dinner.ContactPhone)%>
        </p>
        <p>
            <%: Html.HiddenFor(m => Model.Dinner.Latitude)%>
            <%: Html.HiddenFor(m => Model.Dinner.Longitude)%>
        </p>                 
        <p>
            <input type="submit" value="Save" />
        </p>
            
        </div>
        
        <div id="mapDiv">    
            <% Html.RenderPartial("Map", Model.Dinner); %>
            (drag the pin in the map if it doesn't look right)
        </div> 
            
    </fieldset>

<% } %>


<script type="text/javascript">
//<![CDATA[
    $(document).ready(function () {
        NerdDinner.EnableMapMouseClickCallback();

        $("#Dinner_Address").blur(function (evt) {
            //If it's time to look for an address, 
            // clear out the Lat and Lon
            $("#Dinner_Latitude").val("0");
            $("#Dinner_Longitude").val("0");
            var address = jQuery.trim($("#Dinner_Address").val());
            if (address.length < 1)
                return;
            NerdDinner.FindAddressOnMap(address);
        });
    });
//]]>
</script>

