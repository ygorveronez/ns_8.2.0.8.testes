<%@ Page Title="Cadastro de Tipos de Despesas" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="TiposDeDespesas.aspx.cs" Inherits="EmissaoCTe.WebApp.TiposDeDespesas" %>

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
            CarregarConsultaDeTiposDeDespesas("default-search", "default-search", "", RetornoConsultaTipoDeDespesa, true, false);
            CarregarConsultaDePlanosDeContas("btnBuscarContaContabil", "btnBuscarContaContabil", "A", "A", RetornoConsultaContaContabil, true, false);

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            $("#txtContaContabil").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("contaContabil", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });
        });

        function RetornoConsultaContaContabil(conta) {
            $("body").data("contaContabil", conta.Codigo);
            $("#txtContaContabil").val(conta.Conta + " - " + conta.Descricao);
        }

        function RetornoConsultaTipoDeDespesa(tipo) {
            $("#txtDescricao").val(tipo.Descricao);
            $("#selStatus").val(tipo.Status);
            $("#hddCodigo").val(tipo.Codigo);
            $("body").data("contaContabil", tipo.CodigoContaContabil);
            $("#txtContaContabil").val(tipo.ContaContabil);
        }

        function LimparCampos() {
            $("#txtDescricao").val('');
            $("#txtContaContabil").val("");
            $("body").data("contaContabil", null);
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
                executarRest("/TipoDeDespesa/Salvar?callback=?", { Codigo: $("#hddCodigo").val(), Descricao: $("#txtDescricao").val(), Status: $("#selStatus").val(), CodigoContaContabil: $("body").data("contaContabil") }, function (r) {
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
        <h2>Cadastro de Tipos de Despesas
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-8 col-md-8 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Descrição*:
                </span>
                <input type="text" id="txtDescricao" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Conta Contábil">Conta Cont.</abbr>:
                </span>
                <input type="text" id="txtContaContabil" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarContaContabil" class="btn btn-primary">Buscar</button>
                </span>
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
