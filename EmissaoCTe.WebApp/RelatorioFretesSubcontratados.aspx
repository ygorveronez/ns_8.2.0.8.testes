<%@ Page Title="Relatório Fretes Subcontratados" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioFretesSubcontratados.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioFretesSubcontratados" %>

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
            $("#txtDataEntregaInicio").mask("99/99/9999");
            $("#txtDataEntregaFim").mask("99/99/9999");

            $("#txtDataEntregaInicio").datepicker();
            $("#txtDataEntregaFim").datepicker();

            $("#txtDataEntradaInicio").mask("99/99/9999");
            $("#txtDataEntradaFim").mask("99/99/9999");

            $("#txtDataEntradaInicio").datepicker();
            $("#txtDataEntradaFim").datepicker();
           
            $("#txtParceiro").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("parceiro", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnGerarRelatorio").click(function () {
                DownloadRelatorio();
            });

            CarregarConsultadeClientes("btnBuscarParceiro", "btnBuscarParceiro", RetornoConsultaParceiro, true, false, "");
        });

        function DownloadRelatorio() {
            var dados = {
                Parceiro: $("body").data("parceiro"),
                DataEntradaInicio: $("#txtDataEntradaInicio").val(),
                DataEntradaFim: $("#txtDataEntradaFim").val(),
                DataEntregaInicio: $("#txtDataEntregaInicio").val(),
                DataEntregaFim: $("#txtDataEntregaFim").val(),
                Tipo: $("#selTipo").val(),
                Status: $("#selStatus").val(),
                TipoArquivo: $("#selTipoArquivo").val(),
                TipoRelatorio: $("#selTipoRelatorio").val()
            };

            executarDownload("/RelatorioFretesSubcontratados/DownloadRelatorio", dados);
        }

        function RetornoConsultaParceiro(parceiro) {
            $("body").data("parceiro", parceiro.CPFCNPJ);
            $("#txtParceiro").val(parceiro.CPFCNPJ + " - " + parceiro.Nome);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório Fretes Subcontradados
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Entrega Inicial:
                </span>
                <input type="text" id="txtDataEntregaInicio" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Entrega Final:
                </span>
                <input type="text" id="txtDataEntregaFim" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Entrada Inicial:
                </span>
                <input type="text" id="txtDataEntradaInicio" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Entrada Final:
                </span>
                <input type="text" id="txtDataEntradaFim" class="form-control maskedInput" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Parceiro:
                </span>
                <input type="text" id="txtParceiro" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarParceiro" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo:
                </span>
                <select id="selTipo" class="form-control">
                    <option value="">Todos</option>
                    <option value="0">Entrega</option>
                    <option value="1">Reentrega</option>
                    <option value="2">Coleta</option>
                    <option value="3">Devolução</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="1">Fechado</option>
                    <option value="0">Aberto</option>
                    <option value="">Todos</option>
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
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo Relatório:
                </span>
                <select id="selTipoRelatorio" class="form-control">
                    <option value="RelatorioFretesSubcontratados">Comissão</option>
                    <option value="RelatorioFretesSubcontratadosEntrega">Entrega</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarRelatorio" class="btn btn-primary">Gerar Relatório</button>
</asp:Content>
