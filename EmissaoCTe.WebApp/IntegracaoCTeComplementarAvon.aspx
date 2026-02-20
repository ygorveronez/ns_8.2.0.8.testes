<%@ Page Title="Integração CT-e Complementar Avon" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="IntegracaoCTeComplementarAvon.aspx.cs" Inherits="EmissaoCTe.WebApp.IntegracaoCTeComplementarAvon" %>

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
            CarregarConsultaDeManifestosAvon("btnBuscarManifesto", "btnBuscarManifesto", "", RetornoConsultaManifesto, true, false);

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

            $("#txtManifesto").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("body").data("codigoManifesto", null);
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

        function RetornoConsultaManifesto(manifesto) {
            $("#txtManifesto").val(manifesto.Numero);
            $("body").data("codigoManifesto", manifesto.Codigo);
        }

        function VincularCTe() {
            if (ValidarDados()) {

                executarRest("/IntegracaoCTeComplementarAvon/AdicionarCTeAoManifesto?callback=?", { CodigoCTe: $("body").data("codigoCTe"), CodigoManifesto: $("body").data("codigoManifesto") }, function (r) {
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
            var codigoManifesto = $("body").data("codigoManifesto");
            var valido = true;

            if (codigoCTe <= 0) {
                CampoComErro("#txtCTe");
                valido = false;
            } else {
                CampoSemErro("#txtCTe");
            }

            if (codigoManifesto <= 0) {
                CampoComErro("#txtManifesto");
                valido = false;
            } else {
                CampoSemErro("#txtManifesto");
            }

            return valido;
        }

        function LimparCampos() {
            $("body").data("codigoCTe", null);
            $("body").data("codigoManifesto", null);
            $("#txtCTe").val("");
            $("#txtManifesto").val("");
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div class="page-header">
        <h2>Integração CT-e Complementar Avon
        </h2>
    </div>
    <div id="messages-placeholder">
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-6 col-md-6 col-lg-6">
            <div class="input-group">
                <span class="input-group-addon">Manifesto*:
                </span>
                <input type="text" id="txtManifesto" class="form-control" />
                <span class="input-group-btn">
                    <button type="button" id="btnBuscarManifesto" class="btn btn-primary">Buscar</button>
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
