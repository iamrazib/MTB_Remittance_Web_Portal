<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="PullAPISearchTxn.aspx.cs" Inherits="RemittanceOperation.PullAPISearchTxn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css">
        .WaitForSearching {
            width: 37%;
            height: 40px;
            background-color: #cccccc;
            display: none;
            z-index: 999;
            position: absolute;
            left: 28.3%;
            top: 60%;
            font-size: large;
        }
    </style>

    <script type="text/javascript">
        function TxnSearch() {
            $('.WaitForSearching').show();
        }
    </script>

    <table style="width: 100%; border: solid 1px; margin-left: 15px; margin-top: 20px; padding-left: 20px; padding-bottom: 10px;">
        <tr>
            <td colspan="3" style="font-weight: bold; text-decoration: underline; text-align: center;">
                <asp:Label ID="Label2" runat="server" Text="Individual Transaction Search"></asp:Label>
            </td>
        </tr>
        <tr>            
            <td>
                <div>
                    <div style="float:left; width:130px;">Select Exchange : </div>
                    <div style="float:left; width:200px;"><asp:DropDownList ID="comboBoxAPIExh" runat="server"></asp:DropDownList></div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div>
                    <div style="float:left; width:130px;">PIN/Reference : </div>
                    <div style="float:left; width:200px;"><asp:TextBox ID="textBoxRefNo" runat="server" Width="180px"></asp:TextBox></div>
                </div>
            </td>
        </tr>
        <tr>            
            <td>
                <div>
                    <div style="float:left; width:130px;">Account No : </div>
                    <div style="float:left; width:200px;"><asp:TextBox ID="textBoxAccountNo" runat="server" Width="180px"></asp:TextBox></div>
                </div>                
            </td>

        </tr>
        <tr>
            <td>
                <div>
                    <div style="float:left; width:130px;">BDT Amount :</div>
                    <div style="float:left; width:200px;"><asp:TextBox ID="textBoxBdtAmt" runat="server" Width="180px"></asp:TextBox></div>
                </div>
            </td>

        </tr>
        <tr>
            <td>
                <div>
                    <div style="float:left; width:130px;">&nbsp;&nbsp;</div>
                    <div style="float:left; width:200px;"><asp:Button ID="btnAPIDataSearch" runat="server" Text="Search" Width="120px" OnClientClick="return TxnSearch();" OnClick="btnAPIDataSearch_Click"  /></div>
                &nbsp;&nbsp;
                <div class="WaitForSearching">Please Wait...!</div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">             
                &nbsp;&nbsp;<asp:Label ID="lblErrorMsg" runat="server" Text="" ForeColor="Red"></asp:Label>             
            </td>
            <td style="text-align:right; padding-right:15px;">
                <asp:Label ID="lblRecordCount" runat="server" Font-Size="Small" Text=""></asp:Label></td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Panel ID="Panel1" runat="server" ScrollBars="Both" Width="980px" Height="300px">
                    <asp:GridView ID="dataGridViewTxnSearch" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" ShowHeaderWhenEmpty="True" Font-Size="Small">
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
    <br />
    <br />

</asp:Content>
