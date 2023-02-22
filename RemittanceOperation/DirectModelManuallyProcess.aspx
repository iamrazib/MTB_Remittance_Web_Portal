<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DirectModelManuallyProcess.aspx.cs" Inherits="RemittanceOperation.DirectModelManuallyProcess" %>

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

        function ConfirmProcess() {
            if (confirm("Are You Sure to Process ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }

    </script>

    <%--<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>--%>

    <table style="width: 100%; border: solid 1px; margin-top: 20px; margin-left: 15px;">
        <tr>
            <td colspan="2" style="font-weight: bold; text-decoration: underline; text-align: center;">
                <asp:Label ID="Label2" runat="server" Text="bKash Direct Manually Update Into System"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="MTO Name : "></asp:Label>
                &nbsp;&nbsp;<asp:DropDownList ID="comboBoxDirectMTO" runat="server"></asp:DropDownList>
                &nbsp;&nbsp;<asp:Button ID="btnSearchUnprocsTxn" runat="server" Text="Search Un-Processed Txn" Width="220px" OnClick="btnSearchUnprocsTxn_Click" OnClientClick="return TxnSearch();" />
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td style="text-align: right; padding-right: 20px;">
                <asp:Label ID="lblPendingTxnCount" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Panel ID="Panel3" runat="server" BorderWidth="1px" ScrollBars="Vertical" Width="900px" Height="200px">
                    <%--ScrollBars="Both"--%>
                    <asp:GridView ID="dataGridViewDirectUnprocessedTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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
        <tr style="margin-bottom:10px; margin-top:10px;">
            <td style="width: 200px;">
                <asp:Button ID="btnDownloadExcelUnprocs" runat="server" Text="Download As Excel" Width="200px" OnClick="btnDownloadExcelUnprocs_Click" /></td>
            <td>
                <asp:Label ID="lblFileSaveProgress" runat="server" Text=""></asp:Label>
                <div class="WaitForSearching">Searching... Please Wait!</div>
            </td>
        </tr>
    </table>
    
    <br />
    <table style="width: 100%; border: solid 1px; margin-top: 20px; margin-left: 15px;">
        <tr>
            <td colspan="2" style="font-weight: bold; text-decoration: underline; text-align: left;">
                <asp:Label ID="Label3" runat="server" Text="Update System"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="Label8" runat="server" Text="Select : "></asp:Label>
                &nbsp;&nbsp;
                <asp:FileUpload ID="FileUpload1" runat="server" />
                &nbsp;&nbsp;
                 <asp:Button ID="btnLoadDirectFile" runat="server" Text="Load" Width="80px" OnClick="btnLoadDirectFile_Click" />
                &nbsp;&nbsp;
                <asp:Label ID="lblFileLoadMsg" runat="server" Text="" ForeColor="Green"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="Label4" runat="server" Font-Bold="true" Text="Select MTO : "></asp:Label>
                &nbsp;&nbsp;<asp:DropDownList ID="ddlDirectMTO" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlDirectMTO_SelectedIndexChanged"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="width: 80px;">
                <asp:Label ID="Label5" runat="server" Text="From A/C :"></asp:Label></td>
            <td>
                <asp:TextBox ID="txtFromAcc" runat="server" Width="150px"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="width: 80px;">
                <asp:Label ID="Label6" runat="server" Text="To A/C :"></asp:Label></td>
            <td>
                <asp:TextBox ID="txtToAcc" runat="server" Width="150px"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="width: 80px;">
                <asp:Label ID="Label7" runat="server" Text="Journal No :"></asp:Label></td>
            <td>
                <asp:TextBox ID="txtJournalNumber" runat="server" Width="150px"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="width: 80px;">&nbsp;&nbsp;</td>
            <td>
                <asp:Button ID="btnProcess" runat="server" Text="Process" Width="120px" OnClick="btnProcess_Click" OnClientClick="return ConfirmProcess();" />
                &nbsp;&nbsp;<asp:Label ID="lblFileProcessMsg" runat="server" Text="" ForeColor="DarkRed"></asp:Label>
                <div class="pleaseWait">Processing ... Please Wait!</div>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
    <br />
    <br />
    <%--</ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
