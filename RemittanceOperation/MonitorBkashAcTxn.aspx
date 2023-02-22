<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="MonitorBkashAcTxn.aspx.cs" Inherits="RemittanceOperation.MonitorBkashAcTxn" %>
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

    <table style="width: 100%; border: solid 1px; padding-top: 20px;">
        <tr>
            <td colspan="4" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label5" runat="server" Text="Failed bKash Transactions"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="3" style="width: 490px;">
                <asp:Label ID="Label3" runat="server" Text="From : "></asp:Label>
                <asp:TextBox ID="dtPickerFromDt" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;&nbsp;<asp:Label ID="Label4" runat="server" Text="To : "></asp:Label>
                <asp:TextBox ID="dtPickerToDt" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;&nbsp;

               <asp:Button ID="btnBkashFailedTxnSearch" runat="server" Text="Search" Width="100px" OnClientClick="return TxnSearch();" OnClick="btnBkashFailedTxnSearch_Click" />&nbsp;&nbsp;                        
            </td>
            <td style="font-weight:bold"><asp:Label ID="lblTotalRows" runat="server" Text=""></asp:Label></td>            
        </tr>
        <tr>
            <td>
                <div class="WaitForSearching">Please Wait...!</div>
            </td>
            <td colspan="2">                
                
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="4">

                <asp:Panel ID="Panel3" runat="server" ScrollBars="Both" Width="900px" Height="250px">
                    <asp:GridView ID="dataGridViewBkashFailedTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
            <td colspan="4" style="text-align:center; font-weight:bold">
                &nbsp;</td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Button ID="btnDownloadFailedTxn" runat="server" Text="Download As Excel" OnClick="btnDownloadFailedTxn_Click"  />
                &nbsp;&nbsp;<asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>


    <br />
    <br />
    <table style="width: 100%; border: solid 1px; padding-top: 20px;">
        <tr>
            <td colspan="4" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label1" runat="server" Text="Successful bKash Transactions"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="3" style="width: 490px;">
                <asp:Label ID="Label2" runat="server" Text="From : "></asp:Label>
                <asp:TextBox ID="dtPickerFromDtSuccs" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;&nbsp;<asp:Label ID="Label6" runat="server" Text="To : "></asp:Label>
                <asp:TextBox ID="dtPickerToDtSuccs" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;&nbsp;

               <asp:Button ID="btnBkashSuccessTxnSearch" runat="server" Text="Search" Width="100px" OnClientClick="return TxnSearch();" OnClick="btnBkashSuccessTxnSearch_Click" />&nbsp;&nbsp;
                        
            </td>
            <td style="font-weight:bold"><asp:Label ID="lblTotalRowsSuccs" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td>
                <div class="WaitForSearching">Please Wait...!</div>
            </td>
            <td colspan="2">                
                
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="4">

                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="900px" Height="250px">
                    <asp:GridView ID="dataGridViewBkashSuccessfulTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
            <td colspan="4" style="text-align:center; font-weight:bold">
                &nbsp;</td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Button ID="btnDownloadSuccessfulTxn" runat="server" Text="Download As Excel" OnClick="btnDownloadSuccessfulTxn_Click"  />
                &nbsp;&nbsp;<asp:Label ID="lblMsg2" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <br />

</asp:Content>
