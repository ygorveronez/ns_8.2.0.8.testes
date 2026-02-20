<%@ Page Title="Cadastro de Impressoras" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Impressoras.aspx.cs" Inherits="EmissaoCTe.WebApp.Impressoras" %>

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
            CarregarConsultaDeImpressoras("default-search", "default-search", "", RetornoConsultaImpressoras, true, false);

            $("#txtUnidade").priceFormat({ centsLimit: 0, centsSeparator: '' });

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            LimparCampos();
        });

        function RetornoConsultaImpressoras(impressora) {
            $("body").data("codigo", impressora.Codigo);
            $("#txtCodigo").val(impressora.Codigo);
            $("#txtUnidade").val(impressora.NumeroDaUnidade);
            $("#txtImpressora").val(impressora.NomeImpressora);
            $("#txtLog").val(impressora.Log);
            $("#selStatus").val(impressora.Status);
        }

        function ValidarDados() {
            var unidade = $("#txtUnidade").val();
            var impressora = $("#txtImpressora").val();
            var valido = true;

            if (unidade == null || unidade == "") {
                CampoComErro("#txtUnidade");
                valido = false;
            } else {
                CampoSemErro("#txtUnidade");
            }

            if (impressora == null || impressora == "") {
                CampoComErro("#txtImpressora");
                valido = false;
            } else {
                CampoSemErro("#txtImpressora");
            }

            return valido;
        }

        function Salvar() {
            if (ValidarDados()) {
                var impressora = {
                    Codigo: $("body").data("codigo"),
                    Unidade: $("#txtUnidade").val(),
                    Impressora: $("#txtImpressora").val(),
                    Status: $("#selStatus").val()
                }

                executarRest("/Impressoras/Salvar?callback=?", impressora, function (r) {
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
            $("#txtCodigo").val('');
            $("#txtUnidade").val('');
            $("#txtImpressora").val('');
            $("#txtLog").val('');
            $("#selStatus").val($("#selStatus option:first").val());

            document.getElementById("txtCodigo").disabled = true;
            document.getElementById("txtLog").disabled = true;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Impressoras
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-6 col-sm-3 col-md-2 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Código:
                </span>
                <input type="text" id="txtCodigo" class="form-control" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Unidade*:
                </span>
                <input type="text" id="txtUnidade" class="form-control" maxlength="12" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-4 col-lg-4">
            <div class="input-group">
                <span class="input-group-addon">Impressora*:
                </span>
                <input type="text" id="txtImpressora" class="form-control" maxlength="200" />
            </div>
        </div>
        <div class="col-xs-6 col-sm-3 col-md-2 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="A">Ativo</option>
                    <option value="I">Inativo</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Log de alterações">Log</abbr>:
                </span>
                <textarea id="txtLog" class="form-control" rows="3"></textarea>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
