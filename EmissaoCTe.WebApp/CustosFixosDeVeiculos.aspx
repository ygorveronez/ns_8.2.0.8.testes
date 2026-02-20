<%@ Page Title="Cadastro de Custos Fixos de Veículos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="CustosFixosDeVeiculos.aspx.cs" Inherits="EmissaoCTe.WebApp.CustosFixosDeVeiculos" %>

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
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtValorTotal").priceFormat({ prefix: '' });
            $("#txtRateio").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
            $("#txtDataInicial").datepicker().on('changeDate', function (e) {
                CalcularDataFinal();
            });
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataInicial, #txtRateio").blur(function () {
                CalcularDataFinal();
            });
            $("#txtVeiculo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoVeiculo").val("0");
                        $("#txtMotorista").attr("disabled", false);
                        $("#txtVeiculo").attr("disabled", false);
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
                        $("#txtMotorista").attr("disabled", false);
                        $("#txtVeiculo").attr("disabled", false);
                    } else {
                        e.preventDefault();
                    }
                }
            });            
            $("#txtTipoCustoFixo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoTipoCustoFixo").val("0");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            CarregarConsultaDeCustosFixosDeVeiculos("default-search", "default-search", "", RetornoConsultaCustoFixo, true, false);
            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculos, true, false);
            CarregarConsultaDeTiposDeCustosFixos("btnBuscarTipoCustoFixo", "btnBuscarTipoCustoFixo", "A", RetornoConsultaTipoDeCustoFixo, true, false);
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", "A", RetornoConsultaMotorista, true, false);


            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            LimparCampos();
        });
        function RetornoConsultaVeiculos(veiculo) {
            $("#txtVeiculo").val(veiculo.Placa);
            $("#hddCodigoVeiculo").val(veiculo.Codigo);

            $("#txtMotorista").attr("disabled", true);
            $("#txtMotorista").val("");
        }
        function RetornoConsultaMotorista(motorista) {
            $("#txtMotorista").val(motorista.Nome);
            $("#hddCodigoMotorista").val(motorista.Codigo);

            $("#txtVeiculo").attr("disabled", true);
            $("#txtVeiculo").val("");
        }
        function RetornoConsultaTipoDeCustoFixo(tipo) {
            $("#txtTipoCustoFixo").val(tipo.Descricao);
            $("#hddCodigoTipoCustoFixo").val(tipo.Codigo);
        }
        function RetornoConsultaCustoFixo(custo) {
            executarRest("/CustoFixo/ObterDetalhes?callback=?", { Codigo: custo.Codigo }, function (r) {
                if (r.Sucesso) {
                    custo = r.Objeto;
                    $("#txtDescricao").val(custo.Descricao);
                    $("#selStatus").val(custo.Status);
                    $("#hddCodigo").val(custo.Codigo);
                    $("#hddCodigoTipoCustoFixo").val(custo.CodigoTipoCustoFixo);
                    $("#hddCodigoVeiculo").val(custo.CodigoVeiculo);
                    $("#txtVeiculo").val(custo.PlacaVeiculo);
                    $("#txtTipoCustoFixo").val(custo.DescricaoTipoCustoFixo);
                    $("#txtValorTotal").val(custo.ValorTotal);
                    $("#txtRateio").val(custo.Rateio);
                    $("#txtDataInicial").val(custo.DataInicial);
                    $("#txtDataFinal").val(custo.DataFinal);
                    $("#hddCodigoMotorista").val(custo.CodigoMotorista);
                    $("#txtMotorista").val(custo.Motorista);
                    
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
        function LimparCampos() {
            $("#txtDescricao").val('');
            $("#selStatus").val($("#selStatus option:first").val());
            $("#hddCodigo").val('0');
            $("#hddCodigoTipoCustoFixo").val('0');
            $("#hddCodigoVeiculo").val('0');
            $("#txtVeiculo").val('');
            $("#txtTipoCustoFixo").val('');
            $("#txtValorTotal").val('');
            $("#txtRateio").val('');
            $("#txtDataInicial").val('');
            $("#txtDataFinal").val('');
            $("#hddCodigoMotorista").val('0');
            $("#txtMotorista").val('');

            $("#txtVeiculo").attr("disabled", false);
            $("#txtMotorista").attr("disabled", false);
        }
        function ValidarCampos() {
            var descricao = $("#txtDescricao").val().trim();
            var codigoTipoCustoFixo = Globalize.parseInt($("#hddCodigoTipoCustoFixo").val());
            var codigoVeiculo = Globalize.parseInt($("#hddCodigoVeiculo").val());
            var valorTotal = Globalize.parseFloat($("#txtValorTotal").val());
            var rateio = Globalize.parseInt($("#txtRateio").val());
            var dataInicial = Globalize.parseDate($("#txtDataInicial").val());
            var dataFinal = Globalize.parseDate($("#txtDataFinal").val());
            var codigoMotorista = Globalize.parseInt($("#hddCodigoMotorista").val());

            var valido = true;

            if (descricao != "") {
                CampoSemErro("#txtDescricao");
            } else {
                CampoComErro("#txtDescricao");
                valido = false;
            }

            if (codigoTipoCustoFixo > 0) {
                CampoSemErro("#txtTipoCustoFixo");
            } else {
                CampoComErro("#txtTipoCustoFixo");
                valido = false;
            }

            if (codigoVeiculo == 0 && codigoMotorista == 0) {
                CampoComErro("#txtMotorista");
                CampoComErro("#txtVeiculo");
                valido = false;
            } else {                
                CampoSemErro("#txtMotorista");                
                CampoSemErro("#txtVeiculo");                
            }

            if (valorTotal > 0) {
                CampoSemErro("#txtValorTotal");
            } else {
                CampoComErro("#txtValorTotal");
                valido = false;
            }

            if (rateio > 0) {
                CampoSemErro("#txtRateio");
            } else {
                CampoComErro("#txtRateio");
                valido = false;
            }

            if (dataInicial != null) {
                CampoSemErro("#txtDataInicial");
            } else {
                CampoComErro("#txtDataInicial");
                valido = false;
            }

            if (dataFinal != null) {
                CampoSemErro("#txtDataFinal");
            } else {
                CampoComErro("#txtDataFinal");
                valido = false;
            }

            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    CodigoTipoCustoFixo: $("#hddCodigoTipoCustoFixo").val(),
                    CodigoVeiculo: $("#hddCodigoVeiculo").val(),
                    Descricao: $("#txtDescricao").val(),
                    ValorTotal: $("#txtValorTotal").val(),
                    Rateio: $("#txtRateio").val(),
                    DataInicial: $("#txtDataInicial").val(),
                    DataFinal: $("#txtDataFinal").val(),
                    Status: $("#selStatus").val(),
                    CodigoMotorista: $("#hddCodigoMotorista").val()
                };
                executarRest("/CustoFixo/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
            }
        }
        function CalcularDataFinal() {
            var dataInicial = Globalize.parseDate($("#txtDataInicial").val());
            var rateio = Globalize.parseInt($("#txtRateio").val());
            if (dataInicial != null && rateio > 0) {
                var dataFinal = new Date(dataInicial.setMonth(dataInicial.getMonth() + rateio - 1));
                $("#txtDataFinal").val(Globalize.format(dataFinal, "dd/MM/yyyy"));
            } else {
                $("#txtDataFinal").val("");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoTipoCustoFixo" value="0" />
        <input type="hidden" id="hddCodigoVeiculo" value="0" />
        <input type="hidden" id="hddCodigoMotorista" value="0" />        
    </div>
    <div class="page-header">
        <h2>Cadastro de Custos Fixos de Veículos
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
                <span class="input-group-addon">Tipo Custo*:
                </span>
                <input type="text" id="txtTipoCustoFixo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTipoCustoFixo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Descrição*:
                </span>
                <input type="text" id="txtDescricao" class="form-control" maxlength="200" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor Total*:
                </span>
                <input type="text" id="txtValorTotal" class="form-control " />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Rateio em meses">Rateio</abbr>*:
                </span>
                <input type="text" id="txtRateio" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial*:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final*:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" disabled="disabled" />
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
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
