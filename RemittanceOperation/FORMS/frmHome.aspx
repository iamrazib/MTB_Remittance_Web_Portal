<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="frmHome.aspx.cs" Inherits="RemittanceOperation.FORMS.frmHome" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <%--<script type="text/javascript">
        $(document).ready(function () {
            $("#<%=txtDatePicker.ClientID %>").datepicker({
                dateFormat: 'dd-M-yy',
                changeMonth: true,
                changeYear: true,
                yearRange: '1950:2100'
            });
        })

        <asp:TextBox ID="txtDatePicker" runat="server"></asp:TextBox>

    </script>--%>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    webform1
    <br /><br />
    CURRENT_USER_RM: <asp:Label ID="S_CURRENT_USER_RM" runat="server" Text="Label"></asp:Label><br />
    LOGIN_TIME: <asp:Label ID="S_LOGIN_TIME" runat="server" Text="Label"></asp:Label><br />
    SESSION_ID: <asp:Label ID="S_SESSION_ID" runat="server" Text="Label"></asp:Label><br />
    S_CURRENT_USERID: <asp:Label ID="S_CURRENT_USERID" runat="server" Text="Label"></asp:Label><br />
    S_CURRENT_USER_FULL_NAME: <asp:Label ID="S_CURRENT_USER_FULL_NAME" runat="server" Text="Label"></asp:Label><br />
    S_CURRENT_USER_EMAIL: <asp:Label ID="S_CURRENT_USER_EMAIL" runat="server" Text="Label"></asp:Label><br />

</asp:Content>
