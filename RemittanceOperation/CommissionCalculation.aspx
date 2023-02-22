<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CommissionCalculation.aspx.cs" Inherits="RemittanceOperation.CommissionCalculation" %>
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

        function PleaseWait() {
            $('.pleaseWait').show();
        }

    </script>

    <table style="width: 100%; margin-left: 30px; margin-top:10px">
        <tr>
            <td colspan="4" style="text-decoration: underline; font-weight: bold; text-align:center; font-size:large">Calculate Commission</td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="Label2" runat="server" Text="From : "></asp:Label>
                <asp:TextBox ID="dtpickerMonthlyCommFrom" runat="server" Width="120px" TextMode="Date"></asp:TextBox>

                &nbsp;&nbsp;<asp:Label ID="Label5" runat="server" Text="To : "></asp:Label>
                <asp:TextBox ID="dtpickerMonthlyCommTo" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;&nbsp;
                <asp:Button ID="btnCalculateCommission" runat="server" Text="Calculate" Width="100px" OnClientClick="return PleaseWait();" OnClick="btnCalculateCommission_Click"  />
            </td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <div class="WaitForSearchingComm">Searching... Please Wait!</div>
                <div class="pleaseWait">Please Wait ...!</div>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Panel ID="Panel2" runat="server" ScrollBars="Vertical" Width="950px" Height="300px">
                    <asp:GridView ID="dataGridViewCalculateCommission" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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
            <td>Total BDT Commission : <asp:Label ID="lblTotalBDTCommission" runat="server" Text=""></asp:Label></td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>Total USD Commission : <asp:Label ID="lblTotalUSDCommission" runat="server" Text=""></asp:Label></td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>

    </table>
    <div style="margin-left: 30px;">        
        <asp:Button ID="btnDownloadMonthlyCommissionDataAsExcelV2" runat="server" Text="Download Commission As Excel" Width="250px" OnClick="btnDownloadMonthlyCommissionDataAsExcelV2_Click"    />
    </div>
        
    <br />
    <div style="margin-left: 30px;">
        <asp:Button ID="btnDownloadAutoDebitForEFTMTBCashAsExcel" runat="server" Text="Calculate &amp; Download AUTO DEBIT File For EFT,MTB,Cash, bKash" Width="430px" OnClick="btnDownloadAutoDebitForEFTMTBCashAsExcel_Click"  />
    </div>
    
    <%--<div style="margin-left: 30px;">
        <asp:Button ID="btnDownloadAutoDebitForBkashAsExcel" runat="server" Text="Calculate & Download AUTO DEBIT File For bKash" Width="400px" OnClick="btnDownloadAutoDebitForBkashAsExcel_Click" Visible="False" />
    </div>--%>
    
    <br />
    <table style="width: 100%; margin-left: 30px; margin-top:10px">
        <tr>
            <td colspan="4" style="text-decoration: underline; font-weight: bold; text-align:center; font-size:large">Download Exchange House Wise Commission Txn</td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="Label1" runat="server" Text="Select ExHouse :"></asp:Label>
                &nbsp;&nbsp;<asp:DropDownList ID="cbExchWiseSumr" runat="server"></asp:DropDownList>
                &nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnDownloadExhWiseCommissionDataAsExcel" runat="server" Text="Download As Excel" Width="200px" OnClick="btnDownloadExhWiseCommissionDataAsExcel_Click"    />
            </td>
            <td></td>
            <td></td>
        </tr>        
    </table>    
    <br />
    <br />
</asp:Content>
