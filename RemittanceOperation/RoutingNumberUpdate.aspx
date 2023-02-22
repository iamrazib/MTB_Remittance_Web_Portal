<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="RoutingNumberUpdate.aspx.cs" Inherits="RemittanceOperation.RoutingNumberUpdate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.WaitForSearching { width: 200px; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
        .pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	</style>

    <script type="text/javascript">                
        function ConfirmSave() {
            if (confirm("Are You Sure to Save ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
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


    <table style="width: 100%; margin-top:15px; margin-left:10px;">
        <tr>
            <td colspan="1" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label1" runat="server" Text="Existing Routing Numbers"></asp:Label>
            </td>
            <td>
                <asp:LinkButton ID="LinkButtonRefreshExistingList" runat="server" OnClick="LinkButtonRefreshExistingList_Click">Refresh List</asp:LinkButton>
            </td>
            <td style="text-align:right; font-size:small; padding-right:20px;">
                <asp:Label ID="lblExistTotalRouting" runat="server" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="900px" Height="300px">
                    <asp:GridView ID="dgridViewExistRoutingInfos" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small" AllowPaging="True" OnPageIndexChanging="dgridViewExistRoutingInfos_PageIndexChanging">
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
    <table style="width: 100%; margin-left:10px; border: solid 1px">
        <tr>
            <td colspan="4" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label6" runat="server" Text="New Routing File Upload"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Label ID="Label8" runat="server" Text="Select File : "></asp:Label>
                &nbsp;&nbsp;
                <asp:FileUpload ID="FileUploadNewRoutingFile" runat="server" />
                &nbsp;&nbsp;
                 <asp:Button ID="btnUploadNewRoutingFile" runat="server" Text="Upload" Width="90px" OnClientClick="return ConfirmUpload();" OnClick="btnUploadNewRoutingFile_Click"   />
                &nbsp;&nbsp;
                <asp:Label ID="lblNewRoutingFileStats" runat="server" Text="" ForeColor="Green"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblNotExistsRoutingNumbers" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Panel ID="Panel2" runat="server" ScrollBars="Both" Width="900px" Height="200px">
                    <asp:GridView ID="dgridViewNotExistRoutingInfos" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small" >
                        <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="#FFFFCC" />                        
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
            <td style="padding-left:10px;">
                <asp:Button ID="btnInsertNewRoutingNo" runat="server" Text="Insert New Routing Number into System" OnClick="btnInsertNewRoutingNo_Click" OnClientClick="return ConfirmSave();" />&nbsp;&nbsp;
                <asp:Label ID="lblNewRoutingInsertStatus" runat="server" Text="" ForeColor="Green"></asp:Label>
                <div class="pleaseWait">Processing ... Please Wait!</div>
            </td>
        </tr>
    </table>

    <div>

    </div>
</asp:Content>
