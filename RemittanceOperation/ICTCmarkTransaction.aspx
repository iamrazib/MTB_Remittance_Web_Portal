<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ICTCmarkTransaction.aspx.cs" Inherits="RemittanceOperation.ICTCmarkTransaction" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.WaitForSearching { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:40%;font-size:large}
        .pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:40%;font-size:large}
	</style>

    <script type="text/javascript">
        function TxnSearch() {
            $('.WaitForSearching').show();
        }

        function ConfirmMark() {
            if (confirm("Are You Sure to Confirm ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }

        function ConfirmReject() {
            if (confirm("Are You Sure to Reject ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }

    </script>

    <table style="width: 100%; margin-top:20px; margin-left:15px;">
        <tr>
            <td colspan="4" style="font-weight: bold; text-decoration: underline; text-align:center;">
                <asp:Label ID="Label1" runat="server" Text="ICTC Transaction Mark"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="4" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label2" runat="server" Text="Mark Reject Failed Txn"></asp:Label>
            </td>
        </tr>
        <tr style="padding-left:20px;">
            <td>ICTC No:</td>
            <td><asp:TextBox ID="textBoxRefNo" Width="140" runat="server"></asp:TextBox></td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr style="padding-left:20px;">
            <td>Remark:</td>
            <td><asp:TextBox ID="txtRejectRemark" Width="300" runat="server"></asp:TextBox></td>
            <td><asp:Button ID="btnMarkReject" runat="server" Text="Mark Reject" Width="120px"  OnClientClick="return ConfirmReject();" OnClick="btnMarkReject_Click"  /></td>
            <td>&nbsp;</td>
        </tr>   
        <tr>
            <td>&nbsp;</td>
            <td colspan="3">
                <asp:Label ID="lblRejectStatus" runat="server" Text=""></asp:Label>
            </td>
        </tr>     

        <tr>
            <td colspan="4">&nbsp;</td>
        </tr>
        <tr>
            <td colspan="4" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label3" runat="server" Text="Mark Confirm Downloaded Account Credit Txn"></asp:Label>
            </td>
        </tr>
        <tr style="padding-left:20px;">
            <td>ICTC No:</td>
            <td><asp:TextBox ID="textBoxFailedTxn" Width="140" runat="server"></asp:TextBox></td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr style="padding-left:20px;">
            <td>&nbsp;</td>
            <td><asp:Button ID="btnMarkConfirm" runat="server" Text="Mark Confirm" Width="120px"  OnClientClick="return ConfirmMark();" OnClick="btnMarkConfirm_Click"  /></td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td colspan="3">
                <asp:Label ID="lblManualConfirmStatus" runat="server" Text=""></asp:Label>
            </td>
        </tr>  

    </table>
</asp:Content>
