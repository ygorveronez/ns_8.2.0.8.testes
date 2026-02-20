<%@ Page Title="Cadatro de Abastecimentos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Abastecimentos.aspx.cs" Inherits="EmissaoCTe.WebApp.Abastecimentos" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
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
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtKMInicialAbastecimento").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
            $("#txtKMFinalAbastecimento").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
            $("#txtValorTotalAbastecimento").priceFormat({ prefix: '' });
            $("#txtLitrosAbastecimento").priceFormat({ prefix: '' });
            $("#txtValorUnitarioAbastecimento").priceFormat({ prefix: '', centsLimit: 4 });
            $("#txtDataAbastecimento").mask("99/99/9999");
            $("#txtDataAbastecimento").datepicker();
            $("#txtVeiculo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoVeiculo").val("0");
                        $("#hddDescricaoVeiculo").val("");
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#txtMotorista").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoMotorista").val("0");
                        $("#hddDescricaoMotorista").val("");
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#txtPostoAbastecimento").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoPosto").val("");
                        $("#hddDescricaoPosto").val("");
                        $("#txtDescricaoPostoNaoCadastrado").attr("disabled", false);
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#txtDescricaoPostoNaoCadastrado").blur(function () {
                if ($(this).val().trim() != "") {
                    $("#btnBuscarPostoAbastecimento").attr("disabled", true);
                } else {
                    $("#btnBuscarPostoAbastecimento").attr("disabled", false);
                }
            });
            $("#txtKMInicialAbastecimento").blur(function () {
                CalcularMedia();
            });
            $("#txtKMFinalAbastecimento").blur(function () {
                CalcularMedia();
            });
            $("#txtLitrosAbastecimento").blur(function () {
                var litros = Globalize.parseFloat($(this).val());
                var valorUnitario = Globalize.parseFloat($("#txtValorUnitarioAbastecimento").val());
                var valorTotal = litros * valorUnitario;
                $("#txtValorTotalAbastecimento").val(Globalize.format(valorTotal, "n2"));
                CalcularMedia();
            });
            $("#txtValorUnitarioAbastecimento").blur(function () {
                var litros = Globalize.parseFloat($("#txtLitrosAbastecimento").val());
                var valorUnitario = Globalize.parseFloat($(this).val());
                var valorTotal = litros * valorUnitario;
                $("#txtValorTotalAbastecimento").val(Globalize.format(valorTotal, "n2"));
            });
            $("#txtValorTotalAbastecimento").blur(function () {
                var litros = Globalize.parseFloat($("#txtLitrosAbastecimento").val());
                var valorTotal = Globalize.parseFloat($(this).val());
                var valorUnitario = 0;
                if (litros > 0 && valorTotal > 0)
                    valorUnitario = valorTotal / litros;
                $("#txtValorUnitarioAbastecimento").val(Globalize.format(valorUnitario, "n4"));
            });
            $("#btnSalvar").click(function () {
                SalvarAbastecimento();
            });
            $("#btnCancelar").click(function () {
                LimparCamposAbastecimento();
            });
            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);
            CarregarConsultadeClientes("btnBuscarPostoAbastecimento", "btnBuscarPostoAbastecimento", RetornoConsultaPosto, true, false);
            CarregarConsultaDeAbastecimentos("default-search", "default-search", RetornoConsultaAbastecimento, true, false);
            SetarDadosPadrao();
        });
        function SetarDadosPadrao() {
            $("#txtDataAbastecimento").val(Globalize.format(new Date(), "dd/MM/yyyy"));
        }
        function RetornoConsultaAbastecimento(abastecimento) {
            LimparCamposAbastecimento();
            $("#txtDataAbastecimento").val('');
            executarRest("/Abastecimento/ObterDetalhes?callback=?", { Codigo: abastecimento.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#hddCodigo").val(r.Objeto.Codigo);
                    $("#hddCodigoPosto").val(r.Objeto.CodigoPosto);
                    if (r.Objeto.CodigoPosto == "") {
                        $("#txtDescricaoPostoNaoCadastrado").val(r.Objeto.DescricaoPosto);
                        if (r.Objeto.DescricaoPosto)
                            $("#btnBuscarPostoAbastecimento").attr("disabled", true);
                    } else {
                        $("#txtDescricaoPostoNaoCadastrado").attr("disabled", true);
                        $("#txtPostoAbastecimento").val(r.Objeto.DescricaoPosto);
                        $("#hddDescricaoPosto").val(r.Objeto.DescricaoPosto);
                    }
                    $("#txtDataAbastecimento").val(r.Objeto.Data);
                    $("#txtKMInicialAbastecimento").val(Globalize.format(r.Objeto.KilometragemAnterior, "n0"));
                    $("#txtKMFinalAbastecimento").val(Globalize.format(r.Objeto.Kilometragem, "n0"));
                    $("#txtLitrosAbastecimento").val(Globalize.format(r.Objeto.Litros, "n2"));
                    $("#txtValorUnitarioAbastecimento").val(Globalize.format(r.Objeto.ValorUnitario, "n4"));
                    $("#txtValorTotalAbastecimento").val(Globalize.format((r.Objeto.ValorUnitario * Globalize.parseFloat(r.Objeto.Litros)), "n2"));
                    $("#txtMediaAbastecimento").val(Globalize.format(r.Objeto.Media, "n2"));
                    $("#hddCodigoVeiculo").val(r.Objeto.CodigoVeiculo);
                    $("#hddDescricaoVeiculo").val(r.Objeto.DescricaoVeiculo);
                    $("#txtVeiculo").val(r.Objeto.DescricaoVeiculo);
                    $("#hddCodigoMotorista").val(r.Objeto.CodigoMotorista);
                    $("#hddDescricaoMotorista").val(r.Objeto.DescricaoMotorista);
                    $("#txtMotorista").val(r.Objeto.DescricaoMotorista);
                    $("#selSituacao").val(r.Objeto.Situacao);
                    $("#selStatus").val(r.Objeto.Status);
                    $("#chkPago").prop("checked", r.Objeto.Pago);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RetornoConsultaVeiculos(veiculo) {
            $("#hddCodigoVeiculo").val(veiculo.Codigo);
            $("#txtVeiculo").val(veiculo.Placa);
            $("#hddDescricaoVeiculo").val(veiculo.Placa);
            executarRest("/Veiculo/BuscarKilometragemPorPlaca?callback=?", { Placa: veiculo.Placa }, function (r) {
                if (r.Sucesso) {
                    $("#txtKMInicialAbastecimento").val(Globalize.format(r.Objeto.KilometragemAtual, "n0"));
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
            executarRest("/Veiculo/BuscarPorPlaca?callback=?", { Placa: veiculo.Placa }, function (r) {
                if (r.Sucesso && r.Objeto.CodigoMotorista > 0) {
                    $("#hddCodigoMotorista").val(r.Objeto.CodigoMotorista);
                    $("#txtMotorista").val(r.Objeto.CPFMotorista + " - " + r.Objeto.NomeMotorista);
                    $("#hddDescricaoMotorista").val(r.Objeto.CPFMotorista + " - " + r.Objeto.NomeMotorista);
                }
            });
        }
        function RetornoConsultaMotorista(motorista) {
            $("#hddCodigoMotorista").val(motorista.Codigo);
            $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
            $("#hddDescricaoMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
        }
        function RetornoConsultaPosto(posto) {
            $("#hddCodigoPosto").val(posto.CPFCNPJ);
            $("#txtPostoAbastecimento").val(posto.CPFCNPJ + " - " + posto.Nome);
            $("#hddDescricaoPosto").val(posto.CPFCNPJ + " - " + posto.Nome);
            $("#txtDescricaoPostoNaoCadastrado").attr("disabled", true);
        }
        function CalcularMedia() {
            var kmInicial = Globalize.parseInt($("#txtKMInicialAbastecimento").val());
            var kmFinal = Globalize.parseInt($("#txtKMFinalAbastecimento").val());
            var litros = Globalize.parseFloat($("#txtLitrosAbastecimento").val());
            var media = (kmFinal - kmInicial) / litros;
            if (isNaN(media))
                media = 0;
            $("#txtMediaAbastecimento").val(Globalize.format(media, "n2"));
        }
        function LimparCamposAbastecimento() {
            $("#hddCodigo").val("0");
            $("#hddDescricaoPosto").val("");
            $("#hddCodigoPosto").val("0");
            $("#txtPostoAbastecimento").val("");
            $("#hddCodigoVeiculo").val("0");
            $("#hddDescricaoVeiculo").val("");
            $("#txtVeiculo").val("");
            $("#hddCodigoMotorista").val("");
            $("#hddDescricaoMotorista").val("");
            $("#txtMotorista").val("");
            $("#txtDataAbastecimento").val("");
            $("#txtKMInicialAbastecimento").val("0");
            $("#txtKMFinalAbastecimento").val("0");
            $("#txtLitrosAbastecimento").val("0,00");
            $("#txtValorUnitarioAbastecimento").val("0,0000");
            $("#txtValorTotalAbastecimento").val('0,00');
            $("#txtDescricaoPostoNaoCadastrado").val('');
            $("#txtDescricaoPostoNaoCadastrado").attr("disabled", false);
            $("#btnBuscarPostoAbastecimento").attr("disabled", false);
            $("#txtMediaAbastecimento").val("0,00");
            $("#selSituacao").val($("#selSituacao option:first").val());
            $("#selStatus").val($("#selStatus option:first").val());
            $("#chkPago").prop("checked", false);

            SetarDadosPadrao();
        }
        function ValidarCamposAbastecimento() {
            var codigoVeiculo = Globalize.parseInt($("#hddCodigoVeiculo").val());
            var kmInicial = Globalize.parseInt($("#txtKMInicialAbastecimento").val());
            var kmFinal = Globalize.parseInt($("#txtKMFinalAbastecimento").val());
            var valido = true;
            if (codigoVeiculo > 0) {
                CampoSemErro("#txtVeiculo");
            } else {
                CampoComErro("#txtVeiculo");
                valido = false;
            }
            if (kmFinal <= kmInicial) {
                CampoComErro("#txtKMInicialAbastecimento");
                CampoComErro("#txtKMFinalAbastecimento");
                valido = false;
            } else {
                CampoSemErro("#txtKMInicialAbastecimento");
                CampoSemErro("#txtKMFinalAbastecimento");
            }
            return valido;
        }
        function SalvarAbastecimento() {
            if (ValidarCamposAbastecimento()) {
                var abastecimento = {
                    Codigo: Globalize.parseInt($("#hddCodigo").val()),
                    DescricaoPosto: $("#txtDescricaoPostoNaoCadastrado").val(),
                    CodigoPosto: $("#hddCodigoPosto").val(),
                    CodigoVeiculo: Globalize.parseInt($("#hddCodigoVeiculo").val()),
                    CodigoMotorista: $("#hddCodigoMotorista").val(),
                    KMInicial: Globalize.parseInt($("#txtKMInicialAbastecimento").val()),
                    KMFinal: Globalize.parseInt($("#txtKMFinalAbastecimento").val()),
                    Litros: $("#txtLitrosAbastecimento").val(),
                    ValorUnitario: $("#txtValorUnitarioAbastecimento").val(),
                    Media: $("#txtMediaAbastecimento").val(),
                    Data: $("#txtDataAbastecimento").val(),
                    Situacao: $("#selSituacao").val(),
                    Status: $("#selStatus").val(),
                    Pago: $("#chkPago")[0].checked
                };
                executarRest("/Abastecimento/Salvar?callback=?", abastecimento, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCamposAbastecimento();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoVeiculo" value="0" />
        <input type="hidden" id="hddDescricaoVeiculo" value="" />
        <input type="hidden" id="hddCodigoMotorista" value="0" />
        <input type="hidden" id="hddDescricaoMotorista" value="" />
        <input type="hidden" id="hddCodigoPosto" value="" />
        <input type="hidden" id="hddDescricaoPosto" value="" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Abastecimentos
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
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
                <span class="input-group-addon">Motorista*:
                </span>
                <input type="text" id="txtMotorista" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Posto:
                </span>
                <input type="text" id="txtPostoAbastecimento" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPostoAbastecimento" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Posto Não Cadastrado">Posto Não Cad.</abbr>:
                </span>
                <input type="text" id="txtDescricaoPostoNaoCadastrado" class="form-control" maxlength="500" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data:
                </span>
                <input type="text" id="txtDataAbastecimento" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">KM Inicial:
                </span>
                <input type="text" id="txtKMInicialAbastecimento" class="form-control" value="0" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">KM Final:
                </span>
                <input type="text" id="txtKMFinalAbastecimento" class="form-control" value="0" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Litros:
                </span>
                <input type="text" id="txtLitrosAbastecimento" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor Unitário:
                </span>
                <input type="text" id="txtValorUnitarioAbastecimento" class="form-control" value="0,0000" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor Total:
                </span>
                <input type="text" id="txtValorTotalAbastecimento" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Média:
                </span>
                <input type="text" id="txtMediaAbastecimento" class="form-control" value="0,00" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Situação*:
                </span>
                <select id="selSituacao" class="form-control">
                    <option value="A">Aberto</option>
                    <option value="F">Fechado</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="checkbox">
                <label>
                    <input type="checkbox" id="chkPago" value="" />
                    Pago
                </label>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
