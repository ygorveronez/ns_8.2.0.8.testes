<%@ Page Title="Relatório de Notas Fiscais de Serviço" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioNFSeEmitidas.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioDocumentosEntrada" %>

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

            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");

            $("#txtDataFinal").datepicker();
            $("#txtDataInicial").datepicker();

            $("#txtTomador").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("tomador", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnGerarRelatorio").click(function () {
                DownloadRelatorio();
            });

            $("#btnBaixarLoteXML").click(function () {
                DownloadLoteXML();
            });

            $("#btnBaixarLotePDF").click(function () {
                DownloadLotePDF();
            });

            var date = new Date();
            $("#txtDataInicial").val(Globalize.format(date, "dd/MM/yyyy"));
            $("#txtDataFinal").val(Globalize.format(date, "dd/MM/yyyy"));

            CarregarConsultadeClientes("btnBuscarTomador", "btnBuscarTomador", RetornoConsultaTomador, true, false, "");
        });

        function DownloadRelatorio() {
            var dados = {
                Tomador: $("body").data("tomador"),
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                NumeroInicial: $("#txtNumeroInicial").val(),
                NumeroFinal: $("#txtNumeroFinal").val(),
                Status: $("#selStatus").val(),
                TipoArquivo: $("#selTipoArquivo").val()
            };

            executarDownload("/RelatorioNFSeEmitidas/DownloadRelatorio", dados);
        }

        function DownloadLoteXML() {
            var dados = {
                Tomador: $("body").data("tomador"),
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                NumeroInicial: $("#txtNumeroInicial").val(),
                NumeroFinal: $("#txtNumeroFinal").val(),
                Status: $("#selStatus").val(),
                TipoArquivo: $("#selTipoArquivo").val()
            };

            executarDownload("/RelatorioNFSeEmitidas/DownloadLoteXML", dados);
        }

        function DownloadLotePDF() {
            var dados = {
                Tomador: $("body").data("tomador"),
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                NumeroInicial: $("#txtNumeroInicial").val(),
                NumeroFinal: $("#txtNumeroFinal").val(),
                Status: $("#selStatus").val(),
                TipoArquivo: $("#selTipoArquivo").val()
            };

            executarDownload("/RelatorioNFSeEmitidas/DownloadLotePDF", dados);
        }


        function RetornoConsultaTomador(cli) {
            $("body").data("tomador", cli.CPFCNPJ);
            $("#txtTomador").val(cli.CPFCNPJ + " - " + cli.Nome);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Notas Fiscais de Serviço
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
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Tomador:
                </span>
                <input type="text" id="txtTomador" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTomador" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="">Todos</option>
                    <option value="0">Digitação</option>
                    <option value="3">Autorizadas</option>
                    <option value="5">Canceladas</option>
                    <option value="9">Rejeitadas</option>
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
    <button type="button" id="btnBaixarLoteXML" class="btn btn-primary"><span class="glyphicon glyphicon-download"></span>&nbsp;Baixar Lote de XML</button>
    <button type="button" id="btnBaixarLotePDF" class="btn btn-primary"><span class="glyphicon glyphicon-download"></span>&nbsp;Baixar Lote de PDF</button>
</asp:Content>
