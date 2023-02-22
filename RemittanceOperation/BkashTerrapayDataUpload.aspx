<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BkashTerrapayDataUpload.aspx.cs" Inherits="RemittanceOperation.BkashTerrapayDataUpload" %>
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

        function ConfirmUpload() {
            if (confirm("Are You Sure to Upload ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }

    </script>

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <table style="width: 100%; margin-left: 30px; margin-top:10px">
        <tr>
            <td colspan="4" style="text-decoration: underline; font-weight: bold; text-align:center; font-size:large">Upload Terrapay Bkash Data</td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="Label2" runat="server" Text="From : "></asp:Label>
                <asp:TextBox ID="dtpickerFrom" runat="server" Width="120px" TextMode="Date"></asp:TextBox>

                &nbsp;&nbsp;<asp:Label ID="Label5" runat="server" Text="To : "></asp:Label>
                <asp:TextBox ID="dtpickerTo" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;&nbsp;
            </td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td><div class="pleaseWait">Saving... Please Wait!</div>
                <asp:Label ID="Label6" runat="server" Text="Select Party : "></asp:Label>
                    &nbsp;<asp:DropDownList ID="ddlBkashParty" runat="server" OnSelectedIndexChanged="ddlBkashParty_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td colspan="2">
                <div class="WaitForSearching">Searching... Please Wait!</div>
                <asp:FileUpload ID="FileUploadBkashFile" runat="server" /><asp:Button ID="btnUploadFile" runat="server" Text="Fetch Data"  OnClientClick="return TxnSearch();" OnClick="btnUploadFile_Click" />
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:Label ID="lblFileUploadMsg" runat="server" Text="" ForeColor="#cc0000"></asp:Label>
            </td>
            <td>&nbsp;</td>
            <td></td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Panel ID="Panel2" runat="server" ScrollBars="Vertical" Width="900px" Height="300px">
                    <asp:GridView ID="dataGridViewBkashData" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" AllowPaging="True" OnPageIndexChanged="dataGridViewBkashData_PageIndexChanged" OnPageIndexChanging="dataGridViewBkashData_PageIndexChanging" >
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
            <%--<td colspan="4" style="text-align:center"><asp:Button ID="btnDataUploadSystem" runat="server" Text="Data Upload Into System" OnClientClick="return ConfirmUpload();" OnClick="btnDataUploadSystem_Click" />
                <br />
                <asp:Label ID="lblDataCountSaveIntoDB" runat="server" Text=""></asp:Label>
            </td>--%>
        </tr>
        
    </table>


    <asp:UpdatePanel ID="UpdatePanel1" runat="server" >
        <ContentTemplate>
            <table style="width: 100%; margin-left:30px; margin-top:10px">
                <tr>
                    <td style="text-align:center">
                        <asp:Button ID="btnDataUploadSystem" runat="server" Text="Data Upload Into System" OnClick="btnDataUploadSystem_Click" />
                        <br />
                        <asp:Label ID="lblDataCountSaveIntoDB" runat="server" Text=""></asp:Label>
                        <br />
                        <asp:Label ID="lblUploadStatus" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
