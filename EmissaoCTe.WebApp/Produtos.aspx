<%@ Page Title="Cadastro de Produtos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Produtos.aspx.cs" Inherits="EmissaoCTe.WebApp.Produtos" %>

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
            CarregarConsultaDeProdutos("default-search", "default-search", "", RetornoConsultaProduto, true, false);

            CarregarConsultaDeNCMs("btnBuscarNCM", "btnBuscarNCM", RetornoConsultaNCM, true, false);

            CarregarConsultaDeUnidadesDeMedidaGerais("btnBuscarUnidadeMedida", "btnBuscarUnidadeMedida", "A", RetornoConsultaUnidadeMedida, true, false);

            $("#txtNCM").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("ncm", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtUnidadeMedida").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("unidadeMedida", null);
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
        });

        function RetornoConsultaUnidadeMedida(unidade) {
            $("#txtUnidadeMedida").val(unidade.Sigla + " - " + unidade.Descricao);
            $("body").data("unidadeMedida", unidade.Codigo);
        }

        function RetornoConsultaNCM(ncm) {
            $("#txtNCM").val(ncm.Numero + " - " + ncm.Descricao);
            $("body").data("ncm", ncm.Codigo);
        }

        function ValidarDados() {
            var ncm = Globalize.parseInt($("body").data("ncm").toString());
            var unidadeMedida = Globalize.parseInt($("body").data("unidadeMedida").toString());
            var descricao = $("#txtDescricao").val();
            var valido = true;

            if (isNaN(ncm) || ncm <= 0) {
                CampoComErro("#txtNCM");
                valido = false;
            } else {
                CampoSemErro("#txtNCM");
            }

            if (isNaN(unidadeMedida) || unidadeMedida <= 0) {
                CampoComErro("#txtUnidadeMedida");
                valido = false;
            } else {
                CampoSemErro("#txtUnidadeMedida");
            }

            if (descricao == null || descricao == "") {
                CampoComErro("#txtDescricao");
                valido = false;
            } else {
                CampoSemErro("#txtDescricao");
            }

            return valido;
        }

        function Salvar() {
            if (ValidarDados()) {
                var produto = {
                    Codigo: $("body").data("codigo"),
                    NCM: $("body").data("ncm"),
                    UnidadeMedida: $("body").data("unidadeMedida"),
                    Descricao: $("#txtDescricao").val(),
                    Status: $("#selStatus").val(),
                    CodigoProduto: $("#txtCodigoProduto").val()
                }

                executarRest("/Produto/Salvar?callback=?", produto, function (r) {
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
            $("#txtDescricao").val('');
            $("#txtCodigoProduto").val('');
            $("#txtNCM").val('');
            $("#txtUnidadeMedida").val('');
            $("#selStatus").val($("#selStatus option:first").val());
            $("body").data("ncm", 0);
            $("body").data("unidadeMedida", 0);
            $("body").data("codigo", 0);
        }

        function RetornoConsultaProduto(produto) {
            executarRest("/Produto/ObterDetalhes?callback=?", { CodigoProduto: produto.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#txtDescricao").val(r.Objeto.Descricao);
                    $("#txtNCM").val(r.Objeto.DescricaoNCM);
                    $("#txtUnidadeMedida").val(r.Objeto.DescricaoUnidadeMedida);
                    $("#selStatus").val(r.Objeto.Status);
                    $("body").data("ncm", r.Objeto.CodigoNCM);
                    $("body").data("unidadeMedida", r.Objeto.CodigoUnidadeMedida);
                    $("body").data("codigo", r.Objeto.Codigo);
                    $("#txtCodigoProduto").val(r.Objeto.CodigoProduto);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Produtos
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
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Código Produto:
                </span>
                <input type="text" id="txtCodigoProduto" class="form-control" maxlength="200" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">NCM*:
                </span>
                <input type="text" id="txtNCM" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarNCM" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-6 col-sm-3 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">
                    <abbr title="Unidade de Medida">U.M.</abbr>*:
                </span>
                <input type="text" id="txtUnidadeMedida" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarUnidadeMedida" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-6 col-sm-3 col-md-3 col-lg-3">
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
