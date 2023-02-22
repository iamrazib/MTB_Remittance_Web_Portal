<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="NBLFailedTxnReport.aspx.cs" Inherits="RemittanceOperation.NBLFailedTxnReport" %>
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
                        <asp:Label ID="Label1" runat="server" Text="NBL Pending/Failed Transaction List"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2" style="width:490px;">
                        <asp:Label ID="Label3" runat="server" Text="From : "></asp:Label>
                        <asp:TextBox ID="dTPickerFrom" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                        
                        &nbsp;&nbsp;<asp:Label ID="Label4" runat="server" Text="To : "></asp:Label>
                        <asp:TextBox ID="dTPickerTo" runat="server" Width="120px" TextMode="Date"></asp:TextBox>                       
                        &nbsp;&nbsp;
                        
                    </td>                    
                    <td>                        
                    </td>
                    <td>                        
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="Exchange House: "></asp:Label>
                        &nbsp;&nbsp;
                        <asp:DropDownList ID="ddlExh" runat="server">
                            <asp:ListItem Selected="True" Value="065">NBL Singapore</asp:ListItem>
                            <asp:ListItem Value="060">NBL Malaysia</asp:ListItem>
                        </asp:DropDownList>
                        &nbsp;&nbsp;
                        <asp:Button ID="btnSearch" runat="server" Text="Search" Width="100px" OnClientClick="return TxnSearch();" OnClick="btnSearch_Click"   />
                    </td>
                    <td></td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td><div class="WaitForSearching">Searching... Please Wait!</div></td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Width="900px" Height="250px">
                            <asp:GridView ID="dataGridViewNBLFailedTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
                        <asp:Label ID="lblRowCount" runat="server" Font-Bold="True"></asp:Label>
                        </td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
                </tr>
                               
            </table>

    <div style="clear:both; float:left; margin:10px 0 25px 15px">
        <asp:Button ID="btnDownloadNBLFailedTxn" runat="server" Text="Download As Excel" OnClick="btnDownloadNBLFailedTxn_Click"  />        
    </div>
    <br /><asp:Label ID="lblDownloadMsg" runat="server" Text=""></asp:Label><br />
</asp:Content>
