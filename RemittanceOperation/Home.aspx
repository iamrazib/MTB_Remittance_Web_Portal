<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="RemittanceOperation.Home" %>
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
    <br />
    <table style="width:100%; font-family:Arial">
        <tr>
            <td colspan="3" style="text-align:center; text-decoration:underline; font-weight:bold">Login Successful</td>
        </tr>
        <tr>
            <td>Logged in As</td>
            <td>:</td>
            <td><asp:Label ID="S_CURRENT_USERID" runat="server" Text="" /></td>            
        </tr>
        <tr>
            <td>User Name</td>
            <td>:</td>
            <td><asp:Label ID="S_CURRENT_USER_FULL_NAME" runat="server" Text="" /></td>            
        </tr>
        <tr>
            <td>User Email</td>
            <td>:</td>
            <td><asp:Label ID="S_CURRENT_USER_EMAIL" runat="server" Text="" /></td>            
        </tr>
        <tr>
            <td>Login Time</td>
            <td>:</td>
            <td><asp:Label ID="S_LOGIN_TIME" runat="server" Text="" /></td>            
        </tr>
        <tr>
            <td>SessionID</td>
            <td>:</td>
            <td><asp:Label ID="S_SESSION_ID" runat="server" Text="" /></td>            
        </tr>
    </table>
    
    <br /><br />
    <%--CURRENT_USER_RM: <asp:Label ID="S_CURRENT_USER_RM" runat="server" Text="Label"></asp:Label><br />--%>
    <%--LOGIN_TIME: <asp:Label ID="S_LOGIN_TIME" runat="server" Text="Label"></asp:Label><br />--%>
    <%--SESSION_ID: <asp:Label ID="S_SESSION_ID" runat="server" Text="Label"></asp:Label><br />--%>
    <%--S_CURRENT_USERID: <asp:Label ID="S_CURRENT_USERID" runat="server" Text="Label"></asp:Label><br />--%>
    <%--S_CURRENT_USER_FULL_NAME: <asp:Label ID="S_CURRENT_USER_FULL_NAME" runat="server" Text="Label"></asp:Label><br />--%>
    <%--S_CURRENT_USER_EMAIL: <asp:Label ID="S_CURRENT_USER_EMAIL" runat="server" Text="Label"></asp:Label><br />--%>

</asp:Content>
