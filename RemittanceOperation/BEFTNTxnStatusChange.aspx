<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BEFTNTxnStatusChange.aspx.cs" Inherits="RemittanceOperation.BEFTNTxnStatusChange" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.WaitForSearching { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
        .pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:40%;font-size:large}
	</style>

    <script type="text/javascript">
        function TxnSearch() {
            $('.WaitForSearching').show();
        }

        function ConfirmMarkPrincipal() {
            if (confirm("Are You Sure to Change Principal Txn Status ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }

        function ConfirmMarkIncentive() {
            if (confirm("Are You Sure to Change Incentive Txn Status ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }
     </script>


    <table style="width: 100%; margin-left:10px; margin-top:10px;">
        <tr>
            <td colspan="3" style="font-weight: bold; text-decoration: underline; text-align:center;">
                <asp:Label ID="Label2" runat="server" Text="BEFTN Status Change For Pending Transactions"></asp:Label>
            </td>
        </tr>
        <tr>
            <td rowspan="5" style="vertical-align:top; width:170px; ">Enter PIN/Reference No:</td>
            <td rowspan="5" style="vertical-align:top; width:200px;">
                <asp:TextBox ID="textBoxRefNo" runat="server" Height="208px" TextMode="MultiLine" style="resize:none"></asp:TextBox></td>
            <td>
                <asp:Button ID="btnCheckBeftnPrincipalTxn" runat="server" Text="Check BEFTN (Principal Txn)"  OnClientClick="return TxnSearch();" OnClick="btnCheckBeftnPrincipalTxn_Click" /></td>
        </tr>
        <tr>
            <td><asp:Button ID="btnChangeStatusPrincipalTxn" runat="server" Text="Change Status (Principal Txn) to Received"  OnClientClick="return ConfirmMarkPrincipal();" OnClick="btnChangeStatusPrincipalTxn_Click" />
                &nbsp;&nbsp;<asp:Label ID="lblChangeStatusMainSuccessMsg" runat="server" Text=""></asp:Label>
            </td>                    
        </tr>
        <tr>
            <td>&nbsp;</td>                    
        </tr>
        <tr>
            <td><asp:Button ID="btnCheckBeftnIncentive" runat="server" Text="Check BEFTN (Incentive Txn)"  OnClientClick="return TxnSearch();" OnClick="btnCheckBeftnIncentive_Click" /></td>                    
        </tr>
        <tr>
            <td><asp:Button ID="btnChangeStatusIncentiveTxn" runat="server" Text="Change Status (Incentive Txn) to Received"  OnClientClick="return ConfirmMarkIncentive();" OnClick="btnChangeStatusIncentiveTxn_Click" />
                &nbsp;&nbsp;<asp:Label ID="lblChangeStatusIncentiveSuccessMsg" runat="server" Text=""></asp:Label>
            </td>                    
        </tr>
    </table>
    <div class="WaitForSearching">Please Wait...!</div>
            <br /><br />

    <table style="width: 100%; margin-left:10px;">
        <tr>
            <td><asp:Label ID="lblOp" runat="server" Text="Output :"></asp:Label></td>
            <td><asp:Label ID="lblFoundRecordNo" runat="server" ForeColor="Red"></asp:Label></td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="930px" Height="300px">
                    <asp:GridView ID="dgviewFoundRecord" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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
    <br /><br />

</asp:Content>
