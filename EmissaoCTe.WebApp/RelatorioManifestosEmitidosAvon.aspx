<%@ Page Title="Relatório de Manifestos Emitidos Avon" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioManifestosEmitidosAvon.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioManifestosEmitidosAvon" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker") %>
        <%: Scripts.Render("~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/fileDownload") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);

            $("#txtDataEmissaoInicial").mask("99/99/9999");
            $("#txtDataEmissaoFinal").mask("99/99/9999");

            $("#txtDataEmissaoInicial").datepicker();
            $("#txtDataEmissaoFinal").datepicker();

            $("#txtNumeroManifesto").mask("9?9999999999999");

            $("#btnGerarRelatorioJS").click(function () {
                DownloadRelatorio();
            });

            var date = new Date();
            $("#txtDataEmissaoInicial").val(Globalize.format(new Date(date.getFullYear(), date.getMonth(), 1), "dd/MM/yyyy"));
            $("#txtDataEmissaoFinal").val(Globalize.format(date, "dd/MM/yyyy"));

            $("#txtVeiculo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoVeiculo", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtMotorista").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoMotorista", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });
        });

        function RetornoConsultaVeiculos(veiculo) {
            $("body").data("codigoVeiculo", veiculo.Codigo);
            $("#txtVeiculo").val(veiculo.Placa);
        }

        function RetornoConsultaMotorista(motorista) {
            $("body").data("codigoMotorista", motorista.Codigo);
            $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
        }
    </script>
    <script defer="defer" type="text/javascript" id="ScriptDownloadDocumentos">
        function DownloadRelatorio() {
            var dados = {
                DataEmissaoInicial: $("#txtDataEmissaoInicial").val(),
                DataEmissaoFinal: $("#txtDataEmissaoFinal").val(),
                NumeroManifesto: $("#txtNumeroManifesto").val(),
                CodigoMotorista: $("body").data("codigoMotorista"),
                CodigoVeiculo: $("body").data("codigoVeiculo"),
                Status: $("#selStatus").val(),
                TipoArquivo: $("#selTipoArquivo").val()
            };

            executarDownload("/RelatorioManifestosEmitidosAvon/DownloadRelatorio", dados);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Manifestos Emitidos Avon
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataEmissaoInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataEmissaoFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº Manifesto:
                </span>
                <input type="text" id="txtNumeroManifesto" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="">Todos</option>
                    <option value="0">Enviado</option>
                    <option value="1">Emitido</option>
                    <option value="2">Finalizado</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Arquivo:
                </span>
                <select id="selTipoArquivo" class="form-control">
                    <option value="PDF">PDF</option>
                    <option value="Excel">Excel</option>
                    <option value="Image">Imagem</option>
                </select>
            </div>
        </div>
    </div>
    <div class="panel-group" id="filtrosAdicionais" style="margin-bottom: 10px;">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title">
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#filtrosAdicionais" href="#filtros">Filtros Adicionais
                    </a>
                </h4>
            </div>
            <div id="filtros" class="panel-collapse collapse ">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Veículo:
                                </span>
                                <input type="text" id="txtVeiculo" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon">Motorista:
                                </span>
                                <input type="text" id="txtMotorista" class="form-control" />
                                <span class="input-group-btn">
                                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarRelatorioJS" class="btn btn-primary"><span class="glyphicon glyphicon-print"></span>&nbsp;Gerar Relatório</button>
</asp:Content>
