<%@ Page Title="Importação de Pré CT-es" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="ImportacaoDePreCTe.aspx.cs" Inherits="EmissaoCTe.WebAdmin.ImportacaoDePreCTe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/ui/ui.datepicker.min.css" rel="stylesheet" />
    <style type="text/css">
        .floatingMenu {
            position: absolute;
            width: 150px;
            z-index: 200;
        }

            .floatingMenu ul li {
                padding: 0;
            }

                .floatingMenu ul li a:hover {
                    background-color: #FFFFEA;
                }

                .floatingMenu ul li a {
                    display: block;
                    border: 1px solid #000;
                    background-color: #F6F6F6;
                    text-decoration: none;
                    border-bottom: none;
                    color: #0B6ABA;
                    padding: 5px 5px 5px 15px;
                    cursor: pointer;
                }

            .floatingMenu .first a {
                font-weight: bold;
                cursor: default;
                color: #000 !important;
                background-color: #DEDEDE !important;
                -webkit-border-top-left-radius: 6px 6px;
                -webkit-border-top-right-radius: 6px 6px;
                -moz-border-top-left-radius: 6px 6px;
                border-top-left-radius: 6px 6px;
                -moz-border-top-right-radius: 6px 6px;
                border-top-right-radius: 6px 6px;
            }

            .floatingMenu .last a {
                border-bottom: 1px solid #000 !important;
                -webkit-border-bottom-left-radius: 6px 6px;
                -webkit-border-bottom-right-radius: 6px 6px;
                -moz-border-bottom-left-radius: 6px 6px;
                border-bottom-left-radius: 6px 6px;
                -moz-border-bottom-right-radius: 6px 6px;
                border-bottom-right-radius: 6px 6px;
            }
    </style>
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
    <script defer="defer" src="Scripts/ui/ui.datepicker.min.js"></script>
    <script defer="defer" src="Scripts/jquery.ui.datepicker-pt-BR.min.js" type="text/javascript"></script>
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {
            $("#txtDataInicial").datepicker({});
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").datepicker({});
            $("#txtDataFinal").mask("99/99/9999");
            $("#txtNumeroInicial").mask("9?99999999999");
            $("#txtNumeroFinal").mask("9?99999999999");
            $("#btnFiltrarPreCTes").click(function () {
                AtualizarGridPreCTes();
            });
            AtualizarGridPreCTes();
        });
        function AtualizarGridPreCTes() {
            var dados = {
                Empresa: $("#txtEmpresa").val(),
                DataInicial: $("#txtDataInicial").val(),
                DataFinal: $("#txtDataFinal").val(),
                NumeroInicial: $("#txtNumeroInicial").val(),
                NumeroFinal: $("#txtNumeroFinal").val(),
                Finalidade: $("#selFinalidade").val(),
                Status: $("#selStatus").val(),
                inicioRegistros: 0
            };

            CriarGridView("/ImportacaoPreCTe/Consultar?callback=?", dados, "tbl_pre_ctes_table", "tbl_pre_ctes", "tbl_paginacao_pre_ctes", [{ Descricao: "Opções", Evento: AbrirMenu }], [0, 1]);
        }
        function AcessarSistema() {
            var cte = JSON.parse($("#hddCTeMenu").val());

            var win = window.open();
            executarRest("/Empresa/BuscarDadosParaAcesso?callback=?", { CodigoEmpresa: cte.CodigoEmpresa }, function (r) {
                if (r.Sucesso) {
                    var uriAcesso = r.Objeto.UriAcesso + "?x=" + r.Objeto.Login + "&y=" + r.Objeto.Senha + "&z=" + r.Objeto.Usuario;
                    win.location = uriAcesso;
                    win.focus();
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }
        function EmitirCTe() {
            var cte = JSON.parse($("#hddCTeMenu").val());

            executarRest("/ConhecimentoDeTransporteEletronico/Emitir?callback=?", { CodigoCTe: cte.Codigo, FormaEmissao: 1 }, function (r) {
                if (r.Sucesso) {
                    jAlert("CT-e emitido com sucesso!", "Sucesso");

                    AtualizarGridPreCTes();
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }
        function FinalizarImportacao() {
            var cte = JSON.parse($("#hddCTeMenu").val());

            executarRest("/ImportacaoPreCTe/FinalizarImportacao?callback=?", { CodigoCTe: cte.Codigo }, function (r) {
                if (r.Sucesso) {
                    jAlert("CT-e transmitido e finalizado com sucesso!", "Sucesso");

                    AtualizarGridPreCTes();
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }

    </script>
    <script defer="defer" id="ScriptMenu" type="text/javascript">
        var timeKeeper;
        $(document).ready(function () {
            $('#divMenuCTe ul').mouseenter(function () {
                clearTimeout(timeKeeper);
            });
            $('#divMenuCTe ul').mouseleave(function () {
                timeKeeper = setTimeout(function () { $('#divMenuCTe').slideUp(200); $("#hddCTeMenu").val(''); }, 300);
            });
            $('#divMenuCTe').attr('tabIndex', -1);
        });
        function AbrirMenu(cte) {
            clearTimeout(timeKeeper);
            $("#hddCTeMenu").val(JSON.stringify(cte.data));
            $("#divMenuCTe").css("top", cte.pageY + 3);
            $("#divMenuCTe").css("left", cte.clientX - 140);
            $("#divMenuCTe").slideDown();
            timeKeeper = setTimeout(function () { $('#divMenuCTe').slideUp(200); $("#hddCTeMenu").val(''); }, 1500);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCTeMenu" value="" />
    </div>
    <div id="divMenuCTe" class="floatingMenu" style="display: none;">
        <ul>
            <li class='first'><a>Opções</a></li>
            <li><a href="javascript:void(0);" onclick="EmitirCTe();">Emitir CT-e</a></li>
            <li><a href="javascript:void(0);" onclick="FinalizarImportacao();">Transferir e Finalizar</a></li>
            <li class='last'><a href="javascript:void(0);" onclick="AcessarSistema();">Acessar Sistema</a></li>
        </ul>
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Importação de Pré CT-e's
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
                    <div class="fields">
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
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        Empresa:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEmpresa" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Data Inicial:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDataInicial" />
                                </div>
                            </div>
                            <div class="field fieldtres">
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
                                        Número Inicial:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtNumeroInicial" />
                                </div>
                            </div>
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Número Final:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtNumeroFinal" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Finalidade:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selFinalidade" class="select">
                                        <option value="">Todas</option>
                                        <option value="0">Normal</option>
                                        <option value="1">Complemento</option>
                                        <option value="2">Anulação</option>
                                        <option value="3">Substituto</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Status:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selStatus" class="select">
                                        <option value="">Todos</option>
                                        <option value="A">Autorizado</option>
                                        <option value="C">Cancelado</option>
                                        <option value="D">Denegado</option>
                                        <option value="S">Em Digitação</option>
                                        <option value="E">Enviado</option>
                                        <option value="I">Inutilizado</option>
                                        <option value="P">Pendente</option>
                                        <option value="R">Rejeição</option>
                                        <option value="K">Em Cancelamento</option>
                                        <option value="L">Em Inutilização</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao" style="margin-left: 5px;">
                            <div class="field fieldseis">
                                <div class="buttons">
                                    <input type="button" id="btnFiltrarPreCTes" value="Buscar / Atualizar Pr-é CT-e's" style="margin-top: 0;" />
                                </div>
                            </div>
                        </div>
                        <div class="table" style="margin-left: 5px;">
                            <div id="tbl_pre_ctes">
                            </div>
                            <div id="tbl_paginacao_pre_ctes" class="pagination">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
