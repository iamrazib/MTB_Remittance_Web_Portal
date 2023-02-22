<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="NonIndividualKeyword.aspx.cs" Inherits="RemittanceOperation.NonIndividualKeyword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  
    <script type="text/javascript">

        function ConfirmRemove() {
            if (confirm("Are You Sure to Remove ?") == true) 
                return true;            
            else  return false;
        }
        function ConfirmSave() {
            if (confirm("Are You Sure to Save ?") == true) 
                return true;            
            else  return false;
        }
    </script>

    <br />
    <table style="width: 100%; font-family: Arial; margin-left:15px;">
        <tr>
            <td colspan="2" style="font-weight: bold; text-decoration: underline;">
                <asp:Label ID="Label1" runat="server" Text="Company Name List"></asp:Label>
            </td>
        </tr>
        <tr>
            <td><asp:Button ID="btnCompanyNameSearch" runat="server" Text="Search" Width="150px" OnClick="btnCompanyNameSearch_Click" /></td>
            <td style="text-decoration: underline;">Add New Name:</td>
        </tr>
        <tr>
            <td>                
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="400px" Height="170px">
                    <asp:GridView ID="dataGridViewCompanyName" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small" >
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
                        <%--<td style="width: 38px;">
                            &nbsp;</td>
                        <td style="width: 10px;">:</td>--%>
                        <td style="width: 100px;">
                            <asp:TextBox ID="textBoxCompanyName" runat="server" Width="300px" TextMode="MultiLine" style="resize:none" Height="150px"></asp:TextBox>
                        </td>
                     </tr>
                    <tr>
                        <%--<td>&nbsp;</td>
                        <td>&nbsp;</td>--%>
                        <td>
                            <asp:Button ID="btnCompanyNameSave" runat="server" Text="Save" Width="100px" OnClientClick="return ConfirmSave();" OnClick="btnCompanyNameSave_Click" />
                            &nbsp; <asp:Label ID="lblCompanyNameSaveResult" runat="server" ForeColor="Red" Font-Size="Small" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2">&nbsp;</td>
        </tr>
        </table>
        <table style="vertical-align: top; margin-left: 15px;">            
            <tr>
                <td colspan="3" style="text-decoration: underline;">Search In List:</td>
                <td style="width: 50px">&nbsp;</td>
                <td colspan="3" style="text-decoration: underline;">Remove Any Name From System</td>
            </tr>            
            <tr>
                <td style="width: 45px; vertical-align:top;"><asp:Label ID="Label3" runat="server" Text="Name"></asp:Label></td>
                <td style="width: 10px; vertical-align:top;">:</td>
                <td style="width: 100px; vertical-align:top;"><asp:TextBox ID="txtCompanyNameSearchInput" runat="server" Width="200px"></asp:TextBox>
                    <asp:Button ID="btnSearchCompanyNameIsExist" runat="server" Text="Search" Width="100px" OnClick="btnSearchCompanyNameIsExist_Click" />
                </td>
                <td>&nbsp;</td>

                <td style="vertical-align: top; padding-top: 10px;">
                    <asp:Label ID="Label4" runat="server" Text="Enter ID "></asp:Label></td>
                <td style="vertical-align: top; padding-top: 10px;">:</td>
                <td style="vertical-align: top; padding-top: 10px;">
                    <asp:TextBox ID="txtCompId" runat="server"></asp:TextBox>&nbsp;
                            <asp:Button ID="btnRemoveCompanyName" runat="server" Text="Remove" OnClientClick="return ConfirmRemove();" OnClick="btnRemoveCompanyName_Click" /><br />
                    <asp:Label ID="lblRemoveStatus" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td><asp:Label ID="lblCompanyNameSearchResult" runat="server" Text="" ForeColor="Red"></asp:Label></td>
            </tr>            
        </table>
       <br />
       <hr />

        <table style="width: 100%; font-family: Arial; margin-left:15px;">
            <tr>
                <td colspan="2" style="font-weight: bold; text-decoration: underline;">
                    <asp:Label ID="Label5" runat="server" Text="Company Account List"></asp:Label>
                </td>
            </tr>
            <tr>
                <td><asp:Button ID="btnCompanyAccountSearch" runat="server" Text="Search" Width="150px" OnClick="btnCompanyAccountSearch_Click" /></td>
                <td style="text-decoration: underline;">Add New Company Account:</td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="Panel2" runat="server" ScrollBars="Both" Width="400px" Height="170px">
                        <asp:GridView ID="dataGridViewCompanyAccount" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small" >
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
                                <asp:TextBox ID="textBoxCompanyAccount" runat="server" Width="300px" TextMode="MultiLine" style="resize:none" Height="150px"></asp:TextBox>
                            </td>
                         </tr>
                        <tr>
                            <td>
                                <asp:Button ID="btnCompanyAccountSave" runat="server" Text="Save" Width="100px" OnClientClick="return ConfirmSave();" OnClick="btnCompanyAccountSave_Click" />
                                &nbsp; <asp:Label ID="lblCompanyAccountSaveResult" runat="server" ForeColor="Red" Font-Size="Small" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
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
                <td style="width: 100px; vertical-align:top;"><asp:TextBox ID="txtCompanyAccountSearchInput" runat="server" Width="200px"></asp:TextBox>
                    <asp:Button ID="btnSearchCompanyAccountIsExist" runat="server" Text="Search" Width="100px" OnClick="btnSearchCompanyAccountIsExist_Click" />
                </td>
                <td>&nbsp;</td>

                <td style="vertical-align: top; padding-top: 10px;">
                    <asp:Label ID="Label6" runat="server" Text="Enter ID "></asp:Label></td>
                <td style="vertical-align: top; padding-top: 10px;">:</td>
                <td style="vertical-align: top; padding-top: 10px;">
                    <asp:TextBox ID="txtCompAccId" runat="server"></asp:TextBox>&nbsp;
                    <asp:Button ID="btnRemoveCompanyAccountNo" runat="server" Text="Remove" OnClientClick="return ConfirmRemove();" OnClick="btnRemoveCompanyAccountNo_Click"  /><br />
                    <asp:Label ID="lblRemoveCompanyAccountStatus" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td><asp:Label ID="lblCompanyAccountNoSearchResult" runat="server" Text="" ForeColor="Red"></asp:Label></td>
            </tr>            
        </table>
       <br />

</asp:Content>
