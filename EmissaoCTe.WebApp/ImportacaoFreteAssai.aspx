<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportacaoFreteAssai.aspx.cs" Inherits="EmissaoCTe.WebApp.ImportacaoFreteAssai" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2>Importação Fretes ASSAÍ
    </h2>

    <asp:FileUpload ID="fupArquivo" runat="server" />

    <asp:Button ID="btnEnviarArquivo" runat="server" Text="Enviar" OnClick="btnEnviarArquivo_Click" />
</asp:Content>
