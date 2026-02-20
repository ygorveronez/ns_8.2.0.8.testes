<%@ Page Title="Cadastro de Ocorrências de Funcionários" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="OcorrenciasDeFuncionarios.aspx.cs" Inherits="EmissaoCTe.WebApp.OcorrenciasDeFuncionarios" %>

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
                           "~/bundle/scripts/datepicker") %>
    </asp:PlaceHolder>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#txtDataDaOcorrencia").datepicker();
            $("#txtDataDaOcorrencia").mask("99/99/9999");
            CarregarConsultaDeFuncionarios("btnBuscarFuncionario", "btnBuscarFuncionario", "A", RetornoConsultaFuncionario, true, false);
            CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculo, true, false);
            CarregarConsultaDeTiposDeOcorrencias("btnBuscarTipoDeOcorrencia", "btnBuscarTipoDeOcorrencia", "A", RetornoConsultaTipoDeOcorrencia, true, false);
            CarregarConsultaDeOcorrenciasDeFuncionarios("default-search", "default-search", "", RetornoConsultaOcorrenciaDeFuncionario, true, false);
            $("#txtFuncionario").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoFuncionario").val("");
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
            $("#txtTipoDeOcorrencia").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoTipoOcorrencia").val("0");
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
            $("#txtDataDaOcorrencia").val(Globalize.format(new Date(), "dd/MM/yyyy"));
        }
        function RetornoConsultaOcorrenciaDeFuncionario(ocorrencia) {
            executarRest("/OcorrenciaDeFuncionario/ObterDetalhes?callback=?", { Codigo: ocorrencia.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#hddCodigo").val(r.Objeto.Codigo);
                    $("#hddCodigoFuncionario").val(r.Objeto.CodigoFuncionario);
                    $("#txtFuncionario").val(r.Objeto.CPFFuncionario + " - " + r.Objeto.NomeFuncionario);
                    $("#hddCodigoVeiculo").val(r.Objeto.CodigoVeiculo);
                    $("#txtVeiculo").val(r.Objeto.PlacaVeiculo);
                    $("#hddCodigoTipoOcorrencia").val(r.Objeto.CodigoTipoOcorrencia);
                    $("#txtTipoDeOcorrencia").val(r.Objeto.DescricaoTipoOcorrencia);
                    $("#txtDataDaOcorrencia").val(r.Objeto.DataDaOcorrencia);
                    $("#txtDescricao").val(r.Objeto.Descricao);
                    $("#selStatus").val(r.Objeto.Status);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RetornoConsultaFuncionario(funcionario) {
            $("#hddCodigoFuncionario").val(funcionario.Codigo);
            $("#txtFuncionario").val(funcionario.CPFCNPJ + " - " + funcionario.Nome);
        }
        function RetornoConsultaVeiculo(veiculo) {
            $("#hddCodigoVeiculo").val(veiculo.Codigo);
            $("#txtVeiculo").val(veiculo.Placa);
        }
        function RetornoConsultaTipoDeOcorrencia(tipo) {
            $("#hddCodigoTipoOcorrencia").val(tipo.Codigo);
            $("#txtTipoDeOcorrencia").val(tipo.Descricao);
        }
        function LimparCampos() {
            $("#hddCodigo").val('0');
            $("#hddCodigoFuncionario").val('0');
            $("#txtFuncionario").val('');
            $("#hddCodigoVeiculo").val('0');
            $("#txtVeiculo").val('');
            $("#hddCodigoTipoOcorrencia").val('0');
            $("#txtTipoDeOcorrencia").val('');
            $("#txtDataDaOcorrencia").val('');
            $("#txtDescricao").val('');
            $("#selStatus").val($("#selStatus option:first").val());
            SetarDadosPadrao();
        }
        function ValidarCampos() {
            var descricao = $("#txtDescricao").val().trim();
            var codigoFuncionario = Globalize.parseInt($("#hddCodigoFuncionario").val());
            var codigoTipoOcorrencia = Globalize.parseInt($("#hddCodigoTipoOcorrencia").val());
            var valido = true;
            if (descricao != "") {
                CampoSemErro("#txtDescricao");
            } else {
                CampoComErro("#txtDescricao");
                valido = false;
            }
            if (!isNaN(codigoFuncionario) && codigoFuncionario > 0) {
                CampoSemErro("#txtFuncionario");
            } else {
                CampoComErro("#txtFuncionario");
                valido = false;
            }
            if (codigoTipoOcorrencia > 0) {
                CampoSemErro("#txtTipoDeOcorrencia");
            } else {
                CampoComErro("#txtTipoDeOcorrencia");
                valido = false;
            }
            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    CodigoFuncionario: $("#hddCodigoFuncionario").val(),
                    CodigoVeiculo: $("#hddCodigoVeiculo").val(),
                    CodigoTipoDeOcorrencia: $("#hddCodigoTipoOcorrencia").val(),
                    DataDaOcorrencia: $("#txtDataDaOcorrencia").val(),
                    Descricao: $("#txtDescricao").val(),
                    Status: $("#selStatus").val()
                };
                executarRest("/OcorrenciaDeFuncionario/Salvar?callback=?", dados, function (r) {
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
        <input type="hidden" id="hddCodigoFuncionario" value="0" />
        <input type="hidden" id="hddCodigoVeiculo" value="0" />
        <input type="hidden" id="hddCodigoTipoOcorrencia" value="0" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Ocorrências de Funcionários
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-5 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Data Ocorrência*:
                </span>
                <input type="text" id="txtDataDaOcorrencia" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-7 col-lg-5">
            <div class="input-group">
                <span class="input-group-addon">Funcionário*:
                </span>
                <input type="text" id="txtFuncionario" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarFuncionario" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-4">
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
                <span class="input-group-addon">Tipo Ocorrência*:
                </span>
                <input type="text" id="txtTipoDeOcorrencia" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarTipoDeOcorrencia" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-3">
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
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">Descrição*:
                </span>
                <textarea id="txtDescricao" class="form-control" rows="3"></textarea>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
