<%@ Page Title="Solicitação Envio Arquivos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="SolicitacaoEnvioArquivos.aspx.cs" Inherits="EmissaoCTe.WebAdmin.SolicitacaoEnvioArquivos" %>

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
        $(function () {
            Carregar();

            $("#txtDataInicial").datepicker();
            $("#txtDataInicial").mask("99/99/9999");
            $("#txtDataFinal").datepicker();
            $("#txtDataFinal").mask("99/99/9999");

            $("#btnAtualizar").click(function () {
                LimparCampos();
                Carregar();
            });

            $("#btnNovoEnvio").click(function () {
                LimparCampos();
                $.fancybox({
                    href: '#divEnvio',
                    width: 800,
                    height: 500,
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
                        LimparCampos();
                    }
                });
            });

            $("#btnSalvar").click(function (e) {
                e.preventDefault();
                if (ValidarCampos()) {

                    var solicitacao = {
                        Assunto: $("#txtAssunto").val(),
                        Texto: $("#txtTexto").val()
                    }

                    executarRest("/SolicitacaoEmissao/Salvar?callback=?", solicitacao, function (r) {
                        if (r.Sucesso) {
                            uploader.settings.url = 'SolicitacaoEmissao/EnviarSolicitacao?callback=?&Codigo=' + r.Objeto;
                            $('#divListaArquivos a').remove();
                            $("#btnSalvar").hide();
                            uploader.start();
                        } else {
                            jAlert(r.Erro, "Atenção!");
                        }
                    });
                }
            });

            InicializarPlUpload();
            LimparCampos();
        });



        function Carregar() {
            CriarGridView("/SolicitacaoEmissao/Consultar?callback=?&Codigo=" + $("#txtCodigoFiltro").val() + "&UsuarioEnvio=" + $("#txtUsuarioEnvioFiltro").val() + "&Transportador=" + $("#txtTransportadoraFiltro").val() + "&DataInicial=" + $("#txtDataInicial").val() + "&DataFinal=" + $("#txtDataFinal").val(), { inicioRegistros: 0 }, "tbl_ctes_table", "tbl_ctes", "tbl_paginacao_ctes", [{ Descricao: "Detalhes", Evento: AbrirDetalhes }, { Descricao: "Download Envios", Evento: DownloadEnvios }, { Descricao: "Download Retornos", Evento: DownloadRetornos }], []);
        }

        function AbrirDetalhes() {

        }

        function DownloadEnvios(solicitacao) {
            var dados = {
                Codigo: solicitacao.data.Codigo
            }
            $("#ifrDownload").attr("src", "SolicitacaoEmissao/DownloadEnvios?Codigo=" + solicitacao.data.Codigo);
        }

        function DownloadRetornos(solicitacao) {
            if (solicitacao.data.Status == 'Finalizado')
                $("#ifrDownload").attr("src", "SolicitacaoEmissao/DownloadRetornos?Codigo=" + solicitacao.data.Codigo);
            else
                jAlert("Solicitação deve estar Finalizada para download dos retornos.", "Atenção!");
        }

        var uploader = null;
        var documentos = new Array();
        var erros = false;

        function InicializarPlUpload() {
            documentos = new Array();
            erros = false;
            uploader = new plupload.Uploader({
                runtimes: 'gears,html5,flash,silverlight,browserplus',
                browse_button: 'btnSelecionarArquivos',
                container: 'divArquivosSelecionados',
                max_file_size: '10000kb',
                multi_selection: true,
                flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
                silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
                filters: [{ title: "Arquivos", extensions: "*" }],
            });

            uploader.init();

            uploader.bind('FilesAdded', function (up, files) {
                up.refresh();
                RenderizarArquivosAnexados();
            });

            uploader.bind('UploadProgress', function (up, file) {
                $('#' + file.id + " b").html("   (" + file.percent + "%)");
            });

            uploader.bind('Error', function (up, err) {
                $('#' + err.file.id + " b").html("<div>Erro: " + err.code + " - " + err.message + (err.file ? ", Arquivo: " + err.file.name : "") + "</div>");

                up.refresh();
                erros = true;
            });

            uploader.bind('FileUploaded', function (up, file, response) {
                $('#' + file.id + " b").html("   (100%)");
            });

            uploader.bind('StateChanged', function (up) {
                if (up.state != plupload.STARTED) {
                    if (erros) {
                        jAlert("Ocorreram algumas falhas no retorno dos arquivos", "Atenção");
                        $("#btnSalvar").hide();
                    }
                    else {
                        Carregar();
                        LimparCampos();
                    }
                }
            });

            uploader.bind("UploadComplete", function () {
                if (!erros) {
                    jAlert("Dados salvos com sucesso!", "Sucesso!");
                    $.fancybox.close();
                }
            });
        }

        function RenderizarArquivosAnexados() {
            $('#divListaArquivos').html("");

            for (var i in uploader.files) {
                var file = uploader.files[i];
                $('#divListaArquivos').append('<div id="' + file.id + '">Arquivo selecionado: ' + file.name + ' - <b></b><a href="javascript:void(0);" onclick="RemoverArquivo(\'' + file.id + '\');">Remover</a></div>');
            }
        }

        function RemoverArquivo(id) {
            uploader.removeFile(uploader.getFile(id));
            $("#" + id).remove();
        }

        function ValidarCampos() {
            var assunto = $("#txtAssunto").val();
            var texto = $("#txtTexto").val();

            var valido = true;

            if (assunto == null || assunto == "") {
                CampoComErro("#txtAssunto");
                jAlert("Assunto não informado.", "Atenção!");
                valido = false;
                return valido;
            } else {
                CampoSemErro("#txtAssunto");
            }

            if (texto == null || texto == "") {
                CampoComErro("#txtTexto");
                jAlert("Texto não informado.", "Atenção!");
                valido = false;
                return valido;
            } else {
                CampoSemErro("#txtTexto");
            }

            if (!uploader.files.length > 0) {
                jAlert("O total de arquivos selecionadas é inválido.", "Atenção!");
                valido = false;
            }

            return valido;
        }

        function LimparCampos() {
            // Limpa todos os arquivos já selecionados
            uploader.splice(0, uploader.files.length)

            $("#txtAssunto").val("");
            $("#txtTexto").val("");
            $("#btnSalvar").show();

            RenderizarArquivosAnexados();
        }

        function AbrirDetalhes(solicitacao) {
            LimparCampos();

            executarRest("/SolicitacaoEmissao/ObterDetalhes?callback=?", { Codigo: solicitacao.data.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#txtAssuntoDetalhes").val(r.Objeto.Assunto);
                    $("#txtTextoDetalhes").val(r.Objeto.Texto);
                    $("#txtEmpresaDetalhes").val(r.Objeto.Transportador);

                    codigoSolicitacao = solicitacao.data.Codigo;
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
                        }
                    });

                    $('#txtAssuntoDetalhes').prop('disabled', true);
                    $('#txtTextoDetalhes').attr('disabled', true);
                    $('#txtEmpresaDetalhes').attr('disabled', true);

                } else {
                    jAlert(r.Erro, "Atenção!");
                }
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Solicitação Envio Arquivos
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
                        <div class="buttons" style="margin-left: 5px; margin-bottom: 15px;">
                            <input type="button" id="btnNovoEnvio" value="Nova Solicitação" />
                        </div>
                        <div class="field fieldum">
                            <div class="label">
                                <label>
                                    Código:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtCodigoFiltro" />
                            </div>
                        </div>
                        <div class="field fieldum">
                            <div class="label">
                                <label>
                                    Usuário Envio:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtUsuarioEnvioFiltro" />
                            </div>
                        </div>
                        <div class="field fieldum">
                            <div class="label">
                                <label>
                                    Data Inicial:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtDataInicial" />
                            </div>
                        </div>
                        <div class="field fieldum">
                            <div class="label">
                                <label>
                                    Data Final:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtDataFinal" />
                            </div>
                        </div>
                        <div class="field fieldum">
                            <div class="label">
                                <label>
                                    Nome Transportador:
                                </label>
                            </div>
                            <div class="input">
                                <input type="text" id="txtTransportadoraFiltro" />
                            </div>
                        </div>
                        <div class="buttons" style="margin-left: 5px; margin-bottom: 15px; margin-top: 25px;">
                            <input type="button" id="btnAtualizar" value="Atualizar Lista" />
                        </div>
                        <div class="table" style="margin-left: 5px;">
                            <div id="tbl_ctes">
                            </div>
                            <div id="tbl_paginacao_ctes" class="pagination">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div style="display: none;">
        <div id="divEnvio" style="height: 500px;">
            <div class="content-box">
                <div class="form">
                    <div class="fields">
                        <div class="response-msg error ui-corner-all" id="divMensagemErroEnvio" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg notice ui-corner-all" id="divMensagemAlertaEnvio" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg success ui-corner-all" id="divMensagemSucessoEnvio" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddoze">
                                <div class="label">
                                    <label>
                                        <b>Assunto:</b>
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtAssunto" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao" style="margin-bottom: 10px;">
                            <div class="field fielddoze">
                                <div class="label">
                                    <label>
                                        <b>Texto:</b>
                                    </label>
                                </div>
                                <div class="input">
                                    <textarea id="txtTexto" rows="15" cols="20" style="width: 99.5%;"></textarea>
                                </div>
                            </div>
                        </div>
                        <div id="divArquivosSelecionados">
                            <div id="divListaArquivos" class="fieldzao" style="overflow-y: scroll; height: 100px;">
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="buttons">
                                <input type="button" id="btnSelecionarArquivos" value="Selecionar Arquivos" />
                                <input type="button" id="btnSalvar" value="Salvar Solicitação" />
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
                                        <b>Assunto:</b>
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtAssuntoDetalhes" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao" style="margin-bottom: 10px;">
                            <div class="field fielddoze">
                                <div class="label">
                                    <label>
                                        <b>Texto:</b>
                                    </label>
                                </div>
                                <div class="input">
                                    <textarea id="txtTextoDetalhes" rows="15" cols="20" style="width: 99.5%;"></textarea>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao" style="margin-bottom: 10px;">
                            <div class="field fieldseis">
                                <div class="label">
                                    <label>
                                        Transportador:
                                    </label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtEmpresaDetalhes" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div style="display: none;">
        <iframe id="ifrDownload" src=""></iframe>
    </div>
</asp:Content>
