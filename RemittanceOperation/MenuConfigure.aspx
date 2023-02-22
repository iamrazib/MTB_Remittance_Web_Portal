<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="MenuConfigure.aspx.cs" Inherits="RemittanceOperation.MenuConfigure" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.WaitForSearching { width: 200px; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
        .pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	</style>

    <script type="text/javascript">

        function ConfirmUpdate() {
            if (confirm("Are You Sure to Update ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }

        function ConfirmSave() {
            if (confirm("Are You Sure to Save ?") == true) {
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

    <div style="text-align: center; margin-top: 15px;">
        <asp:Label ID="lblUserAuthorizationMsg" runat="server" Text=""></asp:Label>
    </div>
    <table style="width: 100%; margin-left: 15px;">
        <tr>
            <td colspan="3" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label1" runat="server" Text="Menu Info"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="950px" Height="350px">
                    <asp:GridView ID="dgridViewUserMenus" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" AutoGenerateSelectButton="True" OnSelectedIndexChanged="dgridViewUserMenus_SelectedIndexChanged" >
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
        <asp:Label ID="Label2" runat="server" Text="UPDATE MENU Information" ForeColor="DarkRed" Font-Underline="true" Height="30px"></asp:Label>
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
                <td>Menu Parent :</td>
                <td> 
                    <asp:DropDownList ID="ddlMenuParent" runat="server" ></asp:DropDownList>
                </td>
                <td>&nbsp;</td>
            </tr>            
            <tr>
                <td>Is Active :</td>
                <td>
                    <asp:DropDownList ID="ddlMenuActive" runat="server" >
                        <asp:ListItem Selected="True" Value="1">Y</asp:ListItem>
                        <asp:ListItem Value="0">N</asp:ListItem>
                    </asp:DropDownList>   </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Internal SL. :</td>
                <td><asp:TextBox ID="txtInternalSlNo" runat="server" ></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>

            <%--<tr>
                <td>Menu Priority SL. :</td>
                <td><asp:TextBox ID="txtMenuPrioritySlNo" runat="server" ></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>--%>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btnUpdateMenuInfo" runat="server" Text="Update" Width="120px" OnClientClick="return ConfirmUpdate();" OnClick="btnUpdateMenuInfo_Click"  />
                    &nbsp;<asp:Label ID="lblUpdateSuccMsg" runat="server" Text="" ForeColor="Green"></asp:Label>
                    <asp:Label ID="lblUpdateErrorMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
                <td><div class="pleaseWait">Processing ... Please Wait!</div></td>
            </tr>
        </table>

    </div>

    <br />
    <br />

    <div style="border: solid 1px; margin-left: 15px; padding-left:10px;">
        <asp:Label ID="Label4" runat="server" Text="ADD New MENU Information" ForeColor="DarkRed" Font-Underline="true" Height="30px"></asp:Label>
        <table style="width:100%;">            
            <tr>
                <td style="width:110px;">Menu Title :</td>
                <td><asp:TextBox ID="txtNewMenuTitle" runat="server" Width="250px" ></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Menu URL :</td>
                <td><asp:TextBox ID="txtNewMenuUrl" runat="server" Width="250px" ></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Menu Parent :</td>
                <td> 
                    <asp:DropDownList ID="ddlNewMenuParent" runat="server" ></asp:DropDownList>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Is Active :</td>
                <td>
                    <asp:DropDownList ID="ddlNewMenuActive" runat="server" >
                        <asp:ListItem Selected="True" Value="1">Y</asp:ListItem>
                        <asp:ListItem Value="0">N</asp:ListItem>
                    </asp:DropDownList>   </td>
                <td>&nbsp;</td>
            </tr>
            <%--<tr>
                <td>Internal SL. :</td>
                <td><asp:TextBox ID="txtNewInternalSlNo" runat="server" ></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>--%>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btnSaveNewMenu" runat="server" Text="Save" Width="120px" OnClientClick="return ConfirmSave();" OnClick="btnSaveNewMenu_Click"   />
                    &nbsp;<asp:Label ID="lblSaveSuccMsg" runat="server" ForeColor="Green"></asp:Label>
                    <asp:Label ID="lblSaveErrorMsg" runat="server" ForeColor="Red"></asp:Label>
                </td>
                <td><div class="pleaseWait">Processing ... Please Wait!</div></td>
            </tr>
        </table>

    </div>

    <br />
    <br />

</asp:Content>
