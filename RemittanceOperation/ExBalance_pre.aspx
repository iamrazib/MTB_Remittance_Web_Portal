<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ExBalance_pre.aspx.cs" Inherits="RemittanceOperation.ExBalance_pre" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style type="text/css">
        .nav-tabs a, .nav-tabs a:hover, .nav-tabs a:focus {
            outline: 0;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery191.min.js"></script>
    <link href="Styles/bootstrap.min.css" rel="stylesheet" />

    <%--<link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.2/css/bootstrap.min.css" />
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script type="text/javascript" src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.2/js/bootstrap.min.js"></script>--%>

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <div class="panel panel-default" style="width: 840px; padding: 10px; margin: 10px">
                <div id="Tabs" role="tabpanel">
                    <!-- Nav tabs -->
                    <ul class="nav nav-tabs" role="tablist">

                        <li class="active"><a href="#apibased" aria-controls="personal" role="tab" data-toggle="tab">API Based Exhouse</a></li>
                        <li><a href="#filebased" aria-controls="employment" role="tab" data-toggle="tab">File Based Exhouse</a></li>
                    </ul>
                    <!-- Tab panes -->
                    <div class="tab-content" style="padding-top: 20px">
                        <div role="tabpanel" class="tab-pane active" id="apibased">

                            <asp:Button ID="btnRefreshAPIExchBalance" runat="server" Text="Refresh (API Exch Balance)" OnClick="btnRefreshAPIExchBalance_Click" />
                            <br />
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

                        </div>
                        <div role="tabpanel" class="tab-pane" id="filebased">
                            <asp:Button ID="btnRefreshFileExchBalance" runat="server" Text="Refresh (File Exch Balance)" OnClick="btnRefreshFileExchBalance_Click" />
                            <br />
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

                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>


    <%--<div class="panel panel-default" style="width: 700px; padding: 10px; margin: 10px">
        <div id="Tabs" role="tabpanel">
            <!-- Nav tabs -->
            <ul class="nav nav-tabs" role="tablist">
                
                <li class="active"><a href="#apibased" aria-controls="personal" role="tab" data-toggle="tab">API Based Exhouse</a></li>
                <li><a href="#filebased" aria-controls="employment" role="tab" data-toggle="tab">File Based Exhouse</a></li>
            </ul>
            <!-- Tab panes -->
            <div class="tab-content" style="padding-top: 20px">
                <div role="tabpanel" class="tab-pane active" id="apibased">
                    
                    <asp:Button ID="btnRefreshAPIExchBalance" runat="server" Text="Refresh (API Exch Balance)" OnClick="btnRefreshAPIExchBalance_Click" />
                    <br /><br />
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
                    
                </div>
                <div role="tabpanel" class="tab-pane" id="filebased">
                    <asp:Button ID="btnRefreshFileExchBalance" runat="server" Text="Refresh (File Exch Balance)" OnClick="btnRefreshFileExchBalance_Click" />
                    <br /><br />
                    
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

                </div>
            </div>
        </div>
    </div>--%>
</asp:Content>
