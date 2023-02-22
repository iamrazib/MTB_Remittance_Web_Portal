<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BEFTNAutoProcessFailedTxn.aspx.cs" Inherits="RemittanceOperation.BEFTNAutoProcessFailedTxn" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%; margin-left: 15px;">
                <tr>
                    <td colspan="4" style="font-weight: bold; text-decoration: underline;">
                        <asp:Label ID="Label1" runat="server" Text="BEFTN Auto Process Failed Transactions"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2" style="width:490px;">
                        <asp:Label ID="Label3" runat="server" Text="From : "></asp:Label>
                        <asp:TextBox ID="dTPickerBEFTNAutoFailedTxnFrom" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                        
                        &nbsp;&nbsp;<asp:Label ID="Label4" runat="server" Text="To : "></asp:Label>
                        <asp:TextBox ID="dTPickerBEFTNAutoFailedTxnTo" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                       
                        &nbsp;&nbsp;

                        <asp:Button ID="btnBEFTNAutoFailedTxnSearch" runat="server" Text="Search" Width="100px" OnClick="btnBEFTNAutoFailedTxnSearch_Click"  />&nbsp;&nbsp;
                        <asp:Label ID="lblBEFTNAutoFailedCnt" runat="server" Font-Size="Small" ForeColor="Red"></asp:Label>
                    </td>
                    
                    <td>
                        <%--<asp:Label ID="lblAutoProcessDataFetchTime" runat="server" Text=""></asp:Label>--%>
                    </td>
                    <td>                        
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

                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="900px" Height="350px">
                            <asp:GridView ID="dataGridViewBEFTNAutoFailedTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
                    <td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
                </tr>
                               
            </table>

        </ContentTemplate>
    </asp:UpdatePanel>

    <div style="clear:both; float:left; margin-top:10px; margin-bottom:25px; margin-left:15px;">
        <asp:Button ID="btnDownloadAutoProcessFailedTxn" runat="server" Text="Download As Excel" OnClick="btnDownloadAutoProcessFailedTxn_Click" />
    </div>
    <br />
    <asp:Label ID="lblDownloadMsg" runat="server" Text=""></asp:Label>
    <br />

</asp:Content>
