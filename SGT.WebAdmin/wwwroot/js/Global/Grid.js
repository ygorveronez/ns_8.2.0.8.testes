/// <reference path="../libs/jquery-2.1.1.js" />
/// <reference path="Rest.js" />
/// <reference path="CRUD.js" />
/// <reference path="../libs/jquery.globalize.js" />
/// <reference path="../libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../ViewsScripts/Enumeradores/EnumTipoSumarizacao.js" />
/// <reference path="../../ViewsScripts/Enumeradores/EnumTipoColunaEditavelGrid.js" />

var TypeOptionMenu = { list: "list", link: "link" };
var orderDir = { asc: "asc", desc: "desc" };
var ConfigRowsSelect = { permiteSelecao: false, marcarTodos: false, permiteSelecionarTodos: false };
var Language = {
    "emptyTable": Localization.Resources.Gerais.Geral.NenhumRegistroEncontrado,
    "info": Localization.Resources.Gerais.Geral.ExibindoAteDeRegistros,
    "infoEmpty": "",
    "infoFiltered": "(filtrado de um total de _MAX_ registros)",
    "infoPostFix": "",
    "thousands": ".",
    "lengthMenu": Localization.Resources.Gerais.Geral.ExibindoRegistros,
    "loadingRecords": Localization.Resources.Gerais.Geral.Carregando + "...",
    "processing": Localization.Resources.Gerais.Geral.Processando + "...",
    "search": "Search:",
    "zeroRecords": Localization.Resources.Gerais.Geral.NenhumRegistroEncontrado,
    "paginate": {
        "first": Localization.Resources.Gerais.Geral.Inicio,
        "last": Localization.Resources.Gerais.Geral.Fim,
        "next": Localization.Resources.Gerais.Geral.Proximo,
        "previous": Localization.Resources.Gerais.Geral.Anterior
    },
    "aria": {
        "sortAscending": ": ative para ordenar a coluna ascendente",
        "sortDescending": ": ative para ordenar a coluna descendente"
    }
};

var formatterCurrency = new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
});

var GridViewExportacao = function (idElemento, url, knout, menuOpcoes, configExportacao, ordenacaoPadrao, quantidadePorPagina, infoMultiplaSelecao, limiteRegistro, infoEditarColuna, callbackColumnDefault) {
    return new GridView(idElemento, url, knout, menuOpcoes, ordenacaoPadrao, quantidadePorPagina, null, null, null, infoMultiplaSelecao, limiteRegistro, infoEditarColuna, configExportacao, null, null, null, callbackColumnDefault);
};

