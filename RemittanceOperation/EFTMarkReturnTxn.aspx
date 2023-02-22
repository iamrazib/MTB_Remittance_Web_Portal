<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EFTMarkReturnTxn.aspx.cs" Inherits="RemittanceOperation.EFTMarkReturnTxn" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.WaitForSearching { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
        .pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	</style>

    <script type="text/javascript">        
        function TxnSearch() {
            $('.WaitForSearching').show();
        }

        function ConfirmPrincipalReturn() {
            if (confirm("Are You Sure Mark PRINCIPAL Txn ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }

        function ConfirmIncentiveReturn() {
            if (confirm("Are You Sure Mark INCENTIVE Txn ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }
    </script>


    <table style="width: 100%; font-family: Arial">
        <tr>
            <td colspan="4" style="font-weight: bold; text-decoration: underline; text-align: center;">
                <asp:Label ID="Label2" runat="server" Text="EFT Return Mark Manually"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4">&nbsp;</td>
        </tr>
        <tr>
            <td style="width:130px; font-family: Cambria;">PIN/Reference</td>
            <td>:</td>
            <td style="width:220px"><asp:TextBox ID="txtBoxPinTxnCheck" runat="server" Width="200px"></asp:TextBox></td>
            <td><asp:Button ID="btnSearchTxnCheck" runat="server" Text="Search"  Width="100px" OnClientClick="return TxnSearch();" OnClick="btnSearchTxnCheck_Click"  /></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td><div class="WaitForSearching">Searching... Please Wait!</div></td>
        </tr>
    </table>

    

    <table style="width: 95%;">
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td><asp:Label ID="lblTxnCheckNoDataFound" runat="server" ForeColor="Red"></asp:Label></td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="900px" Height="220px">
                    <asp:GridView ID="dGridViewTxnCheckOutput" runat="server" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3"  ShowHeaderWhenEmpty="True" OnRowDataBound="dGridViewTxnCheckOutput_RowDataBound" Font-Size="Small">
                        <FooterStyle BackColor="White" ForeColor="#000066" />
                        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                        <RowStyle ForeColor="#000066" />
                        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                        <SortedAscendingHeaderStyle BackColor="#007DBB" />
                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                        <SortedDescendingHeaderStyle BackColor="#00547E" />
                    </asp:GridView>

                </asp:Panel>
            </td>
        </tr>

    </table>
    <br /><br />
    <table style="width: 100%;">
        <tr>
            <td style="width:100px">Return Date</td>
            <td>:</td>
            <td><asp:TextBox ID="dtPickerReturnDate" runat="server" Width="120px" TextMode="Date"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td style="width:100px">Return Reason</td>
            <td>:</td>
            <td><asp:TextBox ID="txtReturnReason" runat="server" Width="250px"></asp:TextBox></td>
            <td></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>
                <asp:Button ID="btnMarkPrincipalTxnReturn" runat="server" Text="Mark PRINCIPAL Txn Return" OnClick="btnMarkPrincipalTxnReturn_Click" OnClientClick="return ConfirmPrincipalReturn();" />&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnMarkIncentiveTxnReturn" runat="server" Text="Mark INCENTIVE Txn Return" OnClick="btnMarkIncentiveTxnReturn_Click" OnClientClick="return ConfirmIncentiveReturn();" />
                <div class="pleaseWait">Processing ... Please Wait!</div>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td><asp:Label ID="lblMarkReturnStatusMsg" runat="server" Text=""></asp:Label></td>
            <td>&nbsp;</td>
        </tr>
    </table>
    <br />
</asp:Content>
