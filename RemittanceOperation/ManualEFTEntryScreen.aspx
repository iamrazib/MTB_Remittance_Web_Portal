<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ManualEFTEntryScreen.aspx.cs" Inherits="RemittanceOperation.ManualEFTEntryScreen" %>
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

        function ConfirmUpload() {
            if (confirm("Are You Sure to Upload ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }

    </script>

        <table style="width: 100%; border: solid 1px; margin-left: 15px;">
            <tr>
                <td colspan="3" style="font-weight: bold; text-decoration: underline; text-align: center;">
                    <asp:Label ID="Label2" runat="server" Text="Manual EFT File Entry"></asp:Label>
                </td>
            </tr>            
            <tr>
                <td style="width:110px;">
                    <asp:Label ID="Label1" runat="server" Text="Select ExH : "></asp:Label>
                </td>
                <td style="width:200px;">                    
                    <asp:DropDownList ID="cbExh" runat="server"></asp:DropDownList>
                    <div class="WaitForSearching">Please Wait...!</div>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td style="width:110px;">
                    <asp:Label ID="Label3" runat="server" Text="Select File : "></asp:Label>
                </td>
                <td style="width:200px;">
                    <asp:FileUpload ID="FileUploadTextBoxExhFile" runat="server" />                    
                </td>
                <td>
                    <asp:Button ID="btnLoadExcelFile" runat="server" Text="Load" Width="100px" OnClick="btnLoadExcelFile_Click" OnClientClick="return TxnSearch();" />
                    <asp:Label ID="lblUserMsg" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td colspan="2">
                    <asp:Panel ID="Panel1" runat="server" BorderWidth="1px" ScrollBars="Both" Width="800px" Height="200px">
                    <asp:GridView ID="dataGridViewExhDataTellerScreen" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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
                <td colspan="2">
                    <asp:Button ID="buttonUploadDataFromTeller" runat="server" Text="Upload" Width="110px" OnClientClick="return ConfirmUpload();" OnClick="buttonUploadDataFromTeller_Click" />
                    &nbsp;&nbsp;<asp:Label ID="lblTellerFileUploadMsg" runat="server" Text=""></asp:Label>
                    <div class="pleaseWait">Processing ... Please Wait!</div>
                </td>
            </tr>
            <tr>
                <td colspan="3">&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td colspan="2">
                    <asp:Label ID="Label4" runat="server" Text="Error List : "></asp:Label><br />
                    <%--<asp:Panel ID="Panel2" runat="server" BorderWidth="1px" ScrollBars="Both" Width="600px" Height="100px">--%>
                        <asp:TextBox ID="listBoxError" runat="server" Height="100px" TextMode="MultiLine" Width="600px" ReadOnly="true" style="resize:none"></asp:TextBox>
                    <%--</asp:Panel>--%>
                </td>
            </tr>
        </table>
    <br />
    <br />

</asp:Content>
