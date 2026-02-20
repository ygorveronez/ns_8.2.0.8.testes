<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ReenvioCTesRejeitados.aspx.cs" Inherits="EmissaoCTe.WebApp.ReenvioCTesRejeitados" %>
<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <script defer="defer" type="text/javascript">
            CKEDITOR_BASEPATH = ObterPath() + '/Scripts/ckeditor/';
        </script>
        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/ckeditor",
                           "~/bundle/scripts/ckeditoradapters") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        idConfiguracao = 0;
        $(document).ready(function () {
            $("#corrigirDicasEmpA").click(function () {
                Looping("A");
            });

            $("#corrigirDicasEmpI").click(function () {
                Looping("I");
            });
        });

        function Looping(status) {
            executarRest("/ConfiguracaoEmpresa/CDLoopCorrecao?callback=?", { Status: status }, function (r) {
                if (r.Sucesso)
                    ExibirMensagemSucesso(r.Objeto, "Sucesso! " + status, "mensagens");
                else
                    ExibirMensagemAlerta(r.Erro, "Erro! " + status, "mensagens");
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    
    <asp:Button ID="btnReiniciarFilas" runat="server" Text="Reiniciar Filas" OnClick="btnReiniciarFilas_Click" />
    <asp:Button ID="btnReenviarCTes" runat="server" Text="Reenviar CT-es" OnClick="btnReenviarCTes_Click" />

    <button class="btn btn-default" id="corrigirDicasEmpA" type="button">Corrigir Dicas Empresas Ativas</button>

     <asp:Button ID="btnGerarCargasCTeGPA" runat="server" Text="Gerar Carga CT-es GPA" OnClick="btnGerarCargasCTeGPA_Click" />
     <asp:Button ID="btnAjustarNumerosPagamentosCTe" runat="server" Text="Cancelar NFSe Vigor/Coopercarga" OnClick="btnAjustarNumerosPagamentosCTe_Click" />
    
    <div id="mensagens"></div>
</asp:Content>