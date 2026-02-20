<%@ Page Title="Cadastro de Unidades de Medidas" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="UnidadeMedida.aspx.cs" Inherits="EmissaoCTe.WebApp.UnidadeMedida" %>

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
    <script type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDeUnidadesDeMedidaGerais("default-search", "default-search", "", RetornoConsultaUnidadeMedida, true, false);

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            LimparCampos();
        });

        function RetornoConsultaUnidadeMedida(unidadeMedida) {
            $("body").data("codigo", unidadeMedida.Codigo);
            $("#txtSigla").val(unidadeMedida.Sigla);
            $("#txtDescricao").val(unidadeMedida.Descricao);
            $("#selStatus").val(unidadeMedida.Status);
        }

        function ValidarDados() {
            var sigla = $("#txtSigla").val();
            var descricao = $("#txtDescricao").val();
            var valido = true;

            if (descricao == null || descricao == "") {
                CampoComErro("#txtDescricao");
                valido = false;
            } else {
                CampoSemErro("#txtDescricao");
            }

            if (sigla == null || sigla == "") {
                CampoComErro("#txtSigla");
                valido = false;
            } else {
                CampoSemErro("#txtSigla");
            }

            return valido;
        }

        function Salvar() {
            if (ValidarDados()) {
                var unidadeMedida = {
                    Codigo: $("body").data("codigo"),
                    Sigla: $("#txtSigla").val(),
                    Descricao: $("#txtDescricao").val(),
                    Status: $("#selStatus").val()
                }

                executarRest("/UnidadeMedidaGeral/Salvar?callback=?", unidadeMedida, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!");
            }
        }

        function LimparCampos() {
            $("body").data("codigo", 0);
            $("#txtSigla").val('');
            $("#txtDescricao").val('');
            $("#selStatus").val($("#selStatus option:first").val());
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Unidades de Medidas
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Sigla*:
                </span>
                <input type="text" id="txtSigla" class="form-control" maxlength="3" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Descrição*:
                </span>
                <input type="text" id="txtDescricao" class="form-control" maxlength="8" />
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
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
