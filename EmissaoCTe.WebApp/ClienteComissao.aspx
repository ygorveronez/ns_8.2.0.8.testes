<%@ Page Title="Cadatro Comissão Cliente/Cidade" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ClienteComissao.aspx.cs" Inherits="EmissaoCTe.WebApp.ClienteComissao" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datetimepicker")%>
        <%: Scripts.Render("~/bundle/scripts/json",
                           "~/bundle/scripts/blockui",
                           "~/bundle/scripts/maskedinput",
                           "~/bundle/scripts/datatables",
                           "~/bundle/scripts/ajax",
                           "~/bundle/scripts/gridview",
                           "~/bundle/scripts/consulta",
                           "~/bundle/scripts/baseConsultas",
                           "~/bundle/scripts/mensagens",
                           "~/bundle/scripts/datetimepicker",
                           "~/bundle/scripts/validaCampos",
                           "~/bundle/scripts/priceformat",
                           "~/bundle/scripts/plupload") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaClienteComissao("default-search", "default-search", "", RetornoConsultaClienteComissao, true, false);
            CarregarConsultadeClientes("btnBuscarParceiro", "btnBuscarParceiro", RetornoConsultaParceiro, true, false);
            CarregarConsultaDeLocalidades("btnBuscarLocalidade", "btnBuscarLocalidade", RetornoConsultaLocalidade, true, false);
            
            $("#txtValorMinimo").priceFormat();
            $("#txtPercComissao").priceFormat();
            $("#txtValorTDA").priceFormat();

            $("#txtParceiro").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("parceiro", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtLocalidade").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("localidade", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnSalvar").click(function () {
                SalvarComissao();
            });
            $("#btnCancelar").click(function () {
                LimparCamposComissao();
            });
        });

        function RetornoConsultaParceiro(parceiro) {
            $("body").data("parceiro", parceiro.CPFCNPJ);
            $("#txtParceiro").val(parceiro.CPFCNPJ + " - " + parceiro.Nome);
        }

        function RetornoConsultaLocalidade(cidade) {
            $("body").data("localidade", cidade.Codigo);
            $("#txtLocalidade").val(cidade.Descricao + " / " + cidade.UF);
        }

        function LimparCamposComissao() {
            $("body").data("comissao", null);
            $("#txtParceiro").val("");
            $("body").data("parceiro", null);
            $("#txtLocalidade").val("");
            $("body").data("localidade", null);
            $("#txtPercComissao").val("0,00");
            $("#txtValorMinimo").val('0,00');
            $("#txtValorTDA").priceFormat();
            $("#selStatus").val($("#selStatus option:first").val()).change();
        }

        function ValidarCamposComissao() {
            var parceiro = $("body").data("parceiro");
            var localidade = $("body").data("localidade");
            var percComissao = Globalize.parseFloat($("#txtPercComissao").val());
            var valido = true;

            if (parceiro == null || parceiro == "") {
                CampoComErro("#txtParceiro");
                valido = false;
            } else {
                CampoSemErro("#txtParceiro");
            }

            if (localidade == null || localidade == "") {
                CampoComErro("#txtLocalidade");
                valido = false;
            } else {
                CampoSemErro("#txtLocalidade");
            }

            if (percComissao > 0) {
                CampoSemErro("#txtPercComissao");
            } else {
                CampoComErro("#txtPercComissao");
                valido = false;
            }

            return valido;
        }

        function SalvarComissao() {
            if (ValidarCamposComissao()) {
                var comissao = {
                    Codigo: $("body").data("comissao") != null ? $("body").data("comissao").Codigo : 0,
                    Parceiro: $("body").data("parceiro"),
                    Localidade: $("body").data("localidade"),
                    PercComissao: $("#txtPercComissao").val(),
                    ValorMinimo: $("#txtValorMinimo").val(),
                    Status: $("#selStatus").val(),
                    ValorTDA: $("#txtValorTDA").val()
                };
                executarRest("/ClienteComissao/Salvar?callback=?", comissao, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCamposComissao();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!");
            }
        }

        function RetornoConsultaClienteComissao(comissao) {
            executarRest("/ClienteComissao/ObterDetalhes?callback=?", { Codigo: comissao.Codigo }, function (r) {
                if (r.Sucesso) {
                    RenderizarComissao(r.Objeto);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

        function RenderizarComissao(comissao) {
            LimparCamposComissao();

            $("body").data("comissao", comissao);

            $("#txtParceiro").val(comissao.Parceiro);
            $("body").data("parceiro", comissao.CPFCNPJParceiro);
            $("#txtLocalidade").val(comissao.Localidade);
            $("body").data("localidade", comissao.CodigoLocalidade);
            $("#txtPercComissao").val(Globalize.format(comissao.PercComissao, "n2")); 
            $("#txtValorMinimo").val(Globalize.format(comissao.ValorMinimo, "n2"));
            $("#txtValorTDA").val(Globalize.format(comissao.ValorTDA, "n2"));
            $("#selStatus").val(comissao.Status);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro Comissão Cliente/Cidade
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Parceiro*:
                </span>
                <input type="text" id="txtParceiro" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarParceiro" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Cidade Destino*:
                </span>
                <input type="text" id="txtLocalidade" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarLocalidade" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Perc. Comissão*:
                </span>
                <input type="text" id="txtPercComissao" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor Mínimo:
                </span>
                <input type="text" id="txtValorMinimo" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor TDA:
                </span>
                <input type="text" id="txtValorTDA" class="form-control" value="0,00" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-2">
            <div class="input-group">
                <span class="input-group-addon">Status*:
                </span>
                <select id="selStatus" class="form-control">
                    <option value="1">Ativo</option>
                    <option value="0">Inativo</option>
                </select>
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
