<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="GlobalDataCheck.aspx.cs" Inherits="RemittanceOperation.GlobalDataCheck" %>

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

        $(function () {
            $("#products").accordion();
        });
    </script>

    <%--<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>--%>


            <table style="width: 100%; border: solid 1px; margin-left: 15px;">
                <tr>
                    <td colspan="2" style="font-weight: bold; text-decoration: underline; text-align: center;">
                        <asp:Label ID="Label2" runat="server" Text="Exchange House Wise Txn Check"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="Label3" runat="server" Text="From : "></asp:Label>
                        <asp:TextBox ID="dTPickerFromGlobalChk" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                        &nbsp;&nbsp;<asp:Label ID="Label4" runat="server" Text="To : "></asp:Label>
                        <asp:TextBox ID="dTPickerToGlobalChk" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="Label1" runat="server" Text="Select ExH : "></asp:Label>
                        <asp:DropDownList ID="comboBoxGlobalExh" runat="server"></asp:DropDownList>
                        &nbsp;&nbsp;<asp:Button ID="btnGlobalBkashSearch" runat="server" Text="Search" Width="120px" OnClick="btnGlobalBkashSearch_Click" OnClientClick="return TxnSearch();" />
                        <div class="WaitForSearching">Please Wait...!</div>
                    </td>
                </tr>
            </table>
            <br />
            <div id="products" style="width: 950px; margin-left: 15px;">
                <h3><a href="#">bKash</a></h3>
                <div>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblGlobalDataBkashCount" runat="server" ForeColor="Red" Font-Size="Small"></asp:Label>
                                &nbsp;&nbsp;<asp:Label ID="lblGlobalDataBkashUnsuccCount" runat="server" Font-Size="Small"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="900px" Height="200px">
                                    <asp:GridView ID="dataGridViewGlobalBkashTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
                </div>
                <h3><a href="#">BEFTN</a></h3>
                <div>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblGlobalDataBEFTNCount" runat="server" ForeColor="Red" Font-Size="Small"></asp:Label>
                                &nbsp;&nbsp;<asp:Label ID="lblGlobalDataBeftnSuccUnsucsCount" runat="server" Font-Size="Small"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="Panel2" runat="server" ScrollBars="Both" Width="900px" Height="200px">
                                    <asp:GridView ID="dataGridViewGlobalBEFTNTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
                </div>
                <h3><a href="#">MTB A/C</a></h3>
                <div>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblGlobalDataMTBCount" runat="server" ForeColor="Red" Font-Size="Small"></asp:Label>
                                <%--&nbsp;&nbsp;<asp:Label ID="lblGlobalDataMTBSuccCount" runat="server" Text=""></asp:Label>--%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="Panel3" runat="server" ScrollBars="Both" Width="900px" Height="200px">
                                    <asp:GridView ID="dataGridViewGlobalMTBTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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

                </div>
                <h3><a href="#">CASH</a></h3>
                <div>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblGlobalDataCASHCount" runat="server" ForeColor="Red" Font-Size="Small"></asp:Label>
                                <%--&nbsp;&nbsp;<asp:Label ID="lblGlobalDataMTBSuccCount" runat="server" Text=""></asp:Label>--%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="Panel4" runat="server" ScrollBars="Both" Width="900px" Height="200px">
                                    <asp:GridView ID="dataGridViewGlobalCASHTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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

                </div>
            </div>

        <%--</ContentTemplate>
    </asp:UpdatePanel>--%>

</asp:Content>
