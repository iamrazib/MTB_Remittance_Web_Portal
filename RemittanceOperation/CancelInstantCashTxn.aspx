<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CancelInstantCashTxn.aspx.cs" Inherits="RemittanceOperation.CancelInstantCashTxn" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	</style>

    <script type="text/javascript">
        
        function ConfirmCancel() {
            if (confirm("Are You Sure to Cancel ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }
    </script>

    <div style="text-align: center; margin-top: 15px;">
        <asp:Label ID="lblUserAuthorizationMsg" runat="server" Text=""></asp:Label>
    </div>

    <table style="width: 100%; margin-left:10px;">

        <tr>
            <td colspan="3" style="font-weight:bold; text-decoration:underline;">Instant Cash Txn Reject/Cancel</td>
            <%--<td></td>
            <td></td>--%>
        </tr>

        <tr>
            <td style="width: 130px; text-align: right;">PIN/ReferenceNo</td>
            <td>:</td>
            <td><asp:TextBox ID="textBoxRefNo" runat="server" Width="250px"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="width: 130px; text-align: right;"></td>
            <td></td>
            <td><asp:Button ID="btnSearchTxn" runat="server" Text="Search" OnClick="btnSearchTxn_Click"  /></td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="980px" Height="200px">
                    <asp:GridView ID="dataGridViewTxnSearch" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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
        <tr>
            <td colspan="3">             
                &nbsp;&nbsp;<asp:Label ID="lblErrorMsg" runat="server" Text="" ForeColor="Red"></asp:Label>             
            </td>
        </tr>
        <tr>
            <td style="width: 130px; text-align: right;">Remarks</td>
            <td>:</td>
            <td><asp:TextBox ID="textBoxRemarks" runat="server" Width="250px"></asp:TextBox></td>            
        </tr>
        <tr>
            <td style="width: 130px; text-align: right;"></td>
            <td></td>
            <td><asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="100px" OnClick="btnCancel_Click"  OnClientClick="return ConfirmCancel();" /></td>
        </tr>
    </table>

</asp:Content>
