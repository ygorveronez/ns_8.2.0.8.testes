/// <reference path="MovimentacaoPneu.js" />
/// <reference path="../../Consultas/Almoxarifado.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _estoque;
var _itensPorPaginaEstoque = 24; 
var _paginaAtualEstoque = 1;
var _pesquisaEstoque;
var _pesquisaEstoqueAnterior;

/*
 * Declaração das Classes
 */

var Estoque = function () {
    this.Pneus = ko.observableArray();
    this.QuantidadeRegistros = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    
    this.ExibirDados = PropertyEntity({
        eventClick: function (e) {
            e.ExibirDados.visibleFade(!e.ExibirDados.visibleFade());
        }, type: types.event, text: "", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var PesquisaEstoque = function () {
    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Almoxarifado:", idBtnSearch: guid() });
    this.NumeroFogo = PropertyEntity({ text: "Número de Fogo:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500 });

    this.Pesquisar = PropertyEntity({
        eventClick: function () { buscarEstoque(); }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadEstoque() {
    _pesquisaEstoque = new PesquisaEstoque();
    KoBindings(_pesquisaEstoque, "knockoutPesquisaEstoque", false, _pesquisaEstoque.Pesquisar.id);

    _estoque = new Estoque();
    KoBindings(_estoque, "knockoutEstoque");

    new BuscarAlmoxarifado(_pesquisaEstoque.Almoxarifado);
}

/*
 * Declaração das Funções Públicas
 */

function adicionarPneuEstoque(pneuAdicionar) {
    if (isAdicionarPneuEstoque(pneuAdicionar))
        atualizarEstoque();
}

function adicionarPneuEstoquePorIndice(indiceAdicionar) {
    var pneu = _estoque.Pneus()[indiceAdicionar];

    if (pneu)
        pneu.Removido.val(false);
}

function atualizarEstoque() {
    buscarEstoque(_paginaAtualEstoque);
}

function buscarEstoque(paginaSelecionada) {
    var dadosPesquisa = {
        Codigo: _pesquisaEstoque.Almoxarifado.codEntity(),
        NumeroFogo: _pesquisaEstoque.NumeroFogo.val(),
        Inicio: _itensPorPaginaEstoque * ((paginaSelecionada || 1) - 1),
        Limite: _itensPorPaginaEstoque,
    }

    executarReST("MovimentacaoPneu/BuscarEstoque", dadosPesquisa, function (retorno) {
        limparEstoque();

        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaEstoque.ExibirFiltros.visibleFade(false);

                preencherEstoque(retorno.Data);

                _estoque.ExibirDados.visibleFade(true);

                salvarPesquisaEstoqueAnterior();

                if ((retorno.Data.Pneus.length == 0) && paginaSelecionada && (paginaSelecionada > 1))
                    buscarEstoque(paginaSelecionada - 1);
                else
                    controlarPaginacaoEstoque(paginaSelecionada);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function obterPneuEstoque(codigoPneu) {
    for (var i = 0; i < _estoque.Pneus().length; i++) {
        var pneu = _estoque.Pneus()[i];

        if (pneu.CodigoPneu.val() == codigoPneu) {
            return pneu;
        }
    }

    return undefined;
}

function removerPneuEstoque(codigoPneu) {
    for (var i = 0; i < _estoque.Pneus().length; i++) {
        var pneu = _estoque.Pneus()[i];

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

function controlarPaginacaoEstoque(paginaAtual) {
    if (_estoque.QuantidadeRegistros.val() == 0) {
        $("#container-paginacao-estoque").html('');
        return;
    }
    _paginaAtualEstoque = (paginaAtual || 1);

    $("#container-paginacao-estoque").html('<ul style="float:right" id="paginacao-estoque" class="pagination"></ul>');
    $("#container-paginacao-estoque").append('<div class=\"col-xs-12 col-sm-6\" style=\"margin-top: 5px;\"><div class=\"dataTables_info pagination no-padding\" id=\"dt-info-pagination\" role=\"status\" aria-live=\"polite\"></div></div>');
    
    var paginas = Math.ceil((_estoque.QuantidadeRegistros.val() / _itensPorPaginaEstoque));
    
    var a = $('#paginacao-estoque').twbsPagination({
        first: 'Primeiro',
        prev: 'Anterior',
        next: 'Próximo',
        last: 'Último',
        initiateStartPageClick: false,
        startPage: _paginaAtualEstoque,
        totalPages: paginas,
        visiblePages: 5,
        onPageClick: function (event, paginaSelecionada) {
            if (paginaSelecionada != _paginaAtualEstoque) {
                buscarEstoque(paginaSelecionada);
                _paginaAtualEstoque = paginaSelecionada;
                configuracaoHtmlPaginacao(paginaSelecionada);
            }
        }
    });
    
    configuracaoHtmlPaginacao(_paginaAtualEstoque);
}

function isAdicionarPneuEstoque(pneuAdicionar) {
    if (!_pesquisaEstoqueAnterior)
        return false;

    return (
        ((_pesquisaEstoqueAnterior.Almoxarifado.codEntity() == 0) || (_pesquisaEstoqueAnterior.Almoxarifado.codEntity() == pneuAdicionar.Almoxarifado.codEntity())) &&
        (!_pesquisaEstoqueAnterior.NumeroFogo.val() || (pneuAdicionar.NumeroFogo.val().indexOf(_pesquisaEstoqueAnterior.NumeroFogo.val()) > -1))
    );
}

function configuracaoHtmlPaginacao(paginaSelecionada) {
    var numeroRegistrosDaPagina = (Number(paginaSelecionada) * _itensPorPaginaEstoque) > _estoque.QuantidadeRegistros.val() ? _estoque.QuantidadeRegistros.val() : (Number(paginaSelecionada) * _itensPorPaginaEstoque);
    var numeroRegistrosInicialDaPagina = (Number(paginaSelecionada) * _itensPorPaginaEstoque) - _itensPorPaginaEstoque + 1;
    
    $("#dt-info-pagination").html('Exibindo ' + numeroRegistrosInicialDaPagina + ' até ' + numeroRegistrosDaPagina + ' de ' + _estoque.QuantidadeRegistros.val() + ' registros');
}

function limparEstoque() {
    _estoque.Pneus.removeAll();
}

function preencherEstoque(estoque) {
    _estoque.QuantidadeRegistros.val(estoque.QuantidadeRegistros);

    for (var i = 0; i < estoque.Pneus.length; i++) {
        var pneu = estoque.Pneus[i];
        var estoquePneu = new Pneu();

        PreencherObjetoKnout(estoquePneu, { Data: pneu });

        _estoque.Pneus.push(estoquePneu);
        
        adicionarEventosDraggableEClickPneu("pneu-estoque-" + estoquePneu.CodigoPneu.val());
    }
}

function salvarPesquisaEstoqueAnterior() {
    if (!_pesquisaEstoqueAnterior)
        _pesquisaEstoqueAnterior = new PesquisaEstoque();

    _pesquisaEstoqueAnterior.Almoxarifado.codEntity(_pesquisaEstoque.Almoxarifado.codEntity());
    _pesquisaEstoqueAnterior.Almoxarifado.val(_pesquisaEstoque.Almoxarifado.val());
    _pesquisaEstoqueAnterior.NumeroFogo.val(_pesquisaEstoque.NumeroFogo.val());
}
