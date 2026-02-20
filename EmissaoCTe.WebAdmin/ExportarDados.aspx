<%@ Page Title="Exportar Dados" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ExportarDados.aspx.cs" Inherits="EmissaoCTe.WebAdmin.ExportarDados" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/Fancybox/jquery.fancybox.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-buttons.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-thumbs.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/plupload/jquery.plupload.queue.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />
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
    <script defer="defer" src="Scripts/jquery.filedownload.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDeEmpresas("btnBuscarEmpresa", "btnBuscarEmpresa", "A", RetornoConsultaEmpresa, true, false);

            $("#txtEmpresa").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $(this).data("codigo", null);
                    }

                    e.preventDefault();
                }
            });

            $("#txtDataInicio").datepicker();
            $("#txtDataInicio").mask("99/99/9999");

            $("#txtDataFim").datepicker();
            $("#txtDataFim").mask("99/99/9999");

            LimparCampos();

            $("#btnGerarArquivoEmpresas").click(function () {
                GerarArquivoEmpresas();
            });

            $("#btnGerarArquivoEmissoes").click(function () {
                GerarArquivoEmissoes();
            });
        });

        function RetornoConsultaEmpresa(empresa) {
            $("#txtEmpresa").val(empresa.NomeFantasia);
            $("#txtEmpresa").data("codigo", empresa.Codigo);
        }

        function GerarArquivoEmpresas() {
            if (ValidarDados()) {
                executarDownload("/ExportarDados/ExportarEmpresas", { DataInicial: $("#txtDataInicio").val(), DataFinal: $("#txtDataFim").val(), CodigoEmpresa: $("#txtEmpresa").data("codigo") });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }

        function GerarArquivoEmissoes() {
            if (ValidarDados()) {
                executarDownload("/ExportarDados/ExportarEmissoes", { DataInicial: $("#txtDataInicio").val(), DataFinal: $("#txtDataFim").val(), CodigoEmpresa: $("#txtEmpresa").data("codigo") });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }

        function LimparCampos() {
            $("#txtEmpresa").data("codigo", null);
            $("#txtEmpresa").val("");
            $("#txtDataInicio").val('');
            $("#txtDataInicio").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtDataFim").val('');
            $("#txtDataFim").val(Globalize.format(new Date(), "dd/MM/yyyy"));
        }

        function ValidarDados() {
            var codigoEmpresa = $("#txtEmpresa").data("codigo");
            var dataInicio = $("#txtDataInicio").val();
            var dataFim = $("#txtDataFim").val();
            var valido = true;

            //if (codigoEmpresa == null || codigoEmpresa <= 0) {
            //    CampoComErro("#txtEmpresa");
            //    valido = false;
            //} else {
            //    CampoSemErro("#txtEmpresa");
            //}

            if (dataInicio == null || dataInicio.length <= 0) {
                CampoComErro("#txtDataInicio");
                valido = false;
            } else {
                CampoSemErro("#txtDataInicio");
            }

            if (dataFim == null || dataFim.length <= 0) {
                CampoComErro("#txtDataFim");
                valido = false;
            } else {
                CampoSemErro("#txtDataFim");
            }

            return valido;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Exportar Dados
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
                        <div class="fields">
                            <div class="fieldzao">
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Empresa:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtEmpresa" />
                                    </div>
                                </div>
                                <div class="field fieldum" style="width: 65px;">
                                    <div class="buttons">
                                        <input type="button" id="btnBuscarEmpresa" value="Buscar" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Data Inicio*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtDataInicio" class="maskedInput" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Data Fim*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtDataFim" class="maskedInput" />
                                    </div>
                                </div>
                            </div>
                            <div id="divExportar" class="fieldzao">
                                <div class="buttons" style="margin-left: 5px;">
                                    <input type="button" id="btnGerarArquivoEmpresas" value="Exportar Empresas" />
                                    <input type="button" id="btnGerarArquivoEmissoes" value="Exportar Emissões" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
