/// <reference path="LicitacaoParticipacaoAnexo.js" />
/// <reference path="LicitacaoParticipacaoCadastro.js" />
/// <reference path="LicitacaoParticipacaoEtapa.js" />
/// <reference path="LicitacaoParticipacaoOferta.js" />
/// <reference path="LicitacaoParticipacaoResumo.js" />
/// <reference path="LicitacaoParticipacaoRetornoOferta.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridLicitacao;
var _pesquisaLicitacao;

/*
 * Declaração das Classes
 */

var PesquisaLicitacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: ", maxlength: 200 });
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tabela de Frete: "), idBtnSearch: guid() });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: recarregarGridLicitacao, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadLicitacaoParticipacao() {
    _pesquisaLicitacao = new PesquisaLicitacao();
    KoBindings(_pesquisaLicitacao, "knockoutPesquisaLicitacao");

    new BuscarTabelasDeFrete(_pesquisaLicitacao.TabelaFrete);

    loadGridLicitacao();

    loadResumoLicitacaoParticipacaoCadastro();
    loadLicitacaoParticipacaoCadastro();
    loadAnexo();
    loadLicitacaoParticipacaoOferta();
    loadLicitacaoParticipacaoRetornoOferta();
    loadEtapaLicitacaoParticipacao();
}

function loadGridLicitacao() {
    var opcaoParticipar = { descricao: "Participar", id: guid(), evento: "onclick", metodo: exibirModalParticipacaoClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoParticipar] };
    var quantidadePorPagina = 25;
    var configuracaoExportacao = { url: "LicitacaoParticipacao/ExportarPesquisa", titulo: "Licitações Disponíveis para Participação" };

    _gridLicitacao = new GridViewExportacao(_pesquisaLicitacao.Pesquisar.idGrid, "LicitacaoParticipacao/Pesquisa", _pesquisaLicitacao, menuOpcoes, configuracaoExportacao, null, quantidadePorPagina);
    _gridLicitacao.CarregarGrid();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function exibirModalParticipacaoClick(registroSelecionado) {
    exibirLicitacaoParticipacaoCadastro(registroSelecionado);
}

/*
 * Declaração das Funções
 */

function recarregarGridLicitacao() {
    _gridLicitacao.CarregarGrid();
}
