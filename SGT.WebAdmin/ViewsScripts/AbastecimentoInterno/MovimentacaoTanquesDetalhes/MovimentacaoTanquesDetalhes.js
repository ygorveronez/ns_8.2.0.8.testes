
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _detalhesMovimentacoes;
var _pesquisaMovimentacaoTanquesDetalhes;
var _gridMovimentacaoTanques;
var _gridMovimentacaoTanquesDetalhes;

/*
 * Declaração das Classes
 */

var MovimentacaoTanquesDetalhada = function () {
    this.ExibirDetalhesMovimentacoes = PropertyEntity({ idGrid: guid(), visible: ko.observable(true), id: guid() });
    this.LocalArmazenamentoDetalhes = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local Armazenamento: ", idBtnSearch: guid(), visible: ko.observable(false) });
    this.DataInicialMovimentacaoDetalhes = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: ko.observable(false) });
    this.DataFinalMovimentacaoDetalhes = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: ko.observable(false) });

}

var PesquisaMovimentacaoTanquesDetalhes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local Armazenamento: ", idBtnSearch: guid() });
    this.DataInicialMovimentacao = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinalMovimentacao = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridMovimentacaoTanquesDetalhes, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridMovimentacaoTanquesDetalhes() {
    var opcaoExibirDetalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: ExibirDetalhesMovimentacoes, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoExibirDetalhes] };

    _gridMovimentacaoTanques = new GridViewExportacao(_pesquisaMovimentacaoTanquesDetalhes.Pesquisar.idGrid, "MovimentacaoTanquesDetalhes/Pesquisa", _pesquisaMovimentacaoTanquesDetalhes, menuOpcoes);
    _gridMovimentacaoTanques.CarregarGrid();
}

function LoadMovimentacaoTanquesDetalhes() {
    _pesquisaMovimentacaoTanquesDetalhes = new PesquisaMovimentacaoTanquesDetalhes();
    KoBindings(_pesquisaMovimentacaoTanquesDetalhes, "knockoutPesquisaMovimentacaoTanquesDetalhada", false, _pesquisaMovimentacaoTanquesDetalhes.Pesquisar.id);

    _detalhesMovimentacoes = new MovimentacaoTanquesDetalhada();
    KoBindings(_detalhesMovimentacoes, "knockoutDetalhesMovimentacoes", false, _detalhesMovimentacoes.ExibirDetalhesMovimentacoes.id);

    BuscarLocalArmazenamentoProduto(_pesquisaMovimentacaoTanquesDetalhes.LocalArmazenamento);

    LoadGridMovimentacaoTanquesDetalhes();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function ExibirDetalhesMovimentacoes(registroSelecionado) {
    _detalhesMovimentacoes = new MovimentacaoTanquesDetalhada();
    KoBindings(_detalhesMovimentacoes, "knockoutDetalhesMovimentacoes", false, _detalhesMovimentacoes.ExibirDetalhesMovimentacoes.id);

    PreencherDetalhesMovimentacoes(registroSelecionado);

    _gridMovimentacaoTanquesDetalhes = new GridViewExportacao(_detalhesMovimentacoes.ExibirDetalhesMovimentacoes.idGrid, "MovimentacaoTanquesDetalhes/PesquisaDetalhes", _detalhesMovimentacoes, null, null, null, 50);
    _gridMovimentacaoTanquesDetalhes.CarregarGrid();
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */
function LimparCamposMovimentoPesquisa() {
    LimparCampos(_pesquisaMovimentacaoTanquesDetalhes);
}

function RecarregarGridMovimentacaoTanquesDetalhes() {
    _gridMovimentacaoTanques.CarregarGrid();
}

function PreencherDetalhesMovimentacoes(registroSelecionado) {
    _detalhesMovimentacoes.LocalArmazenamentoDetalhes.codEntity(registroSelecionado.CodigoLocalArmazenamento);
    _detalhesMovimentacoes.LocalArmazenamentoDetalhes.val(registroSelecionado.LocalArmazenamento);
    _detalhesMovimentacoes.DataInicialMovimentacaoDetalhes.val(registroSelecionado.DataInicialMovimentacaoDetalhes);
    _detalhesMovimentacoes.DataFinalMovimentacaoDetalhes.val(registroSelecionado.DataFinalMovimentacaoDetalhes);
}