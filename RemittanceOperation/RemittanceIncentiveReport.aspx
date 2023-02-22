<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="RemittanceIncentiveReport.aspx.cs" Inherits="RemittanceOperation.RemittanceIncentiveReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
        .WaitForSearching { width: 200px; height: 40px; background-color: #cccccc; display: none; z-index: 999;position: absolute;left: 28.3%;top: 60%;font-size: large; }                
        .pleaseWait { width: 37%;height: 40px;background-color: #cccccc;display: none;z-index: 999;position: absolute;left: 28.3%;top: 60%;font-size: large; }
    </style>

    <script type="text/javascript">
        function TxnSearch() {
            $('.WaitForSearching').show();
        }

    </script>

    <table style="width: 100%; margin-left: 30px; margin-top:10px">
        <tr>
            <td colspan="4" style="text-decoration: underline; font-weight: bold; text-align:center; font-size:large">Remittance Incentive Report</td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="Label2" runat="server" Text="From : "></asp:Label>
                <asp:TextBox ID="dtpickerFrom" runat="server" Width="120px" TextMode="Date"></asp:TextBox>

                &nbsp;&nbsp;<asp:Label ID="Label5" runat="server" Text="To : "></asp:Label>
                <asp:TextBox ID="dtpickerTo" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;&nbsp;
            </td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" Text="Exchange House: "></asp:Label><asp:DropDownList ID="ddlParty" runat="server" OnSelectedIndexChanged="ddlParty_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                <asp:DropDownList ID="ddlPartyId" runat="server" Visible="False"></asp:DropDownList>
            </td>
            <td><asp:Label ID="Label3" runat="server" Text="Status: "></asp:Label><asp:DropDownList ID="ddlStatus" runat="server"></asp:DropDownList></td>
            <td><asp:Label ID="Label4" runat="server" Text="Type: "></asp:Label><asp:DropDownList ID="ddlTrnType" runat="server"></asp:DropDownList></td>
            <td>
                <asp:Button ID="btnSearch" runat="server" Text="Search" Width="120px" OnClick="btnSearch_Click" OnClientClick="return TxnSearch();" /></td>
        </tr>
        <tr>
            <td colspan="4"><div class="WaitForSearching">Searching... Please Wait!</div></td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Panel ID="Panel2" runat="server" ScrollBars="Vertical" Width="900px" Height="300px">
                    <asp:GridView ID="dataGridViewIncentiveRptData" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" AllowPaging="True" OnPageIndexChanged="dataGridViewIncentiveRptData_PageIndexChanged" OnPageIndexChanging="dataGridViewIncentiveRptData_PageIndexChanging" Font-Size="Small" >
                        <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />
                        <PagerSettings Mode="NumericFirstLast" />
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
            <td colspan="4"></td>
        </tr>
        <tr>
            <td colspan="4"><asp:Label ID="lblTotalRecords" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td colspan="4" style="text-align:center"> <asp:Button ID="btnExportExcel" runat="server" Text="Export to Excel" Width="150px"  OnClick="btnExportExcel_Click" /></td>
        </tr>
    </table>

</asp:Content>
