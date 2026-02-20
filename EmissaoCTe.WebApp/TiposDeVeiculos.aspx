<%@ Page Title="Cadastro de Tipos de Veículos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="TiposDeVeiculos.aspx.cs" Inherits="EmissaoCTe.WebApp.TiposDeVeiculos" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
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
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtPesoLiquido").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
            $("#txtPesoBruto").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
            $("#txtNumeroEixos").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
            CarregarConsultaDeTiposDeVeiculos("default-search", "default-search", "", RetornoConsultaTipoDeVeiculo, true, false);
            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
        });
        function RetornoConsultaTipoDeVeiculo(tipo) {
            $("#txtDescricao").val(tipo.Descricao);
            $("#selStatus").val(tipo.Status);
            $("#hddCodigo").val(tipo.Codigo);
            $("#txtPesoLiquido").val(tipo.PesoLiquido);
            $("#txtPesoBruto").val(tipo.PesoBruto);
            $("#txtCodigoIntegracao").val(tipo.CodigoIntegracao);
            $("#txtNumeroEixos").val(tipo.NumeroEixos);
            BuscarEixos(tipo);
        }
        function LimparCampos() {
            $("#txtDescricao").val('');
            $("#txtPesoLiquido").val('0');
            $("#txtPesoBruto").val('0');
            $("#txtCodigoIntegracao").val('');
            $("#txtNumeroEixos").val('0');
            $("#selStatus").val($("#selStatus option:first").val());
            $("#hddCodigo").val('0');
            LimparCamposEixo();
            $("#hddEixos").val("");
            RenderizarEixos();
        }
        function ValidarCampos() {
            var descricao = $("#txtDescricao").val().trim();
            var valido = true;
            if (descricao != "") {
                CampoSemErro("#txtDescricao");
            } else {
                CampoComErro("#txtDescricao");
                valido = false;
            }
            var eixos = $("#hddEixos").val() == "" ? new Array() : JSON.parse($("#hddEixos").val());
            var numeroEixos = Globalize.parseInt($("#txtNumeroEixos").val());
            if (isNaN(numeroEixos))
                numeroEixos = 0;
            var valido = true;
            for (var i = 0; i < eixos.length; i++) {
                var ordemEixo = Globalize.parseInt(eixos[i].OrdemEixo);
                if (ordemEixo > numeroEixos || ordemEixo < 0) {
                    valido = false;
                    break;
                }
            }
            if (!valido) {
                jAlert("A ordem de algum dos eixos configurados não é valida de acordo com o número de eixos informado.", "Atenção!");
            }
            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                executarRest("/TipoDeVeiculo/Salvar?callback=?", {
                    Codigo: $("#hddCodigo").val(),
                    Descricao: $("#txtDescricao").val(),
                    PesoLiquido: Globalize.parseInt($("#txtPesoLiquido").val()),
                    PesoBruto: Globalize.parseInt($("#txtPesoBruto").val()),
                    CodigoIntegracao: $("#txtCodigoIntegracao").val(),
                    NumeroEixos: $("#txtNumeroEixos").val(),
                    Status: $("#selStatus").val(),
                    Eixos: $("#hddEixos").val()
                }, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
            }
        }
    </script>
    <script defer="defer" id="ScriptEixos" type="text/javascript">
        $(document).ready(function () {
            $("#txtEixo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddEixoEmEdicao").val("");
                    } else {
                        e.preventDefault();
                    }
                }
            });
            $("#btnSalvarEixo").click(function () {
                SalvarEixo();
            });
            $("#btnExcluirEixo").click(function () {
                ExcluirEixo();
            });
            $("#btnCancelarEixo").click(function () {
                LimparCamposEixo();
            });
            CarregarConsultaDeEixosDeVeiculos("btnBuscarEixo", "btnBuscarEixo", "A", RetornoConsultaEixo, true, false);
        });
        function BuscarEixos(tipoVeiculo) {
            executarRest("/TipoDeVeiculo/BuscarEixos?callback=?", { Codigo: tipoVeiculo.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#hddEixos").val(JSON.stringify(r.Objeto));
                    RenderizarEixos();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }
        function RetornoConsultaEixo(eixo) {
            $("#txtEixo").val(eixo.Descricao);
            $("#hddEixoEmEdicao").val(JSON.stringify(eixo));
        }
        function LimparCamposEixo() {
            $("#txtEixo").val('');
            $("#hddEixoEmEdicao").val("");
            $("#btnExcluirEixo").hide();
        }
        function ValidarEixo() {
            if ($("#hddEixoEmEdicao").val() != "") {
                CampoSemErro("#txtEixo");
                return true;
            }
            else {
                CampoComErro("#txtEixo");
                return false;
            }
        }
        function SalvarEixo() {
            if (ValidarEixo()) {
                var eixo = JSON.parse($("#hddEixoEmEdicao").val());
                var eixos = $("#hddEixos").val() == "" ? new Array() : JSON.parse($("#hddEixos").val());
                var numeroEixos = Globalize.parseInt($("#txtNumeroEixos").val());
                if (isNaN(numeroEixos))
                    numeroEixos = 0;
                var ordemEixo = Globalize.parseInt(eixo.OrdemEixo);
                if (ordemEixo > numeroEixos || ordemEixo < 0) {
                    jAlert("Ordem do eixo incorreta.", "Atenção!");
                    return;
                }
                for (var i = 0; i < eixos.length; i++) {
                    if (eixos[i].Codigo == eixo.Codigo) {
                        eixos.splice(i, 1);
                        break;
                    }
                }
                eixos.push(eixo);
                $("#hddEixos").val(JSON.stringify(eixos));
                RenderizarEixos();
                LimparCamposEixo();
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!");
            }
        }
        function ExcluirEixo() {
            var eixo = JSON.parse($("#hddEixoEmEdicao").val());
            var eixos = $("#hddEixos").val() == "" ? new Array() : JSON.parse($("#hddEixos").val());
            for (var i = 0; i < eixos.length; i++) {
                if (eixos[i].Codigo == eixo.Codigo) {
                    eixos.splice(i, 1);
                    break;
                }
            }
            $("#hddEixos").val(JSON.stringify(eixos));
            RenderizarEixos();
            LimparCamposEixo();
        }
        function EditarEixo(eixo) {
            $("#txtEixo").val(eixo.Descricao);
            $("#hddEixoEmEdicao").val(JSON.stringify(eixo));
            $("#btnExcluirEixo").show();
        }
        function RenderizarEixos() {
            $("#tblEixos tbody").html("");
            var eixos = $("#hddEixos").val() == "" ? new Array() : JSON.parse($("#hddEixos").val());
            for (var i = 0; i < eixos.length; i++) {
                $("#tblEixos tbody").append("<tr><td>" + eixos[i].Descricao + "</td><td>" + eixos[i].DescricaoDianteiro + "</td><td>" + eixos[i].DescricaoTipo + "</td><td>" + eixos[i].OrdemEixo + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarEixo(" + JSON.stringify(eixos[i]) + ")'>Editar</button></td></tr>");
            }
            if ($("#tblEixos tbody").html() == "")
                $("#tblEixos tbody").html("<tr><td colspan='5'>Nenhum registro encontrado.</td></tr>");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
        <input type="hidden" id="hddEixoEmEdicao" value="" />
        <input type="hidden" id="hddEixos" value="" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Tipos de Veículos
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Descrição*:
                </span>
                <input type="text" id="txtDescricao" class="form-control" maxlength="200" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Peso Líquido:
                </span>
                <input type="text" id="txtPesoLiquido" class="form-control" value="0" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Peso Bruto:
                </span>
                <input type="text" id="txtPesoBruto" class="form-control" value="0" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon"><abbr title="Código de Integração">Cód. Integ.</abbr>:
                </span>
                <input type="text" id="txtCodigoIntegracao" class="form-control" maxlength="20" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Número Eixos:
                </span>
                <input type="text" id="txtNumeroEixos" class="form-control" value="0" />
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
    </div>
    <h3 style="margin-bottom: 10px;">Eixos do Veículo
    </h3>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Eixo:
                </span>
                <input type="text" id="txtEixo" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarEixo" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <button type="button" id="btnSalvarEixo" class="btn btn-primary">Salvar</button>
                <button type="button" id="btnExcluirEixo" class="btn btn-danger" style="display: none;">Excluir</button>
                <button type="button" id="btnCancelarEixo" class="btn btn-default">Cancelar</button>
            </div>
        </div>
    </div>
    <div class="table-responsive">
        <table id="tblEixos" class="table table-bordered table-condensed table-hover">
            <thead>
                <tr>
                    <th style="width: 45%;">Descrição
                    </th>
                    <th style="width: 15%;">Dianteiro
                    </th>
                    <th style="width: 15%;">Tipo
                    </th>
                    <th style="width: 15%;">Ordem
                    </th>
                    <th style="width: 10%;">Opções
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td colspan="5">Nenhum registro encontrado.
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
