<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" EnableEventValidation="false" CodeBehind="Recado.aspx.cs" Inherits="EmissaoCTe.WebAdmin.Recado" %>

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
    <script id="ScriptMensagensDeAviso" type="text/javascript">
        $(document).ready(function () {
            $("#txtDataLancamento").mask("99/99/9999");
            $("#txtDataLancamento").datepicker();

            CarregarConsultaDeRecado("default-search", "default-search", RetornoConsultaRecado, true, false);

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });

            LimparCampos();
        });

        function RetornoConsultaRecado(recado) {
            //var dados = {
            //    Codigo: recado.Codigo,
            //};

            //executarRest("/Recado/ObterDetalhes?callback=?", dados, function (r) {
            //    if (r.Sucesso) {
            //        LimparCampos();
            //        $("#hddCodigo").val(r.Objeto.Codigo);
            //        $("#selAtivo").val(r.Objeto.Ativo);
            //        $("#txtMensagem").val(r.Objeto.Descricao);
            //        $("#txtTitulo").val(r.Objeto.Titulo);
            //        $("#txtDataLancamento").val(r.Objeto.DataLancamento);
            //        $("#txtUsuario").val(r.Objeto.Usuario);

            //    } else {
            //        ExibirMensagemErro(r.Erro, "Atenção");
            //    }
            //});

            $("#hddCodigo").val(recado.Codigo);
            $("#selAtivo").val(recado.Ativo);
            $("#txtMensagem").val(recado.Descricao);
            $("#txtTitulo").val(recado.Titulo);
            $("#txtDataLancamento").val(recado.DataLancamento);
            $("#txtUsuario").val(recado.Usuario);

            CriarGridView("/Recado/ConsultarUsuariosRecados?callback=?", {
                inicioRegistros: 0,
                Codigo: recado.Codigo,
            }, "tbl_usuarios_table", "tbl_usuarios", "tbl_paginacao_usuarios", null, null, null, false);

        }

        function Salvar() {
            if (ValidarDados()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    Mensagem: encodeURIComponent($("#txtMensagem").val()),
                    Titulo: $("#txtTitulo").val(),
                    Ativo: $("#selAtivo").val()
                };

                executarRest("/Recado/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            }
        }

        function LimparCampos() {
            $("#hddCodigo").val("0");
            $("#txtMensagem").val("");
            $("#txtTitulo").val("");
            $("#selAtivo").val($("#selAtivo option:first").val());
            $("#txtUsuario").val("");
            $("#txtDataLancamento").val("");

            CriarGridView("/Recado/ConsultarUsuariosRecados?callback=?", {
                inicioRegistros: 0,
                Codigo: 0,
            }, "tbl_usuarios_table", "tbl_usuarios", "tbl_paginacao_usuarios", null, null, null, false);

            executarRest("/Recado/BuscarUsuario?callback=?", {}, function (r) {
                if (r.Sucesso) {
                    $("#txtDataLancamento").val(r.Objeto.DataLancamento);
                    $("#txtUsuario").val(r.Objeto.Usuario);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function ValidarDados() {
            var titulo = $("#txtTitulo").val();
            var observacao = $("#txtMensagem").val();

            var valido = true;

            if (observacao != "") {
                CampoSemErro("#txtMensagem");
            } else {
                CampoComErro("#txtMensagem");
                valido = false;
            }

            if (titulo != "") {
                CampoSemErro("#txtTitulo");
            } else {
                CampoComErro("#txtTitulo");
                valido = false;
            }

            return valido;
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
                <h3>Cadastro de Recados
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
                    <div id="default-search" class="default-search">
                        Pesquisar
                    </div>
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
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        Título*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtTitulo" maxlength="200" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        Mensagem*:
                                    </label>
                                </div>
                                <div class="input">
                                    <textarea id="txtMensagem" rows="10" cols="10" style="width: 99.5%"></textarea>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao" style="margin-bottom: 15px;">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Data Lançamento:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDataLancamento" value="" class="maskedInput" disabled />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Usuário:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtUsuario" value="" class="maskedInput" disabled />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Status*:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selAtivo" class="select">
                                        <option value="true">Ativo</option>
                                        <option value="false">Inativo</option>
                                    </select>
                                </div>
                            </div>
                            <div class="fields">
                                <div class="table" style="width: 860px; margin-left: 5px;">
                                    <div id="tbl_usuarios">
                                    </div>
                                    <div id="tbl_paginacao_usuarios" class="pagination">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="buttons" style="margin-left: 5px;">
                            <input type="button" id="btnSalvar" value="Salvar" />
                            <input type="button" id="btnCancelar" value="Cancelar" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
