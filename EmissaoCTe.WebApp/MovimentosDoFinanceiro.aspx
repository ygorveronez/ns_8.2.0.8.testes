<%@ Page Title="Cadastro de Movimentos do Financeiro" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="MovimentosDoFinanceiro.aspx.cs" Inherits="EmissaoCTe.WebApp.MovimentosDoFinanceiro" %>

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
                           "~/bundle/scripts/datepicker",
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {

            $("#txtData").mask("99/99/9999");
            $("#txtData").datepicker();

            $("#txtDataPagamento").mask("99/99/9999");
            $("#txtDataPagamento").datepicker();

            $("#txtDataBaixa").mask("99/99/9999");
            $("#txtDataBaixa").datepicker();

            $("#txtValor").priceFormat({ prefix: '' });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#txtPlanoDeConta").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoPlanoConta").val("0");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtVeiculo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoVeiculo").val("0");
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

            CarregarConsultaDePlanosDeContas("btnBuscarPlanoDeConta", "btnBuscarPlanoDeConta", "A", "A", RetornoConsultaPlanoConta, true, false);
            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculo, true, false);
            CarregarConsultaDeMovimentosDoFinanceiro("default-search", "default-search", RetornoConsultaMovimento, true, false);
            CarregarConsultadeClientes("btnBuscarPessoa", "btnBuscarPessoa", RetornoConsultaPessoa, true, false);
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, true, false);

            LimparCampos();

        });

        function RetornoConsultaMotorista(motorista) {
            $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
            $("#hddCodigoMotorista").val(motorista.Codigo);
        }

        function RetornoConsultaPessoa(pessoa) {
            $("body").data("pessoa", pessoa.CPFCNPJ);
            $("#txtPessoa").val(pessoa.CPFCNPJ + " - " + pessoa.Nome);
        }

        function RetornoConsultaPlanoConta(plano) {
            $("#txtPlanoDeConta").val(plano.Conta + " - " + plano.Descricao);
            $("#hddCodigoPlanoConta").val(plano.Codigo);
        }

        function RetornoConsultaVeiculo(veiculo) {
            $("#txtVeiculo").val(veiculo.Placa);
            $("#hddCodigoVeiculo").val(veiculo.Codigo);
        }

        function RetornoConsultaMovimento(movimento) {
            executarRest("/MovimentoDoFinanceiro/ObterDetalhes?callback=?", { Codigo: movimento.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#hddCodigo").val(r.Objeto.Codigo);
                    $("#txtData").val(r.Objeto.Data);
                    $("#txtDataPagamento").val(r.Objeto.DataPagamento);
                    $("#txtDataBaixa").val(r.Objeto.DataBaixa);
                    $("#txtValor").val(r.Objeto.Valor);
                    $("#txtDocumento").val(r.Objeto.Documento);
                    $("#txtPlanoDeConta").val(r.Objeto.DescricaoPlanoDeConta);
                    $("#hddCodigoPlanoConta").val(r.Objeto.CodigoPlanoDeConta);
                    $("#txtObservacao").val(r.Objeto.Observacao);
                    $("#txtVeiculo").val(r.Objeto.PlacaVeiculo);
                    $("#hddCodigoVeiculo").val(r.Objeto.CodigoVeiculo);
                    $("body").data("pessoa", r.Objeto.CPFCNPJPessoa);
                    $("#txtPessoa").val(r.Objeto.NomePessoa);
                    $("#txtPessoa").val(r.Objeto.NomePessoa);
                    $("#selTipo").val(r.Objeto.Tipo).change();
                    $("#hddCodigoMotorista").val(r.Objeto.CodigoMotorista);
                    $("#txtMotorista").val(r.Objeto.Motorista);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function LimparCampos() {
            $("#hddCodigo").val("0");
            $("#txtData").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtDataPagamento").val("");
            $("#txtDataBaixa").val("");
            $("#txtValor").val("0,00");
            $("#txtDocumento").val("");
            $("#txtPlanoDeConta").val("");
            $("#hddCodigoPlanoConta").val("0");
            $("#txtObservacao").val("");
            $("#txtVeiculo").val('');
            $("#hddCodigoVeiculo").val('0');
            $("body").data("pessoa", null);
            $("#txtPessoa").val("");
            $("#selTipo").val($("#selTipo option:first").val()).change();
            $("#txtMotorista").val('');
            $("#hddCodigoMotorista").val('0');
        }

        function ValidarCampos() {
            var data = $("#txtData").val();
            var valor = Globalize.parseFloat($("#txtValor").val());
            var planoDeConta = Globalize.parseInt($("#hddCodigoPlanoConta").val());
            var valido = true;

            if (data != "") {
                CampoSemErro("#txtData");
            } else {
                CampoComErro("#txtData");
                valido = false;
            }

            if (!isNaN(valor) && valor > 0) {
                CampoSemErro("#txtValor");
            } else {
                CampoComErro("#txtValor");
                valido = false;
            }

            if (!isNaN(planoDeConta) && planoDeConta > 0) {
                CampoSemErro("#txtPlanoDeConta");
            } else {
                CampoComErro("#txtPlanoDeConta");
                valido = false;
            }

            return valido;
        }

        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    Data: $("#txtData").val(),
                    DataPagamento: $("#txtDataPagamento").val(),
                    DataBaixa: $("#txtDataBaixa").val(),
                    Valor: $("#txtValor").val(),
                    Documento: $("#txtDocumento").val(),
                    CodigoPlanoDeConta: $("#hddCodigoPlanoConta").val(),
                    Observacao: $("#txtObservacao").val(),
                    CodigoVeiculo: $("#hddCodigoVeiculo").val(),
                    CodigoPessoa: $("body").data("pessoa"),
                    Tipo: $("#selTipo").val(),
                    CodigoMotorista: $("#hddCodigoMotorista").val()
                };
                executarRest("/MovimentoDoFinanceiro/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        LimparCampos();
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            }
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoPlanoConta" value="0" />
        <input type="hidden" id="hddCodigoVeiculo" value="0" />
        <input type="hidden" id="hddCodigoMotorista" value="0" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Movimentos do Financeiro
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo*:
                </span>
                <select id="selTipo" class="form-control">
                    <option value="0">Entrada</option>
                    <option value="1">Saída</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Lcto.*:
                </span>
                <input type="text" id="txtData" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor*:
                </span>
                <input type="text" id="txtValor" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Documento:
                </span>
                <input type="text" id="txtDocumento" class="form-control" maxlength="50" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Vcto.*:
                </span>
                <input type="text" id="txtDataPagamento" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Dt. Baixa*:
                </span>
                <input type="text" id="txtDataBaixa" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Plano de Conta*:
                </span>
                <input type="text" id="txtPlanoDeConta" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPlanoDeConta" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Pessoa:
                </span>
                <input type="text" id="txtPessoa" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarPessoa" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Veículo:
                </span>
                <input type="text" id="txtVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-8 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Motorista:
                </span>
                <input type="text" id="txtMotorista" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarMotorista" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Observação:
                </span>
                <textarea id="txtObservacao" class="form-control" rows="3"></textarea>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
