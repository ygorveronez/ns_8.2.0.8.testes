<%@ Page Title="Cadastro de Ajuda" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="CadastroAjuda.aspx.cs" Inherits="EmissaoCTe.WebAdmin.CadastroAjuda" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/plupload/jquery.plupload.queue.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-buttons.min.css" rel="stylesheet" type="text/css" />
    <link href="Styles/Fancybox/jquery.fancybox-thumbs.min.css" rel="stylesheet" type="text/css" />

    <script defer="defer" src="Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.datatables.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-buttons.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-thumbs.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/Fancybox/jquery.fancybox-media.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Ajax.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.GridView.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Consulta.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTe.Base.Consultas.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/CTE.Mensagens.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/validaCampos.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/json2.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/jquery.filedownload.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/plupload.full.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/jquery.plupload.queue.min.js" type="text/javascript"></script>
    <script defer="defer" src="Scripts/plupload/pt-br.min.js" type="text/javascript"></script>
    <script defer="defer" type="text/javascript">
        var EnumTipoAjuda = {
            Arquivo: 1,
            Video: 2
        }
        function stop(e) {
            if (e && e.preventDefault) e.preventDefault();
        }
        var path = (function () {
            var paths = document.location.pathname.split("/"), path = "";
            if (paths.length > 1) {
                for (var i = 0; (paths.length - 1) > i; i++) {
                    if (paths[i] != "") path += "/" + paths[i];
                }
            }

            return path;
        }());
        $(document).ready(function () {
            CarregarConsultaDeAjuda("default-search", "default-search", RetornaBusca, true, false);
            LimparCampos();
            
            $("#btnSalvar").click(function () {
                Salvar();
            });

            $("#btnCancelar").click(function () {
                Cancelar();
            });

            $("#btnArquivo").click(function () {
                if ($("body").data("Codigo") > 0)
                    AdicionarArquivo();
                else 
                    Salvar(function (r) {
                        // Habilita o botao
                        $("#btnCancelar").show();

                        // Seta as informacoes
                        $("body").data("Codigo", r.Objeto.Codigo);
                        AdicionarArquivo();
                    });
            });

            $("#btnRemover").click(function (e) {
                stop(e);
                //RemoverArquivo();
            });

            $("#btnDownload").click(function (e) {
                stop(e);
                DownloadArquivo();
            });
            
            $("#selTipo").change(function () {
                TipoModificado();
            });

            $("#txtLinkVideo").change(function () {
                VerificarLink();
            });
        });

        function DownloadArquivo() {
            // Informacoes do formulario
            var dados = {
                Codigo: $("body").data("Codigo"),
            };

            executarDownload("/Ajuda/DownloadArquivo?callback=?", dados);
        }

        /*function RemoverArquivo() {
            var dados = {
                Codigo: $("body").data("Codigo"),
            };

            executarRest("/Ajuda/RemoverArquivo?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    // Exibe mensagem
                    ExibirMensagemSucesso("Arquivo removido com sucesso!", "Sucesso");

                    $("#lblNomeArquivo").html("");
                    $("#divArquivo").hide();
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }*/

        function AdicionarArquivo() {
            var codigo = $("body").data("Codigo");
            InicializarPlUpload(codigo);
            AbrirPlUpload();
        }

        function AbrirPlUpload() {
            $.fancybox({
                href: '#divUploadArquivos',
                width: 500,
                height: 340,
                fitToView: false,
                autoSize: false,
                closeClick: false,
                closeBtn: true,
                openEffect: 'none',
                closeEffect: 'none',
                centerOnScroll: true,
                type: 'inline',
                padding: 0,
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
        }

        function InicializarPlUpload(codigo) {
            var countArquivos = 0;
            var erros = "";
            var nomeArquivo = "";
            $("#divUploadArquivos").pluploadQueue({
                runtimes: 'html5,flash,gears,silverlight,browserplus',
                url: path + '/Ajuda/InserirArquivo?callback=?&Codigo=' + codigo,
                unique_names: true,
                filters: [{ title: 'Arquivos de Ajuda', extensions: 'pdf' }],
                silverlight_xap_url: 'Scripts/plupload/plupload.silverlight.xap',
                flash_swf_url: 'Scripts/plupload/plupload.flash.swf',
                init: {
                    StateChanged: function (up) {
                        if (up.state != plupload.STARTED) {
                            if (erros != "") {
                                jAlert("Não foi possível enviar o arquivo: " + erros, "Atenção");
                            }
                            else {
                                $("#lblNomeArquivo").html(nomeArquivo);
                                $("#divArquivo").show();
                                $("#btnArquivo").hide();
                            }
                            $.fancybox.close();
                        }
                    },
                    FilesAdded: function (up, files) {
                        countArquivos += files.length;
                        if (countArquivos > 1) {
                            $(".plupload_start").css("display", "none");
                            jAlert('O sistema só permite enviar um arquivo. Remova os demais!', 'Atenção');
                        }
                    },
                    FilesRemoved: function (up, files) {
                        countArquivos -= files.length;
                        if (countArquivos <= 1) {
                            $(".plupload_start").css("display", "");
                        }
                    },
                    FileUploaded: function (up, file, response) {
                        $('#' + file.id + " b").html("   (100%)");

                        var retorno = JSON.parse(response.response.replace(");", "").replace("?(", ""));
                        if (retorno.Sucesso)
                            nomeArquivo = retorno.Objeto.Nome;
                        else
                            erros += retorno.Erro + "<br />"
                    }
                }
            });
        }

        function RetornaBusca(dados) {
            executarRest("/Ajuda/ObterDetalhes?callback=?", dados, function (r) {
                if (r.Sucesso) {
                    Editar(r.Objeto);
                } else {
                    ExibirMensagemErro(r.Erro, "Atenção");
                }
            });
        }

        function Editar(dados) {
            // Limpa os campos do formulario
            LimparCampos();

            // Habilita o botao
            $("#btnCancelar").show();

            // Seta as informacoes
            $("body").data("Codigo", dados.Codigo);
            $("#txtDescricao").val(dados.Descricao);
            $("#txtLinkVideo").val(dados.LinkVideo);
            $("#txtLog").val(dados.Log);
            $("#selTipo").val(dados.TipoAjuda);
            $("#selStatus").val(dados.Status);
            
            if (dados.Arquivo != "") {
                $("#divArquivo").show();
                $("#lblNomeArquivo").html(dados.Arquivo);
                $("#btnArquivo").hide();
            } else {
                $("#divArquivo").hide();
            }
            VerificarLink();
            TipoModificado();
        }

        function Cancelar() {
            // Limpa os campos do formulario
            LimparCampos();

            // Esconde o botao
            $("#btnCancelar").hide();
        }
        
        function Salvar(cb) {
            if (ValidarAjuda()) {
                // Informacoes do formulario
                var dados = {
                    Codigo: $("body").data("Codigo"),
                    Descricao: $("#txtDescricao").val(),
                    Tipo: $("#selTipo").val(),
                    LinkVideo: ExtrairIdYouTube(),
                    Status: $("#selStatus").val(),
                };

                executarRest("/Ajuda/Salvar?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        // Exibe mensagem
                        ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");

                        if(cb)
                            cb(r)
                        else
                            Cancelar();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            } else {
                ExibirMensagemErro("Preencha os campos obrigatórios!", "Campos Obrigatórios");
            }
        }

        function ValidarAjuda() {
            var valido = true;
            if ($("#txtDescricao").val() == "") {
                CampoComErro("#txtDescricao");
                valido = false;
            } else {
                CampoSemErro("#txtDescricao");
            }

            if ($("#selTipo").val() == EnumTipoAjuda.Video && $("#txtLinkVideo").val() == "") {
                CampoComErro("#txtLinkVideo");
                valido = false;
            } else {
                CampoSemErro("#txtLinkVideo");
            }

            return valido;
        }

        function LimparCampos() {
            $("body").data("Codigo", 0),
            $("#txtDescricao").val("");
            $("#txtLinkVideo").val("");
            $("#txtLog").val("");
            $("#selStatus").val($("#selStatus option:first").val());
            $("#selTipo").val($("#selTipo option:first").val());
            $("#btnArquivo").show();
            TipoModificado();

            $("#divArquivo").hide();
        }

        function TipoModificado() {
            var tipo = $("#selTipo").val();
            $(".tipo-ajuda").hide();
            $(".tipo-ajuda-" + tipo).show();
        }

        function ExtrairIdYouTube() {
            var regExpYouTube = /(?:youtube\.com\/(?:(?:embed\/)|(?:watch\?v=)))([A-Z]\w+)/gi;
            var link = $("#txtLinkVideo").val();
            var youTubeId = null;
            var matches = regExpYouTube.exec(link);

            if (matches != null)
                youTubeId = matches[1];
            
            return youTubeId;
        }

        function VerificarLink() {
            var youTubeId = ExtrairIdYouTube();
            
            if (youTubeId != null) {
                $("#divPreview iframe")
                    .attr("src", 'http://www.youtube.com/embed/' + youTubeId)
                    .one("load", function () {
                        $("#divPreview").show();
                    });
            } else {
                $("#divPreview").hide();
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="page-layout">
        <div id="page-content" style="min-height: 500px;">
            <div class="inner-page-title">
                <h3>Cadastro de Ajudas
                </h3>
            </div>
            <div class="content-box">
                <div class="form">
                    <div id="default-search" class="default-search">
                        Pesquisar
                    </div>
                    <div class="fields" style="margin-top:15px">
                        <div class="response-msg error ui-corner-all" id="divMensagemErro" style="display: none;">
                            <span></span>
                            <label class="mensagem"></label>
                        </div>
                        <div class="response-msg notice ui-corner-all" id="divMensagemAlerta" style="display: none;">
                            <span></span>
                            <label class="mensagem"></label>
                        </div>
                        <div class="response-msg success ui-corner-all" id="divMensagemSucesso" style="display: none;">
                            <span></span>
                            <label class="mensagem"></label>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldtres">
                                <div class="label">
                                    <label>Descrição:</label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtDescricao" />
                                </div>
                            </div>
                            <div class="field fielddois">
                                <div class="label">
                                    <label>Tipo:</label>
                                </div>
                                <div class="input">
                                    <select id="selTipo">
                                        <option value="1">Arquivo</option>
                                        <option value="2">Video</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao tipo-ajuda tipo-ajuda-1">
                            <div class="field fielddois">
                                <div class="label" id="divArquivo" style="display: none;">
                                    <label id="lblNomeArquivo"></label> - <a href="#" id="btnDownload">Download</a><!--  - <a href="#" id="btnRemover">Remover</a> -->
                                </div>
                                <div class="buttons" style="margin: -3px 0 3px 5px">
                                    <input type="button" id="btnArquivo" value="Arquivo" />
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao tipo-ajuda tipo-ajuda-2">
                            <div class="field fieldcinco">
                                <div class="label">
                                    <label>Link (YouTube):</label>
                                </div>
                                <div class="input">
                                    <input type="text" id="txtLinkVideo" />
                                </div>
                                <div id="divPreview" style="margin: 39px 5px 0 5px;display: none;">
                                    <iframe width="100%" height="390" src="#" frameborder="0" allowfullscreen></iframe>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fielddois">
                                <div class="label">
                                    <label>Status:</label>
                                </div>
                                <div class="input">
                                    <select id="selStatus">
                                        <option value="A">Ativo</option>
                                        <option value="I">Inativo</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="field fieldcinco">
                                <div class="label">
                                    <label>Log:</label>
                                </div>
                                <div class="input">
                                    <textarea id="txtLog" readonly="readonly" rows="5" cols="" style="width: 99%"></textarea>
                                </div>
                            </div>
                        </div>
                        <div class="fieldzao">
                            <div class="buttons" style="margin-left: 5px;">
                                <input type="button" id="btnSalvar" value="Salvar" />
                                <input type="button" id="btnCancelar" value="Cancelar" style="display: none;" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div style="display: none;">
        <div id="divUploadArquivos">
            Seu navegador não possui suporte para Flash, Silverlight, Gears, BrowserPlus ou HTML5.
        </div>
    </div>
</asp:Content>
