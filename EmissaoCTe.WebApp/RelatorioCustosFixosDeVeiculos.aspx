<%@ Page Title="Relatório de Custos Fixos de Veículos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioCustosFixosDeVeiculos.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioCustosFixosDeVeiculos" %>

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
            $("#txtDataEmissaoInicial").datepicker();
            $("#txtDataEmissaoFinal").datepicker();

            $("#txtDataEmissaoInicial").mask("99/99/9999");
            $("#txtDataEmissaoFinal").mask("99/99/9999");

            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
            CarregarConsultaDeTiposDeCustosFixos("btnBuscarTipoCustoFixo", "btnBuscarTipoCustoFixo", "A", RetornoConsultaTipoDeCustoFixo, true, false);

            $("#txtVeiculo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("veiculo", 0);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtTipoCustoFixo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("tipoCustoFixo", 0);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnGerarRelatorioJS").click(function () {
                DownloadRelatorio();
            });
        });

        function DownloadRelatorio() {
            var dados = {
                Veiculo: $("body").data("veiculo"),
                TipoCustoFixo: $("body").data("tipoCustoFixo"),
                DataInicial: $("#txtDataEmissaoInicial").val(),
                DataFinal: $("#txtDataEmissaoFinal").val(),
                TipoArquivo: $("#selTipoArquivo").val()
            };

            executarDownload("/RelatorioCustosFixosDeVeiculos/DownloadRelatorio", dados);
        }

        function RetornoConsultaVeiculos(veiculo) {
            $("#txtVeiculo").val(veiculo.Placa);
            $("body").data("veiculo", veiculo.Codigo);
        }

        function RetornoConsultaTipoDeCustoFixo(tipo) {
            $("#txtTipoCustoFixo").val(tipo.Descricao);
            $("body").data("tipoCustoFixo", tipo.Codigo);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Custos Fixos de Veículos
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Veículo*:
                </span>
                <input type="text" id="txtVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Tipo Custo*:
                </span>
                <input type="text" id="txtTipoCustoFixo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTipoCustoFixo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
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
    <button type="button" id="btnGerarRelatorioJS" class="btn btn-primary">Gerar Relatório</button>
</asp:Content>
