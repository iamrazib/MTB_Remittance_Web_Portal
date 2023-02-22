<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="UATExchUserList.aspx.cs" Inherits="RemittanceOperation.UATExchUserList" %>
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

    <table style="width: 100%; margin-left: 15px; margin-top:10px;">
        <tr>
            <td colspan="3" style="font-weight: bold; text-decoration: underline; text-align:center;">
                <asp:Label ID="Label1" runat="server" Text="UAT Exchange House User List"></asp:Label>
            </td>
        </tr>

        <tr>
            <td colspan="3">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="900px" Height="300px">
                    <asp:GridView ID="dgridViewUserInfos" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" AutoGenerateSelectButton="True" Font-Size="Small" OnSelectedIndexChanged="dgridViewUserInfos_SelectedIndexChanged" >
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
        <asp:Label ID="Label2" runat="server" Text="UPDATE ExH Information" ForeColor="DarkRed" Font-Underline="true" Height="30px"></asp:Label>
        <table style="width:100%;">
            <tr>
                <td style="width:110px;">ID :</td>
                <td><asp:TextBox ID="txtIdUpd" runat="server" ReadOnly="true" BackColor="#ccccff"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>PartyId :</td>
                <td><asp:TextBox ID="txtPartyIdUpd" runat="server" ></asp:TextBox>
                    <asp:Label ID="Label5" runat="server" Font-Size="Small" Text="Numeric Value"></asp:Label>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>UserId :</td>
                <td><asp:TextBox ID="txtUserIdUpd" runat="server"  Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Password :</td>
                <td><asp:TextBox ID="txtPassUpd" runat="server" Width="200px"></asp:TextBox>
                    <asp:Label ID="Label4" runat="server" Font-Size="Small" Text="Password will decrypt and stored in DB"></asp:Label>
                </td>
                <td>&nbsp;</td>
            </tr>            
            <tr>
                <td>Is Active :</td>
                <td>
                    <asp:DropDownList ID="ddlUserActiveUpd" runat="server" >
                        <asp:ListItem Selected="True" Value="1">True</asp:ListItem>
                        <asp:ListItem Value="0">False</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>NRT Account :</td>
                <td><asp:TextBox ID="txtNRTUpd" runat="server" Width="150px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>  
            <tr>
                <td>Wallet Account :</td>
                <td><asp:TextBox ID="txtWalletUpd" runat="server" Width="150px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>  
            <tr>
                <td>Party Type :</td>
                <td>
                    <asp:DropDownList ID="ddlPartyTypeUpd" runat="server" Width="130px"  >
                        <asp:ListItem Selected="True" Value="1">1 - WageEarners</asp:ListItem>
                        <asp:ListItem Value="2">2 - ServiceRem</asp:ListItem>
                    </asp:DropDownList>  
                    &nbsp;<asp:Label ID="Label3" runat="server" Font-Size="Small" Text="Wage:1, Service:2"></asp:Label>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btnUpdateExchUserInfo" runat="server" Text="Update" Width="120px" OnClientClick="return ConfirmUpdate();" OnClick="btnUpdateExchUserInfo_Click"  />
                    &nbsp;<asp:Label ID="lblUpdateSuccMsg" runat="server" Text="" ForeColor="Green"></asp:Label>
                    <asp:Label ID="lblUpdateErrorMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
                <td><div class="pleaseWait">Processing ... Please Wait!</div></td>
            </tr>
        </table>
    </div>
    <br />

    <div style="border: solid 1px; padding-left:15px;">
        <asp:Label ID="Label6" runat="server" Text="ADD New ExH Information" ForeColor="DarkRed" Font-Underline="true" Height="30px"></asp:Label>
        <table style="width:100%;">            
            <tr>
                <td style="width:120px;">PartyId :</td>
                <td><asp:TextBox ID="txtNewPartyId" runat="server" ></asp:TextBox>
                    <asp:Label ID="Label7" runat="server" Font-Size="Small" Text="Numeric Value"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>UserId :</td>
                <td><asp:TextBox ID="txtNewUserId" runat="server" Width="150px" ></asp:TextBox></td>
            </tr>
            <tr>
                <td>NRT Account :</td>
                <td><asp:TextBox ID="txtNewNRT" runat="server" Width="150px"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Wallet Account :</td>
                <td><asp:TextBox ID="txtNewWallet" runat="server"  Width="150px"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Party Type :</td>
                <td>
                    <asp:DropDownList ID="ddlPartyTypeNew" runat="server" Width="130px" >
                        <asp:ListItem Selected="True" Value="1">1 - WageEarners</asp:ListItem>
                        <asp:ListItem Value="2">2 - ServiceRem</asp:ListItem>
                    </asp:DropDownList>  
                </td>
            </tr>            
            
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btnSaveExchUserInfo" runat="server" Text="Save" Width="120px" OnClientClick="return ConfirmSave();" OnClick="btnSaveExchUserInfo_Click"  />
                    &nbsp;<asp:Label ID="lblSaveSuccMsg" runat="server" ForeColor="Green"></asp:Label>
                    <asp:Label ID="lblSaveErrorMsg" runat="server" ForeColor="Red"></asp:Label>
                    <div class="pleaseWait">Processing ... Please Wait!</div>
                </td>
            </tr>
        </table>
    </div>
    <br />
    <br />

</asp:Content>
