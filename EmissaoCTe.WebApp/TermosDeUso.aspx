<%@ Page Title="Termos de uso" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="TermosDeUso.aspx.cs" Inherits="EmissaoCTe.WebApp.TermosDeUso" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%: System.Web.Optimization.Scripts.Render("~/bundle/scripts/json",
                                               "~/bundle/scripts/blockui",
                                               "~/bundle/scripts/maskedinput",
                                               "~/bundle/scripts/ajax",
                                               "~/bundle/scripts/mensagens",
                                               "~/bundle/scripts/validaCampos") %>

    <script type="text/javascript">
        $(document).ready(function () {
            $("#btnConcordarComTermos").click(ConcordarComTermos);

            $("#btnVisualizarTermo").click(function () {
                ExibirTermo();
            });

            $("#TermoUso").hide();
            OcultarTermo();

            BuscarContrato();
        });

        function BuscarContrato() {
            executarRest("/EmpresaContrato/ObterContratoEmpresa?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    $("#txtContrato").val(r.Objeto.Contrato);
                    if (r.Objeto.PermiteAceitar) {
                        $("body").addClass("aceite-pendente");
                    }
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function ConcordarComTermos() {
            executarRest("/EmpresaContrato/ConcordarTermosDeUso?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    window.location.href = "Default.aspx";
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function ExibirTermo() {
            $("#TermoUso").show("slow");
        }

        function OcultarTermo() {
            var exibir = GetUrlParam("x");
            if (isNaN(exibir) && exibir == "true")
                $("#TermoUso").show();                   
        }

        function GetUrlParam(name) {
            var url = window.location.search.replace("?", "");
            var itens = url.split("&");
            for (n in itens) {
                if (itens[n].match(name)) {
                    return itens[n].replace(name + "=", "");
                }
            }
            return null;
        }
    </script>
    <style>
        .aceite-block {
            display: none;
        }

        body.aceite-pendente .aceite-block {
            display: block;
        }

        .btn-aceite {
            margin-top: 30px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Termos de uso <small class="aceite-block" style="float: right">Confirmar para utilizar o sistema</small>
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="text-left btn-aceite aceite-block">
                <button type="button" id="btnVisualizarTermo" class="btn btn-primary">Visualizar Termos de Uso</button>
                <button class="btn btn-success" id="btnConcordarComTermos">Li e Concordo com os Termos de Uso</button>
            </div>
        </div>
        <div id="TermoUso" class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr>Contrato</abbr>:
                </span>
                <textarea id="txtContrato" class="form-control taggedInput" rows="50" readonly="readonly"></textarea>
            </div>
        </div>
    </div>
</asp:Content>
