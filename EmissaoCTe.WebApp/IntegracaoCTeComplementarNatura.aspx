<%@ Page Title="Integração CT-e Complementar Natura" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="IntegracaoCTeComplementarNatura.aspx.cs" Inherits="EmissaoCTe.WebApp.IntegracaoCTeComplementarNatura" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundle/styles/datepicker") %>
        <%: Scripts.Render("~/bundle/scripts/blockui",
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
            CarregarConsultaDeCTes("btnBuscarCTe", "btnBuscarCTe", null, RetornoConsultaCTe, true, false);
            CarregarConsultaDeDtNatura("btnBuscarDocumentoTransporte", "btnBuscarDocumentoTransporte", "", RetornoConsultaDocumentoTransporte, true, false);

            $("#txtCTe").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoCTe", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtDocumentoTransporte").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoDocumentoTransporte", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnVincularCTe").click(function () {
                VincularCTe();
            });
        });

        function RetornoConsultaCTe(cte) {
            $("#txtCTe").val(cte.Numero + " - " + cte.Serie);
            $("body").data("codigoCTe", cte.Codigo);
        }

        function RetornoConsultaDocumentoTransporte(dt) {
            $("#txtDocumentoTransporte").val(dt.NumeroDT);
            $("body").data("codigoDocumentoTransporte", dt.Codigo);
        }

        function VincularCTe() {
            if (ValidarDados()) {

                executarRest("/IntegracaoCTeComplementarNatura/AdicionarCTeAoDocumentoDeTransporte?callback=?", { CodigoCTe: $("body").data("codigoCTe"), CodigoDt: $("body").data("codigoDocumentoTransporte") }, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso!");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção!");
                    }
                });

            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios ou possuem dados incorretos.", "Atenção!");
            }
        }

        function ValidarDados() {
            var codigoCTe = $("body").data("codigoCTe");
            var codigoDocumentoTransporte = $("body").data("codigoDocumentoTransporte");
            var valido = true;

            if (codigoCTe <= 0) {
                CampoComErro("#txtCTe");
                valido = false;
            } else {
                CampoSemErro("#txtCTe");
            }

            if (codigoDocumentoTransporte <= 0) {
                CampoComErro("#txtDocumentoTransporte");
                valido = false;
            } else {
                CampoSemErro("#txtDocumentoTransporte");
            }

            return valido;
        }

        function LimparCampos() {
            $("body").data("codigoCTe", null);
            $("body").data("codigoDocumentoTransporte", null);
            $("#txtCTe").val("");
            $("#txtDocumentoTransporte").val("");
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Integração CT-e Complementar Natura
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Documento de Transporte*:
                </span>
                <input type="text" id="txtDocumentoTransporte" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarDocumentoTransporte" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">CT-e*:
                </span>
                <input type="text" id="txtCTe" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarCTe" class="btn btn-primary">Buscar</button>
                </span>
            </div>
        </div>
    </div>
    <button type="button" id="btnVincularCTe" class="btn btn-primary">Vincular CT-e</button>
</asp:Content>
