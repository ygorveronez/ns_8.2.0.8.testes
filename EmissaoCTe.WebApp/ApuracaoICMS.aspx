<%@ Page Title="Apuração do ICMS" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ApuracaoICMS.aspx.cs" Inherits="EmissaoCTe.WebApp.ApuracaoICMS" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker") %>
        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/datepicker") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#txtDataInicial").datepicker();
            $("#txtDataFinal").datepicker();

            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");

            $("#txtValorCreditosPeriodoAnterior").priceFormat();

            $("#btnAtualizarValores").click(function () {
                AtualizarValores();
            });

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#txtValorCreditosPeriodoAnterior").focusout(function (r) {
                AtualizarSaldoCredorPeriodoAnterior();
            });
        });

        function AtualizarValores() {
            var dataInicial = $("#txtDataInicial").val(), dataFinal = $("#txtDataFinal").val();
            var valido = true;

            if (dataInicial == null || dataInicial == "") {
                CampoComErro("#txtDataInicial");
                valido = false;
            } else {
                CampoSemErro("#txtDataInicial");
            }

            if (dataFinal == null || dataFinal == "") {
                CampoComErro("#txtDataFinal");
                valido = false;
            } else {
                CampoSemErro("#txtDataFinal");
            }

            if (valido) {
                executarRest("/ApuracaoICMS/ObterValoresParaApuracao?callback=?", { DataInicial: dataInicial, DataFinal: dataFinal }, function (r) {
                    if (r.Sucesso) {
                        RenderizarValores(r.Objeto);
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            }
        }

        function RenderizarValores(valores) {
            $("#txtDataInicial").val(valores.DataInicial);
            $("body").data("dataInicial", valores.DataInicial);

            $("#txtDataFinal").val(valores.DataFinal);
            $("body").data("dataFinal", valores.DataFinal);

            $("#txtValorCreditosPeriodoAnterior").val(valores.ValorSaldoCredorPeriodoAnterior);
            $("#txtValorCreditos").val(valores.ValorCreditos);
            $("#txtValorDebitos").val(valores.ValorDebitos);
            $("#txtValorICMSRecolher").val(valores.ValorICMSRecolher);
            $("#txtValorSaldoCredorTransportar").val(valores.ValorSaldoCredorTransportar);

            if (!valores.PossuiApuracaoPeriodoAnterior) {
                jConfirm("Não há apuração de ICMS para o período anterior.<br/><br/>Deseja informar manualmente o saldo credor do período anterior?<br/><br/><b style='color: red'>É altamente recomendável que se faça a apuração do período anterior, a não ser que seja primeiro mês de utilização do sistema.</b>", "Atenção!", function (r) {
                    if (r) {
                        $("#txtValorCreditosPeriodoAnterior").prop("disabled", false);
                    } else {
                        $("#txtValorCreditosPeriodoAnterior").prop("disabled", true);
                    }
                })
            } else {
                $("#txtValorCreditosPeriodoAnterior").prop("disabled", true);
            }
        }

        function AtualizarSaldoCredorPeriodoAnterior() {
            if ($("#txtValorCreditosPeriodoAnterior").prop("disabled") == false) {
                var valorSaldoCredorPeriodoAnterior = Globalize.parseFloat($("#txtValorCreditosPeriodoAnterior").val());
                var valorCreditos = Globalize.parseFloat($("#txtValorCreditos").val());
                var valorDebitos = Globalize.parseFloat($("#txtValorDebitos").val());

                var valorSaldoCredorTransportar = (valorSaldoCredorPeriodoAnterior + valorCreditos) > valorDebitos ? valorSaldoCredorPeriodoAnterior + valorCreditos - valorDebitos : 0;

                var valorICMSRecolher = valorDebitos > (valorSaldoCredorPeriodoAnterior + valorCreditos) ? valorDebitos - (valorSaldoCredorPeriodoAnterior + valorCreditos) : 0;

                $("#txtValorSaldoCredorTransportar").val(Globalize.format(valorSaldoCredorTransportar, "n2"));
                $("#txtValorICMSRecolher").val(Globalize.format(valorICMSRecolher, "n2"));
            }
        }

        function Salvar() {
            if (ValidarDados()) {
                var dados = {
                    DataInicial: $("body").data("dataInicial"),
                    DataFinal: $("body").data("dataFinal"),
                    SaldoCredorPeriodoAnterior: $("#txtValorCreditosPeriodoAnterior").val(),
                    ValorCreditos: $("#txtValorCreditos").val(),
                    ValorDebitos: $("#txtValorDebitos").val(),
                    ValorICMSRecolher: $("#txtValorICMSRecolher").val(),
                    SaldoCredorTransportar: $("#txtValorSaldoCredorTransportar").val()
                };

                executarRest("/ApuracaoICMS/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            }
        }

        function ValidarDados() {
            var dataInicial = $("body").data("dataInicial");
            var dataFinal = $("body").data("dataFinal");

            if (dataInicial == null || dataInicial == "" || dataFinal == null || dataFinal == "") {
                ExibirMensagemAlerta("Atualize os valores para salvar a apuração do ICMS!", "Atenção!");
                return false;
            } else {
                return true;
            }
        }

        function LimparCampos() {
            $("#txtDataInicial").val('');
            $("body").data("dataInicial", null);

            $("#txtDataFinal").val('');
            $("body").data("dataFinal", null);

            $("#txtValorCreditosPeriodoAnterior").val('');
            $("#txtValorCreditos").val('');
            $("#txtValorDebitos").val('');
            $("#txtValorICMSRecolher").val('');
            $("#txtValorSaldoCredorTransportar").val('');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Apuração do ICMS
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Início*:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Fim*:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
            </div>
        </div>
        <button type="button" id="btnAtualizarValores" class="btn btn-primary">Atualizar Valores</button>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor de Créditos Referente ao Período Anterior"></abbr>
                    Vl. Créd. Ant.*:
                </span>
                <input type="text" id="txtValorCreditosPeriodoAnterior" class="form-control" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Créditos*:
                </span>
                <input type="text" id="txtValorCreditos" class="form-control" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Débitos*:
                </span>
                <input type="text" id="txtValorDebitos" class="form-control" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Vl. ICMS Recolher*:
                </span>
                <input type="text" id="txtValorICMSRecolher" class="form-control" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Valor Total de Saldo Credor a Transportar para o Período Seguinte">Vl. Sld. Credor*</abbr>:
                </span>
                <input type="text" id="txtValorSaldoCredorTransportar" class="form-control" disabled />
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar Apuração</button>
</asp:Content>
