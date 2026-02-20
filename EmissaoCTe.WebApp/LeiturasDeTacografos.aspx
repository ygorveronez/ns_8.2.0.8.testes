<%@ Page Title="Cadastro de Leituras de Tacógrafos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="LeiturasDeTacografos.aspx.cs" Inherits="EmissaoCTe.WebApp.LeiturasDeTacografos" %>

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
    <script type="text/javascript">
        $(document).ready(function () {
            $("#txtDataInicial").datepicker();
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").datepicker();
            $("#txtDataFinal").mask("99/99/9999");
            CarregarConsultaDeMotoristas("btnBuscarMotorista", "btnBuscarMotorista", RetornoConsultaMotorista, true, false);
            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculo, true, false);
            CarregarConsultaDeLeiturasDeTacografos("default-search", "default-search", "", RetornoConsultaLeituraDeTacografo, true, false);
            $("#txtMotorista").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoMotorista").val("");
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

            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            LimparCampos();
        });
        function SetarDadosPadrao() {
            $("#txtDataFinal").val(Globalize.format(new Date(), "dd/MM/yyyy"));
        }
        function RetornoConsultaLeituraDeTacografo(leitura) {
            executarRest("/LeituraDeTacografo/ObterDetalhes?callback=?", { Codigo: leitura.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#hddCodigo").val(r.Objeto.Codigo);
                    $("#hddCodigoMotorista").val(r.Objeto.CodigoMotorista);
                    $("#txtMotorista").val(r.Objeto.CPFMotorista + " - " + r.Objeto.NomeMotorista);
                    $("#hddCodigoVeiculo").val(r.Objeto.CodigoVeiculo);
                    $("#txtVeiculo").val(r.Objeto.PlacaVeiculo);
                    $("#txtDataInicial").val(r.Objeto.DataInicial);
                    $("#txtDataFinal").val(r.Objeto.DataFinal);
                    $("#txtObservacao").val(r.Objeto.Observacao);
                    $("#chkExcesso").attr("checked", r.Objeto.Excesso);
                    $("#selStatus").val(r.Objeto.Status);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RetornoConsultaMotorista(motorista) {
            $("#hddCodigoMotorista").val(motorista.Codigo);
            $("#txtMotorista").val(motorista.CPFCNPJ + " - " + motorista.Nome);
        }
        function RetornoConsultaVeiculo(veiculo) {
            $("#hddCodigoVeiculo").val(veiculo.Codigo);
            $("#txtVeiculo").val(veiculo.Placa);
        }
        function LimparCampos() {
            $("#hddCodigo").val('0');
            $("#hddCodigoMotorista").val('0');
            $("#txtMotorista").val('');
            $("#hddCodigoVeiculo").val('0');
            $("#txtVeiculo").val('');
            $("#txtDataInicial").val(''),
            $("#txtDataFinal").val(''),
            $("#txtObservacao").val(''),
            $("#chkExcesso").attr("checked", false),
            $("#selStatus").val($("#selStatus option:first").val());
            SetarDadosPadrao();
        }
        function ValidarCampos() {
            var descricao = $("#txtObservacao").val().trim();
            var codigoMotorista = Globalize.parseInt($("#hddCodigoMotorista").val());
            var codigoVeiculo = Globalize.parseInt($("#hddCodigoVeiculo").val());
            var dataInicial = $("#txtDataInicial").val();
            var dataFinal = $("#txtDataFinal").val();
            var valido = true;
            if (descricao != "") {
                CampoSemErro("#txtObservacao");
            } else {
                CampoComErro("#txtObservacao");
                valido = false;
            }
            if (!isNaN(codigoMotorista) && codigoMotorista > 0) {
                CampoSemErro("#txtMotorista");
            } else {
                CampoComErro("#txtMotorista");
                valido = false;
            }
            if (!isNaN(codigoVeiculo) && codigoVeiculo > 0) {
                CampoSemErro("#txtVeiculo");
            } else {
                CampoComErro("#txtVeiculo");
                valido = false;
            }
            if (dataInicial != "") {
                CampoSemErro("#txtDataInicial");
            } else {
                CampoComErro("#txtDataInicial");
                valido = false;
            }
            if (dataFinal != "") {
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
                    CodigoMotorista: $("#hddCodigoMotorista").val(),
                    CodigoVeiculo: $("#hddCodigoVeiculo").val(),
                    DataInicial: $("#txtDataInicial").val(),
                    DataFinal: $("#txtDataFinal").val(),
                    Observacao: $("#txtObservacao").val(),
                    Excesso: $("#chkExcesso")[0].checked,
                    Status: $("#selStatus").val()
                };
                executarRest("/LeituraDeTacografo/Salvar?callback=?", dados, function (r) {
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
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddCodigoMotorista" value="" />
        <input type="hidden" id="hddCodigoVeiculo" value="0" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Leituras de Tacógrafos
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Inicial*:
                </span>
                <input type="text" id="txtDataInicial" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Final*:
                </span>
                <input type="text" id="txtDataFinal" class="form-control" />
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
                <span class="input-group-addon">Veículo*:
                </span>
                <input type="text" id="txtVeiculo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarVeiculo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="checkbox">
                <label>
                    <input type="checkbox" id="chkExcesso" value="" />
                    Excesso de Velocidade
                </label>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Observação*:
                </span>
                <textarea id="txtObservacao" class="form-control" rows="3"></textarea>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
