<%@ Page Title="Cadastro de Alíquotas de ICMS" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="AliquotasDeICMS.aspx.cs" Inherits="EmissaoCTe.WebApp.AliquotasDeICMS" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
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
                           "~/bundle/scripts/priceformat") %>
    </asp:PlaceHolder>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDeAliquotasDeICMS("default-search", "default-search", "", RetornoConsultaAliquotas, true, false);
            $("#txtAliquota").priceFormat({
                limit: 5,
                centsLimit: 2
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
            $("#btnSalvar").click(function () {
                Salvar();
            });
            LimparCampos();
        });
        function RetornoConsultaAliquotas(aliquota) {
            $("body").data("codigoAliquota", aliquota.Codigo);
            $("#txtAliquota").val(aliquota.Aliquota);
            $("#selStatus").val(aliquota.Status);
        }
        function Salvar() {
            var dados = {
                Codigo: $("body").data("codigoAliquota"),
                Aliquota: $("#txtAliquota").val(),
                Status: $("#selStatus").val()
            };
            executarRest("/AliquotaDeICMS/Salvar?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                    LimparCampos();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }
        function LimparCampos() {
            $("body").data("codigoAliquota", 0);
            $("#txtAliquota").val("0,00");
            $("#selStatus").val($("#selStatus option:first").val());
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro de Alíquotas de ICMS
        </h2>
    </div>
    <button type="button" id="default-search" class="btn btn-default default-search">
        <span class="glyphicon glyphicon-search"></span>&nbsp;Pesquisar
    </button>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Alíquota:
                </span>
                <input type="text" id="txtAliquota" class="form-control" value="0,00" />
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
