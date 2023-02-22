<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="RMSUserList.aspx.cs" Inherits="RemittanceOperation.RMSUserList" %>

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
                <asp:Label ID="Label1" runat="server" Text="User Info"></asp:Label>
            </td>
        </tr>

        <tr>
            <td colspan="3">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="950px" Height="350px">
                    <asp:GridView ID="dgridViewUserInfos" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" AutoGenerateSelectButton="True" OnSelectedIndexChanged="dgridViewUserInfos_SelectedIndexChanged">
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

    <div style="border: solid 1px; padding-left:15px;">
        <asp:Label ID="Label2" runat="server" Text="UPDATE User Information" ForeColor="DarkRed" Font-Underline="true" Height="30px"></asp:Label>
        <table style="width:100%;">
            <tr>
                <td style="width:110px;">SL No. :</td>
                <td><asp:TextBox ID="txtSlNo" runat="server" ReadOnly="true" BackColor="#ccccff"></asp:TextBox></td>   <%--style="color: grey; background-color: #F0F0F0;"--%>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>UserId :</td>
                <td><asp:TextBox ID="txtUserId" runat="server" ReadOnly="true" BackColor="#ccccff"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>User Name :</td>
                <td><asp:TextBox ID="txtUserName" runat="server" Width="250px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>User Email :</td>
                <td><asp:TextBox ID="txtUserEmail" runat="server"  Width="250px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Role Name :</td>
                <td>
                    <%--<asp:DropDownList ID="ddlUserRole" runat="server" DataTextField="RoleName" DataValueField="RoleId" ></asp:DropDownList>--%>   
                    <asp:DropDownList ID="ddlUserRole" runat="server" ></asp:DropDownList>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Is Active :</td>
                <td>
                    <asp:DropDownList ID="ddlUserActive" runat="server" >
                        <asp:ListItem Selected="True" Value="1">Y</asp:ListItem>
                        <asp:ListItem Value="0">N</asp:ListItem>
                    </asp:DropDownList>   </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>User Type :</td>
                <td>
                    <asp:DropDownList ID="ddlUserType" runat="server" >
                        <asp:ListItem Selected="True" Value="SuperAdmin">SuperAdmin</asp:ListItem>
                        <asp:ListItem Value="Authorizer">Authorizer</asp:ListItem>
                        <asp:ListItem Value="Teller">Teller</asp:ListItem>
                    </asp:DropDownList>  
                    &nbsp;&nbsp;<asp:Label ID="Label3" runat="server" Text="[ This User Type is for Manual File Upload purpose ]" Font-Bold="true" ForeColor="#cc0000"></asp:Label>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Is Mail Recv :</td>
                <td>
                    <asp:DropDownList ID="ddlIsMailRecv" runat="server" >
                        <asp:ListItem Selected="True" Value="Y">Y</asp:ListItem>
                        <asp:ListItem Value="N">N</asp:ListItem>
                    </asp:DropDownList>  </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btnUpdateUserInfo" runat="server" Text="Update" Width="120px" OnClientClick="return ConfirmUpdate();" OnClick="btnUpdateUserInfo_Click" />
                    &nbsp;<asp:Label ID="lblUpdateSuccMsg" runat="server" Text="" ForeColor="Green"></asp:Label>
                    <asp:Label ID="lblUpdateErrorMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
                <td><div class="pleaseWait">Processing ... Please Wait!</div></td>
            </tr>
        </table>

    </div>

    <br />
    <br />

    
    <div style="border: solid 1px; padding-left:15px;">
        <asp:Label ID="Label4" runat="server" Text="ADD New User Information" ForeColor="DarkRed" Font-Underline="true" Height="30px"></asp:Label>
        <table style="width:100%;">            
            <tr>
                <td>UserId :</td>
                <td><asp:TextBox ID="txtNewUserId" runat="server" ></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>UserRM Code :</td>
                <td><asp:TextBox ID="txtNewUserRM" runat="server" ></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>User Name :</td>
                <td><asp:TextBox ID="txtNewUserName" runat="server" Width="250px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>User Email :</td>
                <td><asp:TextBox ID="txtNewUserEmail" runat="server"  Width="250px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Role Name :</td>
                <td>
                    <asp:DropDownList ID="ddlUserRoleNewUser" runat="server" ></asp:DropDownList>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Is Active :</td>
                <td>
                    <asp:DropDownList ID="ddlUserActiveNewUser" runat="server" >
                        <asp:ListItem Selected="True" Value="1">Y</asp:ListItem>
                        <asp:ListItem Value="0">N</asp:ListItem>
                    </asp:DropDownList>   </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>User Type :</td>
                <td>
                    <asp:DropDownList ID="ddlUserTypeNewUser" runat="server" >
                        <asp:ListItem Selected="True" Value="SuperAdmin">SuperAdmin</asp:ListItem>
                        <asp:ListItem Value="Authorizer">Authorizer</asp:ListItem>
                        <asp:ListItem Value="Teller">Teller</asp:ListItem>
                    </asp:DropDownList>  
                    &nbsp;&nbsp;<asp:Label ID="Label5" runat="server" Text="[ This User Type is for Manual File Upload purpose ]" Font-Bold="true" ForeColor="#cc0000"></asp:Label>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Is Mail Recv :</td>
                <td>
                    <asp:DropDownList ID="ddlIsMailRecvNewUser" runat="server" >
                        <asp:ListItem Selected="True" Value="Y">Y</asp:ListItem>
                        <asp:ListItem Value="N">N</asp:ListItem>
                    </asp:DropDownList>  </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btnSaveUser" runat="server" Text="Save" Width="120px" OnClientClick="return ConfirmSave();" OnClick="btnSaveUser_Click"  />
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
