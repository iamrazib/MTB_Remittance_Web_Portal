<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ExhouseBalance.aspx.cs" Inherits="RemittanceOperation.ExhouseBalance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style type="text/css">
        .nav-tabs a, .nav-tabs a:hover, .nav-tabs a:focus {
            outline: 0;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <%--<script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery191.min.js"></script>
    <link href="Styles/bootstrap.min.css" rel="stylesheet" />--%>

    <%--<link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.2/css/bootstrap.min.css" />
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script type="text/javascript" src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.2/js/bootstrap.min.js"></script>--%>

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
                       
            <br />
            &nbsp;<asp:Button ID="btnRefreshExchBalance" runat="server" Text="Refresh" OnClick="btnRefreshExchBalance_Click" />
            <br />
            <table style="width: 100%;">
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td style="width:250px;">Total Exchange House NRT Balance : </td>
                    <td><asp:Label ID="lblTotalBalance" runat="server" Text=""></asp:Label></td>
                </tr>
                <tr>
                    <td style="width:250px;">Total Wage Earners Balance : </td>
                    <td><asp:Label ID="lblTotalWageBalance" runat="server" Text=""></asp:Label></td>
                </tr>
                <tr>
                    <td style="width:250px;">Total Service Remittance Balance : </td>
                    <td><asp:Label ID="lblTotalServiceBalance" runat="server" Text=""></asp:Label></td>
                </tr>
                
            </table>

            <br />
            <asp:Label ID="Label2" runat="server" Text="API Based Exchange House" Font-Underline="true"></asp:Label>
            <br />
            <asp:GridView ID="dataGridViewAPIExchangeHouse" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" OnRowDataBound="dataGridViewAPIExchangeHouse_RowDataBound">
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
            
            <br />
            <br />

            <asp:Label ID="Label1" runat="server" Text="File Based Exchange House" Font-Underline="true"></asp:Label>
            <br />

                <asp:GridView ID="dGridViewFileBasedExch" runat="server" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="4" OnRowDataBound="dGridViewFileBasedExch_RowDataBound">
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

            <br />
            <br />


        </ContentTemplate>
    </asp:UpdatePanel>


   
</asp:Content>
