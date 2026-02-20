<%@ Page Title="Relatório de MDF-es por Embarcador" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioMDFesEmitidosPorEmbarcador.aspx.cs" Inherits="EmissaoCTe.WebAdmin.RelatorioMDFesEmitidosPorEmbarcador" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845DCD8080CC91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/Fancybox/jquery.fancybox.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-buttons.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-thumbs.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" type="text/css" />
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
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
        $(document).ready(function () {
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");
            $("#txtDataAutorizacaoInicial").mask("99/99/9999");
            $("#txtDataAutorizacaoFinal").mask("99/99/9999");
            $("#txtDataFinal").datepicker({ changeMonth: true, changeYear: true });
            $("#txtDataInicial").datepicker({ changeMonth: true, changeYear: true });
            $("#txtDataAutorizacaoInicial").datepicker({ changeMonth: true, changeYear: true });
            $("#txtDataAutorizacaoFinal").datepicker({ changeMonth: true, changeYear: true });

            $("#txtEmpresa").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoEmpresa").val("0");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtEmbarcador").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $("#hddCodigoEmbarcador").val("");
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnGerarRelatorioJS").click(function () {
                $.fancybox({
                    href: '#divRelatorio',
                    width: 800,
                    height: 700,
                    fitToView: false,
                    autoSize: false,
                    closeClick: false,
                    closeBtn: true,
                    openEffect: 'none',
                    closeEffect: 'none',
                    centerOnScroll: true,
                    type: 'inline',
                    padding: 7,
                    scrolling: 'no',
                    helpers: {
                        overlay: {
                            css: {
                                cursor: 'auto'
                            },
                            closeClick: false
                        }
                    }
                });

                $("#btnGerarRelatorio").trigger("click");
            });

            CarregarConsultaDeEmpresas("btnBuscarEmpresa", "btnBuscarEmpresa", "A", RetornoConsultaEmpresa, true, false);
            CarregarConsultadeClientes("btnBuscarEmbarcador", "btnBuscarEmbarcador", RetornoConsultaEmbarcador, true, false, "");
        });
        function RetornoConsultaEmpresa(empresa) {
            $("#hddCodigoEmpresa").val(empresa.Codigo);
            $("#txtEmpresa").val(empresa.RazaoSocial);
        }
        function RetornoConsultaEmbarcador(embarcador) {
            $("#hddCodigoEmbarcador").val(embarcador.CPFCNPJ);
            $("#txtEmbarcador").val(embarcador.CPFCNPJ + " - " + embarcador.Nome);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <asp:HiddenField ID="hddCodigoEmpresa" Value="0" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hddCodigoEmbarcador" Value="" runat="server" ClientIDMode="Static" />
        <asp:Button ID="btnGerarRelatorio" runat="server" OnClick="btnGerarRelatorio_Click" ClientIDMode="Static" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Relatório de MDF-es por Embarcador
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
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Transportador:
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
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Embarcador dos CT-e(s):
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEmbarcador" />
                                </div>
                            </div>
                            <div class="field fieldum" style="width: 65px;">
                                <div class="buttons">
                                    <input type="button" id="btnBuscarEmbarcador" value="Buscar" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Todos os CNPJ da Raiz:
                                    </label>
                                </div>
                                <div class="input">
                                    <asp:DropDownList ID="selTodosCNPJRaiz" CssClass="select" runat="server" ClientIDMode="Static">
                                        <asp:ListItem Value="false" Text="Não"></asp:ListItem>
                                        <asp:ListItem Value="true" Text="Sim"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Data Emissão Inicial:
                                    </label>
                                </div>
                                <div class="input">
                                    <asp:TextBox ID="txtDataInicial" runat="server" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Data Emissão Final:
                                    </label>
                                </div>
                                <div class="input">
                                    <asp:TextBox ID="txtDataFinal" runat="server" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="buttons" style="margin-left: 5px;">
                            <input type="button" id="btnGerarRelatorioJS" value="Gerar Relatório" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div style="display: none;">
        <div id="divRelatorio">
            <asp:UpdatePanel ID="uppRelatorio" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <rsweb:ReportViewer ID="rvwRelatorioMDFesEmitidosPorEmbarcador" runat="server" AsyncRendering="true" Width="800" Height="700" ExportContentDisposition="AlwaysInline" EnableTheming="true" PromptAreaCollapsed="true" ShowFindControls="false"
                        ShowWaitControlCancelLink="false" ZoomMode="PageWidth" ShowZoomControl="false">
                        <LocalReport ReportPath="Relatorios/RelatorioMDFesEmitidosPorEmbarcador.rdlc">
                        </LocalReport>
                    </rsweb:ReportViewer>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="rvwRelatorioMDFesEmitidosPorEmbarcador" />
                    <asp:AsyncPostBackTrigger ControlID="btnGerarRelatorio" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
