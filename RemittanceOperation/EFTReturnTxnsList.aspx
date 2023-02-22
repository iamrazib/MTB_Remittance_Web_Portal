<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EFTReturnTxnsList.aspx.cs" Inherits="RemittanceOperation.EFTReturnTxnsList" %>
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

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%; border: solid 1px; margin-top:20px; margin-left:15px;">

                <tr>
                    <td colspan="2" style="font-weight: bold; text-decoration: underline; text-align: center;">
                        <asp:Label ID="Label2" runat="server" Text="RETURN Txn List"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        &nbsp;<asp:Label ID="Label3" runat="server" Text="Date : "></asp:Label>
                        &nbsp;<asp:TextBox ID="dTPickerFromReturnList" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                        &nbsp;&nbsp;<asp:Label ID="Label4" runat="server" Text="To : "></asp:Label>
                        <asp:TextBox ID="dTPickerToReturnList" runat="server" Width="120px" TextMode="Date"></asp:TextBox>

                        &nbsp;&nbsp;
                        <asp:Button ID="btnSearchReturnTxnList" runat="server" Text="Search" Width="120px" OnClick="btnSearchReturnTxnList_Click" />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="Panel3" runat="server" ScrollBars="Both" Width="900px" Height="400px">  
                            <asp:GridView ID="dataGridViewReturnTxnList" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
                    <td style="text-align:center;">
                        <asp:Label ID="lblTotalRows" runat="server" Text=""></asp:Label></td>
                </tr>
            </table>

        </ContentTemplate>
    </asp:UpdatePanel>

    <div style="clear:both; text-align:center; margin:10px 0 25px 15px">
        <asp:Button ID="btnDownloadReturnTxnList" runat="server" Text="Download As Excel" Width="170px" OnClick="btnDownloadReturnTxnList_Click" />
        <%--<div class="WaitForSearching">Please Wait...!</div>--%>
    </div>
    <br /><br />

</asp:Content>
