<%@ Page Title="Relatório de Cobranças" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioCobrancas.aspx.cs" Inherits="EmissaoCTe.WebAdmin.RelatorioCobrancas" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845DCD8080CC91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/Fancybox/jquery.fancybox.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-buttons.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-thumbs.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.filedownload.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.priceformat.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-buttons.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-thumbs.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-media.min.js" type="text/javascript"></script>
    <script defer="defer" type="text/javascript">
        var CodigoEmpresa = 0;
        $(document).ready(function () {
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");
            $("#txtDataFinal").datepicker({ changeMonth: true, changeYear: true, onSelect: DataModificada });
            $("#txtDataInicial").datepicker({ changeMonth: true, changeYear: true, onSelect: DataModificada });

            $("#txtEmpresa").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        CodigoEmpresa = 0;
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnGerarRelatorio").click(function () {
                GeraRelatorio();
            });

            CarregarConsultaDeEmpresas("btnBuscarEmpresa", "btnBuscarEmpresa", "A", RetornoConsultaEmpresa, true, false);
        });
        function RetornoConsultaEmpresa(empresa) {
            CodigoEmpresa = empresa.Codigo;
            $("#txtEmpresa").val(empresa.RazaoSocial);
        }

        function DataModificada (dt) {
            // this.value = dt.substr(3);
        }

        function GeraRelatorio() {
            if (CodigoEmpresa == 0) {
                jConfirm("O relatório sem filtro por empresa por demorar alguns minutos.<br>Tem certeza que deseja gerar o relatório?", "Confirmar Relatório", function (r) {
                    if (r) IniciaDownloadRelatorio();
                })
            } else {
                IniciaDownloadRelatorio();
            }
        }

        function IniciaDownloadRelatorio() {
            var dados = {
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                Filtro: $("#selFiltro").val(),
                Arquivo: $("#selArquivo").val(),
                Layout: $("#selLayout").val(),
                Empresa: CodigoEmpresa
            };
            executarDownload('/PlanoEmissaoCTe/RelatorioCobrancas', dados);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Relatório de Cobranças</h3>
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
                            <div class="field fielddois">
                                <div class="label">
                                    Será considerado apenas o mês e ano
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Data Inicial:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDataInicial" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Data Final:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDataFinal" />
                                </div>
                            </div>
                        </div>
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
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Filtro:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selFiltro">
                                        <option value="1">Com Emissão</option>
                                        <option value="2">Sem Emissão</option>
                                        <option value="0">Todos</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Arquivo:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selArquivo">
                                        <option value="PDF">PDF</option>
                                        <option value="Excel">EXCEL</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Layout Relatório:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selLayout">
                                        <option value="1">Modelo 1 (Por faixas de emissão)</option>
                                        <option value="2">Modelo 2 (Por documentos e por faixas)</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="buttons" style="margin-left: 5px;">
                            <input type="button" id="btnGerarRelatorio" value="Gerar Relatório" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
</asp:Content>
