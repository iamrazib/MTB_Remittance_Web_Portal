<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BEFTNAutoProcessPendingProcessed.aspx.cs" Inherits="RemittanceOperation.BEFTNAutoProcessPendingProcessed" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
        .WaitForSearching {
            width: 200px;
            height: 40px;
            background-color: #cccccc;
            display: none;
            z-index: 999;
            position: absolute;
            left: 28.3%;
            top: 60%;
            font-size: large;
        }
    </style>


    <script type="text/javascript">
        function TxnSearch() {
            $('.WaitForSearching').show();
        }
    </script>

    <%--<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>--%>

    <table style="width: 100%; border: solid 1px">

        <tr>
            <td colspan="2" style="font-weight: bold; text-decoration: underline; text-align: center;">
                <asp:Label ID="Label2" runat="server" Text="BEFTN Auto Process"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label1" runat="server" Text="Pending Txn"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <div>
                    <div style="float:left;"><asp:Button ID="btnSearchPendingBeftn" runat="server" Text="Search" Width="120px" OnClick="btnSearchPendingBeftn_Click" OnClientClick="return TxnSearch();" /></div>
                    <div style="float:left; margin-left:10px;"><asp:Label ID="lblPendingBeftnCount" runat="server" Font-Names="Cambria" Font-Size="Small"></asp:Label></div>
                </div>
                <br />
                
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="400px" Height="200px">
                    <asp:GridView ID="dataGridViewPendingBEFTNTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
            <td>
                <div>
                    <div style="float: left; font-family: Cambria;">
                        <asp:Label ID="lblAutoProcessRunning" runat="server" Font-Size="Small"></asp:Label></div>
                    <div style="float: right">
                        <asp:Button ID="btnDownloadOnProcessRecords" runat="server" Text="Download" OnClick="btnDownloadOnProcessRecords_Click" /></div>
                </div>
                <asp:Panel ID="Panel2" runat="server" ScrollBars="Both" Width="600px" Height="200px">
                    <asp:GridView ID="dataGridViewOnProcess" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
    <br />
    <table style="width: 100%; border: solid 1px">
        <tr>
            <td colspan="4" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label5" runat="server" Text="Auto Process Transactions"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2" style="width: 490px;">
                <asp:Label ID="Label3" runat="server" Text="From : "></asp:Label>
                <asp:TextBox ID="dtPickerAutoProcessFromDt" runat="server" Width="120px" TextMode="Date"></asp:TextBox>

                &nbsp;&nbsp;<asp:Label ID="Label4" runat="server" Text="To : "></asp:Label>
                <asp:TextBox ID="dtPickerAutoProcessToDt" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;&nbsp;

                        <asp:Button ID="btnBEFTNAutoProcessTxnSearch" runat="server" Text="Search" Width="100px" OnClick="btnBEFTNAutoProcessTxnSearch_Click" OnClientClick="return TxnSearch();" />

            </td>

            <td colspan="2" style="text-align: right; font-family: Cambria; color: darkgreen; padding-right: 10px;">
                <asp:Label ID="lblBEFTNAutoSuccess" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2"></td>
            <td colspan="2" style="text-align: right; font-family: Cambria; color: red; padding-right: 10px;">
                <asp:Label ID="lblBEFTNAutoFail" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td>
                <div class="WaitForSearching">Please Wait...!</div>
                <asp:Label ID="lblDownloadProgress" runat="server" Text=""></asp:Label>
            </td>
            <td></td>
            <td style="text-align: right; font-family: Cambria;">
                <asp:Label ID="lblAutoProcessDataFetchTime" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td colspan="4">

                <asp:Panel ID="Panel3" runat="server" ScrollBars="Both" Width="900px" Height="300px">
                    <asp:GridView ID="dataGridViewBeftnAutoProcessedTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
            <td>&nbsp;</td>
        </tr>
    </table>

    <%--</ContentTemplate>
    </asp:UpdatePanel>--%>

    <div style="clear: both; float: left; margin-top: 10px; margin-bottom: 25px;">
        <asp:Button ID="btnDownloadAutoProcessTxn" runat="server" Text="Download As Excel" OnClick="btnDownloadAutoProcessTxn_Click" />
    </div>
    <br />
    <br />

</asp:Content>
