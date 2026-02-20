<%@ Page Title="Solicitação Retorno Arquivos" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="SolicitacaoRetornoArquivos.aspx.cs" Inherits="EmissaoCTe.WebAdmin.SolicitacaoRetornoArquivos" %>

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

            $("#txtAssunto").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                    }

                    e.preventDefault();
                }
            });

            $("#txtTexto").keydown(function (e) {
                if (e.which != 9 && e.which != 16) {
                    if (e.which == 8 || e.which == 46) {
                        $(this).val("");
                    }

                    e.preventDefault();
                }
            });

            $("#btnAtualizar").click(function () {
                LimparCampos();
                Carregar();
            });

            $("#btnDownloadRetornos").click(function () {
                DownloadRetornos();
            });

            $("#btnSalvar").click(function (e) {
                e.preventDefault();
                if (ValidarCampos()) {

                    var solicitacao = {
                        Codigo: codigoSolicitacao,
                        CodigoEmpresa: $("#hddCodigoEmpresa").val()
                    }

                    executarRest("/SolicitacaoEmissao/SalvarRetorno?callback=?", solicitacao, function (r) {
                        if (r.Sucesso) {
                            uploader.settings.url = 'SolicitacaoEmissao/RetornarSolicitacao?callback=?&Codigo=' + r.Objeto;
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

            CarregarConsultaDeEmpresas("btnBuscarEmpresa", "btnBuscarEmpresa", "A", RetornoConsultaEmpresa, true, false);
        });

        var codigoSolicitacao = 0;

        function Carregar() {
            CriarGridView("/SolicitacaoEmissao/Consultar?callback=?&Codigo=" + $("#txtCodigoFiltro").val() + "&UsuarioEnvio=" + $("#txtUsuarioEnvioFiltro").val() + "&Transportador=" + $("#txtTransportadoraFiltro").val() + "&DataInicial=" + $("#txtDataInicial").val() + "&DataFinal=" + $("#txtDataFinal").val(), { inicioRegistros: 0 }, "tbl_ctes_table", "tbl_ctes", "tbl_paginacao_ctes", [{ Descricao: "Alocar", Evento: AlocarSolicitacao }, { Descricao: "Download Envios", Evento: DownloadEnvios }, { Descricao: "Retornos", Evento: AbrirRetornos }], []);
        }

        function AlocarSolicitacao(solicitacao) {
            if (solicitacao.data.Status == 'Pendente' || solicitacao.data.Status == 'Alocado') {
                var solicitacao = {
                    Codigo: solicitacao.data.Codigo
                }
                executarRest("/SolicitacaoEmissao/Alocar?callback=?", solicitacao, function (r) {
                    if (r.Sucesso) {
                        Carregar();
                        LimparCampos();
                    } else {
                        Carregar();
                        jAlert(r.Erro, "Atenção!");
                    }
                });
            }
            else if (solicitacao.data.Status == 'Finalizado') {
                jAlert("Solicitação já foi finalizada, não é possível alocar.", "Atenção!");
            }
            else
                if (solicitacao.data.Status == 'Alocado') {
                    Carregar();
                    jAlert("Solicitação já está alocada para " + solicitacao.data.Retorno + ".", "Atenção!");
                }
        }


        function DownloadEnvios(solicitacao) {
            var dados = {
                Codigo: solicitacao.data.Codigo
            }
            $("#ifrDownload").attr("src", "SolicitacaoEmissao/DownloadEnvios?Codigo=" + solicitacao.data.Codigo);
        }

        function DownloadRetornos() {
            $("#ifrDownload").attr("src", "SolicitacaoEmissao/DownloadRetornos?Codigo=" + $("#hddCodigoSolicitacao").val());
        }

        function AbrirRetornos(solicitacao) {
            LimparCampos();

            executarRest("/SolicitacaoEmissao/ObterDetalhes?callback=?", { Codigo: solicitacao.data.Codigo }, function (r) {
                if (r.Sucesso) {
                    $("#txtAssunto").val(r.Objeto.Assunto);
                    $("#txtTexto").val(r.Objeto.Texto);
                    $("#txtEmpresa").val(r.Objeto.Transportador);

                    codigoSolicitacao = solicitacao.data.Codigo;
                    $.fancybox({
                        href: '#divRetorno',
                        width: 800,
                        height: 580,
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

                    //$('#txtAssunto').prop('disabled', true);
                    //$('#txtTexto').prop('disabled', true);

                    if (solicitacao.data.Status == 'Finalizado') {
                        $("#btnDownloadRetornos").show();
                        $("#lblArquivos").hide();
                        $("#btnSelecionarArquivos").hide();
                        $("#divArquivosSelecionados").hide();
                        $("#txtEmpresa").prop('disabled', true);
                        $("#btnSalvar").hide();
                        $("#hddCodigoSolicitacao").val(solicitacao.data.Codigo);
                    }
                    else if (solicitacao.data.Status == 'Alocado') {
                        $("#btnDownloadRetornos").hide();
                        $("#btnBuscarEmpresa").show();
                        $("#lblArquivos").show();
                        $("#btnSelecionarArquivos").show();
                        $("#divArquivosSelecionados").show();
                        $("#btnSalvar").show();
                        $("#txtEmpresa").prop('disabled', false);
                        $("#hddCodigoSolicitacao").val("");
                    }
                    else {
                        jAlert("Solicitação não foi alocada!", "Atenção!");
                        $("#btnDownloadRetornos").show();
                        $("#lblArquivos").hide();
                        $("#btnSelecionarArquivos").hide();
                        $("#divArquivosSelecionados").hide();
                        $("#txtEmpresa").prop('disabled', false);
                        $("#btnSalvar").hide();
                        $("#hddCodigoSolicitacao").val("");
                    }

                } else {
                    jAlert(r.Erro, "Atenção!");
                }
            });
        }

        function RetornoConsultaEmpresa(empresa) {
            $("#hddCodigoEmpresa").val(empresa.Codigo);
            $("#txtEmpresa").val(empresa.RazaoSocial);
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

            uploader.bind('FilesAdded', function (up) {
                up.refresh();
                RenderizarArquivosAnexados();
            });

            uploader.bind('UploadProgress', function (up, file) {
                $('#' + file.id + " b").html("  (" + file.percent + "%)");
            });

            uploader.bind('Error', function (up, err) {
                $('#' + err.file.id + " b").html("<div>Erro: " + err.code + " - " + err.message + (err.file ? ", Arquivo: " + err.file.name : "") + "</div>");

                up.refresh();
                erros = true;
            });

            uploader.bind('FileUploaded', function (up, file, response) {
                $('#' + file.id + " b").html("(100%)");
            });

            uploader.bind("UploadComplete", function () {
                if (!erros) {
                    jAlert("Dados salvos com sucesso!", "Sucesso!");
                    $.fancybox.close();
                }
            });

            uploader.bind('StateChanged', function (up) {
                if (up.state != plupload.STARTED) {
                    if (erros) {
                        jAlert("Ocorreram algumas falhas no retorno dos arquivos", "Atenção");
                        $("#btnSalvar").hide();
                    } else {
                        Carregar();
                        LimparCampos();
                    }
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
            RenderizarArquivosAnexados();
        }

        function ValidarCampos() {
            var transportador = $("#hddCodigoEmpresa").val();

            var valido = true;

            if (transportador == null || transportador == "" || transportador == "0") {
                CampoComErro("#txtEmpresa");
                jAlert("Transportador não informado.", "Atenção!");
                valido = false;
                return valido;
            } else {
                CampoSemErro("#txtEmpresa");
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

            $("#hddCodigoEmpresa").val("0");
            $("#txtEmpresa").val("");
            $("#txtAssunto").val("");
            $("#txtTexto").val("");
            codigoSolicitacao = 0;
            $("#btnSalvar").show();

            RenderizarArquivosAnexados();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="display: none;">
        <asp:HiddenField ID="hddCodigoEmpresa" Value="0" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hddCodigoSolicitacao" Value="0" runat="server" ClientIDMode="Static" />
    </div>
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Solicitação Retorno Arquivos
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
                        <div class="buttons" style="margin-left: 5px; margin-bottom: 15px;">
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
        <div id="divRetorno" style="height: 500px;">
            <div class="content-box">
                <div class="form">
                    <div class="fields">
                        <div class="response-msg error ui-corner-all" id="divMensagemErroRetorno" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg notice ui-corner-all" id="divMensagemAlertaRetorno" style="display: none;">
                            <span></span>
                            <label class="mensagem">
                            </label>
                        </div>
                        <div class="response-msg success ui-corner-all" id="divMensagemSucessoRetorno" style="display: none;">
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
                                    <textarea id="txtTexto" rows="11" cols="20" style="width: 99.5%;"></textarea>
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
                            <div class="buttons">
                                <input type="button" id="btnSelecionarArquivos" value="Selecionar Arquivos" />
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="buttons">
                                <input type="button" id="btnDownloadRetornos" value="Download Retornos" />
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddoze">
                                <div id="lblArquivos" class="label">
                                    <label>
                                        <b>Arquivos Retorno:</b>
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div id="divArquivosSelecionados">
                            <div id="divListaArquivos" class="fieldzao" style="overflow-y: scroll; height: 100px;">
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="buttons">
                                <input type="button" id="btnSalvar" value="Salvar Solicitação" />
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
