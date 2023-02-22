<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EFTReturnEmailList.aspx.cs" Inherits="RemittanceOperation.EFTReturnEmailList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.WaitForSearching { width: 200px; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
        .pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	</style>

    <script type="text/javascript">
        function ConfirmUpdate() {
            if (confirm("Are You Sure to Update ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }
    </script>


    <table style="width: 100%;">
        <tr>
            <td colspan="3" style="font-weight: bold; text-decoration: underline; text-align:center;">
                <asp:Label ID="Label1" runat="server" Text="Return Email Addresses"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <div style="text-align: center;">
                    <asp:Label ID="lblUserAuthorizationMsg" runat="server" Text=""></asp:Label>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Button ID="btnSearchReturnEmails" runat="server" Text="Search Again" OnClick="btnSearchReturnEmails_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="900px" Height="400px">
                    <asp:GridView ID="dGridViewReturnEmailAddrs" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" AutoGenerateSelectButton="True"  >
                        <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />
                        <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
                        <RowStyle BackColor="White" ForeColor="#330099" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                        <SortedAscendingCellStyle BackColor="#FEFCEB" />
                        <SortedAscendingHeaderStyle BackColor="#AF0101" />
                        <SortedDescendingCellStyle BackColor="#F6F0C0" />
                        <SortedDescendingHeaderStyle BackColor="#7E0000" />
                    </asp:GridView>
                </asp:Panel>
            </td>
        </tr>
    </table>
    <br />
    <%--<div style="border: solid 1px; padding-left:15px;">
        <asp:Label ID="Label2" runat="server" Text="UPDATE Email" ForeColor="DarkRed" Font-Underline="true"></asp:Label>
        <table style="width:100%;">
            <tr>
                <td style="width:110px;">SL No. :</td>
                <td style="ba"><asp:TextBox ID="txtAutoId" runat="server" ReadOnly="true" BackColor="#CCCCCC"></asp:TextBox></td>   
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>PartyId :</td>
                <td><asp:TextBox ID="txtPartyId" runat="server" ReadOnly="true" BackColor="#CCCCCC"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>UserId :</td>
                <td><asp:TextBox ID="txtUserId" runat="server" ReadOnly="true" BackColor="#CCCCCC"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>ExH Name :</td>
                <td><asp:TextBox ID="txtExhName" runat="server" Width="250px" ReadOnly="true" BackColor="#CCCCCC"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>To Address :</td>
                <td><asp:TextBox ID="txtToAddress" runat="server" TextMode="MultiLine" Width="360px" style="resize:none"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>CC Address :</td>
                <td><asp:TextBox ID="txtCCAddress" runat="server" TextMode="MultiLine" Width="360px" style="resize:none"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btnUpdateMTOReturnEmail" runat="server" Text="Update" Width="120px" OnClientClick="return ConfirmUpdate();" OnClick="btnUpdateMTOReturnEmail_Click" />
                    &nbsp;<asp:Label ID="lblUpdateSuccMsg" runat="server" Text="" ForeColor="Green"></asp:Label>
                    <asp:Label ID="lblUpdateErrorMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
                <td><div class="pleaseWait">Processing ... Please Wait!</div></td>
            </tr>
        </table>

    </div>--%>
    <br /><br />
</asp:Content>
