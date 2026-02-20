<%@ Page Title="Cidades Exportação" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="CidadesExportacao.aspx.cs" Inherits="EmissaoCTe.WebApp.CidadesExportacao" %>

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
        var CodigoCidade = 0;
        $(document).ready(function () {
            CarregarConsultaCidadesExportacao("default-search", "default-search", Editar, true, false);
            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
        });

        function ValidarCampos() {
            var descricao = $("#txtDescricao").val();
            var valido = true;

            if (descricao.length == 0) {
                ExibirMensagemAlerta("A descrição é obrigatória.", "Atenção");
                CampoComErro("#txtDescricao");
                valido = false;
            } else {
                CampoSemErro("#txtDescricao");
            }

            return valido;
        }

        function LimparCampos() {
            $("#txtDescricao").val("");
            CodigoCidade = 0;
        }

        function Salvar() {
            if (ValidarCampos()) {
                executarRest("/Localidade/Salvar?callback=?", { Codigo: CodigoCidade, Descricao: $("#txtDescricao").val() }, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Cidade de exportação salva com sucesso.", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });
            }
        }

        function Editar(cidade) {
            CodigoCidade = cidade.Codigo;
            $("#txtDescricao").val(cidade.Descricao);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
    </div>
    <div class="page-header">
        <h2>Cadastro de Cidades de Exportação</h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6">
            <div class="input-group">
                <span class="input-group-addon">Estado:
                </span>
                <select class="form-control" disabled="disabled">
                    <option>EXPORTACAO</option>
                </select>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6">
            <div class="input-group">
                <span class="input-group-addon">Descrição*:
                </span>
                <input type="text" id="txtDescricao" class="form-control" />
            </div>
        </div>
    </div>
    
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
