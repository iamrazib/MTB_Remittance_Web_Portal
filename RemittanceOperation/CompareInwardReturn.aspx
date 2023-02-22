<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CompareInwardReturn.aspx.cs" Inherits="RemittanceOperation.CompareInwardReturn" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%; border: solid 1px; margin-top:20px;margin-left:15px;">
                <tr>
                    <td colspan="2" style="font-weight: bold; text-decoration: underline; text-align: center;">
                        <asp:Label ID="Label2" runat="server" Text="Compare Inward Return File"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        &nbsp;<asp:Label ID="Label3" runat="server" Text="Return Date : "></asp:Label>
                        &nbsp;<asp:TextBox ID="dTPickerReturnDateCompare" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                        <%--&nbsp;&nbsp;
                        <asp:Button ID="btnReturnAutoMailSearch" runat="server" Text="Search" Width="120px" />--%>
                    </td>
                </tr>
                <tr>
                    <td>
                        Select <asp:FileUpload ID="FileUpload1" runat="server" />
                        &nbsp;&nbsp;<asp:Button ID="btnUploadEFTReturnFile" runat="server" Text="Load" Width="90px" OnClick="btnUploadEFTReturnFile_Click" />
                        &nbsp;&nbsp;<asp:Label ID="lblReturnTxnUploadPrgrs" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnCompareEFTDataWithAPISystem" runat="server" Text="Compare with API System" />
                        &nbsp;&nbsp;<asp:Label ID="lblErrorMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="Panel3" runat="server" ScrollBars="Vertical"  Width="900px" Height="300px">  <%--ScrollBars="Both"--%>
                            <asp:GridView ID="dataGridCompareResultEFTWithAPI" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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
