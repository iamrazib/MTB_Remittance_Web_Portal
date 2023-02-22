<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EFTReturnResendMail.aspx.cs" Inherits="RemittanceOperation.EFTReturnResendMail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%; border: solid 1px; margin-top:20px; margin-left:15px;">

                <tr>
                    <td colspan="2" style="font-weight: bold; text-decoration: underline; text-align: center;">
                        <asp:Label ID="Label2" runat="server" Text="Resend Return Mail"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        &nbsp;<asp:Label ID="Label3" runat="server" Text="Select : "></asp:Label>
                        &nbsp;<asp:DropDownList ID="cbReturnEmailExchList" runat="server"></asp:DropDownList>
                        &nbsp;&nbsp;
                        <asp:TextBox ID="dtPickerReturnEmailResend" runat="server" Width="120px" TextMode="Date"></asp:TextBox>

                        &nbsp;&nbsp;
                        <asp:Button ID="btnReturnEmailCheckStat" runat="server" Text="Search" Width="120px" />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="Panel3" runat="server" ScrollBars="Both" Width="900px" Height="400px">  
                            <asp:GridView ID="dataGridViewReturnTxnList" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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
