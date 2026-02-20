<%@ Page Title="Relatórios de Ocorrências de Funcionários" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioOcorrenciasDeFuncionarios.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioOcorrenciasDeFuncionarios" %>

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
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");

            $("#txtDataFinal").datepicker();
            $("#txtDataInicial").datepicker();

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

            $("#txtTipoOcorrencia").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("tipoOcorrencia", 0);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtFuncionario").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("funcionario", 0);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnGerarRelatorioJS").click(function () {
                DownloadRelatorio();
            });

            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
            CarregarConsultaDeFuncionarios("btnBuscarFuncionario", "btnBuscarFuncionario", "A", RetornoConsultaFuncionarios, true, false);
            CarregarConsultaDeTiposDeOcorrencias("btnBuscarTipoOcorrencia", "btnBuscarTipoOcorrencia", "A", RetornoConsultaTipoOcorrencia, true, false);
        });

        function DownloadRelatorio() {
            var dados = {
                Veiculo: $("body").data("veiculo"),
                TipoOcorrencia: $("body").data("tipoOcorrencia"),
                Funcionario: $("body").data("funcionario"),
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                TipoArquivo: $("#selTipoArquivo").val()
            };

            executarDownload("/RelatorioOcorrenciasFuncionarios/DownloadRelatorio", dados);
        }

        function RetornoConsultaFuncionarios(funcionario) {
            $("body").data("funcionario", funcionario.Codigo);
            $("#txtFuncionario").val(funcionario.CPFCNPJ + " - " + funcionario.Nome);
        }

        function RetornoConsultaVeiculos(veiculo) {
            $("body").data("veiculo", veiculo.Codigo);
            $("#txtVeiculo").val(veiculo.Placa);
        }

        function RetornoConsultaTipoOcorrencia(tipoOcorrencia) {
            $("body").data("tipoOcorrencia", tipoOcorrencia.Codigo);
            $("#txtTipoOcorrencia").val(tipoOcorrencia.Descricao);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Ocorrências de Funcionários
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Funcionário:
                </span>
                <input type="text" id="txtFuncionario" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarFuncionario" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Veículo:
                </span>
                <input type="text" id="txtVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Tipo Ocorrência:
                </span>
                <input type="text" id="txtTipoOcorrencia" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTipoOcorrencia" class="btn btn-primary">Buscar</button>
                </span>
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
