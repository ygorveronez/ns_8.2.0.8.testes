<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" EnableEventValidation="false" CodeBehind="EnvioEmailEmpresas.aspx.cs" Inherits="EmissaoCTe.WebAdmin.EnvioEmailEmpresas" %>

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
    <script defer="defer" src="Scripts/jquery.priceformat.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-buttons.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-thumbs.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-media.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/plupload.full.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/jquery.plupload.queue.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/pt-br.min.js" type="text/javascript"></script>
    <script id="ScriptMensagensDeAviso" type="text/javascript">
        $(document).ready(function () {
            $("#btnEnviar").click(function (e) {

                if (uploader.files.length > 0) {
                    uploader.settings.url = 'Empresa/EnviarImagemCorpoEmail?callback=?&Codigo=0';

                    $('#divListaArquivos a').each(function () {
                        $(this).remove();
                    });

                    uploader.start();

                    e.preventDefault();

                    $.fancybox.close();
                }
                else {
                    temImagem = "";
                    EnviarEmail();
                }
            });

            $("#txtAssinatura").val("MultiSoftware - http://www.multicte.com.br/ <br /> \nFone/Fax: (49)3025-9500 <br /> \nCel.: (49)9999-8880(TIM) <br /> \nE-mail: cte@multisoftware.com.br");

            InicializarPlUpload();
            temImagem = "";
        });

        function EnviarEmail() {
            if (ValidarDados()) {
                var dados = {
                    Assinatura: encodeURIComponent($("#txtAssinatura").val()),
                    Titulo: encodeURIComponent($("#txtTitulo").val()),
                    Mensagem: encodeURIComponent($("#txtMensagem").val()),
                    RNTRCInvalida: $("#chkRNTRCInvalida").prop('checked') ? 1 : 0,
                    RespostaPara: encodeURIComponent($("#txtRespostaPara").val()),
                    EmailCopia: encodeURIComponent($("#txtEmailCopia").val()),     
                    EmailDestino: encodeURIComponent($("#txtEmailDestino").val()),                    
                    TemImagem: temImagem
                };

                executarRest("/Empresa/EnviarEmail?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Emails enviado com sucesso!", "Sucesso");
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            }
        }

        function ValidarDados() {
            var observacao = $("#txtMensagem").val();
            var titulo = $("#txtTitulo").val();

            var valido = true;

            //if (observacao != "") {
            //    CampoSemErro("#txtMensagem");
            //} else {
            //    CampoComErro("#txtMensagem");
            //    valido = false;
            //}

            if (titulo != "") {
                CampoSemErro("#txtTitulo");
            } else {
                CampoComErro("#txtTitulo");
                valido = false;
            }

            return valido;
        }

        var uploader = null;
        var documentos = new Array();
        var erros = "";
        var temImagem = "";

        function InicializarPlUpload() {
            documentos = new Array();
            erros = "";
            uploader = new plupload.Uploader({
                runtimes: 'gears,html5,flash,silverlight,browserplus',
                browse_button: 'btnSelecionarImagens',
                container: 'divArquivosSelecionados',
                max_file_size: '10000kb',
                multi_selection: false,
                flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
                silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
                filters: [{ title: "Arquivos", extensions: "jpg" }],
            });

            uploader.init();

            uploader.bind('FilesAdded', function (up, files) {
                $('#divListaArquivos').html("");

                $.each(uploader.files, function (i, file) {
                    $('#divListaArquivos').append('<div id="' + file.id + '">Arquivo selecionado: ' + file.name + ' - <b></b><a href="javascript:void(0);" onclick="RemoverArquivo(\'' + file.id + '\');">Remover</a></div>');
                });

                up.refresh();
            });

            uploader.bind('UploadProgress', function (up, file) {
                $('#' + file.id + " b").html("   (" + file.percent + "%)");
            });

            uploader.bind('Error', function (up, err) {
                $('#divListaArquivos').html("<div>Erro: " + err.code + " - " + err.message + (err.file ? ", Arquivo: " + err.file.name : "") + "</div>");

                up.refresh();
            });

            uploader.bind('FileUploaded', function (up, file, response) {
                $('#' + file.id + " b").html("   (100%)");

                var retorno = JSON.parse(response.response.replace(");", "").replace("?(", ""));
                if (retorno.Sucesso == false) {
                    erros = retorno.Erro;
                }

            });

            uploader.bind('StateChanged', function (up) {
                if (up.state != plupload.STARTED) {
                    if (erros != "") {
                        temImagem = "";
                        jAlert("Ocorreram as seguintes falhas no envio dos arquivos: <br /><br />" + erros + "<br />", "Atenção", function () {
                            $("#divListaArquivos").html("");
                            uploader.splice(0, uploader.files.length);
                            uploader.destroy();
                            InicializarPlUpload();
                        });
                    }
                    else {
                        temImagem = "SIM";
                        EnviarEmail();
                    }
                }
            });
        }

        function RemoverArquivo(id) {
            uploader.removeFile(uploader.getFile(id));
            $("#" + id).remove();
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
                <h3>Envio E-mail Empresas Ativas em Produção
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
                        <div class="fieldzao">
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        Assinatura*:
                                    </label>
                                </div>
                                <div class="input">
                                    <textarea id="txtAssinatura" rows="4" cols="10" style="width: 99.5%"></textarea>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        E-mail destino (Quando informado será enviado apenas para este e-mail.):
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEmailDestino" maxlength="200" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        E-mail resposta:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtRespostaPara" maxlength="200" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        E-mail copia:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEmailCopia" maxlength="200" />
                                </div>
                            </div>
                        </div>
                        <div class="fields">
                            <div class="field fieldtres" style="margin: 5px 0">
                                <div class="checkbox">
                                    <input type="checkbox" id="chkRNTRCInvalida">
                                    <label for="chkRNTRCInvalida">
                                        Apenas empresas com RNTRC invalida
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div id="divArquivosSelecionados">
                            <div id="divListaArquivos" class="fieldzao" style="overflow-y: scroll; height: 100px;">
                            </div>
                        </div>
                        <div class="buttons" style="margin-left: 5px;">
                            <input type="button" id="btnSelecionarImagens" value="Selecionar Imagens" />
                            <input type="button" id="btnEnviar" value="Enviar" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
