<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DuplicateCashTxnCheck.aspx.cs" Inherits="RemittanceOperation.DuplicateCashTxnCheck" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">
        function CheckValue() {
            //var pinnum = document.getElementById('txtCashCheckPinNumber');
            if (document.getElementById('txtCashCheckPinNumber').value.trim() == "") {
                alert('PIN Number Cannot Empty !!!');
                document.getElementById('txtCashCheckPinNumber').focus();
                return false;
            } else {
                return true;
            }
        }

    </script>


    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>


            <table style="width: 100%; border: solid 1px; margin-top: 20px; margin-left: 15px; padding-left: 10px;">
                <tr>
                    <td colspan="5" style="font-weight: bold; text-decoration: underline; text-align: center;">
                        <asp:Label ID="Label2" runat="server" Text="Cash Transaction Check & Save" ForeColor="#CC0000"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td style="width: 120px">
                        <asp:Label ID="Label1" runat="server" Text="PIN Number" Font-Bold="True" Font-Underline="True"></asp:Label></td>
                    <td style="width: 120px">
                        <asp:Label ID="Label3" runat="server" Text="Journal Number" Font-Bold="True" Font-Underline="True"></asp:Label></td>
                    <td style="width: 120px">
                        <asp:Label ID="Label4" runat="server" Text="Amount" Font-Bold="True" Font-Underline="True"></asp:Label></td>
                    <td style="width: 200px">
                        <asp:Label ID="Label5" runat="server" Text="Exchange House" Font-Bold="True" Font-Underline="True"></asp:Label></td>
                    <td style="width: 120px">
                        <asp:Label ID="Label6" runat="server" Text="Beneficiary Name" Font-Bold="True" Font-Underline="True"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 120px">
                        <asp:TextBox ID="txtCashCheckPinNumber" runat="server"></asp:TextBox></td>
                    <td style="width: 120px">
                        <asp:TextBox ID="txtCashCheckJournal" runat="server"></asp:TextBox></td>
                    <td style="width: 120px">
                        <asp:TextBox ID="txtCashCheckAmount" runat="server"></asp:TextBox></td>
                    <td style="width: 200px">
                        <asp:DropDownList ID="cbExchDupChk" runat="server"></asp:DropDownList></td>
                    <td style="width: 120px">
                        <asp:TextBox ID="txtCashCheckBeneficiary" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnCheckAndSave" runat="server" Text="Check & Save" Width="130px" OnClick="btnCheckAndSave_Click" OnClientClick="CheckValue();" /></td>
                    <td colspan="3">
                        <asp:Label ID="lblPINStatus" runat="server" ForeColor="#FF3300"></asp:Label></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="5">
                        <asp:Label ID="lblDuplicatePinInfo" runat="server" ForeColor="#FF3300"></asp:Label></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
            </table>
            <%--<br />--%>
            <table style="width: 100%; border: solid 1px; margin-top: 20px; margin-left: 15px; padding-left: 10px;">
                <tr>
                    <td colspan="5" style="font-weight: bold; text-decoration: underline; text-align: left;">
                        <asp:Label ID="Label7" runat="server" Text="Saved Transaction" ForeColor="#CC0000"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label8" runat="server" Text="From :"></asp:Label>
                        &nbsp;<asp:TextBox ID="dtPickerCashCheckFrom" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="Label9" runat="server" Text="To :"></asp:Label>
                        &nbsp;<asp:TextBox ID="dtPickerCashCheckTo" runat="server" Width="120px" TextMode="Date"></asp:TextBox></td>
                    <td>
                        <asp:Button ID="btnSearchSavedCash" runat="server" Text="View" Width="120px" OnClick="btnSearchSavedCash_Click" /></td>
                    <td colspan="2">
                        <asp:Label ID="lblRowCount" runat="server" Text="" ForeColor="DarkBlue"></asp:Label></td>

                </tr>
                <tr>
                    <td colspan="5">
                        <asp:Panel ID="Panel3" runat="server" ScrollBars="Both" Height="200px">
                            <asp:GridView ID="dataGridViewCashSavedTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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

            <table style="width: 100%; border: solid 1px; margin-top: 10px; margin-left: 15px; padding-left: 10px;">
                <tr>
                    <td colspan="5" style="font-weight: bold; text-decoration: underline; text-align: left;">
                        <asp:Label ID="Label10" runat="server" Text="Summary" ForeColor="#CC0000"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="5">
                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical">
                            <asp:GridView ID="GridViewCashPassingSummary" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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
            <br />
            <br />

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
