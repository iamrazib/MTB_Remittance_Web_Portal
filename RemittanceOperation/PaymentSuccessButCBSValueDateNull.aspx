<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="PaymentSuccessButCBSValueDateNull.aspx.cs" Inherits="RemittanceOperation.PaymentSuccessButCBSValueDateNull" %>
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

        function ConfirmUpdate() {
            if (confirm("Are You Sure to Update ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }
    </script>

    <table style="width: 100%; margin-left:15px;">
        <tr>
            <td colspan="4" style="font-weight: bold; text-decoration: underline; text-align:center;">
                <asp:Label ID="Label1" runat="server" Text="BEFTN Success Transactions But CBS ValueDate is EMPTY"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2" style="width:490px;">
                <asp:Label ID="Label3" runat="server" Text="From : "></asp:Label>
                <asp:TextBox ID="dtpickerFrom" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                        
                &nbsp;&nbsp;<asp:Label ID="Label4" runat="server" Text="To : "></asp:Label>
                <asp:TextBox ID="dtpickerTo" runat="server" Width="120px" TextMode="Date"></asp:TextBox>                       
                &nbsp;&nbsp;
                <asp:Button ID="btnSearchTxn" runat="server" Text="Search" Width="100px"  OnClientClick="return TxnSearch();" OnClick="btnSearchTxn_Click"  />&nbsp;&nbsp;
                        
            </td>                    
            <td>  </td>
            <td>  </td>
        </tr>
        <tr>
            <td colspan="2" style="width:490px;">
                <asp:Label ID="Label5" runat="server" Text="Exchange House :"></asp:Label>
                &nbsp;&nbsp;<asp:DropDownList ID="cbExchWise" runat="server"></asp:DropDownList>
            </td>                    
            <td>  </td>
            <td>  </td>
        </tr>
        <tr>
            <td><div class="WaitForSearching">Searching... Please Wait!</div></td>
            <td style="text-align: right;"><asp:Label ID="lblTotalRec" runat="server" Font-Size="Small" ></asp:Label></td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Width="920px" Height="250px">
                    <asp:GridView ID="dataGridViewTxn" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
    <div style="margin-left:15px;">
        <asp:Button ID="btnDownloadTxnAsExcel" runat="server" Text="Download As Excel" Width="150px" OnClick="btnDownloadTxnAsExcel_Click"   />
        &nbsp;&nbsp;<asp:Label ID="lblDownloadMsg" runat="server" Font-Size="Small" ForeColor="Red" Text=""></asp:Label>
    </div>        
    <br />
    <table style="width: 100%;  margin-left:15px; border: solid 1px">
        <tr>
            <td colspan="4" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label2" runat="server" Text="Update Value Date"></asp:Label>
            </td>
        </tr>      
        <tr>
            <td colspan="4">
                <asp:Label ID="Label6" runat="server" Text="PIN Number "></asp:Label>
                <asp:TextBox ID="textBoxPinNumber" runat="server" Width="200px" AutoCompleteType="Disabled"></asp:TextBox>

                &nbsp;&nbsp;<asp:Label ID="Label7" runat="server" Text="CBS Date: "></asp:Label>
                <asp:TextBox ID="dtpickerValueDate" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;&nbsp;
                <asp:Button ID="btnUpdateValueDate" runat="server" Text="Update" Width="100" OnClientClick="return ConfirmUpdate();"  OnClick="btnUpdateValueDate_Click" />
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
    <br />
    <table style="width: 100%;  margin-left:15px; border: solid 1px; visibility: hidden;" >
        <tr>
            <td colspan="4" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label8" runat="server" Text="Fetch Transaction Value Date"></asp:Label>
            </td>
        </tr>  
        <tr>
            <td><asp:Label ID="Label9" runat="server" Text="Select ExHouse: "></asp:Label>
                <asp:DropDownList ID="ddlParty" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlParty_SelectedIndexChanged"></asp:DropDownList>
                <asp:DropDownList ID="ddlPartyNRTA" runat="server" Visible="False"></asp:DropDownList>
                &nbsp;&nbsp;
                <asp:Label ID="Label10" runat="server" Text="PIN: "></asp:Label><asp:TextBox ID="txtPinNumber" runat="server" Width="170px"></asp:TextBox>
                &nbsp;&nbsp;
                 <asp:Button ID="btnRemitTranCheck" runat="server" Width="100px" Text="Check" OnClick="btnRemitTranCheck_Click" />
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label11" runat="server" Text="ValueDate:" ForeColor="#990000"></asp:Label>&nbsp;
                <asp:Label ID="lblValueDate" runat="server" Text="" ForeColor="#990000"></asp:Label>&nbsp;&nbsp;
                <asp:Label ID="Label13" runat="server" Text="Journal:" ForeColor="#000099"></asp:Label>&nbsp;
                <asp:Label ID="lblJournal" runat="server" Text="" ForeColor="#000099"></asp:Label>&nbsp;&nbsp;
                <asp:Label ID="Label15" runat="server" Text="Amount:" ForeColor="#ff0000"></asp:Label>&nbsp;
                <asp:Label ID="lblAmount" runat="server" Text="" ForeColor="#ff0000"></asp:Label>
            </td>
        </tr>
    </table>
    <br />
</asp:Content>