var GridView = function (idElemento, url, knout, menuOpcoes, ordenacaoPadrao, quantidadePorPagina, callback, mostrarInfo, draggableRows, infoMultiplaSelecao, limiteRegistro, infoEditarColuna, configExportacao, callbackOrdenacao, isExibirPaginacao, callbackRow, callbackColumnDefault, exibirLoading) {
    var self = this;

    if (limiteRegistro == null)
        limiteRegistro = 50;

    var editarColuna = { permite: false, callback: null, atualizarRow: true, functionPermite: null };

    if (infoEditarColuna != null) {
        editarColuna.permite = infoEditarColuna.permite;
        editarColuna.functionPermite = infoEditarColuna.functionPermite;
        editarColuna.callback = infoEditarColuna.callback;
        editarColuna.atualizarRow = infoEditarColuna.atualizarRow;
    }

    // Objeto de configuracao para exportacao
    configExportacao = $.extend({
        exportar: configExportacao != null,
        url: "",
        titulo: "",
        id: guid(),
        btnText: Localization.Resources.Gerais.Geral.Exportar + " Excel",
        btnLoading: Localization.Resources.Gerais.Geral.Exportando + "...",
        exportarPorRelatorio: false
    }, configExportacao);

    if (exibirLoading !== false)
        exibirLoading = true;

    var checkedMarcarDesmarcar = "checked";

    var multiplaEscolha = { permite: false, basicTable: null, permitirSelecionarSomenteUmRegistro: false };

    if (infoMultiplaSelecao != null) {//quando for diferente de nulo quer dizer que o foco será escolhido com base em multipla seleção
        multiplaEscolha.permite = true;
        multiplaEscolha.basicTable = infoMultiplaSelecao.basicGrid;
        multiplaEscolha.callback = infoMultiplaSelecao.callback;
        multiplaEscolha.selecionados = infoMultiplaSelecao.selecionados != null ? infoMultiplaSelecao.selecionados : new Array();
        multiplaEscolha.naoSelecionados = infoMultiplaSelecao.naoSelecionados != null ? infoMultiplaSelecao.naoSelecionados : new Array();
        multiplaEscolha.SelecionarTodosKnout = infoMultiplaSelecao.SelecionarTodosKnout;
        multiplaEscolha.somenteLeitura = infoMultiplaSelecao.somenteLeitura != null ? infoMultiplaSelecao.somenteLeitura : false;
        multiplaEscolha.callbackSelecionado = infoMultiplaSelecao.callbackSelecionado;
        multiplaEscolha.callbackNaoSelecionado = infoMultiplaSelecao.callbackNaoSelecionado;
        multiplaEscolha.callbackSelecionarTodos = infoMultiplaSelecao.callbackSelecionarTodos;
        multiplaEscolha.afterDefaultCallback = infoMultiplaSelecao.afterDefaultCallback;
        multiplaEscolha.manterSelecionadosMultiPesquisa = Boolean(infoMultiplaSelecao.manterSelecionadosMultiPesquisa);
        multiplaEscolha.permitirSelecionarSomenteUmRegistro = Boolean(infoMultiplaSelecao.permitirSelecionarSomenteUmRegistro);
        multiplaEscolha.classeSelecao = infoMultiplaSelecao.classeSelecao != null ? infoMultiplaSelecao.classeSelecao : "selected";
    }

    var callbackEdicaoColuna = null;
    this.SetCallbackEdicaoColunas = function (callback) { //quando é necessário fazer algo espefico ao ocultar ou exibir uma coluna setar esse callback
        callbackEdicaoColuna = callback; //removido devido à opcao de desmarcar todos no relatório
    };

    var permitirEdicaoColunas = false;
    this.SetPermitirEdicaoColunas = function (permitir) {
        permitirEdicaoColunas = permitir;
        permitirReordenarColunas = permitir;
    };

    var permitirReordenarColunas = false;
    this.SetPermitirReordenarColunas = function (permitir) {
        permitirReordenarColunas = permitir;
    };

    var salvarPreferenciasGrid = false;
    this.SetSalvarPreferenciasGrid = function (salvar) {
        salvarPreferenciasGrid = salvar;
    }

    var modelosGrid = false;
    this.SetHabilitarModelosGrid = function (habilitar) {
        modelosGrid = habilitar;
    };

    var permitirRedimencionarColunas = true;
    this.SetPermitirRedimencionarColunas = function (permitir) {
        permitirRedimencionarColunas = permitir;
    };

    var scrollHorizontal = false;
    this.SetScrollHorizontal = function (habilitar) {
        scrollHorizontal = habilitar;
    }

    var habilitarScrollHorizontal = false;
    this.SetHabilitarScrollHorizontal = function (habilitar, tamanho) {
        habilitarScrollHorizontal = habilitar;
        if (!habilitar) {
            scrollHorizontal = false;
            tamanhoPadraoPorColuna = 0;
        } else {
            tamanhoPadraoPorColuna = tamanho;
        }
    }

    var callbackDrawGridView;
    this.SetCallbackDrawGridView = function (fncallbackDrawGridView) {
        callbackDrawGridView = fncallbackDrawGridView;
    };

    var tamanhoPadraoPorColuna = 0;
    this.setTamanhoPadraoPorColuna = function (tamanho) {
        tamanhoPadraoPorColuna = tamanho;
    }

    this.GetUrl = function () {
        return url;
    };

    this.GetGridId = function () {
        return idElemento;
    };

    var _onBeforeGridLoad = function (data) { };
    this.onBeforeGridLoad = function (fn) {
        _onBeforeGridLoad = fn;
    };

    var _onAfterGridLoad = function (data) { };
    this.onAfterGridLoad = function (fn) {
        _onAfterGridLoad = fn;
    };

    var _onTableCreated = function (data) { };
    this.onTableCreated = function (fn) {
        _onTableCreated = fn;
    };

    var _onTableDestroy = function (data) { };
    this.onTableDestroy = function (fn) {
        _onTableDestroy = fn;
    };

    var _onGridData = function (data) { };
    this.onGridData = function (fn) {
        _onGridData = fn;
    };

    var habilitarLinhaClicavel = false;
    this.SetHabilitarLinhaClicavel = function (permitir) {
        habilitarLinhaClicavel = !!permitir;
    };

    var _onClickLinha = function (data) { };
    var _timeOutClickLinha = undefined;
    this.OnClickLinha = function (fn) {
        _onClickLinha = fn;
    };
    var fnLinhaClicadaHandle = function (e) {
        var tr = $(this).closest('tr');
        var row = table.row(tr);

        if (!_timeOutClickLinha) {
            _timeOutClickLinha = setTimeout(function () {
                _onClickLinha(row.data());
                _timeOutClickLinha = undefined;
            }, 200);
        } else {
            clearTimeout(_timeOutClickLinha);
            _timeOutClickLinha = undefined;
        }
    }
    this.SetDefinicaoColunas = function (obj) {
        callbackColumnDefault = obj;
    };

    var habilitarExpansaoLinha = false;
    this.SetHabilitarExpansaoLinha = function (permitir) {
        habilitarExpansaoLinha = !!permitir;
    };

    var _onLinhaExpandida = function (data) { };
    var _openedRow = null;
    this.OnLinhaExpandida = function (fn) {
        _onLinhaExpandida = fn;
    };

    var _onLinhaRecolhida = function (data) { };
    this.OnLinhaRecolhida = function (fn) {
        _onLinhaRecolhida = fn;
    };
    var fnExpansaoLinhaHandle = function (e) {
        var table = $('#' + idElemento).DataTable();
        var tr = $(this).closest('tr');
        var row = table.row(tr);

        if (row.child.isShown()) {
            _onLinhaRecolhida(row.data(), _openedRow);
            _openedRow = null;
            row.child.hide();
            tr.removeClass('shown');
        } else {
            if (_openedRow != null) {
                _openedRow.row.child.hide();
                _openedRow.tr.removeClass('shown');
            }
            _openedRow = {
                tr: tr,
                row: row,
            };

            var objLinhaExpandida = _onLinhaExpandida(row.data(), _openedRow);
            var htmlLinhaExpandida = typeof objLinhaExpandida == 'string' ? objLinhaExpandida : objLinhaExpandida.html;

            row.child(htmlLinhaExpandida).show();
            tr.addClass('shown');

            if (objLinhaExpandida.callback)
                objLinhaExpandida.callback();
        }
    };

    this.SetQuantidadeLinhasPorPagina = function (numero) {
        quantidadePorPagina = numero;
    };

    this.GetQuantidadeLinhasPorPagina = function () {
        return quantidadePorPagina;
    };

    var ResponseCache = {};
    var ModifiedCache = [];
    var SetResponseCache = function (cache) {
        ResponseCache = $.extend(true, {}, cache);
    };

    var GetResponseCache = function () {
        return ResponseCache;
    };

    this.ClearCache = function () {
        ModifiedCache = [];
    };

    var ApplyCacheModifications = function (jsonData) {
        if (ModifiedCache.length == 0) return;
        for (var i = 0, s = jsonData.data.length; i < s; i++) {
            for (var j = 0, s2 = ModifiedCache.length; j < s2; j++) {
                if (ModifiedCache[j].Codigo == jsonData.data[i].Codigo) {
                    for (var prop in ModifiedCache[j]) {
                        jsonData.data[i][prop] = ModifiedCache[j][prop];
                    }
                }
            }
        }
    };

    var AtualizarDadosEmCache = function (data) {
        var cache = GetResponseCache();

        if (!$.isEmptyObject(cache) && $.isArray(cache.data) && data.Codigo) {
            for (var i = 0, s = cache.data.length; i < s; i++) {
                if (cache.data[i].Codigo == data.Codigo) {
                    for (var prop in data)
                        cache.data[i][prop] = data[prop];
                }
            }

            var jaCacheado = false;
            for (var i = 0, s = ModifiedCache.length; i < s; i++) {
                if (ModifiedCache[i].Codigo == data.Codigo) {
                    jaCacheado = true;
                    ModifiedCache[i] = data;
                }
            }

            if (!jaCacheado)
                ModifiedCache.push(data);

            SetResponseCache(cache);
        }
    };

    var habilitarColunasCabecalho = function (habilitar) {
        var headerResize = header.slice();
        for (var i = 0; i < headerResize.length; i++) {
            var cabecalhoColuna = headerResize[i];
            var nomeColuna = cabecalhoColuna.name;
            var coluna = table.column(nomeColuna + ':name');
            var habilitarColuna = habilitar && Boolean(cabecalhoColuna.title);

            cabecalhoColuna.width = habilitarColuna ? cabecalhoColuna.widthDefault : "0%";
            cabecalhoColuna.visible = habilitarColuna;
            coluna.visible(habilitarColuna);

            $("[data-column=" + nomeColuna + "]").prop('checked', cabecalhoColuna.visible);
        }

        checkedMarcarDesmarcar = "";

        if (habilitar)
            checkedMarcarDesmarcar = "checked";

        recalcularTamanho();
    };

    this.SetarEditarColunas = function (infoEditar) {
        editarColuna.permite = infoEditar.permite;
        editarColuna.callback = infoEditar.callback;
        editarColuna.functionPermite = infoEditar.functionPermite;
        editarColuna.atualizarRow = infoEditar.atualizarRow;
    };

    var cacheLastRequest;
    var FnPipeline = function (opts) {
        // Configuration options
        var conf = $.extend({
            pages: 5,     // number of pages to cache
            url: '',      // script url
            data: null,   // function or object with parameters to send to the server
            // matching how `ajax.data` works in DataTables
            method: 'GET', // Ajax HTTP method
            initCacheLastJson: null,
            initCacheLastRequest: null,
            initCacheUpper: null,
            initCacheLower: -1,
            initDadosPesquisa: null
        }, opts);

        // Private variables for storing the cache

        var cacheLower = conf.initCacheLower;
        var cacheUpper = conf.initCacheUpper;
        cacheLastRequest = conf.initCacheLastRequest;
        SetResponseCache(conf.initCacheLastJson);

        return function (request, drawCallback, settings) {
            var ajax = false;
            var requestStart = request.start;
            var drawStart = request.start;
            var requestLength = request.length;
            var requestEnd = requestStart + requestLength;

            if (settings.clearCache) {
                // API requested that the cache be cleared
                ajax = true;
                settings.clearCache = false;
            }
            else if (cacheLower < 0 || requestStart < cacheLower || requestEnd > cacheUpper) {
                // outside cached data - need to make a request
                ajax = true;
            }
            else if (JSON.stringify(request.order) !== JSON.stringify(cacheLastRequest.order)) {
                // properties changed (ordering, columns, searching)
                ajax = true;
            }

            // Store the request for checking next time around
            cacheLastRequest = $.extend(true, {}, request);

            if (ajax) {
                // Need data from the server
                if (requestStart < cacheLower) {
                    requestStart = requestStart - (requestLength * (conf.pages - 1));

                    if (requestStart < 0) {
                        requestStart = 0;
                    }
                }

                cacheLower = requestStart;
                cacheUpper = requestStart + (requestLength * conf.pages);

                request.start = requestStart;
                request.length = requestLength * conf.pages;

                // Provide the same `data` options as DataTables.
                if ($.isFunction(conf.data)) {
                    // As a function it is executed with the data object as an arg
                    // for manipulation. If an object is returned, it is used as the
                    // data object to submit
                    var d = conf.data(request);
                    if (d) {
                        $.extend(request, d);
                    }
                }
                else if ($.isPlainObject(conf.data)) {
                    // As an object, the data given extends the default
                    $.extend(request, conf.data);
                }
                var dadosPesquisa = conf.initDadosPesquisa;
                dadosPesquisa.Grid = request;
                dadosPesquisa.Grid.header = header;
                dadosPesquisa.Grid.group = group;
                grid = dadosPesquisa.Grid;
                dadosPesquisa.Grid = JSON.stringify(grid);
                _openedRow = null;
                settings.jqXHR = executarReST(conf.url, dadosPesquisa, function (jsonData) {
                    if (jsonData.Success) {
                        todosRegistrosRetornados = $.extend(true, [], jsonData.Data.data);// jsonData.Data.data;

                        var json = jsonData.Data;
                        ApplyCacheModifications(json);
                        SetResponseCache(json);
                        var fulldata = json.data.slice();

                        if (cacheLower != drawStart)
                            json.data.splice(0, drawStart - cacheLower);
                        json.data.splice(requestLength, json.data.length);

                        drawCallback(json);
                        if (_onGridData)
                            _onGridData(fulldata);
                    } else {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, jsonData.Msg);
                    }

                });
            }
            else {
                json = $.extend(true, {}, GetResponseCache());
                json.draw = request.draw; // Update the echo for each response
                json.data.splice(0, requestStart - cacheLower);
                json.data.splice(requestLength, json.data.length);

                drawCallback(json);
            }
        };
    };

    $.fn.dataTable.Api.register('clearPipeline()', function () {
        return this.iterator('table', function (settings) {
            settings.clearCache = true;
        });
    });
    var ColunaOrdenaPadrao;
    var numeroRegistro = 0;
    this.NumeroRegistros = function () {
        return numeroRegistro;
    };

    if (mostrarInfo == null)
        mostrarInfo = true;

    var table;
    var listTabs = new Array();
    var header = new Array();
    this.GetHeader = function () {
        return header;
    };
    var _tableAllData;

    var group = { enable: false, propAgrupa: "", dirOrdena: orderDir.desc };
    this.SetGroup = function (groupSer) {
        group = groupSer;
    };

    var grid;
    this.GetGrid = function () {
        return grid;
    };

    this.SetGrid = function (gridSer) {
        grid = gridSer;
        header = grid.header;
        group = grid.group;
        listTabs = grid.listTabs;

        if (grid.header.length > 0) {
            ordenacaoPadrao = { column: grid.indiceColunaOrdena, dir: grid.dirOrdena };
            ColunaOrdenaPadrao = header[grid.indiceColunaOrdena].data;
        }
    };

    var modelo;
    this.SetModelo = function (codigoModelo) {
        modelo = codigoModelo;
    };

    var selected = new Array();
    var unselected = new Array();
    var selecionarTodos = false;

    var dataRowAnterior;

    var todosRegistrosRetornados = null;

    var ordenacaoGrid;

    var obterTamanhoPadraoTotalHeader = function () {
        var total = 0;

        for (var i = 0; i < header.length; i++) {
            var cabecalhoColuna = header[i];

            if (cabecalhoColuna.visible)
                total += parseFloat(cabecalhoColuna.widthDefault);
        }

        return total;
    };

    var obterTamanhoTotalHeader = function () {
        var total = 0;

        for (var i = 0; i < header.length; i++) {
            var cabecalhoColuna = header[i];

            if (cabecalhoColuna.visible) {
                var tamanho = parseFloat(cabecalhoColuna.width);

                total += (tamanho > 0) ? tamanho : parseFloat(cabecalhoColuna.widthDefault);
            }
        }

        return total;
    };

    var recalcularTamanho = function () {
        var total = obterTamanhoTotalHeader();

        if (total == 0) {
            $("#" + idElemento + "-resize-content").css("min-width", "0");
            return;
        }

        var headerResize = header.slice();
        var totalColunasVisiveis = 0;

        for (var i = 0; i < headerResize.length; i++) {
            var cabecalhoColuna = headerResize[i];
            var $coluna = $(table.column(cabecalhoColuna.name + ':name').header());

            if (cabecalhoColuna.visible) {
                var tamanho = parseFloat(cabecalhoColuna.width);

                if (tamanho == 0) {
                    tamanho = parseFloat(cabecalhoColuna.widthDefault);
                    cabecalhoColuna.width = cabecalhoColuna.widthDefault;
                }

                cabecalhoColuna.width = ((tamanho / total) * 100) + "%";
                totalColunasVisiveis++;
            }
            else
                cabecalhoColuna.width = "0%";

            $coluna.width(cabecalhoColuna.width);
        }

        var tamanhoColunas = scrollHorizontal ? tamanhoPadraoPorColuna : 0;

        $("#" + idElemento + "-resize-content").css("min-width", (totalColunasVisiveis * tamanhoColunas) + "px");

        grid.header = headerResize;

        table.draw();
    };

    var onResizeGrid = function (event) {
        var elemento = $(event.currentTarget);
        var total = elemento.width();
        var headerResize = header.slice();
        var i, cabecalhoColuna, $coluna;

        for (i = 0; i < headerResize.length; i++) {
            cabecalhoColuna = headerResize[i];
            $coluna = $(table.column(cabecalhoColuna.name + ':name').header());

            if (cabecalhoColuna.visible)
                cabecalhoColuna.width = (($coluna.width() / total) * 100) + "%";
            else
                cabecalhoColuna.width = "0%";
        }

        for (i = 0; i < headerResize.length; i++) {
            cabecalhoColuna = headerResize[i];
            $coluna = $(table.column(cabecalhoColuna.name + ':name').header());

            $coluna.width(cabecalhoColuna.width);
        }

        grid.header = headerResize;
    };

    var iniciarColunaResizeble = function (idElemento) {
        if (IsTouchDevice())
            $("#" + idElemento).off("touchstart.resizeble");

        $("#" + idElemento).off("mouseenter.resizeble");
        $('#' + idElemento).colResizable({ disable: true });
        $('#' + idElemento).colResizable({ liveDrag: true, minWidth: 50, resizeMode: "fit", onResize: onResizeGrid });
    };

    var obterCabecalhoColunaPorNome = function (nome) {
        for (var i = 0; i < header.length; i++) {
            if (header[i].name == nome)
                return header[i];
        }

        return undefined;
    }

    var definirTamanhoMinimoTabela = function () {
        if (scrollHorizontal && tamanhoPadraoPorColuna > 0) {
            var cabecalhos = header.slice();
            var totalColunasVisiveis = 0;

            for (var i = 0; i < cabecalhos.length; i++) {
                if (cabecalhos[i].visible)
                    totalColunasVisiveis++;
            }

            $("#" + idElemento + "-resize-content").css("min-width", (totalColunasVisiveis * tamanhoPadraoPorColuna) + "px");
        }
        else
            $("#" + idElemento + "-resize-content").css("min-width", "0");
    }

    this.ObterColunasVisiveis = function () {
        return grid.header
            .filter(obj => obj.visible)
            .map(obj => obj.data);
    };

    this.ExibirColunaPorNome = function (nomeColuna, exibir) {
        var coluna = table.column(nomeColuna + ':name');
        var cabecalhoColuna = obterCabecalhoColunaPorNome(nomeColuna);

        if (!cabecalhoColuna)
            return;

        if (exibir && !cabecalhoColuna.visible)
            cabecalhoColuna.width = cabecalhoColuna.widthDefault;

        coluna.visible(exibir);
        cabecalhoColuna.visible = exibir;

        if (cabecalhoColuna.visible) {
            $("a[data-column=" + nomeColuna + "]").css("text-decoration", "none");
            $("a[data-column=" + nomeColuna + "]").css("color", "#3276b1");
            $("a[data-column=" + nomeColuna + "]").css("font-weight", "bold");
        }
        else {
            $("a[data-column=" + nomeColuna + "]").css("text-decoration", "line-through");
            $("a[data-column=" + nomeColuna + "]").css("color", "#888");
            $("a[data-column=" + nomeColuna + "]").css("font-weight", "normal");
        }

        grid.header = header;

        definirTamanhoMinimoTabela();
        table.draw();
    };

    isExibirPaginacao = isExibirPaginacao != false;

    if (callbackOrdenacao instanceof Function)
        ordenacaoGrid = new GridViewOrdenacao(idElemento, quantidadePorPagina, callbackOrdenacao, draggableRows);

    this.CarregarGrid = function (loadCallback, manterPaginacao) {
        var dadosGrid = new Object();
        var paginaAtual = 0;

        if (manterPaginacao && table != null) {
            paginaAtual = this.ObterPaginaAtual();

            var inicioRegistros = paginaAtual;

            if (paginaAtual > 0)
                inicioRegistros = Math.floor(paginaAtual * (quantidadePorPagina || 5) / limiteRegistro) * limiteRegistro;

            dadosGrid.start = inicioRegistros;
            dadosGrid.length = dadosGrid.start + limiteRegistro;

            if (table.order().length > 0 && table.order()[0].length > 0)
                dadosGrid.order = [{ column: (table.order()[0][0] || 0), dir: (table.order()[0][1] == "asc" ? orderDir.asc : orderDir.desc) }];
            else
                dadosGrid.order = new Array();

        }
        else {
            dadosGrid.start = 0;
            dadosGrid.length = limiteRegistro;
            dadosGrid.order = new Array();
        }

        if (table != null) {
            _onTableDestroy(table);
            table.destroy(false);
            table = null;
            $('#' + idElemento).empty();
            _tableAllData = [];
        }

        if (multiplaEscolha != null && infoMultiplaSelecao != null) {
            if (multiplaEscolha.manterSelecionadosMultiPesquisa) {
                multiplaEscolha.selecionados = multiplaEscolha.selecionados != null ? multiplaEscolha.selecionados : new Array();
                multiplaEscolha.naoSelecionados = multiplaEscolha.naoSelecionados != null ? multiplaEscolha.naoSelecionados : new Array();
            }
            else {
                multiplaEscolha.selecionados = infoMultiplaSelecao.selecionados != null ? infoMultiplaSelecao.selecionados : new Array();
                multiplaEscolha.naoSelecionados = infoMultiplaSelecao.naoSelecionados != null ? infoMultiplaSelecao.naoSelecionados : new Array();
            }
        }

        var prom = new promise.Promise();

        var breakpointDefinition = {
            tablet: 1024,
            phone: 480
        };

        dadosGrid.draw = 1;
        dadosGrid.group = group;
        dadosGrid.header = header;
        dadosGrid.listTabs = listTabs;
        dadosGrid.modelo = modelo;

        if (!manterPaginacao || dadosGrid.order == null || dadosGrid.order.length <= 0) {
            if (ordenacaoPadrao != null)
                dadosGrid.order.push({ column: ordenacaoPadrao.column, dir: ordenacaoPadrao.dir });
            else
                dadosGrid.order.push({ column: 0, dir: orderDir.desc });
        }

        var dadosPesquisa = RetornarObjetoPesquisa(knout);

        dadosPesquisa.Grid = JSON.stringify(dadosGrid);
        grid = dadosGrid;

        executarReST(url, dadosPesquisa, function (retorno) {
            if (retorno.Success) {
                var idDivBusca = idElemento.split("_")[0];

                if (habilitarScrollHorizontal && retorno.Data.scrollHorizontal != undefined) scrollHorizontal = retorno.Data.scrollHorizontal;
                retorno.Data.header.sort(SortByPosition);
                header = retorno.Data.header;
                numeroRegistro = retorno.Data.recordsTotal;
                listTabs = retorno.Data.listTabs;

                var json = retorno.Data;

                _tableAllData = json;

                json.draw = retorno.Data.draw;
                json.recordsTotal = retorno.Data.recordsTotal;
                json.recordsFiltered = retorno.Data.recordsFiltered;

                if (multiplaEscolha.permite) {
                    if (numeroRegistro <= limiteRegistro && numeroRegistro > 0)
                        $("#" + idDivBusca + "_btnSelecionarTodos").show();
                    else
                        $("#" + idDivBusca + "_btnSelecionarTodos").hide();
                }

                var resolucao = window.innerWidth || canvasEl.clientWidth;

                if (resolucao <= breakpointDefinition.phone) {
                    $.each(retorno.Data.header, function (i, head) {
                        if (head.phoneHide)
                            head.visible = false;
                    });
                }
                else if (resolucao <= breakpointDefinition.tablet) {
                    $.each(retorno.Data.header, function (i, head) {
                        if (head.tabletHide)
                            head.visible = false;
                    });
                }

                var headerDefault = retornarColunasPadroes(retorno.Data.header, menuOpcoes, callbackColumnDefault, habilitarExpansaoLinha);
                var initRows = new Array();
                var limit = 5;

                if (quantidadePorPagina != null)
                    limit = quantidadePorPagina;

                var NPages = limiteRegistro / limit;
                var pipeline = FnPipeline({
                    url: url,
                    pages: NPages,
                    method: "POST",
                    initCacheLastRequest: $.extend(true, {}, dadosGrid),
                    initCacheLastJson: $.extend(true, {}, json),
                    initCacheUpper: grid.start + limiteRegistro,
                    initCacheLower: grid.start,
                    initDadosPesquisa: dadosPesquisa
                });

                if (json.recordsTotal < limit)
                    limit = json.recordsTotal;

                todosRegistrosRetornados = $.extend(true, [], retorno.Data.data);

                if (manterPaginacao) {
                    var start = (paginaAtual * limit) - grid.start;
                    var end = start + limit;
                    if (end > retorno.Data.data.length)
                        end = retorno.Data.data.length;

                    for (var i = start; i < end; i++) {
                        if (retorno.Data.data[i] !== undefined)
                            initRows.push(retorno.Data.data[i]);
                    }
                }
                else {
                    for (var i = 0; i < limit; i++) {
                        if (retorno.Data.data[i] !== undefined)
                            initRows.push(retorno.Data.data[i]);
                    }
                }

                /**
                 * sDom -> DOM da tabela
                 * https://datatables.net/examples/basic_init/dom.html
                 *
                 * São 3 informações para ser usadas no rodapé
                 * - Informação da grid (col-3)
                 * - Botão de exportação (col-2)
                 * - Paginação (col-6)
                 */

                var btn_iconTemplate = '<i class="fal fa-file-excel"></i> ';
                var $btnExportar = $('<a href="#" id="' + configExportacao.id + '"  title="' + configExportacao.titulo + '" class="btn btn-sm btn-success waves-effect waves-themed">' + btn_iconTemplate + configExportacao.btnText + '</a>');
                var totalColunasRodapeGrid = 0;

                if (mostrarInfo && isExibirPaginacao)
                    totalColunasRodapeGrid++;

                if (configExportacao.exportar)
                    totalColunasRodapeGrid++;

                if (isExibirPaginacao)
                    totalColunasRodapeGrid++;

                var tamanhoColunasRodapeGrid = (totalColunasRodapeGrid > 0) ? 12 / totalColunasRodapeGrid : 0;

                var sDom =
                    '<"#' + idElemento + '-resize-container-parent.table-resize-container-parent"' +
                    '<"#' + idElemento + '-resize-container.table-resize-container' + ((scrollHorizontal && tamanhoPadraoPorColuna > 0) ? ' table-resize-container-scroll' : '') + '"' +
                    '<"#' + idElemento + '-resize-content.table-resize-content clearfix"' +
                    't' + // The 'T'able -> Tabela
                    '>' +
                    '>' +
                    '>' +
                    '<"dt-toolbar-footer' + (totalColunasRodapeGrid > 0 ? ' d-flex flex-wrap justify-content-between' : ' d-none') + '"' +
                    '<"' + (mostrarInfo ? ' align-self-center' : ' d-none ') + '"i>' + // 'I'nformation -> Exibindo x até x de x registros
                    (configExportacao.exportar ? ('<"btn-exportar align-self-center">') : '') + // Btn Exportacao
                    '<"' + (isExibirPaginacao ? ' align-self-center' : ' d-none') + '"p>' + // 'P'agination -> Lista de Paginação
                    '>';

                _onBeforeGridLoad(retorno.Data);

                if (loadCallback != null)
                    loadCallback(retorno.Data);

                _onAfterGridLoad(retorno.Data);

                if (_onGridData)
                    _onGridData(retorno.Data.data.slice());

                $('#' + idElemento + ' [data-toggle="grid-popover"]').each(function () {
                    let popover = bootstrap.Popover.getOrCreateInstance(this);
                    popover.dispose();
                });
                $('#' + idElemento).html("");

                table = $('#' + idElemento).DataTable({
                    "ajax": pipeline,
                    "autoWidth": false,
                    "bFilter": false,
                    "bLengthChange": false,
                    "bSort": true,
                    "colReorder": permitirReordenarColunas ? {
                        "reorderCallback": function () {
                            header = ReorganizarColunas(table, header);

                            cacheLastRequest.order[0].column = table.context[0].aaSorting[0][0];
                            cacheLastRequest.order[0].dir = table.context[0].aaSorting[0][1];

                            grid.header = header;

                            if (ordenacaoPadrao != null)
                                ordenacaoPadrao.column = buscarIndiceDataColuna(header, ColunaOrdenaPadrao);
                            else
                                ordenacaoPadrao = { column: table.context[0].aaSorting[0][0], dir: table.context[0].aaSorting[0][1] };

                            dadosGrid.order = cacheLastRequest.order;

                            if (permitirRedimencionarColunas)
                                iniciarColunaResizeble(idElemento);
                        }
                    } : false,
                    "columns": retorno.Data.header,
                    "columnDefs": headerDefault,
                    "data": initRows,
                    "deferLoading": retorno.Data.recordsTotal,
                    "destroy": true,
                    "drawCallback": function (settings) {
                        if (group.enable) {
                            var api = this.api();
                            var dadosColunaAgrupamento = api.column(buscarIndiceDataColuna(header, group.propAgrupa), { page: 'current' }).data();

                            if (dadosColunaAgrupamento) {
                                var rows = api.rows({ page: 'current' }).nodes();
                                var last = null;
                                var numeroCabecalho = 0;
                                for (var i = 0; i < retorno.Data.header.length; i++) {
                                    if (retorno.Data.header[i].visible)
                                        numeroCabecalho++;
                                }

                                if (menuOpcoes)
                                    numeroCabecalho++;

                                if (habilitarExpansaoLinha)
                                    numeroCabecalho++;

                                dadosColunaAgrupamento.each(function (grupo, i) {
                                    if (last !== grupo) {
                                        $(rows).eq(i).before('<tr class="group color-primary-900 bg-primary-50"><td colspan="' + numeroCabecalho + '"><b>' + grupo + '</b></td></tr>');

                                        last = grupo;
                                    }
                                });
                            }
                        }
                        if (typeof callbackDrawGridView === 'function')
                            callbackDrawGridView(settings, this.api(), scrollHorizontal);
                    },
                    "fnRowCallback": function (nRow, aData) {
                        if (aData.DT_PopoverContent) {
                            $(nRow).attr('data-bs-content', aData.DT_PopoverContent);
                            $(nRow).attr('data-bs-placement', 'top');
                            $(nRow).attr('title', aData.DT_PopoverTitle);
                            $(nRow).attr('data-bs-toggle', 'grid-popover');
                            $(nRow).attr('data-bs-trigger', 'hover focus');
                        }
                        else
                            adicionarToolTip(nRow);

                        setarCorDataRow(nRow, aData);
                        setarClasseDataRow(nRow, aData);
                        setarEnableDataRow(nRow, aData);

                        if (draggableRows && !(callbackOrdenacao instanceof Function)) {
                            var eventStop = draggableRows instanceof Function ? draggableRows : undefined;

                            $(nRow).draggable({
                                cursor: "move",
                                helper: function (event, ui) {
                                    var html = '';

                                    $(event.currentTarget).children().each(function (i, coluna) {
                                        var $coluna = $(coluna);

                                        html += '<td class="' + $coluna.attr('class') + '" style="width: ' + ($coluna.width() + 1) + 'px; max-width: ' + ($coluna.width() + 1) + 'px;">' + coluna.innerHTML + '</td>';
                                    });

                                    var corLinha = $(event.currentTarget).css("background-color");
                                    var corLinhaSelecionada = "#ecf3f8";
                                    var coresLinhaPadrao = ["#ffffff", "#F9F9F9"];
                                    var isCorPadrao = coresLinhaPadrao.indexOf(corLinha) > -1;

                                    return '<tr style="z-index: 5000; width: ' + $(event.currentTarget).width() + 'px; background-color: ' + (isCorPadrao ? corLinhaSelecionada : corLinha) + ';">' + html + '</tr>';
                                },
                                revert: 'invalid',
                                stop: eventStop
                            });
                        }

                        if (multiplaEscolha.permite) {

                            $(nRow).click(function (event) {
                                var cell = $(event.target).get(0); // This is the TD you clicked

                                if (cell.nodeName != 'TD')
                                    cell = $(cell).closest('td').get(0);

                                var head = retonarHeaderSelecionado(table, cell, header);

                                if (!multiplaEscolha.somenteLeitura && (head != null && (head.editableCell == null || !head.editableCell.editable))) {
                                    var id = this.id;
                                    var vCallbackSelecionado = false;
                                    var vCallbackNaoSelecionado = false;
                                    var selectedList = retornarListaSelecionados(multiplaEscolha);

                                    if (!existeRegistroNaoExisteNaBasicTable(selectedList, id)) {
                                        var index = retornaIndexMultiplaSelecao(selected, id);
                                        var indexUn = retornaIndexMultiplaSelecao(unselected, id);
                                        var item = table.row(this).data();

                                        if (multiplaEscolha.permitirSelecionarSomenteUmRegistro) {
                                            selected = new Array();
                                            unselected = new Array();

                                            if (multiplaEscolha.selecionados != null) {
                                                multiplaEscolha.naoSelecionados = new Array();
                                                multiplaEscolha.selecionados = new Array();
                                            }

                                            $('#' + idElemento + " tr").removeClass(multiplaEscolha.classeSelecao);
                                        }

                                        if (index === -1) {
                                            selected.push(item);

                                            if (!multiplaEscolha.permitirSelecionarSomenteUmRegistro)
                                                unselected.splice(indexUn, 1);

                                            if (multiplaEscolha.selecionados != null) {
                                                multiplaEscolha.selecionados.push(item);

                                                if (!multiplaEscolha.permitirSelecionarSomenteUmRegistro)
                                                    multiplaEscolha.naoSelecionados.splice(indexUn, 1);

                                                vCallbackSelecionado = true;
                                            }

                                            $(this).addClass(multiplaEscolha.classeSelecao);
                                        }
                                        else {
                                            if (!multiplaEscolha.permitirSelecionarSomenteUmRegistro) {
                                                unselected.push(item);
                                                selected.splice(index, 1);
                                            }

                                            if (multiplaEscolha.selecionados != null) {
                                                if (!multiplaEscolha.permitirSelecionarSomenteUmRegistro) {
                                                    multiplaEscolha.naoSelecionados.push(item);
                                                    multiplaEscolha.selecionados.splice(index, 1);
                                                }

                                                vCallbackNaoSelecionado = true;
                                            }

                                            $(this).removeClass(multiplaEscolha.classeSelecao);
                                        }
                                    }

                                    if (selected.length > 0) {
                                        $("#" + idDivBusca + "_btnConfirmarMultiplaEscolha").removeAttr("disabled");
                                        $("#" + idDivBusca + "_lblQtdeSelecionadosMultiplaEscolha").text(Localization.Resources.Gerais.Geral.XRegistrosSelecionados.format(selected.length));
                                    }
                                    else {
                                        $("#" + idDivBusca + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");
                                        $("#" + idDivBusca + "_lblQtdeSelecionadosMultiplaEscolha").text("");
                                    }

                                    // Todo: criar um evento e passar como primeiro paramentro nos callback para poder cancelar operção com o e.preventDefault()
                                    if (vCallbackSelecionado) {
                                        if (multiplaEscolha.callbackSelecionado != null)
                                            multiplaEscolha.callbackSelecionado(null, item);
                                    }
                                    else if (vCallbackNaoSelecionado) {
                                        if (multiplaEscolha.callbackNaoSelecionado != null)
                                            multiplaEscolha.callbackNaoSelecionado(null, item);
                                    }
                                }
                            });

                            $(nRow).dblclick(function () {
                                if (!multiplaEscolha.somenteLeitura && multiplaEscolha.basicTable != null) {
                                    var id = this.id;
                                    var item = table.row(this).data();
                                    var selectedList = retornarListaSelecionados(multiplaEscolha);
                                    if (!existeRegistroNaoExisteNaBasicTable(selectedList, id)) {
                                        if (selected.length > 0) {
                                            if (selected.length == 1 && selected[0] == item) {
                                                selected = new Array();
                                                selected.push(item);
                                                $("#" + idDivBusca + "_btnConfirmarMultiplaEscolha").trigger("click");
                                            } else {
                                                exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.VoceSelecionouOutrosRegistrosDesejaAdicionarSomenteEste, function (e) {
                                                    selected = new Array();
                                                    selected.push(item);
                                                    $("#" + idDivBusca + "_btnConfirmarMultiplaEscolha").trigger("click");
                                                });
                                            }
                                        } else {
                                            selected.push(item);
                                            $("#" + idDivBusca + "_btnConfirmarMultiplaEscolha").trigger("click");
                                        }
                                    }
                                }
                            });

                            if (multiplaEscolha.somenteLeitura) {
                                $(nRow).children().each(function (index, td) {
                                    if (td.firstChild == null || (td.firstChild.localName != "button" && td.firstChild.localName != "a" && td.firstChild.localName != "div"))
                                        $(this).addClass('disabled');
                                });
                                $(nRow).addClass('gridDisabled');
                                $(nRow).css('cursor', 'auto');
                            }
                            else {
                                $(nRow).css('cursor', 'pointer');
                            }

                            var selectedList = retornarListaSelecionados(multiplaEscolha);
                            for (var i = 0; i < selectedList.length; i++) {
                                var idRow;
                                if (selectedList[i].DT_RowId != null)
                                    idRow = selectedList[i].DT_RowId.toString();
                                else
                                    idRow = selectedList[i].Codigo.toString();
                                if (idRow == aData.DT_RowId.toString()) {
                                    $(nRow).addClass('disabled');
                                    $(nRow).css('cursor', 'auto');
                                }
                            }

                            selected = RetornarSelectedDefault(multiplaEscolha, todosRegistrosRetornados);
                            if (retornaIndexMultiplaSelecao(selected, aData.DT_RowId.toString()) !== -1) {
                                $(nRow).addClass(multiplaEscolha.classeSelecao);
                            }
                            else {
                                $(nRow).removeClass(multiplaEscolha.classeSelecao);
                            }
                        }

                        if (callbackRow instanceof Function)
                            callbackRow(nRow, aData, self);
                    },
                    "headerCallback": function (thead) {
                        adicionarToolTip(thead);
                    },
                    "iDisplayLength": limit,
                    "info": mostrarInfo,
                    "language": Language,
                    "order": [[dadosGrid.order[0].column, dadosGrid.order[0].dir]],
                    "paging": true,
                    "processing": false,
                    "sDom": sDom,
                    "search": false,
                    "serverSide": true,
                    "displayStart": manterPaginacao ? paginaAtual * limit : null
                });

                _onTableCreated(table);

                if (configExportacao.exportar) {
                    var retornarBotaoExportarEstadoOriginal = function () {
                        $btnExportar
                            .attr('disabled', false)
                            .removeClass('disabled')
                            .html(btn_iconTemplate + configExportacao.btnText);
                    };

                    $('#' + idElemento + '_wrapper .btn-exportar').append($btnExportar);

                    $btnExportar.on('click', function (e) {
                        if (e && e.preventDefault) e.preventDefault();

                        if (configExportacao.FiltroPesquisaSalvo == true) {
                            knout.FiltroPesquisa.val("");
                            dadosPesquisa = RetornarObjetoPesquisa(knout);
                        }

                        $btnExportar
                            .attr('disabled', true)
                            .addClass('disabled')
                            .html(configExportacao.btnLoading);

                        var dadosPesquisaExportacao = {};

                        $.extend(true, dadosPesquisaExportacao, dadosPesquisa, { Grid: JSON.stringify({ order: cacheLastRequest.order, tituloExportacao: configExportacao.titulo, modelo: modelo }) });

                        if (configExportacao.exportarPorRelatorio) {
                            $.extend(dadosPesquisaExportacao, { GridRelatorio: JSON.stringify(grid) });

                            executarReST(configExportacao.url, dadosPesquisaExportacao, function (retorno) {
                                retornarBotaoExportarEstadoOriginal();
                                try {
                                    if (retorno.Success)
                                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Você solicitou a geração do relatório, assim que terminar você será notificado");
                                    else
                                        exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
                                }
                                catch (err) {
                                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
                                }
                            });
                        } else {
                            executarDownload(configExportacao.url, dadosPesquisaExportacao, retornarBotaoExportarEstadoOriginal, function (html) {
                                retornarBotaoExportarEstadoOriginal();
                                try {

                                    //Expressão regular que remove os parênteses, se necessário e extrai apenas o conteúdo de dentro das tags <pre>
                                    var retorno = JSON.parse(html.replace(/^.*?\((.*?)\);.*$/, '$1').replace(/<.*?>/g, ''));

                                    if (retorno.Success)
                                        exibirMensagem(tipoMensagem.aviso, "Aviso!", retorno.Msg);
                                    else
                                        exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
                                }
                                catch (err) {
                                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
                                }
                            }, true, true);
                        }
                    });
                }

                $("#" + idElemento + "-resize-container").on("scroll", function () {
                    $("#" + idElemento + "-resize-content td .btn-group.open button").trigger("click");
                });

                $("#" + idElemento + "-resize-container").on('shown.bs.dropdown', function (e, f) {
                    var $listaOpcoes = $($(e.target).find(".dropdown-menu")[0]);
                    var coluna = e.target.offsetParent;
                    if (scrollHorizontal) $listaOpcoes.css({ top: coluna.offsetTop + coluna.offsetHeight + 3 + 'px' });
                })

                if (permitirEdicaoColunas) {
                    var html = '<div class="table-preferences-container">';

                    if (salvarPreferenciasGrid) {
                        html += '<div class="table-preferences-container-action">';
                        html += '    <div class="card mb-3">';
                        html += '        <div class="card-header">';
                        html += '            <button id="preferencias-' + idElemento + '" class="btn btn-primary waves-effect waves-themed table-preferences-configurations" data-bs-toggle="collapse" data-bs-target="#container-colunas-' + idElemento + '"><i id="preferencias-icone-' + idElemento + '" class="fal fa-cog"></i> ' + Localization.Resources.Gerais.Geral.Campos + '</button>';
                        html += '        </div>';
                    }

                    var todosCheckboxesMarcados = header.filter(h => h.title !== "").every(h => h.visible === true);

                    html += '<div id="container-colunas-' + idElemento + '" class="card-body table-preferences-container-columns collapse' + (!salvarPreferenciasGrid ? 'show' : '') + '">';
                    html += '    <div class="custom-control custom-checkbox table-preferences-check-uncheck">';
                    html += '        <input type="checkbox" id="marcar-desmarcar-colunas-' + idElemento + '" class="custom-control-input"' + (todosCheckboxesMarcados ? ' checked' : '') + '>';
                    html += '        <label for="marcar-desmarcar-colunas-' + idElemento + '" class="custom-control-label"> ' + Localization.Resources.Gerais.Geral.SelecionarTodos + ' </label>';
                    html += '    </div>';
                    html += '    <hr/>';

                    if (listTabs != null && listTabs.length > 0) {
                        html += '<ul class="nav nav-tabs">';
                        $.each(listTabs, function (i, tab) {
                            if (tab.name != "") {
                                let id = guid();
                                tab.id = id;
                                if (i == 0) {
                                    html += '   <li class="nav-item">';
                                    html += '       <a href="#tab' + id + '" class="nav-link active" data-bs-toggle="tab">';
                                    html += '           <span class="hidden-mobile hidden-tablet">' + tab.name + '</span>';
                                    html += '       </a>';
                                    html += '   </li>';
                                }
                                else {
                                    html += '   <li class="nav-item">';
                                    html += '       <a href="#tab' + id + '" class="nav-link" data-bs-toggle="tab">';
                                    html += '           <span class="hidden-mobile hidden-tablet">' + tab.name + '</span>';
                                    html += '       </a>';
                                    html += '   </li>';
                                }

                            }
                        });
                        html += '</ul>';

                        html += '<div class="tab-content border border-top-0 p-3 mb-4">';
                        $.each(listTabs, function (i, tab) {
                            if (tab.name != "") {
                                if (i == 0) {
                                    html += '<div class="tab-pane fade active show" id="tab' + tab.id + '" role="tabpanel">';
                                    html += '   <div class="row" style="margin-left: 1rem;">';
                                    $.each(header, function (i, cabecalho) {
                                        if (cabecalho.sTitle != "") {
                                            if (tab.name == cabecalho.tabName) {
                                                let id = guid();
                                                html += '       <div class="col-12 col-md-3 custom-control custom-checkbox" style="margin-bottom: 9px;">';
                                                html += '           <input type="checkbox"' + 'id="' + id + '" class="custom-control-input" ' + ((cabecalho.visible == null || cabecalho.visible) ? 'checked' : '') + ' data-column="' + cabecalho.data + '"/>';
                                                html += '               <label for="' + id + '" class="custom-control-label">' + cabecalho.title + '</label>';
                                                html += '       </div> ';
                                            }
                                        }
                                    });
                                    html += '   </div>';
                                    html += '</div>';
                                }
                                else {
                                    html += '<div class="tab-pane fade" id="tab' + tab.id + '" role="tabpanel">';
                                    html += '   <div class="row" style="margin-left: 1rem;">';
                                    $.each(header, function (i, cabecalho) {
                                        if (cabecalho.sTitle != "") {
                                            if (tab.name == cabecalho.tabName) {
                                                let id = guid();
                                                html += '       <div class="col-12 col-md-3 custom-control custom-checkbox" style="margin-bottom: 9px;">';
                                                html += '           <input type="checkbox"' + 'id="' + id + '" class="custom-control-input" ' + ((cabecalho.visible == null || cabecalho.visible) ? 'checked' : '') + ' data-column="' + cabecalho.data + '"/>';
                                                html += '               <label for="' + id + '" class="custom-control-label">' + cabecalho.title + '</label>';
                                                html += '       </div> ';
                                            }
                                        }
                                    });

                                    html += '   </div>';
                                    html += '</div>';
                                }
                            }
                        });
                        html += '</div>';
                    }

                    else {
                        html += '    <div class="checkboxes">';

                        $.each(header, function (i, cabecalho) {
                            if (cabecalho.sTitle != "") {
                                let id = guid();
                                html += '<div class="custom-control custom-checkbox">';
                                html += '  <input type="checkbox"' + 'id="' + id + '" class="custom-control-input" ' + ((cabecalho.visible == null || cabecalho.visible) ? 'checked' : '') + ' data-column="' + cabecalho.data + '"/>';
                                html += '  <label for="' + id + '" class="custom-control-label">' + cabecalho.title + '</label>';
                                html += '</div> ';
                            }
                        });
                        html += '    </div>';
                    }

                    if (salvarPreferenciasGrid) {

                        if (habilitarScrollHorizontal) {
                            var checkedHabilitarScrollHorizontal = (scrollHorizontal) ? " checked" : "";
                            html += '<div id="container-scroll-horizontal-' + idElemento + '" class="table-preferences-container-action table-preferences-container-action-footer hidden">';
                            html += '    <div class="row">';
                            html += '        <div class="col col-xs-12">';
                            html += '            <input type="checkbox" id="checkbox-habilitar-scroll-horizontal-' + idElemento + '"' + checkedHabilitarScrollHorizontal + '> <label for="checkbox-habilitar-scroll-horizontal-' + idElemento + '">Habilitar scroll horizontal</label>';
                            html += '        </div>';
                            html += '    </div>';
                            html += "</div>";
                        }

                        html += '<div id="container-acoes-' + idElemento + '" class="d-flex justify-content-end table-preferences-container-action table-preferences-container-action-footer hidden">';
                        html += '   <button id="restaurar-padrao-' + idElemento + '" class="btn btn-sm btn-default ms-2 waves-effect waves-themed"><i class="fal fa-undo"></i> ' + Localization.Resources.Gerais.Geral.RestaurarPadrao + '</button>';
                        html += '   <button id="salvar-preferencias-' + idElemento + '" class="btn btn-sm btn-success ms-2 waves-effect waves-themed"><i class="fal fa-save"></i> ' + Localization.Resources.Gerais.Geral.SalvarPreferenciasPorUsuario + '</button>';

                        if (modelosGrid)
                            html += '   <button id="gerenciar-modelos-grid-' + idElemento + '" class="btn btn-sm btn-success ms-2 waves-effect waves-themed"><i class="fal fa-cogs"></i> ' + Localization.Resources.Gerais.Geral.GerenciarModelos + '</button>';

                        html += "</div>";
                    }

                    if (salvarPreferenciasGrid) {
                        html += '    </div>';
                        html += "</div>";
                    }

                    html += '</div>';

                    html += '</div>';

                    $('#' + idElemento + "-resize-container-parent").before(html);

                    $("#marcar-desmarcar-colunas-" + idElemento).change(function () {
                        habilitarColunasCabecalho(this.checked);

                        if (callbackEdicaoColuna != null)
                            callbackEdicaoColuna();

                        if (permitirRedimencionarColunas && this.checked)
                            iniciarColunaResizeble(idElemento);
                    });

                    $('.table-preferences-container [data-column]').on('change', function (e) {
                        e.preventDefault();

                        var nomeColuna = $(this).attr('data-column');
                        var cabecalhoColuna = obterCabecalhoColunaPorNome(nomeColuna);
                        var coluna = table.column(nomeColuna + ':name');
                        var colunaVisivel = !coluna.visible();

                        if (colunaVisivel) {
                            var tamanhoPadraoTotalHeader = obterTamanhoPadraoTotalHeader();
                            var tamanhoColuna = parseFloat(cabecalhoColuna.widthDefault);

                            if (tamanhoPadraoTotalHeader > 0) {
                                var tamanhoPadraoPercentualColuna = (tamanhoColuna * 100) / (tamanhoPadraoTotalHeader + tamanhoColuna);
                                var markupDivisor = 100 / (100 - tamanhoPadraoPercentualColuna);

                                tamanhoColuna = tamanhoPadraoPercentualColuna * markupDivisor;
                            }

                            cabecalhoColuna.width = tamanhoColuna + "%";
                        }

                        coluna.visible(colunaVisivel);
                        cabecalhoColuna.visible = colunaVisivel;

                        recalcularTamanho();

                        if (callbackEdicaoColuna != null)
                            callbackEdicaoColuna();

                        if (permitirRedimencionarColunas)
                            iniciarColunaResizeble(idElemento);

                        var $all = $('.table-preferences-container [data-column]');
                        var $checked = $all.filter(':checked');
                        $('#marcar-desmarcar-colunas-' + idElemento).prop('checked', $checked.length === $all.length);
                    });

                    if (salvarPreferenciasGrid) {
                        $("#preferencias-" + idElemento).on('click', function (e) {
                            e.preventDefault();
                            $("#preferencias-icone-" + idElemento).toggleClass("fa-cog");
                            $("#preferencias-icone-" + idElemento).toggleClass("fa-times");
                            $("#container-colunas-" + idElemento).toggleClass("hidden");
                            $("#container-acoes-" + idElemento).toggleClass("hidden");
                            $("#container-scroll-horizontal-" + idElemento).toggleClass("hidden");
                        });

                        $("#salvar-preferencias-" + idElemento).on('click', function (e) {
                            e.preventDefault();

                            var data = {
                                columns: [],
                                scrollHorizontal: (habilitarScrollHorizontal) ? scrollHorizontal : false
                            };

                            for (var i = 0; i < header.length; i++) {
                                data.columns.push({
                                    name: header[i].name,
                                    visible: header[i].visible,
                                    width: header[i].width,
                                    position: header[i].position
                                });
                            }

                            executarReST(
                                "Preferencias/SalvarPreferenciasGrid",
                                {
                                    urlGrid: url,
                                    idGrid: idElemento,
                                    dadosGrid: JSON.stringify(data)
                                },
                                function (response) {
                                    if (response.Success) {
                                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                                        $("#preferencias-icone-" + idElemento).addClass("fa-cog");
                                        $("#preferencias-icone-" + idElemento).removeClass("fa-times");
                                        $("#container-colunas-" + idElemento).addClass("hidden");
                                        $("#container-acoes-" + idElemento).addClass("hidden");
                                        $("#container-scroll-horizontal-" + idElemento).addClass("hidden");
                                    }
                                },
                                null,
                                true
                            );
                        });

                        $("#restaurar-padrao-" + idElemento).on('click', function (e) {
                            e.preventDefault();
                            exibirConfirmacao(Localization.Resources.Gerais.Geral.RestaurarPadrao, Localization.Resources.Gerais.Geral.PreferenciasSeraoPerdidasDesejaContinuar, function (e) {
                                executarReST(
                                    "Preferencias/RestaurarPadraoGrid",
                                    {
                                        urlGrid: url,
                                        idGrid: idElemento
                                    },
                                    function (response) {
                                        if (response.Success) {
                                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                                            self.CarregarGrid();
                                        }
                                    },
                                    null,
                                    true
                                );
                            });
                        });

                        $("#gerenciar-modelos-grid-" + idElemento).on('click', function (e) {
                            e.preventDefault();

                            var data = {
                                columns: [],
                                scrollHorizontal: (habilitarScrollHorizontal) ? scrollHorizontal : false
                            };

                            for (var i = 0; i < header.length; i++) {
                                data.columns.push({
                                    name: header[i].name,
                                    visible: header[i].visible,
                                    width: header[i].width,
                                    position: header[i].position
                                });
                            }

                            _modeloGrid.UrlGrid.val(url);
                            _modeloGrid.IdGrid.val(idElemento);
                            _modeloGrid.DadosGrid.val(JSON.stringify(data));
                            _modeloGrid.CallbackAplicar = function (modelo) {
                                self.SetModelo(modelo);
                                self.CarregarGrid();
                            };
                            _modeloGrid.CallbackAplicarUsuario = function () {
                                self.SetModelo(0);
                                self.CarregarGrid();
                            };

                            Global.abrirModal("modal-modelo-grid");
                        });

                        if (habilitarScrollHorizontal) {
                            $("#checkbox-habilitar-scroll-horizontal-" + idElemento).on('change', function (e) {
                                self.SetScrollHorizontal($("#checkbox-habilitar-scroll-horizontal-" + idElemento).is(":checked"));
                                if (scrollHorizontal) $("#" + idElemento + "-resize-container").addClass("table-resize-container-scroll");
                                else $("#" + idElemento + "-resize-container").removeClass("table-resize-container-scroll");

                                recalcularTamanho();

                                if (callbackEdicaoColuna != null)
                                    callbackEdicaoColuna();

                                if (permitirRedimencionarColunas)
                                    iniciarColunaResizeble(idElemento);
                            });
                        }
                    }
                }

                if (permitirRedimencionarColunas) {
                    if (IsTouchDevice())
                        $("#" + idElemento).on("touchstart.resizeble", function () { iniciarColunaResizeble(idElemento); });

                    $("#" + idElemento).on("mouseenter.resizeble", function () { iniciarColunaResizeble(idElemento); });
                }

                if (!manterPaginacao)
                    recalcularTamanho();

                adicionarEventOpcoes(table, idElemento, menuOpcoes);

                if (callback != null)
                    callback();

                if (callbackOrdenacao instanceof Function)
                    ordenacaoGrid.criarOrdenacao();

                //if (existeHeaderBoolToggle(retorno.Data.header))
                //    $('.chk_toggle').bootstrapToggle({ on: Localization.Resources.Gerais.Geral.Sim, off: Localization.Resources.Gerais.Geral.Nao });

                //$('#' + idElemento + ' [data-toggle="grid-popover"]').popover({ trigger: 'hover', html: true });
                $('#' + idElemento + ' [data-bs-toggle="grid-popover"]').each(function () {
                    bootstrap.Popover.getOrCreateInstance(this, { html: true });
                });

            }
            else {
                finalizarControleManualRequisicao();
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                $('#' + idElemento).html("<thead><tr><th></th>t<tr></thead><tbody><tr><td class='dataTables_empty'>" + Localization.Resources.Gerais.Geral.FalhaPreencherTabela + "</td></tr></tbody>");
            }

            prom.done();
        }, null, exibirLoading);

        var selecaoTdExpandir = '> tbody > tr[role="row"] .row-exapand';
        $('#' + idElemento).off('click', selecaoTdExpandir, fnExpansaoLinhaHandle);
        if (habilitarExpansaoLinha) {
            $('#' + idElemento).on('click', selecaoTdExpandir, fnExpansaoLinhaHandle);
            $('#' + idElemento).addClass('dt-row-expandable');
        }

        var selecaoTrClicavel = '> tbody > tr[role="row"] td:not(.row-exapand)';
        $('#' + idElemento).off('click', selecaoTrClicavel, fnLinhaClicadaHandle);
        if (habilitarLinhaClicavel) {
            $('#' + idElemento).on('click', selecaoTrClicavel, fnLinhaClicadaHandle);
            $('#' + idElemento).addClass('dt-row-clickable');
        }

        return prom;
    };

    if (editarColuna.permite) {
        $('#' + idElemento).off('click', 'tbody td');
        $('#' + idElemento).on('click', 'tbody td', function (e) {
            var data = {};
            try {
                var row = $(this).parents('tr');
                data = table.row(row).data();
            } catch (e) {

            }

            let rowEditavel = (e.target.localName != "input" && editarColuna.permite && (editarColuna.functionPermite == null || editarColuna.functionPermite(data)));

            if (!rowEditavel) {
                //Para checkboxes:
                if (e.target.localName == "td" && e.target.firstChild.localName == "input" && e.target.firstChild.className == 'chk_toggle') {
                    e.target.firstChild.checked = !e.target.firstChild.checked;
                    rowEditavel = true;
                }
                if (e.target.localName == "input" && e.target.className == 'chk_toggle') {
                    rowEditavel = true;
                }
            }

            if (rowEditavel) {
                var head = retonarHeaderSelecionado(table, this, header);
                if (head != null && head.editableCell != null && head.editableCell.editable) {
                    e.stopPropagation();
                    dataRowAnterior = { dataRow: null };
                    EditarTableCellClick(this, table, head, header, editarColuna, dataRowAnterior);
                }
            }
        });
    }

    this.proximaPagina = function (repeat) {
        if (repeat && this.ObterPaginaAtual() == (table.page.info().pages - 1)) {
            table.page('first').draw('page');
        } else {
            table.page('next').draw('page');
        }
    }

    this.SetarPermissaoSelecionarSomenteUmRegistro = function (permitirSelecionarSomenteUmRegistro) {
        multiplaEscolha.permitirSelecionarSomenteUmRegistro = permitirSelecionarSomenteUmRegistro;
    };

    this.SetarRegistrosSomenteLeitura = function (somenteLeitura) {
        multiplaEscolha.somenteLeitura = somenteLeitura;
        infoMultiplaSelecao.somenteLeitura = somenteLeitura;
    };

    this.AtualizarRegistrosSelecionados = function (novosSelecionados) {
        if (infoMultiplaSelecao == null)
            return;

        multiplaEscolha.selecionados = novosSelecionados;
        infoMultiplaSelecao.selecionados = novosSelecionados;
    };

    this.AtualizarRegistrosNaoSelecionados = function (novosNaoSelecionados) {
        multiplaEscolha.naoSelecionados = novosNaoSelecionados;
    };

    //redesenha a tabela se recarrega-la
    this.DrawTable = function (manterPaginaAtual) {
        manterPaginaAtual = !!manterPaginaAtual;
        var paginaAtual = 0;

        if (manterPaginaAtual)
            paginaAtual = this.ObterPaginaAtual();

        if (table != null)
            table.draw();

        if (manterPaginaAtual)
            this.SetarPagina(paginaAtual);
    };

    this.Clear = function () {
        if (table != null)
            table.clear();
    };

    this.ObterMultiplosSelecionados = function () {
        return multiplaEscolha.selecionados;
    };

    this.ObterMultiplosNaoSelecionados = function () {
        return multiplaEscolha.naoSelecionados;
    };

    this.ObterCodigosMultiplosSelecionados = function () {
        var codigos = [];

        multiplaEscolha.selecionados.forEach(function (registro) {
            codigos.push(registro.Codigo);
        });

        return codigos;
    };

    this.ObterCodigosMultiplosNaoSelecionados = function () {
        var codigos = [];

        multiplaEscolha.naoSelecionados.forEach(function (registro) {
            codigos.push(registro.Codigo);
        });

        return codigos;
    };

    if (multiplaEscolha.permite) {
        var idDivBusca = idElemento.split("_")[0];
        var marcouTodos;

        if (multiplaEscolha.callback == null) {
            infoMultiplaSelecao.eventos.multiploEventClick = function () {
                var arrayEscolhidos = new Array();
                for (var i = 0; i < multiplaEscolha.basicTable.BasicTable().rows().data().length; i++) {
                    arrayEscolhidos.push(multiplaEscolha.basicTable.BasicTable().rows().data()[i]);
                }
                var colunasNaoEncontradas = [];
                for (var i = 0; i < selected.length; i++) {
                    var obj = new Object();
                    for (var j = 0; j < multiplaEscolha.basicTable.BasicTable().columns().data().context[0].aoColumns.length; j++) {
                        var indiceTabela = multiplaEscolha.basicTable.BasicTable().columns().data().context[0].aoColumns[j].data;
                        if (indiceTabela != null) {
                            obj[indiceTabela] = selected[i][indiceTabela];
                            if (obj[indiceTabela] == null) {
                                colunasNaoEncontradas.push(indiceTabela);
                            }
                        }
                    }
                    if (colunasNaoEncontradas.length == 0) {
                        arrayEscolhidos.push(obj);
                    } else {
                        break;
                    }
                }

                if (colunasNaoEncontradas.length == 0) {
                    multiplaEscolha.basicTable.CarregarGrid(arrayEscolhidos);
                    $("#" + idDivBusca).modal('hide');
                    $("#" + idDivBusca + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");
                    $("#" + idDivBusca + "_lblQtdeSelecionadosMultiplaEscolha").text("");
                    selected = new Array();
                    if (multiplaEscolha.afterDefaultCallback != null)
                        multiplaEscolha.afterDefaultCallback(selected);
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, "A seleção não retornou o(s) campo(s) " + colunasNaoEncontradas.join(", ") + ". Solicitar a Multisoftware que adicione o(s) campo(s) no retorno, ou crie um callback exclusivo para os dados retornados");
                }
            };
        } else {
            infoMultiplaSelecao.eventos.multiploEventClick = function () {
                multiplaEscolha.callback(selected);
                $("#" + idDivBusca).modal('hide');
                $("#" + idDivBusca + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");
                $("#" + idDivBusca + "_lblQtdeSelecionadosMultiplaEscolha").text("");
                selected = new Array();
            };
        }

        if (multiplaEscolha.SelecionarTodosKnout != null) {
            selecionarTodos = multiplaEscolha.SelecionarTodosKnout.val();
            multiplaEscolha.SelecionarTodosKnout.eventClick = function () {
                if (!multiplaEscolha.somenteLeitura) {
                    selected = new Array();
                    unselected = new Array();
                    multiplaEscolha.naoSelecionados = new Array();
                    multiplaEscolha.selecionados = new Array();
                    var selecionado = multiplaEscolha.SelecionarTodosKnout.val();

                    if (!selecionado) {
                        multiplaEscolha.SelecionarTodosKnout.val(true);
                        if (todosRegistrosRetornados.length <= limiteRegistro) {
                            selected = RetornarSelectedDefault(multiplaEscolha, todosRegistrosRetornados);
                            multiplaEscolha.selecionados = selected;
                        }
                    } else {
                        multiplaEscolha.SelecionarTodosKnout.val(false);
                    }
                    table.draw();
                }
                if (multiplaEscolha.callbackSelecionado != null)
                    multiplaEscolha.callbackSelecionado();
            };
        }

        $("#" + idDivBusca).on("hidden.bs.modal", function () {
            $("#" + idDivBusca + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");
            $("#" + idDivBusca + "_lblQtdeSelecionadosMultiplaEscolha").text("");
            marcouTodos = false;
            selected = new Array();
        });

        infoMultiplaSelecao.eventos.selecionarTodosEventClick = function (e) {
            if (marcouTodos) {
                selected = new Array();
                multiplaEscolha.selecionados = selected;
                table.draw();
                marcouTodos = false;
                $("#" + idDivBusca + "_btnConfirmarMultiplaEscolha").attr("disabled", "disabled");
                $("#" + idDivBusca + "_lblQtdeSelecionadosMultiplaEscolha").text("");
                return;
            }

            if (todosRegistrosRetornados.length <= limiteRegistro) {
                for (var i = 0; i < todosRegistrosRetornados.length; i++) {
                    if (retornaIndexMultiplaSelecao(selected, todosRegistrosRetornados[i].DT_RowId) == -1 && !existeRegistroNaoExisteNaBasicTable(multiplaEscolha.basicTable.BasicTable().rows().data(), todosRegistrosRetornados[i].DT_RowId)) {
                        selected.push(todosRegistrosRetornados[i]);
                    }
                }
                if (selected.length > 0) {
                    multiplaEscolha.selecionados = selected;
                    table.draw();
                    $("#" + idDivBusca + "_btnConfirmarMultiplaEscolha").removeAttr("disabled");
                    $("#" + idDivBusca + "_lblQtdeSelecionadosMultiplaEscolha").text(Localization.Resources.Gerais.Geral.XRegistrosSelecionados.format(selected.length));
                    marcouTodos = true;
                }
            }
        };
    }

    this.GridViewTable = function () {
        return table;
    };

    this.GridViewTableData = function () {
        return _tableAllData.data;
    };

    this.Destroy = function () {
        if (table != null) {
            _onTableDestroy(table);
            table.destroy();
        }
    };

    this.obterDataRow = function (id) {
        if (!table)
            return null;

        var row = table.row("#" + id);

        if (row.length == 0)
            return null;

        return {
            data: row.data(),
            row: $(row.node())
        };
    };

    this.setarCorGridPorID = function (id, cor) {
        var dataRow = this.obterDataRow(id);

        if (dataRow) {
            dataRow.data.DT_RowColor = cor;
            setarCorDataRow(dataRow.row, dataRow.data);
        }
    };

    this.setarClassGridPorID = function (id, classe) {
        let dataRow = this.obterDataRow(id);

        if (dataRow) {
            dataRow.data.DT_RowClass = classe;
            setarClasseDataRow(dataRow.row, dataRow.data);
        }
    };

    this.AtualizarDataRow = function (rowPar, DataRow, callbackTabPress) {
        AtualizarDataRow(table, rowPar, DataRow, callbackTabPress);
        AtualizarDadosEmCache(DataRow);
    };

    this.DesfazerAlteracaoDataRow = function (rowPar) {
        AtualizarDataRow(table, rowPar, dataRowAnterior.dataRow);
        AtualizarDadosEmCache(dataRowAnterior.dataRow);
    };

    this.SetarPagina = function (pagina) {
        if (!table) return;

        table.page(pagina).draw('page');
    };

    this.ObterPaginaAtual = function () {
        if (table) {
            var paginaAtual = table.page.info().page;

            return isNaN(paginaAtual) ? 0 : paginaAtual;
        }

        return -1;
    };

    this.ObterTotalRegistrosEmExibicao = function () {
        if (table) {
            var totalRegistros = table.page.info().recordsTotal;

            return isNaN(totalRegistros) ? null : totalRegistros;
        }

        return null;
    };

    this.GetColumnIndex = function (columnName) {
        var indice = -1;

        for (var i = 0; i < header.length; i++) {
            if (header[i].visible) {
                indice++;

                if (header[i].name == columnName)
                    return indice;
            }
        }

        return undefined;
    };

};

var GridViewOrdenacao = function (idElemento, quantidadePorPagina, callbackOrdenacao, callbackOrdenacaoFinalizada) {
    this._idElemento = idElemento;
    this._quantidadePorPagina = isNaN(quantidadePorPagina) ? 5 : quantidadePorPagina;
    this._callbackOrdenacao = callbackOrdenacao;
    this._callbackOrdenacaoFinalizada = callbackOrdenacaoFinalizada;
    this._itemOrdenadoForaGrid = false;
    this._ordenacaoParada = false;
    this._posicaoAnterior;
};

GridViewOrdenacao.prototype = {
    constructor: GridViewOrdenacao,
    criarOrdenacao: function () {
        var self = this;

        $("#" + self._idElemento + " tbody").sortable({
            distance: 3,
            helper: function (event, ui) { return self._ajustarTamanhoColunas(ui); },
            out: function () {
                if (!self._ordenacaoParada)
                    self._itemOrdenadoForaGrid = true;
            },
            over: function () {
                self._itemOrdenadoForaGrid = false;
            },
            start: function (event, ui) {
                self._itemOrdenadoForaGrid = false;
                self._ordenacaoParada = false;
                self._armazenarPosicaoAnterior(ui);
            },
            beforeStop: function () {
                self._ordenacaoParada = true;
            },
            stop: function (event, ui) {
                if (!self._itemOrdenadoForaGrid)
                    return;

                if (self._callbackOrdenacaoFinalizada instanceof Function)
                    self._callbackOrdenacaoFinalizada(ui);
            },
            update: function (event, ui) {
                if (self._itemOrdenadoForaGrid) {
                    self._reverterOrdenacao();
                    return;
                }

                self._atualizarPosicoes(this, ui);
            }
        });
    },
    _ajustarTamanhoColunas: function (ui) {
        var html = '';

        $(ui).children().each(function (i, coluna) {
            var $coluna = $(coluna);

            html += '<td class="' + $coluna.attr('class') + '" style="width: ' + ($coluna.width() + 1) + 'px; max-width: ' + ($coluna.width() + 1) + 'px;">' + coluna.innerHTML + '</td>';
        });

        var corLinha = $(ui).css("background-color");
        var corLinhaSelecionada = "#ecf3f8";
        var coresLinhaPadrao = ["#ffffff", "#F9F9F9"];
        var isCorPadrao = coresLinhaPadrao.indexOf(corLinha) > -1;

        return '<tr style="z-index: 5000; width: ' + $(ui).width() + 'px; background-color: ' + (isCorPadrao ? corLinhaSelecionada : corLinha) + ';">' + html + '</tr>';
    },
    _armazenarPosicaoAnterior: function (ui) {
        var indiceUltimoRegistroPaginaAnterior = this._obterIndiceUltimoRegistroPaginaAnterior();
        var indiceLinhaReposicionada = this._obterIndiceLinhaReposicionada(ui);

        this._posicaoAnterior = indiceLinhaReposicionada + indiceUltimoRegistroPaginaAnterior;
    },
    _atualizarPosicoes: function (tabela, ui) {
        var self = this;
        var indiceUltimoRegistroPaginaAnterior = self._obterIndiceUltimoRegistroPaginaAnterior();
        var indiceLinhaReposicionada = self._obterIndiceLinhaReposicionada(ui);
        var posicaoLinhaReordenada;
        var listaPosicaoRegistro = new Array();

        $(tabela).children().each(function (indice, linha) {
            var indiceLinhaAtual = indice + 1;
            var posicao = (indiceLinhaAtual + indiceUltimoRegistroPaginaAnterior);

            if (indiceLinhaAtual === indiceLinhaReposicionada)
                posicaoLinhaReordenada = posicao;

            listaPosicaoRegistro.push({
                idLinha: linha.id,
                posicao: posicao
            });
        });

        self._callbackOrdenacao(
            {
                itemReordenado: { idLinha: ui.item[0].id, posicaoAnterior: self._posicaoAnterior, posicaoAtual: posicaoLinhaReordenada },
                listaRegistrosReordenada: listaPosicaoRegistro
            },
            function () { self._reverterOrdenacao(); }
        );
    },
    _obterIndiceLinhaReposicionada: function (ui) {
        return $("#" + this._idElemento + " tr").index(ui.item[0]);
    },
    _obterIndiceUltimoRegistroPaginaAnterior: function () {
        var paginaAtual = $("#" + this._idElemento + "_paginate ul li.active a").text();

        if (isNaN(paginaAtual))
            paginaAtual = 1;

        return (paginaAtual - 1) * this._quantidadePorPagina;
    },
    _reverterOrdenacao: function () {
        $("#" + this._idElemento + " tbody").sortable("cancel");
    }
};

function retonarHeaderSelecionado(table, cell, header) {
    if (table.cell(cell).index() == undefined)
        return;
    var idx = table.cell(cell).index().column;
    var head = header[idx];
    return head;
}

function EditarTableCellClick(cell, table, head, header, editarColuna, dataRowAnterior) {
    var row = $(cell).parent();
    var rowData = table.row(row).data();
    var executou = false;

    if (rowData.DT_Enable) {
        executou = true;

        dataRowAnterior.dataRow = jQuery.extend({}, rowData);

        var idTxt = guid();
        htmlItemEdicao = cell.innerHTML;
        valorItemEdicao = cell.innerText;

        if (head.editableCell.type != EnumTipoColunaEditavelGrid.bool) {

            if (head.editableCell.conditions !== undefined) {
                //var valorCondicao = head.editableCell.conditions.value;
                //var valorDataRow = rowData[head.editableCell.conditions.data];
                //if (valorCondicao !== valorDataRow)
                //    return;

                if (head.editableCell.conditions instanceof Function) {
                    if (!head.editableCell.conditions(rowData))
                        return;
                }
            }

            var html = '<input id="' + idTxt + '" type="text" class="' + head.className + '" ';

            if (head.editableCell.maxlength > 0) {
                html += ' maxlength="' + head.editableCell.maxlength + '" ';
            }
            html += 'style="width: 100%;';
            html += 'margin: -10px 0px -11px 0px;';
            html += 'padding:  5px;';
            html += 'color:  #000;';
            html += 'background-color: #ffffee;';
            html += 'box-sizing: border-box;';
            html += 'border: 2px solid #B0C4DE;';
            html += 'border-radius: 3px;" />';
            cell.innerHTML = html;

            let $txt = $("#" + idTxt);
            $txt.unbind();

            switch (head.editableCell.type) {
                case EnumTipoColunaEditavelGrid.int:
                    if (head.editableCell.numberMask.precision === 0) {
                        valorItemEdicao = parseInt(valorItemEdicao);
                    }

                    var configInt = ConfigInt({ precision: head.editableCell.numberMask.precision, allowZero: head.editableCell.numberMask.allowZero });

                    if (!head.editableCell.numberMask.thousandsSeparator)
                        configInt.thousands = "";

                    $txt.maskMoney(configInt);
                    break;
                case EnumTipoColunaEditavelGrid.decimal:
                    var configDecimal = ConfigDecimal({ precision: head.editableCell.numberMask.precision, allowZero: head.editableCell.numberMask.allowZero });

                    if (!head.editableCell.numberMask.thousandsSeparator)
                        configDecimal.thousands = "";

                    $txt.maskMoney(configDecimal);
                    break;
                case EnumTipoColunaEditavelGrid.data:

                    SetTempusDominusToRawInput($txt, typesKnockout.date);

                    break;
                case EnumTipoColunaEditavelGrid.dateTime:

                    SetTempusDominusToRawInput($txt, typesKnockout.dateTime);

                    break;
                case EnumTipoColunaEditavelGrid.mask:
                    $txt.mask(head.editableCell.mask);
                    break;
                default:
                    break;
            }

            $txt.dispose = function () {
                if ($txt.tempusDominusInstance) {
                    $txt.tempusDominusInstance.dispose();
                    $txt.tempusDominusInstance = null;
                }
            }

            $txt.focus(function () {
                $($txt).select();
            });

            $txt.val(valorItemEdicao);

            if ($txt.updateValue)
                $txt.updateValue();

            $txt.focus();

            $txt.keydown(function (e) {
                var rowIndex = row.index();
                var headIndex = RetornarIndiceHead(header, head);
                var keyCode = e.keyCode || e.which;
                if (keyCode == 27) {//esc
                    $txt.unbind();
                    $txt.dispose();
                    AtualizarDataRow(table, row, rowData);
                } else if (!e.shiftKey && keyCode == 9) {//tab
                    $txt.unbind();
                    $txt.dispose();
                    e.preventDefault();
                    ExecutarAlteracarRowEditable(rowData, head, idTxt, editarColuna, row, table, function () {
                        for (var i = headIndex + 1; i < header.length; i++) {
                            if (ExecutarChamadaNovaCelula(table, header, editarColuna, dataRowAnterior, rowIndex, i))
                                break;
                            if (i == header.length - 1) {
                                rowIndex++;
                                if (table.context[0].aoData[rowIndex] != null) {
                                    i = -1;
                                }
                            }
                        }
                    });
                } else if (e.shiftKey && keyCode == 9) {//shift tab
                    $txt.unbind();
                    $txt.dispose();
                    e.preventDefault();
                    ExecutarAlteracarRowEditable(rowData, head, idTxt, editarColuna, row, table, function () {
                        for (var i = headIndex - 1; i >= 0; i--) {
                            if (ExecutarChamadaNovaCelula(table, header, editarColuna, dataRowAnterior, rowIndex, i))
                                break;
                            if (i == 0) {
                                rowIndex--;
                                if (table.context[0].aoData[rowIndex] != null) {
                                    i = header.length;
                                }
                            }
                        }
                    });
                } else if (keyCode == 38) {//up
                    $txt.unbind();
                    $txt.dispose();
                    e.preventDefault();
                    ExecutarAlteracarRowEditable(rowData, head, idTxt, editarColuna, row, table, function () {
                        while (true) {
                            rowIndex--;
                            if (table.context[0].aoData[rowIndex] != null) {
                                if (ExecutarChamadaNovaCelula(table, header, editarColuna, dataRowAnterior, rowIndex, headIndex))
                                    break;
                            } else
                                break;
                        }
                    });
                } else if (keyCode == 40) {//down
                    $txt.unbind();
                    $txt.dispose();
                    e.preventDefault();
                    ExecutarAlteracarRowEditable(rowData, head, idTxt, editarColuna, row, table, function () {
                        while (true) {
                            rowIndex++;
                            if (table.context[0].aoData[rowIndex] != null) {
                                if (ExecutarChamadaNovaCelula(table, header, editarColuna, dataRowAnterior, rowIndex, headIndex))
                                    break;
                            }
                            else
                                break;
                        }
                    });
                }
            });

            $txt.focusout(function (e) {
                $txt.dispose();
                e.preventDefault();
                ExecutarAlteracarRowEditable(rowData, head, idTxt, editarColuna, row, table);
            });
        } else if (head.editableCell.type == EnumTipoColunaEditavelGrid.bool) {
            if ($(cell).closest('tr').find('input:checkbox')[0].checked != rowData[head.data])
                rowData[head.data] = !rowData[head.data];
            if (editarColuna.atualizarRow) {
                AtualizarDataRow(table, row, rowData);
            }
            if (editarColuna.callback != null) {
                //rowData = Dados da Linha, Row = linha na tabela, head = cabeçalho da propriedade editada
                editarColuna.callback(rowData, row, head, null, table);
            }
        }
    }
    return executou;
}

function ExecutarChamadaNovaCelula(table, header, editarColuna, dataRowAnterior, rowIndex, cellIndex) {
    var executou = false;
    var head = header[cellIndex];
    if (head.editableCell != null && head.editableCell.editable) {
        var cellTab = table.context[0].aoData[rowIndex].anCells[cellIndex];
        if (cellTab != null) {
            executou = EditarTableCellClick(cellTab, table, head, header, editarColuna, dataRowAnterior);
        }
    }
    return executou;
}

function RetornarIndiceHead(header, head) {
    return RetornarIndiceHeadByName(header, head.data);
}

function RetornarIndiceHeadByName(header, name) {
    for (var i = 0; i < header.length; i++) {
        if (header[i].data == name) {
            return i;
        }
    }
}

function RetornarIndiceVisivel(header, indice) {
    var contVisivel = -1;
    for (var i = 0; i <= indice; i++) {
        if (header[i].visible != false) {
            contVisivel++;
        }
    }
    return contVisivel;
}

function ExecutarAlteracarRowEditable(rowData, head, idTxt, editarColuna, row, table, callbackTabPress) {
    var oldValue = rowData[head.data];
    var newValue = $("#" + idTxt).val();

    if (head.editableCell.type == EnumTipoColunaEditavelGrid.int) {
        if (head.editableCell.numberMask.precision === 0) {
            oldValue = parseInt(oldValue);
            newValue = parseInt(newValue);
        }
    }

    if (oldValue != newValue) {
        rowData[head.data] = $("#" + idTxt).val();
        if (editarColuna.atualizarRow) {
            AtualizarDataRow(table, row, rowData, callbackTabPress);
        }
        if (editarColuna.callback != null) {
            //rowData = Dados da Linha, Row = linha na tabela, head = cabeçalho da propriedade editada
            editarColuna.callback(rowData, row, head, callbackTabPress, table);
        }
    } else {
        AtualizarDataRow(table, row, rowData, callbackTabPress, true);
    }
}

function CompararEAtualizarGridEditableDataRow(dataRow, novaRow) {
    $.each(novaRow, function (i, obj) {
        if (dataRow[i] != null) {
            dataRow[i] = obj;
        }
    });
}

function AtualizarDataRow(table, row, data, callbackTabPress) {
    if (table == null)
        return;

    if (row == null || row.length <= 0)
        return;

    if (data == null)
        return;

    table.row(row).data(data);
    setarCorDataRow(row, data);
    setarClasseDataRow(row, data);
    setarEnableDataRow(row, data);

    if (callbackTabPress != null)
        setTimeout(callbackTabPress, 50);
}

function setarCorDataRow(row, data) {
    if (data.DT_RowColor != "") {
        $(row).css('background-color', data.DT_RowColor);

        if (data.DT_FontColor)
            $(row).children('td').css('color', data.DT_FontColor);
        else
            $(row).children('td').css('color', '');
    }
    else {
        $(row).css('background-color', '');
        $(row).children('td').css('color', '');
    }
}

function setarClasseDataRow(row, data) {
    if (data.DT_RowClass)
        $(row).addClass(data.DT_RowClass);
}

function setarEnableDataRow(row, data) {
    if (data.DT_Enable) {
        $(row).children().each(function (index, td) {
            $(this).removeClass('disabled');
        });
    } else {
        $(row).children().each(function (index, td) {
            if (td.firstChild == null || (td.firstChild.localName != "button" && td.firstChild.localName != "a" && td.firstChild.localName != "div"))
                $(this).addClass('disabled');
        });
    }
}

function RetornarSelectedDefault(multiplaEscolha, todosRegistrosRetornados) {
    var selecionados = new Array();
    var selecionado = false;

    if (multiplaEscolha.SelecionarTodosKnout != null) {
        selecionado = multiplaEscolha.SelecionarTodosKnout.val()
    }

    if (selecionado) {
        for (var i = 0; i < todosRegistrosRetornados.length; i++) {
            if (!existeRegistroNaoExisteNaBasicTable(multiplaEscolha.naoSelecionados, todosRegistrosRetornados[i].DT_RowId)) {
                selecionados.push(todosRegistrosRetornados[i]);
            }
        }
    } else {
        $.each(multiplaEscolha.selecionados, function (i, objetoSele) {
            selecionados.push(jQuery.extend(true, {}, objetoSele));
        });
    }
    return selecionados;
}

function retornarListaSelecionados(multiplaEscolha) {
    var listaSelecionados = new Array();
    if (multiplaEscolha.basicTable != null && Boolean(multiplaEscolha.basicTable.BasicTable))
        listaSelecionados = multiplaEscolha.basicTable.BasicTable().rows().data();
    else {
        //listaSelecionados = multiplaEscolha.selecionados;
    }
    return listaSelecionados;
}

function existeRegistroNaoExisteNaBasicTable(rowsBasicTable, id) {

    var existe = false;
    for (var i = 0; i < rowsBasicTable.length; i++) {
        if (rowsBasicTable[i].Codigo.toString() == id.toString()) {
            existe = true;
            break;
        }
    }
    return existe;
}

function retornaIndexMultiplaSelecao(selected, id) {
    var index = -1;
    for (var i = 0; i < selected.length; i++) {
        if (selected[i].DT_RowId.toString() == id) {
            index = i;
            break;
        }
    }
    return index;
}

function buscarIndiceDataColuna(header, propAgrupar) {
    var indice = 0;
    $.each(header, function (i, cabecalho) {
        if (cabecalho.data == propAgrupar) {
            indice = cabecalho.position;
            return false;
        }
    });
    return indice;
}

function ReorganizarColunas(table, header) {
    headerResize = header.slice();
    var totalHeader = header.length;
    if (table != null) {
        var ordem = table.colReorder.order();
        for (var i = 0; i < ordem.length; i++)
            if (i < totalHeader)
                headerResize[ordem[i]].position = i;
    }

    return headerResize;
}

function existeColumnFilterSelect(headers) {
    var existe = false;
    if (headers) {
        headers.forEach(function (header) {
            if (header.filter) {
                existe = true;
            }
        });
    }
    return existe;
}

function existeColumnSum(headers) {
    var existe = false;
    if (headers) {
        headers.forEach(function (header) {
            if (header.sum) {
                existe = true;
            }
        });
    }
    return existe;
}

function DataTableInitComplete(api, header) {
    api.columns().every(function (i) {
        var column = this;
        var index = RetornarIndiceHeadByName(header, column.data().context[0].aoColumns[i].data);
        if (index >= 0) {
            if (header[index].filter) {

                $(column.header()).append("<br/>");
                var select = $('<select style="width: 90%"><option value=""></option></select>').on('click', function (e) { e.stopPropagation(); })
                    //.appendTo($(column.footer()).empty())
                    .appendTo($(column.header()))
                    .on('change', function () {
                        var val = $.fn.dataTable.util.escapeRegex(
                            $(this).val()
                        );
                        column.search(val ? '^' + val + '$' : '', true, false).draw();
                    });

                column.data().unique().sort().each(function (d, j) {
                    select.append('<option value="' + d + '">' + d + '</option>')
                });
            }
        }
    });
}

function StopPropogation(e) {
    e.stopPropagation();
}

function SortByPosition(a, b) {
    var aName = a.position;
    var bName = b.position;
    return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
}

var GridReordering = function (info, idContent, headHTML, bodyHTML, callback) {
    var dataTable;
    var headHTML;
    var HTMLBase = '<table width="100%" class="table table-bordered table-hover" id="tableorder_' + idContent + '" >';
    HTMLBase += '<thead>' + headHTML + '</thead>';
    HTMLBase += '<tbody></tbody>';
    HTMLBase += '</table>';
    $("#" + idContent).html(HTMLBase);
    $("#tableorder_" + idContent + " tbody").html(bodyHTML);

    dataTable = this.CarregarGrid = function () {
        dataTable = $("#tableorder_" + idContent).DataTable({
            "destroy": true,
            "bSort": true,
            "bFilter": false,
            "bLengthChange": false,
            "paging": false,
            "info": true,
            "search": false,
            "language": { "info": info, "zeroRecords": Localization.Resources.Gerais.Geral.NenhumRegistroEncontrado, "infoEmpty": "" }
        });
        dataTable.draw();
        dataTable.rowReordering();
    };

    this.ObterOrdencao = function () {
        var lista = new Array();
        $(dataTable.rows().nodes()).each(function (i) {
            var item = new Object();
            item.id = this.id;
            item.posicao = parseInt(dataTable.row(this).data()[0]);
            lista.push(item);
        });
        return lista;
    };

    this.RecarregarGrid = function (bodyHTML) {
        $("#" + idContent).html("");
        $("#" + idContent).html(HTMLBase);
        $("#tableorder_" + idContent + " tbody").html(bodyHTML);
        this.CarregarGrid();
    };

    this.LimparGrid = function () {
        $("#" + idContent).html("");
        $("#" + idContent).html(HTMLBase);
        $("#tableorder_" + idContent + " tbody").html("");
        this.CarregarGrid();
    };

};

var _api;

var BasicDataTable = function (idElemento, header, menuOpcoes, ordenacaoPadrao, configRowsSelect, quantidadePorPagina, mostrarInfo, isExibirPaginacao, infoEditarColuna, callbackOrdenacao, draggableRows, ordenacaoFixa, configExportacao, callbackRow, callbackColumnDefault, esconderColunasVisibleFalseAoPermitirEdicaoColunas, callbackRegistroSelecionadoChange, callbackSelecionarTodos, url, gridPreferencias) {
    var self = this;

    configExportacao = $.extend({
        exportar: configExportacao != null,
        url: "",
        titulo: "",
        id: guid(),
        btnText: Localization.Resources.Gerais.Geral.Exportar + " Excel",
        btnLoading: Localization.Resources.Gerais.Geral.Exportando + "...",
        funcaoObterParametros: undefined
    }, configExportacao);

    var editarColuna = { permite: false, callback: null, atualizarRow: true };

    if (infoEditarColuna != null) {
        editarColuna.permite = infoEditarColuna.permite;
        editarColuna.callback = infoEditarColuna.callback;
        editarColuna.atualizarRow = infoEditarColuna.atualizarRow;
    }

    var table;
    var listaSelecionadas = new Array();
    var Registros = new Array();
    var ordenacaoGrid;
    var tamanhoPadraoPorColuna = 0;

    if (mostrarInfo == null)
        mostrarInfo = true;

    isExibirPaginacao = isExibirPaginacao != false;

    if (callbackOrdenacao instanceof Function)
        ordenacaoGrid = new GridViewOrdenacao(idElemento, quantidadePorPagina, callbackOrdenacao);

    this.setTamanhoPadraoPorColuna = function (tamanho) {
        tamanhoPadraoPorColuna = tamanho;
    }

    header.forEach(function (cabecalhoColuna) {
        cabecalhoColuna.name = cabecalhoColuna.data;
    });

    var definirTamanhoMinimoTabela = function () {
        if (tamanhoPadraoPorColuna > 0) {
            var cabecalhos = header.slice();
            var totalColunasVisiveis = 0;

            for (var i = 0; i < cabecalhos.length; i++) {
                if (cabecalhos[i].visible)
                    totalColunasVisiveis++;
            }

            $("#" + idElemento + "-resize-content").css("min-width", (totalColunasVisiveis * tamanhoPadraoPorColuna) + "px");
        }
        else
            $("#" + idElemento + "-resize-content").css("min-width", "0");
    }

    this.BuscarRegistros = function () {
        return Registros;
    };

    this.BuscarCodigosRegistros = function () {
        var codigos = [];

        Registros.forEach(function (registro) {
            codigos.push(registro.Codigo);
        });

        return codigos;
    };

    var callbackEdicaoColuna = null;
    this.SetCallbackEdicaoColunas = function (callback) { //quando é necessário fazer algo espefico ao ocultar ou exibir uma coluna setar esse callback
        callbackEdicaoColuna = callback; //removido devido à opcao de desmarcar todos no relatório
    };
    this.SetarRegistros = function (registros) {
        Registros = registros;
    };

    var permitirEdicaoColunas = false;
    this.SetPermitirEdicaoColunas = function (permitir) {
        permitirEdicaoColunas = permitir;
        permitirReordenarColunas = permitir;
    };

    var permitirReordenarColunas = false;
    this.SetPermitirReordenarColunas = function (permitir) {
        permitirReordenarColunas = permitir;
    };

    var permitirRedimencionarColunas = true;
    this.SetPermitirRedimencionarColunas = function (permitir) {
        permitirRedimencionarColunas = permitir;
    };

    var salvarPreferenciasGrid = false;
    this.SetSalvarPreferenciasGrid = function (salvar) {
        salvarPreferenciasGrid = salvar;
    }

    var scrollHorizontal = false;
    this.SetScrollHorizontal = function (habilitar) {
        scrollHorizontal = habilitar;
    }

    var habilitarScrollHorizontal = false;
    this.SetHabilitarScrollHorizontal = function (habilitar, tamanho) {
        habilitarScrollHorizontal = habilitar;
        if (!habilitar) {
            scrollHorizontal = false;
            tamanhoPadraoPorColuna = 0;
        } else {
            tamanhoPadraoPorColuna = tamanho;
        }
    }

    var tamanhoPadraoPorColuna = 0;
    this.setTamanhoPadraoPorColuna = function (tamanho) {
        tamanhoPadraoPorColuna = tamanho;
    }

    var obterTamanhoPadraoTotalHeader = function () {
        var total = 0;

        for (var i = 0; i < header.length; i++) {
            var cabecalhoColuna = header[i];
            if (cabecalhoColuna.visible)
                total += (parseFloat(cabecalhoColuna.widthDefault) > 0 ? parseFloat(cabecalhoColuna.widthDefault) : 0);
        }

        return total;
    };

    var obterTamanhoTotalHeader = function () {
        let total = 0;

        for (let i = 0; i < header.length; i++) {
            let cabecalhoColuna = header[i];
            if (cabecalhoColuna.visible) {
                let tamanho = parseFloat(cabecalhoColuna.width);
                if (!(tamanho > 0) && parseFloat(cabecalhoColuna.widthDefault) > 0)
                    tamanho = parseFloat(cabecalhoColuna.widthDefault);
                total += (tamanho > 0) ? tamanho : 0;
            }
        }

        return total;
    };

    var recalcularTamanho = function () {

        let total = obterTamanhoTotalHeader();

        if (isNaN(total) || total == 0) {
            $("#" + idElemento + "-resize-content").css("min-width", "0");
            return;
        }

        let headerResize = header.slice();

        let totalColunasVisiveis = 0;

        for (let i = 0; i < headerResize.length; i++) {
            let cabecalhoColuna = headerResize[i];
            let $coluna = $(table.column(cabecalhoColuna.name + ':name').header());

            if (cabecalhoColuna.visible) {
                let tamanho = parseFloat(cabecalhoColuna.width);

                if (tamanho == 0) {
                    tamanho = parseFloat(cabecalhoColuna.widthDefault);
                    cabecalhoColuna.width = cabecalhoColuna.widthDefault;
                }

                cabecalhoColuna.width = ((tamanho / total) * 100) + "%";
                totalColunasVisiveis++;
            }
            else
                cabecalhoColuna.width = "0%";

            $coluna.width(cabecalhoColuna.width);
        }

        let tamanhoColunas = scrollHorizontal ? tamanhoPadraoPorColuna : 0;

        $("#" + idElemento + "-resize-content").css("min-width", (totalColunasVisiveis * tamanhoColunas) + "px");


        table.draw();
    };

    var onResizeTable = function (event) {
        var elemento = $(event.currentTarget);
        var total = elemento.width();
        var headerResize = header.slice();
        var i, cabecalhoColuna, $coluna;

        for (i = 0; i < headerResize.length; i++) {
            cabecalhoColuna = headerResize[i];
            $coluna = $(table.column(cabecalhoColuna.name + ':name').header());

            if (cabecalhoColuna.visible === undefined || cabecalhoColuna.visible)
                cabecalhoColuna.width = (($coluna.width() / total) * 100) + "%";
            else
                cabecalhoColuna.width = "0%";
        }

        for (i = 0; i < headerResize.length; i++) {
            cabecalhoColuna = headerResize[i];
            $coluna = $(table.column(cabecalhoColuna.name + ':name').header());

            $coluna.width(cabecalhoColuna.width);
        }
    };

    var iniciarColunaResizeble = function (idElemento) {
        if (IsTouchDevice())
            $("#" + idElemento).off("touchstart.resizeble");

        $("#" + idElemento).off("mouseenter.resizeble");
        $('#' + idElemento).colResizable({ disable: true });
        $('#' + idElemento).colResizable({ liveDrag: true, minWidth: 50, resizeMode: "fit", onResize: onResizeTable });
    };

    var _menuOpcoesDefault = $.extend({}, menuOpcoes);

    this.HabilitarOpcoes = function () {
        this.CarregarGrid(Registros, true);
    };

    this.DesabilitarOpcoes = function () {
        this.CarregarGrid(Registros, false);
    };

    this.ObterPaginaAtual = function () {
        if (table) {
            var paginaAtual = table.page.info().page;

            return isNaN(paginaAtual) ? 0 : paginaAtual;
        }

        return -1;
    };

    this.CarregarGrid = function (data, enable, manterPaginacao) {

        listaSelecionadas = new Array();

        if (enable == null)
            enable = true;

        menuOpcoes = enable ? _menuOpcoesDefault : null;

        var limit = 10;

        if (quantidadePorPagina != null)
            limit = quantidadePorPagina;

        if (data.length < limit)
            limit = data.length;

        var paginaAtual = 0;
        var inicioRegistros = 0;
        var order = [];

        if (manterPaginacao && table != null) {
            paginaAtual = this.ObterPaginaAtual();

            if (paginaAtual > 0)
                inicioRegistros = paginaAtual * limit;

            if (table.order().length > 0 && table.order()[0].length > 0)
                order = [{ column: (table.order()[0][0] || 0), dir: (table.order()[0][1] == "asc" ? orderDir.asc : orderDir.desc) }];
            else
                order = new Array();
        } else {
            order = new Array();
        }

        if (table != null) {
            table.destroy(false);
            table = null;
            $('#' + idElemento + ' [data-toggle="grid-popover"]').each(function () {
                let popover = bootstrap.Popover.getOrCreateInstance(this);
                popover.dispose();
            });
            $('#' + idElemento).empty();
        }

        Registros = data;

        if (configRowsSelect != null)
            ConfigRowsSelect = configRowsSelect;
        else
            ConfigRowsSelect = { permiteSelecao: false, marcarTodos: false, permiteSelecionarTodos: false };

        var checkColDef = null;

        if (ConfigRowsSelect.permiteSelecao) {

            var checkDisplay = 'none';
            if (ConfigRowsSelect.permiteSelecionarTodos) {
                checkDisplay = 'inline-block';
            }

            if (header[0].sTitle != "<input type=\"checkbox\" name=\"select_all\" style=\"display: " + checkDisplay + ";\">") {
                var colunaSelecao = {
                    title: '<input type="checkbox" name="select_all" style="display: ' + checkDisplay + ';">',
                    render: function (data, type, row) {
                        return '<input type="checkbox" class="editor-active">';
                    }, className: "text-center", width: "5%", orderable: false
                };
                header.unshift(colunaSelecao);
            }
            else if (header[0].width === "0%" || header[0].width === "NaN%")
                header[0].width = "5%";


            checkColDef = new Object();
            checkColDef.targets = [0];
            checkColDef.data = null;
            checkColDef.orderable = false;
            checkColDef.searchable = false;
            checkColDef.className = "select-checkbox";
            checkColDef.width = "5%";
            checkColDef.widthDefault = "5%";
        }

        var headerDefault = retornarColunasPadroes(header, menuOpcoes, callbackColumnDefault);

        if (checkColDef != null)
            headerDefault.unshift(checkColDef);

        if (!manterPaginacao || order == null || order.length <= 0) {
            if (ordenacaoPadrao != null) {
                if (Array.isArray(ordenacaoPadrao)) {
                    order = JSON.parse(JSON.stringify(ordenacaoPadrao));
                } else {
                    order.push({ column: ordenacaoPadrao.column, dir: ordenacaoPadrao.dir });
                }
            } else {
                order.push({ column: 0, dir: orderDir.desc });
            }
        }

        var btn_iconTemplate = '<i class="fa fa-file-excel-o"></i> ';
        var $btnExportar = $('<a href="#" id="' + configExportacao.id + '"  title="' + configExportacao.titulo + '" class="btn btn-success waves-effect waves-themed">' + btn_iconTemplate + configExportacao.btnText + '</a>');
        var totalColunasRodapeGrid = 0;

        if (mostrarInfo && isExibirPaginacao)
            totalColunasRodapeGrid++;

        if (configExportacao.exportar)
            totalColunasRodapeGrid++;

        if (isExibirPaginacao)
            totalColunasRodapeGrid++;

        var tamanhoColunasRodapeGrid = (totalColunasRodapeGrid > 0) ? 12 / totalColunasRodapeGrid : 0;

        var sDom =
            '<"#' + idElemento + '-resize-container-parent.table-resize-container-parent"' +
            '<"#' + idElemento + '-resize-container.table-resize-container' + ((scrollHorizontal && tamanhoPadraoPorColuna > 0) ? ' table-resize-container-scroll' : '') + '"' +
            '<"#' + idElemento + '-resize-content.table-resize-content clearfix"' +
            't' + // The 'T'able -> Tabela
            '>' +
            '>' +
            '>' +
            '<"dt-toolbar-footer' + (totalColunasRodapeGrid > 0 ? ' d-flex flex-wrap justify-content-between' : ' d-none') + '"' +
            '<"' + (mostrarInfo ? ' align-self-center' : ' d-none ') + '"i>' + // 'I'nformation -> Exibindo x até x de x registros
            (configExportacao.exportar ? ('<"btn-exportar align-self-center">') : '') + // Btn Exportacao
            '<"' + (isExibirPaginacao ? ' align-self-center' : ' d-none') + '"p>' + // 'P'agination -> Lista de Paginação
            '>';

        //Aki vamos ter que verificar se existe alguma coluna que permite procurar...
        var search = existeColumnFilterSelect(header);
        var sum = existeColumnSum(header);

        $('#' + idElemento).html("");
        table = $('#' + idElemento).DataTable({
            "bAutoWidth": false,
            "bFilter": search, //false,
            "bLengthChange": false,
            "columnDefs": headerDefault,
            "colReorder": permitirReordenarColunas ? {
                "reorderCallback": function () {
                    header = ReorganizarColunas(table, header);

                    ordenacaoPadrao = { column: table.context[0].aaSorting[0][0], dir: table.context[0].aaSorting[0][1] };

                    if (permitirRedimencionarColunas)
                        iniciarColunaResizeble(idElemento);
                }
            } : false,
            "columns": header,
            "data": data,
            "destroy": true,
            "fnRowCallback": function (nRow, aData) {
                if (aData.DT_PopoverContent) {
                    $(nRow).attr('data-bs-content', aData.DT_PopoverContent);
                    $(nRow).attr('data-bs-placement', 'top');
                    $(nRow).attr('title', aData.DT_PopoverTitle);
                    $(nRow).attr('data-bs-toggle', 'grid-popover');
                    $(nRow).attr('data-bs-trigger', 'hover focus');
                }
                else
                    adicionarToolTip(nRow, aData, header);

                if (aData.DT_RowColor != "")
                    $(nRow).css('background-color', aData.DT_RowColor);

                if (aData.DT_FontColor != "")
                    $(nRow).children('td').css('color', aData.DT_FontColor);

                //setarEnableDataRow(nRow, aData);

                if (draggableRows && !(callbackOrdenacao instanceof Function)) {
                    var eventStop = draggableRows instanceof Function ? draggableRows : undefined;

                    $(nRow).draggable({
                        cursor: "move",
                        helper: function (event, ui) {
                            var html = '';

                            $(event.currentTarget).children().each(function (i, coluna) {
                                var $coluna = $(coluna);

                                html += '<td class="' + $coluna.attr('class') + '" style="width: ' + ($coluna.width() + 1) + 'px; max-width: ' + ($coluna.width() + 1) + 'px;">' + coluna.innerHTML + '</td>';
                            });

                            var corLinha = $(event.currentTarget).css("background-color");
                            var corLinhaSelecionada = "#ecf3f8";
                            var coresLinhaPadrao = ["#ffffff", "#F9F9F9"];
                            var isCorPadrao = coresLinhaPadrao.indexOf(corLinha) > -1;

                            return '<tr style="z-index: 5000; width: ' + $(event.currentTarget).width() + 'px; background-color: ' + (isCorPadrao ? corLinhaSelecionada : corLinha) + ';">' + html + '</tr>';
                        },
                        revert: 'invalid',
                        stop: eventStop
                    });
                }

                if (callbackRow instanceof Function)
                    callbackRow(nRow, aData, self);
            },
            "headerCallback": function (thead) {
                adicionarToolTip(thead, null);
            },
            "iDisplayLength": limit,
            "info": mostrarInfo,
            "language": Language,
            "order": order.length > 1 ? order.map(o => [o.column, o.dir]) : [[order[0].column, order[0].dir]],
            "orderFixed": ordenacaoFixa,
            "paging": true,
            "sDom": sDom,
            "search": search,
            "footerCallback": (!sum ? null : function (tfoot, data, start, end, display) {
                var api = this.api();
                // Remove the formatting to get integer data for summation
                var intVal = function (i) {
                    return typeof i === 'string' ?
                        i.replace(/[\$,]/g, '').replace('R', '').replace('$', '').replace(' ', '').replace('.', '').replace(',', '.') * 1 :
                        typeof i === 'number' ?
                            i : 0;
                };

                api.columns().every(function (i) {
                    var column = this;
                    if (column.visible()) {
                        var soma = '';
                        if (header[i]) {
                            var somarApenasSelecionados = (header[i].sumSelected === true);
                            if (header[i].sum === true) {
                                soma = 0;
                                if (somarApenasSelecionados) {
                                    // Aki obtem os indices das linhas marcadas
                                    var rowIndexes = api.rows('.selected').indexes();
                                    for (var j = 0; j < rowIndexes.length; j++) {
                                        soma += intVal(api.rows(rowIndexes[j]).column(i).data()[rowIndexes[j]]);
                                    }
                                } else {

                                    soma = api
                                        .column(i, { search: 'applied' }) //, { page: 'current' })
                                        .data()
                                        .reduce(function (a, b) {
                                            return intVal(a) + intVal(b);
                                        }, 0);

                                }
                            }

                            var indice = RetornarIndiceVisivel(header, i);

                            if ($('#' + idElemento + ' tfoot th')[indice]) {
                                $('#' + idElemento + ' tfoot th')[indice].innerHTML = (header[i].currency === true ? formatterCurrency.format(soma) : soma);
                            }
                        }
                    }
                });
            }),
            "initComplete": (!search ? null : function () {
                var api = this.api();
                DataTableInitComplete(api, header);

                var intVal = function (i) {
                    return typeof i === 'string' ?
                        i.replace(/[\$,]/g, '').replace('R', '').replace('$', '').replace(' ', '').replace('.', '').replace(',', '.') * 1 :
                        typeof i === 'number' ?
                            i : 0;
                };

                if (sum) {
                    var htmlFooter = '<tfoot><tr>';
                    api.columns().every(function (i) {
                        var column = this;
                        var index = RetornarIndiceHeadByName(header, column.data().context[0].aoColumns[i].data);
                        if (index >= 0) {
                            if (header[index].bVisible != false) {

                                if (header[index].sum == undefined) {
                                    htmlFooter += '<th></th>';
                                } else {

                                    soma = api
                                        .column(index, { search: 'applied' }) //, { page: 'current' })
                                        .data()
                                        .reduce(function (a, b) {
                                            return intVal(a) + intVal(b);
                                        }, 0);

                                    htmlFooter += '<th>' + (header[index].currency === true ? formatterCurrency.format(soma) : soma) + '</th>';
                                }
                            }
                        }
                    });

                    htmlFooter += '</tr></tfoot>';
                    $(this).append(htmlFooter);
                }
            }),
            "processing": search,
            "displayStart": manterPaginacao ? inicioRegistros : 0
            //"retrieve": true
        });

        if (configExportacao.exportar) {
            var retornarBotaoExportarEstadoOriginal = function () {
                $btnExportar
                    .attr('disabled', false)
                    .removeClass('disabled')
                    .html(btn_iconTemplate + configExportacao.btnText);
            };

            $('#' + idElemento + '_wrapper .btn-exportar').append($btnExportar);

            $btnExportar.on('click', function (e) {
                if (e && e.preventDefault) e.preventDefault();

                $btnExportar
                    .attr('disabled', true)
                    .addClass('disabled')
                    .html(configExportacao.btnLoading);

                var dadosPesquisaExportacao = {};
                var parametrosExportacao = (configExportacao.funcaoObterParametros instanceof Function) ? configExportacao.funcaoObterParametros() : {};

                $.extend(true, dadosPesquisaExportacao, { tituloExportacao: configExportacao.titulo }, parametrosExportacao);

                executarDownload(configExportacao.url, dadosPesquisaExportacao, retornarBotaoExportarEstadoOriginal, function (html) {
                    retornarBotaoExportarEstadoOriginal();

                    try {
                        var retorno = JSON.parse(html.replace("(", "").replace(");", ""));

                        if (retorno.Success)
                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                        else
                            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                    }
                    catch (err) {
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
                    }
                }, true, true);
            });
        }

        if (tamanhoPadraoPorColuna > 0) {
            $("#" + idElemento + "-resize-container").on("scroll", function () {
                $("#" + idElemento + "-resize-content td .btn-group.open button").trigger("click");
            });

            $("#" + idElemento + "-resize-container").on('shown.bs.dropdown', function (e, f) {
                var $listaOpcoes = $($(e.target).find(".dropdown-menu")[0]);
                var coluna = e.target.offsetParent;

                $listaOpcoes.css({ top: coluna.offsetTop + coluna.offsetHeight + 3 + 'px' });
            })
        }

        adicionarEventOpcoes(table, idElemento, menuOpcoes);
        definirTamanhoMinimoTabela();

        if (ConfigRowsSelect.permiteSelecao) {

            $("#" + idElemento + ' tbody td').css("cursor", "pointer");

            $("#" + idElemento + ' tbody').on('click', 'input[type="checkbox"]', function (e) {
                var $row = $(this).closest('tr');
                var data = table.row($row).data();
                if (this.checked) {
                    AdicionarElementoBasicTable(data, listaSelecionadas, callbackRegistroSelecionadoChange);
                    if (table.context.length == listaSelecionadas.length) {
                        $("#" + idElemento + ' thead input[name="select_all"]').prop("checked", true);
                    }
                    $row.addClass('selected');

                } else {
                    RemoverElementoBasicTable(data, listaSelecionadas, callbackRegistroSelecionadoChange);
                    $("#" + idElemento + ' thead input[name="select_all"]').prop("checked", false);
                    $row.removeClass('selected');
                }

                e.stopPropagation();
                // Comentado #56203 pois ao selecionar um registro da segunda página, volta para a primeira.
                //table.draw();
            });

            $("#" + idElemento + ' thead input[name="select_all"]').on('click', function (e) {
                if (callbackSelecionarTodos instanceof Function) {
                    callbackSelecionarTodos();
                    return;
                }
                // Assai, #23024
                if (ConfigRowsSelect.permiteSelecionarTodos) {

                    var selecionar = this.checked;
                    //{ filter: 'applied' }
                    table.rows({ filter: 'applied' }).every(function (rowIdx, tableLoop, rowLoop) {
                        var data = this.data();
                        var row = table.row(rowIdx);

                        if (selecionar) {
                            AdicionarElementoBasicTable(data, listaSelecionadas, callbackRegistroSelecionadoChange);
                            if (table.context.length == listaSelecionadas.length) {
                                $("#" + idElemento + ' thead input[name="select_all"]').prop("checked", true);
                            }
                            row.nodes().to$().addClass('selected');
                        } else {
                            RemoverElementoBasicTable(data, listaSelecionadas, callbackRegistroSelecionadoChange);
                            $("#" + idElemento + ' thead input[name="select_all"]').prop("checked", false);
                            row.nodes().to$().removeClass('selected');
                        }
                        var node = this.nodes();
                        $('input[type="checkbox"]', node).prop('checked', selecionar);
                    });

                } else {
                    if (this.checked) {
                        $("#" + idElemento + ' tbody input[type="checkbox"]:not(:checked)').trigger('click');
                    } else {
                        $("#" + idElemento + ' input[type="checkbox"]:checked').trigger('click');
                    }
                }

                e.stopPropagation();
                table.draw();
            });

            if (ConfigRowsSelect.marcarTodos) {
                $("#" + idElemento + ' thead input[name="select_all"]').trigger('click');
            }
        }

        if (editarColuna.permite) {
            var _handleClick = function (e) {
                if (e.target.localName != "input" && editarColuna.permite) {
                    var head = retonarHeaderSelecionado(table, this, header);
                    if (head != null && head.editableCell != null && head.editableCell.editable) {
                        e.stopPropagation();
                        dataRowAnterior = { dataRow: null };
                        EditarTableCellClick(this, table, head, header, editarColuna, dataRowAnterior);
                    }
                }
            };
            $('#' + idElemento)
                .off('click', 'tbody td')
                .on('click', 'tbody td', _handleClick);
        }

        if (permitirEdicaoColunas) {
            let html = '<div class="table-preferences-container">';

            if (salvarPreferenciasGrid) {
                html += '<div class="table-preferences-container-action">';
                html += '    <div class="card mb-3">';
                html += '        <div class="card-header">';
                html += '            <button id="preferencias-' + idElemento + '" class="btn btn-primary waves-effect waves-themed table-preferences-configurations" data-bs-toggle="collapse" data-bs-target="#container-colunas-' + idElemento + '"><i id="preferencias-icone-' + idElemento + '" class="fal fa-cog"></i> ' + Localization.Resources.Gerais.Geral.Campos + '</button>';
                html += '        </div>';

                html += '<div id="container-colunas-' + idElemento + '" class="card-body table-preferences-container-columns collapse' + (!salvarPreferenciasGrid ? 'show' : '') + '">';
                html += '    <div class="custom-control custom-checkbox table-preferences-check-uncheck">';
                html += '        <input type="checkbox" id="marcar-desmarcar-colunas-' + idElemento + '" class="custom-control-input" checked="">';
                html += '        <label for="marcar-desmarcar-colunas-' + idElemento + '" class="custom-control-label"> ' + Localization.Resources.Gerais.Geral.SelecionarTodos + ' </label>';
                html += '    </div>';
                html += '    <hr/>';
                html += '    <div class="checkboxes">';

                $.each(header, function (i, cabecalho) {
                    if (cabecalho.sTitle != "" && cabecalho.sTitle != undefined && !cabecalho.sTitle.includes("<input")) {
                        html += '<div class="custom-control custom-checkbox">';
                        html += '  <input type="checkbox"' + 'id="' + cabecalho.data.concat('', idElemento) + '" class="custom-control-input" ' + ((cabecalho.visible == null || cabecalho.visible) ? 'checked' : '') + ' data-column="' + cabecalho.data + '"/>';
                        html += '  <label for="' + cabecalho.data.concat('', idElemento) + '" class="custom-control-label">' + cabecalho.title + '</label>';
                        html += '</div> ';
                    }
                });

                if (url != null && gridPreferencias != null) {
                    executarReST(
                        "Preferencias/BuscarPreferenciasGrid",
                        {
                            urlGrid: url,
                            idGrid: gridPreferencias,
                            CodigoModelo: 0
                        },
                        function (response) {
                            if (response != null && response.Success) {
                                if (response.Data != null) {
                                    const preferenciasGrid = response.Data.columns;
                                    const scrollHorizontalAtivo = response.Data.scrollHorizontal;
                                    if (preferenciasGrid != null) {
                                        $.each(header, function (i, cabecalho) {
                                            if (cabecalho.sTitle != "" && cabecalho.sTitle != undefined && !cabecalho.sTitle.includes("<input")) {
                                                if (preferenciasGrid[i]) {
                                                    const exibir = preferenciasGrid[i].name == cabecalho.data && (preferenciasGrid[i].visible == null || preferenciasGrid[i].visible);
                                                    const element = document.getElementById(cabecalho.data.concat('', idElemento));
                                                    if (element.value == "on" && !exibir) {
                                                        element.checked = false;

                                                        let nomeColuna = element.getAttribute('data-column');
                                                        let cabecalhoColuna = obterCabecalhoColunaPorNome(nomeColuna);
                                                        let coluna = table.column(nomeColuna + ':name');

                                                        coluna.visible(false);
                                                        cabecalhoColuna.visible = false;
                                                    }
                                                }
                                            }
                                        });

                                        recalcularTamanho();
                                    }
                                    if (scrollHorizontalAtivo) {
                                        const elemento = document.getElementById("checkbox-habilitar-scroll-horizontal-" + idElemento);
                                        elemento.checked = true;
                                        ativarScrollHorizontal(idElemento);
                                    }
                                }
                            }
                        },
                        null,
                        true
                    );
                }

                html += '    </div>';

                if (habilitarScrollHorizontal) {
                    let checkedHabilitarScrollHorizontal = (scrollHorizontal) ? " checked" : "";
                    html += '<div id="container-scroll-horizontal-' + idElemento + '" class="table-preferences-container-action table-preferences-container-action-footer hidden">';
                    html += '    <div class="row">';
                    html += '        <div class="col col-xs-12">';
                    html += '            <input type="checkbox" id="checkbox-habilitar-scroll-horizontal-' + idElemento + '"' + checkedHabilitarScrollHorizontal + '> <label for="checkbox-habilitar-scroll-horizontal-' + idElemento + '">Habilitar scroll horizontal</label>';
                    html += '        </div>';
                    html += '    </div>';
                    html += "</div>";
                }

                html += '<div id="container-acoes-' + idElemento + '" class="d-flex justify-content-end table-preferences-container-action table-preferences-container-action-footer hidden">';
                html += '   <button id="restaurar-padrao-' + idElemento + '" class="btn btn-sm btn-default ms-2 waves-effect waves-themed"><i class="fal fa-undo"></i> ' + Localization.Resources.Gerais.Geral.RestaurarPadrao + '</button>';
                html += '   <button id="salvar-preferencias-' + idElemento + '" class="btn btn-sm btn-success ms-2 waves-effect waves-themed"><i class="fal fa-save"></i> ' + Localization.Resources.Gerais.Geral.SalvarPreferenciasPorUsuario + '</button>';

                html += "</div>";
                html += '</div>';
                html += "</div>";

                $('#' + idElemento + "-resize-container-parent").before(html);

                $("#marcar-desmarcar-colunas-" + idElemento).change(function () {
                    habilitarColunasCabecalho(this.checked);

                    if (callbackEdicaoColuna != null)
                        callbackEdicaoColuna();

                    if (permitirRedimencionarColunas && this.checked)
                        iniciarColunaResizeble(idElemento);
                });

                $('.table-preferences-container [data-column]').on('change', function (e) {
                    e.preventDefault();


                    let nomeColuna = $(this).attr('data-column');
                    let cabecalhoColuna = obterCabecalhoColunaPorNome(nomeColuna);
                    let coluna = table.column(nomeColuna + ':name');
                    let colunaVisivel = !coluna.visible();

                    if (colunaVisivel) {
                        let tamanhoPadraoTotalHeader = obterTamanhoPadraoTotalHeader();
                        let tamanhoColuna = parseFloat(cabecalhoColuna.widthDefault);
                        if (isNaN(tamanhoColuna))
                            tamanhoColuna = parseFloat(cabecalhoColuna.width);

                        if (tamanhoPadraoTotalHeader > 0) {
                            let tamanhoPadraoPercentualColuna = (tamanhoColuna * 100) / (tamanhoPadraoTotalHeader + tamanhoColuna);
                            let markupDivisor = 100 / (100 - tamanhoPadraoPercentualColuna);

                            tamanhoColuna = tamanhoPadraoPercentualColuna * markupDivisor;
                        }

                        cabecalhoColuna.width = tamanhoColuna + "%";
                    }

                    coluna.visible(colunaVisivel);
                    cabecalhoColuna.visible = colunaVisivel;

                    recalcularTamanho();

                    if (callbackEdicaoColuna != null)
                        callbackEdicaoColuna();

                    if (permitirRedimencionarColunas)
                        iniciarColunaResizeble(idElemento);
                });

                $("#" + idElemento + "-resize-container").on("scroll", function () {
                    $("#" + idElemento + "-resize-content td .btn-group.open button").trigger("click");
                });

                $("#" + idElemento + "-resize-container").on('shown.bs.dropdown', function (e, f) {
                    var $listaOpcoes = $($(e.target).find(".dropdown-menu")[0]);
                    var coluna = e.target.offsetParent;
                    if (scrollHorizontal) $listaOpcoes.css({ top: coluna.offsetTop + coluna.offsetHeight + 3 + 'px' });
                })

                $("#preferencias-" + idElemento).on('click', function (e) {
                    e.preventDefault();
                    $("#preferencias-icone-" + idElemento).toggleClass("fa-cog");
                    $("#preferencias-icone-" + idElemento).toggleClass("fa-times");
                    $("#container-colunas-" + idElemento).toggleClass("hidden");
                    $("#container-acoes-" + idElemento).toggleClass("hidden");
                    $("#container-scroll-horizontal-" + idElemento).toggleClass("hidden");
                });

                $("#salvar-preferencias-" + idElemento).on('click', function (e) {
                    e.preventDefault();

                    let data = {
                        columns: [],
                        scrollHorizontal: (habilitarScrollHorizontal) ? scrollHorizontal : false
                    };

                    for (let i = 0; i < header.length; i++) {
                        data.columns.push({
                            name: header[i].name,
                            visible: header[i].visible,
                            width: header[i].width,
                            position: header[i].position
                        });
                    }

                    executarReST(
                        "Preferencias/SalvarPreferenciasGrid",
                        {
                            urlGrid: url,
                            idGrid: gridPreferencias,
                            dadosGrid: JSON.stringify(data)
                        },
                        function (response) {
                            if (response.Success) {
                                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                                $("#preferencias-icone-" + idElemento).addClass("fa-cog");
                                $("#preferencias-icone-" + idElemento).removeClass("fa-times");
                                $("#container-colunas-" + idElemento).addClass("hidden");
                                $("#container-acoes-" + idElemento).addClass("hidden");
                                $("#container-scroll-horizontal-" + idElemento).addClass("hidden");
                            }
                        },
                        null,
                        true
                    );
                });

                $("#restaurar-padrao-" + idElemento).on('click', function (e) {
                    e.preventDefault();
                    exibirConfirmacao(Localization.Resources.Gerais.Geral.RestaurarPadrao, Localization.Resources.Gerais.Geral.PreferenciasSeraoPerdidasDesejaContinuar, function (e) {
                        executarReST(
                            "Preferencias/RestaurarPadraoGrid",
                            {
                                urlGrid: url,
                                idGrid: gridPreferencias
                            },
                            function (response) {
                                if (response.Success) {
                                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                                    self.CarregarGrid(Registros, true);
                                    recalcularTamanho();
                                }
                            },
                            null,
                            true
                        );
                    });
                });

                $("#gerenciar-modelos-grid-" + idElemento).on('click', function (e) {
                    e.preventDefault();

                    var data = {
                        columns: [],
                        scrollHorizontal: (habilitarScrollHorizontal) ? scrollHorizontal : false
                    };

                    for (var i = 0; i < header.length; i++) {
                        data.columns.push({
                            name: header[i].name,
                            visible: header[i].visible,
                            width: header[i].width,
                            position: header[i].position
                        });
                    }

                    _modeloGrid.UrlGrid.val(url);
                    _modeloGrid.IdGrid.val(idElemento);
                    _modeloGrid.DadosGrid.val(JSON.stringify(data));
                    _modeloGrid.CallbackAplicar = function (modelo) {
                        self.SetModelo(modelo);
                        self.CarregarGrid();
                    };
                    _modeloGrid.CallbackAplicarUsuario = function () {
                        self.SetModelo(0);
                        self.CarregarGrid();
                    };

                    Global.abrirModal("modal-modelo-grid");
                });

                if (habilitarScrollHorizontal) {
                    $("#checkbox-habilitar-scroll-horizontal-" + idElemento).on('change', function (e) {
                        ativarScrollHorizontal(idElemento);
                    });
                }

                recalcularTamanho();

            } else {

                html += '<div id="container-colunas-' + idElemento + '" class="table-preferences-container-columns' + (salvarPreferenciasGrid ? " hidden" : "") + '">';
                $.each(header, function (i, cabecalho) {
                    if (esconderColunasVisibleFalseAoPermitirEdicaoColunas && (cabecalho.visible != null && !cabecalho.visible) && !cabecalho.permiteEsconderColuna)
                        return;
                    if (cabecalho.sTitle != "" && cabecalho.sTitle != undefined && cabecalho.sTitle.indexOf('<input type="checkbox') < 0) {
                        var style = (cabecalho.visible == null || cabecalho.visible) ? "font-weight: bold;" : "text-decoration: line-through; color: #888; font-weight: normal;";
                        html += '<span class="btn btn-xs btn-default waves-effect waves-themed toggle-vis table-preferences-column-toggle">';
                        html += '  <a style="' + style + '" data-column="' + cabecalho.data + '">' + cabecalho.title + '</a>';
                        html += '</span > ';
                    }
                });

                html += '</div>';

                html += '</div>';

                $('#' + idElemento + "-resize-container-parent").before(html);

                $('span.toggle-vis').on('click', function (e) {
                    e.preventDefault();

                    var nomeColuna = $(this).children("a").attr('data-column');
                    var indiceColuna = RetornarIndiceHeadByName(header, nomeColuna);
                    var cabecalhoColuna = header[indiceColuna];
                    var coluna = table.column(indiceColuna);
                    var colunaVisivel = !coluna.visible();

                    if (colunaVisivel) {
                        var tamanhoPadraoTotalHeader = obterTamanhoPadraoTotalHeader();
                        var tamanhoColuna = parseFloat(cabecalhoColuna.widthDefault);
                        if (isNaN(tamanhoColuna))
                            tamanhoColuna = parseFloat(cabecalhoColuna.width);

                        if (tamanhoPadraoTotalHeader > 0) {
                            var tamanhoPadraoPercentualColuna = (tamanhoColuna * 100) / (tamanhoPadraoTotalHeader + tamanhoColuna);
                            var markupDivisor = 100 / (100 - tamanhoPadraoPercentualColuna);

                            tamanhoColuna = tamanhoPadraoPercentualColuna * markupDivisor;
                        }

                        cabecalhoColuna.width = tamanhoColuna + "%";
                    }

                    coluna.visible(colunaVisivel);
                    cabecalhoColuna.visible = colunaVisivel;

                    if (colunaVisivel) {
                        $(this).children("a").css("text-decoration", "none");
                        $(this).children("a").css("color", "#3276b1");
                        $(this).children("a").css("font-weight", "bold");
                    }
                    else {
                        $(this).children("a").css("text-decoration", "line-through");
                        $(this).children("a").css("color", "#888");
                        $(this).children("a").css("font-weight", "normal");
                    }

                    recalcularTamanho();

                    table.draw(false);
                });

                $("#preferencias-" + idElemento).on('click', function (e) {
                    e.preventDefault();
                    $("#preferencias-icone-" + idElemento).toggleClass("fa-cog");
                    $("#preferencias-icone-" + idElemento).toggleClass("fa-times");
                    $("#container-colunas-" + idElemento).toggleClass("hidden");
                    $("#container-acoes-" + idElemento).toggleClass("hidden");
                    $("#container-scroll-horizontal-" + idElemento).toggleClass("hidden");
                });
            }
        }

        if (permitirRedimencionarColunas) {
            if (IsTouchDevice())
                $("#" + idElemento).on("touchstart.resizeble", function () { iniciarColunaResizeble(idElemento); });

            $("#" + idElemento).on("mouseenter.resizeble", function () { iniciarColunaResizeble(idElemento); });
        }

        $('#' + idElemento + ' [data-bs-toggle="grid-popover"]').each(function () {
            bootstrap.Popover.getOrCreateInstance(this, { html: true });
        });

        if (callbackOrdenacao instanceof Function)
            ordenacaoGrid.criarOrdenacao();
    };

    this.BasicTable = function () {
        return table;
    };

    this.ListaSelecionados = function () {
        return listaSelecionadas;
    };

    this.SetarSelecionados = function (listaSelecao) {
        listaSelecionadas = listaSelecao;
        table.cells().every(function () {
            let dataCell = this.data();
            if (dataCell) {
                let cellNode = this.node();
                let rowIndex = this.index().row;
                let rowNode = table.row(rowIndex).node();
                let rowElement = $(rowNode)
                let checkboxElement = $(cellNode).find('input[type="checkbox"]');
                if (dataCell.Codigo != undefined) {
                    let registroCheckado = listaSelecao.filter(function (selecionado) {
                        return selecionado.Codigo == dataCell.Codigo;
                    }).length > 0;

                    if (registroCheckado) {
                        checkboxElement.prop('checked', true);
                        rowElement.addClass('selected');
                    }
                    else {
                        checkboxElement.prop('checked', false);
                        rowElement.removeClass('selected');
                    };
                }
            }
        });
    }

    this.Destroy = function () {
        if (table != null)
            table.destroy();
    };

    this.AtualizarDataRow = function (rowPar, DataRow, callbackTabPress) {
        AtualizarDataRow(table, rowPar, DataRow, callbackTabPress);
    };

    this.ControlarExibicaoColuna = function (nomeProriedade, exibir) {
        var indiceColuna = self.ObterIndiceColuna(nomeProriedade);
        var column = table.column(indiceColuna);

        column.visible(exibir);

        header[column[0]].visible = exibir;
        header[column[0]].bVisible = exibir;

        table.draw();
    };

    this.ObterIndiceColuna = function (nomeProriedade) {
        for (var i = 0; i < header.length; i++) {
            if (header[i].data == nomeProriedade)
                return i;
        }

        return undefined;
    };

    this.ObterHeader = function () {
        return header;
    };

    var obterCabecalhoColunaPorNome = function (nome) {
        for (var i = 0; i < header.length; i++) {
            if (header[i].name == nome)
                return header[i];
        }

        return undefined;
    }

    var habilitarColunasCabecalho = function (habilitar) {
        var headerResize = header.slice();

        for (var i = 0; i < headerResize.length; i++) {
            var cabecalhoColuna = headerResize[i];
            var nomeColuna = cabecalhoColuna.name;
            var coluna = table.column(nomeColuna + ':name');
            var habilitarColuna = habilitar && Boolean(cabecalhoColuna.title);

            var tamanhoColuna = cabecalhoColuna.widthDefault;
            if (isNaN(tamanhoColuna))
                tamanhoColuna = parseFloat(cabecalhoColuna.width);

            cabecalhoColuna.width = habilitarColuna ? tamanhoColuna : "0%";
            cabecalhoColuna.visible = habilitarColuna;
            coluna.visible(habilitarColuna);

            $("[data-column=" + nomeColuna + "]").prop('checked', cabecalhoColuna.visible);
        }
    }

    var ativarScrollHorizontal = function (idElemento) {
        const elementoCheckbox = document.getElementById("checkbox-habilitar-scroll-horizontal-" + idElemento);

        if (elementoCheckbox) {
            // Verificar se a caixa de seleção está marcada
            self.SetScrollHorizontal($("#checkbox-habilitar-scroll-horizontal-" + idElemento).is(":checked"));
            if (scrollHorizontal) $("#" + idElemento + "-resize-container").addClass("table-resize-container-scroll");
            else $("#" + idElemento + "-resize-container").removeClass("table-resize-container-scroll");

            recalcularTamanho();

            if (callbackEdicaoColuna != null)
                callbackEdicaoColuna();

            if (permitirRedimencionarColunas)
                iniciarColunaResizeble(idElemento);
        }
    }
};

function AdicionarElementoBasicTable(data, listaSelecionadas, callbackRegistroSelecionadoChange) {
    var registroSelecionado = false;
    if (data.Codigo != null) {
        registroSelecionado = listaSelecionadas.filter(function (item) {
            return item.Codigo == data.Codigo;
        }).length > 0;
    }
    if (!registroSelecionado) {
        listaSelecionadas.push(data);
        if (callbackRegistroSelecionadoChange instanceof Function)
            callbackRegistroSelecionadoChange(data, true);
    }
}

function RemoverElementoBasicTable(data, listaSelecionadas, callbackRegistroSelecionadoChange) {
    if (data.Codigo != null) {
        $.each(listaSelecionadas, function (i, obj) {
            if (obj.Codigo == data.Codigo) {
                listaSelecionadas.splice(i, 1);
                if (callbackRegistroSelecionadoChange instanceof Function)
                    callbackRegistroSelecionadoChange(data, false);
                return false;
            }
        });
    } else {
        exibirMensagem(tipoMensagem.falha, "Falha de Componente", "O componente está incompleto, por favor, entre em contato com a Multisoftware");
    }
}

function adicionarToolTip(nRow, aData, header) {

    $.each(nRow.cells, function (i, col) {

        var colunaOpcoes = $(col).children().length != 0;

        if (!colunaOpcoes) {
            var sTip = $(col).text();

            $(col).html("<span>" + sTip + "</span>");

            var toolTipText = sTip;

            // calbackToolTip na coluna....
            if (header) {
                var indiceVisivel = -1;
                var cont = -1;
                $.each(header, function (j, cabecalho) {
                    if (cabecalho.visible == null || cabecalho.visible) {
                        cont++;
                        if (cont == i)
                            indiceVisivel = j;
                    }
                });

                if (header[indiceVisivel] != undefined) {
                    if (header[indiceVisivel].callbackToolTip instanceof Function) {
                        toolTipText = header[indiceVisivel].callbackToolTip(aData);
                    }
                }
            }

            this.children[0].setAttribute('title', toolTipText);
        }
        else if ($(this)[0].firstChild.localName == "div")
            $(this).css("overflow", "visible");
    });
}

function adicionarEventOpcoes(table, idElemento, menuOpcoes) {
    $('#' + idElemento + ' tbody').unbind('click');

    if (menuOpcoes != null) {
        if (menuOpcoes.tipo == TypeOptionMenu.link) {
            $.each(menuOpcoes.opcoes, function (i, opcao) {
                $('#' + idElemento + ' tbody').on('click', 'a.' + opcao.id, function () {
                    var row = $(this).parents('tr');
                    var data = table.row(row).data();
                    opcao.metodo(data, row);
                });
            });
        }
        else if (menuOpcoes.tipo == TypeOptionMenu.list) {
            if (menuOpcoes.opcoes && (menuOpcoes.opcoes.length > 0)) {
                $('#' + idElemento + ' tbody').on('click', 'button', function () {
                    var row = $(this).parents('tr');
                    var data = table.row(row).data();
                    var existeOpcaoVisivel = false;

                    for (var indice = 0; indice < menuOpcoes.opcoes.length; indice++) {
                        var opcao = menuOpcoes.opcoes[indice];

                        if ((opcao.visibilidade instanceof Function) && !opcao.visibilidade(data))
                            $('#' + idElemento + ' tbody a.' + opcao.id).hide();
                        else {
                            $('#' + idElemento + ' tbody a.' + opcao.id).show();
                            existeOpcaoVisivel = true;
                        }
                    }

                    if (existeOpcaoVisivel) {
                        var $dropdown = $(this).parents('tr').find('div.dropdown-menu');
                        setTimeout(function () {
                            $dropdown.css("display", "");
                            var rect = $dropdown[0].getBoundingClientRect();
                            var windowHeight = $(window).height();

                            if (rect.bottom > windowHeight) {
                                $dropdown.css('top', (parseInt($dropdown.css('top')) - (rect.bottom - windowHeight) - 10) + 'px');
                            }
                        }, 50);
                    } else {
                        $(this).parents('tr').find('div.dropdown-menu').css("display", "none");
                    }
                });

                $.each(menuOpcoes.opcoes, function (i, opcao) {
                    $('#' + idElemento + ' tbody').on('click', 'a.' + opcao.id, function () {
                        var row = $(this).parents('tr');
                        var data = table.row(row).data();
                        opcao.metodo(data, row, table);
                    });
                });
            }
        }
    }
}

function retornarColunasPadroes(headers, menuOpcoes, callbackColumnDefault, habilitarExpansaoLinha) {
    var colunasCabecalho = headers.length;
    var colunasPadroes = new Array();

    if (menuOpcoes) {
        if (menuOpcoes.tipo == TypeOptionMenu.link) {
            $.each(menuOpcoes.opcoes, function (i, opcao) {
                var colunaPadrao = new Object();

                colunaPadrao.targets = [colunasCabecalho + i];
                colunaPadrao.data = null;
                colunaPadrao.orderable = false;
                colunaPadrao.title = opcao.descricao;
                colunaPadrao.className = "sorting_disabled_opcao text-center";
                colunaPadrao.width = opcao.tamanho ? opcao.tamanho + "%" : "10%";
                colunaPadrao.defaultContent = "<a href='javascript:;' class='" + opcao.id + "'>" + opcao.descricao + "</a>";

                colunasPadroes.push(colunaPadrao);
            });
        }
        else if (menuOpcoes.tipo == TypeOptionMenu.list) {
            var html = '';

            html += '<div class="btn-group btn-block">';
            html += '    <button type="button" class="btn btn-sm btn-icon btn-outline-primary rounded-circle shadow-0 waves-effect waves-themed" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i class="fal fa-list"></i></button>';
            html += '    <div class="dropdown-menu" role="menu">';

            $.each(menuOpcoes.opcoes, function (i, opcao) {
                html += '    <a class="dropdown-item ' + opcao.id + '" style="display: none;">' + "<i class= '" + opcao.icone + " me-2'> </i>" + opcao.descricao + '</a>';
            });

            html += '    </div>';
            html += '</div>';

            var colunaPadrao = new Object();

            colunaPadrao.targets = [colunasCabecalho];
            colunaPadrao.data = null;
            colunaPadrao.orderable = false;
            colunaPadrao.title = (menuOpcoes.descricao == undefined) ? Localization.Resources.Gerais.Geral.Opcoes : menuOpcoes.descricao;
            colunaPadrao.className = "sorting_disabled_opcao text-center";
            colunaPadrao.width = menuOpcoes.tamanho ? menuOpcoes.tamanho + "%" : "10%";
            colunaPadrao.defaultContent = html;

            colunasPadroes.push(colunaPadrao);
        }

        colunasCabecalho++;
    }

    if (headers) {
        $.each(headers, function (i, head) {
            if (head.editableCell && (head.editableCell.type == EnumTipoColunaEditavelGrid.bool)) {
                var tamanho = (parseFloat(head.width.replace("%", "")) < 10) ? 'mini' : 'small';
                var colunaPadrao = new Object();

                colunaPadrao.targets = i;
                colunaPadrao.orderable = false;
                colunaPadrao.render = function (data, type, row, meta) {
                    return '<input type="checkbox" data-bind="checked" ' + (data === true ? "checked" : '') + ' class="chk_toggle" data-toggle="toggle" data-on="true" data-off="false" data-onstyle="success" data-offstyle="danger" data-size="' + tamanho + '" data-style="ios" />';
                }

                colunasPadroes.push(colunaPadrao);
            }
            else if (callbackColumnDefault instanceof Function) {
                var colunaPadrao = $.extend({}, head, {
                    targets: i,
                    render: function (data, type, row, meta) {
                        var html = callbackColumnDefault(head, data, row);

                        return html || '<span title="' + data + '">' + data + '</span>';
                    }
                });

                colunasPadroes.push(colunaPadrao);
            }
            else if (typeof callbackColumnDefault == "object" && callbackColumnDefault != null && head.name in callbackColumnDefault) {
                var colunaPadrao = $.extend({}, head, {
                    targets: i,
                    render: function (data, type, row, meta) {
                        var html = callbackColumnDefault[head.name](data, row, head);

                        return html || '<span title="' + data + '">' + data + '</span>';
                    }
                });

                colunasPadroes.push(colunaPadrao);
            }
        });
    }

    if (habilitarExpansaoLinha) {
        var colunaPadrao = {
            targets: [colunasCabecalho],
            data: null,
            orderable: false,
            title: "",
            className: "row-exapand",
            width: "3%",
            defaultContent: '<span class="expand-row-icon" style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;"><i class="fas fa-angle-down"></i></span>'
        };
        colunasPadroes.push(colunaPadrao);
    }

    return colunasPadroes;
}

function existeHeaderBoolToggle(headers) {
    var existe = false;
    $.each(headers, function (i, head) {
        if (head.editableCell) {
            if (head.editableCell.type == EnumTipoColunaEditavelGrid.bool) {
                existe = true;
            }
        }
    });
    return existe;
}
