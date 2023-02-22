<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="UATBEFTNTableData.aspx.cs" Inherits="RemittanceOperation.UATBEFTNTableData" %>
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
            <td style="width:120px;">
                <asp:Label ID="Label2" runat="server" Text="Exchange House :"></asp:Label></td>
            <td>
                <asp:DropDownList ID="ddlExhList" runat="server"></asp:DropDownList></td>
            <td>&nbsp;</td>            
        </tr>
        <tr>
            <td style="width:120px;">
                <asp:Label ID="Label3" runat="server" Text="Date :"></asp:Label></td>
            <td>
                <asp:TextBox ID="dtpickerFrom" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
&nbsp;To
                <asp:TextBox ID="dtpickerTo" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnSearch" runat="server" Text="Search"  Width="100px" OnClick="btnSearch_Click" /></td>            
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
                    <asp:GridView ID="dataGridViewBEFTNTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
    <div style="margin-left:10px; margin-top:10px;">
        <asp:Button ID="btnUATBEFTNDataDownload" runat="server" Text="Download As Excel" Width="150" OnClick="btnUATBEFTNDataDownload_Click" />
    &nbsp;<asp:Label ID="lblDownloadMsg" runat="server" Font-Size="Small" ForeColor="#CC0000" Text=""></asp:Label>
    </div>
    <br />
</asp:Content>
