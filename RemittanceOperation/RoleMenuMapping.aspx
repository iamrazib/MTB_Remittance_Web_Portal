<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="RoleMenuMapping.aspx.cs" Inherits="RemittanceOperation.RoleMenuMapping" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.WaitForSearching { width: 200px; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
        .pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	</style>

    <script type="text/javascript">

        function ConfirmUpdate() {
            if (confirm("Are You Sure to Assign ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }

        function ConfirmDeAssign() {
            if (confirm("Are You Sure to De-Assign ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }

        function TxnSearch() {
            $('.WaitForSearching').show();
        }
     </script>


    <table style="width: 100%; margin-left: 15px; margin-top:15px;">
        <tr>
            <td colspan="3" style="font-weight: bold; text-decoration: underline; text-align:center;">
                <asp:Label ID="Label1" runat="server" Text="Role - Menu Mapping Info"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" Text="Select Role: "></asp:Label>&nbsp;&nbsp;
                <asp:DropDownList ID="ddlUserRole" runat="server"></asp:DropDownList>&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnSearchRoleMenu" runat="server" Text="Search" Width="100px" OnClick="btnSearchRoleMenu_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="950px" Height="300px">
                    <asp:GridView ID="dgridViewRoleMenus" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" AutoGenerateSelectButton="True" OnSelectedIndexChanged="dgridViewRoleMenus_SelectedIndexChanged"  >
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

    <div style="border: solid 1px; margin-left: 15px; padding-left:10px;">
        <asp:Label ID="Label3" runat="server" Text="UPDATE MENU Information" ForeColor="DarkRed" Font-Underline="true" Height="30px"></asp:Label>
        <table style="width:100%;">
            <tr>
                <td style="width:110px;">SL No. :</td>
                <td><asp:TextBox ID="txtSlNo" runat="server" ReadOnly="true" BackColor="#ccccff"></asp:TextBox></td>   
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Menu Id :</td>
                <td><asp:TextBox ID="txtMenuId" runat="server" ReadOnly="true" BackColor="#ccccff"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Menu Title :</td>
                <td><asp:TextBox ID="txtMenuTitle" runat="server" Width="250px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Menu URL :</td>
                <td><asp:TextBox ID="txtMenuUrl" runat="server"  Width="250px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>            
            <tr>
                <td>MappingStatus :</td>
                <td><asp:TextBox ID="txtMappingStatus" runat="server" ></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>

            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btnUpdateRoleMenuMapping" runat="server" Text="Assign" Width="120px" OnClientClick="return ConfirmUpdate();" OnClick="btnUpdateRoleMenuMapping_Click"   />
                    &nbsp;<asp:Label ID="lblUpdateSuccMsg" runat="server" Text="" ForeColor="Green"></asp:Label>
                    <asp:Label ID="lblUpdateErrorMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
                <td><div class="pleaseWait">Processing ... Please Wait!</div></td>
            </tr>
            <tr>
                <td></td>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btnDeAssignRoleMenuMapping" runat="server" Text="De-Assign" Width="120px" OnClientClick="return ConfirmDeAssign();" OnClick="btnDeAssignRoleMenuMapping_Click" />
                    &nbsp;<asp:Label ID="lblDeAssignUpdateSuccMsg" runat="server" Text="" ForeColor="Green"></asp:Label>
                    <asp:Label ID="lblDeAssignUpdateErrorMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
                <td></td>
            </tr>
        </table>

    </div>

    <br />
    <br />

</asp:Content>
