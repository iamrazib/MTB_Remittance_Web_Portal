<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SearchBranchCashPassingTxn.aspx.cs" Inherits="RemittanceOperation.SearchBranchCashPassingTxn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%; border: solid 1px; margin-top: 20px; margin-left: 15px;">
                <tr>
                    <td colspan="2" style="font-weight: bold; text-decoration: underline; text-align: center;">
                        <asp:Label ID="Label2" runat="server" Text="Search Cash Txn"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="Journal No. / PIN Number :"></asp:Label>
                        &nbsp;&nbsp;<asp:TextBox ID="txtJournalOrPin" runat="server"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Button ID="btnSearchCashTxn" runat="server" Text="Search" Width="130px" OnClick="btnSearchCashTxn_Click" /></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
            </table>

            <table style="width: 100%; border: solid 1px; margin-top: 20px; margin-left: 15px;">
                <tr>
                    <td colspan="2">
                        <asp:Label ID="Label7" runat="server" Text="Search Result" Font-Underline="true" ForeColor="#CC0000"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 120px;">
                        <asp:Label ID="Label3" runat="server" Text="SL. No. :"></asp:Label>
                    </td>
                    <td style="width: 120px;">
                        <asp:Label ID="lblSlNo" runat="server" Text="" ForeColor="Black" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 120px;">
                        <asp:Label ID="Label5" runat="server" Text="Process Date :"></asp:Label>
                    </td>
                    <td style="width: 120px;">
                        <asp:Label ID="lblProcessDate" runat="server" Text="" ForeColor="OrangeRed" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 120px;">
                        <asp:Label ID="Label8" runat="server" Text="Journal No :"></asp:Label>
                    </td>
                    <td style="width: 120px;">
                        <asp:Label ID="lblJournalNo" runat="server" Text="" ForeColor="ForestGreen" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 120px;">
                        <asp:Label ID="Label10" runat="server" Text="PIN Number :"></asp:Label>
                    </td>
                    <td style="width: 120px;">
                        <asp:Label ID="lblPinNumber" runat="server" Text="" ForeColor="Fuchsia" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 120px;">
                        <asp:Label ID="Label12" runat="server" Text="Amount :"></asp:Label>
                    </td>
                    <td style="width: 120px;">
                        <asp:Label ID="lblAmount" runat="server" Text="" ForeColor="Crimson" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 120px;">
                        <asp:Label ID="Label14" runat="server" Text="Exchange House :"></asp:Label>
                    </td>
                    <td style="width: 120px;">
                        <asp:Label ID="lblExchName" runat="server" Text="" ForeColor="Teal" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 120px;">
                        <asp:Label ID="Label16" runat="server" Text="Beneficiary :"></asp:Label>
                    </td>
                    <td style="width: 120px;">
                        <asp:Label ID="lblBenfName" runat="server" Text="" ForeColor="Black" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width: 120px;">
                        <asp:Label ID="Label18" runat="server" Text="Authorized By :"></asp:Label>
                    </td>
                    <td style="width: 180px;">
                        <asp:Label ID="lblAuthBy" runat="server" Text="" ForeColor="RoyalBlue" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
            </table>


        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
