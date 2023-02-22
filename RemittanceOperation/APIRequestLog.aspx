<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="APIRequestLog.aspx.cs" Inherits="RemittanceOperation.APIRequestLog" %>

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

    <table style="width: 100%; font-family: Arial">
        <tr>
            <td style="width: 130px">PIN/Reference</td>
            <td>:</td>
            <td style="width: 220px">
                <asp:TextBox ID="txtSearchValReqLog" runat="server" Width="200px"></asp:TextBox></td>
            <td>
                <asp:Button ID="btnSearchRequestLog" runat="server" Text="Search"  Width="100px" OnClientClick="return TxnSearch();" OnClick="btnSearchRequestLog_Click" /></td>
        </tr>
        <tr>
            <td></td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td style="text-align: right"><div class="WaitForSearching">Searching... Please Wait!</div>
                <asp:Label ID="lblRecordCount" runat="server" Font-Size="Small" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="900px" Height="500px">
                    <asp:GridView ID="dataGridViewRequestLogResult" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
            <td>
                <asp:Button ID="btnDownload" runat="server" Text="Download Log" OnClick="btnDownload_Click" />
                &nbsp;&nbsp;<asp:Label ID="lblDownloadMsg" runat="server" Font-Size="Small" ForeColor="Red" Text=""></asp:Label>
            </td>
        </tr>
    </table>

</asp:Content>
