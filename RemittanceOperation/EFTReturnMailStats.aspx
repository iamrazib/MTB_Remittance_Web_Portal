<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EFTReturnMailStats.aspx.cs" Inherits="RemittanceOperation.EFTReturnMailStats" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.WaitForSearching { width: 200px; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
        .pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	</style>

    <script type="text/javascript">
        function TxnSearch() {
            $('.WaitForSearching').show();
        }

        function ConfirmSendMail() {
            if (confirm("Are You Sure to Send Mail ?") == true) {
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

            <table style="width: 100%; border: solid 1px; margin-top:20px;margin-left:15px;">
                <tr>
                    <td colspan="2" style="font-weight: bold; text-decoration: underline; text-align: center;">
                        <asp:Label ID="Label2" runat="server" Text="RETURN Email Status"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        &nbsp;<asp:Label ID="Label3" runat="server" Text="Return Date : "></asp:Label>
                        &nbsp;<asp:TextBox ID="dtPickerReturnEmail" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                        &nbsp;&nbsp;
                        <asp:Button ID="btnReturnAutoMailSearch" runat="server" Text="Search" Width="120px" OnClick="btnReturnAutoMailSearch_Click" />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="Panel3" runat="server" ScrollBars="Vertical"  Width="900px" Height="300px">  <%--ScrollBars="Both"--%>
                            <asp:GridView ID="dataGridViewReturnMailSendInfo" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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
                        <asp:Button ID="btnSendMailToAllExch" runat="server" Text="Send Mail to All Exch"  OnClientClick="return ConfirmSendMail();" OnClick="btnSendMailToAllExch_Click" />
                    </td>
                </tr>
            </table>
            <br />
            
            <table style="width: 100%; border: solid 1px; margin-top:20px; margin-left:15px;">
                <tr>
                    <td colspan="2" style="font-weight: bold; text-decoration: underline; text-align: center;">
                        <asp:Label ID="Label1" runat="server" Text="Resend Return Mail"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        &nbsp;<asp:Label ID="Label4" runat="server" Text="Select ExH: "></asp:Label>
                        &nbsp;<asp:DropDownList ID="cbReturnEmailExchList" runat="server" OnSelectedIndexChanged="cbReturnEmailExchList_SelectedIndexChanged"></asp:DropDownList>
                        
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;<asp:Label ID="Label5" runat="server" Text="Return Date : "></asp:Label>
                        &nbsp;&nbsp;
                        <asp:TextBox ID="dtPickerReturnEmailResend" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                        &nbsp;&nbsp;
                        <asp:Button ID="btnReturnEmailCheckStat" runat="server" Text="Search" Width="120px" OnClick="btnReturnEmailCheckStat_Click" />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="Panel1" runat="server"  Width="850px" Height="100px">  
                            <asp:GridView ID="dGridReturnMailSendStatus" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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
                        <asp:Button ID="btnReturnMailResendToMTO" runat="server" Text="Resend Mail" Width="150px" OnClick="btnReturnMailResendToMTO_Click"  OnClientClick="return ConfirmSendMail();" />
                        &nbsp;&nbsp;<asp:Label ID="ReturnResendMailStats" runat="server" Text=""></asp:Label>
                        <div class="pleaseWait">Processing ... Please Wait!</div>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
            </table>
            <br /><br />

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
