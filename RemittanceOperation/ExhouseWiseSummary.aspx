<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ExhouseWiseSummary.aspx.cs" Inherits="RemittanceOperation.ExhouseWiseSummary" %>
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

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%; margin-left:30px;">
                <tr>
                    <td colspan="4" style="font-weight: bold; text-decoration: underline; text-align:center;">
                        <asp:Label ID="Label1" runat="server" Text="Exchange House Wise Summary"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2" style="width:490px;">
                        <asp:Label ID="Label3" runat="server" Text="From : "></asp:Label>
                        <asp:TextBox ID="dTPickerFromSumrExchWise" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                        
                        &nbsp;&nbsp;<asp:Label ID="Label4" runat="server" Text="To : "></asp:Label>
                        <asp:TextBox ID="dTPickerToSumrExchWise" runat="server" Width="120px" TextMode="Date"></asp:TextBox>                       
                        &nbsp;&nbsp;
                        
                    </td>                    
                    <td>                        
                    </td>
                    <td>                        
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="Select ExH :"></asp:Label>
                        &nbsp;&nbsp;<asp:DropDownList ID="cbExchWiseSumr" runat="server"></asp:DropDownList>
                        &nbsp;&nbsp;
                        <asp:Button ID="btnExchWiseSumrSearch" runat="server" Text="Search" Width="100px" OnClientClick="return TxnSearch();" OnClick="btnExchWiseSumrSearch_Click" />
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

                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Width="600px" Height="300px">
                            <asp:GridView ID="dataGridViewSumrExchWise" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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
                    <td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
                </tr>
                               
            </table>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
