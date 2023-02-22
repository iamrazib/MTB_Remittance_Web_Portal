<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="TxnSearch.aspx.cs" Inherits="RemittanceOperation.TxnSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">       

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.WaitForSearching { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	</style>

    <script type="text/javascript">
        //$(document).ready(function () {
        //    $("#clientScreenWidth").val($(window).width());
        //    $("#clientScreenHeight").val($(window).height());
        //});

        function TxnSearch() {
            $('.WaitForSearching').show();
        }
    </script>

    <%--<input type="hidden" value="" name="clientScreenHeight" id="clientScreenHeight" />
    <input type="hidden" value="" name="clientScreenWidth" id="clientScreenWidth" />--%>

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%; font-family: Arial">
                <tr>
                    <td style="width:130px">PIN/Reference</td>
                    <td>:</td>
                    <td style="width:220px"><asp:TextBox ID="txtBoxPinTxnCheck" runat="server" Width="200px"></asp:TextBox></td>
                    <td><asp:Button ID="btnSearchTxnCheck" runat="server" Text="Search" OnClick="btnSearchTxnCheck_Click" Width="100px" OnClientClick="return TxnSearch();"  /></td>
                </tr>
                <tr>
                    <td>Mobile No.</td>
                    <td>:</td>
                    <td><asp:TextBox ID="txtMobileNoTxnCheck" runat="server" Width="150px"></asp:TextBox></td>
                    <td><asp:Button ID="btnSearchTxnCheckByMobile" runat="server" Text="Search By Mobile" OnClick="btnSearchTxnCheckByMobile_Click" OnClientClick="return TxnSearch();"  /></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td><div class="WaitForSearching">Searching... Please Wait!</div></td>
                </tr>
                <%--<tr>
                    <td></td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td><div class="WaitForSearching">Searching... Please Wait!</div></td>
                </tr>--%>
            </table>
                        
            <br />

            <table style="width: 95%;">
                <tr>
                    <td>
                        <asp:Label ID="lblPaymentModeNameTxnCheckPage" runat="server" Text=""></asp:Label></td>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Label ID="lblTxnCheckNoDataFound" runat="server" ForeColor="Red"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="3">
                        <%--<asp:GridView ID="dGridViewTxnCheckOutputNewSys" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" AllowPaging="True" PageSize="5" ShowHeaderWhenEmpty="True">
                            <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />
                            <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
                            <RowStyle BackColor="White" ForeColor="#330099" />
                            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                            <SortedAscendingCellStyle BackColor="#FEFCEB" />
                            <SortedAscendingHeaderStyle BackColor="#AF0101" />
                            <SortedDescendingCellStyle BackColor="#F6F0C0" />
                            <SortedDescendingHeaderStyle BackColor="#7E0000" />
                        </asp:GridView>--%>

                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="900px" Height="220px">
                            <asp:GridView ID="dGridViewTxnCheckOutputNewSys" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4"  ShowHeaderWhenEmpty="True" Font-Size="Small" OnRowDataBound="dGridViewTxnCheckOutputNewSys_RowDataBound">
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
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text=""></asp:Label></td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Panel ID="Panel2" runat="server" ScrollBars="Both" Width="900px" Height="130px">
                            <asp:GridView ID="dGridViewTxnCheckRippleTable" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" Font-Size="Small">
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
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <%--<tr>
                    <td>
                        <asp:Label ID="lblDataInOldSys" runat="server" Text="Data In Old System :"></asp:Label></td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="3" style="width: auto">
                        <asp:Panel ID="Panel3" runat="server" ScrollBars="Both" Width="900px" Height="130px">
                            <asp:GridView ID="dGridViewTxnCheckOutputOldSys" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4">
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
                </tr>--%>

            </table>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
