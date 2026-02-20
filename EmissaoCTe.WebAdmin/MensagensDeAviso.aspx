<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" EnableEventValidation="false" CodeBehind="MensagensDeAviso.aspx.cs" Inherits="EmissaoCTe.WebAdmin.MensagensDeAviso" %>

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
            $("#txtDataInicio").mask("99/99/9999");
            $("#txtDataInicio").datepicker();
            $("#txtDataFinal").mask("99/99/9999");
            $("#txtDataFinal").datepicker();

            CarregarConsultaDeMensagensDeAviso("default-search", "default-search", RetornoConsultaMensagensDeAviso, true, false);

            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                LimparCampos();
            });
        });

        function RetornoConsultaMensagensDeAviso(mensagem) {
            $("#hddCodigo").val(mensagem.Codigo);
            $("#selStatus").val(mensagem.Status);
            $("#txtMensagem").val(mensagem.Descricao);
            $("#txtTitulo").val(mensagem.Titulo);
            $("#txtDataInicio").val(mensagem.DataInicial);
            $("#txtDataFinal").val(mensagem.DataFinal);
        }

        function Salvar() {
            if (ValidarDados()) {
                var dados = {
                    Codigo: $("#hddCodigo").val(),
                    Mensagem: encodeURIComponent($("#txtMensagem").val()),
                    Titulo: $("#txtTitulo").val(),
                    Status: $("#selStatus").val(),
                    DataInicial: $("#txtDataInicio").val(),
                    DataFinal: $("#txtDataFinal").val()
                };

                executarRest("/MensagemDeAviso/Salvar?callback=?", dados, function (r) {
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
            $("#selStatus").val($("#selStatus option:first").val());
            $("#txtDataInicio").val("");
            $("#txtDataFinal").val("");
        }

        function ValidarDados() {
            var titulo = $("#txtTitulo").val();
            var observacao = $("#txtMensagem").val();
            var dataInicial = $("#txtDataInicio").val();
            var dataFinal = $("#txtDataFinal").val();

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

            if (dataInicial != "") {
                CampoSemErro("#txtDataInicio");
            } else {
                CampoComErro("#txtDataInicio");
                valido = false;
            }

            if (dataFinal != "") {
                CampoSemErro("#txtDataFinal");
            } else {
                CampoComErro("#txtDataFinal");
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
                <h3>Cadastro de Mensagens de Aviso
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
                        <div class="fieldzao" style="margin-bottom: 15px;">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Data Inicial*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDataInicio" value="" class="maskedInput" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Data Final*:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDataFinal" value="" class="maskedInput" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>
                                        Status*:
                                    </label>
                                </div>
                                <div class="input">
                                    <select id="selStatus" class="select">
                                        <option value="A">Ativo</option>
                                        <option value="I">Inativo</option>
                                    </select>
                                </div>
                            </div>
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
                                    <textarea id="txtMensagem" rows="4" cols="10" style="width: 99.5%"></textarea>
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
