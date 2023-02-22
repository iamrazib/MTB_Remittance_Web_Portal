<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BEFTNMarking.aspx.cs" Inherits="RemittanceOperation.BEFTNMarking" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
		.pleaseWait { width: 37%; height: 40px; background-color: #cccccc; display: none; z-index:999; position:absolute; left:28.3%; top:60%;font-size:large}
	</style>

    <script type="text/javascript">

        function ConfirmBulkSuccess() {
            if (confirm("Are You Sure to Confirm Success ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }

        function ConfirmBulkCancel() {
            if (confirm("Are You Sure to Confirm Cancel ?") == true) {
                $('.pleaseWait').show();
                return true;
            }
            else
                return false;
        }
        
     </script>


    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <div style="clear:both; font-weight:bold; text-decoration:underline; text-align:center;">
                <asp:Label ID="Label2" runat="server" Text="BEFTN Marking (MAIN TXN)"></asp:Label>
            </div>

            <table style="width: 100%; border: 1px solid #800000; margin-top: 15px; margin-left:20px;">
                
                <tr>
                    <td colspan="3">&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <table style="width: 300px;">
                            <tr>
                                <td colspan="2" style="text-decoration:underline; color:green;">MARK SUCCESS</td>                                
                            </tr>
                            <tr>
                                <td>PIN No:</td>
                                <td><asp:TextBox ID="txtBEFTNSuccessMarkPINNo" runat="server" Width="150px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>Remarks:</td>
                                <td><asp:TextBox ID="txtSuccessRemarks" runat="server" Text="Executed"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td><asp:Button ID="btnBEFTNMarkSuccess" runat="server" Text="Mark Success" OnClick="btnBEFTNMarkSuccess_Click" /></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td><asp:Label ID="lblMarkSuccessMsg" runat="server" Text=""></asp:Label></td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <table style="width: 300px;">
                            <tr>
                                <td colspan="2" style="text-decoration:underline; color:red;">MARK CANCEL</td>                                
                            </tr>
                            <tr>
                                <td>PIN No:</td>
                                <td><asp:TextBox ID="txtBEFTNCancelMarkPINNo" runat="server" Width="150px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>Remarks:</td>
                                <td><asp:TextBox ID="txtCancelledRemarks" runat="server" Text="Cancelled"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td><asp:Button ID="btnBEFTNMarkCancel" runat="server" Text="Mark Cancel" OnClick="btnBEFTNMarkCancel_Click" /></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td><asp:Label ID="lblMarkCancelMsg" runat="server" Text=""></asp:Label></td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <table style="width: 300px;">
                            <tr>
                                <td colspan="2" style="text-decoration:underline; color:red;">RIPPLE - MARK CANCEL</td>                                
                            </tr>
                            <tr>
                                <td>PIN No:</td>
                                <td><asp:TextBox ID="txtRippleEFTCancelMarkPINNo" runat="server" Width="150px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>Remarks:</td>
                                <td><asp:TextBox ID="txtRippleEFTCancelledRemarks" runat="server" Text="Cancelled"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td><asp:Button ID="btnRippleEFTMarkCancel" runat="server" Text="Mark Cancel" OnClick="btnRippleEFTMarkCancel_Click" /></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td><asp:Label ID="lblRippleEFTMarkCancelMsg" runat="server" Text=""></asp:Label></td>
                            </tr>
                        </table>
                    </td>
                </tr>

            </table>
            <br />
            <br />
            <table style="width: 100%; border: 1px solid #800000; margin-top: 15px; margin-left:20px;">
                <tr>
                    <td colspan="2" style="text-decoration:underline; color:green">MARK SUCCESS as BULK :-: MAIN</td>
                    <td colspan="2" style="text-decoration:underline; color:red;">MARK CANCEL as BULK :-: MAIN</td>
                </tr>
                <tr>
                    <td colspan="2">Pin No:</td>
                    <td colspan="2">Pin No:</td>
                </tr>
                <tr>
                    <td rowspan="3" style="vertical-align:top; width:200px;"><asp:TextBox ID="textBoxRefNoBulkSuccess" runat="server" Height="200px" TextMode="MultiLine" style="resize:none"></asp:TextBox></td>
                    <td style="vertical-align:top;">Remarks: <asp:TextBox ID="txtSuccessRemarksBulk" runat="server" Text="Executed"></asp:TextBox></td>

                    <td rowspan="3" style="vertical-align:top; width:200px;"><asp:TextBox ID="txtBEFTNCancelMarkPINNoBulk" runat="server" Height="200px" TextMode="MultiLine" style="resize:none"></asp:TextBox></td>
                    <td style="vertical-align:top;">Remarks: <asp:TextBox ID="txtCancelledRemarksBulk" runat="server" Text="Cancelled"></asp:TextBox></td>
                </tr>
                <tr>
                    <td style="vertical-align:top;">
                        <asp:Button ID="btnBEFTNMarkSuccessBulk" runat="server" Text="Mark Success" OnClick="btnBEFTNMarkSuccessBulk_Click" OnClientClick="return ConfirmBulkSuccess();" />
                        <div class="pleaseWait">Processing ... Please Wait!</div>
                    </td>
                    <td style="vertical-align:top;">
                        <asp:Button ID="btnBEFTNMarkCancelBulk" runat="server" Text="Mark Cancel" OnClick="btnBEFTNMarkCancelBulk_Click" OnClientClick="return ConfirmBulkCancel();" />
                        <div class="pleaseWait">Processing ... Please Wait!</div>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align:top;"><asp:Label ID="lblMarkSuccessMsgBulk" runat="server"></asp:Label></td>
                    <td style="vertical-align:top;"><asp:Label ID="lblMarkCancelMsgBulk" runat="server"></asp:Label></td>
                </tr>
            </table>
            <br />
            <br />

            <table style="width: 100%; border: 1px solid #800000; margin-top: 15px; margin-left:20px; visibility:hidden" >
                <tr>
                    <td colspan="2" style="text-decoration:underline; color:#FF0066; font-weight: bold;">MARK SUCCESS as BULK :-: INCENTIVE</td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="2">Pin No:</td>
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <td rowspan="3" style="vertical-align:top; width:300px;">
                        <asp:TextBox ID="textBoxRefNoBulkIncentiveSuccess" runat="server" Height="200px" TextMode="MultiLine" style="resize:none" Width="254px"></asp:TextBox></td>
                    <td style="vertical-align:top;">Remarks: <asp:TextBox ID="txtSuccessRemarksIncentiveBulk" runat="server" Text="Executed"></asp:TextBox></td>

                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td style="vertical-align:top;">
                        <asp:Button ID="btnBEFTNIncentiveMarkSuccessBulk" runat="server" Text="Mark Success"  OnClientClick="return ConfirmBulkSuccess();" OnClick="btnBEFTNIncentiveMarkSuccessBulk_Click" />
                        <div class="pleaseWait">Processing ... Please Wait!</div>
                    </td>
                    <td style="vertical-align:top;">                        
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align:top;"><asp:Label ID="lblMarkSuccessIncentiveMsgBulk" runat="server"></asp:Label></td>
                    <td></td>
                </tr>
            </table>
            <br />
            <br />

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
