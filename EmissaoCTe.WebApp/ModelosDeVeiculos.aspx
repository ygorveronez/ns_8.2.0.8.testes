<%@ Page Title="Cadastro de Modelos de Veículos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ModelosDeVeiculos.aspx.cs" Inherits="EmissaoCTe.WebApp.ModelosDeVeiculos" %>

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
            $("#txtNumeroEixo").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });

            CarregarConsultaDeModelosDeVeiculos("default-search", "default-search", "", RetornoConsultaModeloDeVeiculo, true, false);

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });
        });
        function RetornoConsultaModeloDeVeiculo(tipo) {
            $("#txtDescricao").val(tipo.Descricao);
            $("#selStatus").val(tipo.Status);
            $("#hddCodigo").val(tipo.Codigo);
            $("#txtNumeroEixo").val(tipo.NumeroEixo);
        }
        function LimparCampos() {
            $("#txtDescricao").val('');
            $("#txtNumeroEixo").val('0');
            $("#selStatus").val($("#selStatus option:first").val());
            $("#hddCodigo").val('0');
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
            if (ValidarCampos()) {
                executarRest("/ModeloDeVeiculo/Salvar?callback=?", { Codigo: $("#hddCodigo").val(), Descricao: $("#txtDescricao").val(), NumeroEixo: Globalize.parseInt($("#txtNumeroEixo").val()), Status: $("#selStatus").val() }, function (r) {
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
        <h2>Cadastro de Modelos de Veículos
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
                <span class="input-group-addon">Número do Eixo:
                </span>
                <input type="text" id="txtNumeroEixo" class="form-control" value="0" />
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
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
