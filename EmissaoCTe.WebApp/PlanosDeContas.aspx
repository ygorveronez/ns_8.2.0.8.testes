<%@ Page Title="Cadastro de Planos de Contas" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="PlanosDeContas.aspx.cs" Inherits="EmissaoCTe.WebApp.PlanosDeContas" %>

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
                           "~/bundle/scripts/validaCampos") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            $("#btnSalvar").click(function () {
                Salvar();
            });
            CarregarConsultaDePlanosDeContas("default-search", "default-search", "", "", RetornoConsultaPlanoConta, true, false);
        });
        function RetornoConsultaPlanoConta(plano) {
            $("#hddCodigo").val(plano.Codigo);
            $("#txtDescricao").val(plano.Descricao);
            $("#selTipo").val(plano.Tipo);
            $("#txtConta").val(plano.Conta);
            $("#txtContaContabil").val(plano.ContaContabil);
            $("#selStatus").val(plano.Status);
            $("#selTipoConta").val(plano.TipoDeConta);
            $("#chkNaoExibirDRE").attr("checked", plano.NaoExibirDRE);
        }
        function LimparCampos() {
            $("#hddCodigo").val("0");
            $("#txtDescricao").val("");
            $("#selTipo").val($("#selTipo option:first").val());
            $("#txtConta").val("");
            $("#txtContaContabil").val("");
            $("#selStatus").val($("#selStatus option:first").val());
            $("#selTipoConta").val($("#selTipoConta option:first").val());
        }
        function ValidarCampos() {
            var descricao = $("#txtDescricao").val().trim();
            var conta = $("#txtConta").val().trim();
            var valido = true;
            if (descricao != "") {
                CampoSemErro("#txtDescricao");
            } else {
                CampoComErro("#txtDescricao");
                valido = false;
            }
            var regex = new RegExp(/^([0-9]+\.){1,15}$/);
            if (regex.test(conta + ".")) {
                CampoSemErro("#txtConta");
            } else {
                CampoComErro("#txtConta");
                valido = false;
            }
            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    Descricao: $("#txtDescricao").val(),
                    Tipo: $("#selTipo").val(),
                    Conta: $("#txtConta").val(),
                    ContaContabil: $("#txtContaContabil").val(),
                    TipoDeConta: $("#selTipoConta").val(),
                    Status: $("#selStatus").val(),
                    NaoExibirDRE: $("#chkNaoExibirDRE").prop('checked')
                };
                executarRest("/PlanoDeConta/Salvar?callback=?", dados, function (r) {
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
    </div>
    <div class="page-header">
        <h2>Cadastro de Planos de Contas
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
                <span class="input-group-addon">Tipo*:
                </span>
                <select id="selTipo" class="form-control">
                    <option value="A">Analítico</option>
                    <option value="S">Sintético</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Conta*:
                </span>
                <input type="text" id="txtConta" class="form-control" maxlength="50" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Tipo de Conta*:
                </span>
                <select id="selTipoConta" class="form-control">
                    <option value="R">Receita</option>
                    <option value="D">Despesa</option>
                    <option value="O">Outras</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Conta Contábil:
                </span>
                <input type="text" id="txtContaContabil" class="form-control" maxlength="50" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-6 col-sm-6 col-md-3 col-lg-3">
            <div class="input-group">
                <div class="checkbox">
                    <label>
                        <input type="checkbox" id="chkNaoExibirDRE" />
                        Não exibir conta no DRE
                    </label>
                </div>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
