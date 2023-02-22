<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="MTBAcTxn.aspx.cs" Inherits="RemittanceOperation.MTBAcTxn" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.WaitForSearching { width: 200px; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
        .pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	</style>

    <script type="text/javascript">
        function TxnSearch() {
            $('.WaitForSearching').show();
        }

    </script>

    <table style="width: 100%; margin-left:15px;">
                <tr>
                    <td colspan="4" style="font-weight: bold; text-decoration: underline; text-align:center;">
                        <asp:Label ID="Label1" runat="server" Text="MTB A/C Transactions"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2" style="width:490px;">
                        <asp:Label ID="Label3" runat="server" Text="From : "></asp:Label>
                        <asp:TextBox ID="dtpickerFrom" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                        
                        &nbsp;&nbsp;<asp:Label ID="Label4" runat="server" Text="To : "></asp:Label>
                        <asp:TextBox ID="dtpickerTo" runat="server" Width="120px" TextMode="Date"></asp:TextBox>                       
                        &nbsp;&nbsp;
                        <asp:Button ID="btnSearchMTBAcTxn" runat="server" Text="Search" Width="100px"  OnClientClick="return TxnSearch();" OnClick="btnSearchMTBAcTxn_Click"  />&nbsp;&nbsp;
                        
                    </td>                    
                    <td>                        
                    </td>
                    <td>                        
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td><div class="WaitForSearching">Searching... Please Wait!</div></td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="4">

                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Width="900px" Height="300px">
                            <asp:GridView ID="dataGridViewMTBAcTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
                        <asp:Label ID="lblTotalRec" runat="server" Text="" ></asp:Label>
                    </td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
                </tr>
                               
            </table>

    <br />
    <div style="margin-left:20px;">
        <asp:Button ID="btnDownloadMTBAcTxnAsExcel" runat="server" Text="Download As Excel" Width="150px" OnClick="btnDownloadMTBAcTxnAsExcel_Click"  />
    </div>
    
    <br />
    <br />

</asp:Content>
