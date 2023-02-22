<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AMLCheck.aspx.cs" Inherits="RemittanceOperation.AMLCheck" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <table style="width: 100%;">

                <tr>
                    <td colspan="3" style="font-weight:bold; text-decoration:underline;">AML Check</td>
                    <%--<td></td>
                    <td></td>--%>
                </tr>

                <tr>
                    <td style="width: 130px; text-align: right;">Name</td>
                    <td>:</td>
                    <td>
                        <asp:TextBox ID="txtName" runat="server" Width="250px"></asp:TextBox></td>
                </tr>
                <tr>
                    <td style="width: 130px; text-align: right;"></td>
                    <td></td>
                    <td><asp:Button ID="btnAMLScore" runat="server" Text="Check AML Score" OnClick="btnAMLScore_Click" /></td>
                </tr>
                <tr>
                    <td style="width: 130px; text-align: right;">Score</td>
                    <td>:</td>
                    <td><asp:Label ID="lblAMLScoreVal" runat="server" Text=""></asp:Label></td>
                </tr>
            </table>

            <%--<br />
            <asp:Label ID="Label1" runat="server" Text="Name"></asp:Label>&nbsp; : &nbsp;
            <br />
            <asp:Label ID="Label3" runat="server" Text=""></asp:Label>&nbsp;
            <br />
            <br />
            <asp:Label ID="Label2" runat="server" Text="Score"></asp:Label>&nbsp; : &nbsp;--%>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
