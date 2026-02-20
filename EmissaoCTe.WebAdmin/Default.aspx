<%@ Page Title="Home" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="EmissaoCTe.WebAdmin.Default" MasterPageFile="Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/Fancybox/jquery.fancybox.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-buttons.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-thumbs.min.css" rel="stylesheet" type="text/css" />
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
    <script defer="defer" type="text/javascript">
        $(document).ready(function () {

            $("#txtTitulo").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    e.preventDefault();
                }
            });

            $("#txtMensagem").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    e.preventDefault();
                }
            });

            $("#btnFiltrar").click(function () {
                AtualizarGrid();
            });

            $("#btnFiltrarRecados").click(function () {
                AtualizarGridRecados();
            });

            $("#btnConfirmarLeitura").click(function () {
                ConfirmarLeitura();
            });

            LimparCamposDetalhes();
            AtualizarGrid();
            AtualizarGridRecados();
        });
        function AtualizarGrid() {
            CriarGridView("/ConhecimentoDeTransporteEletronico/ConsultarPendentesCancelamento?callback=?", { inicioRegistros: 0 }, "tbl_ctes_cancelamento_table", "tbl_ctes_cancelamento", "tbl_paginacao_ctes_cancelamento", [{ Descricao: "Acessar", Evento: AcessarSistema }], [0, 1]);
        }

        function AcessarSistema(cte) {
            executarRest("/Empresa/BuscarDadosParaAcesso?callback=?", { CodigoEmpresa: cte.data.CodigoEmpresa }, function (r) {
                if (r.Sucesso) {
                    var win = window.open();
                    var uriAcesso = "http://" + window.location.host + "/" + r.Objeto.UriAcesso + "?x=" + r.Objeto.Login + "&y=" + r.Objeto.Senha + "&z=" + r.Objeto.Usuario;
                    win.location = uriAcesso;
                    win.focus();
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }

        function AtualizarGridRecados() {
            BuscarRecadosPendentes();
            CriarGridView("/Recado/ConsultarRecadosUsuario?callback=?", { inicioRegistros: 0, Situacao: $("#selSituacao").val(), Titulo: $("#txtTituloRecado").val(), Mensagem: $("#txtMensagemRecado").val() }, "tbl_recados_table", "tbl_recados", "tbl_paginacao_recados", [{ Descricao: "Detalhar", Evento: Detalhar }], [0, 1]);
        }

        function Detalhar(recado) {
            $("#hddCodigo").val(recado.data.Codigo);
            $("#txtTitulo").val(recado.data.Titulo);
            $("#txtMensagem").val(recado.data.Descricao);

            //codigoSolicitacao = recado.Codigo;
            $.fancybox({
                href: '#divDetalhes',
                width: 800,
                height: 380,
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
                },
                afterClose: function () {
                    LimparCamposDetalhes();
                }
            });

        }

        function LimparCamposDetalhes() {
            $("#hddCodigo").val("0");
            $("#txtTitulo").val("");
            $("#txtMensagem").val("");
        }

        function ConfirmarLeitura() {
            executarRest("/Recado/ConfirmarLeitura?callback=?", { Codigo: $("#hddCodigo").val() }, function (r) {
                if (r.Sucesso) {
                    $.fancybox.close();
                    AtualizarGridRecados();
                } else {
                    jAlert(r.Erro, "Atenção");
                }
            });
        }

        function BuscarRecadosPendentes() {
            var id = "divRecadosPendentes";
            $("#" + id).slideUp();
            executarRest("/Recado/BuscarRecadosPendentesUsuario?callback=?", { Codigo: 0 }, function (r) {
                if (r.Sucesso) {
                    if (r.Objeto.MensagemAviso != "") {
                        $("#" + id + " label").text(r.Objeto.MensagemAviso);
                        $("#" + id + " span").text("");
                        $("#" + id).slideDown();
                    }
                }
            });
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <input type="hidden" id="hddCodigo" value="0" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Home
                </h3>
            </div>
            <div class="content-box" style="min-height: 100px;">
                <div class="form">
                    <div class="response-msg notice ui-corner-all" id="divRecadosPendentes" style="display: none;">
                        <span></span>
                        <label class="mensagem">
                        </label>
                    </div>
                    <div class="fields">
                        <div class="fieldzao">
                            <div class="">
                                <h3>Recados
                                </h3>
                            </div>
                        </div>
                    </div>
                    <div class="fields">
                        <div class="fieldzao">
                            <div class="field fieldum">
                                <div class="label">
                                    <label>
                                        Situação:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selSituacao" class="select">
                                        <option value="P">Pendentes</option>
                                        <option value="L">Lidos</option>
                                        <option value="T">Todos</option>
                                    </select>
                                </div>
                            </div>
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Titulo:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtTituloRecado" />
                                </div>
                            </div>
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>
                                        Mensagem:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtMensagemRecado" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="buttons">
                                    <input type="button" id="btnFiltrarRecados" value="Atualizar Lista" />
                                </div>
                            </div>
                        </div>
                        <div class="table" style="margin-left: 5px;">
                            <div id="tbl_recados">
                            </div>
                            <div id="tbl_paginacao_recados" class="pagination">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="content-box" style="min-height: 100px;">
                <div class="form">
                    <div class="fields">
                        <div class="fieldzao">
                            <div class="">
                                <h3>CT-es Pendentes de Cancelamento
                                </h3>
                            </div>
                        </div>
                    </div>
                    <div class="fields">
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="buttons">
                                    <input type="button" id="btnFiltrar" value="Atualizar Lista" />
                                </div>
                            </div>
                        </div>
                        <div class="table" style="margin-left: 5px;">
                            <div id="tbl_ctes_cancelamento">
                            </div>
                            <div id="tbl_paginacao_ctes_cancelamento" class="pagination">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div style="display: none;">
        <div id="divDetalhes" style="height: 500px;">
            <div class="content-box">
                <div class="form">
                    <div class="fields">
                        <div class="fieldzao">
                            <div class="field fielddoze">
                                <div class="label">
                                    <label>
                                        <b>Titulo:</b>
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtTitulo" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao" style="margin-bottom: 10px;">
                            <div class="field fielddoze">
                                <div class="label">
                                    <label>
                                        <b>Mensagem:</b>
                                    </label>
                                </div>
                                <div class="input">
                                    <textarea id="txtMensagem" rows="15" cols="20" style="width: 99.5%;"></textarea>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="buttons">
                                    <input type="button" id="btnConfirmarLeitura" value="Confirmar Leitura" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
