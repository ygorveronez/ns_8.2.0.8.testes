<%@ Page Title="Relatório de Atendimento" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="RelatorioAtendimento.aspx.cs" Inherits="EmissaoCTe.WebAdmin.RelatorioAtendimento" %>

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
        var CodigoTipoAtendimento = 0;
        var CodigoUsuario = 0;
        $(document).ready(function () {
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").mask("99/99/9999");
            $("#txtDataFinal").datepicker({ changeMonth: true, changeYear: true });
            $("#txtDataInicial").datepicker({ changeMonth: true, changeYear: true });

            $("#txtUsuario").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        CodigoUsuario = 0;
                    } else {
                        e.preventDefault();
                    }
                }
            });

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

            $("#txtTipoAtendimento").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        CodigoTipoAtendimento = 0;
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#btnGerarRelatorio").click(function () {
                GeraRelatorio();
            });

            CarregarConsultaDeEmpresas("btnBuscarEmpresa", "btnBuscarEmpresa", "A", RetornoConsultaEmpresa, true, false);
            CarregarConsultaDeTipoAtendimento("btnBuscarTipoAtendimento", "btnBuscarTipoAtendimento", RetornoConsultaTipo, true);
            CarregarConsultaDeUsuariosAdmin("btnBuscarUsuario", "btnBuscarUsuario", RetornoConsultaUsuario, true, false);
        });

        function RetornoConsultaUsuario(usuario) {
            CodigoUsuario = usuario.Codigo;
            $("#txtUsuario").val(usuario.Nome);
        }
                                        
        function RetornoConsultaEmpresa(empresa) {
            CodigoEmpresa = empresa.Codigo;
            $("#txtEmpresa").val(empresa.RazaoSocial);
        }

        function RetornoConsultaTipo(tipo) {
            CodigoTipoAtendimento = tipo.Codigo;
            $("#txtTipoAtendimento").val(tipo.Descricao);
        }

        function GeraRelatorio() {
            var dados = {
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                Situacao: $("#selSituacao").val(),
                Sistema: $("#selSistema").val(),
                Satisfacao: $("#selSatisfacao").val(),
                TipoRelatorio: $("#selTipoRelatorio").val(),
                Empresa: CodigoEmpresa,
                TipoAtendimento: CodigoTipoAtendimento,
                Usuario: CodigoUsuario,

                Arquivo: $("#selArquivo").val()
            };
            executarDownload('/Atendimento/RelatorioAtendimento', dados);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Relatório de Atendimento</h3>
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
                                        Usuário:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtUsuario" />
                                </div>
                            </div>
                            <div class="field fieldum" style="width: 65px;">
                                <div class="buttons">
                                    <input type="button" id="btnBuscarUsuario" value="Buscar" />
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
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Tipo Atendimento:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtTipoAtendimento" />
                                </div>
                            </div>
                            <div class="field fieldum" style="width: 65px;">
                                <div class="buttons">
                                    <input type="button" id="btnBuscarTipoAtendimento" value="Buscar" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Situação:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selSituacao">
                                        <option value="">Todos</option>
                                        <option value="1">Aberto</option>
                                        <option value="4">Fechado</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Satisfação:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selSatisfacao">
                                        <option value="">Todos</option>
                                        <option value="3">Ótimo</option>
                                        <option value="2">Bom</option>
                                        <option value="1">Ruim</option>
                                        <option value="4">Não Avaliado</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Satisfação:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selSistema">
                                        <option value="">Todos</option>
                                        <option value="1">MultiCTe</option>
                                        <option value="2">Embarcador</option>
                                        <option value="3">TMS</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddoi">
                                <div class="label">
                                    <label>
                                        Tipo do Relatório:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selTipoRelatorio">
                                        <option value="0">Suporte</option>
                                        <option value="1">Emissão</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fielddoi">
                                <div class="label">
                                    <label>
                                        Arquivo:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selArquivo">
                                        <option value="PDF">PDF</option>
                                        <option value="Excel" selected="selected">EXCEL</option>
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
