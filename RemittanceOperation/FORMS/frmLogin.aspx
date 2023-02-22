<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="frmLogin.aspx.cs" Inherits="RemittanceOperation.FORMS.frmLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="background-color:bisque">
    <form id="form1" runat="server">
        <div style="border: solid 0px red; height: 35px; text-align: center; font-weight: bold; font-size:larger; text-transform:uppercase; margin-top:20px;">
            Remittance Management Solution
        </div>
        <div style="text-align: center; margin-top: 50px">
            <table style="width: 400px; margin-left: auto; margin-right: auto; background-color:white">
                <tr>
                    <td colspan="2" style="font-weight: bold; text-decoration: underline; color: red">LOGIN</td>
                </tr>
                <tr>
                    <td style="text-align: right;">User ID : </td>
                    <td style="text-align: left;">
                        
                        <asp:DropDownList ID="ddlUserId" runat="server" AutoPostBack="True">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">Password : </td>
                    <td style="text-align: left;">
                        <asp:TextBox ID="txtUserPasswd" runat="server" TextMode="Password"></asp:TextBox></td>
                </tr>
                <tr style="">
                    <td></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="btnLogin" runat="server" Text="Log In" Width="100px" OnClick="btnLogin_Click" /></td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="2" style="color:red">
                        <asp:Label ID="lblLoginMessage" runat="server" Text=""></asp:Label></td>
                </tr>
            </table>
        </div>
    </form>

</body>
</html>
