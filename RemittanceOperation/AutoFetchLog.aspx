<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AutoFetchLog.aspx.cs" Inherits="RemittanceOperation.AutoFetchLog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.WaitForSearching { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	    .auto-style1 {
            height: 34px;
        }
	</style>
      
    <script type="text/javascript">

        function TxnSearch() {
            $('.WaitForSearching').show();
        }

    </script>

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%; border: solid 1px; margin-left: 15px; margin-top:20px; padding-left:20px; padding-bottom:10px;">
                <tr>
                    <td colspan="2" style="font-weight: bold; text-decoration: underline; text-align: center;">
                        <asp:Label ID="Label2" runat="server" Text="PULL API Fetch Data Log"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="Label3" runat="server" Text="From : "></asp:Label>
                        <asp:TextBox ID="dTPickerFromChk" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                        &nbsp;&nbsp;<asp:Label ID="Label4" runat="server" Text="To : "></asp:Label>
                        <asp:TextBox ID="dTPickerToChk" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" class="auto-style1">
                        <asp:Label ID="Label1" runat="server" Text="Select ExcH : "></asp:Label>
                        <asp:DropDownList ID="comboBoxAPIExh" runat="server"></asp:DropDownList>
                        &nbsp;&nbsp;<asp:Button ID="btnAPIDataSearch" runat="server" Text="Search" Width="120px" OnClick="btnAPIDataSearch_Click" OnClientClick="return TxnSearch();" />
                        <div class="WaitForSearching">Please Wait...!</div>

                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="lblFetchLogRowCount" runat="server" Font-Size="Small"></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="lblFetchLogTime" runat="server" ForeColor="#CC0000" Font-Size="Small"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="900px" Height="400px">
                            <asp:GridView ID="dataGridViewFetchLog" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" OnRowDataBound="dataGridViewFetchLog_RowDataBound" Font-Size="Small">
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

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
