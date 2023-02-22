<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AlipaySftpFileUpload.aspx.cs" Inherits="RemittanceOperation.AlipaySftpFileUpload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.WaitForSearching { width: 200px; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
        .pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	</style>

    <script type="text/javascript">
        function ConfirmUpdate() {
            if (confirm("Are You Sure to Upload ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }
    </script>

    <table style="width: 100%; margin-left: 30px; border: solid 1px">
        <tr>
            <td colspan="4" style="font-weight: bold; text-decoration: underline; text-align: center;">
                <asp:Label ID="Label1" runat="server" Text="ALIPAY SFTP FILE UPLOAD"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label21" runat="server" Text="Select Statement File:"></asp:Label></td>
            <td>
                <asp:FileUpload ID="uploadFile" runat="server" /></td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:Button ID="btnUploadFile" runat="server" Text="Upload File" OnClick="btnUploadFile_Click" /></td>
            <td>
                <asp:Label ID="lblFileUploadStatus" runat="server" Text=""></asp:Label></td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>Report Date: </td>
            <td>
                <asp:TextBox ID="dateTimePickerRptDt" runat="server" Width="120px" TextMode="Date"></asp:TextBox></td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>File Type: </td>
            <td>
                <asp:DropDownList ID="ddlFileType" runat="server">
                    <asp:ListItem Selected="True" Value="BDT">BDT</asp:ListItem>
                    <asp:ListItem Value="USD">USD</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnSendFileToSpftLocation" runat="server" Text="Send File To SFTP Location" OnClick="btnSendFileToSpftLocation_Click"  OnClientClick="return ConfirmUpdate();" Width="200px" /></td>
            <td colspan="2"></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <asp:Label ID="lblSftpConnectStatus" runat="server" Text=""></asp:Label></td>
            <td colspan="2">
                <asp:Label ID="lblFileSendStatus" runat="server" Text=""></asp:Label></td>
        </tr>
    </table>
    <br />
    <br />
    <div style="margin-left: 30px;">
        <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="900px" Height="300px">
            <asp:GridView ID="dgridViewAlipayUploadedFiles" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" >
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
    </div>
</asp:Content>
