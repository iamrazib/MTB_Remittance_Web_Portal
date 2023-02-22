<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="RoutingInfo.aspx.cs" Inherits="RemittanceOperation.RoutingInfo" %>
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

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%;">
                <tr>
                    <td colspan="3" style="font-weight: bold; text-decoration: underline;">
                        <asp:Label ID="Label1" runat="server" Text="Routing Info"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>Select Bank : <asp:DropDownList ID="cbBankNameRoutingInfo" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cbBankNameRoutingInfo_SelectedIndexChanged"></asp:DropDownList><asp:DropDownList ID="cbBankCode" runat="server" Visible="false"></asp:DropDownList></td>
                    <td>Select District : <asp:DropDownList ID="cbDistrictNameRoutingInfo" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cbDistrictNameRoutingInfo_SelectedIndexChanged"></asp:DropDownList><%--<asp:DropDownList ID="cbBranchCode" runat="server"></asp:DropDownList>--%></td>
                    <td>Select Branch : <asp:DropDownList ID="cbBranchNameRoutingInfo" runat="server" AutoPostBack="true"></asp:DropDownList><%--<asp:DropDownList ID="cbDistrictCode" runat="server"></asp:DropDownList>--%></td>
                </tr>
                <tr>
                    <td colspan="3">Branch Name / Routing : <asp:TextBox ID="txtRoutingNo" runat="server" Width="220px" OnTextChanged="txtRoutingNo_TextChanged" AutoPostBack="True"></asp:TextBox>                    
                       &nbsp;&nbsp; <asp:Button ID="btnSearchRouting" runat="server" Text="Search" Width="120px" OnClick="btnSearchRouting_Click" OnClientClick="return TxnSearch();" />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
                        <div class="WaitForSearching">Searching... Please Wait!</div>
                    </td>
                    <td style="text-align:right;">
                        <%--<asp:LinkButton ID="LinkButtonDownloadList" runat="server" OnClick="LinkButtonDownloadList_Click">Download List</asp:LinkButton>--%>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="900px" Height="300px">
                            <asp:GridView ID="dgridViewRoutingInfos" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" AutoGenerateSelectButton="True" OnSelectedIndexChanged="dgridViewRoutingInfos_SelectedIndexChanged" Font-Size="Small">
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
                <asp:Label ID="Label2" runat="server" Text="UPDATE Routing Information" ForeColor="DarkRed" Font-Underline="true"></asp:Label>
                <table style="width:100%;">
                    <tr>
                        <td style="width:110px;">SL No. :</td>
                        <td><asp:TextBox ID="txtAutoId" runat="server" ReadOnly="true" BackColor="#ccccff"></asp:TextBox></td>   <%--style="color: grey; background-color: #F0F0F0;"--%>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>Bank Name :</td>
                        <td>
                            <asp:DropDownList ID="cbBankNameUpdateRouting" runat="server" />
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>Branch Name :</td>
                        <td><asp:TextBox ID="txtBranchName" runat="server" Width="250px"></asp:TextBox></td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>District Name :</td>
                        <td>
                            <asp:DropDownList ID="cbDistrictNameUpdateRouting" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>Routing No :</td>
                        <td><asp:TextBox ID="txtRoutingNoEdit" runat="server"></asp:TextBox></td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="btnUpdateRoutingInfo" runat="server" Text="Update" Width="120px" OnClick="btnUpdateRoutingInfo_Click" OnClientClick="return ConfirmUpdate();" />
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
                <asp:Label ID="Label3" runat="server" Text="ADD Routing Information" ForeColor="DarkRed" Font-Underline="true"></asp:Label>
                <table style="width:100%;">
                    <tr>
                        <td style="width:110px;">Bank Name :</td>
                        <td><asp:DropDownList ID="cbBankNameNewRouting" runat="server" OnSelectedIndexChanged="cbBankNameNewRouting_SelectedIndexChanged"></asp:DropDownList>
                            <asp:DropDownList ID="cbBankCodeNewRouting" runat="server" Visible="False"></asp:DropDownList>
                        </td>   <%--style="color: grey; background-color: #F0F0F0;"--%>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>Branch Name :</td>
                        <td><asp:TextBox ID="txtBranchNameNewRouting" runat="server" Width="250px"></asp:TextBox></td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>District Name :</td>
                        <td><asp:DropDownList ID="cbDistrictNameNewRouting" runat="server"></asp:DropDownList></td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>Routing No :</td>
                        <td><asp:TextBox ID="txtRoutingNoNewRouting" runat="server"></asp:TextBox></td>
                        <td>&nbsp;</td>
                    </tr>                    
                    <tr>
                        <td>&nbsp;</td>
                        <td><asp:Button ID="btnSaveRoutingInfo" runat="server" Text="Save" Width="120px" OnClick="btnSaveRoutingInfo_Click" OnClientClick="return ConfirmSave();" />
                            &nbsp;<asp:Label ID="lblSaveMsg" runat="server" Text="" ForeColor="Green"></asp:Label>
                        </td>
                        <td><div class="pleaseWait">Processing ... Please Wait!</div></td>
                    </tr>
                </table>
            </div>

            <br />
            <br />

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
