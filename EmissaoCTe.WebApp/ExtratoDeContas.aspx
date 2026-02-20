<%@ Page Title="Extrato de Contas" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ExtratoDeContas.aspx.cs" Inherits="EmissaoCTe.WebApp.ExtratoDeContas" %>

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
            CarregarConsultadeClientes("btnBuscarPessoa", "btnBuscarPessoa", RetornoConsultaPessoa, true, false);
            CarregarConsultaDePlanosDeContas("btnBuscarPlanoConta", "btnBuscarPlanoConta", "A", "A", RetornoConsultaPlanoConta, true, false);
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, true, false);
            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculo, true, false);

            $("#txtDataLancamentoInicial").mask("99/99/9999");
            $("#txtDataLancamentoInicial").datepicker();

            $("#txtDataLancamentoFinal").mask("99/99/9999");
            $("#txtDataLancamentoFinal").datepicker();

            $("#txtDataPagamentoInicial").mask("99/99/9999");
            $("#txtDataPagamentoInicial").datepicker();

            $("#txtDataPagamentoFinal").mask("99/99/9999");
            $("#txtDataPagamentoFinal").datepicker();

            $("#txtDataBaixaInicial").mask("99/99/9999");
            $("#txtDataBaixaInicial").datepicker();

            $("#txtDataBaixaFinal").mask("99/99/9999");
            $("#txtDataBaixaFinal").datepicker();

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

            $("#txtMotorista").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("motorista", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

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

            $("#txtPlanoConta").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("planoConta", 0);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnGerarRelatorio").click(function () {
                DownloadRelatorio();
            });
        });

        function RetornoConsultaPessoa(pessoa) {
            $("body").data("pessoa", pessoa.CPFCNPJ);
            $("#txtPessoa").val(pessoa.CPFCNPJ + " - " + pessoa.Nome);
        }

        function RetornoConsultaMotorista(motorista) {
            $("body").data("motorista", motorista.Codigo);
            $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
        }

        function RetornoConsultaVeiculo(veiculo) {
            $("#txtVeiculo").val(veiculo.Placa);
            $("body").data("veiculo", veiculo.Codigo);
        }

        function RetornoConsultaPlanoConta(plano) {
            $("#txtPlanoConta").val(plano.Conta + " - " + plano.Descricao);
            $("body").data("planoConta", plano.Codigo);
        }

        function DownloadRelatorio() {
            var dados = {
                PlanoConta: $("body").data("planoConta"),
                Pessoa: $("body").data("pessoa"),
                DataLancamentoInicial: $("#txtDataLancamentoInicial").val(),
                DataLancamentoFinal: $("#txtDataLancamentoFinal").val(),
                DataPagamentoInicial: $("#txtDataPagamentoInicial").val(),
                DataPagamentoFinal: $("#txtDataPagamentoFinal").val(),
                DataBaixaInicial: $("#txtDataBaixaInicial").val(),
                DataBaixaFinal: $("#txtDataBaixaFinal").val(),
                TipoPlanoConta: $("#selTipoPlanoConta").val(),
                SituacaoMovimento: $("#selSituacaoMovimento").val(),
                TipoArquivo: $("#selTipoArquivo").val(),
                Motorista: $("body").data("motorista"),
                Veiculo: $("body").data("veiculo"),
            };

            executarDownload("/ExtratoConta/DownloadRelatorio", dados);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Extrato de Contas
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Conta:
                </span>
                <input type="text" id="txtPlanoConta" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPlanoConta" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Pessoa:
                </span>
                <input type="text" id="txtPessoa" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPessoa" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Motorista:
                </span>
                <input type="text" id="txtMotorista" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
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
                <span class="input-group-addon">
                    <abbr title="Data de Lançamento Inicial">Lcto. Ini.</abbr>:
                </span>
                <input type="text" id="txtDataLancamentoInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Lançamento Final">Lcto. Fin.</abbr>:
                </span>
                <input type="text" id="txtDataLancamentoFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Vencimento Inicial (Movimento de CTe)">Vcto. Ini.</abbr>:
                </span>
                <input type="text" id="txtDataPagamentoInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Vencimento Final (Movimento de CTe)">Vcto. Fin.</abbr>:
                </span>
                <input type="text" id="txtDataPagamentoFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Baixa Inicial (Movimento de CTe)">Baixa Ini.</abbr>:
                </span>
                <input type="text" id="txtDataBaixaInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Data de Baixa Final (Movimento de CTe)">Baixa. Fin.</abbr>:
                </span>
                <input type="text" id="txtDataBaixaFinal" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Situação dos Movimentos">Situação</abbr>:
                </span>
                <select id="selSituacaoMovimento" class="form-control">
                    <option value="">Todos</option>
                    <option value="A">Abertos</option>
                    <option value="P">Pagos</option>
                    <option value="B">Baixados</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Tipo de Conta">Conta</abbr>:
                </span>
                <select id="selTipoPlanoConta" class="form-control">
                    <option value="">Todos</option>
                    <option value="R">Receitas</option>
                    <option value="D">Despesas</option>
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
    <button type="button" id="btnGerarRelatorio" class="btn btn-primary">Gerar Extrato</button>
</asp:Content>
