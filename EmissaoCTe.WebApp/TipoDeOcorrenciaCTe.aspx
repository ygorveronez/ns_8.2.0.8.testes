<%@ Page Title="Cadastro de Tipo de Ocorrência CTe" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="TipoDeOcorrenciaCTe.aspx.cs" Inherits="EmissaoCTe.WebApp.TipoDeOcorrenciaCTe" %>

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
            CarregarConsultaDeTiposDeOcorrenciasDeCTesCadastro("default-search", "default-search", RetornoConsultaTipoDeOcorrenciaCTe, true, false);
            CarregarConsultadeClientes("btnBuscarCliente", "btnBuscarCliente", RetornoConsultaCliente, true, false, "");

            $("#txtCliente").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("cliente", null);
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
        function RetornoConsultaTipoDeOcorrenciaCTe(tipo) {
            executarRest("/TipoDeOcorrenciaDeCTe/ObterDetalhes?callback=?", { Codigo: tipo.Codigo }, function (r) {
                if (r.Sucesso) {
                    //RenderizarTipo(r.Objeto);
                    $("body").data("cliente", r.Objeto.CNPJCliente);
                    $("#txtCliente").val(r.Objeto.Cliente);
                    $("#txtDescricao").val(r.Objeto.Descricao);
                    $("#txtCodigoProceda").val(r.Objeto.CodigoProceda);
                    $("#txtCodigoIntegracao").val(r.Objeto.CodigoIntegracao);
                    $("#selTipo").val(r.Objeto.Tipo);
                    $("#hddCodigo").val(r.Objeto.Codigo);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
        function RetornoConsultaCliente(cliente) {
            $("body").data("cliente", cliente.CPFCNPJ);
            $("#txtCliente").val(cliente.CPFCNPJ + " - " + cliente.Nome);
        }
        function LimparCampos() {
            $("#txtDescricao").val('');
            $("#selTipo").val($("#selTipo option:first").val());
            $("#txtCodigoProceda").val('');
            $("#txtCodigoIntegracao").val('');
            $("#txtCliente").val('');
            $("body").data("cliente", null);
            $("#hddCodigo").val('0');
        }
        function ValidarCampos() {
            var descricao = $("#txtDescricao").val().trim();
            var codigoProceda = $("#txtCodigoProceda").val().trim();
            var valido = true;

            if (descricao != "") {
                CampoSemErro("#txtDescricao");
            } else {
                CampoComErro("#txtDescricao");
                valido = false;
            }

            if (descricao != "") {
                CampoSemErro("#txtCodigoProceda");
            } else {
                CampoComErro("#txtCodigoProceda");
                valido = false;
            }

            return valido;
        }
        function Salvar() {
            if (ValidarCampos()) {
                var tipoDeOcorrencia = {
                    Codigo: $("#hddCodigo").val(),
                    Descricao: $("#txtDescricao").val(), 
                    CodigoProceda: $("#txtCodigoProceda").val(),
                    CodigoIntegracao: $("#txtCodigoIntegracao").val(),
                    Tipo: $("#selTipo").val(), 
                    CNPJCliente: $("body").data("cliente"),
                };

                executarRest("/TipoDeOcorrenciaDeCTe/Salvar?callback=?", tipoDeOcorrencia, function (r) {
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
        <h2>Cadastro de Tipo de Ocorrência CTe
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Descrição*:
                </span>
                <input type="text" id="txtDescricao" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-12 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Cliente*:
                </span>
                <input type="text" id="txtCliente" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarCliente" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Cód. EDI*:
                </span>
                <input type="text" id="txtCodigoProceda" class="form-control" maxlength="50" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-8 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Cód. Integracao:
                </span>
                <input type="text" id="txtCodigoIntegracao" class="form-control" maxlength="100" />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-3 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Tipo*:
                </span>
                <select id="selTipo" class="form-control">
                    <option value="P">Pendente</option>
                    <option value="F">Final</option>
                </select>
            </div>
        </div>

    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
