<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="BaixarDactesMinutaAvon.aspx.cs" Inherits="EmissaoCTe.WebApp.BaixarDactesMinutaAvon" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <asp:TextBox ID="txtMinuta" runat="server" Text="Minuta Avon" ></asp:TextBox>
    <asp:Button ID="btnBaixarDactes" runat="server" Text="Baixar Dactes" OnClick="btnBaixarDactes_Click" />
</asp:Content>
