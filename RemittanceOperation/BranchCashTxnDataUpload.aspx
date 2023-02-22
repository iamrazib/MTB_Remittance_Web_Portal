<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BranchCashTxnDataUpload.aspx.cs" Inherits="RemittanceOperation.BranchCashTxnDataUpload" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
        .WaitForSearching { width: 200px; height: 40px; background-color: #cccccc; display: none; z-index: 999;position: absolute;left: 28.3%;top: 60%;font-size: large; }                
        .pleaseWait { width: 37%;height: 40px;background-color: #cccccc;display: none;z-index: 999;position: absolute;left: 28.3%;top: 60%;font-size: large; }
    </style>

    <script type="text/javascript">
        function TxnSearch() {
            $('.WaitForSearching').show();
        }

        function TxnSave() {
            $('.pleaseWait').show();
        }

    </script>

    <table style="width: 100%; margin-left: 30px; margin-top:10px">
        <tr>
            <td colspan="4" style="text-decoration: underline; font-weight: bold; text-align:center; font-size:large">Upload Branch Cash Payment Excel Data</td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="Label2" runat="server" Text="From : "></asp:Label>
                <asp:TextBox ID="dtpickerFrom" runat="server" Width="120px" TextMode="Date"></asp:TextBox>

                &nbsp;&nbsp;<asp:Label ID="Label5" runat="server" Text="To : "></asp:Label>
                <asp:TextBox ID="dtpickerTo" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;&nbsp;
                <%--<asp:Button ID="btnCalculateCommission" runat="server" Text="Calculate" Width="100px" OnClientClick="return TxnSearch();"   />--%>
            </td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td><div class="pleaseWait">Saving... Please Wait!</div></td>
            <td>
                <div class="WaitForSearching">Searching... Please Wait!</div>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td><asp:FileUpload ID="FileUploadBranchCashFile" runat="server" />&nbsp;<asp:Button ID="btnUploadFile" runat="server" Text="Fetch Data" OnClick="btnUploadFile_Click" OnClientClick="return TxnSearch();" /></td>
            <td>
                <asp:Label ID="lblFileUploadMsg" runat="server" Text="" ForeColor="#cc0000"></asp:Label>
            </td>
            <td>&nbsp;</td>
            <td></td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Panel ID="Panel2" runat="server" ScrollBars="Vertical" Width="900px" Height="300px">
                    <asp:GridView ID="dataGridViewBranchCashData" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" AllowPaging="True" OnPageIndexChanged="dataGridViewBranchCashData_PageIndexChanged" OnPageIndexChanging="dataGridViewBranchCashData_PageIndexChanging">
                        <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />
                        <PagerSettings Mode="NumericFirstLast" />
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
            <td>Total Records : <asp:Label ID="lblTotalRecords" runat="server" Text=""></asp:Label></td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="4" style="text-align:center"><asp:Button ID="btnDataUploadSystem" runat="server" Text="Data Upload Into System" OnClick="btnDataUploadSystem_Click" OnClientClick="return TxnSave();" />
                <br />
                <asp:Label ID="lblDataCountSaveIntoDB" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
