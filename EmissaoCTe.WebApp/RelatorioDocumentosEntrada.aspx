<%@ Page Title="Relatório de Documentos de Entrada" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioDocumentosEntrada.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioDocumentosEntrada" %>

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

    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtNumeroInicial").mask("9?999999999999999");
            $("#txtNumeroFinal").mask("9?999999999999999");

            $("#txtDataInicial, #txtDataEntradaInicial").mask("99/99/9999");
            $("#txtDataFinal, #txtDataEntradaFinal").mask("99/99/9999");

            $("#txtDataFinal, #txtDataEntradaFinal").datepicker();
            $("#txtDataInicial, #txtDataEntradaInicial").datepicker();

            RemoveConsulta($("#txtVeiculo"), function ($this) {
                $this.val("");
                $("body").data("veiculo", null);
            });

            RemoveConsulta($("#txtFornecedor"), function ($this) {
                $this.val("");
                $("body").data("fornecedor", null);
            });

            $("#btnGerarRelatorio").click(function () {
                DownloadRelatorio();
            });

            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
            CarregarConsultadeClientes("btnBuscarFornecedor", "btnBuscarFornecedor", RetornoConsultaFornecedor, true, false, "J");
        });

        function DownloadRelatorio() {
            var dados = {
                Veiculo: $("body").data("veiculo"),
                Fornecedor: $("body").data("fornecedor"),
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                DataEntradaInicial: $("#txtDataEntradaInicial").val(),
                DataEntradaFinal: $("#txtDataEntradaFinal").val(),
                NumeroInicial: $("#txtNumeroInicial").val(),
                NumeroFinal: $("#txtNumeroFinal").val(),
                Status: $("#selStatus").val(),
                TipoArquivo: $("#selTipoArquivo").val(),
                Relatorio: $("#ddlTipoRelatorio").val(),
                Ordenacao: $("#selOrdenacao").val()
            };

            executarDownload("/RelatorioDocumentosEntrada/DownloadRelatorio", dados);
        }

        function RetornoConsultaVeiculos(veiculo) {
            $("body").data("veiculo", veiculo.Codigo);
            $("#txtVeiculo").val(veiculo.Placa);
        }

        function RetornoConsultaFornecedor(forn) {
            $("body").data("fornecedor", forn.CPFCNPJ);
            $("#txtFornecedor").val(forn.CPFCNPJ + " - " + forn.Nome);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Documentos de Entrada
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº Inicial:
                </span>
                <input type="text" id="txtNumeroInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Nº Final:
                </span>
                <input type="text" id="txtNumeroFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt Entrada Inicial:
                </span>
                <input type="text" id="txtDataEntradaInicial" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt Entrada Final:
                </span>
                <input type="text" id="txtDataEntradaFinal" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Fornecedor:
                </span>
                <input type="text" id="txtFornecedor" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarFornecedor" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
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
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="">Todos</option>
                    <option value="0">Aberto</option>
                    <option value="1">Finalizado</option>
                    <option value="2">Cancelado</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Ordenação:
                </span>
                <select id="selOrdenacao" class="form-control">
                    <option value="DataEntrada">Data Entrada</option>
                    <option value="DataEmissao">Data Emissão</option>
                    <option value="Numero">Número</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Tipo Relatório:
                </span>
                <select id="ddlTipoRelatorio" class="form-control">
                    <option value="RelatorioDocumentosEntradaSumarizado">Sumarizado</option>
                    <option value="RelatorioDocumentosEntradaSumarizadoValores">Sumarizado - Valores</option>
                    <option value="RelatorioDocumentosEntradaSumarizadoCFOP">Sumarizado - CFOP</option>
                    <option value="RelatorioDocumentosEntrada">Completo</option>
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
    <button type="button" id="btnGerarRelatorio" class="btn btn-primary">Gerar Relatório</button>
</asp:Content>
