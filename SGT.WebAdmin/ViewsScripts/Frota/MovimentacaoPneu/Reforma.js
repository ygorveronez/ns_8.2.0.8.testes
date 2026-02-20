/// <reference path="MovimentacaoPneu.js" />
/// <reference path="../../Consultas/Pneu.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */
var _itensPorPaginaReforma= 16;
var _paginaAtualReforma = 1;
var _pesquisaReforma;
var _pesquisaReformaAnterior;
var _reforma;

/*
 * Declaração das Classes
 */

var PesquisaReforma = function () {
    this.Pneu = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pneu:", idBtnSearch: guid() });
    this.NumeroFogo = PropertyEntity({ text: "Número de Fogo:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500 });

    this.Pesquisar = PropertyEntity({
        eventClick: function () { buscarReforma(); }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var Reforma = function () {
    this.Pneus = ko.observableArray();
    this.QuantidadeRegistros = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ExibirDados = PropertyEntity({
        eventClick: function (e) {
            e.ExibirDados.visibleFade(!e.ExibirDados.visibleFade());
        }, type: types.event, text: "", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadReforma() {
    _pesquisaReforma = new PesquisaReforma();
    KoBindings(_pesquisaReforma, "knockoutPesquisaReforma", false, _pesquisaReforma.Pesquisar.id);

    _reforma = new Reforma();
    KoBindings(_reforma, "knockoutReforma");

    new BuscarPneu(_pesquisaReforma.Pneu);
}

/*
 * Declaração das Funções Públicas
 */

function adicionarPneuReforma(pneuAdicionar) {
    if (isAdicionarPneuReforma(pneuAdicionar))
        atualizarReforma();
}

function adicionarPneuReformaPorIndice(indiceAdicionar) {
    var pneu = _reforma.Pneus()[indiceAdicionar];

    if (pneu)
        pneu.Removido.val(false);
}

function atualizarReforma() {
    buscarReforma(_paginaAtualReforma);
}

function buscarReforma(paginaSelecionada) {
    var dadosPesquisa = {
        Codigo: _pesquisaReforma.Pneu.codEntity(),
        NumeroFogo: _pesquisaReforma.NumeroFogo.val(),
        Inicio: _itensPorPaginaReforma * ((paginaSelecionada || 1) - 1),
        Limite: _itensPorPaginaReforma,
    }

    executarReST("MovimentacaoPneu/BuscarReformaPorCodigo", dadosPesquisa, function (retorno) {
        limparReforma();

        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaReforma.ExibirFiltros.visibleFade(false);

                preencherReforma(retorno.Data);

                _reforma.ExibirDados.visibleFade(true);

                salvarPesquisaReformaAnterior();

                if ((retorno.Data.Pneus.length == 0) && paginaSelecionada && (paginaSelecionada > 1))
                    buscarReforma(paginaSelecionada - 1);
                else
                    controlarPaginacaoReforma(paginaSelecionada);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function obterPneuReforma(codigoPneu) {
    for (var i = 0; i < _reforma.Pneus().length; i++) {
        var pneu = _reforma.Pneus()[i];

        if (pneu.CodigoPneu.val() == codigoPneu) {
            return pneu;
        }
    }

    return undefined;
}

function preencherReforma(reforma) {
    _reforma.QuantidadeRegistros.val(reforma.QuantidadeRegistros);

    for (var i = 0; i < reforma.Pneus.length; i++) {
        var pneu = reforma.Pneus[i];
        var reformaPneu = new Pneu();

        PreencherObjetoKnout(reformaPneu, { Data: pneu });

        _reforma.Pneus.push(reformaPneu);
        
        adicionarEventosDraggableEClickPneu("pneu-reforma-" + reformaPneu.CodigoPneu.val());
    }
}

function removerPneuReforma(codigoPneu) {
    for (var i = 0; i < _reforma.Pneus().length; i++) {
        var pneu = _reforma.Pneus()[i];

        if (pneu.CodigoPneu.val() == codigoPneu) {
            pneu.Removido.val(true);

            return i;
        }
    }

    return undefined;
}

/*
 * Declaração das Funções Privadas
 */

function controlarPaginacaoReforma(paginaAtual) {
    if (_reforma.QuantidadeRegistros.val() == 0) {
        $("#container-paginacao-reforma").html('');
        return;
    }

    $("#container-paginacao-reforma").html('<ul style="float:right" id="paginacao-reforma" class="pagination"></ul>');

    var paginas = Math.ceil((_reforma.QuantidadeRegistros.val() / _itensPorPaginaReforma));

    _paginaAtualReforma = (paginaAtual || 1);

    var a = $('#paginacao-reforma').twbsPagination({
        first: 'Primeiro',
        prev: 'Anterior',
        next: 'Próximo',
        last: 'Último',
        initiateStartPageClick: false,
        startPage: _paginaAtualReforma,
        totalPages: paginas,
        visiblePages: 5,
        onPageClick: function (event, paginaSelecionada) {
            if (paginaSelecionada != _paginaAtualReforma) {
                buscarReforma(paginaSelecionada);
                _paginaAtualReforma = paginaSelecionada;
            }
        }
    });
}

function isAdicionarPneuReforma(pneuAdicionar) {
    if (!_pesquisaReformaAnterior)
        return false;

    return (
        (_pesquisaReformaAnterior.Pneu.codEntity() == 0) &&
        (!_pesquisaReformaAnterior.NumeroFogo.val() || (pneuAdicionar.NumeroFogo.val().indexOf(_pesquisaReformaAnterior.NumeroFogo.val()) > -1))
    );
}

function limparReforma() {
    _reforma.Pneus.removeAll();
}

function salvarPesquisaReformaAnterior() {
    if (!_pesquisaReformaAnterior)
        _pesquisaReformaAnterior = new PesquisaReforma();

    _pesquisaReformaAnterior.Pneu.codEntity(_pesquisaReforma.Pneu.codEntity());
    _pesquisaReformaAnterior.Pneu.val(_pesquisaReforma.Pneu.val());
    _pesquisaReformaAnterior.NumeroFogo.val(_pesquisaReforma.NumeroFogo.val());
}
