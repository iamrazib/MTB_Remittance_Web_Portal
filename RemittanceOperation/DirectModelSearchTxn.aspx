<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DirectModelSearchTxn.aspx.cs" Inherits="RemittanceOperation.DirectModelSearchTxn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.WaitForSearching { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
        .pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	    .auto-style1 {
            height: 23px;
        }
	</style>

    <script type="text/javascript">
        function TxnSearch() {
            $('.WaitForSearching').show();
        }

        function ConfirmProcess() {
            if (confirm("Are You Sure to Change ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }
     </script>


    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%; border: solid 1px; margin-top: 20px; margin-left: 15px;">
                <tr>
                    <td colspan="2" style="font-weight: bold; text-decoration: underline; text-align: center;">
                        <asp:Label ID="Label2" runat="server" Text="Direct Model Bkash Txn"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;<asp:Label ID="Label3" runat="server" Text="Date : "></asp:Label>
                        &nbsp;<asp:TextBox ID="dateTimePickerDirect" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                        &nbsp;&nbsp;
                        <asp:Button ID="btnSearchTodayDirectTxn" runat="server" Text="Search" Width="120px" OnClick="btnSearchTodayDirectTxn_Click" OnClientClick="return TxnSearch();" />
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="MTO Name : "></asp:Label>
                        <asp:DropDownList ID="comboBoxDirectMTO" runat="server"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td><div class="WaitForSearching">Searching... Please Wait!</div></td>
                </tr>
                <tr>
                    <td style="margin:auto;">
                        <div style="margin:auto;">
                            Summary:  <br />
                            <asp:GridView ID="dataGridViewDirectSummary" runat="server" Font-Size="Small"></asp:GridView>
                        </div>                        
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>ALL Transactions: &nbsp;&nbsp;
                        <asp:Label ID="lblDirectTodayAllCount" runat="server" Text=""></asp:Label></td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="Panel3" runat="server" BorderWidth="1px" ScrollBars="Vertical" Width="900px" Height="200px">
                            <%--ScrollBars="Both"--%>
                            <asp:GridView ID="dataGridViewDirectAll" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
                    <td class="auto-style1"></td>
                </tr>
                <tr>
                    <td style="text-decoration:underline; font-weight:bold;">Pending Transactions:</td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnPendingAML" runat="server" Text="Search ALL Pending" Width="200px" OnClick="btnPendingAML_Click" OnClientClick="return TxnSearch();" />
                        &nbsp;&nbsp;<asp:Label ID="lblDirectAMLCount" runat="server" Text="" ForeColor="DarkRed"></asp:Label>
                        &nbsp;&nbsp;<asp:Label ID="lblDirectAMLAmount" runat="server" Text="" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="Panel1" runat="server" BorderWidth="1px" ScrollBars="Vertical" Width="900px" Height="200px">
                            <%--ScrollBars="Both"--%>
                            <asp:GridView ID="dataGridViewDirectAMLPending" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
                </tr>
                <tr>
                    <td>                        
                        <asp:Button ID="btnConvertPendingStatToRecvdStat" runat="server" Text="Convert Pending(3) status to Processing(1) status" OnClick="btnConvertPendingStatToRecvdStat_Click"  OnClientClick="return ConfirmProcess();" />
                        &nbsp;<asp:Label ID="lblConvertPendingStatToRecvdStatMsg" runat="server" Text=""></asp:Label>
                        <div class="pleaseWait">Processing ... Please Wait!</div>
                    </td>
                </tr>

            </table>
            <br /><br />

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
