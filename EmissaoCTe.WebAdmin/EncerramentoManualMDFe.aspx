<%@ Page Title="Encerramento de MDF-e Manual" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="EncerramentoManualMDFe.aspx.cs" Inherits="EmissaoCTe.WebAdmin.EncerramentoManualMDFe" %>

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
    <script type="text/javascript">
        $(document).ready(function () {
            CarregarConsultaDeEmpresas("btnBuscarEmpresa", "btnBuscarEmpresa", "A", RetornoConsultaEmpresa, true, false);
            CarregarConsultaDeLocalidades("btnBuscarLocalidade", "btnBuscarLocalidade", RetornoConsultaLocalidadeEncerramento, true, false);
            CarregarConsultaDeMDFeEncerrada("btnBuscarMDFeEncerrada", "btnBuscarMDFeEncerrada", RetornoConsultaMDFe, true, false);

            $("#txtEmpresa").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $(this).data("codigo", null);
                    }

                    e.preventDefault();
                }
            });

            $("#txtLocalidade").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                        $(this).data("codigo", null);
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $("#txtDataEncerramento").datepicker();
            $("#txtDataEncerramento").mask("99/99/9999");

            $("#txtHoraEncerramento").mask("99:99");

            $("#btnNovoEncerramento").click(function () {
                NovaConsulta();
            });

            LimparCampos();

            $("#btnEnviar").click(function () {
                EnviarEncerramento();
            });
        });

        function RetornoConsultaEmpresa(empresa) {
            $("#txtEmpresa").val(empresa.NomeFantasia);
            $("#txtEmpresa").data("codigo", empresa.Codigo);
        }

        function RetornoConsultaLocalidadeEncerramento(localidade) {
            $("#txtLocalidade").val(localidade.Descricao + " / " + localidade.UF);
            $("#txtLocalidade").data("codigo", localidade.Codigo);
        }

        function EnviarEncerramento() {
            if (ValidarDados()) {
                var dados = {
                    CodigoEmpresa: $("#txtEmpresa").data("codigo"),
                    CodigoLocalidadeEncerramento: $("#txtLocalidade").data("codigo"),
                    DataEncerramento: $("#txtDataEncerramento").val(),
                    HoraEncerramento: $("#txtHoraEncerramento").val(),
                    Chave: $("#txtChave").val(),
                    Protocolo: $("#txtProtocolo").val()
                };
                executarRest("/EncerramentoManualMDFe/EncerrarMDFe?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Encerramento realizado com sucesso!", "Sucesso");
                        LimparCampos();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção");
            }
        }

        function RetornoConsultaMDFe( mdfe ) {
            executarRest("/EncerramentoManualMDFe/ObterDetalhes?callback=?", { Codigo: mdfe.Codigo }, function (r) {
                if (r.Sucesso) {
                    // Limpa os campos se estiverm com erro
                    CampoSemErro("#txtEmpresa");
                    CampoSemErro("#txtChave");
                    CampoSemErro("#txtProtocolo");
                    CampoSemErro("#txtLocalidade");
                    $("#divMensagemAlerta").hide();

                    // Preenche campos
                    $("#txtEmpresa").val(r.Objeto.NomeDaEmpresa);
                    $("#txtChave").val(r.Objeto.ChaveMDFe);
                    $("#txtProtocolo").val(r.Objeto.Protocolo);
                    $("#txtDataEncerramento").val(r.Objeto.DataEncerramento);
                    $("#txtHoraEncerramento").val(r.Objeto.HoraEncerramento);
                    $("#txtLocalidade").val(r.Objeto.Localidade);

                    // Exibe o log
                    $("#logMDFeEncerrada").show();
                    $("#logMDFeEncerrada span").text(r.Objeto.Log);

                    // Esconde botoes de pesqusia
                    $("#btnBuscarEmpresa").parents(".fieldum").hide();
                    $("#btnBuscarLocalidade").parents(".fieldum").hide();

                    // Alterna botoes de acoes
                    $("#btnEnviar").hide();
                    $("#btnNovoEncerramento").show();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function NovaConsulta() {
            LimparCampos();

            // Limpa o log
            $("#logMDFeEncerrada").hide();
            $("#logMDFeEncerrada span").text("");

            // Exibe botoes de pesqusia
            $("#btnBuscarEmpresa").parents(".fieldum").show();
            $("#btnBuscarLocalidade").parents(".fieldum").show();

            // Alterna botoes de acoes
            $("#btnEnviar").show();
            $("#btnNovoEncerramento").hide();
        }

        function LimparCampos() {
            $("#txtEmpresa").data("codigo", null);
            $("#txtLocalidade").data("codigo", null);
            $("#txtEmpresa").val("");
            $("#txtLocalidade").val("");
            $("#txtDataEncerramento").val('');
            $("#txtHoraEncerramento").val('');
            $("#txtChave").val('');
            $("#txtProtocolo").val('');

            $("#txtDataEncerramento").val(Globalize.format(new Date(), "dd/MM/yyyy"));
            $("#txtHoraEncerramento").val(Globalize.format(new Date(), "HH:mm"));
        }

        function ValidarDados() {
            var codigoEmpresa = $("#txtEmpresa").data("codigo");
            var codigoLocalidade = $("#txtLocalidade").data("codigo");
            var dataEncerramento = $("#txtDataEncerramento").val();
            var horaEncerramento = $("#txtHoraEncerramento").val();
            var chave = $("#txtChave").val();
            var protocolo = $("#txtProtocolo").val();
            var valido = true;

            if (codigoEmpresa == null || codigoEmpresa <= 0) {
                CampoComErro("#txtEmpresa");
                valido = false;
            } else {
                CampoSemErro("#txtEmpresa");
            }

            if (codigoLocalidade == null || codigoLocalidade <= 0) {
                CampoComErro("#txtLocalidade");
                valido = false;
            } else {
                CampoSemErro("#txtLocalidade");
            }

            if (dataEncerramento == null || dataEncerramento.length <= 0) {
                CampoComErro("#txtDataEncerramento");
                valido = false;
            } else {
                CampoSemErro("#txtDataEncerramento");
            }

            if (horaEncerramento == null || horaEncerramento.length <= 0) {
                CampoComErro("#txtHoraEncerramento");
                valido = false;
            } else {
                CampoSemErro("#txtHoraEncerramento");
            }

            if (chave == null || chave.length != 44) {
                CampoComErro("#txtChave");
                valido = false;
            } else {
                CampoSemErro("#txtChave");
            }

            if (protocolo == null || protocolo.length != 15) {
                CampoComErro("#txtProtocolo");
                valido = false;
            } else {
                CampoSemErro("#txtProtocolo");
            }

            return valido;
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Encerramento Manual de MDF-e
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
                    <div id="btnBuscarMDFeEncerrada" class="default-search">
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
                        <div class="fields">
                            <div class="fieldzao">
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Empresa*:
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
                                            Chave do MDF-e*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtChave" value="" maxlength="44" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Protocolo do MDF-e*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtProtocolo" value="" maxlength="15" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Data do Encerramento*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtDataEncerramento" class="maskedInput" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Hora do Encerramento*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtHoraEncerramento" class="maskedInput" />
                                    </div>
                                </div>
                                <div class="field fieldtres">
                                    <div class="label">
                                        <label>
                                            Localidade Encerramento*:
                                        </label>
                                    </div>
                                    <div class="input">
                                        <input type="text" id="txtLocalidade" />
                                    </div>
                                </div>
                                <div class="field fieldum" style="width: 65px;">
                                    <div class="buttons">
                                        <input type="button" id="btnBuscarLocalidade" value="Buscar" />
                                    </div>
                                </div>
                                <div class="field fieldtres" style="display: none" id="logMDFeEncerrada">
                                    <div class="label">
                                        <label>Log:</label>
                                    </div>
                                    <div class="input">
                                        <span></span>
                                    </div>
                                </div>
                            </div>
                            <div id="divArquivosSelecionados" class="fieldzao">
                                <div class="buttons" style="margin-left: 5px;">
                                    <input type="button" id="btnEnviar" value="Enviar Encerramento" />
                                    <input type="button" id="btnNovoEncerramento" style="display: none" value="Novo Encerramento" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
