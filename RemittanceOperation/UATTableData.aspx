<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="UATTableData.aspx.cs" Inherits="RemittanceOperation.UATTableData" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table style="width: 100%; margin-left: 10px; border: solid 1px">
        <tr>
            <td colspan="3" style="font-weight: bold; text-decoration: underline; text-align: center;">
                <asp:Label ID="Label1" runat="server" Text="UAT BEFTN Table Data"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 120px;">
                <asp:Label ID="Label2" runat="server" Text="Exchange House :"></asp:Label></td>
            <td>
                <asp:DropDownList ID="ddlExhList" runat="server"></asp:DropDownList></td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td style="width: 120px;">
                <asp:Label ID="Label4" runat="server" Text="Payment Mode :"></asp:Label></td>
            <td>
                <asp:DropDownList ID="ddlPaymentMode" runat="server">
                    <asp:ListItem Selected="True" Value="BEFTN">BEFTN</asp:ListItem>
                    <asp:ListItem Value="MTB">MTB A/C</asp:ListItem>
                    <asp:ListItem Value="CASH">CASH Txn</asp:ListItem>
                    <asp:ListItem Value="WALLET">Wallet</asp:ListItem>
                    <asp:ListItem Value="WALLBeneVald">Wallet Beneficiary Validation</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td style="width: 120px;">
                <asp:Label ID="Label3" runat="server" Text="Date :"></asp:Label></td>
            <td>
                <asp:TextBox ID="dTPickerFrom" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;To
                <asp:TextBox ID="dTPickerTo" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnSearch" runat="server" Text="Search" Width="100px" OnClick="btnSearch_Click" /></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td colspan="2">
                <asp:Label ID="lblMessage" runat="server" Font-Size="Small" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Width="900px" Height="330px">
                    <asp:GridView ID="dataGridViewUATTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
    <div style="margin-left: 10px; margin-top: 10px;">
        <asp:Button ID="btnUATTableDataDownload" runat="server" Text="Download As Excel" Width="150" OnClick="btnUATTableDataDownload_Click"  />
        &nbsp;<asp:Label ID="lblDownloadMsg" runat="server" Font-Size="Small" ForeColor="#CC0000" Text=""></asp:Label>
    </div>
    <br />

</asp:Content>
