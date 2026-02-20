$(document).ready(function () {
    'use strict';
    var DATA = null;
    var TIPO = "";
    var importador = null;
    var uploader;
    var tabela = $(".main-table");


    // Eventos
    $("#btnIniciaProcesso").on('click', function (e) {
        if (e && e.preventDefault) e.preventDefault();

        AbreModalImportacao();
        LimpaImportacao(false);
    });
    $("#btnConverter").on('click', function (e) {
        if (e && e.preventDefault) e.preventDefault();

        if (ValidaTabela())
            AbreModalConvercao();
    });
    $(".modal-tipo-importacao").on('click', '.tipo-importacao', function (e) {
        if (e && e.preventDefault) e.preventDefault();

        SelecionaImportacao($(this));
    });
    $(".modal-tipo-importacao").on('click', '.arrow-right', function (e) {
        if (e && e.preventDefault) e.preventDefault();

        AbreModalSelecioneArquivo();
    });
    $(".modal-upload-arquivo").on('click', '.arrow-left', function (e) {
        if (e && e.preventDefault) e.preventDefault();

        AbreModalImportacao();
    });
    $(".modal-upload-arquivo").on('click', '.processar-arquivo', function (e) {
        if (e && e.preventDefault) e.preventDefault();

        ProcesaArquivo();
    });
    $(".modal-result").on('hidden.bs.modal', function () {
        $(".modal-result").find('.btn').attr('disabled', true);
        $(".modal-result").find('#carregandoImportacao, #mensagemRetorno, #errosRetorno').hide();
    });

    $(".modal-result").on('click', '.reimportardados', function () {
        $(".modal-result").modal('hide');
        importador.refresh(DATA);
    });


    // Metodos
    function AbreModalImportacao() {
        $(".modal-tipo-importacao").modal('show');
        $(".modal-upload-arquivo").modal('hide');
    }
    function LimpaImportacao(limparTipo) {
        if (limparTipo === true)
            TIPO = "";
        $(".tipos-importacao .active").removeClass('active');
        $(".modal-tipo-importacao .arrow-right").addClass('hide');
        VerificaProcessar();
    }
    function SelecionaImportacao($this) {
        DATA = null;
        TIPO = $this.data('tipo');
        var titulo = $this.data('titulo');
        VerificaProcessar();
        ArquivoSelecionado('Nenhum arquivo selecionado.');
        $('.file-percent').text('');

        $(".tipos-importacao .active").removeClass('active');

        $(".tipos-importacao *[data-tipo='" + TIPO + "']").addClass('active');

        $(".tipo-importar").text(titulo);


        $(".modal-tipo-importacao .arrow-right").removeClass('hide').trigger('click');
    }
    function AbreModalSelecioneArquivo() {
        $(".modal-upload-arquivo").modal('show');
        $(".modal-tipo-importacao").modal('hide');
    }
    function ArquivoSelecionado(arquivo) {
        $(".file-name").text(arquivo);
    }
    function VerificaProcessar() {
        if (DATA == null) {
            $(".processar-arquivo").addClass('hide disabled').attr('disabled', true);
            $(".file-button").removeClass('hide').trigger('focus');
        } else {
            $(".file-button").addClass('hide');
            $(".processar-arquivo").removeClass('hide disabled').attr('disabled', false).trigger('focus');
        }
    }
    function ProcesaArquivo() {
        tabela.removeClass('empty');

        if (importador != null)
            importador = importador.destruir();

        if (DATA != null) {
            importador = tabela.importador({
                configuracao: importacao[TIPO],
                dados: DATA,
                regras: regras,
                selectPadrao: false,
            });
        }

        $(".modal-upload-arquivo").modal('hide');
        $("#btnConverter").removeClass('hide');
    }
    function AbreModalConvercao() {
        var esturtura = {
            TipoImportacao: TIPO,
            Importacao: importador.processa()
        }
        var $modal = $(".modal-result");
        var $body = $modal.find(".modal-body");

        $modal.modal({
            backdrop: 'static',
            keyboard: false
        });

        $body.find("#carregandoImportacao").show();

        executarRest("/ImportacaoArquivo/ImportarInformcaoes?callback=?", { Dados: JSON.stringify(esturtura) }, function (res) {
            // Reseta data
            DATA = null;
            $("#btnConverter").addClass('hide');

            // Excluir tabela
            importador.destruir();
            importador = null;
            tabela.addClass('empty');

            // Mensagem de loading
            $body.find("#carregandoImportacao").hide();
            
            if (res.Sucesso) {
                // Mensagem de sucesso
                try {
                    var msg =
                        '<div class="alert alert-success ">' +
                            '<strong>' + res.Objeto.Importados + "/" + res.Objeto.Total + '</strong> ' +
                            res.Objeto.Label + ' foram importado(s) com sucesso.' +
                        '</div>';
                    $body.find("#mensagemRetorno").html(msg).show();
                } catch (e) { }
            } else if (res.Objeto == false && res.Erro != "") {
                // Mensagem de erro
                var erroImport =
                        '<div class="alert alert-danger ">' +
                            '<strong>' + res.Erro + '</strong>' +
                        '</div>';

                $body.find("#errosRetorno").html(erroImport).show();
            } else {
                // Mensagem de erro
                if (res.Objeto.Erros.length > 0) {
                    var _erros = [];

                    for (var i in res.Objeto.Erros)
                        _erros.push("<li>" + res.Objeto.Erros[i] + "</li>");

                    _erros = _erros.reverse();

                    var erroImport =
                            '<div class="alert alert-danger ">' +
                                '<h4>Ocorreu os seguintes erros:</h4>' +
                                '<p><ul>' + _erros.join("") + '</ul></p>' +
                                '<p class="text-right"><button class="btn btn-default reimportardados" type="button">Reimportar</button></p>' +
                            '</div>';

                    $body.find("#errosRetorno").html(erroImport).show();
                }
            }

            // Reimportacao
            if (res.Objeto.Reimportar != null)
                DATA = res.Objeto.Reimportar;

            $modal.find(".btn").attr("disabled", false);
        }, false);
    }
    function ValidaTabela() {
        var valido = importador.valida();

        // Limpa validacao
        tabela.find('thead th.danger').removeClass('danger');

        if (!valido) {
            // Erros retornados sao por campo
            var erros = importador.erros();
            var _msg = []; // Apenas para evitar duplicidade


            erros.forEach(function (erro) {
                var m = '<li>' + erro.mensagem + '</li>';

                // Erro na coluna
                tabela.find('thead th:eq(' + (erro.j + 1) + ')').addClass('danger');

                if ($.inArray(m, _msg) < 0) _msg.push(m);
            });
            ExibirMensagemErro('<ul>' + _msg.join('') + '</ul>', 'Os seguintes erros foram encontrados:<br>');
        }

        return valido;
    }

    // Uploader
    uploader = new plupload.Uploader({
        runtimes: 'html5,flash,gears,silverlight,browserplus',
        unique_names: true,
        browse_button: 'file-button',
        multi_selection: false,
        filters: {
            max_file_size: '10mb',
            mime_types: ".xls,.xlsx,.csv",
        },
        url: ObterPath() + "/ImportacaoArquivo/ConverterArquivo",
        init: {
            FilesAdded: function (up, files) {
                $(".processar-arquivo").addClass('disabled').attr('disabled', true);
                ArquivoSelecionado(files[0].name);
                uploader.start();
            },

            UploadProgress: function (up, file) {
                $(".file-percent").text(' - ' + file.percent + '%');
            },

            FileUploaded: function (up, file, info) {
                try {
                    var retorno = info.response;
                    retorno = retorno.substr(1, (retorno.length - 3));
                    var retorno_json = JSON.parse(retorno);

                    if (retorno_json.Sucesso) {
                        DATA = retorno_json.Objeto;
                    } else {
                        ExibirMensagemErro(retorno_json.Erro, 'Atenção!', 'messages-placeholder-upload');
                        DATA = null;
                    }

                    VerificaProcessar();
                } catch (e) {
                    ExibirMensagemErro("Erro ao processar " + file.name, 'Atenção!', 'messages-placeholder-upload');
                }
            },

            Error: function (up, err) {
                ExibirMensagemErro(err.message, 'Atenção!', 'messages-placeholder-upload');
            }
        }
    });

    uploader.init();
});
