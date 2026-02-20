<%@ Page Title="Importação de Veículos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ImportacaoDeVeiculos.aspx.cs" Inherits="EmissaoCTe.WebAdmin.ImportacaoDeVeiculos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/Fancybox/jquery.fancybox.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-buttons.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-thumbs.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/plupload/jquery.plupload.queue.min.css" rel="stylesheet" type="text/css" />
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.priceformat.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-buttons.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-thumbs.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-media.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/plupload.full.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/jquery.plupload.queue.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/pt-br.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDeEmpresas("btnBuscarEmpresaOrigem", "btnBuscarEmpresaOrigem", "A", RetornoConsultaEmpresaOrigem, true, false);
            CarregarConsultaDeEmpresas("btnBuscarEmpresaDestino", "btnBuscarEmpresaDestino", "A", RetornoConsultaEmpresaDestino, true, false);

            $("#btnImportarVeiculos").click(function () {
                ImportarVeiculos();
            });

        });

        function RetornoConsultaEmpresaOrigem(empresa) {
            $("#txtEmpresaOrigem").val(empresa.NomeFantasia);
            $("#txtEmpresaOrigem").data("codigo", empresa.Codigo);
        }

        function RetornoConsultaEmpresaDestino(empresa) {
            $("#txtEmpresaDestino").val(empresa.NomeFantasia);
            $("#txtEmpresaDestino").data("codigo", empresa.Codigo);
        }

        function ValidarDados() {
            var codigoEmpresaOrigem = $("#txtEmpresaOrigem").data("codigo");
            var codigoEmpresaDestino = $("#txtEmpresaDestino").data("codigo");
            var valido = true;

            if (codigoEmpresaOrigem == null || codigoEmpresaOrigem <= 0) {
                CampoComErro("#txtEmpresaOrigem");
                valido = false;
            } else {
                CampoSemErro("#txtEmpresaOrigem");
            }

            if (codigoEmpresaDestino == null || codigoEmpresaDestino <= 0) {
                CampoComErro("#txtEmpresaDestino");
                valido = false;
            } else {
                CampoSemErro("#txtEmpresaDestino");
            }

            return valido;
        }

        function LimparCampos() {
            $("#txtEmpresaOrigem").val('');
            $("#txtEmpresaOrigem").data("codigo", null);
            $("#txtEmpresaDestino").val('');
            $("#txtEmpresaDestino").data("codigo", null);
        }

        function ImportarVeiculos() {
            if (ValidarDados()) {
                var dados = {
                    CodigoEmpresaOrigem: $("#txtEmpresaOrigem").data("codigo"),
                    CodigoEmpresaDestino: $("#txtEmpresaDestino").data("codigo")
                };
                executarRest("/ImportacaoVeiculo/Importar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Importação realizada com sucesso!", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Importação de Veículos das Empresas
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
                    <div class="fields" style="margin-top: 15px;">
                        <div class="response-msg error ui-corner-all" id="divMensagemErro" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg notice ui-corner-all" id="divMensagemAlerta" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg success ui-corner-all" id="divMensagemSucesso" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Empresa (de):
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEmpresaOrigem" />
                                </div>
                            </div>
                            <div class="field fieldum" style="width: 65px;">
                                <div class="buttons">
                                    <input type="button" id="btnBuscarEmpresaOrigem" value="Buscar" />
                                </div>
                            </div>
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Empresa (para):
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEmpresaDestino" />
                                </div>
                            </div>
                            <div class="field fieldum" style="width: 65px;">
                                <div class="buttons">
                                    <input type="button" id="btnBuscarEmpresaDestino" value="Buscar" />
                                </div>
                            </div>
                        </div>
                        <div class="buttons" style="margin-left: 5px;">
                            <input type="button" id="btnImportarVeiculos" value="Importar Veículos" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
