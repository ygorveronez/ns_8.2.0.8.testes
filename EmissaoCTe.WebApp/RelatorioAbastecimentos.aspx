<%@ Page Title="Relatório de Abastecimentos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioAbastecimentos.aspx.cs" Inherits="EmissaoCTe.WebApp.RelatorioAbastecimentos" %>

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
        var path = "";
        if (document.location.pathname.split("/").length > 1) {
            var paths = document.location.pathname.split("/");
            for (var i = 0; (paths.length - 1) > i; i++) {
                if (paths[i] != "") {
                    path += "/" + paths[i];
                }
            }
        }

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

            $("#txtModelo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("modeloVeiculo", 0);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtPessoa").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("pessoa", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnGerarRelatorioJS").click(function () {
                DownloadRelatorio();
            });

            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
            CarregarConsultaDeModelosDeVeiculos("btnBuscarModelo", "btnBuscarModelo", RetornoConsultaModelos, true, false);
            CarregarConsultadeClientes("btnBuscarPessoa", "btnBuscarPessoa", RetornoConsultaPessoa, true, false, "");
        });

        function DownloadRelatorio() {
            var dados = {
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                Veiculo: $("body").data("veiculo"),
                ModeloVeiculo: $("body").data("modeloVeiculo"),
                TipoArquivo: $("#selTipoArquivo").val(),
                Cliente: $("body").data("pessoa"),
                Pagamento: $("#selPagamento").val(),
                NomePosto: $("#txtNomePosto").val()
            };

            executarDownload("/RelatorioAbastecimentos/DownloadRelatorio", dados);
        }

        function RetornoConsultaModelos(modelo) {
            $("body").data("modeloVeiculo", modelo.Codigo);
            $("#txtModelo").val(modelo.Descricao);
        }

        function RetornoConsultaVeiculos(veiculo) {
            $("body").data("veiculo", veiculo.Codigo);
            $("#txtVeiculo").val(veiculo.Placa);
        }

        function RetornoConsultaPessoa(pessoa) {
            $("body").data("pessoa", pessoa.CPFCNPJ);
            $("#txtPessoa").val(pessoa.CPFCNPJ + " - " + pessoa.Nome);
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Relatório de Abastecimentos
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
                <span class="input-group-addon">Modelo:
                </span>
                <input type="text" id="txtModelo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarModelo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Posto:
                </span>
                <input type="text" id="txtPessoa" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPessoa" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Nome do Posto quando não cadastrado">Nome do Posto</abbr>:
                </span>
                <input type="text" id="txtNomePosto" class="form-control maskedInput" />
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
                <span class="input-group-addon">Pagamento:
                </span>
                <select id="selPagamento" class="form-control">
                    <option value="">Todos</option>
                    <option value="1">Pago</option>
                    <option value="0">A pagar</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnGerarRelatorioJS" class="btn btn-primary">Gerar Relatório</button>
</asp:Content>
