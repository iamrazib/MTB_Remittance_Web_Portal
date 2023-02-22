<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BkashReversalFailed.aspx.cs" Inherits="RemittanceOperation.BkashReversalFailed" %>
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

        .WaitForSearchingComm {
            width: 200px;
            height: 40px;
            background-color: #cccccc;
            display: none;
            z-index: 999;
            position: absolute;
            left: 28.3%;
            top: 70%;
            font-size: large;
        }

        .pleaseWait {
            width: 37%;
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

    <table style="width: 100%; margin-left: 30px;">
        <tr>
            <td colspan="4" style="font-weight: bold; text-decoration: underline; text-align: center;">
                <asp:Label ID="Label1" runat="server" Text="BKash Reversal Fail List"></asp:Label>
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
                <asp:TextBox ID="dtpickerFrom" runat="server" Width="120px" TextMode="Date"></asp:TextBox>

                &nbsp;&nbsp;<asp:Label ID="Label4" runat="server" Text="To : "></asp:Label>
                <asp:TextBox ID="dtpickerTo" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;&nbsp;
                        <asp:Button ID="btnSearchReversalFailTxn" runat="server" Text="Search" Width="100px" OnClientClick="return TxnSearch();" OnClick="btnSearchReversalFailTxn_Click"  />&nbsp;&nbsp;
                        
            </td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <div class="WaitForSearching">Searching... Please Wait!</div>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="4">

                <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Width="950px" Height="260px">
                    <asp:GridView ID="dataGridViewReversalFailTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
                <asp:Label ID="lblRecordCount" runat="server" ForeColor="Blue" Text=""></asp:Label>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <%--<tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>--%>

    </table>
    <div style="margin-left: 30px; text-align:center">
        <asp:Button ID="btnDownloadReversalFailDataAsExcel" runat="server" Text="Download As Excel" Width="150px" OnClick="btnDownloadReversalFailDataAsExcel_Click"  />
    </div>
    <div style="margin-left: 30px; margin-top:20px;">
        <asp:Label ID="Label2" runat="server" Text="Initiate Reversal :: " Font-Bold="True" Font-Underline="True"></asp:Label>
    </div>
    <div style="margin-left: 30px; text-align:center">
        <asp:Label ID="Label5" runat="server" Text="Before Initiate Reversal Make SURE Respective WALLET Account has sufficient Balance" ForeColor="Red"></asp:Label>
    </div>
    <div style="margin-left: 30px;">
        <asp:Label ID="Label6" runat="server" Text="PIN No: " />&nbsp;&nbsp;
        <asp:TextBox ID="txtPinNoForReversal" runat="server" Width="160px"></asp:TextBox>&nbsp;&nbsp;
        <asp:Button ID="btnChangeStatusForReversal" runat="server" Text="Change Status For Reversal" Width="200px" OnClick="btnChangeStatusForReversal_Click" />&nbsp;
        <asp:Label ID="lblReversalChangeStatus" runat="server" Font-Size="Small" Text=""></asp:Label>
    </div>
    <br />
    <br />

</asp:Content>
