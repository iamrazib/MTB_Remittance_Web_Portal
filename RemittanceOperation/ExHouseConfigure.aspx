<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ExHouseConfigure.aspx.cs" Inherits="RemittanceOperation.ExHouseConfigure" %>
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

    <div style="clear:both;">
        <table style="width: 100%; margin-left: 15px;">
            <tr>
                <td style="font-weight: bold; text-decoration: underline;">
                    <asp:Label ID="Label1" runat="server" Text="Exchange House Info"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="950px" Height="350px">
                        <asp:GridView ID="dgridViewExhInfos" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" AutoGenerateSelectButton="True" OnSelectedIndexChanged="dgridViewExhInfos_SelectedIndexChanged" >
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
    </div>
    <%--<br />--%>
    <div style="clear:both; margin-bottom:10px; margin-top:10px;">
        <table style="width: 100%; margin-left: 15px;">
            <tr>
                <td style="text-align:right; padding-right:15px;">
                    <asp:Button ID="btnDownloadExhList" runat="server" Text="Download List" Width="150" OnClick="btnDownloadExhList_Click"  />
                </td>
            </tr>
        </table>        
    </div>

    <div style="border: solid 1px; padding-left:15px;">        
        <table style="width:100%;">
            <tr>
                <td colspan="2">
                    <asp:Label ID="Label2" runat="server" Text="UPDATE ExchangeHouse Information" ForeColor="DarkRed" Font-Underline="true" Height="30px"></asp:Label>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td style="width:130px;">SL No. :</td>
                <td><asp:TextBox ID="txtSlNoUpd" runat="server" ReadOnly="true" BackColor="#ccccff"></asp:TextBox></td>   <%--style="color: grey; background-color: #F0F0F0;"--%>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Party Id :</td>
                <td><asp:TextBox ID="txtPartyIdUpd" runat="server" ></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>User Id :</td>
                <td><asp:TextBox ID="txtUserIdUpd" runat="server" Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>ExhType :</td>
                <td>
                    <asp:DropDownList ID="ddlExhTypeUpd" runat="server" >
                        <asp:ListItem Selected="True" Value="1">WageEarners</asp:ListItem>
                        <asp:ListItem Value="2">ServiceRemittance</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>ExHouseName :</td>
                <td><asp:TextBox ID="txtExHouseNameUpd" runat="server" Width="350px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>ExHCountry :</td>
                <td><asp:TextBox ID="txtExHCountryUpd" runat="server" Width="350px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>CompanyType :</td>
                <td><asp:TextBox ID="txtCompanyTypeUpd" runat="server" Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>NRT Account :</td>
                <td><asp:TextBox ID="txtNRTAccountUpd" runat="server" Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Wallet Account :</td>
                <td><asp:TextBox ID="txtWalletAccountUpd" runat="server" Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>USD Account :</td>
                <td><asp:TextBox ID="txtUSDAccountUpd" runat="server" Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>AED Account :</td>
                <td><asp:TextBox ID="txtAEDAccountUpd" runat="server" Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Is Active :</td>
                <td>
                    <asp:DropDownList ID="ddlExhActiveUpd" runat="server" >
                        <asp:ListItem Selected="True" Value="1">YES</asp:ListItem>
                        <asp:ListItem Value="0">NO</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>To Address :</td>
                <td><asp:TextBox ID="txtToAddressUpd" runat="server" Width="400px" Height="48px" TextMode="MultiLine" style="resize:none"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>CcAddress :</td>
                <td><asp:TextBox ID="txtCcAddressUpd" runat="server" Width="400px" Height="39px" TextMode="MultiLine" style="resize:none"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>ApiOrFile :</td>
                <td>
                    <asp:DropDownList ID="ddlApiOrFileUpd" runat="server" >
                        <asp:ListItem Selected="True" Value="API">API</asp:ListItem>
                        <asp:ListItem Value="FILE">FILE</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>&nbsp;</td>
            </tr>

            <tr>
                <td>BEFTN BDT Rate:</td>
                <td><asp:TextBox ID="txtBEFTNBDTRateUpd" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>bKash BDT Rate:</td>
                <td><asp:TextBox ID="txtBKASHBDTRateUpd" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>CASH BDT Rate:</td>
                <td><asp:TextBox ID="txtCASHBDTRateUpd" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>A/C BDT Rate:</td>
                <td><asp:TextBox ID="txtACCREDITBDTRateUpd" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>BEFTN USD Rate:</td>
                <td><asp:TextBox ID="txtBEFTNUSDRateUpd" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>bKash USD Rate:</td>
                <td><asp:TextBox ID="txtBKASHUSDRateUpd" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>CASH USD Rate::</td>
                <td><asp:TextBox ID="txtCASHUSDRateUpd" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>A/C USD Rate:</td>
                <td><asp:TextBox ID="txtACCREDITUSDRateUpd" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>

            <tr>
                <td>Comm CURR :</td>
                <td>
                    <asp:DropDownList ID="ddlCommCurrUpd" runat="server" >
                        <asp:ListItem Selected="True" Value="BDT">BDT</asp:ListItem>
                        <asp:ListItem Value="USD">USD</asp:ListItem>
                        <asp:ListItem Value="GBP">GBP</asp:ListItem>
                        <asp:ListItem Value="EUR">EUR</asp:ListItem>
                        <asp:ListItem Value="JPY">JPY</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Exh Short Name:</td>
                <td><asp:TextBox ID="txtExhShortNameUpd" runat="server" Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Has COC ?</td>
                <td>
                    <asp:DropDownList ID="ddlHasCOCUpd" runat="server" >
                        <asp:ListItem Selected="True" Value="Y">Y</asp:ListItem>
                        <asp:ListItem Value="N">N</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btnUpdateExhInfo" runat="server" Text="Update" Width="120px" OnClientClick="return ConfirmUpdate();" OnClick="btnUpdateExhInfo_Click"  />
                    &nbsp;<asp:Label ID="lblUpdateSuccMsg" runat="server" Text="" ForeColor="Green"></asp:Label>
                    <asp:Label ID="lblUpdateErrorMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
                <td><div class="pleaseWait">Processing ... Please Wait!</div></td>
            </tr>
        </table>
    </div>
    <br />
    <div style="border: solid 1px; padding-left:15px;">
        <table style="width:100%;">
            <tr>
                <td colspan="2">
                    <asp:Label ID="Label3" runat="server" Text="ADD NEW ExchangeHouse Information" ForeColor="DarkRed" Font-Underline="true" Height="30px"></asp:Label>
                </td>
                <td>&nbsp;</td>
            </tr>            
            <tr>
                <td style="width:130px;">Party Id :</td>
                <td><asp:TextBox ID="txtPartyIdNew" runat="server" ></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>User Id :</td>
                <td><asp:TextBox ID="txtUserIdNew" runat="server" Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>ExhType :</td>
                <td>
                    <asp:DropDownList ID="ddlExhTypeNew" runat="server" >
                        <asp:ListItem Selected="True" Value="1">WageEarners</asp:ListItem>
                        <asp:ListItem Value="2">ServiceRemittance</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>ExHouseName :</td>
                <td><asp:TextBox ID="txtExHouseNameNew" runat="server" Width="350px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>ExHCountry :</td>
                <td><asp:TextBox ID="txtExHCountryNew" runat="server" Width="350px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>CompanyType :</td>
                <td><asp:TextBox ID="txtCompanyTypeNew" runat="server" Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>NRT Account :</td>
                <td><asp:TextBox ID="txtNRTAccountNew" runat="server" Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Wallet Account :</td>
                <td><asp:TextBox ID="txtWalletAccountNew" runat="server" Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>USD Account :</td>
                <td><asp:TextBox ID="txtUSDAccountNew" runat="server" Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>AED Account :</td>
                <td><asp:TextBox ID="txtAEDAccountNew" runat="server" Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>            
            <tr>
                <td>To Address :</td>
                <td><asp:TextBox ID="txtToAddressNew" runat="server" Width="400px" Height="48px" TextMode="MultiLine" style="resize:none"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>CcAddress :</td>
                <td><asp:TextBox ID="txtCcAddressNew" runat="server" Width="400px" Height="39px" TextMode="MultiLine" style="resize:none"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>ApiOrFile :</td>
                <td>
                    <asp:DropDownList ID="ddlApiOrFileNew" runat="server" >
                        <asp:ListItem Selected="True" Value="API">API</asp:ListItem>
                        <asp:ListItem Value="FILE">FILE</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>&nbsp;</td>
            </tr>

            <tr>
                <td>BEFTN BDT Rate:</td>
                <td><asp:TextBox ID="txtBEFTNBDTRateNew" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>bKash BDT Rate:</td>
                <td><asp:TextBox ID="txtBKASHBDTRateNew" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>CASH BDT Rate:</td>
                <td><asp:TextBox ID="txtCASHBDTRateNew" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>A/C BDT Rate:</td>
                <td><asp:TextBox ID="txtACCREDITBDTRateNew" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>BEFTN USD Rate:</td>
                <td><asp:TextBox ID="txtBEFTNUSDRateNew" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>bKash USD Rate:</td>
                <td><asp:TextBox ID="txtBKASHUSDRateNew" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>CASH USD Rate::</td>
                <td><asp:TextBox ID="txtCASHUSDRateNew" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>A/C USD Rate:</td>
                <td><asp:TextBox ID="txtACCREDITUSDRateNew" runat="server" Width="100px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Comm CURR :</td>
                <td>
                    <asp:DropDownList ID="ddlCommCurrNew" runat="server" >
                        <asp:ListItem Selected="True" Value="BDT">BDT</asp:ListItem>
                        <asp:ListItem Value="USD">USD</asp:ListItem>
                        <asp:ListItem Value="GBP">GBP</asp:ListItem>
                        <asp:ListItem Value="EUR">EUR</asp:ListItem>
                        <asp:ListItem Value="JPY">JPY</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Exh Short Name:</td>
                <td><asp:TextBox ID="txtExhShortNameNew" runat="server" Width="200px"></asp:TextBox></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Has COC ?</td>
                <td>
                    <asp:DropDownList ID="ddlHasCOCNew" runat="server" >
                        <asp:ListItem Selected="True" Value="Y">Y</asp:ListItem>
                        <asp:ListItem Value="N">N</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btnSaveExhInfo" runat="server" Text="Save" Width="120px" OnClientClick="return ConfirmSave();" OnClick="btnSaveExhInfo_Click"  />
                    &nbsp;<asp:Label ID="lblSaveSuccMsg" runat="server" Text="" ForeColor="Green"></asp:Label>
                    <asp:Label ID="lblSaveErrorMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
                <td><div class="pleaseWait">Processing ... Please Wait!</div></td>
            </tr>
        </table>
    </div>

    <br />
    <div style="border: solid 1px; padding-left:15px;">
        <asp:Label ID="Label4" ForeColor="#CC0000" runat="server" Text="Exchange House Having COC transaction : "></asp:Label><asp:Label ID="lblCountCOC" runat="server" Text=""></asp:Label> 
    </div>
    <br />
    <br />
</asp:Content>
