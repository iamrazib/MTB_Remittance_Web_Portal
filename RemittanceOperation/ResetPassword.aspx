<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="RemittanceOperation.ResetPassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	</style>

    <script type="text/javascript">

        function ConfirmReset() {
            if (confirm("Are You Sure to Reset ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }
     </script>

    <table style="width: 100%; margin-left: 15px; margin-top:15px;">
        <tr>
            <td colspan="2" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label3" runat="server" Text="User Password Reset"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>Select User :</td>
            <td><asp:DropDownList ID="ddlUserId" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td><asp:Button ID="btnUserReset" runat="server" Text="Reset" Width="100px" OnClick="btnUserReset_Click" OnClientClick="return ConfirmReset();" /></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:Label ID="lblStatusMsg" runat="server" Text=""></asp:Label></td>
        </tr>
    </table>

    
    

</asp:Content>
