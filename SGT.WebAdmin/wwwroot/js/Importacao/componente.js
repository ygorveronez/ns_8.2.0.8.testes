/// <reference path="../Global/CRUD.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Consultas/ControleImportacao.js" />

/* Exemplo de uso
 * Criar uma propriedade no mapeamento onde vai controlar a importação
 * Ex js: 
 * this.ObjImportar = PropertyEntity({
 *      type: types.local,
 *      text: "Importar",
 *      visible: ko.observable(true),
 *      accept: ".xls,.xlsx,.csv", // arquivos compatíveis (ou null)
 *      cssClass: "btn-default", // classe do botão (apenas default/primary/success/danger/warning)
 * 
 *      UrlImportacao: "Controller/Importar", // URL para enviar os dados da grid
 *      UrlConfiguracao: "Controller/ConfiguracaoImportacao", // URL para buscar os dados de configuração
 *      CodigoControleImportacao: EnumCodigoControleImportacao.I00X_XXXX, // Enumerador regerente a importação
 *      CallbackImportacao:  function () { // Callback para pós importação
 *          _grid.CarregarGrid();
 *      }
 *  });
 *
 * Ex HTML: 
 * <span data-bind='component: {name: "import-file", params: { ko: ObjImportar }}'></span>
 */


ko.components.register('import-file', {
    viewModel: ImportacaoViewModel,
    template:
        '<div class="custom-file-btn" data-bind="visible: visible, attr: { id : id }">' +
        '<label class="btn" data-bind="attr: { for: fileid }, css: cssClass"><i class="fal fa-upload me-1"></i><span data-bind="text: text">Importar</span></label>' +
        '<input type="file" name="file" data-bind="event: {change: fileSelected}, attr: { id : fileid, accept: accept, multiple: multiple }">' +
        '</div>'
});


var _buscaControleImportacao;

function ImportacaoViewModel(params) {
    var self = this;

    this.cssClass = "btn btn-default " + params.ko.cssClass + "  waves-input-wrapper waves-effect waves-themed";
    this.visible = params.ko.visible;
    this.id = params.ko.id;
    this.fileid = 'file_' + params.ko.id;
    this.text = params.ko.text;
    this.accept = params.ko.accept || '';
    this.multiple = params.ko.multiple || null;
    this.fileSelected = function () {
        _servicoImportacaoArquivo.ImportarArquivo("#file_" + self.id);
    };


    var _servicoImportacaoArquivo = new ServicoImportacaoArquivo();

    _servicoImportacaoArquivo.CodigoControleImportacao = params.ko.CodigoControleImportacao || null;
    _servicoImportacaoArquivo.ParametrosRequisicao = params.ko.ParametrosRequisicao || null;
    _servicoImportacaoArquivo.CallbackImportacao = params.ko.CallbackImportacao || null;
    _servicoImportacaoArquivo.CallbackDadosPlanilha = params.ko.CallbackDadosPlanilha || null;
    _servicoImportacaoArquivo.UrlConfiguracao = params.ko.UrlConfiguracao;
    _servicoImportacaoArquivo.UrlImportacao = params.ko.UrlImportacao;
    _servicoImportacaoArquivo.ManterArquivoServidor = params.ko.ManterArquivoServidor || false;
    _servicoImportacaoArquivo.CallbackRegistrosAlterados = params.ko.CallbackRegistrosAlterados;
    _servicoImportacaoArquivo.CallbackSomenteSeSucesso = params.ko.CallbackSomenteSeSucesso;
    _servicoImportacaoArquivo.FecharModalSeSucesso = params.ko.FecharModalSeSucesso;
    _servicoImportacaoArquivo.OcultarMensagemSeSucesso = params.ko.OcultarMensagemSeSucesso || false;
    _servicoImportacaoArquivo.RetornarDadosPlanilha = params.ko.RetornarDadosPlanilha;
    _servicoImportacaoArquivo.ObterPrimeiraConfiguracao = params.ko.ObterPrimeiraConfiguracao;
    _servicoImportacaoArquivo.CallbackPreProcessamento = params.ko.CallbackPreProcessamento || null;
}

