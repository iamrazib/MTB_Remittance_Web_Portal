<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BulkTxnCheck.aspx.cs" Inherits="RemittanceOperation.BulkTxnCheck" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.WaitForSearching { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	</style>

    <script type="text/javascript">
        function TxnSearch() {
            $('.WaitForSearching').show();
        }
     </script>


    <%--<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>--%>

            <table style="width: 100%;">
                <tr>
                    <td rowspan="5" style="vertical-align:top; width:170px; ">Enter PIN/Reference No:</td>
                    <td rowspan="5" style="vertical-align:top; width:200px;">
                        <asp:TextBox ID="textBoxRefNo" runat="server" Height="208px" TextMode="MultiLine" style="resize:none"></asp:TextBox></td>
                    <td>
                        <asp:Button ID="btnCheckBeftnTable" runat="server" Text="Check BEFTN" OnClick="btnCheckBeftnTable_Click" OnClientClick="return TxnSearch();" /></td>
                </tr>
                <tr>
                    <td><asp:Button ID="btnCheckMtbAcTable" runat="server" Text="Check MTB Ac Credit" OnClick="btnCheckMtbAcTable_Click" OnClientClick="return TxnSearch();" /></td>                    
                </tr>
                <tr>
                    <td><asp:Button ID="btnCheckBkashTable" runat="server" Text="Check bKash Txn" OnClick="btnCheckBkashTable_Click" OnClientClick="return TxnSearch();" /></td>                    
                </tr>
                <tr>
                    <td><asp:Button ID="btnCheckCashTable" runat="server" Text="Check Cash Txn" OnClick="btnCheckCashTable_Click" OnClientClick="return TxnSearch();" /></td>                    
                </tr>
                <tr>
                    <td><asp:Button ID="btnCheckBulkBkashDirectTable" runat="server" Text="Check bKash Direct Txn" OnClick="btnCheckBulkBkashDirectTable_Click" OnClientClick="return TxnSearch();" /></td>                    
                </tr>
            </table>
            <div class="WaitForSearching">Please Wait...!</div>
            <asp:Label ID="lblOnProgs" runat="server" Text=""></asp:Label>
            <br /><br />
            <table style="width: 100%;">
                <tr>
                    <td><asp:Label ID="lblOp" runat="server" Text="Output :"></asp:Label></td>
                    <td><asp:Label ID="lblFoundRecordNo" runat="server" ForeColor="Red"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="900px" Height="170px">
                            <asp:GridView ID="dgviewFoundRecord" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small" OnRowDataBound="dgviewFoundRecord_RowDataBound">
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
            <table style="width: 100%;">
                <tr>
                    <td><asp:Label ID="Label1" runat="server" Text="NOT Found Record :"></asp:Label></td>
                    <td><asp:Label ID="lblNotFoundRecordNo" runat="server" ForeColor="Red"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Panel ID="Panel2" runat="server" ScrollBars="Both" Width="900px" Height="170px">
                            <asp:GridView ID="dgviewNotFoundRecord" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
        
        <%--</ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
