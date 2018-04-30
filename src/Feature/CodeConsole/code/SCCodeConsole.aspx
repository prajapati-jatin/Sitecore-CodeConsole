<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SCCodeConsole.aspx.cs" Inherits="Sitecore.Feature.CodeConsole.SCCodeConsole" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sitecore code console</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/semantic-ui@2.3.1/dist/semantic.min.css" />
    <link rel="stylesheet" href="front-end/stylesheets/codeconsole.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="ui segment no-border no-shadow">
            <h1 class="ui header">Sitecore code console
            </h1>
        </div>
        <div class="ui grid">
            <div class="ui sixteen wide column">
                <div class="ui segment no-border no-shadow">
                    <div class="ui grid">
                        <div class="ten wide column">
                            <div class="ui segment">
                                <div id="editorUsingStatement"><%=txtUsingStatements.Text %></div>
                                <asp:TextBox ID="txtUsingStatements" CssClass="hidden" placeholder="Add using statements" ClientIDMode="Static" runat="server" TextMode="MultiLine"></asp:TextBox>
                            </div>
                            <div class="ui segment">
                                <pre>namespace SCCodeConsole {<br />&#09;public class SCCode {<br />&#09;}<br />}</pre>
                            </div>
                        </div>
                        <div class="six wide column">
                            <div class="ui segment">List</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js" type="text/javascript"></script>
    <script src="http://cdnjs.cloudflare.com/ajax/libs/ace/1.3.3/ace.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/semantic-ui@2.3.1/dist/semantic.min.js" type="text/javascript"></script>
    <script src="scripts/codeconsole.min.js" type="text/javascript"></script>
</body>
</html>
