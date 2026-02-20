<%@ Page Title="Relatório de Pagamentos de Motoristas por MDF-e" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioPagamentoMotoristaMDFe.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioPagamentoMotoristaMDFe" %>

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
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);

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

            $("#txtDataPagamentoInicial").datepicker();
            $("#txtDataPagamentoFinal").datepicker();

            $("#txtDataPagamentoInicial").mask("99/99/9999");
            $("#txtDataPagamentoFinal").mask("99/99/9999");

            $("#btnGerarRelatorio").click(function () {
                DownloadRelatorio();
            });
        });

        function RetornoConsultaMotorista(motorista) {
            $("body").data("codigoMotorista", motorista.Codigo);
            $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
        }

        function DownloadRelatorio() {
            var dados = {
                DataInicial: $("#txtDataPagamentoInicial").val(),
                DataFinal: $("#txtDataPagamentoFinal").val(),
                CodigoMotorista: $("body").data("codigoMotorista"),
                TipoArquivo: $("#selTipoArquivo").val(),
                Status: $("#selStatus").val()
            };

            executarDownload("/RelatorioPagamentoMotoristaMDFe/DownloadRelatorio", dados);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Pagamentos de Motoristas por MDF-e
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Pgto. Inicial:
                </span>
                <input type="text" id="txtDataPagamentoInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Pgto. Final:
                </span>
                <input type="text" id="txtDataPagamentoFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Motorista*:
                </span>
                <input type="text" id="txtMotorista" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="">Todos</option>
                    <option value="P">Pendente</option>
                    <option value="A">Pago</option>
                    <option value="C">Cancelado</option>
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
