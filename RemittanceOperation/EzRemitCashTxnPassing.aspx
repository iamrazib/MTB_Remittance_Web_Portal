<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EzRemitCashTxnPassing.aspx.cs" Inherits="RemittanceOperation.EzRemitCashTxnPassing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   
     <style type="text/css">
		.WaitForSearching { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:40%;font-size:large}
        .pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:40%;font-size:large}
	</style>

    <script type="text/javascript">
        function TxnSearch() {
            $('.WaitForSearching').show();
        }

        function ConfirmSave() {
            if (confirm("Are You Sure to Payment ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }
    </script>


    <table style="width: 100%; margin-top:20px; margin-left:15px;">
                <tr>
                    <td colspan="4" style="font-weight: bold; text-decoration: underline; text-align:center;">
                        <asp:Label ID="Label1" runat="server" Text="EzRemit Over The Counter Payment"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr style="padding-left:20px;">
                    <td>Remitance No:</td>
                    <td><asp:TextBox ID="textBoxRefNo" runat="server" Width="200px"></asp:TextBox></td>
                    <td><asp:Button ID="btnSearch" runat="server" Text="Search" Width="120px" OnClientClick="return TxnSearch();" OnClick="btnSearch_Click" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr style="padding-left:20px;">
                    <td>Beneficiary Name:</td>
                    <td><asp:TextBox ID="textBoxBenfName" runat="server" ReadOnly="true" Width="250px"></asp:TextBox></td>
                    <td>Sender Name:</td>
                    <td><asp:TextBox ID="textBoxSenderName" runat="server" ReadOnly="true" Width="250px"></asp:TextBox></td>
                </tr>
                <tr style="padding-left:20px;">
                    <td>Beneficiary Address :</td>
                    <td><asp:TextBox ID="textBoxBenfAddr" runat="server" ReadOnly="true" Width="250px"></asp:TextBox></td>
                    <td>Sender Address :</td>
                    <td><asp:TextBox ID="textBoxSenderAddr" runat="server" ReadOnly="true" Width="250px"></asp:TextBox></td>
                </tr>
                <tr style="padding-left:20px;">
                    <td>Beneficiary Phone No:</td>
                    <td><asp:TextBox ID="textBoxBenfPhone" runat="server" ReadOnly="true" Width="250px"></asp:TextBox></td>
                    <td>Sender Contact No:</td>
                    <td><asp:TextBox ID="textBoxSenderContact" runat="server" ReadOnly="true" Width="250px"></asp:TextBox></td>
                </tr>
                <tr style="padding-left:20px;">
                    <td>Remitance Status:</td>
                    <td><asp:TextBox ID="textBoxRemitStatus" runat="server" ReadOnly="true" Width="250px"></asp:TextBox></td>
                    <td>Branch Name:</td>
                    <td><asp:TextBox ID="textBoxBranch" runat="server" ReadOnly="true" Width="250px"></asp:TextBox></td>
                </tr>
                <tr style="padding-left:20px;">
                    <td>Amount:</td>
                    <td><asp:TextBox ID="textBoxAmount" runat="server" ReadOnly="true"></asp:TextBox></td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr style="padding-left:20px;">
                    <td>&nbsp;</td>
                    <td><div class="WaitForSearching">Please Wait...!</div></td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                
                <tr>
                    <td colspan="4" style="font-weight: bold; text-decoration: underline;">
                        <asp:Label ID="Label2" runat="server" Text="KYC Information"></asp:Label>
                    </td>
                </tr>
                <tr style="padding-left:20px;">
                    <td>ID Type:</td>
                    <td>
                        <asp:DropDownList ID="cbIDType" runat="server"></asp:DropDownList></td>
                    <td rowspan="3">Address:</td>
                    <td rowspan="3"><asp:TextBox ID="textBoxAddress" runat="server" TextMode="MultiLine" Width="250px" Height="80px" style="resize:none"></asp:TextBox></td>
                </tr>
                <tr style="padding-left:20px;">
                    <td>ID Number:</td>
                    <td><asp:TextBox ID="textBoxIDNum" runat="server" Width="200px"></asp:TextBox></td>
                </tr>
                <tr style="padding-left:20px;">
                    <td>Mobile Number :</td>
                    <td><asp:TextBox ID="textBoxMobile" runat="server" Width="200px"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Button ID="btnPayment" runat="server" Text="Payment" Width="120px"  OnClientClick="return ConfirmUpdate();" OnClick="btnPayment_Click" />
                    </td>
                    <td colspan="2">
                        <asp:Label ID="lblPaymentMsg" runat="server" Text="" ForeColor="Green"></asp:Label>
                    </td>
                </tr>
            </table>
            <br />
            <div style="clear:both; float:left; margin-left:15px;">
                <asp:Label ID="lblSearchStats" runat="server" Text="" ForeColor="Red"></asp:Label>
            </div>

</asp:Content>
