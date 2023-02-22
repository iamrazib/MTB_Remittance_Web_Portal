<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="FreelanceAccountNo.aspx.cs" Inherits="RemittanceOperation.FreelanceAccountNo" %>
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
                <asp:Label ID="Label1" runat="server" Text="Freelance Account Number List"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnFreelanceAccountNoSearch" runat="server" Text="Search" Width="150px" OnClick="btnFreelanceAccountNoSearch_Click"  />
                <br />
                <br />
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="400px" Height="200px">
                    <asp:GridView ID="dataGridViewFreelanceAccountNo" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small" OnRowDataBound="dataGridViewFreelanceAccountNo_RowDataBound" >
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
                <table style="width: 100%;">
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
                            <asp:TextBox ID="txtFreelanceAccountNoSearchInput" runat="server" Width="200px"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="btnSearchFreelanceAccountNoIsExist" runat="server" Text="Search" Width="100px" OnClick="btnSearchFreelanceAccountNoIsExist_Click" /></td>
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
                            <asp:Label ID="lblFreelanceAccountNoSearchResult" runat="server" ForeColor="Red" Font-Size="Small"></asp:Label></td>
                    </tr>
                </table>


            </td>
        </tr>
        <tr>
            <td colspan="2">&nbsp;</td>
        </tr>
    </table>
    <table style="margin-left:15px;">   <%--style=" border: solid 1px;"--%>
        <tr>
            <td colspan="3" style="text-decoration: underline;">Add New Account Number:</td>
            <td style="width:50px">&nbsp;</td>
            <td colspan="3" style="text-decoration: underline;">Remove Any Account Number From System</td>
        </tr>        
        <tr>
            <td style="width: 100px;">
                <asp:Label ID="Label3" runat="server" Text="Account "></asp:Label>
            </td>
            <td style="width: 10px;">:</td>
            <td style="width: 100px;">
                <asp:TextBox ID="textBoxFreelanceAccountNo" runat="server" Width="308px" TextMode="MultiLine" style="resize:none" Height="150px"></asp:TextBox>
            </td>

            <td>&nbsp;</td>

            <td style="vertical-align:top; padding-top:10px;"><asp:Label ID="Label4" runat="server" Text="Enter ID "></asp:Label></td>
            <td style="vertical-align:top; padding-top:10px;">:</td>
            <td style="vertical-align:top; padding-top:10px;">
                <asp:TextBox ID="txtAccountNoId" runat="server"></asp:TextBox>&nbsp;
                <asp:Button ID="btnRemoveFreelanceAccountNo" runat="server" Text="Remove"  OnClientClick="return ConfirmRemove();" OnClick="btnRemoveFreelanceAccountNo_Click"  /><br />
                <asp:Label ID="lblRemoveStatus" runat="server" Text="" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>
                <asp:Button ID="btnFreelanceAccountNoSave" runat="server" Text="Save" Width="100px"  OnClientClick="return ConfirmSave();" OnClick="btnFreelanceAccountNoSave_Click"  />
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>
                <asp:Label ID="lblFreelanceAccountNoSaveResult" runat="server" Text="" ForeColor="Red"></asp:Label>
            </td>
        </tr>
    </table>

</asp:Content>
