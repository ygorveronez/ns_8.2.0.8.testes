<%@ Page Title="Cadastro de Eixos de Veículos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="EixosDeVeiculos.aspx.cs" Inherits="EmissaoCTe.WebApp.EixosDeVeiculos" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundle/scripts/blockui",
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
            $("#txtOrdemEixo").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });
            CarregarConsultaDeEixosDeVeiculos("default-search", "default-search", "", RetornoConsultaEixoDeVeiculo, true, false);
            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
        });
        function RetornoConsultaEixoDeVeiculo(eixo) {
            executarRest("/EixoDeVeiculo/ObterDetalhes?callback=?", { Codigo: eixo.Codigo }, function (r) {
                if (r.Sucesso) {
                    eixo = r.Objeto;
                    $("#hddCodigo").val(eixo.Codigo);
                    $("#txtDescricao").val(eixo.Descricao);
                    $("#chkEixoDianteiro").attr("checked", eixo.Dianteiro);
                    $("#selTipo").val(eixo.Tipo);
                    $("#txtOrdemEixo").val(eixo.OrdemEixo);
                    $("#selStatus").val(eixo.Status);
                    $("#selPosicaoVeiculo").val(eixo.Posicao);
                    $("#selPosicaoEixo").val(eixo.InternoExterno);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function LimparCampos() {
            $("#hddCodigo").val('0');
            $("#txtDescricao").val('');
            $("#chkEixoDianteiro").attr("checked", false);
            $("#selTipo").val($("#selTipo option:first").val());
            $("#txtOrdemEixo").val('');
            $("#selStatus").val($("#selStatus option:first").val());
            $("#selPosicaoVeiculo").val($("#selPosicaoVeiculo option:first").val());
            $("#selPosicaoEixo").val($("#selPosicaoVeiculo option:first").val());
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
            return valido;
        }
        function Salvar() {
            var dados = {
                Codigo: Globalize.parseInt($("#hddCodigo").val()),
                Descricao: $("#txtDescricao").val(),
                Dianteiro: $("#chkEixoDianteiro")[0].checked,
                Tipo: $("#selTipo").val(),
                OrdemEixo: $("#txtOrdemEixo").val(),
                Status: $("#selStatus").val(),
                Posicao: $("#selPosicaoVeiculo").val(),
                InternoExterno: $("#selPosicaoEixo").val()
            };
            if (ValidarCampos()) {
                executarRest("/EixoDeVeiculo/Salvar?callback=?", dados, function (r) {
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
    </div>
    <div class="page-header">
        <h2>Cadastro de Eixos de Veículos
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-7">
            <div class="input-group">
                <span class="input-group-addon">Descrição*:
                </span>
                <input type="text" id="txtDescricao" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-3 col-md-2 col-lg-1">
            <div class="checkbox">
                <label>
                    <input type="checkbox" id="chkEixoDianteiro" value="" />
                    Dianteiro
                </label>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Tipo:
                </span>
                <select id="selTipo" class="form-control">
                    <option value="S">Simples</option>
                    <option value="D">Duplo</option>
                    <option value="E">Estepe</option>
                </select>
            </div>
        </div>
        <div class="clearfix hidden-sm"></div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Posição no Veículo:
                </span>
                <select id="selPosicaoVeiculo" class="form-control">
                    <option value="D">Direito</option>
                    <option value="E">Esquerdo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Posição no Eixo:
                </span>
                <select id="selPosicaoEixo" class="form-control">
                    <option value="I">Interno</option>
                    <option value="E">Externo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Posição no Veículo:
                </span>
                <input type="text" id="txtOrdemEixo" class="form-control" value="" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-3">
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
