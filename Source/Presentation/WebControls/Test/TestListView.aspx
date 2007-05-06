<%@
    Page
    language="c#"
    AutoEventWireup="true"
%>

<%@ Register
    TagPrefix="atlanta"
    Namespace="Atlanta.Presentation.WebControls"
    Assembly="Atlanta.Presentation" %>

<script runat="server" language="c#">

    private void Page_Load()
    {
        if (!IsPostBack)
        {
            testList1.AddPercentageColumn("Column 1", 30);
            testList1.AddPercentageColumn("Column 2", 30);
            testList1.AddRemainderColumn("Column 3");

            testList1.AddListItem("Value 1", "Value 2", "Value 3");
            testList1.AddListItem("Value 2", "Value 3", "Value 4");
            testList1.AddListItem("Value 3", "Value 4", "Value 5");
            testList1.AddListItem("Value 4", "Value 5", "Value 6");
            testList1.AddListItem("Value 5", "Value 6", "Value 7");
        }
    }

</script>

<html>
    <head>
        <title>Test ListView Harness</title>
        <link
            rel="stylesheet"
            href="<%=Request.ApplicationPath.TrimEnd('/')%>/web/styles/atlanta.css"
            type="text/css"
            />
        <meta http-equiv="Page-Enter" content="progid:DXImageTransform.Microsoft.RandomDissolve(duration=0)" />
        <meta http-equiv="Page-Exit" content="progid:DXImageTransform.Microsoft.RandomDissolve(duration=0)" />
    </head>
    <body>
        <form runat="server">

            <div>testList1:</div>

            <atlanta:ListView
                id="testList1"
                runat="server"
                />

            <asp:Button
                id="button1"
                runat="server"
                text="Press for postback"
                />

        </form>
    </body>
</html>


