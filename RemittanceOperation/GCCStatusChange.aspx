<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="GCCStatusChange.aspx.cs" Inherits="RemittanceOperation.GCCStatusChange" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table style="width: 100%; border: solid 1px; margin-left: 15px; margin-top: 20px; padding-left: 20px; padding-bottom: 10px;">
        <tr>
            <td colspan="3" style="font-weight: bold; text-decoration: underline; text-align: center;">
                <asp:Label ID="Label2" runat="server" Text="GCC Transaction Status Update"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <div>
                    <div style="float:left; width:130px;">PIN/Reference : </div>
                    <div style="float:left; width:200px;"><asp:TextBox ID="textBoxRefNo" runat="server" Width="180px"></asp:TextBox></div>
                    <div style="float:left;">
                        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" Width="107px" /></div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="950px" Height="170px">
                    <asp:GridView ID="dataGridViewTxnSearch" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
                <asp:Label ID="lblTransactionIssue" runat="server" Font-Size="Small" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="text-align:center">
                <asp:Button ID="btnProcessReceivedTxn" runat="server" Text="Change Status To 'Processed' For Received Txn" Width="300px" OnClick="btnProcessReceivedTxn_Click" />
                &nbsp;
                <asp:Label ID="lblStatusProcessReceivedTxn" runat="server" Font-Size="Small"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td style="text-align:center">
                <asp:Button ID="btnUpdateBEFTNStatusToGCCAlreadySent" runat="server" Text="Update BEFTN Status &quot;PAID&quot; To GCC Already Sent" Width="350px" OnClick="btnUpdateBEFTNStatusToGCCAlreadySent_Click" />
                &nbsp;
                <asp:Label ID="lblUpdateBEFTNStatusToGCCAlreadySent" runat="server" Font-Size="Small"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td style="text-align:center">
                <asp:Button ID="btnUpdateBEFTNStatusAtMtbEnd" runat="server" Text="Update BEFTN Status &quot;PAID&quot; At MTB End" Width="300px" OnClick="btnUpdateBEFTNStatusAtMtbEnd_Click"  />
                &nbsp;
                <asp:Label ID="lblUpdateBEFTNStatusAtMtbEnd" runat="server" Font-Size="Small"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
