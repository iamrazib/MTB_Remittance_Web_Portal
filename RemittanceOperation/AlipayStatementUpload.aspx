<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AlipayStatementUpload.aspx.cs" Inherits="RemittanceOperation.AlipayStatementUpload" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">

     .pleaseWait {
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
        function Validate() {
            $('.pleaseWait').show();
        }

    </script>

    <table style="width: 100%; margin-left:15px;">
                <tr>
                    <td colspan="4" style="font-weight: bold; text-decoration: underline; text-align:center;">
                        <asp:Label ID="Label1" runat="server" Text="Alipay Statement Upload" Font-Size="Large"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label4" runat="server" Text="Select File : " Font-Size="Medium"></asp:Label>
                        <asp:FileUpload ID="FileUpload1" runat="server" Width="350px" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="width:490px;">
                        <asp:Label ID="Label3" runat="server" Text="Report Date : " Font-Size="Medium"></asp:Label>
                        <asp:TextBox ID="dTPickerFrom" runat="server" Width="120px" TextMode="Date"></asp:TextBox>                                                
                    </td>  
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="File Type: "></asp:Label>
                        &nbsp;&nbsp;
                        <asp:DropDownList ID="ddlFileType" runat="server">
                            <asp:ListItem Selected="True" Value="BDT">BDT</asp:ListItem>
                            <asp:ListItem Value="USD">USD</asp:ListItem>
                        </asp:DropDownList>
                        &nbsp;&nbsp;
                        <asp:Button ID="btnUpload" runat="server" Text="Upload" Width="100px" OnClientClick="return Validate();" OnClick="btnUpload_Click"   />
                        <asp:Label ID="lblSftpConnectStatus" runat="server" Text=""></asp:Label>&nbsp;&nbsp;
                        <asp:Label ID="lblFileSendStatus" runat="server" Text=""></asp:Label>
                    </td>
                    <td></td>
                </tr>
                <tr style="text-align:right">
                    <td><div class="pleaseWait">Please Wait !!!</div></td>
                    <td></td>
                    <%--<td></td>  --%>                 
                </tr>

            <tr>
                    <td>
                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Width="900px" Height="250px">
                            <asp:GridView ID="dataGridViewAlipayUploadedFiles" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
    <br /><br />
</asp:Content>
