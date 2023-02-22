<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BEFTNEligibleForIncentive.aspx.cs" Inherits="RemittanceOperation.BEFTNEligibleForIncentive" %>
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

    </script>

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%; margin-left:30px;">
                <tr>
                    <td colspan="4" style="font-weight: bold; text-decoration: underline; text-align:center;">
                        <asp:Label ID="Label1" runat="server" Text="BEFTN Eligible For Incentive"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2" style="width:490px;">
                        <asp:Label ID="Label3" runat="server" Text="From : "></asp:Label>
                        <asp:TextBox ID="dTPickerFrom" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                        
                        &nbsp;&nbsp;<asp:Label ID="Label4" runat="server" Text="To : "></asp:Label>
                        <asp:TextBox ID="dTPickerTo" runat="server" Width="120px" TextMode="Date"></asp:TextBox>                       
                        &nbsp;&nbsp;
                        
                    </td>                    
                    <td>                        
                    </td>
                    <td>                        
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="Incentive Type: "></asp:Label>
                        &nbsp;&nbsp;
                        <asp:DropDownList ID="ddlIncentiveType" runat="server">
                            <asp:ListItem Selected="True" Value="10">Eligible</asp:ListItem>
                            <asp:ListItem Value="11">Required due diligence for further processing</asp:ListItem>
                            <asp:ListItem Value="12">Non-Individual customer</asp:ListItem>
                            <asp:ListItem Value="13">Principal Amount Above 5 Lacs</asp:ListItem>
                            <asp:ListItem Value="14">Not Eligible For Incentive</asp:ListItem>
                        </asp:DropDownList>
                        &nbsp;&nbsp;
                        <asp:Button ID="btnSearch" runat="server" Text="Search" Width="100px" OnClientClick="return TxnSearch();" OnClick="btnSearch_Click"  />
                    </td>
                    <td></td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td><div class="WaitForSearching">Searching... Please Wait!</div></td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="4">

                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Width="900px" Height="300px">
                            <asp:GridView ID="dataGridViewEFTEligibleInct" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
                        <asp:Label ID="lblRowCount" runat="server" Font-Bold="True"></asp:Label>
                    </td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
                </tr>
                               
            </table>

        </ContentTemplate>
    </asp:UpdatePanel>
    <div style="clear:both; float:left; margin:10px 0 25px 15px">
        <asp:Button ID="btnDownloadIncentiveList" runat="server" Text="Download As Excel" OnClick="btnDownloadIncentiveList_Click" />
        
    </div>
    <br /><asp:Label ID="lblDownloadMsg" runat="server" Text=""></asp:Label><br />
</asp:Content>
