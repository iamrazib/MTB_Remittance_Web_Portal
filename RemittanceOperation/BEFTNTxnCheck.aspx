<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BEFTNTxnCheck.aspx.cs" Inherits="RemittanceOperation.BEFTNTxnCheck" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%;">
                <tr>
                    <td colspan="4" style="font-weight:bold; text-decoration:underline;">BEFTN Transaction Check</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td style="width:130px;">PIN/Ref. No.</td>
                    <td style="width:150px;">
                        <asp:TextBox ID="txtBeftnRefNo" runat="server" Width="200px"></asp:TextBox></td>
                    <td>
                        <asp:Button ID="btnSearchBeftnTxn" runat="server" Text="Search" Width="100px" OnClick="btnSearchBeftnTxn_Click" /></td>
                    <td>
                        <asp:Label ID="lblBeftnSearchMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>                
                <tr>
                    <td colspan="4">
                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="900px" Height="170px">
                            <asp:GridView ID="dataGridViewBeftnTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