function ServicoImportacaoArquivo() {
    //*******MAPEAMENTO KNOUCKOUT*******
    function Importacao() {
        this.Codigo = PropertyEntity({ type: types.local, val: ko.observable(0), def: 0 });
        this.ConfiguracaoColuna = PropertyEntity({ type: types.local, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.ConfiguracoesColunas.getFieldDescription(), val: ko.observable(""), idBtnSearch: guid() });
        this.CodigoControleImportacao = PropertyEntity({ type: types.map, codEntity: ko.observable(self.CodigoControleImportacao), val: ko.observable(self.CodigoControleImportacao) });

        this.Descricao = PropertyEntity({ required: true, type: types.map, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
        this.Colunas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable("") });

        this.Processar = PropertyEntity({ eventClick: processarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Processar, visible: ko.observable(true), icon: "fal fa-upload" });
        this.DownloadRetornoProcessamento = PropertyEntity({ eventClick: downloadRetornoProcessamentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.BaixarRetornoProcessamento, icon: "fal fa-download", visible: ko.observable(false) });
        this.Salvar = PropertyEntity({ eventClick: salvarConfiguracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.SalvarColunas, icon: "fal fa-save", visible: ko.observable(true) });
        this.Excluir = PropertyEntity({ eventClick: excluirConfiguracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.ExcluirConfiguracao, icon: "fal fa-remove", visible: ko.observable(false) });

        this.ConfiguracaoColuna.codEntity.subscribe(function (novoValor) {
            if (novoValor > 0)
                _importacao.Excluir.visible(true);
            else
                _importacao.Excluir.visible(false);
        });
    }

    let idImportacao = guid();
    let self = this;
    let _importacao;
    let importador;
    let configuracoes;
    let $modal;
    let $table;
    let arquivoSalvoComo = "";

    //criando flag para tentar evitar multiplosClicks
    var _ProcessandoPlanilha = false;

    //*******EVENTOS*******
    function downloadRetornoProcessamentoClick() {

        let tab_text = "<table border='2px'>";

        let tab = document.getElementById('table_' + idImportacao);

        for (var i = 0; i < tab.rows.length; i++) {
            var row = tab.rows[i];

            tab_text += "<tr>"

            for (var j = 0; j < (row.cells.length - 1); j++) {
                var cell = row.cells[j];

                tab_text += "<td>";

                if (cell.children.length > 0 && cell.children[0].tagName == "SELECT") {
                    tab_text += $(cell.children[0]).find('option:selected').text();
                } else {
                    tab_text += "&nbsp;" + cell.innerHTML + "&nbsp;";
                }

                tab_text += "</td>";
            }

            tab_text += "</tr>";
        }

        tab_text += "</table>";

        let uri = 'data:application/vnd.ms-excel;base64,'
            , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><meta http-equiv="content-type" content="application/vnd.ms-excel; charset=UTF-8"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body>{table}</body></html>'
            , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
            , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) };

        let ctx = { worksheet: "Retorno Processamento Arquivo" || 'Worksheet', table: tab_text };

        let a = document.getElementById("lnkDownloadProcessamento_" + idImportacao);

        a.download = "Retorno Processamento Arquivo.xls";
        a.href = uri + base64(format(template, ctx));

        a.click();
    }

    function processarClick() {
        if (_ProcessandoPlanilha)
            return;

        _ProcessandoPlanilha = true;
        let tabelaDeDados = importador.processa();

        if (self.RetornarDadosPlanilha) {
            _ProcessandoPlanilha = false;
            self._modalWindow.hide();
            self.CallbackDadosPlanilha(tabelaDeDados.dados);
            return;
        }

        let handleProcessamento = function () {
            let parametros = null;

            if (self.ParametrosRequisicao != null)
                parametros = self.ParametrosRequisicao();

            executarReST(self.UrlImportacao, {
                Nome: tabelaDeDados.nome,
                Tamanho: tabelaDeDados.tamanho,
                Tipo: tabelaDeDados.tipo,
                Dados: JSON.stringify(tabelaDeDados.dados),
                ArquivoSalvoComo: arquivoSalvoComo,
                Parametro: JSON.stringify(parametros)
            }, CallbackImportacao, erroProcessarImportacao).fail(ErroImportacao);
        };

        if ($.isFunction(self.CallbackPreProcessamento)) {
            let cbReturn = self.CallbackPreProcessamento(tabelaDeDados);
            return Promise.resolve(cbReturn).then(function (promisseReturn) {
                if (typeof promisseReturn == "boolean" && !promisseReturn) {
                    _ProcessandoPlanilha = false;
                    return;
                }

                handleProcessamento();
            });
        }

        handleProcessamento();
    }

    function salvarConfiguracaoClick(e, sender) {
        // Seta as colunas para salvar ordem
        var colunas = importador.getColunas();
        _importacao.Colunas.val(colunas.join("|"));

        // Salva
        Salvar(_importacao, "ImportacaoArquivo/SalvarConfiguracao", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso");
                    _importacao.Descricao.val(_importacao.Descricao.def);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }

    function excluirConfiguracaoClick(e, sender) {

        exibirConfirmacao("Atenção!", "Deseja realmente excluir a configuração " + _importacao.ConfiguracaoColuna.val() + "?", function () {

            _importacao.Codigo.val(_importacao.ConfiguracaoColuna.codEntity());

            ExcluirPorCodigo(_importacao, "ImportacaoArquivo/ExcluirConfiguracao", function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Registro excluído com sucesso!");

                        _importacao.ConfiguracaoColuna.codEntity(0);
                        _importacao.ConfiguracaoColuna.val("");
                        _importacao.Codigo.val(0);

                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            }, sender);

        });
    }

    function erroProcessarImportacao() {
        _ProcessandoPlanilha = false;

        exibirMensagem(tipoMensagem.falha, "Falha", "Ocorreu um erro na importação de dados.");
        console.log("O método de processar retornou algum erro.");
    }

    //*******MÉTODOS PRIVADOS*******
    let ErroImportacao = function (xhr, status, error) {
        _ProcessandoPlanilha = false;

        var dataErro = {
            xhr: JSON.stringify(xhr.message),
            status: JSON.stringify(status),
            error: JSON.stringify(error),
        };

        executarReST("ImportacaoArquivo/SalvarLogsErro", dataErro, function (arg) {
            if (!arg.Success) {
                var _data = new Date;
                var key = 'ProblemaImportacao' + _data.getMonth() + _data.getDate() + _data.getHours() + _data.getMinutes() + _data.getSeconds();
                localStorage.setItem(key, JSON.stringify(dataErro));

                exibirMensagem(tipoMensagem.falha, "Falha", "Ocorreu uma falha no componente de importação.");
            }
        }, null);
    }

    let CallbackImportacao = function (arg) {
        if (arg.Success) {
            if (arg.Data && arg.Data != null) {
                // Excluir tabela
                //importador.destruir();
                //importador = null;

                // Mensagem de sucesso
                if (!self.OcultarMensagemSeSucesso) {
                    var msg = '<strong>' + arg.Data.Importados + "/" + arg.Data.Total + '</strong> registros foram importado(s) com sucesso.';
                    if (arg.Data.MensagemAviso != null && arg.Data.MensagemAviso != "") {
                        msg += arg.Data.MensagemAviso;
                        exibirMensagem(tipoMensagem.aviso, "Aviso", msg, 60000);
                    } else
                        exibirMensagem(tipoMensagem.ok, "Sucesso", msg, 7000);
                }

                if (arg.Data.Retornolinhas != null && arg.Data.Retornolinhas.length > 0) {
                    importador.setStatus(arg.Data.Retornolinhas);

                    if (self.CallbackSomenteSeSucesso || self.FecharModalSeSucesso) {
                        var sucessoNaImportacao = true;

                        for (var i = 0; i < arg.Data.Retornolinhas.length; i++) {
                            if (!arg.Data.Retornolinhas[i].processou) {
                                sucessoNaImportacao = false;
                                break;
                            }
                        }

                        if (sucessoNaImportacao) {
                            if (self.CallbackSomenteSeSucesso)
                                self.CallbackImportacao(arg);

                            if (self.FecharModalSeSucesso) {
                                importador.destruir();
                                importador = null;
                                self._modalWindow.hide();
                            }
                        }
                    }

                    _importacao.DownloadRetornoProcessamento.visible(true);
                    _importacao.Processar.visible(false);
                } else {
                    //Esconde Modal
                    // Excluir tabela
                    importador.destruir();
                    importador = null;
                    self._modalWindow.hide();
                }

                // Reimportar
                //if (arg.Data.Reimportar != null && arg.Data.Reimportar.length > 0) {
                //    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Data.Reimportar.length + " registro(s) não foi(ram) processado(s)", 7000);
                //} else {
                //}

                if (!self.CallbackSomenteSeSucesso && self.CallbackImportacao != null)
                    self.CallbackImportacao(arg);

                if (arg.Data.RegistrosAlterados != null && arg.Data.RegistrosAlterados.length > 0 && self.CallbackRegistrosAlterados)
                    self.CallbackRegistrosAlterados(arg.Data.RegistrosAlterados);

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
        _ProcessandoPlanilha = false;
    }

    let obterConfiguracaoColuna = function () {
        if (self.ObterPrimeiraConfiguracao) {
            executarReST("ImportacaoArquivo/ObterPrimeiraConfiguracao", { CodigoControleImportacao: self.CodigoControleImportacao }, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        _importacao.ConfiguracaoColuna.val(arg.Data.Descricao);
                        AtualizaColunas(arg.Data);
                    }
                }
            }, null);
        }
    }

    let init = function () {
        $.get("Content/Static/Importacoes/ImportacaoArquivo.html?dyn=" + idImportacao, function (data) {
            _ProcessandoPlanilha = false;
            // Replace ids
            data = data.replace(/#idguid/g, idImportacao);

            // Instancia modal
            $modal = $(data);

            // Instancia tabela
            $table = $modal.find('#table_' + idImportacao);

            // Inclui o HTML no DOM
            $("#js-page-content").append($modal);

            LocalizeCurrentPage();

            self._modalWindow = new bootstrap.Modal(document.getElementById('modal_' + idImportacao), { keyboard: true, backdrop: 'static' });

            // Obj Importacao
            _importacao = new Importacao();
            KoBindings(_importacao, "knockout_" + idImportacao);

            // Busca de colunas
            _buscaControleImportacao = new BuscarControleImportacao(_importacao.ConfiguracaoColuna, self.CodigoControleImportacao, AtualizaColunas);

            // Carrega informacoes da pagina
        });
    }

    var CarregarInformacoesImportacao = function (callback) {
        let parametros = {};
        if (self.ParametrosRequisicao != null)
            parametros = self.ParametrosRequisicao();

        executarReST(self.UrlConfiguracao, parametros, function (arg) {
            if (arg.Success) {
                if (arg.Data != null) {
                    _importacao.DownloadRetornoProcessamento.visible(false);
                    _importacao.Processar.visible(true);
                    configuracoes = arg.Data.slice();
                    callback();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }

    var ValidaTabela = function () {
        let validoHead = importador.validarColunas();

        if (!validoHead) {
            exibirMensagem(tipoMensagem.atencao, "Inválido", "Colunas com * são obrigatórias.");
            importador.tooltipsColunas();
        } else {
            importador.removeTooltipsColunas();
        }

        let validoData = importador.validarDados();
        if (!validoData) {
            exibirMensagem(tipoMensagem.atencao, "Inválido", "Campos inválidos.");
        } else {
            importador.removeTooltipsDados();
        }

        return validoHead && validoData;
    }

    let AtualizaColunas = function (data) {
        let colunas = data.Ordem.split('|');

        // Atuliza as colunas
        importador.setColunas(colunas);
    }


    //*******MÉTODOS PUBLICOS*******
    this.ImportarArquivo = function (id) {
        CarregarInformacoesImportacao(function () {
            let file = $(id)[0];
            let formData = new FormData();

            formData.append("upload", file.files[0]);

            enviarArquivo("ImportacaoArquivo/ConverterArquivo?callback=?", { ManterArquivoServidor: self.ManterArquivoServidor }, formData, function (arg) {
                if (arg.Success) {
                    if (arg.Data != null && arg.Data != false && file.value != null) {
                        arquivoSalvoComo = arg.Data.SalvoComo;
                        obterConfiguracaoColuna();

                        // Carrega os dados na tabela
                        if (importador != null)
                            importador = importador.destruir();

                        importador = $table.importador({
                            configuracao: configuracoes,
                            nome: arg.Data.FileName,
                            tamanho: arg.Data.ContentLength,
                            tipo: arg.Data.ContentType,
                            dados: arg.Data.Content,
                            regras: regras,
                            tooltipContainer: '#modal_' + idImportacao
                        });

                        // Abre Modal 
                        $modal.off();
                        $modal.one('hidden.bs.modal', function () {
                                file.value = null;
                                if (importador != null) importador = importador.destruir();
                                LimparCampoEntity(_importacao.ConfiguracaoColuna);
                            });

                        self._modalWindow.show();

                    } else if (file.value != null && file.value != "") {
                        file.value = null;
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    }
                } else {
                    file.value = null;
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        });
    }

    init();
}