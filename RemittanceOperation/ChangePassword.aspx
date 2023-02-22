<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="RemittanceOperation.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%; font-family: Arial">                
                <tr>
                    <td>Logged in as</td>
                    <td>:</td>
                    <td>
                        <asp:Label ID="S_CURRENT_USERID" runat="server" Text="" /></td>
                </tr>
                <tr>
                    <td>Current Password</td>
                    <td>:</td>
                    <td>
                        <asp:TextBox ID="txtCurrPassword" runat="server" TextMode="Password"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>New Password</td>
                    <td>:</td>
                    <td>
                        <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Confirm Password</td>
                    <td>:</td>
                    <td>
                        <asp:TextBox ID="txtConfPassword" runat="server" TextMode="Password"></asp:TextBox></td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td>
                        <asp:Button ID="btnChangePass" runat="server" Text="Change Password" OnClick="btnChangePass_Click" /></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                    <td style="color: #FF0000">
                        <asp:Label ID="lblErrorMsg" runat="server" Text=""></asp:Label></td>
                </tr>
            </table>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
