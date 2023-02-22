<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="InvalidRoutingList.aspx.cs" Inherits="RemittanceOperation.InvalidRoutingList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%;">
                <tr>
                    <td colspan="4" style="font-weight: bold; text-decoration: underline;">
                        <asp:Label ID="Label1" runat="server" Text="Invalid Routing List"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2" style="width: 490px;">
                        <asp:Button ID="btnInvalidRoutingSearchAgain" runat="server" Text="Search Again" Width="130px" OnClick="btnInvalidRoutingSearchAgain_Click" />&nbsp;&nbsp;
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="1000px" Height="200px">
                            <asp:GridView ID="dataGridViewBEFTNInvalidRoutingList" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:Button ID="btnInvalidRoutingDownload" runat="server" Text="Download List" OnClick="btnInvalidRoutingDownload_Click" />
    <br />
    <br />

    <%--<asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>--%>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <table style="width: 100%; border: solid 1px">
                <tr>
                    <td colspan="4" style="font-weight: bold; text-decoration: underline;">
                        <asp:Label ID="Label2" runat="server" Text="Update Record"></asp:Label>
                    </td>
                </tr>                
                <tr>
                    <td colspan="4" style="text-align: center; color: red; margin-bottom: 15px;">
                        <asp:Label ID="Label3" runat="server" Text="Before Update Make Sure Routing Number is CORRECT"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Label ID="Label4" runat="server" Text="PIN Number "></asp:Label>
                        <asp:TextBox ID="textBoxPinNumber" runat="server" Width="150px"></asp:TextBox>

                        &nbsp;&nbsp;<asp:Label ID="Label5" runat="server" Text="Routing Number "></asp:Label>
                        <asp:TextBox ID="textBoxNewRouting" runat="server" Width="100px"></asp:TextBox>
                        &nbsp;&nbsp;
                        <asp:Button ID="btnUpdateRoutingNo" runat="server" Text="Update" OnClick="btnUpdateRoutingNo_Click" />
                        &nbsp;&nbsp;
                        <asp:Label ID="lblStatus" runat="server" Text="" ForeColor="Green"></asp:Label>
                    </td>

                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
            </table>


        </ContentTemplate>
    </asp:UpdatePanel>

    <table style="width: 100%; border: solid 1px">
        <tr>
            <td colspan="4" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label6" runat="server" Text="Bulk File Update"></asp:Label>
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
                <asp:Label ID="Label8" runat="server" Text="Select File : "></asp:Label>
                &nbsp;&nbsp;
                <asp:FileUpload ID="FileUpload1" runat="server" />
                &nbsp;&nbsp;
                 <asp:Button ID="btnUploadCorrectRoutingInfo" runat="server" Text="Upload And Rectify" OnClick="btnUploadCorrectRoutingInfo_Click" Enabled="False"  />
                &nbsp;&nbsp;
                <asp:Label ID="lblBulkUpdateStats" runat="server" Text="" ForeColor="Green"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>

</asp:Content>
