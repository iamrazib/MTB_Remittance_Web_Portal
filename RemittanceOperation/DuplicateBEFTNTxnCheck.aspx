<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DuplicateBEFTNTxnCheck.aspx.cs" Inherits="RemittanceOperation.DuplicateBEFTNTxnCheck" %>

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
                <asp:Label ID="Label5" runat="server" Text="Duplicate BEFTN Transactions Check"></asp:Label>
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
                <asp:TextBox ID="dtPickerFromDt" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;&nbsp;<asp:Label ID="Label4" runat="server" Text="To : "></asp:Label>
                <asp:TextBox ID="dtPickerToDt" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;&nbsp;

               <asp:Button ID="btnBEFTNDuplicateTxnSearch" runat="server" Text="Search" Width="100px" OnClientClick="return TxnSearch();" OnClick="btnBEFTNDuplicateTxnSearch_Click" />&nbsp;&nbsp;
                        
            </td>

            <td>
                
            </td>
            <td></td>
        </tr>
        <tr>
            <td>
                <div class="WaitForSearching">Please Wait...!</div>
            </td>
            <td colspan="2">
                <%--<asp:Label ID="lblDownloadProgress" runat="server" Text=""></asp:Label>--%>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="4">

                <asp:Panel ID="Panel3" runat="server" ScrollBars="Both" Width="950px" Height="300px">
                    <asp:GridView ID="dataGridViewBeftnDuplicateTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
                <asp:Label ID="lblTotalRows" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4"></td>
        </tr>
    </table>

    <div style="clear: both; float: left; margin-top: 10px; margin-bottom: 25px;">
        <asp:Button ID="btnDownloadDuplicateTxn" runat="server" Text="Download As Excel" OnClick="btnDownloadDuplicateTxn_Click" />
        &nbsp;&nbsp;<asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
    </div>
    <br />
    <br />
    <br />
    <br />
</asp:Content>
