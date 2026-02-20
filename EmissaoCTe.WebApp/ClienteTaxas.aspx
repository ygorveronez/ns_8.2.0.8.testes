<%@ Page Title="Cadatro Taxas Clientes" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ClienteTaxas.aspx.cs" Inherits="EmissaoCTe.WebApp.ClienteTaxas" %>

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
            CarregarConsultadeClientes("default-search", "default-search", RetornoConsultaClientes, true, false);

            $("#txtValorTDE").priceFormat();

            $("#btnSalvar").click(function () {
                Salvar();
            });
            $("#btnCancelar").click(function () {
                LimparCampos();
            });
        });

        function LimparCampos() {
            $("#txtCliente").val("");
            $("body").data("Cliente", null);
            $("#txtValorTDE").val("0,00");

        }

        function ValidarCampos() {
            var Cliente = $("body").data("Cliente");
            var valido = true;

            if (Cliente == null || Cliente == "") {
                CampoComErro("#txtCliente");
                valido = false;
            } else {
                CampoSemErro("#txtCliente");
            }

            return valido;
        }

        function RetornoConsultaClientes(cliente) {
            executarRest("/Cliente/ObterDetalhesPorCPFCNPJ?callback=?", { CPF_CNPJ: cliente.CPFCNPJ }, function (r) {
                if (r.Sucesso) {
                    Renderizar(r.Objeto);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção!");
                }
            });
        }

        function Salvar() {
            if (ValidarCampos()) {
                var cliente = {
                    Cliente: $("body").data("Cliente"),
                    ValorTDE: $("#txtValorTDE").val()
                };
                executarRest("/Cliente/SalvarTaxas?callback=?", cliente, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso.", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!");
            }
        }

        function Renderizar(cliente) {
            LimparCampos();

            $("#txtCliente").val(cliente.CPF_CNPJ + ' ' + cliente.Nome );
            $("body").data("Cliente", cliente.CPF_CNPJ);
            $("#txtValorTDE").val(Globalize.format(cliente.ValorTDE, "n2"));
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Cadastro Taxas Cliente
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
                <span class="input-group-addon">Cliente:
                </span>
                <input type="text" id="txtCliente" class="form-control" disabled />
            </div>
        </div>
        <div class="col-xs-12 col-sm-4 col-md-4 col-lg-3">
            <div class="input-group">
                <span class="input-group-addon">Valor TDE:
                </span>
                <input type="text" id="txtValorTDE" class="form-control" value="0,00" />
            </div>
        </div>
    </div>
    <button type="button" id="btnSalvar" class="btn btn-primary">Salvar</button>
    <button type="button" id="btnCancelar" class="btn btn-default">Cancelar</button>
</asp:Content>
