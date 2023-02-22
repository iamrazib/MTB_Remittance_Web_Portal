<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ExhouseBalanceNew.aspx.cs" Inherits="RemittanceOperation.ExhouseBalanceNew" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    &nbsp;<asp:Button ID="btnRefreshExchBalance" runat="server" Text="Refresh" OnClick="btnRefreshExchBalance_Click" />
    <br />
    <table style="width: 100%;">
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td style="width: 250px;">Total Exchange House NRT Balance : </td>
            <td>
                <asp:Label ID="lblTotalBalance" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td style="width: 250px;">Total Wage Earners Balance : </td>
            <td>
                <asp:Label ID="lblTotalWageBalance" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td style="width: 250px;">Total Service Remittance Balance : </td>
            <td>
                <asp:Label ID="lblTotalServiceBalance" runat="server" Text=""></asp:Label></td>
        </tr>

    </table>

    <br />
    <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="980px" Height="400px">
    <asp:GridView ID="dataGridViewExchangeHouseBalance" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" OnRowDataBound="dataGridViewExchangeHouseBalance_RowDataBound" Font-Size="Small">
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
    <br />
    <br />


</asp:Content>
