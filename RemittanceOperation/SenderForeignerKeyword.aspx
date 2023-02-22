<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SenderForeignerKeyword.aspx.cs" Inherits="RemittanceOperation.SenderForeignerKeyword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">
        
        function ConfirmRemove() {
            if (confirm("Are You Sure to Remove ?") == true) {
                return true;
            }
            else
                return false;
        }

        function ConfirmSave() {
            if (confirm("Are You Sure to Save ?") == true) {
                return true;
            }
            else
                return false;
        }
    </script>

    <table style="width: 100%; font-family: Arial; margin-left:15px;">
        <tr>
            <td colspan="2" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label1" runat="server" Text="Foreigner Sender Name List"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <div>
                    <div style="float:left;">
                        <asp:Button ID="btnForeignerSenderSearch" runat="server" Text="Search" Width="150px" OnClick="btnForeignerSenderSearch_Click" />
                    </div>
                    <div style="float:right;">
                        <asp:LinkButton ID="LinkButtonDownloadSenderNameList" runat="server" Font-Size="Small" OnClick="LinkButtonDownloadSenderNameList_Click">Download List</asp:LinkButton>
                    </div>
                 </div>   
            </td>
            <td style="text-decoration: underline; font-family: Verdana, Geneva, Tahoma, sans-serif; font-size: small; float:right !important;">Add New Sender Name:</td>
        </tr>
        <tr>
            <td>                
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="400px" Height="170px">
                    <asp:GridView ID="dataGridViewForeignerSenderName" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small" OnRowDataBound="dataGridViewForeignerSenderName_RowDataBound">
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
            <td style="vertical-align: top; padding-left: 20px;">

                <table style="vertical-align: top; margin-left: 15px;">                     
                    <tr>
                        <td style="width: 100px;">
                            <asp:TextBox ID="textBoxForeignerSenderName" runat="server" Width="308px" TextMode="MultiLine" style="resize:none" Height="150px"></asp:TextBox>
                        </td>
                     </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnForeignerSenderNameSave" runat="server" Text="Save" Width="100px" OnClick="btnForeignerSenderNameSave_Click" OnClientClick="return ConfirmSave();" />
                            &nbsp; <asp:Label ID="lblForeignerSenderNameSaveResult" runat="server" ForeColor="Red" Font-Size="Small"></asp:Label>
                        </td>
                    </tr>
                </table>
                
                <%--<table style="width: 100%;">
                    <tr>
                        <td colspan="3" style="text-decoration: underline;">Search In List:</td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td style="width: 45px;">
                            <asp:Label ID="Label2" runat="server" Text="Name"></asp:Label></td>
                        <td style="width: 10px;">:</td>
                        <td style="width: 100px;">
                            <asp:TextBox ID="txtForeignerSenderNameSearchInput" runat="server" Width="200px"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="btnSearchForeignerSenderNameIsExist" runat="server" Text="Search" Width="100px" OnClick="btnSearchForeignerSenderNameIsExist_Click" /></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Label ID="lblForeignerSenderNameSearchResult" runat="server" Text="" ForeColor="Red"></asp:Label></td>
                    </tr>
                </table>--%>
                
            </td>
        </tr>        
    </table>
    <table style="margin-left:15px;"> 
        <tr>
            <td colspan="3" style="text-decoration: underline;">Search In List:</td>
            <td style="width:50px">&nbsp;</td>
            <td colspan="3" style="text-decoration: underline;">Remove Any Name From System</td>
        </tr>        
        <tr>
            <td style="width: 45px; vertical-align:top;"><asp:Label ID="Label3" runat="server" Text="Name"></asp:Label></td>
                <td style="width: 10px; vertical-align:top;">:</td>
                <td style="width: 100px; vertical-align:top;"><asp:TextBox ID="txtForeignerSenderNameSearchInput" runat="server" Width="200px"></asp:TextBox>
                    <asp:Button ID="btnSearchForeignerSenderNameIsExist" runat="server" Text="Search" Width="100px" OnClick="btnSearchForeignerSenderNameIsExist_Click" />
                </td>
                <td>&nbsp;</td>


            <td style="vertical-align:top; padding-top:10px;"><asp:Label ID="Label4" runat="server" Text="Enter ID "></asp:Label></td>
            <td style="vertical-align:top; padding-top:10px;">:</td>
            <td style="vertical-align:top; padding-top:10px;">
                <asp:TextBox ID="txtSenderId" runat="server"></asp:TextBox>&nbsp;
                <asp:Button ID="btnRemoveSenderName" runat="server" Text="Remove" OnClick="btnRemoveSenderName_Click" OnClientClick="return ConfirmRemove();" /><br />
                <asp:Label ID="lblRemoveStatus" runat="server" Text="" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td><asp:Label ID="lblForeignerSenderNameSearchResult" runat="server" Text="" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td></td>
        </tr>
    </table>
    <br />
    <hr />
    <table style="width: 100%; font-family: Arial; margin-left:15px;">
            <tr>
                <td colspan="2" style="font-weight: bold; text-decoration: underline;">
                    <asp:Label ID="Label5" runat="server" Text="Foreigner Sender Account List"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <div>
                        <div style="float:left;">
                            <asp:Button ID="btnForeignerSenderAccountSearch" runat="server" Text="Search" Width="150px" OnClick="btnForeignerSenderAccountSearch_Click"  />
                        </div>
                        <div style="float:right;">
                            <asp:LinkButton ID="LinkButtonDownloadSenderAccountList" runat="server" Font-Size="Small" OnClick="LinkButtonDownloadSenderAccountList_Click" >Download List</asp:LinkButton>
                        </div>
                    </div>   
                </td>
                <td style="text-decoration: underline;font-family: Verdana, Geneva, Tahoma, sans-serif; font-size: small; float:right !important;">Add New Sender Account:</td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="Panel2" runat="server" ScrollBars="Both" Width="400px" Height="170px">
                        <asp:GridView ID="dataGridViewForeignerSenderAccount" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small" >
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
                <td style="vertical-align: top; padding-left: 20px;">
                    <table style="vertical-align: top; margin-left: 15px;">                     
                        <tr>
                            <td>
                                <asp:TextBox ID="textBoxForeignerSenderAccount" runat="server" Width="300px" TextMode="MultiLine" style="resize:none" Height="150px"></asp:TextBox>
                            </td>
                         </tr>
                        <tr>
                            <td>
                                <asp:Button ID="btnForeignerSenderAccountSave" runat="server" Text="Save" Width="100px" OnClientClick="return ConfirmSave();" OnClick="btnForeignerSenderAccountSave_Click"  />
                                &nbsp; <asp:Label ID="lblForeignerSenderAccountSaveResult" runat="server" ForeColor="Red" Font-Size="Small" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <%--<tr>
                <td colspan="2">&nbsp;</td>
            </tr>--%>
        </table>
        
        <table style="vertical-align: top; margin-left: 15px;">            
            <tr>
                <td colspan="3" style="text-decoration: underline;">Search In List:</td>
                <td style="width: 50px">&nbsp;</td>
                <td colspan="3" style="text-decoration: underline;">Remove Any Account From System</td>
            </tr>            
            <tr>
                <td style="width: 45px; vertical-align:top;"><asp:Label ID="Label2" runat="server" Text="Name"></asp:Label></td>
                <td style="width: 10px; vertical-align:top;">:</td>
                <td style="width: 100px; vertical-align:top;"><asp:TextBox ID="txtForeignerSenderAccountSearchInput" runat="server" Width="200px"></asp:TextBox>
                    <asp:Button ID="btnSearchForeignerSenderAccountIsExist" runat="server" Text="Search" Width="100px" OnClick="btnSearchForeignerSenderAccountIsExist_Click"  />
                </td>
                <td>&nbsp;</td>

                <td style="vertical-align: top; padding-top: 10px;">
                    <asp:Label ID="Label6" runat="server" Text="Enter ID "></asp:Label></td>
                <td style="vertical-align: top; padding-top: 10px;">:</td>
                <td style="vertical-align: top; padding-top: 10px;">
                    <asp:TextBox ID="txtFrgnerSenderAccId" runat="server"></asp:TextBox>&nbsp;
                    <asp:Button ID="btnRemoveForeignerSenderAccountNo" runat="server" Text="Remove" OnClientClick="return ConfirmRemove();" OnClick="btnRemoveForeignerSenderAccountNo_Click"   /><br />
                    <asp:Label ID="lblRemoveForeignerSenderAccountStatus" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td><asp:Label ID="lblForeignerSenderAccountNoSearchResult" runat="server" Text="" ForeColor="Red"></asp:Label></td>
            </tr>            
        </table>
       <br />

</asp:Content>
