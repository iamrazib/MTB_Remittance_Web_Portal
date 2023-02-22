<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="RemitCertificate.aspx.cs" Inherits="RemittanceOperation.RemitCertificate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
        .WaitForSearching {
            width: 37%;
            height: 40px;
            background-color: #cccccc;
            display: none;
            z-index: 999;
            position: absolute;
            left: 28.3%;
            top: 60%;
            font-size: large;
        }
    </style>

    <script type="text/javascript">
        function TxnSearch() {
            $('.WaitForSearching').show();
        }

        $(function () { $("#datepickerVal").datepicker({ dateFormat: "dd-mm-yyyy" }); });
    </script>

    <table style="width: 100%; border: solid 1px; margin-top: 20px; margin-left: 15px;">

        <tr>
            <td colspan="2" style="font-weight: bold; text-decoration: underline; text-align: center;">
                <asp:Label ID="Label2" runat="server" Text="Remittance Certificate Data"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>&nbsp;<asp:Label ID="Label3" runat="server" Text="Date : "></asp:Label>
                &nbsp;<asp:TextBox ID="dtPickerFromRemitCert" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
                &nbsp;&nbsp;<asp:Label ID="Label4" runat="server" Text="To : "></asp:Label>
                <asp:TextBox ID="dtPickerToRemitCert" runat="server" Width="120px" TextMode="Date"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" Text="Account No : "></asp:Label>
                &nbsp;&nbsp;
                <asp:TextBox ID="textBoxAccountNo" runat="server" Width="180px"></asp:TextBox>
            &nbsp;
                <asp:Label ID="lblAccountMissingError" runat="server" Text="" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label5" runat="server" Text="Mode : "></asp:Label>
                &nbsp;&nbsp;
                <asp:DropDownList ID="cmbRemitCertPayMode" runat="server"></asp:DropDownList>
                &nbsp;&nbsp;
                <asp:Button ID="btnSearchRemitCertificate" runat="server" Text="Search Data" Width="120px" OnClick="btnSearchRemitCertificate_Click" />
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="Panel3" runat="server" ScrollBars="Both" Width="900px" Height="300px">
                    <asp:GridView ID="dataGridViewRemitCertificate" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True">
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

    <div style="clear:both; float:left; margin:10px 0 25px 15px">
        <asp:Button ID="btnRemitCertExcelDownload" runat="server" Text="Download As Excel" OnClick="btnRemitCertExcelDownload_Click" />
    </div>
    <br />
    <br /><br /><br />

</asp:Content>
